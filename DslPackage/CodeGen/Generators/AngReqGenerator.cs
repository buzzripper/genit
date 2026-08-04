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
		private readonly Dictionary<string, ModuleModel> _modules = new Dictionary<string, ModuleModel>();

		internal AngReqGenerator(ModelRoot modelRoot, Dictionary<string, ModuleModel> modules)
		{
			// Convenience vars
			_modelRoot = modelRoot;
			_entities = modelRoot.Types.OfType<EntityModel>().ToList();
			_modules = modules;
		}

		private bool HasReqs(ModuleModel module, EntityModel entity)
		{
			return
				entity.Module == module.Name &&
				entity.InclAngDtos &&
				entity.ServiceModels.Any(s => s.InclAngService) &&
				(
					entity.ServiceModels.Any(s => s.ReadMethods.Any(m => m.UseRequest)) ||
					entity.ServiceModels.Any(s => s.UpdateMethods.Any())
				);
		}

		internal void GenerateCode()
		{
			foreach (var module in _modules.Values)
			{
				var entitiesWithReqs = _entities.Where(e => HasReqs(module, e)).ToList();
				if (!entitiesWithReqs.Any())
					return;

				var indexEntities = new List<string>();
				var reqFolderPath = Path.Combine(PackageUtils.SolutionRootPath, module.NgServiceOutputFolder, "req");
				Directory.CreateDirectory(reqFolderPath);

				foreach (var entity in entitiesWithReqs)
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
					var outputFilepath = Path.Combine(reqFolderPath, $"{entity.Name.ToLower()}.req.g.ts");
					FileHelper.SaveFile(outputFilepath, lines.AsString());
					OutputHelper.Write($"Completed code gen for angular reqs: {entity.Name}");
				}

				if (indexEntities.Any())
				{
					var newLines = new List<string>();
					foreach (var indexEntity in indexEntities)
						newLines.AddLine(0, $"export * from './{indexEntity.ToLower()}.req.g';");
					var indexFilePath = Path.Combine(reqFolderPath, "index.ts");
					FileHelper.PreserveCustomContentAndWriteFile(newLines, indexFilePath);
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
