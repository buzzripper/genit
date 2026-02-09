using Dyvenix.GenIt.DslPackage.CodeGen.Misc;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dyvenix.GenIt.DslPackage.CodeGen.Generators
{
	internal class DataManagerGenerator
	{
		private readonly ModelRoot _modelRoot;
		private readonly List<EntityModel> _entities;
		private readonly string _entitiesNamespace;
		private readonly string _namespace;
		private readonly bool _inclHeader;

		internal DataManagerGenerator(ModelRoot modelRoot)
		{
			_modelRoot = modelRoot;
			_entities = modelRoot.Types.OfType<EntityModel>().Where(e => e.GenerateCode).ToList();
			_entitiesNamespace = modelRoot.EntitiesNamespace;
			_namespace = $"{modelRoot.IntTestsNamespace}.Data";
			_inclHeader = modelRoot.InclHeader;
		}

		public void GenerateCode()
		{
			var fileContents = new List<string>();

			if (_inclHeader)
				fileContents.Add(CodeGenUtils.FileHeader);

			// Usings
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
			fileContents.AddLine(1, "Dictionary<DataSetType, IDataSet> DataSets { get; }");
			fileContents.AddLine(1, "Task<IDataSet> Reset(DataSetType dataSetType);");
			fileContents.AddLine(0, "}");

			// Class declaration
			fileContents.AddLine();
			fileContents.AddLine(0, "public class DataManager : IDataManager");
			fileContents.AddLine(0, "{");

			// Fields
			fileContents.AddLine(1, $"private readonly {_modelRoot.DbContextName} _db;");
			fileContents.AddLine();

			// Constructor / Dispose
			fileContents.AddLine(1, $"public DataManager({_modelRoot.DbContextName} db)");
			fileContents.AddLine(1, "{");
			fileContents.AddLine(2, "_db = db;");
			fileContents.AddLine(2, "DataSets.Add(DataSetType.Default, new DefaultDataSet());");
			fileContents.AddLine(1, "}");
			fileContents.AddLine();
			fileContents.AddLine(1, "public void Dispose()");
			fileContents.AddLine(1, "{");
			fileContents.AddLine(2, "_db?.Dispose();");
			fileContents.AddLine(1, "}");
			fileContents.AddLine();

			// Properties
			fileContents.AddLine(1, "public Dictionary<DataSetType, IDataSet> DataSets { get; } = [];");
			fileContents.AddLine();

			// Reset()
			fileContents.AddLine(1, "public async Task<IDataSet> Reset(DataSetType dataSetType)");
			fileContents.AddLine(1, " {");
			fileContents.AddLine(2, "var dataSet = this.DataSets[dataSetType];");
			fileContents.AddLine();
			fileContents.AddLine(2, "await DeleteAllData();");
			fileContents.AddLine(2, "await InsertAllData(dataSet);");
			fileContents.AddLine();
			fileContents.AddLine(2, "return dataSet;");
			fileContents.AddLine(1, "}");
			fileContents.AddLine();

			// DeleteAllData()
			fileContents.AddLine(1, "private async Task DeleteAllData()");
			fileContents.AddLine(1, "{");
			foreach (var entity in _entities)
				fileContents.AddLine(2, $"await _db.{entity.Name}.ExecuteDeleteAsync();");
			fileContents.AddLine();
			fileContents.AddLine(2, "await _db.SaveChangesAsync();");
			fileContents.AddLine(1, "}");
			fileContents.AddLine();

			// InsertAllData()
			fileContents.AddLine(1, "private async Task InsertAllData(IDataSet dataSet)");
			fileContents.AddLine(1, "{");
			foreach (var entity in _entities)
			{
				fileContents.AddLine(2, $"await _db.{entity.Name}.AddRangeAsync(dataSet.{entity.Name}List);");
				fileContents.AddLine(2, "await _db.SaveChangesAsync();");
			}
			fileContents.AddLine(1, "}");
			fileContents.AddLine(0, "}");

			var filepath = Path.Combine(FileHelper.GetAbsolutePath(_modelRoot.IntTestsOutputFolder), "Data", "DataManager.cs");
			FileHelper.SaveFile(filepath, fileContents.AsString());
		}
	}
}