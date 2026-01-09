using Dyvenix.GenIt.DslPackage.Editors.Services.Controls;
using System.Windows;
using System.Windows.Controls;

namespace Dyvenix.GenIt.DslPackage.Editors
{
    /// <summary>
    /// Host control for the GenItEditorWindow tool window.
    /// Manages visibility of different editor controls based on selection.
    /// </summary>
    public partial class GenItEditorWindowControl : UserControl
    {
        private SvcEditControl _svcEditControl;

        public GenItEditorWindowControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Shows the service editor with the specified entity model.
        /// </summary>
        /// <param name="entityModel">The EntityModel to display.</param>
        /// <param name="serviceModelVersion">The version of the ServiceModel to display.</param>
        public void ShowServiceEditor(EntityModel entityModel, string serviceModelVersion)
        {
            if (entityModel != null)
            {
                // Lazy-create the service edit control
                if (_svcEditControl == null)
                {
                    _svcEditControl = new SvcEditControl();
                }

                _svcEditControl.Initialize(entityModel, serviceModelVersion);
                editorContentHost.Content = _svcEditControl;
                editorContentHost.Visibility = Visibility.Visible;
                txtNoSelection.Visibility = Visibility.Collapsed;
            }
            else
            {
                HideEditor();
            }
        }

        /// <summary>
        /// Hides the current editor and shows the "No item selected" message.
        /// </summary>
        public void HideEditor()
        {
            editorContentHost.Content = null;
            editorContentHost.Visibility = Visibility.Collapsed;
            txtNoSelection.Visibility = Visibility.Hidden;
        }
    }
}
