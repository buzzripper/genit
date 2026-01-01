using Dyvenix.GenIt.DslPackage.CodeGen.Generators;
using Dyvenix.GenIt.DslPackage.CodeGen.Misc;
using Microsoft.VisualStudio.Shell;
using System.Collections.Generic;
using System.Linq;

namespace Dyvenix.GenIt.DslPackage.CodeGen
{
	internal class GenItModel
	{
		private readonly EntityGenerator _entityGenerator;
		private readonly EnumGenerator _enumGenerator;

		internal GenItModel(ModelRoot modelRoot)
		{
			var entities = modelRoot.Types.OfType<EntityModel>().ToList();
			_entityGenerator = new EntityGenerator(entities, modelRoot.EntitiesNamespace, modelRoot.EntitiesOutputFolder, modelRoot.EntitiesEnabled, modelRoot.InclHeader);

			var enums = modelRoot.Types.OfType<EnumModel>().ToList();
			_enumGenerator = new EnumGenerator(enums, modelRoot.EnumsNamespace, modelRoot.EnumsOutputFolder, modelRoot.EnumsEnabled, modelRoot.InclHeader);

		}

		internal bool Validate(out List<string> errors)
		{
			errors = new List<string>();

			_entityGenerator.Validate(errors);
			_enumGenerator.Validate(errors);

			return errors.Count == 0;
		}

		internal void GenerateCode()
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			if (_entityGenerator.Enabled)
				_entityGenerator.GenerateCode();
			else
				OutputHelper.Write("Entity generation is disabled; skipping entity validation.");

			if (_enumGenerator.Enabled)
				_enumGenerator.GenerateCode();
			else
				OutputHelper.Write("Enum  generation is disabled; skipping enum validation.");
		}
	}
}
