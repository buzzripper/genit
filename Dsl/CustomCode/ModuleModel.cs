using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Dyvenix.GenIt
{
    /// <summary>
    /// Partial class to extend ModuleModel with custom properties.
    /// </summary>
    public partial class ModuleModel
    {
        private const char PermissionsSeparator = '\n';

        /// <summary>
        /// Gets or sets the permissions as a List of strings.
        /// This is a convenience wrapper around the PermissionsStorage domain property.
        /// </summary>
        [Browsable(false)]
        public List<string> Permissions
        {
            get
            {
                if (string.IsNullOrEmpty(PermissionsStorage))
                    return new List<string>();

                return PermissionsStorage
                    .Split(new[] { PermissionsSeparator }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(p => p.Trim())
                    .Where(p => !string.IsNullOrEmpty(p))
                    .ToList();
            }
            set
            {
                if (value == null || value.Count == 0)
                {
                    PermissionsStorage = string.Empty;
                }
                else
                {
                    PermissionsStorage = string.Join(PermissionsSeparator.ToString(), value);
                }
            }
        }

        /// <summary>
        /// Read-only property for the property grid that displays the number of permissions.
        /// </summary>
        [Category("Security")]
        [DisplayName("Permissions")]
        [Description("Number of permissions defined for this module. Edit in the custom editor.")]
        public string PermissionsDisplay
        {
            get
            {
                int count = Permissions.Count;
                return $"{count} item{(count == 1 ? "" : "s")}";
            }
        }
    }
}
