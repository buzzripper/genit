using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Microsoft.VisualStudio.Modeling;

namespace Dyvenix.GenIt.DslPackage.Tools.Services.Controls
{
    public partial class InclNavPropEditControl : UserControlBase
    {
        private LinkedElementCollection<NavigationProperty> _navProperties;
        private ReadMethodModel _readMethod;
        private ObservableCollection<NavPropertyDisplayViewModel> _viewModels = new ObservableCollection<NavPropertyDisplayViewModel>();

        public InclNavPropEditControl()
        {
            InitializeComponent();
            grdNavProps.ItemsSource = _viewModels;
        }

        public void SetNavProperties(LinkedElementCollection<NavigationProperty> navProperties)
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

        public void SetInclNavProperties(ReadMethodModel readMethod)
        {
            _suspendUpdates = true;
            try
            {
                _readMethod = readMethod;

                // Reset all to unchecked
                foreach (var vm in _viewModels)
                {
                    vm.IsIncluded = false;
                }

                if (readMethod == null)
                {
                    grdNavProps.IsEnabled = false;
                    return;
                }

                grdNavProps.IsEnabled = true;

                // Load included nav properties from the model
                var inclNavProps = readMethod.InclNavProperties;
                if (!string.IsNullOrEmpty(inclNavProps))
                {
                    var inclNames = inclNavProps.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                .Select(n => n.Trim())
                                                .ToHashSet(StringComparer.OrdinalIgnoreCase);

                    foreach (var vm in _viewModels)
                    {
                        vm.IsIncluded = inclNames.Contains(vm.NavPropertyName);
                    }
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
            if (_suspendUpdates || _readMethod == null || vm == null)
                return;

            if (e.PropertyName == nameof(NavPropertyDisplayViewModel.IsIncluded))
            {
                // Persist the included navigation properties
                var includedNames = _viewModels
                    .Where(v => v.IsIncluded)
                    .Select(v => v.NavPropertyName)
                    .ToList();

                var newValue = string.Join(",", includedNames);

                using (var tx = _readMethod.Store.TransactionManager.BeginTransaction("Update Included Nav Properties"))
                {
                    _readMethod.InclNavProperties = newValue;
                    tx.Commit();
                }
            }
        }
    }

    public class NavPropertyDisplayViewModel : INotifyPropertyChanged
    {
        private bool _isIncluded;

        public NavigationProperty NavProperty { get; }
        public string NavPropertyName { get { return NavProperty.Name; } }

        public NavPropertyDisplayViewModel(NavigationProperty navProperty)
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
