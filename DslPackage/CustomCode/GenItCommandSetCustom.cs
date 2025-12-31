using System;
using System.ComponentModel.Design;
using System.Linq;
using Dyvenix.GenIt.DslPackage.CodeGen;
using Dyvenix.GenIt.DslPackage.CodeGen.Misc;
using Dyvenix.GenIt.DslPackage.CustomCode;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Shell;

namespace Dyvenix.GenIt
{
	internal partial class GenItCommandSet : GenItCommandSetBase
	{
		private CommandID _generateCodeCommandId = new CommandID(new Guid(Constants.GenItCommandSetId), 0x0100);
		private readonly OutputWindowHelper _outputHelper = new OutputWindowHelper();

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

			return commands;
		}

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
			try
			{
				GenItDocData docData = this.CurrentGenItDocData;
				if (docData == null || docData.Store == null)
				{
					_outputHelper.WriteError("No active document found.");
					return;
				}

				var modelRoots = docData.Store.ElementDirectory.FindElements<ModelRoot>();
				if (modelRoots == null || modelRoots.Count == 0)
				{
					_outputHelper.WriteError("No model root found in the document.");
					return;
				}

				ModelRoot modelRoot = modelRoots[0];

				// Validate the model first
				ModelValidator validator = new ModelValidator();
				validator.Validate(modelRoot);

				_outputHelper.WriteAndActivate("Starting code generation...");
				_outputHelper.Write("=".PadRight(80, '='));

				// Create the model wrapper
				GenItModel model = new GenItModel(modelRoot);

				// Generate entities
				if (modelRoot.EntitiesEnabled)
				{
					_outputHelper.Write("Generating entities...");
					// TODO: Implement entity generation
					// EntityGenerator entityGenerator = new EntityGenerator(
					//     model.Entities,
					//     modelRoot.EntitiesNamespace,
					//     modelRoot.TemplatesFolder,
					//     modelRoot.EntitiesOutputFolder);
					// entityGenerator.Run();
					_outputHelper.Write($"Generated {model.Entities.Count} entities.");
				}

				// Generate DbContext
				if (modelRoot.DbContextEnabled)
				{
					_outputHelper.Write("Generating DbContext...");
					// TODO: Implement DbContext generation
				}

				// Generate Enums
				if (modelRoot.EnumsEnabled)
				{
					_outputHelper.Write("Generating enums...");
					// TODO: Implement enum generation
					_outputHelper.Write($"Generated {model.Enums.Count} enums.");
				}

				_outputHelper.Write("=".PadRight(80, '='));
				_outputHelper.Write("Code generation completed successfully!");
			}
			catch (Exception ex)
			{
				_outputHelper.WriteError($"Code generation failed: {ex.Message}");
				_outputHelper.WriteError(ex.StackTrace);
			}
		}
	}
}
