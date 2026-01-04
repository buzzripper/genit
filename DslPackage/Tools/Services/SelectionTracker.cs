using Dyvenix.GenIt.DslPackage.CodeGen.Misc;
using Dyvenix.GenIt.DslPackage.Tools.Services.ViewModels;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections;
using System.Reflection;

namespace Dyvenix.GenIt.DslPackage.Tools.Services
{
	/// <summary>
	/// Tracks selection changes in the DSL designer and updates the GenItEditorWindow accordingly.
	/// Shows the service editor when a ServiceModel is selected, hides it otherwise.
	/// </summary>
	internal class SelectionTracker : IVsSelectionEvents, IDisposable
	{
		private readonly GenItPackage _package;
		private readonly IVsMonitorSelection _monitorSelection;
		private uint _selectionEventsCookie;
		private bool _isDisposed;

		public SelectionTracker(GenItPackage package)
		{
			_package = package ?? throw new ArgumentNullException(nameof(package));

			ThreadHelper.ThrowIfNotOnUIThread();

			_monitorSelection = Package.GetGlobalService(typeof(SVsShellMonitorSelection)) as IVsMonitorSelection;
			if (_monitorSelection != null)
			{
				_monitorSelection.AdviseSelectionEvents(this, out _selectionEventsCookie);
			}
		}

		public int OnSelectionChanged(
			IVsHierarchy pHierOld, uint itemidOld, IVsMultiItemSelect pMISOld, ISelectionContainer pSCOld,
			IVsHierarchy pHierNew, uint itemidNew, IVsMultiItemSelect pMISNew, ISelectionContainer pSCNew)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			GenItEditorWindow toolWindow = GetToolWindow();
			try
			{
				var selectedServiceModel = GetSelectedServiceModel(pSCOld);
				if (selectedServiceModel != null)
				{
					if (toolWindow?.Control != null)
					{
						if (selectedServiceModel != null)
						{
							// Convert domain ServiceModel to ViewModel and show editor
							var entityViewModel = CreateEntityViewModelFromServiceModel(selectedServiceModel);
							toolWindow.Control.ShowServiceEditor(entityViewModel, selectedServiceModel.Version);

							// Ensure the tool window is visible
							var frame = (IVsWindowFrame)toolWindow.Frame;
							frame?.Show();

							return VSConstants.S_OK;
						}
					}
				}
			}
			catch (Exception ex)
			{
				OutputHelper.WriteError($"SelectionTracker.OnSelectionChanged error: {ex.Message}");
			}

			toolWindow?.Control.HideServiceEditor();

			return VSConstants.S_OK;
		}

		private ServiceModel GetSelectedServiceModel(ISelectionContainer selectionContainer)
		{
			if (selectionContainer is GenItDocView docView)
			{
				var selectedElements = typeof(GenItDocView).GetProperty("SelectedElements", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)?.GetValue(selectionContainer) as ArrayList;
				if (selectedElements != null && selectedElements.Count == 1 && selectedElements[0] is ServiceModel)
					return selectedElements[0] as ServiceModel;
			}
			return null;
		}

		//private ServiceModel GetSelectedServiceModel(ISelectionContainer selectionContainer)
		//{
		//	ThreadHelper.ThrowIfNotOnUIThread();

		//	if (selectionContainer == null)
		//		return null;

		//	try
		//	{
		//		uint count;
		//		selectionContainer.CountObjects((uint)Constants.GETOBJS_SELECTED, out count);

		//		if (count == 0)
		//			return null;

		//		object[] selectedObjects = new object[count];
		//		selectionContainer.GetObjects((uint)Constants.GETOBJS_SELECTED, count, selectedObjects);

		//		foreach (var obj in selectedObjects)
		//		{
		//			// Check if the selection is a ServiceModel directly
		//			if (obj is ServiceModel serviceModel)
		//			{
		//				return serviceModel;
		//			}

		//			// Check if it's an ElementListCompartment (compartment shape)
		//			if (obj is ElementListCompartment compartment)
		//			{
		//				// Check if any item in this compartment is a ServiceModel
		//				if (compartment.Items != null)
		//				{
		//					foreach (var item in compartment.Items)
		//					{
		//						if (item is ServiceModel sm)
		//						{
		//							// This compartment contains ServiceModels
		//							// Try to find the focused item by checking the diagram's selection
		//							return FindFocusedServiceModelInCompartment(compartment);
		//						}
		//					}
		//				}
		//			}

		//			// Check if the selection is a presentation element
		//			if (obj is PresentationElement pe)
		//			{
		//				// Direct ServiceModel check
		//				if (pe.ModelElement is ServiceModel sm)
		//				{
		//					return sm;
		//				}

		//				// Check if it's a CompartmentShape - look for selected compartment items
		//				if (pe is CompartmentShape compartmentShape)
		//				{
		//					var selectedItem = GetSelectedItemFromCompartmentShape(compartmentShape);
		//					if (selectedItem is ServiceModel csm)
		//					{
		//						return csm;
		//					}
		//				}
		//			}
		//		}
		//	}
		//	catch (Exception ex)
		//	{
		//		OutputHelper.WriteError($"GetSelectedServiceModel error: {ex.Message}");
		//	}

