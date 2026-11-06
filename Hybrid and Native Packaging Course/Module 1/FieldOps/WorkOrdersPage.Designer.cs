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
            this.pnlActions = new Wisej.Web.FlowLayoutPanel();
            this.btnUpdateStatus = new Wisej.Web.Button();
            this.btnTakePhoto = new Wisej.Web.Button();
            this.uploadPhoto = new Wisej.Web.Upload();
            this.gridOrders = new Wisej.Web.DataGridView();
            this.colNumber = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colSite = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colFault = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colPriority = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.pnlStatus = new Wisej.Web.Panel();
            this.lblConnection = new Wisej.Web.Label();
            this.lblDot = new Wisej.Web.Label();
            ((System.ComponentModel.ISupportInitialize)(this.gridOrders)).BeginInit();
            this.SuspendLayout();
            //
            // ── app bar: "FieldOps · My work orders" ─────────────────────────────────────────
            //
            this.lblAppTitle.Name = "lblAppTitle";
            this.lblAppTitle.AllowHtml = true;
            this.lblAppTitle.AutoSize = false;
            this.lblAppTitle.Dock = Wisej.Web.DockStyle.Fill;
            this.lblAppTitle.Padding = new Wisej.Web.Padding(16, 0, 0, 0);
            this.lblAppTitle.ForeColor = Color.White;
            this.lblAppTitle.Font = Ops.Px(14.5f, FontStyle.Bold);
            this.lblAppTitle.TextAlign = ContentAlignment.MiddleLeft;
            this.lblAppTitle.Text = "FieldOps<span style=\"margin-left:9px;font-weight:500;opacity:.85;font-size:13px\">· My work orders</span>";

            this.pnlAppBar.Name = "pnlAppBar";
            this.pnlAppBar.Dock = Wisej.Web.DockStyle.Top;
            this.pnlAppBar.Size = new Size(820, 40);
            this.pnlAppBar.BackColor = Ops.Accent;
            this.pnlAppBar.Controls.Add(this.lblAppTitle);
            //
            // ── action strip: Update status, then the photo action the client can serve ─────
            //
            this.btnUpdateStatus.Name = "btnUpdateStatus";
            this.btnUpdateStatus.Text = "Update status";
            this.btnUpdateStatus.AutoSize = true;
            this.btnUpdateStatus.MinimumSize = new Size(0, 30);
            this.btnUpdateStatus.Margin = new Wisej.Web.Padding(0, 0, 10, 0);
            this.btnUpdateStatus.BackColor = Ops.Accent;
            this.btnUpdateStatus.ForeColor = Color.White;
            this.btnUpdateStatus.Font = Ops.Px(12.5f, FontStyle.Bold);
            this.btnUpdateStatus.Click += new System.EventHandler(this.btnUpdateStatus_Click);

            this.btnTakePhoto.Name = "btnTakePhoto";
            this.btnTakePhoto.Text = "Take photo";
            this.btnTakePhoto.AutoSize = true;
            this.btnTakePhoto.MinimumSize = new Size(0, 30);
            this.btnTakePhoto.Margin = new Wisej.Web.Padding(0, 0, 10, 0);
            this.btnTakePhoto.BackColor = Color.White;
            this.btnTakePhoto.ForeColor = Ops.SecondaryInk;
            this.btnTakePhoto.Font = Ops.Px(12.5f, FontStyle.Bold);
            this.btnTakePhoto.Visible = false;
            this.btnTakePhoto.Click += new System.EventHandler(this.btnTakePhoto_Click);

            this.uploadPhoto.Name = "uploadPhoto";
            this.uploadPhoto.Text = "Attach photo (upload)";
            this.uploadPhoto.HideValue = true;
            this.uploadPhoto.AllowedFileTypes = "image/*";
            this.uploadPhoto.Size = new Size(196, 30);
            this.uploadPhoto.Margin = new Wisej.Web.Padding(0, 0, 10, 0);
            this.uploadPhoto.Font = Ops.Px(12.5f, FontStyle.Bold);
            this.uploadPhoto.Uploaded += new Wisej.Web.UploadedEventHandler(this.uploadPhoto_Uploaded);

            this.pnlActions.Name = "pnlActions";
            this.pnlActions.Dock = Wisej.Web.DockStyle.Top;
            this.pnlActions.Size = new Size(820, 48);
            this.pnlActions.Padding = new Wisej.Web.Padding(14, 9, 14, 0);
            this.pnlActions.BackColor = Ops.Strip;
            this.pnlActions.CssStyle = "border-bottom:1px solid #e4eaf1";
            this.pnlActions.Controls.Add(this.btnUpdateStatus);
            this.pnlActions.Controls.Add(this.btnTakePhoto);
            this.pnlActions.Controls.Add(this.uploadPhoto);
            //
            // ── the work-order grid ──────────────────────────────────────────────────────────
            //
            this.colNumber.Name = "colNumber";
            this.colNumber.HeaderText = "Order";
            this.colNumber.DataPropertyName = "Number";
            this.colNumber.Width = 100;
            this.colNumber.DefaultCellStyle.Font = Ops.Mono(12, FontStyle.Bold);
            this.colNumber.DefaultCellStyle.ForeColor = Ops.Accent;

            this.colSite.Name = "colSite";
            this.colSite.HeaderText = "Site";
            this.colSite.DataPropertyName = "Customer";
            this.colSite.Width = 210;
            this.colSite.DefaultCellStyle.Font = Ops.Px(12, FontStyle.Bold);
            this.colSite.DefaultCellStyle.ForeColor = Ops.Ink;

            this.colFault.Name = "colFault";
            this.colFault.HeaderText = "Fault";
            this.colFault.DataPropertyName = "Fault";
            this.colFault.AutoSizeMode = Wisej.Web.DataGridViewAutoSizeColumnMode.Fill;
            this.colFault.DefaultCellStyle.ForeColor = Ops.Body;

            this.colPriority.Name = "colPriority";
            this.colPriority.HeaderText = "Priority";
            this.colPriority.DataPropertyName = "Priority";
            this.colPriority.Width = 92;
            this.colPriority.DefaultCellStyle.Font = Ops.Px(12, FontStyle.Bold);

            this.colStatus.Name = "colStatus";
            this.colStatus.HeaderText = "Status";
            this.colStatus.DataPropertyName = "Status";
            this.colStatus.Width = 108;
            this.colStatus.AllowHtml = true;

            this.gridOrders.Name = "gridOrders";
            this.gridOrders.Dock = Wisej.Web.DockStyle.Fill;
            this.gridOrders.AutoGenerateColumns = false;
            this.gridOrders.ReadOnly = true;
            this.gridOrders.RowHeadersVisible = false;
            this.gridOrders.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.gridOrders.MultiSelect = false;
            this.gridOrders.BorderStyle = Wisej.Web.BorderStyle.None;
            this.gridOrders.ColumnHeadersHeight = 28;
            this.gridOrders.ColumnHeadersDefaultCellStyle.BackColor = Ops.HeaderBack;
            this.gridOrders.ColumnHeadersDefaultCellStyle.ForeColor = Ops.Muted;
            this.gridOrders.ColumnHeadersDefaultCellStyle.Font = Ops.Px(11, FontStyle.Bold);
            this.gridOrders.RowTemplate.Height = 36;
            this.gridOrders.DefaultCellStyle.Font = Ops.Px(12);
            this.gridOrders.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
                this.colNumber, this.colSite, this.colFault, this.colPriority, this.colStatus });
            this.gridOrders.CellFormatting += new Wisej.Web.DataGridViewCellFormattingEventHandler(this.gridOrders_CellFormatting);
            this.gridOrders.CellDoubleClick += new Wisej.Web.DataGridViewCellEventHandler(this.gridOrders_CellDoubleClick);
            //
            // ── status strip: "Connected · 4 open orders" ───────────────────────────────────
            //
            this.lblDot.Name = "lblDot";
            this.lblDot.AutoSize = false;
            this.lblDot.Dock = Wisej.Web.DockStyle.Left;
            this.lblDot.Size = new Size(22, 36);
            this.lblDot.AllowHtml = true;
            this.lblDot.TextAlign = ContentAlignment.MiddleLeft;

            this.lblConnection.Name = "lblConnection";
            this.lblConnection.AutoSize = false;
            this.lblConnection.Dock = Wisej.Web.DockStyle.Fill;
            this.lblConnection.Font = Ops.Px(12, FontStyle.Bold);
            this.lblConnection.TextAlign = ContentAlignment.MiddleLeft;

            this.pnlStatus.Name = "pnlStatus";
            this.pnlStatus.Dock = Wisej.Web.DockStyle.Bottom;
            this.pnlStatus.Size = new Size(820, 36);
            this.pnlStatus.Padding = new Wisej.Web.Padding(14, 0, 14, 0);
            this.pnlStatus.BackColor = Ops.Strip;
            this.pnlStatus.CssStyle = "border-top:1px solid #e4eaf1";
            this.pnlStatus.Controls.Add(this.lblConnection);
            this.pnlStatus.Controls.Add(this.lblDot);
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
        private Wisej.Web.FlowLayoutPanel pnlActions;
        private Wisej.Web.Button btnUpdateStatus;
        private Wisej.Web.Button btnTakePhoto;
        private Wisej.Web.Upload uploadPhoto;
        private Wisej.Web.DataGridView gridOrders;
        private Wisej.Web.DataGridViewTextBoxColumn colNumber;
        private Wisej.Web.DataGridViewTextBoxColumn colSite;
        private Wisej.Web.DataGridViewTextBoxColumn colFault;
        private Wisej.Web.DataGridViewTextBoxColumn colPriority;
        private Wisej.Web.DataGridViewTextBoxColumn colStatus;
        private Wisej.Web.Panel pnlStatus;
        private Wisej.Web.Label lblConnection;
        private Wisej.Web.Label lblDot;
    }
}
