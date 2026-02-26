using System;
using System.Collections.Generic;
using System.Linq;

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

		public bool IsSingle => !this.InclPaging && !this.InclSorting && !this.FilterProperties.Any(fp => fp.IsPartialMatch) && !this.FilterProperties.Any(fp => !fp.PropertyModel.IsIndexUnique);

		public bool IsList => !this.IsSingle;

		//public bool IsSearch => this.FilterProperties.Any(fp => fp.IsPartialMatch);

		public List<string> InclNavPropertiesList
		{
			get
			{
				if (string.IsNullOrWhiteSpace(this.InclNavProperties))
					return new List<string>();

				return this.InclNavProperties
					.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
					.Select(line => line.Trim())
					.Where(line => !string.IsNullOrWhiteSpace(line))
					.ToList();
			}
		}

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
