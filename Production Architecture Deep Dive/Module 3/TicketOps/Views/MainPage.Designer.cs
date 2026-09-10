namespace TicketOps.Views
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
            if (disposing)
            {
                // Session-level events: unsubscribe with the page.
                Wisej.Web.Application.ResponsiveProfileChanged -= this.Application_ResponsiveProfileChanged;
                Wisej.Web.Application.BrowserSizeChanged -= this.Application_BrowserSizeChanged;

                if (components != null)
                    components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Wisej.NET Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pnlWorkspaceCard = new Wisej.Web.Panel();
            this.labelScreenTitle = new Wisej.Web.Label();
            this.statusBanner = new TicketOps.Controls.StatusBanner();
            this.lblProfile = new Wisej.Web.Label();
            this.pnlWorkspaceHost = new Wisej.Web.Panel();
            this.workspace = new TicketOps.Controls.TicketWorkspace();
            this.pnlTraceHost = new Wisej.Web.Panel();
            this.tracePanel = new TicketOps.Diagnostics.ActivityTracePanel();
            this.pnlActionsHost = new Wisej.Web.Panel();
            this.flowActions = new Wisej.Web.FlowLayoutPanel();
            this.comboPreview = new Wisej.Web.ComboBox();
            this.buttonTour = new Wisej.Web.Button();
            this.buttonSearchShort = new Wisej.Web.Button();
            this.buttonUnknownProfile = new Wisej.Web.Button();
            this.buttonOutage = new Wisej.Web.Button();
            this.progressTour = new Wisej.Web.ProgressBar();
            this.pnlClearHost = new Wisej.Web.Panel();
            this.buttonClear = new Wisej.Web.Button();
            this.timerTour = new Wisej.Web.Timer(this.components);
            this.pnlWorkspaceCard.SuspendLayout();
            this.pnlWorkspaceHost.SuspendLayout();
            this.pnlTraceHost.SuspendLayout();
            this.pnlActionsHost.SuspendLayout();
            this.flowActions.SuspendLayout();
            this.pnlClearHost.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlWorkspaceCard  (the screen under study: Dock Fill, so it takes whatever the trace card and the bar leave)
            //
            this.pnlWorkspaceCard.BackColor = System.Drawing.Color.White;
            this.pnlWorkspaceCard.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlWorkspaceCard.Controls.Add(this.labelScreenTitle);
            this.pnlWorkspaceCard.Controls.Add(this.statusBanner);
            this.pnlWorkspaceCard.Controls.Add(this.lblProfile);
            this.pnlWorkspaceCard.Controls.Add(this.pnlWorkspaceHost);
            this.pnlWorkspaceCard.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlWorkspaceCard.Name = "pnlWorkspaceCard";
            this.pnlWorkspaceCard.Size = new System.Drawing.Size(780, 580);
            //
            // labelScreenTitle
            //
            this.labelScreenTitle.AutoSize = false;
            this.labelScreenTitle.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.labelScreenTitle.Location = new System.Drawing.Point(24, 18);
            this.labelScreenTitle.Name = "labelScreenTitle";
            this.labelScreenTitle.Size = new System.Drawing.Size(300, 30);
            this.labelScreenTitle.Text = "Ticket Workspace";
            //
            // statusBanner  (Controls/StatusBanner: "● state" + banner line)
            //
            this.statusBanner.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.statusBanner.Location = new System.Drawing.Point(24, 20);
            this.statusBanner.Name = "statusBanner";
            this.statusBanner.Size = new System.Drawing.Size(732, 58);
            //
            // lblProfile  (live: Application.ActiveProfile · Application.Browser.Size · the profiles ClientProfiles.json defines)
            //
            this.lblProfile.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblProfile.AutoEllipsis = true;
            this.lblProfile.AutoSize = false;
            this.lblProfile.Font = new System.Drawing.Font("monospace", 9F);
            this.lblProfile.ForeColor = System.Drawing.Color.FromArgb(58, 77, 99);
            this.lblProfile.Location = new System.Drawing.Point(24, 50);
            this.lblProfile.Name = "lblProfile";
            this.lblProfile.Size = new System.Drawing.Size(732, 22);
            this.lblProfile.Text = "Active profile: — · browser — · device —";
            this.lblProfile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblProfile.ToolTipText = "Application.ActiveProfile.Name and Application.Browser.Size, refreshed on ResponsiveProfileChanged / BrowserSizeChanged. Resize the browser across 600 / 1024 px.";
            //
            // pnlWorkspaceHost → workspace  (the TicketWorkspace UserControl fills the host; a preview narrows it to a device-like column)
            //
            this.pnlWorkspaceHost.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlWorkspaceHost.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.pnlWorkspaceHost.Controls.Add(this.workspace);
            this.pnlWorkspaceHost.Location = new System.Drawing.Point(24, 80);
            this.pnlWorkspaceHost.Name = "pnlWorkspaceHost";
            this.pnlWorkspaceHost.Size = new System.Drawing.Size(732, 476);
            this.workspace.Dock = Wisej.Web.DockStyle.Fill;
            this.workspace.Name = "workspace";
            //
            // pnlTraceHost → tracePanel  (Dock Right on desktop, Bottom on tablet, hidden on phone — see ApplyFrameProfile)
            //
            this.pnlTraceHost.Controls.Add(this.tracePanel);
            this.pnlTraceHost.Dock = Wisej.Web.DockStyle.Right;
            this.pnlTraceHost.Name = "pnlTraceHost";
            this.pnlTraceHost.Padding = new Wisej.Web.Padding(20, 0, 0, 0);
            this.pnlTraceHost.Size = new System.Drawing.Size(528, 580);
            this.tracePanel.Dock = Wisej.Web.DockStyle.Fill;
            this.tracePanel.Name = "tracePanel";
            //
            // pnlActionsHost  (Dock Bottom: a FlowLayoutPanel of paths that wraps on narrow browsers + Clear trace on the right)
            //
            this.pnlActionsHost.Controls.Add(this.flowActions);
            this.pnlActionsHost.Controls.Add(this.pnlClearHost);
            this.pnlActionsHost.Dock = Wisej.Web.DockStyle.Bottom;
            this.pnlActionsHost.Name = "pnlActionsHost";
            this.pnlActionsHost.Padding = new Wisej.Web.Padding(0, 16, 0, 0);
            this.pnlActionsHost.Size = new System.Drawing.Size(1308, 60);
            //
            // flowActions
            //
            this.flowActions.Controls.Add(this.comboPreview);
            this.flowActions.Controls.Add(this.buttonTour);
            this.flowActions.Controls.Add(this.progressTour);
            this.flowActions.Controls.Add(this.buttonSearchShort);
            this.flowActions.Controls.Add(this.buttonUnknownProfile);
            this.flowActions.Controls.Add(this.buttonOutage);
            this.flowActions.Dock = Wisej.Web.DockStyle.Fill;
            this.flowActions.FlowDirection = Wisej.Web.FlowDirection.LeftToRight;
            this.flowActions.Name = "flowActions";
            this.flowActions.WrapContents = true;
            //
            // comboPreview  (success path: apply a layout on demand)
            //
            this.comboPreview.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.comboPreview.Margin = new Wisej.Web.Padding(0, 4, 10, 4);
            this.comboPreview.Name = "comboPreview";
            this.comboPreview.Size = new System.Drawing.Size(230, 36);
            this.comboPreview.ToolTipText = "Success path: applies the desktop / tablet / phone arrangement to the workspace on demand (the browser keeps its real profile). \"Live\" follows the browser again.";
            this.comboPreview.SelectedIndexChanged += new System.EventHandler(this.comboPreview_SelectedIndexChanged);
            //
            // buttonTour  (progress path)
            //
            this.buttonTour.Margin = new Wisej.Web.Padding(0, 4, 10, 4);
            this.buttonTour.Name = "buttonTour";
            this.buttonTour.Size = new System.Drawing.Size(190, 36);
            this.buttonTour.Text = "▶ Tour all profiles";
            this.buttonTour.ToolTipText = "Progress path: a Timer applies desktop → tablet → phone → live, one arrangement per tick";
            this.buttonTour.Click += new System.EventHandler(this.buttonTour_Click);
            //
            // progressTour
            //
            this.progressTour.Margin = new Wisej.Web.Padding(0, 12, 10, 4);
            this.progressTour.Maximum = 4;
            this.progressTour.Name = "progressTour";
            this.progressTour.Size = new System.Drawing.Size(110, 18);
            this.progressTour.Visible = false;
            //
            // buttonSearchShort  (failure path 1: validation)
            //
            this.buttonSearchShort.Margin = new Wisej.Web.Padding(0, 4, 10, 4);
            this.buttonSearchShort.Name = "buttonSearchShort";
            this.buttonSearchShort.Size = new System.Drawing.Size(190, 36);
            this.buttonSearchShort.Text = "Search 1 character";
            this.buttonSearchShort.ToolTipText = "Failure path 1: the service rejects a one-character query (validation) — nothing is loaded";
            this.buttonSearchShort.Click += new System.EventHandler(this.buttonSearchShort_Click);
            //
            // buttonUnknownProfile  (failure path 2: an undefined profile → visible fallback)
            //
            this.buttonUnknownProfile.Margin = new Wisej.Web.Padding(0, 4, 10, 4);
            this.buttonUnknownProfile.Name = "buttonUnknownProfile";
            this.buttonUnknownProfile.Size = new System.Drawing.Size(220, 36);
            this.buttonUnknownProfile.Text = "Apply profile \"Kiosk\"";
            this.buttonUnknownProfile.ToolTipText = "Failure path 2: a profile ClientProfiles.json does not define — the workspace falls back to the desktop layout and says so";
            this.buttonUnknownProfile.Click += new System.EventHandler(this.buttonUnknownProfile_Click);
            //
            // buttonOutage  (error path + recovery)
            //
            this.buttonOutage.Margin = new Wisej.Web.Padding(0, 4, 10, 4);
            this.buttonOutage.Name = "buttonOutage";
            this.buttonOutage.Size = new System.Drawing.Size(200, 36);
            this.buttonOutage.Text = "Simulate data outage";
            this.buttonOutage.ToolTipText = "Error path: the repository throws; the log gets the details, the user gets a safe message. Click again to recover.";
            this.buttonOutage.Click += new System.EventHandler(this.buttonOutage_Click);
            //
            // pnlClearHost → buttonClear  (Dock Right so it always hugs the right edge)
            //
            this.pnlClearHost.Controls.Add(this.buttonClear);
            this.pnlClearHost.Dock = Wisej.Web.DockStyle.Right;
            this.pnlClearHost.Name = "pnlClearHost";
            this.pnlClearHost.Padding = new Wisej.Web.Padding(10, 4, 0, 4);
            this.pnlClearHost.Size = new System.Drawing.Size(150, 44);
            this.buttonClear.Dock = Wisej.Web.DockStyle.Top;
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(140, 36);
            this.buttonClear.Text = "Clear trace";
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            //
            // timerTour
            //
            this.timerTour.Interval = 1600;
            this.timerTour.Tick += new System.EventHandler(this.timerTour_Tick);
            //
            // MainPage  (Dock order: Fill first, then Right, then Bottom — the bar docks first, the trace second, the card takes the rest)
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.pnlWorkspaceCard);
            this.Controls.Add(this.pnlTraceHost);
            this.Controls.Add(this.pnlActionsHost);
            this.Name = "MainPage";
            this.Padding = new Wisej.Web.Padding(20);
            this.Size = new System.Drawing.Size(1348, 680);
            this.Text = "TicketOps Console — Module 3 · Responsive layouts, client profiles & composition";
            this.Load += new System.EventHandler(this.MainPage_Load);
            this.pnlWorkspaceCard.ResumeLayout(false);
            this.pnlWorkspaceHost.ResumeLayout(false);
            this.pnlTraceHost.ResumeLayout(false);
            this.pnlActionsHost.ResumeLayout(false);
            this.flowActions.ResumeLayout(false);
            this.pnlClearHost.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlWorkspaceCard;
        private Wisej.Web.Label labelScreenTitle;
        private TicketOps.Controls.StatusBanner statusBanner;
        private Wisej.Web.Label lblProfile;
        private Wisej.Web.Panel pnlWorkspaceHost;
        private TicketOps.Controls.TicketWorkspace workspace;
        private Wisej.Web.Panel pnlTraceHost;
        private TicketOps.Diagnostics.ActivityTracePanel tracePanel;
        private Wisej.Web.Panel pnlActionsHost;
        private Wisej.Web.FlowLayoutPanel flowActions;
        private Wisej.Web.ComboBox comboPreview;
        private Wisej.Web.Button buttonTour;
        private Wisej.Web.Button buttonSearchShort;
        private Wisej.Web.Button buttonUnknownProfile;
        private Wisej.Web.Button buttonOutage;
        private Wisej.Web.ProgressBar progressTour;
        private Wisej.Web.Panel pnlClearHost;
        private Wisej.Web.Button buttonClear;
        private Wisej.Web.Timer timerTour;
    }
}
