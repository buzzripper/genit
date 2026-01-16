using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Diagrams;
using System.Drawing;

namespace Dyvenix.GenIt
{
	/// <summary>
	/// Extension to GenItDiagram to support background color customization.
	/// </summary>
	public partial class GenItDiagram
	{
		/// <summary>
		/// Override to apply background color from ModelRoot when diagram initializes.
		/// </summary>
		public override void OnInitialize()
		{
			base.OnInitialize();
			ApplyBackgroundColorFromModel();
		}

		/// <summary>
		/// Called after deserialization to perform additional setup.
		/// </summary>
		public override void PostDeserialization(bool loadSucceeded)
		{
			base.PostDeserialization(loadSucceeded);
			if (loadSucceeded)
			{
				// Apply colors after the model is fully loaded
				ApplyBackgroundColorFromModel();
				ApplyConnectorColorsFromModel();
			}
		}

		/// <summary>
		/// Applies the background color from the associated ModelRoot.
		/// </summary>
		public void ApplyBackgroundColorFromModel()
		{
			if (this.ModelElement is ModelRoot modelRoot)
			{
				Color bgColor = modelRoot.DiagramBackgroundColor;
				if (bgColor != Color.Empty && bgColor != Color.Transparent)
				{
					SetBackgroundColorInternal(bgColor);
				}
			}
		}

		/// <summary>
		/// Applies connector colors to all connectors on this diagram.
		/// </summary>
		public void ApplyConnectorColorsFromModel()
		{
			foreach (var shape in this.NestedChildShapes)
			{
				if (shape is AssociationConnector assocConnector)
				{
					assocConnector.ApplyLineColorFromModel();
				}
			}
		}

		/// <summary>
		/// Sets the background color of the diagram (internal, no transaction).
		/// </summary>
		private void SetBackgroundColorInternal(Color color)
		{
			// Use the StyleSet to override the diagram background brush
			BrushSettings brushSettings = new BrushSettings();
			brushSettings.Color = color;
			this.StyleSet.OverrideBrush(DiagramBrushes.DiagramBackground, brushSettings);
			this.Invalidate(true);
		}
	}

	/// <summary>
	/// Rule to update diagram background color when ModelRoot.DiagramBackgroundColor changes.
	/// </summary>
	[RuleOn(typeof(ModelRoot), FireTime = TimeToFire.TopLevelCommit)]
	internal sealed class DiagramBackgroundColorChangeRule : ChangeRule
	{
		public override void ElementPropertyChanged(ElementPropertyChangedEventArgs e)
		{
			if (e.DomainProperty.Id == ModelRoot.DiagramBackgroundColorDomainPropertyId)
			{
				ModelRoot modelRoot = (ModelRoot)e.ModelElement;
				Color newColor = (Color)e.NewValue;

				// Find all diagrams associated with this model root and update their background
				foreach (var pel in PresentationViewsSubject.GetPresentation(modelRoot))
				{
					if (pel is GenItDiagram diagram)
					{
						diagram.ApplyBackgroundColorFromModel();
					}
				}
			}
		}
	}
}
