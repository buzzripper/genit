using Dyvenix.GenIt.DslPackage.Editors.Association.Controls;
using Dyvenix.GenIt.DslPackage.Editors.Entity.Controls;
using Dyvenix.GenIt.DslPackage.Editors.Enum.Controls;
using Dyvenix.GenIt.DslPackage.Editors.Model.Controls;
using System.Windows;
using System.Windows.Controls;

namespace Dyvenix.GenIt.DslPackage.Editors
{
    /// <summary>
    /// Host control for the GenItEditorWindow tool window.
    /// Manages visibility of different editor controls based on selection.
    /// </summary>
    public partial class GenItEditorWindowControl : UserControl
    {
        public event System.EventHandler<string> CaptionRequested;

        private void RequestCaption(string caption)
        {
            CaptionRequested?.Invoke(this, caption);
        }

        private ModelRootEditControl _modelRootEditControl;
        private EntityEditControl _entityEditControl;
        private AssociationEditControl _associationEditControl;
        private EnumEditControl _enumEditControl;

        public GenItEditorWindowControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Shows the ModelRoot editor with the specified model root.
        /// </summary>
        /// <param name="modelRoot">The ModelRoot to display.</param>
        public void ShowModelRootEditor(ModelRoot modelRoot)
        {
            if (modelRoot != null)
            {
                // Lazy-create the model root edit control
                if (_modelRootEditControl == null)
                {
                    _modelRootEditControl = new ModelRootEditControl();
                }

                _modelRootEditControl.Initialize(modelRoot);
                editorContentHost.Content = _modelRootEditControl;
                editorContentHost.Visibility = Visibility.Visible;
                txtNoSelection.Visibility = Visibility.Collapsed;

				RequestCaption("GenIt - Model");
            }
            else
            {
                HideEditor();
            }
        }

        /// <summary>
        /// Shows the Entity editor with the specified entity model.
        /// </summary>
        /// <param name="entityModel">The EntityModel to display.</param>
        public void ShowEntityEditor(EntityModel entityModel)
        {
            if (entityModel != null)
            {
                // Lazy-create the entity edit control
                if (_entityEditControl == null)
                {
                    _entityEditControl = new EntityEditControl();
                }

                _entityEditControl.Initialize(entityModel);
                editorContentHost.Content = _entityEditControl;
                editorContentHost.Visibility = Visibility.Visible;
                txtNoSelection.Visibility = Visibility.Collapsed;

				RequestCaption("GenIt - Entity");
            }
            else
            {
                HideEditor();
            }
        }

		public void ShowEntityEditor(EntityModel entityModel, PropertyModel selectedPropertyModel)
		{
			ShowEntityEditor(entityModel);

			if (entityModel != null && selectedPropertyModel != null && _entityEditControl != null)
			{
				_entityEditControl.SelectProperty(selectedPropertyModel);
			}
		}

        /// <summary>
        /// Shows the Association editor with the specified association.
        /// </summary>
        /// <param name="association">The Association to display.</param>
        public void ShowAssociationEditor(Dyvenix.GenIt.Association association)
        {
            if (association != null)
            {
                // Lazy-create the association edit control
                if (_associationEditControl == null)
                {
                    _associationEditControl = new AssociationEditControl();
                }

                _associationEditControl.Initialize(association);
                editorContentHost.Content = _associationEditControl;
                editorContentHost.Visibility = Visibility.Visible;
                txtNoSelection.Visibility = Visibility.Collapsed;
            }
            else
            {
                HideEditor();
            }
        }

        /// <summary>
        /// Shows the Enum editor with the specified enum model.
        /// </summary>
        /// <param name="enumModel">The EnumModel to display.</param>
        public void ShowEnumEditor(EnumModel enumModel)
        {
            if (enumModel != null)
            {
                // Lazy-create the enum edit control
                if (_enumEditControl == null)
                {
                    _enumEditControl = new EnumEditControl();
                }

                _enumEditControl.Initialize(enumModel);
                editorContentHost.Content = _enumEditControl;
                editorContentHost.Visibility = Visibility.Visible;
                txtNoSelection.Visibility = Visibility.Collapsed;
            }
            else
            {
                HideEditor();
            }
        }

        /// <summary>
        /// Hides the current editor and shows the "No item selected" message.
        /// </summary>
        public void HideEditor()
        {
            editorContentHost.Content = null;
            editorContentHost.Visibility = Visibility.Collapsed;
            txtNoSelection.Visibility = Visibility.Hidden;

			RequestCaption("GenIt");
        }
    }
}
