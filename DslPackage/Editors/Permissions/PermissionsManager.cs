using System;
using System.Collections.Generic;
using System.Linq;

namespace Dyvenix.GenIt.DslPackage.Editors.Permissions
{
	/// <summary>
	/// Manages permissions for a model, including loading, caching, and validation.
	/// </summary>
	public class PermissionsManager
	{
		private static PermissionsManager _current;
		private PermissionsExtData _extData;
		private string _extDataFilePath;

		/// <summary>
		/// Gets the current PermissionsManager instance.
		/// </summary>
		public static PermissionsManager Current
		{
			get
			{
				if (_current == null)
					_current = new PermissionsManager();
				return _current;
			}
		}

		/// <summary>
		/// Gets the loaded permissions.
		/// </summary>
		public IReadOnlyList<Permission> Permissions => _extData?.Permissions ?? new List<Permission>();

		/// <summary>
		/// Gets whether permissions have been loaded.
		/// </summary>
		public bool IsLoaded => _extData != null;

		/// <summary>
		/// Gets the current extdata file path.
		/// </summary>
		public string ExtDataFilePath => _extDataFilePath;

		/// <summary>
		/// Loads permissions from the extdata file for the given gmdl file.
		/// </summary>
		/// <param name="gmdlFilePath">Path to the .gmdl file</param>
		/// <exception cref="PermissionsExtDataException">Thrown if file is missing or malformed</exception>
		public void Load(string gmdlFilePath)
		{
			_extDataFilePath = PermissionsExtData.GetExtDataFilePath(gmdlFilePath);
			_extData = PermissionsExtData.Load(_extDataFilePath);
		}

		/// <summary>
		/// Unloads the current permissions data.
		/// </summary>
		public void Unload()
		{
			_extData = null;
			_extDataFilePath = null;
		}

		/// <summary>
		/// Creates an empty extdata file for the given gmdl file.
		/// </summary>
		public void CreateEmpty(string gmdlFilePath)
		{
			var extDataPath = PermissionsExtData.GetExtDataFilePath(gmdlFilePath);
			PermissionsExtData.CreateEmpty(extDataPath);
		}

		/// <summary>
		/// Gets all permission names.
		/// </summary>
		public IEnumerable<string> GetPermissionNames()
		{
			return _extData?.GetPermissionNames() ?? Enumerable.Empty<string>();
		}

		/// <summary>
		/// Gets a permission by name.
		/// </summary>
		public Permission GetPermission(string name)
		{
			return _extData?.Permissions.FirstOrDefault(p => 
				string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase));
		}

		/// <summary>
		/// Checks if a permission name is valid.
		/// </summary>
		public bool IsValidPermission(string name)
		{
			return _extData?.HasPermission(name) ?? false;
		}

		/// <summary>
		/// Validates a comma-separated permissions string and returns only the valid permissions.
		/// </summary>
		/// <param name="permissionsString">Comma-separated permissions string</param>
		/// <param name="removedPermissions">List of permissions that were removed because they're invalid</param>
		/// <returns>Validated comma-separated permissions string with invalid permissions removed</returns>
		public string ValidateAndCleanPermissions(string permissionsString, out List<string> removedPermissions)
		{
			removedPermissions = new List<string>();

			if (string.IsNullOrWhiteSpace(permissionsString))
				return string.Empty;

			var permissions = permissionsString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
				.Select(p => p.Trim())
				.ToList();

			var validPermissions = new List<string>();
			foreach (var perm in permissions)
			{
				if (IsValidPermission(perm))
				{
					validPermissions.Add(perm);
				}
				else
				{
					removedPermissions.Add(perm);
				}
			}

			return string.Join(",", validPermissions);
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
