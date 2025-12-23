namespace EdmlModelReader.Models;

public class PropertyModel
{
	public string? Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string Type { get; set; } = string.Empty;
	public bool Nullable { get; set; } = true;
	public int? MaxLength { get; set; }
	public bool? ValidateRequired { get; set; }
	public int? ValidateMaxLength { get; set; }

	public bool IsPrimaryKey { get; set; }
	public bool IsIndexed { get; set; }
	public bool IsIndexUnique { get; set; }
	public bool IsIndexClustered { get; set; }
	public bool IsIdentity { get; set; }

	public List<string> Attributes { get; set; } = [];

	public override string ToString()
	{
		return Name;
	}
}
