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
		private readonly string _entitiesNamespace;
		private readonly bool _inclHeader;

		internal ServiceGenerator(ModelRoot modelRoot)
		{
			// Convenience vars
			_modelRoot = modelRoot;
			_entities = modelRoot.Types.OfType<EntityModel>().ToList();
			_inclHeader = modelRoot.InclHeader;

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
			// Set up local vars
			var serviceName = $"{entity.Name}Service";
			var interfaceName = $"I{serviceName}";
			var module = _modules[entity.Module];
			var fileContent = new List<string>();
			var interfaceFileContent = new List<string>();
			var outputDir = Path.Combine(CodeGenUtils.SolutionRootPath, module.RootFolder, "Services", serviceModel.Version);
			Directory.CreateDirectory(outputDir);  // Ensure output dir exists

			if (_inclHeader)
				fileContent.Add(CodeGenUtils.FileHeader);

			// Usings
			fileContent.AddLines(0, BuildUsings(entity, serviceModel));

			// Namespace 		
			fileContent.AddLine();
			fileContent.AddLine(0, $"namespace {module.Namespace}.{serviceModel.Version}.Services;");

			// Attributes
			foreach (var attr in serviceModel.ServiceAttributes)
				fileContent.AddLine(0, $"[{attr}]");

			// Declaration
			fileContent.AddLine();
			fileContent.AddLine(0, $"public partial class {serviceName} : {interfaceName}");
			fileContent.AddLine(0, "{");

			fileContent.AddLine(0, "}");

			var outputFilepath = Path.Combine(outputDir, $"{entity.Name}.cs");
			FileHelper.SaveFile(outputFilepath, fileContent.AsString());

			OutputHelper.Write($"Completed code gen for entity: {entity.Name}");
		}

		private List<string> BuildUsings(EntityModel entity, ServiceModel serviceModel)
		{
			var usings = new List<string>();

			// Default usings
			usings.Add("System");
			usings.Add("System.Collections.Generic");
			usings.Add("System.Threading.Tasks");

			// Entity usings
			foreach (var u in entity.UsingsList)
				usings.AddIfNotExists(u);

			// Service model usings
			foreach (var u in serviceModel.ServiceUsingsList)
				usings.AddIfNotExists(u);

			return usings.Select(u => $"using {u};").ToList();
		}
	}
}