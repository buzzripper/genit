using System;
using System.Collections.Generic;

namespace Dyvenix.GenIt.SchemaImporter
{
	/// <summary>
	/// Maps SQL Server data types to GenIt DataType strings.
	/// See Dsl\CustomCode\DataTypes.cs for the full list of supported types.
	/// </summary>
	internal static class SqlTypeMapper
	{
		private static readonly Dictionary<string, string> _typeMap =
			new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			// String types
			{ "char", "String" },
			{ "varchar", "String" },
			{ "nchar", "String" },
			{ "nvarchar", "String" },
			{ "text", "String" },
			{ "ntext", "String" },
			{ "xml", "String" },

			// Boolean
			{ "bit", "Boolean" },

			// Integer types
			{ "tinyint", "Byte" },
			{ "smallint", "Int16" },
			{ "int", "Int32" },
			{ "bigint", "Int64" },

			// Floating-point types
			{ "real", "Single" },
			{ "float", "Double" },
			{ "decimal", "Decimal" },
			{ "numeric", "Decimal" },
			{ "money", "Decimal" },
			{ "smallmoney", "Decimal" },

			// Date/time types
			{ "date", "DateTime" },
			{ "datetime", "DateTime" },
			{ "datetime2", "DateTime" },
			{ "smalldatetime", "DateTime" },
			{ "datetimeoffset", "DateTimeOffset" },
			{ "time", "TimeSpan" },

			// GUID
			{ "uniqueidentifier", "Guid" },

			// Binary types
			{ "binary", "ByteArray" },
			{ "varbinary", "ByteArray" },
			{ "image", "ByteArray" },
			{ "timestamp", "ByteArray" },
			{ "rowversion", "ByteArray" },

			// Misc
			{ "sql_variant", "Object" },
		};

		/// <summary>
		/// Maps a SQL Server data type name to a GenIt DataType string.
		/// Returns "String" for unrecognized types.
		/// </summary>
		internal static string MapToGenItType(string sqlDataType)
		{
			if (string.IsNullOrEmpty(sqlDataType))
				return "String";

			return _typeMap.TryGetValue(sqlDataType, out var genItType) ? genItType : "String";
		}

		/// <summary>
		/// Returns the appropriate Length value for the GenIt PropertyModel.
		/// Only meaningful for string-type columns.
		/// </summary>
		internal static int GetLength(string sqlDataType, int? maxLength)
		{
			if (string.IsNullOrEmpty(sqlDataType))
				return 0;

			var mapped = MapToGenItType(sqlDataType);
			if (mapped != "String")
				return 0;

			// -1 means MAX (varchar(max), nvarchar(max))
			if (!maxLength.HasValue || maxLength.Value < 0)
				return 0;

			return maxLength.Value;
		}

		/// <summary>
		/// Determines if a SQL type represents a rowversion/timestamp column.
		/// </summary>
		internal static bool IsRowVersion(string sqlDataType)
		{
			return string.Equals(sqlDataType, "timestamp", StringComparison.OrdinalIgnoreCase)
				|| string.Equals(sqlDataType, "rowversion", StringComparison.OrdinalIgnoreCase);
		}
	}
}
