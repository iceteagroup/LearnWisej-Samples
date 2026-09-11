namespace IntegrationLab
{
    partial class MainPage
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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
            this.panelGrid = new Wisej.Web.Panel();
            this.labelGridTitle = new Wisej.Web.Label();
            this.gridWorkOrders = new IntegrationLab.Widgets.WorkOrderGrid();
            this.panelPivot = new Wisej.Web.Panel();
            this.labelPivotTitle = new Wisej.Web.Label();
            this.pivotWorkOrders = new IntegrationLab.Widgets.WorkOrderPivot();
            this.panelTrace = new Wisej.Web.Panel();
            this.labelTraceTitle = new Wisej.Web.Label();
            this.listTrace = new Wisej.Web.ListBox();
            this.panelGrid.SuspendLayout();
            this.panelPivot.SuspendLayout();
            this.panelTrace.SuspendLayout();
            this.SuspendLayout();
            //
            // panelGrid
            //
            this.panelGrid.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left;
            this.panelGrid.BackColor = System.Drawing.Color.White;
            this.panelGrid.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelGrid.Controls.Add(this.labelGridTitle);
            this.panelGrid.Controls.Add(this.gridWorkOrders);
            this.panelGrid.Location = new System.Drawing.Point(30, 30);
            this.panelGrid.Name = "panelGrid";
            this.panelGrid.Size = new System.Drawing.Size(700, 380);
            //
            // labelGridTitle
            //
            this.labelGridTitle.AutoSize = false;
            this.labelGridTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelGridTitle.Location = new System.Drawing.Point(20, 12);
            this.labelGridTitle.Name = "labelGridTitle";
            this.labelGridTitle.Size = new System.Drawing.Size(660, 28);
            this.labelGridTitle.Text = "Editable grid";
            //
            // gridWorkOrders
            //
            this.gridWorkOrders.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.gridWorkOrders.Editable = true;
            this.gridWorkOrders.Location = new System.Drawing.Point(20, 46);
            this.gridWorkOrders.Name = "gridWorkOrders";
            this.gridWorkOrders.PageSize = 20;
            this.gridWorkOrders.Size = new System.Drawing.Size(660, 318);
            this.gridWorkOrders.Sort = "";
            //
            // panelPivot
            //
            this.panelPivot.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left;
            this.panelPivot.BackColor = System.Drawing.Color.White;
            this.panelPivot.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelPivot.Controls.Add(this.labelPivotTitle);
            this.panelPivot.Controls.Add(this.pivotWorkOrders);
            this.panelPivot.Location = new System.Drawing.Point(30, 426);
            this.panelPivot.Name = "panelPivot";
            this.panelPivot.Size = new System.Drawing.Size(700, 224);
            //
            // labelPivotTitle
            //
            this.labelPivotTitle.AutoSize = false;
            this.labelPivotTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelPivotTitle.Location = new System.Drawing.Point(20, 12);
            this.labelPivotTitle.Name = "labelPivotTitle";
            this.labelPivotTitle.Size = new System.Drawing.Size(660, 28);
            this.labelPivotTitle.Text = "Read-only pivot";
            //
            // pivotWorkOrders
            //
            this.pivotWorkOrders.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pivotWorkOrders.Location = new System.Drawing.Point(20, 46);
            this.pivotWorkOrders.Name = "pivotWorkOrders";
            this.pivotWorkOrders.Size = new System.Drawing.Size(660, 162);
            //
            // panelTrace
            //
            this.panelTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelTrace.BackColor = System.Drawing.Color.White;
            this.panelTrace.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelTrace.Controls.Add(this.labelTraceTitle);
            this.panelTrace.Controls.Add(this.listTrace);
            this.panelTrace.Location = new System.Drawing.Point(758, 30);
            this.panelTrace.Name = "panelTrace";
            this.panelTrace.Size = new System.Drawing.Size(560, 620);
            //
            // labelTraceTitle
            //
            this.labelTraceTitle.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelTraceTitle.AutoSize = false;
            this.labelTraceTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelTraceTitle.Location = new System.Drawing.Point(20, 12);
            this.labelTraceTitle.Name = "labelTraceTitle";
            this.labelTraceTitle.Size = new System.Drawing.Size(520, 28);
            this.labelTraceTitle.Text = "Remote operations";
            //
            // listTrace
            //
            this.listTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.listTrace.Font = new System.Drawing.Font("monospace", 9F);
            this.listTrace.Location = new System.Drawing.Point(20, 48);
            this.listTrace.Name = "listTrace";
            this.listTrace.Size = new System.Drawing.Size(520, 556);
            //
            // MainPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.panelGrid);
            this.Controls.Add(this.panelPivot);
            this.Controls.Add(this.panelTrace);
            this.Name = "MainPage";
            this.Size = new System.Drawing.Size(1348, 680);
            this.Text = "IntegrationLab — Data Widgets";
            this.panelGrid.ResumeLayout(false);
            this.panelPivot.ResumeLayout(false);
            this.panelTrace.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel panelGrid;
        private Wisej.Web.Label labelGridTitle;
        private IntegrationLab.Widgets.WorkOrderGrid gridWorkOrders;
        private Wisej.Web.Panel panelPivot;
        private Wisej.Web.Label labelPivotTitle;
        private IntegrationLab.Widgets.WorkOrderPivot pivotWorkOrders;
        private Wisej.Web.Panel panelTrace;
        private Wisej.Web.Label labelTraceTitle;
        private Wisej.Web.ListBox listTrace;
    }
}
