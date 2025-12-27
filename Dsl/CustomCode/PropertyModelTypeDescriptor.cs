using System;
using System.Collections.Generic;
using System.ComponentModel;
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
                .Select(p => MakeReadOnlyIfNeeded(p))
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

        private PropertyDescriptor MakeReadOnlyIfNeeded(PropertyDescriptor property)
        {
            // Make EnumTypeName read-only when property is tied to an EntityUsesEnum relationship
            if (property.Name.Equals("EnumTypeName", StringComparison.OrdinalIgnoreCase))
            {
                if (IsEnumPropertyTiedToRelationship())
                {
                    return new ReadOnlyPropertyDescriptor(property);
                }
            }

            // Make DataType read-only when property is tied to an EntityUsesEnum relationship
            if (property.Name.Equals("DataType", StringComparison.OrdinalIgnoreCase))
            {
                if (IsEnumPropertyTiedToRelationship())
                {
                    return new ReadOnlyPropertyDescriptor(property);
                }
            }

            return property;
        }

        private bool IsEnumPropertyTiedToRelationship()
        {
            if (_propertyModel.DataType != DataType.Enum)
                return false;

            var entity = _propertyModel.EntityModel;
            if (entity == null)
                return false;

            // Check if there's an EntityUsesEnum link with this property name
            var links = EntityUsesEnum.GetLinksToUsedEnums(entity);
            return links.Any(l => l.PropertyName == _propertyModel.Name);
        }
    }

    /// <summary>
    /// A property descriptor wrapper that makes the property read-only.
    /// </summary>
    public class ReadOnlyPropertyDescriptor : PropertyDescriptor
    {
        private readonly PropertyDescriptor _baseDescriptor;

        public ReadOnlyPropertyDescriptor(PropertyDescriptor baseDescriptor)
            : base(baseDescriptor)
        {
            _baseDescriptor = baseDescriptor;
        }

        public override Type ComponentType => _baseDescriptor.ComponentType;
        public override bool IsReadOnly => true;
        public override Type PropertyType => _baseDescriptor.PropertyType;
        public override string Category => _baseDescriptor.Category;
        public override string Description => _baseDescriptor.Description;
        public override string DisplayName => _baseDescriptor.DisplayName;

        public override bool CanResetValue(object component) => false;
        public override object GetValue(object component) => _baseDescriptor.GetValue(component);
        public override void ResetValue(object component) { }
        public override void SetValue(object component, object value) { } // No-op for read-only
        public override bool ShouldSerializeValue(object component) => _baseDescriptor.ShouldSerializeValue(component);

        public override object GetEditor(Type editorBaseType) => _baseDescriptor.GetEditor(editorBaseType);
        public override TypeConverter Converter => _baseDescriptor.Converter;
    }
}
