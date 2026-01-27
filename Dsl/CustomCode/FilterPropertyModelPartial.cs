using System.Linq;

namespace Dyvenix.GenIt
{
	/// <summary>
	/// Partial class for FilterPropertyModel to add calculated properties.
	/// </summary>
	public partial class FilterPropertyModel
	{
		/// <summary>
		/// Gets the resolved PropertyModel. If the PropertyModel link is null, resolves by name from the parent entity.
		/// </summary>
		public PropertyModel ResolvedPropertyModel
		{
			get
			{
				if (this.PropertyModel != null)
					return this.PropertyModel;

				// Navigate to parent entity: FilterPropertyModel -> ReadMethodModel -> ServiceModel -> EntityModeled
				var entity = this.ReadMethodModel?.ServiceModel?.EntityModeled;
				if (entity == null)
					return null;

				return entity.Properties.FirstOrDefault(p => p.Name == this.Name);
			}
		}
	}
}
