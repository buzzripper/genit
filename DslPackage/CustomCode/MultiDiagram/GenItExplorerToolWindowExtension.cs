using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Dyvenix.GenIt
{
	/// <summary>
	/// Extension for GenItExplorerToolWindow to support VS theme colors.
	/// </summary>
	internal partial class GenItExplorerToolWindow
	{
		private bool _toolWindowThemeInitialized = false;

		/// <summary>
		/// Called after the tree container is created.
		/// </summary>
		protected override void OnToolWindowCreate()
		{
			base.OnToolWindowCreate();
			InitializeToolWindowTheme();
		}

		/// <summary>
		/// Initializes the VS theme for the tool window.
		/// </summary>
		private void InitializeToolWindowTheme()
		{
			if (_toolWindowThemeInitialized)
				return;

			// Subscribe to theme changes
			VSColorTheme.ThemeChanged += OnToolWindowThemeChanged;

			// Apply initial theme
			ApplyToolWindowTheme();

			_toolWindowThemeInitialized = true;
		}

		private void OnToolWindowThemeChanged(ThemeChangedEventArgs e)
		{
			ApplyToolWindowTheme();
		}

		/// <summary>
		/// Applies the current VS theme to the tool window.
		/// </summary>
		private void ApplyToolWindowTheme()
		{
			try
			{
				// Get VS theme colors
				var backgroundColor = VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowBackgroundColorKey);
				var foregroundColor = VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowTextColorKey);

				// Apply to the window content
				if (this.Content is Control contentControl)
				{
					contentControl.BackColor = backgroundColor;
					contentControl.ForeColor = foregroundColor;
					ApplyThemeRecursively(contentControl, backgroundColor, foregroundColor);
				}

				// Also apply to the tree container if available
				if (this.TreeContainer != null)
				{
					this.TreeContainer.BackColor = backgroundColor;
					this.TreeContainer.ForeColor = foregroundColor;
					ApplyThemeRecursively(this.TreeContainer, backgroundColor, foregroundColor);
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Error applying theme to tool window: {ex.Message}");
			}
		}

		/// <summary>
		/// Recursively applies theme colors to all controls.
		/// </summary>
		private void ApplyThemeRecursively(Control parent, Color backColor, Color foreColor)
		{
			foreach (Control child in parent.Controls)
			{
				// Skip user-defined controls that might have their own theming
				if (child.Tag as string == "NoAutoTheme")
					continue;

				child.BackColor = backColor;
				child.ForeColor = foreColor;

				if (child is TreeView treeView)
				{
					treeView.BackColor = backColor;
					treeView.ForeColor = foreColor;
					treeView.LineColor = VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowBorderColorKey);
					treeView.BorderStyle = BorderStyle.None;
				}
				else if (child is ListView listView)
				{
					listView.BackColor = backColor;
					listView.ForeColor = foreColor;
				}
				else if (child is TextBox textBox)
				{
					textBox.BackColor = VSColorTheme.GetThemedColor(EnvironmentColors.ComboBoxBackgroundColorKey);
					textBox.ForeColor = VSColorTheme.GetThemedColor(EnvironmentColors.ComboBoxTextColorKey);
				}

				if (child.HasChildren)
				{
					ApplyThemeRecursively(child, backColor, foreColor);
				}
			}
		}

		/// <summary>
		/// Clean up when disposing.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				VSColorTheme.ThemeChanged -= OnToolWindowThemeChanged;
			}
			base.Dispose(disposing);
		}
	}
}
