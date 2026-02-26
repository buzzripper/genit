using System;
using System.Collections.Generic;
using System.Linq;

namespace Dyvenix.GenIt
{
	/// <summary>
	/// Helper class for managing DataType values - a combination of primitive types
	/// and dynamic enum names from the model.
	/// </summary>
	public static class DataTypes
	{
		public static readonly string String = "String";
		public static readonly string Boolean = "Boolean";
		public static readonly string Byte = "Byte";
		public static readonly string SByte = "SByte";
		public static readonly string Int16 = "Int16";
		public static readonly string Int32 = "Int32";
		public static readonly string Int64 = "Int64";
		public static readonly string UInt16 = "UInt16";
		public static readonly string UInt32 = "UInt32";
		public static readonly string UInt64 = "UInt64";
		public static readonly string Single = "Single";
		public static readonly string Double = "Double";
		public static readonly string Decimal = "Decimal";
		public static readonly string Char = "Char";
		public static readonly string DateTime = "DateTime";
		public static readonly string DateTimeOffset = "DateTimeOffset";
		public static readonly string TimeSpan = "TimeSpan";
		public static readonly string Guid = "Guid";
		public static readonly string ByteArray = "ByteArray";
		public static readonly string Object = "Object";
		public static readonly string StringList = "StringList";

		/// <summary>
		/// Static list of primitive data types (never changes).
		/// </summary>
		public static readonly IReadOnlyList<string> PrimitiveTypes = new List<string>
		{
			DataTypes.String,
			DataTypes.Boolean,
			DataTypes.Byte,
			DataTypes.SByte,
			DataTypes.Int16,
			DataTypes.Int32,
			DataTypes.Int64,
			DataTypes.UInt16,
			DataTypes.UInt32,
			DataTypes.UInt64,
			DataTypes.Single,
			DataTypes.Double,
			DataTypes.Decimal,
			DataTypes.Char,
			DataTypes.DateTime,
			DataTypes.DateTimeOffset,
			DataTypes.TimeSpan,
			DataTypes.Guid,
			DataTypes.ByteArray,
			DataTypes.Object,
			DataTypes.StringList
		};

		/// <summary>
		/// Gets all available DataType values - primitives plus enum names from the model.
		/// </summary>
		public static List<string> GetAllDataTypes(Microsoft.VisualStudio.Modeling.Store store)
		{
			var result = new List<string>(PrimitiveTypes);

			if (store != null)
			{
				var enumNames = GetEnumNames(store);
				result.AddRange(enumNames);
			}

			return result;
		}

		/// <summary>
		/// Gets all enum names from the model.
		/// </summary>
		public static List<string> GetEnumNames(Microsoft.VisualStudio.Modeling.Store store)
		{
			var enumNames = new List<string>();

			if (store == null)
				return enumNames;

			try
			{
				var root = store.ElementDirectory.FindElements<ModelRoot>().FirstOrDefault();
				if (root != null)
				{
					enumNames = root.Types
						.OfType<EnumModel>()
						.Where(e => !e.IsDeleting && !e.IsDeleted)
						.Select(e => e.Name)
						.Where(name => !string.IsNullOrWhiteSpace(name))
						.OrderBy(name => name)
						.ToList();
				}
			}
			catch
			{
				// If we can't read the model, return empty list
			}

			return enumNames;
		}

		/// <summary>
		/// Determines if the given DataType string represents an enum type.
		/// </summary>
		public static bool IsEnumType(string dataType)
		{
			if (string.IsNullOrWhiteSpace(dataType))
				return false;

			return !PrimitiveTypes.Contains(dataType);
		}

		/// <summary>
		/// Determines if the given DataType string is a string-related type that has length.
		/// </summary>
		public static bool HasLength(string dataType)
		{
			return dataType == "String" || dataType == "ByteArray";
		}

		public static bool IsPrimitive(string dataType)
		{
			return DataTypes.PrimitiveTypes.Contains(dataType);
		}

		public static bool IsString(string dataType)
		{
			return dataType == DataTypes.String;
		}
	}
}
