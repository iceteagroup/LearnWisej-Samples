using System.Drawing;

namespace OrderDesk.Shared
{
    /// <summary>Colors shared by every module sample so the course apps look like one product.</summary>
    public static class Palette
    {
        public static readonly Color PageBackground = Color.FromArgb(238, 242, 247);
        public static readonly Color CardBackground = Color.White;
        public static readonly Color Accent = Color.FromArgb(21, 101, 216);      // #1565d8 — the OrderDesk blue
        public static readonly Color AccentSoft = Color.FromArgb(234, 243, 255); // #eaf3ff
        public static readonly Color Ink = Color.FromArgb(31, 45, 58);           // #1f2d3a
        public static readonly Color MutedText = Color.FromArgb(90, 107, 125);   // #5a6b7d
        public static readonly Color Good = Color.FromArgb(31, 157, 107);        // #1f9d6b
        public static readonly Color GoodSoft = Color.FromArgb(240, 250, 244);
        public static readonly Color Warn = Color.FromArgb(232, 161, 60);        // #e8a13c
        public static readonly Color WarnSoft = Color.FromArgb(255, 244, 229);
        public static readonly Color Bad = Color.FromArgb(224, 86, 59);          // #e0563b
        public static readonly Color BadSoft = Color.FromArgb(253, 236, 234);
        public static readonly Color Purple = Color.FromArgb(125, 90, 224);      // #7d5ae0
        public static readonly Color Border = Color.FromArgb(220, 228, 236);     // #dce4ec
        public static readonly Color PanelBackground = Color.FromArgb(244, 246, 249);

        public static Color StatusColor(Domain.OrderStatus status)
        {
            switch (status)
            {
                case Domain.OrderStatus.Shipped: return Good;
                case Domain.OrderStatus.Invoiced: return Purple;
                case Domain.OrderStatus.InProgress: return Warn;
                case Domain.OrderStatus.Hold: return Bad;
                default: return Accent;
            }
        }
    }
}
