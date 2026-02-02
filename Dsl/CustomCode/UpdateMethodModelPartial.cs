using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

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

		[Browsable(false)]
		public List<string> PermissionsList
		{
			get
			{
				if (string.IsNullOrWhiteSpace(this.Permissions))
					return new List<string>();

				return this.Permissions
					.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
					.Select(line => line.Trim())
					.Where(line => !string.IsNullOrWhiteSpace(line))
					.ToList();
			}
		}
	}
}
