using Dyvenix.GenIt.DslPackage.CodeGen.Generators;
using Dyvenix.GenIt.DslPackage.CodeGen.Misc;
using Microsoft.VisualStudio.Shell;
using System.Collections.Generic;
using System.Linq;

namespace Dyvenix.GenIt.DslPackage.CodeGen
{
	internal class GenItModel
	{
		private readonly ModelRoot _modelRoot;
		private readonly EntityGenerator _entityGenerator;
		private readonly EnumGenerator _enumGenerator;
		private readonly DbContextGenerator _dbContextGenerator;
		private readonly ServiceGenerator _serviceGenerator;

		internal GenItModel(ModelRoot modelRoot)
		{
			var entities = modelRoot.Types.OfType<EntityModel>().ToList();
			_entityGenerator = new EntityGenerator(entities, modelRoot.EntitiesNamespace, modelRoot.EntitiesOutputFolder, modelRoot.EntitiesEnabled, modelRoot.InclHeader);

			var enums = modelRoot.Types.OfType<EnumModel>().ToList();
			_enumGenerator = new EnumGenerator(enums, modelRoot.EnumsNamespace, modelRoot.EnumsOutputFolder, modelRoot.EnumsEnabled, modelRoot.InclHeader);

			_dbContextGenerator = new DbContextGenerator(modelRoot);

			_serviceGenerator = new ServiceGenerator(modelRoot);

			_modelRoot = modelRoot;
		}

		internal bool Validate(out List<string> errors)
		{
			errors = new List<string>();

			if (string.IsNullOrEmpty(_modelRoot.Name))
				errors.Add("Model name is required.");

			ValidateModules(errors);

			if (_entityGenerator.Enabled)
				_entityGenerator.Validate(errors);
			else
				OutputHelper.Write("Entity generation is disabled; skipping entity validation.");

			if (_enumGenerator.Enabled)
				_enumGenerator.Validate(errors);
			else
				OutputHelper.Write("Enum  generation is disabled; skipping enum validation.");

			if (_modelRoot.DbContextEnabled)
				_dbContextGenerator.Validate(errors);
			else
				OutputHelper.Write("DbContext  generation is disabled; skipping DbContext validation.");

			_serviceGenerator.Validate(errors);

			return errors.Count == 0;
		}

		internal void GenerateCode()
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			if (_entityGenerator.Enabled)
				_entityGenerator.GenerateCode();

			if (_enumGenerator.Enabled)
				_enumGenerator.GenerateCode();

			if (_modelRoot.DbContextEnabled)
				_dbContextGenerator.GenerateCode();

			_serviceGenerator.GenerateCode();
		}

		private void ValidateModules(List<string> errors)
		{
			foreach (var module in _modelRoot.Types.OfType<ModuleModel>().ToList())
			{
				if (string.IsNullOrWhiteSpace(module.Namespace))
					errors.Add($"Module '{module.Name}' - Namespace is missing .");
				if (string.IsNullOrWhiteSpace(module.RootFolder))
					errors.Add($"Module '{module.Name}' - RootFolder is missing.");
			}
		}
	}
}
