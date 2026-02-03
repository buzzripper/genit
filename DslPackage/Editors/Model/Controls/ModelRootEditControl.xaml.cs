using Dyvenix.GenIt.DslPackage.Editors.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Dyvenix.GenIt.DslPackage.Editors.Model.Controls
{
	/// <summary>
	/// Editor control for ModelRoot properties.
	/// Displays when the diagram surface is clicked.
	/// </summary>
	public partial class ModelRootEditControl : UserControlBase
	{
		private ModelRoot _modelRoot;
		private bool _isUpdating;
		private ObservableCollection<PermissionModel> _permissions;
		private ObservableCollection<EditableString> _usings;

		public ModelRootEditControl()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Initializes the control with the specified ModelRoot.
		/// </summary>
		/// <param name="modelRoot">The ModelRoot to edit.</param>
		public void Initialize(ModelRoot modelRoot)
		{
			_modelRoot = modelRoot;
			LoadFromModel();
		}

		private void LoadFromModel()
		{
			if (_modelRoot == null)
				return;

			_isUpdating = true;
			try
			{
				// General settings
				chkInclHeader.IsChecked = _modelRoot.InclHeader;
				txtTemplatesFolder.Text = _modelRoot.TemplatesFolder ?? string.Empty;
				txtCommonNamespace.Text = _modelRoot.CommonNamespace ?? string.Empty;

				// Color buttons
				UpdateColorButton(btnDiagramBackgroundColor, _modelRoot.DiagramBackgroundColor);
				UpdateColorButton(btnAssociationLineColor, _modelRoot.AssociationLineColor);
				_usings = new ObservableCollection<EditableString>(ParseMultilineString(_modelRoot.Usings).Select(u => new EditableString(u)));
				grdModelUsings.ItemsSource = _usings;

				// Entities tab
				chkEntitiesEnabled.IsChecked = _modelRoot.EntitiesEnabled;
				txtEntitiesOutputFolder.Text = _modelRoot.EntitiesOutputFolder ?? string.Empty;
				txtEntitiesNamespace.Text = _modelRoot.EntitiesNamespace ?? string.Empty;

			// DbContext tab
			chkDbContextEnabled.IsChecked = _modelRoot.DbContextEnabled;
			txtDbContextName.Text = _modelRoot.DbContextName ?? string.Empty;
			txtDbContextOutputFolder.Text = _modelRoot.DbContextOutputFolder ?? string.Empty;
			txtDbContextNamespace.Text = _modelRoot.DbContextNamespace ?? string.Empty;
			dbContextUsingsControl.SetItems(ParseMultilineString(_modelRoot.DbContextUsings));

				// Enums tab
				chkEnumsEnabled.IsChecked = _modelRoot.EnumsEnabled;
				txtEnumsOutputFolder.Text = _modelRoot.EnumsOutputFolder ?? string.Empty;
			txtEnumsNamespace.Text = _modelRoot.EnumsNamespace ?? string.Empty;


				// Permissions tab
				_permissions = new ObservableCollection<PermissionModel>(ParsePermissions(_modelRoot.Permissions));
				grdPermissions.ItemsSource = _permissions;

				// Restore splitter position
				if (_modelRoot.EditorSplitterPosition > 0)
				{
				leftColumn.Width = new GridLength(_modelRoot.EditorSplitterPosition, GridUnitType.Pixel);
				}
				}
				finally
			{
				_isUpdating = false;
			}
		}

		private void UpdateColorButton(System.Windows.Controls.Button button, System.Drawing.Color color)
		{
			button.Background = new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
		}

		private IEnumerable<string> ParseMultilineString(string value)
		{
			if (string.IsNullOrEmpty(value))
				return Enumerable.Empty<string>();

			return value.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
						.Select(s => s.Trim())
						.Where(s => !string.IsNullOrEmpty(s));
		}

		private string JoinToMultilineString(IEnumerable<string> items)
		{
			if (items == null)
				return string.Empty;

			return string.Join("\n", items);
		}

		private void btnAddUsing_Click(object sender, RoutedEventArgs e)
		{
			if (_isUpdating || _modelRoot == null || _usings == null)
				return;

			var newItem = new EditableString($"Using{_usings.Count + 1}");
			_usings.Add(newItem);
			SaveUsings();
			grdModelUsings.SelectedItem = newItem;
			grdModelUsings.ScrollIntoView(newItem);
			grdModelUsings.Dispatcher.BeginInvoke(new Action(() =>
			{
				grdModelUsings.Focus();
				grdModelUsings.CurrentCell = new DataGridCellInfo(newItem, grdModelUsings.Columns[0]);
				grdModelUsings.BeginEdit();
			}), System.Windows.Threading.DispatcherPriority.Background);
		}

		private void btnDeleteUsing_Click(object sender, RoutedEventArgs e)
		{
			if (_isUpdating || _modelRoot == null || _usings == null)
				return;

			if (grdModelUsings.SelectedItem is EditableString selected)
			{
				_usings.Remove(selected);
				SaveUsings();
			}
		}

		private void grdUsings_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
		{
			if (_isUpdating || _modelRoot == null)
				return;

			Dispatcher.BeginInvoke(new Action(() => SaveUsings()), System.Windows.Threading.DispatcherPriority.Background);
		}

		private void grdUsings_GotKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
		{
			if (_isUpdating)
				return;

			if (e.OriginalSource is DataGridCell cell && !cell.IsEditing)
			{
				grdModelUsings.Dispatcher.BeginInvoke(new Action(() =>
				{
					if (!cell.IsEditing)
						grdModelUsings.BeginEdit();
				}), System.Windows.Threading.DispatcherPriority.Background);
			}
		}

		private void SaveUsings()
		{
			if (_isUpdating || _modelRoot == null || _usings == null)
				return;

			var value = JoinToMultilineString(_usings.Select(u => u.Value).Where(v => !string.IsNullOrWhiteSpace(v)).Select(v => v.Trim()));
			UpdateModelProperty(ModelRoot.UsingsDomainPropertyId, value);
		}

		#region General Settings Event Handlers

		private void chkInclHeader_Changed(object sender, RoutedEventArgs e)
		{
			if (_isUpdating || _modelRoot == null)
				return;

			UpdateModelProperty(ModelRoot.InclHeaderDomainPropertyId, chkInclHeader.IsChecked ?? false);
		}

		private void txtTemplatesFolder_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			if (_isUpdating || _modelRoot == null)
				return;

			UpdateModelProperty(ModelRoot.TemplatesFolderDomainPropertyId, txtTemplatesFolder.Text);
		}

		private void txtCommonNamespace_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			if (_isUpdating || _modelRoot == null)
				return;

			UpdateModelProperty(ModelRoot.CommonNamespaceDomainPropertyId, txtCommonNamespace.Text);
		}

		private void btnBrowseTemplates_Click(object sender, RoutedEventArgs e)
		{
			if (_modelRoot == null)
				return;

			if (FolderBrowserHelper.BrowseForFolder(txtTemplatesFolder.Text, "Select Templates Folder", out string selectedPath))
			{
				_isUpdating = true;
				try
				{
					txtTemplatesFolder.Text = selectedPath;
				}
				finally
				{
					_isUpdating = false;
				}

				UpdateModelProperty(ModelRoot.TemplatesFolderDomainPropertyId, selectedPath);
			}
		}

		private void btnDiagramBackgroundColor_Click(object sender, RoutedEventArgs e)
		{
			if (_modelRoot == null)
				return;

			var currentColor = _modelRoot.DiagramBackgroundColor;
			using (var dialog = new System.Windows.Forms.ColorDialog())
			{
				dialog.Color = currentColor;
				dialog.FullOpen = true;

				if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					UpdateColorButton(btnDiagramBackgroundColor, dialog.Color);
					UpdateModelProperty(ModelRoot.DiagramBackgroundColorDomainPropertyId, dialog.Color);
				}
			}
		}

		private void btnAssociationLineColor_Click(object sender, RoutedEventArgs e)
		{
			if (_modelRoot == null)
				return;

			var currentColor = _modelRoot.AssociationLineColor;
			using (var dialog = new System.Windows.Forms.ColorDialog())
			{
				dialog.Color = currentColor;
				dialog.FullOpen = true;

				if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					UpdateColorButton(btnAssociationLineColor, dialog.Color);
					UpdateModelProperty(ModelRoot.AssociationLineColorDomainPropertyId, dialog.Color);
				}
			}
		}

		#endregion


		#region Entities Tab Event Handlers

		private void chkEntitiesEnabled_Changed(object sender, RoutedEventArgs e)
		{
			if (_isUpdating || _modelRoot == null)
				return;

			UpdateModelProperty(ModelRoot.EntitiesEnabledDomainPropertyId, chkEntitiesEnabled.IsChecked ?? false);
		}

		private void txtEntitiesOutputFolder_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			if (_isUpdating || _modelRoot == null)
				return;

			UpdateModelProperty(ModelRoot.EntitiesOutputFolderDomainPropertyId, txtEntitiesOutputFolder.Text);
		}

		private void btnBrowseEntitiesFolder_Click(object sender, RoutedEventArgs e)
		{
			if (_modelRoot == null)
				return;

			if (FolderBrowserHelper.BrowseForFolder(txtEntitiesOutputFolder.Text, "Select Entities Output Folder", out string selectedPath))
			{
				_isUpdating = true;
				try
				{
					txtEntitiesOutputFolder.Text = selectedPath;
				}
				finally
				{
					_isUpdating = false;
				}

				UpdateModelProperty(ModelRoot.EntitiesOutputFolderDomainPropertyId, selectedPath);
			}
		}

		private void txtEntitiesNamespace_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			if (_isUpdating || _modelRoot == null)
				return;

			UpdateModelProperty(ModelRoot.EntitiesNamespaceDomainPropertyId, txtEntitiesNamespace.Text);
		}

		#endregion

		#region DbContext Tab Event Handlers

		private void chkDbContextEnabled_Changed(object sender, RoutedEventArgs e)
		{
			if (_isUpdating || _modelRoot == null)
				return;

			UpdateModelProperty(ModelRoot.DbContextEnabledDomainPropertyId, chkDbContextEnabled.IsChecked ?? false);
		}

		private void txtDbContextName_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			if (_isUpdating || _modelRoot == null)
				return;

			UpdateModelProperty(ModelRoot.DbContextNameDomainPropertyId, txtDbContextName.Text);
		}

		private void txtDbContextOutputFolder_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			if (_isUpdating || _modelRoot == null)
				return;

			UpdateModelProperty(ModelRoot.DbContextOutputFolderDomainPropertyId, txtDbContextOutputFolder.Text);
		}

		private void btnBrowseDbContextFolder_Click(object sender, RoutedEventArgs e)
		{
			if (_modelRoot == null)
				return;

			if (FolderBrowserHelper.BrowseForFolder(txtDbContextOutputFolder.Text, "Select DbContext Output Folder", out string selectedPath))
			{
				_isUpdating = true;
				try
				{
					txtDbContextOutputFolder.Text = selectedPath;
				}
				finally
				{
					_isUpdating = false;
				}

				UpdateModelProperty(ModelRoot.DbContextOutputFolderDomainPropertyId, selectedPath);
			}
		}

		private void txtDbContextNamespace_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			if (_isUpdating || _modelRoot == null)
				return;

			UpdateModelProperty(ModelRoot.DbContextNamespaceDomainPropertyId, txtDbContextNamespace.Text);
		}

		private void dbContextUsingsControl_ItemsChanged(object sender, EventArgs e)
		{
			if (_isUpdating || _modelRoot == null)
				return;

			var usingsValue = JoinToMultilineString(dbContextUsingsControl.Items);
			UpdateModelProperty(ModelRoot.DbContextUsingsDomainPropertyId, usingsValue);
		}

		#endregion

		#region Enums Tab Event Handlers

		private void chkEnumsEnabled_Changed(object sender, RoutedEventArgs e)
		{
			if (_isUpdating || _modelRoot == null)
				return;

			UpdateModelProperty(ModelRoot.EnumsEnabledDomainPropertyId, chkEnumsEnabled.IsChecked ?? false);
		}

		private void txtEnumsOutputFolder_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			if (_isUpdating || _modelRoot == null)
				return;

			UpdateModelProperty(ModelRoot.EnumsOutputFolderDomainPropertyId, txtEnumsOutputFolder.Text);
		}

		private void btnBrowseEnumsFolder_Click(object sender, RoutedEventArgs e)
		{
			if (_modelRoot == null)
				return;

			if (FolderBrowserHelper.BrowseForFolder(txtEnumsOutputFolder.Text, "Select Enums Output Folder", out string selectedPath))
			{
				_isUpdating = true;
				try
				{
					txtEnumsOutputFolder.Text = selectedPath;
				}
				finally
				{
					_isUpdating = false;
				}

				UpdateModelProperty(ModelRoot.EnumsOutputFolderDomainPropertyId, selectedPath);
			}
		}

		private void txtEnumsNamespace_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			if (_isUpdating || _modelRoot == null)
				return;

			UpdateModelProperty(ModelRoot.EnumsNamespaceDomainPropertyId, txtEnumsNamespace.Text);
		}

		#endregion

		#region Permissions Tab Event Handlers

		private void btnAddPermission_Click(object sender, RoutedEventArgs e)
		{
			if (_modelRoot == null || _permissions == null)
				return;

			var newPermission = new PermissionModel($"Permission{_permissions.Count + 1}", string.Empty);
			_permissions.Add(newPermission);
			SavePermissions();
			grdPermissions.SelectedItem = newPermission;
		}

		private void btnDeletePermission_Click(object sender, RoutedEventArgs e)
		{
			if (_modelRoot == null || _permissions == null || grdPermissions.SelectedItem == null)
				return;

			var selectedPermission = grdPermissions.SelectedItem as PermissionModel;
			if (selectedPermission != null)
			{
				_permissions.Remove(selectedPermission);
				SavePermissions();
			}
		}

		private void grdPermissions_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
		{
			if (_isUpdating || _modelRoot == null)
				return;

			// Use Dispatcher to save after the edit is committed
			Dispatcher.BeginInvoke(new Action(() => SavePermissions()), System.Windows.Threading.DispatcherPriority.Background);
		}

		private void SavePermissions()
		{
			if (_modelRoot == null || _permissions == null)
				return;

			var permissionsValue = SerializePermissions(_permissions);
			UpdateModelProperty(ModelRoot.PermissionsDomainPropertyId, permissionsValue);
		}

		private IEnumerable<PermissionModel> ParsePermissions(string value)
		{
			if (string.IsNullOrEmpty(value))
				return Enumerable.Empty<PermissionModel>();

			return value.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
						.Select(s => s.Trim())
						.Where(s => !string.IsNullOrEmpty(s))
						.Select(s => PermissionModel.Deserialize(s));
		}

		private string SerializePermissions(IEnumerable<PermissionModel> permissions)
		{
			if (permissions == null)
				return string.Empty;

			return string.Join("\n", permissions.Select(p => p.Serialize()));
		}

		#endregion

		private void gridSplitter_DragCompleted(object sender, DragCompletedEventArgs e)
		{
			if (_modelRoot == null)
				return;

			UpdateModelProperty(ModelRoot.EditorSplitterPositionDomainPropertyId, leftColumn.ActualWidth);
		}

		private void UpdateModelProperty(System.Guid propertyId, object value)
		{
			if (_modelRoot == null)
				return;

			var property = _modelRoot.Store.DomainDataDirectory.GetDomainProperty(propertyId);
			if (property == null)
				return;

			using (var transaction = _modelRoot.Store.TransactionManager.BeginTransaction("Update " + property.Name))
			{
				property.SetValue(_modelRoot, value);
				transaction.Commit();
			}
		}
	}
}
