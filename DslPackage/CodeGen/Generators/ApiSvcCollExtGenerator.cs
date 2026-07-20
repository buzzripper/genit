using Dyvenix.GenIt.DslPackage.CodeGen.Misc;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dyvenix.GenIt.DslPackage.CodeGen.Generators
{
	internal class ApiSvcCollExtGenerator
	{
		private readonly ModelRoot _modelRoot;
		private readonly Dictionary<ModuleModel, List<EntityModel>> _moduleEntities = new Dictionary<ModuleModel, List<EntityModel>>();

		internal ApiSvcCollExtGenerator(ModelRoot modelRoot)
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
					_moduleEntities.Add(module, entities);
			}
		}

		internal void GenerateCode()
		{
			foreach (var module in _moduleEntities.Keys)
			{
				GenerateApiServiceCollExtCode(module);
				if (_modelRoot.DbContextEnabled)
				{
					var inclAuditing = _moduleEntities[module].Any(e => e.Auditable);
					GenerateDataServicesCollExt(module, inclAuditing);
				}
			}
		}

		private void GenerateApiServiceCollExtCode(ModuleModel module)
		{
			var fileContent = new List<string>();

			if (_modelRoot.InclHeader)
				fileContent.Add(CodeGenUtils.FileHeader);

			fileContent.AddLines(0, GenerateUsings(module));

			fileContent.AddLine();
			fileContent.AddLine(0, $"namespace {module.Namespace}.Api.Extensions;");
			fileContent.AddLine();
			fileContent.AddLine(0, $"public static partial class {module.Name}ApiServiceCollExt");
			fileContent.AddLine(0, "{");
			fileContent.AddLine(1, "static partial void AddGeneratedServices(IServiceCollection services)");
			fileContent.AddLine(1, "{");
			fileContent.AddLines(2, GenerateRegistrations(module));
			fileContent.AddLine(1, "}");
			fileContent.AddLine();
			fileContent.AddLine(1, "static partial void MapGeneratedEndpoints(IEndpointRouteBuilder app)");
			fileContent.AddLine(1, "{");
			fileContent.AddLines(2, GenerateEndpointMappings(module));
			fileContent.AddLine(1, "}");
			fileContent.AddLine(0, "}");

			// Save to file
			var outputDir = module.ApiExtensionsFolder;
			Directory.CreateDirectory(outputDir);  // Ensure output dir exists
			var outputFilepath = Path.Combine(outputDir, $"{module.Name}ApiServiceCollExt.g.cs");
			FileHelper.SaveFile(outputFilepath, fileContent.AsString());

			OutputHelper.Write($"Completed code gen for module: {module.Name}");
		}

		private void GenerateDataServicesCollExt(ModuleModel module, bool inclAuditing)
		{
			var fileContent = new List<string>();

			if (_modelRoot.InclHeader)
				fileContent.Add(CodeGenUtils.FileHeader);

			fileContent.AddLine(0, $"using Microsoft.EntityFrameworkCore;");
			fileContent.AddLine(0, $"using Microsoft.Extensions.Configuration;");
			fileContent.AddLine(0, $"using Microsoft.Extensions.DependencyInjection;");
			fileContent.AddLine(0, $"using {module.Namespace}.Api.Config;");
			fileContent.AddLine(0, $"using {module.Namespace}.Api.Data;");
			//if (inclAuditing)
			//fileContent.AddLine(0, $"using {_modelRoot.CommonNamespace}.Contracts;");

			fileContent.AddLine();
			fileContent.AddLine(0, $"namespace {module.Namespace}.Api.Extensions;");
			fileContent.AddLine();
			fileContent.AddLine(0, $"public static partial class {module.Name}ApiServiceCollExt");
			fileContent.AddLine(0, "{");
			fileContent.AddLine(1, "private static DbContextOptions<App1Db> _options;");
			fileContent.AddLine();
			fileContent.AddLine(1, "public static partial void AddDataServices(IServiceCollection services, IConfiguration configuration)");
			fileContent.AddLine(1, "{");
			fileContent.AddLine(2, "var dataConfig = DataConfigBuilder.Build(configuration);");
			fileContent.AddLine(2, "services.AddSingleton(dataConfig);");
			fileContent.AddLine();
			fileContent.AddLine(2, "services.AddScoped(sp =>");
			fileContent.AddLine(2, "{");
			fileContent.AddLine(3, "if (_options == null)");
			fileContent.AddLine(3, "{");
			fileContent.AddLine(4, $"var optionsBuilder = new DbContextOptionsBuilder<{_modelRoot.DbContextName}>();");
			fileContent.AddLine(4, "optionsBuilder.UseSqlServer(dataConfig.ConnectionString);");
			if (inclAuditing)
				fileContent.AddLine(4, "optionsBuilder.AddInterceptors(sp.GetRequiredService<AuditingInterceptor>());");
			fileContent.AddLine(4, "_options = optionsBuilder.Options;");
			fileContent.AddLine(3, "}");
			fileContent.AddLine(3, "return _options;");
			fileContent.AddLine(2, "});");

			fileContent.AddLine();
			fileContent.AddLine(2, $"services.AddScoped<{_modelRoot.DbContextName}>();");
			if (inclAuditing)
				fileContent.AddLine(2, "services.AddScoped<AuditingInterceptor>();");

			fileContent.AddLine(1, "}");
			fileContent.AddLine(0, "}");

			// Save to file
			var outputDir = module.ApiExtensionsFolder;
			Directory.CreateDirectory(outputDir);  // Ensure output dir exists
			var outputFilepath = Path.Combine(outputDir, $"{module.Name}DataServiceCollExt.g.cs");
			FileHelper.SaveFile(outputFilepath, fileContent.AsString());

			OutputHelper.Write($"Completed code gen for module: {module.Name}");
		}

		private List<string> GenerateUsings(ModuleModel module)
		{
			var lines = new List<string>();
			lines.AddLine(0, "using Microsoft.Extensions.DependencyInjection;");
			lines.AddLine(0, "using Microsoft.Extensions.Logging;");
			lines.AddLine(0, "using Microsoft.AspNetCore.Routing;");
			lines.AddLine(0, $"using {_modelRoot.CommonNamespace}.Api.Filters;");

			var versions = _moduleEntities[module].SelectMany(e => e.ServiceModels).Select(e => e.Version).Distinct().OrderBy(v => v);
			foreach (var version in versions)
			{
				lines.AddLine(0, $"using s{version} = {module.Namespace}.Api.Services.{version};");
				lines.AddLine(0, $"using c{version} = {module.Namespace}.Shared.Contracts.{version};");
				lines.AddLine(0, $"using {module.Namespace}.Endpoints.{version};");
			}

			return lines;
		}

		private List<string> GenerateRegistrations(ModuleModel module)
		{
			var lines = new List<string>();

			foreach (var entity in _moduleEntities[module].Where(e => e.GenerateCode))
			{
				lines.AddLine(0, $"// {entity.Name}Service");
				foreach (var service in entity.ServiceModels.OrderBy(s => s.Version))
				{
					lines.AddLine(0, $"services.AddScoped<c{service.Version}.I{entity.Name}Service, s{service.Version}.{entity.Name}Service>();");
					lines.AddLine(0, $"services.AddScoped<ApiExceptionFilter<s{service.Version}.{entity.Name}Service>>();");
				}
			}

			return lines;
		}

		private List<string> GenerateEndpointMappings(ModuleModel module)
		{
			var lines = new List<string>();

			foreach (var entity in _moduleEntities[module].Where(e => e.GenerateCode))
			{
				lines.AddLine(0, $"app.Map{entity.Name}Endpoints();");
			}

			return lines;
		}
	}
}
