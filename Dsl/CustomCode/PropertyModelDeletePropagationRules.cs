using Microsoft.VisualStudio.Modeling;
using System.Collections.Generic;
using System.Linq;

namespace Dyvenix.GenIt
{
	/// <summary>
	/// When a PropertyModel is deleted, the DSL only tears down the relationship links
	/// (UpdatePropertyModelHasPropertyModel / FilterPropertyModelHasProperty /
	/// DtoModelReferencesPropertyModels) because those are reference relationships. The
	/// dependent elements that own the reference (UpdatePropertyModel / FilterPropertyModel)
	/// are left behind with a null PropertyModel, which corrupts the .gmdl model (the diagram
	/// never shows them) and later causes NullReferenceExceptions during code generation.
	/// DtoModel elements keep a stale reference link that leaves the property lingering on the
	/// DTO in both the model and the diagram compartments.
	///
	/// This rule propagates the delete by removing every dependent that relates to the
	/// property through a model link:
	///   * UpdatePropertyModel entries on any UpdateMethodModel,
	///   * FilterPropertyModel entries on any ReadMethodModel,
	///   * DtoModel property references (the link is removed so the DTO drops the property).
	///
	/// Dependents are gathered by navigating the property's own relationship collections,
	/// which are still intact while ElementDeleting fires, rather than relying on
	/// name-matching or a full-store scan.
	/// </summary>
	[RuleOn(typeof(PropertyModel), FireTime = TimeToFire.LocalCommit)]
	public class PropertyModelDeletePropagationRule : DeletingRule
	{
		public override void ElementDeleting(ElementDeletingEventArgs e)
		{
			if (!(e?.ModelElement is PropertyModel property))
				return;

			var store = property.Store;
			if (store == null || store.InSerializationTransaction)
				return;

			// Snapshot first, then delete, to avoid mutating the collections while enumerating.

			// Update method properties that reference this property.
			var updateProperties = property.UpdatePropertyModels
				.Where(up => !up.IsDeleting && !up.IsDeleted)
				.ToList();

			// Read method filter properties that reference this property.
			var filterProperties = property.FilterPropertyModels
				.Where(fp => !fp.IsDeleting && !fp.IsDeleted)
				.ToList();

			// Dto reference links that expose this property on a DTO. Remove the link (not the
			// DTO) so the property is dropped from the DTO in the model and the diagram.
			var dtoLinks = new List<DtoModelReferencesPropertyModels>();
			foreach (var dto in property.DtoModeled.ToList())
			{
				foreach (var link in DtoModelReferencesPropertyModels.GetLinks(dto, property))
				{
					if (link != null && !link.IsDeleting && !link.IsDeleted)
						dtoLinks.Add(link);
				}
			}

			foreach (var updateProperty in updateProperties)
				updateProperty.Delete();

			foreach (var filterProperty in filterProperties)
				filterProperty.Delete();

			foreach (var dtoLink in dtoLinks)
				dtoLink.Delete();
		}
	}
}
