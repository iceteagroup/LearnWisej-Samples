using System.Drawing;

namespace VisualOperationsStudio.Geometry
{
    /// <summary>
    /// The shapes a half-circle gauge is made of, measured from a rectangle and a normalised value.
    /// Ordinary arithmetic: this file must not reference <c>Graphics</c>, a control or a session, which
    /// is what lets the hard part be tested without ever creating one.
    /// </summary>
    public readonly struct GaugeLayout
    {
        public GaugeLayout(
            bool isEmpty,
            Rectangle ringBounds,
            Rectangle innerBounds,
            float ringThickness,
            PointF centre,
            float startAngle,
            float fullSweep,
            float valueSweep,
            float needleAngle,
            PointF[] needlePoints,
            Rectangle valueBounds,
            Rectangle captionBounds)
        {
            IsEmpty = isEmpty;
            RingBounds = ringBounds;
            InnerBounds = innerBounds;
            RingThickness = ringThickness;
            Centre = centre;
            StartAngle = startAngle;
            FullSweep = fullSweep;
            ValueSweep = valueSweep;
            NeedleAngle = needleAngle;
            NeedlePoints = needlePoints;
            ValueBounds = valueBounds;
            CaptionBounds = captionBounds;
        }

        /// <summary>True when the rectangle is too small to draw anything into.</summary>
        public bool IsEmpty { get; }

        /// <summary>The square the arcs and pies are drawn against.</summary>
        public Rectangle RingBounds { get; }

        /// <summary>The hole a filled pie has to be carved back to, to leave a ring.</summary>
        public Rectangle InnerBounds { get; }

        public float RingThickness { get; }

        public PointF Centre { get; }

        /// <summary>180 degrees: a half gauge opens to the left and sweeps clockwise.</summary>
        public float StartAngle { get; }

        public float FullSweep { get; }

        /// <summary>The sweep of the active arc, 0 for an empty or inverted range.</summary>
        public float ValueSweep { get; }

        /// <summary>The angle the needle points at, in degrees, for the renderer to rotate by.</summary>
        public float NeedleAngle { get; }

        /// <summary>
        /// The needle as a triangle around the origin, pointing along +X: left shoulder, tip, right
        /// shoulder. The renderer translates to <see cref="Centre"/> and rotates by <see cref="NeedleAngle"/>.
        /// </summary>
        public PointF[] NeedlePoints { get; }

        public Rectangle ValueBounds { get; }

        public Rectangle CaptionBounds { get; }
    }

    public static class GaugeGeometry
    {
        /// <summary>A gauge below this size is not worth drawing, and is not drawn.</summary>
        public const int MinimumSide = 60;

        /// <summary>The value and caption font sizes the renderer should use for this layout.</summary>
        public static float ValueFontSize(GaugeLayout layout) => Clamp(layout.RingBounds.Width * 0.13f, 10f, 26f);

        public static float CaptionFontSize(GaugeLayout layout) => Clamp(layout.RingBounds.Width * 0.052f, 8f, 13f);

        private static float Clamp(float candidate, float low, float high) =>
            candidate < low ? low : candidate > high ? high : candidate;

        private const float Start = 180f;
        private const float Full = 180f;

        /// <summary>
        /// Measures a gauge inside <paramref name="bounds"/> for a value already normalised to 0..1.
        /// A normalised value outside that range is clamped; a rectangle too small to draw into comes
        /// back as an empty layout, so the caller can return before it creates a single drawing object.
        /// </summary>
        public static GaugeLayout Measure(Rectangle bounds, float normalized)
        {
            if (bounds.Width < MinimumSide || bounds.Height < MinimumSide / 2)
                return new GaugeLayout(true, Rectangle.Empty, Rectangle.Empty, 0f, PointF.Empty, Start, Full, 0f, Start, new PointF[0], Rectangle.Empty, Rectangle.Empty);

            if (normalized < 0f) normalized = 0f;
            if (normalized > 1f) normalized = 1f;

            // The face is a half disc with a value and a caption under it. Both the disc and the text
            // are sized from one diameter, so a tall cell does not produce enormous lettering.
            var diameter = (int)(System.Math.Min(bounds.Width, bounds.Height * 1.6f) * 0.84f);
            var textHeight = (int)(diameter * 0.34f);
            var thickness = System.Math.Max(6f, diameter * 0.12f);
            var blockHeight = diameter / 2 + (int)thickness + textHeight;
            var top = bounds.Y + System.Math.Max(0, (bounds.Height - blockHeight) / 2);

            var ring = new Rectangle(
                bounds.X + (bounds.Width - diameter) / 2,
                top,
                diameter,
                diameter);

            var inner = Rectangle.Inflate(ring, -(int)thickness, -(int)thickness);
            var centre = new PointF(ring.X + ring.Width / 2f, ring.Y + ring.Height / 2f);

            var sweep = Full * normalized;
            var needle = NeedleShape(ring.Width / 2f - thickness * 0.35f, thickness * 0.34f);

            // Clear of the arc: its end caps reach half the ring thickness below the centre line.
            var valueBounds = new Rectangle(bounds.X, top + diameter / 2 + (int)thickness, bounds.Width, (int)(textHeight * 0.62f));
            var captionBounds = new Rectangle(bounds.X, valueBounds.Bottom, bounds.Width, textHeight - valueBounds.Height);

            return new GaugeLayout(false, ring, inner, thickness, centre, Start, Full, sweep, Start + sweep, needle, valueBounds, captionBounds);
        }

        /// <summary>
        /// Maps a value onto 0..1 for the given range. An inverted range (maximum below minimum) and a
        /// zero-width range both return 0, which the renderer draws as an empty track rather than throwing.
        /// </summary>
        public static float Normalize(double value, double minimum, double maximum)
        {
            var span = maximum - minimum;
            if (span <= 0)
                return 0f;

            var normalized = (value - minimum) / span;
            if (normalized < 0) normalized = 0;
            if (normalized > 1) normalized = 1;
            return (float)normalized;
        }

        /// <summary>The angle, in degrees, at which a normalised value sits on the arc.</summary>
        public static float AngleFor(float normalized)
        {
            if (normalized < 0f) normalized = 0f;
            if (normalized > 1f) normalized = 1f;
            return Start + Full * normalized;
        }

        /// <summary>The sweep between two normalised values, never negative.</summary>
        public static float SweepBetween(float fromNormalized, float toNormalized)
        {
            var sweep = AngleFor(toNormalized) - AngleFor(fromNormalized);
            return sweep < 0f ? 0f : sweep;
        }

        private static PointF[] NeedleShape(float length, float halfWidth) => new[]
        {
            new PointF(0f, halfWidth),
            new PointF(length, 0f),
            new PointF(0f, -halfWidth),
        };
    }
}
