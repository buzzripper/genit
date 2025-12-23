//using Microsoft.OData.Edm;

//namespace CodeModeler.Modeling;

//internal static class Projector
//{
//	public static IReadOnlyList<EntityInfo> ProjectEntities(
//		IEdmModel edm,
//		IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> extendedPropsByKey)
//	{
//		var entities = edm.SchemaElements
//			.OfType<IEdmEntityType>()
//			.OrderBy(e => e.Namespace)
//			.ThenBy(e => e.Name)
//			.Select(e => ProjectEntity(e, extendedPropsByKey))
//			.ToList();

//		return entities;
//	}

//	private static EntityInfo ProjectEntity(
//		IEdmEntityType entity,
//		IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> extendedPropsByKey)
//	{
//		string ns = entity.Namespace ?? string.Empty;
//		string name = entity.Name;

//		var entityKey = $"Entity:{Qualify(ns, name)}";
//		var entityExt = Lookup(extendedPropsByKey, entityKey);

//		var keyProps = new HashSet<string>(
//			entity.Key().Select(k => k.Name),
//			StringComparer.Ordinal);

//		var props = entity.DeclaredProperties
//			.Where(p => p.PropertyKind == EdmPropertyKind.Structural)
//			.OfType<IEdmStructuralProperty>()
//			.OrderBy(p => p.Name)
//			.Select(p =>
//			{
//				var type = p.Type;
//				var propKey = $"Property:{Qualify(ns, name)}/{p.Name}";
//				var ext = Lookup(extendedPropsByKey, propKey);

//				return new PropertyInfo(
//					Name: p.Name,
//					EdmTypeFullName: type.Definition.FullTypeName(),
//					IsNullable: type.IsNullable,
//					IsKey: keyProps.Contains(p.Name),
//					ExtendedProperties: ext);
//			})
//			.ToList();

//		var navs = entity.DeclaredNavigationProperties()
//			.OrderBy(n => n.Name)
//			.Select(n =>
//			{
//				var navKey = $"Navigation:{Qualify(ns, name)}/{n.Name}";
//				var ext = Lookup(extendedPropsByKey, navKey);

//				var target = n.ToEntityType().FullTypeName();
//				var mult = n.TargetMultiplicity();

//				return new NavigationInfo(
//					Name: n.Name,
//					TargetEntityFullName: target,
//					Multiplicity: mult,
//					ExtendedProperties: ext);
//			})
//			.ToList();

//		return new EntityInfo(
//			Namespace: ns,
//			Name: name,
//			ExtendedProperties: entityExt,
//			Properties: props,
//			Navigations: navs);
//	}

//	private static IReadOnlyDictionary<string, string> Lookup(
//		IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> dict,
//		string key)
//		=> dict.TryGetValue(key, out var v) ? v : EmptyProps.Instance;

//	private static string Qualify(string ns, string name)
//		=> string.IsNullOrWhiteSpace(ns) ? name : $"{ns}.{name}";

//	private sealed class EmptyProps : Dictionary<string, string>
//	{
//		public static readonly IReadOnlyDictionary<string, string> Instance = new EmptyProps();
//		private EmptyProps() : base(StringComparer.Ordinal) { }
//	}
//}
