using Dyvenix.GenIt.DslPackage.CodeGen.Misc;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dyvenix.GenIt.DslPackage.CodeGen.Generators
{
	internal class SvcCollExtGenerator
	{
		private readonly ModelRoot _modelRoot;
		private readonly Dictionary<ModuleModel, List<EntityModel>> _modules = new Dictionary<ModuleModel, List<EntityModel>>();

		internal SvcCollExtGenerator(ModelRoot modelRoot)
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
				fileContent.AddLine(0, $"namespace {module.Namespace}.Api.Config;");
				fileContent.AddLine();
				fileContent.AddLine(0, "public static partial class ServiceCollectionExt");
				fileContent.AddLine(0, "{");
				fileContent.AddLine(1, "static partial void AddGeneratedServices(IServiceCollection services)");
				fileContent.AddLine(1, "{");
				fileContent.AddLines(2, GenerateRegistrations(module));
				fileContent.AddLine(1, "}");
				fileContent.AddLine(0, "}");

				// Save to file
				var outputDir = module.ApiConfigFolder;
				Directory.CreateDirectory(outputDir);  // Ensure output dir exists
				var outputFilepath = Path.Combine(outputDir, $"ServiceCollectionExt.part.cs");
				FileHelper.SaveFile(outputFilepath, fileContent.AsString());

				OutputHelper.Write($"Completed code gen for module: {module.Name}");
			}
		}

		private List<string> GenerateUsings(ModuleModel module)
		{
			var lines = new List<string>();
			lines.AddLine(0, "using Microsoft.Extensions.DependencyInjection;");
			lines.AddLine(0, "using Microsoft.Extensions.Logging;");
			lines.AddLine(0, "using Dyvenix.App1.Common.Api.Filters;");

			var versions = _modules[module].SelectMany(e => e.ServiceModels).Select(e => e.Version).Distinct().OrderBy(v => v);
			foreach (var version in versions)
				lines.AddLine(0, $"using {BuildNamespace(module, version)};");

			return lines;
		}

		private List<string> GenerateRegistrations(ModuleModel module)
		{
			var lines = new List<string>();

			foreach (var entity in _modules[module].Where(e => e.GenerateCode))
				foreach (var service in entity.ServiceModels)
				{
					var ns = BuildNamespace(module, service.Version);
					lines.AddLine(0, $"services.AddScoped<{ns}.I{entity.Name}Service, {ns}.{entity.Name}Service>();");
					lines.AddLine(0, $"services.AddScoped<ApiExceptionFilter<{ns}.{entity.Name}Service>>();");
				}

			return lines;
		}

		private string BuildNamespace(ModuleModel module, string version)
		{
			return $"{module.Namespace}.Services.v{version}";
		}

	}
}
