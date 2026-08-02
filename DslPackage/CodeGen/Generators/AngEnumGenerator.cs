using Dyvenix.GenIt.DslPackage.CodeGen.Misc;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dyvenix.GenIt.DslPackage.CodeGen.Generators
{
	internal class AngEnumGenerator
	{
		private readonly ModelRoot _modelRoot;
		private readonly List<EnumModel> _enums;
		private readonly Dictionary<string, ModuleModel> _modules = new Dictionary<string, ModuleModel>();

		internal AngEnumGenerator(ModelRoot modelRoot, Dictionary<string, ModuleModel> modules)
		{
			// Convenience vars
			_modelRoot = modelRoot;
			_enums = modelRoot.Types.OfType<EnumModel>().ToList();
			_modules = modules;
		}

		internal void GenerateCode()
		{
			if (!_enums.Any())
				return;

			var module = _modules.Values.First();
			var enumFolderPath = Path.Combine(PackageUtils.SolutionRootPath, module.NgServiceOutputFolder, "enum")?.ToLower();

			foreach (var enumModel in _enums)
				GenerateEnum(module, enumModel, enumFolderPath);


			OutputHelper.Write($"Completed code gen for angular enums.");

			// Write index.ts file for enums
			var newLines = new List<string>();

			foreach (var enumName in _enums.Select(e => e.Name))
				newLines.AddLine(0, $"export * from './{enumName.ToLower()}.enum.g';");

			var indexFilepath = Path.Combine(enumFolderPath, "index.ts");
			FileHelper.PreserveCustomContentAndWriteFile(newLines, indexFilepath);
			OutputHelper.Write($"Completed code gen for angular enum index file for module: {module.Name}");
		}

		private void GenerateEnum(ModuleModel module, EnumModel enumModel, string enumFolderPath)
		{
			var fileContent = new List<string>();

			if (_modelRoot.InclHeader)
				fileContent.Add(CodeGenUtils.FileHeader);

			fileContent.AddLine();
			fileContent.AddLine(0, $"export const {enumModel.Name} = {{");
			foreach (var member in enumModel.Members)
				fileContent.AddLine(1, $"{member.Name}: '{member.Name}',");
			fileContent.AddLine(0, $"}} as const");

			fileContent.AddLine();
			fileContent.AddLine(0, $"export type {enumModel.Name} = typeof {enumModel.Name}[keyof typeof {enumModel.Name}];");

			Directory.CreateDirectory(enumFolderPath);  // Ensure output dir exists
			var outputFilepath = Path.Combine(enumFolderPath, $"{enumModel.Name.ToLower()}.enum.g.ts");
			FileHelper.SaveFile(outputFilepath, fileContent.AsString());
		}


	}
}
