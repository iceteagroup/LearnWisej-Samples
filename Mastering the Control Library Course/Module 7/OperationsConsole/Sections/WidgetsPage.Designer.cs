namespace OperationsConsole.Sections
{
    partial class WidgetsPage
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
            this.components = new System.ComponentModel.Container();
            this.pnlContent = new Wisej.Web.Panel();
            this.pnlRatingCard = new Wisej.Web.Panel();
            this.lblCardTitle = new Wisej.Web.Label();
            this.lblCustomer = new Wisej.Web.Label();
            this.ratingWidget = new Wisej.Web.Widget();
            this.lblSavedState = new Wisej.Web.Label();
            this.lblWidgetHint = new Wisej.Web.Label();
            this.pnlBridgeCard = new Wisej.Web.Panel();
            this.lblBridgeTitle = new Wisej.Web.Label();
            this.lblLastIn = new Wisej.Web.Label();
            this.lblLastOut = new Wisej.Web.Label();
            this.lblClientState = new Wisej.Web.Label();
            this.pnlContractCard = new Wisej.Web.Panel();
            this.lblContractTitle = new Wisej.Web.Label();
            this.lblContract = new Wisej.Web.Label();
            this.pnlCommands = new Wisej.Web.Panel();
            this.btnPushFromServer = new Wisej.Web.Button();
            this.btnReadClientState = new Wisej.Web.Button();
            this.btnReinit = new Wisej.Web.Button();
            this.btnTheme = new Wisej.Web.Button();
            this.btnSendMalformed = new Wisej.Web.Button();
            this.btnSendOutOfRange = new Wisej.Web.Button();
            this.chkSimulateServiceFailure = new Wisej.Web.CheckBox();
            this.styleSheet = new Wisej.Web.StyleSheet(this.components);
            this.pnlContent.SuspendLayout();
            this.pnlRatingCard.SuspendLayout();
            this.pnlBridgeCard.SuspendLayout();
            this.pnlContractCard.SuspendLayout();
            this.pnlCommands.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlContent  (Dock = Fill — added FIRST so the Top command row can claim the full width)
            //
            this.pnlContent.AutoScroll = true;
            this.pnlContent.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.pnlContent.Controls.Add(this.pnlRatingCard);
            this.pnlContent.Controls.Add(this.pnlBridgeCard);
            this.pnlContent.Controls.Add(this.pnlContractCard);
            this.pnlContent.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Size = new System.Drawing.Size(740, 608);
            this.pnlContent.TabIndex = 1;
            //
            // pnlRatingCard  (the widget itself)
            //
            this.pnlRatingCard.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlRatingCard.BackColor = System.Drawing.Color.White;
            this.pnlRatingCard.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlRatingCard.Controls.Add(this.lblCardTitle);
            this.pnlRatingCard.Controls.Add(this.lblCustomer);
            this.pnlRatingCard.Controls.Add(this.ratingWidget);
            this.pnlRatingCard.Controls.Add(this.lblSavedState);
            this.pnlRatingCard.Controls.Add(this.lblWidgetHint);
            this.pnlRatingCard.Location = new System.Drawing.Point(16, 16);
            this.pnlRatingCard.Name = "pnlRatingCard";
            this.pnlRatingCard.Size = new System.Drawing.Size(692, 250);
            this.pnlRatingCard.TabIndex = 10;
            //
            // lblCardTitle
            //
            this.lblCardTitle.AutoSize = false;
            this.lblCardTitle.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblCardTitle.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblCardTitle.Location = new System.Drawing.Point(20, 14);
            this.lblCardTitle.Name = "lblCardTitle";
            this.lblCardTitle.Size = new System.Drawing.Size(400, 20);
            this.lblCardTitle.Text = "CUSTOMER RATING · WIDGET";
            this.lblCardTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblCustomer
            //
            this.lblCustomer.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblCustomer.AutoSize = false;
            this.lblCustomer.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblCustomer.Location = new System.Drawing.Point(20, 36);
            this.lblCustomer.Name = "lblCustomer";
            this.lblCustomer.Size = new System.Drawing.Size(652, 28);
            this.lblCustomer.Text = "Northwind Traders · customer CUST-1042";
            this.lblCustomer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // ratingWidget  (Wisej.Web.Widget — packages, WiredEvents and the accessible label are declared here;
            //                InitScript and Options are set in WidgetsPage.ConfigureRatingWidget())
            //
            this.ratingWidget.AccessibleName = "Customer satisfaction rating";
            this.ratingWidget.AccessibleDescription = "Five stars. Click a star, or use the arrow keys, to rate this customer from 1 to 5.";
            this.ratingWidget.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.ratingWidget.Location = new System.Drawing.Point(20, 72);
            this.ratingWidget.Name = "ratingWidget";
            // Packages load once per page, in list order: the stylesheet first so the first paint is
            // already themed, then the library that renders into the container.
            this.ratingWidget.Packages.Add(new Wisej.Web.Widget.Package() { Name = "rating-css", Source = "wwwroot/rating.css" });
            this.ratingWidget.Packages.Add(new Wisej.Web.Widget.Package() { Name = "rating-js", Source = "wwwroot/rating.js" });
            this.ratingWidget.Size = new System.Drawing.Size(652, 104);
            this.ratingWidget.TabIndex = 11;
            // The only event the client may raise. Anything else is ignored by the handler.
            this.ratingWidget.WiredEvents = new string[] { "ratingChanged" };
            this.ratingWidget.WidgetEvent += new Wisej.Web.WidgetEventHandler(this.ratingWidget_WidgetEvent);
            //
            // lblSavedState  (what the server believes: never written by the browser)
            //
            this.lblSavedState.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblSavedState.AutoSize = false;
            this.lblSavedState.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblSavedState.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblSavedState.Location = new System.Drawing.Point(20, 184);
            this.lblSavedState.Name = "lblSavedState";
            this.lblSavedState.Size = new System.Drawing.Size(652, 24);
            this.lblSavedState.Text = "Northwind Traders has no saved rating yet — click a star.";
            this.lblSavedState.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblWidgetHint
            //
            this.lblWidgetHint.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblWidgetHint.AutoSize = false;
            this.lblWidgetHint.Font = new System.Drawing.Font("monospace", 8F);
            this.lblWidgetHint.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblWidgetHint.Location = new System.Drawing.Point(20, 210);
            this.lblWidgetHint.Name = "lblWidgetHint";
            this.lblWidgetHint.Size = new System.Drawing.Size(652, 26);
            this.lblWidgetHint.Text = "ratingWidget · Packages: rating.css → rating.js · WiredEvents: ratingChanged";
            this.lblWidgetHint.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // pnlBridgeCard  (the last message each way — the Event log card in MainPage keeps the full trace)
            //
            this.pnlBridgeCard.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlBridgeCard.BackColor = System.Drawing.Color.White;
            this.pnlBridgeCard.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlBridgeCard.Controls.Add(this.lblBridgeTitle);
            this.pnlBridgeCard.Controls.Add(this.lblLastIn);
            this.pnlBridgeCard.Controls.Add(this.lblLastOut);
            this.pnlBridgeCard.Controls.Add(this.lblClientState);
            this.pnlBridgeCard.Location = new System.Drawing.Point(16, 282);
            this.pnlBridgeCard.Name = "pnlBridgeCard";
            this.pnlBridgeCard.Size = new System.Drawing.Size(692, 140);
            this.pnlBridgeCard.TabIndex = 20;
            //
            // lblBridgeTitle
            //
            this.lblBridgeTitle.AutoSize = false;
            this.lblBridgeTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblBridgeTitle.Location = new System.Drawing.Point(20, 12);
            this.lblBridgeTitle.Name = "lblBridgeTitle";
            this.lblBridgeTitle.Size = new System.Drawing.Size(500, 26);
            this.lblBridgeTitle.Text = "Widget bridge · last message each way";
            this.lblBridgeTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblLastIn
            //
            this.lblLastIn.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblLastIn.AutoSize = false;
            this.lblLastIn.Font = new System.Drawing.Font("monospace", 9F);
            this.lblLastIn.ForeColor = System.Drawing.Color.FromArgb(58, 77, 99);
            this.lblLastIn.Location = new System.Drawing.Point(20, 44);
            this.lblLastIn.Name = "lblLastIn";
            this.lblLastIn.Size = new System.Drawing.Size(652, 24);
            this.lblLastIn.Text = "← JS→.NET  (nothing yet)";
            this.lblLastIn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblLastOut
            //
            this.lblLastOut.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblLastOut.AutoSize = false;
            this.lblLastOut.Font = new System.Drawing.Font("monospace", 9F);
            this.lblLastOut.ForeColor = System.Drawing.Color.FromArgb(58, 77, 99);
            this.lblLastOut.Location = new System.Drawing.Point(20, 70);
            this.lblLastOut.Name = "lblLastOut";
            this.lblLastOut.Size = new System.Drawing.Size(652, 24);
            this.lblLastOut.Text = "→ .NET→JS  (nothing yet)";
            this.lblLastOut.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblClientState
            //
            this.lblClientState.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblClientState.AutoSize = false;
            this.lblClientState.Font = new System.Drawing.Font("monospace", 9F);
            this.lblClientState.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblClientState.Location = new System.Drawing.Point(20, 96);
            this.lblClientState.Name = "lblClientState";
            this.lblClientState.Size = new System.Drawing.Size(652, 24);
            this.lblClientState.Text = "getState()  (press \"Read client state\")";
            this.lblClientState.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // pnlContractCard
            //
            this.pnlContractCard.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlContractCard.BackColor = System.Drawing.Color.White;
            this.pnlContractCard.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlContractCard.Controls.Add(this.lblContractTitle);
            this.pnlContractCard.Controls.Add(this.lblContract);
            this.pnlContractCard.Location = new System.Drawing.Point(16, 438);
            this.pnlContractCard.Name = "pnlContractCard";
            this.pnlContractCard.Size = new System.Drawing.Size(692, 152);
            this.pnlContractCard.TabIndex = 30;
            //
            // lblContractTitle
            //
            this.lblContractTitle.AutoSize = false;
            this.lblContractTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblContractTitle.Location = new System.Drawing.Point(20, 12);
            this.lblContractTitle.Name = "lblContractTitle";
            this.lblContractTitle.Size = new System.Drawing.Size(500, 26);
            this.lblContractTitle.Text = "Contract · docs/WidgetContract.md";
            this.lblContractTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblContract
            //
            this.lblContract.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblContract.AutoSize = false;
            this.lblContract.ForeColor = System.Drawing.Color.FromArgb(58, 77, 99);
            this.lblContract.Location = new System.Drawing.Point(20, 42);
            this.lblContract.Name = "lblContract";
            this.lblContract.Size = new System.Drawing.Size(652, 96);
            this.lblContract.Text = "Packages (load order): wwwroot/rating.css → wwwroot/rating.js.  State in (Options): value, max, label, saved, theme, profile.  Event out: ratingChanged { value }.  Calls in: setSaved(value), ratingClearSaved(), getState(), reinit(), sendRawPayload(value).  Every rule (1–5, whole number) is enforced in Services/RatingService.cs; rating.js holds no rule and no colour.";
            this.lblContract.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // pnlCommands  (Dock = Top — added LAST so it spans the full width above pnlContent)
            //
            this.pnlCommands.BackColor = System.Drawing.Color.White;
            this.pnlCommands.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlCommands.Controls.Add(this.btnPushFromServer);
            this.pnlCommands.Controls.Add(this.btnReadClientState);
            this.pnlCommands.Controls.Add(this.btnReinit);
            this.pnlCommands.Controls.Add(this.btnTheme);
            this.pnlCommands.Controls.Add(this.btnSendMalformed);
            this.pnlCommands.Controls.Add(this.btnSendOutOfRange);
            this.pnlCommands.Controls.Add(this.chkSimulateServiceFailure);
            this.pnlCommands.Dock = Wisej.Web.DockStyle.Top;
            this.pnlCommands.Name = "pnlCommands";
            this.pnlCommands.Size = new System.Drawing.Size(740, 96);
            this.pnlCommands.TabIndex = 0;
            //
            // btnPushFromServer  (server → client through Options + Update())
            //
            this.btnPushFromServer.AccessibleName = "Push a rating from the server into the widget";
            this.btnPushFromServer.Location = new System.Drawing.Point(16, 10);
            this.btnPushFromServer.Name = "btnPushFromServer";
            this.btnPushFromServer.Size = new System.Drawing.Size(168, 32);
            this.btnPushFromServer.TabIndex = 1;
            this.btnPushFromServer.Text = "Push 5 from server";
            this.btnPushFromServer.ToolTipText = "Sets ratingWidget.Options.value = 5 and calls Update(): the client update() applies it silently, so a server-driven change never bounces back as a ratingChanged event.";
            this.btnPushFromServer.Click += new System.EventHandler(this.btnPushFromServer_Click);
            //
            // btnReadClientState  (server → client through CallAsync, with a return value)
            //
            this.btnReadClientState.AccessibleName = "Read the widget state from the browser";
            this.btnReadClientState.Location = new System.Drawing.Point(192, 10);
            this.btnReadClientState.Name = "btnReadClientState";
            this.btnReadClientState.Size = new System.Drawing.Size(150, 32);
            this.btnReadClientState.TabIndex = 2;
            this.btnReadClientState.Text = "Read client state";
            this.btnReadClientState.ToolTipText = "await ratingWidget.CallAsync(\"getState\") — the server-to-client call that comes back with a value from the browser.";
            this.btnReadClientState.Click += new System.EventHandler(this.btnReadClientState_Click);
            //
            // btnReinit  (proves init() is safe to run again)
            //
            this.btnReinit.AccessibleName = "Re-create the client widget";
            this.btnReinit.Location = new System.Drawing.Point(350, 10);
            this.btnReinit.Name = "btnReinit";
            this.btnReinit.Size = new System.Drawing.Size(170, 32);
            this.btnReinit.TabIndex = 3;
            this.btnReinit.Text = "Re-run init()";
            this.btnReinit.ToolTipText = "ratingWidget.Call(\"reinit\") re-runs init(): the adapter tears the old library down first, so nothing is duplicated. This is the \"blank widget after the tab was hidden and shown again\" pitfall.";
            this.btnReinit.Click += new System.EventHandler(this.btnReinit_Click);
            //
            // btnTheme  (Application.LoadTheme — the widget follows through rating.css, not through JS colours)
            //
            this.btnTheme.AccessibleName = "Switch the application theme";
            this.btnTheme.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left;
            this.btnTheme.Location = new System.Drawing.Point(528, 10);
            this.btnTheme.Name = "btnTheme";
            this.btnTheme.Size = new System.Drawing.Size(190, 32);
            this.btnTheme.TabIndex = 4;
            this.btnTheme.Text = "Theme → Material-3";
            this.btnTheme.ToolTipText = "Application.LoadTheme() between Bootstrap-4 and Material-3. The widget follows through the rating.css variables — no colour is hard-coded in rating.js.";
            this.btnTheme.Click += new System.EventHandler(this.btnTheme_Click);
            //
            // btnSendMalformed  (failure path 1: a payload the widget would never produce)
            //
            this.btnSendMalformed.AccessibleName = "Send a malformed rating payload";
            this.btnSendMalformed.Location = new System.Drawing.Point(16, 52);
            this.btnSendMalformed.Name = "btnSendMalformed";
            this.btnSendMalformed.Size = new System.Drawing.Size(196, 32);
            this.btnSendMalformed.TabIndex = 5;
            this.btnSendMalformed.Text = "Send malformed payload";
            this.btnSendMalformed.ToolTipText = "Calls a client function that fires ratingChanged with { value: \"seven\" } — exactly what a user with developer tools can type. The server must reject it and store nothing.";
            this.btnSendMalformed.Click += new System.EventHandler(this.btnSendMalformed_Click);
            //
            // btnSendOutOfRange  (failure path 2: a number outside the scale)
            //
            this.btnSendOutOfRange.AccessibleName = "Send an out-of-range rating payload";
            this.btnSendOutOfRange.Location = new System.Drawing.Point(220, 52);
            this.btnSendOutOfRange.Name = "btnSendOutOfRange";
            this.btnSendOutOfRange.Size = new System.Drawing.Size(196, 32);
            this.btnSendOutOfRange.TabIndex = 6;
            this.btnSendOutOfRange.Text = "Send out-of-range payload";
            this.btnSendOutOfRange.ToolTipText = "Fires ratingChanged with { value: 9 }. RatingService.TryNormalize rejects it; nothing is stored and the widget stays editable.";
            this.btnSendOutOfRange.Click += new System.EventHandler(this.btnSendOutOfRange_Click);
            //
            // chkSimulateServiceFailure  (failure path 3: the store refuses the write)
            //
            this.chkSimulateServiceFailure.AccessibleName = "Make the ratings service fail on the next save";
            this.chkSimulateServiceFailure.Location = new System.Drawing.Point(428, 56);
            this.chkSimulateServiceFailure.Name = "chkSimulateServiceFailure";
            this.chkSimulateServiceFailure.Size = new System.Drawing.Size(240, 24);
            this.chkSimulateServiceFailure.TabIndex = 7;
            this.chkSimulateServiceFailure.Text = "Simulate service failure";
            this.chkSimulateServiceFailure.ToolTipText = "The next save throws inside RatingService: a friendly error, nothing stored, and the widget stays editable. Untick it and click a star again to recover.";
            this.chkSimulateServiceFailure.CheckedChanged += new System.EventHandler(this.chkSimulateServiceFailure_CheckedChanged);
            //
            // WidgetsPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            // Docking is applied from the LAST added control to the FIRST: pnlContent (Fill) is added
            // first, pnlCommands (Top) last, so the command row spans the whole width.
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlCommands);
            this.Name = "WidgetsPage";
            this.Size = new System.Drawing.Size(740, 704);
            this.pnlContent.ResumeLayout(false);
            this.pnlRatingCard.ResumeLayout(false);
            this.pnlBridgeCard.ResumeLayout(false);
            this.pnlContractCard.ResumeLayout(false);
            this.pnlCommands.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        // content area
        private Wisej.Web.Panel pnlContent;

        // the rating card
        private Wisej.Web.Panel pnlRatingCard;
        private Wisej.Web.Label lblCardTitle;
        private Wisej.Web.Label lblCustomer;
        private Wisej.Web.Widget ratingWidget;
        private Wisej.Web.Label lblSavedState;
        private Wisej.Web.Label lblWidgetHint;

        // the bridge card (last message each way)
        private Wisej.Web.Panel pnlBridgeCard;
        private Wisej.Web.Label lblBridgeTitle;
        private Wisej.Web.Label lblLastIn;
        private Wisej.Web.Label lblLastOut;
        private Wisej.Web.Label lblClientState;

        // the contract card
        private Wisej.Web.Panel pnlContractCard;
        private Wisej.Web.Label lblContractTitle;
        private Wisej.Web.Label lblContract;

        // command row
        private Wisej.Web.Panel pnlCommands;
        private Wisej.Web.Button btnPushFromServer;
        private Wisej.Web.Button btnReadClientState;
        private Wisej.Web.Button btnReinit;
        private Wisej.Web.Button btnTheme;
        private Wisej.Web.Button btnSendMalformed;
        private Wisej.Web.Button btnSendOutOfRange;
        private Wisej.Web.CheckBox chkSimulateServiceFailure;

        // extender: the application-level override layer for the packaged widget CSS
        private Wisej.Web.StyleSheet styleSheet;
    }
}
