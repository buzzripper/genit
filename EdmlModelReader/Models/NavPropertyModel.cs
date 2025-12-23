
namespace EdmlModelReader.Models
{
	public class NavPropertyModel
	{
		public Guid Id { get; init; }
		public string Name { get; set; } = null!;
		public List<string> Attributes { get; set; } = [];

		public EntityModel? FKEntity { get; set; }
		public Cardinality Cardinality { get; set; }
		public PropertyModel? FKProperty { get; set; }
	}
}