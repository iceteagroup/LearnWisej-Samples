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
                // Application.CultureChanged outlives the page. Leave this out and every page
                // that was ever open keeps handling culture changes for the rest of the session.
                Application.CultureChanged -= this.Application_CultureChanged;

                if (components != null)
                    components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Wisej.NET Designer generated code

        //
        // Nothing in this file carries a user-visible English sentence. Every caption is assigned
        // in ApplyTextResources, from a resource key, because from this module on that method runs
        // again on every culture change.
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
            this.pnlStatus = new Wisej.Web.Panel();
            this.lblStatus = new Wisej.Web.Label();
            this.lblStatusNote = new Wisej.Web.Label();
            this.pnlBody = new Wisej.Web.Panel();
            this.lblWelcome = new Wisej.Web.Label();
            this.btnCustomers = new Wisej.Web.Button();
            this.lblLanguage = new Wisej.Web.Label();
            this.cboLanguage = new Wisej.Web.ComboBox();
            this.pnlLanguage = new Wisej.Web.Panel();
            this.pnlPreview = new Wisej.Web.Panel();
            this.lblPreviewTitle = new Wisej.Web.Label();
            this.lblDateCaption = new Wisej.Web.Label();
            this.lblDateValue = new Wisej.Web.Label();
            this.lblCountCaption = new Wisej.Web.Label();
            this.lblCountValue = new Wisej.Web.Label();
            this.lblAmountCaption = new Wisej.Web.Label();
            this.lblAmountValue = new Wisej.Web.Label();
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
            // ── the heading and the navigation button ──────────────────────────────────────
            //
            this.lblWelcome.Name = "lblWelcome";
            this.lblWelcome.AutoSize = false;
            this.lblWelcome.Location = new Point(26, 56);
            this.lblWelcome.Size = new Size(620, 36);
            this.lblWelcome.Font = Desk.Px(25, FontStyle.Bold);
            this.lblWelcome.ForeColor = Color.FromArgb(0x12, 0x20, 0x2E);
            this.lblWelcome.TextAlign = ContentAlignment.MiddleLeft;

            this.btnCustomers.Name = "btnCustomers";
            this.btnCustomers.AutoSize = true;
            this.btnCustomers.Location = new Point(26, 108);
            this.btnCustomers.MinimumSize = new Size(120, 36);
            this.btnCustomers.Padding = new Wisej.Web.Padding(20, 0, 20, 0);
            this.btnCustomers.BackColor = Desk.Accent;
            this.btnCustomers.ForeColor = Color.White;
            this.btnCustomers.Font = Desk.Px(14.5F, FontStyle.Bold);
            this.btnCustomers.CssStyle = "border-radius:7px;border:none";
            this.btnCustomers.TabIndex = 1;
            //
            // ── the language picker ────────────────────────────────────────────────────────
            //
            // The picker sits in its own column docked to the right edge, so it stays where the
            // walkthrough shows it at any window width - no anchor arithmetic against a
            // design-time size that the browser is never going to match.
            this.lblLanguage.Name = "lblLanguage";
            this.lblLanguage.AutoSize = false;
            this.lblLanguage.Location = new Point(0, 52);
            this.lblLanguage.Size = new Size(288, 20);
            this.lblLanguage.Font = Desk.Px(13, FontStyle.Bold);
            this.lblLanguage.ForeColor = Desk.Muted;
            this.lblLanguage.TextAlign = ContentAlignment.MiddleLeft;

            this.cboLanguage.Name = "cboLanguage";
            this.cboLanguage.Location = new Point(0, 78);
            this.cboLanguage.Size = new Size(288, 36);
            this.cboLanguage.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboLanguage.Font = Desk.Px(14.5F);
            this.cboLanguage.ForeColor = Desk.Body;
            this.cboLanguage.CssStyle = "border:1.5px solid #c9d5e2;border-radius:6px";
            this.cboLanguage.TabIndex = 0;
            this.cboLanguage.SelectedIndexChanged += this.cboLanguage_SelectedIndexChanged;

            this.pnlLanguage.Name = "pnlLanguage";
            this.pnlLanguage.Dock = Wisej.Web.DockStyle.Right;
            this.pnlLanguage.Size = new Size(314, 818);
            this.pnlLanguage.BackColor = Color.White;
            this.pnlLanguage.Controls.Add(this.lblLanguage);
            this.pnlLanguage.Controls.Add(this.cboLanguage);
            //
            // ── the culture preview panel ──────────────────────────────────────────────────
            //
            this.lblPreviewTitle.Name = "lblPreviewTitle";
            this.lblPreviewTitle.AutoSize = false;
            this.lblPreviewTitle.Dock = Wisej.Web.DockStyle.Top;
            this.lblPreviewTitle.Size = new Size(438, 32);
            this.lblPreviewTitle.Padding = new Wisej.Web.Padding(18, 0, 18, 0);
            this.lblPreviewTitle.Font = Desk.Px(12, FontStyle.Bold);
            this.lblPreviewTitle.ForeColor = Desk.Muted;
            this.lblPreviewTitle.CssStyle = "border-bottom:1px solid #e6ecf3;text-transform:uppercase;letter-spacing:.04em";
            this.lblPreviewTitle.TextAlign = ContentAlignment.MiddleLeft;

            SetPreviewRow(this.lblDateCaption, "lblDateCaption", this.lblDateValue, "lblDateValue", 44);
            SetPreviewRow(this.lblCountCaption, "lblCountCaption", this.lblCountValue, "lblCountValue", 82);
            SetPreviewRow(this.lblAmountCaption, "lblAmountCaption", this.lblAmountValue, "lblAmountValue", 120);

            this.pnlPreview.Name = "pnlPreview";
            this.pnlPreview.Location = new Point(26, 174);
            this.pnlPreview.Size = new Size(440, 176);
            this.pnlPreview.BackColor = Color.FromArgb(0xF8, 0xFA, 0xFC);
            this.pnlPreview.CssStyle = "border:1px solid #dbe3ec;border-radius:8px";
            this.pnlPreview.Controls.Add(this.lblDateCaption);
            this.pnlPreview.Controls.Add(this.lblDateValue);
            this.pnlPreview.Controls.Add(this.lblCountCaption);
            this.pnlPreview.Controls.Add(this.lblCountValue);
            this.pnlPreview.Controls.Add(this.lblAmountCaption);
            this.pnlPreview.Controls.Add(this.lblAmountValue);
            this.pnlPreview.Controls.Add(this.lblPreviewTitle);
            //
            // ── where the designer-localized editor lives ──────────────────────────────────
            //
            // A host panel makes the rebuild three lines instead of a hunt.
            this.pnlEditorHost.Name = "pnlEditorHost";
            this.pnlEditorHost.Location = new Point(500, 174);
            this.pnlEditorHost.Size = new Size(474, 176);
            this.pnlEditorHost.BackColor = Color.White;
            //
            // pnlBody
            //
            this.pnlBody.Name = "pnlBody";
            this.pnlBody.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlBody.BackColor = Color.White;
            this.pnlBody.Controls.Add(this.lblWelcome);
            this.pnlBody.Controls.Add(this.btnCustomers);
            this.pnlBody.Controls.Add(this.pnlLanguage);
            this.pnlBody.Controls.Add(this.pnlPreview);
            this.pnlBody.Controls.Add(this.pnlEditorHost);
            //
            // ── the status strip ───────────────────────────────────────────────────────────
            //
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.AutoSize = false;
            this.lblStatus.Dock = Wisej.Web.DockStyle.Fill;
            this.lblStatus.Font = Desk.Px(13.5F);
            this.lblStatus.ForeColor = Desk.Muted;
            this.lblStatus.TextAlign = ContentAlignment.MiddleLeft;

            this.lblStatusNote.Name = "lblStatusNote";
            this.lblStatusNote.AutoSize = true;
            this.lblStatusNote.Visible = false;
            this.lblStatusNote.Dock = Wisej.Web.DockStyle.Right;
            this.lblStatusNote.Font = Desk.Px(13.5F);
            this.lblStatusNote.ForeColor = Color.FromArgb(0x1D, 0x7A, 0x48);
            this.lblStatusNote.TextAlign = ContentAlignment.MiddleRight;

            this.pnlStatus.Name = "pnlStatus";
            this.pnlStatus.Dock = Wisej.Web.DockStyle.Bottom;
            this.pnlStatus.Size = new Size(1400, 42);
            this.pnlStatus.BackColor = Desk.CardHead;
            this.pnlStatus.Padding = new Wisej.Web.Padding(18, 0, 18, 0);
            this.pnlStatus.CssStyle = "border-top:1px solid #e0e7ef";
            this.pnlStatus.Controls.Add(this.lblStatus);
            this.pnlStatus.Controls.Add(this.lblStatusNote);
            //
            // DashboardPage
            //
            this.Name = "DashboardPage";
            this.BackColor = Color.White;
            this.Controls.Add(this.pnlBody);
            this.Controls.Add(this.pnlStatus);
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

        /// <summary>One caption + value row of the culture preview panel.</summary>
        private static void SetPreviewRow(Wisej.Web.Label caption, string captionName,
                                          Wisej.Web.Label value, string valueName, int y)
        {
            caption.Name = captionName;
            caption.AutoSize = false;
            caption.Location = new Point(18, y);
            caption.Size = new Size(108, 34);
            caption.Font = Desk.Px(13.5F, FontStyle.Bold);
            caption.ForeColor = Desk.Muted;
            caption.TextAlign = ContentAlignment.MiddleLeft;

            // Monospaced on purpose: the panel exists to compare separators and digit grouping
            // between cultures, and a proportional font hides the difference.
            value.Name = valueName;
            value.AutoSize = false;
            value.Location = new Point(126, y);
            value.Size = new Size(296, 34);
            value.Font = Desk.Mono(15);
            value.ForeColor = Desk.Body;
            value.TextAlign = ContentAlignment.MiddleLeft;
        }

        #endregion

        private Wisej.Web.Panel pnlAppBar;
        private Wisej.Web.Label lblAppTitle;
        private Wisej.Web.Panel pnlWindowGlyphs;
        private Wisej.Web.Label lblGlyphMinimize;
        private Wisej.Web.Label lblGlyphMaximize;
        private Wisej.Web.Label lblGlyphClose;
        private Wisej.Web.Panel pnlBody;
        private Wisej.Web.Label lblWelcome;
        private Wisej.Web.Button btnCustomers;
        private Wisej.Web.Label lblLanguage;
        private Wisej.Web.Panel pnlLanguage;
        private Wisej.Web.ComboBox cboLanguage;
        private Wisej.Web.Panel pnlPreview;
        private Wisej.Web.Label lblPreviewTitle;
        private Wisej.Web.Label lblDateCaption;
        private Wisej.Web.Label lblDateValue;
        private Wisej.Web.Label lblCountCaption;
        private Wisej.Web.Label lblCountValue;
        private Wisej.Web.Label lblAmountCaption;
        private Wisej.Web.Label lblAmountValue;
        private Wisej.Web.Panel pnlEditorHost;
        private Wisej.Web.Panel pnlStatus;
        private Wisej.Web.Label lblStatus;
        private Wisej.Web.Label lblStatusNote;
    }
}
