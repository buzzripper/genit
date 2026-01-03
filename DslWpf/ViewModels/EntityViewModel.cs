using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DslWpf.ViewModels
{
    public partial class EntityViewModel : ObservableObject
    {
        [ObservableProperty]
        private Guid _itemId;

        [ObservableProperty]
        private string _name = string.Empty;

        [ObservableProperty]
        private ServiceViewModel _service;

        [ObservableProperty]
        private ObservableCollection<PropertyViewModel> _properties = new ObservableCollection<PropertyViewModel>();

        [ObservableProperty]
        private ObservableCollection<NavigationPropertyViewModel> _navProperties = new ObservableCollection<NavigationPropertyViewModel>();

        public static EntityViewModel CreateSample()
        {
            var entity = new EntityViewModel
            {
                ItemId = Guid.NewGuid(),
                Name = "Customer"
            };

            // Add sample properties
            entity.Properties.Add(PropertyViewModel.CreateNew(Guid.NewGuid(), "Id", "int", true));
            entity.Properties.Add(PropertyViewModel.CreateNew(Guid.NewGuid(), "FirstName", "string"));
            entity.Properties.Add(PropertyViewModel.CreateNew(Guid.NewGuid(), "LastName", "string"));
            entity.Properties.Add(PropertyViewModel.CreateNew(Guid.NewGuid(), "Email", "string"));
            entity.Properties.Add(PropertyViewModel.CreateNew(Guid.NewGuid(), "Phone", "string"));
            entity.Properties.Add(PropertyViewModel.CreateNew(Guid.NewGuid(), "CreatedDate", "DateTime"));
            entity.Properties.Add(PropertyViewModel.CreateNew(Guid.NewGuid(), "IsActive", "bool"));

            // Add sample navigation properties
            entity.NavProperties.Add(NavigationPropertyViewModel.CreateNew(Guid.NewGuid(), "Orders", "Order", true));
            entity.NavProperties.Add(NavigationPropertyViewModel.CreateNew(Guid.NewGuid(), "Address", "Address", false));
            entity.NavProperties.Add(NavigationPropertyViewModel.CreateNew(Guid.NewGuid(), "Contacts", "Contact", true));

            // Create and assign service
            entity.Service = ServiceViewModel.CreateSample();

            return entity;
        }
    }
}
