using System.Collections.Generic;
using System.Linq;

namespace Dyvenix.GenIt.DslPackage.CodeGen.Generators
{
	internal class DtoGenerator
	{
		private readonly ModelRoot _modelRoot;
		private readonly List<EntityModel> _entities;
		private readonly Dictionary<string, ModuleModel> _modules = new Dictionary<string, ModuleModel>();
		private readonly List<string> _usings = new List<string>();

		internal DtoGenerator(ModelRoot modelRoot)
		{
			// Convenience vars
			_modelRoot = modelRoot;
			_entities = modelRoot.Types.OfType<EntityModel>().ToList();

			foreach (var module in _modelRoot.Types.OfType<ModuleModel>().ToList())
			{
				if (!_modules.ContainsKey(module.Name))
					_modules.Add(module.Name, module);
			}
		}
	}
}
