using System.ComponentModel;
using Microsoft.VisualStudio.Modeling.Design;

namespace Dyvenix.GenIt
{
    /// <summary>
    /// Partial class for ModelRoot to register the custom TypeDescriptionProvider.
    /// This enables the folder browser button on path properties in the Properties window.
    /// </summary>
    [TypeDescriptionProvider(typeof(ModelRootTypeDescriptionProvider))]
    public partial class ModelRoot
    {
        // The TypeDescriptionProvider attribute registers our custom provider
        // that adds the FolderPathEditor to the EntitiesPath property.
    }
}
