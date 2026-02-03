using Microsoft.VisualStudio.Modeling;

namespace Dyvenix.GenIt
{
	/// <summary>
	/// Rule that fires when ReadMethodModel.InclPaging or InclSorting changes.
	/// Ensures UseRequest is set to true when either paging or sorting is enabled.
	/// </summary>
	[RuleOn(typeof(ReadMethodModel), FireTime = TimeToFire.TopLevelCommit)]
	public class ReadMethodModelChangeRule : ChangeRule
	{
		public override void ElementPropertyChanged(ElementPropertyChangedEventArgs e)
		{
			var readMethod = e.ModelElement as ReadMethodModel;
			if (readMethod == null || readMethod.IsDeleting || readMethod.IsDeleted)
				return;

			// Handle InclPaging change
			if (e.DomainProperty.Id == ReadMethodModel.InclPagingDomainPropertyId)
			{
				bool newValue = (bool)e.NewValue;
				if (newValue && !readMethod.UseRequest)
				{
					readMethod.UseRequest = true;
				}
			}
			// Handle InclSorting change
			else if (e.DomainProperty.Id == ReadMethodModel.InclSortingDomainPropertyId)
			{
				bool newValue = (bool)e.NewValue;
				if (newValue && !readMethod.UseRequest)
				{
					readMethod.UseRequest = true;
				}
			}
		}
	}
}
