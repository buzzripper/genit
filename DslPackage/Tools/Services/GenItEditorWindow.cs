using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace Dyvenix.GenIt.DslPackage.Tools.Services
{
    /// <summary>
    /// GenItEditorWindow - A tool window that hosts the SvcEditControl for editing ServiceModel properties.
    /// This window displays when a ServiceModel is selected in the DSL designer and hides when
    /// any other element (or none) is selected.
    /// </summary>
    [Guid(GenItEditorWindowGuids.GenItEditorWindowGuidString)]
    public class GenItEditorWindow : ToolWindowPane
    {
        private GenItEditorWindowControl _control;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenItEditorWindow"/> class.
        /// </summary>
        public GenItEditorWindow() : base(null)
        {
            this.Caption = "GenIt";

            // Create the content
            _control = new GenItEditorWindowControl();
            this.Content = _control;
        }

        /// <summary>
        /// Gets the control hosted in this tool window.
        /// </summary>
        public GenItEditorWindowControl Control => _control;
    }

    /// <summary>
    /// GUIDs for the GenItEditorWindow.
    /// </summary>
    internal static class GenItEditorWindowGuids
    {
        public const string GenItEditorWindowGuidString = "a7b8c9d0-e1f2-3456-7890-abcdef012345";
        public static readonly Guid GenItEditorWindowGuid = new Guid(GenItEditorWindowGuidString);
    }
}
