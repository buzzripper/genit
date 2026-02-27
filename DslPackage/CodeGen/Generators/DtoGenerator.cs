using Dyvenix.GenIt.DslPackage.CodeGen.Misc;
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

        internal void GenerateCode()
        {
            foreach (var entity in _entities.Where(e => e.GenerateCode))
            {
                foreach (var dto in entity.DtoModels)
                {
                    GenerateDto(_modules[entity.Module], entity, dto);
                }
            }
        }

        private void ResetUsings()
        {
            _usings.Clear();
            _usings.AddLines(0, _modelUsings);
        }

        private void GenerateDto(ModuleModel module, EntityModel entity, DtoModel dto)
        {
            this.ResetUsings();

            //// If any non-primitive property, add DTOs namespace
            //if (dto.PropertyModels.Any(p => !DataTypes.IsPrimitive(p.DataType)))
            //	_usings.AddIfNotExists(module.DtoNamespace);

            // DateTime needs System namespace
            if (dto.PropertyModels.Any(p => p.DataType == DataTypes.DateTime))
                _usings.AddIfNotExists("System");

            var fileContent = new List<string>();

            if (_modelRoot.InclHeader)
                fileContent.Add(CodeGenUtils.FileHeader);

            // Usings
            fileContent.AddLines(0, _usings.Select(u => $"using {u};").ToList());

            // Namespace
            fileContent.AddLine();
            fileContent.AddLine(0, $"namespace {module.DtoNamespace};");

            fileContent.AddLine();
            fileContent.AddLine(0, $"public class {dto.Name}");
            fileContent.AddLine(0, "{");

            foreach (var prop in dto.PropertyModels)
                fileContent.AddLine(1, $"public {prop.CSType} {prop.Name} {{ get; set; }}");

            fileContent.AddLine(0, "}");

            var outputDir = Path.Combine(PackageUtils.SolutionRootPath, module.DtoOuputFolder);
            Directory.CreateDirectory(outputDir);  // Ensure output dir exists
            var outputFilepath = Path.Combine(outputDir, $"{dto.Name}.g.cs");

            FileHelper.SaveFile(outputFilepath, fileContent.AsString());

            OutputHelper.Write($"Completed code gen for entity: {entity.Name}");
        }
    }
}
