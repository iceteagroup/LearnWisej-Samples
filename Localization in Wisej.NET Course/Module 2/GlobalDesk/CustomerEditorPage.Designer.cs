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
        // The host page is NOT localizable in the designer sense: its captions are assigned from
        // the shared resource in ApplyTextResources, so they follow a culture change without a
        // rebuild. The control it hosts is the opposite, and the contrast is the module.
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
            this.pnlHost = new Wisej.Web.Panel();
            this.pnlStrip = new Wisej.Web.Panel();
            this.lblCultureCaption = new Wisej.Web.Label();
            this.btnCulture = new Wisej.Web.Button();
            this.btnRecreateEditor = new Wisej.Web.Button();
            this.pnlEditorHost = new Wisej.Web.Panel();
            this.lblStatus = new Wisej.Web.Label();
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
            this.pnlWindowGlyphs.Size = new Size(108, 42);
            this.pnlWindowGlyphs.BackColor = Desk.Accent;
            this.pnlWindowGlyphs.Controls.Add(this.lblGlyphMinimize);
            this.pnlWindowGlyphs.Controls.Add(this.lblGlyphMaximize);
            this.pnlWindowGlyphs.Controls.Add(this.lblGlyphClose);

            this.pnlAppBar.Name = "pnlAppBar";
            this.pnlAppBar.Dock = Wisej.Web.DockStyle.Top;
            this.pnlAppBar.Size = new Size(1400, 42);
            this.pnlAppBar.BackColor = Desk.Accent;
            this.pnlAppBar.Controls.Add(this.lblAppTitle);
            this.pnlAppBar.Controls.Add(this.pnlWindowGlyphs);
            //
            // ── the host strip: which culture this session is on, and the rebuild button ───
            //
            // "Application.CurrentCulture" is a member name, not a word. It is deliberately not
            // a resource: translating an API name would make the screen harder to read, not
            // easier.
            this.lblCultureCaption.Name = "lblCultureCaption";
            this.lblCultureCaption.AutoSize = false;
            this.lblCultureCaption.Location = new Point(18, 0);
            this.lblCultureCaption.Size = new Size(212, 44);
            this.lblCultureCaption.Font = Desk.Px(15, FontStyle.Bold);
            this.lblCultureCaption.ForeColor = Desk.FieldInk;
            this.lblCultureCaption.Text = "Application.CurrentCulture";
            this.lblCultureCaption.TextAlign = ContentAlignment.MiddleLeft;

            // The chip is the module's culture switch: one click moves the session to the next
            // culture and changes nothing else on screen. Its colours are set in code, because
            // they depend on which culture is active rather than on the language.
            this.btnCulture.Name = "btnCulture";
            this.btnCulture.AutoSize = true;
            this.btnCulture.Location = new Point(236, 7);
            this.btnCulture.MinimumSize = new Size(92, 30);
            this.btnCulture.Padding = new Wisej.Web.Padding(13, 0, 13, 0);
            this.btnCulture.Font = Desk.Px(13, FontStyle.Bold);
            this.btnCulture.TabIndex = 0;
            this.btnCulture.Click += this.btnCulture_Click;

            this.btnRecreateEditor.Name = "btnRecreateEditor";
            this.btnRecreateEditor.AutoSize = true;
            this.btnRecreateEditor.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.btnRecreateEditor.Location = new Point(1150, 7);
            this.btnRecreateEditor.MinimumSize = new Size(172, 30);
            this.btnRecreateEditor.Padding = new Wisej.Web.Padding(20, 0, 20, 0);
            this.btnRecreateEditor.BackColor = Desk.Accent;
            this.btnRecreateEditor.ForeColor = Color.White;
            this.btnRecreateEditor.Font = Desk.Px(14, FontStyle.Bold);
            this.btnRecreateEditor.CssStyle = "border-radius:7px;border:none";
            this.btnRecreateEditor.Click += this.btnRecreateEditor_Click;

            this.pnlStrip.Name = "pnlStrip";
            this.pnlStrip.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlStrip.Size = new Size(1340, 44);
            this.pnlStrip.BackColor = Desk.StripBack;
            this.pnlStrip.CssStyle = "border:1px solid #d8e7fb;border-radius:10px";
            this.pnlStrip.Controls.Add(this.lblCultureCaption);
            this.pnlStrip.Controls.Add(this.btnCulture);
            this.pnlStrip.Controls.Add(this.btnRecreateEditor);

            this.pnlHost.Name = "pnlHost";
            this.pnlHost.Dock = Wisej.Web.DockStyle.Top;
            this.pnlHost.Size = new Size(1400, 66);
            this.pnlHost.BackColor = Color.White;
            this.pnlHost.Padding = new Wisej.Web.Padding(30, 12, 30, 10);
            this.pnlHost.Controls.Add(this.pnlStrip);
            //
            // ── where the designer-localized control lives ─────────────────────────────────
            //
            // A host panel makes the rebuild three lines instead of a hunt: clear it, dispose the
            // old instance, add a new one.
            this.pnlEditorHost.Name = "pnlEditorHost";
            this.pnlEditorHost.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlEditorHost.BackColor = Color.White;
            this.pnlEditorHost.Padding = new Wisej.Web.Padding(28, 20, 28, 22);
            //
            // lblStatus
            //
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.AutoSize = false;
            this.lblStatus.Dock = Wisej.Web.DockStyle.Bottom;
            this.lblStatus.Size = new Size(1400, 40);
            this.lblStatus.Padding = new Wisej.Web.Padding(30, 0, 30, 0);
            this.lblStatus.Font = Desk.Px(14, FontStyle.Bold);
            this.lblStatus.BackColor = Desk.CardHead;
            this.lblStatus.ForeColor = Desk.Muted;
            this.lblStatus.CssStyle = "border-top:1px solid #e4eaf1";
            this.lblStatus.TextAlign = ContentAlignment.MiddleLeft;
            //
            // CustomerEditorPage
            //
            this.Name = "CustomerEditorPage";
            this.BackColor = Color.White;
            this.Controls.Add(this.pnlEditorHost);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.pnlHost);
            this.Controls.Add(this.pnlAppBar);
            this.ResumeLayout(false);
        }

        /// <summary>One of the three window glyphs at the right of the application bar.</summary>
        private static void SetGlyph(Wisej.Web.Label label, string name, string glyph)
        {
            label.Name = name;
            label.AutoSize = false;
            label.Dock = Wisej.Web.DockStyle.Right;
            label.Size = new Size(36, 42);
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
        private Wisej.Web.Panel pnlHost;
        private Wisej.Web.Panel pnlStrip;
        private Wisej.Web.Label lblCultureCaption;
        private Wisej.Web.Button btnCulture;
        private Wisej.Web.Button btnRecreateEditor;
        private Wisej.Web.Panel pnlEditorHost;
        private Wisej.Web.Label lblStatus;
    }
}
