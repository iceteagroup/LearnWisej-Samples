using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Wisej.Web;

namespace OrderDesk.Services
{
    /// <summary>One stop of the tab-order walk: a control, its nesting depth and its bounds relative to the walked root.</summary>
    public sealed class TabStopInfo
    {
        public Control Control { get; set; }
        public int Depth { get; set; }
        public Rectangle Bounds { get; set; }
    }

    /// <summary>
    /// Lab helper for the "Tab order / docking" button: lists the controls of a ported page in
    /// the order the browser will tab through them (children ordered by TabIndex, containers
    /// before their children) and describes the layout properties that were adjusted in the port
    /// (Dock replaced Anchor, TabIndex values preserved).
    /// </summary>
    public static class TabOrderWalker
    {
        /// <summary>Controls of <paramref name="root"/> in tab order. Controls named <paramref name="excludeName"/> (the highlight overlay) are skipped.</summary>
        public static List<TabStopInfo> Collect(Control root, string excludeName = null)
        {
            var list = new List<TabStopInfo>();
            Walk(root, root, 0, list, excludeName);
            return list;
        }

        private static void Walk(Control root, Control parent, int depth, List<TabStopInfo> list, string excludeName)
        {
            foreach (var c in parent.Controls.Cast<Control>().OrderBy(c => c.TabIndex).ToList())
            {
                if (excludeName != null && c.Name == excludeName) continue;
                if (!c.Visible) continue;
                list.Add(new TabStopInfo { Control = c, Depth = depth, Bounds = BoundsIn(root, c) });
                if (c.Controls.Count > 0 && !(c is DataGridView) && !(c is ListBox))
                    Walk(root, c, depth + 1, list, excludeName);
            }
        }

        /// <summary>Bounds of <paramref name="c"/> in the coordinate space of <paramref name="root"/>.</summary>
        public static Rectangle BoundsIn(Control root, Control c)
        {
            int x = c.Left, y = c.Top;
            var p = c.Parent;
            while (p != null && p != root)
            {
                x += p.Left;
                y += p.Top;
                p = p.Parent;
            }
            return new Rectangle(x, y, c.Width, c.Height);
        }

        /// <summary>One trace line: "TabIndex=1 detailPanel (Panel) Dock=Right Anchor=Top, Left TabStop=True".</summary>
        public static string Describe(TabStopInfo info)
        {
            var c = info.Control;
            var indent = new string(' ', info.Depth * 2);
            return indent + "TabIndex=" + c.TabIndex + " " + c.Name + " (" + c.GetType().Name + ")";
        }

        public static string Layout(TabStopInfo info)
        {
            var c = info.Control;
            var dock = c.Dock == DockStyle.None ? "Anchor=" + c.Anchor : "Dock=" + c.Dock;
            return dock + " · " + c.Width + "×" + c.Height + " @ (" + info.Bounds.X + "," + info.Bounds.Y + ") · TabStop=" + c.TabStop + (c.Enabled ? "" : " · disabled");
        }
    }
}
