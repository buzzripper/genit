using System.Windows;
using System.Windows.Controls;

namespace Dyvenix.GenIt.DslPackage.Tools.Services
{
	/// <summary>
	/// Host control for the GenItEditorWindow tool window.
	/// Contains the SvcEditControl and manages visibility based on selection.
	/// </summary>
	public partial class GenItEditorWindowControl : UserControl
	{
		public GenItEditorWindowControl()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Shows the service editor with the specified entity model.
		/// </summary>
		/// <param name="entityModel">The EntityModel to display.</param>
		/// <param name="serviceModelVersion">The version of the ServiceModel to display.</param>
		public void ShowServiceEditor(EntityModel entityModel, string serviceModelVersion)
		{
			if (entityModel != null)
			{
				svcEditControl.Initialize(entityModel, serviceModelVersion);
				svcEditControl.Visibility = Visibility.Visible;
				txtNoSelection.Visibility = Visibility.Collapsed;
			}
			else
			{
				HideServiceEditor();
			}
		}

		/// <summary>
		/// Hides the service editor and shows the "No item selected" message.
		/// </summary>
		public void HideServiceEditor()
		{
			svcEditControl.Visibility = Visibility.Collapsed;
			txtNoSelection.Visibility = Visibility.Hidden;
		}
	}
}
