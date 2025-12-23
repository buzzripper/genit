namespace EdmlModelReader.Models;

/// <summary>
/// Represents the parsed EDML model containing all entities and enums from the ConceptualModels section.
/// </summary>
public class EdmlModel
{
	/// <summary>
	/// Gets or sets the namespace of the schema.
	/// </summary>
	public string Namespace { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the alias of the schema.
	/// </summary>
	public string? Alias { get; set; }

	/// <summary>
	/// Gets or sets the list of entities defined in the schema.
	/// </summary>
	public List<EntityModel> Entities { get; set; } = [];

	/// <summary>
	/// Gets or sets the list of enums defined in the schema.
	/// </summary>
	public List<EnumModel> Enums { get; set; } = [];
}
