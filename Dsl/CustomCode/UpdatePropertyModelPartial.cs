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
				return PackageUtils.ToCamelCase(this.Name);
			}
		}
	}
}
