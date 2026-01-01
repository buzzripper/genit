using System.ComponentModel;

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
		}
	}
}
