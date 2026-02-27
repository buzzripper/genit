using Microsoft.VisualStudio.Modeling;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Dyvenix.GenIt.DslPackage.Editors.Entity.Controls
{
	public partial class EntityDtoNavPropertiesEditControl : UserControlBase
	{
		private DtoModel _selectedDto;
		private LinkedElementCollection<NavigationProperty> _navProperties;
		private ObservableCollection<EntityDtoNavPropertyDisplayViewModel> _viewModels = new ObservableCollection<EntityDtoNavPropertyDisplayViewModel>();

		public Action<NavigationProperty> NavPropertySelected { get; set; }

		public EntityDtoNavPropertiesEditControl()
		{
			InitializeComponent();
			grdDtoNavProperties.ItemsSource = _viewModels;
		}

		private void grdDtoNavProperties_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (_suspendUpdates) return;

			var vm = grdDtoNavProperties.SelectedItem as EntityDtoNavPropertyDisplayViewModel;
			NavPropertySelected?.Invoke(vm?.NavProperty);
		}

		public void SetNavProperties(LinkedElementCollection<NavigationProperty> navProperties)
		{
			_suspendUpdates = true;
			try
			{
				_navProperties = navProperties;
				foreach (var vm in _viewModels)
					vm.PropertyChanged -= ViewModel_PropertyChanged;
				_viewModels.Clear();
				foreach (var navProp in navProperties)
				{
					var vm = new EntityDtoNavPropertyDisplayViewModel(navProp);
					vm.PropertyChanged += ViewModel_PropertyChanged;
					_viewModels.Add(vm);
				}
			}
			finally
			{
				_suspendUpdates = false;
			}
		}

		public void SetSelectedDto(DtoModel dtoModel)
		{
			_suspendUpdates = true;
			try
			{
				_selectedDto = dtoModel;

				foreach (var vm in _viewModels)
				{
					vm.IsIncluded = false;
				}

				if (dtoModel == null)
				{
					grdDtoNavProperties.IsEnabled = false;
					return;
				}

				grdDtoNavProperties.IsEnabled = true;

				var linkedNavProps = dtoModel.NavigationProperties;
				foreach (var vm in _viewModels)
				{
					vm.IsIncluded = linkedNavProps.Contains(vm.NavProperty);
				}
			}
			finally
			{
				_suspendUpdates = false;
			}
		}

		public bool Readonly
		{
			get { return !grdDtoNavProperties.IsEnabled; }
			set { grdDtoNavProperties.IsEnabled = !value; }
		}

		private void btnSelectAll_Click(object sender, RoutedEventArgs e)
		{
			if (_selectedDto == null || _selectedDto.Store == null)
				return;

			_suspendUpdates = true;
			try
			{
				DslTransactionHelper.ExecuteInTransaction(_selectedDto, "Select All Nav Properties", () =>
				{
					foreach (var vm in _viewModels)
					{
						if (!vm.IsIncluded)
						{
							var link = DtoModelReferencesNavigationProperties.GetLink(_selectedDto, vm.NavProperty);
							if (link == null)
							{
								_selectedDto.NavigationProperties.Add(vm.NavProperty);
							}
						}
					}
				});

				foreach (var vm in _viewModels)
				{
					vm.IsIncluded = true;
				}
			}
			finally
			{
				_suspendUpdates = false;
			}
		}

		private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			var vm = sender as EntityDtoNavPropertyDisplayViewModel;
			if (_suspendUpdates || _selectedDto == null || vm == null)
				return;

			if (_selectedDto.Store == null)
				return;

			if (e.PropertyName == nameof(EntityDtoNavPropertyDisplayViewModel.IsIncluded))
			{
				if (vm.IsIncluded)
				{
					var link = DtoModelReferencesNavigationProperties.GetLink(_selectedDto, vm.NavProperty);
					if (link == null)
					{
						DslTransactionHelper.ExecuteInTransaction(_selectedDto, "Add DTO Nav Property", () =>
						{
							_selectedDto.NavigationProperties.Add(vm.NavProperty);
						});
					}
				}
				else
				{
					var link = DtoModelReferencesNavigationProperties.GetLink(_selectedDto, vm.NavProperty);
					if (link != null)
					{
						DslTransactionHelper.ExecuteInTransaction(_selectedDto, "Remove DTO Nav Property", () =>
						{
							link.Delete();
						});
					}
				}
			}
		}
	}

	public class EntityDtoNavPropertyDisplayViewModel : INotifyPropertyChanged
	{
		private bool _isIncluded;

		public NavigationProperty NavProperty { get; }
		public string PropertyName { get { return NavProperty.Name; } }

		public EntityDtoNavPropertyDisplayViewModel(NavigationProperty navProperty)
		{
			NavProperty = navProperty;
		}

		public bool IsIncluded
		{
			get { return _isIncluded; }
			set { _isIncluded = value; OnPropertyChanged(nameof(IsIncluded)); }
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
