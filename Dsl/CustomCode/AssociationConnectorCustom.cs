using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Diagrams;
using System.Drawing;

namespace Dyvenix.GenIt
{
	/// <summary>
	/// Custom partial class for AssociationConnector to support dynamic line color from ModelRoot.
	/// </summary>
	public partial class AssociationConnector
	{
		/// <summary>
		/// Override InitializeInstanceResources to set the line color from ModelRoot.
		/// </summary>
		protected override void InitializeInstanceResources()
		{
			base.InitializeInstanceResources();
			ApplyLineColorFromModel();
		}

		/// <summary>
		/// Applies the line color from the associated ModelRoot.
		/// </summary>
		public void ApplyLineColorFromModel()
		{
			ModelRoot modelRoot = GetModelRoot();
			if (modelRoot != null)
			{
				Color lineColor = modelRoot.AssociationLineColor;
				if (lineColor != Color.Empty && lineColor != Color.Transparent)
				{
					SetLineColor(lineColor);
				}
			}
		}

		/// <summary>
		/// Sets the line color, decorator color, and text decorator color for this connector instance.
		/// </summary>
		private void SetLineColor(Color color)
		{
			// Override the pen for the connection line
			PenSettings penSettings = new PenSettings();
			penSettings.Color = color;
			this.StyleSet.OverridePen(DiagramPens.ConnectionLine, penSettings);

			// Override the pen for connection line decorators (arrows, shapes)
			this.StyleSet.OverridePen(DiagramPens.ConnectionLineDecorator, penSettings);

			// Override the brush for filled decorators
			BrushSettings brushSettings = new BrushSettings();
			brushSettings.Color = color;
			this.StyleSet.OverrideBrush(DiagramBrushes.ConnectionLineDecorator, brushSettings);

			// Override the text brush for text on this connector (like '*' multiplicity)
			this.StyleSet.OverrideBrush(DiagramBrushes.ShapeText, brushSettings);
		}

		/// <summary>
		/// Gets the ModelRoot associated with this connector.
		/// </summary>
		private ModelRoot GetModelRoot()
		{
			// Get the underlying Association relationship
			Association association = this.ModelElement as Association;
			if (association != null)
			{
				// Get the source entity and navigate to ModelRoot
				EntityModel sourceEntity = association.Source;
				if (sourceEntity != null)
				{
					return sourceEntity.ModelRoot;
				}
			}

			return null;
		}
	}

	/// <summary>
	/// Custom partial class for EnumAssociationConnector to support dynamic line color from ModelRoot.
	/// </summary>
	public partial class EnumAssociationConnector
	{
		/// <summary>
		/// Override InitializeInstanceResources to set the line color from ModelRoot.
		/// </summary>
		protected override void InitializeInstanceResources()
		{
			base.InitializeInstanceResources();
			ApplyLineColorFromModel();
		}

		/// <summary>
		/// Applies the line color from the associated ModelRoot.
		/// </summary>
		public void ApplyLineColorFromModel()
		{
			ModelRoot modelRoot = GetModelRoot();
			if (modelRoot != null)
			{
				Color lineColor = modelRoot.AssociationLineColor;
				if (lineColor != Color.Empty && lineColor != Color.Transparent)
				{
					SetLineColor(lineColor);
				}
			}
		}

		/// <summary>
		/// Sets the line color, decorator color, and text decorator color for this connector instance.
		/// </summary>
		private void SetLineColor(Color color)
		{
			// Override the pen for the connection line
			PenSettings penSettings = new PenSettings();
			penSettings.Color = color;
			penSettings.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash; // Keep dashed style
			this.StyleSet.OverridePen(DiagramPens.ConnectionLine, penSettings);

			// Override the pen for connection line decorators (arrows, shapes)
			PenSettings decoratorPen = new PenSettings();
			decoratorPen.Color = color;
			this.StyleSet.OverridePen(DiagramPens.ConnectionLineDecorator, decoratorPen);

			// Override the brush for filled decorators
			BrushSettings brushSettings = new BrushSettings();
			brushSettings.Color = color;
			this.StyleSet.OverrideBrush(DiagramBrushes.ConnectionLineDecorator, brushSettings);

			// Override the text brush for text on this connector
			this.StyleSet.OverrideBrush(DiagramBrushes.ShapeText, brushSettings);
		}

		/// <summary>
		/// Gets the ModelRoot associated with this connector.
		/// </summary>
		private ModelRoot GetModelRoot()
		{
			// Get the underlying EnumAssociation relationship
			EnumAssociation association = this.ModelElement as EnumAssociation;
			if (association != null)
			{
				// Get the entity and navigate to ModelRoot
				EntityModel entity = association.Entity;
				if (entity != null)
				{
					return entity.ModelRoot;
				}
			}

			return null;
		}
	}

	/// <summary>
	/// Rule to update association connector colors when ModelRoot.AssociationLineColor changes.
	/// </summary>
	[RuleOn(typeof(ModelRoot), FireTime = TimeToFire.TopLevelCommit)]
	internal sealed class AssociationLineColorChangeRule : ChangeRule
	{
		public override void ElementPropertyChanged(ElementPropertyChangedEventArgs e)
		{
			if (e.DomainProperty.Id == ModelRoot.AssociationLineColorDomainPropertyId)
			{
				ModelRoot modelRoot = (ModelRoot)e.ModelElement;

				// Find all association connectors and update their colors
				foreach (var type in modelRoot.Types)
				{
					if (type is EntityModel entity)
					{
						// Get all associations where this entity is the source
						foreach (var association in Association.GetLinksToTargets(entity))
						{
							// Find all presentation elements (connectors) for this association
							foreach (var pel in PresentationViewsSubject.GetPresentation(association))
							{
								if (pel is AssociationConnector connector)
								{
									connector.ApplyLineColorFromModel();
									connector.Invalidate(true);
								}
							}
						}

						// Get all enum associations where this entity is the source
						foreach (var enumAssociation in EnumAssociation.GetLinksToUsedEnums(entity))
						{
							// Find all presentation elements (connectors) for this enum association
							foreach (var pel in PresentationViewsSubject.GetPresentation(enumAssociation))
							{
								if (pel is EnumAssociationConnector connector)
								{
									connector.ApplyLineColorFromModel();
									connector.Invalidate(true);
								}
							}
						}
					}
				}
			}
		}
	}
}
