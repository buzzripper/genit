using System;
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
        /// Filters the properties collection to show/hide Length based on DataType.
        /// </summary>
        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            PropertyDescriptorCollection properties = base.GetProperties(attributes);

            // Only show Length property when DataType is String
            if (_propertyModel.DataType != DataType.String)
            {
                // Filter out the Length property
                var filteredProperties = properties.Cast<PropertyDescriptor>()
                    .Where(p => !p.Name.Equals("Length", StringComparison.OrdinalIgnoreCase))
                    .ToArray();
                return new PropertyDescriptorCollection(filteredProperties);
            }

            return properties;
        }
    }
}
