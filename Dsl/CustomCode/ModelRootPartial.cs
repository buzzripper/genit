using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Dyvenix.GenIt
{
	/// <summary>
	/// Represents a permission with Name and Description.
	/// </summary>
	public class PermissionInfo
	{
		public string Name { get; set; }
		public string Description { get; set; }

		public PermissionInfo()
		{
			Name = string.Empty;
			Description = string.Empty;
		}

		public PermissionInfo(string name, string description)
		{
			Name = name ?? string.Empty;
			Description = description ?? string.Empty;
		}
	}

	/// <summary>
	/// Partial class for ModelRoot with additional helper properties
	/// </summary>
	public partial class ModelRoot
	{
		/// <summary>
		/// Gets a list of using statement strings parsed from the DbContextUsings property.
		/// Each line in the DbContextUsings string becomes an item in the list.
		/// Empty lines are skipped.
		/// </summary>
		[Browsable(false)]
		public List<string> DbContextUsingsList
		{
			get
			{
				if (string.IsNullOrWhiteSpace(this.DbContextUsings))
					return new List<string>();

				return this.DbContextUsings
					.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
					.Select(line => line.Trim())
					.Where(line => !string.IsNullOrWhiteSpace(line))
					.ToList();
			}
		}

		/// <summary>
		/// Gets a list of PermissionInfo objects parsed from the Permissions property.
		/// Each line in the Permissions string is in format "Name|Description".
		/// </summary>
		[Browsable(false)]
		public List<PermissionInfo> PermissionsList
		{
			get
			{
				if (string.IsNullOrWhiteSpace(this.Permissions))
					return new List<PermissionInfo>();

				return this.Permissions
					.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
					.Select(line => line.Trim())
					.Where(line => !string.IsNullOrWhiteSpace(line))
					.Select(line =>
					{
						var parts = line.Split(new[] { '|' }, 2);
						var name = parts.Length > 0 ? parts[0] : string.Empty;
						var description = parts.Length > 1 ? parts[1] : string.Empty;
						return new PermissionInfo(name, description);
					})
					.ToList();
			}
		}

		/// <summary>
		/// Gets a list of PermissionInfo objects parsed from the Permissions property.
		/// Each line in the Permissions string is in format "Name|Description".
		/// </summary>
		[Browsable(false)]
		public List<string> UsingsList
		{
			get
			{
				if (string.IsNullOrWhiteSpace(this.Usings))
					return new List<string>();

				return this.Usings
					.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
					.Select(line => line.Trim())
					.Where(line => !string.IsNullOrWhiteSpace(line))
					.ToList();
			}
		}
	}
}
