using System.Windows;
using System.Windows.Controls;
using Dyvenix.GenIt.DslPackage.Tools.Services.ViewModels;

namespace Dyvenix.GenIt.DslPackage.Tools.Services.Controls
{
    public partial class SvcEditControl : UserControlBase
    {
        private ServiceViewModel _service;
        private EntityViewModel _entity;

        public SvcEditControl()
        {
            InitializeComponent();
        }

        public void SetEntity(EntityViewModel entity)
        {
            _entity = entity;
            _service = entity.Service;

            if (_service != null)
            {
                PopulateControls();
            }
        }

        private void PopulateControls()
        {
            if (_service == null || _entity == null) return;

            _suspendUpdates = true;

            ckbEnabled.IsChecked = _service.Enabled;
            ckbInclCreate.IsChecked = _service.InclCreate;
            ckbInclUpdate.IsChecked = _service.InclUpdate;
            ckbInclDelete.IsChecked = _service.InclDelete;
            ckbInclController.IsChecked = _service.InclController;

            readMethodsEditCtl.SetData(_service.ReadMethods, _entity.Properties, _entity.NavProperties);
            updMethodsEditCtl.SetData(_service.UpdateMethods, _entity.Properties);

            // Bind Service tab grids
            grdServiceUsings.ItemsSource = _service.ServiceUsings;
            grdServiceAttributes.ItemsSource = _service.ServiceAttributes;

            // Bind Controller tab grids
            grdControllerUsings.ItemsSource = _service.ControllerUsings;
            grdControllerAttributes.ItemsSource = _service.ControllerAttributes;

            // Update Controller tab visibility
            UpdateControllerTabVisibility();

            _suspendUpdates = false;
        }

        private void UpdateControllerTabVisibility()
        {
            bool showController = ckbInclController.IsChecked ?? false;
            
            // If hiding the Controller tab and it's currently selected, switch to Service tab first
            if (!showController && tabControl.SelectedItem == tabController)
            {
                tabControl.SelectedItem = tabService;
            }
            
            tabController.Visibility = showController 
                ? Visibility.Visible 
                : Visibility.Collapsed;
        }

        private void ckbEnabled_Changed(object sender, RoutedEventArgs e)
        {
            if (!_suspendUpdates && _service != null)
                _service.Enabled = ckbEnabled.IsChecked ?? false;
        }

        private void ckbInclCreate_Changed(object sender, RoutedEventArgs e)
        {
            if (!_suspendUpdates && _service != null)
                _service.InclCreate = ckbInclCreate.IsChecked ?? false;
        }

        private void ckbInclUpdate_Changed(object sender, RoutedEventArgs e)
        {
            if (!_suspendUpdates && _service != null)
                _service.InclUpdate = ckbInclUpdate.IsChecked ?? false;
        }

        private void ckbInclDelete_Changed(object sender, RoutedEventArgs e)
        {
            if (!_suspendUpdates && _service != null)
                _service.InclDelete = ckbInclDelete.IsChecked ?? false;
        }

        private void ckbInclController_Changed(object sender, RoutedEventArgs e)
        {
            if (!_suspendUpdates && _service != null)
            {
                _service.InclController = ckbInclController.IsChecked ?? false;
                UpdateControllerTabVisibility();
            }
        }

        // Service tab event handlers
        private void btnAddServiceUsing_Click(object sender, RoutedEventArgs e)
        {
            if (_service == null) return;
            int nextNum = _service.ServiceUsings.Count + 1;
            _service.ServiceUsings.Add(new EditableString($"Using{nextNum}"));
        }

        private void btnDeleteServiceUsing_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is EditableString item && _service != null)
            {
                _service.ServiceUsings.Remove(item);
            }
        }

        private void btnAddServiceAttribute_Click(object sender, RoutedEventArgs e)
        {
            if (_service == null) return;
            int nextNum = _service.ServiceAttributes.Count + 1;
            _service.ServiceAttributes.Add(new EditableString($"Attribute{nextNum}"));
        }

        private void btnDeleteServiceAttribute_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is EditableString item && _service != null)
            {
                _service.ServiceAttributes.Remove(item);
            }
        }

        // Controller tab event handlers
        private void btnAddControllerUsing_Click(object sender, RoutedEventArgs e)
        {
            if (_service == null) return;
            int nextNum = _service.ControllerUsings.Count + 1;
            _service.ControllerUsings.Add(new EditableString($"Using{nextNum}"));
        }

        private void btnDeleteControllerUsing_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is EditableString item && _service != null)
            {
                _service.ControllerUsings.Remove(item);
            }
        }

        private void btnAddControllerAttribute_Click(object sender, RoutedEventArgs e)
        {
            if (_service == null) return;
            int nextNum = _service.ControllerAttributes.Count + 1;
            _service.ControllerAttributes.Add(new EditableString($"Attribute{nextNum}"));
        }

        private void btnDeleteControllerAttribute_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is EditableString item && _service != null)
            {
                _service.ControllerAttributes.Remove(item);
            }
        }
    }
}
