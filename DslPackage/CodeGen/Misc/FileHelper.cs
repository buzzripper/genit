using Dyvenix.GenIt.DslPackage.CustomCode;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Dyvenix.GenIt.DslPackage.CodeGen.Misc
{
	internal static class FileHelper
	{
		private const string CustomContentDivLine = "// ----------  Generated - do not modify beyond this line  ----------";

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

		internal static void PreserveCustomContentAndWriteFile(List<string> lines, string filepath)
		{
			var fileContent = new List<string>();

			// See if the file exists
			if (File.Exists(filepath))
				fileContent.AddRange(GetExistingCustomLines(filepath));

			fileContent.AddLine();
			fileContent.AddLine(0, CustomContentDivLine);
			fileContent.AddLine();
			fileContent.AddLines(0, lines);

			FileHelper.SaveFile(filepath, fileContent.AsString());
		}

		private static List<string> GetExistingCustomLines(string filepath)
		{
			List<string> allFileLines = File.ReadAllLines(filepath).ToList();

			var customLines = new List<string>();

			foreach (var line in allFileLines)
			{
				if (line == CustomContentDivLine)
					break;
				customLines.Add(line);
			}

			return customLines;
		}
	}
}

