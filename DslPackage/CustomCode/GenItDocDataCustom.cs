using Dyvenix.GenIt.DslPackage.CodeGen.Misc;
using Dyvenix.GenIt.DslPackage.Editors.Permissions;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.IO;
using System.Windows;

namespace Dyvenix.GenIt
{
	/// <summary>
	/// Customizations for GenItDocData to handle extdata file lifecycle.
	/// </summary>
	internal partial class GenItDocData
	{
		private string _currentExtDataFilePath;

		/// <summary>
		/// Override Load to load the extdata file along with the model.
		/// </summary>
		protected override void Load(string fileName, bool isReload)
		{
			// First, check and load the extdata file
			string extDataFilePath = PermissionsExtData.GetExtDataFilePath(fileName);
			bool createdNewFile = false;

			if (!File.Exists(extDataFilePath))
			{
				// Create empty extdata file if it doesn't exist
				try
				{
					PermissionsExtData.CreateEmpty(extDataFilePath);
					createdNewFile = true;
				}
				catch (Exception ex)
				{
					MessageBox.Show(
						$"Failed to create permissions extdata file:\n{ex.Message}\n\nFile: {extDataFilePath}",
						"GenIt - ExtData Error",
						MessageBoxButton.OK,
						MessageBoxImage.Error);
					throw new InvalidOperationException($"Cannot create extdata file: {extDataFilePath}", ex);
				}
			}

			// Try to load permissions
			try
			{
				PermissionsManager.Current.Load(fileName);
				_currentExtDataFilePath = extDataFilePath;
			}
			catch (PermissionsExtDataException ex)
			{
				MessageBox.Show(
					$"Failed to load permissions extdata file:\n{ex.Message}\n\nThe model cannot be opened until this is fixed.",
					"GenIt - ExtData Error",
					MessageBoxButton.OK,
					MessageBoxImage.Error);
				throw new InvalidOperationException(ex.Message, ex);
			}

			// Store flag for later - we'll add the file in OnDocumentLoaded
			_extDataFileJustCreated = createdNewFile;

			// Now load the model
			base.Load(fileName, isReload);

			// After model loads, validate and sync permissions
			ValidateAndSyncPermissions();
		}

		private bool _extDataFileJustCreated;

		/// <summary>
		/// Called after the document is fully loaded.
		/// </summary>
		protected override void OnDocumentLoaded(EventArgs e)
		{
			base.OnDocumentLoaded(e);

			// Add extdata file to project with nesting if we just created it
			if (_extDataFileJustCreated && !string.IsNullOrEmpty(_currentExtDataFilePath))
			{
				_extDataFileJustCreated = false;
				AddExtDataFileToProject(this.FileName, _currentExtDataFilePath);
			}
		}

		/// <summary>
		/// Override OnDocumentSaved to handle file rename for extdata.
		/// </summary>
		protected override void OnDocumentSaved(EventArgs e)
		{
			base.OnDocumentSaved(e);

			// Handle extdata file rename if the model was saved to a new location
			var savedEventArgs = e as Microsoft.VisualStudio.Modeling.Shell.DocumentSavedEventArgs;
			if (savedEventArgs != null)
			{
				HandleExtDataFileRename(savedEventArgs.OldFileName, savedEventArgs.NewFileName);
			}
		}

		/// <summary>
		/// Handles renaming/copying the extdata file when the model is saved to a new location.
		/// </summary>
		private void HandleExtDataFileRename(string oldFileName, string newFileName)
		{
			if (string.IsNullOrEmpty(oldFileName) || string.IsNullOrEmpty(newFileName))
				return;

			if (string.Equals(oldFileName, newFileName, StringComparison.OrdinalIgnoreCase))
				return;

			string oldExtDataPath = PermissionsExtData.GetExtDataFilePath(oldFileName);
			string newExtDataPath = PermissionsExtData.GetExtDataFilePath(newFileName);

			try
			{
				if (File.Exists(oldExtDataPath))
				{
					// Copy to new location (don't move, in case of SaveAs)
					File.Copy(oldExtDataPath, newExtDataPath, overwrite: true);
				}
				else
				{
					// Create empty if old doesn't exist
					PermissionsExtData.CreateEmpty(newExtDataPath);
				}

				// Reload permissions from new location
				PermissionsManager.Current.Load(newFileName);
				_currentExtDataFilePath = newExtDataPath;
			}
			catch (Exception ex)
			{
				MessageBox.Show(
					$"Warning: Failed to handle extdata file rename:\n{ex.Message}",
					"GenIt - ExtData Warning",
					MessageBoxButton.OK,
					MessageBoxImage.Warning);
			}
		}

