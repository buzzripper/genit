using System.IO;

namespace Dyvenix.GenIt.DslPackage.CodeGen.Misc
{
	internal static class CodeGenUtils
	{
		private static OutputWindowHelper _outputWindowHelper;

		internal static OutputWindowHelper OutputWindowHelper
		{
			get
			{
				if (_outputWindowHelper == null)
					_outputWindowHelper = new OutputWindowHelper();
				return _outputWindowHelper;
			}
		}

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
