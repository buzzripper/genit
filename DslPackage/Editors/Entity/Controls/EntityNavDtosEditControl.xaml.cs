using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Dyvenix.GenIt.DslPackage.Editors.Entity.Controls
{
	public partial class EntityNavDtosEditControl : UserControlBase
	{
		private ObservableCollection<NavDtoDisplayViewModel> _viewModels = new ObservableCollection<NavDtoDisplayViewModel>();

		public EntityNavDtosEditControl()
		{
			InitializeComponent();
			grdNavDtos.ItemsSource = _viewModels;
		}

		public void SetSelectedNavProperty(NavigationProperty navProp)
		{
			ClearViewModels();

			if (navProp == null || navProp.Store == null || string.IsNullOrEmpty(navProp.TargetEntityName))
				return;

			var targetEntity = navProp.Store.ElementDirectory
				.FindElements<EntityModel>()
				.FirstOrDefault(e => e.Name == navProp.TargetEntityName);

			if (targetEntity == null)
				return;

			foreach (var dto in targetEntity.DtoModels)
			{
				var vm = new NavDtoDisplayViewModel(dto.Name);
				vm.PropertyChanged += ViewModel_PropertyChanged;
				_viewModels.Add(vm);
			}
		}

		public void Clear()
		{
			ClearViewModels();
		}

		private void ClearViewModels()
		{
			foreach (var vm in _viewModels)
				vm.PropertyChanged -= ViewModel_PropertyChanged;
			_viewModels.Clear();
		}

		private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (_suspendUpdates) return;

			var vm = sender as NavDtoDisplayViewModel;
			if (vm == null || e.PropertyName != nameof(NavDtoDisplayViewModel.IsSelected))
				return;

			if (vm.IsSelected)
			{
				_suspendUpdates = true;
				try
				{
					foreach (var other in _viewModels)
					{
						if (other != vm)
							other.IsSelected = false;
					}
				}
				finally
				{
					_suspendUpdates = false;
				}
			}
		}
	}

	public class NavDtoDisplayViewModel : INotifyPropertyChanged
	{
		private bool _isSelected;

		public string Name { get; }

		public NavDtoDisplayViewModel(string name)
		{
			Name = name;
		}

		public bool IsSelected
		{
			get { return _isSelected; }
			set { _isSelected = value; OnPropertyChanged(nameof(IsSelected)); }
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
