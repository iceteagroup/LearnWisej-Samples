using System.Drawing;
using Wisej.Web;

namespace FieldOps
{
    partial class WorkOrdersPage
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Services.OrderFeed.NewOrder -= this.OnFeed;
                if (components != null) components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Wisej.NET Designer generated code

        private void InitializeComponent()
        {
            this.pnlAppBar = new Wisej.Web.Panel();
            this.lblAppTitle = new Wisej.Web.Label();
            this.lblOffline = new Wisej.Web.Label();
            this.pnlActions = new Wisej.Web.Panel();
            this.flowActions = new Wisej.Web.FlowLayoutPanel();
            this.btnExportReport = new Wisej.Web.Button();
            this.btnOpenWindow = new Wisej.Web.Button();
            this.btnInstall = new Wisej.Web.Button();
            this.btnStartShift = new Wisej.Web.Button();
            this.pnlDeviceActions = new Wisej.Web.FlowLayoutPanel();
            this.btnTakePhoto = new Wisej.Web.Button();
            this.btnSignature = new Wisej.Web.Button();
            this.btnNavigate = new Wisej.Web.Button();
            this.lblNote = new Wisej.Web.Label();
            this.updatePanel = new FieldOps.ClientUpdatePanel();
            this.gridOrders = new Wisej.Web.DataGridView();
            this.colNumber = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colSite = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colTask = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.pnlStatus = new Wisej.Web.Panel();
            this.lblStatusCaption = new Wisej.Web.Label();
            this.lblExportStatus = new Wisej.Web.Label();
            this.lblFooter = new Wisej.Web.Label();
            ((System.ComponentModel.ISupportInitialize)(this.gridOrders)).BeginInit();
            this.SuspendLayout();
            //
            // ── app bar: "FieldOps · Work orders" ────────────────────────────────────────────
            //
            this.lblAppTitle.Name = "lblAppTitle";
            this.lblAppTitle.AllowHtml = true;
            this.lblAppTitle.AutoSize = false;
            this.lblAppTitle.Dock = Wisej.Web.DockStyle.Fill;
            this.lblAppTitle.Padding = new Wisej.Web.Padding(16, 0, 0, 0);
            this.lblAppTitle.ForeColor = Color.White;
            this.lblAppTitle.Font = Ops.Px(14, FontStyle.Bold);
            this.lblAppTitle.TextAlign = ContentAlignment.MiddleLeft;
            this.lblAppTitle.Text = "FieldOps<span style=\"margin-left:9px;font-weight:500;opacity:.85;font-size:12.5px\">· Today</span>";

            this.pnlAppBar.Name = "pnlAppBar";
            this.pnlAppBar.Dock = Wisej.Web.DockStyle.Top;
            this.pnlAppBar.Size = new Size(1000, 38);
            this.pnlAppBar.BackColor = Ops.Accent;
            this.pnlAppBar.Controls.Add(this.lblAppTitle);
            //
            // ── offline banner: only the Local app (FieldOps.Offline) shows it ───────────────
            //
            this.lblOffline.Name = "lblOffline";
            this.lblOffline.AllowHtml = true;
            this.lblOffline.AutoSize = false;
            this.lblOffline.Dock = Wisej.Web.DockStyle.Top;
            this.lblOffline.Size = new Size(1000, 34);
            this.lblOffline.Padding = new Wisej.Web.Padding(16, 0, 16, 0);
            this.lblOffline.BackColor = Color.FromArgb(0xFF, 0xF5, 0xE2);
            this.lblOffline.ForeColor = Color.FromArgb(0x8A, 0x5A, 0x11);
            this.lblOffline.Font = Ops.Px(12.5f, FontStyle.Bold);
            this.lblOffline.TextAlign = ContentAlignment.MiddleLeft;
            this.lblOffline.CssStyle = "border-bottom:1px solid #f0d9a8";
            this.lblOffline.Text = "<span style=\"display:inline-block;width:8px;height:8px;border-radius:999px;background:#e8a13c;margin-right:10px\"></span>Working offline — FieldOps on this laptop's embedded server";
            this.lblOffline.Visible = false;
            //
            // ── action strip: Export report · Open in new window ─────────────────────────────
            //
            this.btnExportReport.Name = "btnExportReport";
            this.btnExportReport.Text = "Export report";
            this.btnExportReport.AutoSize = true;
            this.btnExportReport.MinimumSize = new Size(0, 30);
            this.btnExportReport.Margin = new Wisej.Web.Padding(0, 0, 11, 0);
            this.btnExportReport.BackColor = Ops.Accent;
            this.btnExportReport.ForeColor = Color.White;
            this.btnExportReport.Font = Ops.Px(12.5f, FontStyle.Bold);
            this.btnExportReport.Click += new System.EventHandler(this.btnExportReport_Click);

            this.btnOpenWindow.Name = "btnOpenWindow";
            this.btnOpenWindow.Text = "Open in new window";
            this.btnOpenWindow.AutoSize = true;
            this.btnOpenWindow.MinimumSize = new Size(0, 30);
            this.btnOpenWindow.Margin = new Wisej.Web.Padding(0, 0, 11, 0);
            this.btnOpenWindow.BackColor = Color.White;
            this.btnOpenWindow.ForeColor = Ops.SecondaryInk;
            this.btnOpenWindow.Font = Ops.Px(12.5f, FontStyle.Bold);
            this.btnOpenWindow.Click += new System.EventHandler(this.btnOpenWindow_Click);

            // The PWA install offer from Module 2: browser only, never inside the Hybrid client.
            this.btnInstall.Name = "btnInstall";
            this.btnInstall.Text = "⊕ Install FieldOps";
            this.btnInstall.AutoSize = true;
            this.btnInstall.MinimumSize = new Size(0, 30);
            this.btnInstall.Margin = new Wisej.Web.Padding(0, 0, 11, 0);
            this.btnInstall.BackColor = Color.FromArgb(0xEA, 0xF3, 0xFF);
            this.btnInstall.ForeColor = Ops.Accent;
            this.btnInstall.Font = Ops.Px(12.5f, FontStyle.Bold);
            this.btnInstall.Visible = false;

            // Start shift (Module 6): the version check, then the work orders. Also run at startup.
            this.btnStartShift.Name = "btnStartShift";
            this.btnStartShift.Text = "Start shift";
            this.btnStartShift.AutoSize = true;
            this.btnStartShift.MinimumSize = new Size(0, 30);
            this.btnStartShift.Margin = new Wisej.Web.Padding(0, 0, 11, 0);
            this.btnStartShift.BackColor = Color.White;
            this.btnStartShift.ForeColor = Ops.SecondaryInk;
            this.btnStartShift.Font = Ops.Px(12.5f, FontStyle.Bold);
            this.btnStartShift.Click += new System.EventHandler(this.btnStartShift_Click);

            this.flowActions.Name = "flowActions";
            this.flowActions.Dock = Wisej.Web.DockStyle.Fill;
            this.flowActions.Controls.Add(this.btnExportReport);
            this.flowActions.Controls.Add(this.btnOpenWindow);
            this.flowActions.Controls.Add(this.btnInstall);
            this.flowActions.Controls.Add(this.btnStartShift);

            this.pnlActions.Name = "pnlActions";
            this.pnlActions.Dock = Wisej.Web.DockStyle.Top;
            this.pnlActions.Size = new Size(1000, 50);
            this.pnlActions.Padding = new Wisej.Web.Padding(16, 10, 16, 10);
            this.pnlActions.BackColor = Ops.Strip;
            this.pnlActions.CssStyle = "border-bottom:1px solid #e4eaf1";
            this.pnlActions.Controls.Add(this.flowActions);
            //
            // ── the work-order grid: Order · Site · Task · Status ────────────────────────────
            //
            this.colNumber.Name = "colNumber";
            this.colNumber.HeaderText = "Order";
            this.colNumber.DataPropertyName = "Number";
            this.colNumber.Width = 90;
            this.colNumber.DefaultCellStyle.Font = Ops.Mono(12.5f, FontStyle.Bold);
            this.colNumber.DefaultCellStyle.ForeColor = Ops.Accent;

            this.colSite.Name = "colSite";
            this.colSite.HeaderText = "Location";
            this.colSite.DataPropertyName = "SiteAddress";
            this.colSite.Width = 150;
            this.colSite.DefaultCellStyle.ForeColor = Ops.Ink;

            this.colTask.Name = "colTask";
            this.colTask.HeaderText = "Task";
            this.colTask.DataPropertyName = "Fault";
            this.colTask.AutoSizeMode = Wisej.Web.DataGridViewAutoSizeColumnMode.Fill;
            this.colTask.DefaultCellStyle.ForeColor = Ops.Body;

            this.colStatus.Name = "colStatus";
            this.colStatus.HeaderText = "State";
            this.colStatus.DataPropertyName = "Status";
            this.colStatus.Width = 96;
            this.colStatus.AllowHtml = true;

            this.gridOrders.Name = "gridOrders";
            this.gridOrders.Dock = Wisej.Web.DockStyle.Fill;
            this.gridOrders.AutoGenerateColumns = false;
            this.gridOrders.ReadOnly = true;
            this.gridOrders.RowHeadersVisible = false;
            this.gridOrders.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.gridOrders.MultiSelect = false;
            this.gridOrders.BorderStyle = Wisej.Web.BorderStyle.None;
            this.gridOrders.ColumnHeadersHeight = 29;
            this.gridOrders.ColumnHeadersDefaultCellStyle.BackColor = Ops.HeaderBack;
            this.gridOrders.ColumnHeadersDefaultCellStyle.ForeColor = Ops.Muted;
            this.gridOrders.ColumnHeadersDefaultCellStyle.Font = Ops.Px(11.5f, FontStyle.Bold);
            this.gridOrders.RowTemplate.Height = 33;
            this.gridOrders.DefaultCellStyle.Font = Ops.Px(12.5f);
            this.gridOrders.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
                this.colNumber, this.colTask, this.colSite, this.colStatus });
            this.gridOrders.CellFormatting += new Wisej.Web.DataGridViewCellFormattingEventHandler(this.gridOrders_CellFormatting);
            this.gridOrders.CellDoubleClick += new Wisej.Web.DataGridViewCellEventHandler(this.gridOrders_CellDoubleClick);
            //
            // ── status strip: STATUS  <export / sync / offline text>   ...   N orders · client ─
            //
            this.lblStatusCaption.Name = "lblStatusCaption";
            this.lblStatusCaption.Text = "STATUS";
            this.lblStatusCaption.AutoSize = false;
            this.lblStatusCaption.Dock = Wisej.Web.DockStyle.Left;
            this.lblStatusCaption.Size = new Size(70, 38);
            this.lblStatusCaption.Font = Ops.Px(11.5f, FontStyle.Bold);
            this.lblStatusCaption.ForeColor = Ops.Muted;
            this.lblStatusCaption.TextAlign = ContentAlignment.MiddleLeft;

            this.lblExportStatus.Name = "lblExportStatus";
            this.lblExportStatus.AutoSize = false;
            this.lblExportStatus.Dock = Wisej.Web.DockStyle.Fill;
            this.lblExportStatus.Font = Ops.Px(12.5f, FontStyle.Bold);
            this.lblExportStatus.ForeColor = Ops.Muted;
            this.lblExportStatus.TextAlign = ContentAlignment.MiddleLeft;
            this.lblExportStatus.Text = "Ready";

            this.lblFooter.Name = "lblFooter";
            this.lblFooter.AutoSize = false;
            this.lblFooter.Dock = Wisej.Web.DockStyle.Right;
            this.lblFooter.Size = new Size(300, 38);
            this.lblFooter.Font = Ops.Mono(11.5f);
            this.lblFooter.ForeColor = Color.FromArgb(0x7D, 0x8E, 0xA0);
            this.lblFooter.TextAlign = ContentAlignment.MiddleRight;

            this.pnlStatus.Name = "pnlStatus";
            this.pnlStatus.Dock = Wisej.Web.DockStyle.Bottom;
            this.pnlStatus.Size = new Size(1000, 40);
            this.pnlStatus.Padding = new Wisej.Web.Padding(16, 0, 16, 0);
            this.pnlStatus.BackColor = Ops.Strip;
            this.pnlStatus.CssStyle = "border-top:1px solid #e4eaf1";
            this.pnlStatus.Controls.Add(this.lblExportStatus);
            this.pnlStatus.Controls.Add(this.lblFooter);
            this.pnlStatus.Controls.Add(this.lblStatusCaption);
            //
            // ── device actions for the selected order (Take Photo · Signature · Navigate) ─────
            //
            this.btnTakePhoto.Name = "btnTakePhoto";
            this.btnTakePhoto.Text = "Take Photo";
            this.btnTakePhoto.AutoSize = true;
            this.btnTakePhoto.MinimumSize = new Size(0, 28);
            this.btnTakePhoto.Margin = new Wisej.Web.Padding(0, 0, 9, 0);
            this.btnTakePhoto.BackColor = Ops.Accent;
            this.btnTakePhoto.ForeColor = Color.White;
            this.btnTakePhoto.Font = Ops.Px(11.5f, FontStyle.Bold);
            this.btnTakePhoto.Click += new System.EventHandler(this.DeviceAction_Click);

            this.btnSignature.Name = "btnSignature";
            this.btnSignature.Text = "Signature";
            this.btnSignature.AutoSize = true;
            this.btnSignature.MinimumSize = new Size(0, 28);
            this.btnSignature.Margin = new Wisej.Web.Padding(0, 0, 9, 0);
            this.btnSignature.BackColor = Color.White;
            this.btnSignature.ForeColor = Ops.SecondaryInk;
            this.btnSignature.Font = Ops.Px(11.5f, FontStyle.Bold);
            this.btnSignature.Click += new System.EventHandler(this.DeviceAction_Click);

            this.btnNavigate.Name = "btnNavigate";
            this.btnNavigate.Text = "Navigate";
            this.btnNavigate.AutoSize = true;
            this.btnNavigate.MinimumSize = new Size(0, 28);
            this.btnNavigate.BackColor = Color.White;
            this.btnNavigate.ForeColor = Ops.SecondaryInk;
            this.btnNavigate.Font = Ops.Px(11.5f, FontStyle.Bold);
            this.btnNavigate.Click += new System.EventHandler(this.DeviceAction_Click);

            this.pnlDeviceActions.Name = "pnlDeviceActions";
            this.pnlDeviceActions.Dock = Wisej.Web.DockStyle.Bottom;
            this.pnlDeviceActions.Size = new Size(1000, 50);
            this.pnlDeviceActions.Padding = new Wisej.Web.Padding(14, 11, 14, 0);
            this.pnlDeviceActions.BackColor = Ops.Strip;
            this.pnlDeviceActions.CssStyle = "border-top:1px solid #e4eaf1";
            this.pnlDeviceActions.Controls.Add(this.btnTakePhoto);
            this.pnlDeviceActions.Controls.Add(this.btnSignature);
            this.pnlDeviceActions.Controls.Add(this.btnNavigate);
            //
            // ── the version note: admitted / newer client available / what's new ─────────────
            //
            this.lblNote.Name = "lblNote";
            this.lblNote.AutoSize = false;
            this.lblNote.Dock = Wisej.Web.DockStyle.Bottom;
            this.lblNote.Size = new Size(1000, 34);
            this.lblNote.Padding = new Wisej.Web.Padding(16, 0, 16, 0);
            this.lblNote.Font = Ops.Px(11.5f, FontStyle.Bold);
            this.lblNote.TextAlign = ContentAlignment.MiddleLeft;
            this.lblNote.Visible = false;
            this.lblNote.Click += new System.EventHandler(this.lblNote_Click);
            //
            // ── the blocking update screen (an outdated Hybrid client) ───────────────────────
            //
            this.updatePanel.Name = "updatePanel";
            this.updatePanel.Dock = Wisej.Web.DockStyle.Fill;
            this.updatePanel.Visible = false;
            //
            // WorkOrdersPage - the last control added docks outermost, so the app bar is added last.
            //
            this.Name = "WorkOrdersPage";
            this.Text = "FieldOps";
            this.BackColor = Color.White;
            this.Controls.Add(this.updatePanel);
            this.Controls.Add(this.gridOrders);
            this.Controls.Add(this.pnlDeviceActions);
            this.Controls.Add(this.lblNote);
            this.Controls.Add(this.pnlStatus);
            this.Controls.Add(this.pnlActions);
            this.Controls.Add(this.lblOffline);
            this.Controls.Add(this.pnlAppBar);
            this.Load += new System.EventHandler(this.WorkOrdersPage_Load);
            this.Resize += new System.EventHandler(this.WorkOrdersPage_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.gridOrders)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlAppBar;
        private Wisej.Web.Label lblAppTitle;
        private Wisej.Web.Label lblOffline;
        private Wisej.Web.Panel pnlActions;
        private Wisej.Web.FlowLayoutPanel flowActions;
        private Wisej.Web.Button btnExportReport;
        private Wisej.Web.Button btnOpenWindow;
        private Wisej.Web.Button btnInstall;
        private Wisej.Web.Button btnStartShift;
        private Wisej.Web.FlowLayoutPanel pnlDeviceActions;
        private Wisej.Web.Button btnTakePhoto;
        private Wisej.Web.Button btnSignature;
        private Wisej.Web.Button btnNavigate;
        private Wisej.Web.Label lblNote;
        private FieldOps.ClientUpdatePanel updatePanel;
        private Wisej.Web.DataGridView gridOrders;
        private Wisej.Web.DataGridViewTextBoxColumn colNumber;
        private Wisej.Web.DataGridViewTextBoxColumn colSite;
        private Wisej.Web.DataGridViewTextBoxColumn colTask;
        private Wisej.Web.DataGridViewTextBoxColumn colStatus;
        private Wisej.Web.Panel pnlStatus;
        private Wisej.Web.Label lblStatusCaption;
        private Wisej.Web.Label lblExportStatus;
        private Wisej.Web.Label lblFooter;
    }
}
