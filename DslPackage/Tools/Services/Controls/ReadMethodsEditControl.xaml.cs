using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Dyvenix.GenIt.DslPackage.Tools.Services.ViewModels;

namespace Dyvenix.GenIt.DslPackage.Tools.Services.Controls
{
    public partial class ReadMethodsEditControl : UserControlBase
    {
        private ObservableCollection<ReadMethodViewModel> _readMethods;
        private ObservableCollection<PropertyViewModel> _properties;
        private ObservableCollection<NavigationPropertyViewModel> _navProperties;

        public ReadMethodsEditControl()
        {
            InitializeComponent();
        }

        public void SetData(ObservableCollection<ReadMethodViewModel> readMethods, 
                            ObservableCollection<PropertyViewModel> properties, 
                            ObservableCollection<NavigationPropertyViewModel> navProperties)
        {
            _suspendUpdates = true;
            try
            {
                _readMethods = readMethods;
                _properties = properties;
                _navProperties = navProperties;

                var sortedMethods = new ObservableCollection<ReadMethodViewModel>(readMethods.OrderBy(m => m.DisplayOrder));
                grdMethods.ItemsSource = sortedMethods;

                filterPropsCtl.SetProperties(properties);
                inclNavPropEditCtl.SetNavProperties(navProperties);

                if (readMethods?.Count > 0)
                {
                    grdMethods.SelectedIndex = 0;
                }
            }
            finally
            {
                _suspendUpdates = false;
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (_readMethods == null) return;

            int nextNum = _readMethods.Count + 1;
            var newMethod = ReadMethodViewModel.CreateNew(Guid.NewGuid(), $"ReadMethod{nextNum}", _readMethods.Count);
            _readMethods.Add(newMethod);
            RefreshGrid();
            
            grdMethods.SelectedItem = newMethod;
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is ReadMethodViewModel method && _readMethods != null)
            {
                var result = MessageBox.Show("Delete this item?", "Confirm Delete", 
                    MessageBoxButton.OKCancel, MessageBoxImage.Question);
                
                if (result == MessageBoxResult.OK)
                {
                    _readMethods.Remove(method);
                    RefreshGrid();
                }
            }
        }

        private void btnAttrs_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is ReadMethodViewModel method)
            {
                // TODO: Open string list editor for attributes
                MessageBox.Show($"Edit attributes for {method.Name}\nCount: {method.AttrCount}", "Attributes");
            }
        }

        private void btnUp_Click(object sender, RoutedEventArgs e)
        {
            if (grdMethods.SelectedItem is ReadMethodViewModel selectedMethod && _readMethods != null)
            {
                var currentIdx = selectedMethod.DisplayOrder;
                if (currentIdx > 0)
                {
                    SwapOrder(currentIdx, currentIdx - 1);
                }
            }
        }

        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            if (grdMethods.SelectedItem is ReadMethodViewModel selectedMethod && _readMethods != null)
            {
                var currentIdx = selectedMethod.DisplayOrder;
                if (currentIdx < _readMethods.Count - 1)
                {
                    SwapOrder(currentIdx, currentIdx + 1);
                }
            }
        }

        private void SwapOrder(int srcIdx, int targetIdx)
        {
            if (_readMethods == null) return;

            var srcMethod = _readMethods.First(m => m.DisplayOrder == srcIdx);
            var targetMethod = _readMethods.First(m => m.DisplayOrder == targetIdx);
            var srcItemId = srcMethod.ItemId;

            srcMethod.DisplayOrder = targetIdx;
            targetMethod.DisplayOrder = srcIdx;

            _suspendUpdates = true;
            try
            {
                RefreshGrid();
                grdMethods.UpdateLayout();
                
                // Find the same method in the new collection by ItemId and select it
                var newSelectedItem = grdMethods.Items.Cast<ReadMethodViewModel>()
                    .FirstOrDefault(m => m.ItemId == srcItemId);
                
                if (newSelectedItem != null)
                {
                    grdMethods.SelectedItem = newSelectedItem;
                    grdMethods.UpdateLayout();
                    grdMethods.ScrollIntoView(newSelectedItem);
                    
                    // Focus the DataGrid to maintain selection highlight
                    grdMethods.Focus();
                }
            }
            finally
            {
                _suspendUpdates = false;
            }
            
            // Update button states after selection is restored
            UpdateButtonStates();
        }

        private void RefreshGrid()
        {
            if (_readMethods == null) return;

            var sortedMethods = new ObservableCollection<ReadMethodViewModel>(_readMethods.OrderBy(m => m.DisplayOrder));
            grdMethods.ItemsSource = sortedMethods;
        }

        private void UpdateButtonStates()
        {
            var selectedMethod = grdMethods.SelectedItem as ReadMethodViewModel;
            btnUp.IsEnabled = selectedMethod != null && grdMethods.SelectedIndex > 0;
            btnDown.IsEnabled = selectedMethod != null && grdMethods.SelectedIndex < grdMethods.Items.Count - 1;
        }

        private void grdMethods_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_suspendUpdates) return;

            UpdateButtonStates();

            var selectedMethod = grdMethods.SelectedItem as ReadMethodViewModel;
            if (selectedMethod != null)
            {
                filterPropsCtl.SetFilterProperties(selectedMethod.FilterProperties);
                filterPropsCtl.Readonly = false;
                inclNavPropEditCtl.SetInclNavProperties(selectedMethod.InclNavProperties);
                inclNavPropEditCtl.Readonly = false;
            }
            else
            {
                filterPropsCtl.SetFilterProperties(null);
                filterPropsCtl.Readonly = true;
                inclNavPropEditCtl.SetInclNavProperties(null);
                inclNavPropEditCtl.Readonly = true;
            }
        }
    }
}
