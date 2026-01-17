using Microsoft.VisualStudio.Modeling;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Dyvenix.GenIt.DslPackage.Editors.Entity.Controls
{
	/// <summary>
	/// Converter that enables/shows the Length field only for String and ByteArray types.
	/// </summary>
	public class DataTypeLengthEnabledConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var dataType = value as string;
			var isEnabled = dataType == "String" || dataType == "ByteArray";

			// If parameter is "bool", return boolean for IsEnabled binding
			if (parameter as string == "bool")
				return isEnabled;

			// Otherwise return Visibility
			return isEnabled ? Visibility.Visible : Visibility.Hidden;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public partial class EntityEditControl : UserControlBase
	{
		private const string PropertyModelDragFormat = "GenIt.PropertyModel";
		private EntityModel _entityModel;
		private bool _isUpdating;
		private ObservableCollection<PropertyModel> _properties;
		private Point _dragStartPoint;
		private bool _isDragging;
		private PropertyModel _draggedProperty;
		private string _popupEditingField; // "Usings" or "Attributes"
		private string[] _dataTypes;
		private Transaction _currentEditTransaction;

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
			_dataTypes = DataTypeHelper.GetAllDataTypes(_entityModel.Store).ToArray();
		}

		/// <summary>
		/// Gets the available data types for ComboBox binding in the DataGrid.
		/// </summary>
		public string[] DataTypes => _dataTypes;

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

		private void DataTypeComboBox_Loaded(object sender, RoutedEventArgs e)
		{
			var comboBox = sender as ComboBox;
			if (comboBox != null && _dataTypes != null)
			{
				comboBox.ItemsSource = _dataTypes;
			}
		}

		private void DataTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			// Skip if this is triggered during ComboBox initialization (no removed items means initial load)
			if (e.RemovedItems.Count == 0)
				return;

			// Commit edit and refresh the row to update Max Len visibility
			if (dgProperties.SelectedItem != null)
			{
				dgProperties.CommitEdit(DataGridEditingUnit.Cell, true);
				dgProperties.CommitEdit(DataGridEditingUnit.Row, true);
				dgProperties.Items.Refresh();
			}
		}

		private void dgProperties_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
		{
			if (_isUpdating || _entityModel == null)
				return;

			// Commit or rollback the transaction started in BeginningEdit
			if (_currentEditTransaction != null)
			{
				if (e.EditAction == DataGridEditAction.Commit)
				{
					_currentEditTransaction.Commit();
				}
				else
				{
					_currentEditTransaction.Rollback();
				}
				_currentEditTransaction.Dispose();
				_currentEditTransaction = null;
			}
		}

		private void dgProperties_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
		{
			if (_isUpdating || _entityModel == null)
				return;

			var property = e.Row.Item as PropertyModel;
			if (property == null)
				return;

			// Start a transaction for this edit operation
			string propertyName = GetEditedPropertyName(e.Column);
			_currentEditTransaction = property.Store.TransactionManager.BeginTransaction("Update " + propertyName);
		}



		private string GetEditedPropertyName(DataGridColumn column)
		{
			if (column.Header == null)
				return null;

			var header = column.Header.ToString();
			switch (header)
			{
				case "Name": return "Name";
				case "DataType": return "DataType";
				case "Max Len": return "Length";
				case "PK": return "IsPrimaryKey";
				case "Idnt": return "IsIdentity";
				case "Null": return "IsNullable";
				case "Idx": return "IsIndexed";
				case "UIdx": return "IsIndexUnique";
				case "CIdx": return "IsIndexClustered";
				default: return header;
			}
		}

		#endregion

		#region Drag and Drop for Row Reordering

		private void dgProperties_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			// Only start drag from the handle column
			if (IsOverDragHandle(e.OriginalSource as DependencyObject))
			{
				_dragStartPoint = e.GetPosition(null);
			}
			else
			{
				_dragStartPoint = new Point(0, 0);
			}
		}

		private void dgProperties_PreviewMouseMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton != MouseButtonState.Pressed)
				return;

			// Only drag if started from handle
			if (_dragStartPoint.X == 0 && _dragStartPoint.Y == 0)
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
					_draggedProperty = property;
					var dataObject = new DataObject(PropertyModelDragFormat, true);
					DragDrop.DoDragDrop(row, dataObject, DragDropEffects.Move);
					_isDragging = false;
					_draggedProperty = null;
					_dragStartPoint = new Point(0, 0);
				}
			}
		}

		private bool IsOverDragHandle(DependencyObject source)
		{
			while (source != null)
			{
				if (source is FrameworkElement element && element.Tag as string == "DragHandle")
					return true;
				source = System.Windows.Media.VisualTreeHelper.GetParent(source);
			}
			return false;
		}

		private void dgProperties_DragOver(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(PropertyModelDragFormat))
			{
				e.Effects = DragDropEffects.Move;
			}
			else
			{
				e.Effects = DragDropEffects.None;
			}
			e.Handled = true;
		}

		private void dgProperties_Drop(object sender, DragEventArgs e)
		{
			try
			{
				if (!e.Data.GetDataPresent(PropertyModelDragFormat) || _draggedProperty == null)
					return;

				var targetRow = FindVisualParent<DataGridRow>(e.OriginalSource as DependencyObject);
				if (targetRow == null)
					return;

				var targetProperty = targetRow.Item as PropertyModel;
				if (targetProperty == null || _draggedProperty == targetProperty)
					return;

				var oldIndex = _properties.IndexOf(_draggedProperty);
				var newIndex = _properties.IndexOf(targetProperty);

				if (oldIndex < 0 || newIndex < 0)
					return;

				using (var transaction = _entityModel.Store.TransactionManager.BeginTransaction("Reorder Properties"))
				{
					// Update DisplayOrder for all properties based on new order
					if (oldIndex < newIndex)
					{
						// Moving down: shift items up
						for (int i = oldIndex; i < newIndex; i++)
						{
							_properties[i + 1].DisplayOrder = i;
						}
						_draggedProperty.DisplayOrder = newIndex;
					}
					else
					{
						// Moving up: shift items down
						for (int i = oldIndex; i > newIndex; i--)
						{
							_properties[i - 1].DisplayOrder = i;
						}
						_draggedProperty.DisplayOrder = newIndex;
					}

					transaction.Commit();
				}

				// Refresh the collection outside the transaction
				_isUpdating = true;
				try
				{
					_properties.Move(oldIndex, newIndex);
				}
				finally
				{
					_isUpdating = false;
				}
			}
			catch (COMException ex) when (ex.ErrorCode == unchecked((int)0x80040064))
			{
				// DV_E_FORMATETC - suppress this harmless COM error
			}
			finally
			{
				e.Handled = true;
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

		private void dgProperties_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

		}
	}
}
