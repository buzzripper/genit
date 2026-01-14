using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Dyvenix.GenIt.DslPackage.Editors.Controls
{
	/// <summary>
	/// Reusable control for editing a list of strings with add, edit, and remove functionality.
	/// </summary>
	public partial class StringListControl : UserControlBase
	{
		private ObservableCollection<string> _items;

		/// <summary>
		/// Event raised when the items collection changes.
		/// </summary>
		public event EventHandler ItemsChanged;

		/// <summary>
		/// Gets or sets the title displayed above the list.
		/// </summary>
		public string Title
		{
			get { return txtTitle.Text; }
			set { txtTitle.Text = value; }
		}

		/// <summary>
		/// Gets or sets the dialog title used when adding/editing items.
		/// </summary>
		public string DialogTitle { get; set; } = "Add Item";

		/// <summary>
		/// Gets or sets the label text shown in the add/edit dialog.
		/// </summary>
		public string DialogLabel { get; set; } = "Value:";

		/// <summary>
		/// Gets the current items as a list.
		/// </summary>
		public List<string> Items
		{
			get { return new List<string>(_items); }
		}

		public StringListControl()
		{
			InitializeComponent();
			_items = new ObservableCollection<string>();
			lstItems.ItemsSource = _items;
		}

		/// <summary>
		/// Sets the items in the list.
		/// </summary>
		/// <param name="items">The items to display.</param>
		public void SetItems(IEnumerable<string> items)
		{
			_items.Clear();
			if (items != null)
			{
				foreach (var item in items)
				{
					_items.Add(item);
				}
			}
		}

		/// <summary>
		/// Clears all items from the list.
		/// </summary>
		public void Clear()
		{
			_items.Clear();
			OnItemsChanged();
		}

		private void btnAdd_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new StringInputDialog();
			dialog.Title = DialogTitle;
			dialog.LabelText = DialogLabel;
			dialog.Owner = Window.GetWindow(this);

			if (dialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(dialog.Value))
			{
				_items.Add(dialog.Value.Trim());
				OnItemsChanged();
			}
		}

		private void btnDelete_Click(object sender, RoutedEventArgs e)
		{
			if (lstItems.SelectedItem == null)
				return;

			var selectedItem = lstItems.SelectedItem as string;
			if (selectedItem != null)
			{
				_items.Remove(selectedItem);
				OnItemsChanged();
			}
		}

		private void lstItems_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (lstItems.SelectedItem == null)
				return;

			var selectedItem = lstItems.SelectedItem as string;
			if (selectedItem == null)
				return;

			int selectedIndex = lstItems.SelectedIndex;

			var dialog = new StringInputDialog();
			dialog.Title = "Edit " + DialogTitle.Replace("Add ", "");
			dialog.LabelText = DialogLabel;
			dialog.Value = selectedItem;
			dialog.Owner = Window.GetWindow(this);

			if (dialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(dialog.Value))
			{
				_items[selectedIndex] = dialog.Value.Trim();
				OnItemsChanged();
			}
		}

		private void OnItemsChanged()
		{
			ItemsChanged?.Invoke(this, EventArgs.Empty);
		}
	}
}
