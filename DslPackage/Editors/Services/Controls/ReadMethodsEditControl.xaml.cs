using Dyvenix.GenIt.DslPackage.Editors;
using Microsoft.VisualStudio.Modeling;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Dyvenix.GenIt.DslPackage.Editors.Services.Controls
{
	public partial class ReadMethodsEditControl : UserControlBase
	{
		private ServiceModel _serviceModel;
		private LinkedElementCollection<ReadMethodModel> _readMethods;

		public ReadMethodsEditControl()
		{
			InitializeComponent();
		}

		public void SetData(
			ServiceModel serviceModel,
			LinkedElementCollection<ReadMethodModel> readMethods,
			LinkedElementCollection<PropertyModel> properties,
			LinkedElementCollection<NavigationProperty> navProperties)
		{
			_suspendUpdates = true;
			try
			{
				_serviceModel = serviceModel;
				_readMethods = readMethods;

				RefreshGrid();

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
			if (_readMethods == null || _serviceModel == null)
				return;

			int nextNum = _readMethods.Count + 1;

			DslTransactionHelper.ExecuteInTransaction(_serviceModel, "Add Read Method", () =>
			{
				var newMethod = new ReadMethodModel(_serviceModel.Store);
				newMethod.Name = $"ReadMethod{nextNum}";
				newMethod.ItemId = Guid.NewGuid();
				newMethod.DisplayOrder = _readMethods.Count;
				_readMethods.Add(newMethod);
			});

			RefreshGrid();

			var addedMethod = _readMethods.LastOrDefault();
			if (addedMethod != null)
			{
				grdMethods.SelectedItem = addedMethod;
			}
		}

		private void btnDelete_Click(object sender, RoutedEventArgs e)
		{
			if (sender is Button btn && btn.DataContext is ReadMethodModel method && _readMethods != null)
			{
				var result = MessageBox.Show("Delete this item?", "Confirm Delete",
					MessageBoxButton.OKCancel, MessageBoxImage.Question);

				if (result == MessageBoxResult.OK)
				{
					DslTransactionHelper.ExecuteInTransaction(method, "Delete Read Method", () =>
					{
						method.Delete();
					});
					RefreshGrid();
				}
			}
		}

		private void btnAttrs_Click(object sender, RoutedEventArgs e)
		{
			if (sender is Button btn && btn.DataContext is ReadMethodModel method)
			{
				// TODO: Open string list editor for attributes
				var attrCount = string.IsNullOrWhiteSpace(method.Attributes) ? 0 :
					method.Attributes.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length;
				MessageBox.Show($"Edit attributes for {method.Name}\nCount: {attrCount}", "Attributes");
			}
		}

		private void btnUp_Click(object sender, RoutedEventArgs e)
		{
			if (grdMethods.SelectedItem is ReadMethodModel selectedMethod && _readMethods != null)
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
			if (grdMethods.SelectedItem is ReadMethodModel selectedMethod && _readMethods != null)
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

			DslTransactionHelper.ExecuteInTransaction(srcMethod, "Reorder Read Methods", () =>
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
				var newSelectedItem = grdMethods.Items.Cast<ReadMethodModel>()
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

			var sortedMethods = _readMethods.OrderBy(m => m.DisplayOrder).ToList();
			grdMethods.ItemsSource = sortedMethods;
		}

		private void UpdateButtonStates()
		{
			var selectedMethod = grdMethods.SelectedItem as ReadMethodModel;
			btnUp.IsEnabled = selectedMethod != null && grdMethods.SelectedIndex > 0;
			btnDown.IsEnabled = selectedMethod != null && grdMethods.SelectedIndex < grdMethods.Items.Count - 1;
		}

		private void grdMethods_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (_suspendUpdates) return;

			UpdateButtonStates();

			var selectedMethod = grdMethods.SelectedItem as ReadMethodModel;
			if (selectedMethod != null)
			{
				filterPropsCtl.SetFilterProperties(selectedMethod.FilterProperties, selectedMethod);
				filterPropsCtl.Readonly = false;
				inclNavPropEditCtl.SetInclNavProperties(selectedMethod);
				inclNavPropEditCtl.Readonly = false;
			}
			else
			{
				filterPropsCtl.SetFilterProperties(null, null);
				filterPropsCtl.Readonly = true;
				inclNavPropEditCtl.SetInclNavProperties(null);
				inclNavPropEditCtl.Readonly = true;
			}
		}
	}
}
