using System.ComponentModel;

namespace Dyvenix.GenIt.DslPackage.Tools.Services.ViewModels
{
    public class FilterPropertyViewModel : INotifyPropertyChanged
    {
        private PropertyViewModel _property;
        private bool _isOptional;
        private bool _isInternal;
        private string _internalValue = string.Empty;

        public PropertyViewModel Property
        {
            get => _property;
            set
            {
                if (_property != value)
                {
                    _property = value;
                    OnPropertyChanged(nameof(Property));
                }
            }
        }

        public bool IsOptional
        {
            get => _isOptional;
            set
            {
                if (_isOptional != value)
                {
                    _isOptional = value;
                    OnPropertyChanged(nameof(IsOptional));
                }
            }
        }

        public bool IsInternal
        {
            get => _isInternal;
            set
            {
                if (_isInternal != value)
                {
                    _isInternal = value;
                    OnPropertyChanged(nameof(IsInternal));
                }
            }
        }

        public string InternalValue
        {
            get => _internalValue;
            set
            {
                if (_internalValue != value)
                {
                    _internalValue = value;
                    OnPropertyChanged(nameof(InternalValue));
                }
            }
        }

        public static FilterPropertyViewModel CreateNew(PropertyViewModel property)
        {
            return new FilterPropertyViewModel
            {
                Property = property,
                IsOptional = false,
                IsInternal = false,
                InternalValue = string.Empty
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
