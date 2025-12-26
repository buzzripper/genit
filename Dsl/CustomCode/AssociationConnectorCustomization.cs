using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Diagrams;

namespace Dyvenix.GenIt
{
    /// <summary>
    /// Custom TextField that formats Multiplicity enum values for display
    /// </summary>
    internal class MultiplicityTextField : TextField
    {
        public MultiplicityTextField(string fieldName) : base(fieldName)
        {
        }
        
        /// <summary>
        /// Override GetDisplayText to format multiplicity values
        /// </summary>
        public override string GetDisplayText(ShapeElement parentShape)
        {
            string text = base.GetDisplayText(parentShape);
            
            // Try to parse as Multiplicity enum
            if (Enum.TryParse<Multiplicity>(text, out Multiplicity multiplicity))
            {
                return FormatMultiplicity(multiplicity);
            }
            
            return text;
        }
        
        private static string FormatMultiplicity(Multiplicity multiplicity)
        {
            switch (multiplicity)
            {
                case Multiplicity.ZeroOrOne:
                    return "0..1";
                case Multiplicity.One:
                    return "1";
                case Multiplicity.Many:
                    return "*";
                default:
                    return multiplicity.ToString();
            }
        }
    }

    /// <summary>
    /// Helper class for replacing multiplicity text fields in decorators
    /// </summary>
    internal static class MultiplicityDecoratorHelper
    {
        public static void ReplaceMultiplicityTextField(IList<ShapeField> shapeFields, IList<Decorator> decorators, string fieldName)
        {
            // Find the decorator with the matching field name
            for (int i = 0; i < decorators.Count; i++)
            {
                Decorator decorator = decorators[i];
                if (decorator.Field != null && decorator.Field.Name == fieldName)
                {
                    TextField originalField = decorator.Field as TextField;
                    if (originalField != null)
                    {
                        // Create a new MultiplicityTextField with the same settings
                        MultiplicityTextField newField = new MultiplicityTextField(fieldName);
                        newField.DefaultText = originalField.DefaultText;
                        newField.DefaultFocusable = originalField.DefaultFocusable;
                        newField.DefaultAutoSize = originalField.DefaultAutoSize;
                        newField.AnchoringBehavior.MinimumHeightInLines = originalField.AnchoringBehavior.MinimumHeightInLines;
                        newField.AnchoringBehavior.MinimumWidthInCharacters = originalField.AnchoringBehavior.MinimumWidthInCharacters;
                        newField.DefaultAccessibleState = originalField.DefaultAccessibleState;
                        
                        // Remove the old field from shapeFields if it exists
                        shapeFields.Remove(originalField);
                        
                        // Add the new field
                        shapeFields.Add(newField);
                        
                        // Create a new decorator with the new field
                        ConnectorDecorator connectorDecorator = decorator as ConnectorDecorator;
                        if (connectorDecorator != null)
                        {
                            Decorator newDecorator = new ConnectorDecorator(newField, connectorDecorator.Position, connectorDecorator.Offset);
                            decorators[i] = newDecorator;
                        }
                    }
                    break;
                }
            }
        }
    }

    public partial class AssociationConnector
    {
        private EventHandler<ElementPropertyChangedEventArgs> arrowheadPropertyChangedHandler;
        private EventHandler<ElementAddedEventArgs> arrowheadElementAddedHandler;
        private bool hasAppliedInitialArrowheads;

        public override void OnInitialize()
        {
            base.OnInitialize();

            if (arrowheadPropertyChangedHandler == null)
            {
                arrowheadPropertyChangedHandler = (sender, e) =>
                {
                    var association = this.ModelElement as Association;
                    if (association == null)
                    {
                        return;
                    }

                    if (!ReferenceEquals(e.ModelElement, association))
                    {
                        return;
                    }

                    if (e.DomainProperty.Id == Association.SourceMultiplicityDomainPropertyId ||
                        e.DomainProperty.Id == Association.TargetMultiplicityDomainPropertyId)
                    {
                        UpdateArrowheads(invalidate: true);
                    }
                };

                if (this.Store != null)
                {
                    this.Store.EventManagerDirectory.ElementPropertyChanged.Add(arrowheadPropertyChangedHandler);
                }
            }

            if (arrowheadElementAddedHandler == null)
            {
                arrowheadElementAddedHandler = (sender, e) =>
                {
                    if (hasAppliedInitialArrowheads)
                    {
                        return;
                    }

                    // Wait until the connector element is actually added (so it has a diagram presentation).
                    if (!ReferenceEquals(e.ModelElement, this))
                    {
                        return;
                    }

                    hasAppliedInitialArrowheads = true;
                    UpdateArrowheads(invalidate: true);
                };

                if (this.Store != null)
                {
                    this.Store.EventManagerDirectory.ElementAdded.Add(arrowheadElementAddedHandler);
                }
            }

            // Might run before the connector is connected/presented; ElementAdded handler covers first render.
            UpdateArrowheads(invalidate: false);
        }

        protected override void OnDeleted()
        {
            try
            {
                if (arrowheadPropertyChangedHandler != null && this.Store != null)
                {
                    this.Store.EventManagerDirectory.ElementPropertyChanged.Remove(arrowheadPropertyChangedHandler);
                }

                if (arrowheadElementAddedHandler != null && this.Store != null)
                {
                    this.Store.EventManagerDirectory.ElementAdded.Remove(arrowheadElementAddedHandler);
                }
            }
            finally
            {
                arrowheadPropertyChangedHandler = null;
                arrowheadElementAddedHandler = null;
                base.OnDeleted();
            }
        }

        private void UpdateArrowheads(bool invalidate)
        {
            var association = this.ModelElement as Association;
            if (association == null)
            {
                SetManyEndDecorators(false, false);
                return;
            }

            SetManyEndDecorators(
                association.SourceMultiplicity == Multiplicity.Many,
                association.TargetMultiplicity == Multiplicity.Many);

            if (invalidate)
            {
                this.Invalidate();
            }
        }

        private void SetManyEndDecorators(bool showSourceArrow, bool showTargetArrow)
        {
            this.SetDecorators(
                showSourceArrow ? LinkDecorator.DecoratorHollowArrow : null,
                showSourceArrow ? new SizeD(0.1, 0.1) : SizeD.Empty,
                showTargetArrow ? LinkDecorator.DecoratorHollowArrow : null,
                showTargetArrow ? new SizeD(0.1, 0.1) : SizeD.Empty,
                false);
        }
    }
}
