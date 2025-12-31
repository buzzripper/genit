using Dyvenix.GenIt.DslPackage.CodeGen;
using Dyvenix.GenIt.DslPackage.CodeGen.Misc;
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

					ShowMessageBox("Model validation failed. Check the output window for details.", "Validation Error");
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

		/// <summary>
		/// Shows a message box using Visual Studio's UI shell
		/// </summary>
		private void ShowMessageBox(string message, string title)
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			OLEMSGICON icon = OLEMSGICON.OLEMSGICON_INFO;

			IVsUIShell uiShell = (IVsUIShell)this.ServiceProvider.GetService(typeof(SVsUIShell));
			if (uiShell != null)
			{
				Guid clsid = Guid.Empty;
				int result;
				uiShell.ShowMessageBox(
					0,
					ref clsid,
					title,
					message,
					string.Empty,
					0,
					OLEMSGBUTTON.OLEMSGBUTTON_OK,
					OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
					icon,
					0,
					out result);
			}
		}
	}
}
