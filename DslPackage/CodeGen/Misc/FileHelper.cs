using Dyvenix.GenIt.DslPackage.CustomCode;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Dyvenix.GenIt.DslPackage.CodeGen.Misc
{
	internal static class FileHelper
	{
		internal static string GetAbsolutePath(string relativePath)
		{
			if (string.IsNullOrWhiteSpace(relativePath))
				return string.Empty;

			return Path.GetFullPath(Path.Combine(PackageUtils.SolutionRootPath, relativePath));
		}

		internal static void SaveFile(string filePath, string content)
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			var docData = ThreadHelper.JoinableTaskFactory.Run(() => VsDocument.TryGetOpenDocDataAsync(filePath));
			if (docData != null)
			{
				UpdateOpenDocument(docData, content);
			}
			else
			{
				File.WriteAllText(filePath, content);
			}
		}

		private static void UpdateOpenDocument(IVsPersistDocData docData, string newText)
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			if (!(docData is IVsTextLines textLines))
				return;

			textLines.GetLastLineIndex(out var lastLine, out var lastIndex);

			IntPtr pText = IntPtr.Zero;
			try
			{
				// Allocate unmanaged UTF-16 string for VS interop call
				pText = Marshal.StringToCoTaskMemUni(newText);

				textLines.ReplaceLines(
					0, 0,
					lastLine, lastIndex,
					pText,
					newText.Length,   // length in chars (UTF-16 code units)
					null);

				docData.SaveDocData(VSSAVEFLAGS.VSSAVE_SilentSave, out _, out _);
			}
			finally
			{
				if (pText != IntPtr.Zero)
					Marshal.FreeCoTaskMem(pText);
			}
		}
	}
}

