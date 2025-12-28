using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
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

        /// <summary>
        /// Override to perform additional initialization after the base package is initialized.
        /// </summary>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<VSShell.ServiceProgressData> progress)
        {
            await base.InitializeAsync(cancellationToken, progress);

            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            // Get the solution service and subscribe to solution events
            _solution = await GetServiceAsync(typeof(SVsSolution)) as IVsSolution;
            if (_solution != null)
            {
                _solution.AdviseSolutionEvents(this, out _solutionEventsCookie);
            }
        }

        protected override void Dispose(bool disposing)
        {
            VSShell.ThreadHelper.ThrowIfNotOnUIThread();

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
            // TODO: Add your logic here that should run after a solution is opened
            // fNewSolution is non-zero if this is a newly created solution

            return VSConstants.S_OK;
        }

        public int OnAfterCloseSolution(object pUnkReserved)
        {
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
