using System.Windows.Controls;

namespace Dyvenix.GenIt.DslPackage.Editors.Model.Controls
{
    /// <summary>
    /// Editor control for ModelRoot properties.
    /// Displays when the diagram surface is clicked.
    /// </summary>
    public partial class ModelRootEditControl : UserControlBase
    {
        private ModelRoot _modelRoot;

        public ModelRootEditControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes the control with the specified ModelRoot.
        /// </summary>
        /// <param name="modelRoot">The ModelRoot to edit.</param>
        public void Initialize(ModelRoot modelRoot)
        {
            _modelRoot = modelRoot;
            // Future: Populate controls with ModelRoot properties
        }
    }
}
