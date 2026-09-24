using System.Drawing;

namespace GlobalDesk
{
    /// <summary>
    /// The GlobalDesk palette and the two or three measurements every screen repeats.
    ///
    /// It exists so the designer files read as layout rather than as a wall of
    /// <c>Color.FromArgb</c>, and so a colour is changed in one place. Nothing here is
    /// localizable: a colour and a pixel are the same in every language, which is also why the
    /// designer-localized controls set their colours in code and leave only text, size and
    /// position in the resource file.
    /// </summary>
    internal static class Desk
    {
        public static readonly Color Accent = Color.FromArgb(0x15, 0x65, 0xD8);    // app bar, primary button
        public static readonly Color Ink = Color.FromArgb(0x0D, 0x1B, 0x2A);       // headings
        public static readonly Color Body = Color.FromArgb(0x1F, 0x2D, 0x3A);      // values
        public static readonly Color Caption = Color.FromArgb(0x34, 0x46, 0x5A);   // row captions
        public static readonly Color FieldInk = Color.FromArgb(0x3A, 0x4D, 0x63);  // field labels, secondary buttons
        public static readonly Color GroupInk = Color.FromArgb(0x0B, 0x4E, 0xA8);  // group headers
        public static readonly Color Muted = Color.FromArgb(0x5A, 0x6B, 0x7D);     // status, secondary
        public static readonly Color Line = Color.FromArgb(0xE0, 0xE7, 0xEF);      // card border
        public static readonly Color RowLine = Color.FromArgb(0xEE, 0xF2, 0xF7);   // row separator
        public static readonly Color CardHead = Color.FromArgb(0xF4, 0xF7, 0xFA);  // card header / neutral strip
        public static readonly Color Rail = Color.FromArgb(0xF1, 0xF5, 0xF9);      // navigation column
        public static readonly Color RailActive = Color.FromArgb(0xEA, 0xF3, 0xFF);// selected navigation entry
        public static readonly Color StripBack = Color.FromArgb(0xF4, 0xF8, 0xFF); // host strip
        public static readonly Color GoodInk = Color.FromArgb(0x17, 0x80, 0x4F);
        public static readonly Color GoodBack = Color.FromArgb(0xE6, 0xF6, 0xEE);
        public static readonly Color BadInk = Color.FromArgb(0xA3, 0x20, 0x20);
        public static readonly Color BadBack = Color.FromArgb(0xFD, 0xEC, 0xEC);
        public static readonly Color WarnInk = Color.FromArgb(0x8A, 0x5A, 0x12);
        public static readonly Color WarnBack = Color.FromArgb(0xFF, 0xF6, 0xE8);
        public static readonly Color ChipBlueInk = Color.FromArgb(0x0B, 0x4E, 0xA8);
        public static readonly Color ChipBlueBack = Color.FromArgb(0xEA, 0xF3, 0xFF);
        public static readonly Color Purple = Color.FromArgb(0x7D, 0x5A, 0xE0);
        public static readonly Color Teal = Color.FromArgb(0x1F, 0x9D, 0x6B);
        public static readonly Color Amber = Color.FromArgb(0xE8, 0xA1, 0x3C);

        // The walkthrough measures in CSS pixels; Wisej sizes fonts in points (1pt = 4/3 px).
        public static Font Px(float px, FontStyle style = FontStyle.Regular) =>
            new Font("default", px * 0.75F, style);

        public static Font Mono(float px, FontStyle style = FontStyle.Regular) =>
            new Font("Consolas", px * 0.75F, style);
    }
}
