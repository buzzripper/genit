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
			_usings.Add("System.Linq");
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
			fileContent.AddLine(0, $"namespace {_modelRoot.IntTestsNamespace}.Tests.{module.Name}.{service.Version};");

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
				testMethods.AddLines(0, GenerateReadMethodTests(entity, readMethod));
			}

			return testMethods;
		}

		private List<string> GenerateReadMethodTests(EntityModel entity, ReadMethodModel readMethod)
		{
			var testMethods = new List<string>();

			AddReadSuccessTest(testMethods, entity, readMethod);

			if (readMethod.FilterProperties.Any(fp => fp.IsOptional && !fp.IsInternal))
				AddReadOptionalOmittedTest(testMethods, entity, readMethod);

			if (readMethod.FilterProperties.Any(fp => !fp.IsInternal))
				AddReadInvalidFilterTest(testMethods, entity, readMethod);

			if (readMethod.InclPaging)
				AddReadPagingTest(testMethods, entity, readMethod);

			if (readMethod.InclSorting)
				AddReadSortingTest(testMethods, entity, readMethod);

			return testMethods;
		}

		private void AddReadSuccessTest(List<string> testMethods, EntityModel entity, ReadMethodModel readMethod)
		{
			var entityListExpr = GetEntityListExpression(entity);
			var sampleVarName = GetSampleVarName(entity);
			var includeOptional = readMethod.FilterProperties.Any(fp => fp.IsOptional && !fp.IsInternal);
			var filterPropsToAssign = GetFilterPropsToAssign(readMethod, includeOptional);
			var expectedFilterProps = GetExpectedFilterProps(readMethod, includeOptional);

			testMethods.AddLine();
			testMethods.AddLine(1, "[Fact]");
			testMethods.AddLine(1, $"public async Task {readMethod.Name}_Success()");
			testMethods.AddLine(1, "{");
			testMethods.AddLine(2, "// Arrange");

			AddSampleSelection(testMethods, entityListExpr, sampleVarName, expectedFilterProps);
			var filterVars = AddFilterValueDeclarations(testMethods, filterPropsToAssign, sampleVarName, false);

			if (readMethod.UseRequest)
			{
				testMethods.AddLine(2, $"var request = new {readMethod.Name}Req();");
				if (readMethod.InclPaging)
				{
					testMethods.AddLine(2, "request.PageSize = 0;");
					testMethods.AddLine(2, "request.PageOffset = 0;");
					testMethods.AddLine(2, "request.RecalcRowCount = true;");
					testMethods.AddLine(2, "request.GetRowCountOnly = false;");
				}
				AddRequestAssignments(testMethods, filterVars);
			}

			if (!readMethod.IsSingle)
			{
				AddExpectedList(testMethods, entityListExpr, expectedFilterProps, filterVars);
			}

			testMethods.AddLine();
			testMethods.AddLine(2, "// Act");
			var serviceVarName = $"_{entity.Name.ToCamelCase()}Service";
			var methodArgs = readMethod.UseRequest ? "request" : BuildMethodCallArgs(readMethod, filterVars, includeOptional);
			testMethods.AddLine(2, $"var result = await {serviceVarName}.{readMethod.Name}({methodArgs});");

			testMethods.AddLine();
			testMethods.AddLine(2, "// Assert");
			if (readMethod.IsSingle)
			{
				testMethods.AddLine(2, $"Assert.Equal({sampleVarName}.Id, result.Id);");
			}
			else if (readMethod.InclPaging)
			{
				testMethods.AddLine(2, "Assert.Equal(expectedList.Count, result.Items.Count);");
				testMethods.AddLine(2, "Assert.Equal(expectedList.Count, result.TotalRowCount);");
			}
			else
			{
				testMethods.AddLine(2, "Assert.Equal(expectedList.Count, result.Count);");
			}
			testMethods.AddLine(1, "}");
		}

		private void AddReadOptionalOmittedTest(List<string> testMethods, EntityModel entity, ReadMethodModel readMethod)
		{
			var entityListExpr = GetEntityListExpression(entity);
			var sampleVarName = GetSampleVarName(entity);
			var filterPropsToAssign = GetFilterPropsToAssign(readMethod, false);
			var expectedFilterProps = GetExpectedFilterProps(readMethod, false);

			testMethods.AddLine();
			testMethods.AddLine(1, "[Fact]");
			testMethods.AddLine(1, $"public async Task {readMethod.Name}_OptionalFiltersOmitted_Success()");
			testMethods.AddLine(1, "{");
			testMethods.AddLine(2, "// Arrange");

			AddSampleSelection(testMethods, entityListExpr, sampleVarName, expectedFilterProps);
			var filterVars = AddFilterValueDeclarations(testMethods, filterPropsToAssign, sampleVarName, false);

			if (readMethod.UseRequest)
			{
				testMethods.AddLine(2, $"var request = new {readMethod.Name}Req();");
				if (readMethod.InclPaging)
				{
					testMethods.AddLine(2, "request.PageSize = 0;");
					testMethods.AddLine(2, "request.PageOffset = 0;");
					testMethods.AddLine(2, "request.RecalcRowCount = true;");
					testMethods.AddLine(2, "request.GetRowCountOnly = false;");
				}
				AddRequestAssignments(testMethods, filterVars);
			}

			if (!readMethod.IsSingle)
				AddExpectedList(testMethods, entityListExpr, expectedFilterProps, filterVars);

			testMethods.AddLine();
			testMethods.AddLine(2, "// Act");
			var serviceVarName = $"_{entity.Name.ToCamelCase()}Service";
			var methodArgs = readMethod.UseRequest ? "request" : BuildMethodCallArgs(readMethod, filterVars, false);
			testMethods.AddLine(2, $"var result = await {serviceVarName}.{readMethod.Name}({methodArgs});");

			testMethods.AddLine();
			testMethods.AddLine(2, "// Assert");
			if (readMethod.IsSingle)
			{
				testMethods.AddLine(2, "Assert.NotNull(result);");
			}
			else if (readMethod.InclPaging)
			{
				testMethods.AddLine(2, "Assert.Equal(expectedList.Count, result.Items.Count);");
				testMethods.AddLine(2, "Assert.Equal(expectedList.Count, result.TotalRowCount);");
			}
			else
			{
				testMethods.AddLine(2, "Assert.Equal(expectedList.Count, result.Count);");
			}

			testMethods.AddLine(1, "}");
		}

		private void AddReadInvalidFilterTest(List<string> testMethods, EntityModel entity, ReadMethodModel readMethod)
		{
			var entityListExpr = GetEntityListExpression(entity);
			var sampleVarName = GetSampleVarName(entity);
			var includeOptional = readMethod.FilterProperties.Any(fp => fp.IsOptional && !fp.IsInternal);
			var filterPropsToAssign = GetFilterPropsToAssign(readMethod, includeOptional);
			var expectedFilterProps = GetExpectedFilterProps(readMethod, includeOptional);
			var invalidFilterProp = filterPropsToAssign.FirstOrDefault(fp => !fp.IsInternal) ?? readMethod.FilterProperties.FirstOrDefault(fp => !fp.IsInternal);
			if (invalidFilterProp == null)
				return;

			var outcome = readMethod.IsSingle ? "NotFound" : "NoResults";
			testMethods.AddLine();
			testMethods.AddLine(1, "[Fact]");
			testMethods.AddLine(1, $"public async Task {readMethod.Name}_{outcome}()");
			testMethods.AddLine(1, "{");
			testMethods.AddLine(2, "// Arrange");

			AddSampleSelection(testMethods, entityListExpr, sampleVarName, expectedFilterProps);
			var filterVars = AddFilterValueDeclarations(testMethods, filterPropsToAssign, sampleVarName, false);

			var invalidVarName = $"invalid{invalidFilterProp.PropertyModel.Name}";
			testMethods.AddLine(2, $"var {invalidVarName} = {GetInvalidValueExpression(invalidFilterProp, sampleVarName)};");
			filterVars[invalidFilterProp] = invalidVarName;

			if (readMethod.UseRequest)
			{
				testMethods.AddLine(2, $"var request = new {readMethod.Name}Req();");
				if (readMethod.InclPaging)
				{
					testMethods.AddLine(2, "request.PageSize = 0;");
					testMethods.AddLine(2, "request.PageOffset = 0;");
					testMethods.AddLine(2, "request.RecalcRowCount = true;");
					testMethods.AddLine(2, "request.GetRowCountOnly = false;");
				}
				AddRequestAssignments(testMethods, filterVars);
			}

			testMethods.AddLine();
			testMethods.AddLine(2, "// Act");
			var serviceVarName = $"_{entity.Name.ToCamelCase()}Service";
			var methodArgs = readMethod.UseRequest ? "request" : BuildMethodCallArgs(readMethod, filterVars, includeOptional);
			if (readMethod.IsSingle)
			{
				testMethods.AddLine(2, $"var result = await {serviceVarName}.{readMethod.Name}({methodArgs});");
			}
			else
			{
				testMethods.AddLine(2, $"var result = await {serviceVarName}.{readMethod.Name}({methodArgs});");
			}

			testMethods.AddLine();
			testMethods.AddLine(2, "// Assert");
			if (readMethod.IsSingle)
			{
				testMethods.AddLine(2, "Assert.Null(result);");
			}
			else if (readMethod.InclPaging)
			{
				testMethods.AddLine(2, "Assert.Empty(result.Items);");
			}
			else
			{
				testMethods.AddLine(2, "Assert.Empty(result);");
			}

			testMethods.AddLine(1, "}");
		}

		private void AddReadPagingTest(List<string> testMethods, EntityModel entity, ReadMethodModel readMethod)
		{
			var entityListExpr = GetEntityListExpression(entity);
			var sampleVarName = GetSampleVarName(entity);
			var includeOptional = readMethod.FilterProperties.Any(fp => fp.IsOptional && !fp.IsInternal);
			var filterPropsToAssign = GetFilterPropsToAssign(readMethod, includeOptional);
			var expectedFilterProps = GetExpectedFilterProps(readMethod, includeOptional);

			testMethods.AddLine();
			testMethods.AddLine(1, "[Fact]");
			testMethods.AddLine(1, $"public async Task {readMethod.Name}_PagingSuccess()");
			testMethods.AddLine(1, "{");
			testMethods.AddLine(2, "// Arrange");

			AddSampleSelection(testMethods, entityListExpr, sampleVarName, expectedFilterProps);
			var filterVars = AddFilterValueDeclarations(testMethods, filterPropsToAssign, sampleVarName, true);
			testMethods.AddLine(2, $"var request = new {readMethod.Name}Req();");
			AddRequestAssignments(testMethods, filterVars);
			testMethods.AddLine(2, "request.PageSize = 3;");
			testMethods.AddLine(2, "request.RecalcRowCount = true;");
			testMethods.AddLine(2, "request.GetRowCountOnly = false;");

			AddExpectedList(testMethods, entityListExpr, expectedFilterProps, filterVars);
			testMethods.AddLine(2, "var totalCount = expectedList.Count;");
			testMethods.AddLine(2, "var lastPgOffset = totalCount == 0 ? 0 : totalCount / request.PageSize;");
			testMethods.AddLine(2, "if (totalCount % request.PageSize == 0 && totalCount > 0)");
			testMethods.AddLine(3, "lastPgOffset -= 1;");
			testMethods.AddLine(2, "var lastPgSize = totalCount == 0 ? 0 : totalCount - (lastPgOffset * request.PageSize);");

			testMethods.AddLine();
			testMethods.AddLine(2, "// Act");
			var serviceVarName = $"_{entity.Name.ToCamelCase()}Service";
			testMethods.AddLine(2, "request.PageOffset = 0;");
			testMethods.AddLine(2, $"var firstPgList = await {serviceVarName}.{readMethod.Name}(request);");
			testMethods.AddLine(2, "request.PageOffset = lastPgOffset;");
			testMethods.AddLine(2, $"var lastPgList = await {serviceVarName}.{readMethod.Name}(request);");

			testMethods.AddLine();
			testMethods.AddLine(2, "// Assert");
			testMethods.AddLine(2, "Assert.True(totalCount == firstPgList.TotalRowCount, $\"First total count s/b {totalCount} but was {firstPgList.TotalRowCount}\");");
			testMethods.AddLine(2, "Assert.True(request.PageSize == firstPgList.Items.Count, $\"First page size s/b {request.PageSize} but was {firstPgList.Items.Count}\");");
			testMethods.AddLine(2, "Assert.True(totalCount == lastPgList.TotalRowCount, $\"Last total count s/b {totalCount} but was {lastPgList.TotalRowCount}\");");
			testMethods.AddLine(2, "Assert.True(lastPgSize == lastPgList.Items.Count, $\"Last page size s/b {lastPgSize} but was {lastPgList.Items.Count}\");");
			testMethods.AddLine(1, "}");
		}

		private void AddReadSortingTest(List<string> testMethods, EntityModel entity, ReadMethodModel readMethod)
		{
			var sortProp = GetSortingProperty(entity);
			if (sortProp == null)
				return;

			var entityListExpr = GetEntityListExpression(entity);
			var sampleVarName = GetSampleVarName(entity);
			var includeOptional = readMethod.FilterProperties.Any(fp => fp.IsOptional && !fp.IsInternal);
			var filterPropsToAssign = GetFilterPropsToAssign(readMethod, includeOptional);
			var expectedFilterProps = GetExpectedFilterProps(readMethod, includeOptional);

			testMethods.AddLine();
			testMethods.AddLine(1, "[Fact]");
			testMethods.AddLine(1, $"public async Task {readMethod.Name}_SortingSuccess()");
			testMethods.AddLine(1, "{");
			testMethods.AddLine(2, "// Arrange");

			AddSampleSelection(testMethods, entityListExpr, sampleVarName, expectedFilterProps);
			var filterVars = AddFilterValueDeclarations(testMethods, filterPropsToAssign, sampleVarName, false);
			testMethods.AddLine(2, $"var request = new {readMethod.Name}Req();");
			if (readMethod.InclPaging)
			{
				testMethods.AddLine(2, "request.PageSize = 0;");
				testMethods.AddLine(2, "request.PageOffset = 0;");
				testMethods.AddLine(2, "request.RecalcRowCount = true;");
				testMethods.AddLine(2, "request.GetRowCountOnly = false;");
			}
			AddRequestAssignments(testMethods, filterVars);
			testMethods.AddLine(2, $"request.SortBy = {entity.Name}.PropNames.{sortProp.Name};");

			var predicate = BuildExpectedPredicate(expectedFilterProps, filterVars);
			var baseListExpr = string.IsNullOrWhiteSpace(predicate)
				? $"{entityListExpr}.ToList()"
				: $"{entityListExpr}.Where({predicate}).ToList()";
			testMethods.AddLine(2, $"var expectedAsc = {baseListExpr}.OrderBy(x => x.{sortProp.Name}).ToList();");
			testMethods.AddLine(2, $"var expectedDesc = {baseListExpr}.OrderByDescending(x => x.{sortProp.Name}).ToList();");

			testMethods.AddLine();
			testMethods.AddLine(2, "// Act");
			var serviceVarName = $"_{entity.Name.ToCamelCase()}Service";
			testMethods.AddLine(2, "request.SortDesc = false;");
			testMethods.AddLine(2, $"var ascResult = await {serviceVarName}.{readMethod.Name}(request);");
			testMethods.AddLine(2, "request.SortDesc = true;");
			testMethods.AddLine(2, $"var descResult = await {serviceVarName}.{readMethod.Name}(request);");

			testMethods.AddLine();
			testMethods.AddLine(2, "// Assert");
			if (readMethod.InclPaging)
			{
				testMethods.AddLine(2, "Assert.Equal(expectedAsc.Select(x => x.Id), ascResult.Items.Select(x => x.Id));");
				testMethods.AddLine(2, "Assert.Equal(expectedDesc.Select(x => x.Id), descResult.Items.Select(x => x.Id));");
			}
			else
			{
				testMethods.AddLine(2, "Assert.Equal(expectedAsc.Select(x => x.Id), ascResult.Select(x => x.Id));");
				testMethods.AddLine(2, "Assert.Equal(expectedDesc.Select(x => x.Id), descResult.Select(x => x.Id));");
			}
			testMethods.AddLine(1, "}");
		}

		private static string GetEntityListExpression(EntityModel entity)
		{
			return $"_db.{entity.Name}";
		}

		private static PropertyModel GetSortingProperty(EntityModel entity)
		{
			return entity.Properties.FirstOrDefault(p => !p.IsPrimaryKey && p.DataType == DataTypes.String)
				?? entity.Properties.FirstOrDefault(p => !p.IsPrimaryKey && p.DataType == DataTypes.Int32)
				?? entity.Properties.FirstOrDefault(p => !p.IsPrimaryKey && p.DataType == DataTypes.DateTime)
				?? entity.Properties.FirstOrDefault(p => !p.IsPrimaryKey && p.DataType == DataTypes.DateTimeOffset);
		}

		private static string GetSampleVarName(EntityModel entity)
		{
			return $"{entity.Name.ToCamelCase()}Sample";
		}

		private static List<FilterPropertyModel> GetFilterPropsToAssign(ReadMethodModel readMethod, bool includeOptional)
		{
			var filterProps = new List<FilterPropertyModel>();
			filterProps.AddRange(readMethod.FilterProperties.Where(fp => !fp.IsInternal && !fp.IsOptional));
			if (includeOptional)
				filterProps.AddRange(readMethod.FilterProperties.Where(fp => !fp.IsInternal && fp.IsOptional));
			if (includeOptional && readMethod.UseRequest)
				filterProps.AddRange(readMethod.FilterProperties.Where(fp => fp.IsInternal && fp.IsOptional && fp.PropertyModel.DataType != DataTypes.String));
			return filterProps;
		}

		private static List<FilterPropertyModel> GetExpectedFilterProps(ReadMethodModel readMethod, bool includeOptional)
		{
			var filterProps = new List<FilterPropertyModel>();
			filterProps.AddRange(readMethod.FilterProperties.Where(fp => !fp.IsInternal && !fp.IsOptional));
			if (includeOptional)
				filterProps.AddRange(readMethod.FilterProperties.Where(fp => !fp.IsInternal && fp.IsOptional));

			foreach (var internalProp in readMethod.FilterProperties.Where(fp => fp.IsInternal))
			{
				if (internalProp.PropertyModel.DataType == DataTypes.String)
				{
					filterProps.Add(internalProp);
					continue;
				}

				if (!internalProp.IsOptional || includeOptional)
					filterProps.Add(internalProp);
			}

			return filterProps;
		}

		private void AddSampleSelection(List<string> testMethods, string entityListExpr, string sampleVarName, List<FilterPropertyModel> filterProps)
		{
			var predicate = BuildSamplePredicate(filterProps);
			if (string.IsNullOrWhiteSpace(predicate))
				testMethods.AddLine(2, $"var {sampleVarName} = {entityListExpr}.First();");
			else
				testMethods.AddLine(2, $"var {sampleVarName} = {entityListExpr}.First({predicate});");
		}

		private static Dictionary<FilterPropertyModel, string> AddFilterValueDeclarations(List<string> testMethods, IEnumerable<FilterPropertyModel> filterProps, string sampleVarName, bool usePartialMatchShortValue)
		{
			var filterVars = new Dictionary<FilterPropertyModel, string>();

			foreach (var filterProp in filterProps)
			{
				var varName = filterProp.PropertyModel.ArgName;
				var valueExpression = GetFilterValueExpression(filterProp, sampleVarName, usePartialMatchShortValue);
				testMethods.AddLine(2, $"var {varName} = {valueExpression};");
				filterVars[filterProp] = varName;
			}

			return filterVars;
		}

		private static void AddRequestAssignments(List<string> testMethods, Dictionary<FilterPropertyModel, string> filterVars)
		{
			foreach (var filterVar in filterVars)
			{
				testMethods.AddLine(2, $"request.{filterVar.Key.PropertyModel.Name} = {filterVar.Value};");
			}
		}

		private void AddExpectedList(List<string> testMethods, string entityListExpr, IEnumerable<FilterPropertyModel> filterProps, Dictionary<FilterPropertyModel, string> filterVars)
		{
			var predicate = BuildExpectedPredicate(filterProps, filterVars);
			if (string.IsNullOrWhiteSpace(predicate))
				testMethods.AddLine(2, $"var expectedList = {entityListExpr}.ToList();");
			else
				testMethods.AddLine(2, $"var expectedList = {entityListExpr}.Where({predicate}).ToList();");
		}

		private string BuildSamplePredicate(IEnumerable<FilterPropertyModel> filterProps)
		{
			var conditions = new List<string>();
			foreach (var filterProp in filterProps)
			{
				var propName = filterProp.PropertyModel.Name;
				if (filterProp.IsInternal)
				{
					conditions.Add(BuildInternalCondition(filterProp));
					continue;
				}
				if (filterProp.PropertyModel.DataType == DataTypes.String)
				{
					conditions.Add($"!string.IsNullOrWhiteSpace(x.{propName})");
				}
				else if (filterProp.PropertyModel.IsNullable && IsValueType(filterProp.PropertyModel))
				{
					conditions.Add($"x.{propName}.HasValue");
				}
				else if (filterProp.PropertyModel.IsNullable && IsReferenceType(filterProp.PropertyModel))
				{
					conditions.Add($"x.{propName} != null");
				}
			}

			if (!conditions.Any())
				return null;

			return $"x => {string.Join(" && ", conditions)}";
		}

		private static string BuildExpectedPredicate(IEnumerable<FilterPropertyModel> filterProps, Dictionary<FilterPropertyModel, string> filterVars)
		{
			var conditions = new List<string>();
			foreach (var filterProp in filterProps)
			{
				var propName = filterProp.PropertyModel.Name;
				if (filterProp.IsInternal)
				{
					conditions.Add(BuildInternalCondition(filterProp));
					continue;
				}

				if (!filterVars.TryGetValue(filterProp, out var varName))
					continue;

				if (filterProp.PropertyModel.DataType == DataTypes.String)
				{
					if (filterProp.IsPartialMatch)
						conditions.Add($"x.{propName} != null && x.{propName}.Contains({varName})");
					else
						conditions.Add($"x.{propName} == {varName}");
				}
				else
				{
					conditions.Add($"x.{propName} == {varName}");
				}
			}

			if (!conditions.Any())
				return null;

			return $"x => {string.Join(" && ", conditions)}";
		}

		private static string BuildInternalCondition(FilterPropertyModel filterProp)
		{
			var propName = filterProp.PropertyModel.Name;
			if (filterProp.PropertyModel.DataType == DataTypes.String)
				return $"x.{propName} != null && x.{propName}.Contains(\"{filterProp.InternalValue}\")";

			if (!DataTypes.IsPrimitive(filterProp.PropertyModel.DataType))
				return $"x.{propName} == {filterProp.PropertyModel.DataType}.{filterProp.InternalValue}";

			return $"x.{propName} == {filterProp.InternalValue}";
		}

		private static string GetFilterValueExpression(FilterPropertyModel filterProp, string sampleVarName, bool usePartialMatchShortValue)
		{
			var propName = filterProp.PropertyModel.Name;
			if (usePartialMatchShortValue && filterProp.IsPartialMatch && filterProp.PropertyModel.DataType == DataTypes.String)
				return $"{sampleVarName}.{propName}.Substring(0, 1)";
			if (!filterProp.IsOptional && filterProp.PropertyModel.IsNullable && IsValueType(filterProp.PropertyModel))
				return $"{sampleVarName}.{propName}.Value";

			return $"{sampleVarName}.{propName}";
		}

		private static string BuildMethodCallArgs(ReadMethodModel readMethod, Dictionary<FilterPropertyModel, string> filterVars, bool includeOptional)
		{
			var args = new List<string>();
			var requiredFilters = readMethod.FilterProperties.Where(fp => !fp.IsInternal && !fp.IsOptional).ToList();
			var optionalFilters = readMethod.FilterProperties.Where(fp => !fp.IsInternal && fp.IsOptional).ToList();

			foreach (var filterProp in requiredFilters)
				args.Add(filterVars[filterProp]);

			foreach (var filterProp in optionalFilters)
			{
				if (includeOptional && filterVars.TryGetValue(filterProp, out var varName))
					args.Add(varName);
				else
					args.Add("null");
			}

			return string.Join(", ", args);
		}

		private static string GetInvalidValueExpression(FilterPropertyModel filterProp, string sampleVarName)
		{
			switch (filterProp.PropertyModel.DataType)
			{
				case "String":
					return "\"__invalid__\"";
				case "Guid":
					return "Guid.NewGuid()";
				case "DateTime":
					return "DateTime.MaxValue";
				case "DateTimeOffset":
					return "DateTimeOffset.MaxValue";
				case "TimeSpan":
					return "TimeSpan.MaxValue";
				case "Int32":
					return "int.MaxValue";
				case "Int16":
					return "short.MaxValue";
				case "Int64":
					return "long.MaxValue";
				case "UInt16":
					return "ushort.MaxValue";
				case "UInt32":
					return "uint.MaxValue";
				case "UInt64":
					return "ulong.MaxValue";
				case "Byte":
					return "byte.MaxValue";
				case "SByte":
					return "sbyte.MaxValue";
				case "Decimal":
					return "decimal.MaxValue";
				case "Double":
					return "double.MaxValue";
				case "Single":
					return "float.MaxValue";
				case "Boolean":
					return $"!{sampleVarName}.{filterProp.PropertyModel.Name}";
				case "ByteArray":
					return "new byte[0]";
				case "StringList":
					return "new System.Collections.Generic.List<string>()";
				case "Object":
					return "new object()";
				default:
					return $"({filterProp.PropertyModel.DataType})int.MaxValue";
			}
		}

		private static bool IsValueType(PropertyModel prop)
		{
			return !IsReferenceType(prop);
		}

		private static bool IsReferenceType(PropertyModel prop)
		{
			return prop.DataType == DataTypes.String
				|| prop.DataType == DataTypes.StringList
				|| prop.DataType == DataTypes.ByteArray
				|| prop.DataType == DataTypes.Object;
		}
	}
}
