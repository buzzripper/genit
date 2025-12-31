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
        private const string RowVersionPropertyName = "RowVersion";

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
            if (IsRowVersionProperty() && !property.Name.Equals("Description", StringComparison.OrdinalIgnoreCase))
            {
                return TypeDescriptorHelper.CreateReadOnlyPropertyDescriptor(property);
            }

            // Make EnumTypeName read-only when property is tied to an EnumAssociation relationship
            if (property.Name.Equals("EnumTypeName", StringComparison.OrdinalIgnoreCase))
            {
                if (IsEnumPropertyTiedToRelationship())
                {
                    return TypeDescriptorHelper.CreateReadOnlyPropertyDescriptor(property);
                }
            }

            // Make DataType read-only when property is tied to an EnumAssociation relationship
            if (property.Name.Equals("DataType", StringComparison.OrdinalIgnoreCase))
            {
                if (IsEnumPropertyTiedToRelationship())
                {
                    return TypeDescriptorHelper.CreateReadOnlyPropertyDescriptor(property);
                }
            }

            return property;
        }

        private bool IsRowVersionProperty()
        {
            // Check if this is the RowVersion property
            if (_propertyModel.Name != RowVersionPropertyName)
                return false;

            // Check if it's a ByteArray type
            if (_propertyModel.DataType != DataType.ByteArray)
                return false;

            var entity = _propertyModel.EntityModel;
            if (entity == null)
                return false;

            // Check if the entity has InclRowVersion enabled
            return entity.InclRowVersion;
        }

        private bool IsEnumPropertyTiedToRelationship()
        {
            if (_propertyModel.DataType != DataType.Enum)
                return false;

            var entity = _propertyModel.EntityModel;
            if (entity == null)
                return false;

            // Check if there's an EnumAssociation link with this property name
            var links = EnumAssociation.GetLinksToUsedEnums(entity);
            return links.Any(l => l.PropertyName == _propertyModel.Name);
        }
    }
}
