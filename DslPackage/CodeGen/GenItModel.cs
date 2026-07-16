using Dyvenix.GenIt.DslPackage.CodeGen.Generators;
using Dyvenix.GenIt.DslPackage.CodeGen.Misc;
using Microsoft.VisualStudio.Shell;
using System.Collections.Generic;
using System.IO;
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
		private readonly RequestGenerator _requestGenerator;
		private readonly EndpointGenerator _endpointGenerator;
		private readonly ApiSvcCollExtGenerator _apiSvcCollExtGenerator;
		private readonly SharedSvcCollExtGenerator _sharedSvcCollExtGenerator;
		private readonly DataSetGenerator _dataSetGenerator;
		private readonly TestDataGenerator _testDataGenerator;
		private readonly DataManagerGenerator _dataManagerGenerator;
		private readonly AngServiceGenerator _angServiceGenerator;
		private readonly AngDtoGenerator _angDtoGenerator;
		private readonly AngReqGenerator _angReqGenerator;
		private readonly IntTestGenerator _intTestGenerator;
		private readonly Dictionary<string, ModuleModel> _modules = new Dictionary<string, ModuleModel>();

		internal GenItModel(ModelRoot modelRoot)
		{
			_modelRoot = modelRoot;
			_entities = modelRoot.Types.OfType<EntityModel>().ToList();
			foreach (var module in _modelRoot.Types.OfType<ModuleModel>().ToList())
			{
				if (!_modules.ContainsKey(module.Name))
					_modules.Add(module.Name, module);
			}

			_entityGenerator = new EntityGenerator(modelRoot, _modules);
			_enumGenerator = new EnumGenerator(modelRoot.Types.OfType<EnumModel>().ToList(), modelRoot.EnumsNamespace, modelRoot.EnumsOutputFolder, modelRoot.EnumsEnabled, modelRoot.InclHeader);
			_dbContextGenerator = new DbContextGenerator(modelRoot);

			_serviceGenerator = new ServiceGenerator(modelRoot, _modules);
			_dtoGenerator = new DtoGenerator(modelRoot, _modules);
			_requestGenerator = new RequestGenerator(modelRoot, _modules);
			_endpointGenerator = new EndpointGenerator(modelRoot, _modules);
			_apiSvcCollExtGenerator = new ApiSvcCollExtGenerator(modelRoot);
			_sharedSvcCollExtGenerator = new SharedSvcCollExtGenerator(modelRoot);
			_dataSetGenerator = new DataSetGenerator(modelRoot);

			_angServiceGenerator = new AngServiceGenerator(modelRoot, _modules);
			_angDtoGenerator = new AngDtoGenerator(modelRoot, _modules);
			_angReqGenerator = new AngReqGenerator(modelRoot, _modules);

			_testDataGenerator = new TestDataGenerator(modelRoot);
			_dataManagerGenerator = new DataManagerGenerator(modelRoot);
			_intTestGenerator = new IntTestGenerator(modelRoot, _modules);
		}

		internal bool Validate(out List<string> errors)
		{
			errors = new List<string>();

			// Modules don't have generators, so validate them here
			ValidateModules(errors);

			if (string.IsNullOrWhiteSpace(_modelRoot.CommonNamespace))
				errors.Add("Common Namespace is missing.");

			if (_entityGenerator.Enabled)
				_entityGenerator.Validate(errors);
			else
				OutputHelper.Write("Entity generation is disabled, skipping validation.");

			if (_enumGenerator.Enabled)
				_enumGenerator.Validate(errors);
			else
				OutputHelper.Write("Enum  generation is disabled, skipping validation.");

			if (_modelRoot.DbContextEnabled)
				_dbContextGenerator.Validate(errors);
			else
				OutputHelper.Write("DbContext  generation is disabled, skipping validation.");

			if (_modelRoot.IntTestsEnabled)
				_intTestGenerator.Validate(errors);
			else
				OutputHelper.Write("Integration test generation is disabled, skipping validation.");

			_angServiceGenerator.Validate(errors);

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
			_requestGenerator.GenerateCode();
			_endpointGenerator.GenerateCode();
			_apiSvcCollExtGenerator.GenerateCode();
			_sharedSvcCollExtGenerator.GenerateCode();

			_angServiceGenerator.GenerateCode();
			_angDtoGenerator.GenerateCode();
			_angReqGenerator.GenerateCode();

			foreach (var module in _modules.Values)
			{
				if (module.DtoGlobalUsings.Any())
					GenerateGlobalUsings(module);
			}

			if (_modelRoot.IntTestsEnabled)
			{
				_dataSetGenerator.GenerateCode();
				_testDataGenerator.GenerateCode();
				_dataManagerGenerator.GenerateCode();
				_intTestGenerator.GenerateCode();
			}
		}

		private void ValidateModules(List<string> errors)
		{
			foreach (var module in _modules.Values)
			{
				if (string.IsNullOrWhiteSpace(module.Namespace))
					errors.Add($"Module '{module.Name}' - Namespace is missing .");

				if (string.IsNullOrWhiteSpace(module.RootFolder))
					errors.Add($"Module '{module.Name}' - RootFolder is missing.");

				if (this.AnyDTOs(module))
				{
					if (string.IsNullOrWhiteSpace(module.DtoNamespace))
						errors.Add($"Module '{module.Name}' - DtoNamespace is missing.");
				}

				if (this.AnyQueries(module))
				{
					if (string.IsNullOrWhiteSpace(module.RequestNamespace))
						errors.Add($"Module '{module.Name}' - QueryNamespace is missing.");
				}
			}
		}

		private bool AnyDTOs(ModuleModel module)
		{
			return false;
		}

		private bool AnyQueries(ModuleModel module)
		{
			foreach (var entity in _entities.Where(e => e.Module == module.Name))
			{
				foreach (var service in entity.ServiceModels)
				{
					if (service.ReadMethods.Any(m => m.UseRequest))
						return true;
				}
			}
			return false;
		}

		private void GenerateGlobalUsings(ModuleModel module)
		{
			var lines = new List<string>();

			foreach (var entityName in module.DtoGlobalUsings)
				lines.Add($"global using {module.DtoNamespace};");

			foreach (var entityName in module.RequestGlobalUsings)
				lines.Add($"global using {module.RequestNamespace};");

			var filepath = Path.Combine(module.GlobalUsingsFolder, "GlobalUsings.cs");
			File.WriteAllLines(filepath, lines);
		}
	}
}
