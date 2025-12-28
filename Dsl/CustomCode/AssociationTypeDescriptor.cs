using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Design;

namespace Dyvenix.GenIt
{
    /// <summary>
    /// Custom TypeDescriptionProvider for Association that organizes properties into categories.
    /// </summary>
    public partial class AssociationTypeDescriptionProvider : ElementTypeDescriptionProvider
    {
        /// <summary>
        /// Creates the type descriptor for Association instances.
        /// </summary>
        protected override ElementTypeDescriptor CreateTypeDescriptor(ICustomTypeDescriptor parent, ModelElement element)
        {
            if (element is Association association)
            {
                return new AssociationTypeDescriptor(parent, association);
            }
            return base.CreateTypeDescriptor(parent, element);
        }
    }

    /// <summary>
    /// Custom TypeDescriptor for Association - placeholder for future customization.
    /// </summary>
    public class AssociationTypeDescriptor : ElementTypeDescriptor
    {
        public AssociationTypeDescriptor(ICustomTypeDescriptor parent, Association element)
            : base(parent, element)
        {
        }
    }
}
