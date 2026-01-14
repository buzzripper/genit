using System;
using System.ComponentModel;
using System.Drawing.Design;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Design;

namespace Dyvenix.GenIt
{
    /// <summary>
    /// Custom TypeDescriptionProvider for EntityModel that adds multiline editor support.
    /// </summary>
    public class EntityModelTypeDescriptionProvider : ElementTypeDescriptionProvider
    {
        /// <summary>
        /// Creates the type descriptor for EntityModel instances.
        /// </summary>
        protected override ElementTypeDescriptor CreateTypeDescriptor(ICustomTypeDescriptor parent, ModelElement element)
        {
            if (element is EntityModel entityModel)
            {
                return new EntityModelTypeDescriptor(parent, entityModel);
            }
            return base.CreateTypeDescriptor(parent, element);
        }
    }

    /// <summary>
    /// Custom TypeDescriptor that adds multiline editor to the Attributes property
    /// and module dropdown editor to the Module property.
    /// </summary>
    public class EntityModelTypeDescriptor : ElementTypeDescriptor
    {
        public EntityModelTypeDescriptor(ICustomTypeDescriptor parent, EntityModel element)
            : base(parent, element)
        {
        }

        /// <summary>
        /// Filters the properties collection to add multiline editor to Attributes and Usings,
        /// and module dropdown editor to Module.
        /// </summary>
        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            PropertyDescriptorCollection properties = base.GetProperties(attributes);
            PropertyDescriptorCollection newProperties = new PropertyDescriptorCollection(null);

            foreach (PropertyDescriptor prop in properties)
            {
                if (prop.Name == "Attributes" || prop.Name == "Usings")
                {
                    // Add multiline string editor to Attributes and Usings properties
                    newProperties.Add(TypeDescriptorHelper.CreateMultilineStringPropertyDescriptor(prop));
                }
                else if (prop.Name == "Module")
                {
                    // Add module dropdown editor to Module property (dropdown-only, no manual text editing)
                    newProperties.Add(TypeDescriptorHelper.CreateDropdownOnlyModuleNamePropertyDescriptor(prop));
                }
                else
                {
                    newProperties.Add(prop);
                }
            }

            return newProperties;
        }
    }
}
