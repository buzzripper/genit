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
		private readonly string _entitiesNamespace;
		private readonly Dictionary<string, ModuleModel> _modules = new Dictionary<string, ModuleModel>();
		private readonly List<string> _usings = new List<string>();

		internal DtoGenerator(ModelRoot modelRoot)
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

		private void ResetUsings(EntityModel entity, ModuleModel module)
		{
			_usings.Clear();

			// Default usings
			_usings.Add("System");
			_usings.Add(_entitiesNamespace);
			foreach (var u in entity.UsingsList)
				_usings.AddIfNotExists(u);
		}

		internal void GenerateCode()
		{
			foreach (var entity in _entities.Where(e => e.GenerateCode))
			{
				foreach (var service in entity.ServiceModels.Where(s => s.Enabled))
				{
					foreach (var updateMethod in service.UpdateMethods.Where(m => m.UseDto))
					{
						GenerateDto(_modules[entity.Module], entity, service, updateMethod);
					}
				}
			}
		}

		private void GenerateDto(ModuleModel module, EntityModel entity, ServiceModel service, UpdateMethodModel updateMethod)
		{
			ResetUsings(entity, module);
			var dtoName = $"{updateMethod.Name}Req";

			// If any non-primitive property, add entities namespace
			if (updateMethod.UpdateProperties.Any(x => !DataTypes.IsPrimitiveType(x.PropertyModel.DataType)))
				_usings.AddIfNotExists(_entitiesNamespace);

			var fileContent = new List<string>();

			if (_modelRoot.InclHeader)
				fileContent.Add(CodeGenUtils.FileHeader);

			// Usings
			fileContent.AddLines(0, _usings.Select(u => $"using {u};").ToList());

			// Namespace
			fileContent.AddLine();
			fileContent.AddLine(0, $"namespace {module.DtoNamespace}.{service.Version};");

			fileContent.AddLine();
			fileContent.AddLine(0, $"public class {dtoName}");
			fileContent.AddLine(0, "{");

			// Always include Id and RowVersion if applicable
			fileContent.AddLine(1, "public Guid Id { get; set; }");
			if (entity.InclRowVersion)
				fileContent.AddLine(1, "public byte[] RowVersion { get; set; }");

			// Required properties first
			var requiredUpdateProps = updateMethod.UpdateProperties.Where(x => !x.IsOptional);
			if (requiredUpdateProps.Any())
			{
				fileContent.AddLine();
				fileContent.AddLine(1, "// Required properties");
				foreach (var requiredUpdateProp in requiredUpdateProps)
					fileContent.AddLine(1, $"public {requiredUpdateProp.PropertyModel.CSType} {requiredUpdateProp.PropertyModel.Name} {{ get; set; }}");
			}

			// Optional properties last
			var optionalUpdateProps = updateMethod.UpdateProperties.Where(x => x.IsOptional);
			if (optionalUpdateProps.Any())
			{
				fileContent.AddLine();
				fileContent.AddLine(1, "// Required properties");
				foreach (var optionalUpdateProp in optionalUpdateProps)
					fileContent.AddLine(1, $"public {optionalUpdateProp.PropertyModel.CSType} {optionalUpdateProp.PropertyModel.Name} {{ get; set; }}");
			}

			fileContent.AddLine(0, "}");

			var outputDir = Path.Combine(PackageUtils.SolutionRootPath, module.DtoOutputFolder, service.Version);
			Directory.CreateDirectory(outputDir);  // Ensure output dir exists
			var outputFilepath = Path.Combine(outputDir, $"{dtoName}.cs");

			FileHelper.SaveFile(outputFilepath, fileContent.AsString());

			OutputHelper.Write($"Completed code gen for entity: {entity.Name}");
		}
	}
}
