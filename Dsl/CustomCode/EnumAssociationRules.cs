using System;
using System.Linq;
using Microsoft.VisualStudio.Modeling;

namespace Dyvenix.GenIt
{
    /// <summary>
    /// Rule that fires when an EnumAssociation relationship is created.
    /// Creates a PropertyModel on the EntityModel with DataType=Enum and EnumTypeName set to the enum's name.
    /// </summary>
    [RuleOn(typeof(EnumAssociation), FireTime = TimeToFire.TopLevelCommit)]
    public class EnumAssociationAddRule : AddRule
    {
        public override void ElementAdded(ElementAddedEventArgs e)
        {
            var enumAssociation = e.ModelElement as EnumAssociation;
            if (enumAssociation == null || enumAssociation.IsDeleting || enumAssociation.IsDeleted)
                return;

            // Don't create properties if we're deserializing from file
            if (enumAssociation.Store.InSerializationTransaction)
                return;

            var entity = enumAssociation.Entity;
            var enumModel = enumAssociation.Enum;

            if (entity == null || enumModel == null)
                return;

            // Generate a unique property name based on the enum name
            string propertyName = GetUniquePropertyName(entity, enumModel.Name);

            // Store the property name on the association
            enumAssociation.PropertyName = propertyName;

            // Create the property with DataType=Enum and EnumTypeName set
            var enumProperty = new PropertyModel(entity.Partition)
            {
                Name = propertyName,
                DataType = DataType.Enum,
                EnumTypeName = enumModel.Name,
                Description = $"Enum property of type {enumModel.Name}"
            };

            entity.Properties.Add(enumProperty);
        }

        private string GetUniquePropertyName(EntityModel entity, string baseName)
        {
            if (!entity.Properties.Any(p => p.Name == baseName))
                return baseName;

            int suffix = 2;
            while (entity.Properties.Any(p => p.Name == baseName + suffix))
            {
                suffix++;
            }
            return baseName + suffix;
        }
    }

    /// <summary>
    /// Rule that fires when an EnumAssociation relationship is being deleted.
    /// Deletes the associated PropertyModel.
    /// </summary>
    [RuleOn(typeof(EnumAssociation), FireTime = TimeToFire.TopLevelCommit)]
    public class EnumAssociationDeleteRule : DeleteRule
    {
        public override void ElementDeleted(ElementDeletedEventArgs e)
        {
            var enumAssociation = e.ModelElement as EnumAssociation;
            if (enumAssociation == null)
                return;

            var entity = enumAssociation.Entity;

            // Delete the associated property
            if (entity != null && !entity.IsDeleting && !entity.IsDeleted && !string.IsNullOrEmpty(enumAssociation.PropertyName))
            {
                var property = entity.Properties.FirstOrDefault(p => p.Name == enumAssociation.PropertyName);
                if (property != null && !property.IsDeleting && !property.IsDeleted)
                {
                    property.Delete();
                }
            }
        }
    }

    /// <summary>
    /// Rule that fires when EnumAssociation.PropertyName changes.
    /// Syncs the name change to the PropertyModel.
    /// </summary>
    [RuleOn(typeof(EnumAssociation), FireTime = TimeToFire.TopLevelCommit)]
    public class EnumAssociationPropertyNameChangeRule : ChangeRule
    {
        public override void ElementPropertyChanged(ElementPropertyChangedEventArgs e)
        {
            if (e.DomainProperty.Id != EnumAssociation.PropertyNameDomainPropertyId)
                return;

            var enumAssociation = e.ModelElement as EnumAssociation;
            if (enumAssociation == null || enumAssociation.IsDeleting || enumAssociation.IsDeleted)
                return;

            var entity = enumAssociation.Entity;
            if (entity == null || entity.IsDeleting || entity.IsDeleted)
                return;

            string oldName = e.OldValue as string;
            string newName = e.NewValue as string;

            if (string.IsNullOrEmpty(oldName) || string.IsNullOrEmpty(newName) || oldName == newName)
                return;

            // Find and update the property name
            var property = entity.Properties.FirstOrDefault(p => p.Name == oldName);
            if (property != null && !property.IsDeleting && !property.IsDeleted)
            {
                property.Name = newName;
            }
        }
    }

