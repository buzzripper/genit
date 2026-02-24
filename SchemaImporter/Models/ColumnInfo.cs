namespace Dyvenix.GenIt.SchemaImporter.Models
{
	internal class ColumnInfo
	{
		public string ColumnName { get; set; }
		public string DataType { get; set; }
		public int? MaxLength { get; set; }
		public int? NumericPrecision { get; set; }
		public int? NumericScale { get; set; }
		public bool IsNullable { get; set; }
		public bool IsPrimaryKey { get; set; }
		public bool IsIdentity { get; set; }
		public bool IsForeignKey { get; set; }
		public int OrdinalPosition { get; set; }
	}
}
