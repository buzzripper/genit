using Microsoft.VisualStudio.Shell;
using System;
using System.Diagnostics;

namespace Dyvenix.GenIt
{
	/// <summary>
	/// Customizations for GenItDocData.
	/// </summary>
	internal partial class GenItDocData
	{
		/// <summary>
		/// Called after the .gmdl document is loaded (opened).
		/// </summary>
		protected override void OnDocumentLoaded()
		{
			base.OnDocumentLoaded();

			Debug.WriteLine($"GenItDocData.OnDocumentLoaded: File opened - {this.FileName}");

			// Your custom logic here when the model file is opened
			// Example: Access the model root
			// var modelRoot = this.RootElement as ModelRoot;
		}

		/// <summary>
		/// Called when the .gmdl document is closing.
		/// </summary>
		protected override void OnDocumentClosing(EventArgs e)
		{
			Debug.WriteLine($"GenItDocData.OnDocumentClosing: File closing - {this.FileName}");

			// Your custom logic here when the model file is closing

			base.OnDocumentClosing(e);
		}

		/// <summary>
		/// Called after the .gmdl document has been saved.
		/// </summary>
		protected override void OnDocumentSaved(EventArgs e)
		{
			base.OnDocumentSaved(e);

			Debug.WriteLine($"GenItDocData.OnDocumentSaved: File saved - {this.FileName}");

			// Your custom logic here after the model file is saved
		}
	}
}
