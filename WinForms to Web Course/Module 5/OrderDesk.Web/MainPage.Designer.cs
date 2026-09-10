namespace OrderDesk
{
    partial class MainPage
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

        #region Wisej Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.appBar = new Wisej.Web.Panel();
            this.appTitle = new Wisej.Web.Label();
            this.trace = new OrderDesk.Shared.TracePanel();
            this.ordersCard = new Wisej.Web.Panel();
            this.ordersTitle = new Wisej.Web.Label();
            this.storeLabel = new Wisej.Web.Label();
            this.tabs = new Wisej.Web.TabControl();
            this.tabNaive = new Wisej.Web.TabPage();
            this.naivePanel = new OrderDesk.Pages.NaiveOrdersPanel();
            this.tabOptimized = new Wisej.Web.TabPage();
            this.optimizedPanel = new OrderDesk.Pages.OptimizedOrdersPanel();
            this.perfCard = new Wisej.Web.Panel();
            this.perfTitle = new Wisej.Web.Label();
            this.perfModeCaption = new Wisej.Web.Label();
            this.perfMode = new Wisej.Web.Label();
            this.perfFetchedCaption = new Wisej.Web.Label();
            this.perfFetched = new Wisej.Web.Label();
            this.perfInGridCaption = new Wisej.Web.Label();
            this.perfInGrid = new Wisej.Web.Label();
            this.perfMsCaption = new Wisej.Web.Label();
            this.perfMs = new Wisej.Web.Label();
            this.perfMemoryCaption = new Wisej.Web.Label();
            this.perfMemory = new Wisej.Web.Label();
            this.perfPayloadCaption = new Wisej.Web.Label();
            this.perfPayload = new Wisej.Web.Label();
            this.perfNotes = new Wisej.Web.Label();
            this.statusLabel = new Wisej.Web.Label();
            this.bannerLabel = new Wisej.Web.Label();
            this.naiveButton = new Wisej.Web.Button();
            this.bindAllButton = new Wisej.Web.Button();
            this.optimizedButton = new Wisej.Web.Button();
            this.measureButton = new Wisej.Web.Button();
            this.editButton = new Wisej.Web.Button();
            this.saveInvalidButton = new Wisej.Web.Button();
            this.measureTimer = new Wisej.Web.Timer(this.components);
            this.appBar.SuspendLayout();
            this.ordersCard.SuspendLayout();
            this.tabs.SuspendLayout();
            this.tabNaive.SuspendLayout();
            this.tabOptimized.SuspendLayout();
            this.perfCard.SuspendLayout();
            this.SuspendLayout();
            //
            // appBar
            //
            this.appBar.BackColor = OrderDesk.Shared.Palette.Accent;
            this.appBar.Controls.Add(this.appTitle);
            this.appBar.Dock = Wisej.Web.DockStyle.Top;
            this.appBar.Name = "appBar";
            this.appBar.Size = new System.Drawing.Size(1400, 44);
            //
            // appTitle
            //
            this.appTitle.AutoSize = false;
            this.appTitle.Dock = Wisej.Web.DockStyle.Fill;
            this.appTitle.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.appTitle.ForeColor = System.Drawing.Color.White;
            this.appTitle.Name = "appTitle";
            this.appTitle.Padding = new Wisej.Web.Padding(20, 0, 0, 0);
            this.appTitle.Text = "OrderDesk — Module 5 · DataGridView, Validation & Performance";
            this.appTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // trace
            //
            this.trace.Dock = Wisej.Web.DockStyle.Right;
            this.trace.Name = "trace";
            this.trace.Size = new System.Drawing.Size(560, 716);
            this.trace.Title = "Server ⇄ Client  ·  migration trace";
            //
            // ordersCard
            //
            this.ordersCard.BackColor = OrderDesk.Shared.Palette.CardBackground;
            this.ordersCard.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.ordersCard.Controls.Add(this.ordersTitle);
            this.ordersCard.Controls.Add(this.storeLabel);
            this.ordersCard.Controls.Add(this.tabs);
            this.ordersCard.Location = new System.Drawing.Point(16, 56);
            this.ordersCard.Name = "ordersCard";
            this.ordersCard.Size = new System.Drawing.Size(548, 468);
            //
            // ordersTitle
            //
            this.ordersTitle.AutoSize = false;
            this.ordersTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.ordersTitle.Location = new System.Drawing.Point(12, 8);
            this.ordersTitle.Name = "ordersTitle";
            this.ordersTitle.Size = new System.Drawing.Size(230, 28);
            this.ordersTitle.Text = "Orders — the OrdersForm grid, ported";
            this.ordersTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // storeLabel
            //
            this.storeLabel.AutoSize = false;
            this.storeLabel.Font = new System.Drawing.Font("default", 9F);
            this.storeLabel.ForeColor = OrderDesk.Shared.Palette.Warn;
            this.storeLabel.Location = new System.Drawing.Point(246, 8);
            this.storeLabel.Name = "storeLabel";
            this.storeLabel.Size = new System.Drawing.Size(290, 28);
            this.storeLabel.Text = "● seeding 200,000 orders in the background…";
            this.storeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // tabs
            //
            this.tabs.Location = new System.Drawing.Point(12, 44);
            this.tabs.Name = "tabs";
            this.tabs.Size = new System.Drawing.Size(524, 412);
            this.tabs.TabPages.Add(this.tabNaive);
            this.tabs.TabPages.Add(this.tabOptimized);
            this.tabs.SelectedIndexChanged += new System.EventHandler(this.tabs_SelectedIndexChanged);
            //
            // tabNaive
            //
            this.tabNaive.Controls.Add(this.naivePanel);
            this.tabNaive.Name = "tabNaive";
            this.tabNaive.Text = "Naive port ✕  (DataSource = GetAll)";
            //
            // naivePanel
            //
            this.naivePanel.Dock = Wisej.Web.DockStyle.Fill;
            this.naivePanel.Name = "naivePanel";
            this.naivePanel.EditRequested += new System.Action<OrderDesk.Domain.Order>(this.naivePanel_EditRequested);
            //
            // tabOptimized
            //
            this.tabOptimized.Controls.Add(this.optimizedPanel);
            this.tabOptimized.Name = "tabOptimized";
            this.tabOptimized.Text = "Optimized ✓  (filter + VirtualMode)";
            //
            // optimizedPanel
            //
            this.optimizedPanel.Dock = Wisej.Web.DockStyle.Fill;
            this.optimizedPanel.Name = "optimizedPanel";
            this.optimizedPanel.ApplyRequested += new System.EventHandler(this.optimizedPanel_ApplyRequested);
            this.optimizedPanel.EditRequested += new System.Action<OrderDesk.Domain.Order>(this.optimizedPanel_EditRequested);
            //
            // perfCard
            //
            this.perfCard.BackColor = OrderDesk.Shared.Palette.CardBackground;
            this.perfCard.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.perfCard.Controls.Add(this.perfTitle);
            this.perfCard.Controls.Add(this.perfModeCaption);
            this.perfCard.Controls.Add(this.perfMode);
            this.perfCard.Controls.Add(this.perfFetchedCaption);
            this.perfCard.Controls.Add(this.perfFetched);
            this.perfCard.Controls.Add(this.perfInGridCaption);
            this.perfCard.Controls.Add(this.perfInGrid);
            this.perfCard.Controls.Add(this.perfMsCaption);
            this.perfCard.Controls.Add(this.perfMs);
            this.perfCard.Controls.Add(this.perfMemoryCaption);
            this.perfCard.Controls.Add(this.perfMemory);
            this.perfCard.Controls.Add(this.perfPayloadCaption);
            this.perfCard.Controls.Add(this.perfPayload);
            this.perfCard.Controls.Add(this.perfNotes);
            this.perfCard.Location = new System.Drawing.Point(576, 56);
            this.perfCard.Name = "perfCard";
            this.perfCard.Size = new System.Drawing.Size(232, 468);
            //
            // perfTitle
            //
            this.perfTitle.AutoSize = false;
            this.perfTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.perfTitle.Location = new System.Drawing.Point(14, 8);
            this.perfTitle.Name = "perfTitle";
            this.perfTitle.Size = new System.Drawing.Size(204, 28);
            this.perfTitle.Text = "Performance";
            this.perfTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // metric captions + values
            //
            StyleCaption(this.perfModeCaption, "perfModeCaption", "MODE", 46);
            StyleValue(this.perfMode, "perfMode", "—", 64);
            StyleCaption(this.perfFetchedCaption, "perfFetchedCaption", "ROWS FETCHED FROM STORE", 106);
            StyleValue(this.perfFetched, "perfFetched", "—", 124);
            StyleCaption(this.perfInGridCaption, "perfInGridCaption", "ROWS IN GRID (SERVER)", 166);
            StyleValue(this.perfInGrid, "perfInGrid", "—", 184);
            StyleCaption(this.perfMsCaption, "perfMsCaption", "SERVER TIME (STOPWATCH)", 226);
            StyleValue(this.perfMs, "perfMs", "—", 244);
            StyleCaption(this.perfMemoryCaption, "perfMemoryCaption", "Δ HEAP", 286);
            StyleValue(this.perfMemory, "perfMemory", "—", 304);
            StyleCaption(this.perfPayloadCaption, "perfPayloadCaption", "EST. PAYLOAD IF EVERY ROW SHIPS", 346);
            StyleValue(this.perfPayload, "perfPayload", "—", 364);
            //
            // perfNotes
            //
            this.perfNotes.AutoSize = false;
            this.perfNotes.Font = new System.Drawing.Font("default", 8F);
            this.perfNotes.ForeColor = OrderDesk.Shared.Palette.MutedText;
            this.perfNotes.Location = new System.Drawing.Point(14, 404);
            this.perfNotes.Name = "perfNotes";
            this.perfNotes.Size = new System.Drawing.Size(204, 56);
            this.perfNotes.Text = "Measure runs naive 20k and optimized 5× each and prints the table in the trace. Payload = rows × 5 cols × 96 B + 14 KB (estimate).";
            this.perfNotes.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // statusLabel
            //
            this.statusLabel.AutoSize = false;
            this.statusLabel.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.statusLabel.ForeColor = OrderDesk.Shared.Palette.Warn;
            this.statusLabel.Location = new System.Drawing.Point(16, 532);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(792, 26);
            this.statusLabel.Text = "● working — seeding the 200,000-order store";
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // bannerLabel
            //
            this.bannerLabel.AutoSize = false;
            this.bannerLabel.BackColor = OrderDesk.Shared.Palette.WarnSoft;
            this.bannerLabel.Font = new System.Drawing.Font("default", 9F);
            this.bannerLabel.ForeColor = OrderDesk.Shared.Palette.Ink;
            this.bannerLabel.Location = new System.Drawing.Point(16, 562);
            this.bannerLabel.Name = "bannerLabel";
            this.bannerLabel.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.bannerLabel.Size = new System.Drawing.Size(792, 40);
            this.bannerLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bannerLabel.Visible = false;
            //
            // naiveButton
            //
            this.naiveButton.Enabled = false;
            this.naiveButton.Location = new System.Drawing.Point(16, 612);
            this.naiveButton.Name = "naiveButton";
            this.naiveButton.Size = new System.Drawing.Size(168, 36);
            this.naiveButton.Text = "Naive load (20k) ✕";
            this.naiveButton.ToolTipText = "The WinForms way: GetAll() then DataSource = list — capped at 20,000 rows";
            this.naiveButton.Click += new System.EventHandler(this.naiveButton_Click);
            //
            // bindAllButton
            //
            this.bindAllButton.Enabled = false;
            this.bindAllButton.Location = new System.Drawing.Point(192, 612);
            this.bindAllButton.Name = "bindAllButton";
            this.bindAllButton.Size = new System.Drawing.Size(150, 36);
            this.bindAllButton.Text = "Bind all 200k ✕";
            this.bindAllButton.ToolTipText = "Really binds all 200,000 rows — expect seconds";
            this.bindAllButton.Click += new System.EventHandler(this.bindAllButton_Click);
            //
            // optimizedButton
            //
            this.optimizedButton.Enabled = false;
            this.optimizedButton.Location = new System.Drawing.Point(350, 612);
            this.optimizedButton.Name = "optimizedButton";
            this.optimizedButton.Size = new System.Drawing.Size(160, 36);
            this.optimizedButton.Text = "Optimized load ✓";
            this.optimizedButton.ToolTipText = "Filter Open + VirtualMode + server-side query, one block at a time";
            this.optimizedButton.Click += new System.EventHandler(this.optimizedButton_Click);
            //
            // measureButton
            //
            this.measureButton.Enabled = false;
            this.measureButton.Location = new System.Drawing.Point(518, 612);
            this.measureButton.Name = "measureButton";
            this.measureButton.Size = new System.Drawing.Size(110, 36);
            this.measureButton.Text = "Measure";
            this.measureButton.ToolTipText = "Naive 20k then optimized, 5× each, with a Timer between runs";
            this.measureButton.Click += new System.EventHandler(this.measureButton_Click);
            //
            // editButton
            //
            this.editButton.Enabled = false;
            this.editButton.Location = new System.Drawing.Point(16, 656);
            this.editButton.Name = "editButton";
            this.editButton.Size = new System.Drawing.Size(210, 36);
            this.editButton.Text = "Edit selected → validate";
            this.editButton.ToolTipText = "EditOrderDialog with OrderValidator + ErrorProvider field messages";
            this.editButton.Click += new System.EventHandler(this.editButton_Click);
            //
            // saveInvalidButton
            //
            this.saveInvalidButton.Enabled = false;
            this.saveInvalidButton.Location = new System.Drawing.Point(234, 656);
            this.saveInvalidButton.Name = "saveInvalidButton";
            this.saveInvalidButton.Size = new System.Drawing.Size(150, 36);
            this.saveInvalidButton.Text = "Save invalid ✕";
            this.saveInvalidButton.ToolTipText = "Programmatic: order 1039 clone with no owner and a huge quantity → OrderService.Save throws ValidationException";
            this.saveInvalidButton.Click += new System.EventHandler(this.saveInvalidButton_Click);
            //
            // measureTimer
            //
            this.measureTimer.Interval = 700;
            this.measureTimer.Tick += new System.EventHandler(this.measureTimer_Tick);
            //
            // MainPage
            //
            this.BackColor = OrderDesk.Shared.Palette.PageBackground;
            this.Controls.Add(this.ordersCard);
            this.Controls.Add(this.perfCard);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.bannerLabel);
            this.Controls.Add(this.naiveButton);
            this.Controls.Add(this.bindAllButton);
            this.Controls.Add(this.optimizedButton);
            this.Controls.Add(this.measureButton);
            this.Controls.Add(this.editButton);
            this.Controls.Add(this.saveInvalidButton);
            this.Controls.Add(this.trace);
            this.Controls.Add(this.appBar);
            this.Name = "MainPage";
            this.Text = "OrderDesk — Module 5";
            this.Load += new System.EventHandler(this.MainPage_Load);
            this.appBar.ResumeLayout(false);
            this.ordersCard.ResumeLayout(false);
            this.tabs.ResumeLayout(false);
            this.tabNaive.ResumeLayout(false);
            this.tabOptimized.ResumeLayout(false);
            this.perfCard.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private static void StyleCaption(Wisej.Web.Label label, string name, string text, int top)
        {
            label.AutoSize = false;
            label.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            label.ForeColor = OrderDesk.Shared.Palette.MutedText;
            label.Location = new System.Drawing.Point(14, top);
            label.Name = name;
            label.Size = new System.Drawing.Size(204, 16);
            label.Text = text;
            label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        }

        private static void StyleValue(Wisej.Web.Label label, string name, string text, int top)
        {
            label.AutoSize = false;
            label.Font = new System.Drawing.Font("monospace", 13F, System.Drawing.FontStyle.Bold);
            label.ForeColor = OrderDesk.Shared.Palette.Ink;
            label.Location = new System.Drawing.Point(14, top);
            label.Name = name;
            label.Size = new System.Drawing.Size(204, 30);
            label.Text = text;
            label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        }

        #endregion

        private Wisej.Web.Panel appBar;
        private Wisej.Web.Label appTitle;
        private OrderDesk.Shared.TracePanel trace;
        private Wisej.Web.Panel ordersCard;
        private Wisej.Web.Label ordersTitle;
        private Wisej.Web.Label storeLabel;
        private Wisej.Web.TabControl tabs;
        private Wisej.Web.TabPage tabNaive;
        private OrderDesk.Pages.NaiveOrdersPanel naivePanel;
        private Wisej.Web.TabPage tabOptimized;
        private OrderDesk.Pages.OptimizedOrdersPanel optimizedPanel;
        private Wisej.Web.Panel perfCard;
        private Wisej.Web.Label perfTitle;
        private Wisej.Web.Label perfModeCaption;
        private Wisej.Web.Label perfMode;
        private Wisej.Web.Label perfFetchedCaption;
        private Wisej.Web.Label perfFetched;
        private Wisej.Web.Label perfInGridCaption;
        private Wisej.Web.Label perfInGrid;
        private Wisej.Web.Label perfMsCaption;
        private Wisej.Web.Label perfMs;
        private Wisej.Web.Label perfMemoryCaption;
        private Wisej.Web.Label perfMemory;
        private Wisej.Web.Label perfPayloadCaption;
        private Wisej.Web.Label perfPayload;
        private Wisej.Web.Label perfNotes;
        private Wisej.Web.Label statusLabel;
        private Wisej.Web.Label bannerLabel;
        private Wisej.Web.Button naiveButton;
        private Wisej.Web.Button bindAllButton;
        private Wisej.Web.Button optimizedButton;
        private Wisej.Web.Button measureButton;
        private Wisej.Web.Button editButton;
        private Wisej.Web.Button saveInvalidButton;
        private Wisej.Web.Timer measureTimer;
    }
}
