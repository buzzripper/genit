using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace Dyvenix.GenIt.DslPackage.Editors.Entity.Controls
{
	/// <summary>
	/// Editor control for EntityModel properties.
	/// Displays when an EntityModel shape is selected.
	/// </summary>
	public partial class EntityEditControl : UserControlBase
	{
		private EntityModel _entityModel;
		private bool _isUpdating;

		public EntityEditControl()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Initializes the control with the specified EntityModel.
		/// </summary>
		/// <param name="entityModel">The EntityModel to edit.</param>
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
				// Basic properties
				txtName.Text = _entityModel.Name ?? string.Empty;
				txtDescription.Text = _entityModel.Description ?? string.Empty;

				// Populate and select module
				LoadModules();

				// Checkboxes
				chkEnabled.IsChecked = _entityModel.Enabled;
				chkGenerateCode.IsChecked = _entityModel.GenerateCode;
				chkAuditable.IsChecked = _entityModel.Auditable;
				chkInclRowVersion.IsChecked = _entityModel.InclRowVersion;

				// List controls
				attributesControl.SetItems(ParseMultilineString(_entityModel.Attributes));
				usingsControl.SetItems(ParseMultilineString(_entityModel.Usings));

				// Restore splitter position
				if (_entityModel.EditorSplitterPosition > 0)
				{
					leftColumn.Width = new GridLength(_entityModel.EditorSplitterPosition, GridUnitType.Pixel);
				}
			}
			finally
			{
				_isUpdating = false;
			}
		}

		private void LoadModules()
		{
			cboModule.Items.Clear();
			cboModule.Items.Add(string.Empty); // Allow no selection

			var modelRoot = _entityModel.ModelRoot;
			if (modelRoot != null)
			{
				var modules = modelRoot.Types.OfType<ModuleModel>().OrderBy(m => m.Name);
				foreach (var module in modules)
				{
					cboModule.Items.Add(module.Name);
				}
			}

			// Select current value
			var currentModule = _entityModel.Module ?? string.Empty;
			cboModule.SelectedItem = currentModule;
			if (cboModule.SelectedItem == null)
			{
				cboModule.SelectedIndex = 0; // Select empty if module not found
			}
		}

		private IEnumerable<string> ParseMultilineString(string value)
		{
			if (string.IsNullOrEmpty(value))
				return Enumerable.Empty<string>();

			return value.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
						.Select(s => s.Trim())
						.Where(s => !string.IsNullOrEmpty(s));
		}

		private string JoinToMultilineString(IEnumerable<string> items)
		{
			if (items == null)
				return string.Empty;

			return string.Join("\n", items);
		}

		#region Event Handlers

		private void txtName_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			if (_isUpdating || _entityModel == null)
				return;

			UpdateModelProperty(NamedElement.NameDomainPropertyId, txtName.Text);
		}

		private void txtDescription_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			if (_isUpdating || _entityModel == null)
				return;

			UpdateModelProperty(ClassModelElement.DescriptionDomainPropertyId, txtDescription.Text);
		}

		private void cboModule_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			if (_isUpdating || _entityModel == null)
				return;

			var selectedModule = cboModule.SelectedItem as string ?? string.Empty;
			UpdateModelProperty(EntityModel.ModuleDomainPropertyId, selectedModule);
		}

		private void chkEnabled_Changed(object sender, RoutedEventArgs e)
		{
			if (_isUpdating || _entityModel == null)
				return;

			UpdateModelProperty(EntityModel.EnabledDomainPropertyId, chkEnabled.IsChecked ?? false);
		}

		private void chkGenerateCode_Changed(object sender, RoutedEventArgs e)
		{
			if (_isUpdating || _entityModel == null)
				return;

			UpdateModelProperty(EntityModel.GenerateCodeDomainPropertyId, chkGenerateCode.IsChecked ?? false);
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

		private void attributesControl_ItemsChanged(object sender, EventArgs e)
		{
			if (_isUpdating || _entityModel == null)
				return;

			var attributesValue = JoinToMultilineString(attributesControl.Items);
			UpdateModelProperty(EntityModel.AttributesDomainPropertyId, attributesValue);
		}

		private void usingsControl_ItemsChanged(object sender, EventArgs e)
		{
			if (_isUpdating || _entityModel == null)
				return;

			var usingsValue = JoinToMultilineString(usingsControl.Items);
			UpdateModelProperty(EntityModel.UsingsDomainPropertyId, usingsValue);
		}

		private void gridSplitter_DragCompleted(object sender, DragCompletedEventArgs e)
		{
			if (_entityModel == null)
				return;

			UpdateModelProperty(EntityModel.EditorSplitterPositionDomainPropertyId, leftColumn.ActualWidth);
		}

		#endregion

		private void UpdateModelProperty(System.Guid propertyId, object value)
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
