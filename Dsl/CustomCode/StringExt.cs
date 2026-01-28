
namespace Dyvenix.GenIt
{
	public static class StringExt
	{
		public static string ToCamelCase(this string input)
		{
			if (string.IsNullOrWhiteSpace(input))
				return input;

			if (input.Length == 1)
				return input.ToLower();

			var firstChar = input.Substring(0, 1).ToLower();
			return $"{firstChar}{input.Substring(1)}";
		}
	}
}
