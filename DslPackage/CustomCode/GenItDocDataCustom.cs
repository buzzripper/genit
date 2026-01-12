using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Diagrams;
using Microsoft.VisualStudio.Modeling.Shell;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dyvenix.GenIt
{
	/// <summary>
	/// Custom DocData that supports saving/loading multiple diagrams.
	/// </summary>
	internal partial class GenItDocData
	{
		/// <summary>
		/// Override Load to load ALL diagrams, not just the first one.
		/// </summary>
		protected override void Load(string fileName, bool isReload)
		{
			SerializationResult serializationResult = new SerializationResult();
			ModelRoot modelRoot = null;
			ISchemaResolver schemaResolver = new ModelingSchemaResolver(this.ServiceProvider);
			
			// Clear the current root element
			this.SetRootElement(null);
			
			string diagramFileName = fileName + this.DiagramExtension;

			// Check if this is a multi-diagram file - if so, DO NOT enable diagram rules during loading
			// This prevents shapes from being auto-created on all diagrams
			bool isMultiDiagram = File.Exists(diagramFileName) && IsMultiDiagramFile(diagramFileName);
			
			if (!isMultiDiagram)
			{
				// Single diagram file - use standard behavior with diagram rules enabled
				GenItDomainModel.EnableDiagramRules(this.Store);
			}
			// For multi-diagram: keep rules DISABLED during loading - shapes come from file

			// Use the multi-diagram load method
			IList<GenItDiagram> loadedDiagrams;
			modelRoot = GenItSerializationHelper.Instance.LoadModelAndDiagrams(
				serializationResult, 
				this.GetModelPartition(), 
				fileName, 
				this.GetDiagramPartition(), 
				diagramFileName, 
				schemaResolver, 
				null /* no load-time validation */, 
				this.SerializerLocator,
				out loadedDiagrams);

			// NOW enable diagram rules for normal operation after loading is complete
			if (isMultiDiagram)
			{
				GenItDomainModel.EnableDiagramRules(this.Store);
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

				if (this.Hierarchy != null && File.Exists(diagramFileName))
				{
					// Add a lock to the subordinate diagram file.
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
		/// Checks if a diagram file contains multiple diagrams by looking for the root "diagrams" element.
		/// </summary>
		private bool IsMultiDiagramFile(string diagramFileName)
		{
			try
			{
				// Simple text-based check to avoid assembly reference issues
				// Read first 500 chars and look for <diagrams
				using (var sr = new StreamReader(diagramFileName))
				{
					char[] buffer = new char[500];
					int read = sr.Read(buffer, 0, 500);
					string content = new string(buffer, 0, read);
					return content.Contains("<diagrams");
				}
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// Override Save to save ALL diagrams, not just the first one.
		/// </summary>
		protected override void Save(string fileName)
		{
			SerializationResult serializationResult = new SerializationResult();
			ModelRoot modelRoot = (ModelRoot)this.RootElement;

			// Only save the diagrams if
			// a) There are any to save
			// b) This is NOT a SaveAs operation.
			bool saveAs = global::System.StringComparer.OrdinalIgnoreCase.Compare(fileName, this.FileName) != 0;

			// Get ALL diagrams from the diagram partition, not just from PresentationViewsSubject
			var diagramPartition = this.GetDiagramPartition();
			var allDiagrams = diagramPartition.ElementDirectory.FindElements<GenItDiagram>().ToList();

			if (allDiagrams.Count > 0 && (!saveAs || this.diagramDocumentLockHolder == null))
			{
				string diagramFileName = fileName + this.DiagramExtension;
				try
				{
					this.SuspendFileChangeNotification(diagramFileName);

					// Use the multi-diagram save method
					GenItSerializationHelper.Instance.SaveModelAndDiagrams(
						serializationResult, 
						modelRoot, 
						fileName, 
						allDiagrams, 
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
		/// Override SaveSubordinateFile to save ALL diagrams.
		/// </summary>
		protected override void SaveSubordinateFile(DocData subordinateDocument, string fileName)
		{
			SerializationResult serializationResult = new SerializationResult();

			// Get ALL diagrams from the diagram partition
			var diagramPartition = this.GetDiagramPartition();
			var allDiagrams = diagramPartition.ElementDirectory.FindElements<GenItDiagram>().ToList();

			if (allDiagrams.Count > 0)
			{
				try
				{
					this.SuspendFileChangeNotification(fileName);

					// Use the multi-diagram save method
					GenItSerializationHelper.Instance.SaveDiagrams(
						serializationResult,
						allDiagrams,
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
