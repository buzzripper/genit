using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Diagrams;

namespace Dyvenix.GenIt
{
    public partial class AssociationConnector
    {
        private EventHandler<ElementPropertyChangedEventArgs> multiplicityPropertyChangedHandler;

        public override void OnInitialize()
        {
            base.OnInitialize();

            // Remove any end decorators (no diamonds/arrowheads)
            this.SetDecorators(null, SizeD.Empty, null, SizeD.Empty, false);

            if (multiplicityPropertyChangedHandler == null)
            {
                multiplicityPropertyChangedHandler = (sender, e) =>
                {
                    var association = this.ModelElement as Association;
                    if (association == null || !ReferenceEquals(e.ModelElement, association))
                    {
                        return;
                    }

                    if (e.DomainProperty.Id == Association.SourceMultiplicityDomainPropertyId ||
                        e.DomainProperty.Id == Association.TargetMultiplicityDomainPropertyId)
                    {
                        this.Invalidate();
                    }
                };

                this.Store?.EventManagerDirectory.ElementPropertyChanged.Add(multiplicityPropertyChangedHandler);
            }

            this.Invalidate();
        }

        protected override void OnDeleted()
        {
            try
            {
                if (multiplicityPropertyChangedHandler != null)
                {
                    this.Store?.EventManagerDirectory.ElementPropertyChanged.Remove(multiplicityPropertyChangedHandler);
                }
            }
            finally
            {
                multiplicityPropertyChangedHandler = null;
                base.OnDeleted();
            }
        }

        public override void OnPaintShape(DiagramPaintEventArgs e)
        {
            base.OnPaintShape(e);
            DrawMultiplicityLabels(e);
        }

        private void DrawMultiplicityLabels(DiagramPaintEventArgs e)
        {
            // Draw multiplicity text at the actual connector endpoints
            var association = this.ModelElement as Association;
            if (association == null)
            {
                return;
            }

            var edgePoints = this.EdgePoints;
            if (edgePoints == null || edgePoints.Count < 2)
            {
                return;
            }

            // Get the first and last points (source and target endpoints)
            var sourcePoint = edgePoints[0].Point;
            var targetPoint = edgePoints[edgePoints.Count - 1].Point;

            // Get next points for direction calculation
            var sourceNextPoint = edgePoints.Count > 1 ? edgePoints[1].Point : targetPoint;
            var targetNextPoint = edgePoints.Count > 1 ? edgePoints[edgePoints.Count - 2].Point : sourcePoint;

            // Source multiplicity - draw label based on multiplicity type
            string sourceLabel = GetMultiplicityLabel(association.SourceMultiplicity);
            if (!string.IsNullOrEmpty(sourceLabel))
            {
                DrawMultiplicityAtEndpoint(e, sourceLabel, sourcePoint, sourceNextPoint);
            }

            // Target multiplicity - draw label based on multiplicity type
            string targetLabel = GetMultiplicityLabel(association.TargetMultiplicity);
            if (!string.IsNullOrEmpty(targetLabel))
            {
                DrawMultiplicityAtEndpoint(e, targetLabel, targetPoint, targetNextPoint);
            }
        }

        /// <summary>
        /// Gets the display label for a multiplicity value.
        /// </summary>
        private string GetMultiplicityLabel(Multiplicity multiplicity)
        {
            switch (multiplicity)
            {
                case Multiplicity.One:
                    return "1";
                case Multiplicity.ZeroOne:
                    return "0..1";
                case Multiplicity.Many:
                    return "*";
                default:
                    return null;
            }
        }

        private void DrawMultiplicityAtEndpoint(DiagramPaintEventArgs e, string text, PointD endpoint, PointD nextPoint)
        {
            // Calculate direction from endpoint toward the center of the line
            double dx = nextPoint.X - endpoint.X;
            double dy = nextPoint.Y - endpoint.Y;
            double length = Math.Sqrt(dx * dx + dy * dy);

            if (length < 0.001)
            {
                return;
            }

            // Normalize direction vector
            dx /= length;
            dy /= length;

            // Offset values (in world/inches coordinates)
            double offsetAlongLine = 0.12; // Move along the line from the endpoint (farther from the box)
            double offsetPerpendicular = 0.15; // Move perpendicular to the line (farther from the line)

            // Calculate text position by offsetting along the line first
            double textX = endpoint.X + dx * offsetAlongLine;
            double textY = endpoint.Y + dy * offsetAlongLine;
            
            // Determine perpendicular offset direction based on line orientation
            if (Math.Abs(dx) > Math.Abs(dy))
            {
                // More horizontal line - offset downward (positive Y)
                textY += offsetPerpendicular;
            }
            else
            {
                // More vertical line
                // If line goes downward (dy > 0), this endpoint is at top - offset to the right
                // If line goes upward (dy < 0), this endpoint is at bottom - offset to the right
                // But if this endpoint is at the TOP (line goes down from here), we need more space
                // because the * would be too close to the box above
                if (dy > 0)
                {
                    // Line goes DOWN from this endpoint (this box is above)
                    // Offset to the right, but also add a bit more distance from the box
                    textX += offsetPerpendicular;
                    textY += 0.05; // Add extra vertical offset to move away from the box above
                }
                else
                {
                    // Line goes UP from this endpoint (this box is below)
                    // Offset to the right
                    textX += offsetPerpendicular;
                }
            }

            // Create font scaled appropriately for world coordinates
            // In DSL Tools, world coords are in inches, so font size needs to account for that
            // Use different sizes: larger for '*', smaller for '1' and '0..1'
            float fontSize = (text == "*") ? 0.18f : 0.11f;

            // Get the line color from ModelRoot to match the connector line color
            Color textColor = GetLineColorFromModel();

            using (var font = new Font("Segoe UI", fontSize, FontStyle.Regular, GraphicsUnit.Inch))
            using (var brush = new SolidBrush(textColor))
            {
                // Measure text
                var textSize = e.Graphics.MeasureString(text, font);

                // Draw centered on the calculated point
                float drawX = (float)textX - textSize.Width / 2;
                float drawY = (float)textY - textSize.Height / 2;

                e.Graphics.DrawString(text, font, brush, drawX, drawY);
            }
        }

        /// <summary>
        /// Gets the line color from the ModelRoot, or returns a default color if not available.
        /// </summary>
        private Color GetLineColorFromModel()
        {
            ModelRoot modelRoot = GetModelRoot();
            if (modelRoot != null)
            {
                Color lineColor = modelRoot.AssociationLineColor;
                if (lineColor != Color.Empty && lineColor != Color.Transparent)
                {
                    return lineColor;
                }
            }
            // Default color if ModelRoot or color is not available
            return Color.FromArgb(113, 111, 110);
        }
    }
}
