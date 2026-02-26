using System;
using System.Linq;
using Microsoft.VisualStudio.Modeling;

namespace Dyvenix.GenIt
{
	[RuleOn(typeof(PropertyModel), FireTime = TimeToFire.TopLevelCommit)]
	public class PropertyModelNameChangeRule : ChangeRule
	{
		public override void ElementPropertyChanged(ElementPropertyChangedEventArgs e)
		{
			if (e == null)
				return;

			if (e.DomainProperty == null || e.DomainProperty.Name != nameof(PropertyModel.Name))
				return;

			var property = e.ModelElement as PropertyModel;
			if (property == null || property.IsDeleting || property.IsDeleted)
				return;

			if (property.Store != null && property.Store.InSerializationTransaction)
				return;

			var newName = property.Name;
			var oldName = e.OldValue as string;
			if (string.IsNullOrWhiteSpace(newName) || string.Equals(oldName, newName, StringComparison.Ordinal))
				return;

			using (var tx = property.Store.TransactionManager.BeginTransaction("Propagate property rename"))
			{
				tx.Commit();
			}
		}
	}
}
