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
            this.panelPostback = new Wisej.Web.Panel();
            this.labelPostbackTitle = new Wisej.Web.Label();
            this.labelPostbackSub = new Wisej.Web.Label();
            this.labelPostbackStatus = new Wisej.Web.Label();
            this.gridPostback = new IntegrationLab.Widgets.GridWidget();
            this.labelPostbackInfo = new Wisej.Web.Label();
            this.labelPostbackBanner = new Wisej.Web.Label();
            this.panelLookup = new Wisej.Web.Panel();
            this.labelLookupTitle = new Wisej.Web.Label();
            this.labelLookupSub = new Wisej.Web.Label();
            this.labelLookupStatus = new Wisej.Web.Label();
            this.gridLookup = new IntegrationLab.Widgets.LookupWidget();
            this.labelLookupInfo = new Wisej.Web.Label();
            this.labelLookupBanner = new Wisej.Web.Label();
            this.panelTrace = new Wisej.Web.Panel();
            this.labelTraceTitle = new Wisej.Web.Label();
            this.listTrace = new Wisej.Web.ListBox();
            this.labelTraceFooter = new Wisej.Web.Label();
            this.panelActions = new Wisej.Web.Panel();
            this.buttonReload = new Wisej.Web.Button();
            this.buttonNextPage = new Wisej.Web.Button();
            this.buttonSortStatus = new Wisej.Web.Button();
            this.buttonInvalidAction = new Wisej.Web.Button();
            this.buttonOversized = new Wisej.Web.Button();
            this.buttonSwitchTarget = new Wisej.Web.Button();
            this.buttonShowUrl = new Wisej.Web.Button();
            this.buttonClear = new Wisej.Web.Button();
            this.panelPostback.SuspendLayout();
            this.panelLookup.SuspendLayout();
            this.panelTrace.SuspendLayout();
            this.panelActions.SuspendLayout();
            this.SuspendLayout();
            //
            // panelPostback  (card 1: URL data source)
            //
            this.panelPostback.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.panelPostback.BackColor = System.Drawing.Color.White;
            this.panelPostback.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelPostback.Controls.Add(this.labelPostbackTitle);
            this.panelPostback.Controls.Add(this.labelPostbackSub);
            this.panelPostback.Controls.Add(this.labelPostbackStatus);
            this.panelPostback.Controls.Add(this.gridPostback);
            this.panelPostback.Controls.Add(this.labelPostbackInfo);
            this.panelPostback.Controls.Add(this.labelPostbackBanner);
            this.panelPostback.Location = new System.Drawing.Point(30, 30);
            this.panelPostback.Name = "panelPostback";
            this.panelPostback.Size = new System.Drawing.Size(440, 560);
            //
            // labelPostbackTitle
            //
            this.labelPostbackTitle.AutoSize = false;
            this.labelPostbackTitle.Font = new System.Drawing.Font("default", 13F, System.Drawing.FontStyle.Bold);
            this.labelPostbackTitle.Location = new System.Drawing.Point(16, 12);
            this.labelPostbackTitle.Name = "labelPostbackTitle";
            this.labelPostbackTitle.Size = new System.Drawing.Size(260, 28);
            this.labelPostbackTitle.Text = "Postback URL data source";
            //
            // labelPostbackSub
            //
            this.labelPostbackSub.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelPostbackSub.AutoSize = false;
            this.labelPostbackSub.Font = new System.Drawing.Font("default", 9F);
            this.labelPostbackSub.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelPostbackSub.Location = new System.Drawing.Point(16, 40);
            this.labelPostbackSub.Name = "labelPostbackSub";
            this.labelPostbackSub.Size = new System.Drawing.Size(408, 20);
            this.labelPostbackSub.Text = "GridWidget · vendor GETs getPostbackUrl() + \"&&action=load\" → WebRequest";
            //
            // labelPostbackStatus
            //
            this.labelPostbackStatus.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.labelPostbackStatus.AutoSize = false;
            this.labelPostbackStatus.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelPostbackStatus.ForeColor = System.Drawing.Color.FromArgb(232, 161, 60);
            this.labelPostbackStatus.Location = new System.Drawing.Point(230, 14);
            this.labelPostbackStatus.Name = "labelPostbackStatus";
            this.labelPostbackStatus.Size = new System.Drawing.Size(194, 24);
            this.labelPostbackStatus.Text = "● idle";
            this.labelPostbackStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // gridPostback  (Widget host: the vendor grid lives inside its container)
            //
            this.gridPostback.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.gridPostback.Location = new System.Drawing.Point(16, 66);
            this.gridPostback.Name = "gridPostback";
            this.gridPostback.PageSize = 10;
            this.gridPostback.Size = new System.Drawing.Size(408, 386);
            //
            // labelPostbackInfo  (last request line)
            //
            this.labelPostbackInfo.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelPostbackInfo.AutoSize = false;
            this.labelPostbackInfo.Font = new System.Drawing.Font("monospace", 8F);
            this.labelPostbackInfo.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelPostbackInfo.Location = new System.Drawing.Point(16, 458);
            this.labelPostbackInfo.Name = "labelPostbackInfo";
            this.labelPostbackInfo.Size = new System.Drawing.Size(408, 40);
            this.labelPostbackInfo.Text = "";
            this.labelPostbackInfo.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // labelPostbackBanner
            //
            this.labelPostbackBanner.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelPostbackBanner.AutoSize = false;
            this.labelPostbackBanner.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
            this.labelPostbackBanner.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelPostbackBanner.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            this.labelPostbackBanner.Location = new System.Drawing.Point(16, 502);
            this.labelPostbackBanner.Name = "labelPostbackBanner";
            this.labelPostbackBanner.Padding = new Wisej.Web.Padding(10, 0, 10, 0);
            this.labelPostbackBanner.Size = new System.Drawing.Size(408, 44);
            this.labelPostbackBanner.Text = "";
            this.labelPostbackBanner.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelPostbackBanner.Visible = false;
            //
            // panelLookup  (card 2: RPC callback)
            //
            this.panelLookup.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.panelLookup.BackColor = System.Drawing.Color.White;
            this.panelLookup.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelLookup.Controls.Add(this.labelLookupTitle);
            this.panelLookup.Controls.Add(this.labelLookupSub);
            this.panelLookup.Controls.Add(this.labelLookupStatus);
            this.panelLookup.Controls.Add(this.gridLookup);
            this.panelLookup.Controls.Add(this.labelLookupInfo);
            this.panelLookup.Controls.Add(this.labelLookupBanner);
            this.panelLookup.Location = new System.Drawing.Point(486, 30);
            this.panelLookup.Name = "panelLookup";
            this.panelLookup.Size = new System.Drawing.Size(440, 560);
            //
            // labelLookupTitle
            //
            this.labelLookupTitle.AutoSize = false;
            this.labelLookupTitle.Font = new System.Drawing.Font("default", 13F, System.Drawing.FontStyle.Bold);
            this.labelLookupTitle.Location = new System.Drawing.Point(16, 12);
            this.labelLookupTitle.Name = "labelLookupTitle";
            this.labelLookupTitle.Size = new System.Drawing.Size(260, 28);
            this.labelLookupTitle.Text = "WebMethod RPC callback";
            //
            // labelLookupSub
            //
            this.labelLookupSub.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelLookupSub.AutoSize = false;
            this.labelLookupSub.Font = new System.Drawing.Font("default", 9F);
            this.labelLookupSub.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelLookupSub.Location = new System.Drawing.Point(16, 40);
            this.labelLookupSub.Name = "labelLookupSub";
            this.labelLookupSub.Size = new System.Drawing.Size(408, 20);
            this.labelLookupSub.Text = "LookupWidget · load() awaits App.MainPage.GetWorkOrders(page, size, sort, desc)";
            //
            // labelLookupStatus
            //
            this.labelLookupStatus.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.labelLookupStatus.AutoSize = false;
            this.labelLookupStatus.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelLookupStatus.ForeColor = System.Drawing.Color.FromArgb(232, 161, 60);
            this.labelLookupStatus.Location = new System.Drawing.Point(230, 14);
            this.labelLookupStatus.Name = "labelLookupStatus";
            this.labelLookupStatus.Size = new System.Drawing.Size(194, 24);
            this.labelLookupStatus.Text = "● idle";
            this.labelLookupStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // gridLookup
            //
            this.gridLookup.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.gridLookup.DataSourceMode = "page";
            this.gridLookup.Location = new System.Drawing.Point(16, 66);
            this.gridLookup.Name = "gridLookup";
            this.gridLookup.PageSize = 10;
            this.gridLookup.Size = new System.Drawing.Size(408, 386);
            //
            // labelLookupInfo  (last call line)
            //
            this.labelLookupInfo.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelLookupInfo.AutoSize = false;
            this.labelLookupInfo.Font = new System.Drawing.Font("monospace", 8F);
            this.labelLookupInfo.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelLookupInfo.Location = new System.Drawing.Point(16, 458);
            this.labelLookupInfo.Name = "labelLookupInfo";
            this.labelLookupInfo.Size = new System.Drawing.Size(408, 40);
            this.labelLookupInfo.Text = "";
            this.labelLookupInfo.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // labelLookupBanner
            //
            this.labelLookupBanner.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelLookupBanner.AutoSize = false;
            this.labelLookupBanner.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
            this.labelLookupBanner.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelLookupBanner.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            this.labelLookupBanner.Location = new System.Drawing.Point(16, 502);
            this.labelLookupBanner.Name = "labelLookupBanner";
            this.labelLookupBanner.Padding = new Wisej.Web.Padding(10, 0, 10, 0);
            this.labelLookupBanner.Size = new System.Drawing.Size(408, 44);
            this.labelLookupBanner.Text = "";
            this.labelLookupBanner.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelLookupBanner.Visible = false;
            //
            // panelTrace  (Server ⇄ Client live message trace)
            //
            this.panelTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelTrace.BackColor = System.Drawing.Color.White;
            this.panelTrace.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelTrace.Controls.Add(this.labelTraceTitle);
            this.panelTrace.Controls.Add(this.listTrace);
            this.panelTrace.Controls.Add(this.labelTraceFooter);
            this.panelTrace.Location = new System.Drawing.Point(942, 30);
            this.panelTrace.Name = "panelTrace";
            this.panelTrace.Size = new System.Drawing.Size(376, 560);
            //
            // labelTraceTitle
            //
            this.labelTraceTitle.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelTraceTitle.AutoSize = false;
            this.labelTraceTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelTraceTitle.Location = new System.Drawing.Point(16, 14);
            this.labelTraceTitle.Name = "labelTraceTitle";
            this.labelTraceTitle.Size = new System.Drawing.Size(344, 30);
            this.labelTraceTitle.Text = "Server ⇄ Client  ·  live message trace";
            //
            // listTrace
            //
            this.listTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.listTrace.Font = new System.Drawing.Font("monospace", 9F);
            this.listTrace.Location = new System.Drawing.Point(16, 52);
            this.listTrace.Name = "listTrace";
            this.listTrace.Size = new System.Drawing.Size(344, 460);
            //
            // labelTraceFooter
            //
            this.labelTraceFooter.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelTraceFooter.AutoSize = false;
            this.labelTraceFooter.Font = new System.Drawing.Font("default", 9F);
            this.labelTraceFooter.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelTraceFooter.Location = new System.Drawing.Point(16, 518);
            this.labelTraceFooter.Name = "labelTraceFooter";
            this.labelTraceFooter.Size = new System.Drawing.Size(344, 30);
            this.labelTraceFooter.Text = "→ .NET→JS   ← JS→.NET   ⇄ HTTP (postback request/response)   • server";
            //
            // panelActions
            //
            this.panelActions.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelActions.Controls.Add(this.buttonReload);
            this.panelActions.Controls.Add(this.buttonNextPage);
            this.panelActions.Controls.Add(this.buttonSortStatus);
            this.panelActions.Controls.Add(this.buttonInvalidAction);
            this.panelActions.Controls.Add(this.buttonOversized);
            this.panelActions.Controls.Add(this.buttonSwitchTarget);
            this.panelActions.Controls.Add(this.buttonShowUrl);
            this.panelActions.Controls.Add(this.buttonClear);
            this.panelActions.Location = new System.Drawing.Point(30, 606);
            this.panelActions.Name = "panelActions";
            this.panelActions.Size = new System.Drawing.Size(1288, 44);
            //
            // success + progress path
            //
            this.buttonReload.Location = new System.Drawing.Point(0, 4);
            this.buttonReload.Name = "buttonReload";
            this.buttonReload.Size = new System.Drawing.Size(110, 36);
            this.buttonReload.Text = "Reload both";
            this.buttonReload.ToolTipText = "Both grids back to action=load, page size 10, page 1 (also the recovery step).";
            this.buttonReload.Click += new System.EventHandler(this.buttonReload_Click);
            this.buttonNextPage.Location = new System.Drawing.Point(118, 4);
            this.buttonNextPage.Name = "buttonNextPage";
            this.buttonNextPage.Size = new System.Drawing.Size(100, 36);
            this.buttonNextPage.Text = "Next page";
            this.buttonNextPage.ToolTipText = "Call(\"setPage\", n) on both grids; wraps to page 1 after the last page.";
            this.buttonNextPage.Click += new System.EventHandler(this.buttonNextPage_Click);
            this.buttonSortStatus.Location = new System.Drawing.Point(226, 4);
            this.buttonSortStatus.Name = "buttonSortStatus";
            this.buttonSortStatus.Size = new System.Drawing.Size(120, 36);
            this.buttonSortStatus.Text = "Sort by status";
            this.buttonSortStatus.ToolTipText = "Call(\"sort\", \"status\") on both grids; a second click toggles the direction.";
            this.buttonSortStatus.Click += new System.EventHandler(this.buttonSortStatus_Click);
            //
            // failure paths
            //
            this.buttonInvalidAction.Location = new System.Drawing.Point(370, 4);
            this.buttonInvalidAction.Name = "buttonInvalidAction";
            this.buttonInvalidAction.Size = new System.Drawing.Size(120, 36);
            this.buttonInvalidAction.Text = "Invalid action";
            this.buttonInvalidAction.ToolTipText = "Postback grid GETs …&&action=delete: the handler answers 400 + {\"error\"} and the grid shows a red row.";
            this.buttonInvalidAction.Click += new System.EventHandler(this.buttonInvalidAction_Click);
            this.buttonOversized.Location = new System.Drawing.Point(498, 4);
            this.buttonOversized.Name = "buttonOversized";
            this.buttonOversized.Size = new System.Drawing.Size(130, 36);
            this.buttonOversized.Text = "Oversized page";
            this.buttonOversized.ToolTipText = "size=1000 on both: postback → 400; WebMethod → ArgumentException (Wisej popup, Promise resolves null).";
            this.buttonOversized.Click += new System.EventHandler(this.buttonOversized_Click);
            //
            // WebMethod target + postback URL
            //
            this.buttonSwitchTarget.Location = new System.Drawing.Point(652, 4);
            this.buttonSwitchTarget.Name = "buttonSwitchTarget";
            this.buttonSwitchTarget.Size = new System.Drawing.Size(330, 36);
            this.buttonSwitchTarget.Text = "WebMethod target: App.MainPage ▸ switch to widget";
            this.buttonSwitchTarget.ToolTipText = "Toggles LookupWidget.DataSourceMode between \"page\" (App.MainPage.GetWorkOrders) and \"widget\" (this.GetWorkOrders via RegisterWebMethods).";
            this.buttonSwitchTarget.Click += new System.EventHandler(this.buttonSwitchTarget_Click);
            this.buttonShowUrl.Location = new System.Drawing.Point(990, 4);
            this.buttonShowUrl.Name = "buttonShowUrl";
            this.buttonShowUrl.Size = new System.Drawing.Size(150, 36);
            this.buttonShowUrl.Text = "Show postback URL";
            this.buttonShowUrl.ToolTipText = "Traces GetPostbackURL() (middle redacted) and the request counters.";
            this.buttonShowUrl.Click += new System.EventHandler(this.buttonShowUrl_Click);
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
            this.Controls.Add(this.panelPostback);
            this.Controls.Add(this.panelLookup);
            this.Controls.Add(this.panelTrace);
            this.Controls.Add(this.panelActions);
            this.Name = "MainPage";
            this.Size = new System.Drawing.Size(1348, 680);
            this.Text = "IntegrationLab — Work Orders";
            this.Load += new System.EventHandler(this.MainPage_Load);
            this.panelPostback.ResumeLayout(false);
            this.panelLookup.ResumeLayout(false);
            this.panelTrace.ResumeLayout(false);
            this.panelActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel panelPostback;
        private Wisej.Web.Label labelPostbackTitle;
        private Wisej.Web.Label labelPostbackSub;
        private Wisej.Web.Label labelPostbackStatus;
        private IntegrationLab.Widgets.GridWidget gridPostback;
        private Wisej.Web.Label labelPostbackInfo;
        private Wisej.Web.Label labelPostbackBanner;
        private Wisej.Web.Panel panelLookup;
        private Wisej.Web.Label labelLookupTitle;
        private Wisej.Web.Label labelLookupSub;
        private Wisej.Web.Label labelLookupStatus;
        private IntegrationLab.Widgets.LookupWidget gridLookup;
        private Wisej.Web.Label labelLookupInfo;
        private Wisej.Web.Label labelLookupBanner;
        private Wisej.Web.Panel panelTrace;
        private Wisej.Web.Label labelTraceTitle;
        private Wisej.Web.ListBox listTrace;
        private Wisej.Web.Label labelTraceFooter;
        private Wisej.Web.Panel panelActions;
        private Wisej.Web.Button buttonReload;
        private Wisej.Web.Button buttonNextPage;
        private Wisej.Web.Button buttonSortStatus;
        private Wisej.Web.Button buttonInvalidAction;
        private Wisej.Web.Button buttonOversized;
        private Wisej.Web.Button buttonSwitchTarget;
        private Wisej.Web.Button buttonShowUrl;
        private Wisej.Web.Button buttonClear;
    }
}
