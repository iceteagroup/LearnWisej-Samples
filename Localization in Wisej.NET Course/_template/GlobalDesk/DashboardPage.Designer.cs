using System.Drawing;
using Wisej.Web;

namespace GlobalDesk
{
    partial class DashboardPage
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
        // Nothing in this file carries a user-visible English sentence. Every caption is assigned
        // in ApplyTextResources, from a resource key. A literal here is a string no translator
        // will ever see.
        //
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pnlAppBar = new Wisej.Web.Panel();
            this.lblAppTitle = new Wisej.Web.Label();
            this.pnlWindowGlyphs = new Wisej.Web.Panel();
            this.lblGlyphClose = new Wisej.Web.Label();
            this.lblGlyphMaximize = new Wisej.Web.Label();
            this.lblGlyphMinimize = new Wisej.Web.Label();
            this.lblStatus = new Wisej.Web.Label();
            this.pnlBody = new Wisej.Web.Panel();
            this.pnlPreview = new Wisej.Web.Panel();
            this.pnlPreviewHeader = new Wisej.Web.Panel();
            this.lblPreviewTitle = new Wisej.Web.Label();
            this.lblCulture = new Wisej.Web.Label();
            this.pnlRowAmount = new Wisej.Web.Panel();
            this.lblAmountValue = new Wisej.Web.Label();
            this.lblAmountCaption = new Wisej.Web.Label();
            this.pnlRowCount = new Wisej.Web.Panel();
            this.lblCountValue = new Wisej.Web.Label();
            this.lblCountCaption = new Wisej.Web.Label();
            this.pnlRowDate = new Wisej.Web.Panel();
            this.lblDateValue = new Wisej.Web.Label();
            this.lblDateCaption = new Wisej.Web.Label();
            this.pnlNav = new Wisej.Web.Panel();
            this.btnCustomers = new Wisej.Web.Button();
            this.lblWelcome = new Wisej.Web.Label();
            this.SuspendLayout();
            //
            // ── the application bar: title on the left, window glyphs on the right ──────────
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
            // Docked Right, so the control added last claims the right edge: close is added last
            // and sits furthest right, exactly as the window chrome in the walkthrough does.
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
            // ── the heading ────────────────────────────────────────────────────────────────
            //
            this.lblWelcome.Name = "lblWelcome";
            this.lblWelcome.AutoSize = false;
            this.lblWelcome.Dock = Wisej.Web.DockStyle.Top;
            this.lblWelcome.Size = new Size(840, 40);
            this.lblWelcome.Font = Desk.Px(22, FontStyle.Bold);
            this.lblWelcome.ForeColor = Desk.Ink;
            this.lblWelcome.TextAlign = ContentAlignment.MiddleLeft;
            //
            // ── the navigation row ─────────────────────────────────────────────────────────
            //
            // AutoSize, because a caption is a different length in every language and a fixed
            // width is a clipped word waiting to happen.
            this.btnCustomers.Name = "btnCustomers";
            this.btnCustomers.AutoSize = true;
            this.btnCustomers.Location = new Point(0, 10);
            this.btnCustomers.MinimumSize = new Size(130, 40);
            this.btnCustomers.BackColor = Desk.Accent;
            this.btnCustomers.ForeColor = Color.White;
            this.btnCustomers.Font = Desk.Px(15, FontStyle.Bold);
            this.btnCustomers.Padding = new Wisej.Web.Padding(22, 0, 22, 0);
            this.btnCustomers.CssStyle = "border-radius:7px;border:none";
            this.btnCustomers.TabIndex = 0;
            // No Click handler yet: the customer editor this button opens is built in Module 2.

            this.pnlNav.Name = "pnlNav";
            this.pnlNav.Dock = Wisej.Web.DockStyle.Top;
            this.pnlNav.Size = new Size(840, 70);
            this.pnlNav.Controls.Add(this.btnCustomers);
            //
            // ── the culture preview card ───────────────────────────────────────────────────
            //
            this.lblPreviewTitle.Name = "lblPreviewTitle";
            this.lblPreviewTitle.AutoSize = false;
            this.lblPreviewTitle.Dock = Wisej.Web.DockStyle.Fill;
            this.lblPreviewTitle.Font = Desk.Px(14, FontStyle.Bold);
            this.lblPreviewTitle.ForeColor = Desk.Ink;
            this.lblPreviewTitle.TextAlign = ContentAlignment.MiddleLeft;

            // The one thing on this screen that is neither text nor a value: which culture the
            // session is actually on. It is the answer to "why does my screen look like that?".
            this.lblCulture.Name = "lblCulture";
            this.lblCulture.AutoSize = true;
            this.lblCulture.Dock = Wisej.Web.DockStyle.Right;
            this.lblCulture.Font = Desk.Mono(11.5F, FontStyle.Bold);
            this.lblCulture.ForeColor = Desk.GoodInk;
            this.lblCulture.BackColor = Desk.GoodBack;
            this.lblCulture.Padding = new Wisej.Web.Padding(9, 2, 9, 2);
            this.lblCulture.CssStyle = "border:1px solid #a6e0c3;border-radius:999px";
            this.lblCulture.TextAlign = ContentAlignment.MiddleCenter;

