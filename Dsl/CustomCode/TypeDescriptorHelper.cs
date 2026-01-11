using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.VisualStudio.Modeling;

namespace Dyvenix.GenIt
{
    /// <summary>
    /// Helper class for creating common property descriptors.
    /// </summary>
    internal static class TypeDescriptorHelper
    {
        /// <summary>
        /// Creates a property descriptor that adds a multiline string editor.
        /// </summary>
        public static PropertyDescriptor CreateMultilineStringPropertyDescriptor(PropertyDescriptor baseDescriptor)
        {
            return new MultilineStringPropertyDescriptor(baseDescriptor);
        }

        /// <summary>
        /// Creates a read-only property descriptor.
        /// </summary>
        public static PropertyDescriptor CreateReadOnlyPropertyDescriptor(PropertyDescriptor baseDescriptor)
        {
            return new ReadOnlyPropertyDescriptor(baseDescriptor);
        }

        /// <summary>
        /// Creates a property descriptor that adds an enum type name dropdown editor.
        /// </summary>
        public static PropertyDescriptor CreateEnumTypeNamePropertyDescriptor(PropertyDescriptor baseDescriptor)
        {
            return new EnumTypeNamePropertyDescriptor(baseDescriptor);
        }

        /// <summary>
        /// Creates a property descriptor that adds a module name dropdown editor.
        /// </summary>
        public static PropertyDescriptor CreateModuleNamePropertyDescriptor(PropertyDescriptor baseDescriptor)
        {
            return new ModuleNamePropertyDescriptor(baseDescriptor);
        }

        /// <summary>
        /// Creates a property descriptor that adds a module name dropdown editor with no manual text editing.
        /// The value can only be set via the dropdown, not by typing.
        /// </summary>
        public static PropertyDescriptor CreateDropdownOnlyModuleNamePropertyDescriptor(PropertyDescriptor baseDescriptor)
        {
            return new DropdownOnlyModuleNamePropertyDescriptor(baseDescriptor);
        }

        /// <summary>
        /// A property descriptor wrapper that adds the module name dropdown editor
        /// and prevents manual text entry (dropdown-only).
        /// </summary>
        private class DropdownOnlyModuleNamePropertyDescriptor : PropertyDescriptor
        {
            private readonly PropertyDescriptor _innerDescriptor;
            private static readonly UITypeEditor _moduleEditor = new ModuleNameEditor();
            private static readonly TypeConverter _noEditConverter = new DropdownOnlyTypeConverter();

            public DropdownOnlyModuleNamePropertyDescriptor(PropertyDescriptor innerDescriptor)
                : base(innerDescriptor)
            {
                _innerDescriptor = innerDescriptor;
            }

            public override Type ComponentType => _innerDescriptor.ComponentType;
            public override bool IsReadOnly => false; // Not read-only, but only editable via dropdown
            public override Type PropertyType => _innerDescriptor.PropertyType;
            public override string Category => _innerDescriptor.Category;
            public override string Description => _innerDescriptor.Description;
            public override string DisplayName => _innerDescriptor.DisplayName;

            public override bool CanResetValue(object component) => _innerDescriptor.CanResetValue(component);
            public override object GetValue(object component) => _innerDescriptor.GetValue(component);
            public override void ResetValue(object component) => _innerDescriptor.ResetValue(component);
            public override void SetValue(object component, object value) => _innerDescriptor.SetValue(component, value);
            public override bool ShouldSerializeValue(object component) => _innerDescriptor.ShouldSerializeValue(component);

            public override object GetEditor(Type editorBaseType)
            {
                if (editorBaseType == typeof(UITypeEditor))
                {
                    return _moduleEditor;
                }
                return base.GetEditor(editorBaseType);
            }

            /// <summary>
            /// Returns a TypeConverter that prevents manual text entry.
            /// </summary>
            public override TypeConverter Converter => _noEditConverter;
        }

