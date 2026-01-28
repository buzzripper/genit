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
		private readonly List<EntityModel> _entities;
		private readonly EntityGenerator _entityGenerator;
		private readonly EnumGenerator _enumGenerator;
		private readonly DbContextGenerator _dbContextGenerator;
		private readonly ServiceGenerator _serviceGenerator;
		private readonly DtoGenerator _dtoGenerator;

		internal GenItModel(ModelRoot modelRoot)
		{
			_modelRoot = modelRoot;
			_entities = modelRoot.Types.OfType<EntityModel>().ToList();

			_entityGenerator = new EntityGenerator(_entities, modelRoot.EntitiesNamespace, modelRoot.EntitiesOutputFolder, modelRoot.EntitiesEnabled, modelRoot.InclHeader);
			_enumGenerator = new EnumGenerator(modelRoot.Types.OfType<EnumModel>().ToList(), modelRoot.EnumsNamespace, modelRoot.EnumsOutputFolder, modelRoot.EnumsEnabled, modelRoot.InclHeader);
			_dbContextGenerator = new DbContextGenerator(modelRoot);
			_serviceGenerator = new ServiceGenerator(modelRoot);
			_dtoGenerator = new DtoGenerator(modelRoot);
		}

		internal bool Validate(out List<string> errors)
		{
			errors = new List<string>();

			if (string.IsNullOrEmpty(_modelRoot.Name))
				errors.Add("Model name is required.");

			// Modules don't have generators, so validate them here
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
			_dtoGenerator.GenerateCode();
		}

		private void ValidateModules(List<string> errors)
		{
			foreach (var module in _modelRoot.Types.OfType<ModuleModel>().ToList())
			{
				if (string.IsNullOrWhiteSpace(module.Namespace))
					errors.Add($"Module '{module.Name}' - Namespace is missing .");

				if (string.IsNullOrWhiteSpace(module.RootFolder))
					errors.Add($"Module '{module.Name}' - RootFolder is missing.");

				if (this.AnyDTOs(module))
				{
					if (string.IsNullOrWhiteSpace(module.DtoNamespace))
						errors.Add($"Module '{module.Name}' - DtoNamespace is missing.");
					if (string.IsNullOrWhiteSpace(module.DtoOutputFolder))
						errors.Add($"Module '{module.Name}' - DtoOutputFolder is missing.");
				}

				if (this.AnyQueries(module))
				{
					if (string.IsNullOrWhiteSpace(module.QueryNamespace))
						errors.Add($"Module '{module.Name}' - QueryNamespace is missing.");
					if (string.IsNullOrWhiteSpace(module.QueryOutputFolder))
						errors.Add($"Module '{module.Name}' - QueryOutputFolder is missing.");
				}
			}
		}

		private bool AnyDTOs(ModuleModel module)
		{
			foreach (var entity in _entities.Where(e => e.Module == module.Name))
			{
				foreach (var service in entity.ServiceModels)
				{
					if (service.UpdateMethods.Any(m => m.UseDto))
						return true;
				}
			}
			return false;
		}

		private bool AnyQueries(ModuleModel module)
		{
			foreach (var entity in _entities.Where(e => e.Module == module.Name))
			{
				foreach (var service in entity.ServiceModels)
				{
					if (service.ReadMethods.Any(m => m.UseQuery))
						return true;
				}
			}
			return false;
		}
	}
}
