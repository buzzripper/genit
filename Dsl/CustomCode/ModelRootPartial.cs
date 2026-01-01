using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Dyvenix.GenIt
{
    /// <summary>
    /// Partial class for ModelRoot with additional helper properties
    /// </summary>
    public partial class ModelRoot
    {
        /// <summary>
        /// Gets a list of using statement strings parsed from the DbContextUsings property.
        /// Each line in the DbContextUsings string becomes an item in the list.
        /// Empty lines are skipped.
        /// </summary>
        [Browsable(false)]
        public List<string> DbContextUsingsList
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.DbContextUsings))
                    return new List<string>();

                return this.DbContextUsings
                    .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(line => line.Trim())
                    .Where(line => !string.IsNullOrWhiteSpace(line))
                    .ToList();
            }
        }
    }
}
