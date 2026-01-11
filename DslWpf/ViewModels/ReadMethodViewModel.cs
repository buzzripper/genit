using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DslWpf.ViewModels
{
    public partial class ReadMethodViewModel : ObservableObject
    {
        [ObservableProperty]
        private Guid _itemId;

        [ObservableProperty]
        private string _name = string.Empty;

        [ObservableProperty]
        private bool _inclPaging;

        [ObservableProperty]
        private bool _useQuery;

        [ObservableProperty]
        private bool _inclSorting;

        [ObservableProperty]
        private int _displayOrder;

        [ObservableProperty]
        private ObservableCollection<string> _attributes = new ObservableCollection<string>();

        [ObservableProperty]
        private ObservableCollection<FilterPropertyViewModel> _filterProperties = new ObservableCollection<FilterPropertyViewModel>();

        [ObservableProperty]
        private ObservableCollection<NavigationPropertyViewModel> _inclNavProperties = new ObservableCollection<NavigationPropertyViewModel>();

        public int AttrCount
        {
            get { return Attributes?.Count ?? 0; }
        }

        public static ReadMethodViewModel CreateNew(Guid itemId, string name, int displayOrder)
        {
            return new ReadMethodViewModel
            {
                ItemId = itemId,
                Name = name,
                DisplayOrder = displayOrder,
                InclPaging = false,
                UseQuery = false,
                InclSorting = false
            };
        }
    }
}
