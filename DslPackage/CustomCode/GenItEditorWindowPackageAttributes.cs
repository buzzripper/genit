using Dyvenix.GenIt.DslPackage.Editors;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using VSShell = Microsoft.VisualStudio.Shell;

namespace Dyvenix.GenIt
{
    /// <summary>
    /// Partial class to add the GenItEditorWindow tool window registration.
    /// </summary>
    [VSShell.ProvideToolWindow(typeof(GenItEditorWindow), 
        MultiInstances = false, 
        Style = VSShell.VsDockStyle.Tabbed, 
        Orientation = VSShell.ToolWindowOrientation.Bottom, 
        Window = "{34E76E81-EE4A-11D0-AE2E-00A0C90FFFC3}")] // Output window
    [VSShell.ProvideToolWindowVisibility(typeof(GenItEditorWindow), Constants.GenItEditorFactoryId)]
    // Auto-load when a solution opens so we can subscribe to solution events
    [VSShell.ProvideAutoLoad(UIContextGuids80.SolutionExists, PackageAutoLoadFlags.BackgroundLoad)]
    internal sealed partial class GenItPackage
    {
    }
}
