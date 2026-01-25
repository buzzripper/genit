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

		internal void Validate(List<string> errors)
		{
			foreach (var entity in _entities)
			{
				if (!entity.GenerateCode || entity.ServiceModels.Count == 0)
					continue;

				if (string.IsNullOrEmpty(entity.Module))
					errors.Add($"Entity '{entity.Name}' does not have a Module assigned. Please set it in the Entity properties.");
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
			var serviceOutputDir = Path.Combine(PackageUtils.SolutionRootPath, module.RootFolder, "Services", serviceModel.Version);

			Directory.CreateDirectory(serviceOutputDir);  // Ensure output dir exists

			// Addl usings
			var usings = BuildServiceUsings(entity, serviceModel, module);

			// Interface contents
			var interfaceContent = new List<string>();

			// Attributes
			var serviceAttrs = BuildServiceAttributes(serviceModel);

			// Declaration
			var declaration = new List<string>();
			declaration.AddLine(0, $"public partial class {serviceName} : I{serviceName}");

			// Fields
			var fields = new List<string>();
			fields.AddLine(1, $"private readonly ILogger _logger;");
			fields.AddLine(1, $"private readonly {_modelRoot.Name}Db _db;");
			fields.AddLine(1, $"private readonly IDbContextFactory _dbContextFactory;");

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

			//// Read methods - single
			//var singleMethodsOutput = new List<string>();
			//foreach (var singleMethod in serviceModel.ReadMethods.Where(m => !m.UseQuery && !m.IsList))
			//{
			//	if (singleMethodsOutput.Count == 0)
			//	{
			//		singleMethodsOutput.AddLine(1, "#region Single Methods");
			//	}
			//	serviceMethodGenerator.GenerateReadMethod(entity, singleMethod, singleMethodsOutput, interfaceContent);
			//}
			//if (singleMethodsOutput.Count > 0)
			//	singleMethodsOutput.AddLine(1, "#endregion");

			//// Read methods - list
			//var listMethodsOutput = new List<string>();
			//foreach (var listMethod in serviceModel.ReadMethods.Where(m => !m.UseQuery && m.IsList))
			//{
			//	if (listMethodsOutput.Count == 0)
			//	{
			//		listMethodsOutput.AddLine(1, "#region List Methods");
			//	}
			//	serviceMethodGenerator.GenerateReadMethod(entity, listMethod, listMethodsOutput, interfaceContent);
			//}
			//if (listMethodsOutput.Count > 0)
			//{
			//	listMethodsOutput.AddLine();
			//	listMethodsOutput.AddLine(1, "#endregion");
			//}

			//// Read methods - query
			//var queryMethodsOutput = new List<string>();
			//if (serviceModel.ReadMethods.Any(m => m.UseQuery))
			//{
			//	if (queryMethodsOutput.Count == 0)
			//	{
			//		queryMethodsOutput.AddLine(1, "#region Query Methods");
			//	}
			//	foreach (var queryMethod in serviceModel.ReadMethods.Where(m => m.UseQuery))
			//		serviceMethodGenerator.GenerateQueryMethod(entity, queryMethod, queryMethodsOutput, interfaceContent);
			//}
			//if (queryMethodsOutput.Count > 0)
			//	queryMethodsOutput.AddLine(1, "#endregion");

			//// Sorting method
			//if (serviceModel.ReadMethods.Where(m => m.UseQuery && m.InclSorting).Any())
			//{
			//	serviceMethodGenerator.GenerateSortingMethod(entity, queryMethodsOutput);
			//	queryMethodsOutput.AddLine();
			//}

			// Write the file

			var fileContent = new List<string>();

			fileContent.AddLines(0, usings);
			fileContent.AddLine();
			fileContent.AddLine(0, $"namespace {module.Namespace}.{serviceModel.Version}.Services;");
			fileContent.AddLine();
			fileContent.AddLine(0, $"public interface I{entity.Name}Service");
			fileContent.AddLine(0, "{");
			fileContent.AddLines(1, interfaceContent);
			fileContent.AddLine(0, "}");
			fileContent.AddLine();
			fileContent.AddLines(0, serviceAttrs);
			fileContent.AddLines(0, declaration);
			fileContent.AddLine(0, "{");
			fileContent.AddLines(1, fields);
			fileContent.AddLines(1, createMethodOutput);
			fileContent.AddLines(1, deleteMethodsOutput);
			fileContent.AddLines(1, updMethodsOutput);
			//fileContent.AddLines(1, singleMethodsOutput);
			//fileContent.AddLines(1, listMethodsOutput);
			//fileContent.AddLines(1, queryMethodsOutput);
			fileContent.AddLine(1, "}");
			fileContent.AddLine(0, "}");

			var fileContents = fileContent.AsString();

			var outputDir = Path.Combine(PackageUtils.SolutionRootPath, module.RootFolder, "Services", serviceModel.Version);
			Directory.CreateDirectory(outputDir);  // Ensure output dir exists
			var outputFilepath = Path.Combine(outputDir, $"{serviceName}.cs");

			FileHelper.SaveFile(outputFilepath, fileContent.AsString());

			OutputHelper.Write($"Completed code gen for entity: {entity.Name}");
		}

		private List<string> BuildServiceUsings(EntityModel entity, ServiceModel serviceModel, ModuleModel module)
		{
			var usings = new List<string>();

			// Default usings
			usings.Add("System");
			usings.Add("System.Collections.Generic");
			usings.Add("System.Linq");
			usings.Add("System.Threading.Tasks");
			usings.Add("Microsoft.Extensions.Logging");

			Microsoft.EntityFrameworkCore

			usings.AddIfNotExists(_modelRoot.EntitiesNamespace);
			usings.AddIfNotExists(_modelRoot.DbContextNamespace);

			// Entity usings
			foreach (var u in entity.UsingsList)
				usings.AddIfNotExists(u);

			// Service model usings
			foreach (var u in serviceModel.ServiceUsingsList)
				usings.AddIfNotExists(u);

			// Queries namespace if used
			if (serviceModel.ReadMethods.Any(m => m.UseQuery))
				usings.AddIfNotExists($"{module.Namespace}.Shared.Queries.{serviceModel.Version}");

			// DTOs namespace if used
			if (serviceModel.UpdateMethods.Any(m => m.UseDto))
				usings.AddIfNotExists($"{module.Namespace}.Shared.DTOs.{serviceModel.Version}");

			return usings.Select(u => $"using {u};").ToList();
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