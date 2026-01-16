using System.Collections.Generic;
using System.Linq;

namespace Dyvenix.GenIt
{
    /// <summary>
    /// Helper class for managing DataType values - a combination of primitive types
    /// and dynamic enum names from the model.
    /// </summary>
    public static class DataTypeHelper
    {
        /// <summary>
        /// Static list of primitive data types (never changes).
        /// </summary>
        public static readonly IReadOnlyList<string> PrimitiveTypes = new List<string>
        {
            "String",
            "Boolean",
            "Byte",
            "SByte",
            "Int16",
            "Int32",
            "Int64",
            "UInt16",
            "UInt32",
            "UInt64",
            "Single",
            "Double",
            "Decimal",
            "Char",
            "DateTime",
            "DateTimeOffset",
            "TimeSpan",
            "Guid",
            "ByteArray",
            "Object",
            "StringList"
        };

        /// <summary>
        /// Gets all available DataType values - primitives plus enum names from the model.
        /// </summary>
        public static List<string> GetAllDataTypes(Microsoft.VisualStudio.Modeling.Store store)
        {
            var result = new List<string>(PrimitiveTypes);

            if (store != null)
            {
                var enumNames = GetEnumNames(store);
                result.AddRange(enumNames);
            }

            return result;
        }

        /// <summary>
        /// Gets all enum names from the model.
        /// </summary>
        public static List<string> GetEnumNames(Microsoft.VisualStudio.Modeling.Store store)
        {
            var enumNames = new List<string>();

            if (store == null)
                return enumNames;

            try
            {
                var root = store.ElementDirectory.FindElements<ModelRoot>().FirstOrDefault();
                if (root != null)
                {
                    enumNames = root.Types
                        .OfType<EnumModel>()
                        .Where(e => !e.IsDeleting && !e.IsDeleted)
                        .Select(e => e.Name)
                        .Where(name => !string.IsNullOrWhiteSpace(name))
                        .OrderBy(name => name)
                        .ToList();
                }
            }
            catch
            {
                // If we can't read the model, return empty list
            }

            return enumNames;
        }

        /// <summary>
        /// Determines if the given DataType string represents an enum type.
        /// </summary>
        public static bool IsEnumType(string dataType)
        {
            if (string.IsNullOrWhiteSpace(dataType))
                return false;

            return !PrimitiveTypes.Contains(dataType);
        }

        /// <summary>
        /// Determines if the given DataType string is a string-related type that has length.
        /// </summary>
        public static bool HasLength(string dataType)
        {
            return dataType == "String" || dataType == "ByteArray";
        }
    }
}
