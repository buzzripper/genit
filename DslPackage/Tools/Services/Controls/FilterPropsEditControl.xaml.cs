using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Dyvenix.GenIt.DslPackage.Tools.Services.Helpers;
using Microsoft.VisualStudio.Modeling;

namespace Dyvenix.GenIt.DslPackage.Tools.Services.Controls
{
    public partial class FilterPropsEditControl : UserControlBase
    {
        private LinkedElementCollection<FilterPropertyModel> _filterProps;
        private ReadMethodModel _readMethod;
        private LinkedElementCollection<PropertyModel> _properties;
        private ObservableCollection<FilterPropertyDisplayViewModel> _viewModels = new ObservableCollection<FilterPropertyDisplayViewModel>();

        public FilterPropsEditControl()
        {
            InitializeComponent();
            grdProps.ItemsSource = _viewModels;
        }

        public void SetProperties(LinkedElementCollection<PropertyModel> properties)
        {
            _suspendUpdates = true;
            try
            {
                _properties = properties;
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

        public void SetFilterProperties(LinkedElementCollection<FilterPropertyModel> filterProps, ReadMethodModel readMethod)
        {
            _suspendUpdates = true;
            try
            {
                _filterProps = filterProps;
                _readMethod = readMethod;

                foreach (var vm in _viewModels)
                {
                    vm.Reset();
                }

                if (filterProps == null || filterProps.Count == 0)
                {
                    grdProps.IsEnabled = filterProps != null;
                    return;
                }

                grdProps.IsEnabled = true;

                foreach (var vm in _viewModels)
                {
                    var filterProp = filterProps.FirstOrDefault(fp => fp.Name == vm.Property.Name);
                    if (filterProp != null)
                    {
                        vm.IsIncluded = true;
                        vm.IsOptional = filterProp.IsOptional;
                        vm.IsInternal = filterProp.IsInternal;
                        vm.InternalValue = filterProp.InternalValue;
                        vm.FilterPropertyModel = filterProp;
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
            if (_suspendUpdates || _filterProps == null || _readMethod == null || vm == null)
                return;

            if (e.PropertyName == nameof(FilterPropertyDisplayViewModel.IsIncluded))
            {
                if (vm.IsIncluded && vm.FilterPropertyModel == null)
                {
                    // Add new FilterPropertyModel
                    DslTransactionHelper.ExecuteInTransaction(_readMethod, "Add Filter Property", () =>
                    {
                        var newFilterProp = new FilterPropertyModel(_readMethod.Store);
                        newFilterProp.Name = vm.Property.Name;
                        newFilterProp.IsOptional = vm.IsOptional;
                        newFilterProp.IsInternal = vm.IsInternal;
                        newFilterProp.InternalValue = vm.InternalValue;
                        _filterProps.Add(newFilterProp);
                        vm.FilterPropertyModel = newFilterProp;
                    });
                }
                else if (!vm.IsIncluded && vm.FilterPropertyModel != null)
                {
                    // Remove FilterPropertyModel
                    var filterPropToRemove = vm.FilterPropertyModel;
                    DslTransactionHelper.ExecuteInTransaction(_readMethod, "Remove Filter Property", () =>
                    {
                        filterPropToRemove.Delete();
                    });
                    vm.FilterPropertyModel = null;
                    vm.IsOptional = false;
                    vm.IsInternal = false;
                    vm.InternalValue = string.Empty;
                }
            }
            else if (vm.FilterPropertyModel != null)
            {
                var filterProp = vm.FilterPropertyModel;
                if (e.PropertyName == nameof(FilterPropertyDisplayViewModel.IsOptional))
                {
                    DslTransactionHelper.SetPropertyIfChanged(filterProp, nameof(FilterPropertyModel.IsOptional),
                        filterProp.IsOptional, vm.IsOptional, () => filterProp.IsOptional = vm.IsOptional);
                }
                else if (e.PropertyName == nameof(FilterPropertyDisplayViewModel.IsInternal))
                {
                    DslTransactionHelper.SetPropertyIfChanged(filterProp, nameof(FilterPropertyModel.IsInternal),
                        filterProp.IsInternal, vm.IsInternal, () => filterProp.IsInternal = vm.IsInternal);
                }
                else if (e.PropertyName == nameof(FilterPropertyDisplayViewModel.InternalValue))
                {
                    DslTransactionHelper.SetPropertyIfChanged(filterProp, nameof(FilterPropertyModel.InternalValue),
                        filterProp.InternalValue, vm.InternalValue, () => filterProp.InternalValue = vm.InternalValue);
                }
            }
        }
    }

    public class FilterPropertyDisplayViewModel : INotifyPropertyChanged
    {
        private bool _isIncluded;
        private bool _isOptional;
        private bool _isInternal;
        private string _internalValue = string.Empty;

        public PropertyModel Property { get; }
        public FilterPropertyModel FilterPropertyModel { get; set; }
        public string PropertyName { get { return Property.Name; } }

        public FilterPropertyDisplayViewModel(PropertyModel property)
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
            FilterPropertyModel = null;
            OnPropertyChanged(string.Empty);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
