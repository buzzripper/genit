using System.Linq;

namespace Dyvenix.GenIt
{
	/// <summary>
	/// Utility class for storing package-level information that can be shared across DSL components.
	/// </summary>
	public static class PackageUtils
	{
		/// <summary>
		/// Gets or sets the root path of the current solution.
		/// </summary>
		public static string SolutionRootPath { get; set; }

		public static string ToCamelCase(string input)
		{
			if (string.IsNullOrWhiteSpace(input))
				return input;

			if (input.Length == 1)
				return input.ToLower();

			var firstChar = input.Substring(0, 1).ToLower();
			return $"{firstChar}{input.Substring(1)}";
		}

		public static bool IsPrimitiveDataType(string dataType)
		{
			return DataTypeHelper.PrimitiveTypes.Contains(dataType);
		}

		public static bool IsString(string dataType)
		{
			return dataType?.ToLower() == "string";
		}
	}
}
