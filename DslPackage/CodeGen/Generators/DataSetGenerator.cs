using Dyvenix.GenIt.DslPackage.CodeGen.Misc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dyvenix.GenIt.DslPackage.CodeGen.Generators
{
	internal class DataSetGenerator
	{
		private readonly ModelRoot _modelRoot;
		private readonly List<EntityModel> _entities;
		private readonly List<EnumModel> _enums;
		private readonly string _entitiesNamespace;
		private readonly string _namespace;
		private readonly string _outputFolderpath;
		private readonly bool _inclHeader;

		internal DataSetGenerator(ModelRoot modelRoot)
		{
			_modelRoot = modelRoot;
			_entities = modelRoot.Types.OfType<EntityModel>().Where(e => e.GenerateCode).ToList();
			_enums = modelRoot.Types.OfType<EnumModel>().ToList();
			_entitiesNamespace = modelRoot.EntitiesNamespace;
			_namespace = $"{modelRoot.IntTestsNamespace}.DataSets";
			_inclHeader = modelRoot.InclHeader;
		}

		internal void GenerateCode()
		{
			if (_entities.Count == 0)
				return;

			var fileContent = new List<string>();

			if (_inclHeader)
				fileContent.Add(CodeGenUtils.FileHeader);

			// Usings
			fileContent.AddLine(0, "using System;");
			fileContent.AddLine(0, "using System.Collections.Generic;");
			if (!string.IsNullOrEmpty(_entitiesNamespace))
				fileContent.AddLine(0, $"using {_entitiesNamespace};");
			fileContent.AddLine(0, $"using {_modelRoot.IntTestsNamespace}.Data;");

			// Namespace
			fileContent.AddLine();
			fileContent.AddLine(0, $"namespace {_namespace};");

			// Class declaration
			fileContent.AddLine();
			fileContent.AddLine(0, "public class DefaultDataSet : IDataSet");
			fileContent.AddLine(0, "{");

			// Initialize method
			fileContent.AddLine(1, "public DefaultDataSet()");
			fileContent.AddLine(1, "{");
			foreach (var entity in _entities)
			{
				fileContent.AddLine(2, $"{entity.Name}List = Create{entity.Name}List();");
			}
			fileContent.AddLine(1, "}");
			fileContent.AddLine();

			// Properties - one List<Entity> per entity
			fileContent.AddLine(1, "#region Properties");
			fileContent.AddLine();
			fileContent.AddLine(1, $"public DataSetType DataSetType => DataSetType.Default;");
			fileContent.AddLine();
			foreach (var entity in _entities)
				fileContent.AddLine(1, $"public List<{entity.Name}> {entity.Name}List {{ get; set; }} = new List<{entity.Name}>();");
			fileContent.AddLine();
			fileContent.AddLine(1, "#endregion");

			// Per-entity creation methods
			foreach (var entity in _entities)
			{
				GenerateEntityCreateMethod(entity, fileContent);
			}

			fileContent.AddLine(0, "}");

			// Write DataSet file
			var filepath = Path.Combine(FileHelper.GetAbsolutePath(_modelRoot.IntTestsOutputFolder), "DataSets", "DefaultDataSet.cs");
			FileHelper.SaveFile(filepath, fileContent.AsString());
		}

		private void GenerateEntityCreateMethod(EntityModel entity, List<string> fileContent)
		{
			var props = entity.Properties.ToList();
			var navProps = entity.NavigationProperties.ToList();

			var pkProps = props.Where(p => p.IsPrimaryKey).ToList();
			var fkProps = props.Where(p => p.IsForeignKey).ToList();
			var rowVersionProps = props.Where(p => p.IsRowVersion).ToList();
			var regularProps = props.Where(p => !p.IsPrimaryKey && !p.IsForeignKey && !p.IsRowVersion).ToList();
			var nullableProps = regularProps.Where(p => p.IsNullable).ToList();
			var requiredProps = regularProps.Where(p => !p.IsNullable).ToList();

			int rowCount = Math.Max(5, fkProps.Count > 0 ? 6 : 5);

			fileContent.AddLine();
			fileContent.AddLine(1, $"private List<{entity.Name}> Create{entity.Name}List()");
			fileContent.AddLine(1, "{");
			fileContent.AddLine(2, $"return new List<{entity.Name}>");
			fileContent.AddLine(2, "{");

			for (int i = 0; i < rowCount; i++)
			{
				fileContent.AddLine(3, $"new {entity.Name}");
				fileContent.AddLine(3, "{");

				foreach (var prop in pkProps)
				{
					var value = GenerateValue(prop, i, entity.Name, true);
					fileContent.AddLine(4, $"{prop.Name} = {value},");
				}

				foreach (var prop in fkProps)
				{
					if (prop.IsNullable && i == rowCount - 1)
						fileContent.AddLine(4, $"{prop.Name} = null,");
					else
					{
						var value = GenerateFkValue(prop, i);
						fileContent.AddLine(4, $"{prop.Name} = {value},");
					}
				}

				foreach (var prop in requiredProps)
				{
					var value = GenerateValue(prop, i, entity.Name, false);
					fileContent.AddLine(4, $"{prop.Name} = {value},");
				}

				foreach (var prop in nullableProps)
				{
					if (i % 3 == 2)
						fileContent.AddLine(4, $"{prop.Name} = null,");
					else
					{
						var value = GenerateValue(prop, i, entity.Name, false);
						fileContent.AddLine(4, $"{prop.Name} = {value},");
					}
				}

				foreach (var prop in rowVersionProps)
				{
					fileContent.AddLine(4, $"{prop.Name} = new byte[] {{ {(i + 1)}, 0, 0, 0, 0, 0, 0, {(i + 1)} }},");
				}

				foreach (var navProp in navProps)
				{
					if (navProp.IsCollection && i % 2 == 0)
						fileContent.AddLine(4, $"{navProp.Name} = new List<{navProp.TargetEntityName}>(),");
				}

				fileContent.AddLine(3, "},");
			}

			fileContent.AddLine(2, "};");
			fileContent.AddLine(1, "}");
		}

		private string GenerateValue(PropertyModel prop, int index, string entityName, bool isForPk)
		{
			var dt = prop.DataType;

			if (IsEnumType(dt))
			{
				var enumModel = _enums.FirstOrDefault(e => e.Name == dt);
				if (enumModel != null && enumModel.Members.Count > 0)
				{
					var memberIndex = index % enumModel.Members.Count;
					return $"{dt}.{enumModel.Members[memberIndex].Name}";
				}
				return $"({dt})({index})";
			}

			if (dt == DataTypes.Guid)
			{
				if (isForPk)
					return $"new Guid(\"0000000{index + 1}-0000-0000-0000-{entityName.PadRight(12, '0').Substring(0, 12)}\")";
				return "Guid.NewGuid()";
			}
			if (dt == DataTypes.String)
				return GenerateStringValue(prop, index, entityName);
			if (dt == DataTypes.Int32)
				return isForPk ? $"{index + 1}" : $"{(index + 1) * 10 + index}";
			if (dt == DataTypes.Int64)
				return isForPk ? $"{index + 1}L" : $"{(long)(index + 1) * 100 + index}L";
			if (dt == DataTypes.Int16)
				return $"(short){index + 1}";
			if (dt == DataTypes.Boolean)
				return index % 2 == 0 ? "true" : "false";
			if (dt == DataTypes.DateTime)
				return $"new DateTime(2024, {(index % 12) + 1}, {(index % 28) + 1}, {index % 24}, {index % 60}, 0)";
			if (dt == DataTypes.DateTimeOffset)
				return $"new DateTimeOffset(2024, {(index % 12) + 1}, {(index % 28) + 1}, {index % 24}, {index % 60}, 0, TimeSpan.Zero)";
			if (dt == DataTypes.Decimal)
				return $"{(index + 1) * 10}.{index + 1:00}m";
			if (dt == DataTypes.Double)
				return $"{(index + 1) * 10}.{index + 1}d";
			if (dt == DataTypes.Single)
				return $"{(index + 1) * 10}.{index + 1}f";
			if (dt == DataTypes.Byte)
				return $"(byte){index + 1}";
			if (dt == DataTypes.SByte)
				return $"(sbyte){index + 1}";
			if (dt == DataTypes.Char)
				return $"'{(char)('A' + index)}'";
			if (dt == DataTypes.UInt16)
				return $"(ushort){index + 1}";
			if (dt == DataTypes.UInt32)
				return $"{index + 1}u";
			if (dt == DataTypes.UInt64)
				return $"{index + 1}ul";
			if (dt == DataTypes.TimeSpan)
				return $"TimeSpan.FromHours({index + 1})";
			if (dt == DataTypes.ByteArray)
				return $"new byte[] {{ {index + 1}, {index + 2}, {index + 3} }}";
			if (dt == DataTypes.StringList)
				return $"new List<string> {{ \"item{index + 1}a\", \"item{index + 1}b\" }}";
			if (dt == DataTypes.Object)
				return "null";

			return $"\"{entityName}_{prop.Name}_{index + 1}\"";
		}

		private string GenerateStringValue(PropertyModel prop, int index, string entityName)
		{
			var maxLen = prop.Length > 0 ? prop.Length : 0;
			var baseName = $"{entityName}_{prop.Name}";

			if (maxLen > 0)
			{
				var variant = index % 5;
				if (variant == 0)
				{
					var normal = $"{baseName}_{index + 1}";
					if (normal.Length > maxLen) normal = normal.Substring(0, maxLen);
					return $"\"{normal}\"";
				}
				if (variant == 1)
					return $"\"{(char)('A' + index)}\"";
				if (variant == 2)
				{
					var maxStr = new string('X', Math.Min(maxLen, 200));
					return $"\"{maxStr}\"";
				}
				if (variant == 3)
				{
					var spaced = $"Test Value {index + 1}";
					if (spaced.Length > maxLen) spaced = spaced.Substring(0, maxLen);
					return $"\"{spaced}\"";
				}
				var special = $"{baseName}-{index + 1}!";
				if (special.Length > maxLen) special = special.Substring(0, maxLen);
				return $"\"{special}\"";
			}

			var v = index % 4;
			if (v == 0) return $"\"{baseName}_{index + 1}\"";
			if (v == 1) return $"\"{(char)('A' + index)}\"";
			if (v == 2) return $"\"Test Value {index + 1}\"";
			return $"\"{baseName}-{index + 1}!\"";
		}

		private string GenerateFkValue(PropertyModel prop, int index)
		{
			var dt = prop.DataType;
			var fkIndex = (index % 3) + 1;

			if (dt == DataTypes.Guid)
				return $"new Guid(\"0000000{fkIndex}-0000-0000-0000-{prop.Name.PadRight(12, '0').Substring(0, 12)}\")";
			if (dt == DataTypes.Int32)
				return $"{fkIndex}";
			if (dt == DataTypes.Int64)
				return $"{fkIndex}L";
			if (dt == DataTypes.Int16)
				return $"(short){fkIndex}";

			return $"{fkIndex}";
		}

		private bool IsEnumType(string dataType)
		{
			if (string.IsNullOrEmpty(dataType))
				return false;

			if (dataType == DataTypes.String) return false;
			if (dataType == DataTypes.Int32) return false;
			if (dataType == DataTypes.Boolean) return false;
			if (dataType == DataTypes.DateTime) return false;
			if (dataType == DataTypes.Guid) return false;
			if (dataType == DataTypes.StringList) return false;
			if (dataType == DataTypes.TimeSpan) return false;
			if (dataType == DataTypes.DateTimeOffset) return false;
			if (dataType == DataTypes.Object) return false;
			if (dataType == DataTypes.Decimal) return false;
			if (dataType == DataTypes.Char) return false;
			if (dataType == DataTypes.ByteArray) return false;
			if (dataType == DataTypes.Byte) return false;
			if (dataType == DataTypes.SByte) return false;
			if (dataType == DataTypes.Int16) return false;
			if (dataType == DataTypes.Int64) return false;
			if (dataType == DataTypes.UInt16) return false;
			if (dataType == DataTypes.UInt32) return false;
			if (dataType == DataTypes.UInt64) return false;
			if (dataType == DataTypes.Single) return false;
			if (dataType == DataTypes.Double) return false;

			return true;
		}

		private List<string> GenerateIDataSet(List<string> interfaceProps)
		{
			var fileContents = new List<string>();

			if (_inclHeader)
				fileContents.Add(CodeGenUtils.FileHeader);

			// Usings
			fileContents.AddLine(0, $"using {_entitiesNamespace};");
			var p = _modelRoot.IntTestsNamespace.IndexOf(".Integration");
			if (p > -1)
				fileContents.AddLine(0, $"using {_modelRoot.IntTestsNamespace.Substring(0, p)}.Common.Data;");  // Hack

			// Namespace
			fileContents.AddLine();
			fileContents.AddLine(0, $"namespace {_namespace};");

			// Class declaration
			fileContents.AddLine();
			fileContents.AddLine(0, "public interface IDataSet");
			fileContents.AddLine(0, "{");
			fileContents.AddLine(1, "DataSetType DataSetType { get; }");
			fileContents.AddLine();
			fileContents.AddLines(1, interfaceProps);
			fileContents.AddLine(0, "}");

			return fileContents;
		}
	}
}
