using System;

namespace Dyvenix.GenIt
{
	/// <summary>
	/// Partial class for UpdateMethodModel to add calculated properties for UI binding.
	/// </summary>
	public partial class UpdateMethodModel
	{
		/// <summary>
		/// Gets the count of permissions in the Permissions string.
		/// </summary>
		public int PermsCount
		{
			get
			{
				if (string.IsNullOrWhiteSpace(Permissions))
					return 0;
				return Permissions.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length;
			}
		}
	}
}
