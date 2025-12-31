using Dyvenix.GenIt.DslPackage.CodeGen.Misc;
using System;
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
		private readonly string _templatesFolderpath;
		private readonly string _outputFolderpath;
		private readonly bool _enabled;

		internal EntityGenerator(List<EntityModel> entities, string entitiesNamespace, string templatesFolderpath, string outputFolderpath, bool enabled)
		{
			_entities = entities;
			_entitiesNamespace = entitiesNamespace;
			_templatesFolderpath = templatesFolderpath;
			_outputFolderpath = outputFolderpath;
			_enabled = enabled;

			Validate();
		}

		private void Validate()
		{
			if (_entities == null || _entities.Count == 0)
				throw new Exception("No entities found in the model. Add some entities first.");

			if (string.IsNullOrEmpty(_entitiesNamespace))
				throw new Exception("EntitiesNamespace is not set. Please set it in the ModelRoot properties.");

			if (string.IsNullOrEmpty(_templatesFolderpath))
				throw new Exception("TemplatesFolder is not set. Please set it in the ModelRoot properties.");
			if (!Directory.Exists(_templatesFolderpath))
				throw new Exception("TemplatesFolder does not exist. Please select a valid folder.");

			if (string.IsNullOrEmpty(_outputFolderpath))
				throw new Exception("EntitiesOutputFolder is not set. Please set it in the ModelRoot properties.");
			if (!Directory.Exists(_outputFolderpath))
				throw new Exception("EntitiesOutputFolder does not exist. Please select a valid folder.");
		}

		#region Properties


		#endregion

		internal void Run()
		{
			if (!_enabled)
				return;

			// Get absolute paths
			var templateFilepath = Path.Combine(_templatesFolderpath, cTemplateFilename);
			var outputFolder = CodeGenUtils.ResolveRelativePath(_outputFolderpath);

			Validate(outputFolder, templateFilepath);

			var template = File.ReadAllText(templateFilepath);

			foreach (var entity in _entities.Where(e => e.GenerateCode))
			{
				var cleanTemplate = $"{template}";
				//GenerateEntity(entity, _entitiesNamespace, $"{cleanTemplate}", outputFolder);
			}
		}

		private void Validate(string outputFolder, string templateFilepath)
		{
			if (!File.Exists(templateFilepath))
				throw new ApplicationException($"Template file does not exist: {templateFilepath}");

			if (!Directory.Exists(outputFolder))
				throw new ApplicationException($"OutputFolder does not exist: {outputFolder}");
		}

		//private void GenerateEntity(EntityModel entity, string entitiesNamespace, string template, string outputFolder)
		//{
		//	// Addl usings
		//	var usings = BuildAddlUsings(entity);

		//	var propsOutput = new List<string>();

		//	// PK
		//	propsOutput.AddLine(0, $"// PK");
		//	foreach (var property in entity.Properties.Where(p => p.IsPrimaryKey))
		//		this.GenerateProperty(property, propsOutput, usings);
		//	propsOutput.AddLine();

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

		//	// Properties
		//	propsOutput.AddLine(1, $"// Properties");
		//	foreach (var property in entity.Properties.Where(p => !p.IsPrimaryKey && !p.IsForeignKey))
		//		GenerateProperty(property, propsOutput, usings);

		//	// Navigation properties
		//	var navPropsOutput = new List<string>();
		//	if (entity.NavProperties.Any())
		//		navPropsOutput.AddLine(0, $"// Navigation Properties");
		//	foreach (var navProperty in entity.NavProperties)
		//		GenerateNavigationProperty(navProperty, navPropsOutput, usings);

		//	// Property names
		//	var propNames = GeneratePropNames(entity);

		//	// Replace tokens in template
		//	var fileContents = ReplaceTemplateTokens(template, usings, entitiesNamespace, entity, propsOutput, navPropsOutput, propNames);

		//	var outputFile = Path.Combine(outputFolder, $"{entity.Name}.cs");
		//	if (File.Exists(outputFile))
		//		File.Delete(outputFile);
		//	File.WriteAllText(outputFile, fileContents);
		//}

		//private List<string> BuildAddlUsings(EntityModel entity)
		//{
		//	var usings = new List<string>();

		//	entity.AddlUsings?.ToList().ForEach(u => usings.Add(u));

		//	return usings;
		//}

		//private void GenerateProperty(PropertyModel prop, List<string> output, List<string> usings)
		//{
		//	var tc = 1;

		//	if (prop.Attributes.Any())
		//		foreach (var attr in prop.Attributes)
		//			output.AddLine(tc, $"[{attr}]");

		//	if ((prop.PrimitiveType ?? PrimitiveType.None) != PrimitiveType.None)
		//	{
		//		var nullStr = (prop.Nullable && (prop.PrimitiveType.CSType != "string")) ? "?" : string.Empty;
		//		var datatype = $"{prop.PrimitiveType.CSType}{nullStr}";
		//		output.AddLine(tc, $"internal {datatype} {prop.Name} {{ get; set; }}");

		//	}
		//	else if (prop.EnumType != null)
		//	{
		//		var nullStr = prop.Nullable ? "?" : string.Empty;
		//		//output.AddLine(tc, $"[JsonConverter(typeof(JsonStringEnumConverter))]");
		//		output.AddLine(tc, $"internal {prop.EnumType.Name}{nullStr} {prop.Name} {{ get; set; }}");
		//		usings.AddIfNotExists("System.Text.Json.Serialization");
		//		if (!string.IsNullOrWhiteSpace(prop.EnumType.Namespace))
		//			usings.AddIfNotExists(prop.EnumType.Namespace);
		//	}

		//	if (prop.AddlUsings.Any())
		//		foreach (var usingStr in prop.AddlUsings)
		//			usings.AddIfNotExists(usingStr);
		//}

		//private void GenerateNavigationProperty(NavPropertyModel navProperty, List<string> propOutputList, List<string> usings)
		//{
		//	var tabCount = 1;

		//	switch (navProperty.Cardinality)
		//	{
		//		case Cardinality.OneToOne:
		//			propOutputList.AddLine(tabCount, $"internal {navProperty.FKEntity.Name} {navProperty.Name} {{ get; set; }}");
		//			break;

		//		case Cardinality.OneToMany:
		//			usings.AddIfNotExists("System.Collections.Generic");
		//			propOutputList.AddLine(tabCount, $"internal virtual ICollection<{navProperty.FKEntity.Name}> {navProperty.Name} {{ get; set; }} = new List<{navProperty.FKEntity.Name}>();");
		//			break;

		//		default:
		//			throw new ApplicationException($"Error determining data type for property '{navProperty.Name}': Cardinality '{navProperty.Cardinality}' not supported.");
		//	}
		//}

		//private string GeneratePropNames(EntityModel entity)
		//{
		//	var sb = new StringBuilder();

		//	foreach (var prop in entity.Properties)
		//	{
		//		if (sb.Length > 0)
		//			sb.Append(Environment.NewLine);
		//		sb.Append($"\t\tpublic const string {prop.Name} = nameof({entity.Name}.{prop.Name});");
		//	}

		//	return sb.ToString();
		//}

		//private string ReplaceTemplateTokens(string template, List<string> usings, string entitiesNamespace, EntityModel entity, List<string> propsOutput, List<string> navPropsOutput, string propNames)
		//{
		//	// Usings
		//	var sb = new StringBuilder();
		//	usings.ForEach(x => sb.AppendLine($"using {x};"));
		//	template = template.Replace(Utils.FmtToken(cToken_AddlUsings), sb.ToString());

		//	// Entities namespace 		
		//	template = template.Replace(Utils.FmtToken(cToken_EntitiesNs), entitiesNamespace);

		//	// Entity name
		//	template = template.Replace(Utils.FmtToken(cToken_EntityName), entity.Name);

		//	// Properties
		//	sb = new StringBuilder();
		//	propsOutput.ForEach(x => sb.AppendLine(x));
		//	template = template.Replace(Utils.FmtToken(cToken_Properties), sb.ToString());

		//	// Nav Properties
		//	sb = new StringBuilder();
		//	navPropsOutput.ForEach(x => sb.AppendLine(x));
		//	template = template.Replace(Utils.FmtToken(cToken_NavProperties), sb.ToString());

		//	// PropNames
		//	template = template.Replace(Utils.FmtToken(cToken_PropNames), propNames);

		//	return template;
		//}
	}
}
