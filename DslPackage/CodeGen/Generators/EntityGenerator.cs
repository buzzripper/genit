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
				GenerateEntity(entity, _entitiesNamespace, _outputFolderpath);
			}
		}

		private void GenerateEntity(EntityModel entity, string entitiesNamespace, string outputFolder)
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			// Usings
			var usings = new List<string>();
			foreach (var u in entity.UsingsList)
				usings.Add(u);

			var propsOutput = new List<string>();

			// PK
			propsOutput.Add("// PK");
			foreach (var property in entity.Properties.Where(p => p.IsPrimaryKey))
				this.GenerateProperty(property, propsOutput, usings);

			//	//// RowVersion
			//	//if (entity.InclRowVersion)
			//	//{
			//	//	var rowVerProp = new PropertyModel
			//	//	{
			//	//		Id = Guid.NewGuid(),
			//	//		Name = "RowVersion",
			//	//		PrimitiveType = PrimitiveType.ByteArray
			//	//	};
			//	//	this.GenerateProperty(rowVerProp, propsOutput, usings);
			//	//	propsOutput.AddLine();
			//	//}

			//	// FK properties
			//	var fkProperties = entity.Properties.Where(p => p.IsForeignKey);
			//	if (fkProperties.Any())
			//		propsOutput.AddLine(1, $"// FKs");
			//	foreach (var property in fkProperties)
			//		GenerateProperty(property, propsOutput, usings);
			//	if (fkProperties.Any())
			//		propsOutput.AddLine();

			// Properties
			propsOutput.Add($"// Properties");
			foreach (var property in entity.Properties.Where(p => !p.IsPrimaryKey && !p.IsForeignKey))
				GenerateProperty(property, propsOutput, usings);

			// Navigation properties
			var navPropsOutput = new List<string>();
			//	if (entity.NavProperties.Any())
			//		navPropsOutput.AddLine(0, $"// Navigation Properties");
			//	foreach (var navProperty in entity.NavProperties)
			//		GenerateNavigationProperty(navProperty, navPropsOutput, usings);

			// Property names
			var propNames = GeneratePropNames(entity);

			// Replace tokens in template
			var fileContents = CreateContents(usings, entitiesNamespace, entity, propsOutput, navPropsOutput, propNames);

			var outputFilepath = Path.Combine(outputFolder, $"{entity.Name}.cs");
			//if (File.Exists(outputFile))
			//	File.Delete(outputFile);
			//File.WriteAllText(outputFile, fileContents);
			FileHelper.SaveFile(outputFilepath, fileContents);

			OutputHelper.Write($"Completed code gen for entity: {entity.Name}");
		}

		private void GenerateProperty(PropertyModel prop, List<string> output, List<string> usings)
		{
			var tc = 1;

			if (prop.Attributes.Any())
				foreach (var attr in prop.AttributesList)
					output.AddLine(tc, $"[{attr}]");

			var dataTypeName = (prop.DataType == DataType.Enum) ? prop.EnumTypeName : CodeGenUtils.GetCSharpType(prop.DataType);
			var nullStr = prop.IsNullable && prop.DataType == DataType.String ? "?" : string.Empty;
			output.Add($"public {dataTypeName}{nullStr} {prop.Name} {{ get; set; }}");

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

		//private void GenerateNavigationProperty(NavPropertyModel navProperty, List<string> propOutputList, List<string> usings)
		//{
		//	var tabCount = 1;

		//	switch (navProperty.Cardinality)
		//	{
		//		case Cardinality.OneToOne:
		//			propOutputList.AddLine(tabCount, $"public {navProperty.FKEntity.Name} {navProperty.Name} {{ get; set; }}");
		//			break;

		//		case Cardinality.OneToMany:
		//			usings.AddIfNotExists("System.Collections.Generic");
		//			propOutputList.AddLine(tabCount, $"public virtual ICollection<{navProperty.FKEntity.Name}> {navProperty.Name} {{ get; set; }} = new List<{navProperty.FKEntity.Name}>();");
		//			break;

		//		default:
		//			throw new ApplicationException($"Error determining data type for property '{navProperty.Name}': Cardinality '{navProperty.Cardinality}' not supported.");
		//	}
		//}

		private List<string> GeneratePropNames(EntityModel entity)
		{
			var propNames = new List<string>();

			foreach (var prop in entity.Properties)
				propNames.Append($"public const string {prop.Name} = \"{prop.Name}\";");

			return propNames;
		}

		private string CreateContents(List<string> usings, string entitiesNamespace, EntityModel entity, List<string> propsOutput, List<string> navPropsOutput, List<string> propNames)
		{
			var content = new List<string>();

			if (_inclHeader)
				content.Add(CodeGenUtils.FileHeader);

			// Usings
			if (usings?.Count > 0)
				usings.ForEach(x => content.AddLine(0, $"using {x};"));

			// Namespace 		
			content.AddLine();
			content.AddLine(0, $"namespace {entitiesNamespace};");

			// Declaration
			content.AddLine();
			content.AddLine(0, $"public partial class {entity.Name}");
			content.AddLine(0, "{");

			// Properties
			if (propsOutput.Count > 0)
			{
				propsOutput.ForEach(propLine => content.AddLine(1, $"{propLine}"));
			}

			// Nav Properties
			if (navPropsOutput.Count > 0)
			{
				content.AddLine();
				navPropsOutput.ForEach(navPropLine => content.AddLine(1, $"{navPropLine}"));
			}

			// PropNames
			if (propNames.Count > 0)
			{
				content.AddLine();
				propNames.ForEach(propNameLine => content.AddLine(1, $"{propNameLine}"));
			}

			content.AddLine(0, "}");

			return content.AsString();
		}
	}
}