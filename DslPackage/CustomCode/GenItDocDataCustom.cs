using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using DslModeling = Microsoft.VisualStudio.Modeling;
using DslShell = Microsoft.VisualStudio.Modeling.Shell;

namespace Dyvenix.GenIt
{
	/// <summary>
	/// Customizations for GenItDocData.
	/// </summary>
	internal partial class GenItDocData
	{
		#region Multi-view state

		/// <summary>
		/// All diagram views loaded for this document, in display order.
		/// </summary>
		private readonly List<GenItDiagram> _views = new List<GenItDiagram>();

		/// <summary>
		/// Name of the view that should be shown when the document opens.
		/// </summary>
		private string _activeViewName;

		#endregion

		/// <summary>
		/// Loads the given file, deserializing the model and every diagram view stored in the
		/// single subordinate ".diagram" envelope file.
		/// </summary>
		protected override void Load(string fileName, bool isReload)
		{
			DslModeling::SerializationResult serializationResult = new DslModeling::SerializationResult();
			DslModeling::ISchemaResolver schemaResolver = new DslShell::ModelingSchemaResolver(this.ServiceProvider);

			// Clear the current root element.
			this.SetRootElement(null);
			_views.Clear();
			_activeViewName = null;

			// Enable diagram fixup rules in our store, because we will load diagram data.
			GenItDomainModel.EnableDiagramRules(this.Store);
			string diagramFileName = fileName + this.DiagramExtension;

			IList<GenItDiagram> loadedViews;
			string activeViewName;
			ModelRoot modelRoot = GenItSerializationHelper.Instance.LoadModelAndDiagrams(
				serializationResult,
				this.GetModelPartition(),
				fileName,
				this.GetDiagramPartition(),
				diagramFileName,
				schemaResolver,
				null /* no load-time validation */,
				this.SerializerLocator,
				out loadedViews,
				out activeViewName);

			// Report serialization messages.
			this.SuspendErrorListRefresh();
			try
			{
				foreach (DslModeling::SerializationMessage serializationMessage in serializationResult)
				{
					this.AddErrorListItem(new DslShell::SerializationErrorListItem(this.ServiceProvider, serializationMessage));
				}
			}
			finally
			{
				this.ResumeErrorListRefresh();
			}

			if (serializationResult.Failed)
			{
				// Load failed, can't open the file.
				throw new InvalidOperationException(GenItDomainModel.SingletonResourceManager.GetString("CannotOpenDocument"));
			}

			this.SetRootElement(modelRoot);

			if (loadedViews != null)
			{
				_views.AddRange(loadedViews);
			}
			_activeViewName = activeViewName;

			// Attempt to set the encoding.
			if (serializationResult.Encoding != null)
			{
				this.ModelingDocStore.SetEncoding(serializationResult.Encoding);
				Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(this.SetDocDataDirty(0)); // Setting the encoding marks the document dirty, so clear the flag.
			}

			if (this.Hierarchy != null && File.Exists(diagramFileName))
			{
				// Add a lock to the subordinate diagram file.
				if (this.diagramDocumentLockHolder == null)
				{
					uint itemId = DslShell::SubordinateFileHelper.GetChildProjectItemId(this.Hierarchy, this.ItemId, this.DiagramExtension);
					if (itemId != Microsoft.VisualStudio.VSConstants.VSITEMID_NIL)
					{
						this.diagramDocumentLockHolder = DslShell::SubordinateFileHelper.LockSubordinateDocument(this.ServiceProvider, this, diagramFileName, itemId);
						if (this.diagramDocumentLockHolder == null)
						{
							throw new InvalidOperationException(string.Format(
								System.Globalization.CultureInfo.CurrentCulture,
								GenItDomainModel.SingletonResourceManager.GetString("CannotCloseExistingDiagramDocument"),
								diagramFileName));
						}
					}
				}
			}
		}

		/// <summary>
		/// Called after the .gmdl document is loaded (opened).
		/// </summary>
		protected override void OnDocumentLoaded()
		{
			base.OnDocumentLoaded();

			Debug.WriteLine($"GenItDocData.OnDocumentLoaded: File opened - {this.FileName}");

			// Subscribe compartment-item events for every loaded view (the generated base only
			// subscribes the first diagram).
			foreach (GenItDiagram view in _views)
			{
				view?.SubscribeCompartmentItemsEvents();
			}
		}

