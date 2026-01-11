using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Design;

namespace Dyvenix.GenIt
{
    /// <summary>
    /// Custom TypeDescriptionProvider for PropertyModel that controls property visibility
    /// based on the selected DataType.
    /// </summary>
    public partial class PropertyModelTypeDescriptionProvider : ElementTypeDescriptionProvider
    {
        /// <summary>
        /// Creates the type descriptor for PropertyModel instances.
        /// </summary>
        protected override ElementTypeDescriptor CreateTypeDescriptor(ICustomTypeDescriptor parent, ModelElement element)
        {
            if (element is PropertyModel propertyModel)
            {
                return new PropertyModelTypeDescriptor(parent, propertyModel);
            }
            return base.CreateTypeDescriptor(parent, element);
        }
    }

    /// <summary>
    /// Custom TypeDescriptor that filters properties based on DataType value.
    /// </summary>
    public class PropertyModelTypeDescriptor : ElementTypeDescriptor
    {
        private readonly PropertyModel _propertyModel;

        public PropertyModelTypeDescriptor(ICustomTypeDescriptor parent, PropertyModel element)
            : base(parent, element)
        {
            _propertyModel = element;
        }

        /// <summary>
        /// Filters the properties collection to show/hide properties based on DataType
        /// and makes certain properties read-only.
        /// </summary>
        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            PropertyDescriptorCollection properties = base.GetProperties(attributes);

            var modifiedProperties = properties.Cast<PropertyDescriptor>()
                .Where(p => ShouldShowProperty(p))
                .Select(p => ModifyProperty(p))
                .ToArray();

            return new PropertyDescriptorCollection(modifiedProperties);
        }

        private bool ShouldShowProperty(PropertyDescriptor property)
        {
            // Only show Length property when DataType is String or ByteArray
            if (property.Name.Equals("Length", StringComparison.OrdinalIgnoreCase))
            {
                return _propertyModel.DataType == DataType.String || _propertyModel.DataType == DataType.ByteArray;
            }

            // Only show EnumTypeName when DataType is Enum
            if (property.Name.Equals("EnumTypeName", StringComparison.OrdinalIgnoreCase))
            {
                return _propertyModel.DataType == DataType.Enum;
            }

            return true;
        }

        /// <summary>
        /// Checks if the property is associated with an EnumAssociation.
        /// </summary>
        private bool IsEnumAssociationProperty()
        {
            if (_propertyModel.DataType != DataType.Enum)
                return false;

            var entity = _propertyModel.EntityModel;
            if (entity == null)
                return false;

            // Check if there's an EnumAssociation with a matching PropertyName
            var enumAssociations = EnumAssociation.GetLinksToUsedEnums(entity);
            return enumAssociations.Any(assoc => 
                !assoc.IsDeleting && 
                !assoc.IsDeleted && 
                assoc.PropertyName == _propertyModel.Name);
        }

        private PropertyDescriptor ModifyProperty(PropertyDescriptor property)
        {
            // Add multiline editor to Attributes property
            if (property.Name.Equals("Attributes", StringComparison.OrdinalIgnoreCase))
            {
                return TypeDescriptorHelper.CreateMultilineStringPropertyDescriptor(property);
            }

            // Add multiline editor to Usings property
            if (property.Name.Equals("Usings", StringComparison.OrdinalIgnoreCase))
            {
                return TypeDescriptorHelper.CreateMultilineStringPropertyDescriptor(property);
            }

            // Make all properties read-only for RowVersion property (except Description)
            if (_propertyModel.IsRowVersion && !property.Name.Equals("Description", StringComparison.OrdinalIgnoreCase))
            {
                return TypeDescriptorHelper.CreateReadOnlyPropertyDescriptor(property);
            }

            // Make IsNullable read-only when IsPrimaryKey or IsForeignKey is true
            if (property.Name.Equals("IsNullable", StringComparison.OrdinalIgnoreCase))
            {
                if (_propertyModel.IsPrimaryKey || _propertyModel.IsForeignKey)
                {
                    return TypeDescriptorHelper.CreateReadOnlyPropertyDescriptor(property);
                }
            }

            // Make DataType and EnumTypeName read-only when associated with an EnumAssociation
            if (property.Name.Equals("DataType", StringComparison.OrdinalIgnoreCase) ||
                property.Name.Equals("EnumTypeName", StringComparison.OrdinalIgnoreCase))
            {
                if (IsEnumAssociationProperty())
                {
                    return TypeDescriptorHelper.CreateReadOnlyPropertyDescriptor(property);
                }
            }

            // Add dropdown with enum names for EnumTypeName property (only if not read-only)
            if (property.Name.Equals("EnumTypeName", StringComparison.OrdinalIgnoreCase))
            {
                // Add the custom enum dropdown editor
                return TypeDescriptorHelper.CreateEnumTypeNamePropertyDescriptor(property);
            }

            return property;
        }
    }
}
