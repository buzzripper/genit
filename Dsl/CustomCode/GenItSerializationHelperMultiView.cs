using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using DslModeling = Microsoft.VisualStudio.Modeling;
using DslValidation = Microsoft.VisualStudio.Modeling.Validation;

namespace Dyvenix.GenIt
{
	/// <summary>
	/// Multi-view serialization support for the GenIt DSL.
	///
	/// All diagram views are persisted inside the single subordinate ".diagram" file using a
	/// lightweight envelope wrapper:
	///
	/// <code>
	/// &lt;genItDiagramViews activeView="Main"&gt;
	///   &lt;view name="Main"&gt;
	///     &lt;genItDiagram ...&gt; ... &lt;/genItDiagram&gt;
	///   &lt;/view&gt;
	///   &lt;view name="Billing"&gt;
	///     &lt;genItDiagram ...&gt; ... &lt;/genItDiagram&gt;
	///   &lt;/view&gt;
	/// &lt;/genItDiagramViews&gt;
	/// </code>
	///
	/// Each &lt;genItDiagram&gt; payload is produced/consumed by the generated diagram serializer, so
	/// the per-view content stays byte-for-byte compatible with the single-diagram format. A legacy
	/// file whose root element is a bare &lt;genItDiagram&gt; is detected on load and migrated into a
	/// single "Main" view.
	/// </summary>
	public sealed partial class GenItSerializationHelper
	{
		#region Envelope constants

		/// <summary>Root element name of the multi-view envelope.</summary>
		internal const string ViewsEnvelopeElementName = "genItDiagramViews";

		/// <summary>Per-view wrapper element name.</summary>
		internal const string ViewElementName = "view";

		/// <summary>Attribute holding a view's display name.</summary>
		internal const string ViewNameAttributeName = "name";

		/// <summary>Attribute on the envelope root holding the name of the active view.</summary>
		internal const string ActiveViewAttributeName = "activeView";

		/// <summary>Default view name used for new models and legacy single-diagram migration.</summary>
		public const string DefaultViewName = "Main";

		#endregion

		#region Save

		/// <summary>
		/// Saves the model and all of its diagram views. The model is written to
		/// <paramref name="modelFileName"/>; every view is written into a single envelope file at
		/// <paramref name="diagramsFileName"/>.
		/// </summary>
		public void SaveModelAndDiagrams(
			DslModeling::SerializationResult serializationResult,
			ModelRoot modelRoot,
			string modelFileName,
			IList<GenItDiagram> diagrams,
			string activeViewName,
			string diagramsFileName,
			System.Text.Encoding encoding,
			bool writeOptionalPropertiesWithDefaultValue)
		{
			#region Check Parameters
			if (serializationResult == null)
				throw new System.ArgumentNullException("serializationResult");
			if (modelRoot == null)
				throw new System.ArgumentNullException("modelRoot");
			if (string.IsNullOrEmpty(modelFileName))
				throw new System.ArgumentNullException("modelFileName");
			if (diagrams == null)
				throw new System.ArgumentNullException("diagrams");
			if (string.IsNullOrEmpty(diagramsFileName))
				throw new System.ArgumentNullException("diagramsFileName");
			#endregion

			if (serializationResult.Failed)
				return;

			// Serialize the model first so a failure aborts before any file is written.
			using (MemoryStream modelFileContent = this.InternalSaveModel(serializationResult, modelRoot, modelFileName, encoding, writeOptionalPropertiesWithDefaultValue))
			{
				if (serializationResult.Failed)
					return;

				using (MemoryStream diagramsFileContent = this.InternalSaveDiagramsEnvelope(serializationResult, diagrams, activeViewName, diagramsFileName, encoding, writeOptionalPropertiesWithDefaultValue))
				{
					if (serializationResult.Failed)
						return;

					// Only write to disk once both streams serialized without error.
					if (modelFileContent != null)
					{
						using (FileStream fileStream = new FileStream(modelFileName, FileMode.Create, FileAccess.Write, FileShare.None))
						using (BinaryWriter writer = new BinaryWriter(fileStream, encoding))
						{
							writer.Write(modelFileContent.ToArray());
						}
					}

					if (diagramsFileContent != null)
					{
						using (FileStream fileStream = new FileStream(diagramsFileName, FileMode.Create, FileAccess.Write, FileShare.None))
						using (BinaryWriter writer = new BinaryWriter(fileStream, encoding))
						{
							writer.Write(diagramsFileContent.ToArray());
						}
					}
				}
			}
		}

