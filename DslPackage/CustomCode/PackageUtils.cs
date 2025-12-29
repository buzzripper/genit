// This file is kept for backwards compatibility.
// The PackageUtils class has been moved to the Dsl project (Dyvenix.GenIt.PackageUtils)
// to allow shared access between DSL and DslPackage projects.

namespace Dyvenix.GenIt.DslPackage.CustomCode
{
	/// <summary>
	/// Wrapper class that delegates to the main PackageUtils in the Dsl project.
	/// </summary>
	internal static class PackageUtils
	{
		public static string SolutionRootPath 
		{ 
			get => Dyvenix.GenIt.PackageUtils.SolutionRootPath;
			set => Dyvenix.GenIt.PackageUtils.SolutionRootPath = value;
		}
	}
}
