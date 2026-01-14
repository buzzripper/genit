using System.Windows;

namespace Dyvenix.GenIt.DslPackage.Editors.Controls
{
	/// <summary>
	/// Dialog for inputting or editing a string value.
	/// </summary>
	public partial class StringInputDialog : Window
	{
		/// <summary>
		/// Gets or sets the input value.
		/// </summary>
		public string Value
		{
			get { return txtValue.Text; }
			set { txtValue.Text = value; }
		}

		/// <summary>
		/// Gets or sets the label text shown next to the input field.
		/// </summary>
		public string LabelText
		{
			get { return lblInput.Content as string; }
			set { lblInput.Content = value; }
		}

		public StringInputDialog()
		{
			InitializeComponent();
			Loaded += (s, e) => txtValue.Focus();
		}

		private void btnOK_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			Close();
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}
	}
}
