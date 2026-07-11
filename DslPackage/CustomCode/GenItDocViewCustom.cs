using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using DslDiagrams = Microsoft.VisualStudio.Modeling.Diagrams;

namespace Dyvenix.GenIt
{
	/// <summary>
	/// Customizations for GenItDocView to support multiple diagram views in a single document.
	/// The document hosts one diagram surface at a time; switching views reassigns
	/// <see cref="Microsoft.VisualStudio.Modeling.Shell.SingleDiagramDocView.Diagram"/> to another
	/// loaded <see cref="GenItDiagram"/>.
	/// </summary>
	internal partial class GenItDocView
	{
		/// <summary>
		/// The view currently shown on the diagram surface.
		/// </summary>
		private GenItDiagram _currentView;

		/// <summary>
		/// True once the document-window toolbar has been attached to the frame.
		/// </summary>
		private bool _toolbarAttached;

		/// <summary>
		/// The document's data object as a <see cref="GenItDocData"/>, or null.
		/// </summary>
		internal GenItDocData GenItDocData
		{
			get { return this.DocData as GenItDocData; }
		}

		/// <summary>
		/// The view currently bound to the diagram surface.
		/// </summary>
		internal GenItDiagram CurrentView
		{
			get { return _currentView; }
		}

		/// <summary>
		/// Binds the persisted active view (or the first available view) to the surface after the
		/// document loads.
		/// </summary>
		protected override bool LoadView()
		{
			// Do NOT call base.LoadView(): the generated implementation asserts a single diagram.
			Debug.Assert(this.DocData.RootElement != null);
			if (this.DocData.RootElement == null)
				return false;

			GenItDocData docData = this.GenItDocData;
			Debug.Assert(docData != null, "DocData for GenItDocView should be a GenItDocData!");
			if (docData == null)
				return false;

			GenItDiagram initialView = ResolveInitialView(docData);
			if (initialView == null)
				return false;

			BindView(initialView);
			EnsureToolbarAttached();
			return true;
		}

		/// <summary>
		/// Attaches the view-selector document toolbar to this window's frame. Idempotent.
		/// </summary>
		private void EnsureToolbarAttached()
		{
			if (_toolbarAttached)
				return;

			IVsWindowFrame frame = this.Frame;
			if (frame == null)
				return;

			if (ErrorHandler.Failed(frame.GetProperty((int)__VSFPROPID.VSFPROPID_ToolbarHost, out object hostObject)))
				return;

			if (!(hostObject is IVsToolWindowToolbarHost toolbarHost))
				return;

			Guid toolbarGuid = GenItMultiViewCommands.CommandSetGuid;
			int hr = toolbarHost.AddToolbar(VSTWT_LOCATION.VSTWT_TOP, ref toolbarGuid, GenItMultiViewCommands.ViewToolbarId);
			if (ErrorHandler.Succeeded(hr))
			{
				_toolbarAttached = true;
			}
		}

		/// <summary>
		/// Picks the view to display first: the persisted active view when present, otherwise the
		/// first loaded view.
		/// </summary>
		private static GenItDiagram ResolveInitialView(GenItDocData docData)
		{
			IReadOnlyList<GenItDiagram> views = docData.Views;
			if (views == null || views.Count == 0)
				return null;

			GenItDiagram active = docData.GetViewByName(docData.ActiveViewName);
			return active ?? views[0];
		}

		/// <summary>
		/// Switches the document surface to the given view. No-op when it is already current.
		/// </summary>
		internal void SwitchToView(GenItDiagram view)
		{
			if (view == null)
				throw new ArgumentNullException(nameof(view));
			if (ReferenceEquals(view, _currentView))
				return;

			BindView(view);

			GenItDocData docData = this.GenItDocData;
			if (docData != null)
			{
				docData.ActiveViewName = GenItSerializationHelper.GetViewName(view);
			}
		}

		/// <summary>
		/// Assigns the diagram to the surface and records it as current. Also registers it as the
		/// active fix-up target so interactively added elements only appear on this view.
		/// </summary>
		private void BindView(GenItDiagram view)
		{
			this.Diagram = (DslDiagrams::Diagram)view;
			_currentView = view;
			GenItViewTargeting.SetActiveDiagram(view);
		}
	}
}
