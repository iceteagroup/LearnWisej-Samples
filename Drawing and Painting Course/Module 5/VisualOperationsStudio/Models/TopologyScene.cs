using System;
using System.Collections.Generic;
using System.Drawing;

namespace VisualOperationsStudio.Models
{
    /// <summary>
    /// A node of the plant topology, in world coordinates. A rectangle in the browser is not a pump:
    /// the meaning lives here, on the server, and the renderer only turns it into drawing calls.
    /// </summary>
    public class NodeModel
    {
        public NodeModel(string id, string label, RectangleF bounds, string severity)
        {
            Id = id;
            Label = label;
            Bounds = bounds;
            Severity = severity;
        }

        /// <summary>Stable across renders, drags and zooms - the selection is remembered by id.</summary>
        public string Id { get; }

        /// <summary>The name on the node, for example "Pump-02".</summary>
        public string Label { get; }

        /// <summary>World-space geometry. Screen coordinates never touch the model.</summary>
        public RectangleF Bounds { get; set; }

        /// <summary>"OK", "Warning" or "Critical".</summary>
        public string Severity { get; }

        public bool Selected { get; set; }

        /// <summary>The keyboard focus ring, which moves with Tab without changing the selection.</summary>
        public bool Focused { get; set; }

        public PointF Centre => new PointF(Bounds.X + Bounds.Width / 2f, Bounds.Y + Bounds.Height / 2f);

        /// <summary>The health colour of the band across the top of the node.</summary>
        public Color Tone =>
            Severity == "Critical" ? Color.FromArgb(214, 69, 69) :
            Severity == "Warning" ? Color.FromArgb(232, 161, 60) :
            Color.FromArgb(31, 157, 107);
    }

    /// <summary>A link between two nodes, by id.</summary>
    public class EdgeModel
    {
        public EdgeModel(string id, string fromId, string toId)
        {
            Id = id;
            FromId = fromId;
            ToId = toId;
        }

        public string Id { get; }

        public string FromId { get; }

        public string ToId { get; }
    }

    /// <summary>
    /// The whole scene: nodes, edges and the viewport. Held per session on the page instance - never in
    /// a mutable static field, which every user of the application would share.
    /// </summary>
    public class TopologyScene
    {
        public const float MinimumZoom = 0.4f;
        public const float MaximumZoom = 3f;

        public List<NodeModel> Nodes { get; } = new List<NodeModel>();

        public List<EdgeModel> Edges { get; } = new List<EdgeModel>();

        public float PanX { get; set; }

        public float PanY { get; set; }

        public float Zoom { get; set; } = 1f;

        public NodeModel Selected
        {
            get
            {
                for (var i = 0; i < Nodes.Count; i++)
                {
                    if (Nodes[i].Selected)
                        return Nodes[i];
                }

                return null;
            }
        }

        public NodeModel Find(string id)
        {
            for (var i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i].Id == id)
                    return Nodes[i];
            }

