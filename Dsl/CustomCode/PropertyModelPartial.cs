using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Dyvenix.GenIt
{
    /// <summary>
    /// Partial class for PropertyModel with additional helper properties
    /// </summary>
    public partial class PropertyModel
    {
        /// <summary>
        /// Gets a list of attribute strings parsed from the Attributes property.
        /// Each line in the Attributes string becomes an item in the list.
        /// Empty lines are skipped.
        /// </summary>
        [Browsable(false)]
        public List<string> AttributesList
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.Attributes))
                    return new List<string>();

                return this.Attributes
                    .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(line => line.Trim())
                    .Where(line => !string.IsNullOrWhiteSpace(line))
                    .ToList();
            }
        }

        /// <summary>
        /// Gets a list of using statement strings parsed from the Usings property.
        /// Each line in the Usings string becomes an item in the list.
        /// Empty lines are skipped.
        /// </summary>
        [Browsable(false)]
        public List<string> UsingsList
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.Usings))
                    return new List<string>();

                return this.Usings
                    .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(line => line.Trim())
                    .Where(line => !string.IsNullOrWhiteSpace(line))
                    .ToList();
            }
        }

        [Browsable(false)]
        public string CSType
        {
            get
            {
                if (string.IsNullOrEmpty(this.DataType))
                    return "EMPTY";
                if (this.DataType == "String")
                    return "string";
                if (this.DataType == "Int32")
                    return "int";
                if (this.DataType == "Boolean")
                    return "bool";
                if (this.DataType == "DateTime")
                    return "DateTime";
                if (this.DataType == "Guid")
                    return "Guid";
                if (this.DataType == "StringList")
                    return "List<string>";
                if (this.DataType == "TimeSpan")
                    return "TimeSpan";
                if (this.DataType == "DateTimeOffset")
                    return "DateTimeOffset";
                if (this.DataType == "Object")
                    return "object";
                if (this.DataType == "Decimal")
                    return "decimal";
                if (this.DataType == "Char")
                    return "char";
                if (this.DataType == "ByteArray")
                    return "byte[]";
                if (this.DataType == "Byte")
                    return "byte";
                if (this.DataType == "SByte")
                    return "sbyte";
                if (this.DataType == "Int16")
                    return "short";
                if (this.DataType == "Int64")
                    return "long";
                if (this.DataType == "UInt16")
                    return "ushort";
                if (this.DataType == "UInt32")
                    return "uint";
                if (this.DataType == "UInt64")
                    return "ulong";
                if (this.DataType == "Single")
                    return "float";
                if (this.DataType == "Double")
                    return "double";
                return this.DataType;
            }
        }

        [Browsable(false)]
        public string TSType
        {
            get
            {
                if (this.DataType == "String")
                    return "string";
                if (this.DataType == "Int32")
                    return "number";
                if (this.DataType == "Boolean")
                    return "boolean";
                if (this.DataType == "DateTime")
                    return "Date";
                if (this.DataType == "Guid")
                    return "string";
                if (this.DataType == "StringList")
                    return "string[]";
                if (this.DataType == "TimeSpan")
                    return "string";
                if (this.DataType == "DateTimeOffset")
                    return "Date";
                if (this.DataType == "Object")
                    return "any";
                if (this.DataType == "Decimal")
                    return "number";
                if (this.DataType == "Char")
                    return "string";
                if (this.DataType == "ByteArray")
                    return "Uint8Array";
                if (this.DataType == "Byte")
                    return "number";
                if (this.DataType == "SByte")
                    return "number";
                if (this.DataType == "Int16")
                    return "number";
                if (this.DataType == "Int64")
                    return "number";
                if (this.DataType == "UInt16")
                    return "number";
                if (this.DataType == "UInt32")
                    return "number";
                if (this.DataType == "UInt64")
                    return "number";
                if (this.DataType == "Single")
                    return "number";
                if (this.DataType == "Double")
                    return "number";
                if (string.IsNullOrEmpty(this.DataType))
                    return "EMPTY";
                return this.DataType;
            }
        }

        [Browsable(false)]
        public bool RequiresInit
        {
            get
            {
                if (this.IsNullable)
                    return false;

                if (this.DataType == DataTypes.String || this.DataType == DataTypes.StringList || this.DataType == DataTypes.ByteArray || this.DataType == DataTypes.Object)
                    return true;

                return false;
            }
        }
    }
}
