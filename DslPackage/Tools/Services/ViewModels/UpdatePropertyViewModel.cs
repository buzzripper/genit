using System.ComponentModel;

namespace Dyvenix.GenIt.DslPackage.Tools.Services.ViewModels
{
    public class UpdatePropertyViewModel : INotifyPropertyChanged
    {
        private PropertyViewModel _property;
        private bool _isOptional;

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

        public static UpdatePropertyViewModel CreateNew(PropertyViewModel property, bool isOptional = false)
        {
            return new UpdatePropertyViewModel
            {
                Property = property,
                IsOptional = isOptional
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
