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

        //
        // The capstone screen: the navigation rail with the language picker at its foot, the three
        // value tiles, the ticket grid and the customer record's actions.
        //
        // Not one user-visible English sentence in this file. Every caption is assigned from
        // LocalizationService, which is the only thing in the solution that touches a
        // ResourceManager.
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
            this.pnlRail = new Wisej.Web.Panel();
            this.pnlPicker = new Wisej.Web.Panel();
            this.cboLanguage = new Wisej.Web.ComboBox();
            this.lblNavSettings = new Wisej.Web.Label();
            this.lblNavTickets = new Wisej.Web.Label();
            this.lblNavCustomers = new Wisej.Web.Label();
            this.lblNavDashboard = new Wisej.Web.Label();
            this.pnlMain = new Wisej.Web.Panel();
            this.pnlEditorHost = new Wisej.Web.Panel();
            this.pnlGridHost = new Wisej.Web.Panel();
            this.gridTickets = new Wisej.Web.DataGridView();
            this.colReference = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colCustomer = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colDue = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colAmount = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.pnlTiles = new Wisej.Web.Panel();
            this.pnlTileResolved = new Wisej.Web.Panel();
            this.lblResolvedCaption = new Wisej.Web.Label();
            this.lblResolvedValue = new Wisej.Web.Label();
            this.pnlTileAmount = new Wisej.Web.Panel();
            this.lblAmountCaption = new Wisej.Web.Label();
            this.lblAmountValue = new Wisej.Web.Label();
            this.pnlTileDue = new Wisej.Web.Panel();
            this.lblDueCaption = new Wisej.Web.Label();
            this.lblDueValue = new Wisej.Web.Label();
            this.lblHeading = new Wisej.Web.Label();
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
            // ── the navigation rail, with the language picker at its foot ─────────────────
            //
            SetNavEntry(this.lblNavSettings, "lblNavSettings", false);
            SetNavEntry(this.lblNavTickets, "lblNavTickets", false);
            SetNavEntry(this.lblNavCustomers, "lblNavCustomers", false);
            SetNavEntry(this.lblNavDashboard, "lblNavDashboard", true);

            this.cboLanguage.Name = "cboLanguage";
            this.cboLanguage.Dock = Wisej.Web.DockStyle.Fill;
            this.cboLanguage.Size = new Size(186, 34);
            this.cboLanguage.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboLanguage.Font = Desk.Px(13);
            this.cboLanguage.ForeColor = Desk.Body;
            this.cboLanguage.CssStyle = "border:1px solid #cdd9e6;border-radius:8px";
            this.cboLanguage.TabIndex = 0;
            this.cboLanguage.SelectedIndexChanged += this.cboLanguage_SelectedIndexChanged;

            this.pnlPicker.Name = "pnlPicker";
            this.pnlPicker.Dock = Wisej.Web.DockStyle.Bottom;
            this.pnlPicker.Size = new Size(186, 34);
            this.pnlPicker.BackColor = Color.Transparent;
            this.pnlPicker.Controls.Add(this.cboLanguage);

            this.pnlRail.Name = "pnlRail";
            this.pnlRail.Dock = Wisej.Web.DockStyle.Left;
            this.pnlRail.Size = new Size(210, 760);
            this.pnlRail.MinimumSize = new Size(210, 0);
            this.pnlRail.BackColor = Desk.Rail;
            this.pnlRail.Padding = new Wisej.Web.Padding(12, 14, 12, 14);
            this.pnlRail.CssStyle = "border-inline-end:1px solid #e4eaf1";
            this.pnlRail.Controls.Add(this.pnlPicker);
            this.pnlRail.Controls.Add(this.lblNavSettings);
            this.pnlRail.Controls.Add(this.lblNavTickets);
            this.pnlRail.Controls.Add(this.lblNavCustomers);
            this.pnlRail.Controls.Add(this.lblNavDashboard);
            //
            // ── the heading and the three value tiles ─────────────────────────────────────
            //
            this.lblHeading.Name = "lblHeading";
            this.lblHeading.AutoSize = false;
            this.lblHeading.Dock = Wisej.Web.DockStyle.Top;
            this.lblHeading.Size = new Size(1148, 30);
            this.lblHeading.Font = Desk.Px(19, FontStyle.Bold);
            this.lblHeading.ForeColor = Desk.Ink;
            this.lblHeading.TextAlign = ContentAlignment.MiddleLeft;

            SetTile(this.pnlTileDue, "pnlTileDue", this.lblDueCaption, "lblDueCaption",
                    this.lblDueValue, "lblDueValue", Desk.Accent, "#1565d8");
            SetTile(this.pnlTileAmount, "pnlTileAmount", this.lblAmountCaption, "lblAmountCaption",
                    this.lblAmountValue, "lblAmountValue", Desk.Amber, "#e8a13c");
            SetTile(this.pnlTileResolved, "pnlTileResolved", this.lblResolvedCaption, "lblResolvedCaption",
                    this.lblResolvedValue, "lblResolvedValue", Desk.Teal, "#1f9d6b");

            this.pnlTiles.Name = "pnlTiles";
            this.pnlTiles.Dock = Wisej.Web.DockStyle.Top;
            this.pnlTiles.Size = new Size(1148, 96);
            this.pnlTiles.BackColor = Color.FromArgb(0xF7, 0xF9, 0xFB);
            this.pnlTiles.Padding = new Wisej.Web.Padding(0, 14, 0, 0);
            this.pnlTiles.Controls.Add(this.pnlTileResolved);
            this.pnlTiles.Controls.Add(this.pnlTileAmount);
            this.pnlTiles.Controls.Add(this.pnlTileDue);
            //
            // ── the ticket grid ───────────────────────────────────────────────────────────
            //
            this.colReference.Name = "colReference";
            this.colReference.Width = 110;
            this.colReference.DefaultCellStyle.Font = Desk.Mono(12.5F, FontStyle.Bold);
            this.colReference.DefaultCellStyle.ForeColor = Desk.Accent;
            // An identifier: it must read the same in every language, including a mirrored one.
            this.colReference.DefaultCellStyle.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleLeft;

            this.colCustomer.Name = "colCustomer";
            this.colCustomer.AutoSizeMode = Wisej.Web.DataGridViewAutoSizeColumnMode.Fill;

            this.colDue.Name = "colDue";
            this.colDue.Width = 150;
            this.colDue.DefaultCellStyle.Font = Desk.Mono(12.5F);

            this.colAmount.Name = "colAmount";
            this.colAmount.Width = 140;
            this.colAmount.DefaultCellStyle.Font = Desk.Mono(12.5F);

            this.colStatus.Name = "colStatus";
            this.colStatus.Width = 160;

            this.gridTickets.Name = "gridTickets";
            this.gridTickets.Dock = Wisej.Web.DockStyle.Fill;
            this.gridTickets.AllowUserToAddRows = false;
            this.gridTickets.AllowUserToDeleteRows = false;
            this.gridTickets.AllowUserToResizeRows = false;
            this.gridTickets.ReadOnly = true;
            this.gridTickets.RowHeadersVisible = false;
            this.gridTickets.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.gridTickets.ColumnHeadersHeight = 30;
            this.gridTickets.RowTemplate.Height = 38;
            this.gridTickets.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.gridTickets.Font = Desk.Px(12.5F);
            this.gridTickets.ColumnHeadersDefaultCellStyle.BackColor = Desk.CardHead;
            this.gridTickets.ColumnHeadersDefaultCellStyle.ForeColor = Desk.Muted;
            this.gridTickets.ColumnHeadersDefaultCellStyle.Font = Desk.Px(11.5F, FontStyle.Bold);
            this.gridTickets.ColumnHeadersDefaultCellStyle.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleLeft;
            this.gridTickets.Columns.Add(this.colReference);
            this.gridTickets.Columns.Add(this.colCustomer);
            this.gridTickets.Columns.Add(this.colDue);
            this.gridTickets.Columns.Add(this.colAmount);
            this.gridTickets.Columns.Add(this.colStatus);

            this.pnlGridHost.Name = "pnlGridHost";
            this.pnlGridHost.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlGridHost.BackColor = Color.FromArgb(0xF7, 0xF9, 0xFB);
            this.pnlGridHost.Padding = new Wisej.Web.Padding(0, 14, 0, 14);
            this.pnlGridHost.Controls.Add(this.gridTickets);
            //
            // ── the customer record's actions ─────────────────────────────────────────────
            //
            this.pnlEditorHost.Name = "pnlEditorHost";
            this.pnlEditorHost.Dock = Wisej.Web.DockStyle.Bottom;
            this.pnlEditorHost.Size = new Size(1148, 34);
            this.pnlEditorHost.BackColor = Color.FromArgb(0xF7, 0xF9, 0xFB);

            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlMain.BackColor = Color.FromArgb(0xF7, 0xF9, 0xFB);
            this.pnlMain.Padding = new Wisej.Web.Padding(20, 16, 20, 16);
            this.pnlMain.Controls.Add(this.pnlGridHost);
            this.pnlMain.Controls.Add(this.pnlEditorHost);
            this.pnlMain.Controls.Add(this.pnlTiles);
            this.pnlMain.Controls.Add(this.lblHeading);
            //
            // DashboardPage
            //
            this.Name = "DashboardPage";
            this.BackColor = Color.FromArgb(0xF7, 0xF9, 0xFB);
            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.pnlRail);
            this.Controls.Add(this.pnlAppBar);
            this.ResumeLayout(false);
        }

        private static void SetGlyph(Wisej.Web.Label label, string name, string glyph)
        {
            label.Name = name;
            label.AutoSize = false;
            label.Dock = Wisej.Web.DockStyle.Right;
            label.Size = new Size(36, 42);
            label.Text = glyph;   // a shape, not a word
            label.ForeColor = Color.White;
            label.Font = Desk.Px(13);
            label.TextAlign = ContentAlignment.MiddleCenter;
        }

        private static void SetNavEntry(Wisej.Web.Label label, string name, bool selected)
        {
            label.Name = name;
            label.AutoSize = false;
            label.Dock = Wisej.Web.DockStyle.Top;
            label.Size = new Size(186, 36);
            label.Padding = new Wisej.Web.Padding(13, 0, 13, 0);
            label.Font = Desk.Px(13.5F, selected ? FontStyle.Bold : FontStyle.Regular);
            label.ForeColor = selected ? Desk.Accent : Desk.Muted;
            label.BackColor = selected ? Desk.RailActive : Color.Transparent;
            label.CssStyle = "border-radius:8px";
            label.TextAlign = ContentAlignment.MiddleLeft;
        }

        /// <summary>One value tile: a caption and the value the session's culture wrote.</summary>
        private static void SetTile(Wisej.Web.Panel tile, string tileName,
                                    Wisej.Web.Label caption, string captionName,
                                    Wisej.Web.Label value, string valueName,
                                    Color accent, string accentCss)
        {
            caption.Name = captionName;
            caption.AutoSize = false;
            caption.Dock = Wisej.Web.DockStyle.Top;
            caption.Size = new Size(340, 20);
            caption.Font = Desk.Px(11.5F, FontStyle.Bold);
            caption.ForeColor = Desk.Muted;
            caption.CssStyle = "text-transform:uppercase;letter-spacing:.03em";
            caption.TextAlign = ContentAlignment.MiddleLeft;

            value.Name = valueName;
            value.AutoSize = false;
            value.Dock = Wisej.Web.DockStyle.Top;
            value.Size = new Size(340, 34);
            value.Font = Desk.Mono(24, FontStyle.Bold);
            value.ForeColor = accent;
            value.TextAlign = ContentAlignment.MiddleLeft;

            tile.Name = tileName;
            tile.Dock = Wisej.Web.DockStyle.Left;
            tile.Size = new Size(378, 82);
            tile.BackColor = Color.White;
            tile.Margin = new Wisej.Web.Padding(0, 0, 14, 0);
            tile.Padding = new Wisej.Web.Padding(15, 13, 15, 13);
            tile.CssStyle = "border:1px solid #e4eaf1;border-top:3px solid " + accentCss + ";border-radius:11px";
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
        private Wisej.Web.Panel pnlRail;
        private Wisej.Web.Label lblNavDashboard;
        private Wisej.Web.Label lblNavCustomers;
        private Wisej.Web.Label lblNavTickets;
        private Wisej.Web.Label lblNavSettings;
        private Wisej.Web.Panel pnlPicker;
        private Wisej.Web.ComboBox cboLanguage;
        private Wisej.Web.Panel pnlMain;
        private Wisej.Web.Label lblHeading;
        private Wisej.Web.Panel pnlTiles;
        private Wisej.Web.Panel pnlTileDue;
        private Wisej.Web.Label lblDueCaption;
        private Wisej.Web.Label lblDueValue;
        private Wisej.Web.Panel pnlTileAmount;
        private Wisej.Web.Label lblAmountCaption;
        private Wisej.Web.Label lblAmountValue;
        private Wisej.Web.Panel pnlTileResolved;
        private Wisej.Web.Label lblResolvedCaption;
        private Wisej.Web.Label lblResolvedValue;
        private Wisej.Web.Panel pnlGridHost;
        private Wisej.Web.DataGridView gridTickets;
        private Wisej.Web.DataGridViewTextBoxColumn colReference;
        private Wisej.Web.DataGridViewTextBoxColumn colCustomer;
        private Wisej.Web.DataGridViewTextBoxColumn colDue;
        private Wisej.Web.DataGridViewTextBoxColumn colAmount;
        private Wisej.Web.DataGridViewTextBoxColumn colStatus;
        private Wisej.Web.Panel pnlEditorHost;
    }
}
