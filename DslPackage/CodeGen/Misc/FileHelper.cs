
namespace Dyvenix.GenIt.DslPackage.CodeGen.Misc
{
	internal static class FileHelper
	{
		internal static string GetAbsolutePath(string relativePath)
		{
			if (string.IsNullOrWhiteSpace(relativePath))
				return string.Empty;

			return System.IO.Path.GetFullPath(System.IO.Path.Combine(PackageUtils.SolutionRootPath, relativePath));
		}
	}
}
