using System;
using System.Collections.Generic;
using System.Linq;

namespace Dyvenix.GenIt.DslPackage.Editors.Permissions
{
	/// <summary>
	/// Represents a permission with name and description for the permissions dialog.
	/// </summary>
	public class Permission
	{
		public string Name { get; set; }
		public string Description { get; set; }

		public Permission()
		{
			Name = string.Empty;
			Description = string.Empty;
		}

		public Permission(string name, string description)
		{
			Name = name ?? string.Empty;
			Description = description ?? string.Empty;
		}
	}

	/// <summary>
	/// Helper class for parsing and formatting permissions strings.
	/// </summary>
	public static class PermissionsHelper
	{
		/// <summary>
		/// Gets permissions from a ModelRoot as Permission objects.
		/// </summary>
		public static List<Permission> GetPermissions(ModelRoot modelRoot)
		{
			if (modelRoot == null)
				return new List<Permission>();

			return modelRoot.PermissionsList
				.Select(p => new Permission(p.Name, p.Description))
				.OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase)
				.ToList();
		}

		/// <summary>
		/// Parses a comma-separated permissions string into a list.
		/// </summary>
		public static List<string> ParsePermissions(string permissionsString)
		{
			if (string.IsNullOrWhiteSpace(permissionsString))
				return new List<string>();

			return permissionsString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
				.Select(p => p.Trim())
				.Where(p => !string.IsNullOrEmpty(p))
				.ToList();
		}

		/// <summary>
		/// Converts a list of permissions to a comma-separated string.
		/// </summary>
		public static string ToPermissionsString(IEnumerable<string> permissions)
		{
			if (permissions == null)
				return string.Empty;

			return string.Join(",", permissions.Where(p => !string.IsNullOrWhiteSpace(p)));
		}

		/// <summary>
		/// Gets the count of permissions in a comma-separated string.
		/// </summary>
		public static int GetPermissionCount(string permissionsString)
		{
			return ParsePermissions(permissionsString).Count;
		}
	}
}