        /// <summary>
        /// TypeConverter that prevents manual text entry by not allowing conversion from string.
        /// Values can only be set via the dropdown editor.
        /// </summary>
        private class DropdownOnlyTypeConverter : TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                // Don't allow conversion from string - this prevents manual text entry
                return false;
            }

            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                // Allow conversion to string for display purposes
                if (destinationType == typeof(string))
                    return true;
                return base.CanConvertTo(context, destinationType);
            }

            public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
            {
                if (destinationType == typeof(string))
                {
                    return value?.ToString() ?? string.Empty;
                }
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }

        /// <summary>
        /// A property descriptor wrapper that adds the multiline string editor to a property.
        /// </summary>
        private class MultilineStringPropertyDescriptor : PropertyDescriptor
        {
            private readonly PropertyDescriptor _innerDescriptor;
            private static readonly UITypeEditor _multilineEditor = new SimpleMultilineStringEditor();

            public MultilineStringPropertyDescriptor(PropertyDescriptor innerDescriptor)
                : base(innerDescriptor)
            {
                _innerDescriptor = innerDescriptor;
            }

            public override Type ComponentType => _innerDescriptor.ComponentType;
            public override bool IsReadOnly => _innerDescriptor.IsReadOnly;
            public override Type PropertyType => _innerDescriptor.PropertyType;
            public override string Category => _innerDescriptor.Category;
            public override string Description => _innerDescriptor.Description;
            public override string DisplayName => _innerDescriptor.DisplayName;

            public override bool CanResetValue(object component) => _innerDescriptor.CanResetValue(component);
            public override object GetValue(object component) => _innerDescriptor.GetValue(component);
            public override void ResetValue(object component) => _innerDescriptor.ResetValue(component);
            public override void SetValue(object component, object value) => _innerDescriptor.SetValue(component, value);
            public override bool ShouldSerializeValue(object component) => _innerDescriptor.ShouldSerializeValue(component);

            /// <summary>
            /// Returns the editor to use for this property - the multiline string editor.
            /// </summary>
            public override object GetEditor(Type editorBaseType)
            {
                if (editorBaseType == typeof(UITypeEditor))
                {
                    return _multilineEditor;
                }
                return base.GetEditor(editorBaseType);
            }
        }

        /// <summary>
        /// A property descriptor wrapper that makes the property read-only.
        /// </summary>
        private class ReadOnlyPropertyDescriptor : PropertyDescriptor
        {
            private readonly PropertyDescriptor _baseDescriptor;

            public ReadOnlyPropertyDescriptor(PropertyDescriptor baseDescriptor)
                : base(baseDescriptor)
            {
                _baseDescriptor = baseDescriptor;
            }

            public override Type ComponentType => _baseDescriptor.ComponentType;
            public override bool IsReadOnly => true;
            public override Type PropertyType => _baseDescriptor.PropertyType;
            public override string Category => _baseDescriptor.Category;
            public override string Description => _baseDescriptor.Description;
            public override string DisplayName => _baseDescriptor.DisplayName;

            public override bool CanResetValue(object component) => false;
            public override object GetValue(object component) => _baseDescriptor.GetValue(component);
            public override void ResetValue(object component) { }
            public override void SetValue(object component, object value) { } // No-op for read-only
            public override bool ShouldSerializeValue(object component) => _baseDescriptor.ShouldSerializeValue(component);

            public override object GetEditor(Type editorBaseType) => _baseDescriptor.GetEditor(editorBaseType);
            public override TypeConverter Converter => _baseDescriptor.Converter;
        }

        /// <summary>
        /// A property descriptor wrapper that adds the enum type name dropdown editor.
        /// </summary>
        private class EnumTypeNamePropertyDescriptor : PropertyDescriptor
        {
            private readonly PropertyDescriptor _innerDescriptor;
            private static readonly UITypeEditor _enumEditor = new EnumTypeNameEditor();

            public EnumTypeNamePropertyDescriptor(PropertyDescriptor innerDescriptor)
                : base(innerDescriptor)
            {
                _innerDescriptor = innerDescriptor;
            }

            public override Type ComponentType => _innerDescriptor.ComponentType;
            public override bool IsReadOnly => _innerDescriptor.IsReadOnly;
            public override Type PropertyType => _innerDescriptor.PropertyType;
            public override string Category => _innerDescriptor.Category;
            public override string Description => _innerDescriptor.Description;
            public override string DisplayName => _innerDescriptor.DisplayName;

            public override bool CanResetValue(object component) => _innerDescriptor.CanResetValue(component);
            public override object GetValue(object component) => _innerDescriptor.GetValue(component);
            public override void ResetValue(object component) => _innerDescriptor.ResetValue(component);
            public override void SetValue(object component, object value) => _innerDescriptor.SetValue(component, value);
            public override bool ShouldSerializeValue(object component) => _innerDescriptor.ShouldSerializeValue(component);

            /// <summary>
            /// Returns the editor to use for this property - the enum type name editor.
            /// </summary>
            public override object GetEditor(Type editorBaseType)
            {
                if (editorBaseType == typeof(UITypeEditor))
                {
                    return _enumEditor;
                }
                return base.GetEditor(editorBaseType);
            }
        }

        /// <summary>
        /// A property descriptor wrapper that adds the module name dropdown editor.
        /// </summary>
        private class ModuleNamePropertyDescriptor : PropertyDescriptor
        {
            private readonly PropertyDescriptor _innerDescriptor;
            private static readonly UITypeEditor _moduleEditor = new ModuleNameEditor();

            public ModuleNamePropertyDescriptor(PropertyDescriptor innerDescriptor)
                : base(innerDescriptor)
            {
                _innerDescriptor = innerDescriptor;
            }

            public override Type ComponentType => _innerDescriptor.ComponentType;
            public override bool IsReadOnly => _innerDescriptor.IsReadOnly;
            public override Type PropertyType => _innerDescriptor.PropertyType;
            public override string Category => _innerDescriptor.Category;
            public override string Description => _innerDescriptor.Description;
            public override string DisplayName => _innerDescriptor.DisplayName;

            public override bool CanResetValue(object component) => _innerDescriptor.CanResetValue(component);
            public override object GetValue(object component) => _innerDescriptor.GetValue(component);
            public override void ResetValue(object component) => _innerDescriptor.ResetValue(component);
            public override void SetValue(object component, object value) => _innerDescriptor.SetValue(component, value);
            public override bool ShouldSerializeValue(object component) => _innerDescriptor.ShouldSerializeValue(component);

            public override object GetEditor(Type editorBaseType)
            {
                if (editorBaseType == typeof(UITypeEditor))
                {
                    return _moduleEditor;
                }
                return base.GetEditor(editorBaseType);
            }
        }

        /// <summary>
        /// Simple multiline string editor for property grid.
        /// </summary>
        private class SimpleMultilineStringEditor : UITypeEditor
        {
            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
            {
                return UITypeEditorEditStyle.DropDown;
            }

            public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
            {
                if (provider != null)
                {
                    IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                    if (edSvc != null)
                    {
                        TextBox textBox = new TextBox
                        {
                            Multiline = true,
                            ScrollBars = ScrollBars.Vertical,
                            AcceptsReturn = true,
                            Width = 300,
                            Height = 150,
                            Text = value as string ?? string.Empty
                        };

                        edSvc.DropDownControl(textBox);
                        return textBox.Text;
                    }
                }
                return value;
            }
        }

        /// <summary>
        /// UITypeEditor for EnumTypeName property that provides a dropdown of available enum types.
        /// </summary>
        private class EnumTypeNameEditor : UITypeEditor
        {
            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
            {
                return UITypeEditorEditStyle.DropDown;
            }

            public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
            {
                if (provider != null)
                {
                    var editorService = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
                    if (editorService != null && context?.Instance is PropertyModel property)
                    {
                        // Get enum names from the model
                        var enumNames = GetEnumNames(property);

                        if (enumNames.Count > 0)
                        {
                            // Create and show the listbox
                            var listBox = new ListBox();
                            listBox.SelectionMode = SelectionMode.One;
                            listBox.BorderStyle = BorderStyle.None;

                            foreach (var enumName in enumNames)
                            {
                                listBox.Items.Add(enumName);
                            }

                            // Pre-select current value if it exists
                            if (value is string currentValue && !string.IsNullOrEmpty(currentValue))
                            {
                                int index = listBox.Items.IndexOf(currentValue);
                                if (index >= 0)
                                {
                                    listBox.SelectedIndex = index;
                                }
                            }

                            // Handle selection
                            listBox.Click += (sender, e) =>
                            {
                                if (listBox.SelectedItem != null)
                                {
                                    editorService.CloseDropDown();
                                }
                            };

                            // Show the dropdown
                            editorService.DropDownControl(listBox);

                            // Return selected value
                            if (listBox.SelectedItem != null)
                            {
                                return listBox.SelectedItem.ToString();
                            }
                        }
                    }
                }

                return value;
            }

            /// <summary>
            /// Gets the list of enum names from the model.
            /// </summary>
            private System.Collections.Generic.List<string> GetEnumNames(PropertyModel property)
            {
                var enumNames = new System.Collections.Generic.List<string>();

                try
                {
                    var entity = property.EntityModel;
                    if (entity?.Store != null)
                    {
                        var store = entity.Store;
                        var root = store.ElementDirectory.FindElements<ModelRoot>().FirstOrDefault();
                        if (root != null)
                        {
                            enumNames = root.Types
                                .OfType<EnumModel>()
                                .Select(e => e.Name)
                                .Where(name => !string.IsNullOrWhiteSpace(name))
                                .OrderBy(name => name)
                                .ToList();
                        }
                    }
                }
                catch
                {
                    // If we can't read the model, return empty list
                }

                return enumNames;
            }
        }

        /// <summary>
        /// UITypeEditor for Module property that provides a dropdown of available module names.
        /// </summary>
        private class ModuleNameEditor : UITypeEditor
        {
            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
            {
                return UITypeEditorEditStyle.DropDown;
            }

            public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
            {
                if (provider == null)
                    return value;

                var editorService = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
                if (editorService == null)
                    return value;

                // Try to get EntityModel from context
                EntityModel entity = null;
                Store store = null;

                // Try direct cast first
                if (context?.Instance is EntityModel directEntity)
                {
                    entity = directEntity;
                    store = entity.Store;
                }
                // Try array (multi-select scenario)
                else if (context?.Instance is object[] instances && instances.Length > 0 && instances[0] is EntityModel firstEntity)
                {
                    entity = firstEntity;
                    store = entity.Store;
                }
                // Try to get store from any ModelElement
                else if (context?.Instance is ModelElement modelElement)
                {
                    store = modelElement.Store;
                }

                // If we couldn't get the store, we can't show module names
                if (store == null)
                    return value;

                // Get module names from the model
                var moduleNames = GetModuleNames(store);

                // Create and show the listbox
                var listBox = new ListBox();
                listBox.SelectionMode = SelectionMode.One;
                listBox.BorderStyle = BorderStyle.None;

                // Add empty option first to allow clearing the value
                listBox.Items.Add("(None)");

                foreach (var moduleName in moduleNames)
                {
                    listBox.Items.Add(moduleName);
                }

                // Pre-select current value if it exists
                if (value is string currentValue && !string.IsNullOrEmpty(currentValue))
                {
                    int index = listBox.Items.IndexOf(currentValue);
                    if (index >= 0)
                    {
                        listBox.SelectedIndex = index;
                    }
                }
                else
                {
                    listBox.SelectedIndex = 0; // Select "(None)"
                }

                // Handle selection
                listBox.Click += (sender, e) =>
                {
                    editorService.CloseDropDown();
                };

                // Show the dropdown
                editorService.DropDownControl(listBox);

                // Return selected value
                if (listBox.SelectedItem != null)
                {
                    string selected = listBox.SelectedItem.ToString();
                    return selected == "(None)" ? string.Empty : selected;
                }

                return value;
            }

            /// <summary>
            /// Gets the list of module names from the store.
            /// </summary>
            private System.Collections.Generic.List<string> GetModuleNames(Store store)
            {
                var moduleNames = new System.Collections.Generic.List<string>();

                try
                {
                    if (store == null)
                        return moduleNames;

                    // Try to find modules directly from the ElementDirectory
                    // This should find all ModuleModel instances regardless of how they're linked
                    var modules = store.ElementDirectory.FindElements<ModuleModel>();
                    
                    if (modules != null)
                    {
                        moduleNames = modules
                            .Where(m => m != null && !m.IsDeleting && !m.IsDeleted)
                            .Select(m => m.Name)
                            .Where(name => !string.IsNullOrWhiteSpace(name))
                            .OrderBy(name => name)
                            .ToList();
                    }
                }
                catch
                {
                    // If we can't read the model, return empty list
                }

                return moduleNames;
            }
        }
    }
}
