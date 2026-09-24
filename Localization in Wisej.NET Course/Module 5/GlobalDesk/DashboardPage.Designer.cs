using System.Drawing;
using Wisej.Web;

namespace GlobalDesk
{
    partial class DashboardPage
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Application.CultureChanged outlives the page.
                Application.CultureChanged -= this.Application_CultureChanged;

                if (components != null)
                    components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Wisej.NET Designer generated code

        //
        // Module 5 rebuilds this layout so a longer translation cannot clip it. Nothing here is
        // positioned absolutely: the navigation column and the status strip are docked, the main
        // area fills what is left, and the rows inside the editor flow.
        //
        // Nothing here is mirrored by hand either. RightToLeftLayout on the page (set in the
        // constructor) mirrors the arrangement, and every child is left on RightToLeft.Inherit.
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
            this.lblStatus = new Wisej.Web.Label();
            this.pnlNav = new Wisej.Web.Panel();
            this.lblNavSettings = new Wisej.Web.Label();
            this.lblNavContacts = new Wisej.Web.Label();
            this.lblNavCustomers = new Wisej.Web.Label();
            this.lblNavDashboard = new Wisej.Web.Label();
            this.pnlMain = new Wisej.Web.Panel();
            this.pnlTop = new Wisej.Web.Panel();
            this.lblWelcome = new Wisej.Web.Label();
            this.cboLanguage = new Wisej.Web.ComboBox();
            this.pnlEditorHost = new Wisej.Web.Panel();
            this.SuspendLayout();
            //
            // ── the application bar ────────────────────────────────────────────────────────
            //
            this.lblAppTitle.Name = "lblAppTitle";
            this.lblAppTitle.AutoSize = false;
            this.lblAppTitle.Dock = Wisej.Web.DockStyle.Fill;
            this.lblAppTitle.Padding = new Wisej.Web.Padding(16, 0, 0, 0);
            this.lblAppTitle.ForeColor = Color.White;
            this.lblAppTitle.Font = Desk.Px(14, FontStyle.Bold);
            this.lblAppTitle.TextAlign = ContentAlignment.MiddleLeft;

            SetGlyph(this.lblGlyphMinimize, "lblGlyphMinimize", "—");
            SetGlyph(this.lblGlyphMaximize, "lblGlyphMaximize", "□");
            SetGlyph(this.lblGlyphClose, "lblGlyphClose", "✕");

            // The glyph strip mirrors with everything else: a right-to-left window puts its
            // controls at the other end, which is what the operating system does too.
            this.pnlWindowGlyphs.Name = "pnlWindowGlyphs";
            this.pnlWindowGlyphs.Dock = Wisej.Web.DockStyle.Right;
            this.pnlWindowGlyphs.Size = new Size(108, 38);
            this.pnlWindowGlyphs.BackColor = Desk.Accent;
            this.pnlWindowGlyphs.Controls.Add(this.lblGlyphMinimize);
            this.pnlWindowGlyphs.Controls.Add(this.lblGlyphMaximize);
            this.pnlWindowGlyphs.Controls.Add(this.lblGlyphClose);

            this.pnlAppBar.Name = "pnlAppBar";
            this.pnlAppBar.Dock = Wisej.Web.DockStyle.Top;
            this.pnlAppBar.Size = new Size(1400, 38);
            this.pnlAppBar.BackColor = Desk.Accent;
            this.pnlAppBar.Controls.Add(this.lblAppTitle);
            this.pnlAppBar.Controls.Add(this.pnlWindowGlyphs);
            //
            // ── the navigation column ──────────────────────────────────────────────────────
            //
            // Docked, not positioned, and wide enough for Kontaktverzeichnis. A fixed 146 px
            // column is exactly how German clips.
            SetNavEntry(this.lblNavSettings, "lblNavSettings", false);
            SetNavEntry(this.lblNavContacts, "lblNavContacts", false);
            SetNavEntry(this.lblNavCustomers, "lblNavCustomers", false);
            SetNavEntry(this.lblNavDashboard, "lblNavDashboard", true);

            this.pnlNav.Name = "pnlNav";
            this.pnlNav.Dock = Wisej.Web.DockStyle.Left;
            this.pnlNav.Size = new Size(210, 780);
            this.pnlNav.MinimumSize = new Size(210, 0);
            this.pnlNav.BackColor = Desk.Rail;
            this.pnlNav.Padding = new Wisej.Web.Padding(10, 14, 10, 14);
            this.pnlNav.CssStyle = "border-inline-end:1px solid #e4eaf1";
            this.pnlNav.Controls.Add(this.lblNavSettings);
            this.pnlNav.Controls.Add(this.lblNavContacts);
            this.pnlNav.Controls.Add(this.lblNavCustomers);
            this.pnlNav.Controls.Add(this.lblNavDashboard);
            //
            // ── the main area: greeting, language picker, and the customer screen ──────────
            //
            this.lblWelcome.Name = "lblWelcome";
            this.lblWelcome.AutoSize = false;
            this.lblWelcome.Dock = Wisej.Web.DockStyle.Fill;
            this.lblWelcome.Font = Desk.Px(19, FontStyle.Bold);
            this.lblWelcome.ForeColor = Desk.Ink;
            this.lblWelcome.TextAlign = ContentAlignment.MiddleLeft;

