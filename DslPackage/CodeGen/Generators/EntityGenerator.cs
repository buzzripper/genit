using Dyvenix.GenIt.DslPackage.CodeGen.Misc;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dyvenix.GenIt.DslPackage.CodeGen.Generators
{
	internal class EntityGenerator
	{
		private readonly ModelRoot _modelRoot;
		private readonly Dictionary<string, ModuleModel> _modules = new Dictionary<string, ModuleModel>();
		private readonly List<EntityModel> _entities;
		private readonly string _entitiesNamespace;
		private readonly string _outputFolderpath;
		private readonly bool _inclHeader;

		internal EntityGenerator(ModelRoot modelRoot)
		{
			// Convenience vars
			_modelRoot = modelRoot;
			foreach (var module in _modelRoot.Types.OfType<ModuleModel>().ToList())
				if (!_modules.ContainsKey(module.Name))
					_modules.Add(module.Name, module);
			_entities = modelRoot.Types.OfType<EntityModel>().ToList();
			_entitiesNamespace = modelRoot.EntitiesNamespace;
			_outputFolderpath = FileHelper.GetAbsolutePath(modelRoot.EntitiesOutputFolder);
			_inclHeader = modelRoot.InclHeader;
			this.Enabled = modelRoot.EntitiesEnabled;
		}

		internal void Validate(List<string> errors)
		{
			if (_entities == null || _entities.Count == 0)
				errors.Add("No entities found in the model. Add some entities first.");

			if (string.IsNullOrEmpty(_entitiesNamespace))
				errors.Add("EntitiesNamespace is not set. Please set it in the ModelRoot properties.");

			if (string.IsNullOrEmpty(_outputFolderpath))
				errors.Add("EntitiesOutputFolder is not set. Please set it in the ModelRoot properties.");
			else if (!Directory.Exists(_outputFolderpath))
				errors.Add("EntitiesOutputFolder does not exist. Please select a valid folder.");

			foreach (var entity in _entities)
			{
				if (string.IsNullOrEmpty(entity.Module))
					errors.Add($"Entity '{entity.Name} does is not assigned to a module. Select one.");
			}
		}

		#region Properties

		internal bool Enabled { get; private set; }

		#endregion

		internal void GenerateCode()
		{
			foreach (var entity in _entities.Where(e => e.GenerateCode))
				GenerateEntity(entity);
		}

		private void GenerateEntity(EntityModel entity)
		{
			var module = _modules[entity.Module];

			var fileContent = new List<string>();

			if (_inclHeader)
				fileContent.Add(CodeGenUtils.FileHeader);

			// Usings
			if (entity.UsingsList?.Count > 0)
				entity.UsingsList.ForEach(u => fileContent.AddLine(0, $"using {u};"));
			// If any properties are enums, include the namespace
			if (entity.Properties.Any(p => !DataTypes.IsEnumType(p.DataType)))
				fileContent.AddLine(0, $"using {_modelRoot.EnumsNamespace};");
			// If it's an auditable entity, include the namespace for the IAuditable interface
			if (entity.Auditable)
				fileContent.AddLine(0, $"using {_modelRoot.CommonNamespace}.Shared.Contracts;");

			// Namespace 		
			fileContent.AddLine();
			fileContent.AddLine(0, $"namespace {_entitiesNamespace};");

			// Declaration
			fileContent.AddLine();
			var auditableInterface = entity.Auditable ? " : IAuditable" : null;
			fileContent.AddLine(0, $"public partial class {entity.Name}{auditableInterface}");
			fileContent.AddLine(0, "{");

			// PK
			fileContent.AddLine(1, "// PK");
			foreach (var property in entity.Properties.Where(p => p.IsPrimaryKey))
				this.GenerateProperty(property, fileContent);

			// FK
			if (entity.Properties.Any(p => p.IsForeignKey))
			{
				fileContent.AddLine();
				fileContent.AddLine(1, "// FKs");
				foreach (var property in entity.Properties.Where(p => p.IsForeignKey))
					this.GenerateProperty(property, fileContent);
			}

			// RowVersion
			if (entity.Properties.Any(p => p.IsRowVersion))
			{
				fileContent.AddLine();
				fileContent.AddLine(1, "// Rowversion");
				foreach (var property in entity.Properties.Where(p => p.IsRowVersion))
					this.GenerateProperty(property, fileContent);
			}

			// Properties
			if (entity.Properties.Count > 0)
			{
				fileContent.AddLine();
				fileContent.AddLine(1, $"// Properties");
				foreach (var property in entity.Properties.Where(p => !p.IsPrimaryKey && !p.IsForeignKey & !p.IsRowVersion).OrderBy(p => p.DisplayOrder))
					GenerateProperty(property, fileContent);
			}

			// Navigation Properties
			if (entity.NavigationProperties.Count > 0)
			{
				fileContent.AddLine();
				fileContent.AddLine(1, $"// Navigation Properties");
				foreach (var navProperty in entity.NavigationProperties)
				{
					var dataType = navProperty.IsCollection ? $"List<{navProperty.TargetEntityName}>" : navProperty.TargetEntityName;
					var initValue = navProperty.IsCollection ? $" = new();" : string.Empty;
					fileContent.AddLine(1, $"public {dataType} {navProperty.Name} {{ get; set; }}{initValue}");
				}
			}

			// Property names
			fileContent.AddLine();
			fileContent.AddLine(1, "#region PropNames");
			fileContent.AddLine();
			fileContent.AddLine(1, "public static class PropNames");
			fileContent.AddLine(1, "{");
			foreach (var prop in entity.Properties)
				fileContent.AddLine(2, $"public const string {prop.Name} = \"{prop.Name}\";");
			foreach (var navProp in entity.NavigationProperties)
				fileContent.AddLine(2, $"public const string {navProp.Name} = \"{navProp.Name}\";");
			fileContent.AddLine(1, "}");

			fileContent.AddLine();
			fileContent.AddLine(1, "#endregion");
			fileContent.AddLine(0, "}");

			var outputFilepath = Path.Combine(_outputFolderpath, $"{entity.Name}.g.cs");
			FileHelper.SaveFile(outputFilepath, fileContent.AsString());

			OutputHelper.Write($"Completed code gen for entity: {entity.Name}");
		}

		private void GenerateProperty(PropertyModel prop, List<string> fileContent)
		{
			if (prop.Attributes.Any())
				foreach (var attr in prop.AttributesList)
					fileContent.AddLine(1, $"[{attr}]");

			string nullSuffix = prop.IsNullable ? "?" : null;
			string nullInit = prop.RequiresInit ? " = null!;" : null;
			fileContent.AddLine(1, $"public {prop.CSType}{nullSuffix} {prop.Name} {{ get; set; }}{nullInit}");
		}
	}
}