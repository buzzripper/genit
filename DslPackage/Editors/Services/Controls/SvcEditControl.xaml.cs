using Dyvenix.GenIt.DslPackage.Editors;
using Dyvenix.GenIt.DslPackage.Editors.Services.Adapters;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Dyvenix.GenIt.DslPackage.Editors.Services.Controls
{
	public partial class SvcEditControl : UserControlBase
	{
		private EntityModel _entity;
		private ServiceModelAdapter _serviceAdapter;

		public SvcEditControl()
		{
			InitializeComponent();
		}

		public void Initialize(EntityModel entityModel, string serviceModelVersion)
		{
			_entity = entityModel;
			var serviceModel = entityModel.ServiceModels.FirstOrDefault(s => s.Version == serviceModelVersion);
			if (serviceModel == null)
				throw new System.Exception($"Service model version '{serviceModelVersion}' not found in entity '{entityModel.Name}'.");

			_serviceAdapter = new ServiceModelAdapter(serviceModel);
			PopulateControls(serviceModel);
		}

		private void PopulateControls(ServiceModel serviceModel)
		{
			if (_serviceAdapter == null || _entity == null)
				return;

			_suspendUpdates = true;

			// Set entity name and version
			txtEntityName.Text = _entity.Name;
			txtVersion.Text = _serviceAdapter.Version ?? "v1";

			ckbEnabled.IsChecked = _serviceAdapter.Enabled;
			ckbInclCreate.IsChecked = _serviceAdapter.InclCreate;
			ckbInclUpdate.IsChecked = _serviceAdapter.InclUpdate;
			ckbInclDelete.IsChecked = _serviceAdapter.InclDelete;
			ckbInclController.IsChecked = _serviceAdapter.InclController;

			readMethodsEditCtl.SetData(serviceModel, _serviceAdapter.ReadMethods, _entity.Properties, _entity.NavigationProperties);
			updMethodsEditCtl.SetData(serviceModel, _serviceAdapter.Model.UpdateMethods, _entity.Properties);

			// Bind Service tab grids using the adapter's collection properties
			grdServiceUsings.ItemsSource = _serviceAdapter.ServiceUsingsList;
			grdServiceAttributes.ItemsSource = _serviceAdapter.ServiceAttributesList;

			// Bind Controller tab grids
			grdControllerUsings.ItemsSource = _serviceAdapter.ControllerUsingsList;
			grdControllerAttributes.ItemsSource = _serviceAdapter.ControllerAttributesList;

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
			if (!_suspendUpdates && _serviceAdapter != null)
			{
				_serviceAdapter.Enabled = ckbEnabled.IsChecked ?? false;
			}
		}

		private void ckbInclCreate_Changed(object sender, RoutedEventArgs e)
		{
			if (!_suspendUpdates && _serviceAdapter != null)
			{
				_serviceAdapter.InclCreate = ckbInclCreate.IsChecked ?? false;
			}
		}

		private void ckbInclUpdate_Changed(object sender, RoutedEventArgs e)
		{
			if (!_suspendUpdates && _serviceAdapter != null)
			{
				_serviceAdapter.InclUpdate = ckbInclUpdate.IsChecked ?? false;
			}
		}

		private void ckbInclDelete_Changed(object sender, RoutedEventArgs e)
		{
			if (!_suspendUpdates && _serviceAdapter != null)
			{
				_serviceAdapter.InclDelete = ckbInclDelete.IsChecked ?? false;
			}
		}

		private void ckbInclController_Changed(object sender, RoutedEventArgs e)
		{
			if (!_suspendUpdates && _serviceAdapter != null)
			{
				_serviceAdapter.InclController = ckbInclController.IsChecked ?? false;
				UpdateControllerTabVisibility();
			}
		}

		// Service tab event handlers
		private void btnAddServiceUsing_Click(object sender, RoutedEventArgs e)
		{
			if (_serviceAdapter == null) return;
			int nextNum = _serviceAdapter.ServiceUsingsList.Count + 1;
			_serviceAdapter.ServiceUsingsList.Add(new EditableString($"Using{nextNum}"));
		}

		private void btnDeleteServiceUsing_Click(object sender, RoutedEventArgs e)
		{
			if (sender is Button btn && btn.DataContext is EditableString item && _serviceAdapter != null)
			{
				_serviceAdapter.ServiceUsingsList.Remove(item);
			}
		}

		private void btnAddServiceAttribute_Click(object sender, RoutedEventArgs e)
		{
			if (_serviceAdapter == null) return;
			int nextNum = _serviceAdapter.ServiceAttributesList.Count + 1;
			_serviceAdapter.ServiceAttributesList.Add(new EditableString($"Attribute{nextNum}"));
		}

		private void btnDeleteServiceAttribute_Click(object sender, RoutedEventArgs e)
		{
			if (sender is Button btn && btn.DataContext is EditableString item && _serviceAdapter != null)
			{
				_serviceAdapter.ServiceAttributesList.Remove(item);
			}
		}

		// Controller tab event handlers
		private void btnAddControllerUsing_Click(object sender, RoutedEventArgs e)
		{
			if (_serviceAdapter == null) return;
			int nextNum = _serviceAdapter.ControllerUsingsList.Count + 1;
			_serviceAdapter.ControllerUsingsList.Add(new EditableString($"Using{nextNum}"));
		}

		private void btnDeleteControllerUsing_Click(object sender, RoutedEventArgs e)
		{
			if (sender is Button btn && btn.DataContext is EditableString item && _serviceAdapter != null)
			{
				_serviceAdapter.ControllerUsingsList.Remove(item);
			}
		}

		private void btnAddControllerAttribute_Click(object sender, RoutedEventArgs e)
		{
			if (_serviceAdapter == null) return;
			int nextNum = _serviceAdapter.ControllerAttributesList.Count + 1;
			_serviceAdapter.ControllerAttributesList.Add(new EditableString($"Attribute{nextNum}"));
		}

		private void btnDeleteControllerAttribute_Click(object sender, RoutedEventArgs e)
		{
			if (sender is Button btn && btn.DataContext is EditableString item && _serviceAdapter != null)
			{
				_serviceAdapter.ControllerAttributesList.Remove(item);
			}
		}
	}
}
