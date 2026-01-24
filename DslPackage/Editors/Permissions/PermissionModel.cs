using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Dyvenix.GenIt.DslPackage.Editors.Permissions
{
	/// <summary>
	/// Represents a single permission definition loaded from the extdata file.
	/// </summary>
	public class Permission
	{
		public string Name { get; set; }
		public int Value { get; set; }
		public string Description { get; set; }

		public Permission()
		{
		}

		public Permission(string name, int value, string description)
		{
			Name = name;
			Value = value;
			Description = description;
		}
	}

	/// <summary>
	/// Represents the permissions data loaded from a .gmdl.extdata file.
	/// </summary>
	public class PermissionsExtData
	{
		public List<Permission> Permissions { get; set; } = new List<Permission>();

		/// <summary>
		/// Gets the extdata file path for a given gmdl file path.
		/// </summary>
		public static string GetExtDataFilePath(string gmdlFilePath)
		{
			if (string.IsNullOrEmpty(gmdlFilePath))
				return null;
			return gmdlFilePath + ".extdata";
		}

		/// <summary>
		/// Loads permissions from the extdata XML file.
		/// </summary>
		/// <param name="filePath">Path to the .gmdl.extdata file</param>
		/// <returns>PermissionsExtData instance, or null if file doesn't exist or is malformed</returns>
		/// <exception cref="PermissionsExtDataException">Thrown if file is missing or malformed</exception>
		public static PermissionsExtData Load(string filePath)
		{
			if (!File.Exists(filePath))
			{
				throw new PermissionsExtDataException($"Permissions extdata file not found: {filePath}");
			}

			try
			{
				var doc = XDocument.Load(filePath);
				var root = doc.Root;

				if (root == null || root.Name.LocalName != "ExtData")
				{
					throw new PermissionsExtDataException($"Invalid extdata file format: root element must be 'ExtData'. File: {filePath}");
				}

				var permissionsElement = root.Element("Permissions");
				if (permissionsElement == null)
				{
					throw new PermissionsExtDataException($"Invalid extdata file format: missing 'Permissions' element. File: {filePath}");
				}

				var extData = new PermissionsExtData();

				foreach (var permElement in permissionsElement.Elements("Permission"))
				{
					var name = permElement.Attribute("name")?.Value;
					var valueStr = permElement.Attribute("value")?.Value;
					var description = permElement.Attribute("description")?.Value ?? string.Empty;

					if (string.IsNullOrWhiteSpace(name))
					{
						throw new PermissionsExtDataException($"Invalid permission: missing 'name' attribute. File: {filePath}");
					}

					if (!int.TryParse(valueStr, out int value))
					{
						throw new PermissionsExtDataException($"Invalid permission '{name}': 'value' must be an integer. File: {filePath}");
					}

					extData.Permissions.Add(new Permission(name, value, description));
				}

				// Sort permissions alphabetically by name
				extData.Permissions = extData.Permissions.OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase).ToList();

				return extData;
			}
			catch (PermissionsExtDataException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new PermissionsExtDataException($"Error loading extdata file: {ex.Message}. File: {filePath}", ex);
			}
		}

		/// <summary>
		/// Creates a new empty extdata file.
		/// </summary>
		public static void CreateEmpty(string filePath)
		{
			var doc = new XDocument(
				new XDeclaration("1.0", "utf-8", null),
				new XElement("ExtData",
					new XElement("Permissions")
				)
			);
			doc.Save(filePath);
		}

		/// <summary>
		/// Saves the permissions to the extdata file.
		/// </summary>
		public void Save(string filePath)
		{
			var doc = new XDocument(
				new XDeclaration("1.0", "utf-8", null),
				new XElement("ExtData",
					new XElement("Permissions",
						Permissions.Select(p => new XElement("Permission",
							new XAttribute("name", p.Name),
							new XAttribute("value", p.Value),
							new XAttribute("description", p.Description ?? string.Empty)
						))
					)
				)
			);
			doc.Save(filePath);
		}

		/// <summary>
		/// Checks if a permission name exists in this collection.
		/// </summary>
		public bool HasPermission(string name)
		{
			return Permissions.Any(p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase));
		}

		/// <summary>
		/// Gets all permission names.
		/// </summary>
		public IEnumerable<string> GetPermissionNames()
		{
			return Permissions.Select(p => p.Name);
		}
	}

	/// <summary>
	/// Exception thrown when there's an error with the permissions extdata file.
	/// </summary>
	public class PermissionsExtDataException : Exception
	{
		public PermissionsExtDataException(string message) : base(message)
		{
		}

		public PermissionsExtDataException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
