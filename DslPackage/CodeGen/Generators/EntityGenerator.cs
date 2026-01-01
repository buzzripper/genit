using Dyvenix.GenIt.DslPackage.CodeGen.Misc;
using Dyvenix.GenIt.DslPackage.CodeGen.Templates;
using Microsoft.VisualStudio.Shell;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dyvenix.GenIt.DslPackage.CodeGen.Generators
{
	internal class EntityGenerator
	{
		#region Constants

		private const string cTemplateFilename = "Entity.tmpl";

		private const string cToken_AddlUsings = "ADDL_USINGS";
		private const string cToken_EntitiesNs = "ENTITIES_NS";
		private const string cToken_EntityName = "ENTITY_NAME";
		private const string cToken_Properties = "PROPERTIES";
		private const string cToken_NavProperties = "NAV_PROPERTIES";
		private const string cToken_PropNames = "PROP_NAMES";

		#endregion

		private readonly List<EntityModel> _entities;
		private readonly string _entitiesNamespace;
		private readonly TemplatesManager _templatesManager;
		private readonly string _outputFolderpath;
		private readonly bool _inclHeader;

		internal EntityGenerator(List<EntityModel> entities, string entitiesNamespace, string templatesFolder, string outputFolderpath, bool enabled, bool inclHeader)
		{
			_entities = entities;
			_entitiesNamespace = entitiesNamespace;
			_outputFolderpath = FileHelper.GetAbsolutePath(outputFolderpath);
			_inclHeader = inclHeader;

			_templatesManager = new TemplatesManager(templatesFolder, cTemplateFilename);

			this.Enabled = enabled;
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

			_templatesManager.Validate(errors);
		}

		#region Properties

		internal bool Enabled { get; private set; }

		#endregion

		internal void GenerateCode()
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			foreach (var entity in _entities.Where(e => e.GenerateCode))
			{
				GenerateEntity(entity);
			}
		}

		private void GenerateEntity(EntityModel entity)
		{
			var fileContent = new List<string>();

			if (_inclHeader)
				fileContent.Add(CodeGenUtils.FileHeader);

			// Usings
			if (entity.UsingsList?.Count > 0)
				entity.UsingsList.ForEach(u => fileContent.AddLine(0, $"using {u};"));

			// Namespace 		
			fileContent.AddLine();
			fileContent.AddLine(0, $"namespace {_entitiesNamespace};");

			// Declaration
			fileContent.AddLine();
			fileContent.AddLine(0, $"public partial class {entity.Name}");
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
			if (entity.InclRowVersion)
			{
				fileContent.AddLine();
				fileContent.AddLine(1, "// RowVersion");
				fileContent.AddLine(1, $"public byte[] RowVersion {{ get; set; }}");
			}

			// Properties
			if (entity.Properties.Count > 0)
			{
				fileContent.AddLine();
				fileContent.AddLine(1, $"// Properties");
				foreach (var property in entity.Properties.Where(p => !p.IsPrimaryKey && !p.IsForeignKey))
					GenerateProperty(property, fileContent);
			}

			if (entity.NavigationProperties.Count > 0)
			{
				fileContent.AddLine();
				fileContent.AddLine(1, $"// Navigation Properties");
				foreach (var navProperty in entity.NavigationProperties)
				{
					var dataType = navProperty.IsCollection ? $"List<{navProperty.TargetEntityName}>" : navProperty.TargetEntityName;
					fileContent.AddLine(1, $"public {dataType} {navProperty.Name} {{ get; set; }}");
				}
			}

			var outputFilepath = Path.Combine(_outputFolderpath, $"{entity.Name}.cs");
			FileHelper.SaveFile(outputFilepath, fileContent.AsString());

			OutputHelper.Write($"Completed code gen for entity: {entity.Name}");
		}

		private void GenerateProperty(PropertyModel prop, List<string> fileContent)
		{
			if (prop.Attributes.Any())
				foreach (var attr in prop.AttributesList)
					fileContent.AddLine(1, $"[{attr}]");

			var dataTypeName = (prop.DataType == DataType.Enum) ? prop.EnumTypeName : CodeGenUtils.GetCSharpType(prop.DataType);
			var nullStr = prop.IsNullable && prop.DataType == DataType.String ? "?" : string.Empty;
			fileContent.AddLine(1, $"public {dataTypeName}{nullStr} {prop.Name} {{ get; set; }}");

			//}
			//else if (prop.EnumType != null)
			//{
			//	var nullStr = prop.Nullable ? "?" : string.Empty;
			//	//output.AddLine(tc, $"[JsonConverter(typeof(JsonStringEnumConverter))]");
			//	output.AddLine(tc, $"public {prop.EnumType.Name}{nullStr} {prop.Name} {{ get; set; }}");
			//	usings.AddIfNotExists("System.Text.Json.Serialization");
			//	if (!string.IsNullOrWhiteSpace(prop.EnumType.Namespace))
			//		usings.AddIfNotExists(prop.EnumType.Namespace);
			//}

			//	if (prop.AddlUsings.Any())
			//		foreach (var usingStr in prop.AddlUsings)
			//			usings.AddIfNotExists(usingStr);
		}

		//private void GenerateNavigationProperty(NavigationProperty navProperty, List<string> fileContent)
		//{
		//	var tabCount = 1;

		//	break;

		//		case Cardinality.OneToMany:
		//		usings.AddIfNotExists("System.Collections.Generic");
		//		fileContent.AddLine(tabCount, $"public virtual ICollection<{navProperty.FKEntity.Name}> {navProperty.Name} {{ get; set; }} = new List<{navProperty.FKEntity.Name}>();");
		//		break;

		//	default:
		//		throw new ApplicationException($"Error determining data type for property '{navProperty.Name}': Cardinality '{navProperty.Cardinality}' not supported.");
		//	}
		//}

		//private List<string> GeneratePropNames(EntityModel entity)
		//{
		//	var propNames = new List<string>();

		//	foreach (var prop in entity.Properties)
		//		propNames.Append($"public const string {prop.Name} = \"{prop.Name}\";");

		//	return propNames;
		//}

		//private string CreateContents(List<string> usings, string entitiesNamespace, EntityModel entity, List<string> propsOutput, List<string> navPropsOutput, List<string> propNames)
		//{
		//	var content = new List<string>();

		//	if (_inclHeader)
		//		content.Add(CodeGenUtils.FileHeader);

		//	// Usings
		//	if (usings?.Count > 0)
		//		usings.ForEach(x => content.AddLine(0, $"using {x};"));

		//	// Namespace 		
		//	content.AddLine();
		//	content.AddLine(0, $"namespace {entitiesNamespace};");

		//	// Declaration
		//	content.AddLine();
		//	content.AddLine(0, $"public partial class {entity.Name}");
		//	content.AddLine(0, "{");

		//	// Properties
		//	if (propsOutput.Count > 0)
		//	{
		//		propsOutput.ForEach(propLine => content.AddLine(1, $"{propLine}"));
		//	}

		//	// Nav Properties
		//	if (navPropsOutput.Count > 0)
		//	{
		//		content.AddLine();
		//		navPropsOutput.ForEach(navPropLine => content.AddLine(1, $"{navPropLine}"));
		//	}

		//	// PropNames
		//	if (propNames.Count > 0)
		//	{
		//		content.AddLine();
		//		propNames.ForEach(propNameLine => content.AddLine(1, $"{propNameLine}"));
		//	}

		//	content.AddLine(0, "}");

		//	return content.AsString();
		//}
	}
}