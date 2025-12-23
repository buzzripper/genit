//using Microsoft.OData.Edm;
//using Microsoft.OData.Edm.Csdl;
//using Microsoft.OData.Edm.Validation;
//using System.Xml;
//using System.Xml.Linq;

//namespace CodeModeler.Modeling;

//public static class EdmlReader
//{
//	/// <summary>
//	/// Reads a Devart Entity Developer .edml file, returning an EDM model plus a convenient enriched object model.
//	/// </summary>
//	public static DevartEdmlModel Read(string edmlPath)
//	{
//		if (string.IsNullOrWhiteSpace(edmlPath))
//			throw new ArgumentException("Path is required.", nameof(edmlPath));

//		if (!File.Exists(edmlPath))
//			throw new FileNotFoundException("EDML file not found.", edmlPath);

//		// Parse EDM model directly from file to avoid namespace prefix issues
//		IEdmModel edm = ParseEdmModel(edmlPath);

//		// Load XML separately for custom property extraction
//		XDocument xdoc = XDocument.Load(edmlPath, LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);

//		// Extract Devart/user extended properties (annotations) from XML
//		var extendedProps = ExtendedPropertyExtractor.Extract(xdoc);

//		// Project into a friendly object model and attach extended properties
//		var entities = Projector.ProjectEntities(edm, extendedProps);

//		return new DevartEdmlModel
//		{
//			EdmModel = edm,
//			Entities = entities,
//			ExtendedPropertiesByKey = extendedProps
//		};
//	}

//	private static IEdmModel ParseEdmModel(string edmlPath)
//	{
//		// Parse directly from file stream to preserve proper namespace handling for CsdlReader
//		using var stream = File.OpenRead(edmlPath);
//		using var reader = XmlReader.Create(stream, new XmlReaderSettings { IgnoreWhitespace = false });

//		if (!CsdlReader.TryParse(reader, out IEdmModel? model, out IEnumerable<EdmError>? errors))
//		{
//			var msg = errors is null
//				? "(no details)"
//				: string.Join(Environment.NewLine, errors.Select(e => e.ErrorMessage));

//			throw new InvalidOperationException("Failed to parse CSDL. " + msg);
//		}

//		if (errors is not null && errors.Any(e => e.Severity == Severity.Error))
//		{
//			var msg = string.Join(Environment.NewLine, errors.Select(e => e.ErrorMessage));
//			throw new InvalidOperationException("CSDL parsed with errors: " + msg);
//		}

//		return model;
//	}
//}
