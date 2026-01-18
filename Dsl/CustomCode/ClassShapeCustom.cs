using DslDiagrams = Microsoft.VisualStudio.Modeling.Diagrams;
using DslModeling = Microsoft.VisualStudio.Modeling;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dyvenix.GenIt
{
	partial class ClassShape
	{
		private static bool _mappingsModified;

		/// <summary>
		/// Override GetCompartmentMappings to provide sorted properties.
		/// </summary>
		protected override DslDiagrams::CompartmentMapping[] GetCompartmentMappings(Type melType)
		{
			// Call base to get/initialize mappings
			var baseMappings = base.GetCompartmentMappings(melType);

			// Modify the mappings once to use sorted getter
			if (!_mappingsModified && melType == typeof(EntityModel))
			{
				_mappingsModified = true;

				// Use reflection to replace the PropertiesCompartment mapping
				var field = typeof(ClassShapeBase).GetField("compartmentMappings", BindingFlags.NonPublic | BindingFlags.Static);
				if (field != null)
				{
					var mappingsDict = field.GetValue(null) as Dictionary<Type, DslDiagrams::CompartmentMapping[]>;
					if (mappingsDict != null && mappingsDict.ContainsKey(typeof(EntityModel)))
					{
						var mappings = mappingsDict[typeof(EntityModel)];
						for (int i = 0; i < mappings.Length; i++)
						{
							if (mappings[i] is DslDiagrams::ElementListCompartmentMapping mapping &&
								mapping.CompartmentId == "PropertiesCompartment")
							{
								mappings[i] = new DslDiagrams::ElementListCompartmentMapping(
									"PropertiesCompartment",
									NamedElement.NameDomainPropertyId,
									PropertyModel.DomainClassId,
									GetPropertiesSortedByDisplayOrder,
									null,
									null,
									null);
								break;
							}
						}
					}
				}
			}

			return baseMappings;
		}

		/// <summary>
		/// Gets the elements for the PropertiesCompartment, sorted by DisplayOrder.
		/// </summary>
		internal static IList GetPropertiesSortedByDisplayOrder(DslModeling::ModelElement rootElement)
		{
			EntityModel root = (EntityModel)rootElement;
			return root.Properties.OrderBy(p => p.DisplayOrder).ToList();
		}
	}
}

