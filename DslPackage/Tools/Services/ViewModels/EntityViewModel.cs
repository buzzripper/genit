using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Dyvenix.GenIt.DslPackage.Tools.Services.ViewModels
{
	public class EntityViewModel : INotifyPropertyChanged
	{
		private Guid _itemId;
		private string _name = string.Empty;
		private ObservableCollection<ServiceViewModel> _services = new ObservableCollection<ServiceViewModel>();
		private ObservableCollection<PropertyViewModel> _properties = new ObservableCollection<PropertyViewModel>();
		private ObservableCollection<NavigationPropertyViewModel> _navProperties = new ObservableCollection<NavigationPropertyViewModel>();

		public EntityViewModel()
		{
		}

		public EntityViewModel(EntityModel entityModel)
		{
			ItemId = Guid.NewGuid();
			Name = entityModel.Name;

			foreach (var prop in entityModel.Properties)
			{
				Properties.Add(new PropertyViewModel(prop));
			}
			foreach (var navProp in entityModel.NavigationProperties)
			{
				NavProperties.Add(new NavigationPropertyViewModel(navProp));
			}
			foreach (var svc in entityModel.ServiceModels)
			{
				Services.Add(new ServiceViewModel(svc));
			}
		}

		public Guid ItemId
		{
			get => _itemId;
			set
			{
				if (_itemId != value)
				{
					_itemId = value;
					OnPropertyChanged(nameof(ItemId));
				}
			}
		}

		public string Name
		{
			get => _name;
			set
			{
				if (_name != value)
				{
					_name = value;
					OnPropertyChanged(nameof(Name));
				}
			}
		}

		public ObservableCollection<ServiceViewModel> Services
		{
			get => _services;
			set
			{
				if (_services != value)
				{
					_services = value;
					OnPropertyChanged(nameof(Services));
				}
			}
		}

		public ObservableCollection<PropertyViewModel> Properties
		{
			get => _properties;
			set
			{
				if (_properties != value)
				{
					_properties = value;
					OnPropertyChanged(nameof(Properties));
				}
			}
		}

		public ObservableCollection<NavigationPropertyViewModel> NavProperties
		{
			get => _navProperties;
			set
			{
				if (_navProperties != value)
				{
					_navProperties = value;
					OnPropertyChanged(nameof(NavProperties));
				}
			}
		}

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
			entity.Services = ServiceViewModel.CreateSample();

			return entity;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
