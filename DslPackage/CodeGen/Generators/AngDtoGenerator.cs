using Dyvenix.GenIt.DslPackage.CodeGen.Misc;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dyvenix.GenIt.DslPackage.CodeGen.Generators
{
	internal class AngDtoGenerator
	{
		private readonly ModelRoot _modelRoot;
		private readonly List<EntityModel> _entities;
		private readonly Dictionary<string, ModuleModel> _modules = new Dictionary<string, ModuleModel>();

		internal AngDtoGenerator(ModelRoot modelRoot, Dictionary<string, ModuleModel> modules)
		{
			// Convenience vars
			_modelRoot = modelRoot;
			_entities = modelRoot.Types.OfType<EntityModel>().ToList();
			_modules = modules;
		}

		internal void GenerateCode()
		{
			foreach (var module in _modules.Values)
			{
				var dtoFolderPath = Path.Combine(PackageUtils.SolutionRootPath, module.NgServiceOutputFolder, "dto")?.ToLower();
				var indexEntities = new List<string>();

				foreach (var entity in _entities.Where(e => e.Module == module.Name && e.DtoModels.Any()))
				{
					GenerateDtos(module, entity, dtoFolderPath);
					indexEntities.Add(entity.Name.ToLower());
				}

				// Write the Angular 'barrel' (index.ts) file
				if (indexEntities.Any())
				{
					var newLines = new List<string>();
					foreach (var indexEntity in indexEntities)
						newLines.AddLine(0, $"export * from './{indexEntity.ToLower()}.dtos.g';");
					var indexFilepath = Path.Combine(dtoFolderPath, "index.ts");
					FileHelper.PreserveCustomContentAndWriteFile(newLines, indexFilepath);
					OutputHelper.Write($"Completed code gen for angular dto index file for module: {module.Name}");
				}
			}
		}

		private void GenerateDtos(ModuleModel module, EntityModel entity, string dtoFolderPath)
		{
			var dtoLines = new List<string>();
			var impEnums = new List<string>();

			foreach (var dto in entity.DtoModels)
			{
				dtoLines.AddLine();
				dtoLines.AddLine(0, $"export class {dto.Name} {{");
				foreach (var dtoProp in dto.PropertyModels)
				{
					dtoLines.AddLine(1, $"{dtoProp.Name.ToCamelCase()}!: {dtoProp.TSType};");
					if (DataTypes.IsEnumType(dtoProp.DataType))
						impEnums.Add(dtoProp.TSType);
				}
				dtoLines.AddLine(0, $"}}");
			}

			var fileContent = new List<string>();

			if (_modelRoot.InclHeader)
				fileContent.Add(CodeGenUtils.FileHeader);

			if (impEnums.Any())
				fileContent.AddLine(0, $"import {{{string.Join(", ", impEnums)}}} from '../enum';");

			fileContent.AddLine();
			fileContent.AddLines(0, dtoLines);

			Directory.CreateDirectory(dtoFolderPath);  // Ensure output dir exists
			var outputFilepath = Path.Combine(dtoFolderPath, $"{entity.Name.ToLower()}.dtos.g.ts");
			FileHelper.SaveFile(outputFilepath, fileContent.AsString());
			OutputHelper.Write($"Completed code gen for angular dtos: {entity.Name}");
		}


	}
}
