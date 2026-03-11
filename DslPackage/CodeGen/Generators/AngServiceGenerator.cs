using Dyvenix.GenIt.DslPackage.CodeGen.Misc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Dyvenix.GenIt.DslPackage.CodeGen.Generators
{
    internal class AngServiceGenerator
    {
        private readonly ModelRoot _modelRoot;
        private readonly List<EntityModel> _entities;
        private readonly Dictionary<string, ModuleModel> _modules = new Dictionary<string, ModuleModel>();

        internal AngServiceGenerator(ModelRoot modelRoot)
        {
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
            // Bail if there's no DTOs or services for Angular
            if (!_entities.Any(e => e.ServiceModels.Any(s => s.InclAngService)))
                return;

            var modulesChecked = new List<string>();

            foreach (var entity in _entities)
            {
                if (!entity.GenerateCode || entity.ServiceModels.Count == 0)
                    continue;

                var module = _modules[entity.Module];

                if (string.IsNullOrWhiteSpace(module.NgServiceOutputFolder))
                    errors.Add($"Module '{module.Name}' does not have a Angular Output Folder assigned. Please set it in the Module properties editor.");
            }
        }

        internal void GenerateCode()
        {
            foreach (var entity in _entities)
            {
                foreach (var serviceModel in entity.ServiceModels.Where(s => s.InclAngService))
                {
                    GenerateService(_modules[entity.Module], entity, serviceModel);
                }
            }
        }

        internal void GenerateService(ModuleModel module, EntityModel entity, ServiceModel service)
        {
            var className = $"{entity.Name}Service";

            // Imports
            var imports = new List<string>();
            imports.AddLine(0, "import { HttpClient } from '@angular/common/http';");
            imports.AddLine(0, "import { Injectable, inject } from '@angular/core';");
            imports.AddLine(0, "import { Observable } from 'rxjs';");
            imports.AddLine(0, "import { environment } from 'environments/environment';");
            imports.AddLine(0, "import { ListPage } from '../common/dtos';");

            var dtoClassNames = BuildDtoClassNames(entity);
            if (!string.IsNullOrWhiteSpace(dtoClassNames))
                imports.AddLine(0, $"import {{ {dtoClassNames} }} from './dto';");

            var dtoReqNames = BuildReqClassNames(service);
            if (!string.IsNullOrWhiteSpace(dtoClassNames))
                imports.AddLine(0, $"import {{ {dtoReqNames} }} from './req';");

            // Declaration
            var declaration = new List<string>();
            declaration.AddLine(0, $"public partial class {className} : ApiClientBase, I{entity.Name}Service");

            // Constructor
            var constructor = new List<string>();
            constructor.AddLine(0, $"public {className}(HttpClient httpClient) : base(httpClient)");
            constructor.AddLine(0, "{");
            constructor.AddLine(0, "}");

            // Update methods
            var updMethodsOutput = new List<string>();
            foreach (UpdateMethodModel method in service.UpdateMethods)
                updMethodsOutput.AddLines(1, this.GenerateUpdateMethod(entity, method));

            // Delete
            var deleteMethodsOutput = new List<string>();
            if (service.InclDelete)
                deleteMethodsOutput.AddLines(1, this.GenerateDeleteMethod(entity));

            // Read methods - single
            var singleMethodsOutput = new List<string>();
            if (service.ReadMethods.Where(m => !m.IsList).Any())
            {
                foreach (ReadMethodModel singleMethod in service.ReadMethods.Where(m => !m.IsList))
                    singleMethodsOutput.AddLines(1, this.GenerateReadMethod(singleMethod));
            }

            // Read methods - list
            var listMethodsOutput = new List<string>();
            if (service.ReadMethods.Where(m => m.IsList).Any())
            {
                foreach (ReadMethodModel listMethod in service.ReadMethods.Where(m => m.IsList))
                    listMethodsOutput.AddLines(1, this.GenerateReadMethod(listMethod));
            }

            // Write the file
            var fileContent = new List<string>();

            if (entity.ModelRoot.InclHeader)
                fileContent.Add(CodeGenUtils.FileHeader);
            fileContent.AddLines(0, imports);
            fileContent.AddLine();
            fileContent.AddLine(0, @"@Injectable({ providedIn: 'root' })");
            fileContent.AddLine(0, $"export class {className} {{");
            fileContent.AddLine(1, $"private _httpClient = inject(HttpClient);");
            fileContent.AddLine(1, $"private readonly _baseUrl = `${{environment.apiBaseUrl}}/api/{module.Name.ToLower()}/v1/{entity.Name.ToLower()}`;");

            if (updMethodsOutput.Count > 0)
            {
                fileContent.AddLine();
                fileContent.AddLine(1, "// Create/Update");
                fileContent.AddLines(0, updMethodsOutput);
            }

            if (deleteMethodsOutput.Count > 0)
            {
                fileContent.AddLine();
                fileContent.AddLine(1, "// Delete");
                fileContent.AddLines(0, deleteMethodsOutput);
            }

            if (singleMethodsOutput.Count > 0)
            {
                fileContent.AddLine();
                fileContent.AddLine(1, "// Read Single");
                fileContent.AddLines(0, singleMethodsOutput);
            }

            if (listMethodsOutput.Count > 0)
            {
                fileContent.AddLine();
                fileContent.AddLine(1, "// Read List");
                fileContent.AddLines(0, listMethodsOutput);
            }

            fileContent.AddLine(0, "}");


            var outputDir = Path.Combine(PackageUtils.SolutionRootPath, module.NgServiceOutputFolder);
            Directory.CreateDirectory(outputDir);  // Ensure output dir exists
            var outputFilepath = Path.Combine(outputDir, $"{entity.Name.ToLower()}.service.ts");

            FileHelper.SaveFile(outputFilepath, fileContent.AsString());

            OutputHelper.Write($"Completed code gen for Angular service: {className}");
        }

        private List<string> GenerateUpdateMethod(EntityModel entity, UpdateMethodModel method)
        {
            var tc = 0;
            var lines = new List<string>();
            var methodName = method.Name;
            var returnType = entity.InclRowVersion ? "Uint8Array" : "void";

            lines.AddLine();
            lines.AddLine(tc, $" {methodName.ToCamelCase()}(request: {methodName}Req): Observable<{returnType}> {{");
            lines.AddLine(tc + 1, $"return this._httpClient.patch<{returnType}>(`${{this._baseUrl}}/{methodName}`, request);");
            lines.AddLine(tc, "}");

            return lines;
        }

        private List<string> GenerateDeleteMethod(EntityModel entity)
        {
            var tc = 0;
            var lines = new List<string>();
            var className = entity.Name;
            var varName = className.ToCamelCase();
            var returnType = entity.InclRowVersion ? "Uint8Array" : "void";

            lines.AddLine();
            lines.AddLine(tc, $"delete(id: string): Observable<{returnType}> {{");
            lines.AddLine(tc + 1, $"return this._httpClient.delete<{returnType}>(`${{this._baseUrl}}/Delete{className}`, {{ body: {{ id }} }});");
            lines.AddLine(tc, "}");

            return lines;
        }

        private List<string> GenerateReadMethod(ReadMethodModel method)
        {
            var tc = 0;
            var methodName = method.Name;
            var sbSigArgs = new StringBuilder();
            var sbRoute = new StringBuilder();
            var sbQry = new StringBuilder();
            string restVerb = null;
            string payload = null;

            if (method.UseRequest)
            {
                sbSigArgs.Append($"request: {method.Name}Req");
                restVerb = "post";
                payload = ", request";
            }
            else
            {
                // Required params first, in url segments, and then optional as query params
                foreach (var reqFilterProp in method.FilterProperties.Where(fp => !fp.IsOptional && !fp.IsInternal).ToList())
                {
                    // Args
                    if (sbSigArgs.Length > 0)
                        sbSigArgs.Append(", ");
                    sbSigArgs.Append($"{reqFilterProp.PropertyModel.ArgName.ToCamelCase()}: {reqFilterProp.PropertyModel.TSType}");
                    // Query
                    sbRoute.Append($"/${{{reqFilterProp.PropertyModel.ArgName.ToCamelCase()}}}");
                }

                foreach (var optFilterProp in method.FilterProperties.Where(fp => fp.IsOptional && !fp.IsInternal).ToList())
                {
                    // Args
                    if (sbSigArgs.Length > 0)
                        sbSigArgs.Append(", ");
                    sbSigArgs.Append($"{optFilterProp.PropertyModel.ArgName.ToCamelCase()}: {optFilterProp.PropertyModel.CSType}");

                    if (sbQry.Length == 0)
                        sbQry.Append("?");
                    else
                        sbQry.Append("&");
                    sbQry.Append($"{optFilterProp.PropertyModel.ArgName.ToCamelCase()}=${{{optFilterProp.PropertyModel.ArgName.ToCamelCase()}}}");
                }
                restVerb = "get";
            }

            string returnType = method.InclPaging ? $"ListPage<{method.ReturnDto.Name}>" : method.IsList ? $"{method.ReturnDto.Name}[]" : method.ReturnDto.Name;

            // Write lines
            var lines = new List<string>();
            lines.AddLine();
            lines.AddLine(tc, $"{method.Name.ToCamelCase()}({sbSigArgs}): Observable<{returnType}> {{");
            lines.AddLine(tc + 1, $"return this._httpClient.{restVerb}<{returnType}>(`${{this._baseUrl}}/{method.Name}{sbRoute}{sbQry}`{payload});");
            lines.AddLine(tc, "}");

            return lines;
        }

        private string BuildDtoClassNames(EntityModel entity)
        {
            var sb = new StringBuilder();

            foreach (var dtoName in entity.DtoModels.Select(d => d.Name))
            {
                var comma = sb.Length > 0 ? ", " : null;
                sb.Append($"{comma}{dtoName}");
            }

            return sb.ToString();
        }

        private string BuildReqClassNames(ServiceModel service)
        {
            var sb = new StringBuilder();

            foreach (var readMethod in service.ReadMethods.Where(m => m.UseRequest))
            {
                var comma = sb.Length > 0 ? ", " : null;
                sb.Append($"{comma}{readMethod.Name}Req");
            }

            foreach (var updateMethod in service.UpdateMethods)
            {
                var comma = sb.Length > 0 ? ", " : null;
                sb.Append($"{comma}{updateMethod.Name}Req");
            }

            return sb.ToString();
        }
    }
}