		/// <summary>
		/// Saves the model and every diagram view. All views are written into the single subordinate
		/// ".diagram" envelope file.
		/// </summary>
		protected override void Save(string fileName)
		{
			DslModeling::SerializationResult serializationResult = new DslModeling::SerializationResult();
			ModelRoot modelRoot = (ModelRoot)this.RootElement;

			// SaveAs should let the subordinate document control saving of its own data, except when
			// there is no subordinate yet (no lock holder) - in that case we must write the diagram here.
			bool saveAs = string.Compare(fileName, this.FileName, StringComparison.OrdinalIgnoreCase) != 0;

			if (_views.Count > 0 && (!saveAs || this.diagramDocumentLockHolder == null))
			{
				string diagramFileName = fileName + this.DiagramExtension;
				try
				{
					this.SuspendFileChangeNotification(diagramFileName);

					GenItSerializationHelper.Instance.SaveModelAndDiagrams(
						serializationResult,
						modelRoot,
						fileName,
						_views,
						_activeViewName,
						diagramFileName,
						this.Encoding,
						false);
				}
				finally
				{
					this.ResumeFileChangeNotification(diagramFileName);
				}
			}
			else
			{
				GenItSerializationHelper.Instance.SaveModel(serializationResult, modelRoot, fileName, this.Encoding, false);
			}

			// Report serialization messages.
			this.SuspendErrorListRefresh();
			try
			{
				foreach (DslModeling::SerializationMessage serializationMessage in serializationResult)
				{
					this.AddErrorListItem(new DslShell::SerializationErrorListItem(this.ServiceProvider, serializationMessage));
				}
			}
			finally
			{
				this.ResumeErrorListRefresh();
			}

			if (serializationResult.Failed)
			{
				throw new InvalidOperationException(GenItDomainModel.SingletonResourceManager.GetString("CannotSaveDocument"));
			}
		}

		/// <summary>
		/// Saves the subordinate ".diagram" file (all views wrapped in one envelope).
		/// </summary>
		protected override void SaveSubordinateFile(DslShell::DocData subordinateDocument, string fileName)
		{
			DslModeling::SerializationResult serializationResult = new DslModeling::SerializationResult();

			if (_views.Count > 0)
			{
				try
				{
					this.SuspendFileChangeNotification(fileName);

					GenItSerializationHelper.Instance.SaveDiagrams(
						serializationResult,
						_views,
						_activeViewName,
						fileName,
						this.Encoding,
						false);
				}
				finally
				{
					this.ResumeFileChangeNotification(fileName);
				}
			}

			// Report serialization messages.
			this.SuspendErrorListRefresh();
			try
			{
				foreach (DslModeling::SerializationMessage serializationMessage in serializationResult)
				{
					this.AddErrorListItem(new DslShell::SerializationErrorListItem(this.ServiceProvider, serializationMessage));
				}
			}
			finally
			{
				this.ResumeErrorListRefresh();
			}

			if (!serializationResult.Failed)
			{
				this.NotifySubordinateDocumentSaved(subordinateDocument.FileName, fileName);
			}
			else
			{
				throw new InvalidOperationException(GenItDomainModel.SingletonResourceManager.GetString("CannotSaveDocument"));
			}
		}

		#region Multi-view API

		/// <summary>
		/// The diagram views loaded for this document, in display order.
		/// </summary>
		internal IReadOnlyList<GenItDiagram> Views
		{
			get { return _views; }
		}

		/// <summary>
		/// Name of the view that should be shown initially (persisted "activeView"). May be null.
		/// </summary>
		internal string ActiveViewName
		{
			get { return _activeViewName; }
			set { _activeViewName = value; }
		}

		/// <summary>
		/// Returns the view whose name matches (case-insensitive), or null when not found.
		/// </summary>
		internal GenItDiagram GetViewByName(string name)
		{
			if (string.IsNullOrEmpty(name))
				return null;

			foreach (GenItDiagram view in _views)
			{
				if (string.Equals(GenItSerializationHelper.GetViewName(view), name, StringComparison.OrdinalIgnoreCase))
					return view;
			}
			return null;
		}

		/// <summary>
		/// Returns true when a view with the given name already exists (case-insensitive).
		/// </summary>
		internal bool ViewNameExists(string name)
		{
			return GetViewByName(name) != null;
		}

