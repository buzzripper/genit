using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

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
    }
}
