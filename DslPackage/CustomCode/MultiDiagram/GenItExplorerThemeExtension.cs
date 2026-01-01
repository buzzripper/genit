using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Shell;
using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace Dyvenix.GenIt
{
	/// <summary>
	/// Extension for GenItExplorer to support VS theme colors and drag-drop.
	/// </summary>
	internal partial class GenItExplorer
	{
		private bool _themeInitialized = false;
		private TreeView _treeView;
		private bool _dragInitialized = false;

		/// <summary>
		/// Called when the explorer is created and shown.
		/// </summary>
		protected override void OnCreateControl()
		{
			base.OnCreateControl();
			InitializeTheme();
			InitializeDragDrop();
		}

		#region Drag-Drop Support

		/// <summary>
		/// Initializes drag-drop support for the explorer TreeView.
		/// </summary>
		private void InitializeDragDrop()
		{
			if (_dragInitialized)
				return;

			if (_treeView == null)
			{
				_treeView = FindTreeView();
			}

			if (_treeView != null)
			{
				_treeView.ItemDrag += OnTreeViewItemDrag;
				_dragInitialized = true;
			}
		}

		/// <summary>
		/// Handles the ItemDrag event to start a drag operation from the explorer.
		/// </summary>
		private void OnTreeViewItemDrag(object sender, ItemDragEventArgs e)
		{
			if (e.Item is TreeNode node)
			{
				// Get the model element from the tree node
				ModelElement element = GetModelElementFromNode(node);
				if (element != null && CanDragElement(element))
				{
					// Create an ElementGroupPrototype - this is what the DSL framework uses for drag-drop
					var dataObject = CreateDragDataObject(element);
					if (dataObject != null)
					{
						// Start the drag operation
						DoDragDrop(dataObject, DragDropEffects.Copy | DragDropEffects.Move);
					}
				}
			}
		}

		/// <summary>
		/// Determines if an element can be dragged to a diagram.
		/// </summary>
		private bool CanDragElement(ModelElement element)
		{
			// Only allow dragging of elements that have shapes
			return element is EntityModel
				|| element is EnumModel
				|| element is ModelInterface
				|| element is Comment;
		}

		/// <summary>
		/// Creates a data object for dragging that the diagram can understand.
		/// </summary>
		private IDataObject CreateDragDataObject(ModelElement element)
		{
			try
			{
				// Create an ElementGroup containing the element
				var elementGroup = new ElementGroup(element.Store);
				elementGroup.Add(element);

				// Mark elements as already created (we're not creating new ones)
				elementGroup.MarkAsRoot(element);

				// Create a prototype from the group
				var prototype = elementGroup.CreatePrototype();

				// Create a DataObject with the prototype
				var dataObject = new DataObject();
				dataObject.SetData(typeof(ElementGroupPrototype), prototype);

				// Also add a reference to the actual element for our custom handling
				dataObject.SetData("GenItModelElement", element);

				return dataObject;
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Error creating drag data: {ex.Message}");
				return null;
			}
		}

		/// <summary>
		/// Gets the model element from a tree node.
		/// </summary>
		private ModelElement GetModelElementFromNode(TreeNode node)
		{
			// The explorer tree node's Tag contains the represented element
			// Try direct cast first
			if (node.Tag is ModelElement element)
			{
				return element;
			}

			// Try to get from ExplorerElementNode via reflection
			try
			{
				var nodeType = node.GetType();

				// Try RepresentedElement property
				var repElementProp = nodeType.GetProperty("RepresentedElement",
					BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
				if (repElementProp != null)
				{
					return repElementProp.GetValue(node) as ModelElement;
				}

				// Try Element property
				var elementProp = nodeType.GetProperty("Element",
					BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
				if (elementProp != null)
				{
					return elementProp.GetValue(node) as ModelElement;
				}

				// Try ModelElement property
				var modelElementProp = nodeType.GetProperty("ModelElement",
					BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
				if (modelElementProp != null)
				{
					return modelElementProp.GetValue(node) as ModelElement;
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Error getting element from node: {ex.Message}");
			}

			return null;
		}

		#endregion

		#region Theme Support

		/// <summary>
		/// Initializes the VS theme colors for the explorer.
		/// </summary>
		private void InitializeTheme()
		{
			if (_themeInitialized)
				return;

			// Subscribe to theme changes
			VSColorTheme.ThemeChanged += OnVsThemeChanged;

			// Apply initial theme
			ApplyVsTheme();

			_themeInitialized = true;
		}

		private void OnVsThemeChanged(ThemeChangedEventArgs e)
		{
			ApplyVsTheme();
		}

		/// <summary>
		/// Applies the current VS theme to the explorer controls.
		/// </summary>
		private void ApplyVsTheme()
		{
			try
			{
				// Get VS theme colors
				var backgroundColor = VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowBackgroundColorKey);
				var foregroundColor = VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowTextColorKey);

				// Apply to this control
				this.BackColor = backgroundColor;
				this.ForeColor = foregroundColor;

				// Apply to all child controls
				ApplyThemeToChildren(this, backgroundColor, foregroundColor);

				// Try to get the TreeView and apply theme
				if (_treeView == null)
				{
					_treeView = FindTreeView();
				}

				if (_treeView != null)
				{
					ApplyThemeToTreeView(_treeView, backgroundColor, foregroundColor);
				}

				this.Invalidate(true);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Error applying theme to explorer: {ex.Message}");
			}
		}

		/// <summary>
		/// Finds the TreeView control in the explorer.
		/// </summary>
		private TreeView FindTreeView()
		{
			// Try to get TreeView via reflection from the base class
			try
			{
				var treeViewProperty = typeof(ModelExplorerTreeContainer).GetProperty("ObjectModelBrowser",
					BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

				if (treeViewProperty != null)
				{
					return treeViewProperty.GetValue(this) as TreeView;
				}

				// Also try the TreeView field directly
				var treeViewField = typeof(ModelExplorerTreeContainer).GetField("objectModelBrowser",
					BindingFlags.NonPublic | BindingFlags.Instance);

				if (treeViewField != null)
				{
					return treeViewField.GetValue(this) as TreeView;
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Could not access TreeView via reflection: {ex.Message}");
			}

			// Fallback: search controls
			return FindTreeViewInControls(this);
		}

		private TreeView FindTreeViewInControls(Control parent)
		{
			foreach (Control child in parent.Controls)
			{
				if (child is TreeView tv)
					return tv;

				var found = FindTreeViewInControls(child);
				if (found != null)
					return found;
			}
			return null;
		}

		/// <summary>
		/// Applies theme colors to all child controls.
		/// </summary>
		private void ApplyThemeToChildren(Control parent, Color backColor, Color foreColor)
		{
			foreach (Control child in parent.Controls)
			{
				child.BackColor = backColor;
				child.ForeColor = foreColor;

				// Special handling for TreeView
				if (child is TreeView treeView)
				{
					ApplyThemeToTreeView(treeView, backColor, foreColor);
				}

				// Recurse into children
				if (child.HasChildren)
				{
					ApplyThemeToChildren(child, backColor, foreColor);
				}
			}
		}

		/// <summary>
		/// Applies theme to a TreeView control with proper selection colors.
		/// </summary>
		private void ApplyThemeToTreeView(TreeView treeView, Color backColor, Color foreColor)
		{
			treeView.BackColor = backColor;
			treeView.ForeColor = foreColor;

			// Set line color for the tree lines to be visible
			var lineColor = VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowBorderColorKey);
			treeView.LineColor = lineColor;

			// BorderStyle
			treeView.BorderStyle = BorderStyle.None;

			// Enable owner draw to handle selection colors properly
			if (treeView.DrawMode != TreeViewDrawMode.OwnerDrawText)
			{
				treeView.DrawMode = TreeViewDrawMode.OwnerDrawText;
				treeView.DrawNode -= OnTreeViewDrawNode;
				treeView.DrawNode += OnTreeViewDrawNode;
			}

			// Make sure HideSelection is false so we can see selection when not focused
			treeView.HideSelection = false;
		}

		/// <summary>
		/// Custom draw handler for tree nodes to fix selection colors in dark mode.
		/// </summary>
		private void OnTreeViewDrawNode(object sender, DrawTreeNodeEventArgs e)
		{
			if (e.Node == null)
				return;

			// Get theme colors
			var backColor = VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowBackgroundColorKey);
			var foreColor = VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowTextColorKey);
			var selectedBackColor = VSColorTheme.GetThemedColor(EnvironmentColors.SystemHighlightColorKey);
			var selectedForeColor = VSColorTheme.GetThemedColor(EnvironmentColors.SystemHighlightTextColorKey);

			bool isSelected = (e.State & TreeNodeStates.Selected) != 0;
			bool isFocused = (e.State & TreeNodeStates.Focused) != 0;
			bool treeViewFocused = e.Node.TreeView?.Focused ?? false;

			Color nodeBackColor;
			Color nodeForeColor;

			if (isSelected)
			{
				if (treeViewFocused)
				{
					// Active selection
					nodeBackColor = selectedBackColor;
					nodeForeColor = selectedForeColor;
				}
				else
				{
					// Inactive selection - use a slightly lighter/darker version of background
					var borderColor = VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowBorderColorKey);
					nodeBackColor = borderColor;
					nodeForeColor = foreColor;
				}
			}
			else
			{
				nodeBackColor = backColor;
				nodeForeColor = foreColor;
			}

			// Draw background
			using (var brush = new SolidBrush(nodeBackColor))
			{
				e.Graphics.FillRectangle(brush, e.Bounds);
			}

			// Draw text
			TextRenderer.DrawText(
				e.Graphics,
				e.Node.Text,
				e.Node.TreeView?.Font ?? SystemFonts.DefaultFont,
				e.Bounds,
				nodeForeColor,
				nodeBackColor,
				TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.NoPrefix);

			// Draw focus rectangle if focused
			if (isFocused && treeViewFocused)
			{
				ControlPaint.DrawFocusRectangle(e.Graphics, e.Bounds, nodeForeColor, nodeBackColor);
			}
		}

		#endregion

		/// <summary>
		/// Disposes the explorer and unsubscribes from events.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				VSColorTheme.ThemeChanged -= OnVsThemeChanged;

				if (_treeView != null)
				{
					_treeView.DrawNode -= OnTreeViewDrawNode;
					_treeView.ItemDrag -= OnTreeViewItemDrag;
				}
			}
			base.Dispose(disposing);
		}
	}
}
