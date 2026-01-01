using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Dyvenix.GenIt
{
	/// <summary>
	/// Dialog for adding or renaming a view.
	/// </summary>
	internal class AddViewDialog : Form
	{
		private TextBox _nameTextBox;
		private Button _okButton;
		private Button _cancelButton;
		private Label _nameLabel;

		/// <summary>
		/// Gets the view name entered by the user.
		/// </summary>
		public string ViewName => _nameTextBox?.Text?.Trim();

		/// <summary>
		/// Creates a new AddViewDialog for adding a view.
		/// </summary>
		public AddViewDialog() : this(string.Empty)
		{
		}

		/// <summary>
		/// Creates a new AddViewDialog with a pre-filled name (for renaming).
		/// </summary>
		public AddViewDialog(string currentName)
		{
			InitializeComponent();
			_nameTextBox.Text = currentName ?? string.Empty;
			ApplyVsTheme();

			VSColorTheme.ThemeChanged += OnThemeChanged;
		}

		private void InitializeComponent()
		{
			this.Text = "Add View";
			this.Size = new Size(350, 140);
			this.MinimumSize = new Size(300, 130);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.ShowInTaskbar = false;
			this.StartPosition = FormStartPosition.CenterParent;
			this.FormBorderStyle = FormBorderStyle.FixedDialog;

			_nameLabel = new Label
			{
				Text = "View name:",
				Location = new Point(12, 18),
				Size = new Size(70, 20),
				TextAlign = ContentAlignment.MiddleLeft
			};

			_nameTextBox = new TextBox
			{
				Location = new Point(90, 15),
				Size = new Size(230, 23),
				Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
			};
			_nameTextBox.KeyDown += (s, e) =>
			{
				if (e.KeyCode == Keys.Enter && !string.IsNullOrWhiteSpace(_nameTextBox.Text))
				{
					this.DialogResult = DialogResult.OK;
					this.Close();
				}
			};

			_okButton = new Button
			{
				Text = "OK",
				DialogResult = DialogResult.OK,
				Location = new Point(160, 55),
				Size = new Size(75, 28)
			};

			_cancelButton = new Button
			{
				Text = "Cancel",
				DialogResult = DialogResult.Cancel,
				Location = new Point(245, 55),
				Size = new Size(75, 28)
			};

			this.Controls.Add(_nameLabel);
			this.Controls.Add(_nameTextBox);
			this.Controls.Add(_okButton);
			this.Controls.Add(_cancelButton);

			this.AcceptButton = _okButton;
			this.CancelButton = _cancelButton;
		}

		private void OnThemeChanged(ThemeChangedEventArgs e)
		{
			ApplyVsTheme();
		}

		private void ApplyVsTheme()
		{
			BackColor = VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowBackgroundColorKey);
			ForeColor = VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowTextColorKey);

			_nameLabel.ForeColor = ForeColor;

			_nameTextBox.BackColor = VSColorTheme.GetThemedColor(EnvironmentColors.ComboBoxBackgroundColorKey);
			_nameTextBox.ForeColor = VSColorTheme.GetThemedColor(EnvironmentColors.ComboBoxTextColorKey);

			_okButton.BackColor = VSColorTheme.GetThemedColor(EnvironmentColors.ComboBoxBackgroundColorKey);
			_okButton.ForeColor = VSColorTheme.GetThemedColor(EnvironmentColors.ComboBoxTextColorKey);
			_okButton.FlatStyle = FlatStyle.Flat;

			_cancelButton.BackColor = VSColorTheme.GetThemedColor(EnvironmentColors.ComboBoxBackgroundColorKey);
			_cancelButton.ForeColor = VSColorTheme.GetThemedColor(EnvironmentColors.ComboBoxTextColorKey);
			_cancelButton.FlatStyle = FlatStyle.Flat;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				VSColorTheme.ThemeChanged -= OnThemeChanged;
			}
			base.Dispose(disposing);
		}

		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);
			_nameTextBox.Focus();
			_nameTextBox.SelectAll();
		}
	}
}
