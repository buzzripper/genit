using Dyvenix.GenIt.DslPackage.CustomCode;
using Dyvenix.GenIt.DslPackage.Editors;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Threading;
using System;
using System.Diagnostics;
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
}
