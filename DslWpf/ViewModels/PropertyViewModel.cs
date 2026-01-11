using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DslWpf.ViewModels
{
    public partial class PropertyViewModel : ObservableObject
    {
        [ObservableProperty]
        private Guid _itemId;

        [ObservableProperty]
        private string _name = string.Empty;

        [ObservableProperty]
        private string _type = string.Empty;

        [ObservableProperty]
        private bool _isPrimaryKey;

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
    }
}
