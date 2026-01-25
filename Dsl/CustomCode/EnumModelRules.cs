using System.Linq;
using Microsoft.VisualStudio.Modeling;

namespace Dyvenix.GenIt
{
    /// <summary>
    /// Rule that fires when EnumModel.Name changes.
    /// Updates DataType on all PropertyModels that reference this enum by name.
    /// </summary>
    [RuleOn(typeof(EnumModel), FireTime = TimeToFire.TopLevelCommit)]
    public class EnumModelNameChangeRule : ChangeRule
    {
        public override void ElementPropertyChanged(ElementPropertyChangedEventArgs e)
        {
            // Only handle Name property changes
            if (e.DomainProperty.Id != NamedElement.NameDomainPropertyId)
                return;

            var enumModel = e.ModelElement as EnumModel;
            if (enumModel == null || enumModel.IsDeleting || enumModel.IsDeleted)
                return;

            string oldName = e.OldValue as string;
            string newName = e.NewValue as string;

            if (string.IsNullOrEmpty(oldName) || string.IsNullOrEmpty(newName) || oldName == newName)
                return;

            // Update any PropertyModel that references this enum by DataType name
            var store = enumModel.Store;
            var allProperties = store.ElementDirectory.FindElements<PropertyModel>();

            foreach (var prop in allProperties)
            {
                if (prop.IsDeleting || prop.IsDeleted)
                    continue;

                // If DataType equals the old enum name, update it to the new name
                if (prop.DataType == oldName)
                {
                    prop.DataType = newName;
                }
            }
        }
    }
}