		/// <summary>
		/// Override Dispose to unload permissions.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				PermissionsManager.Current.Unload();
			}
			base.Dispose(disposing);
		}

		/// <summary>
		/// Adds the extdata file to the project as a nested item under the gmdl file.
		/// </summary>
		private void AddExtDataFileToProject(string gmdlFilePath, string extDataFilePath)
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			try
			{
				// Use the Hierarchy from the DocData to add the nested file
				if (this.Hierarchy == null)
					return;

				// Get the project
				var project = this.Hierarchy as IVsProject;
				if (project == null)
					return;

				// Find the parent item (gmdl file)
				uint parentItemId = this.ItemId;

				// Add the extdata file as a nested item
				var result = new VSADDRESULT[1];
				var files = new string[] { extDataFilePath };

				int hr = project.AddItem(
					parentItemId,
					VSADDITEMOPERATION.VSADDITEMOP_LINKTOFILE,
					Path.GetFileName(extDataFilePath),
					1,
					files,
					IntPtr.Zero,
					result);

				if (hr < 0 || result[0] != VSADDRESULT.ADDRESULT_Success)
				{
					// Try alternative: use DTE to add and set DependentUpon
					AddExtDataFileViaDTE(gmdlFilePath, extDataFilePath);
				}
			}
			catch (Exception ex)
			{
				// Don't fail the load if we can't add to project - just log
				System.Diagnostics.Debug.WriteLine($"Failed to add extdata file to project: {ex.Message}");
			}
		}

		/// <summary>
		/// Alternative method to add extdata file using DTE.
		/// </summary>
		private void AddExtDataFileViaDTE(string gmdlFilePath, string extDataFilePath)
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			try
			{
				var dte = this.ServiceProvider.GetService(typeof(DTE)) as DTE;
				if (dte == null)
					return;

				// Find the project item for the gmdl file
				ProjectItem gmdlItem = dte.Solution.FindProjectItem(gmdlFilePath);
				if (gmdlItem == null)
					return;

				// Check if extdata is already in the project
				bool found = false;
				try
				{
					foreach (ProjectItem child in gmdlItem.ProjectItems)
					{
						if (string.Equals(child.Name, Path.GetFileName(extDataFilePath), StringComparison.OrdinalIgnoreCase))
						{
							found = true;
							break;
						}
					}
				}
				catch
				{
					// ProjectItems might throw if empty
				}

				if (!found)
				{
					// Add the extdata file as a nested item under the gmdl file
					gmdlItem.ProjectItems.AddFromFile(extDataFilePath);
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Failed to add extdata file via DTE: {ex.Message}");
			}
		}

		/// <summary>
		/// Validates permissions on method models and syncs the Permissions EnumModel.
		/// </summary>
		private void ValidateAndSyncPermissions()
		{
			var modelRoot = this.RootElement as ModelRoot;
			if (modelRoot == null)
				return;

			using (var transaction = modelRoot.Store.TransactionManager.BeginTransaction("Sync Permissions"))
			{
				bool modified = false;

				// Validate and clean orphaned permissions on method models
				modified |= ValidateMethodPermissions(modelRoot);

				// Sync the Permissions EnumModel from extdata
				modified |= SyncPermissionsEnumModel(modelRoot);

				if (modified)
				{
					transaction.Commit();
				}
				else
				{
					transaction.Rollback();
				}
			}
		}

		/// <summary>
		/// Validates permissions on all ReadMethodModel and UpdateMethodModel instances,
		/// removing any that no longer exist in the extdata file.
		/// </summary>
		private bool ValidateMethodPermissions(ModelRoot modelRoot)
		{
			bool modified = false;
			var orphanedPermissions = new System.Collections.Generic.List<string>();

			foreach (var modelType in modelRoot.Types)
			{
				var entityModel = modelType as EntityModel;
				if (entityModel == null)
					continue;

				foreach (var serviceModel in entityModel.ServiceModels)
				{
					// Validate ReadMethodModels
					foreach (var readMethod in serviceModel.ReadMethods)
					{
						var cleanedPerms = PermissionsManager.Current.ValidateAndCleanPermissions(
							readMethod.Permissions, out var removed);

						if (removed.Count > 0)
						{
							orphanedPermissions.AddRange(removed);
							readMethod.Permissions = cleanedPerms;
							modified = true;
						}
					}

					// Validate UpdateMethodModels
					foreach (var updateMethod in serviceModel.UpdateMethods)
					{
						var cleanedPerms = PermissionsManager.Current.ValidateAndCleanPermissions(
							updateMethod.Permissions, out var removed);

						if (removed.Count > 0)
						{
							orphanedPermissions.AddRange(removed);
							updateMethod.Permissions = cleanedPerms;
							modified = true;
						}
					}
				}
			}

			if (orphanedPermissions.Count > 0)
			{
				var uniqueOrphans = new System.Collections.Generic.HashSet<string>(orphanedPermissions);
				OutputHelper.WriteWarning($"Removed {orphanedPermissions.Count} orphaned permission reference(s): {string.Join(", ", uniqueOrphans)}");
			}

			return modified;
		}

		/// <summary>
		/// Syncs the Permissions EnumModel from the extdata file.
		/// Creates the EnumModel if it doesn't exist, replaces all members with extdata contents.
		/// </summary>
		private bool SyncPermissionsEnumModel(ModelRoot modelRoot)
		{
			bool modified = false;

			// Find or create the Permissions EnumModel
			EnumModel permissionsEnum = null;
			foreach (var modelType in modelRoot.Types)
			{
				var enumModel = modelType as EnumModel;
				if (enumModel != null && string.Equals(enumModel.Name, "Permissions", StringComparison.OrdinalIgnoreCase))
				{
					permissionsEnum = enumModel;
					break;
				}
			}

			if (permissionsEnum == null)
			{
				// Create new Permissions enum
				permissionsEnum = new EnumModel(modelRoot.Store);
				permissionsEnum.Name = "Permissions";
				modelRoot.Types.Add(permissionsEnum);
				modified = true;
			}

			// Clear existing members
			while (permissionsEnum.Members.Count > 0)
			{
				permissionsEnum.Members[0].Delete();
				modified = true;
			}

			// Add members from extdata
			foreach (var permission in PermissionsManager.Current.Permissions)
			{
				var member = new EnumMember(modelRoot.Store);
				member.Name = permission.Name;
				member.Value = permission.Value.ToString();
				permissionsEnum.Members.Add(member);
				modified = true;
			}

			return modified;
		}
	}
}
