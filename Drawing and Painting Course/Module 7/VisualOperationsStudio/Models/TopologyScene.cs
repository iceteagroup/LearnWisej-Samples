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

        /// <summary>"None", "Normal", "Warning" or "Critical". "None" is a link, not an asset.</summary>
        public string Severity { get; }

        public bool Selected { get; set; }

        public PointF Centre => new PointF(Bounds.X + Bounds.Width / 2f, Bounds.Y + Bounds.Height / 2f);

        /// <summary>
        /// The outline and label colour, from the one set of thresholds. A selected node is drawn in
        /// the selection colour instead, which the renderer decides - not the model.
        /// </summary>
        public Color Tone =>
            Severity == "Critical" ? Color.FromArgb(224, 90, 90) :
            Severity == "Warning" ? Color.FromArgb(232, 161, 60) :
            Severity == "Normal" ? Color.FromArgb(31, 157, 107) :
            Color.FromArgb(185, 201, 220);

        /// <summary>A link node carries no state of its own, so its label stays neutral.</summary>
        public Color TextTone => Severity == "None" ? Color.FromArgb(70, 88, 106) : Tone;
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

        /// <summary>
        /// The four assets of the capstone screen and the line that joins them, in the world
        /// coordinates the walkthrough's topology card is drawn from. PUMP-04 opens selected, which is
        /// the same asset the painted gauge and the first grid row show.
        /// </summary>
        public static TopologyScene CreateAssets()
        {
            var scene = new TopologyScene();

            scene.Nodes.Add(new NodeModel("pump", "PUMP-04", new RectangleF(30, 50, 80, 38), "Warning"));
            scene.Nodes.Add(new NodeModel("line", "LINE-A", new RectangleF(132, 112, 76, 36), "None"));
            scene.Nodes.Add(new NodeModel("mix", "MIX-11", new RectangleF(218, 52, 74, 36), "Critical"));
            scene.Nodes.Add(new NodeModel("dry", "DRY-02", new RectangleF(68, 182, 74, 36), "Normal"));
            scene.Nodes.Add(new NodeModel("pack", "PACK-07", new RectangleF(208, 177, 76, 36), "Warning"));

            scene.Edges.Add(new EdgeModel("e1", "pump", "line"));
            scene.Edges.Add(new EdgeModel("e2", "line", "mix"));
            scene.Edges.Add(new EdgeModel("e3", "line", "dry"));
            scene.Edges.Add(new EdgeModel("e4", "line", "pack"));

            scene.Select(scene.Find("pump"));

            return scene;
        }
    }
}
