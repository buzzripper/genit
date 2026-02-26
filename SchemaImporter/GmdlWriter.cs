using Dyvenix.GenIt.SchemaImporter.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Dyvenix.GenIt.SchemaImporter
{
	/// <summary>
	/// Generates a .gmdl file compatible with the GenIt DSL Modeling SDK serializer.
	/// The XML format mirrors what ModelRootSerializer.Write() produces.
	/// </summary>
	internal class GmdlWriter
	{
		private const string Namespace = "http://schemas.microsoft.com/dsltools/GenIt";
		private const string DslToolsCoreNs = "http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core";
		private const string DslVersion = "1.0.0.0";

		private readonly string _modelName;

		internal GmdlWriter(string modelName)
		{
			_modelName = modelName ?? "ImportedModel";
		}

		internal void Write(List<TableInfo> tables, string outputPath)
		{
			var settings = new XmlWriterSettings
			{
				Indent = true,
				Encoding = Encoding.UTF8
			};

			using (var writer = XmlWriter.Create(outputPath, settings))
			{
				WriteModelRoot(writer, tables);
			}
		}

		private void WriteModelRoot(XmlWriter writer, List<TableInfo> tables)
		{
			writer.WriteStartElement("modelRoot", Namespace);
			writer.WriteAttributeString("xmlns", "dm0", null, DslToolsCoreNs);
			writer.WriteAttributeString("dslVersion", DslVersion);
			writer.WriteAttributeString("Id", NewGuid());
			writer.WriteAttributeString("name", _modelName);
			writer.WriteAttributeString("entitiesEnabled", "true");
			writer.WriteAttributeString("dbContextEnabled", "true");

			// Write types
			if (tables.Count > 0)
			{
				writer.WriteStartElement("types");

				foreach (var table in tables)
				{
					WriteEntityModel(writer, table);
				}

				writer.WriteEndElement(); // types
			}

			writer.WriteEndElement(); // modelRoot
		}

		private void WriteEntityModel(XmlWriter writer, TableInfo table)
		{
			// ModelRootHasTypes uses "full form" (UseFullForm="true")
			writer.WriteStartElement("modelRootHasTypes");
			writer.WriteAttributeString("Id", NewGuid());

			writer.WriteStartElement("entityModel");
			writer.WriteAttributeString("Id", NewGuid());
			writer.WriteAttributeString("name", ToEntityName(table.TableName));
			writer.WriteAttributeString("description", "");
			writer.WriteAttributeString("tableName", table.TableName);
			writer.WriteAttributeString("generateCode", "true");
			writer.WriteAttributeString("enabled", "true");

			bool hasRowVersion = table.Columns.Any(c => SqlTypeMapper.IsRowVersion(c.DataType));
			if (hasRowVersion)
			{
				writer.WriteAttributeString("inclRowVersion", "true");
			}

			// Write properties
			var columns = table.Columns
				.Where(c => !SqlTypeMapper.IsRowVersion(c.DataType))
				.OrderBy(c => c.OrdinalPosition)
				.ToList();

			if (columns.Count > 0)
			{
				writer.WriteStartElement("properties");

				int displayOrder = 0;
				foreach (var col in columns)
				{
					WritePropertyModel(writer, col, displayOrder++);
				}

				writer.WriteEndElement(); // properties
			}

			writer.WriteEndElement(); // entityModel
			writer.WriteEndElement(); // modelRootHasTypes
		}

		private void WritePropertyModel(XmlWriter writer, ColumnInfo column, int displayOrder)
		{
			var genItType = SqlTypeMapper.MapToGenItType(column.DataType);
			var length = SqlTypeMapper.GetLength(column.DataType, column.MaxLength);

			writer.WriteStartElement("propertyModel");
			writer.WriteAttributeString("Id", NewGuid());
			writer.WriteAttributeString("name", ToPascalCase(column.ColumnName));
			writer.WriteAttributeString("description", "");
			writer.WriteAttributeString("dataType", genItType);

			if (length > 0)
			{
				writer.WriteAttributeString("length", length.ToString(CultureInfo.InvariantCulture));
			}

			if (column.IsPrimaryKey)
			{
				writer.WriteAttributeString("isPrimaryKey", "true");
			}

			if (column.IsNullable)
			{
				writer.WriteAttributeString("isNullable", "true");
			}

			if (column.IsIdentity)
			{
				writer.WriteAttributeString("isIdentity", "true");
			}

			if (column.IsForeignKey)
			{
				writer.WriteAttributeString("isForeignKey", "true");
			}

			writer.WriteAttributeString("displayOrder", displayOrder.ToString(CultureInfo.InvariantCulture));

			writer.WriteEndElement(); // propertyModel
		}

		/// <summary>
		/// Converts a table name to a PascalCase entity name.
		/// Strips common prefixes like "tbl_" and singularizes simple plural forms.
		/// </summary>
		private static string ToEntityName(string tableName)
		{
			if (string.IsNullOrEmpty(tableName))
				return tableName;

			var name = tableName;

			// Strip common table prefixes
			if (name.StartsWith("tbl_", StringComparison.OrdinalIgnoreCase))
				name = name.Substring(4);
			else if (name.StartsWith("tbl", StringComparison.OrdinalIgnoreCase) && name.Length > 3 && char.IsUpper(name[3]))
				name = name.Substring(3);

			return ToPascalCase(name);
		}

		private static string ToPascalCase(string name)
		{
			if (string.IsNullOrEmpty(name))
				return name;

			// If the name contains underscores, split and capitalize each part
			if (name.Contains("_"))
			{
				var parts = name.Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
				return string.Concat(parts.Select(p =>
					char.ToUpperInvariant(p[0]) + (p.Length > 1 ? p.Substring(1) : "")));
			}

			// Otherwise just ensure the first character is uppercase
			return char.ToUpperInvariant(name[0]) + (name.Length > 1 ? name.Substring(1) : "");
		}

		private static string NewGuid()
		{
			return Guid.NewGuid().ToString("D", CultureInfo.InvariantCulture);
		}
	}
}
