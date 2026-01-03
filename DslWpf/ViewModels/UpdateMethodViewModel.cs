using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DslWpf.ViewModels
{
    public partial class UpdateMethodViewModel : ObservableObject
    {
        [ObservableProperty]
        private Guid _itemId;

        [ObservableProperty]
        private string _name = string.Empty;

        [ObservableProperty]
        private bool _useDto;

        [ObservableProperty]
        private int _displayOrder;

        [ObservableProperty]
        private ObservableCollection<UpdatePropertyViewModel> _updateProperties = new ObservableCollection<UpdatePropertyViewModel>();

        public static UpdateMethodViewModel CreateNew(Guid itemId, string name, int displayOrder)
        {
            return new UpdateMethodViewModel
            {
                ItemId = itemId,
                Name = name,
                DisplayOrder = displayOrder,
                UseDto = false
            };
        }
    }
}
