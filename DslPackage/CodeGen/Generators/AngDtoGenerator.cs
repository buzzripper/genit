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
				var indexEntities = new List<string>();
				var dtoFolderPath = Path.Combine(PackageUtils.SolutionRootPath, module.NgServiceOutputFolder, "dto");

				foreach (var entity in _entities.Where(e => e.Module == module.Name && e.InclAngDtos))
				{
					indexEntities.Add(entity.Name.ToLower());
					GenerateDtos(module, entity, dtoFolderPath);
				}

				if (indexEntities.Any())
				{
					var indexFileContent = new List<string>();
					foreach (var indexEntity in indexEntities)
						indexFileContent.AddLine(0, $"export * from './{indexEntity.ToLower()}.dto';");
					var indexFilePath = Path.Combine(dtoFolderPath, "index.ts");
					FileHelper.SaveFile(indexFilePath, indexFileContent.AsString());
					OutputHelper.Write($"Completed code gen for angular dto index file for module: {module.Name}");
				}
			}
		}

		private void GenerateDtos(ModuleModel module, EntityModel entity, string dtoFolderPath)
		{
			var fileContent = new List<string>();

			if (_modelRoot.InclHeader)
				fileContent.Add(CodeGenUtils.FileHeader);

			foreach (var dto in entity.DtoModels)
			{
				fileContent.AddLine();
				fileContent.AddLine(0, $"export interface {dto.Name} {{");
				foreach (var dtoProp in dto.PropertyModels)
					fileContent.AddLine(1, $"{dtoProp.Name.ToCamelCase()}: {dtoProp.TSType};");
				fileContent.AddLine(0, $"}}");
			}

			Directory.CreateDirectory(dtoFolderPath);  // Ensure output dir exists
			var outputFilepath = Path.Combine(dtoFolderPath, $"{entity.Name}.dto.ts");
			FileHelper.SaveFile(outputFilepath, fileContent.AsString());
			OutputHelper.Write($"Completed code gen for angular dtos: {entity.Name}");
		}
	}
}
