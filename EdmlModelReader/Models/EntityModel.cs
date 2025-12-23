
namespace EdmlModelReader.Models;

public class EntityModel
{
	public string? Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string Namespace { get; set; } = string.Empty;
	public bool InclRowVersion { get; set; } = false;
	public bool GenerateCode { get; set; } = true;
	public bool Auditable { get; set; } = false;

	public List<PropertyModel> Properties { get; set; } = [];

	public List<string> KeyPropertyNames { get; set; } = [];
	public IEnumerable<PropertyModel> KeyProperties => Properties.Where(p => KeyPropertyNames.Contains(p.Name));

	public override string ToString()
	{
		return Name;
	}
}
