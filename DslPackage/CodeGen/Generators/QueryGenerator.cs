using Dyvenix.GenIt.DslPackage.CodeGen.Misc;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dyvenix.GenIt.DslPackage.CodeGen.Generators
{
	internal class QueryGenerator
	{
		private readonly ModelRoot _modelRoot;
		private readonly List<EntityModel> _entities;
		private readonly string _entitiesNamespace;
		private readonly Dictionary<string, ModuleModel> _modules = new Dictionary<string, ModuleModel>();
		private readonly List<string> _usings = new List<string>();

		internal QueryGenerator(ModelRoot modelRoot)
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
			_usings.AddLines(0, _modelRoot.UsingsList);
		}

		internal void GenerateCode()
		{
			foreach (var entity in _entities.Where(e => e.GenerateCode))
			{
				foreach (var service in entity.ServiceModels.Where(s => s.Enabled))
				{
					foreach (var queryMethod in service.ReadMethods.Where(m => m.UseQuery))
					{
						GenerateQuery(_modules[entity.Module], entity, service, queryMethod);
					}
				}
			}
		}

		private void GenerateQuery(ModuleModel module, EntityModel entity, ServiceModel service, ReadMethodModel readMethod)
		{
			ResetUsings(entity, module);
			var queryName = $"{readMethod.Name}Query";
			string interfaceDecl = null;

			// If any non-primitive property, add entities namespace
			if (readMethod.FilterProperties.Any(x => !DataTypes.IsPrimitiveType(x.PropertyModel.DataType)))
				_usings.AddIfNotExists(_entitiesNamespace);

			// Paging
			var paging = new List<string>();
			if (readMethod.InclPaging)
			{
				paging.AddLine(1, "public int PageSize { get; set; }");
				paging.AddLine(1, "public int PageOffset { get; set; }");
				paging.AddLine(1, "public bool RecalcRowCount { get; set; }");
				paging.AddLine(1, "public bool GetRowCountOnly { get; set; }");
				interfaceDecl = "IPagingQuery";
			}

			// Sorting
			var sorting = new List<string>();
			if (readMethod.InclSorting)
			{
				sorting.AddLine(1, "public string SortBy { get; set; }");
				sorting.AddLine(1, "public bool SortDesc { get; set; }");
				interfaceDecl = readMethod.InclPaging ? ", ISortingQuery" : "ISortingQuery";
				_usings.AddIfNotExists("SORTING NS");
			}

			var fileContent = new List<string>();

			if (_modelRoot.InclHeader)
				fileContent.Add(CodeGenUtils.FileHeader);

			// Usings
			fileContent.AddLines(0, _usings.Select(u => $"using {u};").ToList());

			// Namespace
			fileContent.AddLine();
			fileContent.AddLine(0, $"namespace {module.QueryNamespace}.{service.Version};");

			// Class declaration
			if (readMethod.InclPaging || readMethod.InclSorting)
				interfaceDecl = $" : {interfaceDecl}";
			fileContent.AddLine();
			fileContent.AddLine(0, $"public class {queryName}{interfaceDecl}");
			fileContent.AddLine(0, "{");

			// Paging
			fileContent.AddLines(0, paging);

			// Sorting
			fileContent.AddLines(0, sorting);

			// Filter properties
			var filterProps = readMethod.FilterProperties.Where(x => !x.IsOptional);
			if (filterProps.Any())
			{
				fileContent.AddLine();
				foreach (var filterProp in filterProps)
					fileContent.AddLine(1, $"public {filterProp.PropertyModel.CSType} {filterProp.PropertyModel.Name} {{ get; set; }}");
			}

			fileContent.AddLine(0, "}");

			var outputDir = Path.Combine(PackageUtils.SolutionRootPath, module.QueryOutputFolder, service.Version);
			Directory.CreateDirectory(outputDir);  // Ensure output dir exists
			var outputFilepath = Path.Combine(outputDir, $"{queryName}.cs");

			FileHelper.SaveFile(outputFilepath, fileContent.AsString());

			OutputHelper.Write($"Completed code gen for entity: {entity.Name}");
		}
	}
}
