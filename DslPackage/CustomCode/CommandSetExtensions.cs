using System;
using System.ComponentModel.Design;
using System.Linq;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Shell;

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

                // Get all entities from the model
                var entities = modelRoot.Types.OfType<EntityModel>().ToList();
                if (entities.Count == 0)
                {
                    ShowMessage("No entities found in the model. Add some entities first.", "GenIt Code Generator");
                    return;
                }

                // TODO: Replace this with your actual code generator
                // For now, just show what would be generated
                string summary = $"Model: {modelRoot.Name}\n\n" +
                               $"Entities found: {entities.Count}\n\n";

                foreach (var entity in entities)
                {
                    summary += $"Entity: {entity.Name}\n";
                    summary += $"  Properties: {entity.Attributes.Count}\n";
                    summary += $"  Operations: {entity.Operations.Count}\n";
                    
                    foreach (var prop in entity.Attributes)
                    {
                        summary += $"    - {prop.Name}: {prop.DataType}";
                        if (prop.DataType == DataType.String || prop.DataType == DataType.ByteArray)
                        {
                            if (prop.Length > 0)
                                summary += $"({prop.Length})";
                        }
                        summary += "\n";
                    }
                    summary += "\n";
                }

                // TODO: Call your actual code generator here
                // CodeGenerator.Generate(modelRoot, docData.FileName);

                ShowMessage(summary + "\nReady to generate code!\n\nImplement your code generator logic here.", 
                    "GenIt Code Generator");
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error generating code:\n\n{ex.Message}", "GenIt Code Generator");
            }
        }

        /// <summary>
        /// Shows an information message to the user.
        /// </summary>
        private void ShowMessage(string message, string title)
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
