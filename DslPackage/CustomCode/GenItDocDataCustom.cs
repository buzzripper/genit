using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Diagrams;
using Microsoft.VisualStudio.Modeling.Shell;
using System.Collections.Generic;
using System.Linq;

namespace Dyvenix.GenIt
{
	/// <summary>
	/// Custom DocData that supports saving and loading multiple diagrams.
	/// </summary>
	internal partial class GenItDocData
	{
		/// <summary>
		/// Loads the given file, including multiple diagrams.
		/// </summary>
		protected override void Load(string fileName, bool isReload)
		{
			SerializationResult serializationResult = new SerializationResult();
			ModelRoot modelRoot = null;
			ISchemaResolver schemaResolver = new ModelingSchemaResolver(this.ServiceProvider);
			
			// Clear the current root element
			this.SetRootElement(null);
			
			// Enable diagram fixup rules in our store
			GenItDomainModel.EnableDiagramRules(this.Store);
			
			string diagramFileName = fileName + this.DiagramExtension;

			// Load model and all diagrams
			IList<GenItDiagram> loadedDiagrams;
			modelRoot = GenItSerializationHelper.Instance.LoadModelAndDiagrams(
				serializationResult, 
				this.GetModelPartition(), 
				fileName, 
				this.GetDiagramPartition(), 
				diagramFileName, 
				schemaResolver, 
				null, // no load-time validation
				this.SerializerLocator,
				out loadedDiagrams);

			// Report serialization messages
			this.SuspendErrorListRefresh();
			try
			{
				foreach (SerializationMessage serializationMessage in serializationResult)
				{
					this.AddErrorListItem(new SerializationErrorListItem(this.ServiceProvider, serializationMessage));
				}
			}
			finally
			{
				this.ResumeErrorListRefresh();
			}

			if (serializationResult.Failed)
			{
				throw new global::System.InvalidOperationException(GenItDomainModel.SingletonResourceManager.GetString("CannotOpenDocument"));
			}
			else
			{
				this.SetRootElement(modelRoot);

				// Attempt to set the encoding
				if (serializationResult.Encoding != null)
				{
					this.ModelingDocStore.SetEncoding(serializationResult.Encoding);
					global::Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(this.SetDocDataDirty(0));
				}

				if (this.Hierarchy != null && global::System.IO.File.Exists(diagramFileName))
				{
					// Add a lock to the subordinate diagram file
					if (this.diagramDocumentLockHolder == null)
					{
						uint itemId = SubordinateFileHelper.GetChildProjectItemId(this.Hierarchy, this.ItemId, this.DiagramExtension);
						if (itemId != global::Microsoft.VisualStudio.VSConstants.VSITEMID_NIL)
						{
							this.diagramDocumentLockHolder = SubordinateFileHelper.LockSubordinateDocument(this.ServiceProvider, this, diagramFileName, itemId);
							if (this.diagramDocumentLockHolder == null)
							{
								throw new global::System.InvalidOperationException(string.Format(global::System.Globalization.CultureInfo.CurrentCulture,
									GenItDomainModel.SingletonResourceManager.GetString("CannotCloseExistingDiagramDocument"),
									diagramFileName));
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Gets all diagrams from the diagram partition.
		/// </summary>
		private List<GenItDiagram> GetAllDiagrams()
		{
			var diagrams = new List<GenItDiagram>();
			
			// Get diagrams from the diagram partition
			var diagramPartition = this.GetDiagramPartition();
			if (diagramPartition != null)
			{
				var partitionDiagrams = diagramPartition.ElementDirectory.FindElements<GenItDiagram>();
				diagrams.AddRange(partitionDiagrams);
			}

			// If no diagrams found in partition, try PresentationViewsSubject
			if (diagrams.Count == 0 && this.RootElement != null)
			{
				var presentationDiagrams = PresentationViewsSubject.GetPresentation(this.RootElement)
					.OfType<GenItDiagram>()
					.ToList();
				diagrams.AddRange(presentationDiagrams);
			}

			return diagrams;
		}

		/// <summary>
		/// Saves the given file, including all diagrams.
		/// </summary>
		protected override void Save(string fileName)
		{
			SerializationResult serializationResult = new SerializationResult();
			ModelRoot modelRoot = (ModelRoot)this.RootElement;

			bool saveAs = global::System.StringComparer.OrdinalIgnoreCase.Compare(fileName, this.FileName) != 0;

			// Get all diagrams from the diagram partition
			var allDiagrams = GetAllDiagrams();
			
			if (allDiagrams.Count > 0 && (!saveAs || this.diagramDocumentLockHolder == null))
			{
				// Save all diagrams to the diagram file
				string diagramFileName = fileName + this.DiagramExtension;
				try
				{
					this.SuspendFileChangeNotification(diagramFileName);
					
					// Save model and all diagrams
					GenItSerializationHelper.Instance.SaveModelAndDiagrams(serializationResult, modelRoot, fileName, allDiagrams, diagramFileName, this.Encoding, false);
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
				foreach (SerializationMessage serializationMessage in serializationResult)
				{
					this.AddErrorListItem(new SerializationErrorListItem(this.ServiceProvider, serializationMessage));
				}
			}
			finally
			{
				this.ResumeErrorListRefresh();
			}

			if (serializationResult.Failed)
			{
				throw new global::System.InvalidOperationException(GenItDomainModel.SingletonResourceManager.GetString("CannotSaveDocument"));
			}
		}

		/// <summary>
		/// Save the given document that is subordinate to this document.
		/// </summary>
		protected override void SaveSubordinateFile(DocData subordinateDocument, string fileName)
		{
			SerializationResult serializationResult = new SerializationResult();

			var allDiagrams = GetAllDiagrams();

			if (allDiagrams.Count > 0)
			{
				try
				{
					this.SuspendFileChangeNotification(fileName);
					GenItSerializationHelper.Instance.SaveDiagrams(serializationResult, allDiagrams, fileName, this.Encoding, false);
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
				foreach (SerializationMessage serializationMessage in serializationResult)
				{
					this.AddErrorListItem(new SerializationErrorListItem(this.ServiceProvider, serializationMessage));
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
				throw new global::System.InvalidOperationException(GenItDomainModel.SingletonResourceManager.GetString("CannotSaveDocument"));
			}
		}
	}
}
