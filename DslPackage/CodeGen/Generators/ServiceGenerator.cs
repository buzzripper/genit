using Dyvenix.GenIt.DslPackage.CodeGen.Misc;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dyvenix.GenIt.DslPackage.CodeGen.Generators
{
	internal class ServiceGenerator
	{
		private readonly ModelRoot _modelRoot;
		private readonly List<EntityModel> _entities;
		private readonly Dictionary<string, ModuleModel> _modules = new Dictionary<string, ModuleModel>();
		private readonly List<string> _usings = new List<string>();

		internal ServiceGenerator(ModelRoot modelRoot)
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

		private void ResetUsings(EntityModel entity, ServiceModel serviceModel, ModuleModel module)
		{
			_usings.Clear();

			// Default usings
			_usings.Add("System");
			_usings.Add("System.Collections.Generic");
			_usings.Add("System.Linq");
			_usings.Add("System.Threading.Tasks");
			_usings.Add("Microsoft.Extensions.Logging");
			_usings.Add("Microsoft.EntityFrameworkCore");
			_usings.AddLines(0, _modelRoot.UsingsList);
			_usings.Add(_modelRoot.EntitiesNamespace);
			_usings.Add(_modelRoot.DbContextNamespace);
			_usings.Add($"{_modelRoot.CommonNamespace}.Shared.Exceptions");
			_usings.Add($"{module.Namespace}.Shared.Contracts.{serviceModel.Version}");
			_usings.Add($"{module.Namespace}.Shared.Requests.{serviceModel.Version}");

			foreach (var u in entity.UsingsList)
				_usings.AddIfNotExists(u);

			foreach (var u in serviceModel.ServiceUsingsList)
				_usings.AddIfNotExists(u);

			if (serviceModel.ReadMethods.Any(m => m.UseRequest))
				_usings.AddIfNotExists($"{module.RequestNamespace}.{serviceModel.Version}");

			if (serviceModel.ReadMethods.Any(m => m.InclPaging))
			{
				_usings.AddIfNotExists($"{_modelRoot.CommonNamespace}.Shared.Extensions");
				_usings.AddIfNotExists($"{_modelRoot.CommonNamespace}.Shared.DTOs");
				_usings.AddIfNotExists($"{_modelRoot.CommonNamespace}.Shared.Requests");
			}

			if (serviceModel.ReadMethods.Any(m => m.InclSorting))
				_usings.AddIfNotExists($"{_modelRoot.CommonNamespace}.Shared.Requests");
		}

		internal void Validate(List<string> errors)
		{
			foreach (var entity in _entities)
			{
				if (!entity.GenerateCode || entity.ServiceModels.Count == 0)
					continue;

				if (string.IsNullOrEmpty(entity.Module))
					errors.Add($"Entity '{entity.Name}' does not have a Module assigned. Please set it in the Entity properties.");

				foreach (var service in entity.ServiceModels)
				{
					foreach (var method in service.ReadMethods)
					{
						foreach (var filterProp in method.FilterProperties)
						{
							if (filterProp.IsInternal && string.IsNullOrWhiteSpace(filterProp.InternalValue))
								errors.Add($"Read method {method.Name} ({service.Version}) on entity '{entity.Name}' has filter property for '{filterProp.PropertyModel.Name}' which is set to Internal, but no value is supplied.");
						}
					}
				}
			}
		}

		internal void GenerateCode()
		{
			foreach (var entity in _entities.Where(e => e.GenerateCode))
			{
				foreach (var serviceModel in entity.ServiceModels)
				{
					GenerateService(entity, serviceModel);
				}
			}
		}

		private void GenerateService(EntityModel entity, ServiceModel serviceModel)
		{
			var serviceName = $"{entity.Name}Service";

			var module = _modules[entity.Module];
			var serviceOutputDir = Path.Combine(module.ServicesFolder, serviceModel.Version);
			ResetUsings(entity, serviceModel, module);

			Directory.CreateDirectory(serviceOutputDir);  // Ensure output dir exists

			// Interface contents
			var interfaceLines = new List<string>();

			// Attributes
			var serviceAttrs = BuildServiceAttributes(serviceModel);

			// Declaration
			var declaration = new List<string>();
			declaration.AddLine(0, $"public partial class {serviceName} : I{serviceName}");

			// Fields
			var fields = new List<string>();
			fields.AddLine(1, $"private readonly ILogger<{serviceName}> _logger;");
			fields.AddLine(1, $"private readonly {_modelRoot.DbContextName} _db;");

			// Constructor
			var constructor = new List<string>();
			constructor.AddLine(0, $"public {serviceName}({_modelRoot.DbContextName} db, ILogger<{serviceName}> logger)");
			constructor.AddLine(0, "{");
			constructor.AddLine(1, $"_db = db;");
			constructor.AddLine(1, $"_logger = logger;");
			constructor.AddLine(0, "}");

			var serviceMethodGenerator = new ServiceMethodGenerator();

			// Create
			var createMethodOutput = new List<string>();
			if (serviceModel.InclCreate)
				serviceMethodGenerator.GenerateCreateMethod(entity, createMethodOutput, interfaceLines);

			// Delete
			var deleteMethodsOutput = new List<string>();
			if (serviceModel.InclDelete)
				serviceMethodGenerator.GenerateDeleteMethod(entity, deleteMethodsOutput, interfaceLines);

			// Update 
			var updMethodsOutput = new List<string>();
			if (serviceModel.InclUpdate || serviceModel.UpdateMethods.Any())
			{
				updMethodsOutput.AddLine();
				updMethodsOutput.AddLine(1, "#region Update");

				// Full update method
				if (serviceModel.InclUpdate)
					serviceMethodGenerator.GenerateFullUpdateMethod(entity, updMethodsOutput, interfaceLines);

				// Normal update methods
				foreach (var updMethod in serviceModel.UpdateMethods)
				{
					serviceMethodGenerator.GenerateUpdateMethod(entity, updMethod, updMethodsOutput, interfaceLines);
				}

				updMethodsOutput.AddLine();
				updMethodsOutput.AddLine(1, "#endregion");
			}

			// Read methods - single
			var singleMethodsOutput = new List<string>();
			foreach (var singleMethod in serviceModel.ReadMethods.Where(m => m.IsSingle))
			{
				if (singleMethodsOutput.Count == 0)
				{
					singleMethodsOutput.AddLine();
					singleMethodsOutput.AddLine(0, "#region Read - Single");
				}
				serviceMethodGenerator.GenerateReadMethod(entity, singleMethod, singleMethodsOutput, interfaceLines);
			}
			if (singleMethodsOutput.Count > 0)
			{
				singleMethodsOutput.AddLine();
				singleMethodsOutput.AddLine(0, "#endregion");
			}

			// Read methods - list
			var listMethodsOutput = new List<string>();
			foreach (var listMethod in serviceModel.ReadMethods.Where(m => m.IsList))
			{
				if (listMethodsOutput.Count == 0)
				{
					listMethodsOutput.AddLine();
					listMethodsOutput.AddLine(0, "#region Read - List");
				}
				serviceMethodGenerator.GenerateReadMethod(entity, listMethod, listMethodsOutput, interfaceLines);
			}
			if (listMethodsOutput.Count > 0)
			{
				listMethodsOutput.AddLine();
				listMethodsOutput.AddLine(0, "#endregion");
			}

			// Sorting method
			if (serviceModel.ReadMethods.Where(m => m.UseRequest && m.InclSorting).Any())
			{
				serviceMethodGenerator.GenerateSortingMethod(entity, listMethodsOutput);
				listMethodsOutput.AddLine();
			}

			// Write the service file

			var svcFileContent = new List<string>();

			if (_modelRoot.InclHeader)
				svcFileContent.Add(CodeGenUtils.FileHeader);
			svcFileContent.AddLines(0, _usings.Select(u => $"using {u};").ToList());
			svcFileContent.AddLine();
			svcFileContent.AddLine(0, $"namespace {module.Namespace}.Api.Services.{serviceModel.Version};");
			svcFileContent.AddLine();
			svcFileContent.AddLines(0, serviceAttrs);
			svcFileContent.AddLines(0, declaration);
			svcFileContent.AddLine(0, "{");
			svcFileContent.AddLines(0, fields);
			svcFileContent.AddLine();
			svcFileContent.AddLines(1, constructor);
			svcFileContent.AddLines(0, createMethodOutput);
			svcFileContent.AddLines(0, deleteMethodsOutput);
			svcFileContent.AddLines(0, updMethodsOutput);
			svcFileContent.AddLines(1, singleMethodsOutput);
			svcFileContent.AddLines(1, listMethodsOutput);
			svcFileContent.AddLine(0, "}");

			var svcOutputDir = Path.Combine(module.ServicesFolder, $"{serviceModel.Version}");
			Directory.CreateDirectory(svcOutputDir);  // Ensure output dir exists
			var svcOutputFilepath = Path.Combine(svcOutputDir, $"{serviceName}.g.cs");
			FileHelper.SaveFile(svcOutputFilepath, svcFileContent.AsString());

			// Write the interface file

			var intFileContent = new List<string>();
			if (_modelRoot.InclHeader)
				intFileContent.Add(CodeGenUtils.FileHeader);
			intFileContent.AddLines(0, _usings.Select(u => $"using {u};").ToList());
			intFileContent.AddLine();
			intFileContent.AddLine(0, $"namespace {module.Namespace}.Shared.Contracts.{serviceModel.Version};");
			intFileContent.AddLine();
			intFileContent.AddLine(0, $"public interface I{entity.Name}Service");
			intFileContent.AddLine(0, "{");
			intFileContent.AddLines(1, interfaceLines);
			intFileContent.AddLine(0, "}");

			var intOutputDir = Path.Combine(module.RootFolder, $"{module.Name}.Shared", "Contracts", $"{serviceModel.Version}");
			Directory.CreateDirectory(intOutputDir);  // Ensure output dir exists
			var intOutputFilepath = Path.Combine(intOutputDir, $"I{serviceName}.g.cs");
			FileHelper.SaveFile(intOutputFilepath, intFileContent.AsString());

			OutputHelper.Write($"Completed code gen for service: {serviceName}");
		}

		private List<string> BuildServiceAttributes(ServiceModel serviceModel)
		{
			var attrs = new List<string>();

			foreach (var a in serviceModel.ServiceAttributesList)
				attrs.AddIfNotExists(a);

			return attrs.Select(a => $"[{a}]").ToList();
		}
	}
}