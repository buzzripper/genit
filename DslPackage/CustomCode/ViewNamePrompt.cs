using System;
using System.Drawing;
using System.Windows.Forms;

namespace Dyvenix.GenIt
{
	/// <summary>
	/// Simple modal dialog used to prompt the user for a diagram view name.
	/// </summary>
	internal sealed class ViewNamePrompt : Form
	{
		private readonly TextBox _textBox;

		private ViewNamePrompt(string title, string prompt, string defaultValue)
		{
			this.Text = title;
			this.FormBorderStyle = FormBorderStyle.FixedDialog;
			this.StartPosition = FormStartPosition.CenterParent;
			this.MinimizeBox = false;
			this.MaximizeBox = false;
			this.ShowInTaskbar = false;
			this.ClientSize = new Size(360, 120);

			Label label = new Label
			{
				Text = prompt,
				AutoSize = false,
				Location = new Point(12, 12),
				Size = new Size(336, 20)
			};

			_textBox = new TextBox
			{
				Text = defaultValue ?? string.Empty,
				Location = new Point(12, 36),
				Size = new Size(336, 23),
				Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
			};
			_textBox.SelectAll();

			Button okButton = new Button
			{
				Text = "OK",
				DialogResult = DialogResult.OK,
				Location = new Point(192, 80),
				Size = new Size(75, 25),
				Anchor = AnchorStyles.Bottom | AnchorStyles.Right
			};

			Button cancelButton = new Button
			{
				Text = "Cancel",
				DialogResult = DialogResult.Cancel,
				Location = new Point(273, 80),
				Size = new Size(75, 25),
				Anchor = AnchorStyles.Bottom | AnchorStyles.Right
			};

			this.Controls.Add(label);
			this.Controls.Add(_textBox);
			this.Controls.Add(okButton);
			this.Controls.Add(cancelButton);
			this.AcceptButton = okButton;
			this.CancelButton = cancelButton;
		}

		/// <summary>
		/// Shows the prompt and returns the trimmed entered name, or null when cancelled or empty.
		/// </summary>
		public static string Show(string title, string prompt, string defaultValue)
		{
			using (ViewNamePrompt dialog = new ViewNamePrompt(title, prompt, defaultValue))
			{
				if (dialog.ShowDialog() != DialogResult.OK)
					return null;

				string result = dialog._textBox.Text?.Trim();
				return string.IsNullOrEmpty(result) ? null : result;
			}
		}
	}
}
