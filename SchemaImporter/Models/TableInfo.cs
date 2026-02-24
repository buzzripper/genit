using System.Collections.Generic;

namespace Dyvenix.GenIt.SchemaImporter.Models
{
	internal class TableInfo
	{
		public string Schema { get; set; }
		public string TableName { get; set; }
		public List<ColumnInfo> Columns { get; set; } = new List<ColumnInfo>();
	}
}