		/// <summary>
		/// Saves only the diagram views (all wrapped in one envelope) to
		/// <paramref name="diagramsFileName"/>. Used when the subordinate ".diagram" file is saved
		/// independently of the model.
		/// </summary>
		public void SaveDiagrams(
			DslModeling::SerializationResult serializationResult,
			IList<GenItDiagram> diagrams,
			string activeViewName,
			string diagramsFileName,
			System.Text.Encoding encoding,
			bool writeOptionalPropertiesWithDefaultValue)
		{
			#region Check Parameters
			if (serializationResult == null)
				throw new System.ArgumentNullException("serializationResult");
			if (diagrams == null)
				throw new System.ArgumentNullException("diagrams");
			if (string.IsNullOrEmpty(diagramsFileName))
				throw new System.ArgumentNullException("diagramsFileName");
			#endregion

			if (serializationResult.Failed)
				return;

			using (MemoryStream diagramsFileContent = this.InternalSaveDiagramsEnvelope(serializationResult, diagrams, activeViewName, diagramsFileName, encoding, writeOptionalPropertiesWithDefaultValue))
			{
				if (serializationResult.Failed || diagramsFileContent == null)
					return;

				using (FileStream fileStream = new FileStream(diagramsFileName, FileMode.Create, FileAccess.Write, FileShare.None))
				using (BinaryWriter writer = new BinaryWriter(fileStream, encoding))
				{
					writer.Write(diagramsFileContent.ToArray());
				}
			}
		}

		/// <summary>
		/// Serializes the given diagram views into an in-memory envelope stream. Each view's
		/// <see cref="GenItDiagram"/> payload is produced by the generated diagram serializer and then
		/// copied verbatim into a &lt;view&gt; wrapper.
		/// </summary>
		internal MemoryStream InternalSaveDiagramsEnvelope(
			DslModeling::SerializationResult serializationResult,
			IList<GenItDiagram> diagrams,
			string activeViewName,
			string diagramsFileName,
			System.Text.Encoding encoding,
			bool writeOptionalPropertiesWithDefaultValue)
		{
			if (serializationResult.Failed)
				return null;

			MemoryStream envelopeContent = new MemoryStream();

			XmlWriterSettings writerSettings = new XmlWriterSettings
			{
				Encoding = encoding,
				Indent = true,
				OmitXmlDeclaration = false,
				CloseOutput = false
			};

			XmlReaderSettings readerSettings = new XmlReaderSettings
			{
				CloseInput = false,
				IgnoreWhitespace = true
			};

			using (XmlWriter writer = XmlWriter.Create(envelopeContent, writerSettings))
			{
				writer.WriteStartElement(ViewsEnvelopeElementName);
				if (!string.IsNullOrEmpty(activeViewName))
				{
					writer.WriteAttributeString(ActiveViewAttributeName, activeViewName);
				}

				foreach (GenItDiagram diagram in diagrams)
				{
					if (serializationResult.Failed)
						break;
					if (diagram == null)
						continue;

					using (MemoryStream diagramContent = this.InternalSaveDiagram(serializationResult, diagram, diagramsFileName, encoding, writeOptionalPropertiesWithDefaultValue))
					{
						if (serializationResult.Failed || diagramContent == null)
							break;

						diagramContent.Position = 0;

						writer.WriteStartElement(ViewElementName);
						writer.WriteAttributeString(ViewNameAttributeName, GetViewName(diagram));

						using (XmlReader reader = XmlReader.Create(diagramContent, readerSettings))
						{
							// Position on the <genItDiagram> element, skipping the XML declaration,
							// then copy the whole subtree (including its namespace declarations).
							reader.MoveToContent();
							writer.WriteNode(reader, false);
						}

						writer.WriteEndElement(); // view
					}
				}

				writer.WriteEndElement(); // genItDiagramViews
			}

			return envelopeContent;
		}

		#endregion

		#region Load

