using Dyvenix.GenIt.DslPackage.Editors.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
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
				txtName.Text = _modelRoot.Name ?? string.Empty;
				chkInclHeader.IsChecked = _modelRoot.InclHeader;
				txtTemplatesFolder.Text = _modelRoot.TemplatesFolder ?? string.Empty;

				// Color buttons
				UpdateColorButton(btnDiagramBackgroundColor, _modelRoot.DiagramBackgroundColor);
				UpdateColorButton(btnAssociationLineColor, _modelRoot.AssociationLineColor);

				// Entities tab
				chkEntitiesEnabled.IsChecked = _modelRoot.EntitiesEnabled;
				txtEntitiesOutputFolder.Text = _modelRoot.EntitiesOutputFolder ?? string.Empty;
				txtEntitiesNamespace.Text = _modelRoot.EntitiesNamespace ?? string.Empty;

				// DbContext tab
				chkDbContextEnabled.IsChecked = _modelRoot.DbContextEnabled;
				txtDbContextOutputFolder.Text = _modelRoot.DbContextOutputFolder ?? string.Empty;
				txtDbContextNamespace.Text = _modelRoot.DbContextNamespace ?? string.Empty;
				dbContextUsingsControl.SetItems(ParseMultilineString(_modelRoot.DbContextUsings));

				// Enums tab
				chkEnumsEnabled.IsChecked = _modelRoot.EnumsEnabled;
				txtEnumsOutputFolder.Text = _modelRoot.EnumsOutputFolder ?? string.Empty;
				txtEnumsNamespace.Text = _modelRoot.EnumsNamespace ?? string.Empty;

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

		#region General Settings Event Handlers

		private void txtName_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			if (_isUpdating || _modelRoot == null)
				return;

			UpdateModelProperty(NamedElement.NameDomainPropertyId, txtName.Text);
		}

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
