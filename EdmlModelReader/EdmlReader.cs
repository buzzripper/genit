using EdmlModelReader.Models;
using System.Xml.Linq;

namespace EdmlModelReader;

/// <summary>
/// Reads Devart Entity Developer .edml files and produces a strongly-typed C# object model.
/// </summary>
public static class EdmlReader
{
	private static readonly XNamespace EdmxNs = "http://schemas.microsoft.com/ado/2009/11/edmx";
	private static readonly XNamespace EdmNs = "http://schemas.microsoft.com/ado/2009/11/edm";
	private static readonly XNamespace DevartNs = "http://devart.com/schemas/EntityDeveloper/1.0";
	private static readonly XNamespace CodeGenNs = "http://schemas.microsoft.com/ado/2006/04/codegeneration";

	/// <summary>
	/// Reads a Devart Entity Developer .edml file, returning a strongly-typed object model.
	/// </summary>
	/// <param name="edmlPath">The path to the .edml file.</param>
	/// <returns>An <see cref="EdmlModel"/> containing the parsed entities from the ConceptualModels section.</returns>
	/// <exception cref="ArgumentException">Thrown when the path is null or empty.</exception>
	/// <exception cref="FileNotFoundException">Thrown when the .edml file does not exist.</exception>
	/// <exception cref="InvalidOperationException">Thrown when parsing fails.</exception>
	public static EdmlModel Read(string edmlPath)
	{
		if (string.IsNullOrWhiteSpace(edmlPath))
			throw new ArgumentException("Path is required.", nameof(edmlPath));

		if (!File.Exists(edmlPath))
			throw new FileNotFoundException("EDML file not found.", edmlPath);

		XDocument xdoc = XDocument.Load(edmlPath, LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);
		return ParseEdmlModel(xdoc);
	}

	/// <summary>
	/// Reads a Devart Entity Developer .edml file from a stream.
	/// </summary>
	/// <param name="stream">A readable stream containing the .edml file content.</param>
	/// <returns>An <see cref="EdmlModel"/> containing the parsed entities from the ConceptualModels section.</returns>
	/// <exception cref="ArgumentNullException">Thrown when the stream is null.</exception>
	/// <exception cref="InvalidOperationException">Thrown when parsing fails.</exception>
	public static EdmlModel Read(Stream stream)
	{
		ArgumentNullException.ThrowIfNull(stream);

		XDocument xdoc = XDocument.Load(stream, LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);
		return ParseEdmlModel(xdoc);
	}

	private static EdmlModel ParseEdmlModel(XDocument xdoc)
	{
		var model = new EdmlModel();

		// Navigate to edmx:ConceptualModels
		var conceptualModels = xdoc.Root?
			.Element(EdmxNs + "Runtime")?
			.Element(EdmxNs + "ConceptualModels");

		if (conceptualModels is null)
			throw new InvalidOperationException("Could not find ConceptualModels section in EDML file.");

		// Parse the single Schema element (there will only ever be one)
		var schemaElement = conceptualModels.Element(EdmNs + "Schema");
		if (schemaElement is null)
			throw new InvalidOperationException("Could not find Schema element in ConceptualModels.");

		// Parse schema properties directly into EdmlModel
		model.Namespace = schemaElement.Attribute("Namespace")?.Value ?? string.Empty;
		model.Alias = schemaElement.Attribute("Alias")?.Value;

		// Parse EntityType elements
		foreach (var entityElement in schemaElement.Elements(EdmNs + "EntityType"))
		{
			var entity = ParseEntity(entityElement, model.Namespace);
			model.Entities.Add(entity);
		}

		// Parse EnumType elements
		foreach (var enumElement in schemaElement.Elements(EdmNs + "EnumType"))
		{
			var enumModel = ParseEnum(enumElement, model.Namespace);
			model.Enums.Add(enumModel);
		}

		return model;
	}

	private static EntityModel ParseEntity(XElement entityElement, string schemaNamespace)
	{
		var entity = new EntityModel
		{
			Name = entityElement.Attribute("Name")?.Value ?? string.Empty,
			Id = entityElement.Attribute(DevartNs + "Guid")?.Value
		};

		// Use ed:Namespace if present, otherwise fall back to schema namespace
		var entityNamespace = entityElement.Attribute(DevartNs + "Namespace")?.Value;
		entity.Namespace = !string.IsNullOrWhiteSpace(entityNamespace) ? entityNamespace : schemaNamespace;

		// Parse ed:InclRowVersion attribute (defaults to false)
		var inclRowVersionAttr = entityElement.Attribute(DevartNs + "InclRowVersion")?.Value;
		if (bool.TryParse(inclRowVersionAttr, out bool inclRowVersion))
			entity.InclRowVersion = inclRowVersion;

		// Parse ed:GenerateCode attribute (defaults to true)
		var generateCodeAttr = entityElement.Attribute(DevartNs + "GenerateCode")?.Value;
		if (bool.TryParse(generateCodeAttr, out bool generateCode))
			entity.GenerateCode = generateCode;

		// Parse ed:Auditable attribute (defaults to true)
		var auditableAttr = entityElement.Attribute(DevartNs + "Auditable")?.Value;
		if (bool.TryParse(auditableAttr, out bool auditable))
			entity.Auditable = auditable;

		// Parse Key element to get key property names
		var keyElement = entityElement.Element(EdmNs + "Key");
		if (keyElement is not null)
		{
			foreach (var propertyRef in keyElement.Elements(EdmNs + "PropertyRef"))
			{
				var keyName = propertyRef.Attribute("Name")?.Value;
				if (!string.IsNullOrEmpty(keyName))
					entity.KeyPropertyNames.Add(keyName);
			}
		}

		// Parse Property elements
		foreach (var propElement in entityElement.Elements(EdmNs + "Property"))
		{
			var prop = ParseProperty(propElement, entity.KeyPropertyNames);
			entity.Properties.Add(prop);
		}

		return entity;
	}

