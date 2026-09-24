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
                Application.CultureChanged -= this.Application_CultureChanged;

                if (components != null)
                    components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Wisej.NET Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pnlAppBar = new Wisej.Web.Panel();
            this.lblAppTitle = new Wisej.Web.Label();
            this.pnlWindowGlyphs = new Wisej.Web.Panel();
            this.lblGlyphMinimize = new Wisej.Web.Label();
            this.lblGlyphMaximize = new Wisej.Web.Label();
            this.lblGlyphClose = new Wisej.Web.Label();
            this.pnlToolbar = new Wisej.Web.Panel();
            this.lblBrand = new Wisej.Web.Label();
            this.pnlPicker = new Wisej.Web.Panel();
            this.lblLanguage = new Wisej.Web.Label();
            this.cboLanguage = new Wisej.Web.ComboBox();
            this.pnlRail = new Wisej.Web.Panel();
            this.lblUntranslated = new Wisej.Web.Label();
            this.lblCultureLine = new Wisej.Web.Label();
            this.lblCultureHeading = new Wisej.Web.Label();
            this.lblNavTickets = new Wisej.Web.Label();
            this.lblNavCustomers = new Wisej.Web.Label();
            this.lblRailBrand = new Wisej.Web.Label();
            this.pnlMain = new Wisej.Web.Panel();
            this.pnlKpis = new Wisej.Web.Panel();
            this.pnlKpiAmount = new Wisej.Web.Panel();
            this.lblAmountCaption = new Wisej.Web.Label();
            this.lblAmountValue = new Wisej.Web.Label();
            this.pnlKpiCount = new Wisej.Web.Panel();
            this.lblCountCaption = new Wisej.Web.Label();
            this.lblCountValue = new Wisej.Web.Label();
            this.pnlKpiDate = new Wisej.Web.Panel();
            this.lblDateCaption = new Wisej.Web.Label();
            this.lblDateValue = new Wisej.Web.Label();
            this.pnlPreviewHeader = new Wisej.Web.Panel();
            this.lblPreviewTitle = new Wisej.Web.Label();
            this.lblChipOpen = new Wisej.Web.Label();
            this.lblChipClosed = new Wisej.Web.Label();
            this.pnlEditorHost = new Wisej.Web.Panel();
            this.lblWelcome = new Wisej.Web.Label();
            this.lblTitle = new Wisej.Web.Label();
            this.SuspendLayout();
            //
            // ── the application bar ────────────────────────────────────────────────────────
            //
            this.lblAppTitle.Name = "lblAppTitle";
            this.lblAppTitle.AutoSize = false;
            this.lblAppTitle.Dock = Wisej.Web.DockStyle.Fill;
            this.lblAppTitle.Padding = new Wisej.Web.Padding(16, 0, 0, 0);
            this.lblAppTitle.ForeColor = Color.White;
            this.lblAppTitle.Font = Desk.Px(14.5F, FontStyle.Bold);
            this.lblAppTitle.TextAlign = ContentAlignment.MiddleLeft;

            SetGlyph(this.lblGlyphMinimize, "lblGlyphMinimize", "—");
            SetGlyph(this.lblGlyphMaximize, "lblGlyphMaximize", "□");
            SetGlyph(this.lblGlyphClose, "lblGlyphClose", "✕");

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
            // ── the toolbar: product name on the left, language picker on the right ────────
            //
            this.lblBrand.Name = "lblBrand";
            this.lblBrand.AutoSize = false;
            this.lblBrand.Dock = Wisej.Web.DockStyle.Fill;
            this.lblBrand.Font = Desk.Px(14.5F, FontStyle.Bold);
            this.lblBrand.ForeColor = Desk.Body;
            this.lblBrand.TextAlign = ContentAlignment.MiddleLeft;

            this.lblLanguage.Name = "lblLanguage";
            this.lblLanguage.AutoSize = false;
            this.lblLanguage.Dock = Wisej.Web.DockStyle.Left;
            this.lblLanguage.Size = new Size(88, 50);
            this.lblLanguage.Font = Desk.Px(12.5F, FontStyle.Bold);
            this.lblLanguage.ForeColor = Color.FromArgb(0x6B, 0x7D, 0x90);
            this.lblLanguage.TextAlign = ContentAlignment.MiddleRight;

            this.cboLanguage.Name = "cboLanguage";
            this.cboLanguage.Dock = Wisej.Web.DockStyle.Right;
            this.cboLanguage.Size = new Size(210, 32);
            this.cboLanguage.Margin = new Wisej.Web.Padding(10, 9, 0, 9);
            this.cboLanguage.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboLanguage.Font = Desk.Px(13);
            this.cboLanguage.ForeColor = Desk.Body;
            this.cboLanguage.CssStyle = "border:1px solid #cdd9e6;border-radius:7px";
            this.cboLanguage.TabIndex = 0;
            this.cboLanguage.SelectedIndexChanged += this.cboLanguage_SelectedIndexChanged;

            this.pnlPicker.Name = "pnlPicker";
            this.pnlPicker.Dock = Wisej.Web.DockStyle.Right;
            this.pnlPicker.Size = new Size(308, 50);
            this.pnlPicker.BackColor = Color.FromArgb(0xFB, 0xFC, 0xFE);
            this.pnlPicker.Padding = new Wisej.Web.Padding(0, 9, 0, 9);
            this.pnlPicker.Controls.Add(this.cboLanguage);
            this.pnlPicker.Controls.Add(this.lblLanguage);

            this.pnlToolbar.Name = "pnlToolbar";
            this.pnlToolbar.Dock = Wisej.Web.DockStyle.Top;
            this.pnlToolbar.Size = new Size(1400, 50);
            this.pnlToolbar.BackColor = Color.FromArgb(0xFB, 0xFC, 0xFE);
            this.pnlToolbar.Padding = new Wisej.Web.Padding(16, 0, 16, 0);
            this.pnlToolbar.CssStyle = "border-bottom:1px solid #e3e9f0";
            this.pnlToolbar.Controls.Add(this.lblBrand);
            this.pnlToolbar.Controls.Add(this.pnlPicker);
            //
            // ── the rail ───────────────────────────────────────────────────────────────────
            //
            this.lblRailBrand.Name = "lblRailBrand";
            this.lblRailBrand.AutoSize = false;
            this.lblRailBrand.Dock = Wisej.Web.DockStyle.Top;
            this.lblRailBrand.Size = new Size(182, 28);
            this.lblRailBrand.Font = Desk.Px(15, FontStyle.Bold);
            this.lblRailBrand.ForeColor = Color.FromArgb(0x12, 0x28, 0x3E);
            this.lblRailBrand.TextAlign = ContentAlignment.MiddleLeft;

            SetRailEntry(this.lblNavCustomers, "lblNavCustomers", true);
            SetRailEntry(this.lblNavTickets, "lblNavTickets", false);

            this.lblCultureHeading.Name = "lblCultureHeading";
            this.lblCultureHeading.AutoSize = false;
            this.lblCultureHeading.Dock = Wisej.Web.DockStyle.Top;
            this.lblCultureHeading.Size = new Size(182, 30);
            this.lblCultureHeading.Padding = new Wisej.Web.Padding(0, 14, 0, 0);
            this.lblCultureHeading.Font = Desk.Px(11.5F, FontStyle.Bold);
            this.lblCultureHeading.ForeColor = Color.FromArgb(0x7D, 0x8F, 0xA3);
            this.lblCultureHeading.CssStyle = "text-transform:uppercase;letter-spacing:.05em";
            this.lblCultureHeading.TextAlign = ContentAlignment.BottomLeft;

            this.lblCultureLine.Name = "lblCultureLine";
            this.lblCultureLine.AutoSize = false;
            this.lblCultureLine.Dock = Wisej.Web.DockStyle.Top;
            this.lblCultureLine.Size = new Size(182, 22);
            this.lblCultureLine.Font = Desk.Mono(12.5F);
            this.lblCultureLine.ForeColor = Color.FromArgb(0x41, 0x56, 0x6C);
            this.lblCultureLine.TextAlign = ContentAlignment.MiddleLeft;

            // The translation report. It answers one question - how many keys still come back with
            // the neutral value in this language - and it is the number the round is measured by.
            this.lblUntranslated.Name = "lblUntranslated";
            this.lblUntranslated.AutoSize = false;
            this.lblUntranslated.Dock = Wisej.Web.DockStyle.Top;
            this.lblUntranslated.Size = new Size(182, 22);
            this.lblUntranslated.Font = Desk.Mono(12.5F);
            this.lblUntranslated.TextAlign = ContentAlignment.MiddleLeft;

            this.pnlRail.Name = "pnlRail";
            this.pnlRail.Dock = Wisej.Web.DockStyle.Left;
            this.pnlRail.Size = new Size(210, 700);
            this.pnlRail.MinimumSize = new Size(210, 0);
            this.pnlRail.BackColor = Color.FromArgb(0xF4, 0xF7, 0xFB);
            this.pnlRail.Padding = new Wisej.Web.Padding(14, 16, 14, 16);
            this.pnlRail.CssStyle = "border-inline-end:1px solid #e3e9f0";
            this.pnlRail.Controls.Add(this.lblUntranslated);
            this.pnlRail.Controls.Add(this.lblCultureLine);
            this.pnlRail.Controls.Add(this.lblCultureHeading);
            this.pnlRail.Controls.Add(this.lblNavTickets);
            this.pnlRail.Controls.Add(this.lblNavCustomers);
            this.pnlRail.Controls.Add(this.lblRailBrand);
            //
            // ── the main area ──────────────────────────────────────────────────────────────
            //
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.AutoSize = false;
            this.lblTitle.Dock = Wisej.Web.DockStyle.Top;
            this.lblTitle.Size = new Size(1148, 30);
            this.lblTitle.Font = Desk.Px(22, FontStyle.Bold);
            this.lblTitle.ForeColor = Color.FromArgb(0x12, 0x28, 0x3E);
            this.lblTitle.TextAlign = ContentAlignment.MiddleLeft;

            this.lblWelcome.Name = "lblWelcome";
            this.lblWelcome.AutoSize = false;
            this.lblWelcome.Dock = Wisej.Web.DockStyle.Top;
            this.lblWelcome.Size = new Size(1148, 26);
            this.lblWelcome.Font = Desk.Px(14.5F);
            this.lblWelcome.ForeColor = Desk.Muted;
            this.lblWelcome.TextAlign = ContentAlignment.MiddleLeft;

            this.pnlEditorHost.Name = "pnlEditorHost";
            this.pnlEditorHost.Dock = Wisej.Web.DockStyle.Top;
            this.pnlEditorHost.Size = new Size(1148, 158);
            this.pnlEditorHost.BackColor = Color.White;
            this.pnlEditorHost.Padding = new Wisej.Web.Padding(0, 16, 0, 0);
            //
            // the culture preview heading with its two status chips
            //
            // Wide enough for "ANTEPRIMA CULTURA" and "معاينة الثقافة" on one line: an AutoSize
            // label docked Left wraps instead of growing, and a wrapped heading pushes the tiles
            // out of place.
            this.lblPreviewTitle.Name = "lblPreviewTitle";
            this.lblPreviewTitle.AutoSize = false;
            this.lblPreviewTitle.Dock = Wisej.Web.DockStyle.Left;
            this.lblPreviewTitle.Size = new Size(190, 30);
            this.lblPreviewTitle.Font = Desk.Px(12.5F, FontStyle.Bold);
            this.lblPreviewTitle.ForeColor = Color.FromArgb(0x7D, 0x8F, 0xA3);
            this.lblPreviewTitle.Padding = new Wisej.Web.Padding(0, 0, 10, 0);
            this.lblPreviewTitle.CssStyle = "text-transform:uppercase;letter-spacing:.05em";
            this.lblPreviewTitle.TextAlign = ContentAlignment.MiddleLeft;

            SetChip(this.lblChipOpen, "lblChipOpen", Desk.Accent, Color.FromArgb(0xE6, 0xF0, 0xFF), "#bcd8ff");
            SetChip(this.lblChipClosed, "lblChipClosed", Desk.Purple, Color.FromArgb(0xF1, 0xEC, 0xFF), "#d6c8ff");

            this.pnlPreviewHeader.Name = "pnlPreviewHeader";
            this.pnlPreviewHeader.Dock = Wisej.Web.DockStyle.Top;
            this.pnlPreviewHeader.Size = new Size(1148, 38);
            this.pnlPreviewHeader.BackColor = Color.White;
            this.pnlPreviewHeader.Padding = new Wisej.Web.Padding(0, 8, 0, 0);
            this.pnlPreviewHeader.Controls.Add(this.lblChipClosed);
            this.pnlPreviewHeader.Controls.Add(this.lblChipOpen);
            this.pnlPreviewHeader.Controls.Add(this.lblPreviewTitle);
            //
            // the three value tiles
            //
            SetKpi(this.pnlKpiDate, "pnlKpiDate", this.lblDateCaption, "lblDateCaption", this.lblDateValue, "lblDateValue");
            SetKpi(this.pnlKpiCount, "pnlKpiCount", this.lblCountCaption, "lblCountCaption", this.lblCountValue, "lblCountValue");
            SetKpi(this.pnlKpiAmount, "pnlKpiAmount", this.lblAmountCaption, "lblAmountCaption", this.lblAmountValue, "lblAmountValue");

            this.pnlKpis.Name = "pnlKpis";
            this.pnlKpis.Dock = Wisej.Web.DockStyle.Top;
            this.pnlKpis.Size = new Size(1148, 80);
            this.pnlKpis.BackColor = Color.White;
            this.pnlKpis.Padding = new Wisej.Web.Padding(0, 9, 0, 0);
            this.pnlKpis.Controls.Add(this.pnlKpiAmount);
            this.pnlKpis.Controls.Add(this.pnlKpiCount);
            this.pnlKpis.Controls.Add(this.pnlKpiDate);

            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlMain.BackColor = Color.White;
            this.pnlMain.Padding = new Wisej.Web.Padding(22, 18, 22, 22);
            this.pnlMain.Controls.Add(this.pnlKpis);
            this.pnlMain.Controls.Add(this.pnlPreviewHeader);
            this.pnlMain.Controls.Add(this.pnlEditorHost);
            this.pnlMain.Controls.Add(this.lblWelcome);
            this.pnlMain.Controls.Add(this.lblTitle);
            //
            // DashboardPage
            //
            this.Name = "DashboardPage";
            this.BackColor = Color.White;
            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.pnlRail);
            this.Controls.Add(this.pnlToolbar);
            this.Controls.Add(this.pnlAppBar);
            this.ResumeLayout(false);
        }

        private static void SetGlyph(Wisej.Web.Label label, string name, string glyph)
        {
            label.Name = name;
            label.AutoSize = false;
            label.Dock = Wisej.Web.DockStyle.Right;
            label.Size = new Size(36, 38);
            label.Text = glyph;   // a shape, not a word
            label.ForeColor = Color.White;
            label.Font = Desk.Px(13);
            label.TextAlign = ContentAlignment.MiddleCenter;
        }

        private static void SetRailEntry(Wisej.Web.Label label, string name, bool selected)
        {
            label.Name = name;
            label.AutoSize = false;
            label.Dock = Wisej.Web.DockStyle.Top;
            label.Size = new Size(182, 36);
            label.Margin = new Wisej.Web.Padding(0, 8, 0, 0);
            label.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            label.Font = Desk.Px(13.5F, FontStyle.Bold);
            label.ForeColor = selected ? Color.White : Color.FromArgb(0x41, 0x56, 0x6C);
            label.BackColor = selected ? Desk.Accent : Color.White;
            label.CssStyle = selected ? "border-radius:7px" : "border:1px solid #d8e2ec;border-radius:7px";
            label.TextAlign = ContentAlignment.MiddleLeft;
        }

        private static void SetChip(Wisej.Web.Label label, string name, Color ink, Color back, string border)
        {
            label.Name = name;
            label.AutoSize = true;
            label.Dock = Wisej.Web.DockStyle.Left;
            label.Margin = new Wisej.Web.Padding(0, 0, 10, 0);
            label.Font = Desk.Px(12, FontStyle.Bold);
            label.ForeColor = ink;
            label.BackColor = back;
            label.Padding = new Wisej.Web.Padding(11, 3, 11, 3);
            label.CssStyle = "border:1px solid " + border + ";border-radius:999px";
            label.TextAlign = ContentAlignment.MiddleCenter;
        }

        /// <summary>One value tile: a caption and the value the session's culture wrote.</summary>
        private static void SetKpi(Wisej.Web.Panel tile, string tileName,
                                   Wisej.Web.Label caption, string captionName,
                                   Wisej.Web.Label value, string valueName)
        {
            caption.Name = captionName;
            caption.AutoSize = false;
            caption.Dock = Wisej.Web.DockStyle.Top;
            caption.Size = new Size(340, 20);
            caption.Font = Desk.Px(12, FontStyle.Bold);
            caption.ForeColor = Color.FromArgb(0x6B, 0x7D, 0x90);
            caption.TextAlign = ContentAlignment.MiddleLeft;

            value.Name = valueName;
            value.AutoSize = false;
            value.Dock = Wisej.Web.DockStyle.Top;
            value.Size = new Size(340, 30);
            value.Font = Desk.Mono(20, FontStyle.Bold);
            value.ForeColor = Color.FromArgb(0x12, 0x28, 0x3E);
            value.TextAlign = ContentAlignment.MiddleLeft;

            tile.Name = tileName;
            tile.Dock = Wisej.Web.DockStyle.Left;
            tile.Size = new Size(372, 71);
            tile.BackColor = Color.FromArgb(0xF7, 0xFA, 0xFD);
            tile.Margin = new Wisej.Web.Padding(0, 0, 12, 0);
            tile.Padding = new Wisej.Web.Padding(14, 11, 14, 11);
            tile.CssStyle = "border:1px solid #e1e9f2;border-radius:9px";
            tile.Controls.Add(value);
            tile.Controls.Add(caption);
        }

        #endregion

        private Wisej.Web.Panel pnlAppBar;
        private Wisej.Web.Label lblAppTitle;
        private Wisej.Web.Panel pnlWindowGlyphs;
        private Wisej.Web.Label lblGlyphMinimize;
        private Wisej.Web.Label lblGlyphMaximize;
        private Wisej.Web.Label lblGlyphClose;
        private Wisej.Web.Panel pnlToolbar;
        private Wisej.Web.Label lblBrand;
        private Wisej.Web.Panel pnlPicker;
        private Wisej.Web.Label lblLanguage;
        private Wisej.Web.ComboBox cboLanguage;
        private Wisej.Web.Panel pnlRail;
        private Wisej.Web.Label lblRailBrand;
        private Wisej.Web.Label lblNavCustomers;
        private Wisej.Web.Label lblNavTickets;
        private Wisej.Web.Label lblCultureHeading;
        private Wisej.Web.Label lblCultureLine;
        private Wisej.Web.Label lblUntranslated;
        private Wisej.Web.Panel pnlMain;
        private Wisej.Web.Label lblTitle;
        private Wisej.Web.Label lblWelcome;
        private Wisej.Web.Panel pnlEditorHost;
        private Wisej.Web.Panel pnlPreviewHeader;
        private Wisej.Web.Label lblPreviewTitle;
        private Wisej.Web.Label lblChipOpen;
        private Wisej.Web.Label lblChipClosed;
        private Wisej.Web.Panel pnlKpis;
        private Wisej.Web.Panel pnlKpiDate;
        private Wisej.Web.Label lblDateCaption;
        private Wisej.Web.Label lblDateValue;
        private Wisej.Web.Panel pnlKpiCount;
        private Wisej.Web.Label lblCountCaption;
        private Wisej.Web.Label lblCountValue;
        private Wisej.Web.Panel pnlKpiAmount;
        private Wisej.Web.Label lblAmountCaption;
        private Wisej.Web.Label lblAmountValue;
    }
}
