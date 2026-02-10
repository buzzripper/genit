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
		private readonly string _entitiesNamespace;
		private readonly Dictionary<string, ModuleModel> _modules = new Dictionary<string, ModuleModel>();
		private readonly List<string> _usings = new List<string>();

		internal RequestGenerator(ModelRoot modelRoot)
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
			_usings.Add("System");
		}

		internal void GenerateCode()
		{
			foreach (var entity in _entities.Where(e => e.GenerateCode))
			{
				var module = _modules[entity.Module];
				foreach (var service in entity.ServiceModels.Where(s => s.Enabled))
				{
					foreach (var requestMethod in service.ReadMethods.Where(m => m.UseRequest))
						GenerateRequestClass(module, entity, service, requestMethod);
				}
			}
		}

		private void GenerateRequestClass(ModuleModel module, EntityModel entity, ServiceModel service, ReadMethodModel readMethod)
		{
			ResetUsings(entity, module);
			var requestName = $"{readMethod.Name}Req";
			string interfaceDecl = null;

			// Need 'Models' namespace if using IPaging or ISorting
			if (readMethod.InclPaging || readMethod.InclSorting)
				_usings.AddIfNotExists($"{_modelRoot.CommonNamespace}.Shared.Models");

			// If any non-primitive property, add entities namespace
			if (readMethod.FilterProperties.Any(x => !DataTypes.IsPrimitive(x.PropertyModel.DataType)))
				_usings.AddIfNotExists(_entitiesNamespace);

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

			// Usings
			fileContent.AddLines(0, _usings.Select(u => $"using {u};").ToList());

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

			var outputDir = Path.Combine(PackageUtils.SolutionRootPath, module.RequestOutputFolder, $"{service.Version}");
			Directory.CreateDirectory(outputDir);  // Ensure output dir exists
			var outputFilepath = Path.Combine(outputDir, $"{requestName}.cs");

			FileHelper.SaveFile(outputFilepath, fileContent.AsString());

			OutputHelper.Write($"Completed code gen for entity: {entity.Name}");
		}
	}
}
