using Dyvenix.GenIt.DslPackage.Editors;
using Microsoft.VisualStudio.Modeling;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Dyvenix.GenIt.DslPackage.Editors.Services.Controls
{
	public partial class UpdateMethodsEditControl : UserControlBase
	{
		private ServiceModel _serviceModel;
		private LinkedElementCollection<UpdateMethodModel> _updateMethods;

		public UpdateMethodsEditControl()
		{
			InitializeComponent();
		}

		public void SetData(
			ServiceModel serviceModel,
			LinkedElementCollection<UpdateMethodModel> updateMethods,
			LinkedElementCollection<PropertyModel> properties)
		{
			_suspendUpdates = true;
			try
			{
				_serviceModel = serviceModel;
				_updateMethods = updateMethods;

				RefreshGrid();

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
			if (_updateMethods == null || _serviceModel == null)
				return;

			int nextNum = _updateMethods.Count + 1;

			DslTransactionHelper.ExecuteInTransaction(_serviceModel, "Add Update Method", () =>
			{
				var newMethod = new UpdateMethodModel(_serviceModel.Store);
				newMethod.Name = $"UpdateMethod{nextNum}";
				newMethod.ItemId = Guid.NewGuid();
				newMethod.DisplayOrder = _updateMethods.Count;
				_updateMethods.Add(newMethod);
			});

			RefreshGrid();

			var addedMethod = _updateMethods.LastOrDefault();
			if (addedMethod != null)
			{
				grdMethods.SelectedItem = addedMethod;
			}
		}

		private void btnDelete_Click(object sender, RoutedEventArgs e)
		{
			if (sender is Button btn && btn.DataContext is UpdateMethodModel method && _updateMethods != null)
			{
				var result = MessageBox.Show("Delete this item?", "Confirm Delete",
					MessageBoxButton.OKCancel, MessageBoxImage.Question);

				if (result == MessageBoxResult.OK)
				{
					DslTransactionHelper.ExecuteInTransaction(method, "Delete Update Method", () =>
					{
						method.Delete();
					});
					RefreshGrid();
				}
			}
		}

		private void btnUp_Click(object sender, RoutedEventArgs e)
		{
			if (grdMethods.SelectedItem is UpdateMethodModel selectedMethod && _updateMethods != null)
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
			if (grdMethods.SelectedItem is UpdateMethodModel selectedMethod && _updateMethods != null)
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

			DslTransactionHelper.ExecuteInTransaction(srcMethod, "Reorder Update Methods", () =>
			{
				srcMethod.DisplayOrder = targetIdx;
				targetMethod.DisplayOrder = srcIdx;
			});

			_suspendUpdates = true;
			try
			{
				RefreshGrid();
				grdMethods.UpdateLayout();

				// Find the same method in the new collection by ItemId and select it
				var newSelectedItem = grdMethods.Items.Cast<UpdateMethodModel>()
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

			var sortedMethods = _updateMethods.OrderBy(m => m.DisplayOrder).ToList();
			grdMethods.ItemsSource = sortedMethods;
		}

		private void UpdateButtonStates()
		{
			var selectedMethod = grdMethods.SelectedItem as UpdateMethodModel;
			btnUp.IsEnabled = selectedMethod != null && grdMethods.SelectedIndex > 0;
			btnDown.IsEnabled = selectedMethod != null && grdMethods.SelectedIndex < grdMethods.Items.Count - 1;
		}

		private void grdMethods_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (_suspendUpdates) return;

			UpdateButtonStates();

			var selectedMethod = grdMethods.SelectedItem as UpdateMethodModel;
			if (selectedMethod != null)
			{
				updPropsEditCtl.SetUpdateProperties(selectedMethod.PropertyModels, selectedMethod);
				updPropsEditCtl.Readonly = false;
			}
			else
			{
				updPropsEditCtl.SetUpdateProperties(null, null);
				updPropsEditCtl.Readonly = true;
				updPropsEditCtl.Clear();
			}
		}
	}
}
