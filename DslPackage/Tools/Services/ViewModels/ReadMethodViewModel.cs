using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Dyvenix.GenIt.DslPackage.Tools.Services.ViewModels
{
    public class ReadMethodViewModel : INotifyPropertyChanged
    {
        private Guid _itemId;
        private string _name = string.Empty;
        private bool _inclPaging;
        private bool _useQuery;
        private bool _inclSorting;
        private int _displayOrder;
        private ObservableCollection<string> _attributes = new ObservableCollection<string>();
        private ObservableCollection<FilterPropertyViewModel> _filterProperties = new ObservableCollection<FilterPropertyViewModel>();
        private ObservableCollection<NavigationPropertyViewModel> _inclNavProperties = new ObservableCollection<NavigationPropertyViewModel>();

        public Guid ItemId
        {
            get => _itemId;
            set
            {
                if (_itemId != value)
                {
                    _itemId = value;
                    OnPropertyChanged(nameof(ItemId));
                }
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public bool InclPaging
        {
            get => _inclPaging;
            set
            {
                if (_inclPaging != value)
                {
                    _inclPaging = value;
                    OnPropertyChanged(nameof(InclPaging));
                }
            }
        }

        public bool UseQuery
        {
            get => _useQuery;
            set
            {
                if (_useQuery != value)
                {
                    _useQuery = value;
                    OnPropertyChanged(nameof(UseQuery));
                }
            }
        }

        public bool InclSorting
        {
            get => _inclSorting;
            set
            {
                if (_inclSorting != value)
                {
                    _inclSorting = value;
                    OnPropertyChanged(nameof(InclSorting));
                }
            }
        }

        public int DisplayOrder
        {
            get => _displayOrder;
            set
            {
                if (_displayOrder != value)
                {
                    _displayOrder = value;
                    OnPropertyChanged(nameof(DisplayOrder));
                }
            }
        }

        public ObservableCollection<string> Attributes
        {
            get => _attributes;
            set
            {
                if (_attributes != value)
                {
                    _attributes = value;
                    OnPropertyChanged(nameof(Attributes));
                    OnPropertyChanged(nameof(AttrCount));
                }
            }
        }

        public ObservableCollection<FilterPropertyViewModel> FilterProperties
        {
            get => _filterProperties;
            set
            {
                if (_filterProperties != value)
                {
                    _filterProperties = value;
                    OnPropertyChanged(nameof(FilterProperties));
                }
            }
        }

        public ObservableCollection<NavigationPropertyViewModel> InclNavProperties
        {
            get => _inclNavProperties;
            set
            {
                if (_inclNavProperties != value)
                {
                    _inclNavProperties = value;
                    OnPropertyChanged(nameof(InclNavProperties));
                }
            }
        }

        public int AttrCount => Attributes?.Count ?? 0;

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

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
