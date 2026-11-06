using System.Drawing;
using Wisej.Web;

namespace FieldOps
{
    partial class WorkOrderPage
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                UnsubscribeDevice();
                if (components != null) components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Wisej.NET Designer generated code

        private void InitializeComponent()
        {
            this.pnlAppBar = new Wisej.Web.Panel();
            this.btnBack = new Wisej.Web.Button();
            this.lblAppTitle = new Wisej.Web.Label();
            this.lblBanner = new Wisej.Web.Label();
            this.pnlBody = new Wisej.Web.Panel();
            this.gridList = new Wisej.Web.DataGridView();
            this.colCard = new Wisej.Web.DataGridViewTextBoxColumn();
            this.pnlDetail = new Wisej.Web.Panel();
            this.lblDetailHeader = new Wisej.Web.Label();
            this.lblReadingCaption = new Wisej.Web.Label();
            this.txtReading = new Wisej.Web.TextBox();
            this.lblNotesCaption = new Wisej.Web.Label();
            this.txtNotes = new Wisej.Web.TextBox();
            this.btnSubmitReading = new Wisej.Web.Button();
            this.lblDetailMessage = new Wisej.Web.Label();
            this.pnlStatus = new Wisej.Web.Panel();
            this.lblStatusText = new Wisej.Web.Label();
            this.lblPlatform = new Wisej.Web.Label();
            ((System.ComponentModel.ISupportInitialize)(this.gridList)).BeginInit();
            this.SuspendLayout();
            //
            // ── app bar: the app's own back arrow, "FieldOps · Work orders | WO-nnnn" ─────────
            //
            this.btnBack.Name = "btnBack";
            this.btnBack.Text = "‹";
            this.btnBack.Dock = Wisej.Web.DockStyle.Left;
            this.btnBack.Size = new Size(40, 42);
            this.btnBack.BackColor = Ops.Accent;
            this.btnBack.ForeColor = Color.White;
            this.btnBack.Font = Ops.Px(22, FontStyle.Bold);
            this.btnBack.Display = Wisej.Web.Display.Label;
            this.btnBack.ToolTipText = "Back";
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);

            this.lblAppTitle.Name = "lblAppTitle";
            this.lblAppTitle.AllowHtml = true;
            this.lblAppTitle.AutoSize = false;
            this.lblAppTitle.Dock = Wisej.Web.DockStyle.Fill;
            this.lblAppTitle.ForeColor = Color.White;
            this.lblAppTitle.Font = Ops.Px(13.5f, FontStyle.Bold);
            this.lblAppTitle.TextAlign = ContentAlignment.MiddleLeft;

            this.pnlAppBar.Name = "pnlAppBar";
            this.pnlAppBar.Dock = Wisej.Web.DockStyle.Top;
            this.pnlAppBar.Size = new Size(360, 42);
            this.pnlAppBar.BackColor = Ops.Accent;
            this.pnlAppBar.Controls.Add(this.lblAppTitle);
            this.pnlAppBar.Controls.Add(this.btnBack);
            //
            // ── banner: draft restored / moved to the background / reloading ─────────────────
            //
            this.lblBanner.Name = "lblBanner";
            this.lblBanner.AllowHtml = true;
            this.lblBanner.AutoSize = false;
            this.lblBanner.Dock = Wisej.Web.DockStyle.Top;
            this.lblBanner.Size = new Size(360, 38);
            this.lblBanner.Visible = false;
            //
            // ── the list: one card per order ─────────────────────────────────────────────────
            //
            this.colCard.Name = "colCard";
            this.colCard.AllowHtml = true;
            this.colCard.AutoSizeMode = Wisej.Web.DataGridViewAutoSizeColumnMode.Fill;

