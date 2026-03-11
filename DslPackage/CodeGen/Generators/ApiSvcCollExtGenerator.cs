using Dyvenix.GenIt.DslPackage.CodeGen.Misc;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dyvenix.GenIt.DslPackage.CodeGen.Generators
{
    internal class ApiSvcCollExtGenerator
    {
        private readonly ModelRoot _modelRoot;
        private readonly Dictionary<ModuleModel, List<EntityModel>> _modules = new Dictionary<ModuleModel, List<EntityModel>>();

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
        }

        private List<string> GenerateUsings(ModuleModel module)
        {
            var lines = new List<string>();
            lines.AddLine(0, "using Microsoft.Extensions.DependencyInjection;");
            lines.AddLine(0, "using Microsoft.Extensions.Logging;");
            lines.AddLine(0, "using Microsoft.AspNetCore.Routing;");
            lines.AddLine(0, $"using {_modelRoot.CommonNamespace}.Api.Filters;");

            var versions = _modules[module].SelectMany(e => e.ServiceModels).Select(e => e.Version).Distinct().OrderBy(v => v);
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

            foreach (var entity in _modules[module].Where(e => e.GenerateCode))
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

            foreach (var entity in _modules[module].Where(e => e.GenerateCode))
            {
                lines.AddLine(0, $"app.Map{entity.Name}Endpoints();");
            }

            return lines;
        }
    }
}
