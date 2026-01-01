using Microsoft.VisualStudio.Modeling;
using System.Linq;

namespace Dyvenix.GenIt
{
	/// <summary>
	/// Rule that handles PropertyModel.IsPrimaryKey changes.
	/// When IsPrimaryKey = true, sets IsNullable = false.
	/// </summary>
	[RuleOn(typeof(PropertyModel), FireTime = TimeToFire.TopLevelCommit)]
	public class PropertyModelIsPrimaryKeyChangeRule : ChangeRule
	{
		public override void ElementPropertyChanged(ElementPropertyChangedEventArgs e)
		{
			if (e.ModelElement.Store.InUndoRedoOrRollback)
				return;

			PropertyModel property = e.ModelElement as PropertyModel;
			if (property == null)
				return;

			// Handle IsPrimaryKey changes
			if (e.DomainProperty.Id == PropertyModel.IsPrimaryKeyDomainPropertyId)
			{
				bool isPrimaryKey = (bool)e.NewValue;
				if (isPrimaryKey)
				{
					// When IsPrimaryKey = true, set IsNullable = false
					property.IsNullable = false;
				}
			}
		}
	}

	/// <summary>
	/// Rule that handles PropertyModel.IsForeignKey changes.
	/// When IsForeignKey = true, syncs IsNullable based on the association's SourceMultiplicity.
	/// </summary>
	[RuleOn(typeof(PropertyModel), FireTime = TimeToFire.TopLevelCommit)]
	public class PropertyModelIsForeignKeyChangeRule : ChangeRule
	{
		public override void ElementPropertyChanged(ElementPropertyChangedEventArgs e)
		{
			if (e.ModelElement.Store.InUndoRedoOrRollback)
				return;

			PropertyModel property = e.ModelElement as PropertyModel;
			if (property == null)
				return;

			// Handle IsForeignKey changes
			if (e.DomainProperty.Id == PropertyModel.IsForeignKeyDomainPropertyId)
			{
				bool isForeignKey = (bool)e.NewValue;
				if (isForeignKey)
				{
					// Sync IsNullable based on the association's source multiplicity
					SyncForeignKeyNullability(property);
				}
			}
		}

		/// <summary>
		/// Syncs the IsNullable value of a FK property based on the association's source multiplicity.
		/// If SourceMultiplicity is 'One', IsNullable = false; otherwise true.
		/// </summary>
		internal static void SyncForeignKeyNullability(PropertyModel property)
		{
			EntityModel entity = property.EntityModel;
			if (entity == null)
				return;

			// Find the association where this property is the FK
			// The FK property is on the target entity, so we look for associations where this entity is the target
			var associations = Association.GetLinksToSources(entity);
			var matchingAssociation = associations.FirstOrDefault(a => a.FkPropertyName == property.Name);

			if (matchingAssociation != null)
			{
				// If SourceMultiplicity is 'One', the FK is required (not nullable)
				// Otherwise (ZeroOne or Many), the FK is optional (nullable)
				property.IsNullable = matchingAssociation.SourceMultiplicity != Multiplicity.One;
			}
		}
	}

	/// <summary>
	/// Rule that handles Association.SourceMultiplicity changes to sync FK property nullability.
	/// </summary>
	[RuleOn(typeof(Association), FireTime = TimeToFire.TopLevelCommit)]
	public class AssociationSourceMultiplicityChangeRule : ChangeRule
	{
		public override void ElementPropertyChanged(ElementPropertyChangedEventArgs e)
		{
			if (e.ModelElement.Store.InUndoRedoOrRollback)
				return;

			Association association = e.ModelElement as Association;
			if (association == null)
				return;

			// Handle SourceMultiplicity changes
			if (e.DomainProperty.Id == Association.SourceMultiplicityDomainPropertyId)
			{
				SyncForeignKeyNullability(association);
			}
		}

		private void SyncForeignKeyNullability(Association association)
		{
			if (string.IsNullOrEmpty(association.FkPropertyName))
				return;

			EntityModel targetEntity = association.Target;
			if (targetEntity == null)
				return;

			// Find the FK property on the target entity
			var fkProperty = targetEntity.Properties.FirstOrDefault(p => p.Name == association.FkPropertyName && p.IsForeignKey);
			if (fkProperty != null)
			{
				// If SourceMultiplicity is 'One', the FK is required (not nullable)
				fkProperty.IsNullable = association.SourceMultiplicity != Multiplicity.One;
			}
		}
	}
}
