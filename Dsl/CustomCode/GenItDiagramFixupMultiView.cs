using Microsoft.VisualStudio.Modeling;

namespace Dyvenix.GenIt
{
	/// <summary>
	/// Multi-view customization of the generated diagram fix-up rule.
	/// </summary>
	/// <remarks>
	/// The generated <see cref="FixUpDiagram"/> rule assumes a single diagram per model, so its shape
	/// helper (<c>GenItDiagram.ShouldAddShapeForElement</c>) always returns <c>true</c>. During load
	/// every model element is re-added inside the serialization transaction; when it commits, view
	/// fix-up would create a shape on <em>every</em> loaded view for each element - repopulating empty
	/// or subset views (for example a newly created view) with the entire model.
	///
	/// Skipping fix-up while deserializing lets each view keep exactly the shapes that were persisted
	/// for it. Interactive edits are unaffected because <see cref="Store.InSerializationTransaction"/>
	/// is only true during load/save.
	/// </remarks>
	internal sealed partial class FixUpDiagram
	{
		/// <summary>
		/// Suppresses view fix-up during serialization so persisted views are not auto-populated.
		/// </summary>
		protected override bool SkipFixup(ModelElement childElement)
		{
			if (base.SkipFixup(childElement))
				return true;

			if (childElement != null && childElement.Store != null && childElement.Store.InSerializationTransaction)
				return true;

			return false;
		}
	}
}
