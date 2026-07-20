using Dyvenix.GenIt.DslPackage.CodeGen.Misc;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dyvenix.GenIt.DslPackage.CodeGen.Generators
{
	internal class RequestGenerator
	{
		private readonly ModelRoot _modelRoot;
		private readonly List<EntityModel> _entities;
		private readonly Dictionary<string, ModuleModel> _modules = new Dictionary<string, ModuleModel>();
		private readonly List<string> _readMethodUsings = new List<string>();
		private readonly List<string> _updateMethodUsings = new List<string>();
		private readonly List<string> _modelRootUsings;

		internal RequestGenerator(ModelRoot modelRoot, Dictionary<string, ModuleModel> modules)
		{
			// Convenience vars
			_modelRoot = modelRoot;
			_entities = modelRoot.Types.OfType<EntityModel>().ToList();
			_modules = modules;
			_modelRootUsings = modelRoot.UsingsList;
		}

		internal void GenerateCode()
		{
			foreach (var entity in _entities.Where(e => e.GenerateCode))
			{
				var module = _modules[entity.Module];
				foreach (var service in entity.ServiceModels.Where(s => s.Enabled))
				{
					foreach (var readMethod in service.ReadMethods.Where(m => m.UseRequest))
						GenerateReadMethodRequestClass(module, entity, service, readMethod);

					foreach (var updateMethod in service.UpdateMethods)
						GenerateUpdateMethodRequestClass(module, entity, service, updateMethod);
				}
			}
		}

		#region Read Methods

		private void GenerateReadMethodRequestClass(ModuleModel module, EntityModel entity, ServiceModel service, ReadMethodModel readMethod)
		{
			ResetReadMethodUsings(entity, module);
			var requestName = $"{readMethod.Name}Req";
			string interfaceDecl = null;

			// Need 'Requests' namespace if using IPaging or ISorting
			if (readMethod.InclPaging || readMethod.InclSorting)
				_readMethodUsings.AddIfNotExists($"{_modelRoot.CommonNamespace}.Shared.Requests");

			// If any non-primitive property, add DTOs namespace
			if (readMethod.FilterProperties.Any(x => !DataTypes.IsPrimitive(x.PropertyModel.DataType)))
				_readMethodUsings.AddIfNotExists(module.DtoNamespace);

			_readMethodUsings.AddIfNotExists($"{module.DtoNamespace}.Enums");

			// Paging
			var paging = new List<string>();
			if (readMethod.InclPaging)
			{
				paging.AddLine(1, "public int PageSize { get; set; }");
				paging.AddLine(1, "public int PageOffset { get; set; }");
				paging.AddLine(1, "public bool RecalcRowCount { get; set; }");
				paging.AddLine(1, "public bool GetRowCountOnly { get; set; }");
				interfaceDecl = "IPagingRequest";
			}

			// Sorting
			var sorting = new List<string>();
			if (readMethod.InclSorting)
			{
				sorting.AddLine(1, "public string SortBy { get; set; } = null!;");
				sorting.AddLine(1, "public bool SortDesc { get; set; }");
				interfaceDecl = string.IsNullOrWhiteSpace(interfaceDecl) ? "ISortingRequest" : $"{interfaceDecl}, ISortingRequest";
			}

			var fileContent = new List<string>();

			if (_modelRoot.InclHeader)
				fileContent.Add(CodeGenUtils.FileHeader);
			fileContent.AddLine(0, CodeGenUtils.NullableEnableDirective);

			// Usings
			fileContent.AddLines(0, _readMethodUsings.Select(u => $"using {u};").ToList());

			// Namespace
			fileContent.AddLine();
			fileContent.AddLine(0, $"namespace {module.RequestNamespace}.{service.Version};");

			// Class declaration
			if (readMethod.InclPaging || readMethod.InclSorting)
				interfaceDecl = $" : {interfaceDecl}";
			fileContent.AddLine();
			fileContent.AddLine(0, $"public class {requestName}{interfaceDecl}");
			fileContent.AddLine(0, "{");

			// Paging
			fileContent.AddLines(0, paging);

			// Sorting
			fileContent.AddLines(0, sorting);

			// Filter properties
			var filterProps = readMethod.FilterProperties;
			if (filterProps.Any())
			{
				string nullStr = null;
				string initStr = null;
				foreach (var filterProp in filterProps)
				{
					if (filterProp.IsOptional)
					{
						nullStr = "?";
						initStr = "";
					}
					else
					{
						nullStr = null;
						initStr = " = null!;";
					}
					fileContent.AddLine(1, $"public {filterProp.PropertyModel.CSType}{nullStr} {filterProp.PropertyModel.Name} {{ get; set; }}{initStr}");
				}
			}

			fileContent.AddLine(0, "}");

			var outputDir = Path.Combine(PackageUtils.SolutionRootPath, module.RequestOutputFolder, service.Version, entity.Name);
			Directory.CreateDirectory(outputDir);  // Ensure output dir exists
			var outputFilepath = Path.Combine(outputDir, $"{requestName}.g.cs");

			FileHelper.SaveFile(outputFilepath, fileContent.AsString());

			module.DtoGlobalUsings.AddIfNotExists($"{service.Version}.{entity.Name}");

			OutputHelper.Write($"Completed code gen for entity: {entity.Name}");
		}

		private void ResetReadMethodUsings(EntityModel entity, ModuleModel module)
		{
			_readMethodUsings.Clear();
			_readMethodUsings.Add("System");
		}

		#endregion

		#region Update Methods

		private void GenerateUpdateMethodRequestClass(ModuleModel module, EntityModel entity, ServiceModel service, UpdateMethodModel updateMethod)
		{
			var dtoName = $"{updateMethod.Name}Req";

			this.ResetUpdateMethodUsings();

			// If any non-primitive property, add entities namespace
			if (updateMethod.UpdateProperties.Any(x => !DataTypes.IsPrimitive(x.PropertyModel.DataType)))
				_updateMethodUsings.AddIfNotExists(module.DtoNamespace);

			// DateTime needs System namespace
			if (updateMethod.UpdateProperties.Any(x => x.PropertyModel.DataType == DataTypes.DateTime))
				_updateMethodUsings.AddIfNotExists("System");

			// If any enum properties, add Enums namespace
			if (entity.Properties.Any(p => DataTypes.IsEnumType(p.DataType)))
				_updateMethodUsings.AddIfNotExists($"{module.DtoNamespace}.Enums");

			var fileContent = new List<string>();

			if (_modelRoot.InclHeader)
				fileContent.Add(CodeGenUtils.FileHeader);

			// Usings
			fileContent.AddLines(0, _updateMethodUsings.Select(u => $"using {u};").ToList());

			// Namespace
			fileContent.AddLine();
			fileContent.AddLine(0, $"namespace {module.RequestNamespace}.{service.Version}.{entity.Name};");

			fileContent.AddLine();
			fileContent.AddLine(0, $"public class {dtoName}");
			fileContent.AddLine(0, "{");

			// Always include Id and RowVersion if applicable
			fileContent.AddLine(1, "public Guid Id { get; set; }");
			if (entity.InclRowVersion)
				fileContent.AddLine(1, "public byte[] RowVersion { get; set; }");

			// Required properties first
			var requiredUpdateProps = updateMethod.UpdateProperties.Where(x => !x.IsOptional && !x.PropertyModel.IsRowVersion);
			if (requiredUpdateProps.Any())
			{
				fileContent.AddLine();
				fileContent.AddLine(1, "// Required properties");
				foreach (var requiredUpdateProp in requiredUpdateProps)
					fileContent.AddLine(1, $"public {requiredUpdateProp.PropertyModel.CSType} {requiredUpdateProp.PropertyModel.Name} {{ get; set; }}");
			}

			// Optional properties last
			var optionalUpdateProps = updateMethod.UpdateProperties.Where(x => x.IsOptional && !x.PropertyModel.IsRowVersion);
			if (optionalUpdateProps.Any())
			{
				fileContent.AddLine();
				fileContent.AddLine(1, "// Optional properties");
				foreach (var optionalUpdateProp in optionalUpdateProps)
					fileContent.AddLine(1, $"public {optionalUpdateProp.PropertyModel.CSType} {optionalUpdateProp.PropertyModel.Name} {{ get; set; }}");
			}

			fileContent.AddLine(0, "}");

			var outputDir = Path.Combine(PackageUtils.SolutionRootPath, module.RequestOutputFolder, service.Version, entity.Name);
			Directory.CreateDirectory(outputDir);  // Ensure output dir exists
			var outputFilepath = Path.Combine(outputDir, $"{dtoName}.g.cs");

			FileHelper.SaveFile(outputFilepath, fileContent.AsString());

			module.RequestGlobalUsings.AddIfNotExists($"{service.Version}.{entity.Name}");

			OutputHelper.Write($"Completed code gen for entity: {entity.Name}");
		}

		private void ResetUpdateMethodUsings()
		{
			_updateMethodUsings.Clear();
			_updateMethodUsings.AddLines(0, _modelRootUsings);
		}

		#endregion
	}
}
