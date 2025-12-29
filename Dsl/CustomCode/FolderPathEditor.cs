using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
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
                    // If the current path is relative, try to make it absolute for the dialog
                    string absolutePath = currentPath;
                    string solutionRoot = PackageUtils.SolutionRootPath;
                    
                    if (!Path.IsPathRooted(currentPath) && !string.IsNullOrEmpty(solutionRoot))
                    {
                        try
                        {
                            absolutePath = Path.GetFullPath(Path.Combine(solutionRoot, currentPath));
                        }
                        catch
                        {
                            // If path combination fails, just use the current path
                        }
                    }
                    
                    if (Directory.Exists(absolutePath))
                    {
                        dialog.SelectedPath = absolutePath;
                    }
                }

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedPath = dialog.SelectedPath;
                    
                    // Try to convert to relative path
                    string relativePath = TryMakeRelativePath(selectedPath);
                    return relativePath;
                }
            }

            // Return the original value if the user cancels
            return value;
        }

        /// <summary>
        /// Attempts to convert an absolute path to a relative path based on the solution root.
        /// If conversion fails, returns the original absolute path.
        /// </summary>
        private string TryMakeRelativePath(string absolutePath)
        {
            string solutionRoot = null;
            
            try
            {
                solutionRoot = PackageUtils.SolutionRootPath;
                
                if (string.IsNullOrEmpty(solutionRoot))
                {
                    System.Diagnostics.Debug.WriteLine("FolderPathEditor: Unable to create relative path - Solution root path is not set. Using absolute path.");
                    return absolutePath;
                }

                // Ensure paths are properly formatted
                if (!solutionRoot.EndsWith(Path.DirectorySeparatorChar.ToString()) && 
                    !solutionRoot.EndsWith(Path.AltDirectorySeparatorChar.ToString()))
                {
                    solutionRoot = solutionRoot + Path.DirectorySeparatorChar;
                }

                Uri solutionUri = new Uri(solutionRoot);
                Uri pathUri = new Uri(absolutePath);

                // Create relative path
                Uri relativeUri = solutionUri.MakeRelativeUri(pathUri);
                string result = Uri.UnescapeDataString(relativeUri.ToString());
                
                // Convert forward slashes to backslashes for Windows paths
                result = result.Replace('/', Path.DirectorySeparatorChar);
                
                System.Diagnostics.Debug.WriteLine($"FolderPathEditor: Converted '{absolutePath}' to relative path '{result}' (solution root: '{solutionRoot}')");
                
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"FolderPathEditor: Error creating relative path - {ex.Message}. SolutionRoot='{solutionRoot ?? "null"}', AbsolutePath='{absolutePath}'. Using absolute path.");
                return absolutePath;
            }
        }
    }
}
