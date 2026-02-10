using Dyvenix.GenIt.DslPackage.CodeGen.Misc;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dyvenix.GenIt.DslPackage.CodeGen.Generators
{
	internal class DataSetGenerator
	{
		private readonly ModelRoot _modelRoot;
		private readonly List<EntityModel> _entities;
		private readonly string _entitiesNamespace;
		private readonly string _namespace;
		private readonly bool _inclHeader;

		internal DataSetGenerator(ModelRoot modelRoot)
		{
			_modelRoot = modelRoot;
			_entities = modelRoot.Types.OfType<EntityModel>().Where(e => e.GenerateCode).ToList();
			_entitiesNamespace = modelRoot.EntitiesNamespace;
			_namespace = $"{modelRoot.IntTestsNamespace}.DataSets";
			_inclHeader = modelRoot.InclHeader;
		}

		public void GenerateCode()
		{
			var fileContents = new List<string>();

			if (_inclHeader)
				fileContents.Add(CodeGenUtils.FileHeader);

			// Usings
			fileContents.AddLine(0, $"using {_entitiesNamespace};");
			fileContents.AddLine(0, $"using {_modelRoot.IntTestsNamespace}.Data;");

			// Namespace
			fileContents.AddLine();
			fileContents.AddLine(0, $"namespace {_namespace};");

			// Class declaration
			fileContents.AddLine();
			fileContents.AddLine(0, "public class TestDataSet");
			fileContents.AddLine(0, "{");

			foreach (var entity in _entities)
				fileContents.AddLine(1, $"public List<{entity.Name}> {entity.Name}List {{ get; set; }} = null!;");
			fileContents.AddLine(0, "}");

			var filepath = Path.Combine(FileHelper.GetAbsolutePath(_modelRoot.IntTestsOutputFolder), "Data", "TestDataSet.cs");

			FileHelper.SaveFile(filepath, fileContents.AsString());
		}
	}
}
