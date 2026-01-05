using Dyvenix.GenIt.DslPackage.CodeGen.Misc;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections;
using System.Reflection;

namespace Dyvenix.GenIt.DslPackage.Tools.Services
{
	/// <summary>
	/// Tracks selection changes in the DSL designer and updates the GenItEditorWindow accordingly.
	/// Shows the service editor when a ServiceModel is selected, hides it otherwise.
	/// </summary>
	internal class SelectionTracker : IVsSelectionEvents, IDisposable
	{
		private readonly GenItPackage _package;
		private readonly IVsMonitorSelection _monitorSelection;
		private uint _selectionEventsCookie;
		private bool _isDisposed;

		public SelectionTracker(GenItPackage package)
		{
			_package = package ?? throw new ArgumentNullException(nameof(package));

			ThreadHelper.ThrowIfNotOnUIThread();

			_monitorSelection = Package.GetGlobalService(typeof(SVsShellMonitorSelection)) as IVsMonitorSelection;
			if (_monitorSelection != null)
			{
				_monitorSelection.AdviseSelectionEvents(this, out _selectionEventsCookie);
			}
		}

		public int OnSelectionChanged(
			IVsHierarchy pHierOld, uint itemidOld, IVsMultiItemSelect pMISOld, ISelectionContainer pSCOld,
			IVsHierarchy pHierNew, uint itemidNew, IVsMultiItemSelect pMISNew, ISelectionContainer pSCNew)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			GenItEditorWindow toolWindow = GetToolWindow();
			try
			{
				var selectedServiceModel = GetSelectedServiceModel(pSCOld);
				if (selectedServiceModel != null)
				{
					if (toolWindow?.Control != null)
					{
						// Get the EntityModel from the ServiceModel and pass it directly
						var entityModel = selectedServiceModel.EntityModeled;
						if (entityModel != null)
						{
							toolWindow.Control.ShowServiceEditor(entityModel, selectedServiceModel.Version);

							// Ensure the tool window is visible
							var frame = (IVsWindowFrame)toolWindow.Frame;
							frame?.Show();

							return VSConstants.S_OK;
						}
					}
				}
			}
			catch (Exception ex)
			{
				OutputHelper.WriteError($"SelectionTracker.OnSelectionChanged error: {ex.Message}");
			}

			toolWindow?.Control.HideServiceEditor();

			return VSConstants.S_OK;
		}

		private ServiceModel GetSelectedServiceModel(ISelectionContainer selectionContainer)
		{
			if (selectionContainer is GenItDocView docView)
			{
				var selectedElements = typeof(GenItDocView).GetProperty("SelectedElements", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)?.GetValue(selectionContainer) as ArrayList;
				if (selectedElements != null && selectedElements.Count == 1 && selectedElements[0] is ServiceModel)
					return selectedElements[0] as ServiceModel;
			}
			return null;
		}

		private GenItEditorWindow GetToolWindow()
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			try
			{
				var window = _package.FindToolWindow(typeof(GenItEditorWindow), 0, true) as GenItEditorWindow;
				return window;
			}
			catch (Exception ex)
			{
				OutputHelper.WriteError($"SelectionTracker.GetToolWindow error: {ex.Message}");
				return null;
			}
		}

		public int OnElementValueChanged(uint elementid, object varValueOld, object varValueNew)
		{
			return VSConstants.S_OK;
		}

		public int OnCmdUIContextChanged(uint dwCmdUICookie, int fActive)
		{
			return VSConstants.S_OK;
		}

		public void Dispose()
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			if (!_isDisposed)
			{
				if (_monitorSelection != null && _selectionEventsCookie != 0)
				{
					_monitorSelection.UnadviseSelectionEvents(_selectionEventsCookie);
					_selectionEventsCookie = 0;
				}

				_isDisposed = true;
			}
		}

		private static class Constants
		{
			public const int GETOBJS_SELECTED = 1;
		}
	}
}
