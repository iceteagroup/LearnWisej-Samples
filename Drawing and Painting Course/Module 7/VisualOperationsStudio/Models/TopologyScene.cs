using System;
using System.Collections.Generic;
using System.Drawing;
using VisualOperationsStudio.Geometry;

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

        public string Label { get; }

        /// <summary>World-space geometry. Screen coordinates never touch the model.</summary>
        public RectangleF Bounds { get; set; }

        /// <summary>"Normal", "Warning" or "Critical".</summary>
        public string Severity { get; }

        public bool Selected { get; set; }

        public PointF Centre => new PointF(Bounds.X + Bounds.Width / 2f, Bounds.Y + Bounds.Height / 2f);
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

        /// <summary>
        /// World to screen: apply the zoom, then the pan offset. <see cref="ToWorld"/> runs it backwards.
        /// </summary>
        public PointF ToScreen(PointF world) => ViewportGeometry.ToScreen(world, PanX, PanY, Zoom);

        public PointF ToWorld(PointF screen) => ViewportGeometry.ToWorld(screen, PanX, PanY, Zoom);

        /// <summary>The world rectangle currently visible, computed once per render for culling.</summary>
        public RectangleF VisibleWorld(int width, int height) =>
            ViewportGeometry.VisibleWorld(width, height, PanX, PanY, Zoom);

        /// <summary>
        /// The node under a screen point, or null over empty space. The list is walked backwards so the
        /// node drawn last - and therefore on top - is the one the user gets. The Canvas offers no
        /// point-in-path test to lean on, so this arithmetic is ours.
        /// </summary>
        public NodeModel HitTest(PointF screen)
        {
            var bounds = new List<RectangleF>(Nodes.Count);
            for (var i = 0; i < Nodes.Count; i++)
                bounds.Add(Nodes[i].Bounds);

            var index = ViewportGeometry.HitTest(bounds, ToWorld(screen));
            return index < 0 ? null : Nodes[index];
        }

        /// <summary>Keeps the world point under the pointer fixed while the zoom factor changes.</summary>
        public void ZoomAbout(PointF screen, float factor)
        {
            var panX = PanX;
            var panY = PanY;
            var zoom = Zoom;

            ViewportGeometry.ZoomAbout(screen, factor, ref panX, ref panY, ref zoom);

            PanX = panX;
            PanY = panY;
            Zoom = zoom;
        }

        /// <summary>The demonstration plant: six pumps and tanks, plus the lines between them.</summary>
        public static TopologyScene CreatePlant()
        {
            var scene = new TopologyScene { PanX = 40, PanY = 40 };

            scene.Nodes.Add(new NodeModel("tank-1", "Feed tank", new RectangleF(40, 60, 150, 70), "Normal"));
            scene.Nodes.Add(new NodeModel("pump-1", "Pump 1", new RectangleF(260, 40, 130, 60), "Normal"));
            scene.Nodes.Add(new NodeModel("pump-2", "Pump 2", new RectangleF(260, 150, 130, 60), "Warning"));
            scene.Nodes.Add(new NodeModel("press-3", "Line 3 Press", new RectangleF(470, 90, 160, 80), "Critical"));
            scene.Nodes.Add(new NodeModel("oven-4", "Cure oven", new RectangleF(700, 40, 140, 70), "Normal"));
            scene.Nodes.Add(new NodeModel("tank-2", "Waste tank", new RectangleF(700, 170, 140, 70), "Normal"));
            scene.Nodes.Add(new NodeModel("far-1", "Outstation", new RectangleF(1500, 520, 150, 70), "Normal"));

            scene.Edges.Add(new EdgeModel("e1", "tank-1", "pump-1"));
            scene.Edges.Add(new EdgeModel("e2", "tank-1", "pump-2"));
            scene.Edges.Add(new EdgeModel("e3", "pump-1", "press-3"));
            scene.Edges.Add(new EdgeModel("e4", "pump-2", "press-3"));
            scene.Edges.Add(new EdgeModel("e5", "press-3", "oven-4"));
            scene.Edges.Add(new EdgeModel("e6", "press-3", "tank-2"));

            return scene;
        }
    }
}
