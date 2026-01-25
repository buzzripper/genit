using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace Dyvenix.GenIt.DslPackage.Editors.Permissions
{
	/// <summary>
	/// View model for a permission item in the checked list.
	/// </summary>
	public class PermissionItemViewModel : INotifyPropertyChanged
	{
		private bool _isSelected;

		public Permission Permission { get; set; }

		public bool IsSelected
		{
			get => _isSelected;
			set
			{
				if (_isSelected != value)
				{
					_isSelected = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}

	/// <summary>
	/// Modal dialog for selecting permissions from a checked listbox.
	/// </summary>
	public partial class PermissionsEditorDialog : Window
	{
		private List<PermissionItemViewModel> _items;

		/// <summary>
		/// Gets the selected permission names after the dialog closes with OK.
		/// </summary>
		public List<string> SelectedPermissions { get; private set; }

		public PermissionsEditorDialog()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Initializes the dialog with available permissions from ModelRoot and currently selected ones.
		/// </summary>
		/// <param name="modelRoot">The ModelRoot containing available permissions</param>
		/// <param name="currentPermissions">Comma-separated string of currently selected permissions</param>
		public void Initialize(ModelRoot modelRoot, string currentPermissions)
		{
			var selectedSet = new HashSet<string>(
				PermissionsHelper.ParsePermissions(currentPermissions),
				System.StringComparer.OrdinalIgnoreCase);

			_items = PermissionsHelper.GetPermissions(modelRoot)
				.Select(p => new PermissionItemViewModel
				{
					Permission = p,
					IsSelected = selectedSet.Contains(p.Name)
				})
				.ToList();

			lstPermissions.ItemsSource = _items;
			SelectedPermissions = selectedSet.ToList();
		}

		private void btnOK_Click(object sender, RoutedEventArgs e)
		{
			SelectedPermissions = _items
				.Where(i => i.IsSelected)
				.Select(i => i.Permission.Name)
				.ToList();

			DialogResult = true;
			Close();
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}

		/// <summary>
		/// Shows the dialog and returns the selected permissions as a comma-separated string.
		/// </summary>
		/// <param name="owner">Owner window</param>
		/// <param name="modelRoot">The ModelRoot containing available permissions</param>
		/// <param name="currentPermissions">Current permissions string</param>
		/// <returns>New permissions string if OK was clicked, null if cancelled</returns>
		public static string ShowDialog(Window owner, ModelRoot modelRoot, string currentPermissions)
		{
			var dialog = new PermissionsEditorDialog();
			dialog.Owner = owner;
			dialog.Initialize(modelRoot, currentPermissions);

			if (dialog.ShowDialog() == true)
			{
				return PermissionsHelper.ToPermissionsString(dialog.SelectedPermissions);
			}

			return null;
		}
	}
}
