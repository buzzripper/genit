using System.ComponentModel;

namespace Dyvenix.GenIt
{
    /// <summary>
    /// Partial class to attach the custom TypeDescriptionProvider to ServiceModel.
    /// This makes the Version property read-only in the Properties window.
    /// </summary>
    [TypeDescriptionProvider(typeof(ServiceModelTypeDescriptionProvider))]
    public partial class ServiceModel
    {
    }
}
