using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Diagrams;
using System;
using System.Diagnostics;
using System.Linq;

namespace Dyvenix.GenIt
{
	/// <summary>
	/// Customizations for GenItDocData.
	/// </summary>
	internal partial class GenItDocData
	{
		internal DiagramViewState DiagramViews { get; private set; }

		internal DiagramViewState EnsureDiagramViews()
		{
			if (DiagramViews == null)
			{
				DiagramViews = new DiagramViewState(this.FileName, MarkDiagramViewsDirty);
				DiagramViews.Load(this.RootElement as ModelRoot, GetDiagram());
			}
			else if (DiagramViews.Views.Count == 0)
			{
				DiagramViews.Load(this.RootElement as ModelRoot, GetDiagram());
			}

			return DiagramViews;
		}

		internal void ClearDirtyState()
		{
			global::Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(this.SetDocDataDirty(0));

			if (this.diagramDocumentLockHolder?.SubordinateDocData != null)
			{
				global::Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(this.diagramDocumentLockHolder.SubordinateDocData.SetDocDataDirty(0));
			}
		}

		/// <summary>
		/// Marks the document (and its subordinate diagram document) dirty so the Save command
		/// becomes enabled, and schedules a backup. This is distinct from
		/// <see cref="MarkDocumentChangedForBackup"/>, which only schedules a backup and does not
		/// set the dirty flag that the Save button/command relies on.
		/// </summary>
		private void MarkDiagramViewsDirty()
		{
			global::Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(this.SetDocDataDirty(1));

			if (this.diagramDocumentLockHolder?.SubordinateDocData != null)
			{
				global::Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(this.diagramDocumentLockHolder.SubordinateDocData.SetDocDataDirty(1));
			}

			this.MarkDocumentChangedForBackup();
		}

		/// <summary>
		/// Called after the .gmdl document is loaded (opened).
		/// </summary>
		protected override void OnDocumentLoaded()
		{
			base.OnDocumentLoaded();

			Debug.WriteLine($"GenItDocData.OnDocumentLoaded: File opened - {this.FileName}");

			var diagramViews = EnsureDiagramViews();

			// If this is a brand-new model (or one created before the sidecar existed),
			// persist the initial view state immediately so the .views.xml file exists
			// even if the user never explicitly saves the .gmdl file.
			diagramViews?.SaveIfMissing(GetDiagram());
		}

		/// <summary>
		/// Called when the .gmdl document is closing.
		/// </summary>
		protected override void OnDocumentClosing(EventArgs e)
		{
			Debug.WriteLine($"GenItDocData.OnDocumentClosing: File closing - {this.FileName}");

			DiagramViews?.Save(GetDiagram());

			base.OnDocumentClosing(e);
		}

		/// <summary>
		/// Called after the .gmdl document has been saved.
		/// </summary>
		protected override void OnDocumentSaved(EventArgs e)
		{
			base.OnDocumentSaved(e);

			Debug.WriteLine($"GenItDocData.OnDocumentSaved: File saved - {this.FileName}");
			DiagramViews?.Save(GetDiagram());
		}

		private Diagram GetDiagram()
		{
			var diagramPartition = GetDiagramPartition();
			if (diagramPartition == null)
			{
				return null;
			}

			return diagramPartition.ElementDirectory.FindElements<GenItDiagram>().FirstOrDefault();
		}
	}
}
