using System.Windows.Controls;

namespace Dyvenix.GenIt.DslPackage.Editors.Association.Controls
{
    /// <summary>
    /// Editor control for Association properties.
    /// Displays when an Association connector is selected.
    /// </summary>
    public partial class AssociationEditControl : UserControlBase
    {
        private Dyvenix.GenIt.Association _association;

        public AssociationEditControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes the control with the specified Association.
        /// </summary>
        /// <param name="association">The Association to edit.</param>
        public void Initialize(Dyvenix.GenIt.Association association)
        {
            _association = association;
            // Future: Populate controls with Association properties
        }
    }
}
