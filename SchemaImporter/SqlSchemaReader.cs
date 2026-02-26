using Dyvenix.GenIt.SchemaImporter.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Dyvenix.GenIt.SchemaImporter
{
	internal class SqlSchemaReader
	{
		private readonly string _connectionString;

		internal SqlSchemaReader(string connectionString)
		{
			_connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
		}

		internal List<TableInfo> ReadTables(string schemaFilter = null)
		{
			var tables = new List<TableInfo>();
			var primaryKeys = ReadPrimaryKeys();
			var foreignKeys = ReadForeignKeyColumns();
			var identityColumns = ReadIdentityColumns();

			using (var conn = new SqlConnection(_connectionString))
			{
				conn.Open();

				// Read all tables
				var tableCmd = new SqlCommand(@"
					SELECT TABLE_SCHEMA, TABLE_NAME
					FROM INFORMATION_SCHEMA.TABLES
					WHERE TABLE_TYPE = 'BASE TABLE'
					ORDER BY TABLE_SCHEMA, TABLE_NAME", conn);

				using (var reader = tableCmd.ExecuteReader())
				{
					while (reader.Read())
					{
						var schema = reader.GetString(0);
						var tableName = reader.GetString(1);

						if (schemaFilter != null && !string.Equals(schema, schemaFilter, StringComparison.OrdinalIgnoreCase))
							continue;

						tables.Add(new TableInfo
						{
							Schema = schema,
							TableName = tableName
						});
					}
				}

				// Read columns for each table
				foreach (var table in tables)
				{
					var colCmd = new SqlCommand(@"
						SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH,
						       NUMERIC_PRECISION, NUMERIC_SCALE, IS_NULLABLE, ORDINAL_POSITION
						FROM INFORMATION_SCHEMA.COLUMNS
						WHERE TABLE_SCHEMA = @Schema AND TABLE_NAME = @TableName
						ORDER BY ORDINAL_POSITION", conn);

					colCmd.Parameters.AddWithValue("@Schema", table.Schema);
					colCmd.Parameters.AddWithValue("@TableName", table.TableName);

					using (var reader = colCmd.ExecuteReader())
					{
						while (reader.Read())
						{
							var columnName = reader.GetString(0);
							var fullTableName = $"{table.Schema}.{table.TableName}";
							var pkKey = $"{fullTableName}.{columnName}";
							var fkKey = pkKey;
							var identityKey = pkKey;

							table.Columns.Add(new ColumnInfo
							{
								ColumnName = columnName,
								DataType = reader.GetString(1),
								MaxLength = reader.IsDBNull(2) ? (int?)null : (int)reader.GetInt64(2),
								NumericPrecision = reader.IsDBNull(3) ? (int?)null : reader.GetByte(3),
								NumericScale = reader.IsDBNull(4) ? (int?)null : (int)reader.GetInt32(4),
								IsNullable = string.Equals(reader.GetString(5), "YES", StringComparison.OrdinalIgnoreCase),
								IsPrimaryKey = primaryKeys.Contains(pkKey),
								IsForeignKey = foreignKeys.Contains(fkKey),
								IsIdentity = identityColumns.Contains(identityKey),
								OrdinalPosition = reader.GetInt32(6)
							});
						}
					}
				}
			}

			return tables;
		}

		private HashSet<string> ReadPrimaryKeys()
		{
			var keys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

			using (var conn = new SqlConnection(_connectionString))
			{
				conn.Open();
				var cmd = new SqlCommand(@"
					SELECT ku.TABLE_SCHEMA, ku.TABLE_NAME, ku.COLUMN_NAME
					FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
					INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE ku
						ON tc.CONSTRAINT_NAME = ku.CONSTRAINT_NAME
						AND tc.TABLE_SCHEMA = ku.TABLE_SCHEMA
						AND tc.TABLE_NAME = ku.TABLE_NAME
					WHERE tc.CONSTRAINT_TYPE = 'PRIMARY KEY'", conn);

				using (var reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						keys.Add($"{reader.GetString(0)}.{reader.GetString(1)}.{reader.GetString(2)}");
					}
				}
			}

			return keys;
		}

		private HashSet<string> ReadForeignKeyColumns()
		{
			var keys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

			using (var conn = new SqlConnection(_connectionString))
			{
				conn.Open();
				var cmd = new SqlCommand(@"
					SELECT ku.TABLE_SCHEMA, ku.TABLE_NAME, ku.COLUMN_NAME
					FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
					INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE ku
						ON tc.CONSTRAINT_NAME = ku.CONSTRAINT_NAME
						AND tc.TABLE_SCHEMA = ku.TABLE_SCHEMA
						AND tc.TABLE_NAME = ku.TABLE_NAME
					WHERE tc.CONSTRAINT_TYPE = 'FOREIGN KEY'", conn);

				using (var reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						keys.Add($"{reader.GetString(0)}.{reader.GetString(1)}.{reader.GetString(2)}");
					}
				}
			}

			return keys;
		}

		private HashSet<string> ReadIdentityColumns()
		{
			var keys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

			using (var conn = new SqlConnection(_connectionString))
			{
				conn.Open();
				var cmd = new SqlCommand(@"
					SELECT s.name AS SchemaName, t.name AS TableName, c.name AS ColumnName
					FROM sys.columns c
					INNER JOIN sys.tables t ON c.object_id = t.object_id
					INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
					WHERE c.is_identity = 1", conn);

				using (var reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						keys.Add($"{reader.GetString(0)}.{reader.GetString(1)}.{reader.GetString(2)}");
					}
				}
			}

			return keys;
		}
	}
}