    /// <summary>
    /// Rule that fires when EnumModel.Name changes.
    /// Updates EnumTypeName on all PropertyModels that reference this enum via EnumAssociation.
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

            // Get all EnumAssociations that point to this enum
            var enumAssociations = EnumAssociation.GetLinksToUsingEntities(enumModel);

            foreach (var assoc in enumAssociations)
            {
                if (assoc.IsDeleting || assoc.IsDeleted)
                    continue;

                var entity = assoc.Entity;
                if (entity == null || entity.IsDeleting || entity.IsDeleted)
                    continue;

                // Find the property and update EnumTypeName
                var property = entity.Properties.FirstOrDefault(p => p.Name == assoc.PropertyName);
                if (property != null && !property.IsDeleting && !property.IsDeleted)
                {
                    property.EnumTypeName = newName;
                }
            }

            // Also update any PropertyModel that references this enum by EnumTypeName (even if not via EnumAssociation)
            var store = enumModel.Store;
            var allProperties = store.ElementDirectory.FindElements<PropertyModel>();

            foreach (var prop in allProperties)
            {
                if (prop.IsDeleting || prop.IsDeleted)
                    continue;

                if (prop.DataType == DataType.Enum && prop.EnumTypeName == oldName)
                {
                    prop.EnumTypeName = newName;
                }
            }
        }
    }

    /// <summary>
    /// Rule that fires when a PropertyModel is being deleted.
    /// If it's an enum property tied to an EnumAssociation, delete the association.
    /// </summary>
    [RuleOn(typeof(PropertyModel), FireTime = TimeToFire.TopLevelCommit)]
    public class EnumPropertyDeleteRule : DeletingRule
    {
        public override void ElementDeleting(ElementDeletingEventArgs e)
        {
            var property = e.ModelElement as PropertyModel;
            if (property == null || property.DataType != DataType.Enum)
                return;

            var entity = property.EntityModel;
            if (entity == null || entity.IsDeleting || entity.IsDeleted)
                return;

            // Find EnumAssociations where this entity is the source and the property name matches
            var enumAssociations = EnumAssociation.GetLinksToUsedEnums(entity);
            foreach (var assoc in enumAssociations)
            {
                if (!assoc.IsDeleting && !assoc.IsDeleted && assoc.PropertyName == property.Name)
                {
                    // Delete the association when its property is deleted
                    assoc.Delete();
                    return;
                }
            }
        }
    }

    /// <summary>
    /// Rule that fires when PropertyModel.Name changes.
    /// If it's an enum property tied to an EnumAssociation, sync the name back to the association.
    /// </summary>
    [RuleOn(typeof(PropertyModel), FireTime = TimeToFire.TopLevelCommit)]
    public class EnumPropertyNameChangeRule : ChangeRule
    {
        public override void ElementPropertyChanged(ElementPropertyChangedEventArgs e)
        {
            if (e.DomainProperty.Id != NamedElement.NameDomainPropertyId)
                return;

            var property = e.ModelElement as PropertyModel;
            if (property == null || property.IsDeleting || property.IsDeleted)
                return;

            // Only handle enum type properties
            if (property.DataType != DataType.Enum)
                return;

            var entity = property.EntityModel;
            if (entity == null || entity.IsDeleting || entity.IsDeleted)
                return;

            string oldName = e.OldValue as string;
            string newName = e.NewValue as string;

            if (string.IsNullOrEmpty(oldName) || string.IsNullOrEmpty(newName) || oldName == newName)
                return;

            // Find EnumAssociations where this entity is the source and update PropertyName
            var enumAssociations = EnumAssociation.GetLinksToUsedEnums(entity);
            foreach (var assoc in enumAssociations)
            {
                if (!assoc.IsDeleting && !assoc.IsDeleted && assoc.PropertyName == oldName)
                {
                    assoc.PropertyName = newName;
                    return;
                }
            }
        }
    }
}
