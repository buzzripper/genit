using Dyvenix.GenIt.DslPackage.CodeGen.Misc;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dyvenix.GenIt.DslPackage.CodeGen.Generators
{
	internal class SharedSvcCollExtGenerator
	{
		private readonly ModelRoot _modelRoot;
		private readonly Dictionary<ModuleModel, List<EntityModel>> _modules = new Dictionary<ModuleModel, List<EntityModel>>();

		internal SharedSvcCollExtGenerator(ModelRoot modelRoot)
		{
			// Convenience vars
			_modelRoot = modelRoot;

			var modules = modelRoot.Types.OfType<ModuleModel>().ToList();
			foreach (var module in modules)
			{
				var entities = modelRoot.Types
					.OfType<EntityModel>()
					.Where(e => e.GenerateCode && e.Module == module.Name && e.ServiceModels.Count > 0)
					.ToList();
				if (entities.Any())
					_modules.Add(module, entities);
			}
		}

		internal void GenerateCode()
		{
			foreach (var module in _modules.Keys)
			{
				var fileContent = new List<string>();

				if (_modelRoot.InclHeader)
					fileContent.Add(CodeGenUtils.FileHeader);

				fileContent.AddLines(0, GenerateUsings(module));

				fileContent.AddLine();
				fileContent.AddLine(0, $"namespace {module.Namespace}.Shared.Extensions;");
				fileContent.AddLine();
				fileContent.AddLine(0, $"public static partial class {module.Name}SharedServiceCollExt");
				fileContent.AddLine(0, "{");
				fileContent.AddLine(1, "static partial void AddGeneratedServices(IServiceCollection services)");
				fileContent.AddLine(1, "{");
				fileContent.AddLines(2, GenerateRegistrations(module));
				fileContent.AddLine(1, "}");
				fileContent.AddLine(0, "}");

				// Save to file
				var outputDir = Path.Combine(module.RootFolder, $"{module.Name}.Shared", "Extensions");
				var filename = $"{module.Name}SharedServiceCollExt.g.cs";
				Directory.CreateDirectory(outputDir);  // Ensure output dir exists
				var outputFilepath = Path.Combine(outputDir, filename);
				FileHelper.SaveFile(outputFilepath, fileContent.AsString());

				OutputHelper.Write($"Completed code gen for file: {module.Name}");
			}
		}

		private List<string> GenerateUsings(ModuleModel module)
		{
			var lines = new List<string>();
			lines.AddLine(0, "using Microsoft.Extensions.DependencyInjection;");
			lines.AddLine(0, $"using {module.Namespace}.Shared.ApiClients;");
			lines.AddLine(0, $"using {module.Namespace}.Shared.Contracts;");
			var versions = _modules[module].SelectMany(e => e.ServiceModels).Select(e => e.Version).Distinct().OrderBy(v => v);
			foreach (var version in versions)
			{
				lines.AddLine(0, $"using s{version} = {module.Namespace}.Shared.ApiClients.{version};");
				lines.AddLine(0, $"using c{version} = {module.Namespace}.Shared.Contracts.{version};");
			}

			return lines;
		}

		private List<string> GenerateRegistrations(ModuleModel module)
		{
			var lines = new List<string>();

			lines.AddLine(0, $"services.AddHttpClient<I{module.Name}SystemService, SystemApiClient>();");
			lines.AddLine();

			foreach (var entity in _modules[module].Where(e => e.GenerateCode))
			{
				lines.AddLine(0, $"// {entity.Name}Service");
				foreach (var service in entity.ServiceModels.OrderBy(s => s.Version))
				{
					lines.AddLine(0, $"services.AddHttpClient<c{service.Version}.I{entity.Name}Service, s{service.Version}.{entity.Name}ApiClient>();");
				}
			}

			return lines;
		}
	}
}
