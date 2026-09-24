using System.Drawing;
using Wisej.Web;

namespace GlobalDesk
{
    partial class CustomerEditorPage
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Wisej.NET Designer generated code

        //
        // The host page is NOT localizable in the designer sense: its one caption is assigned from
        // the shared resource, so it follows a culture change without a rebuild. The control it
        // hosts is the opposite, and the contrast is what Modules 2 and 3 are about.
        //
        // Module 2's culture chip is gone from this screen. The session's culture now arrives with
        // the request - from the browser, or from ?lang= in the address bar - which is what the
        // walkthrough types into the URL.
        //
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pnlAppBar = new Wisej.Web.Panel();
            this.lblAppTitle = new Wisej.Web.Label();
            this.pnlWindowGlyphs = new Wisej.Web.Panel();
            this.lblGlyphMinimize = new Wisej.Web.Label();
            this.lblGlyphMaximize = new Wisej.Web.Label();
            this.lblGlyphClose = new Wisej.Web.Label();
            this.pnlEditorHost = new Wisej.Web.Panel();
            this.SuspendLayout();
            //
            // ── the application bar ────────────────────────────────────────────────────────
            //
            this.lblAppTitle.Name = "lblAppTitle";
            this.lblAppTitle.AutoSize = false;
            this.lblAppTitle.Dock = Wisej.Web.DockStyle.Fill;
            this.lblAppTitle.Padding = new Wisej.Web.Padding(18, 0, 0, 0);
            this.lblAppTitle.ForeColor = Color.White;
            this.lblAppTitle.Font = Desk.Px(15, FontStyle.Bold);
            this.lblAppTitle.TextAlign = ContentAlignment.MiddleLeft;

            SetGlyph(this.lblGlyphMinimize, "lblGlyphMinimize", "—");
            SetGlyph(this.lblGlyphMaximize, "lblGlyphMaximize", "□");
            SetGlyph(this.lblGlyphClose, "lblGlyphClose", "✕");

            this.pnlWindowGlyphs.Name = "pnlWindowGlyphs";
            this.pnlWindowGlyphs.Dock = Wisej.Web.DockStyle.Right;
            this.pnlWindowGlyphs.Size = new Size(108, 40);
            this.pnlWindowGlyphs.BackColor = Desk.Accent;
            this.pnlWindowGlyphs.Controls.Add(this.lblGlyphMinimize);
            this.pnlWindowGlyphs.Controls.Add(this.lblGlyphMaximize);
            this.pnlWindowGlyphs.Controls.Add(this.lblGlyphClose);

            this.pnlAppBar.Name = "pnlAppBar";
            this.pnlAppBar.Dock = Wisej.Web.DockStyle.Top;
            this.pnlAppBar.Size = new Size(1400, 40);
            this.pnlAppBar.BackColor = Desk.Accent;
            this.pnlAppBar.Controls.Add(this.lblAppTitle);
            this.pnlAppBar.Controls.Add(this.pnlWindowGlyphs);
            //
            // ── where the designer-localized control lives ─────────────────────────────────
            //
            this.pnlEditorHost.Name = "pnlEditorHost";
            this.pnlEditorHost.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlEditorHost.BackColor = Color.White;
            this.pnlEditorHost.Padding = new Wisej.Web.Padding(26, 18, 26, 20);
            //
            // CustomerEditorPage
            //
            this.Name = "CustomerEditorPage";
            this.BackColor = Color.White;
            this.Controls.Add(this.pnlEditorHost);
            this.Controls.Add(this.pnlAppBar);
            this.ResumeLayout(false);
        }

        /// <summary>One of the three window glyphs at the right of the application bar.</summary>
        private static void SetGlyph(Wisej.Web.Label label, string name, string glyph)
        {
            label.Name = name;
            label.AutoSize = false;
            label.Dock = Wisej.Web.DockStyle.Right;
            label.Size = new Size(36, 40);
            // A glyph is a shape, not a word. It never goes through a resource.
            label.Text = glyph;
            label.ForeColor = Color.White;
            label.Font = Desk.Px(13);
            label.TextAlign = ContentAlignment.MiddleCenter;
        }

        #endregion

        private Wisej.Web.Panel pnlAppBar;
        private Wisej.Web.Label lblAppTitle;
        private Wisej.Web.Panel pnlWindowGlyphs;
        private Wisej.Web.Label lblGlyphMinimize;
        private Wisej.Web.Label lblGlyphMaximize;
        private Wisej.Web.Label lblGlyphClose;
        private Wisej.Web.Panel pnlEditorHost;
    }
}
