using System.ComponentModel;

namespace Dyvenix.GenIt
{
    /// <summary>
    /// Partial class to attach the custom TypeDescriptionProvider to Association.
    /// </summary>
    [TypeDescriptionProvider(typeof(AssociationTypeDescriptionProvider))]
    public partial class Association
    {
    }
}
