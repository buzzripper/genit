using Dyvenix.GenIt.DslPackage.CodeGen;
using Dyvenix.GenIt.DslPackage.CodeGen.Misc;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Diagrams;
using Microsoft.VisualStudio.Modeling.Shell;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace Dyvenix.GenIt
{
	internal partial class GenItCommandSet : GenItCommandSetBase
	{
		private CommandID _generateCodeCommandId = new CommandID(new Guid(Constants.GenItCommandSetId), 0x0100);
		private CommandID _removeFromViewCommandId = new CommandID(new Guid(Constants.GenItCommandSetId), 0x0102);
		private CommandID _validateModelCommandId = new CommandID(new Guid(Constants.GenItCommandSetId), 0x0103);

		/// <summary>
		/// Provide the menu commands that this command set handles
		/// </summary>
		protected override System.Collections.Generic.IList<MenuCommand> GetMenuCommands()
		{
			// Get the base commands
			System.Collections.Generic.IList<MenuCommand> commands = base.GetMenuCommands();

			// Add the Generate Code command
			DynamicStatusMenuCommand generateCodeCommand = new DynamicStatusMenuCommand(
				new EventHandler(OnStatusGenerateCode),
				new EventHandler(OnMenuGenerateCode),
				_generateCodeCommandId);
			commands.Add(generateCodeCommand);

			// Add the "Remove from View" command
			DynamicStatusMenuCommand removeFromViewCommand = new DynamicStatusMenuCommand(
				new EventHandler(OnStatusRemoveFromView),
				new EventHandler(OnMenuRemoveFromView),
				_removeFromViewCommandId);
			commands.Add(removeFromViewCommand);

			// Add the "Validate Model" command
			DynamicStatusMenuCommand validateModelCommand = new DynamicStatusMenuCommand(
				new EventHandler(OnStatusGenerateCode),
				new EventHandler(OnMenuValidateModel),
				_validateModelCommandId);
			commands.Add(validateModelCommand);

			return commands;
		}

		#region Generate Code Command

		/// <summary>
		/// Determines whether the Generate Code menu item should be visible and if so, enabled.
		/// </summary>
		private void OnStatusGenerateCode(object sender, EventArgs args)
		{
			MenuCommand command = sender as MenuCommand;
			if (command == null)
				return;

			command.Visible = true;
			command.Enabled = false;

			// Check if we have a valid document with a ModelRoot
			GenItDocData docData = this.CurrentGenItDocData;
			if (docData != null && docData.Store != null)
			{
				var modelRoots = docData.Store.ElementDirectory.FindElements<ModelRoot>();
				if (modelRoots != null && modelRoots.Count > 0)
				{
					command.Enabled = true;
				}
			}
		}

		/// <summary>
		/// Event handler to generate code from the model
		/// </summary>
		private void OnMenuGenerateCode(object sender, EventArgs args)
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			// Confirmation popup
			int result = VsShellUtilities.ShowMessageBox(
				this.ServiceProvider,
				"Are you sure you want to generate code?",
				"Confirm Code Generation",
				OLEMSGICON.OLEMSGICON_QUERY,
				OLEMSGBUTTON.OLEMSGBUTTON_YESNO,
				OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
			if (result != (int)DialogResult.Yes)
				return;

			try
			{
				OutputHelper.Write("=".PadRight(80, '='));
				OutputHelper.WriteAndActivate("Reading model file...");

				GenItModel model = CreateAndValidateModel();
				if (model == null)
					return;

				OutputHelper.Write("Starting code generation...");

				model.GenerateCode();

				OutputHelper.Write("=".PadRight(80, '='));
				OutputHelper.Write("Code generation completed successfully!");

				VsShellUtilities.ShowMessageBox(
					this.ServiceProvider,
					"Code generation completed successfully.",
					"Code Generation",
					OLEMSGICON.OLEMSGICON_INFO,
					OLEMSGBUTTON.OLEMSGBUTTON_OK,
					OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
			}
			catch (Exception ex)
			{
				VsShellUtilities.ShowMessageBox(
					this.ServiceProvider,
					ex.Message,
					"Code Generation Error",
					OLEMSGICON.OLEMSGICON_CRITICAL,
					OLEMSGBUTTON.OLEMSGBUTTON_OK,
					OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
				OutputHelper.WriteError($"Code generation failed: {ex.Message}");
				OutputHelper.WriteError(ex.StackTrace);
			}
			finally
			{
				OutputHelper.Write("=".PadRight(80, '='));
			}
		}

		#endregion

		#region Validate Model Command

		/// <summary>
		/// Event handler to validate the model without generating code
		/// </summary>
		private void OnMenuValidateModel(object sender, EventArgs args)
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			try
			{
				OutputHelper.Write("=".PadRight(80, '='));
				OutputHelper.WriteAndActivate("Reading model file...");

				GenItModel model = CreateAndValidateModel();
				if (model == null)
					return;

				VsShellUtilities.ShowMessageBox(
					this.ServiceProvider,
					"Model validation passed.",
					"Validate Model",
					OLEMSGICON.OLEMSGICON_INFO,
					OLEMSGBUTTON.OLEMSGBUTTON_OK,
					OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
			}
			catch (Exception ex)
			{
				VsShellUtilities.ShowMessageBox(
					this.ServiceProvider,
					ex.Message,
					"Validation Error",
					OLEMSGICON.OLEMSGICON_CRITICAL,
					OLEMSGBUTTON.OLEMSGBUTTON_OK,
					OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
				OutputHelper.WriteError($"Model validation failed: {ex.Message}");
				OutputHelper.WriteError(ex.StackTrace);
			}
			finally
			{
				OutputHelper.Write("=".PadRight(80, '='));
			}
		}

		#endregion

		#region Shared Helpers

		/// <summary>
		/// Creates a GenItModel from the current document and validates it.
		/// Returns the model if valid, or null if validation fails (with errors shown).
		/// </summary>
		private GenItModel CreateAndValidateModel()
		{
			GenItDocData docData = this.CurrentGenItDocData;
			if (docData == null || docData.Store == null)
			{
				OutputHelper.WriteError("No active document found.");
				return null;
			}

			var modelRoots = docData.Store.ElementDirectory.FindElements<ModelRoot>();
			if (modelRoots == null || modelRoots.Count == 0)
			{
				OutputHelper.WriteError("No model root found in the document.");
				return null;
			}

			ModelRoot modelRoot = modelRoots[0];

			GenItModel model = new GenItModel(modelRoot);
			OutputHelper.Write("Validating model...");
			if (!model.Validate(out var errors))
			{
				OutputHelper.WriteError("Model validation failed with the following errors:");
				foreach (var error in errors)
					OutputHelper.Write($"\u2022  {error}");
				OutputHelper.ShowOutputToolWindow();
				VsShellUtilities.ShowMessageBox(
					this.ServiceProvider,
					"Model validation failed. Check the output window for details.",
					"Validation Error",
					OLEMSGICON.OLEMSGICON_CRITICAL,
					OLEMSGBUTTON.OLEMSGBUTTON_OK,
					OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
				return null;
			}
			OutputHelper.Write("Model validated.");

			return model;
		}

		#endregion

		#region Remove from View Command

		/// <summary>
		/// Determines whether the "Remove from View" menu item should be visible and enabled.
		/// Only visible when right-clicking on an EntityModel or EnumModel shape.
		/// </summary>
		private void OnStatusRemoveFromView(object sender, EventArgs args)
		{
			MenuCommand command = sender as MenuCommand;
			if (command == null)
				return;

			command.Visible = false;
			command.Enabled = false;

			// Check if we have a valid selection
			var selection = this.CurrentSelection;
			if (selection == null || selection.Count == 0)
				return;

			// Check each selected object for valid shape types
			foreach (object selectedObject in selection)
			{
				if (selectedObject is ClassShape classShape && classShape.ModelElement is EntityModel)
				{
					command.Visible = true;
					command.Enabled = true;
					return;
				}

				if (selectedObject is EnumShape enumShape && enumShape.ModelElement is EnumModel)
				{
					command.Visible = true;
					command.Enabled = true;
					return;
				}
			}
		}

		/// <summary>
		/// Event handler to remove the selected shape from the current view.
		/// Does not delete the underlying model element.
		/// </summary>
		private void OnMenuRemoveFromView(object sender, EventArgs args)
		{
			try
			{
				var selection = this.CurrentSelection;
				if (selection == null || selection.Count == 0)
					return;

				// Collect all shapes to remove
				var shapesToRemove = new System.Collections.Generic.List<NodeShape>();
				string elementName = null;

				foreach (object selectedObject in selection)
				{
					NodeShape shapeToRemove = null;

					if (selectedObject is ClassShape classShape && classShape.ModelElement is EntityModel entityModel)
					{
						shapeToRemove = classShape;
						elementName = entityModel.Name;
					}
					else if (selectedObject is EnumShape enumShape && enumShape.ModelElement is EnumModel enumModel)
					{
						shapeToRemove = enumShape;
						elementName = enumModel.Name;
					}

					if (shapeToRemove != null && !shapesToRemove.Contains(shapeToRemove))
					{
						shapesToRemove.Add(shapeToRemove);
					}
				}

				if (shapesToRemove.Count == 0)
					return;

				// Show confirmation dialog
				string message = shapesToRemove.Count == 1
					? $"Remove '{elementName}' from this view?\n\nThe element will remain in the model and can be added back to this view later."
					: $"Remove {shapesToRemove.Count} elements from this view?\n\nThe elements will remain in the model and can be added back to this view later.";

				DialogResult result = MessageBox.Show(
					message,
					"Remove from View",
					MessageBoxButtons.YesNo,
					MessageBoxIcon.Question,
					MessageBoxDefaultButton.Button2);

				if (result != DialogResult.Yes)
					return;

				// Get the document data to mark it dirty
				GenItDocData docData = this.CurrentGenItDocData;

				// Remove shapes within a transaction
				using (Transaction transaction = shapesToRemove[0].Store.TransactionManager.BeginTransaction("Remove from View"))
				{
					foreach (var shape in shapesToRemove)
					{
						// Also remove any connectors connected to this shape
						var connectorsToRemove = new System.Collections.Generic.List<LinkShape>();
						foreach (LinkShape ls in shape.FromRoleLinkShapes)
						{
							connectorsToRemove.Add(ls);
						}
						foreach (LinkShape ls in shape.ToRoleLinkShapes)
						{
							connectorsToRemove.Add(ls);
						}
						foreach (var connector in connectorsToRemove)
						{
							if (!connector.IsDeleted && !connector.IsDeleting)
							{
								connector.Delete();
							}
						}

						// Delete only the shape, not the model element
						shape.Delete();
					}

					transaction.Commit();
				}

				// Mark the document as changed so it will be saved
				if (docData != null)
				{
					docData.MarkDocumentChangedForBackup();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(
					$"Error removing element from view: {ex.Message}",
					"Remove from View Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);
			}
		}

		#endregion
	}
}
