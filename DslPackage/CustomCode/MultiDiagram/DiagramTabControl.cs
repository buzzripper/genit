using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Dyvenix.GenIt
{
    /// <summary>
    /// Custom tab control for diagram views, positioned at the bottom of the editor.
    /// Supports VS theming for dark/light modes.
    /// </summary>
    internal class DiagramTabControl : TabControl
    {
        private const int WM_ERASEBKGND = 0x0014;
        private const int WM_PAINT = 0x000F;

        private ContextMenuStrip _tabContextMenu;
        private int _rightClickedTabIndex = -1;
        private Font _tabFont;

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
            this.Height = 36;
            this.Alignment = TabAlignment.Bottom;
            this.SizeMode = TabSizeMode.Normal;
            this.Multiline = false;
            this.HotTrack = true;
            this.DrawMode = TabDrawMode.OwnerDrawFixed;
            this.ItemSize = new Size(120, 28);
            this.Padding = new Point(12, 0);

            // Create a larger font for tabs
            _tabFont = new Font("Segoe UI", 10f, FontStyle.Regular);

            // Enable custom painting
            this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);

            // Subscribe to VS theme changes
            VSColorTheme.ThemeChanged += OnThemeChanged;
            ApplyVsTheme();

            // Create context menu
            CreateContextMenu();
        }

        private void CreateContextMenu()
        {
            _tabContextMenu = new ContextMenuStrip();
            _tabContextMenu.Font = new Font("Segoe UI", 10f, FontStyle.Regular);

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

        /// <summary>
        /// Override WndProc to handle background erasing with theme color.
        /// </summary>
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_ERASEBKGND)
            {
                // Handle background erasing ourselves
                using (var g = Graphics.FromHdc(m.WParam))
                {
                    var backColor = VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowBackgroundColorKey);
                    using (var brush = new SolidBrush(backColor))
                    {
                        g.FillRectangle(brush, ClientRectangle);
                    }
                }
                m.Result = (IntPtr)1; // Indicate we handled it
                return;
            }
            base.WndProc(ref m);
        }

        /// <summary>
        /// Override OnPaint to fully custom draw the tab control.
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            // Get VS theme colors
            var backColor = VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowBackgroundColorKey);
            var foreColor = VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowTextColorKey);
            var selectedBackColor = VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowTabSelectedTabColorKey);
            var borderColor = VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowBorderColorKey);
            var hoverBackColor = VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowTabMouseOverBackgroundBeginColorKey);

            // Fill the entire background
            using (var backBrush = new SolidBrush(backColor))
            {
                e.Graphics.FillRectangle(backBrush, ClientRectangle);
            }

            // Enable text anti-aliasing
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Draw each tab
            for (int i = 0; i < TabCount; i++)
            {
                var tabRect = GetTabRect(i);
                var tabPage = TabPages[i];
                bool isSelected = (SelectedIndex == i);

                // Inflate the rect slightly for better text fit
                var textRect = new Rectangle(tabRect.X + 6, tabRect.Y + 2, tabRect.Width - 12, tabRect.Height - 4);

                // Draw tab background
                Color tabBackColor = isSelected ? selectedBackColor : backColor;
                using (var tabBrush = new SolidBrush(tabBackColor))
                {
                    e.Graphics.FillRectangle(tabBrush, tabRect);
                }

                // Draw tab border
                using (var borderPen = new Pen(borderColor))
                {
                    e.Graphics.DrawRectangle(borderPen, tabRect);
                }

                // Draw tab text with proper formatting
                var textFormat = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisCharacter,
                    FormatFlags = StringFormatFlags.NoWrap
                };

                using (var textBrush = new SolidBrush(foreColor))
                {
                    e.Graphics.DrawString(tabPage.Text, _tabFont, textBrush, textRect, textFormat);
                }
            }
        }

        /// <summary>
        /// Override OnPaintBackground to prevent default background painting.
        /// </summary>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Get VS theme background color and fill the entire background
            var backColor = VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowBackgroundColorKey);
            using (var backBrush = new SolidBrush(backColor))
            {
                e.Graphics.FillRectangle(backBrush, ClientRectangle);
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
                _tabFont?.Dispose();
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
