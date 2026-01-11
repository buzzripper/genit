using System;
using System.ComponentModel;
using System.Linq;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Design;

namespace Dyvenix.GenIt
{
    /// <summary>
    /// Custom TypeDescriptionProvider for ServiceModel that makes Version read-only.
    /// </summary>
    public class ServiceModelTypeDescriptionProvider : ElementTypeDescriptionProvider
    {
        /// <summary>
        /// Creates the type descriptor for ServiceModel instances.
        /// </summary>
        protected override ElementTypeDescriptor CreateTypeDescriptor(ICustomTypeDescriptor parent, ModelElement element)
        {
            if (element is ServiceModel serviceModel)
            {
                return new ServiceModelTypeDescriptor(parent, serviceModel);
            }
            return base.CreateTypeDescriptor(parent, element);
        }
    }

    /// <summary>
    /// Custom TypeDescriptor that makes the Version property read-only.
    /// </summary>
    public class ServiceModelTypeDescriptor : ElementTypeDescriptor
    {
        private readonly ServiceModel _serviceModel;

        public ServiceModelTypeDescriptor(ICustomTypeDescriptor parent, ServiceModel element)
            : base(parent, element)
        {
            _serviceModel = element;
        }

        /// <summary>
        /// Filters the properties collection to make Version read-only.
        /// </summary>
        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            PropertyDescriptorCollection properties = base.GetProperties(attributes);

            var modifiedProperties = properties.Cast<PropertyDescriptor>()
                .Select(p => ModifyProperty(p))
                .ToArray();

            return new PropertyDescriptorCollection(modifiedProperties);
        }

        private PropertyDescriptor ModifyProperty(PropertyDescriptor property)
        {
            // Make Version property read-only
            if (property.Name.Equals("Version", StringComparison.OrdinalIgnoreCase))
            {
                return TypeDescriptorHelper.CreateReadOnlyPropertyDescriptor(property);
            }

            return property;
        }
    }
}
