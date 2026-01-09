using System.Windows.Controls;

namespace Dyvenix.GenIt.DslPackage.Editors
{
    /// <summary>
    /// Base class for all editor user controls.
    /// Provides common functionality shared across different model editors.
    /// </summary>
    public class UserControlBase : UserControl
    {
        /// <summary>
        /// When true, suspends updates to the underlying model.
        /// Use this to prevent feedback loops when programmatically setting control values.
        /// </summary>
        protected bool _suspendUpdates;

        public UserControlBase()
        {
        }
    }
}
