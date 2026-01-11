using System;
using System.Linq;
using Microsoft.VisualStudio.Modeling;

namespace Dyvenix.GenIt
{
    /// <summary>
    /// Rule that fires when ModuleModel.Name changes.
    /// Updates EntityModel.Module on all entities that reference this module.
    /// </summary>
    [RuleOn(typeof(ModuleModel), FireTime = TimeToFire.TopLevelCommit)]
    public class ModuleModelNameChangeRule : ChangeRule
    {
        public override void ElementPropertyChanged(ElementPropertyChangedEventArgs e)
        {
            // Only handle Name property changes
            if (e.DomainProperty.Id != NamedElement.NameDomainPropertyId)
                return;

            var moduleModel = e.ModelElement as ModuleModel;
            if (moduleModel == null || moduleModel.IsDeleting || moduleModel.IsDeleted)
                return;

            string oldName = e.OldValue as string;
            string newName = e.NewValue as string;

            if (string.IsNullOrEmpty(oldName) || oldName == newName)
                return;

            // Find all EntityModels that reference this module and update their Module property
            var store = moduleModel.Store;
            var allEntities = store.ElementDirectory.FindElements<EntityModel>();

            foreach (var entity in allEntities)
            {
                if (entity.IsDeleting || entity.IsDeleted)
                    continue;

                if (entity.Module == oldName)
                {
                    entity.Module = newName ?? string.Empty;
                }
            }
        }
    }

    /// <summary>
    /// Rule that fires when a ModuleModel is being deleted.
    /// Clears the Module property on all entities that reference this module.
    /// </summary>
    [RuleOn(typeof(ModuleModel), FireTime = TimeToFire.TopLevelCommit)]
    public class ModuleModelDeleteRule : DeletingRule
    {
        public override void ElementDeleting(ElementDeletingEventArgs e)
        {
            var moduleModel = e.ModelElement as ModuleModel;
            if (moduleModel == null)
                return;

            string moduleName = moduleModel.Name;
            if (string.IsNullOrEmpty(moduleName))
                return;

            // Find all EntityModels that reference this module and clear their Module property
            var store = moduleModel.Store;
            var allEntities = store.ElementDirectory.FindElements<EntityModel>();

            foreach (var entity in allEntities)
            {
                if (entity.IsDeleting || entity.IsDeleted)
                    continue;

                if (entity.Module == moduleName)
                {
                    entity.Module = string.Empty;
                }
            }
        }
    }
}
