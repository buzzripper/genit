using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;

namespace Dyvenix.GenIt
{
    /// <summary>
    /// A UITypeEditor that displays a folder browser dialog for selecting folder paths.
    /// </summary>
    public class FolderPathEditor : UITypeEditor
    {
        /// <summary>
        /// Gets the editor style used by the EditValue method.
        /// </summary>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        /// <summary>
        /// Edits the specified object's value using the editor style indicated by the GetEditStyle method.
        /// </summary>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select a folder";
                dialog.ShowNewFolderButton = true;

                // Set the initial path if one exists
                if (value is string currentPath && !string.IsNullOrEmpty(currentPath))
                {
                    dialog.SelectedPath = currentPath;
                }

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    return dialog.SelectedPath;
                }
            }

            // Return the original value if the user cancels
            return value;
        }
    }
}
