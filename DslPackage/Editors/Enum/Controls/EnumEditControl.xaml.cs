namespace Dyvenix.GenIt.DslPackage.Editors.Enum.Controls
{
    /// <summary>
    /// Editor control for EnumModel properties.
    /// Displays when an EnumModel is selected in the diagram.
    /// </summary>
    public partial class EnumEditControl : UserControlBase
    {
        private EnumModel _enumModel;

        public EnumEditControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes the control with the specified EnumModel.
        /// </summary>
        /// <param name="enumModel">The EnumModel to edit.</param>
        public void Initialize(EnumModel enumModel)
        {
            _enumModel = enumModel;
            // Future: Populate controls with EnumModel properties
        }
    }
}
