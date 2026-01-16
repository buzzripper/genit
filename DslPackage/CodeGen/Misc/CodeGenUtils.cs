using System;
using System.IO;

namespace Dyvenix.GenIt.DslPackage.CodeGen.Misc
{
	internal static class CodeGenUtils
	{
		static CodeGenUtils()
		{
			FileHeader = $"//------------------------------------------------------------------------------------------------------------{Environment.NewLine}";
			FileHeader += $"// This file was auto-generated on {DateTime.Now:g}. Any changes made to it will be lost.{Environment.NewLine}";
			FileHeader += $"//------------------------------------------------------------------------------------------------------------";
		}

		public static string SolutionRootPath { get; set; }
		public static string FileHeader { get; }

		public static string ResolveRelativePath(string path)
		{
			if (string.IsNullOrWhiteSpace(SolutionRootPath) || string.IsNullOrWhiteSpace(path))
				return path;

			if (Path.IsPathRooted(path))
				return path;

			var bp = Path.GetDirectoryName(SolutionRootPath);   // In case it's a filepath

			return Path.GetFullPath(Path.Combine(bp, path));
		}

		public static string FormatToken(string tokenTitle)
		{
			return $"${{{{{tokenTitle}}}}}";
		}

		/// <summary>
		/// Converts a DataType string to its C# type representation.
		/// For primitive types, returns the appropriate C# keyword.
		/// For enum types (non-primitives), returns the type name as-is.
		/// </summary>
		public static string GetCSharpType(string dataType)
		{
			if (string.IsNullOrEmpty(dataType))
				return "string";

			switch (dataType)
			{
				case "String":
					return "string";
				case "Boolean":
					return "bool";
				case "Int32":
					return "int";
				case "TimeSpan":
					return "TimeSpan";
				case "DateTime":
					return "DateTime";
				case "Guid":
					return "Guid";
				case "Int64":
					return "long";
				case "Int16":
					return "short";
				case "ByteArray":
					return "byte[]";
				case "Object":
					return "object";
				case "Byte":
					return "byte";
				case "SByte":
					return "sbyte";
				case "Char":
					return "char";
				case "DateTimeOffset":
					return "DateTimeOffset";
				case "Decimal":
					return "decimal";
				case "Double":
					return "double";
				case "Single":
					return "float";
				case "StringList":
					return "List<string>";
				case "UInt16":
					return "ushort";
				case "UInt32":
					return "uint";
				case "UInt64":
					return "ulong";
				default:
					// For enum types, return the type name as-is
					return dataType;
			}
		}
	}
}
