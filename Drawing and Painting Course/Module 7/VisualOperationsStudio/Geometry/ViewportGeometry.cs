using System;
using System.Collections.Generic;
using System.Drawing;

namespace VisualOperationsStudio.Geometry
{
    /// <summary>
    /// The viewport arithmetic every topology surface shares: world to screen and back, the visible
    /// world rectangle, hit testing and zooming about a point. Pure functions over a pan offset and a
    /// zoom factor - no control, no session, no <c>Graphics</c>, which is what makes them testable.
    /// </summary>
    public static class ViewportGeometry
    {
        public const float MinimumZoom = 0.4f;
        public const float MaximumZoom = 3f;

        public static PointF ToScreen(PointF world, float panX, float panY, float zoom) =>
            new PointF(world.X * zoom + panX, world.Y * zoom + panY);

        public static PointF ToWorld(PointF screen, float panX, float panY, float zoom)
        {
            if (zoom <= 0f)
                zoom = 1f;

            return new PointF((screen.X - panX) / zoom, (screen.Y - panY) / zoom);
        }

        /// <summary>The world rectangle a surface of this size currently shows. Computed once per render.</summary>
        public static RectangleF VisibleWorld(int width, int height, float panX, float panY, float zoom)
        {
            var topLeft = ToWorld(new PointF(0, 0), panX, panY, zoom);
            var bottomRight = ToWorld(new PointF(width, height), panX, panY, zoom);
            return RectangleF.FromLTRB(topLeft.X, topLeft.Y, bottomRight.X, bottomRight.Y);
        }

        /// <summary>
        /// The index of the last rectangle containing the world point, or -1. Last wins, because the
        /// shape drawn last is the one on top - which is what the user believes they clicked.
        /// </summary>
        public static int HitTest(IReadOnlyList<RectangleF> bounds, PointF world)
        {
            if (bounds == null)
                return -1;

            for (var i = bounds.Count - 1; i >= 0; i--)
            {
                if (bounds[i].Contains(world))
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// The new pan and zoom that keep the world point under <paramref name="screen"/> fixed while
        /// the zoom factor changes. The zoom is clamped, so a runaway wheel cannot invert the viewport.
        /// </summary>
        public static void ZoomAbout(PointF screen, float factor, ref float panX, ref float panY, ref float zoom)
        {
            var before = ToWorld(screen, panX, panY, zoom);

            zoom = Math.Min(MaximumZoom, Math.Max(MinimumZoom, zoom * factor));

            var after = ToWorld(screen, panX, panY, zoom);
            panX += (after.X - before.X) * zoom;
            panY += (after.Y - before.Y) * zoom;
        }

        /// <summary>The rectangle that fits <paramref name="content"/> into a page, with a margin.</summary>
        public static float FitScale(RectangleF content, int width, int height, float margin)
        {
            var usableWidth = Math.Max(1f, width - margin * 2);
            var usableHeight = Math.Max(1f, height - margin * 2);

            return Math.Min(
                usableWidth / Math.Max(1f, content.Width),
                usableHeight / Math.Max(1f, content.Height));
        }
    }
}
