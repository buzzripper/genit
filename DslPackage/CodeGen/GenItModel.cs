using Dyvenix.GenIt.DslPackage.CodeGen.Generators;
using System.Collections.Generic;
using System.Linq;

namespace Dyvenix.GenIt.DslPackage.CodeGen
{
	internal class GenItModel
	{
		private readonly EntityGenerator _entityGenerator;

		//private readonly List<EntityModel> _entities;
		private readonly List<Association> _associations;
		//private readonly string _entitiesOutputFolder;
		//private readonly string EntitiesNamespace;
		//private readonly List<EnumModel> _enums;
		//private readonly List<EnumAssociation> _enumAssociations;

		internal GenItModel(ModelRoot modelRoot)
		{
			var store = modelRoot.Store;

			var entities = modelRoot.Types.OfType<EntityModel>().ToList();

			_entityGenerator = new EntityGenerator(entities, modelRoot.EntitiesNamespace, modelRoot.TemplatesFolder, modelRoot.EntitiesOutputFolder, modelRoot.EntitiesEnabled);



			//_enums = modelRoot.Types.OfType<EnumModel>().ToList();
			//_associations = store.ElementDirectory.FindElements<Association>().Where(a => a.Source != null && a.Target != null).ToList();
			//_enumAssociations = store.ElementDirectory.FindElements<EnumAssociation>().Where(ea => ea.Entity != null && ea.Enum != null).ToList();

			//this._entitiesOutputFolder = modelRoot.EntitiesOutputFolder;
			//this.EntitiesNamespace = modelRoot.EntitiesNamespace;
		}

		//public List<EntityModel> Entities => _entities;
		//public List<Association> Associations => _associations;
		//public List<EnumModel> Enums => _enums;
		//public List<EnumAssociation> EnumAssociations => _enumAssociations;
		//public string EntitiesOutputFolder => _entitiesOutputFolder;

		internal void GenerateCode()
		{

		}
	}
}
