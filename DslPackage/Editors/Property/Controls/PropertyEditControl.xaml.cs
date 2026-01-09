using System.Windows.Controls;

namespace Dyvenix.GenIt.DslPackage.Editors.Property.Controls
{
    /// <summary>
    /// Editor control for PropertyModel properties.
    /// Displays when a PropertyModel is selected in a compartment.
    /// </summary>
    public partial class PropertyEditControl : UserControlBase
    {
        private PropertyModel _propertyModel;

        public PropertyEditControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes the control with the specified PropertyModel.
        /// </summary>
        /// <param name="propertyModel">The PropertyModel to edit.</param>
        public void Initialize(PropertyModel propertyModel)
        {
            _propertyModel = propertyModel;
            // Future: Populate controls with PropertyModel properties
        }
    }
}
