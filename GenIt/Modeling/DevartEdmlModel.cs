//using Microsoft.OData.Edm;

//namespace CodeModeler.Modeling;

///// <summary>
///// Result of reading a Devart Entity Developer .edml file:
///// - <see cref="EdmModel"/> provides the canonical EDM structural model.
///// - <see cref="Entities"/> exposes a convenient object model enriched with ExtendedProperties.
///// </summary>
//public sealed class DevartEdmlModel
//{
//    public required IEdmModel EdmModel { get; init; }

//    /// <summary>
//    /// Convenience object model (entities/properties/navs) with Devart/user extended properties attached.
//    /// </summary>
//    public required IReadOnlyList<EntityInfo> Entities { get; init; }

//    /// <summary>
//    /// Raw extended properties keyed by a stable-ish string identifier.
//    /// Useful if you want to attach metadata to other EDM elements not projected into <see cref="Entities"/>.
//    /// </summary>
//    public required IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> ExtendedPropertiesByKey { get; init; }
//}

//public sealed record EntityInfo(
//    string Namespace,
//    string Name,
//    IReadOnlyDictionary<string, string> ExtendedProperties,
//    IReadOnlyList<PropertyInfo> Properties,
//    IReadOnlyList<NavigationInfo> Navigations)
//{
//    public string FullName => string.IsNullOrWhiteSpace(Namespace) ? Name : $"{Namespace}.{Name}";
//}

//public sealed record PropertyInfo(
//    string Name,
//    string EdmTypeFullName,
//    bool IsNullable,
//    bool IsKey,
//    IReadOnlyDictionary<string, string> ExtendedProperties);

//public sealed record NavigationInfo(
//    string Name,
//    string TargetEntityFullName,
//    EdmMultiplicity Multiplicity,
//    IReadOnlyDictionary<string, string> ExtendedProperties);
