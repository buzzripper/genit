using Microsoft.VisualStudio.Modeling;
using System;

namespace Dyvenix.GenIt.DslPackage.Editors
{
    /// <summary>
    /// Helper class for executing DSL model changes within transactions.
    /// DSL domain classes require all property modifications to be wrapped in transactions.
    /// </summary>
    public static class DslTransactionHelper
    {
        /// <summary>
        /// Executes an action within a DSL transaction.
        /// </summary>
        /// <param name="element">The model element to modify.</param>
        /// <param name="transactionName">Name for the transaction (for undo/redo).</param>
        /// <param name="action">The action to execute.</param>
        public static void ExecuteInTransaction(ModelElement element, string transactionName, Action action)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            // Guard against orphaned model elements (e.g., document closed but reference still held)
            if (element.Store == null)
                return;

            using (var tx = element.Store.TransactionManager.BeginTransaction(transactionName))
            {
                action();
                tx.Commit();
            }
        }

        /// <summary>
        /// Sets a property value within a DSL transaction.
        /// </summary>
        /// <typeparam name="T">The type of the property value.</typeparam>
        /// <param name="element">The model element to modify.</param>
        /// <param name="propertyName">Name of the property being set.</param>
        /// <param name="setter">Action that sets the property value.</param>
        public static void SetProperty<T>(ModelElement element, string propertyName, Action setter)
        {
            ExecuteInTransaction(element, $"Set {propertyName}", setter);
        }

        /// <summary>
        /// Sets a property value within a DSL transaction if the new value differs from the current value.
        /// </summary>
        /// <typeparam name="T">The type of the property value.</typeparam>
        /// <param name="element">The model element to modify.</param>
        /// <param name="propertyName">Name of the property being set.</param>
        /// <param name="currentValue">The current property value.</param>
        /// <param name="newValue">The new property value.</param>
        /// <param name="setter">Action that sets the property value.</param>
        public static void SetPropertyIfChanged<T>(ModelElement element, string propertyName, T currentValue, T newValue, Action setter)
        {
            if (!Equals(currentValue, newValue))
            {
                SetProperty<T>(element, propertyName, setter);
            }
        }
    }
}
