using System;
using System.Windows.Forms;

namespace Dyvenix.GenIt
{
	/// <summary>
	/// Extended GenItDocView that supports multiple diagram views with tabs at the bottom.
	/// </summary>
	internal partial class GenItDocView
	{
		private DiagramTabControl _tabControl;
		private DiagramManager _diagramManager;
		private TabbedDiagramContainer _container;

		/// <summary>
		/// Gets the diagram manager for this view.
		/// </summary>
		public DiagramManager DiagramManager => _diagramManager;

		/// <summary>
		/// Returns a custom container that includes both the diagram and tabs.
		/// </summary>
		public override IWin32Window Window
		{
			get
			{
				if (_container == null)
				{
					// Create our custom container with the base window
					_container = new TabbedDiagramContainer(this, (Control)base.Window);
				}
				return _container;
			}
		}

		/// <summary>
		/// Called to initialize the view after the corresponding document has been loaded.
		/// </summary>
		protected override bool LoadView()
		{
			System.Diagnostics.Debug.Assert(this.DocData.RootElement != null);
			if (this.DocData.RootElement == null)
			{
				return false;
			}

			var docData = this.DocData as GenItDocDataBase;
			System.Diagnostics.Debug.Assert(docData != null, "DocData should be GenItDocDataBase!");

			var diagramPartition = docData.GetDiagramPartition();
			if (diagramPartition == null)
			{
				return false;
			}

			// Initialize the diagram manager
			var modelRoot = this.DocData.RootElement as ModelRoot;
			if (modelRoot == null)
			{
				return false;
			}

			_diagramManager = new DiagramManager(docData.Store, modelRoot, diagramPartition);
			_diagramManager.Initialize();
			_diagramManager.ActiveDiagramChanged += OnActiveDiagramChanged;

			// Set the initial diagram
			if (_diagramManager.ActiveDiagram != null)
			{
				this.Diagram = _diagramManager.ActiveDiagram;
			}
			else
			{
				return false;
			}

			// Initialize the container's tab control after diagram manager is ready
			if (_container != null)
			{
				_container.InitializeTabs(_diagramManager);
			}

			return true;
		}

		private void OnActiveDiagramChanged(object sender, DiagramChangedEventArgs e)
		{
			if (e.NewDiagram != null)
			{
				this.Diagram = e.NewDiagram;
			}
		}
	}

	/// <summary>
	/// Custom container that holds both the DSL diagram and tab control.
	/// Does NOT interfere with the built-in drag-drop handling for toolbox items.
	/// </summary>
	internal class TabbedDiagramContainer : Panel
	{
		private readonly GenItDocView _docView;
		private readonly Control _diagramControl;
		private DiagramTabControl _tabControl;
		private DiagramManager _diagramManager;

		public TabbedDiagramContainer(GenItDocView docView, Control diagramControl)
		{
			_docView = docView;
			_diagramControl = diagramControl;

			// Set up the layout
			this.Dock = DockStyle.Fill;

			// DO NOT enable drag-drop on the container - let the diagram handle its own drag-drop
			// This preserves the built-in toolbox drag-drop functionality

			// Add the diagram control (it already has its own drag-drop handlers)
			_diagramControl.Dock = DockStyle.Fill;
			this.Controls.Add(_diagramControl);

			// Create and add the tab control at the bottom
			_tabControl = new DiagramTabControl();
			_tabControl.AddTabRequested += OnAddTabRequested;
			_tabControl.RenameTabRequested += OnRenameTabRequested;
			_tabControl.DeleteTabRequested += OnDeleteTabRequested;
			_tabControl.SelectedIndexChanged += OnTabSelectedIndexChanged;

			this.Controls.Add(_tabControl);

			// Add a default tab initially
			_tabControl.TabPages.Add(new TabPage("Default"));
		}

		/// <summary>
		/// Initializes the tabs based on the diagram manager.
		/// </summary>
		public void InitializeTabs(DiagramManager diagramManager)
		{
			_diagramManager = diagramManager;
			RefreshTabs();
		}

		private void RefreshTabs()
		{
			if (_tabControl == null || _diagramManager == null)
				return;

			_tabControl.SelectedIndexChanged -= OnTabSelectedIndexChanged;
			_tabControl.TabPages.Clear();

			foreach (var diagram in _diagramManager.Diagrams)
			{
				var tabPage = new TabPage(diagram.Name ?? "Unnamed");
				tabPage.Tag = diagram;
				_tabControl.TabPages.Add(tabPage);
			}

			if (_diagramManager.ActiveDiagramIndex >= 0 && _diagramManager.ActiveDiagramIndex < _tabControl.TabCount)
			{
				_tabControl.SelectedIndex = _diagramManager.ActiveDiagramIndex;
			}

			_tabControl.SelectedIndexChanged += OnTabSelectedIndexChanged;
		}

		private void OnTabSelectedIndexChanged(object sender, EventArgs e)
		{
			if (_tabControl == null || _diagramManager == null)
				return;

			if (_tabControl.SelectedIndex >= 0 && _tabControl.SelectedIndex != _diagramManager.ActiveDiagramIndex)
			{
				_diagramManager.ActiveDiagramIndex = _tabControl.SelectedIndex;
			}
		}

		private void OnAddTabRequested(object sender, EventArgs e)
		{
			if (_diagramManager == null)
				return;

			using (var dialog = new AddViewDialog())
			{
				if (dialog.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.ViewName))
				{
					try
					{
						_diagramManager.CreateDiagram(dialog.ViewName, showAllElements: false);
						RefreshTabs();

						// Switch to the new tab
						_tabControl.SelectedIndex = _tabControl.TabCount - 1;
					}
					catch (Exception ex)
					{
						MessageBox.Show($"Error creating view: {ex.Message}", "Error",
							MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
			}
		}

		private void OnRenameTabRequested(object sender, TabEventArgs e)
		{
			if (_diagramManager == null)
				return;

			using (var dialog = new AddViewDialog(e.TabName))
			{
				dialog.Text = "Rename View";
				if (dialog.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.ViewName))
				{
					try
					{
						_diagramManager.RenameDiagram(e.TabIndex, dialog.ViewName);
						RefreshTabs();
					}
					catch (Exception ex)
					{
						MessageBox.Show($"Error renaming view: {ex.Message}", "Error",
							MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
			}
		}

		private void OnDeleteTabRequested(object sender, TabEventArgs e)
		{
			if (_diagramManager == null)
				return;

			var result = MessageBox.Show(
				$"Are you sure you want to delete the view '{e.TabName}'?\n\nThis will remove the view but not the model elements.",
				"Delete View",
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Question);

			if (result == DialogResult.Yes)
			{
				try
				{
					_diagramManager.RemoveDiagram(e.TabIndex);
					RefreshTabs();
				}
				catch (Exception ex)
				{
					MessageBox.Show($"Error deleting view: {ex.Message}", "Error",
						MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}
	}
}
