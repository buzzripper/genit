using System.ComponentModel;
using Dyvenix.GenIt.DslPackage.Editors;

namespace Dyvenix.GenIt
{
	/// <summary>
	/// Partial class for package initialization extensions.
	/// </summary>
	internal abstract partial class GenItPackageBase
	{
		/// <summary>
		/// Initializes extension components.
		/// This method is called during package initialization.
		/// </summary>
		partial void InitializeExtensions()
		{
			// Register the TypeConverter for Multiplicity enum to display formatted values in property grid
			TypeDescriptor.AddAttributes(typeof(Multiplicity),
				new TypeConverterAttribute(typeof(MultiplicityConverter)));

			// Register the GenItEditorWindow tool window
			this.AddToolWindow(typeof(GenItEditorWindow));
		}
	}
}
