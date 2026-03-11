using System;

namespace Dyvenix.GenIt.SchemaImporter
{
    class Program
    {
        static int Main(string[] args)
        {
            string connectionString = "Data Source=localhost;Initial Catalog=Auth;Integrated Security=True;Encrypt=False;"; ;
            string outputPath = "D:\\Active\\AuthTest.gmdl"; string modelName = "AuthTest";
            string schemaFilter = "dbo";

            try
            {
                Console.WriteLine($"Connecting to database...");

                var schemaReader = new SqlSchemaReader(connectionString);
                var tables = schemaReader.ReadTables(schemaFilter);

                Console.WriteLine($"Found {tables.Count} table(s).");

                if (tables.Count == 0)
                {
                    Console.WriteLine("No tables found. Check the connection string and schema filter.");
                    return 1;
                }

                foreach (var table in tables)
                {
                    Console.WriteLine($"  [{table.Schema}].[{table.TableName}] - {table.Columns.Count} column(s)");
                }

                var writer = new GmdlWriter(modelName);
                writer.Write(tables, outputPath);

                Console.WriteLine($"Successfully wrote {outputPath}");
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}");
                Console.ReadLine();
                return 1;
            }
        }

        private static void PrintUsage()
        {
            Console.WriteLine("GenIt Schema Importer - Creates .gmdl files from SQL Server database schemas");
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine("  SchemaImporter <connectionString> <outputFile> [modelName] [schemaFilter]");
            Console.WriteLine();
            Console.WriteLine("Arguments:");
            Console.WriteLine("  connectionString  SQL Server connection string");
            Console.WriteLine("  outputFile        Path for the output .gmdl file");
            Console.WriteLine("  modelName         (Optional) Name for the model root. Defaults to the output file name.");
            Console.WriteLine("  schemaFilter      (Optional) Only include tables from this schema (e.g. 'dbo')");
            Console.WriteLine();
            Console.WriteLine("Example:");
            Console.WriteLine("  SchemaImporter \"Server=localhost;Database=MyDb;Integrated Security=true\" MyModel.gmdl MyModel dbo");
        }
    }
}
