using System.Windows.Controls;

namespace Dyvenix.GenIt.DslPackage.Editors.Entity.Controls
{
    /// <summary>
    /// Editor control for EntityModel properties.
    /// Displays when an EntityModel shape is selected.
    /// </summary>
    public partial class EntityEditControl : UserControlBase
    {
        private EntityModel _entityModel;

        public EntityEditControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes the control with the specified EntityModel.
        /// </summary>
        /// <param name="entityModel">The EntityModel to edit.</param>
        public void Initialize(EntityModel entityModel)
        {
            _entityModel = entityModel;
            // Future: Populate controls with EntityModel properties
        }
    }
}
