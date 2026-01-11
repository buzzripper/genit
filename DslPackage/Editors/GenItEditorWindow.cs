using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace Dyvenix.GenIt.DslPackage.Editors
{
    /// <summary>
    /// GenItEditorWindow - A tool window that hosts editor controls for various model elements.
    /// This window displays context-sensitive editors based on the currently selected DSL element.
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
