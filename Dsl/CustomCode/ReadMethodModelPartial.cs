using System;

namespace Dyvenix.GenIt
{
	/// <summary>
	/// Partial class for ReadMethodModel to add calculated properties for UI binding.
	/// </summary>
	public partial class ReadMethodModel
	{
		/// <summary>
		/// Gets the count of attributes (lines in the Attributes string).
		/// </summary>
		public int AttrCount
		{
			get
			{
				if (string.IsNullOrWhiteSpace(Attributes))
					return 0;
				return Attributes.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length;
			}
		}

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
