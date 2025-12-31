using System;

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
                // EntityModel add rule
                typeof(EntityModelAddRule),
                
                // EntityModel RowVersion rule
                typeof(EntityModelRowVersionChangeRule),
                
                // PropertyModel add rule
                typeof(PropertyModelAddRule),
                
                // RowVersion property delete rule
                typeof(RowVersionPropertyDeleteRule),
                
                // Association navigation property rules
                typeof(AssociationAddRule),
				typeof(AssociationDeleteRule),
				typeof(AssociationPropertyChangeRule),
				typeof(NavigationPropertyDeleteRule),
				typeof(NavigationPropertyNameChangeRule),
                
                // Association FK property rule
                typeof(FkPropertyDeleteRule),
			};
		}
	}
}
