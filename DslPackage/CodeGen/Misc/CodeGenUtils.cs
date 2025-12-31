using System.IO;

namespace Dyvenix.GenIt.DslPackage.CodeGen.Misc
{
	internal static class CodeGenUtils
	{
		public static string SolutionRootPath { get; set; }

		public static string ResolveRelativePath(string path)
		{
			if (string.IsNullOrWhiteSpace(SolutionRootPath) || string.IsNullOrWhiteSpace(path))
				return path;

			if (Path.IsPathRooted(path))
				return path;

			var bp = Path.GetDirectoryName(SolutionRootPath);   // In case it's a filepath

			return Path.GetFullPath(Path.Combine(bp, path));
		}
	}
}
