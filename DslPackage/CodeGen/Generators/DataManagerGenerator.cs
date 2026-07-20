using Dyvenix.GenIt.DslPackage.CodeGen.Misc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dyvenix.GenIt.DslPackage.CodeGen.Generators
{
	internal class DataManagerGenerator
	{
		private readonly ModelRoot _modelRoot;
		private readonly List<EntityModel> _entitiesSortedForDeletion;
		private readonly List<EntityModel> _entitiesSortedForCreation;
		private readonly string _entitiesNamespace;
		private readonly string _namespace;
		private readonly bool _inclHeader;

		internal DataManagerGenerator(ModelRoot modelRoot)
		{
			_modelRoot = modelRoot;

			var entities = modelRoot.Types.OfType<EntityModel>().Where(e => e.GenerateCode).ToList();
			_entitiesSortedForDeletion = CodeGenUtils.SortEntitiesForDeletion(entities);
			_entitiesSortedForCreation = _entitiesSortedForDeletion.AsEnumerable().Reverse().ToList();

			_entitiesNamespace = modelRoot.EntitiesNamespace;
			_namespace = $"{modelRoot.IntTestsNamespace}.Data";
			_inclHeader = modelRoot.InclHeader;
		}

		public void GenerateCode()
		{
			var fileContents = new List<string>();

			if (_inclHeader)
				fileContents.Add(CodeGenUtils.FileHeader);
			fileContents.AddLine(0, CodeGenUtils.NullableEnableDirective);

			// Usings
			fileContents.AddLine(0, $"using System.Text.Json;");
			fileContents.AddLine(0, $"using Microsoft.EntityFrameworkCore;");
			fileContents.AddLine(0, $"using {_modelRoot.DbContextNamespace};");
			fileContents.AddLine(0, $"using {_modelRoot.IntTestsNamespace}.DataSets;");

			// Namespace
			fileContents.AddLine();
			fileContents.AddLine(0, $"namespace {_modelRoot.IntTestsNamespace}.Data;");

			// Interface declaration
			fileContents.AddLine();
			fileContents.AddLine(0, "public interface IDataManager : IDisposable");
			fileContents.AddLine(0, "{");
			fileContents.AddLine(1, "Dictionary<string, TestDataSet> DataSets { get; }");
			fileContents.AddLine(1, "Task<TestDataSet> Reset(string dataSetName);");
			fileContents.AddLine(1, "Task Initialize();");
			fileContents.AddLine(0, "}");

			// Class declaration
			fileContents.AddLine();
			fileContents.AddLine(0, "public class DataManager : IDataManager");
			fileContents.AddLine(0, "{");

			// Fields
			fileContents.AddLine(1, $"private readonly {_modelRoot.DbContextName} _db;");

			// Constructor / Dispose
			fileContents.AddLine();
			fileContents.AddLine(1, $"public DataManager({_modelRoot.DbContextName} db)");
			fileContents.AddLine(1, "{");
			fileContents.AddLine(2, "_db = db;");
			fileContents.AddLine(1, "}");
			fileContents.AddLine();
			fileContents.AddLine(1, "public void Dispose()");
			fileContents.AddLine(1, "{");
			fileContents.AddLine(2, "_db?.Dispose();");
			fileContents.AddLine(1, "}");

			// Properties
			fileContents.AddLine();
			fileContents.AddLine(1, "public Dictionary<string, TestDataSet> DataSets { get; } = [];");

			// Load DataSets from json files
			fileContents.AddLine();
			fileContents.AddLine(1, "public async Task Initialize()");
			fileContents.AddLine(1, "{");
			fileContents.AddLine(2, "// Each json file is a dataset. The \"Default\" can optionally hold common data that should be included in all datasets.");
			fileContents.AddLine(2, "var testDataRootDir = Path.Combine(AppContext.BaseDirectory, \"TestData\");");
			fileContents.AddLine(2, "var defaultDataDir = Path.Combine(testDataRootDir, \"Default\");");
			fileContents.AddLine();
			fileContents.AddLine(2, "var jsonFilepaths = Directory.GetFiles(testDataRootDir, \"*.json\", SearchOption.TopDirectoryOnly);");
			fileContents.AddLine(2, "foreach (var jsonFilepath in jsonFilepaths)");
			fileContents.AddLine(2, "{");
			fileContents.AddLine(3, "var dataSetName = Path.GetFileNameWithoutExtension(jsonFilepath);");
			fileContents.AddLine(3, "var dataSet = await LoadDataSet(jsonFilepath);");
			fileContents.AddLine(3, "DataSets.Add(dataSetName, dataSet);");
			fileContents.AddLine(2, "}");
			fileContents.AddLine(1, "}");

			// LoadDataSet()
			fileContents.AddLine();
			fileContents.AddLine(1, "public async Task<TestDataSet> LoadDataSet(string jsonFilepath)");
			fileContents.AddLine(1, "{");
			fileContents.AddLine(2, "try");
			fileContents.AddLine(2, "{");
			fileContents.AddLine(3, "using var reader = new StreamReader(jsonFilepath);");
			fileContents.AddLine(3, "var json = await reader.ReadToEndAsync();");
			fileContents.AddLine(3, "var dataSet = JsonSerializer.Deserialize<TestDataSet>(json);");
			fileContents.AddLine(3, "if (dataSet == null)");
			fileContents.AddLine(4, "throw new Exception($\"Error attempting to load dataset file {jsonFilepath}. Returned null.\");");
			fileContents.AddLine(3, "return dataSet;");
			fileContents.AddLine(2, "}");
			fileContents.AddLine(2, "catch (Exception ex)");
			fileContents.AddLine(2, "{");
			fileContents.AddLine(3, "throw new Exception($\"Error loading dataset from file '{jsonFilepath}': {ex.Message}\", ex);");
			fileContents.AddLine(2, "}");
			fileContents.AddLine(1, "}");

			// Reset()
			fileContents.AddLine();
			fileContents.AddLine(1, "public async Task<TestDataSet> Reset(string dataSetName)");
			fileContents.AddLine(1, " {");
			fileContents.AddLine(2, "var dataSet = this.DataSets[dataSetName];");
			fileContents.AddLine();
			fileContents.AddLine(2, "await DeleteAllData();");
			fileContents.AddLine(2, "await InsertAllData(dataSet);");
			fileContents.AddLine();
			fileContents.AddLine(2, "return dataSet;");
			fileContents.AddLine(1, "}");

			// DeleteAllData()
			fileContents.AddLine();
			fileContents.AddLine(1, "private async Task DeleteAllData()");
			fileContents.AddLine(1, "{");

			foreach (var entity in _entitiesSortedForDeletion)
				fileContents.AddLine(2, $"await _db.{entity.Name}.ExecuteDeleteAsync();");

			fileContents.AddLine(1, "}");

			// InsertAllData()
			fileContents.AddLine();
			fileContents.AddLine(1, "private async Task InsertAllData(TestDataSet dataSet)");
			fileContents.AddLine(1, "{");
			foreach (var entity in _entitiesSortedForCreation)
			{
				fileContents.AddLine(2, $"await _db.{entity.Name}.AddRangeAsync(dataSet.{entity.Name}List);");
				fileContents.AddLine(2, "await _db.SaveChangesAsync();");
			}
			fileContents.AddLine(1, "}");

			fileContents.AddLine(0, "}");

			var filepath = Path.Combine(FileHelper.GetAbsolutePath(_modelRoot.IntTestsRootFolder), "Data", "DataManager.cs");
			FileHelper.SaveFile(filepath, fileContents.AsString());
		}
	}
}