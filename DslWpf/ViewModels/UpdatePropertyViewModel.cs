using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DslWpf.ViewModels
{
    public partial class UpdatePropertyViewModel : ObservableObject
    {
        [ObservableProperty]
        private PropertyViewModel _property;

        [ObservableProperty]
        private bool _isOptional;

        public static UpdatePropertyViewModel CreateNew(PropertyViewModel property, bool isOptional = false)
        {
            return new UpdatePropertyViewModel
            {
                Property = property,
                IsOptional = isOptional
            };
        }
    }
}
