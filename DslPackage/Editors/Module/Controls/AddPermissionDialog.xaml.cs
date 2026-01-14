using System.Windows;

namespace Dyvenix.GenIt.DslPackage.Editors.Module.Controls
{
    /// <summary>
    /// Dialog for adding a new permission.
    /// </summary>
    public partial class AddPermissionDialog : Window
    {
        public string PermissionName { get; private set; }

        public AddPermissionDialog()
        {
            InitializeComponent();
            txtPermission.Focus();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            PermissionName = txtPermission.Text;
            DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
