namespace Dyvenix.GenIt
{
	/// <summary>
	/// Partial class for UpdatePropertyModel to add calculated properties for UI binding.
	/// </summary>
	public partial class UpdatePropertyModel
	{
		public string ArgName
		{
			get
			{
				return this.PropertyModel?.Name.ToCamelCase();
			}
		}

		/// <summary>
		/// Gets the resolved PropertyModel.
		/// If the PropertyModel link is null (legacy models), attempts to resolve by walking parent links.
		/// If parent links are unavailable (e.g. code generation path), falls back to scanning the Store.
		/// </summary>
		public PropertyModel ResolvedPropertyModel
		{
			get
			{
				return this.PropertyModel;
			}
		}
	}
}
