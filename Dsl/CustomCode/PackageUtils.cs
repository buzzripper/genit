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
	}
}
