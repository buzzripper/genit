using Dyvenix.GenIt.DslPackage.Editors;
using Microsoft.VisualStudio.Modeling;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Dyvenix.GenIt.DslPackage.Editors.Services.Controls
{
	public partial class DtosEditControl : UserControlBase
	{
		private ServiceModel _serviceModel;
		private LinkedElementCollection<DtoModel> _dtoModels;

		public DtosEditControl()
		{
			InitializeComponent();
		}

		public void SetData(
			ServiceModel serviceModel,
			LinkedElementCollection<DtoModel> dtoModels,
			LinkedElementCollection<PropertyModel> properties)
		{
			_suspendUpdates = true;
			try
			{
				_serviceModel = serviceModel;
				_dtoModels = dtoModels;

				RefreshGrid();

				dtoPropertiesCtl.SetProperties(properties);

				if (dtoModels?.Count > 0)
				{
					grdDtos.SelectedIndex = 0;
				}
			}
			finally
			{
				_suspendUpdates = false;
			}

			if (dtoModels?.Count > 0)
			{
				var selectedDto = grdDtos.SelectedItem as DtoModel;
				if (selectedDto != null)
				{
					dtoPropertiesCtl.SetSelectedDto(selectedDto);
					dtoPropertiesCtl.Readonly = false;
				}
			}
		}

		private void btnAdd_Click(object sender, RoutedEventArgs e)
		{
			if (_dtoModels == null || _serviceModel == null)
				return;

			int nextNum = _dtoModels.Count + 1;

			DslTransactionHelper.ExecuteInTransaction(_serviceModel, "Add DTO", () =>
			{
				var newDto = new DtoModel(_serviceModel.Store);
				newDto.Name = $"Dto{nextNum}";
				_dtoModels.Add(newDto);
			});

			RefreshGrid();

			var addedDto = _dtoModels.LastOrDefault();
			if (addedDto != null)
			{
				grdDtos.SelectedItem = addedDto;
			}
		}

		private void btnDelete_Click(object sender, RoutedEventArgs e)
		{
			if (sender is Button btn && btn.DataContext is DtoModel dto && _dtoModels != null)
			{
				var result = MessageBox.Show("Delete this DTO?", "Confirm Delete",
					MessageBoxButton.OKCancel, MessageBoxImage.Question);

				if (result == MessageBoxResult.OK)
				{
					DslTransactionHelper.ExecuteInTransaction(dto, "Delete DTO", () =>
					{
						dto.Delete();
					});
					RefreshGrid();
				}
			}
		}

		private void btnUp_Click(object sender, RoutedEventArgs e)
		{
			if (grdDtos.SelectedItem is DtoModel selectedDto && _dtoModels != null)
			{
				var currentIdx = grdDtos.SelectedIndex;
				if (currentIdx > 0)
				{
					SwapOrder(currentIdx, currentIdx - 1);
				}
			}
		}

		private void btnDown_Click(object sender, RoutedEventArgs e)
		{
			if (grdDtos.SelectedItem is DtoModel selectedDto && _dtoModels != null)
			{
				var currentIdx = grdDtos.SelectedIndex;
				if (currentIdx < grdDtos.Items.Count - 1)
				{
					SwapOrder(currentIdx, currentIdx + 1);
				}
			}
		}

		private void SwapOrder(int srcIdx, int targetIdx)
		{
			if (_dtoModels == null) return;

			var items = _dtoModels.ToList();
			var srcDto = items[srcIdx];

			DslTransactionHelper.ExecuteInTransaction(_serviceModel, "Reorder DTOs", () =>
			{
				_dtoModels.Move(srcIdx, targetIdx);
			});

			_suspendUpdates = true;
			try
			{
				RefreshGrid();
				grdDtos.UpdateLayout();
				grdDtos.SelectedItem = srcDto;
				grdDtos.UpdateLayout();
				grdDtos.ScrollIntoView(srcDto);
				grdDtos.Focus();
			}
			finally
			{
				_suspendUpdates = false;
			}

			UpdateButtonStates();
		}

		private void RefreshGrid()
		{
			if (_dtoModels == null) return;
			grdDtos.ItemsSource = _dtoModels.ToList();
		}

		private void UpdateButtonStates()
		{
			var selectedDto = grdDtos.SelectedItem as DtoModel;
			btnUp.IsEnabled = selectedDto != null && grdDtos.SelectedIndex > 0;
			btnDown.IsEnabled = selectedDto != null && grdDtos.SelectedIndex < grdDtos.Items.Count - 1;
		}

		private void grdDtos_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (_suspendUpdates) return;

			UpdateButtonStates();

			var selectedDto = grdDtos.SelectedItem as DtoModel;
			if (selectedDto != null)
			{
				dtoPropertiesCtl.SetSelectedDto(selectedDto);
				dtoPropertiesCtl.Readonly = false;
			}
			else
			{
				dtoPropertiesCtl.SetSelectedDto(null);
				dtoPropertiesCtl.Readonly = true;
			}
		}
	}
}
