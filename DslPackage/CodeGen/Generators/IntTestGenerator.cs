using Dyvenix.GenIt.DslPackage.CodeGen.Misc;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dyvenix.GenIt.DslPackage.CodeGen.Generators
{
	internal class IntTestGenerator
	{
		private readonly ModelRoot _modelRoot;
		private readonly List<EntityModel> _entities;
		private readonly string _entitiesNamespace;
		private readonly Dictionary<string, ModuleModel> _modules = new Dictionary<string, ModuleModel>();
		private readonly List<string> _usings = new List<string>();

		internal IntTestGenerator(ModelRoot modelRoot)
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

		internal void Validate(List<string> errors)
		{
			if (string.IsNullOrWhiteSpace(_modelRoot.IntTestsNamespace))
				errors.Add("Integration Tests namespace is missing.");

			if (string.IsNullOrWhiteSpace(_modelRoot.IntTestsRootFolder))
				errors.Add("Integration Tests root folder is missing.");
		}

		private void ResetUsings(EntityModel entity, ModuleModel module)
		{
			_usings.Clear();
			_usings.Add("System");
			_usings.Add($"{_modelRoot.IntTestsNamespace}.Data");
			_usings.Add($"{_modelRoot.IntTestsNamespace}.DataSets");
			_usings.Add($"{_modelRoot.IntTestsNamespace}.Fixtures");


			_usings.AddIfNotExists(_entitiesNamespace);
		}

		internal void GenerateCode()
		{
			foreach (var entity in _entities.Where(e => e.GenerateCode))
			{
				var module = _modules[entity.Module];
				foreach (var service in entity.ServiceModels.Where(s => s.Enabled))
				{
					GenerateReadTestsClass(module, entity, service);
				}
			}
		}

		private void GenerateReadTestsClass(ModuleModel module, EntityModel entity, ServiceModel service)
		{
			var testClassName = $"{entity.Name}ReadTests";
			var outputDir = Path.Combine(PackageUtils.SolutionRootPath, _modelRoot.IntTestsRootFolder, "Tests", module.Name, $"{service.Version}");
			var outputFilepath = Path.Combine(outputDir, $"{testClassName}.g.cs");

			// Need Common.Requests namespace if using IPaging or ISorting
			if (service.ReadMethods.Any(m => m.InclPaging) || service.ReadMethods.Any(m => m.InclSorting))
				_usings.AddIfNotExists($"{_modelRoot.CommonNamespace}.Shared.Requests");

			var fileContent = new List<string>();

			if (_modelRoot.InclHeader)
				fileContent.Add(CodeGenUtils.FileHeader);

			// Usings
			ResetUsings(entity, module);
			_usings.AddIfNotExists($"{module.Namespace}.Shared.Contracts.{service.Version}");
			_usings.AddIfNotExists($"{module.Namespace}.Shared.Requests.{service.Version}");
			fileContent.AddLines(0, _usings.Select(u => $"using {u};").ToList());

			// Namespace
			fileContent.AddLine();
			fileContent.AddLine(0, $"namespace {module.RequestNamespace}.{service.Version};");

			// Fixture class
			fileContent.AddLine();
			fileContent.AddLine(0, $"public class {entity.Name}ReadTestFixture(GlobalTestFixture globalFixture) : IAsyncLifetime");
			fileContent.AddLine(0, "{");
			fileContent.AddLine(1, "public GlobalTestFixture GlobalFixture { get; } = globalFixture;");
			fileContent.AddLine(1, "public TestDataSet DataSet { get; private set; } = default!;");
			fileContent.AddLine();
			fileContent.AddLine(1, "public async ValueTask InitializeAsync()");
			fileContent.AddLine(1, "{");
			fileContent.AddLine(2, "var dataManager = GlobalFixture.Services.GetRequiredService<IDataManager>();");
			fileContent.AddLine(2, "DataSet = await dataManager.Reset(DataSetType.Main.ToString());");
			fileContent.AddLine(1, "}");
			fileContent.AddLine();
			fileContent.AddLine(1, "public ValueTask DisposeAsync() => default;");
			fileContent.AddLine(0, "}");

			// Class declaration
			fileContent.AddLine();
			fileContent.AddLine(0, $"[Collection(nameof(GlobalTestCollection))]");
			fileContent.AddLine(0, $"public class {testClassName} : TestBase, IClassFixture<{entity.Name}ReadTestFixture>");
			fileContent.AddLine(0, "{");
			fileContent.AddLine(1, $"private readonly {entity.Name}ReadTestFixture _fixture;");
			fileContent.AddLine(1, $"private I{entity.Name}Service _{entity.Name.ToCamelCase()}Service = default!;");
			fileContent.AddLine();
			fileContent.AddLine(1, $"public {entity.Name}ReadTests(GlobalTestFixture globalFixture, {entity.Name}ReadTestFixture fixture)");
			fileContent.AddLine(2, $": base(globalFixture)");
			fileContent.AddLine(1, "{");
			fileContent.AddLine(2, $"_fixture = fixture;");
			fileContent.AddLine(1, "}");
			fileContent.AddLine();
			fileContent.AddLine(1, $"public override async ValueTask InitializeAsync()");
			fileContent.AddLine(1, "{");
			fileContent.AddLine(2, "await base.InitializeAsync();");
			fileContent.AddLine(2, $"_{entity.Name.ToCamelCase()}Service = _scope.ServiceProvider.GetRequiredService<I{entity.Name}Service>();");
			fileContent.AddLine(1, "}");

			// Tests
			fileContent.AddLines(0, GenerateReadTestMethods(entity, service));

			fileContent.AddLine(0, "}");

			Directory.CreateDirectory(outputDir);  // Ensure output dir exists
			FileHelper.SaveFile(outputFilepath, fileContent.AsString());
			OutputHelper.Write($"Completed code gen for integration test class: {testClassName}");
		}

		private List<string> GenerateReadTestMethods(EntityModel entity, ServiceModel service)
		{
			var testMethods = new List<string>();

			foreach (var readMethod in service.ReadMethods)
			{
				// TODO
			}

			return testMethods;
		}
	}
}
