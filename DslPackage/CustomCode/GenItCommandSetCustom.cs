using Dyvenix.GenIt.DslPackage.CodeGen;
using Dyvenix.GenIt.DslPackage.CodeGen.Misc;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Diagrams;
using Microsoft.VisualStudio.Modeling.Shell;
using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;
using System.Linq;
using System.Windows.Forms;

namespace Dyvenix.GenIt
{
	internal partial class GenItCommandSet : GenItCommandSetBase
	{
		private CommandID _generateCodeCommandId = new CommandID(new Guid(Constants.GenItCommandSetId), 0x0100);
		private CommandID _addServiceCommandId = new CommandID(new Guid(Constants.GenItCommandSetId), 0x0101);

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

			// Add the "Add New Service" command
			DynamicStatusMenuCommand addServiceCommand = new DynamicStatusMenuCommand(
				new EventHandler(OnStatusAddNewService),
				new EventHandler(OnMenuAddNewService),
				_addServiceCommandId);
			commands.Add(addServiceCommand);

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

			command.Visible = false;
			command.Enabled = false;

			// Check if we have a valid document with a ModelRoot
			GenItDocData docData = this.CurrentGenItDocData;
			if (docData != null && docData.Store != null)
			{
				var modelRoots = docData.Store.ElementDirectory.FindElements<ModelRoot>();
				if (modelRoots != null && modelRoots.Count > 0)
				{
					command.Visible = true;
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

			try
			{
				OutputHelper.WriteAndActivate("Reading model file...");

				GenItDocData docData = this.CurrentGenItDocData;
				if (docData == null || docData.Store == null)
				{
					OutputHelper.WriteError("No active document found.");
					return;
				}

				var modelRoots = docData.Store.ElementDirectory.FindElements<ModelRoot>();
				if (modelRoots == null || modelRoots.Count == 0)
				{
					OutputHelper.WriteError("No model root found in the document.");
					return;
				}

				ModelRoot modelRoot = modelRoots[0];

				// Create the code gen model and validate
				GenItModel model = new GenItModel(modelRoot);
				OutputHelper.Write("Validating model...");
				if (!model.Validate(out var errors))
				{
					OutputHelper.WriteError("Model validation failed with the following errors:");
					foreach (var error in errors)
					{
						OutputHelper.WriteError($"  {error}");
					}

					MessageBox.Show("Model validation failed. Check the output window for details.", "Validation Error",
						MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
				OutputHelper.Write("Model validated.");

				OutputHelper.Write("Starting code generation...");
				OutputHelper.Write("=".PadRight(80, '='));

				model.GenerateCode();

				OutputHelper.Write("=".PadRight(80, '='));
				OutputHelper.Write("Code generation completed successfully!");
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Code Generation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				OutputHelper.WriteError($"Code generation failed: {ex.Message}");
				OutputHelper.WriteError(ex.StackTrace);
			}
			finally
			{

			}
		}

		#endregion

		#region Add New Service Command

		/// <summary>
		/// Determines whether the "Add New Service" menu item should be visible and enabled.
		/// Only visible when right-clicking on an EntityModel shape or ServiceModel compartment item.
		/// </summary>
		private void OnStatusAddNewService(object sender, EventArgs args)
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

			// Check each selected object
			foreach (object selectedObject in selection)
			{
				// Check if clicking on the EntityModel shape
				if (selectedObject is ClassShape classShape && classShape.ModelElement is EntityModel)
				{
					command.Visible = true;
					command.Enabled = true;
					return;
				}

				// Check if clicking directly on a ServiceModel item in the compartment
				if (selectedObject is ServiceModel serviceModel)
				{
					// Ensure we can get back to the parent EntityModel
					if (serviceModel.EntityModeled != null)
					{
						command.Visible = true;
						command.Enabled = true;
						return;
					}
				}
			}
		}

		/// <summary>
		/// Event handler to add a new ServiceModel to the EntityModel's ServiceModeled collection
		/// </summary>
		private void OnMenuAddNewService(object sender, EventArgs args)
		{
			try
			{
				var selection = this.CurrentSelection;
				if (selection == null || selection.Count == 0)
					return;

				EntityModel entityModel = null;

				// Find the EntityModel from the selection
				foreach (object selectedObject in selection)
				{
					// Case 1: Selected a ClassShape (the visual representation)
					if (selectedObject is ClassShape classShape)
					{
						entityModel = classShape.ModelElement as EntityModel;
						if (entityModel != null)
							break;
					}
					// Case 2: Selected a ServiceModel directly
					else if (selectedObject is ServiceModel serviceModel)
					{
						// Get the EntityModel from the ServiceModel's EntityModeled property
						entityModel = serviceModel.EntityModeled;
						if (entityModel != null)
							break;
					}
				}

				if (entityModel == null)
				{
					MessageBox.Show(
						"Could not determine the EntityModel. Please select an EntityModel or Service in the diagram.",
						"Add New Service",
						MessageBoxButtons.OK,
						MessageBoxIcon.Warning);
					return;
				}

				// Create the new ServiceModel with a transaction
				using (Transaction transaction = entityModel.Store.TransactionManager.BeginTransaction("Add New Service"))
				{
					// Determine the version number based on existing services
					int nextVersion = entityModel.ServiceModeled.Count + 1;
					string versionString = $"v{nextVersion}";

					// Create the new ServiceModel
					ServiceModel newService = new ServiceModel(entityModel.Store);
					newService.Name = $"Service{nextVersion}";
					newService.Version = versionString;
					
					// Set default properties
					newService.Enabled = true;
					newService.InclCreate = true;
					newService.InclUpdate = true;
					newService.InclDelete = true;
					newService.InclController = true;

					// Create the relationship link to add it to the EntityModel's ServiceModeled collection
					new EntityModelHasServiceModels(entityModel, newService);

					transaction.Commit();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(
					$"Error adding new service: {ex.Message}",
					"Add New Service Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);
			}
		}

		#endregion
	}
}
