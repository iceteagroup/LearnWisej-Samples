using System.Drawing;
using Wisej.Web;

namespace FieldOps
{
    partial class WorkOrdersPage
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

        private void InitializeComponent()
        {
            this.pnlAppBar = new Wisej.Web.Panel();
            this.lblAppTitle = new Wisej.Web.Label();
            this.pnlActions = new Wisej.Web.Panel();
            this.flowActions = new Wisej.Web.FlowLayoutPanel();
            this.btnStartJob = new Wisej.Web.Button();
            this.btnUpdateStatus = new Wisej.Web.Button();
            this.btnInstall = new Wisej.Web.Button();
            this.lblSynced = new Wisej.Web.Label();
            this.gridOrders = new Wisej.Web.DataGridView();
            this.colNumber = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colSite = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colPriority = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colDue = new Wisej.Web.DataGridViewTextBoxColumn();
            this.pnlStatus = new Wisej.Web.Panel();
            this.lblSessionCaption = new Wisej.Web.Label();
            this.lblConnection = new Wisej.Web.Label();
            this.lblCount = new Wisej.Web.Label();
            ((System.ComponentModel.ISupportInitialize)(this.gridOrders)).BeginInit();
            this.SuspendLayout();
            //
            // ── app bar: "FieldOps · My work orders" ─────────────────────────────────────────
            //
            this.lblAppTitle.Name = "lblAppTitle";
            this.lblAppTitle.AllowHtml = true;
            this.lblAppTitle.AutoSize = false;
            this.lblAppTitle.Dock = Wisej.Web.DockStyle.Fill;
            this.lblAppTitle.Padding = new Wisej.Web.Padding(18, 0, 0, 0);
            this.lblAppTitle.ForeColor = Color.White;
            this.lblAppTitle.Font = Ops.Px(14.5f, FontStyle.Bold);
            this.lblAppTitle.TextAlign = ContentAlignment.MiddleLeft;
            this.lblAppTitle.Text = "FieldOps<span style=\"margin-left:9px;font-weight:500;opacity:.85;font-size:13px\">· My work orders</span>";

            this.pnlAppBar.Name = "pnlAppBar";
            this.pnlAppBar.Dock = Wisej.Web.DockStyle.Top;
            this.pnlAppBar.Size = new Size(900, 42);
            this.pnlAppBar.BackColor = Ops.Accent;
            this.pnlAppBar.Controls.Add(this.lblAppTitle);
            //
            // ── action strip: Start job · Update status · Install FieldOps ...  synced hh:mm ──
            //
            this.btnStartJob.Name = "btnStartJob";
            this.btnStartJob.Text = "Start job";
            this.btnStartJob.AutoSize = true;
            this.btnStartJob.MinimumSize = new Size(0, 30);
            this.btnStartJob.Margin = new Wisej.Web.Padding(0, 0, 12, 0);
            this.btnStartJob.BackColor = Ops.Accent;
            this.btnStartJob.ForeColor = Color.White;
            this.btnStartJob.Font = Ops.Px(12.5f, FontStyle.Bold);
            this.btnStartJob.Click += new System.EventHandler(this.btnStartJob_Click);

            this.btnUpdateStatus.Name = "btnUpdateStatus";
            this.btnUpdateStatus.Text = "Update status";
            this.btnUpdateStatus.AutoSize = true;
            this.btnUpdateStatus.MinimumSize = new Size(0, 30);
            this.btnUpdateStatus.Margin = new Wisej.Web.Padding(0, 0, 12, 0);
            this.btnUpdateStatus.BackColor = Color.White;
            this.btnUpdateStatus.ForeColor = Ops.SecondaryInk;
            this.btnUpdateStatus.Font = Ops.Px(12.5f, FontStyle.Bold);
            this.btnUpdateStatus.Click += new System.EventHandler(this.btnUpdateStatus_Click);

            // The install offer: hidden until the server reveals it after the first good session.
            // Its click is handled IN THE PAGE ("execute" client event), because prompt() only
            // opens from a real user gesture - a server round trip would lose it.
            this.btnInstall.Name = "btnInstall";
            this.btnInstall.Text = "⊕ Install FieldOps";
            this.btnInstall.AutoSize = true;
            this.btnInstall.MinimumSize = new Size(0, 30);
            this.btnInstall.Margin = new Wisej.Web.Padding(0, 0, 12, 0);
            this.btnInstall.BackColor = Color.FromArgb(0xEA, 0xF3, 0xFF);
            this.btnInstall.ForeColor = Ops.Accent;
            this.btnInstall.Font = Ops.Px(12.5f, FontStyle.Bold);
            this.btnInstall.Visible = false;


            this.flowActions.Name = "flowActions";
            this.flowActions.Dock = Wisej.Web.DockStyle.Fill;
            this.flowActions.Controls.Add(this.btnStartJob);
            this.flowActions.Controls.Add(this.btnUpdateStatus);
            this.flowActions.Controls.Add(this.btnInstall);

            this.lblSynced.Name = "lblSynced";
            this.lblSynced.AutoSize = false;
            this.lblSynced.Dock = Wisej.Web.DockStyle.Right;
            this.lblSynced.Size = new Size(110, 30);
            this.lblSynced.Font = Ops.Mono(11.5f);
            this.lblSynced.ForeColor = Ops.Muted;
            this.lblSynced.TextAlign = ContentAlignment.MiddleRight;

            this.pnlActions.Name = "pnlActions";
            this.pnlActions.Dock = Wisej.Web.DockStyle.Top;
            this.pnlActions.Size = new Size(900, 50);
            this.pnlActions.Padding = new Wisej.Web.Padding(16, 10, 16, 10);
            this.pnlActions.BackColor = Ops.Strip;
            this.pnlActions.CssStyle = "border-bottom:1px solid #e4eaf1";
            this.pnlActions.Controls.Add(this.flowActions);
            this.pnlActions.Controls.Add(this.lblSynced);
            //
            // ── the work-order grid: Work order · Site / asset · Priority · Status · Due ──────
            //
            this.colNumber.Name = "colNumber";
            this.colNumber.HeaderText = "Work order";
            this.colNumber.DataPropertyName = "Number";
            this.colNumber.Width = 130;
            this.colNumber.DefaultCellStyle.Font = Ops.Mono(12.5f, FontStyle.Bold);
            this.colNumber.DefaultCellStyle.ForeColor = Ops.Accent;

            this.colSite.Name = "colSite";
            this.colSite.HeaderText = "Site / asset";
            this.colSite.DataPropertyName = "SiteAndAsset";
            this.colSite.Width = 300;
            this.colSite.DefaultCellStyle.ForeColor = Ops.Ink;

            this.colPriority.Name = "colPriority";
            this.colPriority.HeaderText = "Priority";
            this.colPriority.DataPropertyName = "Priority";
            this.colPriority.Width = 110;
            this.colPriority.DefaultCellStyle.ForeColor = Ops.Body;

            this.colStatus.Name = "colStatus";
            this.colStatus.HeaderText = "Status";
            this.colStatus.DataPropertyName = "Status";
            this.colStatus.Width = 130;
            this.colStatus.AllowHtml = true;

            this.colDue.Name = "colDue";
            this.colDue.HeaderText = "Due";
            this.colDue.DataPropertyName = "DueText";
            this.colDue.AutoSizeMode = Wisej.Web.DataGridViewAutoSizeColumnMode.Fill;
            this.colDue.DefaultCellStyle.Font = Ops.Mono(12.5f);
            this.colDue.DefaultCellStyle.ForeColor = Ops.Muted;

            this.gridOrders.Name = "gridOrders";
            this.gridOrders.Dock = Wisej.Web.DockStyle.Fill;
            this.gridOrders.AutoGenerateColumns = false;
            this.gridOrders.ReadOnly = true;
            this.gridOrders.RowHeadersVisible = false;
            this.gridOrders.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.gridOrders.MultiSelect = false;
            this.gridOrders.BorderStyle = Wisej.Web.BorderStyle.None;
            this.gridOrders.ColumnHeadersHeight = 30;
            this.gridOrders.ColumnHeadersDefaultCellStyle.BackColor = Ops.HeaderBack;
            this.gridOrders.ColumnHeadersDefaultCellStyle.ForeColor = Ops.Muted;
            this.gridOrders.ColumnHeadersDefaultCellStyle.Font = Ops.Px(11.5f, FontStyle.Bold);
            this.gridOrders.RowTemplate.Height = 32;
            this.gridOrders.DefaultCellStyle.Font = Ops.Px(12.5f);
            this.gridOrders.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
                this.colNumber, this.colSite, this.colPriority, this.colStatus, this.colDue });
            this.gridOrders.CellFormatting += new Wisej.Web.DataGridViewCellFormattingEventHandler(this.gridOrders_CellFormatting);
            this.gridOrders.CellDoubleClick += new Wisej.Web.DataGridViewCellEventHandler(this.gridOrders_CellDoubleClick);
            //
            // ── session strip: SESSION  Connected · 42 ms  ...  5 work orders · server-rendered ─
            //
            this.lblSessionCaption.Name = "lblSessionCaption";
            this.lblSessionCaption.Text = "SESSION";
            this.lblSessionCaption.AutoSize = false;
            this.lblSessionCaption.Dock = Wisej.Web.DockStyle.Left;
            this.lblSessionCaption.Size = new Size(76, 38);
            this.lblSessionCaption.Font = Ops.Px(11.5f, FontStyle.Bold);
            this.lblSessionCaption.ForeColor = Ops.Muted;
            this.lblSessionCaption.TextAlign = ContentAlignment.MiddleLeft;

            this.lblConnection.Name = "lblConnection";
            this.lblConnection.AutoSize = false;
            this.lblConnection.Dock = Wisej.Web.DockStyle.Fill;
            this.lblConnection.Font = Ops.Px(13, FontStyle.Bold);
            this.lblConnection.TextAlign = ContentAlignment.MiddleLeft;

            this.lblCount.Name = "lblCount";
            this.lblCount.AutoSize = false;
            this.lblCount.Dock = Wisej.Web.DockStyle.Right;
            this.lblCount.Size = new Size(260, 38);
            this.lblCount.Font = Ops.Mono(11.5f);
            this.lblCount.ForeColor = Color.FromArgb(0x7D, 0x8E, 0xA0);
            this.lblCount.TextAlign = ContentAlignment.MiddleRight;

            this.pnlStatus.Name = "pnlStatus";
            this.pnlStatus.Dock = Wisej.Web.DockStyle.Bottom;
            this.pnlStatus.Size = new Size(900, 40);
            this.pnlStatus.Padding = new Wisej.Web.Padding(16, 0, 16, 0);
            this.pnlStatus.BackColor = Ops.Strip;
            this.pnlStatus.CssStyle = "border-top:1px solid #e4eaf1";
            this.pnlStatus.Controls.Add(this.lblConnection);
            this.pnlStatus.Controls.Add(this.lblCount);
            this.pnlStatus.Controls.Add(this.lblSessionCaption);
            //
            // WorkOrdersPage - the last control added docks outermost, so the app bar is added last.
            //
            this.Name = "WorkOrdersPage";
            this.Text = "FieldOps";
            this.BackColor = Color.White;
            this.Controls.Add(this.gridOrders);
            this.Controls.Add(this.pnlStatus);
            this.Controls.Add(this.pnlActions);
            this.Controls.Add(this.pnlAppBar);
            this.Load += new System.EventHandler(this.WorkOrdersPage_Load);
            this.Resize += new System.EventHandler(this.WorkOrdersPage_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.gridOrders)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlAppBar;
        private Wisej.Web.Label lblAppTitle;
        private Wisej.Web.Panel pnlActions;
        private Wisej.Web.FlowLayoutPanel flowActions;
        private Wisej.Web.Button btnStartJob;
        private Wisej.Web.Button btnUpdateStatus;
        private Wisej.Web.Button btnInstall;
        private Wisej.Web.Label lblSynced;
        private Wisej.Web.DataGridView gridOrders;
        private Wisej.Web.DataGridViewTextBoxColumn colNumber;
        private Wisej.Web.DataGridViewTextBoxColumn colSite;
        private Wisej.Web.DataGridViewTextBoxColumn colPriority;
        private Wisej.Web.DataGridViewTextBoxColumn colStatus;
        private Wisej.Web.DataGridViewTextBoxColumn colDue;
        private Wisej.Web.Panel pnlStatus;
        private Wisej.Web.Label lblSessionCaption;
        private Wisej.Web.Label lblConnection;
        private Wisej.Web.Label lblCount;
    }
}
