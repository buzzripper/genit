namespace Dyvenix.GenIt
{
	using Microsoft.VisualStudio.Modeling;
	using Microsoft.VisualStudio.Modeling.Diagrams;
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Xml.Serialization;

	internal sealed class DiagramViewState
	{
		private const string DefaultViewName = "Main";

		private readonly Action _markDirty;
		private readonly string _modelFilePath;
		private readonly List<DiagramViewDefinition> _views = new List<DiagramViewDefinition>();

		private string _currentViewName;

		public DiagramViewState(string modelFilePath, Action markDirty)
		{
			_modelFilePath = modelFilePath ?? throw new ArgumentNullException(nameof(modelFilePath));
			_markDirty = markDirty;
		}

		public event EventHandler ViewsChanged;

		public IReadOnlyList<DiagramViewDefinition> Views => _views;

		public string CurrentViewName => _currentViewName;

		public DiagramViewDefinition CurrentView => GetCurrentView();

		private string SidecarFilePath => string.IsNullOrWhiteSpace(_modelFilePath) || !Path.IsPathRooted(_modelFilePath)
			? null
			: _modelFilePath + ".views.xml";

		public void Load(ModelRoot modelRoot, Diagram diagram)
		{
			_views.Clear();
			_currentViewName = null;

			if (!string.IsNullOrWhiteSpace(SidecarFilePath) && File.Exists(SidecarFilePath))
			{
				var serializer = new XmlSerializer(typeof(DiagramViewStateFile));
				using (var stream = File.OpenRead(SidecarFilePath))
				{
					if (serializer.Deserialize(stream) is DiagramViewStateFile file)
					{
						if (file.Views != null)
						{
							_views.AddRange(file.Views.Where(view => view != null && !string.IsNullOrWhiteSpace(view.Name)));
						}

						_currentViewName = file.CurrentViewName;
					}
				}
			}

			EnsureDefaultView(modelRoot, diagram);
			NormalizeState();
			OnViewsChanged();
		}

		public void Save(Diagram diagram)
		{
			CaptureCurrentLayout(diagram);
			NormalizeState();

			if (string.IsNullOrWhiteSpace(SidecarFilePath))
			{
				return;
			}

			var serializer = new XmlSerializer(typeof(DiagramViewStateFile));
			var file = new DiagramViewStateFile
			{
				CurrentViewName = _currentViewName,
				Views = _views
			};

			var directory = Path.GetDirectoryName(SidecarFilePath);
			if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			using (var stream = File.Create(SidecarFilePath))
			{
				serializer.Serialize(stream, file);
			}
		}

		public void SaveIfMissing(Diagram diagram)
		{
			if (string.IsNullOrWhiteSpace(SidecarFilePath) || File.Exists(SidecarFilePath))
			{
				return;
			}

			Save(diagram);
		}

		public bool SetCurrentView(string viewName, Diagram diagram)
		{
			if (string.IsNullOrWhiteSpace(viewName))
			{
				return false;
			}

			var targetView = _views.FirstOrDefault(view => string.Equals(view.Name, viewName, StringComparison.Ordinal));
			if (targetView == null)
			{
				return false;
			}

			if (string.Equals(_currentViewName, targetView.Name, StringComparison.Ordinal))
			{
				return false;
			}

			CaptureCurrentLayout(diagram);
			_currentViewName = targetView.Name;
			ApplyCurrentView(diagram);
			OnViewsChanged();
			return true;
		}

		public bool CreateView(string viewName, Diagram diagram)
		{
			var normalizedName = NormalizeViewName(viewName);
			if (string.IsNullOrWhiteSpace(normalizedName) || _views.Any(view => string.Equals(view.Name, normalizedName, StringComparison.OrdinalIgnoreCase)))
			{
				return false;
			}

			CaptureCurrentLayout(diagram);

			var newView = new DiagramViewDefinition
			{
				Name = normalizedName,
				ElementIds = new List<string>(),
				Layout = new List<DiagramViewElementLayout>()
			};

			_views.Add(newView);
			_currentViewName = newView.Name;
			ApplyCurrentView(diagram);
			MarkDirty();
			OnViewsChanged();
			return true;
		}

		public bool DeleteCurrentView(Diagram diagram)
		{
			CaptureCurrentLayout(diagram);
			NormalizeState();

			var currentView = GetCurrentView();
			if (currentView == null)
			{
				return false;
			}

			_views.Remove(currentView);
			if (_views.Count == 0)
			{
				_views.Add(new DiagramViewDefinition
				{
					Name = DefaultViewName,
					ElementIds = new List<string>(),
					Layout = new List<DiagramViewElementLayout>()
				});
			}

			_currentViewName = _views[0].Name;
			ApplyCurrentView(diagram);
			MarkDirty();
			OnViewsChanged();
			return true;
		}

		public bool AddElement(ModelElement element, Diagram diagram)
		{
			if (!CanPersistElement(element))
			{
				return false;
			}

			var currentView = EnsureCurrentView();
			var elementId = element.Id.ToString("D");
			if (currentView.ElementIds.Any(id => string.Equals(id, elementId, StringComparison.OrdinalIgnoreCase)))
			{
				return false;
			}

			currentView.ElementIds.Add(elementId);
			ApplyCurrentView(diagram);
			MarkDirty();
			OnViewsChanged();
			return true;
		}

		public bool RemoveElement(ModelElement element, Diagram diagram)
		{
			if (element == null)
			{
				return false;
			}

			var currentView = GetCurrentView();
			if (currentView == null)
			{
				return false;
			}

			var elementId = element.Id.ToString("D");
			var removed = currentView.ElementIds.RemoveAll(id => string.Equals(id, elementId, StringComparison.OrdinalIgnoreCase)) > 0;
			currentView.Layout.RemoveAll(layout => string.Equals(layout.ElementId, elementId, StringComparison.OrdinalIgnoreCase));
			if (!removed)
			{
				return false;
			}

			ApplyCurrentView(diagram);
			MarkDirty();
			OnViewsChanged();
			return true;
		}

		public void ApplyCurrentView(Diagram diagram)
		{
			if (diagram == null)
			{
				return;
			}

			var currentView = GetCurrentView();
			var modelRoot = diagram.ModelElement as ModelRoot;
			if (currentView == null || modelRoot == null)
			{
				return;
			}

			NormalizeState();
			var allPersistableElements = modelRoot.Types.Cast<ModelElement>()
				.Concat(modelRoot.Comments.Cast<ModelElement>())
				.Where(CanPersistElement)
				.ToList();
			var visibleElements = ResolveViewElements(modelRoot.Store, currentView).ToList();
			var visibleElementIds = new HashSet<Guid>(visibleElements.Select(element => element.Id));
			var visibleEntityIds = new HashSet<Guid>(visibleElements.OfType<EntityModel>().Select(entity => entity.Id));
			var visibleAssociations = modelRoot.Store.ElementDirectory.AllElements
				.OfType<Association>()
				.Where(association => association.Source != null
					&& association.Target != null
					&& visibleEntityIds.Contains(association.Source.Id)
					&& visibleEntityIds.Contains(association.Target.Id))
				.ToList();
			var visibleAssociationIds = new HashSet<Guid>(visibleAssociations.Select(association => association.Id));

			using (var transaction = diagram.Store.TransactionManager.BeginTransaction("Ensure Diagram View Shapes"))
			{
				foreach (var element in visibleElements)
				{
					if (!HasShapeOnDiagram(diagram, element))
					{
						Diagram.FixUpDiagram(modelRoot, element);
					}
				}

				foreach (var association in visibleAssociations)
				{
					if (!HasShapeOnDiagram(diagram, association))
					{
						Diagram.FixUpDiagram(diagram, association);
					}
				}
				transaction.Commit();
			}

			using (var transaction = diagram.Store.TransactionManager.BeginTransaction("Apply Diagram View Visibility"))
			{
				foreach (var element in allPersistableElements)
				{
					var show = visibleElementIds.Contains(element.Id);
					foreach (var shape in PresentationViewsSubject.GetPresentation(element).OfType<NodeShape>().Where(shape => shape.Diagram == diagram))
					{
						shape.SetShowHideState(show);
					}
				}

				foreach (var association in modelRoot.Store.ElementDirectory.AllElements.OfType<Association>())
				{
					var show = visibleAssociationIds.Contains(association.Id);
					foreach (var shape in PresentationViewsSubject.GetPresentation(association).OfType<LinkShape>().Where(shape => shape.Diagram == diagram))
					{
						shape.SetShowHideState(show);
					}
				}

				RestoreLayout(diagram, currentView);
				transaction.Commit();
			}
		}

		public void CaptureCurrentLayout(Diagram diagram)
		{
			var currentView = GetCurrentView();
			if (diagram == null || currentView == null)
			{
				return;
			}

			// Elements already known to belong to *some* view (so we don't "steal" elements
			// that legitimately belong only to another, currently-hidden view).
			var trackedElsewhere = new HashSet<string>(
				_views.Where(view => !ReferenceEquals(view, currentView))
					.SelectMany(view => view.ElementIds),
				StringComparer.OrdinalIgnoreCase);

			foreach (var shape in diagram.NestedChildShapes.OfType<NodeShape>())
			{
				if (shape.ModelElement == null || !CanPersistElement(shape.ModelElement))
				{
					continue;
				}

				var elementId = shape.ModelElement.Id.ToString("D");
				var isTrackedByCurrentView = currentView.ElementIds.Any(id => string.Equals(id, elementId, StringComparison.OrdinalIgnoreCase));

				if (!isTrackedByCurrentView)
				{
					// Only adopt elements that aren't already owned by another view. This handles
					// elements added directly to the diagram surface (e.g. via toolbox/drag-drop)
					// that never went through AddElement().
					if (!shape.IsVisible || trackedElsewhere.Contains(elementId))
					{
						continue;
					}

					currentView.ElementIds.Add(elementId);
				}

				if (!shape.IsVisible)
				{
					// Don't record layout for hidden shapes; their bounds aren't meaningful for this view.
					continue;
				}

				var bounds = shape.AbsoluteBounds;
				var layout = currentView.Layout.FirstOrDefault(item => string.Equals(item.ElementId, elementId, StringComparison.OrdinalIgnoreCase));
				if (layout == null)
				{
					layout = new DiagramViewElementLayout
					{
						ElementId = elementId
					};

					currentView.Layout.Add(layout);
				}

				layout.X = bounds.X;
				layout.Y = bounds.Y;
				layout.Width = bounds.Width;
				layout.Height = bounds.Height;
			}

			// Prune only entries whose model element no longer exists at all (e.g. deleted elements),
			// not entries that are simply not currently visible on the diagram surface.
			currentView.ElementIds = currentView.ElementIds.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
			currentView.Layout.RemoveAll(layout => layout == null
				|| string.IsNullOrWhiteSpace(layout.ElementId)
				|| !currentView.ElementIds.Any(id => string.Equals(id, layout.ElementId, StringComparison.OrdinalIgnoreCase)));
		}

		public bool ContainsElement(ModelElement element)
		{
			if (element == null)
			{
				return false;
			}

			var currentView = GetCurrentView();
			return currentView != null
				&& currentView.ElementIds.Any(id => string.Equals(id, element.Id.ToString("D"), StringComparison.OrdinalIgnoreCase));
		}

		private DiagramViewDefinition EnsureCurrentView()
		{
			NormalizeState();
			return GetCurrentView();
		}

		private DiagramViewDefinition GetCurrentView()
		{
			return _views.FirstOrDefault(view => string.Equals(view.Name, _currentViewName, StringComparison.Ordinal));
		}

		private void EnsureDefaultView(ModelRoot modelRoot, Diagram diagram)
		{
			if (_views.Count > 0)
			{
				return;
			}

			var defaultView = new DiagramViewDefinition
			{
				Name = DefaultViewName,
				ElementIds = new List<string>(),
				Layout = new List<DiagramViewElementLayout>()
			};

			if (diagram != null)
			{
				foreach (var shape in diagram.NestedChildShapes.OfType<NodeShape>())
				{
					if (!CanPersistElement(shape.ModelElement))
					{
						continue;
					}

					defaultView.ElementIds.Add(shape.ModelElement.Id.ToString("D"));
					var bounds = shape.AbsoluteBounds;
					defaultView.Layout.Add(new DiagramViewElementLayout
					{
						ElementId = shape.ModelElement.Id.ToString("D"),
						X = bounds.X,
						Y = bounds.Y,
						Width = bounds.Width,
						Height = bounds.Height
					});
				}
			}
			else if (modelRoot != null)
			{
				foreach (var element in modelRoot.Types)
				{
					if (CanPersistElement(element))
					{
						defaultView.ElementIds.Add(element.Id.ToString("D"));
					}
				}
			}

			_views.Add(defaultView);
			_currentViewName = defaultView.Name;
		}

		private IEnumerable<ModelElement> ResolveViewElements(Store store, DiagramViewDefinition view)
		{
			foreach (var elementId in view.ElementIds.ToList())
			{
				Guid parsedId;
				if (!Guid.TryParse(elementId, out parsedId))
				{
					continue;
				}

				var element = store.ElementDirectory.AllElements.FirstOrDefault(candidate => candidate.Id == parsedId);
				if (CanPersistElement(element))
				{
					yield return element;
				}
			}
		}

		private static bool HasShapeOnDiagram(Diagram diagram, ModelElement element)
		{
			return PresentationViewsSubject.GetPresentation(element)
				.OfType<ShapeElement>()
				.Any(shape => shape.Diagram == diagram && !shape.IsDeleted && !shape.IsDeleting);
		}

		private void RestoreLayout(Diagram diagram, DiagramViewDefinition currentView)
		{
			foreach (var shape in diagram.NestedChildShapes.OfType<NodeShape>())
			{
				if (shape.ModelElement == null || !shape.IsVisible)
				{
					continue;
				}

				var layout = currentView.Layout.FirstOrDefault(item => string.Equals(item.ElementId, shape.ModelElement.Id.ToString("D"), StringComparison.OrdinalIgnoreCase));
				if (layout == null)
				{
					continue;
				}

				shape.AbsoluteBounds = new RectangleD(layout.X, layout.Y, layout.Width, layout.Height);
			}
		}

		private void NormalizeState()
		{
			foreach (var view in _views)
			{
				view.ElementIds = (view.ElementIds ?? new List<string>())
					.Where(id => !string.IsNullOrWhiteSpace(id))
					.Distinct(StringComparer.OrdinalIgnoreCase)
					.ToList();

				view.Layout = (view.Layout ?? new List<DiagramViewElementLayout>())
					.Where(layout => layout != null && !string.IsNullOrWhiteSpace(layout.ElementId))
					.GroupBy(layout => layout.ElementId, StringComparer.OrdinalIgnoreCase)
					.Select(group => group.First())
					.ToList();
			}

			if (_views.Count == 0)
			{
				_views.Add(new DiagramViewDefinition
				{
					Name = DefaultViewName,
					ElementIds = new List<string>(),
					Layout = new List<DiagramViewElementLayout>()
				});
			}

			if (string.IsNullOrWhiteSpace(_currentViewName) || !_views.Any(view => string.Equals(view.Name, _currentViewName, StringComparison.Ordinal)))
			{
				_currentViewName = _views[0].Name;
			}
		}

		private static bool CanPersistElement(ModelElement element)
		{
			return element is EntityModel
				|| element is EnumModel
				|| element is ModuleModel
				|| element is ModelInterface
				|| element is Comment;
		}

		private static string NormalizeViewName(string viewName)
		{
			return string.IsNullOrWhiteSpace(viewName) ? null : viewName.Trim();
		}

		private void MarkDirty()
		{
			_markDirty?.Invoke();
		}

		private void OnViewsChanged()
		{
			ViewsChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	[XmlRoot("diagramViews")]
	public sealed class DiagramViewStateFile
	{
		[XmlAttribute("current")]
		public string CurrentViewName { get; set; }

		[XmlElement("view")]
		public List<DiagramViewDefinition> Views { get; set; } = new List<DiagramViewDefinition>();
	}

	public sealed class DiagramViewDefinition
	{
		[XmlAttribute("name")]
		public string Name { get; set; }

		[XmlArray("elements")]
		[XmlArrayItem("element")]
		public List<string> ElementIds { get; set; } = new List<string>();

		[XmlArray("layout")]
		[XmlArrayItem("item")]
		public List<DiagramViewElementLayout> Layout { get; set; } = new List<DiagramViewElementLayout>();
	}

	public sealed class DiagramViewElementLayout
	{
		[XmlAttribute("id")]
		public string ElementId { get; set; }

		[XmlAttribute("x")]
		public double X { get; set; }

		[XmlAttribute("y")]
		public double Y { get; set; }

		[XmlAttribute("width")]
		public double Width { get; set; }

		[XmlAttribute("height")]
		public double Height { get; set; }
	}
}