            this.gridList.Name = "gridList";
            this.gridList.Dock = Wisej.Web.DockStyle.Fill;
            this.gridList.AutoGenerateColumns = false;
            this.gridList.ReadOnly = true;
            this.gridList.RowHeadersVisible = false;
            this.gridList.ColumnHeadersVisible = false;
            this.gridList.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.gridList.MultiSelect = false;
            this.gridList.BorderStyle = Wisej.Web.BorderStyle.None;
            this.gridList.RowTemplate.Height = 64;
            this.gridList.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] { this.colCard });
            this.gridList.CellClick += new Wisej.Web.DataGridViewCellEventHandler(this.gridList_CellClick);
            //
            // ── the detail: header, meter reading, notes, Submit reading ────────────────────
            //
            this.lblDetailHeader.Name = "lblDetailHeader";
            this.lblDetailHeader.AllowHtml = true;
            this.lblDetailHeader.AutoSize = false;
            this.lblDetailHeader.Dock = Wisej.Web.DockStyle.Top;
            this.lblDetailHeader.Size = new Size(330, 70);

            this.lblReadingCaption.Name = "lblReadingCaption";
            this.lblReadingCaption.Text = "METER READING";
            this.lblReadingCaption.AutoSize = false;
            this.lblReadingCaption.Dock = Wisej.Web.DockStyle.Top;
            this.lblReadingCaption.Size = new Size(330, 26);
            this.lblReadingCaption.Font = Ops.Px(10.5f, FontStyle.Bold);
            this.lblReadingCaption.ForeColor = Ops.Muted;
            this.lblReadingCaption.TextAlign = ContentAlignment.BottomLeft;

            this.txtReading.Name = "txtReading";
            this.txtReading.Dock = Wisej.Web.DockStyle.Top;
            this.txtReading.Size = new Size(330, 34);
            this.txtReading.Font = Ops.Mono(13);
            this.txtReading.InputType.Type = Wisej.Web.TextBoxType.Number;
            this.txtReading.InputType.Step = 0.1;
            this.txtReading.TextChanged += new System.EventHandler(this.Draft_Changed);

            this.lblNotesCaption.Name = "lblNotesCaption";
            this.lblNotesCaption.Text = "NOTES";
            this.lblNotesCaption.AutoSize = false;
            this.lblNotesCaption.Dock = Wisej.Web.DockStyle.Top;
            this.lblNotesCaption.Size = new Size(330, 26);
            this.lblNotesCaption.Font = Ops.Px(10.5f, FontStyle.Bold);
            this.lblNotesCaption.ForeColor = Ops.Muted;
            this.lblNotesCaption.TextAlign = ContentAlignment.BottomLeft;

            this.txtNotes.Name = "txtNotes";
            this.txtNotes.Dock = Wisej.Web.DockStyle.Top;
            this.txtNotes.Multiline = true;
            this.txtNotes.Size = new Size(330, 64);
            this.txtNotes.Font = Ops.Px(11.5f);
            this.txtNotes.TextChanged += new System.EventHandler(this.Draft_Changed);

            this.btnSubmitReading.Name = "btnSubmitReading";
            this.btnSubmitReading.Text = "Submit reading";
            this.btnSubmitReading.Dock = Wisej.Web.DockStyle.Top;
            this.btnSubmitReading.Size = new Size(330, 36);
            this.btnSubmitReading.Margin = new Wisej.Web.Padding(0, 12, 0, 0);
            this.btnSubmitReading.BackColor = Ops.Accent;
            this.btnSubmitReading.ForeColor = Color.White;
            this.btnSubmitReading.Font = Ops.Px(12, FontStyle.Bold);
            this.btnSubmitReading.Click += new System.EventHandler(this.btnSubmitReading_Click);

            this.lblDetailMessage.Name = "lblDetailMessage";
            this.lblDetailMessage.AllowHtml = true;
            this.lblDetailMessage.AutoSize = false;
            this.lblDetailMessage.Dock = Wisej.Web.DockStyle.Top;
            this.lblDetailMessage.Size = new Size(330, 50);

            this.pnlDetail.Name = "pnlDetail";
            this.pnlDetail.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlDetail.Padding = new Wisej.Web.Padding(15, 12, 15, 12);
            this.pnlDetail.AutoScroll = true;
            this.pnlDetail.Visible = false;
            this.pnlDetail.Controls.Add(this.lblDetailMessage);
            this.pnlDetail.Controls.Add(this.btnSubmitReading);
            this.pnlDetail.Controls.Add(this.txtNotes);
            this.pnlDetail.Controls.Add(this.lblNotesCaption);
            this.pnlDetail.Controls.Add(this.txtReading);
            this.pnlDetail.Controls.Add(this.lblReadingCaption);
            this.pnlDetail.Controls.Add(this.lblDetailHeader);

            this.pnlBody.Name = "pnlBody";
            this.pnlBody.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlBody.Controls.Add(this.pnlDetail);
            this.pnlBody.Controls.Add(this.gridList);
            //
            // ── app status strip: "Remote · FieldOps server" ... platform ───────────────────
            //
            this.lblStatusText.Name = "lblStatusText";
            this.lblStatusText.AllowHtml = true;
            this.lblStatusText.AutoSize = false;
            this.lblStatusText.Dock = Wisej.Web.DockStyle.Fill;
            this.lblStatusText.Font = Ops.Px(10.5f, FontStyle.Bold);
            this.lblStatusText.ForeColor = Ops.Muted;
            this.lblStatusText.TextAlign = ContentAlignment.MiddleLeft;

            this.lblPlatform.Name = "lblPlatform";
            this.lblPlatform.AutoSize = false;
            this.lblPlatform.Dock = Wisej.Web.DockStyle.Right;
            this.lblPlatform.Size = new Size(90, 30);
            this.lblPlatform.Font = Ops.Mono(10);
            this.lblPlatform.ForeColor = Color.FromArgb(0x9A, 0xA9, 0xB8);
            this.lblPlatform.TextAlign = ContentAlignment.MiddleRight;

            this.pnlStatus.Name = "pnlStatus";
            this.pnlStatus.Dock = Wisej.Web.DockStyle.Bottom;
            this.pnlStatus.Size = new Size(360, 32);
            this.pnlStatus.Padding = new Wisej.Web.Padding(14, 0, 14, 0);
            this.pnlStatus.BackColor = Ops.Strip;
            this.pnlStatus.CssStyle = "border-top:1px solid #e4eaf1";
            this.pnlStatus.Controls.Add(this.lblStatusText);
            this.pnlStatus.Controls.Add(this.lblPlatform);
            //
            // WorkOrderPage
            //
            this.Name = "WorkOrderPage";
            this.Text = "FieldOps";
            this.BackColor = Color.White;
            this.Controls.Add(this.pnlBody);
            this.Controls.Add(this.pnlStatus);
            this.Controls.Add(this.lblBanner);
            this.Controls.Add(this.pnlAppBar);
            this.Load += new System.EventHandler(this.WorkOrderPage_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridList)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlAppBar;
        private Wisej.Web.Button btnBack;
        private Wisej.Web.Label lblAppTitle;
        private Wisej.Web.Label lblBanner;
        private Wisej.Web.Panel pnlBody;
        private Wisej.Web.DataGridView gridList;
        private Wisej.Web.DataGridViewTextBoxColumn colCard;
        private Wisej.Web.Panel pnlDetail;
        private Wisej.Web.Label lblDetailHeader;
        private Wisej.Web.Label lblReadingCaption;
        private Wisej.Web.TextBox txtReading;
        private Wisej.Web.Label lblNotesCaption;
        private Wisej.Web.TextBox txtNotes;
        private Wisej.Web.Button btnSubmitReading;
        private Wisej.Web.Label lblDetailMessage;
        private Wisej.Web.Panel pnlStatus;
        private Wisej.Web.Label lblStatusText;
        private Wisej.Web.Label lblPlatform;
    }
}
