using Dyvenix.GenIt.DslPackage.CustomCode;
using Dyvenix.GenIt.DslPackage.Editors.Controls;
using Dyvenix.GenIt.DslPackage.Editors;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Threading;
using System;
using System.Drawing;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Threading;
using System.Threading.Tasks;
using VSShell = Microsoft.VisualStudio.Shell;

namespace Dyvenix.GenIt
{
	/// <summary>
	/// Customization for the GenItPackage to handle solution events.
	/// </summary>
	internal sealed partial class GenItPackage : IVsSolutionEvents
	{
		private IVsSolution _solution;
		private uint _solutionEventsCookie;
		private SelectionTracker _selectionTracker;


		/// <summary>
		/// Override to perform additional initialization after the base package is initialized.
		/// </summary>
		protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<VSShell.ServiceProgressData> progress)
		{
			await base.InitializeAsync(cancellationToken, progress);

			await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

			VsServices.Initialize(this);

			// Get the solution service and subscribe to solution events
			_solution = await GetServiceAsync(typeof(SVsSolution)) as IVsSolution;
			if (_solution != null)
			{
				_solution.AdviseSolutionEvents(this, out _solutionEventsCookie);

			// Check if a solution is already open
				if (_solution.GetSolutionInfo(out string solutionDirectory, out string solutionFile, out string userOptsFile) == VSConstants.S_OK
					&& !string.IsNullOrEmpty(solutionDirectory))
				{
					PackageUtils.SolutionRootPath = solutionDirectory;
					SolutionRootCache.Set(solutionDirectory);
					System.Diagnostics.Debug.WriteLine($"GenItPackage.InitializeAsync: Solution already open, set SolutionRootPath to '{solutionDirectory}'");
				}
			}

			// Initialize the selection tracker for the GenItEditorWindow
			_selectionTracker = new SelectionTracker(this);
		}

		protected override void Dispose(bool disposing)
		{
			VSShell.ThreadHelper.ThrowIfNotOnUIThread();

			// Dispose the selection tracker
			_selectionTracker?.Dispose();
			_selectionTracker = null;

			// Unsubscribe from solution events
			if (_solutionEventsCookie != 0 && _solution != null)
			{
				_solution.UnadviseSolutionEvents(_solutionEventsCookie);
				_solutionEventsCookie = 0;
			}

			base.Dispose(disposing);
		}

		#region IVsSolutionEvents Implementation

