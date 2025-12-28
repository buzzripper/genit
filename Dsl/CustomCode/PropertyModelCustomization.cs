using System.ComponentModel;
using Microsoft.VisualStudio.Modeling.Design;

namespace Dyvenix.GenIt
{
    /// <summary>
    /// Partial class to register the custom TypeDescriptionProvider for PropertyModel.
    /// This enables conditional property visibility in the Properties window.
    /// </summary>
    [TypeDescriptionProvider(typeof(PropertyModelTypeDescriptionProvider))]
    public partial class PropertyModel
    {
        // The TypeDescriptionProvider attribute registers our custom provider
        // that controls the visibility of the Length property based on DataType.
    }
}