	private static PropertyModel ParseProperty(XElement propElement, List<string> keyPropertyNames)
	{
		var prop = new PropertyModel
		{
			Name = propElement.Attribute("Name")?.Value ?? string.Empty,
			Type = propElement.Attribute("Type")?.Value ?? string.Empty,
			Id = propElement.Attribute(DevartNs + "Guid")?.Value
		};

		// Determine if this is a primary key property
		prop.IsPrimaryKey = keyPropertyNames.Contains(prop.Name);

		// Parse Nullable (defaults to true if not specified)
		var nullableAttr = propElement.Attribute("Nullable")?.Value;
		prop.Nullable = nullableAttr is null || !string.Equals(nullableAttr, "false", StringComparison.OrdinalIgnoreCase);

		// Parse MaxLength
		var maxLengthAttr = propElement.Attribute("MaxLength")?.Value;
		if (int.TryParse(maxLengthAttr, out int maxLength))
			prop.MaxLength = maxLength;

		// Parse Devart ValidateRequired
		var validateRequiredAttr = propElement.Attribute(DevartNs + "ValidateRequired")?.Value;
		if (bool.TryParse(validateRequiredAttr, out bool validateRequired))
			prop.ValidateRequired = validateRequired;

		// Parse Devart ValidateMaxLength
		var validateMaxLengthAttr = propElement.Attribute(DevartNs + "ValidateMaxLength")?.Value;
		if (int.TryParse(validateMaxLengthAttr, out int validateMaxLength))
			prop.ValidateMaxLength = validateMaxLength;

		// Parse ed:IsIndexed attribute (defaults to false)
		var isIndexedAttr = propElement.Attribute(DevartNs + "IsIndexed")?.Value;
		if (bool.TryParse(isIndexedAttr, out bool isIndexed))
			prop.IsIndexed = isIndexed;

		// Parse ed:IsIndexUnique attribute (defaults to false)
		var isIndexUniqueAttr = propElement.Attribute(DevartNs + "IsIndexUnique")?.Value;
		if (bool.TryParse(isIndexUniqueAttr, out bool isIndexUnique))
			prop.IsIndexUnique = isIndexUnique;

		// Parse ed:IsIndexClustered attribute (defaults to false)
		var isIndexClusteredAttr = propElement.Attribute(DevartNs + "IsIndexClustered")?.Value;
		if (bool.TryParse(isIndexClusteredAttr, out bool isIndexClustered))
			prop.IsIndexClustered = isIndexClustered;

		// Parse ed:IsIdentity attribute (defaults to false)
		var isIdentityAttr = propElement.Attribute(DevartNs + "IsIdentity")?.Value;
		if (bool.TryParse(isIdentityAttr, out bool isIdentity))
			prop.IsIdentity = isIdentity;

		return prop;
	}

	private static EnumModel ParseEnum(XElement enumElement, string schemaNamespace)
	{
		// Parse ed:Guid attribute first
		var guidAttr = enumElement.Attribute(DevartNs + "Guid")?.Value;
		Guid enumId = Guid.TryParse(guidAttr, out Guid parsedId) ? parsedId : Guid.Empty;

		var enumModel = new EnumModel
		{
			Id = enumId,
			Name = enumElement.Attribute("Name")?.Value ?? string.Empty,
			Namespace = schemaNamespace
		};

		// Parse ed:GenerateCode attribute (defaults to true)
		var generateCodeAttr = enumElement.Attribute(DevartNs + "GenerateCode")?.Value;
		if (bool.TryParse(generateCodeAttr, out bool generateCode))
			enumModel.GenerateCode = generateCode;

		// Parse IsExternal: check if d5p1:ExternalTypeName attribute exists and is not empty
		var externalTypeNameAttr = enumElement.Attribute(CodeGenNs + "ExternalTypeName")?.Value;
		enumModel.IsExternal = !string.IsNullOrWhiteSpace(externalTypeNameAttr);

		// Parse IsFlags attribute (direct attribute on EnumType, not ed: prefixed)
		var isFlagsAttr = enumElement.Attribute("IsFlags")?.Value;
		if (bool.TryParse(isFlagsAttr, out bool isFlags))
			enumModel.IsFlags = isFlags;

		// Parse Member elements
		foreach (var memberElement in enumElement.Elements(EdmNs + "Member"))
		{
			var memberName = memberElement.Attribute("Name")?.Value;
			if (!string.IsNullOrEmpty(memberName))
				enumModel.Members.Add(memberName);
		}

		return enumModel;
	}
}
