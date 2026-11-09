using Dyvenix.GenIt.DslPackage.CodeGen.Generators;
using Dyvenix.GenIt.DslPackage.CodeGen.Misc;
using Microsoft.VisualStudio.Shell;
using System.Collections.Generic;
using System.Linq;

namespace Dyvenix.GenIt.DslPackage.CodeGen
{
	internal class GenItModel
	{
		private readonly ModelRoot _modelRoot;
		private readonly EntityGenerator _entityGenerator;
		private readonly EnumGenerator _enumGenerator;
		private readonly DbContextGenerator _dbContextGenerator;

		internal GenItModel(ModelRoot modelRoot)
		{
			_modelRoot = modelRoot;

			_entityGenerator = new EntityGenerator(modelRoot);
			_enumGenerator = new EnumGenerator(modelRoot.Types.OfType<EnumModel>().ToList(), modelRoot.EnumsNamespace, modelRoot.EnumsOutputFolder, modelRoot.EnumsEnabled, modelRoot.InclHeader);
			_dbContextGenerator = new DbContextGenerator(modelRoot);
		}

		internal bool Validate(out List<string> errors)
		{
			errors = new List<string>();

			if (_entityGenerator.Enabled)
				_entityGenerator.Validate(errors);
			else
				OutputHelper.Write("Entity generation is disabled, skipping validation.");

			if (_enumGenerator.Enabled)
				_enumGenerator.Validate(errors);
			else
				OutputHelper.Write("Enum  generation is disabled, skipping validation.");

			if (_modelRoot.DbContextEnabled)
				_dbContextGenerator.Validate(errors);
			else
				OutputHelper.Write("DbContext  generation is disabled, skipping validation.");

			return errors.Count == 0;
		}

		internal void GenerateCode()
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			if (_entityGenerator.Enabled)
				_entityGenerator.GenerateCode();

			if (_enumGenerator.Enabled)
				_enumGenerator.GenerateCode();

			if (_modelRoot.DbContextEnabled)
				_dbContextGenerator.GenerateCode();
		}
	}
}
