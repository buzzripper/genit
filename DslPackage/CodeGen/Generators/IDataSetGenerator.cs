using Dyvenix.GenIt.DslPackage.CodeGen.Misc;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dyvenix.GenIt.DslPackage.CodeGen.Generators
{
	internal class IDataSetGenerator
	{
		private readonly ModelRoot _modelRoot;
		private readonly List<EntityModel> _entities;
		private readonly string _entitiesNamespace;
		private readonly string _namespace;
		private readonly bool _inclHeader;

		internal IDataSetGenerator(ModelRoot modelRoot)
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
			fileContents.AddLine(0, "public interface IDataSet");
			fileContents.AddLine(0, "{");
			fileContents.AddLine(1, "DataSetType DataSetType { get; }");
			fileContents.AddLine();

			foreach (var entity in _entities)
				fileContents.AddLine(1, $"List<{entity.Name}> {entity.Name}List {{ get; set; }}");
			fileContents.AddLine(0, "}");

			var filepath = Path.Combine(FileHelper.GetAbsolutePath(_modelRoot.IntTestsOutputFolder), "DataSets", "IDataSet.cs");
			FileHelper.SaveFile(filepath, fileContents.AsString());
		}
	}
}
