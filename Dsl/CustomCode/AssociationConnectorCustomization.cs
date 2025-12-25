using System;
using System.Collections.Generic;
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
    /// Partial class for UnidirectionalConnector to replace multiplicity text fields
    /// </summary>
    public partial class UnidirectionalConnector
    {
        /// <summary>
        /// Initialize the collection of decorators associated with this shape type.
        /// </summary>
        protected override void InitializeDecorators(IList<ShapeField> shapeFields, IList<Decorator> decorators)
        {
            base.InitializeDecorators(shapeFields, decorators);
            ReplaceMultiplicityTextFields(shapeFields, decorators);
        }

        private static void ReplaceMultiplicityTextFields(IList<ShapeField> shapeFields, IList<Decorator> decorators)
        {
            MultiplicityDecoratorHelper.ReplaceMultiplicityTextField(shapeFields, decorators, "TargetMultiplicity");
            MultiplicityDecoratorHelper.ReplaceMultiplicityTextField(shapeFields, decorators, "SourceMultiplicity");
        }
    }

    /// <summary>
    /// Partial class for BidirectionalConnector to replace multiplicity text fields
    /// </summary>
    public partial class BidirectionalConnector
    {
        /// <summary>
        /// Initialize the collection of decorators associated with this shape type.
        /// </summary>
        protected override void InitializeDecorators(IList<ShapeField> shapeFields, IList<Decorator> decorators)
        {
            base.InitializeDecorators(shapeFields, decorators);
            ReplaceMultiplicityTextFields(shapeFields, decorators);
        }

        private static void ReplaceMultiplicityTextFields(IList<ShapeField> shapeFields, IList<Decorator> decorators)
        {
            MultiplicityDecoratorHelper.ReplaceMultiplicityTextField(shapeFields, decorators, "TargetMultiplicity");
            MultiplicityDecoratorHelper.ReplaceMultiplicityTextField(shapeFields, decorators, "SourceMultiplicity");
        }
    }

    /// <summary>
    /// Partial class for AggregationConnector to replace multiplicity text fields
    /// </summary>
    public partial class AggregationConnector
    {
        /// <summary>
        /// Initialize the collection of decorators associated with this shape type.
        /// </summary>
        protected override void InitializeDecorators(IList<ShapeField> shapeFields, IList<Decorator> decorators)
        {
            base.InitializeDecorators(shapeFields, decorators);
            ReplaceMultiplicityTextFields(shapeFields, decorators);
        }

        private static void ReplaceMultiplicityTextFields(IList<ShapeField> shapeFields, IList<Decorator> decorators)
        {
            MultiplicityDecoratorHelper.ReplaceMultiplicityTextField(shapeFields, decorators, "TargetMultiplicity");
            MultiplicityDecoratorHelper.ReplaceMultiplicityTextField(shapeFields, decorators, "SourceMultiplicity");
        }
    }

    /// <summary>
    /// Partial class for CompositionConnector to replace multiplicity text fields
    /// </summary>
    public partial class CompositionConnector
    {
        /// <summary>
        /// Initialize the collection of decorators associated with this shape type.
        /// </summary>
        protected override void InitializeDecorators(IList<ShapeField> shapeFields, IList<Decorator> decorators)
        {
            base.InitializeDecorators(shapeFields, decorators);
            ReplaceMultiplicityTextFields(shapeFields, decorators);
        }

        private static void ReplaceMultiplicityTextFields(IList<ShapeField> shapeFields, IList<Decorator> decorators)
        {
            MultiplicityDecoratorHelper.ReplaceMultiplicityTextField(shapeFields, decorators, "TargetMultiplicity");
            MultiplicityDecoratorHelper.ReplaceMultiplicityTextField(shapeFields, decorators, "SourceMultiplicity");
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
}
