using Microsoft.VisualStudio.Modeling;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Dyvenix.GenIt.DslPackage.Editors.Services.Controls
{
    public partial class DtoPropertiesEditControl : UserControlBase
    {
        private DtoModel _selectedDto;
        private LinkedElementCollection<PropertyModel> _properties;
        private ObservableCollection<DtoPropertyDisplayViewModel> _viewModels = new ObservableCollection<DtoPropertyDisplayViewModel>();

        public DtoPropertiesEditControl()
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
                _viewModels.Clear();
                foreach (var prop in properties)
                {
                    var vm = new DtoPropertyDisplayViewModel(prop);
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

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var vm = sender as DtoPropertyDisplayViewModel;
            if (_suspendUpdates || _selectedDto == null || vm == null)
                return;

            if (e.PropertyName == nameof(DtoPropertyDisplayViewModel.IsIncluded))
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

    public class DtoPropertyDisplayViewModel : INotifyPropertyChanged
    {
        private bool _isIncluded;

        public PropertyModel Property { get; }
        public string PropertyName { get { return Property.Name; } }

        public DtoPropertyDisplayViewModel(PropertyModel property)
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