		public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			_ = ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
			{
				// Switch when needed
				await TaskScheduler.Default;

				// background work here (I/O, parsing, etc.)

				await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

				// Get the solution's root folder path
				if (_solution != null)
				{
					if (_solution.GetSolutionInfo(out string solutionDirectory, out string solutionFile, out string userOptsFile) == VSConstants.S_OK)
					{
						PackageUtils.SolutionRootPath = solutionDirectory;
						SolutionRootCache.Set(solutionDirectory);
						System.Diagnostics.Debug.WriteLine($"GenItPackage.OnAfterOpenSolution: Set SolutionRootPath to '{solutionDirectory}'");
					}
				}
			});

			return VSConstants.S_OK;
		}

		public int OnAfterCloseSolution(object pUnkReserved)
		{
			SolutionRootCache.Set(null);
			PackageUtils.SolutionRootPath = null;

			Debug.WriteLine("GenItPackage.OnAfterCloseSolution: Cleared SolutionRootPath");

			return VSConstants.S_OK;
		}

		public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
		{
			return VSConstants.S_OK;
		}

		public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
		{
			return VSConstants.S_OK;
		}

		public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
		{
			return VSConstants.S_OK;
		}

		public int OnBeforeCloseSolution(object pUnkReserved)
		{
			return VSConstants.S_OK;
		}

		public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
		{
			return VSConstants.S_OK;
		}

		public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
		{
			return VSConstants.S_OK;
		}

		public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
		{
			return VSConstants.S_OK;
		}

		public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
		{
			return VSConstants.S_OK;
		}

		#endregion
	}

	internal partial class GenItDocView
	{
		private bool _appliedInitialView;
		private bool _isRefreshingViewHeader;
		private DiagramViewContainer _windowContainer;
		private Panel _viewHeaderPanel;
		private ComboBox _viewSelector;
		private Button _addViewButton;
		private Button _deleteViewButton;
		private DiagramViewState _subscribedDiagramViews;

		public override System.Windows.Forms.IWin32Window Window
		{
			get
			{
				if (_windowContainer == null)
				{
					var baseWindow = base.Window as Control;
					if (baseWindow == null)
					{
						return base.Window;
					}

					_windowContainer = new DiagramViewContainer(baseWindow);
					InitializeViewHeader(_windowContainer.HeaderHost);
					RefreshViewHeader();
				}

				return _windowContainer;
			}
		}

		protected override bool LoadView()
		{
			var result = base.LoadView();
			if (result)
			{
				RefreshViewHeader();
			}

			return result;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && _subscribedDiagramViews != null)
			{
				_subscribedDiagramViews.ViewsChanged -= OnDiagramViewsChanged;
				_subscribedDiagramViews = null;
			}

			base.Dispose(disposing);
		}

		private void InitializeViewHeader(Control headerHost)
		{
			if (_viewHeaderPanel != null)
			{
				return;
			}

			_viewHeaderPanel = new Panel
			{
				Dock = DockStyle.Top,
				Height = 32,
				Padding = new Padding(6, 4, 6, 4)
			};

			var viewsLabel = new Label
			{
				AutoSize = true,
				Text = "View:",
				TextAlign = ContentAlignment.MiddleLeft,
				Margin = new Padding(0, 6, 6, 0)
			};

			_viewSelector = new ComboBox
			{
				DropDownStyle = ComboBoxStyle.DropDownList,
				Width = 220,
				Margin = new Padding(0, 2, 8, 0)
			};
			_viewSelector.SelectedIndexChanged += OnViewSelectorSelectedIndexChanged;

			_addViewButton = new Button
			{
				AutoSize = true,
				Text = "Add View",
				Margin = new Padding(0, 0, 6, 0)
			};
			_addViewButton.Click += OnAddViewButtonClick;

			_deleteViewButton = new Button
			{
				AutoSize = true,
				Text = "Delete View"
			};
			_deleteViewButton.Click += OnDeleteViewButtonClick;

			var host = new FlowLayoutPanel
			{
				Dock = DockStyle.Fill,
				FlowDirection = FlowDirection.LeftToRight,
				WrapContents = false
			};

			host.Controls.Add(viewsLabel);
			host.Controls.Add(_viewSelector);
			host.Controls.Add(_addViewButton);
			host.Controls.Add(_deleteViewButton);

			_viewHeaderPanel.Controls.Add(host);
			headerHost.Controls.Add(_viewHeaderPanel);
			_viewHeaderPanel.BringToFront();
		}

		private void RefreshViewHeader()
		{
			if (_viewHeaderPanel == null)
			{
				var windowContainer = this.Window as DiagramViewContainer;
				if (windowContainer == null)
				{
					return;
				}

				InitializeViewHeader(windowContainer.HeaderHost);
			}

			var docData = this.DocData as GenItDocData;
			var diagramViews = docData?.EnsureDiagramViews();
			if (diagramViews == null || _viewSelector == null)
			{
				return;
			}

			if (!ReferenceEquals(_subscribedDiagramViews, diagramViews))
			{
				if (_subscribedDiagramViews != null)
				{
					_subscribedDiagramViews.ViewsChanged -= OnDiagramViewsChanged;
				}

				_subscribedDiagramViews = diagramViews;
				_subscribedDiagramViews.ViewsChanged += OnDiagramViewsChanged;
			}

			if (!_appliedInitialView && this.Diagram != null)
			{
				_appliedInitialView = true;
				diagramViews.ApplyCurrentView(this.Diagram);
			}

			_isRefreshingViewHeader = true;
			try
			{
				_viewSelector.BeginUpdate();
				_viewSelector.Items.Clear();
				foreach (var view in diagramViews.Views)
				{
					_viewSelector.Items.Add(view.Name);
				}

				if (!string.IsNullOrWhiteSpace(diagramViews.CurrentViewName))
				{
					if (!string.Equals(_viewSelector.SelectedItem as string, diagramViews.CurrentViewName, StringComparison.Ordinal))
					{
						_viewSelector.SelectedItem = diagramViews.CurrentViewName;
					}
				}
				else if (_viewSelector.Items.Count > 0)
				{
					if (_viewSelector.SelectedIndex != 0)
					{
						_viewSelector.SelectedIndex = 0;
					}
				}

				_deleteViewButton.Enabled = diagramViews.Views.Count > 1;
				_addViewButton.Enabled = true;
			}
			finally
			{
				_viewSelector.EndUpdate();
				_isRefreshingViewHeader = false;
			}
		}

		private void OnDiagramViewsChanged(object sender, EventArgs e)
		{
			if (_windowContainer == null || _windowContainer.IsDisposed)
			{
				return;
			}

			if (_windowContainer.InvokeRequired)
			{
				_windowContainer.BeginInvoke(new MethodInvoker(RefreshViewHeader));
				return;
			}

			RefreshViewHeader();
		}

		private void OnViewSelectorSelectedIndexChanged(object sender, EventArgs e)
		{
			if (_isRefreshingViewHeader)
			{
				return;
			}

			var docData = this.DocData as GenItDocData;
			var diagramViews = docData?.EnsureDiagramViews();
			if (diagramViews == null || _viewSelector?.SelectedItem == null)
			{
				return;
			}

				if (diagramViews.SetCurrentView(_viewSelector.SelectedItem.ToString(), this.Diagram))
				{
					docData.ClearDirtyState();
				}
		}

		private void OnAddViewButtonClick(object sender, EventArgs e)
		{
			var docData = this.DocData as GenItDocData;
			var diagramViews = docData?.EnsureDiagramViews();
			if (diagramViews == null)
			{
				return;
			}

			var dialog = new StringInputDialog();
			try
			{
				dialog.Title = "Add View";
				dialog.LabelText = "View name:";
				dialog.Value = GetNextViewName(diagramViews);

				var hostControl = this.Window as Control;
				if (hostControl == null)
				{
					return;
				}

				var interopHelper = new WindowInteropHelper(dialog);
				interopHelper.Owner = hostControl.Handle;

				if (dialog.ShowDialog() == true)
				{
					if (!diagramViews.CreateView(dialog.Value, this.Diagram))
					{
						MessageBox.Show(hostControl, "A view with that name already exists.", "Add View", MessageBoxButtons.OK, MessageBoxIcon.Information);
					}
				}
			}
			finally
			{
				dialog.Close();
			}
		}

		private void OnDeleteViewButtonClick(object sender, EventArgs e)
		{
			var docData = this.DocData as GenItDocData;
			var diagramViews = docData?.EnsureDiagramViews();
			if (diagramViews == null)
			{
				return;
			}

			var viewName = diagramViews.CurrentViewName;
			var hostControl = this.Window as Control;
			if (hostControl == null)
			{
				return;
			}

			var result = MessageBox.Show(
				hostControl,
				$"Delete view '{viewName}'?",
				"Delete View",
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Question,
				MessageBoxDefaultButton.Button2);

			if (result == DialogResult.Yes)
			{
				diagramViews.DeleteCurrentView(this.Diagram);
			}
		}

		private static string GetNextViewName(DiagramViewState diagramViews)
		{
			var index = 1;
			while (true)
			{
				var candidate = $"View {index}";
				if (!diagramViews.Views.Any(view => string.Equals(view.Name, candidate, StringComparison.OrdinalIgnoreCase)))
				{
					return candidate;
				}

				index++;
			}
		}
	}

	internal sealed class DiagramViewContainer : UserControl
	{
		public DiagramViewContainer(Control diagramControl)
		{
			if (diagramControl == null)
			{
				throw new ArgumentNullException(nameof(diagramControl));
			}

			Dock = DockStyle.Fill;

			HeaderHost = new Panel
			{
				Dock = DockStyle.Top,
				Height = 32
			};

			diagramControl.Dock = DockStyle.Fill;

			Controls.Add(diagramControl);
			Controls.Add(HeaderHost);
		}

		public Panel HeaderHost { get; }
	}
}
