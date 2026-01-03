using System;
using System.Linq;
using System.Text;
using System.Windows;
using DslWpf.ViewModels;

namespace DslWpf
{
    public partial class MainWindow : Window
    {
        private EntityViewModel _entity;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadSampleData();
        }

        private void LoadSampleData()
        {
            _entity = EntityViewModel.CreateSample();
            svcEditCtl.SetEntity(_entity);
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadSampleData();
        }

        private void btnShowState_Click(object sender, RoutedEventArgs e)
        {
            if (_entity?.Service == null)
            {
                MessageBox.Show("No entity loaded.", "State", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var sb = new StringBuilder();
            var svc = _entity.Service;

            sb.AppendLine($"Entity: {_entity.Name}");
            sb.AppendLine();
            sb.AppendLine("=== SERVICE STATE ===");
            sb.AppendLine($"Enabled: {svc.Enabled}");
            sb.AppendLine($"Include Create: {svc.InclCreate}");
            sb.AppendLine($"Include Update: {svc.InclUpdate}");
            sb.AppendLine($"Include Delete: {svc.InclDelete}");
            sb.AppendLine($"Include Controller: {svc.InclController}");
            sb.AppendLine($"Controller Version: {svc.ControllerVersion}");
            sb.AppendLine();
            sb.AppendLine($"=== READ METHODS ({svc.ReadMethods.Count}) ===");
            foreach (var rm in svc.ReadMethods.OrderBy(m => m.DisplayOrder))
            {
                sb.AppendLine($"  - {rm.Name} (Paging:{rm.InclPaging}, Query:{rm.UseQuery}, Sorting:{rm.InclSorting})");
                sb.AppendLine($"    Filter Props: {rm.FilterProperties.Count}");
                sb.AppendLine($"    Nav Props: {rm.InclNavProperties.Count}");
            }
            sb.AppendLine();
            sb.AppendLine($"=== UPDATE METHODS ({svc.UpdateMethods.Count}) ===");
            foreach (var um in svc.UpdateMethods.OrderBy(m => m.DisplayOrder))
            {
                sb.AppendLine($"  - {um.Name} (UseDto:{um.UseDto})");
                sb.AppendLine($"    Update Props: {um.UpdateProperties.Count}");
            }

            MessageBox.Show(sb.ToString(), "Current Model State", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
