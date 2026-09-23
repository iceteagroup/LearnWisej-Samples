using System.Drawing;
using VisualOperationsStudio.Geometry;

namespace VisualOperationsStudio.Tests;

/// <summary>
/// The hard part of the drawing code is arithmetic, and arithmetic can be tested without ever creating
/// a control. Nothing in this file references Wisej.NET, a session or a browser.
/// </summary>
public class GaugeGeometryTests
{
    [Theory]
    [InlineData(0, 0f)]
    [InlineData(50, 0.5f)]
    [InlineData(100, 1f)]
    public void Normalize_maps_a_value_onto_its_range(double value, float expected)
    {
        Assert.Equal(expected, GaugeGeometry.Normalize(value, 0, 100), 4);
    }

    [Theory]
    [InlineData(-25)]
    [InlineData(132)]
    public void Normalize_clamps_a_value_outside_the_range(double value)
    {
        var normalized = GaugeGeometry.Normalize(value, 0, 100);

        Assert.InRange(normalized, 0f, 1f);
    }

    [Fact]
    public void Normalize_degrades_an_inverted_range_to_zero()
    {
        // Maximum below minimum: an empty track, not an exception and not a negative sweep.
        Assert.Equal(0f, GaugeGeometry.Normalize(50, 100, 0));
    }

    [Fact]
    public void Normalize_degrades_a_zero_width_range_to_zero()
    {
        Assert.Equal(0f, GaugeGeometry.Normalize(50, 50, 50));
    }

    [Theory]
    [InlineData(0f, 180f)]
    [InlineData(0.5f, 270f)]
    [InlineData(1f, 360f)]
    public void AngleFor_sweeps_the_half_gauge_clockwise_from_nine_oclock(float normalized, float expected)
    {
        Assert.Equal(expected, GaugeGeometry.AngleFor(normalized), 3);
    }

    [Fact]
    public void SweepBetween_is_never_negative()
    {
        Assert.Equal(0f, GaugeGeometry.SweepBetween(0.9f, 0.2f));
        Assert.Equal(90f, GaugeGeometry.SweepBetween(0.25f, 0.75f), 3);
    }

    [Fact]
    public void Measure_returns_an_empty_layout_below_the_minimum_side()
    {
        var layout = GaugeGeometry.Measure(new Rectangle(0, 0, 10, 4), 0.5f);

        Assert.True(layout.IsEmpty);
        Assert.Empty(layout.NeedlePoints);
    }

    [Fact]
    public void Measure_keeps_the_face_and_the_text_inside_the_bounds()
    {
        var bounds = new Rectangle(0, 0, 320, 260);
        var layout = GaugeGeometry.Measure(bounds, 0.75f);

        Assert.False(layout.IsEmpty);
        Assert.True(layout.RingBounds.Left >= bounds.Left);
        Assert.True(layout.RingBounds.Right <= bounds.Right);
        Assert.True(layout.CaptionBounds.Bottom <= bounds.Bottom);
        Assert.Equal(180f + 180f * 0.75f, layout.NeedleAngle, 3);
    }
}

public class ViewportGeometryTests
{
    [Theory]
    [InlineData(0.5f)]
    [InlineData(1f)]
    [InlineData(2f)]
    public void ToWorld_undoes_ToScreen_at_every_zoom(float zoom)
    {
        var world = new PointF(137f, 84f);

        var screen = ViewportGeometry.ToScreen(world, 40f, 25f, zoom);
        var back = ViewportGeometry.ToWorld(screen, 40f, 25f, zoom);

        Assert.Equal(world.X, back.X, 3);
        Assert.Equal(world.Y, back.Y, 3);
    }

    [Fact]
    public void HitTest_returns_the_shape_drawn_last()
    {
        var bounds = new List<RectangleF>
        {
            new RectangleF(0, 0, 100, 100),
            new RectangleF(50, 50, 100, 100),   // drawn later, so on top
        };

        Assert.Equal(1, ViewportGeometry.HitTest(bounds, new PointF(75, 75)));
        Assert.Equal(0, ViewportGeometry.HitTest(bounds, new PointF(10, 10)));
        Assert.Equal(-1, ViewportGeometry.HitTest(bounds, new PointF(400, 400)));
    }

    [Fact]
    public void HitTest_over_empty_space_returns_nothing()
    {
        Assert.Equal(-1, ViewportGeometry.HitTest(new List<RectangleF>(), new PointF(1, 1)));
        Assert.Equal(-1, ViewportGeometry.HitTest(null, new PointF(1, 1)));
    }

    [Fact]
    public void VisibleWorld_grows_as_the_viewport_zooms_out()
    {
        var near = ViewportGeometry.VisibleWorld(800, 600, 0f, 0f, 2f);
        var far = ViewportGeometry.VisibleWorld(800, 600, 0f, 0f, 0.5f);

        Assert.True(far.Width > near.Width);
        Assert.Equal(400f, near.Width, 3);
        Assert.Equal(1600f, far.Width, 3);
    }

    [Fact]
    public void Culling_skips_what_the_visible_rectangle_does_not_touch()
    {
        var visible = ViewportGeometry.VisibleWorld(800, 600, 40f, 40f, 1f);

        Assert.True(visible.IntersectsWith(new RectangleF(260, 40, 130, 60)));    // Pump 1
        Assert.False(visible.IntersectsWith(new RectangleF(1500, 520, 150, 70))); // Outstation
    }

    [Fact]
    public void ZoomAbout_keeps_the_world_point_under_the_pointer_fixed()
    {
        float panX = 40f, panY = 25f, zoom = 1f;
        var pointer = new PointF(300f, 200f);

        var before = ViewportGeometry.ToWorld(pointer, panX, panY, zoom);
        ViewportGeometry.ZoomAbout(pointer, 1.5f, ref panX, ref panY, ref zoom);
        var after = ViewportGeometry.ToWorld(pointer, panX, panY, zoom);

        Assert.Equal(1.5f, zoom, 3);
        Assert.Equal(before.X, after.X, 2);
        Assert.Equal(before.Y, after.Y, 2);
    }

    [Fact]
    public void ZoomAbout_clamps_the_zoom_factor()
    {
        float panX = 0f, panY = 0f, zoom = 1f;

        for (var i = 0; i < 40; i++)
            ViewportGeometry.ZoomAbout(new PointF(100, 100), 1.5f, ref panX, ref panY, ref zoom);

        Assert.Equal(ViewportGeometry.MaximumZoom, zoom, 3);

        for (var i = 0; i < 80; i++)
            ViewportGeometry.ZoomAbout(new PointF(100, 100), 1f / 1.5f, ref panX, ref panY, ref zoom);

        Assert.Equal(ViewportGeometry.MinimumZoom, zoom, 3);
    }
}
