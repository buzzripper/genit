using System.Linq;
using Microsoft.VisualStudio.Modeling;

namespace Dyvenix.GenIt
{
    /// <summary>
    /// Rule that fires when an EnumAssociation relationship is created.
    /// Creates a PropertyModel on the Entity with DataType = Enum and sets EnumTypeName.
    /// </summary>
    [RuleOn(typeof(EnumAssociation), FireTime = TimeToFire.TopLevelCommit)]
    public class EnumAssociationAddRule : AddRule
    {
        public override void ElementAdded(ElementAddedEventArgs e)
        {
            var link = e.ModelElement as EnumAssociation;
            if (link == null || link.IsDeleting || link.IsDeleted)
                return;

            var entity = link.Entity;
            var enumModel = link.Enum;

            if (entity == null || enumModel == null)
                return;

            // Generate a unique property name based on the enum name
            string baseName = enumModel.Name;
            string propertyName = GetUniquePropertyName(entity, baseName);

            // Set the PropertyName on the relationship
            link.PropertyName = propertyName;

            // Create the PropertyModel
            var property = new PropertyModel(entity.Partition)
            {
                Name = propertyName,
                DataType = DataType.Enum,
                EnumTypeName = enumModel.Name,
                Description = $"Property of type {enumModel.Name}"
            };

            // Add the property to the entity
            entity.Properties.Add(property);
        }

        private string GetUniquePropertyName(EntityModel entity, string baseName)
        {
            // Check if base name is available
            if (!entity.Properties.Any(a => a.Name == baseName))
                return baseName;

            // Find a unique name by adding a number suffix
            int suffix = 2;
            while (entity.Properties.Any(a => a.Name == baseName + suffix))
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
            var link = e.ModelElement as EnumAssociation;
            if (link == null)
                return;

            var entity = link.Entity;
            var propertyName = link.PropertyName;

            if (entity == null || entity.IsDeleting || entity.IsDeleted || string.IsNullOrEmpty(propertyName))
                return;

            // Find and delete the associated property
            var property = entity.Properties.FirstOrDefault(p => 
                p.Name == propertyName && 
                p.DataType == DataType.Enum);

            if (property != null && !property.IsDeleting && !property.IsDeleted)
            {
                property.Delete();
            }
        }
    }

    /// <summary>
    /// Rule that fires when a PropertyModel is being deleted.
    /// Deletes the associated EnumAssociation relationship if it's an enum property.
    /// </summary>
    [RuleOn(typeof(PropertyModel), FireTime = TimeToFire.TopLevelCommit)]
    public class PropertyModelDeleteRule : DeletingRule
    {
        public override void ElementDeleting(ElementDeletingEventArgs e)
        {
            var property = e.ModelElement as PropertyModel;
            if (property == null || property.DataType != DataType.Enum)
                return;

            var entity = property.EntityModel;
            if (entity == null || entity.IsDeleting || entity.IsDeleted)
                return;

            // Find the associated EnumAssociation link
            var links = EnumAssociation.GetLinksToUsedEnums(entity);
            var linkToDelete = links.FirstOrDefault(l => 
                l.PropertyName == property.Name && 
                !l.IsDeleting && 
                !l.IsDeleted);

            if (linkToDelete != null)
            {
                linkToDelete.Delete();
            }
        }
    }

    /// <summary>
    /// Rule that fires when EnumAssociation.PropertyName changes.
    /// Updates the associated PropertyModel's name.
    /// </summary>
    [RuleOn(typeof(EnumAssociation), FireTime = TimeToFire.TopLevelCommit)]
    public class EnumAssociationPropertyNameChangeRule : ChangeRule
    {
        public override void ElementPropertyChanged(ElementPropertyChangedEventArgs e)
        {
            if (e.DomainProperty.Id != EnumAssociation.PropertyNameDomainPropertyId)
                return;

            var link = e.ModelElement as EnumAssociation;
            if (link == null || link.IsDeleting || link.IsDeleted)
                return;

            var entity = link.Entity;
            if (entity == null || entity.IsDeleting || entity.IsDeleted)
                return;

            string oldName = e.OldValue as string;
            string newName = e.NewValue as string;

            if (string.IsNullOrEmpty(oldName) || string.IsNullOrEmpty(newName) || oldName == newName)
                return;

            // Find the property with the old name and update it
            var property = entity.Properties.FirstOrDefault(p => 
                p.Name == oldName && 
                p.DataType == DataType.Enum);

            if (property != null && !property.IsDeleting && !property.IsDeleted)
            {
                property.Name = newName;
            }
        }
    }

    /// <summary>
    /// Rule that fires when EnumModel.Name changes.
    /// Updates the EnumTypeName on all PropertyModels that reference this enum.
    /// </summary>
    [RuleOn(typeof(EnumModel), FireTime = TimeToFire.TopLevelCommit)]
    public class EnumModelNameChangeRule : ChangeRule
    {
        public override void ElementPropertyChanged(ElementPropertyChangedEventArgs e)
        {
            // Since EnumModel inherits from NamedElement, the Name property is defined there
            if (e.DomainProperty.Id != NamedElement.NameDomainPropertyId)
                return;

            var enumModel = e.ModelElement as EnumModel;
            if (enumModel == null || enumModel.IsDeleting || enumModel.IsDeleted)
                return;

            string oldName = e.OldValue as string;
            string newName = e.NewValue as string;

            if (string.IsNullOrEmpty(oldName) || string.IsNullOrEmpty(newName) || oldName == newName)
                return;

            // Find all EnumAssociation links for this enum and update the associated properties
            var links = EnumAssociation.GetLinksToUsingEntities(enumModel);
            foreach (var link in links)
            {
                if (link.IsDeleting || link.IsDeleted)
                    continue;

                var entity = link.Entity;
                if (entity == null || entity.IsDeleting || entity.IsDeleted)
                    continue;

                // Find the property with this enum type and update EnumTypeName
                var property = entity.Properties.FirstOrDefault(p =>
                    p.Name == link.PropertyName &&
                    p.DataType == DataType.Enum &&
                    p.EnumTypeName == oldName);

                if (property != null && !property.IsDeleting && !property.IsDeleted)
                {
                    property.EnumTypeName = newName;
                }
            }
        }
    }
}
