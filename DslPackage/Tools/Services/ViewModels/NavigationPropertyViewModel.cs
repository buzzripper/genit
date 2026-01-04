using System;
using System.ComponentModel;

namespace Dyvenix.GenIt.DslPackage.Tools.Services.ViewModels
{
    public class NavigationPropertyViewModel : INotifyPropertyChanged
    {
        private Guid _itemId;
        private string _name = string.Empty;
        private string _targetEntity = string.Empty;
        private bool _isCollection;

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

        public string TargetEntity
        {
            get => _targetEntity;
            set
            {
                if (_targetEntity != value)
                {
                    _targetEntity = value;
                    OnPropertyChanged(nameof(TargetEntity));
                }
            }
        }

        public bool IsCollection
        {
            get => _isCollection;
            set
            {
                if (_isCollection != value)
                {
                    _isCollection = value;
                    OnPropertyChanged(nameof(IsCollection));
                }
            }
        }

        public static NavigationPropertyViewModel CreateNew(Guid itemId, string name, string targetEntity, bool isCollection = false)
        {
            return new NavigationPropertyViewModel
            {
                ItemId = itemId,
                Name = name,
                TargetEntity = targetEntity,
                IsCollection = isCollection
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
