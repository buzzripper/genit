using Microsoft.VisualStudio.Modeling;

namespace Dyvenix.GenIt
{
	/// <summary>
	/// Re-sorts the Properties compartment on the entity diagram immediately when a
	/// property's DisplayOrder changes (e.g. after reordering rows in the editor grid),
	/// so the diagram reflects the new order without having to close/re-open it.
	/// </summary>
	[RuleOn(typeof(PropertyModel), FireTime = TimeToFire.TopLevelCommit)]
	public class PropertyModelDisplayOrderChangeRule : ChangeRule
	{
		public override void ElementPropertyChanged(ElementPropertyChangedEventArgs e)
		{
			if (e == null || e.DomainProperty == null)
				return;

			if (e.DomainProperty.Id != PropertyModel.DisplayOrderDomainPropertyId)
				return;

			var property = e.ModelElement as PropertyModel;
			if (property == null || property.IsDeleting || property.IsDeleted)
				return;

			if (property.Store != null && property.Store.InSerializationTransaction)
				return;

			var elements = CompartmentItemAddRule.GetEntityModelForClassShapePropertiesCompartment(property);

			// repaintOnly: false forces the compartment to repopulate using the
			// DisplayOrder-sorted getter, re-ordering the items on the diagram.
			bool repaintOnly = property.Store != null && property.Store.InUndoRedoOrRollback;
			CompartmentItemAddRule.UpdateCompartments(elements, typeof(ClassShape), "PropertiesCompartment", repaintOnly);
		}
	}
}
