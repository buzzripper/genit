using Dyvenix.GenIt.DslPackage.CustomCode;
using System;
using System.IO;

namespace Dyvenix.GenIt.DslPackage.Editors.Helpers
{
	/// <summary>
	/// Helper class for folder browser operations with solution-relative path support.
	/// </summary>
	public static class FolderBrowserHelper
	{
		/// <summary>
		/// Shows a folder browser dialog and returns the selected path, optionally converted to a relative path.
		/// </summary>
		/// <param name="currentPath">The current path value (may be relative or absolute).</param>
		/// <param name="description">Description to show in the folder browser dialog.</param>
		/// <param name="selectedPath">The resulting path (relative if under solution root, absolute otherwise).</param>
		/// <returns>True if user selected a folder, false if cancelled.</returns>
		public static bool BrowseForFolder(string currentPath, string description, out string selectedPath)
		{
			selectedPath = null;
			string solutionRoot = SolutionRootCache.Current;

			// Determine initial folder
			string initialFolder = GetAbsolutePath(currentPath, solutionRoot);

			using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
			{
				dialog.Description = description;
				dialog.ShowNewFolderButton = true;

				// Set the initial folder if it exists
				if (!string.IsNullOrEmpty(initialFolder) && Directory.Exists(initialFolder))
				{
					dialog.SelectedPath = initialFolder;
				}
				else if (!string.IsNullOrEmpty(solutionRoot) && Directory.Exists(solutionRoot))
				{
					// Fallback to solution root if initial folder doesn't exist
					dialog.SelectedPath = solutionRoot;
				}

				if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					selectedPath = TransformToRelativeIfApplicable(dialog.SelectedPath, solutionRoot);
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Converts a potentially relative path to an absolute path.
		/// </summary>
		/// <param name="path">The path to convert.</param>
		/// <param name="solutionRoot">The solution root folder.</param>
		/// <returns>The absolute path.</returns>
		public static string GetAbsolutePath(string path, string solutionRoot)
		{
			if (string.IsNullOrEmpty(path))
				return solutionRoot;

			if (Path.IsPathRooted(path))
				return path;

			// It's a relative path, combine with solution root
			if (!string.IsNullOrEmpty(solutionRoot))
				return Path.GetFullPath(Path.Combine(solutionRoot, path));

			return path;
		}

		/// <summary>
		/// Converts an absolute path to a relative path if it's under the solution root.
		/// </summary>
		/// <param name="selectedPath">The selected absolute path.</param>
		/// <param name="solutionRoot">The solution root folder.</param>
		/// <returns>A relative path if under solution root, otherwise the original path.</returns>
		public static string TransformToRelativeIfApplicable(string selectedPath, string solutionRoot)
		{
			if (string.IsNullOrEmpty(solutionRoot) || string.IsNullOrEmpty(selectedPath))
				return selectedPath;

			// Normalize both paths for comparison
			string normalizedSelected = Path.GetFullPath(selectedPath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
			string normalizedSolutionRoot = Path.GetFullPath(solutionRoot).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

			// Check if selected path equals solution root
			if (string.Equals(normalizedSelected, normalizedSolutionRoot, StringComparison.OrdinalIgnoreCase))
			{
				return ".";
			}

			// Check if selected path is under solution root (must start with solutionRoot + separator)
			string solutionRootWithSeparator = normalizedSolutionRoot + Path.DirectorySeparatorChar;
			if (normalizedSelected.StartsWith(solutionRootWithSeparator, StringComparison.OrdinalIgnoreCase))
			{
				// Get the relative part
				return normalizedSelected.Substring(solutionRootWithSeparator.Length);
			}

			// Outside solution root - use absolute path
			return selectedPath;
		}
	}
}
