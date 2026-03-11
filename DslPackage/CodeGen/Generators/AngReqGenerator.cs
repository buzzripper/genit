using Dyvenix.GenIt.DslPackage.CodeGen.Misc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dyvenix.GenIt.DslPackage.CodeGen.Generators
{
    internal class AngReqGenerator
    {
        private readonly ModelRoot _modelRoot;
        private readonly List<EntityModel> _entities;
        private readonly string _entitiesNamespace;
        private readonly Dictionary<string, ModuleModel> _modules = new Dictionary<string, ModuleModel>();
        private readonly List<string> _usings = new List<string>();

        internal AngReqGenerator(ModelRoot modelRoot)
        {
            // Convenience vars
            _modelRoot = modelRoot;
            _entitiesNamespace = modelRoot.EntitiesNamespace;
            _entities = modelRoot.Types.OfType<EntityModel>().ToList();
            foreach (var module in _modelRoot.Types.OfType<ModuleModel>().ToList())
            {
                if (!_modules.ContainsKey(module.Name))
                    _modules.Add(module.Name, module);
            }
        }

        internal void GenerateCode()
        {
            foreach (var module in _modelRoot.Types.OfType<ModuleModel>().ToList())
            {
                var indexEntities = new List<string>();
                var reqFolderPath = Path.Combine(PackageUtils.SolutionRootPath, module.NgServiceOutputFolder, "req");

                foreach (var entity in _entities.Where(e => e.Module == module.Name && e.InclAngDtos))
                {
                    indexEntities.Add(entity.Name.ToLower());
                    var lines = new List<string>();

                    foreach (var service in entity.ServiceModels.Where(s => s.InclAngService))
                    {
                        lines.AddLines(0, GenerateReadMethodReqs(module, entity, service));
                        lines.AddLines(0, GenerateUpdateMethodReqs(module, entity, service));
                    }

                    if (lines.Count == 0)
                        continue;

                    if (_modelRoot.InclHeader)
                        lines.Insert(0, $"{CodeGenUtils.FileHeader}");

                    // Write the file
                    Directory.CreateDirectory(reqFolderPath);  // Ensure output dir exists
                    var outputFilepath = Path.Combine(reqFolderPath, $"{entity.Name}.reqs.ts");
                    FileHelper.SaveFile(outputFilepath, lines.AsString());
                    OutputHelper.Write($"Completed code gen for angular reqs: {entity.Name}");
                }

                if (indexEntities.Any())
                {
                    var indexFileContent = new List<string>();
                    foreach (var indexEntity in indexEntities)
                        indexFileContent.AddLine(0, $"export * from './{indexEntity.ToLower()}.reqs';");
                    var indexFilePath = Path.Combine(reqFolderPath, "index.ts");
                    FileHelper.SaveFile(indexFilePath, indexFileContent.AsString());
                    OutputHelper.Write($"Completed code gen for angular req index file for module: {module.Name}");
                }
            }
        }

        private List<string> GenerateReadMethodReqs(ModuleModel module, EntityModel entity, ServiceModel service)
        {
            var lines = new List<string>();
            var tc = 0;

            foreach (var readMethod in service.ReadMethods.Where(m => m.UseRequest))
            {
                lines.AddLine();
                lines.AddLine(0, $"export interface {readMethod.Name}Req {{");

                foreach (var filterProp in readMethod.FilterProperties)
                    lines.AddLine(tc + 1, $"{filterProp.PropertyModel.Name.ToCamelCase()} : {filterProp.PropertyModel.TSType};");

                if (readMethod.InclPaging)
                {
                    lines.AddLine();
                    lines.AddLine(1, "pageSize : number;");
                    lines.AddLine(1, "pageOffset : number;");
                    lines.AddLine(1, "recalcRowCount : boolean;");
                    lines.AddLine(1, "getRowCountOnly : boolean;");
                }

                if (readMethod.InclSorting)
                {
                    lines.AddLine();
                    lines.AddLine(1, "sortBy : string");
                    lines.AddLine(1, "sortDesc : boolean");
                }

                lines.AddLine(0, "}");
            }

            if (lines.Count > 0)
                lines.Insert(0, $"{Environment.NewLine}// Read methods");

            return lines;
        }

        private List<string> GenerateUpdateMethodReqs(ModuleModel module, EntityModel entity, ServiceModel service)
        {
            var lines = new List<string>();
            var tc = 0;

            foreach (var updateMethod in service.UpdateMethods)
            {
                lines.AddLine();
                lines.AddLine(tc, $"export interface {updateMethod.Name}Req {{");

                // Always include Id and RowVersion if applicable
                lines.AddLine(1, "id: string");
                if (entity.InclRowVersion)
                    lines.AddLine(1, "rowVersion: Uint8Array");

                var requiredUpdateProps = updateMethod.UpdateProperties.Where(x => !x.IsOptional && !x.PropertyModel.IsRowVersion);
                if (requiredUpdateProps.Any())
                {
                    lines.AddLine();
                    lines.AddLine(tc + 1, "// Required properties");
                    foreach (var requiredUpdateProp in requiredUpdateProps)
                        lines.AddLine(tc + 1, $"{requiredUpdateProp.PropertyModel.Name.ToCamelCase()} : {requiredUpdateProp.PropertyModel.TSType};");
                }

                // Optional properties last
                var optionalUpdateProps = updateMethod.UpdateProperties.Where(x => x.IsOptional && !x.PropertyModel.IsRowVersion);
                if (optionalUpdateProps.Any())
                {
                    lines.AddLine();
                    lines.AddLine(tc + 1, "// Optional properties");
                    foreach (var optionalUpdateProp in optionalUpdateProps)
                        lines.AddLine(tc + 1, $"{optionalUpdateProp.PropertyModel.Name.ToCamelCase()} : {optionalUpdateProp.PropertyModel.TSType};");
                }

                lines.AddLine(tc, "}");
            }

            if (lines.Count > 0)
                lines.Insert(tc, $"{Environment.NewLine}// Update methods");

            return lines;
        }
    }
}
