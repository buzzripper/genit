using Dyvenix.GenIt.DslPackage.CodeGen.Misc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Dyvenix.GenIt.DslPackage.CodeGen.Generators
{
    internal class EndpointGenerator
    {
        private readonly ModelRoot _modelRoot;
        private readonly List<EntityModel> _entities;
        private readonly Dictionary<string, ModuleModel> _modules = new Dictionary<string, ModuleModel>();
        private readonly List<string> _usings = new List<string>();
        private readonly ApiClientGenerator _apiClientGenerator = new ApiClientGenerator();
        internal EndpointGenerator(ModelRoot modelRoot)
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
            _usings.AddLine(0, "Microsoft.AspNetCore.Mvc");
            _usings.AddLine(0, "Microsoft.AspNetCore.Authorization");
            _usings.AddLine(0, "Microsoft.AspNetCore.Routing");
            _usings.AddLine(0, "Microsoft.AspNetCore.Builder");
            _usings.AddLine(0, "Microsoft.AspNetCore.Http");

            _usings.AddLines(0, _modelRoot.UsingsList);
            _usings.Add(_modelRoot.EntitiesNamespace);
            _usings.Add($"{module.Namespace}.Api.Services.{serviceModel.Version}");
            _usings.AddLine(0, $"{_modelRoot.CommonNamespace}.Api.Extensions");
            _usings.AddLine(0, $"{_modelRoot.CommonNamespace}.Api.Filters");
            _usings.AddLine(0, $"{_modelRoot.CommonNamespace}.Shared.Requests");
            _usings.AddLine(0, $"{_modelRoot.CommonNamespace}.Shared.DTOs");
            _usings.AddLine(0, $"{module.Namespace}.Shared.Contracts.{serviceModel.Version}");
            _usings.AddLine(0, $"{module.Namespace}.Shared.Authorization");
            _usings.AddLine(0, module.DtoNamespace);

            foreach (var u in entity.UsingsList)
                _usings.AddIfNotExists(u);

            foreach (var u in serviceModel.ControllerUsingsList)
                _usings.AddIfNotExists(u);

            if (serviceModel.UpdateMethods.Any() || serviceModel.ReadMethods.Any(m => m.UseRequest))
                _usings.AddIfNotExists($"{module.RequestNamespace}.{serviceModel.Version}");
        }

        internal void Validate(List<string> errors)
        {
        }

        internal void GenerateCode()
        {
            foreach (var entity in _entities)
            {
                foreach (var serviceModel in entity.ServiceModels)
                {

                    GenerateEndpoints(entity, serviceModel);
                    _apiClientGenerator.GenerateCode(entity, serviceModel, _modules[entity.Module]);
                }
            }
        }

        private void GenerateEndpoints(EntityModel entity, ServiceModel serviceModel)
        {
            var module = _modules[entity.Module];
            ResetUsings(entity, serviceModel, module);
            var className = $"{entity.Name}Endpoints";
            var serviceName = $"{entity.Name}Service";
            var serviceVarName = serviceName.ToCamelCase();

            //// Attributes
            //var attrs = BuildEndpointsAttributes(serviceModel, module, serviceName);

            // Declaration
            var declaration = new List<string>();
            declaration.AddLine(0, $"public static class {className}");

            // Maps method
            var mapMethods = new List<string>();

            // Update methods
            var updMethodsOutput = new List<string>();
            if (serviceModel.InclUpdate || serviceModel.UpdateMethods.Any())
            {
                // Normal update methods
                if (serviceModel.UpdateMethods.Any())
                {
                    mapMethods.AddLine();
                    mapMethods.AddLine(0, "// Update");
                }
                foreach (UpdateMethodModel updMethod in serviceModel.UpdateMethods)
                {
                    if (updMethodsOutput.Count > 0)
                        updMethodsOutput.AddLine();

                    this.GenerateUpdateMethod(entity, updMethod, serviceVarName, updMethodsOutput, mapMethods);
                }
            }

            // Delete
            var deleteMethodsOutput = new List<string>();
            if (serviceModel.InclDelete)
            {
                // Map
                mapMethods.AddLines(0, GenerateDeleteMapMethod(entity, serviceModel));
                // Method
                this.GenerateDeleteMethod(entity, serviceModel, serviceVarName, deleteMethodsOutput);
            }

            // Read methods - single

            var singleReadMethodsOutput = new List<string>();
            foreach (ReadMethodModel singleMethod in serviceModel.ReadMethods.Where(m => m.IsSingle))
            {
                if (singleReadMethodsOutput.Count == 0)
                {
                    mapMethods.AddLine();
                    mapMethods.AddLine(0, "// Read - Single");
                }
                this.GenerateReadSingleMethod(entity, singleMethod, serviceVarName, singleReadMethodsOutput, mapMethods);
            }

            // Read methods - list

            var listMethodsOutput = new List<string>();
            foreach (ReadMethodModel listMethod in serviceModel.ReadMethods.Where(m => m.IsList))
            {
                if (listMethodsOutput.Count == 0)
                {
                    mapMethods.AddLine();
                    mapMethods.AddLine(0, "// Read - List");
                }
                this.GenerateReadListMethod(entity, listMethod, serviceVarName, listMethodsOutput, mapMethods);
            }

            // Write the file
            var fileContent = new List<string>();

            if (_modelRoot.InclHeader)
                fileContent.Add(CodeGenUtils.FileHeader);
            fileContent.AddLines(0, _usings.Select(u => $"using {u};").ToList());
            fileContent.AddLine();
            fileContent.AddLine(0, $"namespace {module.Namespace}.Endpoints.{serviceModel.Version};");
            fileContent.AddLine();
            fileContent.AddLines(0, declaration);
            fileContent.AddLine(0, "{");
            fileContent.AddLines(1, GenerateMapEndpointsMethod(mapMethods, module, entity, serviceModel));

            if (deleteMethodsOutput.Count > 0)
                fileContent.AddLines(1, deleteMethodsOutput);

            if (updMethodsOutput.Count > 0)
            {
                fileContent.AddLine();
                fileContent.AddLine(1, "#region Updates");
            }
            fileContent.AddLines(1, updMethodsOutput);
            if (updMethodsOutput.Count > 0)
            {
                fileContent.AddLine();
                fileContent.AddLine(1, "#endregion");
            }

            if (singleReadMethodsOutput.Count > 0)
            {
                fileContent.AddLine();
                fileContent.AddLine(1, "#region Read Methods - Single");
            }
            fileContent.AddLines(1, singleReadMethodsOutput);
            if (singleReadMethodsOutput.Count > 0)
            {
                fileContent.AddLine();
                fileContent.AddLine(1, "#endregion");
            }

            if (listMethodsOutput.Count > 0)
            {
                fileContent.AddLine();
                fileContent.AddLine(1, "#region Read Methods - List");
            }
            fileContent.AddLines(1, listMethodsOutput);
            if (listMethodsOutput.Count > 0)
            {
                fileContent.AddLine();
                fileContent.AddLine(1, "#endregion");
            }

            fileContent.AddLine(0, "}");

            var fileContents = fileContent.AsString();

            var outputDir = Path.Combine(module.EndpointsFolder, $"{serviceModel.Version}");
            Directory.CreateDirectory(outputDir);  // Ensure output dir exists
            var outputFilepath = Path.Combine(outputDir, $"{className}.g.cs");

            FileHelper.SaveFile(outputFilepath, fileContent.AsString());

            OutputHelper.Write($"Completed code gen for controller: {className}");
        }

        internal List<string> GenerateMapEndpointsMethod(List<string> mapMethods, ModuleModel module, EntityModel entity, ServiceModel serviceModel)
        {
            var lines = new List<string>();

            lines.AddLine(0, $"public static IEndpointRouteBuilder Map{entity.Name}Endpoints(this IEndpointRouteBuilder app)");
            lines.AddLine(0, "{");
            lines.AddLine(1, $"var group = app.MapGroup(\"api/{module.Name.ToLower()}/{serviceModel.Version}/{entity.Name.ToLower()}\")");
            lines.AddLine(2, $".WithTags(\"{entity.Name}\");");
            lines.AddLines(1, mapMethods);
            lines.AddLine();
            lines.AddLine(1, "return app;");
            lines.AddLine(0, "}");

            return lines;
        }

        internal List<string> GenerateDeleteMapMethod(EntityModel entity, ServiceModel serviceModel)
        {
            var lines = new List<string>();

            lines.AddLine();
            lines.AddLine(0, "// Delete");
            lines.AddLine();
            lines.AddLine(0, $"group.MapDelete(\"Delete{entity.Name}\", Delete{entity.Name})");
            lines.AddLine(1, $".Produces<Guid>(StatusCodes.Status200OK)");
            if (serviceModel.DeletePermissionsList.Count > 0)
                lines.AddLine(1, $".RequireAuthorization({FormatPermList(serviceModel.DeletePermissionsList)});");
            else
                lines.AppendToLast(";");

            return lines;
        }

        internal void GenerateDeleteMethod(EntityModel entity, ServiceModel serviceModel, string svcVarName, List<string> output)
        {
            output.AddLine();
            output.AddLine(0, "#region Delete");
            output.AddLine();
            output.AddLine(0, $"public static async Task<Result> Delete{entity.Name}(I{entity.Name}Service {svcVarName}, [FromBody] DeleteReq deleteReq)");
            output.AddLine(0, "{");
            output.AddLine(0 + 1, $"await {svcVarName}.Delete{entity.Name}(deleteReq.Id);");
            output.AddLine(0 + 1, $"return Result.Ok();");
            output.AddLine(0, "}");
            output.AddLine();
            output.AddLine(0, "#endregion");
        }

        // Updates

        internal void GenerateUpdateMethod(EntityModel entity, UpdateMethodModel method, string svcVarName, List<string> output, List<string> mapMethods)
        {
            var tc = 0;

            var updateProps = new List<UpdatePropertyModel>();
            foreach (var updProp in method.UpdateProperties.Where(p => !p.IsOptional))
                updateProps.Add(updProp);
            foreach (var updProp in method.UpdateProperties.Where(p => p.IsOptional))
                updateProps.Add(updProp);

            var resultType = entity.InclRowVersion ? "<byte[]>" : null;

            output.AddLine();
            output.AddLine(tc, $"public static async Task<Result{resultType}> {method.Name}(I{entity.Name}Service {svcVarName}, [FromBody] {method.Name}Req request)");
            output.AddLine(tc, "{");
            if (entity.InclRowVersion)
            {
                output.AddLine(tc + 1, $"var rowVersion = await {svcVarName}.{method.Name}(request);");
                output.AddLine(tc + 1, $"return Result{resultType}.Ok(rowVersion);");
            }
            else
            {
                output.AddLine(1, $"await {svcVarName}.{method.Name}(request);");
                output.AddLine(1, $"return Result.Ok();");
            }
            output.AddLine(tc, "}");

            mapMethods.AddLines(0, this.GenerateUpdateMapMethod(method.Name, method));
        }

        private List<string> GenerateUpdateMapMethod(string methodName, UpdateMethodModel updMethodModel)
        {
            var lines = new List<string>();

            lines.AddLine();
            if (updMethodModel.IsCreate)
                lines.AddLine(0, $"group.MapPost(\"{methodName}\", {methodName})");
            else
                lines.AddLine(0, $"group.MapPatch(\"{methodName}\", {methodName})");
            lines.AddLine(1, $".Produces<Guid>(StatusCodes.Status200OK)");
            lines.AddLine(1, $".Produces(StatusCodes.Status409Conflict)");
            if (updMethodModel.PermissionsList.Count > 0)
                lines.AddLine(1, $".RequireAuthorization({FormatPermList(updMethodModel.PermissionsList)})");
            lines.AppendToLast(";");

            return lines;
        }

        // Read Single

        internal void GenerateReadSingleMethod(EntityModel entity, ReadMethodModel method, string svcVarName, List<string> output, List<string> mapMethods)
        {
            var tc = 0;
            var returnDtoVarName = method.ReturnDto.Name.ToCamelCase();

            // Args and map path

            var sbMapUrl = new StringBuilder(method.Name);
            var sbInArgs = new StringBuilder();
            var sbOutArgs = new StringBuilder();
            if (!method.UseRequest)
            {
                method.FilterProperties.Where(fp => !fp.IsInternal).ToList().ForEach(fp =>
                {
                    // Url for map
                    sbMapUrl.Append("/{");
                    sbMapUrl.Append(fp.PropertyModel.Name.ToCamelCase());
                    sbMapUrl.Append("}");
                    // Input arguments
                    sbInArgs.Append($", {fp.PropertyModel.CSType} {fp.PropertyModel.ArgName}");
                    // Output arguments
                    if (sbOutArgs.Length > 0)
                        sbOutArgs.Append(",");
                    sbOutArgs.Append($"{fp.PropertyModel.ArgName}");
                });
            }

            // Attributes
            if (method.Attributes.Any())
                foreach (var attr in method.Attributes)
                    output.AddLine(tc, $"[{attr}]");

            output.AddLine();
            output.AddLine(tc, $"public static async Task<Result<{method.ReturnDto.Name}>> {method.Name}(I{entity.Name}Service {svcVarName}{sbInArgs})");
            output.AddLine(tc, "{");
            output.AddLine(tc + 1, $"var {returnDtoVarName} = await {svcVarName}.{method.Name}({sbOutArgs});");
            output.AddLine(tc + 1, $"return Result<{method.ReturnDto.Name}>.Ok({returnDtoVarName});");
            output.AddLine(tc, "}");

            mapMethods.AddLines(0, this.GenerateReadSingleMapMethod(entity, method.Name, sbMapUrl.ToString(), method));
        }

        private List<string> GenerateReadSingleMapMethod(EntityModel entity, string methodName, string mapUrl, ReadMethodModel readMethodModel)
        {
            var lines = new List<string>();

            lines.AddLine();
            lines.AddLine(0, $"group.MapGet(\"{mapUrl}\", {methodName})");
            lines.AddLine(1, $".Produces<Guid>(StatusCodes.Status200OK)");
            lines.AddLine(1, $".Produces(StatusCodes.Status404NotFound)");
            if (entity.InclRowVersion)
                lines.AddLine(1, $".Produces(StatusCodes.Status409Conflict)");
            if (readMethodModel.PermissionsList.Count > 0)
                lines.AddLine(1, $".RequireAuthorization({FormatPermList(readMethodModel.PermissionsList)});");
            else
                lines.AppendToLast(";");

            return lines;
        }

        // Read List

        internal void GenerateReadListMethod(EntityModel entity, ReadMethodModel method, string svcVarName, List<string> output, List<string> mapMethods)
        {
            var tc = 0;

            // Attributes
            if (method.Attributes.Any())
                foreach (var attr in method.Attributes)
                    output.AddLine(tc, $"[{attr}]");

            // In/Out args and map path
            var sbMapUrl = new StringBuilder(method.Name);
            var sbInArgs = new StringBuilder();
            var sbOutArgs = new StringBuilder();
            if (method.UseRequest)
            {
                sbInArgs.Append($", {method.Name}Req {method.Name.ToCamelCase()}Req");
                sbOutArgs.Append($"{method.Name.ToCamelCase()}Req");
            }
            else
            {
                // Required params first, in url segments, and then optional as query params
                var filterProps = method.FilterProperties.Where(fp => !fp.IsOptional && !fp.IsInternal).ToList();
                filterProps.AddRange(method.FilterProperties.Where(fp => fp.IsOptional && !fp.IsInternal));
                foreach (var fp in filterProps)
                {
                    if (!fp.IsOptional)
                    {
                        // Url for map
                        sbMapUrl.Append("/{");
                        sbMapUrl.Append(fp.PropertyModel.Name.ToCamelCase());
                        sbMapUrl.Append("}");
                        // Input arg
                        sbInArgs.Append($", [FromRoute] {fp.PropertyModel.CSType} {fp.PropertyModel.ArgName}");
                    }
                    else
                    {
                        // Input arg
                        sbInArgs.Append($", [FromQuery] {fp.PropertyModel.CSType} {fp.PropertyModel.ArgName}");
                    }
                    // Output arguments
                    if (sbOutArgs.Length > 0)
                        sbOutArgs.Append(", ");
                    sbOutArgs.Append($"{fp.PropertyModel.ArgName}");
                }
                ;
            }

            string returnType = method.InclPaging ? $"ListPage<{method.ReturnDto.Name}>" : $"IReadOnlyList<{method.ReturnDto.Name}>";

            output.AddLine();
            output.AddLine(tc, $"public static async Task<Result<{returnType}>> {method.Name}(I{entity.Name}Service {svcVarName}{sbInArgs})");
            output.AddLine(tc, "{");
            output.AddLine(tc + 1, $"var data = await {svcVarName}.{method.Name}({sbOutArgs});");
            output.AddLine(tc + 1, $"return Result<{returnType}>.Ok(data);");
            output.AddLine(tc, "}");

            mapMethods.AddLines(0, GenerateReadListMapMethod(entity, method.Name, sbMapUrl.ToString(), method));
        }

        private List<string> GenerateReadListMapMethod(EntityModel entity, string methodName, string methodUrl, ReadMethodModel readMethodModel)
        {
            var lines = new List<string>();
            var restVerb = readMethodModel.UseRequest ? "Post" : "Get";
            lines.AddLine();
            lines.AddLine(0, $"group.Map{restVerb}(\"{methodUrl}\", {methodName})");
            lines.AddLine(1, $".Produces<Guid>(StatusCodes.Status200OK)");
            if (entity.InclRowVersion)
                lines.AddLine(1, $".Produces(StatusCodes.Status409Conflict)");
            if (readMethodModel.PermissionsList.Count > 0)
                lines.AddLine(1, $".RequireAuthorization({FormatPermList(readMethodModel.PermissionsList)});");
            else
                lines.AppendToLast(";");

            return lines;
        }

        private string FormatPermList(List<string> permList)
        {
            var sb = new StringBuilder();

            sb.Append("[");

            foreach (var perm in permList)
            {
                if (sb.Length > 1)
                    sb.Append(", ");
                sb.Append(perm);
            }
            sb.Append("]");

            return sb.ToString();
        }
    }
}