		/// <summary>
		/// Loads a <see cref="ModelRoot"/> and all of its diagram views. The model is read from
		/// <paramref name="modelFileName"/>; every view is read from the envelope file at
		/// <paramref name="diagramsFileName"/>. A legacy file whose root is a bare
		/// &lt;genItDiagram&gt; is migrated into a single "Main" view. When no diagram file exists a
		/// new empty "Main" view is created.
		/// </summary>
		/// <param name="loadedDiagrams">Receives every diagram view that was loaded/created.</param>
		/// <param name="activeViewName">Receives the persisted active view name (may be null).</param>
		public ModelRoot LoadModelAndDiagrams(
			DslModeling::SerializationResult serializationResult,
			DslModeling::Partition modelPartition,
			string modelFileName,
			DslModeling::Partition diagramPartition,
			string diagramsFileName,
			DslModeling::ISchemaResolver schemaResolver,
			DslValidation::ValidationController validationController,
			DslModeling::ISerializerLocator serializerLocator,
			out IList<GenItDiagram> loadedDiagrams,
			out string activeViewName)
		{
			#region Check Parameters
			if (serializationResult == null)
				throw new ArgumentNullException("serializationResult");
			if (modelPartition == null)
				throw new ArgumentNullException("modelPartition");
			if (diagramPartition == null)
				throw new ArgumentNullException("diagramPartition");
			if (string.IsNullOrEmpty(diagramsFileName))
				throw new ArgumentNullException("diagramsFileName");
			#endregion

			loadedDiagrams = new List<GenItDiagram>();
			activeViewName = null;

			// An outer transaction (started by the framework) must span both loads so moniker
			// resolution works correctly.
			if (!diagramPartition.Store.TransactionActive)
			{
				throw new InvalidOperationException(GenItDomainModel.SingletonResourceManager.GetString("MissingTransaction"));
			}

			ModelRoot modelRoot = this.LoadModel(serializationResult, modelPartition, modelFileName, schemaResolver, validationController, serializerLocator);
			if (serializationResult.Failed)
			{
				// Don't try to deserialize diagram data if model load failed.
				return modelRoot;
			}

			DslModeling::DomainXmlSerializerDirectory directory = this.GetDirectory(diagramPartition.Store);
			DslModeling::DomainClassXmlSerializer diagramSerializer = directory.GetSerializer(GenItDiagram.DomainClassId);
			System.Diagnostics.Debug.Assert(diagramSerializer != null, "Cannot find serializer for GenItDiagram");
			if (diagramSerializer == null)
				return modelRoot;

			if (!File.Exists(diagramsFileName))
			{
				// Missing diagram file: create a new empty default view.
				GenItDiagram diagram = this.CreateDiagramHelper(diagramPartition, modelRoot);
				SetViewName(diagram, DefaultViewName);
				loadedDiagrams.Add(diagram);
				activeViewName = DefaultViewName;
			}
			else
			{
				List<ViewFragment> fragments = new List<ViewFragment>();
				activeViewName = ReadEnvelopeFragments(diagramsFileName, fragments, serializationResult);

				if (!serializationResult.Failed && fragments.Count > 0)
				{
					DslModeling::SerializationContext serializationContext = new DslModeling::SerializationContext(directory, diagramsFileName, serializationResult);
					this.InitializeSerializationContext(diagramPartition, serializationContext, true);
					DslModeling::TransactionContext transactionContext = new DslModeling::TransactionContext();
					transactionContext.Add(DslModeling::SerializationContext.TransactionContextKey, serializationContext);

					using (DslModeling::Transaction t = diagramPartition.Store.TransactionManager.BeginTransaction("LoadDiagrams", true, transactionContext))
					{
						foreach (ViewFragment fragment in fragments)
						{
							if (serializationResult.Failed)
								break;

							GenItDiagram diagram = DeserializeDiagramFragment(diagramSerializer, serializationContext, diagramPartition, fragment, schemaResolver);
							if (diagram != null)
							{
								SetViewName(diagram, fragment.Name);
								loadedDiagrams.Add(diagram);
							}
						}

						if (serializationResult.Failed)
						{
							loadedDiagrams.Clear();
							t.Rollback();
						}
						else if (t.IsActive)
						{
							t.Commit();
						}
					}

					// Do load-time validation if a ValidationController is provided.
					if (!serializationResult.Failed && validationController != null)
					{
						using (new SerializationValidationObserver(serializationResult, validationController))
						{
							validationController.Validate(diagramPartition, DslValidation::ValidationCategories.Load);
						}
					}
				}

				if (!serializationResult.Failed && loadedDiagrams.Count == 0)
				{
					// Empty/blank envelope: fall back to a new default view.
					GenItDiagram diagram = this.CreateDiagramHelper(diagramPartition, modelRoot);
					SetViewName(diagram, DefaultViewName);
					loadedDiagrams.Add(diagram);
					activeViewName = DefaultViewName;
				}
			}

			// Link each view to the model root and run post-deserialization/orphan cleanup, mirroring
			// the single-diagram generated flow.
			foreach (GenItDiagram diagram in loadedDiagrams)
			{
				if (!serializationResult.Failed)
				{
					diagram.ModelElement = modelRoot;
					diagram.PostDeserialization(true);
					this.CheckForOrphanedShapes(diagram, serializationResult);
				}
				else
				{
					diagram.PostDeserialization(false);
				}
			}

			return modelRoot;
		}

