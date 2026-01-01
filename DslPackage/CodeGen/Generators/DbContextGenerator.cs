using Dyvenix.GenIt.DslPackage.CodeGen.Misc;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dyvenix.GenIt.DslPackage.CodeGen.Generators
{
	internal class DbContextGenerator
	{
		private readonly List<EntityModel> _entities;
		private readonly string _dbContextNamespace;
		private readonly List<string> _dbContextUsings;
		private readonly string _entitiesNamespace;
		private readonly string _outputFolderpath;
		private readonly bool _inclHeader;

		internal DbContextGenerator(List<EntityModel> entities, string dbContextNamespace, string entitiesNamespace, string outputFolderpath, bool enabled, bool inclHeader, List<string> dbContextUsings)
		{
			_entities = entities;
			_dbContextNamespace = dbContextNamespace;
			_dbContextUsings = dbContextUsings;
			_entitiesNamespace = entitiesNamespace;
			_outputFolderpath = FileHelper.GetAbsolutePath(outputFolderpath);
			_inclHeader = inclHeader;

			this.Enabled = enabled;
		}

		internal void Validate(List<string> errors)
		{
			if (_entities == null || _entities.Count == 0)
				errors.Add("No entities found in the model. Add some entities first.");

			if (string.IsNullOrEmpty(_dbContextNamespace))
				errors.Add("DbContextNamespace is not set. Please set it in the ModelRoot properties.");

			if (string.IsNullOrEmpty(_entitiesNamespace))
				errors.Add("EntitiesNamespace is not set. Please set it in the ModelRoot properties.");

			if (string.IsNullOrEmpty(_outputFolderpath))
				errors.Add("DbContextOutputFolder is not set. Please set it in the ModelRoot properties.");
			else if (!Directory.Exists(_outputFolderpath))
				errors.Add("DbContextOutputFolder does not exist. Please select a valid folder.");
		}

		#region Properties

		internal bool Enabled { get; private set; }

		#endregion

		internal void GenerateCode()
		{
			var fileContent = new List<string>();

			if (_inclHeader)
				fileContent.Add(CodeGenUtils.FileHeader);

			// Usings
			fileContent.AddLine(0, "using Microsoft.EntityFrameworkCore;");
			foreach (var u in _dbContextUsings)
				fileContent.AddLine(0, $"using {u};");
			fileContent.AddLine(0, $"using {_entitiesNamespace};");

			// Namespace 		
			fileContent.AddLine();
			fileContent.AddLine(0, $"namespace {_dbContextNamespace};");

			// Declaration
			fileContent.AddLine();
			fileContent.AddLine(0, "public partial class Db : DbContext");
			fileContent.AddLine(0, "{");
			fileContent.AddLine(1, "public Db(DbContextOptions<Db> options)");
			fileContent.AddLine(2, ": base(options)");
			fileContent.AddLine(1, "{");
			fileContent.AddLine(1, "}");
			fileContent.AddLine();
			fileContent.AddLine(1, "# region Properties");
			fileContent.AddLine();

			foreach (var entity in _entities.Where(e => e.GenerateCode))
				fileContent.AddLine(1, $"public DbSet<{entity.Name}> {entity.Name} {{ get; set; }}");

			fileContent.AddLine();
			fileContent.AddLine(1, "# endregion");
			fileContent.AddLine();
			fileContent.AddLine(1, "protected override void OnModelCreating(ModelBuilder modelBuilder)");
			fileContent.AddLine(1, "{");
			fileContent.AddLine(2, "base.OnModelCreating(modelBuilder);");

			foreach (var entity in _entities.Where(e => e.GenerateCode))
			{
				fileContent.AddLine();
				fileContent.AddLine(2, $"#region {entity.Name}");
				fileContent.AddLine();

				fileContent.AddLine(2, $"modelBuilder.Entity<{entity.Name}>(entity =>");
				fileContent.AddLine(2, "{");
				fileContent.AddLine(3, $"entity.ToTable(\"{entity.Name}\");");

				// PK
				foreach (var prop in entity.Properties.Where(p => p.IsPrimaryKey))
					fileContent.AddLine(3, $"entity.HasKey(e => e.{prop.Name});");

				// RowVersion
				if (entity.InclRowVersion)
					fileContent.AddLine(3, $"entity.Property(e => e.RowVersion).IsRowVersion();");

				// FKs
				foreach (var prop in entity.Properties.Where(p => p.IsForeignKey))
				{
					var line = $"entity.Property(e => e.{prop.Name})";
					if (!prop.IsNullable)
						line += ".IsRequired()";
					line += ";";
					fileContent.AddLine(3, line);
				}

				// Properties
				foreach (var prop in entity.Properties.Where(p => !p.IsPrimaryKey && !p.IsForeignKey && !p.IsRowVersion))
				{
					var line = $"entity.Property(e => e.{prop.Name})";
					if (!prop.IsNullable)
						line += ".IsRequired()";
					if (prop.DataType == DataType.String && prop.Length > 0)
						line += $".HasMaxLength({prop.Length})";
					line += ";";
					fileContent.AddLine(3, line);
				}

				// Indexes
				fileContent.AddLine();
				foreach (var prop in entity.Properties.Where(p => p.IsPrimaryKey || p.IsForeignKey || p.IsIndexed))
				{
					var line = $"entity.HasIndex(e => e.{prop.Name}, \"IX_{entity.Name}_{prop.Name}\")";
					if (!prop.IsIndexUnique)
						line += ".IsUnique()";
					if (!prop.IsIndexClustered)
						line += ".Clustered()";
					line += ";";
					fileContent.AddLine(3, line);
				}

				fileContent.AddLine(2, "});");
				fileContent.AddLine();
				fileContent.AddLine(2, $"#endregion");
			}

			fileContent.AddLine();
			fileContent.AddLine(2, "OnModelCreatingPartial(modelBuilder);");
			fileContent.AddLine(1, "}");

			fileContent.AddLine();
			fileContent.AddLine(1, "partial void OnModelCreatingPartial(ModelBuilder modelBuilder);");

			fileContent.AddLine(0, "}");

			var outputFilepath = Path.Combine(_outputFolderpath, $"Db.cs");
			FileHelper.SaveFile(outputFilepath, fileContent.AsString());
		}

		//private void GenerateEntity(EntityModel entity, List<string> fileContent)
		//{

		//	// Declaration
		//	fileContent.AddLine();
		//	fileContent.AddLine(0, $"public partial class {entity.Name}");
		//	fileContent.AddLine(0, "{");

		//	// PK
		//	fileContent.AddLine(1, "// PK");
		//	foreach (var property in entity.Properties.Where(p => p.IsPrimaryKey))
		//		this.GenerateProperty(property, fileContent);

		//	// FK
		//	if (entity.Properties.Any(p => p.IsForeignKey))
		//	{
		//		fileContent.AddLine();
		//		fileContent.AddLine(1, "// FKs");
		//		foreach (var property in entity.Properties.Where(p => p.IsForeignKey))
		//			this.GenerateProperty(property, fileContent);
		//	}

		//	// RowVersion
		//	if (entity.Properties.Any(p => p.IsRowVersion))
		//	{
		//		fileContent.AddLine();
		//		fileContent.AddLine(1, "// Rowversion");
		//		foreach (var property in entity.Properties.Where(p => p.IsRowVersion))
		//			this.GenerateProperty(property, fileContent);
		//	}

		//	// Properties
		//	if (entity.Properties.Count > 0)
		//	{
		//		fileContent.AddLine();
		//		fileContent.AddLine(1, $"// Properties");
		//		foreach (var property in entity.Properties.Where(p => !p.IsPrimaryKey && !p.IsForeignKey & !p.IsRowVersion))
		//			GenerateProperty(property, fileContent);
		//	}

		//	if (entity.NavigationProperties.Count > 0)
		//	{
		//		fileContent.AddLine();
		//		fileContent.AddLine(1, $"// Navigation Properties");
		//		foreach (var navProperty in entity.NavigationProperties)
		//		{
		//			var dataType = navProperty.IsCollection ? $"List<{navProperty.TargetEntityName}>" : navProperty.TargetEntityName;
		//			fileContent.AddLine(1, $"public {dataType} {navProperty.Name} {{ get; set; }}");
		//		}
		//	}

		//	// Property names
		//	fileContent.AddLine();
		//	fileContent.AddLine(1, "public static class PropNames");
		//	fileContent.AddLine(1, "{");
		//	foreach (var prop in entity.Properties)
		//	{
		//		fileContent.AddLine(2, $"public const string {prop.Name} = \"{prop.Name}\";");
		//	}
		//	fileContent.AddLine(1, "}");

		//	fileContent.AddLine(0, "}");

		//	var outputFilepath = Path.Combine(_outputFolderpath, $"{entity.Name}.cs");
		//	FileHelper.SaveFile(outputFilepath, fileContent.AsString());

		//	OutputHelper.Write($"Completed code gen for entity: {entity.Name}");
		//}

		//private void GenerateProperty(PropertyModel prop, List<string> fileContent)
		//{
		//	if (prop.Attributes.Any())
		//		foreach (var attr in prop.AttributesList)
		//			fileContent.AddLine(1, $"[{attr}]");

		//	var dataTypeName = (prop.DataType == DataType.Enum) ? prop.EnumTypeName : CodeGenUtils.GetCSharpType(prop.DataType);
		//	var nullTypeSuffix = prop.IsNullable && prop.DataType == DataType.String ? "?" : string.Empty;
		//	var nullInit = !prop.IsNullable ? " = null!;" : string.Empty;

		//	fileContent.AddLine(1, $"public {dataTypeName}{nullTypeSuffix} {prop.Name} {{ get; set; }}{nullInit}");
		//}
	}
}