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
				foreach (var service in entity.ServiceModels.Where(s => s.Enabled))
				{
					foreach (var updateMethod in service.UpdateMethods)
					{
						GenerateDto(_modules[entity.Module], entity, service, updateMethod);
					}
				}
			}
		}

		private void ResetUsings()
		{
			_usings.Clear();
			_usings.AddLines(0, _modelUsings);
		}

		private void GenerateDto(ModuleModel module, EntityModel entity, ServiceModel service, UpdateMethodModel updateMethod)
		{
			var dtoName = $"{updateMethod.Name}Req";

			this.ResetUsings();

			// If any non-primitive property, add entities namespace
			if (updateMethod.UpdateProperties.Any(x => !DataTypes.IsPrimitive(x.PropertyModel.DataType)))
				_usings.AddIfNotExists(_modelRoot.EntitiesNamespace);

			// DateTime needs System namespace
			if (updateMethod.UpdateProperties.Any(x => x.PropertyModel.DataType == DataTypes.DateTime))
				_usings.AddIfNotExists("System");

			var fileContent = new List<string>();

			if (_modelRoot.InclHeader)
				fileContent.Add(CodeGenUtils.FileHeader);

			// Usings
			fileContent.AddLines(0, _usings.Select(u => $"using {u};").ToList());

			// Namespace
			fileContent.AddLine();
			fileContent.AddLine(0, $"namespace {module.RequestNamespace}.v{service.Version};");

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

			var outputDir = Path.Combine(PackageUtils.SolutionRootPath, module.RequestOutputFolder, $"v{service.Version}");
			Directory.CreateDirectory(outputDir);  // Ensure output dir exists
			var outputFilepath = Path.Combine(outputDir, $"{dtoName}.cs");

			FileHelper.SaveFile(outputFilepath, fileContent.AsString());

			OutputHelper.Write($"Completed code gen for entity: {entity.Name}");
		}
	}
}
