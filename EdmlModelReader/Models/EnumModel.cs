
namespace EdmlModelReader.Models;

public class EnumModel
{
	public Guid Id { get; init; }
	public string Name { get; set; } = null!;
	public bool IsExternal { get; set; } = false;
	public bool IsFlags { get; set; } = false;
	public bool GenerateCode { get; set; } = true;
	public string? Namespace { get; set; }
	public List<string> Members { get; set; } = new();

	public override string ToString()
	{
		return Name;
	}
}