using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DslWpf.ViewModels
{
    public partial class NavigationPropertyViewModel : ObservableObject
    {
        [ObservableProperty]
        private Guid _itemId;

        [ObservableProperty]
        private string _name = string.Empty;

        [ObservableProperty]
        private string _targetEntity = string.Empty;

        [ObservableProperty]
        private bool _isCollection;

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
    }
}
