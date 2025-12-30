using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Dyvenix.GenIt
{
	/// <summary>
	/// Custom tab control for diagram views, positioned at the bottom of the editor.
	/// Supports VS theming for dark/light modes.
	/// </summary>
	internal class DiagramTabControl : TabControl
	{
		private ContextMenuStrip _tabContextMenu;
		private int _rightClickedTabIndex = -1;

		/// <summary>
		/// Event raised when user requests to add a new tab.
		/// </summary>
		public event EventHandler AddTabRequested;

		/// <summary>
		/// Event raised when user requests to rename a tab.
		/// </summary>
		public event EventHandler<TabEventArgs> RenameTabRequested;

		/// <summary>
		/// Event raised when user requests to delete a tab.
		/// </summary>
		public event EventHandler<TabEventArgs> DeleteTabRequested;

		public DiagramTabControl()
		{
			this.Dock = DockStyle.Bottom;
			this.Height = 24;
			this.Alignment = TabAlignment.Bottom;
			this.SizeMode = TabSizeMode.Normal;
			this.Multiline = false;
			this.HotTrack = true;
			this.DrawMode = TabDrawMode.OwnerDrawFixed;
			this.ItemSize = new Size(80, 20);

			// Subscribe to VS theme changes
			VSColorTheme.ThemeChanged += OnThemeChanged;
			ApplyVsTheme();

			// Create context menu
			CreateContextMenu();

			// Handle custom drawing for theme support
			this.DrawItem += DiagramTabControl_DrawItem;
		}

		private void CreateContextMenu()
		{
			_tabContextMenu = new ContextMenuStrip();

			var addItem = new ToolStripMenuItem("Add View...");
			addItem.Click += (s, e) => AddTabRequested?.Invoke(this, EventArgs.Empty);

			var renameItem = new ToolStripMenuItem("Rename View...");
			renameItem.Click += (s, e) =>
			{
				if (_rightClickedTabIndex >= 0 && _rightClickedTabIndex < TabCount)
				{
					RenameTabRequested?.Invoke(this, new TabEventArgs(_rightClickedTabIndex, TabPages[_rightClickedTabIndex].Text));
				}
			};

			var deleteItem = new ToolStripMenuItem("Delete View");
			deleteItem.Click += (s, e) =>
			{
				if (_rightClickedTabIndex >= 0 && _rightClickedTabIndex < TabCount)
				{
					// Don't allow deleting the last tab (Default view)
					if (TabCount > 1)
					{
						DeleteTabRequested?.Invoke(this, new TabEventArgs(_rightClickedTabIndex, TabPages[_rightClickedTabIndex].Text));
					}
					else
					{
						MessageBox.Show("Cannot delete the last view.", "Delete View",
							MessageBoxButtons.OK, MessageBoxIcon.Warning);
					}
				}
			};

			_tabContextMenu.Items.Add(addItem);
			_tabContextMenu.Items.Add(new ToolStripSeparator());
			_tabContextMenu.Items.Add(renameItem);
			_tabContextMenu.Items.Add(deleteItem);
		}

		protected override void OnMouseClick(MouseEventArgs e)
		{
			base.OnMouseClick(e);

			if (e.Button == MouseButtons.Right)
			{
				for (int i = 0; i < TabCount; i++)
				{
					if (GetTabRect(i).Contains(e.Location))
					{
						_rightClickedTabIndex = i;

						// Disable delete for "Default" tab (index 0) or if it's the only tab
						var deleteItem = _tabContextMenu.Items[3] as ToolStripMenuItem;
						if (deleteItem != null)
						{
							deleteItem.Enabled = i > 0 && TabCount > 1;
						}

						_tabContextMenu.Show(this, e.Location);
						return;
					}
				}
			}
		}

		private void DiagramTabControl_DrawItem(object sender, DrawItemEventArgs e)
		{
			if (e.Index < 0 || e.Index >= TabCount)
				return;

			var tabPage = TabPages[e.Index];
			var tabRect = GetTabRect(e.Index);

			// Get VS theme colors
			var backColor = VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowBackgroundColorKey);
			var foreColor = VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowTextColorKey);
			var selectedBackColor = VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowTabSelectedTabColorKey);

			bool isSelected = (SelectedIndex == e.Index);

			using (var backBrush = new SolidBrush(isSelected ? selectedBackColor : backColor))
			{
				e.Graphics.FillRectangle(backBrush, tabRect);
			}

			// Draw text
			var textFormat = new StringFormat
			{
				Alignment = StringAlignment.Center,
				LineAlignment = StringAlignment.Center
			};

			using (var textBrush = new SolidBrush(foreColor))
			{
				e.Graphics.DrawString(tabPage.Text, Font, textBrush, tabRect, textFormat);
			}

			// Draw border
			var borderColor = VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowBorderColorKey);
			using (var borderPen = new Pen(borderColor))
			{
				e.Graphics.DrawRectangle(borderPen, tabRect);
			}
		}

		private void OnThemeChanged(ThemeChangedEventArgs e)
		{
			ApplyVsTheme();
			Invalidate();
		}

		private void ApplyVsTheme()
		{
			BackColor = VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowBackgroundColorKey);
			ForeColor = VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowTextColorKey);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				VSColorTheme.ThemeChanged -= OnThemeChanged;
				_tabContextMenu?.Dispose();
			}
			base.Dispose(disposing);
		}
	}

	/// <summary>
	/// Event args for tab-related events.
	/// </summary>
	internal class TabEventArgs : EventArgs
	{
		public int TabIndex { get; }
		public string TabName { get; }

		public TabEventArgs(int tabIndex, string tabName)
		{
			TabIndex = tabIndex;
			TabName = tabName;
		}
	}
}
