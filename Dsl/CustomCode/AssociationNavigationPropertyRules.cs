using System;
using System.Linq;
using Microsoft.VisualStudio.Modeling;

namespace Dyvenix.GenIt
{
    /// <summary>
    /// Rule that fires when an Association relationship is created.
    /// Creates NavigationProperties based on GenSourceNavProperty and GenTargetNavProperty settings.
    /// Also creates an FK property on the target entity.
    /// </summary>
    [RuleOn(typeof(Association), FireTime = TimeToFire.TopLevelCommit)]
    public class AssociationAddRule : AddRule
    {
        public override void ElementAdded(ElementAddedEventArgs e)
        {
            var association = e.ModelElement as Association;
            if (association == null || association.IsDeleting || association.IsDeleted)
                return;

            var source = association.Source;
            var target = association.Target;

            if (source == null || target == null)
                return;

            // Create source navigation property if enabled (navigates from source to target)
            if (association.GenSourceNavProperty)
            {
                CreateSourceNavigationProperty(association, source, target);
            }

            // Create target navigation property if enabled (navigates from target to source)
            if (association.GenTargetNavProperty)
            {
                CreateTargetNavigationProperty(association, target, source);
            }

            // Create FK property on target entity
            CreateFkProperty(association, target, source);
        }

        private void CreateSourceNavigationProperty(Association association, EntityModel source, EntityModel target)
        {
            // Name from SourceRoleName, or default to target entity name
            string navPropName = !string.IsNullOrEmpty(association.SourceRoleName)
                ? association.SourceRoleName
                : GetUniqueNavPropertyName(source, target.Name);

            // Set the role name if it was empty
            if (string.IsNullOrEmpty(association.SourceRoleName))
            {
                association.SourceRoleName = navPropName;
            }

            // Determine if collection based on target multiplicity (Many = collection)
            bool isCollection = association.TargetMultiplicity == Multiplicity.Many;

            var navProp = new NavigationProperty(source.Partition)
            {
                Name = navPropName,
                TargetEntityName = target.Name,
                IsCollection = isCollection,
                Description = $"Navigation to {target.Name}"
            };

            source.NavigationProperties.Add(navProp);
        }

        private void CreateTargetNavigationProperty(Association association, EntityModel target, EntityModel source)
        {
            // Name from TargetRoleName, or default to source entity name
            string navPropName = !string.IsNullOrEmpty(association.TargetRoleName)
                ? association.TargetRoleName
                : GetUniqueNavPropertyName(target, source.Name);

            // Set the role name if it was empty
            if (string.IsNullOrEmpty(association.TargetRoleName))
            {
                association.TargetRoleName = navPropName;
            }

            // Determine if collection based on source multiplicity (Many = collection)
            bool isCollection = association.SourceMultiplicity == Multiplicity.Many;

            var navProp = new NavigationProperty(target.Partition)
            {
                Name = navPropName,
                TargetEntityName = source.Name,
                IsCollection = isCollection,
                Description = $"Navigation to {source.Name}"
            };

            target.NavigationProperties.Add(navProp);
        }

        private void CreateFkProperty(Association association, EntityModel target, EntityModel source)
        {
            // FK property name: <SourceEntityName>Id
            string fkPropName = GetUniqueFkPropertyName(target, source.Name + "Id");

            // Store the FK property name on the association
            association.FkPropertyName = fkPropName;

            var fkProp = new PropertyModel(target.Partition)
            {
                Name = fkPropName,
                DataType = DataType.Guid,
                Description = $"Foreign key to {source.Name}"
            };

            target.Attributes.Add(fkProp);
        }

        private string GetUniqueNavPropertyName(EntityModel entity, string baseName)
        {
            if (!entity.NavigationProperties.Any(np => np.Name == baseName))
                return baseName;

            int suffix = 2;
            while (entity.NavigationProperties.Any(np => np.Name == baseName + suffix))
            {
                suffix++;
            }
            return baseName + suffix;
        }

        private string GetUniqueFkPropertyName(EntityModel entity, string baseName)
        {
            if (!entity.Attributes.Any(a => a.Name == baseName))
                return baseName;

            int suffix = 2;
            while (entity.Attributes.Any(a => a.Name == baseName + suffix))
            {
                suffix++;
            }
            return baseName + suffix;
        }
    }

