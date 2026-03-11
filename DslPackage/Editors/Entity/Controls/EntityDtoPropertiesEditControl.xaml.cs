using Microsoft.VisualStudio.Modeling;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace Dyvenix.GenIt.DslPackage.Editors.Entity.Controls
{
	public partial class EntityDtoPropertiesEditControl : UserControlBase
	{
		private DtoModel _selectedDto;
		private LinkedElementCollection<PropertyModel> _properties;
		private ObservableCollection<EntityDtoPropertyDisplayViewModel> _viewModels = new ObservableCollection<EntityDtoPropertyDisplayViewModel>();

		public EntityDtoPropertiesEditControl()
		{
			InitializeComponent();
			grdDtoProperties.ItemsSource = _viewModels;
		}

		public void SetProperties(LinkedElementCollection<PropertyModel> properties)
		{
			_suspendUpdates = true;
			try
			{
				_properties = properties;
				foreach (var vm in _viewModels)
					vm.PropertyChanged -= ViewModel_PropertyChanged;
				_viewModels.Clear();
				foreach (var prop in properties)
				{
					var vm = new EntityDtoPropertyDisplayViewModel(prop);
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
					grdDtoProperties.IsEnabled = false;
					return;
				}

				grdDtoProperties.IsEnabled = true;

				var linkedProperties = dtoModel.PropertyModels;
				foreach (var vm in _viewModels)
				{
					vm.IsIncluded = linkedProperties.Contains(vm.Property);
				}
			}
			finally
			{
				_suspendUpdates = false;
			}
		}

		public bool Readonly
		{
			get { return !grdDtoProperties.IsEnabled; }
			set { grdDtoProperties.IsEnabled = !value; }
		}

		private void btnSelectAll_Click(object sender, RoutedEventArgs e)
		{
			if (_selectedDto == null || _selectedDto.Store == null)
				return;

			_suspendUpdates = true;
			try
			{
				DslTransactionHelper.ExecuteInTransaction(_selectedDto, "Select All Properties", () =>
				{
					foreach (var vm in _viewModels)
					{
						if (!vm.IsIncluded)
						{
							var link = DtoModelReferencesPropertyModels.GetLink(_selectedDto, vm.Property);
							if (link == null)
							{
								_selectedDto.PropertyModels.Add(vm.Property);
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
			var vm = sender as EntityDtoPropertyDisplayViewModel;
			if (_suspendUpdates || _selectedDto == null || vm == null)
				return;

			if (_selectedDto.Store == null)
				return;

			if (e.PropertyName == nameof(EntityDtoPropertyDisplayViewModel.IsIncluded))
			{
				if (vm.IsIncluded)
				{
					var link = DtoModelReferencesPropertyModels.GetLink(_selectedDto, vm.Property);
					if (link == null)
					{
						DslTransactionHelper.ExecuteInTransaction(_selectedDto, "Add DTO Property", () =>
						{
							_selectedDto.PropertyModels.Add(vm.Property);
						});
					}
				}
				else
				{
					var link = DtoModelReferencesPropertyModels.GetLink(_selectedDto, vm.Property);
					if (link != null)
					{
						DslTransactionHelper.ExecuteInTransaction(_selectedDto, "Remove DTO Property", () =>
						{
							link.Delete();
						});
					}
				}
			}
		}
	}

	public class EntityDtoPropertyDisplayViewModel : INotifyPropertyChanged
	{
		private bool _isIncluded;

		public PropertyModel Property { get; }
		public string PropertyName { get { return Property.Name; } }

		public EntityDtoPropertyDisplayViewModel(PropertyModel property)
		{
			Property = property;
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
