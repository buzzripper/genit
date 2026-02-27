using Dyvenix.GenIt.DslPackage.Editors;
using Microsoft.VisualStudio.Modeling;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Dyvenix.GenIt.DslPackage.Editors.Entity.Controls
{
	public partial class EntityDtosEditControl : UserControlBase
	{
		private EntityModel _entityModel;

		public EntityDtosEditControl()
		{
			InitializeComponent();
			dtoNavPropertiesCtl.NavPropertySelected = OnNavPropertySelected;
		}

		private void OnNavPropertySelected(NavigationProperty navProp)
		{
			navDtosCtl.SetSelectedNavProperty(navProp);
		}

		public void SetData(EntityModel entityModel)
		{
			_suspendUpdates = true;
			try
			{
				grdDtos.ItemsSource = null;
				dtoPropertiesCtl.SetSelectedDto(null);
				dtoNavPropertiesCtl.SetSelectedDto(null);
				navDtosCtl.Clear();

				_entityModel = entityModel;

				RefreshGrid();

				dtoPropertiesCtl.SetProperties(entityModel.Properties);
				dtoNavPropertiesCtl.SetNavProperties(entityModel.NavigationProperties);

				if (entityModel.DtoModels?.Count > 0)
				{
					grdDtos.SelectedIndex = 0;
				}
			}
			finally
			{
				_suspendUpdates = false;
			}

			if (entityModel.DtoModels?.Count > 0)
			{
				var selectedDto = grdDtos.SelectedItem as DtoModel;
				if (selectedDto != null)
				{
					dtoPropertiesCtl.SetSelectedDto(selectedDto);
					dtoPropertiesCtl.Readonly = false;
					dtoNavPropertiesCtl.SetSelectedDto(selectedDto);
					dtoNavPropertiesCtl.Readonly = false;
				}
			}
		}

		private void btnAdd_Click(object sender, RoutedEventArgs e)
		{
			if (_entityModel == null)
				return;

			// DtoModel requires an embedding parent (ServiceModelHasDtoModels).
			// Create it in the first ServiceModel, then reference it from EntityModel.
			var serviceModel = _entityModel.ServiceModels.FirstOrDefault();
			if (serviceModel == null)
			{
				MessageBox.Show("Cannot add a DTO because this entity has no Service Model.\nPlease add a Service Model first.",
					"No Service Model", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			int nextNum = _entityModel.DtoModels.Count + 1;

			DslTransactionHelper.ExecuteInTransaction(_entityModel, "Add DTO", () =>
			{
				var newDto = new DtoModel(_entityModel.Store);
				newDto.Name = $"Dto{nextNum}";
				serviceModel.DtoModels.Add(newDto);
				_entityModel.DtoModels.Add(newDto);
			});

			RefreshGrid();

			var addedDto = _entityModel.DtoModels.LastOrDefault();
			if (addedDto != null)
			{
				grdDtos.SelectedItem = addedDto;
			}
		}

		private void btnDelete_Click(object sender, RoutedEventArgs e)
		{
			if (sender is Button btn && btn.DataContext is DtoModel dto && _entityModel != null)
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
			if (grdDtos.SelectedItem is DtoModel selectedDto && _entityModel != null)
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
			if (grdDtos.SelectedItem is DtoModel selectedDto && _entityModel != null)
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
			if (_entityModel == null) return;

			var items = _entityModel.DtoModels.ToList();
			var srcDto = items[srcIdx];

			DslTransactionHelper.ExecuteInTransaction(_entityModel, "Reorder DTOs", () =>
			{
				_entityModel.DtoModels.Move(srcIdx, targetIdx);
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
			if (_entityModel == null) return;
			grdDtos.ItemsSource = _entityModel.DtoModels.ToList();
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
			if (_entityModel == null || _entityModel.Store == null) return;

			UpdateButtonStates();

			var selectedDto = grdDtos.SelectedItem as DtoModel;
			if (selectedDto != null)
			{
				dtoPropertiesCtl.SetSelectedDto(selectedDto);
				dtoPropertiesCtl.Readonly = false;
				dtoNavPropertiesCtl.SetSelectedDto(selectedDto);
				dtoNavPropertiesCtl.Readonly = false;
			}
			else
			{
				dtoPropertiesCtl.SetSelectedDto(null);
				dtoPropertiesCtl.Readonly = true;
				dtoNavPropertiesCtl.SetSelectedDto(null);
				dtoNavPropertiesCtl.Readonly = true;
			}
			navDtosCtl.Clear();
		}
	}
}