		/// <summary>
		/// Deserializes a single view fragment (a captured &lt;genItDiagram&gt; XML string) into the
		/// diagram partition using the generated diagram serializer.
		/// </summary>
		private GenItDiagram DeserializeDiagramFragment(
			DslModeling::DomainClassXmlSerializer diagramSerializer,
			DslModeling::SerializationContext serializationContext,
			DslModeling::Partition diagramPartition,
			ViewFragment fragment,
			DslModeling::ISchemaResolver schemaResolver)
		{
			GenItDiagram diagram = null;
			XmlReaderSettings settings = GenItSerializationHelper.Instance.CreateXmlReaderSettings(serializationContext, false);
			try
			{
				using (StringReader stringReader = new StringReader(fragment.Xml))
				using (XmlReader reader = XmlReader.Create(stringReader, settings))
				{
					reader.MoveToContent();
					diagram = diagramSerializer.TryCreateInstance(serializationContext, reader, diagramPartition) as GenItDiagram;
					if (diagram != null)
					{
						this.ReadRootElement(serializationContext, diagram, reader, schemaResolver);
					}
				}
			}
			catch (XmlException xEx)
			{
				DslModeling::SerializationUtilities.AddMessage(
					serializationContext,
					DslModeling::SerializationMessageKind.Error,
					xEx);
			}

			return diagram;
		}

		/// <summary>
		/// Reads the diagram file and captures each view's &lt;genItDiagram&gt; XML. Supports both the
		/// multi-view envelope and the legacy bare &lt;genItDiagram&gt; root (migrated to "Main").
		/// </summary>
		/// <returns>The persisted active view name, or null if none was recorded.</returns>
		private static string ReadEnvelopeFragments(string diagramsFileName, IList<ViewFragment> fragments, DslModeling::SerializationResult serializationResult)
		{
			string activeViewName = null;

			try
			{
				XmlReaderSettings settings = new XmlReaderSettings
				{
					IgnoreWhitespace = true,
					IgnoreComments = true,
					CloseInput = true
				};

				using (FileStream fileStream = File.OpenRead(diagramsFileName))
				{
					// Blank (or almost blank) files produce no fragments so the caller creates a new view.
					if (fileStream.Length <= 5)
						return null;

					using (XmlReader reader = XmlReader.Create(fileStream, settings))
					{
						reader.MoveToContent();

						if (reader.NodeType != XmlNodeType.Element)
							return null;

						if (string.Equals(reader.LocalName, ViewsEnvelopeElementName, StringComparison.Ordinal))
						{
							// Multi-view envelope.
							activeViewName = reader.GetAttribute(ActiveViewAttributeName);

							if (!reader.IsEmptyElement)
							{
								while (reader.Read())
								{
									if (reader.NodeType == XmlNodeType.EndElement)
										break;

									if (reader.NodeType == XmlNodeType.Element && string.Equals(reader.LocalName, ViewElementName, StringComparison.Ordinal))
									{
										string viewName = reader.GetAttribute(ViewNameAttributeName);
										if (string.IsNullOrEmpty(viewName))
											viewName = DefaultViewName;

										if (!reader.IsEmptyElement)
										{
											// Advance to the inner <genItDiagram> element and capture it.
											using (XmlReader viewSubtree = reader.ReadSubtree())
											{
												viewSubtree.MoveToContent();
												while (viewSubtree.Read())
												{
													if (viewSubtree.NodeType == XmlNodeType.Element)
													{
														fragments.Add(new ViewFragment(viewName, viewSubtree.ReadOuterXml()));
														break;
													}
												}
											}
										}
									}
								}
							}
						}
						else
						{
							// Legacy single-diagram file: migrate to a "Main" view.
							fragments.Add(new ViewFragment(DefaultViewName, reader.ReadOuterXml()));
							activeViewName = DefaultViewName;
						}
					}
				}
			}
			catch (XmlException xEx)
			{
				DslModeling::SerializationUtilities.AddMessage(
					new DslModeling::SerializationContext(null, diagramsFileName, serializationResult),
					DslModeling::SerializationMessageKind.Error,
					xEx);
			}

			return activeViewName;
		}

		#endregion

		#region Helpers

		/// <summary>
		/// Returns the display name for a diagram view, falling back to the default when unnamed.
		/// </summary>
		internal static string GetViewName(GenItDiagram diagram)
		{
			if (diagram == null)
				return DefaultViewName;

			return string.IsNullOrEmpty(diagram.Name) ? DefaultViewName : diagram.Name;
		}

		/// <summary>
		/// Assigns the display name to a diagram view. Must be called inside a transaction.
		/// </summary>
		internal static void SetViewName(GenItDiagram diagram, string name)
		{
			if (diagram == null)
				return;

			if (string.IsNullOrEmpty(name))
				name = DefaultViewName;

			if (!string.Equals(diagram.Name, name, StringComparison.Ordinal))
			{
				diagram.Name = name;
			}
		}

		/// <summary>
		/// Captured XML for a single diagram view, extracted from the envelope during load.
		/// </summary>
		private struct ViewFragment
		{
			public ViewFragment(string name, string xml)
			{
				Name = name;
				Xml = xml;
			}

			public string Name { get; }

			public string Xml { get; }
		}

		#endregion
	}
}
