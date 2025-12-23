//using System.Xml.Linq;

//namespace CodeModeler.Modeling;

///// <summary>
///// Extracts Devart/user "extended properties" from a Devart Entity Developer .edml XML document.
/////
///// This deliberately does NOT try to understand every possible Devart annotation shape.
///// It looks for elements named 'ExtendedProperty' (any namespace) and reads:
///// - @Name (or @Key)
///// - @Value
/////
///// Then it attaches those properties to the nearest logical ancestor:
///// - EntityType
///// - Property
///// - NavigationProperty
/////
///// Keys used in the returned dictionary:
///// - Entity:      "Entity:{namespace}.{entityName}"
///// - Property:    "Property:{namespace}.{entityName}/{propertyName}"
///// - Navigation:  "Navigation:{namespace}.{entityName}/{navName}"
/////
///// If an ExtendedProperty can't be associated, it is placed under:
///// - "Unbound"
///// </summary>
//internal static class ExtendedPropertyExtractor
//{
//    public static IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> Extract(XDocument xdoc)
//    {
//        var dict = new Dictionary<string, Dictionary<string, string>>(StringComparer.Ordinal);

//        foreach (var ep in xdoc.Descendants().Where(e => e.Name.LocalName == "ExtendedProperty"))
//        {
//            var name = (string?)ep.Attribute("Name") ?? (string?)ep.Attribute("Key");
//            var value = (string?)ep.Attribute("Value");

//            if (string.IsNullOrWhiteSpace(name))
//                continue;

//            // If Value is missing, support inner text as a fallback.
//            if (value is null)
//                value = ep.Value?.Trim();

//            string key = ComputeAttachmentKey(ep);

//            if (!dict.TryGetValue(key, out var kv))
//            {
//                kv = new Dictionary<string, string>(StringComparer.Ordinal);
//                dict[key] = kv;
//            }

//            // Last-write-wins if duplicates occur.
//            kv[name] = value ?? string.Empty;
//        }

//        // Freeze into read-only shapes
//        return dict.ToDictionary(
//            kvp => kvp.Key,
//            kvp => (IReadOnlyDictionary<string, string>)new Dictionary<string, string>(kvp.Value, StringComparer.Ordinal),
//            StringComparer.Ordinal);
//    }

//    private static string ComputeAttachmentKey(XElement ep)
//    {
//        // Walk up to find nearest relevant ancestor.
//        var prop = ep.Ancestors().FirstOrDefault(a =>
//            a.Name.LocalName is "Property" or "NavigationProperty" or "EntityType");

//        if (prop is null)
//            return "Unbound";

//        // Determine entity context (Namespace + EntityType name)
//        var entity = prop.Name.LocalName == "EntityType"
//            ? prop
//            : prop.Ancestors().FirstOrDefault(a => a.Name.LocalName == "EntityType");

//        var schema = prop.Ancestors().FirstOrDefault(a => a.Name.LocalName == "Schema");

//        string ns = (string?)schema?.Attribute("Namespace") ?? string.Empty;
//        string entityName = (string?)entity?.Attribute("Name") ?? string.Empty;

//        if (prop.Name.LocalName == "EntityType")
//        {
//            return $"Entity:{Qualify(ns, entityName)}";
//        }

//        string memberName = (string?)prop.Attribute("Name") ?? string.Empty;
//        if (prop.Name.LocalName == "Property")
//            return $"Property:{Qualify(ns, entityName)}/{memberName}";

//        if (prop.Name.LocalName == "NavigationProperty")
//            return $"Navigation:{Qualify(ns, entityName)}/{memberName}";

//        return "Unbound";
//    }

//    private static string Qualify(string ns, string name)
//        => string.IsNullOrWhiteSpace(ns) ? name : $"{ns}.{name}";
//}
