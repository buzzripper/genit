using Dyvenix.GenIt.DslPackage.CodeGen.Misc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Dyvenix.GenIt.DslPackage.CodeGen.Generators
{
	internal class TestDataGenerator
	{
		private const int RootEntityCount = 5;
		private const int MaxChildCount = 10;
		private const double NullProbability = 0.3;
		private const string AlphaChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

		private readonly ModelRoot _modelRoot;
		private readonly List<EntityModel> _entitiesToCreate;
		private readonly Random _random = new Random();

		internal TestDataGenerator(ModelRoot modelRoot)
		{
			_modelRoot = modelRoot;

			var entities = modelRoot.Types.OfType<EntityModel>().Where(e => e.GenerateCode).ToList();
			_entitiesToCreate = CodeGenUtils.SortEntitiesForCreation(entities.Where(e => e.GenIntTestData).ToList());
		}

		internal void GenerateCode()
		{
			var entityMap = _entitiesToCreate.ToDictionary(e => e.Name);

			// Build child-to-parent relationships from collection NavigationProperties.
			// The entity owning the IsCollection nav prop is the parent; TargetEntityName is the child.
			var childToParents = new Dictionary<string, List<ParentRelation>>();

			foreach (var entity in _entitiesToCreate)
			{
				foreach (var navProp in entity.NavigationProperties)
				{
					if (!navProp.IsCollection)
						continue;

					var childEntityName = navProp.TargetEntityName;
					if (!entityMap.ContainsKey(childEntityName))
						continue;

					var fkPropertyName = $"{entity.Name}Id";
					var parentPkPropName = entity.Properties
						.Where(p => p.IsPrimaryKey)
						.Select(p => p.Name)
						.FirstOrDefault() ?? "Id";

					if (!childToParents.ContainsKey(childEntityName))
						childToParents[childEntityName] = new List<ParentRelation>();

					childToParents[childEntityName].Add(new ParentRelation
					{
						ParentEntityName = entity.Name,
						FkPropertyName = fkPropertyName,
						ParentPkPropertyName = parentPkPropName
					});
				}
			}

			// Generate instances in topological order (parents first)
			var entityData = new Dictionary<string, List<Dictionary<string, object>>>();

			foreach (var entity in _entitiesToCreate)
			{
				var instances = new List<Dictionary<string, object>>();

				if (!childToParents.ContainsKey(entity.Name))
				{
					// Root entity – generate a fixed number of instances
					for (int i = 0; i < RootEntityCount; i++)
						instances.Add(GenerateInstance(entity));
				}
				else
				{
					// Child entity – generate 0-10 children per primary-parent instance
					var relations = childToParents[entity.Name];
					var primaryRelation = relations[0];

					if (entityData.ContainsKey(primaryRelation.ParentEntityName))
					{
						var parentInstances = entityData[primaryRelation.ParentEntityName];
						foreach (var parentInstance in parentInstances)
						{
							int childCount = _random.Next(0, MaxChildCount + 1);
							for (int c = 0; c < childCount; c++)
							{
								var child = GenerateInstance(entity);
								child[primaryRelation.FkPropertyName] = parentInstance[primaryRelation.ParentPkPropertyName];
								instances.Add(child);
							}
						}
					}
					else
					{
						// Fallback if parent was not generated
						for (int i = 0; i < RootEntityCount; i++)
							instances.Add(GenerateInstance(entity));
					}

					// For additional FK relationships, randomly pick from existing parent instances
					for (int r = 1; r < relations.Count; r++)
					{
						var relation = relations[r];
						if (!entityData.ContainsKey(relation.ParentEntityName))
							continue;

						var otherParentInstances = entityData[relation.ParentEntityName];
						if (otherParentInstances.Count == 0)
							continue;

						foreach (var instance in instances)
						{
							var randomParent = otherParentInstances[_random.Next(otherParentInstances.Count)];
							instance[relation.FkPropertyName] = randomParent[relation.ParentPkPropertyName];
						}
					}
				}

				entityData[entity.Name] = instances;
			}

			// Serialize all entity lists into a single JSON file matching the TestDataSet structure
			var outputFolder = Path.Combine(FileHelper.GetAbsolutePath(_modelRoot.IntTestsRootFolder), "TestData");
			Directory.CreateDirectory(outputFolder);

			var json = SerializeDataSet(entityData);
			var filepath = Path.Combine(outputFolder, "Main.json");
			File.WriteAllText(filepath, json);
		}

		#region Instance Generation

		private Dictionary<string, object> GenerateInstance(EntityModel entity)
		{
			var instance = new Dictionary<string, object>();

			foreach (var prop in entity.Properties)
			{
				if (prop.IsRowVersion)
					continue;

				if (prop.IsNullable && !prop.IsPrimaryKey && _random.NextDouble() < NullProbability)
				{
					instance[prop.Name] = null;
					continue;
				}

				instance[prop.Name] = GeneratePropertyValue(prop);
			}

			return instance;
		}

		private object GeneratePropertyValue(PropertyModel prop)
		{
			var dataType = prop.DataType;

			if (Dyvenix.GenIt.DataTypes.IsEnumType(dataType))
			{
				var enumModel = _modelRoot.Types.OfType<EnumModel>()
					.FirstOrDefault(e => e.Name == dataType);
				if (enumModel != null && enumModel.Members.Count > 0)
					return _random.Next(0, enumModel.Members.Count);
				return 0;
			}

			switch (dataType)
			{
				case "Guid":
					return Guid.NewGuid().ToString();
				case "String":
					int maxLen = prop.Length > 0 ? prop.Length : 50;
					int len = _random.Next(1, maxLen + 1);
					return GenerateRandomString(len);
				case "Int32":
					return _random.Next(1, 10000);
				case "Int16":
					return (short)_random.Next(1, 1000);
				case "Int64":
					return (long)_random.Next(1, 100000);
				case "UInt16":
					return (ushort)_random.Next(0, 1000);
				case "UInt32":
					return (uint)_random.Next(0, 10000);
				case "UInt64":
					return (ulong)_random.Next(0, 100000);
				case "Byte":
					return (byte)_random.Next(0, 256);
				case "SByte":
					return (sbyte)_random.Next(-128, 128);
				case "Boolean":
					return _random.Next(0, 2) == 1;
				case "DateTime":
					return DateTime.Now.AddDays(_random.Next(-365, 365)).ToString("o");
				case "DateTimeOffset":
					return DateTimeOffset.Now.AddDays(_random.Next(-365, 365)).ToString("o");
				case "TimeSpan":
					return TimeSpan.FromMinutes(_random.Next(1, 1440)).ToString();
				case "Decimal":
					return Math.Round((decimal)(_random.NextDouble() * 10000), 2);
				case "Double":
					return Math.Round(_random.NextDouble() * 10000, 2);
				case "Single":
					return (float)Math.Round(_random.NextDouble() * 10000, 2);
				case "Char":
					return AlphaChars[_random.Next(AlphaChars.Length)].ToString();
				case "ByteArray":
					int byteLen = prop.Length > 0 ? _random.Next(1, prop.Length + 1) : _random.Next(1, 33);
					var bytes = new byte[byteLen];
					_random.NextBytes(bytes);
					return Convert.ToBase64String(bytes);
				case "StringList":
					var items = new List<string>();
					int listCount = _random.Next(1, 6);
					for (int i = 0; i < listCount; i++)
						items.Add(GenerateRandomString(_random.Next(3, 16)));
					return items;
				case "Object":
					return "{}";
				default:
					return null;
			}
		}

		private string GenerateRandomString(int length)
		{
			var sb = new StringBuilder(length);
			for (int i = 0; i < length; i++)
				sb.Append(AlphaChars[_random.Next(AlphaChars.Length)]);
			return sb.ToString();
		}

		#endregion

		#region JSON Serialization

		private string SerializeDataSet(Dictionary<string, List<Dictionary<string, object>>> entityData)
		{
			var sb = new StringBuilder();
			sb.AppendLine("{");

			var entityNames = entityData.Keys.ToList();
			for (int e = 0; e < entityNames.Count; e++)
			{
				var entityName = entityNames[e];
				var instances = entityData[entityName];

				sb.AppendLine($"\t\"{entityName}List\": [");

				for (int i = 0; i < instances.Count; i++)
				{
					SerializeObject(sb, instances[i], 2);
					if (i < instances.Count - 1)
						sb.AppendLine(",");
					else
						sb.AppendLine();
				}

				if (e < entityNames.Count - 1)
					sb.AppendLine("\t],");
				else
					sb.AppendLine("\t]");
			}

			sb.Append("}");
			return sb.ToString();
		}

		private void SerializeObject(StringBuilder sb, Dictionary<string, object> obj, int indent)
		{
			var prefix = new string('\t', indent);
			var innerPrefix = new string('\t', indent + 1);

			sb.AppendLine($"{prefix}{{");

			var keys = obj.Keys.ToList();
			for (int i = 0; i < keys.Count; i++)
			{
				var key = keys[i];
				var value = obj[key];
				sb.Append($"{innerPrefix}\"{EscapeJsonString(key)}\": ");
				SerializeValue(sb, value);

				if (i < keys.Count - 1)
					sb.AppendLine(",");
				else
					sb.AppendLine();
			}

			sb.Append($"{prefix}}}");
		}

		private void SerializeValue(StringBuilder sb, object value)
		{
			if (value == null)
			{
				sb.Append("null");
				return;
			}

			if (value is bool boolVal)
			{
				sb.Append(boolVal ? "true" : "false");
				return;
			}

			if (value is string strVal)
			{
				sb.Append($"\"{EscapeJsonString(strVal)}\"");
				return;
			}

			if (value is int || value is long || value is short || value is byte || value is sbyte
				|| value is uint || value is ulong || value is ushort)
			{
				sb.Append(value);
				return;
			}

			if (value is decimal decVal)
			{
				sb.Append(decVal);
				return;
			}

			if (value is double dblVal)
			{
				sb.Append(dblVal);
				return;
			}

			if (value is float fltVal)
			{
				sb.Append(fltVal);
				return;
			}

			if (value is List<string> listVal)
			{
				sb.Append("[");
				for (int i = 0; i < listVal.Count; i++)
				{
					sb.Append($"\"{EscapeJsonString(listVal[i])}\"");
					if (i < listVal.Count - 1)
						sb.Append(", ");
				}
				sb.Append("]");
				return;
			}

			sb.Append($"\"{EscapeJsonString(value.ToString())}\"");
		}

		private static string EscapeJsonString(string value)
		{
			if (string.IsNullOrEmpty(value))
				return value;

			return value
				.Replace("\\", "\\\\")
				.Replace("\"", "\\\"")
				.Replace("\n", "\\n")
				.Replace("\r", "\\r")
				.Replace("\t", "\\t");
		}

		#endregion

		private class ParentRelation
		{
			public string ParentEntityName { get; set; }
			public string FkPropertyName { get; set; }
			public string ParentPkPropertyName { get; set; }
		}
	}
}
