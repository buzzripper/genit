using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Diagrams;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Dyvenix.GenIt
{
	/// <summary>
	/// Extension to GenItDiagram to support drag-drop from Model Explorer and background color.
	/// </summary>
	public partial class GenItDiagram
	{
		/// <summary>
		/// Custom data format for elements dragged from the Model Explorer.
		/// </summary>
		private const string ModelElementDataFormat = "GenItModelElement";

		/// <summary>
		/// Thread-static field to track which diagram is currently active for shape creation.
		/// When set, only this diagram will create shapes for new elements.
		/// </summary>
		[System.ThreadStatic]
		private static GenItDiagram _activeCreationDiagram;

		/// <summary>
		/// Thread-static flag to indicate document is being loaded.
		/// When true, shape filtering is disabled to allow all shapes to load.
		/// </summary>
		[System.ThreadStatic]
		private static bool _isLoading;

		/// <summary>
		/// Gets or sets the diagram that should receive new shapes.
		/// When null, shapes are created on all diagrams (default DSL behavior for loading).
		/// </summary>
		public static GenItDiagram ActiveCreationDiagram
		{
			get => _activeCreationDiagram;
			set => _activeCreationDiagram = value;
		}

		/// <summary>
		/// Gets or sets whether the document is being loaded.
		/// </summary>
		public static bool IsLoading
		{
			get => _isLoading;
			set => _isLoading = value;
		}

		/// <summary>
		/// Override to apply background color from ModelRoot when diagram initializes.
		/// </summary>
		public override void OnInitialize()
		{
			base.OnInitialize();
			ApplyBackgroundColorFromModel();
		}

		/// <summary>
		/// Called after deserialization to perform additional setup.
		/// </summary>
		public override void PostDeserialization(bool loadSucceeded)
		{
			base.PostDeserialization(loadSucceeded);
			if (loadSucceeded)
			{
				// Apply colors after the model is fully loaded
				ApplyBackgroundColorFromModel();
				ApplyConnectorColorsFromModel();
			}
		}

		/// <summary>
		/// Applies the background color from the associated ModelRoot.
		/// </summary>
		public void ApplyBackgroundColorFromModel()
		{
			if (this.ModelElement is ModelRoot modelRoot)
			{
				Color bgColor = modelRoot.DiagramBackgroundColor;
				if (bgColor != Color.Empty && bgColor != Color.Transparent)
				{
					SetBackgroundColorInternal(bgColor);
				}
			}
		}

		/// <summary>
		/// Applies connector colors to all connectors on this diagram.
		/// </summary>
		public void ApplyConnectorColorsFromModel()
		{
			foreach (var shape in this.NestedChildShapes)
			{
				if (shape is AssociationConnector assocConnector)
				{
					assocConnector.ApplyLineColorFromModel();
				}
				else if (shape is EnumAssociationConnector enumConnector)
				{
					enumConnector.ApplyLineColorFromModel();
				}
			}
		}

		/// <summary>
		/// Sets the background color of the diagram (internal, no transaction).
		/// </summary>
		private void SetBackgroundColorInternal(Color color)
		{
			// Use the StyleSet to override the diagram background brush
			BrushSettings brushSettings = new BrushSettings();
			brushSettings.Color = color;
			this.StyleSet.OverrideBrush(DiagramBrushes.DiagramBackground, brushSettings);
			this.Invalidate(true);
		}

		/// <summary>
		/// Sets the background color of the diagram (wraps in transaction if needed).
		/// </summary>
		private void SetBackgroundColor(Color color)
		{
			if (this.Store.TransactionManager.InTransaction)
			{
				SetBackgroundColorInternal(color);
			}
			else
			{
				using (var transaction = this.Store.TransactionManager.BeginTransaction("Set Diagram Background"))
				{
					SetBackgroundColorInternal(color);
					transaction.Commit();
				}
			}
		}

		/// <summary>
		/// Override to handle drag over events, including from Model Explorer.
		/// </summary>
		public override void OnDragOver(DiagramDragEventArgs e)
		{
			base.OnDragOver(e);

			// If not already handled, check for our custom format
			if (e.Effect == DragDropEffects.None)
			{
				if (e.Data.GetDataPresent(ModelElementDataFormat))
				{
					var element = e.Data.GetData(ModelElementDataFormat) as ModelElement;
					if (element != null && CanAcceptElement(element))
					{
						e.Effect = DragDropEffects.Copy;
						e.Handled = true;
					}
				}
			}
		}

		/// <summary>
		/// Override to handle drop events, including from Model Explorer.
		/// </summary>
		public override void OnDragDrop(DiagramDragEventArgs e)
		{
			// First check for our custom format (from Model Explorer)
			if (e.Data.GetDataPresent(ModelElementDataFormat))
			{
				var element = e.Data.GetData(ModelElementDataFormat) as ModelElement;
				if (element != null && CanAcceptElement(element))
				{
					// Check if shape already exists on this diagram
					if (!HasShapeForElement(element))
					{
						// Get drop position
						PointD dropPoint = e.MousePosition;

						// Set this diagram as active for the drop operation
						var previousActive = _activeCreationDiagram;
						_activeCreationDiagram = this;
						try
						{
							// Create shape for the element
							using (var tx = this.Store.TransactionManager.BeginTransaction("Add to View"))
							{
								CreateShapeForExistingElement(element, dropPoint);
								tx.Commit();
							}
						}
						finally
						{
							// Restore previous active diagram
							_activeCreationDiagram = previousActive;
						}
					}
					e.Effect = DragDropEffects.Copy;
					e.Handled = true;
					return;
				}
			}

			// Let base class handle other drag-drop (toolbox items, etc.)
			base.OnDragDrop(e);
		}

		/// <summary>
		/// Determines if the diagram can accept the given element.
		/// </summary>
		private bool CanAcceptElement(ModelElement element)
		{
			return element is EntityModel
				|| element is EnumModel
				|| element is ModelInterface
				|| element is Comment;
		}

		/// <summary>
		/// Checks if a shape already exists on this diagram for the given element.
		/// </summary>
		private bool HasShapeForElement(ModelElement element)
		{
			var presentations = PresentationViewsSubject.GetPresentation(element);
			return presentations.Any(p => p is NodeShape ns && ns.Diagram == this);
		}

		/// <summary>
		/// Creates a shape for an existing model element at the specified position.
		/// </summary>
		private void CreateShapeForExistingElement(ModelElement element, PointD position)
		{
			// Use the diagram's built-in FixUpDiagram mechanism to create the shape properly
			// This ensures all the compartment initialization, decorators, etc. are set up correctly

			// Get the parent element for the shape (ModelRoot for top-level elements)
			ModelElement parentElement = null;

			if (element is ModelType modelType)
			{
				parentElement = modelType.ModelRoot;
			}
			else if (element is Comment comment)
			{
				parentElement = comment.ModelRoot;
			}

			if (parentElement == null)
			{
				return;
			}

			// Use the standard fixup mechanism which properly initializes compartments
			FixUpDiagram(parentElement, element);

			// Now find the shape that was created and set its position
			NodeShape createdShape = null;
			foreach (var pe in PresentationViewsSubject.GetPresentation(element))
			{
				if (pe is NodeShape ns && ns.Diagram == this)
				{
					createdShape = ns;
					break;
				}
			}

			if (createdShape != null)
			{
				// Set the position
				createdShape.AbsoluteBounds = new RectangleD(position, createdShape.AbsoluteBounds.Size);

				// Also create connectors for any existing relationships
				CreateConnectorsForElement(element);
			}
		}

		/// <summary>
		/// Creates connectors for relationships of an element when its shape is added.
		/// </summary>
		private void CreateConnectorsForElement(ModelElement element)
		{
			if (element is EntityModel entity)
			{
				// Create connectors for associations where this entity is source
				foreach (var association in Association.GetLinksToTargets(entity))
				{
					CreateConnectorIfBothEndsExist(association);
				}

				// Create connectors for associations where this entity is target
				foreach (var association in Association.GetLinksToSources(entity))
				{
					CreateConnectorIfBothEndsExist(association);
				}
			}
		}

		/// <summary>
		/// Creates a connector for a relationship if shapes for both ends exist on this diagram.
		/// </summary>
		private void CreateConnectorIfBothEndsExist(ElementLink link)
		{
			if (link == null) return;

			var linkedElements = link.LinkedElements;
			if (linkedElements.Count != 2) return;

			// Check if shapes exist for both ends
			NodeShape sourceShape = null;
			NodeShape targetShape = null;

			foreach (var pe in PresentationViewsSubject.GetPresentation(linkedElements[0]))
			{
				if (pe is NodeShape ns && ns.Diagram == this)
				{
					sourceShape = ns;
					break;
				}
			}

			foreach (var pe in PresentationViewsSubject.GetPresentation(linkedElements[1]))
			{
				if (pe is NodeShape ns && ns.Diagram == this)
				{
					targetShape = ns;
					break;
				}
			}

			if (sourceShape == null || targetShape == null)
				return;

			// Check if connector already exists
			foreach (var pe in PresentationViewsSubject.GetPresentation(link))
			{
				if (pe is BinaryLinkShape bls && bls.Diagram == this)
					return; // Connector already exists
			}

			// Use FixUpDiagram to create the connector properly
			FixUpDiagram(this.ModelElement, link);
		}
	}

	/// <summary>
	/// Rule that fires when new ModelType elements are added to the model.
	/// Cleans up shapes on non-active diagrams after the FixUpDiagram rules have run.
	/// </summary>
	[RuleOn(typeof(ModelRootHasTypes), FireTime = TimeToFire.TopLevelCommit, Priority = DiagramFixupConstants.AddShapeRulePriority + 1000)]
	internal sealed class FilterShapeCreationRule : AddRule
	{
		public override void ElementAdded(ElementAddedEventArgs e)
		{
			// Skip during undo/redo/rollback
			if (e.ModelElement.Store.InUndoRedoOrRollback)
				return;

			// Skip during document load
			if (GenItDiagram.IsLoading)
				return;

			// Must have an active diagram set to filter
			if (GenItDiagram.ActiveCreationDiagram == null)
				return;

			var link = e.ModelElement as ModelRootHasTypes;
			if (link == null)
				return;

			var modelType = link.Type;
			if (modelType == null)
				return;

			// Find all shapes for this element and delete those on non-active diagrams
			var shapesToDelete = new List<ShapeElement>();
			foreach (var pe in PresentationViewsSubject.GetPresentation(modelType))
			{
				if (pe is NodeShape ns)
				{
					var diagram = ns.Diagram as GenItDiagram;
					if (diagram != null && diagram != GenItDiagram.ActiveCreationDiagram)
					{
						shapesToDelete.Add(ns);
					}
				}
			}

			// Delete the shapes on non-active diagrams
			foreach (var shape in shapesToDelete)
			{
				shape.Delete();
			}
		}
	}

	/// <summary>
	/// Rule to update diagram background color when ModelRoot.DiagramBackgroundColor changes.
	/// </summary>
	[RuleOn(typeof(ModelRoot), FireTime = TimeToFire.TopLevelCommit)]
	internal sealed class DiagramBackgroundColorChangeRule : ChangeRule
	{
		public override void ElementPropertyChanged(ElementPropertyChangedEventArgs e)
		{
			if (e.DomainProperty.Id == ModelRoot.DiagramBackgroundColorDomainPropertyId)
			{
				ModelRoot modelRoot = (ModelRoot)e.ModelElement;
				Color newColor = (Color)e.NewValue;

				// Find all diagrams associated with this model root and update their background
				foreach (var pel in PresentationViewsSubject.GetPresentation(modelRoot))
				{
					if (pel is GenItDiagram diagram)
					{
						diagram.ApplyBackgroundColorFromModel();
					}
				}
			}
		}
	}
}