            this.cboLanguage.Name = "cboLanguage";
            this.cboLanguage.Dock = Wisej.Web.DockStyle.Right;
            this.cboLanguage.Size = new Size(240, 32);
            this.cboLanguage.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboLanguage.Font = Desk.Px(13.5F);
            this.cboLanguage.ForeColor = Desk.Body;
            this.cboLanguage.CssStyle = "border:1.5px solid #c9d4e0;border-radius:6px";
            this.cboLanguage.TabIndex = 0;
            this.cboLanguage.SelectedIndexChanged += this.cboLanguage_SelectedIndexChanged;

            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Dock = Wisej.Web.DockStyle.Top;
            this.pnlTop.Size = new Size(1190, 40);
            this.pnlTop.BackColor = Color.White;
            this.pnlTop.Controls.Add(this.lblWelcome);
            this.pnlTop.Controls.Add(this.cboLanguage);

            this.pnlEditorHost.Name = "pnlEditorHost";
            this.pnlEditorHost.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlEditorHost.BackColor = Color.White;
            this.pnlEditorHost.Padding = new Wisej.Web.Padding(0, 14, 0, 0);

            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlMain.BackColor = Color.White;
            this.pnlMain.Padding = new Wisej.Web.Padding(20, 16, 20, 16);
            this.pnlMain.Controls.Add(this.pnlEditorHost);
            this.pnlMain.Controls.Add(this.pnlTop);
            //
            // lblStatus
            //
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.AutoSize = false;
            this.lblStatus.Dock = Wisej.Web.DockStyle.Bottom;
            this.lblStatus.Size = new Size(1400, 30);
            this.lblStatus.Padding = new Wisej.Web.Padding(16, 0, 16, 0);
            this.lblStatus.Font = Desk.Px(12.5F, FontStyle.Bold);
            this.lblStatus.BackColor = Desk.CardHead;
            this.lblStatus.ForeColor = Desk.Muted;
            this.lblStatus.CssStyle = "border-top:1px solid #e4eaf1";
            this.lblStatus.TextAlign = ContentAlignment.MiddleLeft;
            //
            // DashboardPage
            //
            this.Name = "DashboardPage";
            this.BackColor = Color.White;
            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.pnlNav);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.pnlAppBar);
            this.ResumeLayout(false);
        }

        /// <summary>One of the three window glyphs at the right of the application bar.</summary>
        private static void SetGlyph(Wisej.Web.Label label, string name, string glyph)
        {
            label.Name = name;
            label.AutoSize = false;
            label.Dock = Wisej.Web.DockStyle.Right;
            label.Size = new Size(36, 38);
            // A glyph is a shape, not a word. It never goes through a resource.
            label.Text = glyph;
            label.ForeColor = Color.White;
            label.Font = Desk.Px(13);
            label.TextAlign = ContentAlignment.MiddleCenter;
        }

        /// <summary>One entry in the navigation column. AutoSize height with a docked top edge,
        /// so a two-line translation grows the entry instead of cutting it.</summary>
        private static void SetNavEntry(Wisej.Web.Label label, string name, bool selected)
        {
            label.Name = name;
            label.AutoSize = false;
            label.Dock = Wisej.Web.DockStyle.Top;
            label.Size = new Size(190, 34);
            label.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            label.Font = Desk.Px(13, selected ? FontStyle.Bold : FontStyle.Regular);
            label.ForeColor = selected ? Desk.Accent : Desk.Muted;
            label.BackColor = selected ? Desk.RailActive : Color.Transparent;
            label.CssStyle = "border-radius:8px";
            label.TextAlign = ContentAlignment.MiddleLeft;
        }

        #endregion

        private Wisej.Web.Panel pnlAppBar;
        private Wisej.Web.Label lblAppTitle;
        private Wisej.Web.Panel pnlWindowGlyphs;
        private Wisej.Web.Label lblGlyphMinimize;
        private Wisej.Web.Label lblGlyphMaximize;
        private Wisej.Web.Label lblGlyphClose;
        private Wisej.Web.Panel pnlNav;
        private Wisej.Web.Label lblNavDashboard;
        private Wisej.Web.Label lblNavCustomers;
        private Wisej.Web.Label lblNavContacts;
        private Wisej.Web.Label lblNavSettings;
        private Wisej.Web.Panel pnlMain;
        private Wisej.Web.Panel pnlTop;
        private Wisej.Web.Label lblWelcome;
        private Wisej.Web.ComboBox cboLanguage;
        private Wisej.Web.Panel pnlEditorHost;
        private Wisej.Web.Label lblStatus;
    }
}
