using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Dyvenix.GenIt.DslPackage.Tools.Services.ViewModels
{
    public class ServiceViewModel : INotifyPropertyChanged
    {
        private bool _enabled;
        private bool _inclCreate;
        private bool _inclUpdate;
        private bool _inclDelete;
        private bool _inclController;
        private string _controllerVersion = "v1";
        private ObservableCollection<EditableString> _serviceUsings = new ObservableCollection<EditableString>();
        private ObservableCollection<EditableString> _serviceAttributes = new ObservableCollection<EditableString>();
        private ObservableCollection<EditableString> _controllerUsings = new ObservableCollection<EditableString>();
        private ObservableCollection<EditableString> _controllerAttributes = new ObservableCollection<EditableString>();
        private ObservableCollection<ReadMethodViewModel> _readMethods = new ObservableCollection<ReadMethodViewModel>();
        private ObservableCollection<UpdateMethodViewModel> _updateMethods = new ObservableCollection<UpdateMethodViewModel>();

        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    OnPropertyChanged(nameof(Enabled));
                }
            }
        }

        public bool InclCreate
        {
            get => _inclCreate;
            set
            {
                if (_inclCreate != value)
                {
                    _inclCreate = value;
                    OnPropertyChanged(nameof(InclCreate));
                }
            }
        }

        public bool InclUpdate
        {
            get => _inclUpdate;
            set
            {
                if (_inclUpdate != value)
                {
                    _inclUpdate = value;
                    OnPropertyChanged(nameof(InclUpdate));
                }
            }
        }

        public bool InclDelete
        {
            get => _inclDelete;
            set
            {
                if (_inclDelete != value)
                {
                    _inclDelete = value;
                    OnPropertyChanged(nameof(InclDelete));
                }
            }
        }

        public bool InclController
        {
            get => _inclController;
            set
            {
                if (_inclController != value)
                {
                    _inclController = value;
                    OnPropertyChanged(nameof(InclController));
                }
            }
        }

        public string ControllerVersion
        {
            get => _controllerVersion;
            set
            {
                if (_controllerVersion != value)
                {
                    _controllerVersion = value;
                    OnPropertyChanged(nameof(ControllerVersion));
                }
            }
        }

        public ObservableCollection<EditableString> ServiceUsings
        {
            get => _serviceUsings;
            set
            {
                if (_serviceUsings != value)
                {
                    _serviceUsings = value;
                    OnPropertyChanged(nameof(ServiceUsings));
                }
            }
        }

        public ObservableCollection<EditableString> ServiceAttributes
        {
            get => _serviceAttributes;
            set
            {
                if (_serviceAttributes != value)
                {
                    _serviceAttributes = value;
                    OnPropertyChanged(nameof(ServiceAttributes));
                }
            }
        }

        public ObservableCollection<EditableString> ControllerUsings
        {
            get => _controllerUsings;
            set
            {
                if (_controllerUsings != value)
                {
                    _controllerUsings = value;
                    OnPropertyChanged(nameof(ControllerUsings));
                }
            }
        }

        public ObservableCollection<EditableString> ControllerAttributes
        {
            get => _controllerAttributes;
            set
            {
                if (_controllerAttributes != value)
                {
                    _controllerAttributes = value;
                    OnPropertyChanged(nameof(ControllerAttributes));
                }
            }
        }

        public ObservableCollection<ReadMethodViewModel> ReadMethods
        {
            get => _readMethods;
            set
            {
                if (_readMethods != value)
                {
                    _readMethods = value;
                    OnPropertyChanged(nameof(ReadMethods));
                }
            }
        }

        public ObservableCollection<UpdateMethodViewModel> UpdateMethods
        {
            get => _updateMethods;
            set
            {
                if (_updateMethods != value)
                {
                    _updateMethods = value;
                    OnPropertyChanged(nameof(UpdateMethods));
                }
            }
        }

        public static ServiceViewModel CreateSample()
        {
            var service = new ServiceViewModel
            {
                Enabled = true,
                InclCreate = true,
                InclUpdate = true,
                InclDelete = true,
                InclController = true,
                ControllerVersion = "v1"
            };

            service.ReadMethods.Add(ReadMethodViewModel.CreateNew(Guid.NewGuid(), "GetById", 0));
            service.ReadMethods.Add(ReadMethodViewModel.CreateNew(Guid.NewGuid(), "GetAll", 1));
            service.UpdateMethods.Add(UpdateMethodViewModel.CreateNew(Guid.NewGuid(), "UpdateName", 0));

            return service;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
