using Dyvenix.GenIt.DslPackage.CodeGen.Misc;
using Microsoft.VisualStudio.Shell;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dyvenix.GenIt.DslPackage.CodeGen.Generators
{
    internal class DtoGenerator
    {
        private readonly ModelRoot _modelRoot;
        private readonly List<EntityModel> _entities;
        private readonly Dictionary<string, ModuleModel> _modules = new Dictionary<string, ModuleModel>();
        private readonly List<string> _usings = new List<string>();
        private readonly List<string> _modelUsings;

        internal DtoGenerator(ModelRoot modelRoot)
        {
            // Convenience vars
            _modelRoot = modelRoot;
            _entities = modelRoot.Types.OfType<EntityModel>().ToList();
            foreach (var module in _modelRoot.Types.OfType<ModuleModel>().ToList())
            {
                if (!_modules.ContainsKey(module.Name))
                    _modules.Add(module.Name, module);
            }
            _modelUsings = modelRoot.UsingsList;
        }

        private void ResetUsings()
        {
            _usings.Clear();
            _usings.AddLines(0, _modelUsings);
        }

        internal void GenerateCode()
        {
            foreach (var entity in _entities.Where(e => e.GenerateCode))
                GenerateDtos(_modules[entity.Module], entity);
        }

        private void GenerateDtos(ModuleModel module, EntityModel entity)
        {
            this.ResetUsings();

            //// If any non-primitive property, add DTOs namespace
            //if (dto.PropertyModels.Any(p => !DataTypes.IsPrimitive(p.DataType)))
            //	_usings.AddIfNotExists(module.DtoNamespace);

            // DateTime needs System namespace
            if (entity.DtoModels.Any(dto => dto.PropertyModels.Any(p => p.DataType == DataTypes.DateTime)))
                _usings.AddIfNotExists("System");

            var fileContent = new List<string>();

            if (_modelRoot.InclHeader)
                fileContent.Add(CodeGenUtils.FileHeader);

            // Usings
            fileContent.AddLines(0, _usings.Select(u => $"using {u};").ToList());

            // Namespace
            fileContent.AddLine();
            fileContent.AddLine(0, $"namespace {module.DtoNamespace};");

            foreach (var dto in entity.DtoModels)
                fileContent.AddLines(0, GenerateDto(_modules[entity.Module], entity, dto));

            var outputDir = Path.Combine(PackageUtils.SolutionRootPath, module.DtoOuputFolder);
            Directory.CreateDirectory(outputDir);  // Ensure output dir exists
            var outputFilepath = Path.Combine(outputDir, $"{entity.Name}Dtos.g.cs");

            FileHelper.SaveFile(outputFilepath, fileContent.AsString());

            OutputHelper.Write($"Completed code gen for entity: {entity.Name}");
        }

        private List<string> GenerateDto(ModuleModel module, EntityModel entity, DtoModel dto)
        {
            var dtoLines = new List<string>();

            dtoLines.AddLine();
            dtoLines.AddLine(0, $"public record {dto.Name} (");

            foreach (var prop in dto.PropertyModels)
                dtoLines.AddLine(1, $"{prop.CSType} {prop.Name},");

            foreach (var navProp in dto.NavigationProperties)
            {
                if (navProp.IsCollection)
                    dtoLines.AddLine(1, $"IReadOnlyList<{navProp.EntityModel.Name}> {navProp.Name},");
            }
            dtoLines[dtoLines.Count - 1] = dtoLines[dtoLines.Count - 1].TrimSuffix(",");

            dtoLines.AddLine(0, ");");

            return dtoLines;
        }
    }
}