            this.pnlPreviewHeader.Name = "pnlPreviewHeader";
            this.pnlPreviewHeader.Dock = Wisej.Web.DockStyle.Top;
            this.pnlPreviewHeader.Size = new Size(838, 40);
            this.pnlPreviewHeader.BackColor = Desk.CardHead;
            this.pnlPreviewHeader.Padding = new Wisej.Web.Padding(15, 5, 15, 5);
            this.pnlPreviewHeader.CssStyle = "border-bottom:1px solid #e0e7ef";
            this.pnlPreviewHeader.Controls.Add(this.lblPreviewTitle);
            this.pnlPreviewHeader.Controls.Add(this.lblCulture);

            SetRow(this.pnlRowDate, "pnlRowDate", this.lblDateCaption, "lblDateCaption", this.lblDateValue, "lblDateValue");
            SetRow(this.pnlRowCount, "pnlRowCount", this.lblCountCaption, "lblCountCaption", this.lblCountValue, "lblCountValue");
            SetRow(this.pnlRowAmount, "pnlRowAmount", this.lblAmountCaption, "lblAmountCaption", this.lblAmountValue, "lblAmountValue");

            this.pnlPreview.Name = "pnlPreview";
            this.pnlPreview.Dock = Wisej.Web.DockStyle.Top;
            this.pnlPreview.Size = new Size(840, 172);
            this.pnlPreview.BackColor = Color.White;
            this.pnlPreview.CssStyle = "border:1px solid #e0e7ef;border-radius:10px;overflow:hidden";
            this.pnlPreview.Controls.Add(this.pnlRowAmount);
            this.pnlPreview.Controls.Add(this.pnlRowCount);
            this.pnlPreview.Controls.Add(this.pnlRowDate);
            this.pnlPreview.Controls.Add(this.pnlPreviewHeader);
            //
            // pnlBody
            //
            this.pnlBody.Name = "pnlBody";
            this.pnlBody.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlBody.BackColor = Color.White;
            this.pnlBody.Padding = new Wisej.Web.Padding(30, 24, 30, 18);
            this.pnlBody.Controls.Add(this.pnlPreview);
            this.pnlBody.Controls.Add(this.pnlNav);
            this.pnlBody.Controls.Add(this.lblWelcome);
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
            // DashboardPage
            //
            this.Name = "DashboardPage";
            this.BackColor = Color.White;
            this.Controls.Add(this.pnlBody);
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
            label.Size = new Size(36, 42);
            // A glyph is a shape, not a word. It never goes through a resource.
            label.Text = glyph;
            label.ForeColor = Color.White;
            label.Font = Desk.Px(13);
            label.TextAlign = ContentAlignment.MiddleCenter;
        }

        /// <summary>One caption + value row of the culture preview card.</summary>
        private static void SetRow(Wisej.Web.Panel row, string rowName,
                                   Wisej.Web.Label caption, string captionName,
                                   Wisej.Web.Label value, string valueName)
        {
            caption.Name = captionName;
            caption.AutoSize = false;
            caption.Dock = Wisej.Web.DockStyle.Left;
            caption.Size = new Size(210, 44);
            caption.Font = Desk.Px(14.5F);
            caption.ForeColor = Desk.Caption;
            caption.TextAlign = ContentAlignment.MiddleLeft;

            // The values are monospaced on purpose: the walkthrough compares separators and
            // digit grouping between cultures, and a proportional font hides the difference.
            value.Name = valueName;
            value.AutoSize = false;
            value.Dock = Wisej.Web.DockStyle.Fill;
            value.Font = Desk.Mono(15.5F, FontStyle.Bold);
            value.ForeColor = Desk.Body;
            value.TextAlign = ContentAlignment.MiddleLeft;

            row.Name = rowName;
            row.Dock = Wisej.Web.DockStyle.Top;
            row.Size = new Size(838, 44);
            row.BackColor = Color.White;
            row.Padding = new Wisej.Web.Padding(15, 0, 15, 0);
            row.CssStyle = "border-bottom:1px solid #eef2f7";
            row.Controls.Add(value);
            row.Controls.Add(caption);
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
        private Wisej.Web.Panel pnlNav;
        private Wisej.Web.Button btnCustomers;
        private Wisej.Web.Panel pnlPreview;
        private Wisej.Web.Panel pnlPreviewHeader;
        private Wisej.Web.Label lblPreviewTitle;
        private Wisej.Web.Label lblCulture;
        private Wisej.Web.Panel pnlRowDate;
        private Wisej.Web.Label lblDateCaption;
        private Wisej.Web.Label lblDateValue;
        private Wisej.Web.Panel pnlRowCount;
        private Wisej.Web.Label lblCountCaption;
        private Wisej.Web.Label lblCountValue;
        private Wisej.Web.Panel pnlRowAmount;
        private Wisej.Web.Label lblAmountCaption;
        private Wisej.Web.Label lblAmountValue;
        private Wisej.Web.Label lblStatus;
    }
}
