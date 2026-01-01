using System;
using System.ComponentModel;
using System.Linq;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Design;

namespace Dyvenix.GenIt
{
    /// <summary>
    /// Custom TypeDescriptionProvider for NavigationProperty that makes certain properties read-only.
    /// </summary>
    public partial class NavigationPropertyTypeDescriptionProvider : ElementTypeDescriptionProvider
    {
        /// <summary>
        /// Creates the type descriptor for NavigationProperty instances.
        /// </summary>
        protected override ElementTypeDescriptor CreateTypeDescriptor(ICustomTypeDescriptor parent, ModelElement element)
        {
            if (element is NavigationProperty navigationProperty)
            {
                return new NavigationPropertyTypeDescriptor(parent, navigationProperty);
            }
            return base.CreateTypeDescriptor(parent, element);
        }
    }

    /// <summary>
    /// Custom TypeDescriptor that makes TargetEntityName and IsCollection read-only.
    /// These properties are managed by the Association and should not be edited directly.
    /// </summary>
    public class NavigationPropertyTypeDescriptor : ElementTypeDescriptor
    {
        private readonly NavigationProperty _navigationProperty;

        public NavigationPropertyTypeDescriptor(ICustomTypeDescriptor parent, NavigationProperty element)
            : base(parent, element)
        {
            _navigationProperty = element;
        }

        /// <summary>
        /// Modifies the properties collection to make certain properties read-only.
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
            // Make TargetEntityName read-only - it's managed by the Association
            if (property.Name.Equals("TargetEntityName", StringComparison.OrdinalIgnoreCase))
            {
                return TypeDescriptorHelper.CreateReadOnlyPropertyDescriptor(property);
            }

            // Make IsCollection read-only - it's synced from the Association multiplicity
            if (property.Name.Equals("IsCollection", StringComparison.OrdinalIgnoreCase))
            {
                return TypeDescriptorHelper.CreateReadOnlyPropertyDescriptor(property);
            }

            return property;
        }
    }
}
