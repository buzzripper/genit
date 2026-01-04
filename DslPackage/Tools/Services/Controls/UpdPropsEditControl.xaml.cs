using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Dyvenix.GenIt.DslPackage.Tools.Services.ViewModels;

namespace Dyvenix.GenIt.DslPackage.Tools.Services.Controls
{
    public partial class UpdPropsEditControl : UserControlBase
    {
        private ObservableCollection<PropertyViewModel> _properties;
        private ObservableCollection<UpdatePropertyViewModel> _updateProps;
        private ObservableCollection<UpdatePropertyDisplayViewModel> _viewModels = new ObservableCollection<UpdatePropertyDisplayViewModel>();

        public UpdPropsEditControl()
        {
            InitializeComponent();
            grdProps.ItemsSource = _viewModels;
        }

        public void SetProperties(ObservableCollection<PropertyViewModel> properties)
        {
            _suspendUpdates = true;
            try
            {
                _properties = properties;
                _viewModels.Clear();

                foreach (var prop in properties)
                {
                    if (prop.IsPrimaryKey)
                        continue;

                    var vm = new UpdatePropertyDisplayViewModel(prop);
                    vm.PropertyChanged += ViewModel_PropertyChanged;
                    _viewModels.Add(vm);
                }
            }
            finally
            {
                _suspendUpdates = false;
            }
        }

        public void SetUpdateProperties(ObservableCollection<UpdatePropertyViewModel> updateProperties)
        {
            _suspendUpdates = true;
            try
            {
                _updateProps = updateProperties;

                foreach (var vm in _viewModels)
                {
                    vm.Reset();
                }

                if (updateProperties == null || updateProperties.Count == 0)
                {
                    grdProps.IsEnabled = false;
                    return;
                }

                grdProps.IsEnabled = true;

                foreach (var vm in _viewModels)
                {
                    var updProp = updateProperties.FirstOrDefault(up => up.Property == vm.Property);
                    if (updProp != null)
                    {
                        vm.IsIncluded = true;
                        vm.IsOptional = updProp.IsOptional;
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

        public void Clear()
        {
            grdProps.UnselectAll();
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var vm = sender as UpdatePropertyDisplayViewModel;
            if (_suspendUpdates || _updateProps == null || vm == null)
                return;

            var updProp = _updateProps.FirstOrDefault(up => up.Property == vm.Property);

            if (e.PropertyName == nameof(UpdatePropertyDisplayViewModel.IsIncluded))
            {
                if (vm.IsIncluded && updProp == null)
                {
                    _updateProps.Add(UpdatePropertyViewModel.CreateNew(vm.Property, false));
                }
                else if (!vm.IsIncluded && updProp != null)
                {
                    _updateProps.Remove(updProp);
                    vm.IsOptional = false;
                }
            }
            else if (e.PropertyName == nameof(UpdatePropertyDisplayViewModel.IsOptional) && updProp != null)
            {
                updProp.IsOptional = vm.IsOptional;
            }
        }
    }

    public class UpdatePropertyDisplayViewModel : INotifyPropertyChanged
    {
        private bool _isIncluded;
        private bool _isOptional;

        public PropertyViewModel Property { get; }
        public string PropertyName { get { return Property.Name; } }

        public UpdatePropertyDisplayViewModel(PropertyViewModel property)
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

        public void Reset()
        {
            _isIncluded = false;
            _isOptional = false;
            OnPropertyChanged(string.Empty);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
