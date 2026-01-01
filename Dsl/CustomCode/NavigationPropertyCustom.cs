using System.ComponentModel;

namespace Dyvenix.GenIt
{
    /// <summary>
    /// Partial class to attach the custom TypeDescriptionProvider to NavigationProperty.
    /// </summary>
    [TypeDescriptionProvider(typeof(NavigationPropertyTypeDescriptionProvider))]
    public partial class NavigationProperty
    {
    }
}