		//	return null;
		//}

		//private ServiceModel FindFocusedServiceModelInCompartment(ElementListCompartment compartment)
		//{
		//	try
		//	{
		//		// Get the diagram to check what's actually focused
		//		var diagram = compartment.Diagram;
		//		if (diagram != null && diagram.ActiveDiagramView?.DiagramClientView?.Selection != null)
		//		{
		//			var selection = diagram.ActiveDiagramView.DiagramClientView.Selection;

		//			// Iterate through the selection using the enumerator
		//			foreach (DiagramItem view in selection)
		//			{
		//				if (view.Shape == compartment && view.SubField is ListItemSubField listItemSubField)
		//				{
		//					int row = listItemSubField.Row;
		//					if (row >= 0 && compartment.Items != null && row < compartment.Items.Count)
		//					{
		//						var item = compartment.Items[row];
		//						if (item is ServiceModel sm)
		//						{
		//							return sm;
		//						}
		//					}
		//				}
		//			}
		//		}

		//		// Fallback: If we can't find the focused item, return the first ServiceModel if there's only one
		//		if (compartment.Items != null && compartment.Items.Count == 1)
		//		{
		//			var firstItem = compartment.Items[0];
		//			if (firstItem is ServiceModel sm)
		//			{
		//				return sm;
		//			}
		//		}
		//	}
		//	catch (Exception ex)
		//	{
		//		OutputHelper.WriteError($"FindFocusedServiceModelInCompartment error: {ex.Message}");
		//	}
		//	return null;
		//}

		//private ModelElement GetSelectedItemFromCompartmentShape(CompartmentShape shape)
		//{
		//	try
		//	{
		//		// Iterate through all compartments in the shape
		//		foreach (var nestedShape in shape.NestedChildShapes)
		//		{
		//			if (nestedShape is ElementListCompartment compartment)
		//			{
		//				// Check if this compartment contains ServiceModels
		//				if (compartment.Items != null)
		//				{
		//					foreach (var item in compartment.Items)
		//					{
		//						if (item is ServiceModel)
		//						{
		//							// Try to find the selected one
		//							var selected = FindFocusedServiceModelInCompartment(compartment);
		//							if (selected != null)
		//							{
		//								return selected;
		//							}
		//						}
		//					}
		//				}
		//			}
		//		}
		//	}
		//	catch (Exception ex)
		//	{
		//		OutputHelper.WriteError($"GetSelectedItemFromCompartmentShape error: {ex.Message}");
		//	}
		//	return null;
		//}

		//private (ServiceViewModel, ObservableCollection<PropertyViewModel>) CreateViewModelsFromServiceModel(ServiceModel serviceModel)
		//{
		//	var serviceVm = new ServiceViewModel
		//	{
		//		Enabled = serviceModel.Enabled,
		//		InclCreate = serviceModel.InclCreate,
		//		InclUpdate = serviceModel.InclUpdate,
		//		InclDelete = serviceModel.InclDelete,
		//		InclController = serviceModel.InclController,
		//		ControllerVersion = serviceModel.Version ?? "v1"
		//	};

		//	var propertyVms = new ObservableCollection<PropertyViewModel>();
		//	if (serviceModel.EntityModeled != null)
		//	{
		//		foreach (var prop in serviceModel.EntityModeled.Properties)
		//		{
		//			propertyVms.Add(PropertyViewModel.CreateNew(
		//				Guid.NewGuid(),
		//				prop.Name,
		//				prop.DataType.ToString(),
		//				prop.IsPrimaryKey
		//			));
		//		}
		//	}
		//}

		private EntityViewModel CreateEntityViewModelFromServiceModel(ServiceModel serviceModel)
		{
			if (serviceModel == null)
				throw new ArgumentNullException(nameof(serviceModel));

			return new EntityViewModel(serviceModel.EntityModeled);
		}

		private GenItEditorWindow GetToolWindow()
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			try
			{
				var window = _package.FindToolWindow(typeof(GenItEditorWindow), 0, true) as GenItEditorWindow;
				return window;
			}
			catch (Exception ex)
			{
				OutputHelper.WriteError($"SelectionTracker.GetToolWindow error: {ex.Message}");
				return null;
			}
		}

		public int OnElementValueChanged(uint elementid, object varValueOld, object varValueNew)
		{
			return VSConstants.S_OK;
		}

		public int OnCmdUIContextChanged(uint dwCmdUICookie, int fActive)
		{
			return VSConstants.S_OK;
		}

		public void Dispose()
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			if (!_isDisposed)
			{
				if (_monitorSelection != null && _selectionEventsCookie != 0)
				{
					_monitorSelection.UnadviseSelectionEvents(_selectionEventsCookie);
					_selectionEventsCookie = 0;
				}

				_isDisposed = true;
			}
		}

		private static class Constants
		{
			public const int GETOBJS_SELECTED = 1;
		}
	}
}
