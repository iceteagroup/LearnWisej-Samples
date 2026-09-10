namespace EnterpriseOps.UI
{
    partial class WorkOrderHistoryPage
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
            this.pnlHeader = new Wisej.Web.Panel();
            this.lblTitle = new Wisej.Web.Label();
            this.lblTenant = new Wisej.Web.Label();
            this.lblUser = new Wisej.Web.Label();
            this.lblCorrelation = new Wisej.Web.Label();
            this.pnlHistory = new Wisej.Web.Panel();
            this.lblCardTitle = new Wisej.Web.Label();
            this.lblStatus = new Wisej.Web.Label();
            this.cboWorkOrder = new Wisej.Web.ComboBox();
            this.btnReload = new Wisej.Web.Button();
            this.btnSampleMode = new Wisej.Web.Button();
            this.btnResourcePackage = new Wisej.Web.Button();
            this.statusTimeline = new EnterpriseOps.Controls.StatusTimeline();
            this.chartWorkOrders = new EnterpriseOps.Widgets.WorkOrderChartWidget();
            this.lblSegmentTitle = new Wisej.Web.Label();
            this.dgvSegment = new Wisej.Web.DataGridView();
            this.colId = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colTitle = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colCustomer = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colPriority = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colAssignedTo = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colDue = new Wisej.Web.DataGridViewTextBoxColumn();
            this.lblBanner = new Wisej.Web.Label();
            this.lblStatusBar = new Wisej.Web.Label();
            this.pnlTrace = new Wisej.Web.Panel();
            this.lblTraceTitle = new Wisej.Web.Label();
            this.lstTrace = new Wisej.Web.ListBox();
            this.lblTraceFooter = new Wisej.Web.Label();
            this.pnlActions = new Wisej.Web.Panel();
            this.btnBadOption = new Wisej.Web.Button();
            this.btnForgedKey = new Wisej.Web.Button();
            this.btnVendorError = new Wisej.Web.Button();
            this.btnBlockVendor = new Wisej.Web.Button();
            this.btnRestoreVendor = new Wisej.Web.Button();
            this.btnComponentGate = new Wisej.Web.Button();
            this.btnClearTrace = new Wisej.Web.Button();
            this.pnlHeader.SuspendLayout();
            this.pnlHistory.SuspendLayout();
            this.pnlTrace.SuspendLayout();
            this.pnlActions.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlHeader  (slim header bar: screen name · tenant · user · correlation id)
            //
            this.pnlHeader.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(21, 101, 216);
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Controls.Add(this.lblTenant);
            this.pnlHeader.Controls.Add(this.lblUser);
            this.pnlHeader.Controls.Add(this.lblCorrelation);
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1348, 44);
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = false;
            this.lblTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(24, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(460, 44);
            this.lblTitle.Text = "EnterpriseOps — Work order history";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblTenant
            //
            this.lblTenant.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblTenant.AutoSize = false;
            this.lblTenant.Font = new System.Drawing.Font("default", 9F);
            this.lblTenant.ForeColor = System.Drawing.Color.FromArgb(214, 228, 243);
            this.lblTenant.Location = new System.Drawing.Point(700, 0);
            this.lblTenant.Name = "lblTenant";
            this.lblTenant.Size = new System.Drawing.Size(160, 44);
            this.lblTenant.Text = "tenant: fabrikam";
            this.lblTenant.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblUser
            //
            this.lblUser.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblUser.AutoSize = false;
            this.lblUser.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblUser.ForeColor = System.Drawing.Color.White;
            this.lblUser.Location = new System.Drawing.Point(870, 0);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(280, 44);
            this.lblUser.Text = "Signed in: ana.ops · Manager";
            this.lblUser.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblCorrelation
            //
            this.lblCorrelation.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblCorrelation.AutoSize = false;
            this.lblCorrelation.Font = new System.Drawing.Font("monospace", 9F);
            this.lblCorrelation.ForeColor = System.Drawing.Color.FromArgb(214, 228, 243);
            this.lblCorrelation.Location = new System.Drawing.Point(1160, 0);
            this.lblCorrelation.Name = "lblCorrelation";
            this.lblCorrelation.Size = new System.Drawing.Size(164, 44);
            this.lblCorrelation.Text = "corr —";
            this.lblCorrelation.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // pnlHistory  (the usage example screen: two reusable components, a picker, a result grid)
            //
            this.pnlHistory.BackColor = System.Drawing.Color.White;
            this.pnlHistory.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlHistory.Controls.Add(this.lblCardTitle);
            this.pnlHistory.Controls.Add(this.lblStatus);
            this.pnlHistory.Controls.Add(this.cboWorkOrder);
            this.pnlHistory.Controls.Add(this.btnReload);
            this.pnlHistory.Controls.Add(this.btnSampleMode);
            this.pnlHistory.Controls.Add(this.btnResourcePackage);
            this.pnlHistory.Controls.Add(this.statusTimeline);
            this.pnlHistory.Controls.Add(this.chartWorkOrders);
            this.pnlHistory.Controls.Add(this.lblSegmentTitle);
            this.pnlHistory.Controls.Add(this.dgvSegment);
            this.pnlHistory.Controls.Add(this.lblBanner);
            this.pnlHistory.Controls.Add(this.lblStatusBar);
            this.pnlHistory.Location = new System.Drawing.Point(24, 64);
            this.pnlHistory.Name = "pnlHistory";
            this.pnlHistory.Size = new System.Drawing.Size(900, 506);
            //
            // lblCardTitle
            //
            this.lblCardTitle.AutoSize = false;
            this.lblCardTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblCardTitle.ForeColor = System.Drawing.Color.FromArgb(31, 45, 58);
            this.lblCardTitle.Location = new System.Drawing.Point(20, 12);
            this.lblCardTitle.Name = "lblCardTitle";
            this.lblCardTitle.Size = new System.Drawing.Size(420, 24);
            this.lblCardTitle.Text = "Work order 2002 — history";
            this.lblCardTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = false;
            this.lblStatus.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.lblStatus.Location = new System.Drawing.Point(450, 12);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(430, 24);
            this.lblStatus.Text = "● ready";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // cboWorkOrder  (the tenant's work orders, plus one that belongs to another tenant)
            //
            this.cboWorkOrder.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboWorkOrder.Location = new System.Drawing.Point(20, 44);
            this.cboWorkOrder.Name = "cboWorkOrder";
            this.cboWorkOrder.Size = new System.Drawing.Size(280, 30);
            this.cboWorkOrder.ToolTipText = "The last entry belongs to another tenant: AccessPolicy refuses it on the server.";
            this.cboWorkOrder.SelectedIndexChanged += new System.EventHandler(this.cboWorkOrder_SelectedIndexChanged);
            //
            // btnReload  (the success path)
            //
            this.btnReload.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.btnReload.Location = new System.Drawing.Point(308, 44);
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(110, 30);
            this.btnReload.Text = "⟳ Reload";
            this.btnReload.ToolTipText = "btnReload_Click → await _service.GetHistoryAsync(...) → statusTimeline.SetItems(...) and chartWorkOrders.SetSegments(...).";
            this.btnReload.Click += new System.EventHandler(this.btnReload_Click);
            //
            // btnSampleMode  (what the Designer shows, at run time)
            //
            this.btnSampleMode.Location = new System.Drawing.Point(426, 44);
            this.btnSampleMode.Name = "btnSampleMode";
            this.btnSampleMode.Size = new System.Drawing.Size(190, 30);
            this.btnSampleMode.Text = "Design-time sample mode";
            this.btnSampleMode.ToolTipText = "Turns SampleMode on for both components — the same rendering the Wisej Designer shows, with no service call.";
            this.btnSampleMode.Click += new System.EventHandler(this.btnSampleMode_Click);
            //
            // btnResourcePackage  (the embedded resource package manifest)
            //
            this.btnResourcePackage.Location = new System.Drawing.Point(624, 44);
            this.btnResourcePackage.Name = "btnResourcePackage";
            this.btnResourcePackage.Size = new System.Drawing.Size(180, 30);
            this.btnResourcePackage.Text = "Resource package";
            this.btnResourcePackage.ToolTipText = "Lists what the component library embeds, in load order, and verifies the served copies against the embedded ones.";
            this.btnResourcePackage.Click += new System.EventHandler(this.btnResourcePackage_Click);
            //
            // statusTimeline  (the reusable UserControl — dropped from the Toolbox, configured by properties)
            //
            this.statusTimeline.Caption = "STATUS TIMELINE";
            this.statusTimeline.EmptyText = "No history to show.";
            this.statusTimeline.Location = new System.Drawing.Point(20, 88);
            this.statusTimeline.Name = "statusTimeline";
            this.statusTimeline.Size = new System.Drawing.Size(420, 210);
            this.statusTimeline.TimeFormat = "MMM dd, HH:mm";
            this.statusTimeline.ToolTipText = "EnterpriseOps.Controls.StatusTimeline — Items, SelectedItem, ItemSelected, SampleMode.";
            this.statusTimeline.ItemSelected += new System.EventHandler<EnterpriseOps.Controls.TimelineItemEventArgs>(this.statusTimeline_ItemSelected);
            //
            // chartWorkOrders  (the wrapped vendor chart — the screen never sees JavaScript)
            //
            this.chartWorkOrders.Caption = "WORK-ORDER HISTORY";
            this.chartWorkOrders.Location = new System.Drawing.Point(456, 88);
            this.chartWorkOrders.Name = "chartWorkOrders";
            this.chartWorkOrders.ShowLegend = true;
            this.chartWorkOrders.Size = new System.Drawing.Size(420, 210);
            this.chartWorkOrders.ToolTipText = "EnterpriseOps.Widgets.WorkOrderChartWidget — SetSegments, Palette, SegmentClicked, WidgetError.";
            this.chartWorkOrders.SegmentClicked += new System.EventHandler<EnterpriseOps.Widgets.ChartSegmentEventArgs>(this.chartWorkOrders_SegmentClicked);
            this.chartWorkOrders.WidgetError += new System.EventHandler<EnterpriseOps.Widgets.WidgetErrorEventArgs>(this.chartWorkOrders_WidgetError);
            //
            // lblSegmentTitle
            //
            this.lblSegmentTitle.AutoSize = false;
            this.lblSegmentTitle.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.lblSegmentTitle.ForeColor = System.Drawing.Color.FromArgb(74, 90, 106);
            this.lblSegmentTitle.Location = new System.Drawing.Point(20, 306);
            this.lblSegmentTitle.Name = "lblSegmentTitle";
            this.lblSegmentTitle.Size = new System.Drawing.Size(860, 20);
            this.lblSegmentTitle.Text = "CLICK A SLICE — THE SERVER RE-VALIDATES THE KEY AND FILTERS HERE";
            this.lblSegmentTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // dgvSegment  (bound to WorkQueueRow — the projection, never the entity)
            //
            this.dgvSegment.AllowUserToAddRows = false;
            this.dgvSegment.AllowUserToDeleteRows = false;
            this.dgvSegment.AutoGenerateColumns = false;
            this.dgvSegment.AutoSizeColumnsMode = Wisej.Web.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvSegment.BackColor = System.Drawing.Color.White;
            this.dgvSegment.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
            this.colId,
            this.colTitle,
            this.colCustomer,
            this.colStatus,
            this.colPriority,
            this.colAssignedTo,
            this.colDue});
            this.dgvSegment.Location = new System.Drawing.Point(20, 330);
            this.dgvSegment.MultiSelect = false;
            this.dgvSegment.Name = "dgvSegment";
            this.dgvSegment.ReadOnly = true;
            this.dgvSegment.RowHeadersVisible = false;
            this.dgvSegment.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSegment.Size = new System.Drawing.Size(860, 104);
            //
            // colId
            //
            this.colId.DataPropertyName = "Id";
            this.colId.FillWeight = 9F;
            this.colId.HeaderText = "Id";
            this.colId.Name = "colId";
            this.colId.ReadOnly = true;
            //
            // colTitle
            //
            this.colTitle.DataPropertyName = "Title";
            this.colTitle.FillWeight = 26F;
            this.colTitle.HeaderText = "Work order";
            this.colTitle.Name = "colTitle";
            this.colTitle.ReadOnly = true;
            //
            // colCustomer
            //
            this.colCustomer.DataPropertyName = "Customer";
            this.colCustomer.FillWeight = 19F;
            this.colCustomer.HeaderText = "Customer";
            this.colCustomer.Name = "colCustomer";
            this.colCustomer.ReadOnly = true;
            //
            // colStatus
            //
            this.colStatus.DataPropertyName = "Status";
            this.colStatus.FillWeight = 13F;
            this.colStatus.HeaderText = "Status";
            this.colStatus.Name = "colStatus";
            this.colStatus.ReadOnly = true;
            //
            // colPriority
            //
            this.colPriority.DataPropertyName = "Priority";
            this.colPriority.FillWeight = 11F;
            this.colPriority.HeaderText = "Priority";
            this.colPriority.Name = "colPriority";
            this.colPriority.ReadOnly = true;
            //
            // colAssignedTo
            //
            this.colAssignedTo.DataPropertyName = "AssignedTo";
            this.colAssignedTo.FillWeight = 12F;
            this.colAssignedTo.HeaderText = "Assigned to";
            this.colAssignedTo.Name = "colAssignedTo";
            this.colAssignedTo.ReadOnly = true;
            //
            // colDue
            //
            this.colDue.DataPropertyName = "Due";
            this.colDue.FillWeight = 10F;
            this.colDue.HeaderText = "Due";
            this.colDue.Name = "colDue";
            this.colDue.ReadOnly = true;
            //
            // lblBanner  (failure banner; hidden until something needs saying)
            //
            this.lblBanner.AutoSize = false;
            this.lblBanner.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
            this.lblBanner.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblBanner.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            this.lblBanner.Location = new System.Drawing.Point(20, 440);
            this.lblBanner.Name = "lblBanner";
            this.lblBanner.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.lblBanner.Size = new System.Drawing.Size(860, 26);
            this.lblBanner.Text = "";
            this.lblBanner.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblBanner.Visible = false;
            //
            // lblStatusBar  (the walkthrough's dark footer line)
            //
            this.lblStatusBar.AutoSize = false;
            this.lblStatusBar.BackColor = System.Drawing.Color.FromArgb(15, 36, 64);
            this.lblStatusBar.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.lblStatusBar.ForeColor = System.Drawing.Color.FromArgb(159, 192, 232);
            this.lblStatusBar.Location = new System.Drawing.Point(20, 470);
            this.lblStatusBar.Name = "lblStatusBar";
            this.lblStatusBar.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.lblStatusBar.Size = new System.Drawing.Size(860, 28);
            this.lblStatusBar.Text = "Loading…";
            this.lblStatusBar.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // pnlTrace  (Server · live activity trace)
            //
            this.pnlTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlTrace.BackColor = System.Drawing.Color.White;
            this.pnlTrace.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlTrace.Controls.Add(this.lblTraceTitle);
            this.pnlTrace.Controls.Add(this.lstTrace);
            this.pnlTrace.Controls.Add(this.lblTraceFooter);
            this.pnlTrace.Location = new System.Drawing.Point(940, 64);
            this.pnlTrace.Name = "pnlTrace";
            this.pnlTrace.Size = new System.Drawing.Size(384, 506);
            //
            // lblTraceTitle
            //
            this.lblTraceTitle.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblTraceTitle.AutoSize = false;
            this.lblTraceTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblTraceTitle.Location = new System.Drawing.Point(16, 12);
            this.lblTraceTitle.Name = "lblTraceTitle";
            this.lblTraceTitle.Size = new System.Drawing.Size(352, 28);
            this.lblTraceTitle.Text = "Server · live activity trace";
            //
            // lstTrace
            //
            this.lstTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lstTrace.Font = new System.Drawing.Font("monospace", 9F);
            this.lstTrace.Location = new System.Drawing.Point(16, 46);
            this.lstTrace.Name = "lstTrace";
            this.lstTrace.Size = new System.Drawing.Size(352, 416);
            //
            // lblTraceFooter
            //
            this.lblTraceFooter.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblTraceFooter.AutoSize = false;
            this.lblTraceFooter.Font = new System.Drawing.Font("default", 8F);
            this.lblTraceFooter.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblTraceFooter.Location = new System.Drawing.Point(16, 468);
            this.lblTraceFooter.Name = "lblTraceFooter";
            this.lblTraceFooter.Size = new System.Drawing.Size(352, 28);
            this.lblTraceFooter.Text = "UI → · Component: · Client → · Service: · Data: · Security: · Package:";
            //
            // pnlActions  (bottom bar: the failure paths, the recovery, the anti-pattern gate, clear)
            //
            this.pnlActions.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlActions.Controls.Add(this.btnBadOption);
            this.pnlActions.Controls.Add(this.btnForgedKey);
            this.pnlActions.Controls.Add(this.btnVendorError);
            this.pnlActions.Controls.Add(this.btnBlockVendor);
            this.pnlActions.Controls.Add(this.btnRestoreVendor);
            this.pnlActions.Controls.Add(this.btnComponentGate);
            this.pnlActions.Controls.Add(this.btnClearTrace);
            this.pnlActions.Location = new System.Drawing.Point(24, 584);
            this.pnlActions.Name = "pnlActions";
            this.pnlActions.Size = new System.Drawing.Size(1300, 44);
            //
            // btnBadOption  (failure path 1: an option the component does not support, refused on the server)
            //
            this.btnBadOption.Location = new System.Drawing.Point(0, 4);
            this.btnBadOption.Name = "btnBadOption";
            this.btnBadOption.Size = new System.Drawing.Size(180, 36);
            this.btnBadOption.Text = "Fail: invalid palette";
            this.btnBadOption.ToolTipText = "chartWorkOrders.Palette = \"neon-pink\" → ArgumentOutOfRangeException on the server. Nothing is rendered, the chart is untouched.";
            this.btnBadOption.Click += new System.EventHandler(this.btnBadOption_Click);
            //
            // btnForgedKey  (failure path 2: a segment key the browser could have forged)
            //
            this.btnForgedKey.Location = new System.Drawing.Point(188, 4);
            this.btnForgedKey.Name = "btnForgedKey";
            this.btnForgedKey.Size = new System.Drawing.Size(180, 36);
            this.btnForgedKey.Text = "Fail: forged segment key";
            this.btnForgedKey.ToolTipText = "Calls the service with a key the chart never rendered. The service rejects it — keys from the browser are never trusted.";
            this.btnForgedKey.Click += new System.EventHandler(this.btnForgedKey_Click);
            //
            // btnVendorError  (failure path 3: the vendor library throws, reported through the contract)
            //
            this.btnVendorError.Location = new System.Drawing.Point(376, 4);
            this.btnVendorError.Name = "btnVendorError";
            this.btnVendorError.Size = new System.Drawing.Size(180, 36);
            this.btnVendorError.Text = "Fail: vendor throws";
            this.btnVendorError.ToolTipText = "The next update hands the vendor an option it rejects. The adapter catches it and raises the contract's error event.";
            this.btnVendorError.Click += new System.EventHandler(this.btnVendorError_Click);
            //
            // btnBlockVendor  (failure path 4: the walkthrough's blocked script)
            //
            this.btnBlockVendor.Location = new System.Drawing.Point(564, 4);
            this.btnBlockVendor.Name = "btnBlockVendor";
            this.btnBlockVendor.Size = new System.Drawing.Size(180, 36);
            this.btnBlockVendor.Text = "Fail: block the vendor script";
            this.btnBlockVendor.ToolTipText = "Simulates a proxy blocking Widgets/vendor-opschart.js: the adapter renders the embedded fallback and the timeline is unaffected.";
            this.btnBlockVendor.Click += new System.EventHandler(this.btnBlockVendor_Click);
            //
            // btnRestoreVendor  (the recovery)
            //
            this.btnRestoreVendor.Location = new System.Drawing.Point(752, 4);
            this.btnRestoreVendor.Name = "btnRestoreVendor";
            this.btnRestoreVendor.Size = new System.Drawing.Size(180, 36);
            this.btnRestoreVendor.Text = "Recover: reload the chart";
            this.btnRestoreVendor.ToolTipText = "Unblocks the package and re-renders the breakdown; the chart comes back with the same data and the same API.";
            this.btnRestoreVendor.Click += new System.EventHandler(this.btnRestoreVendor_Click);
            //
            // btnComponentGate  (the walkthrough's anti-pattern, measured)
            //
            this.btnComponentGate.Location = new System.Drawing.Point(940, 4);
            this.btnComponentGate.Name = "btnComponentGate";
            this.btnComponentGate.Size = new System.Drawing.Size(180, 36);
            this.btnComponentGate.Text = "Anti-pattern: leaky screen";
            this.btnComponentGate.ToolTipText = "Runs ComponentApiGate over Controls/Samples/LeakyChartScreen.cs.txt and over this screen's own source file.";
            this.btnComponentGate.Click += new System.EventHandler(this.btnComponentGate_Click);
            //
            // btnClearTrace
            //
            this.btnClearTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.btnClearTrace.Location = new System.Drawing.Point(1190, 4);
            this.btnClearTrace.Name = "btnClearTrace";
            this.btnClearTrace.Size = new System.Drawing.Size(110, 36);
            this.btnClearTrace.Text = "Clear trace";
            this.btnClearTrace.Click += new System.EventHandler(this.btnClearTrace_Click);
            //
            // WorkOrderHistoryPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.pnlHistory);
            this.Controls.Add(this.pnlTrace);
            this.Controls.Add(this.pnlActions);
            this.Name = "WorkOrderHistoryPage";
            this.Size = new System.Drawing.Size(1348, 680);
            this.Text = "EnterpriseOps — Work order history";
            this.Load += new System.EventHandler(this.WorkOrderHistoryPage_Load);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHistory.ResumeLayout(false);
            this.pnlTrace.ResumeLayout(false);
            this.pnlActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlHeader;
        private Wisej.Web.Label lblTitle;
        private Wisej.Web.Label lblTenant;
        private Wisej.Web.Label lblUser;
        private Wisej.Web.Label lblCorrelation;
        private Wisej.Web.Panel pnlHistory;
        private Wisej.Web.Label lblCardTitle;
        private Wisej.Web.Label lblStatus;
        private Wisej.Web.ComboBox cboWorkOrder;
        private Wisej.Web.Button btnReload;
        private Wisej.Web.Button btnSampleMode;
        private Wisej.Web.Button btnResourcePackage;
        private EnterpriseOps.Controls.StatusTimeline statusTimeline;
        private EnterpriseOps.Widgets.WorkOrderChartWidget chartWorkOrders;
        private Wisej.Web.Label lblSegmentTitle;
        private Wisej.Web.DataGridView dgvSegment;
        private Wisej.Web.DataGridViewTextBoxColumn colId;
        private Wisej.Web.DataGridViewTextBoxColumn colTitle;
        private Wisej.Web.DataGridViewTextBoxColumn colCustomer;
        private Wisej.Web.DataGridViewTextBoxColumn colStatus;
        private Wisej.Web.DataGridViewTextBoxColumn colPriority;
        private Wisej.Web.DataGridViewTextBoxColumn colAssignedTo;
        private Wisej.Web.DataGridViewTextBoxColumn colDue;
        private Wisej.Web.Label lblBanner;
        private Wisej.Web.Label lblStatusBar;
        private Wisej.Web.Panel pnlTrace;
        private Wisej.Web.Label lblTraceTitle;
        private Wisej.Web.ListBox lstTrace;
        private Wisej.Web.Label lblTraceFooter;
        private Wisej.Web.Panel pnlActions;
        private Wisej.Web.Button btnBadOption;
        private Wisej.Web.Button btnForgedKey;
        private Wisej.Web.Button btnVendorError;
        private Wisej.Web.Button btnBlockVendor;
        private Wisej.Web.Button btnRestoreVendor;
        private Wisej.Web.Button btnComponentGate;
        private Wisej.Web.Button btnClearTrace;
    }
}
