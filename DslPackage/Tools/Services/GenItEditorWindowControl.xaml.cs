using System.Windows;
using System.Windows.Controls;
using Dyvenix.GenIt.DslPackage.Tools.Services.ViewModels;

namespace Dyvenix.GenIt.DslPackage.Tools.Services
{
    /// <summary>
    /// Host control for the GenItEditorWindow tool window.
    /// Contains the SvcEditControl and manages visibility based on selection.
    /// </summary>
    public partial class GenItEditorWindowControl : UserControl
    {
        public GenItEditorWindowControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Shows the service editor with the specified entity view model.
        /// </summary>
        /// <param name="entity">The entity view model to display.</param>
        public void ShowServiceEditor(EntityViewModel entity)
        {
            if (entity != null && entity.Service != null)
            {
                svcEditControl.SetEntity(entity);
                svcEditControl.Visibility = Visibility.Visible;
                txtNoSelection.Visibility = Visibility.Collapsed;
            }
            else
            {
                HideServiceEditor();
            }
        }

        /// <summary>
        /// Hides the service editor and shows the "No item selected" message.
        /// </summary>
        public void HideServiceEditor()
        {
            svcEditControl.Visibility = Visibility.Collapsed;
            txtNoSelection.Visibility = Visibility.Visible;
        }
    }
}
