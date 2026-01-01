using System;
using System.IO;

namespace Dyvenix.GenIt.DslPackage.CodeGen.Misc
{
	internal static class CodeGenUtils
	{
		static CodeGenUtils()
		{
			FileHeader = $"//------------------------------------------------------------------------------------------------------------{Environment.NewLine}";
			FileHeader += $"// This file was auto-generated. Any changes made to it will be lost.{Environment.NewLine}";
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

		public static string GetCSharpType(DataType dataType)
		{
			switch (dataType)
			{
				case DataType.String:
					return "string";
				case DataType.Boolean:
					return "bool";
				case DataType.Int32:
					return "int";
				case DataType.TimeSpan:
					return "string";
				case DataType.DateTime:
					return "DateTime";
				case DataType.Guid:
					return "Guid";
				case DataType.Int64:
					return "long";
				case DataType.Int16:
					return "short";
				case DataType.ByteArray:
					return "byte[]";
				case DataType.Object:
					return "object";
				case DataType.Byte:
					return "byte";
				case DataType.Char:
					return "char";
				case DataType.DateTimeOffset:
					return "DateTimeOffset";
				case DataType.Decimal:
					return "decimal";
				case DataType.Double:
					return "double";
				case DataType.Single:
					return "single";
				case DataType.StringList:
					return "List<string>";
				case DataType.UInt16:
					return "ushort";
				case DataType.UInt32:
					return "uint";
				case DataType.UInt64:
					return "ulong";
			}

			return dataType.ToString();
		}
	}
}
