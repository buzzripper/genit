using System;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.Modeling;

namespace Dyvenix.GenIt
{
	/// <summary>
	/// Coordinates which <see cref="GenItDiagram"/> receives a shape during view fix-up when a model
	/// element is added interactively.
	/// </summary>
	/// <remarks>
	/// The generated diagram assumes a single diagram per model, so its
	/// <c>GenItDiagram.ShouldAddShapeForElement</c> always returns <c>true</c>. With multiple views
	/// sharing one <see cref="ModelRoot"/>, that causes every loaded view to receive a shape for each
	/// newly added element (fan-out). This helper narrows fix-up to a single target diagram:
	/// <list type="bullet">
	/// <item>An explicit target set via <see cref="BeginTargetScope"/> (used when adding an existing
	/// element to the current view) wins.</item>
	/// <item>Otherwise the active diagram most recently bound to the visible surface (per store) is
	/// used.</item>
	/// </list>
	/// State is keyed by <see cref="Store"/> so multiple open documents do not interfere. The tables
	/// hold the store weakly (<see cref="ConditionalWeakTable{TKey,TValue}"/>), so entries are released
	/// automatically once a document's store is collected - no explicit teardown is required. When no
	/// targeting information is available the helper falls back to permissive behaviour (returns
	/// <c>true</c>) so single-view scenarios and unforeseen paths keep working.
	/// </remarks>
	internal static class GenItViewTargeting
	{
		private static readonly object _sync = new object();

		/// <summary>The active (surface-bound) diagram per store.</summary>
		private static readonly ConditionalWeakTable<Store, GenItDiagram> _activeDiagrams = new ConditionalWeakTable<Store, GenItDiagram>();

		/// <summary>The explicit fix-up target per store, set while a <see cref="TargetScope"/> is open.</summary>
		private static readonly ConditionalWeakTable<Store, GenItDiagram> _explicitTargets = new ConditionalWeakTable<Store, GenItDiagram>();

		/// <summary>
		/// Records the diagram currently bound to the document surface for its store.
		/// </summary>
		internal static void SetActiveDiagram(GenItDiagram diagram)
		{
			if (diagram == null || diagram.Store == null)
				return;

			lock (_sync)
			{
				_activeDiagrams.Remove(diagram.Store);
				_activeDiagrams.Add(diagram.Store, diagram);
			}
		}

		/// <summary>
		/// Drops any active/target state associated with the given store. Optional: entries are also
		/// released automatically when the store is garbage-collected.
		/// </summary>
		internal static void ClearStore(Store store)
		{
			if (store == null)
				return;

			lock (_sync)
			{
				_activeDiagrams.Remove(store);
				_explicitTargets.Remove(store);
			}
		}

		/// <summary>
		/// Opens a scope during which fix-up shapes are routed exclusively to <paramref name="target"/>.
		/// Dispose the returned scope (e.g. with <c>using</c>) once the model change completes.
		/// </summary>
		internal static IDisposable BeginTargetScope(GenItDiagram target)
		{
			if (target == null)
				throw new ArgumentNullException(nameof(target));
			if (target.Store == null)
				throw new ArgumentException("Target diagram has no store.", nameof(target));

			return new TargetScope(target);
		}

		/// <summary>
		/// Decides whether the given diagram should create a shape for <paramref name="element"/> during
		/// interactive view fix-up. Returns <c>true</c> when the diagram is the intended target, or when
		/// no targeting information is available (permissive fallback).
		/// </summary>
		internal static bool ShouldAddShapeForElement(GenItDiagram diagram, ModelElement element)
		{
			if (diagram == null)
				return true;

			Store store = diagram.Store;
			if (store == null)
				return true;

			// During load/save the fix-up rule is already suppressed elsewhere; be permissive here.
			if (store.InSerializationTransaction)
				return true;

			GenItDiagram target;
			lock (_sync)
			{
				if (!_explicitTargets.TryGetValue(store, out target) || target == null)
				{
					if (!_activeDiagrams.TryGetValue(store, out target))
						target = null;
				}
			}

			// No targeting info yet, or only a single view exists: keep the generated behaviour.
			if (target == null || !HasMultipleViews(store))
				return true;

			return ReferenceEquals(diagram, target);
		}

		/// <summary>
		/// True when the store contains more than one <see cref="GenItDiagram"/>.
		/// </summary>
		private static bool HasMultipleViews(Store store)
		{
			return store.ElementDirectory.FindElements(GenItDiagram.DomainClassId).Count > 1;
		}

		private sealed class TargetScope : IDisposable
		{
			private readonly Store _store;
			private readonly GenItDiagram _previous;
			private readonly bool _hadPrevious;
			private bool _disposed;

			internal TargetScope(GenItDiagram target)
			{
				_store = target.Store;
				lock (_sync)
				{
					_hadPrevious = _explicitTargets.TryGetValue(_store, out _previous);
					_explicitTargets.Remove(_store);
					_explicitTargets.Add(_store, target);
				}
			}

			public void Dispose()
			{
				if (_disposed)
					return;
				_disposed = true;

				lock (_sync)
				{
					_explicitTargets.Remove(_store);
					if (_hadPrevious && _previous != null)
						_explicitTargets.Add(_store, _previous);
				}
			}
		}
	}
}
