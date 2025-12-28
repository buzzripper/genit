using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Diagrams;

namespace Dyvenix.GenIt
{
    internal static class DslRuntimeDebugCommands
    {
        public static string DumpAssociationConnectorDecoratorMetadata(AssociationConnector connector)
        {
            if (connector == null)
            {
                return "(null connector)";
            }

            var connectorType = connector.GetType();

            var lines = new List<string>
            {
                "AssociationConnector runtime metadata",
                "- Connector type: " + connectorType.FullName,
                "- Base type: " + (connectorType.BaseType?.FullName ?? "(null)"),
                "- RoutingStyle: " + connector.RoutingStyle.ToString(),
            };

            // Geometry/points info differs across DSL Tools versions; use reflection to avoid hard dependencies.
            lines.Add("- Geometry (best effort): " + GetBestEffortGeometrySummary(connector));

            for (int i = 0; i < connector.Decorators.Count; i++)
            {
                var decorator = connector.Decorators[i];
                lines.Add($"Decorator[{i}]: {decorator.GetType().FullName}");

                if (decorator is ConnectorDecorator cd)
                {
                    lines.Add("  - Field: " + (cd.Field?.Name ?? "(null)"));

                    var posProp = cd.GetType().GetProperty("Position", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (posProp != null)
                    {
                        object posValue = null;
                        try { posValue = posProp.GetValue(cd, null); } catch { }
                        lines.Add("  - Position: " + (posValue?.ToString() ?? "(unavailable)"));
                    }

                    var offProp = cd.GetType().GetProperty("Offset", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (offProp != null)
                    {
                        object offValue = null;
                        try { offValue = offProp.GetValue(cd, null); } catch { }
                        lines.Add("  - Offset: " + (offValue?.ToString() ?? "(unavailable)"));
                    }
                }
            }

            // Try to surface potential override hooks for placement/geometry.
            var hookCandidates = connectorType
                .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(m => m.IsVirtual && !m.IsAbstract)
                .Select(m => m.Name)
                .Distinct(StringComparer.Ordinal)
                .Where(n => n.IndexOf("decor", StringComparison.OrdinalIgnoreCase) >= 0 ||
                            n.IndexOf("layout", StringComparison.OrdinalIgnoreCase) >= 0 ||
                            n.IndexOf("anchor", StringComparison.OrdinalIgnoreCase) >= 0 ||
                            n.IndexOf("route", StringComparison.OrdinalIgnoreCase) >= 0 ||
                            n.IndexOf("geometry", StringComparison.OrdinalIgnoreCase) >= 0)
                .OrderBy(n => n, StringComparer.Ordinal)
                .ToArray();

            lines.Add("Potential virtual hooks on connector (name match):");
            if (hookCandidates.Length == 0)
            {
                lines.Add("- (none found by name filter)");
            }
            else
            {
                foreach (var name in hookCandidates)
                {
                    lines.Add("- " + name);
                }
            }

            return string.Join(Environment.NewLine, lines);
        }

        private static string GetBestEffortGeometrySummary(BinaryLinkShape connector)
        {
            if (connector == null)
            {
                return "(null)";
            }

            try
            {
                var t = connector.GetType();

                // Common properties/methods vary by DSL Tools version. Probe a few likely candidates.
                var candidateNames = new[]
                {
                    "Points",
                    "EdgePoints",
                    "RoutingPoints",
                    "SourcePoint",
                    "TargetPoint",
                    "SourceAnchorPoint",
                    "TargetAnchorPoint",
                    "GetSourcePoint",
                    "GetTargetPoint",
                };

                foreach (var name in candidateNames)
                {
                    var p = t.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (p != null)
                    {
                        object val = null;
                        try { val = p.GetValue(connector, null); } catch { }
                        if (val != null)
                        {
                            return name + "=" + val;
                        }
                    }

                    var m = t.GetMethod(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
                    if (m != null)
                    {
                        object val = null;
                        try { val = m.Invoke(connector, null); } catch { }
                        if (val != null)
                        {
                            return name + "()=" + val;
                        }
                    }
                }

                return "(no known geometry members found)";
            }
            catch (Exception ex)
            {
                return "(error: " + ex.GetType().Name + ")";
            }
        }
    }
}
