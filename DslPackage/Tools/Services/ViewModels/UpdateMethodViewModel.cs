using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Dyvenix.GenIt.DslPackage.Tools.Services.ViewModels
{
    public class UpdateMethodViewModel : INotifyPropertyChanged
    {
        private Guid _itemId;
        private string _name = string.Empty;
        private bool _useDto;
        private int _displayOrder;
        private ObservableCollection<UpdatePropertyViewModel> _updateProperties = new ObservableCollection<UpdatePropertyViewModel>();

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

        public bool UseDto
        {
            get => _useDto;
            set
            {
                if (_useDto != value)
                {
                    _useDto = value;
                    OnPropertyChanged(nameof(UseDto));
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

        public ObservableCollection<UpdatePropertyViewModel> UpdateProperties
        {
            get => _updateProperties;
            set
            {
                if (_updateProperties != value)
                {
                    _updateProperties = value;
                    OnPropertyChanged(nameof(UpdateProperties));
                }
            }
        }

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

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
