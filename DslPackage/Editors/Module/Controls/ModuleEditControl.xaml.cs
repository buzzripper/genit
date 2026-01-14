using Dyvenix.GenIt.DslPackage.Editors.Helpers;
using System;
using System.Windows;

namespace Dyvenix.GenIt.DslPackage.Editors.Module.Controls
{
	/// <summary>
	/// Editor control for ModuleModel properties.
	/// Displays when a ModuleModel is selected in the diagram.
	/// </summary>
	public partial class ModuleEditControl : UserControlBase
	{
		private ModuleModel _moduleModel;
		private bool _isUpdating;

		public ModuleEditControl()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Initializes the control with the specified ModuleModel.
		/// </summary>
		/// <param name="moduleModel">The ModuleModel to edit.</param>
		public void Initialize(ModuleModel moduleModel)
		{
			_moduleModel = moduleModel;
			LoadFromModel();
		}

		private void LoadFromModel()
		{
			if (_moduleModel == null)
				return;

			_isUpdating = true;
			try
			{
				txtName.Text = _moduleModel.Name ?? string.Empty;
				txtNamespace.Text = _moduleModel.Namespace ?? string.Empty;
				txtRootFolder.Text = _moduleModel.RootFolder ?? string.Empty;

				permissionsControl.SetItems(_moduleModel.Permissions);
			}
			finally
			{
				_isUpdating = false;
			}
		}

		private void txtName_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			if (_isUpdating || _moduleModel == null)
				return;

			UpdateModelProperty(NamedElement.NameDomainPropertyId, txtName.Text);
		}

		private void txtNamespace_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			if (_isUpdating || _moduleModel == null)
				return;

			UpdateModelProperty(ModuleModel.NamespaceDomainPropertyId, txtNamespace.Text);
		}

		private void txtRootFolder_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			if (_isUpdating || _moduleModel == null)
				return;

			UpdateModelProperty(ModuleModel.RootFolderDomainPropertyId, txtRootFolder.Text);
		}

		private void btnBrowseFolder_Click(object sender, RoutedEventArgs e)
		{
			if (_moduleModel == null)
				return;

			if (FolderBrowserHelper.BrowseForFolder(txtRootFolder.Text, "Select Root Folder", out string selectedPath))
			{
				_isUpdating = true;
				try
				{
					txtRootFolder.Text = selectedPath;
				}
				finally
				{
					_isUpdating = false;
				}

				UpdateModelProperty(ModuleModel.RootFolderDomainPropertyId, selectedPath);
			}
		}

		private void permissionsControl_ItemsChanged(object sender, EventArgs e)
		{
			if (_isUpdating || _moduleModel == null)
				return;

			using (var transaction = _moduleModel.Store.TransactionManager.BeginTransaction("Update Permissions"))
			{
				_moduleModel.Permissions = permissionsControl.Items;
				transaction.Commit();
			}
		}

		private void UpdateModelProperty(System.Guid propertyId, object value)
		{
			if (_moduleModel == null)
				return;

			var property = _moduleModel.Store.DomainDataDirectory.GetDomainProperty(propertyId);
			if (property == null)
				return;

			using (var transaction = _moduleModel.Store.TransactionManager.BeginTransaction("Update " + property.Name))
			{
				property.SetValue(_moduleModel, value);
				transaction.Commit();
			}
		}
	}
}
