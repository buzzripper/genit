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

		internal DtoGenerator(ModelRoot modelRoot, Dictionary<string, ModuleModel> modules)
		{
			// Convenience vars
			_modelRoot = modelRoot;
			_entities = modelRoot.Types.OfType<EntityModel>().ToList();
			_modules = modules;
			_modelUsings = modelRoot.UsingsList;
		}

		private void ResetUsings()
		{
			_usings.Clear();
			_usings.AddLines(0, _modelUsings);
		}

		internal void GenerateCode()
		{
			foreach (var entity in _entities.Where(e => e.GenerateCode && e.DtoModels.Any()))
				GenerateDtos(_modules[entity.Module], entity);
		}

		private void GenerateDtos(ModuleModel module, EntityModel entity)
		{
			this.ResetUsings();

			// DateTime needs System namespace
			if (entity.DtoModels.Any(dto => dto.PropertyModels.Any(p => p.DataType == DataTypes.DateTime)))
				_usings.AddIfNotExists("System");

			// If any enum properties, add Enums namespace
			if (entity.Properties.Any(p => DataTypes.IsEnumType(p.DataType)))
				_usings.AddIfNotExists($"{module.DtoNamespace}.Enums");

			var fileContent = new List<string>();

			if (_modelRoot.InclHeader)
				fileContent.Add(CodeGenUtils.FileHeader);

			// Usings
			fileContent.AddLines(0, _usings.Select(u => $"using {u};").ToList());

			// Namespace
			fileContent.AddLine();
			fileContent.AddLine(0, $"namespace {module.DtoNamespace}.{entity.Name};");

			foreach (var dto in entity.DtoModels)
				fileContent.AddLines(0, GenerateDto(_modules[entity.Module], entity, dto));

			var outputDir = Path.Combine(PackageUtils.SolutionRootPath, module.DtoOuputFolder, entity.Name);
			Directory.CreateDirectory(outputDir);  // Ensure output dir exists
			var outputFilepath = Path.Combine(outputDir, $"{entity.Name}Dtos.g.cs");

			FileHelper.SaveFile(outputFilepath, fileContent.AsString());

			module.DtoGlobalUsings.AddIfNotExists(entity.Name);

			OutputHelper.Write($"Completed code gen for entity: {entity.Name}");
		}

		private List<string> GenerateDto(ModuleModel module, EntityModel entity, DtoModel dto)
		{
			var dtoLines = new List<string>();

			dtoLines.AddLine();
			dtoLines.AddLine(0, $"public record {dto.Name} (");

			// Non-nullables first (retuired by Record types)
			foreach (var prop in dto.PropertyModelsOrdered)
			{
				if (!prop.IsNullable)
				{
					dtoLines.AddLine(1, $"{prop.CSType} {prop.Name},");
				}
				else
				{
					string nullSuffix = prop.IsNullable ? "?" : null;
					string nullInit = prop.RequiresInit ? " = null!" : null;
					dtoLines.AddLine(1, $"{prop.CSType}{nullSuffix} {prop.Name}{nullInit},");
				}
			}

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
