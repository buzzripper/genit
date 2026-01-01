using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Dyvenix.GenIt
{
    /// <summary>
    /// Partial class for EntityModel with additional helper properties
    /// </summary>
    [TypeDescriptionProvider(typeof(EntityModelTypeDescriptionProvider))]
    public partial class EntityModel
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
    }
}
