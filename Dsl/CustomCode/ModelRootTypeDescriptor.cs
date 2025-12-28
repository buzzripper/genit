using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Design;
using System;
using System.ComponentModel;
using System.Drawing.Design;

namespace Dyvenix.GenIt
{
	/// <summary>
	/// Custom TypeDescriptionProvider for ModelRoot to add folder browser to path properties.
	/// </summary>
	public class ModelRootTypeDescriptionProvider : ElementTypeDescriptionProvider
	{
		/// <summary>
		/// Creates a custom type descriptor for the ModelRoot class.
		/// </summary>
		protected override ElementTypeDescriptor CreateTypeDescriptor(ICustomTypeDescriptor parent, ModelElement element)
		{
			if (element is ModelRoot modelRoot)
			{
				return new ModelRootTypeDescriptor(parent, modelRoot);
			}
			return base.CreateTypeDescriptor(parent, element);
		}
	}

	/// <summary>
	/// Custom TypeDescriptor for ModelRoot that adds the FolderPathEditor to path properties.
	/// </summary>
	public class ModelRootTypeDescriptor : ElementTypeDescriptor
	{
		public ModelRootTypeDescriptor(ICustomTypeDescriptor parent, ModelRoot modelRoot)
			: base(parent, modelRoot)
		{
		}

		/// <summary>
		/// Returns a collection of property descriptors for the ModelRoot.
		/// </summary>
		public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection properties = base.GetProperties(attributes);
			PropertyDescriptorCollection newProperties = new PropertyDescriptorCollection(null);

			foreach (PropertyDescriptor prop in properties)
			{
				// Add FolderPathEditor to EntitiesPath property
				if (prop.Name == "EntitiesPath")
				{
					newProperties.Add(new FolderPathPropertyDescriptor(prop));
				}
				else
				{
					newProperties.Add(prop);
				}
			}

			return newProperties;
		}
	}

	/// <summary>
	/// A property descriptor wrapper that adds the FolderPathEditor to a property.
	/// </summary>
	public class FolderPathPropertyDescriptor : PropertyDescriptor
	{
		private readonly PropertyDescriptor _innerDescriptor;

		public FolderPathPropertyDescriptor(PropertyDescriptor innerDescriptor)
			: base(innerDescriptor)
		{
			_innerDescriptor = innerDescriptor;
		}

		public override Type ComponentType => _innerDescriptor.ComponentType;
		public override bool IsReadOnly => _innerDescriptor.IsReadOnly;
		public override Type PropertyType => _innerDescriptor.PropertyType;

		public override bool CanResetValue(object component) => _innerDescriptor.CanResetValue(component);
		public override object GetValue(object component) => _innerDescriptor.GetValue(component);
		public override void ResetValue(object component) => _innerDescriptor.ResetValue(component);
		public override void SetValue(object component, object value) => _innerDescriptor.SetValue(component, value);
		public override bool ShouldSerializeValue(object component) => _innerDescriptor.ShouldSerializeValue(component);

		/// <summary>
		/// Returns the editor to use for this property - the FolderPathEditor.
		/// </summary>
		public override object GetEditor(Type editorBaseType)
		{
			if (editorBaseType == typeof(UITypeEditor))
			{
				var folderPathEditor = new FolderPathEditor();
				return folderPathEditor;
			}
			return base.GetEditor(editorBaseType);
		}
	}
}
