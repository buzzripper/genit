
namespace Dyvenix.GenIt.DslPackage.CustomCode
{
	public static class SolutionRootCache
	{
		private static string _current;
		public static string Current => _current;

		internal static void Set(string path) => _current = path;
	}
}