		/// <summary>
		/// Creates a new empty diagram view with the given (unique) name and returns it.
		/// Must be called from the UI thread; wraps its own transaction.
		/// </summary>
		internal GenItDiagram CreateView(string name)
		{
			if (string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name));
			if (ViewNameExists(name))
				throw new InvalidOperationException($"A view named '{name}' already exists.");

			ModelRoot modelRoot = this.RootElement as ModelRoot;
			if (modelRoot == null)
				throw new InvalidOperationException("Cannot create a view before the model is loaded.");

			DslModeling::Partition diagramPartition = this.GetDiagramPartition();
			GenItDiagram diagram;

			using (DslModeling::Transaction t = this.Store.TransactionManager.BeginTransaction("Create View"))
			{
				diagram = new GenItDiagram(diagramPartition);
				diagram.Name = name;
				diagram.ModelElement = modelRoot;
				t.Commit();
			}

			diagram.SubscribeCompartmentItemsEvents();
			_views.Add(diagram);
			this.MarkDirty();
			return diagram;
		}

		/// <summary>
		/// Renames an existing view. Must be called from the UI thread; wraps its own transaction.
		/// </summary>
		internal void RenameView(GenItDiagram view, string newName)
		{
			if (view == null)
				throw new ArgumentNullException(nameof(view));
			if (string.IsNullOrEmpty(newName))
				throw new ArgumentNullException(nameof(newName));

			if (string.Equals(GenItSerializationHelper.GetViewName(view), newName, StringComparison.Ordinal))
				return;

			GenItDiagram existing = GetViewByName(newName);
			if (existing != null && existing != view)
				throw new InvalidOperationException($"A view named '{newName}' already exists.");

			string oldName = GenItSerializationHelper.GetViewName(view);

			using (DslModeling::Transaction t = this.Store.TransactionManager.BeginTransaction("Rename View"))
			{
				view.Name = newName;
				t.Commit();
			}

			if (string.Equals(_activeViewName, oldName, StringComparison.OrdinalIgnoreCase))
			{
				_activeViewName = newName;
			}
			this.MarkDirty();
		}

		/// <summary>
		/// Deletes the given view. The last remaining view cannot be deleted. Must be called from the
		/// UI thread; wraps its own transaction. Returns the view that should be shown afterward.
		/// </summary>
		internal GenItDiagram DeleteView(GenItDiagram view)
		{
			if (view == null)
				throw new ArgumentNullException(nameof(view));
			if (_views.Count <= 1)
				throw new InvalidOperationException("The last remaining view cannot be deleted.");

			int index = _views.IndexOf(view);
			if (index < 0)
				throw new InvalidOperationException("The specified view does not belong to this document.");

			view.UnsubscribeCompartmentItemsEvents();

			using (DslModeling::Transaction t = this.Store.TransactionManager.BeginTransaction("Delete View"))
			{
				view.Delete();
				t.Commit();
			}

			_views.RemoveAt(index);

			// Pick a neighbouring view to become active.
			GenItDiagram next = _views[Math.Min(index, _views.Count - 1)];
			if (string.Equals(_activeViewName, GenItSerializationHelper.GetViewName(view), StringComparison.OrdinalIgnoreCase))
			{
				_activeViewName = GenItSerializationHelper.GetViewName(next);
			}
			this.MarkDirty();
			return next;
		}

		/// <summary>
		/// Marks the document dirty so the multi-view diagram envelope is re-saved.
		/// </summary>
		private void MarkDirty()
		{
			if (this.Store != null)
			{
				this.MarkDocumentChangedForBackup();
			}
			this.SetDocDataDirty(1);
		}

		#endregion

		/// <summary>
		/// Called when the .gmdl document is closing.
		/// </summary>
		protected override void OnDocumentClosing(EventArgs e)
		{
			Debug.WriteLine($"GenItDocData.OnDocumentClosing: File closing - {this.FileName}");

			// Your custom logic here when the model file is closing

			base.OnDocumentClosing(e);
		}

		/// <summary>
		/// Called after the .gmdl document has been saved.
		/// </summary>
		protected override void OnDocumentSaved(EventArgs e)
		{
			base.OnDocumentSaved(e);

			Debug.WriteLine($"GenItDocData.OnDocumentSaved: File saved - {this.FileName}");

			// Your custom logic here after the model file is saved
		}
	}
}