    /// <summary>
    /// Rule that fires when an Association relationship is being deleted.
    /// Deletes the associated NavigationProperties and FK property.
    /// </summary>
    [RuleOn(typeof(Association), FireTime = TimeToFire.TopLevelCommit)]
    public class AssociationDeleteRule : DeleteRule
    {
        public override void ElementDeleted(ElementDeletedEventArgs e)
        {
            var association = e.ModelElement as Association;
            if (association == null)
                return;

            var source = association.Source;
            var target = association.Target;

            // Delete source navigation property
            if (source != null && !source.IsDeleting && !source.IsDeleted && !string.IsNullOrEmpty(association.SourceRoleName))
            {
                var navProp = source.NavigationProperties.FirstOrDefault(np => np.Name == association.SourceRoleName);
                if (navProp != null && !navProp.IsDeleting && !navProp.IsDeleted)
                {
                    navProp.Delete();
                }
            }

            // Delete target navigation property
            if (target != null && !target.IsDeleting && !target.IsDeleted && !string.IsNullOrEmpty(association.TargetRoleName))
            {
                var navProp = target.NavigationProperties.FirstOrDefault(np => np.Name == association.TargetRoleName);
                if (navProp != null && !navProp.IsDeleting && !navProp.IsDeleted)
                {
                    navProp.Delete();
                }
            }

            // Delete FK property on target
            if (target != null && !target.IsDeleting && !target.IsDeleted && !string.IsNullOrEmpty(association.FkPropertyName))
            {
                var fkProp = target.Attributes.FirstOrDefault(a => a.Name == association.FkPropertyName);
                if (fkProp != null && !fkProp.IsDeleting && !fkProp.IsDeleted)
                {
                    fkProp.Delete();
                }
            }
        }
    }

    /// <summary>
    /// Rule that fires when Association properties change.
    /// Handles GenSourceNavProperty, GenTargetNavProperty, SourceRoleName, and TargetRoleName changes.
    /// </summary>
    [RuleOn(typeof(Association), FireTime = TimeToFire.TopLevelCommit)]
    public class AssociationPropertyChangeRule : ChangeRule
    {
        public override void ElementPropertyChanged(ElementPropertyChangedEventArgs e)
        {
            var association = e.ModelElement as Association;
            if (association == null || association.IsDeleting || association.IsDeleted)
                return;

            var source = association.Source;
            var target = association.Target;

            if (source == null || target == null)
                return;

            // Handle GenSourceNavProperty change
            if (e.DomainProperty.Id == Association.GenSourceNavPropertyDomainPropertyId)
            {
                bool newValue = (bool)e.NewValue;
                if (newValue)
                {
                    // Create source nav property
                    CreateSourceNavigationProperty(association, source, target);
                }
                else
                {
                    // Delete source nav property
                    DeleteNavigationPropertyByName(source, association.SourceRoleName);
                }
            }
            // Handle GenTargetNavProperty change
            else if (e.DomainProperty.Id == Association.GenTargetNavPropertyDomainPropertyId)
            {
                bool newValue = (bool)e.NewValue;
                if (newValue)
                {
                    // Create target nav property
                    CreateTargetNavigationProperty(association, target, source);
                }
                else
                {
                    // Delete target nav property
                    DeleteNavigationPropertyByName(target, association.TargetRoleName);
                }
            }
            // Handle SourceRoleName change - sync to source nav property name
            else if (e.DomainProperty.Id == Association.SourceRoleNameDomainPropertyId)
            {
                string oldName = e.OldValue as string;
                string newName = e.NewValue as string;
                if (!string.IsNullOrEmpty(oldName) && !string.IsNullOrEmpty(newName) && oldName != newName)
                {
                    var navProp = source.NavigationProperties.FirstOrDefault(np => np.Name == oldName);
                    if (navProp != null && !navProp.IsDeleting && !navProp.IsDeleted)
                    {
                        navProp.Name = newName;
                    }
                }
            }
            // Handle TargetRoleName change - sync to target nav property name
            else if (e.DomainProperty.Id == Association.TargetRoleNameDomainPropertyId)
            {
                string oldName = e.OldValue as string;
                string newName = e.NewValue as string;
                if (!string.IsNullOrEmpty(oldName) && !string.IsNullOrEmpty(newName) && oldName != newName)
                {
                    var navProp = target.NavigationProperties.FirstOrDefault(np => np.Name == oldName);
                    if (navProp != null && !navProp.IsDeleting && !navProp.IsDeleted)
                    {
                        navProp.Name = newName;
                    }
                }
            }
        }

        private void CreateSourceNavigationProperty(Association association, EntityModel source, EntityModel target)
        {
            string navPropName = !string.IsNullOrEmpty(association.SourceRoleName)
                ? association.SourceRoleName
                : GetUniqueNavPropertyName(source, target.Name);

            if (string.IsNullOrEmpty(association.SourceRoleName))
            {
                association.SourceRoleName = navPropName;
            }

            bool isCollection = association.TargetMultiplicity == Multiplicity.Many;

            var navProp = new NavigationProperty(source.Partition)
            {
                Name = navPropName,
                TargetEntityName = target.Name,
                IsCollection = isCollection,
                Description = $"Navigation to {target.Name}"
            };

            source.NavigationProperties.Add(navProp);
        }

        private void CreateTargetNavigationProperty(Association association, EntityModel target, EntityModel source)
        {
            string navPropName = !string.IsNullOrEmpty(association.TargetRoleName)
                ? association.TargetRoleName
                : GetUniqueNavPropertyName(target, source.Name);

            if (string.IsNullOrEmpty(association.TargetRoleName))
            {
                association.TargetRoleName = navPropName;
            }

            bool isCollection = association.SourceMultiplicity == Multiplicity.Many;

            var navProp = new NavigationProperty(target.Partition)
            {
                Name = navPropName,
                TargetEntityName = source.Name,
                IsCollection = isCollection,
                Description = $"Navigation to {source.Name}"
            };

            target.NavigationProperties.Add(navProp);
        }

