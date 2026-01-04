using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Dyvenix.GenIt.DslPackage.Tools.Services.ViewModels;

namespace Dyvenix.GenIt.DslPackage.Tools.Services.Controls
{
    public partial class UpdateMethodsEditControl : UserControlBase
    {
        private ObservableCollection<UpdateMethodViewModel> _updateMethods;
        private ObservableCollection<PropertyViewModel> _properties;

        public UpdateMethodsEditControl()
        {
            InitializeComponent();
        }

        public void SetData(ObservableCollection<UpdateMethodViewModel> updateMethods, 
                            ObservableCollection<PropertyViewModel> properties)
        {
            _suspendUpdates = true;
            try
            {
                _updateMethods = updateMethods;
                _properties = properties;

                var sortedMethods = new ObservableCollection<UpdateMethodViewModel>(updateMethods.OrderBy(m => m.DisplayOrder));
                grdMethods.ItemsSource = sortedMethods;

                updPropsEditCtl.SetProperties(properties);

                if (updateMethods?.Count > 0)
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
            if (_updateMethods == null) return;

            int nextNum = _updateMethods.Count + 1;
            var newMethod = UpdateMethodViewModel.CreateNew(Guid.NewGuid(), $"UpdateMethod{nextNum}", _updateMethods.Count);
            _updateMethods.Add(newMethod);
            RefreshGrid();
            
            grdMethods.SelectedItem = newMethod;
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is UpdateMethodViewModel method && _updateMethods != null)
            {
                var result = MessageBox.Show("Delete this item?", "Confirm Delete", 
                    MessageBoxButton.OKCancel, MessageBoxImage.Question);
                
                if (result == MessageBoxResult.OK)
                {
                    _updateMethods.Remove(method);
                    RefreshGrid();
                }
            }
        }

        private void btnUp_Click(object sender, RoutedEventArgs e)
        {
            if (grdMethods.SelectedItem is UpdateMethodViewModel selectedMethod && _updateMethods != null)
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
            if (grdMethods.SelectedItem is UpdateMethodViewModel selectedMethod && _updateMethods != null)
            {
                var currentIdx = selectedMethod.DisplayOrder;
                if (currentIdx < _updateMethods.Count - 1)
                {
                    SwapOrder(currentIdx, currentIdx + 1);
                }
            }
        }

        private void SwapOrder(int srcIdx, int targetIdx)
        {
            if (_updateMethods == null) return;

            var srcMethod = _updateMethods.First(m => m.DisplayOrder == srcIdx);
            var targetMethod = _updateMethods.First(m => m.DisplayOrder == targetIdx);
            var srcItemId = srcMethod.ItemId;

            srcMethod.DisplayOrder = targetIdx;
            targetMethod.DisplayOrder = srcIdx;

            _suspendUpdates = true;
            try
            {
                RefreshGrid();
                grdMethods.UpdateLayout();
                
                // Find the same method in the new collection by ItemId and select it
                var newSelectedItem = grdMethods.Items.Cast<UpdateMethodViewModel>()
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
            if (_updateMethods == null) return;

            var sortedMethods = new ObservableCollection<UpdateMethodViewModel>(_updateMethods.OrderBy(m => m.DisplayOrder));
            grdMethods.ItemsSource = sortedMethods;
        }

        private void UpdateButtonStates()
        {
            var selectedMethod = grdMethods.SelectedItem as UpdateMethodViewModel;
            btnUp.IsEnabled = selectedMethod != null && grdMethods.SelectedIndex > 0;
            btnDown.IsEnabled = selectedMethod != null && grdMethods.SelectedIndex < grdMethods.Items.Count - 1;
        }

        private void grdMethods_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_suspendUpdates) return;

            UpdateButtonStates();

            var selectedMethod = grdMethods.SelectedItem as UpdateMethodViewModel;
            if (selectedMethod != null)
            {
                updPropsEditCtl.SetUpdateProperties(selectedMethod.UpdateProperties);
                updPropsEditCtl.Readonly = false;
            }
            else
            {
                updPropsEditCtl.SetUpdateProperties(null);
                updPropsEditCtl.Readonly = true;
                updPropsEditCtl.Clear();
            }
        }
    }
}
