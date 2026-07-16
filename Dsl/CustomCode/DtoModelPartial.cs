using System.Collections.Generic;
using System.Linq;

namespace Dyvenix.GenIt
{
	public partial class DtoModel
	{
		public virtual List<PropertyModel> PropertyModelsOrdered
		{
			[global::System.Diagnostics.DebuggerStepThrough]
			get
			{
				var orderedPropertyModels = new List<PropertyModel>();

				orderedPropertyModels.AddRange(this.PropertyModels.Where(p => !p.IsNullable));
				orderedPropertyModels.AddRange(this.PropertyModels.Where(p => p.IsNullable));

				return orderedPropertyModels;
			}
		}
	}
}
