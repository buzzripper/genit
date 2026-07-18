using Dyvenix.GenIt.DslPackage.CodeGen.Misc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dyvenix.GenIt.DslPackage.CodeGen.Generators
{
	internal class DbContextGenerator
	{
		private readonly List<EntityModel> _entities;
		private readonly string _dbContextName;
		private readonly string _dbContextNamespace;
		private readonly List<string> _dbContextUsings;
		private readonly string _entitiesNamespace;
		private readonly string _outputFolderpath;
		private readonly string _baseClass;
		private readonly bool _inclHeader;
		private readonly List<Association> _associations;

		internal DbContextGenerator(ModelRoot modelRoot)
		{
			_entities = modelRoot.Types.OfType<EntityModel>().ToList();
			_dbContextName = modelRoot.DbContextName;
			_dbContextNamespace = modelRoot.DbContextNamespace;
			_entitiesNamespace = modelRoot.EntitiesNamespace;
			_outputFolderpath = FileHelper.GetAbsolutePath(modelRoot.DbContextOutputFolder);
			_inclHeader = modelRoot.InclHeader;
			_dbContextUsings = modelRoot.DbContextUsingsList;
			_baseClass = modelRoot.DbContextBaseClass;
			_associations = modelRoot.Store.ElementDirectory.FindElements<Association>().ToList();
		}

		internal void Validate(List<string> errors)
		{
			if (_entities == null || _entities.Count == 0)
				errors.Add("No entities found in the model. Add some entities first.");

			if (string.IsNullOrEmpty(_dbContextName))
				errors.Add("DbContextNames is not set. Please set it in the DbContext properties.");

			if (string.IsNullOrEmpty(_dbContextNamespace))
				errors.Add("DbContextNamespace is not set. Please set it in the DbContext properties.");

			if (string.IsNullOrEmpty(_entitiesNamespace))
				errors.Add("EntitiesNamespace is not set. Please set it in the DbContext properties.");

			if (string.IsNullOrEmpty(_outputFolderpath))
				errors.Add("DbContextOutputFolder is not set. Please set it in the DbContext properties.");
			else if (!Directory.Exists(_outputFolderpath))
				errors.Add("DbContextOutputFolder does not exist. Please select a valid folder.");

			if (string.IsNullOrEmpty(_baseClass))
				errors.Add("DbContext BaseClass is not set. Please set it in the DbContext properties.");
		}

		internal void GenerateCode()
		{
			var className = $"{_dbContextName}";

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
			fileContent.AddLine(0, $"public partial class {className} : {_baseClass}");
			fileContent.AddLine(0, "{");
			fileContent.AddLine(1, $"partial void OnModelCreatingExt(ModelBuilder builder);");
			fileContent.AddLine();
			fileContent.AddLine(1, $"public {className}(DbContextOptions<{className}> options)");
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
			fileContent.AddLine(2, "this.OnModelCreatingExt(modelBuilder);");

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
				{
					fileContent.AddLine(3, "// PK");
					fileContent.AddLine(3, $"entity.HasKey(e => e.{prop.Name});");
				}

				// RowVersion
				if (entity.InclRowVersion)
				{
					fileContent.AddLine(3, "// RowVersion");
					fileContent.AddLine(3, $"entity.Property(e => e.RowVersion).IsRowVersion();");
				}

				// FK properties
				var fkProps = entity.Properties.Where(p => p.IsForeignKey);
				if (fkProps.Count() > 0)
				{
					fileContent.AddLine(3, "// FKs");
					foreach (var prop in fkProps)
					{
						var line = $"entity.Property(e => e.{prop.Name})";
						if (!prop.IsNullable)
							line += ".IsRequired()";
						line += ";";
						fileContent.AddLine(3, line);
					}
				}

				// Properties
				var otherProps = entity.Properties.Where(p => !p.IsPrimaryKey && !p.IsForeignKey && !p.IsRowVersion && !p.IsSoftDelete && !p.IsAuditable);
				if (otherProps.Count() > 0)
				{
					fileContent.AddLine(3, "// Other Properties");
					foreach (var otherProp in otherProps)
					{
						var line = $"entity.Property(e => e.{otherProp.Name})";
						if (!otherProp.IsNullable)
							line += ".IsRequired()";
						if (otherProp.DataType == DataTypes.String && otherProp.Length > 0)
							line += $".HasMaxLength({otherProp.Length})";
						line += ";";
						fileContent.AddLine(3, line);
					}
				}

				// Auditable
				if (entity.SoftDelete)
				{
					fileContent.AddLine();
					fileContent.AddLine(3, "// Auditing");
					foreach (var prop in entity.Properties.Where(p => p.IsAuditable))
					{
						var line = $"entity.Property(e => e.{prop.Name})";
						if (!prop.IsNullable)
							line += ".IsRequired()";
						if (prop.DataType == DataTypes.String && prop.Length > 0)
							line += $".HasMaxLength({prop.Length})";
						line += ";";
						fileContent.AddLine(3, line);
					}
				}

				// Soft delete
				if (entity.SoftDelete)
				{
					fileContent.AddLine();
					fileContent.AddLine(3, "// Soft delete");
					fileContent.AddLine(3, $"modelBuilder.Entity<{entity.Name}>().HasQueryFilter(x => x.DeletedUtc == null);");
					foreach (var prop in entity.Properties.Where(p => p.IsSoftDelete))
					{
						var line = $"entity.Property(e => e.{prop.Name})";
						if (!prop.IsNullable)
							line += ".IsRequired()";
						if (prop.DataType == DataTypes.String && prop.Length > 0)
							line += $".HasMaxLength({prop.Length})";
						line += ";";
						fileContent.AddLine(3, line);
					}
				}

				// Foreign keys
				if (fkProps.Count() > 0)
				{
					fileContent.AddLine();
					fileContent.AddLine(3, "// Foreign Keys");
					foreach (var prop in fkProps)
					{
						fileContent.AddLine();

						var assoc = _associations.FirstOrDefault(a => a.Target.Name == entity.Name && a.FkPropertyName == prop.Name);
						if (assoc == null)
							throw new ApplicationException($"No association found for foreign key '{prop.Name}' in entity '{entity.Name}'.");

						var sourceEntityName = assoc.Source.Name;
						var sourceNavPropName = assoc.SourceRoleName;
						var targetEntityName = assoc.Target.Name;
						var targetNavPropName = assoc.TargetRoleName;

						// One-to-many
						if ((assoc.SourceMultiplicity == Multiplicity.One || assoc.SourceMultiplicity == Multiplicity.ZeroOne) && assoc.TargetMultiplicity == Multiplicity.Many)
						{
							if (string.IsNullOrWhiteSpace(sourceNavPropName) && string.IsNullOrWhiteSpace(targetNavPropName))
							{
								// No nav properties on either end
								fileContent.AddLine(3, $"entity.HasOne<{sourceEntityName}>()");
								fileContent.AddLine(4, $".WithMany()");
								fileContent.AddLine(4, $".HasForeignKey(te => te.{prop.Name})");
								fileContent.AddLine(4, $".OnDelete(DeleteBehavior.NoAction);");
							}
							else if (string.IsNullOrWhiteSpace(sourceNavPropName) && !string.IsNullOrWhiteSpace(targetNavPropName))
							{
								// Navigation property only on target
								fileContent.AddLine(3, $"entity.HasOne(te => te.{targetEntityName})");
								fileContent.AddLine(4, $".WithMany()");
								fileContent.AddLine(4, $".HasForeignKey(te => te.{prop.Name})");
								fileContent.AddLine(4, $".OnDelete(DeleteBehavior.NoAction);");
							}
							else if (!string.IsNullOrWhiteSpace(sourceNavPropName) && string.IsNullOrWhiteSpace(targetNavPropName))
							{
								// Navigation property only on source
								fileContent.AddLine(3, $"entity.HasOne<{sourceEntityName}>()");
								fileContent.AddLine(4, $".WithMany(se => se.{sourceNavPropName})");
								fileContent.AddLine(4, $".HasForeignKey(te => te.{prop.Name})");
								fileContent.AddLine(4, $".OnDelete(DeleteBehavior.NoAction);");
							}
							else if (!string.IsNullOrWhiteSpace(sourceNavPropName) && !string.IsNullOrWhiteSpace(targetNavPropName))
							{
								// Navigation property only on both sides
								fileContent.AddLine(3, $"entity.HasOne(te => te.{targetEntityName})");
								fileContent.AddLine(4, $".WithMany(se => se.{sourceNavPropName})");
								fileContent.AddLine(4, $".HasForeignKey(te => te.{prop.Name})");
								fileContent.AddLine(4, $".OnDelete(DeleteBehavior.NoAction);");
							}
						}
						// One-to-one
						else if ((assoc.SourceMultiplicity == Multiplicity.One || assoc.SourceMultiplicity == Multiplicity.ZeroOne) && (assoc.TargetMultiplicity == Multiplicity.One || assoc.TargetMultiplicity == Multiplicity.ZeroOne))
						{
							if (string.IsNullOrWhiteSpace(sourceNavPropName) && string.IsNullOrWhiteSpace(targetNavPropName))
							{
								// No nav properties on either end
								fileContent.AddLine(3, $"entity.HasOne<{sourceEntityName}>()");
								fileContent.AddLine(4, $".WithOne()");
								fileContent.AddLine(4, $".HasForeignKey<{targetEntityName}>(te => te.{prop.Name})");
								fileContent.AddLine(4, $".OnDelete(DeleteBehavior.NoAction);");
							}
							else if (string.IsNullOrWhiteSpace(sourceNavPropName) && !string.IsNullOrWhiteSpace(targetNavPropName))
							{
								// Navigation property only on target
								fileContent.AddLine(3, $"entity.HasOne(te => te.{targetNavPropName})");
								fileContent.AddLine(4, $".WithOne()");
								fileContent.AddLine(4, $".HasForeignKey<{targetEntityName}>(te => te.{prop.Name})");
								fileContent.AddLine(4, $".OnDelete(DeleteBehavior.NoAction);");
							}
							else if (!string.IsNullOrWhiteSpace(sourceNavPropName) && string.IsNullOrWhiteSpace(targetNavPropName))
							{
								// Navigation property only on source
								fileContent.AddLine(3, $"entity.HasOne<{sourceEntityName}>()");
								fileContent.AddLine(4, $".WithOne(se => se.{sourceNavPropName})");
								fileContent.AddLine(4, $".HasForeignKey<{targetEntityName}>(te => te.{prop.Name})");
								fileContent.AddLine(4, $".OnDelete(DeleteBehavior.NoAction);");
							}
							else if (!string.IsNullOrWhiteSpace(sourceNavPropName) && !string.IsNullOrWhiteSpace(targetNavPropName))
							{
								// Navigation property only on both sides
								fileContent.AddLine(3, $"entity.HasOne(te => te.{targetNavPropName})");
								fileContent.AddLine(4, $".WithOne(se => se.{sourceNavPropName})");
								fileContent.AddLine(4, $".HasForeignKey<{targetEntityName}>(te => te.{prop.Name})");
								fileContent.AddLine(4, $".OnDelete(DeleteBehavior.NoAction);");
							}
						}

						// NOTE: Do NOT do many-to-many here, they need their own loop, which is done below
					}
				}

				// Indexes
				fileContent.AddLine();
				fileContent.AddLine(3, "// Indexes");
				foreach (var prop in entity.Properties.Where(p => p.IsPrimaryKey || p.IsForeignKey || p.IsIndexed))
				{
					var line = $"entity.HasIndex(e => e.{prop.Name}, \"IX_{entity.Name}_{prop.Name}\")";
					if (prop.IsIndexUnique)
						line += ".IsUnique()";
					if (prop.IsIndexClustered)
						line += ".IsClustered()";
					line += ";";
					fileContent.AddLine(3, line);
				}

				fileContent.AddLine(2, "});");
				fileContent.AddLine();
				fileContent.AddLine(2, $"#endregion");
			}

			// Foreign key many-to-many relationships
			var manyToManyAssocs = _associations.Where(a => a.SourceMultiplicity == Multiplicity.Many && a.TargetMultiplicity == Multiplicity.Many).ToList();
			if (manyToManyAssocs.Count > 0)
			{
				fileContent.AddLine();
				fileContent.AddLine(2, "#region Many-to-many relationships");
				foreach (var assoc in manyToManyAssocs)
				{
					var sourceEntityName = assoc.Source.Name;
					var sourceNavPropName = assoc.SourceRoleName;
					var targetEntityName = assoc.Target.Name;
					var targetNavPropName = assoc.TargetRoleName;

					fileContent.AddLine();
					if (!string.IsNullOrWhiteSpace(sourceNavPropName) && !string.IsNullOrWhiteSpace(targetNavPropName))
					{
						// Navigation property on both source and target
						fileContent.AddLine(2, $"modelBuilder.Entity<{sourceEntityName}>()");
						fileContent.AddLine(3, $".HasMany(se => se.{sourceNavPropName})");
						fileContent.AddLine(3, $".WithMany(te => te.{targetNavPropName});");
					}
					else if (string.IsNullOrWhiteSpace(sourceNavPropName) && !string.IsNullOrWhiteSpace(targetNavPropName))
					{
						// Navigation property only on target
						fileContent.AddLine(2, $"modelBuilder.Entity<{targetEntityName}>()");
						fileContent.AddLine(3, $".HasMany(te => te.{targetNavPropName})");
						fileContent.AddLine(3, $".WithMany();");
					}
					else if (!string.IsNullOrWhiteSpace(sourceNavPropName) && string.IsNullOrWhiteSpace(targetNavPropName))
					{
						// Navigation property only on source
						fileContent.AddLine(2, $"modelBuilder.Entity<{sourceEntityName}>()");
						fileContent.AddLine(3, $".HasMany(se => se.{sourceNavPropName})");
						fileContent.AddLine(3, $".WithMany();");
					}
				}
				fileContent.AddLine();
				fileContent.AddLine(2, "#endregion");
			}

			fileContent.AddLine();
			fileContent.AddLine(2, "OnModelCreatingPartial(modelBuilder);");
			fileContent.AddLine(1, "}");

			fileContent.AddLine();

			fileContent.AddLine(1, "partial void OnModelCreatingPartial(ModelBuilder modelBuilder);");

			fileContent.AddLine(0, "}");

			var outputFilepath = Path.Combine(_outputFolderpath, $"{className}.cs");
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