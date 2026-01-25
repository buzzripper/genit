using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Design;
using System;
using System.ComponentModel;
using System.Linq;

namespace Dyvenix.GenIt
{
	/// <summary>
	/// Custom TypeDescriptionProvider for PropertyModel that controls property visibility
	/// based on the selected DataType.
	/// </summary>
	public partial class PropertyModelTypeDescriptionProvider : ElementTypeDescriptionProvider
	{
		/// <summary>
		/// Creates the type descriptor for PropertyModel instances.
		/// </summary>
		protected override ElementTypeDescriptor CreateTypeDescriptor(ICustomTypeDescriptor parent, ModelElement element)
		{
			if (element is PropertyModel propertyModel)
			{
				return new PropertyModelTypeDescriptor(parent, propertyModel);
			}
			return base.CreateTypeDescriptor(parent, element);
		}
	}

	/// <summary>
	/// Custom TypeDescriptor that filters properties based on DataType value.
	/// </summary>
	public class PropertyModelTypeDescriptor : ElementTypeDescriptor
	{
		private readonly PropertyModel _propertyModel;

		public PropertyModelTypeDescriptor(ICustomTypeDescriptor parent, PropertyModel element)
			: base(parent, element)
		{
			_propertyModel = element;
		}

		/// <summary>
		/// Filters the properties collection to show/hide properties based on DataType
		/// and makes certain properties read-only.
		/// </summary>
		public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection properties = base.GetProperties(attributes);

			var modifiedProperties = properties.Cast<PropertyDescriptor>()
				.Where(p => ShouldShowProperty(p))
				.Select(p => ModifyProperty(p))
				.ToArray();

			return new PropertyDescriptorCollection(modifiedProperties);
		}

		private bool ShouldShowProperty(PropertyDescriptor property)
		{
			// Only show Length property when DataType is String or ByteArray
			if (property.Name.Equals("Length", StringComparison.OrdinalIgnoreCase))
			{
				return DataTypeHelper.HasLength(_propertyModel.DataType);
			}

			return true;
		}

		private PropertyDescriptor ModifyProperty(PropertyDescriptor property)
		{
			// Add multiline editor to Attributes property
			if (property.Name.Equals("Attributes", StringComparison.OrdinalIgnoreCase))
			{
				return TypeDescriptorHelper.CreateMultilineStringPropertyDescriptor(property);
			}

			// Add multiline editor to Usings property
			if (property.Name.Equals("Usings", StringComparison.OrdinalIgnoreCase))
			{
				return TypeDescriptorHelper.CreateMultilineStringPropertyDescriptor(property);
			}

			// Make all properties read-only for RowVersion property (except Description)
			if (_propertyModel.IsRowVersion && !property.Name.Equals("Description", StringComparison.OrdinalIgnoreCase))
			{
				return TypeDescriptorHelper.CreateReadOnlyPropertyDescriptor(property);
			}

			// Add dropdown with dynamic data types (primitives + enums) for DataType property
			if (property.Name.Equals("DataType", StringComparison.OrdinalIgnoreCase))
			{
				return TypeDescriptorHelper.CreateDataTypePropertyDescriptor(property);
			}

			return property;
		}
	}
}
