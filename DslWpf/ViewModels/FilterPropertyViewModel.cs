using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DslWpf.ViewModels
{
    public partial class FilterPropertyViewModel : ObservableObject
    {
        [ObservableProperty]
        private PropertyViewModel _property;

        [ObservableProperty]
        private bool _isOptional;

        [ObservableProperty]
        private bool _isInternal;

        [ObservableProperty]
        private string _internalValue = string.Empty;

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
    }
}
