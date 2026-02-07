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

			foreach (var u in entity.UsingsList)
				_usings.AddIfNotExists(u);

			foreach (var u in serviceModel.ServiceUsingsList)
				_usings.AddIfNotExists(u);

			if (serviceModel.ReadMethods.Any(m => m.UseRequest))
				_usings.AddIfNotExists($"{module.RequestNamespace}.v{serviceModel.Version}");

			if (serviceModel.UpdateMethods.Any(m => m.UseDto))
				_usings.AddIfNotExists($"{module.DtoNamespace}.v{serviceModel.Version}");

			if (serviceModel.ReadMethods.Any(m => m.InclPaging))
				_usings.AddIfNotExists($"{_modelRoot.CommonNamespace}.Shared.Extensions");
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
								errors.Add($"Read method {method.Name} (v{service.Version}) on entity '{entity.Name}' has filter property for '{filterProp.PropertyModel.Name}' which is set to Internal, but no value is supplied.");
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
			var serviceOutputDir = Path.Combine(PackageUtils.SolutionRootPath, module.ApiRootFolder, "Services", serviceModel.Version);
			ResetUsings(entity, serviceModel, module);

			Directory.CreateDirectory(serviceOutputDir);  // Ensure output dir exists

			// Interface contents
			var interfaceContent = new List<string>();

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
				serviceMethodGenerator.GenerateCreateMethod(entity, createMethodOutput, interfaceContent);

			// Delete
			var deleteMethodsOutput = new List<string>();
			if (serviceModel.InclDelete)
				serviceMethodGenerator.GenerateDeleteMethod(entity, deleteMethodsOutput, interfaceContent);

			// Update 
			var updMethodsOutput = new List<string>();
			if (serviceModel.InclUpdate || serviceModel.UpdateMethods.Any())
			{
				updMethodsOutput.AddLine();
				updMethodsOutput.AddLine(1, "#region Update");

				// Full update method
				if (serviceModel.InclUpdate)
					serviceMethodGenerator.GenerateFullUpdateMethod(entity, updMethodsOutput, interfaceContent);

				// Normal update methods
				foreach (var updMethod in serviceModel.UpdateMethods)
				{
					serviceMethodGenerator.GenerateUpdateMethod(entity, updMethod, updMethodsOutput, interfaceContent);
				}

				updMethodsOutput.AddLine();
				updMethodsOutput.AddLine(1, "#endregion");
			}

			// Read methods - single
			var singleMethodsOutput = new List<string>();
			foreach (var singleMethod in serviceModel.ReadMethods.Where(m => !m.IsList))
			{
				if (singleMethodsOutput.Count == 0)
				{
					singleMethodsOutput.AddLine();
					singleMethodsOutput.AddLine(0, "#region Single Methods");
				}
				serviceMethodGenerator.GenerateReadMethod(entity, singleMethod, singleMethodsOutput, interfaceContent);
			}
			if (singleMethodsOutput.Count > 0)
			{
				singleMethodsOutput.AddLine();
				singleMethodsOutput.AddLine(0, "#endregion");
			}

			// Read methods - list
			var listMethodsOutput = new List<string>();
			foreach (var listMethod in serviceModel.ReadMethods.Where(m => !m.UseRequest && m.IsList))
			{
				if (listMethodsOutput.Count == 0)
				{
					listMethodsOutput.AddLine();
					listMethodsOutput.AddLine(0, "#region List Methods");
				}
				serviceMethodGenerator.GenerateReadMethod(entity, listMethod, listMethodsOutput, interfaceContent);
			}
			if (listMethodsOutput.Count > 0)
			{
				listMethodsOutput.AddLine();
				listMethodsOutput.AddLine(0, "#endregion");
			}

			// Search methods - query
			var queryMethodsOutput = new List<string>();
			if (serviceModel.ReadMethods.Any(m => m.UseRequest))
			{
				if (queryMethodsOutput.Count == 0)
				{
					queryMethodsOutput.AddLine();
					queryMethodsOutput.AddLine(1, "#region Search Methods");
				}
				foreach (var queryMethod in serviceModel.ReadMethods.Where(m => m.UseRequest))
					serviceMethodGenerator.GenerateSearchMethod(entity, queryMethod, queryMethodsOutput, interfaceContent);
			}
			if (queryMethodsOutput.Count > 0)
			{
				queryMethodsOutput.AddLine();
				queryMethodsOutput.AddLine(1, "#endregion");
			}

			// Sorting method
			if (serviceModel.ReadMethods.Where(m => m.UseRequest && m.InclSorting).Any())
			{
				serviceMethodGenerator.GenerateSortingMethod(entity, queryMethodsOutput);
				queryMethodsOutput.AddLine();
			}

			// Write the file

			var fileContent = new List<string>();

			if (_modelRoot.InclHeader)
				fileContent.Add(CodeGenUtils.FileHeader);
			fileContent.AddLines(0, _usings.Select(u => $"using {u};").ToList());
			fileContent.AddLine();
			fileContent.AddLine(0, $"namespace {module.Namespace}.Services.v{serviceModel.Version};");
			fileContent.AddLine();
			fileContent.AddLine(0, $"public interface I{entity.Name}Service");
			fileContent.AddLine(0, "{");
			fileContent.AddLines(1, interfaceContent);
			fileContent.AddLine(0, "}");
			fileContent.AddLine();
			fileContent.AddLines(0, serviceAttrs);
			fileContent.AddLines(0, declaration);
			fileContent.AddLine(0, "{");
			fileContent.AddLines(0, fields);
			fileContent.AddLine();
			fileContent.AddLines(1, constructor);
			fileContent.AddLines(0, createMethodOutput);
			fileContent.AddLines(0, deleteMethodsOutput);
			fileContent.AddLines(0, updMethodsOutput);
			fileContent.AddLines(1, singleMethodsOutput);
			fileContent.AddLines(1, listMethodsOutput);
			fileContent.AddLines(0, queryMethodsOutput);
			fileContent.AddLine(0, "}");

			var fileContents = fileContent.AsString();

			var outputDir = Path.Combine(PackageUtils.SolutionRootPath, module.ApiRootFolder, "Services", $"v{serviceModel.Version}");
			Directory.CreateDirectory(outputDir);  // Ensure output dir exists
			var outputFilepath = Path.Combine(outputDir, $"{serviceName}.cs");

			FileHelper.SaveFile(outputFilepath, fileContent.AsString());

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