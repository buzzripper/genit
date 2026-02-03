using System;
using System.Globalization;
using System.Windows.Data;

namespace Dyvenix.GenIt.DslPackage.Editors.Converters
{
	/// <summary>
	/// Converter that returns true if UseRequest is true OR if InclPaging is true OR if InclSorting is true.
	/// Used to ensure the UseRequest checkbox shows as checked when paging or sorting is enabled.
	/// </summary>
	public class UseRequestMultiConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values == null || values.Length < 3)
				return false;

			// values[0] = UseRequest, values[1] = InclPaging, values[2] = InclSorting
			bool useRequest = values[0] is bool b0 && b0;
			bool inclPaging = values[1] is bool b1 && b1;
			bool inclSorting = values[2] is bool b2 && b2;

			// Return true if any of these are true
			return useRequest || inclPaging || inclSorting;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			// Only convert back to UseRequest (first value)
			// Return UnsetValue for the other two to indicate they shouldn't be updated
			bool isChecked = value is bool b && b;
			return new object[] { isChecked, Binding.DoNothing, Binding.DoNothing };
		}
	}
}