            return null;
        }

        public void Select(NodeModel node)
        {
            for (var i = 0; i < Nodes.Count; i++)
                Nodes[i].Selected = false;

            if (node != null)
                node.Selected = true;
        }

        public void Focus(NodeModel node)
        {
            for (var i = 0; i < Nodes.Count; i++)
                Nodes[i].Focused = false;

            if (node != null)
                node.Focused = true;
        }

        /// <summary>
        /// World to screen: apply the zoom, then the pan offset. <see cref="ToWorld"/> runs it backwards.
        /// </summary>
        public PointF ToScreen(PointF world) =>
            new PointF(world.X * Zoom + PanX, world.Y * Zoom + PanY);

        public PointF ToWorld(PointF screen) =>
            new PointF((screen.X - PanX) / Zoom, (screen.Y - PanY) / Zoom);

        /// <summary>The world rectangle currently visible, computed once per render for culling.</summary>
        public RectangleF VisibleWorld(int width, int height)
        {
            var topLeft = ToWorld(new PointF(0, 0));
            var bottomRight = ToWorld(new PointF(width, height));
            return RectangleF.FromLTRB(topLeft.X, topLeft.Y, bottomRight.X, bottomRight.Y);
        }

        /// <summary>
        /// The node under a screen point, or null over empty space. The list is walked backwards so the
        /// node drawn last - and therefore on top - is the one the user gets. The Canvas offers no
        /// point-in-path test to lean on, so this arithmetic is ours.
        /// </summary>
        public NodeModel HitTest(PointF screen)
        {
            var world = ToWorld(screen);

            for (var i = Nodes.Count - 1; i >= 0; i--)
            {
                if (Nodes[i].Bounds.Contains(world))
                    return Nodes[i];
            }

            return null;
        }

        /// <summary>Keeps the world point under the pointer fixed while the zoom factor changes.</summary>
        public void ZoomAbout(PointF screen, float factor)
        {
            var before = ToWorld(screen);

            Zoom = Math.Min(MaximumZoom, Math.Max(MinimumZoom, Zoom * factor));

            var after = ToWorld(screen);
            PanX += (after.X - before.X) * Zoom;
            PanY += (after.Y - before.Y) * Zoom;
        }

        /// <summary>Fits every node into <paramref name="width"/> x <paramref name="height"/>.</summary>
        public void Fit(int width, int height)
        {
            if (Nodes.Count == 0 || width <= 0 || height <= 0)
                return;

            var bounds = Nodes[0].Bounds;
            for (var i = 1; i < Nodes.Count; i++)
                bounds = RectangleF.Union(bounds, Nodes[i].Bounds);

            const int margin = 40;
            var zoom = Math.Min(
                (width - margin * 2) / Math.Max(1f, bounds.Width),
                (height - margin * 2) / Math.Max(1f, bounds.Height));

            Zoom = Math.Min(MaximumZoom, Math.Max(MinimumZoom, zoom));
            PanX = (width - bounds.Width * Zoom) / 2f - bounds.X * Zoom;
            PanY = (height - bounds.Height * Zoom) / 2f - bounds.Y * Zoom;
        }

        /// <summary>
        /// The demonstration plant. The seven nodes and their world coordinates are the ones the
        /// walkthrough draws, Spare-09 included: it sits far enough out to be culled at most zooms.
        /// </summary>
        public static TopologyScene CreatePlant()
        {
            var scene = new TopologyScene { PanX = 40, PanY = 40, Zoom = 0.72f };

            scene.Nodes.Add(new NodeModel("ingest", "Ingest-01", new RectangleF(30, 90, 180, 78), "OK"));
            scene.Nodes.Add(new NodeModel("valve", "Valve-07", new RectangleF(300, 30, 180, 78), "OK"));
            scene.Nodes.Add(new NodeModel("pump", "Pump-02", new RectangleF(300, 220, 180, 78), "Warning"));
            scene.Nodes.Add(new NodeModel("mixer", "Mixer-A", new RectangleF(540, 125, 180, 78), "OK"));
            scene.Nodes.Add(new NodeModel("silo", "Silo-03", new RectangleF(900, 30, 180, 78), "OK"));
            scene.Nodes.Add(new NodeModel("export", "Export-01", new RectangleF(900, 235, 180, 78), "Critical"));
            scene.Nodes.Add(new NodeModel("spare", "Spare-09", new RectangleF(1230, 350, 180, 78), "OK"));

            scene.Edges.Add(new EdgeModel("e1", "ingest", "valve"));
            scene.Edges.Add(new EdgeModel("e2", "ingest", "pump"));
            scene.Edges.Add(new EdgeModel("e3", "valve", "mixer"));
            scene.Edges.Add(new EdgeModel("e4", "pump", "mixer"));
            scene.Edges.Add(new EdgeModel("e5", "mixer", "silo"));
            scene.Edges.Add(new EdgeModel("e6", "mixer", "export"));
            scene.Edges.Add(new EdgeModel("e7", "export", "spare"));

            return scene;
        }
    }
}
