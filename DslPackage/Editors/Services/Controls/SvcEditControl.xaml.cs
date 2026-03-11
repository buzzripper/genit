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
			txtVersion.Text = _serviceAdapter.Version ?? "1.0";

			ckbEnabled.IsChecked = _serviceAdapter.Enabled;
			ckbInclCreate.IsChecked = _serviceAdapter.InclCreate;
			ckbInclUpdate.IsChecked = _serviceAdapter.InclUpdate;
			ckbInclDelete.IsChecked = _serviceAdapter.InclDelete;
			ckbInclEndpoints.IsChecked = _serviceAdapter.InclEndpoints;
			ckbInclAngService.IsChecked = _serviceAdapter.InclAngService;

			readMethodsEditCtl.SetData(serviceModel, _serviceAdapter.ReadMethods, _entity.Properties, _entity.NavigationProperties, _entity.DtoModels);
			updMethodsEditCtl.SetData(serviceModel, _serviceAdapter.Model.UpdateMethods, _entity.Properties);

			// Update permission counts for standard methods
			UpdatePermissionsCounts();

			// Bind Service tab grids using the adapter's collection properties
			grdServiceUsings.ItemsSource = _serviceAdapter.ServiceUsingsList;
			grdServiceAttributes.ItemsSource = _serviceAdapter.ServiceAttributesList;

			// Bind Endpoints tab grids
			grdEndpointsUsings.ItemsSource = _serviceAdapter.EndpointsUsingsList;
			grdEndpointsAttributes.ItemsSource = _serviceAdapter.EndpointsAttributesList;

			// Update Endpoints tab visibility
			UpdateEndpointsTabVisibility();

			_suspendUpdates = false;
		}

		private void txtVersion_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (_suspendUpdates || _serviceAdapter == null)
				return;

			_serviceAdapter.Version = txtVersion.Text;
		}

		private void UpdateEndpointsTabVisibility()
		{
			bool showEndpoints = ckbInclEndpoints.IsChecked ?? false;

			// If hiding the Endpoints tab and it's currently selected, switch to Service tab first
			if (!showEndpoints && tabControl.SelectedItem == tabEndpoints)
			{
				tabControl.SelectedItem = tabService;
			}

			tabEndpoints.Visibility = showEndpoints
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
				var isChecked = ckbInclCreate.IsChecked ?? false;
				_serviceAdapter.InclCreate = isChecked;
				btnCreatePerms.IsEnabled = isChecked;
				if (!isChecked)
				{
					DslTransactionHelper.ExecuteInTransaction(_serviceAdapter.Model, "Clear Create Permissions", () =>
					{
						_serviceAdapter.Model.CreatePermissions = string.Empty;
					});
					UpdatePermissionsCounts();
				}
			}
		}

		private void ckbInclUpdate_Changed(object sender, RoutedEventArgs e)
		{
			if (!_suspendUpdates && _serviceAdapter != null)
			{
				var isChecked = ckbInclUpdate.IsChecked ?? false;
				_serviceAdapter.InclUpdate = isChecked;
				btnUpdatePerms.IsEnabled = isChecked;
				if (!isChecked)
				{
					DslTransactionHelper.ExecuteInTransaction(_serviceAdapter.Model, "Clear Update Permissions", () =>
					{
						_serviceAdapter.Model.UpdatePermissions = string.Empty;
					});
					UpdatePermissionsCounts();
				}
			}
		}

		private void ckbInclDelete_Changed(object sender, RoutedEventArgs e)
		{
			if (!_suspendUpdates && _serviceAdapter != null)
			{
				var isChecked = ckbInclDelete.IsChecked ?? false;
				_serviceAdapter.InclDelete = isChecked;
				btnDeletePerms.IsEnabled = isChecked;
				if (!isChecked)
				{
					DslTransactionHelper.ExecuteInTransaction(_serviceAdapter.Model, "Clear Delete Permissions", () =>
					{
						_serviceAdapter.Model.DeletePermissions = string.Empty;
					});
					UpdatePermissionsCounts();
				}
			}
		}

		private void ckbInclEndpoints_Changed(object sender, RoutedEventArgs e)
		{
			if (!_suspendUpdates && _serviceAdapter != null)
			{
				_serviceAdapter.InclEndpoints = ckbInclEndpoints.IsChecked ?? false;
				UpdateEndpointsTabVisibility();
			}
		}

		private void ckbInclAngService_Changed(object sender, RoutedEventArgs e)
		{
			if (!_suspendUpdates && _serviceAdapter != null)
			{
				_serviceAdapter.InclAngService = ckbInclAngService.IsChecked ?? false;
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

		// Endpoints tab event handlers
		private void btnAddEndpointsUsing_Click(object sender, RoutedEventArgs e)
		{
			if (_serviceAdapter == null) return;
			int nextNum = _serviceAdapter.EndpointsUsingsList.Count + 1;
			_serviceAdapter.EndpointsUsingsList.Add(new EditableString($"Using{nextNum}"));
		}

		private void btnDeleteEndpointsUsing_Click(object sender, RoutedEventArgs e)
		{
			if (sender is Button btn && btn.DataContext is EditableString item && _serviceAdapter != null)
			{
				_serviceAdapter.EndpointsUsingsList.Remove(item);
			}
		}

		private void btnAddEndpointsAttribute_Click(object sender, RoutedEventArgs e)
		{
			if (_serviceAdapter == null) return;
			int nextNum = _serviceAdapter.EndpointsAttributesList.Count + 1;
			_serviceAdapter.EndpointsAttributesList.Add(new EditableString($"Attribute{nextNum}"));
		}

		private void btnDeleteEndpointsAttribute_Click(object sender, RoutedEventArgs e)
		{
			if (sender is Button btn && btn.DataContext is EditableString item && _serviceAdapter != null)
			{
				_serviceAdapter.EndpointsAttributesList.Remove(item);
			}
		}

		// Service Permissions popup handler
		private void btnPermissions_Click(object sender, RoutedEventArgs e)
		{
			if (_serviceAdapter == null) return;

			var modelRoot = GetModelRoot();
			if (modelRoot == null) return;

			var ownerWindow = Window.GetWindow(this);
			var newPermissions = Permissions.PermissionsEditorDialog.ShowDialog(ownerWindow, modelRoot, _serviceAdapter.Model.Permissions);

			if (newPermissions != null)
			{
				DslTransactionHelper.ExecuteInTransaction(_serviceAdapter.Model, "Update Service Permissions", () =>
				{
					_serviceAdapter.Model.Permissions = newPermissions;
				});
				UpdatePermissionsCounts();
			}
		}

		// Standard Methods permission button handlers
		private void btnCreatePerms_Click(object sender, RoutedEventArgs e)
		{
			if (_serviceAdapter == null) return;

			var modelRoot = GetModelRoot();
			if (modelRoot == null) return;

			var ownerWindow = Window.GetWindow(this);
			var newPermissions = Permissions.PermissionsEditorDialog.ShowDialog(ownerWindow, modelRoot, _serviceAdapter.Model.CreatePermissions);

			if (newPermissions != null)
			{
				DslTransactionHelper.ExecuteInTransaction(_serviceAdapter.Model, "Update Create Permissions", () =>
				{
					_serviceAdapter.Model.CreatePermissions = newPermissions;
				});
				UpdatePermissionsCounts();
			}
		}

		private void btnUpdatePerms_Click(object sender, RoutedEventArgs e)
		{
			if (_serviceAdapter == null) return;

			var modelRoot = GetModelRoot();
			if (modelRoot == null) return;

			var ownerWindow = Window.GetWindow(this);
			var newPermissions = Permissions.PermissionsEditorDialog.ShowDialog(ownerWindow, modelRoot, _serviceAdapter.Model.UpdatePermissions);

			if (newPermissions != null)
			{
				DslTransactionHelper.ExecuteInTransaction(_serviceAdapter.Model, "Update Update Permissions", () =>
				{
					_serviceAdapter.Model.UpdatePermissions = newPermissions;
				});
				UpdatePermissionsCounts();
			}
		}

		private void btnDeletePerms_Click(object sender, RoutedEventArgs e)
		{
			if (_serviceAdapter == null) return;

			var modelRoot = GetModelRoot();
			if (modelRoot == null) return;

			var ownerWindow = Window.GetWindow(this);
			var newPermissions = Permissions.PermissionsEditorDialog.ShowDialog(ownerWindow, modelRoot, _serviceAdapter.Model.DeletePermissions);

			if (newPermissions != null)
			{
				DslTransactionHelper.ExecuteInTransaction(_serviceAdapter.Model, "Update Delete Permissions", () =>
				{
					_serviceAdapter.Model.DeletePermissions = newPermissions;
				});
				UpdatePermissionsCounts();
			}
		}

		private ModelRoot GetModelRoot()
		{
			if (_serviceAdapter?.Model?.Store == null) return null;

			foreach (var element in _serviceAdapter.Model.Store.ElementDirectory.AllElements)
			{
				if (element is ModelRoot modelRoot)
					return modelRoot;
			}
			return null;
		}

		private void UpdatePermissionsCounts()
		{
			btnCreatePerms.Content = CountPermissions(_serviceAdapter.Model.CreatePermissions).ToString();
			btnUpdatePerms.Content = CountPermissions(_serviceAdapter.Model.UpdatePermissions).ToString();
			btnDeletePerms.Content = CountPermissions(_serviceAdapter.Model.DeletePermissions).ToString();

			// Update service permissions button text
			var permsCount = CountPermissions(_serviceAdapter.Model.Permissions);
			btnPermissions.Content = $"Permissions ({permsCount})";

			// Update button enabled states based on checkbox states
			btnCreatePerms.IsEnabled = ckbInclCreate.IsChecked ?? false;
			btnUpdatePerms.IsEnabled = ckbInclUpdate.IsChecked ?? false;
			btnDeletePerms.IsEnabled = ckbInclDelete.IsChecked ?? false;
		}

		private int CountPermissions(string permissions)
		{
			if (string.IsNullOrWhiteSpace(permissions))
				return 0;
			return permissions.Split(new[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries).Length;
		}
	}
}
