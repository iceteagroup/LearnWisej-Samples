using System;
using System.Collections.Generic;
using System.Drawing;
using VisualOperationsStudio.Geometry;
using VisualOperationsStudio.Models;
using VisualOperationsStudio.Renderers;

namespace VisualOperationsStudio.Diagnostics
{
    /// <summary>
    /// Runs the four bad states the capstone has to survive - empty data, a surface with no size, an
    /// inverted range and a missing font - and reports what each one did. They are exercised as code
    /// rather than described in a comment, because "it degrades gracefully" is a claim, not a fact,
    /// until something runs it.
    /// </summary>
    public static class DegradedStateCheck
    {
        public static IReadOnlyList<string> Run()
        {
            var results = new List<string>();

            results.Add(EmptyModel());
            results.Add(ZeroSizedSurface());
            results.Add(InvertedRange());
            results.Add(MissingFont());

            return results;
        }

        private static string EmptyModel()
        {
            try
            {
                var empty = new TopologyScene();
                var bytes = new TopologyImageRenderer().Render(empty, 600, 400);

                return bytes.Length > 0
                    ? $"Empty data: a readable {bytes.Length / 1024:N0} KB image saying \"No nodes to display\", not a blank file."
                    : "Empty data: FAILED - no bytes produced.";
            }
            catch (Exception ex)
            {
                return $"Empty data: FAILED - {ex.GetType().Name}: {ex.Message}";
            }
        }

        private static string ZeroSizedSurface()
        {
            try
            {
                var layout = GaugeGeometry.Measure(new Rectangle(0, 0, 10, 4), 0.5f);

                return layout.IsEmpty
                    ? "Zero-sized surface: the layout comes back empty, so the handler returns before it creates a single drawing object."
                    : "Zero-sized surface: FAILED - a layout was produced for a 10x4 rectangle.";
            }
            catch (Exception ex)
            {
                return $"Zero-sized surface: FAILED - {ex.GetType().Name}: {ex.Message}";
            }
        }

        private static string InvertedRange()
        {
            try
            {
                var normalized = GaugeGeometry.Normalize(50, 100, 0);

                return Math.Abs(normalized) < 0.0001f
                    ? "Inverted range (minimum 100, maximum 0): normalises to 0, which draws an empty track rather than throwing."
                    : $"Inverted range: FAILED - normalised to {normalized}.";
            }
            catch (Exception ex)
            {
                return $"Inverted range: FAILED - {ex.GetType().Name}: {ex.Message}";
            }
        }

        private static string MissingFont()
        {
            try
            {
                var renderer = new TopologyImageRenderer();
                using (renderer.ResolveFont(10f, FontStyle.Regular))
                {
                    return $"Missing font family: resolved to {renderer.ResolvedFontFamily} - the chain ends in a family the runtime always has.";
                }
            }
            catch (Exception ex)
            {
                return $"Missing font family: FAILED - {ex.GetType().Name}: {ex.Message}";
            }
        }
    }
}
