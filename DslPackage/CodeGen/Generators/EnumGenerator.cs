using Dyvenix.GenIt.DslPackage.CodeGen.Misc;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dyvenix.GenIt.DslPackage.CodeGen.Generators
{
	internal class EnumGenerator
	{
		private readonly List<EnumModel> _enums;
		private readonly string _enumsNamespace;
		private readonly string _outputFolderpath;
		private readonly bool _inclHeader;

		internal EnumGenerator(List<EnumModel> enums, string enumsNamespace, string outputFolderpath, bool enabled, bool inclHeader)
		{
			_enums = enums;
			_enumsNamespace = enumsNamespace;
			_outputFolderpath = FileHelper.GetAbsolutePath(outputFolderpath);
			_inclHeader = inclHeader;

			this.Enabled = enabled;
		}

		internal void Validate(List<string> errors)
		{
			if (string.IsNullOrEmpty(_enumsNamespace))
				errors.Add("EnumsNamespace is not set. Please set it in the ModelRoot properties.");

			if (string.IsNullOrEmpty(_outputFolderpath))
				errors.Add("EnumsOutputFolder is not set. Please set it in the ModelRoot properties.");
			else if (!Directory.Exists(_outputFolderpath))
				errors.Add("EnumsOutputFolder does not exist. Please select a valid folder.");
		}

		#region Properties

		internal bool Enabled { get; private set; }

		#endregion

		internal void GenerateCode()
		{
			foreach (var enm in _enums.Where(e => e.GenerateCode))
				GenerateEnum(enm);
		}

		private void GenerateEnum(EnumModel enumModel)
		{
			var fileContent = new List<string>();

			if (_inclHeader)
				fileContent.Add(CodeGenUtils.FileHeader);

			// Namespace 		 
			fileContent.AddLine();
			fileContent.AddLine(0, $"namespace {_enumsNamespace};");

			// Declaration
			fileContent.AddLine();
			fileContent.AddLine(0, $"public enum {enumModel.Name}");
			fileContent.AddLine(0, "{");

			// Members
			for (var i = 0; i < enumModel.Members?.Count; i++)
			{
				var comma = i == enumModel.Members.Count - 1 ? string.Empty : ",";
				fileContent.AddLine(1, $"{enumModel.Members[i].Name}{comma}");
			}

			fileContent.AddLine(0, "}");

			var outputFilepath = Path.Combine(_outputFolderpath, $"{enumModel.Name}.cs");
			FileHelper.SaveFile(outputFilepath, fileContent.AsString());

			OutputHelper.Write($"Completed code gen for entity: {enumModel.Name}");
		}
	}
}