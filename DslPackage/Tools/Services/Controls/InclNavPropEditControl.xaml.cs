using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Dyvenix.GenIt.DslPackage.Tools.Services.ViewModels;

namespace Dyvenix.GenIt.DslPackage.Tools.Services.Controls
{
    public partial class InclNavPropEditControl : UserControlBase
    {
        private ObservableCollection<NavigationPropertyViewModel> _navProperties;
        private ObservableCollection<NavigationPropertyViewModel> _inclNavProps;
        private ObservableCollection<NavPropertyDisplayViewModel> _viewModels = new ObservableCollection<NavPropertyDisplayViewModel>();

        public InclNavPropEditControl()
        {
            InitializeComponent();
            grdNavProps.ItemsSource = _viewModels;
        }

        public void SetNavProperties(ObservableCollection<NavigationPropertyViewModel> navProperties)
        {
            _suspendUpdates = true;
            try
            {
                _navProperties = navProperties;
                _viewModels.Clear();

                foreach (var navProp in navProperties)
                {
                    var vm = new NavPropertyDisplayViewModel(navProp);
                    vm.PropertyChanged += ViewModel_PropertyChanged;
                    _viewModels.Add(vm);
                }
            }
            finally
            {
                _suspendUpdates = false;
            }
        }

        public void SetInclNavProperties(ObservableCollection<NavigationPropertyViewModel> inclNavProperties)
        {
            _suspendUpdates = true;
            try
            {
                _inclNavProps = inclNavProperties;

                foreach (var vm in _viewModels)
                {
                    vm.IsIncluded = false;
                }

                if (inclNavProperties == null || inclNavProperties.Count == 0)
                {
                    grdNavProps.IsEnabled = false;
                    return;
                }

                grdNavProps.IsEnabled = true;

                foreach (var vm in _viewModels)
                {
                    var inclProp = inclNavProperties.FirstOrDefault(np => np == vm.NavProperty);
                    vm.IsIncluded = inclProp != null;
                }
            }
            finally
            {
                _suspendUpdates = false;
            }
        }

        public bool Readonly
        {
            get { return !grdNavProps.IsEnabled; }
            set { grdNavProps.IsEnabled = !value; }
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var vm = sender as NavPropertyDisplayViewModel;
            if (_suspendUpdates || _inclNavProps == null || vm == null)
                return;

            if (e.PropertyName == nameof(NavPropertyDisplayViewModel.IsIncluded))
            {
                var existingProp = _inclNavProps.FirstOrDefault(np => np == vm.NavProperty);

                if (vm.IsIncluded && existingProp == null)
                {
                    _inclNavProps.Add(vm.NavProperty);
                }
                else if (!vm.IsIncluded && existingProp != null)
                {
                    _inclNavProps.Remove(existingProp);
                }
            }
        }
    }

    public class NavPropertyDisplayViewModel : INotifyPropertyChanged
    {
        private bool _isIncluded;

        public NavigationPropertyViewModel NavProperty { get; }
        public string NavPropertyName { get { return NavProperty.Name; } }

        public NavPropertyDisplayViewModel(NavigationPropertyViewModel navProperty)
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
