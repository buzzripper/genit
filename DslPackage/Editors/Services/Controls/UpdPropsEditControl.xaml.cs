using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Dyvenix.GenIt.DslPackage.Editors;
using Microsoft.VisualStudio.Modeling;

namespace Dyvenix.GenIt.DslPackage.Editors.Services.Controls
{
    public partial class UpdPropsEditControl : UserControlBase
    {
        private LinkedElementCollection<PropertyModel> _properties;
        private LinkedElementCollection<UpdatePropertyModel> _updateProps;
        private UpdateMethodModel _updateMethod;
        private ObservableCollection<UpdatePropertyDisplayViewModel> _viewModels = new ObservableCollection<UpdatePropertyDisplayViewModel>();

        public UpdPropsEditControl()
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

        public void SetUpdateProperties(LinkedElementCollection<UpdatePropertyModel> updateProperties, UpdateMethodModel updateMethod)
        {
            _suspendUpdates = true;
            try
            {
                _updateProps = updateProperties;
                _updateMethod = updateMethod;

                foreach (var vm in _viewModels)
                {
                    vm.Reset();
                }

                if (updateProperties == null || updateProperties.Count == 0)
                {
                    grdProps.IsEnabled = updateProperties != null;
                    return;
                }

                grdProps.IsEnabled = true;

                foreach (var vm in _viewModels)
                {
                    var updProp = updateProperties.FirstOrDefault(up => up.PropertyModel == vm.Property);
                    if (updProp != null)
                    {
                        vm.IsIncluded = true;
                        vm.IsOptional = updProp.IsOptional;
                        vm.UpdatePropertyModel = updProp;

                        // If a model loads with missing links, it should be migrated; do not clear links globally.
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
            if (_suspendUpdates || _updateProps == null || _updateMethod == null || vm == null)
                return;

            if (e.PropertyName == nameof(UpdatePropertyDisplayViewModel.IsIncluded))
            {
                if (vm.IsIncluded)
                {
                    // If the VM has no model, or has a stale model that is not linked to this property,
                    // normalize to an existing model element that belongs to THIS method's collection.
                    if (vm.UpdatePropertyModel == null || vm.UpdatePropertyModel.PropertyModel != vm.Property)
                    {
                        var existing = _updateProps.FirstOrDefault(up => up.PropertyModel == vm.Property);
                        if (existing != null)
                        {
                            vm.UpdatePropertyModel = existing;
                            vm.IsOptional = existing.IsOptional;
                        }
                    }

                    if (vm.UpdatePropertyModel != null)
                        return;

                    // Add new UpdatePropertyModel
                    DslTransactionHelper.ExecuteInTransaction(_updateMethod, "Add Update Property", () =>
                    {
                        var existingUpdProp = _updateProps.FirstOrDefault(up => up.PropertyModel == vm.Property);
                        if (existingUpdProp != null)
                        {
                            existingUpdProp.IsOptional = vm.IsOptional;
                            vm.UpdatePropertyModel = existingUpdProp;
                            return;
                        }

                        var newUpdProp = new UpdatePropertyModel(_updateMethod.Store);
                        newUpdProp.IsOptional = vm.IsOptional;
                        // Add to collection first to establish the UpdateMethodModel link
                        _updateProps.Add(newUpdProp);
                        newUpdProp.PropertyModel = vm.Property;
                        vm.UpdatePropertyModel = newUpdProp;
                    });
                }
                else if (!vm.IsIncluded && vm.UpdatePropertyModel != null)
                {
                    // Remove UpdatePropertyModel
                    var updPropToRemove = vm.UpdatePropertyModel;
                    DslTransactionHelper.ExecuteInTransaction(_updateMethod, "Remove Update Property", () =>
                    {
                        updPropToRemove.Delete();
                    });
                    vm.UpdatePropertyModel = null;
                    vm.IsOptional = false;
                }
            }
            else if (e.PropertyName == nameof(UpdatePropertyDisplayViewModel.IsOptional) && vm.UpdatePropertyModel != null)
            {
                var updProp = vm.UpdatePropertyModel;
                DslTransactionHelper.SetPropertyIfChanged(updProp, nameof(UpdatePropertyModel.IsOptional),
                    updProp.IsOptional, vm.IsOptional, () => updProp.IsOptional = vm.IsOptional);
            }
        }
    }

    public class UpdatePropertyDisplayViewModel : INotifyPropertyChanged
    {
        private bool _isIncluded;
        private bool _isOptional;

        public PropertyModel Property { get; }
        public UpdatePropertyModel UpdatePropertyModel { get; set; }
        public string PropertyName { get { return Property.Name; } }

        public UpdatePropertyDisplayViewModel(PropertyModel property)
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
            UpdatePropertyModel = null;
            OnPropertyChanged(string.Empty);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
