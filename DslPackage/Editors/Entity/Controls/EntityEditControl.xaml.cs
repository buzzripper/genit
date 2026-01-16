using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Dyvenix.GenIt.DslPackage.Editors.Entity.Controls
{
	public partial class EntityEditControl : UserControlBase
	{
		private EntityModel _entityModel;
		private bool _isUpdating;
		private ObservableCollection<PropertyModel> _properties;
		private Point _dragStartPoint;
		private bool _isDragging;
		private string _popupEditingField; // "Usings" or "Attributes"

		public EntityEditControl()
		{
			InitializeComponent();
		}

		public void Initialize(EntityModel entityModel)
		{
			_entityModel = entityModel;
			LoadFromModel();
		}

		private void LoadFromModel()
		{
			if (_entityModel == null)
				return;

			_isUpdating = true;
			try
			{
				// Header
				txtEntityName.Text = _entityModel.Name ?? "Entity";

				// Row 2: Name, Module
				txtName.Text = _entityModel.Name ?? string.Empty;
				LoadModules();

				// Row 3: TableName
				txtTableName.Text = _entityModel.TableName ?? string.Empty;

				// Checkboxes
				chkAuditable.IsChecked = _entityModel.Auditable;
				chkInclRowVersion.IsChecked = _entityModel.InclRowVersion;
				chkGenerateCode.IsChecked = _entityModel.GenerateCode;

				// Link buttons
				UpdateUsingsLabel();
				UpdateAttributesLabel();

				// Load DataType values (primitives + enum names from model)
				LoadDataTypes();

				// Load properties grid
				LoadProperties();
			}
			finally
			{
				_isUpdating = false;
			}
		}

		private void LoadModules()
		{
			cboModule.Items.Clear();
			cboModule.Items.Add(string.Empty);

			var modelRoot = _entityModel.ModelRoot;
			if (modelRoot != null)
			{
				var modules = modelRoot.Types.OfType<ModuleModel>().OrderBy(m => m.Name);
				foreach (var module in modules)
				{
					cboModule.Items.Add(module.Name);
				}
			}

			var currentModule = _entityModel.Module ?? string.Empty;
			cboModule.SelectedItem = currentModule;
			if (cboModule.SelectedItem == null)
				cboModule.SelectedIndex = 0;
		}

		private void LoadDataTypes()
		{
			// Load primitive types + enum names from the model
			var dataTypes = DataTypeHelper.GetAllDataTypes(_entityModel.Store);
			colDataType.ItemsSource = dataTypes;
		}

		private void LoadProperties()
		{
			var sortedProperties = _entityModel.Properties
				.OrderBy(p => p.DisplayOrder)
				.ThenBy(p => p.Name)
				.ToList();

			_properties = new ObservableCollection<PropertyModel>(sortedProperties);
			dgProperties.ItemsSource = _properties;
		}

		private void UpdateUsingsLabel()
		{
			var count = CountLines(_entityModel.Usings);
			btnUsings.Content = $"Usings ({count})";
		}

		private void UpdateAttributesLabel()
		{
			var count = CountLines(_entityModel.Attributes);
			btnAttributes.Content = $"Attributes ({count})";
		}

		private int CountLines(string text)
		{
			if (string.IsNullOrEmpty(text))
				return 0;
			return text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
		}

		#region Event Handlers - Entity Properties

		private void txtName_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (_isUpdating || _entityModel == null)
				return;

			UpdateModelProperty(NamedElement.NameDomainPropertyId, txtName.Text);
			txtEntityName.Text = txtName.Text;
		}

		private void txtTableName_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (_isUpdating || _entityModel == null)
				return;

			UpdateModelProperty(EntityModel.TableNameDomainPropertyId, txtTableName.Text);
		}

		private void cboModule_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (_isUpdating || _entityModel == null)
				return;

			var selectedModule = cboModule.SelectedItem as string ?? string.Empty;
			UpdateModelProperty(EntityModel.ModuleDomainPropertyId, selectedModule);
		}

		private void chkAuditable_Changed(object sender, RoutedEventArgs e)
		{
			if (_isUpdating || _entityModel == null)
				return;

			UpdateModelProperty(EntityModel.AuditableDomainPropertyId, chkAuditable.IsChecked ?? false);
		}

		private void chkInclRowVersion_Changed(object sender, RoutedEventArgs e)
		{
			if (_isUpdating || _entityModel == null)
				return;

			UpdateModelProperty(EntityModel.InclRowVersionDomainPropertyId, chkInclRowVersion.IsChecked ?? false);
		}

		private void chkGenerateCode_Changed(object sender, RoutedEventArgs e)
		{
			if (_isUpdating || _entityModel == null)
				return;

			UpdateModelProperty(EntityModel.GenerateCodeDomainPropertyId, chkGenerateCode.IsChecked ?? false);
		}

		#endregion

		#region Event Handlers - Usings/Attributes Popup

		private void btnUsings_Click(object sender, RoutedEventArgs e)
		{
			_popupEditingField = "Usings";
			popupTitle.Text = "Edit Usings (one per line)";
			txtPopupEditor.Text = _entityModel.Usings ?? string.Empty;
			popupEditor.IsOpen = true;
		}

		private void btnAttributes_Click(object sender, RoutedEventArgs e)
		{
			_popupEditingField = "Attributes";
			popupTitle.Text = "Edit Attributes (one per line)";
			txtPopupEditor.Text = _entityModel.Attributes ?? string.Empty;
			popupEditor.IsOpen = true;
		}

		private void popupOK_Click(object sender, RoutedEventArgs e)
		{
			if (_popupEditingField == "Usings")
			{
				UpdateModelProperty(EntityModel.UsingsDomainPropertyId, txtPopupEditor.Text);
				UpdateUsingsLabel();
			}
			else if (_popupEditingField == "Attributes")
			{
				UpdateModelProperty(EntityModel.AttributesDomainPropertyId, txtPopupEditor.Text);
				UpdateAttributesLabel();
			}
			popupEditor.IsOpen = false;
		}

		private void popupCancel_Click(object sender, RoutedEventArgs e)
		{
			popupEditor.IsOpen = false;
		}

		#endregion

		#region Event Handlers - Properties Grid

		private void btnAddProperty_Click(object sender, RoutedEventArgs e)
		{
			if (_entityModel == null)
				return;

			using (var transaction = _entityModel.Store.TransactionManager.BeginTransaction("Add Property"))
			{
				var newProperty = new PropertyModel(_entityModel.Store);
				newProperty.Name = GenerateNewPropertyName();
				newProperty.DataType = "String";
				newProperty.DisplayOrder = _properties.Count > 0 ? _properties.Max(p => p.DisplayOrder) + 1 : 0;
				_entityModel.Properties.Add(newProperty);
				transaction.Commit();

				_properties.Add(newProperty);
				dgProperties.SelectedItem = newProperty;
			}
		}

		private string GenerateNewPropertyName()
		{
			var baseName = "NewProperty";
			var existingNames = _entityModel.Properties.Select(p => p.Name).ToHashSet();
			var counter = 1;
			var name = baseName;
			while (existingNames.Contains(name))
			{
				name = $"{baseName}{counter++}";
			}
			return name;
		}

		private void btnDeleteProperty_Click(object sender, RoutedEventArgs e)
		{
			if (_entityModel == null || dgProperties.SelectedItem == null)
				return;

			var selectedProperty = dgProperties.SelectedItem as PropertyModel;
			if (selectedProperty == null)
				return;

			using (var transaction = _entityModel.Store.TransactionManager.BeginTransaction("Delete Property"))
			{
				selectedProperty.Delete();
				transaction.Commit();

				_properties.Remove(selectedProperty);
			}
		}

		private void dgProperties_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
		{
			if (_isUpdating || _entityModel == null)
				return;

			var property = e.Row.Item as PropertyModel;
			if (property == null)
				return;

			// Changes are automatically committed via binding; we just need to ensure transaction
			Dispatcher.BeginInvoke(new Action(() =>
			{
				if (!property.Store.TransactionManager.InTransaction)
				{
					// Property already updated via binding
				}
			}));
		}

		#endregion

		#region Drag and Drop for Row Reordering

		private void dgProperties_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			_dragStartPoint = e.GetPosition(null);
		}

		private void dgProperties_PreviewMouseMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton != MouseButtonState.Pressed)
				return;

			var mousePos = e.GetPosition(null);
			var diff = _dragStartPoint - mousePos;

			if (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
				Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
			{
				var row = FindVisualParent<DataGridRow>(e.OriginalSource as DependencyObject);
				if (row != null && row.Item is PropertyModel property)
				{
					_isDragging = true;
					DragDrop.DoDragDrop(row, property, DragDropEffects.Move);
					_isDragging = false;
				}
			}
		}

		private void dgProperties_Drop(object sender, DragEventArgs e)
		{
			if (!e.Data.GetDataPresent(typeof(PropertyModel)))
				return;

			var droppedProperty = e.Data.GetData(typeof(PropertyModel)) as PropertyModel;
			var targetRow = FindVisualParent<DataGridRow>(e.OriginalSource as DependencyObject);

			if (droppedProperty == null || targetRow == null)
				return;

			var targetProperty = targetRow.Item as PropertyModel;
			if (targetProperty == null || droppedProperty == targetProperty)
				return;

			var oldIndex = _properties.IndexOf(droppedProperty);
			var newIndex = _properties.IndexOf(targetProperty);

			if (oldIndex < 0 || newIndex < 0)
				return;

			using (var transaction = _entityModel.Store.TransactionManager.BeginTransaction("Reorder Properties"))
			{
				_properties.Move(oldIndex, newIndex);

				// Update DisplayOrder for all properties
				for (int i = 0; i < _properties.Count; i++)
				{
					_properties[i].DisplayOrder = i;
				}

				transaction.Commit();
			}
		}

		private static T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
		{
			var parentObject = System.Windows.Media.VisualTreeHelper.GetParent(child);
			if (parentObject == null)
				return null;

			if (parentObject is T parent)
				return parent;

			return FindVisualParent<T>(parentObject);
		}

		#endregion

		private void UpdateModelProperty(Guid propertyId, object value)
		{
			if (_entityModel == null)
				return;

			var property = _entityModel.Store.DomainDataDirectory.GetDomainProperty(propertyId);
			if (property == null)
				return;

			using (var transaction = _entityModel.Store.TransactionManager.BeginTransaction("Update " + property.Name))
			{
				property.SetValue(_entityModel, value);
				transaction.Commit();
			}
		}
	}
}