        private void DeleteNavigationPropertyByName(EntityModel entity, string name)
        {
            if (entity == null || entity.IsDeleting || entity.IsDeleted || string.IsNullOrEmpty(name))
                return;

            var navProp = entity.NavigationProperties.FirstOrDefault(np => np.Name == name);
            if (navProp != null && !navProp.IsDeleting && !navProp.IsDeleted)
            {
                navProp.Delete();
            }
        }

        private string GetUniqueNavPropertyName(EntityModel entity, string baseName)
        {
            if (!entity.NavigationProperties.Any(np => np.Name == baseName))
                return baseName;

            int suffix = 2;
            while (entity.NavigationProperties.Any(np => np.Name == baseName + suffix))
            {
                suffix++;
            }
            return baseName + suffix;
        }
    }

    /// <summary>
    /// Rule that fires when a NavigationProperty is being deleted.
    /// Syncs the deletion back to the Association's Gen*NavProperty flag.
    /// </summary>
    [RuleOn(typeof(NavigationProperty), FireTime = TimeToFire.TopLevelCommit)]
    public class NavigationPropertyDeleteRule : DeletingRule
    {
        public override void ElementDeleting(ElementDeletingEventArgs e)
        {
            var navProp = e.ModelElement as NavigationProperty;
            if (navProp == null)
                return;

            var entity = navProp.EntityModel;
            if (entity == null || entity.IsDeleting || entity.IsDeleted)
                return;

            // Find associations where this entity is the source and the nav prop name matches SourceRoleName
            var sourceAssociations = Association.GetLinksToTargets(entity);
            foreach (var assoc in sourceAssociations)
            {
                if (!assoc.IsDeleting && !assoc.IsDeleted && assoc.SourceRoleName == navProp.Name)
                {
                    assoc.GenSourceNavProperty = false;
                    return;
                }
            }

            // Find associations where this entity is the target and the nav prop name matches TargetRoleName
            var targetAssociations = Association.GetLinksToSources(entity);
            foreach (var assoc in targetAssociations)
            {
                if (!assoc.IsDeleting && !assoc.IsDeleted && assoc.TargetRoleName == navProp.Name)
                {
                    assoc.GenTargetNavProperty = false;
                    return;
                }
            }
        }
    }

    /// <summary>
    /// Rule that fires when NavigationProperty.Name changes.
    /// Syncs the name change back to the Association's SourceRoleName or TargetRoleName.
    /// </summary>
    [RuleOn(typeof(NavigationProperty), FireTime = TimeToFire.TopLevelCommit)]
    public class NavigationPropertyNameChangeRule : ChangeRule
    {
        public override void ElementPropertyChanged(ElementPropertyChangedEventArgs e)
        {
            if (e.DomainProperty.Id != NavigationProperty.NameDomainPropertyId)
                return;

            var navProp = e.ModelElement as NavigationProperty;
            if (navProp == null || navProp.IsDeleting || navProp.IsDeleted)
                return;

            var entity = navProp.EntityModel;
            if (entity == null || entity.IsDeleting || entity.IsDeleted)
                return;

            string oldName = e.OldValue as string;
            string newName = e.NewValue as string;

            if (string.IsNullOrEmpty(oldName) || string.IsNullOrEmpty(newName) || oldName == newName)
                return;

            // Find associations where this entity is the source and update SourceRoleName
            var sourceAssociations = Association.GetLinksToTargets(entity);
            foreach (var assoc in sourceAssociations)
            {
                if (!assoc.IsDeleting && !assoc.IsDeleted && assoc.SourceRoleName == oldName)
                {
                    assoc.SourceRoleName = newName;
                    return;
                }
            }

            // Find associations where this entity is the target and update TargetRoleName
            var targetAssociations = Association.GetLinksToSources(entity);
            foreach (var assoc in targetAssociations)
            {
                if (!assoc.IsDeleting && !assoc.IsDeleted && assoc.TargetRoleName == oldName)
                {
                    assoc.TargetRoleName = newName;
                    return;
                }
            }
        }
    }

    /// <summary>
    /// Rule that fires when a PropertyModel (FK property) is being deleted.
    /// Deletes the associated Association if the FK property was tied to it.
    /// </summary>
    [RuleOn(typeof(PropertyModel), FireTime = TimeToFire.TopLevelCommit)]
    public class FkPropertyDeleteRule : DeletingRule
    {
        public override void ElementDeleting(ElementDeletingEventArgs e)
        {
            var property = e.ModelElement as PropertyModel;
            if (property == null)
                return;

            var entity = property.EntityModel;
            if (entity == null || entity.IsDeleting || entity.IsDeleted)
                return;

            // Find associations where this entity is the target and the property name matches FkPropertyName
            var targetAssociations = Association.GetLinksToSources(entity);
            foreach (var assoc in targetAssociations)
            {
                if (!assoc.IsDeleting && !assoc.IsDeleted && assoc.FkPropertyName == property.Name)
                {
                    // Delete the association when its FK property is deleted
                    assoc.Delete();
                    return;
                }
            }
        }
    }
}
