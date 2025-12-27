using System;
using Microsoft.VisualStudio.Modeling;

namespace Dyvenix.GenIt
{
    /// <summary>
    /// Partial class to register custom domain model types (rules) with the DSL.
    /// </summary>
    public partial class GenItDomainModel
    {
        /// <summary>
        /// Gets the list of custom domain model types (rules, etc.) to be registered.
        /// </summary>
        /// <returns>List of custom types.</returns>
        protected override Type[] GetCustomDomainModelTypes()
        {
            return new Type[]
            {
                typeof(EntityUsesEnumAddRule),
                typeof(EntityUsesEnumDeleteRule),
                typeof(PropertyModelDeleteRule),
                typeof(EntityUsesEnumPropertyNameChangeRule),
            };
        }
    }
}
