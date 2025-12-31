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
    }
}
