using System;
using System.ComponentModel;

namespace Dyvenix.GenIt.DslPackage.Tools.Services.ViewModels
{
    public class PropertyViewModel : INotifyPropertyChanged
    {
        private Guid _itemId;
        private string _name = string.Empty;
        private string _type = string.Empty;
        private bool _isPrimaryKey;

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

        public string Type
        {
            get => _type;
            set
            {
                if (_type != value)
                {
                    _type = value;
                    OnPropertyChanged(nameof(Type));
                }
            }
        }

        public bool IsPrimaryKey
        {
            get => _isPrimaryKey;
            set
            {
                if (_isPrimaryKey != value)
                {
                    _isPrimaryKey = value;
                    OnPropertyChanged(nameof(IsPrimaryKey));
                }
            }
        }

        public static PropertyViewModel CreateNew(Guid itemId, string name, string type, bool isPrimaryKey = false)
        {
            return new PropertyViewModel
            {
                ItemId = itemId,
                Name = name,
                Type = type,
                IsPrimaryKey = isPrimaryKey
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
