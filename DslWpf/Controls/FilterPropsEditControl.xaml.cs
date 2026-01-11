using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using DslWpf.ViewModels;

namespace DslWpf.Controls
{
    public partial class FilterPropsEditControl : UserControlBase
    {
        private ObservableCollection<FilterPropertyViewModel> _filterProps;
        private ObservableCollection<FilterPropertyDisplayViewModel> _viewModels = new ObservableCollection<FilterPropertyDisplayViewModel>();

        public FilterPropsEditControl()
        {
            InitializeComponent();
            grdProps.ItemsSource = _viewModels;
        }

        public void SetProperties(ObservableCollection<PropertyViewModel> properties)
        {
            _suspendUpdates = true;
            try
            {
                _viewModels.Clear();
                foreach (var prop in properties)
                {
                    var vm = new FilterPropertyDisplayViewModel(prop);
                    vm.PropertyChanged += ViewModel_PropertyChanged;
                    _viewModels.Add(vm);
                }
            }
            finally
            {
                _suspendUpdates = false;
            }
        }

        public void SetFilterProperties(ObservableCollection<FilterPropertyViewModel> filterProps)
        {
            _suspendUpdates = true;
            try
            {
                _filterProps = filterProps;

                foreach (var vm in _viewModels)
                {
                    vm.Reset();
                }

                if (filterProps == null || filterProps.Count == 0)
                {
                    grdProps.IsEnabled = false;
                    return;
                }

                grdProps.IsEnabled = true;

                foreach (var vm in _viewModels)
                {
                    var filterProp = filterProps.FirstOrDefault(fp => fp.Property == vm.Property);
                    if (filterProp != null)
                    {
                        vm.IsIncluded = true;
                        vm.IsOptional = filterProp.IsOptional;
                        vm.IsInternal = filterProp.IsInternal;
                        vm.InternalValue = filterProp.InternalValue;
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
            get { return !grdProps.IsEnabled; }
            set { grdProps.IsEnabled = !value; }
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var vm = sender as FilterPropertyDisplayViewModel;
            if (_suspendUpdates || _filterProps == null || vm == null)
                return;

            var filterProp = _filterProps.FirstOrDefault(fp => fp.Property == vm.Property);

            if (e.PropertyName == nameof(FilterPropertyDisplayViewModel.IsIncluded))
            {
                if (vm.IsIncluded && filterProp == null)
                {
                    _filterProps.Add(FilterPropertyViewModel.CreateNew(vm.Property));
                }
                else if (!vm.IsIncluded && filterProp != null)
                {
                    _filterProps.Remove(filterProp);
                    vm.IsOptional = false;
                    vm.IsInternal = false;
                    vm.InternalValue = string.Empty;
                }
            }
            else if (filterProp != null)
            {
                if (e.PropertyName == nameof(FilterPropertyDisplayViewModel.IsOptional))
                    filterProp.IsOptional = vm.IsOptional;
                else if (e.PropertyName == nameof(FilterPropertyDisplayViewModel.IsInternal))
                    filterProp.IsInternal = vm.IsInternal;
                else if (e.PropertyName == nameof(FilterPropertyDisplayViewModel.InternalValue))
                    filterProp.InternalValue = vm.InternalValue;
            }
        }
    }

    public class FilterPropertyDisplayViewModel : INotifyPropertyChanged
    {
        private bool _isIncluded;
        private bool _isOptional;
        private bool _isInternal;
        private string _internalValue = string.Empty;

        public PropertyViewModel Property { get; }
        public string PropertyName { get { return Property.Name; } }

        public FilterPropertyDisplayViewModel(PropertyViewModel property)
        {
            Property = property;
        }

        public bool IsIncluded
        {
            get { return _isIncluded; }
            set { _isIncluded = value; OnPropertyChanged(nameof(IsIncluded)); }
        }

        public bool IsOptional
        {
            get { return _isOptional; }
            set { _isOptional = value; OnPropertyChanged(nameof(IsOptional)); }
        }

        public bool IsInternal
        {
            get { return _isInternal; }
            set { _isInternal = value; OnPropertyChanged(nameof(IsInternal)); }
        }

        public string InternalValue
        {
            get { return _internalValue; }
            set { _internalValue = value ?? string.Empty; OnPropertyChanged(nameof(InternalValue)); }
        }

        public void Reset()
        {
            _isIncluded = false;
            _isOptional = false;
            _isInternal = false;
            _internalValue = string.Empty;
            OnPropertyChanged(string.Empty);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
