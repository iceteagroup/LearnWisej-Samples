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
            this.components = new System.ComponentModel.Container();
            this.panelGrid = new Wisej.Web.Panel();
            this.labelGridTitle = new Wisej.Web.Label();
            this.labelStatus = new Wisej.Web.Label();
            this.gridWorkOrders = new IntegrationLab.Widgets.WorkOrderGrid();
            this.labelGridState = new Wisej.Web.Label();
            this.panelPivot = new Wisej.Web.Panel();
            this.labelPivotTitle = new Wisej.Web.Label();
            this.pivotWorkOrders = new IntegrationLab.Widgets.WorkOrderPivot();
            this.labelPivotState = new Wisej.Web.Label();
            this.panelTrace = new Wisej.Web.Panel();
            this.labelTraceTitle = new Wisej.Web.Label();
            this.listTrace = new Wisej.Web.ListBox();
            this.labelTraceFooter = new Wisej.Web.Label();
            this.labelAlarm = new Wisej.Web.Label();
            this.panelActions = new Wisej.Web.Panel();
            this.buttonReloadGrid = new Wisej.Web.Button();
            this.buttonNextPage = new Wisej.Web.Button();
            this.buttonSortStatus = new Wisej.Web.Button();
            this.buttonInsert = new Wisej.Web.Button();
            this.buttonDelete = new Wisej.Web.Button();
            this.buttonReloadPivot = new Wisej.Web.Button();
            this.buttonPivotPriority = new Wisej.Web.Button();
            this.buttonUnknownKey = new Wisej.Web.Button();
            this.buttonTake1000 = new Wisej.Web.Button();
            this.buttonClear = new Wisej.Web.Button();
            this.panelGrid.SuspendLayout();
            this.panelPivot.SuspendLayout();
            this.panelTrace.SuspendLayout();
            this.panelActions.SuspendLayout();
            this.SuspendLayout();
            //
            // panelGrid  (the editable grid card)
            //
            this.panelGrid.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left;
            this.panelGrid.BackColor = System.Drawing.Color.White;
            this.panelGrid.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelGrid.Controls.Add(this.labelGridTitle);
            this.panelGrid.Controls.Add(this.labelStatus);
            this.panelGrid.Controls.Add(this.gridWorkOrders);
            this.panelGrid.Controls.Add(this.labelGridState);
            this.panelGrid.Location = new System.Drawing.Point(30, 30);
            this.panelGrid.Name = "panelGrid";
            this.panelGrid.Size = new System.Drawing.Size(700, 330);
            //
            // labelGridTitle
            //
            this.labelGridTitle.AutoSize = false;
            this.labelGridTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelGridTitle.Location = new System.Drawing.Point(20, 12);
            this.labelGridTitle.Name = "labelGridTitle";
            this.labelGridTitle.Size = new System.Drawing.Size(500, 28);
            this.labelGridTitle.Text = "Editable grid — Kendo-style DataSource → postback";
            //
            // labelStatus
            //
            this.labelStatus.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.labelStatus.AutoSize = false;
            this.labelStatus.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelStatus.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.labelStatus.Location = new System.Drawing.Point(520, 14);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(160, 24);
            this.labelStatus.Text = "● idle";
            this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // gridWorkOrders  (Widget host: the vendor grid lives inside its container)
            //
            this.gridWorkOrders.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.gridWorkOrders.Editable = true;
            this.gridWorkOrders.Location = new System.Drawing.Point(20, 46);
            this.gridWorkOrders.Name = "gridWorkOrders";
            this.gridWorkOrders.PageSize = 20;
            this.gridWorkOrders.Size = new System.Drawing.Size(660, 240);
            this.gridWorkOrders.Sort = "";
            //
            // labelGridState  (what the server owns right now)
            //
            this.labelGridState.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelGridState.AutoSize = false;
            this.labelGridState.Font = new System.Drawing.Font("monospace", 9F);
            this.labelGridState.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelGridState.Location = new System.Drawing.Point(20, 292);
            this.labelGridState.Name = "labelGridState";
            this.labelGridState.Size = new System.Drawing.Size(660, 30);
            this.labelGridState.Text = "";
            this.labelGridState.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // panelPivot  (the read-only pivot card)
            //
            this.panelPivot.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left;
            this.panelPivot.BackColor = System.Drawing.Color.White;
            this.panelPivot.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelPivot.Controls.Add(this.labelPivotTitle);
            this.panelPivot.Controls.Add(this.pivotWorkOrders);
            this.panelPivot.Controls.Add(this.labelPivotState);
            this.panelPivot.Location = new System.Drawing.Point(30, 376);
            this.panelPivot.Name = "panelPivot";
            this.panelPivot.Size = new System.Drawing.Size(700, 214);
            //
            // labelPivotTitle
            //
            this.labelPivotTitle.AutoSize = false;
            this.labelPivotTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelPivotTitle.Location = new System.Drawing.Point(20, 12);
            this.labelPivotTitle.Name = "labelPivotTitle";
            this.labelPivotTitle.Size = new System.Drawing.Size(660, 28);
            this.labelPivotTitle.Text = "Read-only pivot — DevExtreme-style CustomStore → WebMethod";
            //
            // pivotWorkOrders  (Widget host: the vendor pivot lives inside its container)
            //
            this.pivotWorkOrders.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pivotWorkOrders.Location = new System.Drawing.Point(20, 46);
            this.pivotWorkOrders.Name = "pivotWorkOrders";
            this.pivotWorkOrders.Size = new System.Drawing.Size(660, 132);
            //
            // labelPivotState
            //
            this.labelPivotState.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelPivotState.AutoSize = false;
            this.labelPivotState.Font = new System.Drawing.Font("monospace", 9F);
            this.labelPivotState.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelPivotState.Location = new System.Drawing.Point(20, 184);
            this.labelPivotState.Name = "labelPivotState";
            this.labelPivotState.Size = new System.Drawing.Size(660, 22);
            this.labelPivotState.Text = "";
            this.labelPivotState.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // panelTrace  (Server ⇄ Client live message trace)
            //
            this.panelTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelTrace.BackColor = System.Drawing.Color.White;
            this.panelTrace.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelTrace.Controls.Add(this.labelTraceTitle);
            this.panelTrace.Controls.Add(this.listTrace);
            this.panelTrace.Controls.Add(this.labelTraceFooter);
            this.panelTrace.Location = new System.Drawing.Point(758, 30);
            this.panelTrace.Name = "panelTrace";
            this.panelTrace.Size = new System.Drawing.Size(560, 560);
            //
            // labelTraceTitle
            //
            this.labelTraceTitle.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelTraceTitle.AutoSize = false;
            this.labelTraceTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelTraceTitle.Location = new System.Drawing.Point(20, 12);
            this.labelTraceTitle.Name = "labelTraceTitle";
            this.labelTraceTitle.Size = new System.Drawing.Size(520, 28);
            this.labelTraceTitle.Text = "Server ⇄ Client  ·  live message trace";
            //
            // listTrace
            //
            this.listTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.listTrace.Font = new System.Drawing.Font("monospace", 9F);
            this.listTrace.Location = new System.Drawing.Point(20, 48);
            this.listTrace.Name = "listTrace";
            this.listTrace.Size = new System.Drawing.Size(520, 464);
            //
            // labelTraceFooter
            //
            this.labelTraceFooter.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelTraceFooter.AutoSize = false;
            this.labelTraceFooter.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelTraceFooter.Location = new System.Drawing.Point(20, 520);
            this.labelTraceFooter.Name = "labelTraceFooter";
            this.labelTraceFooter.Size = new System.Drawing.Size(520, 26);
            this.labelTraceFooter.Text = "grid ops: HTTP → postback URL   ·   pivot ops: WebMethod   ·   → .NET→JS   ← JS→.NET   • server";
            //
            // labelAlarm  (error banner: appears on a failure path, disappears on the next action)
            //
            this.labelAlarm.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelAlarm.AutoSize = false;
            this.labelAlarm.BackColor = System.Drawing.Color.FromArgb(255, 244, 229);
            this.labelAlarm.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelAlarm.ForeColor = System.Drawing.Color.FromArgb(146, 64, 14);
            this.labelAlarm.Location = new System.Drawing.Point(30, 602);
            this.labelAlarm.Name = "labelAlarm";
            this.labelAlarm.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.labelAlarm.Size = new System.Drawing.Size(1288, 36);
            this.labelAlarm.Text = "";
            this.labelAlarm.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelAlarm.Visible = false;
            //
            // panelActions
            //
            this.panelActions.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelActions.Controls.Add(this.buttonReloadGrid);
            this.panelActions.Controls.Add(this.buttonNextPage);
            this.panelActions.Controls.Add(this.buttonSortStatus);
            this.panelActions.Controls.Add(this.buttonInsert);
            this.panelActions.Controls.Add(this.buttonDelete);
            this.panelActions.Controls.Add(this.buttonReloadPivot);
            this.panelActions.Controls.Add(this.buttonPivotPriority);
            this.panelActions.Controls.Add(this.buttonUnknownKey);
            this.panelActions.Controls.Add(this.buttonTake1000);
            this.panelActions.Controls.Add(this.buttonClear);
            this.panelActions.Location = new System.Drawing.Point(30, 646);
            this.panelActions.Name = "panelActions";
            this.panelActions.Size = new System.Drawing.Size(1288, 44);
            //
            // grid buttons (success paths)
            //
            this.buttonReloadGrid.Location = new System.Drawing.Point(0, 4);
            this.buttonReloadGrid.Name = "buttonReloadGrid";
            this.buttonReloadGrid.Size = new System.Drawing.Size(100, 36);
            this.buttonReloadGrid.Text = "Reload grid";
            this.buttonReloadGrid.ToolTipText = "Call(\"reload\") → the vendor GETs &action=load again.";
            this.buttonReloadGrid.Click += new System.EventHandler(this.buttonReloadGrid_Click);
            this.buttonNextPage.Location = new System.Drawing.Point(108, 4);
            this.buttonNextPage.Name = "buttonNextPage";
            this.buttonNextPage.Size = new System.Drawing.Size(92, 36);
            this.buttonNextPage.Text = "Next page";
            this.buttonNextPage.ToolTipText = "Call(\"nextPage\") → remote paging: skip += take.";
            this.buttonNextPage.Click += new System.EventHandler(this.buttonNextPage_Click);
            this.buttonSortStatus.Location = new System.Drawing.Point(208, 4);
            this.buttonSortStatus.Name = "buttonSortStatus";
            this.buttonSortStatus.Size = new System.Drawing.Size(112, 36);
            this.buttonSortStatus.Text = "Sort by status";
            this.buttonSortStatus.ToolTipText = "Typed property Sort → Options.sort → update() → remote sort.";
            this.buttonSortStatus.Click += new System.EventHandler(this.buttonSortStatus_Click);
            this.buttonInsert.Location = new System.Drawing.Point(328, 4);
            this.buttonInsert.Name = "buttonInsert";
            this.buttonInsert.Size = new System.Drawing.Size(132, 36);
            this.buttonInsert.Text = "Insert sample row";
            this.buttonInsert.ToolTipText = "Call(\"insertRow\", values) → POST &action=create.";
            this.buttonInsert.Click += new System.EventHandler(this.buttonInsert_Click);
            this.buttonDelete.Location = new System.Drawing.Point(468, 4);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(122, 36);
            this.buttonDelete.Text = "Delete selected";
            this.buttonDelete.ToolTipText = "Call(\"deleteSelected\") → POST &action=destroy (click a cell first).";
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            //
            // pivot buttons
            //
            this.buttonReloadPivot.Location = new System.Drawing.Point(606, 4);
            this.buttonReloadPivot.Name = "buttonReloadPivot";
            this.buttonReloadPivot.Size = new System.Drawing.Size(104, 36);
            this.buttonReloadPivot.Text = "Reload pivot";
            this.buttonReloadPivot.ToolTipText = "Call(\"reload\") → CustomStore.load() → WebMethod LoadPivot.";
            this.buttonReloadPivot.Click += new System.EventHandler(this.buttonReloadPivot_Click);
            this.buttonPivotPriority.Location = new System.Drawing.Point(718, 4);
            this.buttonPivotPriority.Name = "buttonPivotPriority";
            this.buttonPivotPriority.Size = new System.Drawing.Size(140, 36);
            this.buttonPivotPriority.Text = "Pivot Site×Priority";
            this.buttonPivotPriority.ToolTipText = "Replaces the JSON option object → update() → the vendor loads again.";
            this.buttonPivotPriority.Click += new System.EventHandler(this.buttonPivotPriority_Click);
            //
            // failure paths
            //
            this.buttonUnknownKey.Location = new System.Drawing.Point(874, 4);
            this.buttonUnknownKey.Name = "buttonUnknownKey";
            this.buttonUnknownKey.Size = new System.Drawing.Size(148, 36);
            this.buttonUnknownKey.Text = "Unknown key update";
            this.buttonUnknownKey.ToolTipText = "update WO-9999 → the store has no such key → 404 → error event.";
            this.buttonUnknownKey.Click += new System.EventHandler(this.buttonUnknownKey_Click);
            this.buttonTake1000.Location = new System.Drawing.Point(1030, 4);
            this.buttonTake1000.Name = "buttonTake1000";
            this.buttonTake1000.Size = new System.Drawing.Size(90, 36);
            this.buttonTake1000.Text = "Take 1000";
            this.buttonTake1000.ToolTipText = "load with take=1000 → contract says MaxTake=100 → 400 → error event.";
            this.buttonTake1000.Click += new System.EventHandler(this.buttonTake1000_Click);
            //
            // buttonClear
            //
            this.buttonClear.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.buttonClear.Location = new System.Drawing.Point(1178, 4);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(110, 36);
            this.buttonClear.Text = "Clear trace";
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            //
            // MainPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.panelGrid);
            this.Controls.Add(this.panelPivot);
            this.Controls.Add(this.panelTrace);
            this.Controls.Add(this.labelAlarm);
            this.Controls.Add(this.panelActions);
            this.Name = "MainPage";
            this.Size = new System.Drawing.Size(1348, 720);
            this.Text = "IntegrationLab — Data Widgets";
            this.Load += new System.EventHandler(this.MainPage_Load);
            this.panelGrid.ResumeLayout(false);
            this.panelPivot.ResumeLayout(false);
            this.panelTrace.ResumeLayout(false);
            this.panelActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel panelGrid;
        private Wisej.Web.Label labelGridTitle;
        private Wisej.Web.Label labelStatus;
        private IntegrationLab.Widgets.WorkOrderGrid gridWorkOrders;
        private Wisej.Web.Label labelGridState;
        private Wisej.Web.Panel panelPivot;
        private Wisej.Web.Label labelPivotTitle;
        private IntegrationLab.Widgets.WorkOrderPivot pivotWorkOrders;
        private Wisej.Web.Label labelPivotState;
        private Wisej.Web.Panel panelTrace;
        private Wisej.Web.Label labelTraceTitle;
        private Wisej.Web.ListBox listTrace;
        private Wisej.Web.Label labelTraceFooter;
        private Wisej.Web.Label labelAlarm;
        private Wisej.Web.Panel panelActions;
        private Wisej.Web.Button buttonReloadGrid;
        private Wisej.Web.Button buttonNextPage;
        private Wisej.Web.Button buttonSortStatus;
        private Wisej.Web.Button buttonInsert;
        private Wisej.Web.Button buttonDelete;
        private Wisej.Web.Button buttonReloadPivot;
        private Wisej.Web.Button buttonPivotPriority;
        private Wisej.Web.Button buttonUnknownKey;
        private Wisej.Web.Button buttonTake1000;
        private Wisej.Web.Button buttonClear;
    }
}
