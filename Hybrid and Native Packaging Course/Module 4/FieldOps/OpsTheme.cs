using System.Drawing;
using FieldOps.Model;

namespace FieldOps
{
    /// <summary>
    /// The FieldOps palette from the walkthrough, and the conversion from its CSS pixel sizes to
    /// the points Wisej.NET uses for fonts. Layout, not content.
    /// </summary>
    internal static class Ops
    {
        public static readonly Color Accent = Color.FromArgb(0x15, 0x65, 0xD8);     // app bar, primary button
        public static readonly Color Ink = Color.FromArgb(0x1F, 0x2D, 0x3A);        // site names, values
        public static readonly Color Body = Color.FromArgb(0x46, 0x58, 0x6A);       // fault text
        public static readonly Color Muted = Color.FromArgb(0x5A, 0x6B, 0x7D);      // header row, captions
        public static readonly Color SecondaryInk = Color.FromArgb(0x3A, 0x4D, 0x63);
        public static readonly Color SecondaryBorder = Color.FromArgb(0xCD, 0xD9, 0xE6);
        public static readonly Color Strip = Color.FromArgb(0xF8, 0xFA, 0xFC);      // action + status strips
        public static readonly Color StripLine = Color.FromArgb(0xE4, 0xEA, 0xF1);
        public static readonly Color HeaderBack = Color.FromArgb(0xF4, 0xF7, 0xFA);
        public static readonly Color RowAlt = Color.FromArgb(0xFB, 0xFC, 0xFE);
        public static readonly Color RowLine = Color.FromArgb(0xF0, 0xF3, 0xF7);
        public static readonly Color High = Color.FromArgb(0xC0, 0x39, 0x2B);
        public static readonly Color Good = Color.FromArgb(0x1F, 0x9D, 0x6B);
        public static readonly Color Bad = Color.FromArgb(0xC0, 0x39, 0x2B);
        public static readonly Color Warn = Color.FromArgb(0x8A, 0x6D, 0x1F);
        public static readonly Color GoodBack = Color.FromArgb(0xE6, 0xF6, 0xEE);
        public static readonly Color BadBack = Color.FromArgb(0xFD, 0xEC, 0xEA);
        public static readonly Color WarnBack = Color.FromArgb(0xFF, 0xF8, 0xE6);

        public static Font Px(float px, FontStyle style = FontStyle.Regular) =>
            new Font("default", px * 0.75F, style);

        public static Font Mono(float px, FontStyle style = FontStyle.Regular) =>
            new Font("Consolas", px * 0.75F, style);

        /// <summary>The status chip colours of the walkthrough.</summary>
        public static string StatusHex(WorkOrderStatus s) => s switch
        {
            WorkOrderStatus.Assigned => "#7d5ae0",
            WorkOrderStatus.Accepted => "#1565d8",
            WorkOrderStatus.OnSite => "#e8a13c",
            _ => "#1f9d6b",
        };

        /// <summary>A rounded status chip, rendered as HTML in a grid cell or a label.</summary>
        public static string StatusChip(WorkOrderStatus s, float px = 10.5f)
        {
            var c = StatusHex(s);
            return $"<span style=\"font-size:{px}px;font-weight:800;color:{c};background:{c}1a;border:1px solid {c}55;border-radius:999px;padding:2px 8px;white-space:nowrap\">{WorkOrder.StatusText(s)}</span>";
        }
    }
}
