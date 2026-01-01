using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Dyvenix.GenIt.DslPackage.CustomCode
{

	public static class VsDocument
	{
		public static async Task<IVsPersistDocData> TryGetOpenDocDataAsync(string filePath)
		{
			await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

			var rdt = await VsServices.GetAsync<IVsRunningDocumentTable>(typeof(SVsRunningDocumentTable));
			if (rdt == null) return null;

			IntPtr punkDocData = IntPtr.Zero;

			try
			{
				int hr = rdt.FindAndLockDocument(
					(uint)_VSRDTFLAGS.RDT_NoLock,
					filePath,
					out _,
					out _,
					out punkDocData,
					out _);

				if (!ErrorHandler.Succeeded(hr) || punkDocData == IntPtr.Zero)
					return null;

				// Convert IUnknown -> RCW
				var obj = Marshal.GetObjectForIUnknown(punkDocData);

				// Not all docs implement IVsPersistDocData; many do.
				return obj as IVsPersistDocData;
			}
			finally
			{
				// IMPORTANT: release the IUnknown ref returned by FindAndLockDocument
				if (punkDocData != IntPtr.Zero)
					Marshal.Release(punkDocData);
			}
		}
	}
}
