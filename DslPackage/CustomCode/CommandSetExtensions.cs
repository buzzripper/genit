using Dyvenix.GenIt.DslPackage.CodeGen;
using Microsoft.VisualStudio.Modeling.Shell;
using System;
using System.ComponentModel.Design;
using System.Text;

namespace Dyvenix.GenIt
{
	/// <summary>
	/// Extends the GenItCommandSet with custom commands.
	/// </summary>
	internal partial class GenItCommandSet
	{
		/// <summary>
		/// Command ID for Generate Code command.
		/// </summary>
		private const int CommandIdGenerateCode = 0x0100;

		/// <summary>
		/// Adds custom commands to the menu.
		/// </summary>
		protected override System.Collections.Generic.IList<MenuCommand> GetMenuCommands()
		{
			var commands = base.GetMenuCommands();

			// Add "Generate Code" command to the diagram context menu
			var generateCodeCommand = new DynamicStatusMenuCommand(
				new EventHandler(OnStatusGenerateCode),
				new EventHandler(OnMenuGenerateCode),
				new CommandID(
					new Guid(Constants.GenItCommandSetId),
					CommandIdGenerateCode));

			commands.Add(generateCodeCommand);

			return commands;
		}

		/// <summary>
		/// Determines whether Generate Code command should be visible/enabled.
		/// </summary>
		private void OnStatusGenerateCode(object sender, EventArgs e)
		{
			MenuCommand command = sender as MenuCommand;
			if (command != null)
			{
				// Enable only when a diagram is open and has entities
				command.Visible = true;
				command.Enabled = (this.CurrentGenItDocData != null);
			}
		}

		/// <summary>
		/// Handler for Generate Code menu command.
		/// </summary>
		private void OnMenuGenerateCode(object sender, EventArgs e)
		{
			GenItModel genitModel = null;
			try
			{
				// Get the current document
				GenItDocData docData = this.CurrentGenItDocData;
				if (docData == null || docData.RootElement == null)
				{
					ShowMessage("No model is currently open.", "GenIt Code Generator");
					return;
				}

				ModelRoot modelRoot = docData.RootElement as ModelRoot;
				if (modelRoot == null)
				{
					ShowMessage("Could not access the model root.", "GenIt Code Generator");
					return;
				}

				genitModel = new GenItModel(modelRoot);

				// TODO: Replace this with your actual code generator
				// For now, just show what would be generated
				var sb = new StringBuilder();
				sb.AppendLine($"Entities: {genitModel.Entities.Count}");
				sb.AppendLine($"Enums: {genitModel.Enums.Count}");
				sb.AppendLine($"Associations: {genitModel.Associations.Count}");
				sb.AppendLine($"Enum Associations: {genitModel.EnumAssociations.Count}");

				ShowMessage(sb.ToString());
			}
			catch (Exception ex)
			{
				ShowErrorMessage($"Error generating code:\n\n{ex.Message}", "GenIt Code Generator");
			}
			finally
			{
				genitModel = null;
			}
		}

		/// <summary>
		/// Shows an information message to the user.
		/// </summary>
		private void ShowMessage(string message, string title = "GenIt")
		{
			System.Windows.Forms.MessageBox.Show(
				message,
				title,
				System.Windows.Forms.MessageBoxButtons.OK,
				System.Windows.Forms.MessageBoxIcon.Information);
		}

		/// <summary>
		/// Shows an error message to the user.
		/// </summary>
		private void ShowErrorMessage(string message, string title)
		{
			System.Windows.Forms.MessageBox.Show(
				message,
				title,
				System.Windows.Forms.MessageBoxButtons.OK,
				System.Windows.Forms.MessageBoxIcon.Error);
		}
	}
}
