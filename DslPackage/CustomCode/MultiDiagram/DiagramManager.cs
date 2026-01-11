using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Diagrams;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dyvenix.GenIt
{
	/// <summary>
	/// Manages multiple diagrams (views) for a single model.
	/// Each diagram represents a different view showing a subset of model elements.
	/// </summary>
	internal class DiagramManager
	{
		private readonly Store _store;
		private readonly ModelRoot _modelRoot;
		private readonly Partition _diagramPartition;
		private readonly List<GenItDiagram> _diagrams = new List<GenItDiagram>();
		private int _activeDiagramIndex = 0;

		/// <summary>
		/// Event raised when the active diagram changes.
		/// </summary>
		public event EventHandler<DiagramChangedEventArgs> ActiveDiagramChanged;

		/// <summary>
		/// Event raised when a diagram is added.
		/// </summary>
		public event EventHandler<DiagramChangedEventArgs> DiagramAdded;

		/// <summary>
		/// Event raised when a diagram is removed.
		/// </summary>
		public event EventHandler<DiagramChangedEventArgs> DiagramRemoved;

		/// <summary>
		/// Gets all managed diagrams.
		/// </summary>
		public IReadOnlyList<GenItDiagram> Diagrams => _diagrams.AsReadOnly();

		/// <summary>
		/// Gets the currently active diagram.
		/// </summary>
		public GenItDiagram ActiveDiagram => _activeDiagramIndex >= 0 && _activeDiagramIndex < _diagrams.Count
			? _diagrams[_activeDiagramIndex]
			: null;

		/// <summary>
		/// Gets or sets the active diagram index.
		/// </summary>
		public int ActiveDiagramIndex
		{
			get => _activeDiagramIndex;
			set
			{
				if (value >= 0 && value < _diagrams.Count && value != _activeDiagramIndex)
				{
					var oldDiagram = ActiveDiagram;
					_activeDiagramIndex = value;
					ActiveDiagramChanged?.Invoke(this, new DiagramChangedEventArgs(oldDiagram, ActiveDiagram));
				}
			}
		}

		public DiagramManager(Store store, ModelRoot modelRoot, Partition diagramPartition)
		{
			_store = store ?? throw new ArgumentNullException(nameof(store));
			_modelRoot = modelRoot ?? throw new ArgumentNullException(nameof(modelRoot));
			_diagramPartition = diagramPartition ?? throw new ArgumentNullException(nameof(diagramPartition));
		}

		/// <summary>
		/// Initializes the manager with existing diagrams from the store.
		/// </summary>
		public void Initialize()
		{
			// Find all existing diagrams in the diagram partition
			var existingDiagrams = _diagramPartition.ElementDirectory.FindElements<GenItDiagram>();

			foreach (var diagram in existingDiagrams)
			{
				_diagrams.Add(diagram);
			}

			// If no diagrams exist, create a default one
			if (_diagrams.Count == 0)
			{
				CreateDiagram("Default", showAllElements: true);
			}

			_activeDiagramIndex = 0;
		}

		/// <summary>
		/// Creates a new diagram (view) with the specified name.
		/// </summary>
		/// <param name="name">Name of the new view.</param>
		/// <param name="showAllElements">If true, creates shapes for all model elements.</param>
		/// <returns>The created diagram.</returns>
		public GenItDiagram CreateDiagram(string name, bool showAllElements = false)
		{
			GenItDiagram newDiagram = null;

			using (var tx = _store.TransactionManager.BeginTransaction("Create View"))
			{
				newDiagram = new GenItDiagram(_diagramPartition);
				newDiagram.Name = name;

				// Associate the diagram with the model root
				newDiagram.ModelElement = _modelRoot;

				if (showAllElements)
				{
					// Create shapes for all model elements
					AddAllModelElementsToView(newDiagram);
				}

				_diagrams.Add(newDiagram);
				tx.Commit();
			}

			DiagramAdded?.Invoke(this, new DiagramChangedEventArgs(null, newDiagram));
			return newDiagram;
		}

		/// <summary>
		/// Removes a diagram (view).
		/// </summary>
		/// <param name="index">Index of the diagram to remove.</param>
		public void RemoveDiagram(int index)
		{
			if (index < 0 || index >= _diagrams.Count)
				throw new ArgumentOutOfRangeException(nameof(index));

			if (_diagrams.Count <= 1)
				throw new InvalidOperationException("Cannot remove the last diagram.");

			var diagram = _diagrams[index];

			using (var tx = _store.TransactionManager.BeginTransaction("Remove View"))
			{
				_diagrams.RemoveAt(index);
				diagram.Delete();
				tx.Commit();
			}

			// Adjust active index
			if (_activeDiagramIndex >= _diagrams.Count)
			{
				_activeDiagramIndex = _diagrams.Count - 1;
			}

			DiagramRemoved?.Invoke(this, new DiagramChangedEventArgs(diagram, ActiveDiagram));
		}

		/// <summary>
		/// Renames a diagram.
		/// </summary>
		/// <param name="index">Index of the diagram to rename.</param>
		/// <param name="newName">New name for the diagram.</param>
		public void RenameDiagram(int index, string newName)
		{
			if (index < 0 || index >= _diagrams.Count)
				throw new ArgumentOutOfRangeException(nameof(index));

			if (string.IsNullOrWhiteSpace(newName))
				throw new ArgumentException("Name cannot be empty.", nameof(newName));

			using (var tx = _store.TransactionManager.BeginTransaction("Rename View"))
			{
				_diagrams[index].Name = newName;
				tx.Commit();
			}
		}

		/// <summary>
		/// Adds all model elements to the specified diagram as shapes.
		/// </summary>
		private void AddAllModelElementsToView(GenItDiagram diagram)
		{
			double x = 1.0;
			double y = 1.0;
			double xSpacing = 3.0;
			double ySpacing = 2.5;
			int itemsPerRow = 4;
			int count = 0;

			// Add entities
			foreach (var entity in _modelRoot.Types.OfType<EntityModel>())
			{
				AddShapeForElement(diagram, entity, x, y);
				count++;
				if (count % itemsPerRow == 0)
				{
					x = 1.0;
					y += ySpacing;
				}
				else
				{
					x += xSpacing;
				}
			}

			// Add enums
			foreach (var enumModel in _modelRoot.Types.OfType<EnumModel>())
			{
				AddShapeForElement(diagram, enumModel, x, y);
				count++;
				if (count % itemsPerRow == 0)
				{
					x = 1.0;
					y += ySpacing;
				}
				else
				{
					x += xSpacing;
				}
			}

			// Add modules
			foreach (var module in _modelRoot.Types.OfType<ModuleModel>())
			{
				AddShapeForElement(diagram, module, x, y);
				count++;
				if (count % itemsPerRow == 0)
				{
					x = 1.0;
					y += ySpacing;
				}
				else
				{
					x += xSpacing;
				}
			}

			// Add interfaces
			foreach (var iface in _modelRoot.Types.OfType<ModelInterface>())
			{
				AddShapeForElement(diagram, iface, x, y);
				count++;
				if (count % itemsPerRow == 0)
				{
					x = 1.0;
					y += ySpacing;
				}
				else
				{
					x += xSpacing;
				}
			}
		}

		/// <summary>
		/// Adds a shape for a model element to the specified diagram.
		/// </summary>
		public NodeShape AddShapeForElement(GenItDiagram diagram, ModelElement element, double x, double y)
		{
			if (diagram == null || element == null)
				return null;

			// Check if shape already exists on this diagram
			foreach (var pe in PresentationViewsSubject.GetPresentation(element))
			{
				if (pe is NodeShape ns && ns.Diagram == diagram)
				{
					return ns;
				}
			}

			NodeShape shape = null;

			// Create shape using the diagram's AutomaticallyCopyAncestorShapes
			ShapeElement parentShape = diagram;

			// Try to find if a shape was created via the FixUpDiagram rules
			foreach (var shapeType in GetShapeTypesForElement(element))
			{
				shape = CreateShapeInstance(shapeType, element, diagram) as NodeShape;
				if (shape != null)
				{
					// Position the shape
					shape.Bounds = new RectangleD(x, y, shape.Bounds.Width, shape.Bounds.Height);
					break;
				}
			}

			return shape;
		}

		/// <summary>
		/// Gets the shape types that can represent the given model element.
		/// </summary>
		private System.Collections.Generic.IEnumerable<System.Type> GetShapeTypesForElement(ModelElement element)
		{
			if (element is EntityModel)
				yield return typeof(ClassShape);
			else if (element is EnumModel)
				yield return typeof(EnumShape);
			else if (element is ModuleModel)
				yield return typeof(ModuleShape);
			else if (element is ModelInterface)
				yield return typeof(InterfaceShape);
			else if (element is Comment)
				yield return typeof(CommentBoxShape);
		}

		/// <summary>
		/// Creates a shape instance for the given element.
		/// </summary>
		private ShapeElement CreateShapeInstance(System.Type shapeType, ModelElement element, Diagram diagram)
		{
			try
			{
				// Get the domain class ID for the shape type
				var domainClassInfo = diagram.Store.DomainDataDirectory.FindDomainClass(shapeType);
				if (domainClassInfo == null)
					return null;

				// Create the shape in the diagram's partition
				var shape = diagram.Store.ElementFactory.CreateElement(domainClassInfo.Id) as NodeShape;
				if (shape != null)
				{
					// Associate shape with element
					shape.Associate(element);

					// Add to diagram
					diagram.NestedChildShapes.Add(shape);

					return shape;
				}
			}
			catch (System.Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Error creating shape: {ex.Message}");
			}
			return null;
		}

		/// <summary>
		/// Removes a shape from the active diagram (does not delete the model element).
		/// </summary>
		public void RemoveShapeFromActiveView(ShapeElement shape)
		{
			if (shape == null || ActiveDiagram == null)
				return;

			using (var tx = _store.TransactionManager.BeginTransaction("Remove from View"))
			{
				shape.Delete();
				tx.Commit();
			}
		}

		/// <summary>
		/// Gets a diagram by name.
		/// </summary>
		public GenItDiagram GetDiagramByName(string name)
		{
			return _diagrams.FirstOrDefault(d => string.Equals(d.Name, name, StringComparison.OrdinalIgnoreCase));
		}
	}

	/// <summary>
	/// Event args for diagram change events.
	/// </summary>
	internal class DiagramChangedEventArgs : EventArgs
	{
		public GenItDiagram OldDiagram { get; }
		public GenItDiagram NewDiagram { get; }

		public DiagramChangedEventArgs(GenItDiagram oldDiagram, GenItDiagram newDiagram)
		{
			OldDiagram = oldDiagram;
			NewDiagram = newDiagram;
		}
	}
}
