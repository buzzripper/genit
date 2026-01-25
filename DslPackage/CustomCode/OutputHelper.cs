using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;

namespace Dyvenix.GenIt.DslPackage.CodeGen.Misc
{
	/// <summary>
	/// Helper class for writing messages to the Visual Studio Output window.
	/// </summary>
	internal static class OutputHelper
	{
		// Custom output pane GUID for GenIt messages
		private static readonly Guid GenItOutputPaneGuid = new Guid("E13B7B5C-4F3A-4A1E-9D5B-2C3F4E5A6B7C");
		private const string GenItPaneName = "GenIt";

		/// <summary>
		/// Writes a message to the GenIt output pane.
		/// </summary>
		public static void Write(string message)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			WriteToPane(GenItOutputPaneGuid, GenItPaneName, message, activate: false);
		}

		/// <summary>
		/// Writes a message to the GenIt output pane and activates it.
		/// </summary>
		public static void WriteAndActivate(string message)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			WriteToPane(GenItOutputPaneGuid, GenItPaneName, message, activate: true);
		}

		/// <summary>
		/// Writes an error message to the GenIt output pane and activates it.
		/// </summary>
		public static void WriteError(string message)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			WriteToPane(GenItOutputPaneGuid, GenItPaneName, $"ERROR: {message}", activate: true);
		}

		/// <summary>
		/// Writes a warning message to the GenIt output pane.
		/// </summary>
		public static void WriteWarning(string message)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			WriteToPane(GenItOutputPaneGuid, GenItPaneName, $"WARNING: {message}", activate: false);
		}

		private static void WriteToPane(Guid paneGuid, string paneName, string message, bool activate)
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			try
			{
				var outputWindow = Package.GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow;
				if (outputWindow == null)
					return;

				IVsOutputWindowPane pane;

				// Try to get the existing pane
				if (outputWindow.GetPane(ref paneGuid, out pane) != VSConstants.S_OK || pane == null)
				{
					// Create the pane if it doesn't exist
					outputWindow.CreatePane(ref paneGuid, paneName, 1, 1);
					outputWindow.GetPane(ref paneGuid, out pane);
				}

				if (pane != null)
				{
					pane.OutputStringThreadSafe(message + Environment.NewLine);

					if (activate)
						pane.Activate();
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Failed to write to output window: {ex.Message}");
			}
		}

		public static void ShowOutputToolWindow()
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			// Show the Output tool window
			var shell = Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell;
			if (shell != null)
			{
				var outputWindowGuid = new Guid(ToolWindowGuids.Outputwindow);
				IVsWindowFrame frame;
				shell.FindToolWindow((uint)__VSFINDTOOLWIN.FTW_fForceCreate, ref outputWindowGuid, out frame);
				frame?.Show();
			}
		}

		/// <summary>
		/// Clears the GenIt output pane.
		/// </summary>
		public static void Clear()
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			try
			{
				var outputWindow = Package.GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow;
				if (outputWindow == null)
					return;

				Guid paneGuid = GenItOutputPaneGuid;
				IVsOutputWindowPane pane;

				if (outputWindow.GetPane(ref paneGuid, out pane) == VSConstants.S_OK && pane != null)
				{
					pane.Clear();
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Failed to clear output window: {ex.Message}");
			}
		}
	}
}