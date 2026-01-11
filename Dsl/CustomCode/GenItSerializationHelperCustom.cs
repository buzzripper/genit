using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Diagrams;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Dyvenix.GenIt
{
	/// <summary>
	/// Custom serialization helper extension for saving and loading multiple diagrams.
	/// </summary>
	public partial class GenItSerializationHelperBase
	{
		/// <summary>
		/// Saves the model and multiple diagrams.
		/// </summary>
		public virtual void SaveModelAndDiagrams(SerializationResult serializationResult, ModelRoot modelRoot, string modelFileName, 
			IList<GenItDiagram> diagrams, string diagramFileName, Encoding encoding, bool writeOptionalPropertiesWithDefaultValue)
		{
			if (serializationResult == null)
				throw new global::System.ArgumentNullException(nameof(serializationResult));
			if (modelRoot == null)
				throw new global::System.ArgumentNullException(nameof(modelRoot));
			if (string.IsNullOrEmpty(modelFileName))
				throw new global::System.ArgumentNullException(nameof(modelFileName));
			if (diagrams == null || diagrams.Count == 0)
				throw new global::System.ArgumentNullException(nameof(diagrams));
			if (string.IsNullOrEmpty(diagramFileName))
				throw new global::System.ArgumentNullException(nameof(diagramFileName));

			if (serializationResult.Failed)
				return;

			// For a single diagram, use the standard serialization to maintain backward compatibility
			if (diagrams.Count == 1)
			{
				this.SaveModelAndDiagram(serializationResult, modelRoot, modelFileName, diagrams[0], diagramFileName, encoding, writeOptionalPropertiesWithDefaultValue);
				return;
			}

			// Save the model file first
			using (MemoryStream modelFileContent = this.InternalSaveModel(serializationResult, modelRoot, modelFileName, encoding, writeOptionalPropertiesWithDefaultValue))
			{
				if (serializationResult.Failed)
					return;

				using (MemoryStream diagramFileContent = InternalSaveDiagrams(serializationResult, diagrams, diagramFileName, encoding, writeOptionalPropertiesWithDefaultValue))
				{
					if (!serializationResult.Failed)
					{
						// Only write the contents if there's no error encountered during serialization.
						if (modelFileContent != null)
						{
							using (FileStream fileStream = new FileStream(modelFileName, FileMode.Create, FileAccess.Write, FileShare.None))
							{
								using (BinaryWriter writer = new BinaryWriter(fileStream, encoding))
								{
									writer.Write(modelFileContent.ToArray());
								}
							}
						}
						if (diagramFileContent != null)
						{
							using (FileStream fileStream = new FileStream(diagramFileName, FileMode.Create, FileAccess.Write, FileShare.None))
							{
								using (BinaryWriter writer = new BinaryWriter(fileStream, encoding))
								{
									writer.Write(diagramFileContent.ToArray());
								}
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Saves multiple diagrams to a file.
		/// </summary>
		public virtual void SaveDiagrams(SerializationResult serializationResult, IList<GenItDiagram> diagrams, string diagramFileName, Encoding encoding, bool writeOptionalPropertiesWithDefaultValue)
		{
			if (serializationResult == null)
				throw new global::System.ArgumentNullException(nameof(serializationResult));
			if (diagrams == null || diagrams.Count == 0)
				throw new global::System.ArgumentNullException(nameof(diagrams));
			if (string.IsNullOrEmpty(diagramFileName))
				throw new global::System.ArgumentNullException(nameof(diagramFileName));

			if (serializationResult.Failed)
				return;

			// For a single diagram, use the standard serialization
			if (diagrams.Count == 1)
			{
				this.SaveDiagram(serializationResult, diagrams[0], diagramFileName, encoding, writeOptionalPropertiesWithDefaultValue);
				return;
			}

			using (MemoryStream diagramFileContent = InternalSaveDiagrams(serializationResult, diagrams, diagramFileName, encoding, writeOptionalPropertiesWithDefaultValue))
			{
				if (!serializationResult.Failed && diagramFileContent != null)
				{
					using (FileStream fileStream = new FileStream(diagramFileName, FileMode.Create, FileAccess.Write, FileShare.None))
					{
						using (BinaryWriter writer = new BinaryWriter(fileStream, encoding))
						{
							writer.Write(diagramFileContent.ToArray());
						}
					}
				}
			}
		}

		/// <summary>
		/// Saves multiple diagrams to a memory stream.
		/// </summary>
		internal MemoryStream InternalSaveDiagrams(SerializationResult serializationResult, IList<GenItDiagram> diagrams, string diagramFileName, Encoding encoding, bool writeOptionalPropertiesWithDefaultValue)
		{
			if (diagrams == null || diagrams.Count == 0)
				return null;

			// For a single diagram, use the standard serialization
			if (diagrams.Count == 1)
			{
				return this.InternalSaveDiagram(serializationResult, diagrams[0], diagramFileName, encoding, writeOptionalPropertiesWithDefaultValue);
			}

			// For multiple diagrams, we need to serialize them all in a wrapper element
			DomainXmlSerializerDirectory directory = this.GetDirectory(diagrams[0].Store);
			MemoryStream newFileContent = new MemoryStream();

			SerializationContext serializationContext = new SerializationContext(directory, diagramFileName, serializationResult);
			this.InitializeSerializationContext(diagrams[0].Partition, serializationContext, false);
			serializationContext.WriteOptionalPropertiesWithDefaultValue = writeOptionalPropertiesWithDefaultValue;

			XmlWriterSettings settings = GenItSerializationHelper.Instance.CreateXmlWriterSettings(serializationContext, true, encoding);
			using (XmlWriter writer = XmlWriter.Create(newFileContent, settings))
			{
				writer.WriteStartElement("diagrams");
				writer.WriteAttributeString("xmlns", "dslDiagrams", null, "http://schemas.microsoft.com/VisualStudio/2005/DslTools/CoreDesignSurface");

				foreach (var diagram in diagrams)
				{
					// Serialize each diagram
					this.WriteRootElement(serializationContext, diagram, writer);
				}

				writer.WriteEndElement(); // diagrams
			}

			return newFileContent;
		}

		/// <summary>
		/// Loads model and multiple diagrams.
		/// </summary>
		public virtual ModelRoot LoadModelAndDiagrams(SerializationResult serializationResult, Partition modelPartition, string modelFileName, 
			Partition diagramPartition, string diagramFileName, ISchemaResolver schemaResolver, 
			Microsoft.VisualStudio.Modeling.Validation.ValidationController validationController, ISerializerLocator serializerLocator,
			out IList<GenItDiagram> loadedDiagrams)
		{
			loadedDiagrams = new List<GenItDiagram>();

			// Check if diagram file exists and if it's a multi-diagram file
			bool isMultiDiagramFile = false;
			if (File.Exists(diagramFileName))
			{
				isMultiDiagramFile = IsMultiDiagramFile(diagramFileName);
			}

			// For single diagram files (legacy), use the standard serialization
			if (!isMultiDiagramFile)
			{
				ModelRoot modelRoot = this.LoadModelAndDiagram(serializationResult, modelPartition, modelFileName, diagramPartition, diagramFileName, schemaResolver, validationController, serializerLocator);
				
				// Find the diagram that was loaded
				if (!serializationResult.Failed)
				{
					var diagrams = diagramPartition.ElementDirectory.FindElements<GenItDiagram>();
					foreach (var d in diagrams)
					{
						loadedDiagrams.Add(d);
					}
				}
				
				return modelRoot;
			}

			// For multi-diagram files, use custom loading
			// Ensure there is an outer transaction spanning both model and diagram load
			if (!diagramPartition.Store.TransactionActive)
			{
				throw new global::System.InvalidOperationException(GenItDomainModel.SingletonResourceManager.GetString("MissingTransaction"));
			}

			// Load the model first
			ModelRoot result = this.LoadModel(serializationResult, modelPartition, modelFileName, schemaResolver, validationController, serializerLocator);

			if (serializationResult.Failed)
			{
				return result;
			}

			// Load multiple diagrams
			DomainXmlSerializerDirectory directory = this.GetDirectory(diagramPartition.Store);

			using (FileStream fileStream = File.OpenRead(diagramFileName))
			{
				SerializationContext serializationContext = new SerializationContext(directory, fileStream.Name, serializationResult);
				this.InitializeSerializationContext(diagramPartition, serializationContext, true);
				TransactionContext transactionContext = new TransactionContext();
				transactionContext.Add(SerializationContext.TransactionContextKey, serializationContext);

				using (Transaction t = diagramPartition.Store.TransactionManager.BeginTransaction("LoadDiagrams", true, transactionContext))
				{
					if (fileStream.Length > 5)
					{
						XmlReaderSettings settings = GenItSerializationHelper.Instance.CreateXmlReaderSettings(serializationContext, false);
						try
						{
							using (XmlReader reader = XmlReader.Create(fileStream, settings))
							{
								reader.MoveToContent();

								// Read multiple diagrams from wrapper element
								if (reader.LocalName == "diagrams" && reader.Read())
								{
									while (reader.NodeType != XmlNodeType.EndElement || reader.LocalName != "diagrams")
									{
										if (reader.NodeType == XmlNodeType.Element && reader.LocalName == "genItDiagram")
										{
											DomainClassXmlSerializer diagramSerializer = directory.GetSerializer(GenItDiagram.DomainClassId);
											GenItDiagram diagram = diagramSerializer.TryCreateInstance(serializationContext, reader, diagramPartition) as GenItDiagram;
											if (diagram != null)
											{
												diagramSerializer.Read(serializationContext, diagram, reader);
												diagram.ModelElement = result;
												loadedDiagrams.Add(diagram);
											}
										}
										else if (!reader.Read())
										{
											break;
										}
									}
								}
							}
						}
						catch (XmlException xEx)
						{
							SerializationUtilities.AddMessage(serializationContext, SerializationMessageKind.Error, xEx);
						}

						if (serializationResult.Failed)
						{
							loadedDiagrams.Clear();
							t.Rollback();
						}
					}

					if (loadedDiagrams.Count == 0 && !serializationResult.Failed)
					{
						// Create default diagram if none were loaded
						GenItDiagram diagram = this.CreateDiagramHelper(diagramPartition, result);
						diagram.ModelElement = result;
						loadedDiagrams.Add(diagram);
					}

					if (t.IsActive)
						t.Commit();
				}

				// Call PostDeserialization on all diagrams
				foreach (var diagram in loadedDiagrams)
				{
					if (!serializationResult.Failed)
					{
						diagram.PostDeserialization(true);
						this.CheckForOrphanedShapes(diagram, serializationResult);
					}
					else
					{
						diagram.PostDeserialization(false);
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Checks if a diagram file contains multiple diagrams.
		/// </summary>
		private bool IsMultiDiagramFile(string diagramFileName)
		{
			try
			{
				using (FileStream fs = File.OpenRead(diagramFileName))
				using (XmlReader reader = XmlReader.Create(fs))
				{
					reader.MoveToContent();
					return reader.LocalName == "diagrams";
				}
			}
			catch
			{
				return false;
			}
		}
	}
}
