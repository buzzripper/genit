using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DslWpf.ViewModels
{
    public partial class ServiceViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool _enabled;

        [ObservableProperty]
        private bool _inclCreate;

        [ObservableProperty]
        private bool _inclUpdate;

        [ObservableProperty]
        private bool _inclDelete;

        [ObservableProperty]
        private bool _inclController;

        [ObservableProperty]
        private string _controllerVersion = "v1";

        [ObservableProperty]
        private ObservableCollection<EditableString> _serviceUsings = new ObservableCollection<EditableString>();

        [ObservableProperty]
        private ObservableCollection<EditableString> _serviceAttributes = new ObservableCollection<EditableString>();

        [ObservableProperty]
        private ObservableCollection<EditableString> _controllerUsings = new ObservableCollection<EditableString>();

        [ObservableProperty]
        private ObservableCollection<EditableString> _controllerAttributes = new ObservableCollection<EditableString>();

        [ObservableProperty]
        private ObservableCollection<ReadMethodViewModel> _readMethods = new ObservableCollection<ReadMethodViewModel>();

        [ObservableProperty]
        private ObservableCollection<UpdateMethodViewModel> _updateMethods = new ObservableCollection<UpdateMethodViewModel>();

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
    }
}
