using System;
using System.Collections.Generic;
using System.Linq;

namespace Dyvenix.GenIt.DslPackage.CodeGen
{
	internal class GenItModel
	{
		internal GenItModel(ModelRoot modelRoot)
		{
			Entities = modelRoot.Types.OfType<EntityModel>().ToList();
			Enums = modelRoot.Types.OfType<EnumModel>().ToList();

			// Get associations and enum associations from the Store
			var store = modelRoot.Store;

			this.Associations = store.ElementDirectory
				.FindElements<Association>()
				.Where(a => a.Source != null && a.Target != null)
				.ToList();
			this.EnumAssociations = store.ElementDirectory
				.FindElements<EnumAssociation>()
				.Where(ea => ea.Entity != null && ea.Enum != null)
				.ToList();

			if (Entities.Count == 0)
				throw new Exception("No entities found in the model. Add some entities first.");

			this.EntitiesOutputFolder = modelRoot.EntitiesOutputFolder;
			this.EntitiesNamespace = modelRoot.EntitiesNamespace;
		}

		internal List<EntityModel> Entities { get; private set; }
		internal List<Association> Associations { get; private set; }
		internal string EntitiesOutputFolder { get; private set; }
		internal string EntitiesNamespace { get; private set; }

		internal List<EnumModel> Enums { get; private set; }
		internal List<EnumAssociation> EnumAssociations { get; private set; }


	}
}
