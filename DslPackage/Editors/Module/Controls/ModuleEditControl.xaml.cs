using Dyvenix.GenIt.DslPackage.CustomCode;
using System;
using System.Collections.ObjectModel;
using System.IO;
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
		private ObservableCollection<string> _permissions;

		public ModuleEditControl()
		{
			InitializeComponent();
			_permissions = new ObservableCollection<string>();
			lstPermissions.ItemsSource = _permissions;
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

				_permissions.Clear();
				foreach (var permission in _moduleModel.Permissions)
				{
					_permissions.Add(permission);
				}
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

			string solutionRoot = SolutionRootCache.Current;
			
			// Determine initial folder - use solution root if text box is empty
			string initialFolder;
			if (string.IsNullOrWhiteSpace(txtRootFolder.Text))
			{
				initialFolder = solutionRoot;
			}
			else
			{
				initialFolder = GetAbsolutePath(txtRootFolder.Text, solutionRoot);
			}

			using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
			{
				dialog.Description = "Select Root Folder";
				dialog.ShowNewFolderButton = true;

				// Set the initial folder if it exists
				if (!string.IsNullOrEmpty(initialFolder) && Directory.Exists(initialFolder))
				{
					dialog.SelectedPath = initialFolder;
				}
				else if (!string.IsNullOrEmpty(solutionRoot) && Directory.Exists(solutionRoot))
				{
					// Fallback to solution root if initial folder doesn't exist
					dialog.SelectedPath = solutionRoot;
				}

				if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					string selectedPath = dialog.SelectedPath;
					
					// Normalize both paths for comparison
					string normalizedSelected = Path.GetFullPath(selectedPath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
					string normalizedSolutionRoot = string.IsNullOrEmpty(solutionRoot) 
						? string.Empty 
						: Path.GetFullPath(solutionRoot).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

					string resultPath;
					
					// Check if selected path equals solution root
					if (!string.IsNullOrEmpty(normalizedSolutionRoot) && 
					    string.Equals(normalizedSelected, normalizedSolutionRoot, StringComparison.OrdinalIgnoreCase))
					{
						resultPath = ".";
					}
					// Check if selected path is under solution root
					else if (!string.IsNullOrEmpty(normalizedSolutionRoot))
					{
						string solutionRootWithSeparator = normalizedSolutionRoot + Path.DirectorySeparatorChar;
						if (normalizedSelected.StartsWith(solutionRootWithSeparator, StringComparison.OrdinalIgnoreCase))
						{
							// Get the relative part
							resultPath = normalizedSelected.Substring(solutionRootWithSeparator.Length);
						}
						else
						{
							// Outside solution root - use absolute path
							resultPath = selectedPath;
						}
					}
					else
					{
						// No solution root available - use absolute path
						resultPath = selectedPath;
					}

					_isUpdating = true;
					try
					{
						txtRootFolder.Text = resultPath;
					}
					finally
					{
						_isUpdating = false;
					}

					UpdateModelProperty(ModuleModel.RootFolderDomainPropertyId, resultPath);
				}
			}
		}

		private string GetAbsolutePath(string path, string solutionRoot)
		{
			if (string.IsNullOrEmpty(path))
				return solutionRoot;

			if (Path.IsPathRooted(path))
				return path;

			// It's a relative path, combine with solution root
			if (!string.IsNullOrEmpty(solutionRoot))
				return Path.GetFullPath(Path.Combine(solutionRoot, path));

			return path;
		}

		private void btnAddPermission_Click(object sender, RoutedEventArgs e)
		{
			if (_moduleModel == null)
				return;

			var dialog = new AddPermissionDialog();
			dialog.Owner = Window.GetWindow(this);
			if (dialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(dialog.PermissionName))
			{
				_permissions.Add(dialog.PermissionName.Trim());
				SavePermissionsToModel();
			}
		}

		private void btnDeletePermission_Click(object sender, RoutedEventArgs e)
		{
			if (_moduleModel == null || lstPermissions.SelectedItem == null)
				return;

			var selectedPermission = lstPermissions.SelectedItem as string;
			if (selectedPermission != null)
			{
				_permissions.Remove(selectedPermission);
				SavePermissionsToModel();
			}
		}

		private void SavePermissionsToModel()
		{
			if (_moduleModel == null)
				return;

			var permissionsList = new System.Collections.Generic.List<string>(_permissions);

			using (var transaction = _moduleModel.Store.TransactionManager.BeginTransaction("Update Permissions"))
			{
				_moduleModel.Permissions = permissionsList;
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
