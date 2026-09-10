namespace SupportDesk.Web
{
    partial class TicketBrowserPage
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
            this.panelQuery = new Wisej.Web.Panel();
            this.labelTitle = new Wisej.Web.Label();
            this.labelState = new Wisej.Web.Label();
            this.labelLead = new Wisej.Web.Label();
            this.countButton = new Wisej.Web.Button();
            this.btnSeed = new Wisej.Web.Button();
            this.statusLabel = new Wisej.Web.Label();
            this.labelBanner = new Wisej.Web.Label();
            this.labelModelTitle = new Wisej.Web.Label();
            this.labelModel = new Wisej.Web.Label();
            this.labelLifetimesTitle = new Wisej.Web.Label();
            this.labelLifetimes = new Wisej.Web.Label();
            this.labelRule = new Wisej.Web.Label();
            this.panelTrace = new Wisej.Web.Panel();
            this.labelTraceTitle = new Wisej.Web.Label();
            this.listTrace = new Wisej.Web.ListBox();
            this.labelTraceFooter = new Wisej.Web.Label();
            this.panelActions = new Wisej.Web.Panel();
            this.buttonOverlongTitle = new Wisej.Web.Button();
            this.buttonDeleteCustomer = new Wisej.Web.Button();
            this.buttonDeleteAgent = new Wisej.Web.Button();
            this.buttonDeleteTicket = new Wisej.Web.Button();
            this.buttonReset = new Wisej.Web.Button();
            this.buttonClear = new Wisej.Web.Button();
            this.panelActions2 = new Wisej.Web.Panel();
            this.labelActions2 = new Wisej.Web.Label();
            this.buttonSlowCount = new Wisej.Web.Button();
            this.buttonRapid = new Wisej.Web.Button();
            this.buttonBreak = new Wisej.Web.Button();
            this.buttonRestore = new Wisej.Web.Button();
            this.buttonAntiPattern = new Wisej.Web.Button();
            this.panelQuery.SuspendLayout();
            this.panelTrace.SuspendLayout();
            this.panelActions.SuspendLayout();
            this.panelActions2.SuspendLayout();
            this.SuspendLayout();
            //
            // panelQuery  (the query card)
            //
            this.panelQuery.BackColor = System.Drawing.Color.White;
            this.panelQuery.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelQuery.Controls.Add(this.labelTitle);
            this.panelQuery.Controls.Add(this.labelState);
            this.panelQuery.Controls.Add(this.labelLead);
            this.panelQuery.Controls.Add(this.countButton);
            this.panelQuery.Controls.Add(this.btnSeed);
            this.panelQuery.Controls.Add(this.statusLabel);
            this.panelQuery.Controls.Add(this.labelBanner);
            this.panelQuery.Controls.Add(this.labelModelTitle);
            this.panelQuery.Controls.Add(this.labelModel);
            this.panelQuery.Controls.Add(this.labelLifetimesTitle);
            this.panelQuery.Controls.Add(this.labelLifetimes);
            this.panelQuery.Controls.Add(this.labelRule);
            this.panelQuery.Location = new System.Drawing.Point(30, 30);
            this.panelQuery.Name = "panelQuery";
            this.panelQuery.Size = new System.Drawing.Size(560, 800);
            //
            // labelTitle
            //
            this.labelTitle.AutoSize = false;
            this.labelTitle.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.labelTitle.Location = new System.Drawing.Point(24, 18);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(300, 30);
            this.labelTitle.Text = "Support Desk Data Console";
            //
            // labelState  (● idle / working / ok / fault)
            //
            this.labelState.AutoSize = false;
            this.labelState.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelState.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.labelState.Location = new System.Drawing.Point(330, 20);
            this.labelState.Name = "labelState";
            this.labelState.Size = new System.Drawing.Size(206, 26);
            this.labelState.Text = "● idle";
            this.labelState.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // labelLead
            //
            this.labelLead.AutoSize = false;
            this.labelLead.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelLead.Location = new System.Drawing.Point(24, 52);
            this.labelLead.Name = "labelLead";
            this.labelLead.Size = new System.Drawing.Size(512, 62);
            this.labelLead.Text = "Module 2 · the model, the first migration and the seed. Five entities configured with the Fluent API (lengths, RowVersion, indexes, delete behaviours), MigrateAsync at start in Development, and a seeder that fills five customers, three agents, six categories and sixty tickets through one short-lived context.";
            this.labelLead.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // countButton  (the Module 1 lab control)
            //
            this.countButton.Font = new System.Drawing.Font("default", 11F, System.Drawing.FontStyle.Bold);
            this.countButton.Location = new System.Drawing.Point(24, 124);
            this.countButton.Name = "countButton";
            this.countButton.Size = new System.Drawing.Size(170, 44);
            this.countButton.Text = "Count tickets";
            this.countButton.Click += new System.EventHandler(this.countButton_Click);
            //
            // btnSeed  (the Module 2 lab control)
            //
            this.btnSeed.Font = new System.Drawing.Font("default", 11F, System.Drawing.FontStyle.Bold);
            this.btnSeed.Location = new System.Drawing.Point(204, 124);
            this.btnSeed.Name = "btnSeed";
            this.btnSeed.Size = new System.Drawing.Size(220, 44);
            this.btnSeed.Text = "Seed development data";
            this.btnSeed.ToolTipText = "DevelopmentSeeder.SeedDevelopmentDataAsync: one context, AnyAsync, AddRange, SaveChangesAsync. Click it twice — the second run seeds nothing.";
            this.btnSeed.Click += new System.EventHandler(this.btnSeed_Click);
            //
            // statusLabel  (the lab control)
            //
            this.statusLabel.AutoSize = false;
            this.statusLabel.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.statusLabel.Location = new System.Drawing.Point(24, 176);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(512, 30);
            this.statusLabel.Text = "Click Count to run the first query, or Seed to fill the development database.";
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // labelBanner  (friendly error message)
            //
            this.labelBanner.AutoSize = false;
            this.labelBanner.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
            this.labelBanner.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelBanner.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            this.labelBanner.Location = new System.Drawing.Point(24, 212);
            this.labelBanner.Name = "labelBanner";
            this.labelBanner.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.labelBanner.Size = new System.Drawing.Size(512, 52);
            this.labelBanner.Text = "";
            this.labelBanner.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelBanner.Visible = false;
            //
            // labelModelTitle
            //
            this.labelModelTitle.AutoSize = false;
            this.labelModelTitle.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelModelTitle.Location = new System.Drawing.Point(24, 274);
            this.labelModelTitle.Name = "labelModelTitle";
            this.labelModelTitle.Size = new System.Drawing.Size(512, 24);
            this.labelModelTitle.Text = "Model & migration — read from the database and the model after every operation";
            //
            // labelModel  (monospace table: row counts, migrations, indexes, delete behaviours, checks)
            //
            this.labelModel.AllowHtml = true;
            this.labelModel.AutoSize = false;
            this.labelModel.Font = new System.Drawing.Font("monospace", 9F);
            this.labelModel.ForeColor = System.Drawing.Color.FromArgb(60, 72, 88);
            this.labelModel.Location = new System.Drawing.Point(24, 302);
            this.labelModel.Name = "labelModel";
            this.labelModel.Size = new System.Drawing.Size(512, 316);
            this.labelModel.Text = "Reading the schema…";
            this.labelModel.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // labelLifetimesTitle
            //
            this.labelLifetimesTitle.AutoSize = false;
            this.labelLifetimesTitle.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelLifetimesTitle.Location = new System.Drawing.Point(24, 626);
            this.labelLifetimesTitle.Name = "labelLifetimesTitle";
            this.labelLifetimesTitle.Size = new System.Drawing.Size(512, 24);
            this.labelLifetimesTitle.Text = "Four lifetimes on one server — what this session owns right now (Module 1)";
            //
            // labelLifetimes  (monospace table)
            //
            this.labelLifetimes.AllowHtml = true;
            this.labelLifetimes.AutoSize = false;
            this.labelLifetimes.Font = new System.Drawing.Font("monospace", 9F);
            this.labelLifetimes.ForeColor = System.Drawing.Color.FromArgb(60, 72, 88);
            this.labelLifetimes.Location = new System.Drawing.Point(24, 654);
            this.labelLifetimes.Name = "labelLifetimes";
            this.labelLifetimes.Size = new System.Drawing.Size(512, 96);
            this.labelLifetimes.Text = "";
            this.labelLifetimes.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // labelRule
            //
            this.labelRule.AutoSize = false;
            this.labelRule.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelRule.Location = new System.Drawing.Point(24, 754);
            this.labelRule.Name = "labelRule";
            this.labelRule.Size = new System.Drawing.Size(512, 40);
            this.labelRule.Text = "Rule: the Fluent API declares what the database must guarantee; the friendly validation the user reads arrives in Module 5. A DbContext still lives for one operation.";
            this.labelRule.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // panelTrace  (Server ⇄ Database trace)
            //
            this.panelTrace.BackColor = System.Drawing.Color.White;
            this.panelTrace.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelTrace.Controls.Add(this.labelTraceTitle);
            this.panelTrace.Controls.Add(this.listTrace);
            this.panelTrace.Controls.Add(this.labelTraceFooter);
            this.panelTrace.Location = new System.Drawing.Point(618, 30);
            this.panelTrace.Name = "panelTrace";
            this.panelTrace.Size = new System.Drawing.Size(700, 800);
            //
            // labelTraceTitle
            //
            this.labelTraceTitle.AutoSize = false;
            this.labelTraceTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelTraceTitle.Location = new System.Drawing.Point(20, 14);
            this.labelTraceTitle.Name = "labelTraceTitle";
            this.labelTraceTitle.Size = new System.Drawing.Size(660, 30);
            this.labelTraceTitle.Text = "Server ⇄ Database  ·  EF Core lifetime & SQL trace";
            //
            // listTrace
            //
            this.listTrace.Font = new System.Drawing.Font("monospace", 9F);
            this.listTrace.Location = new System.Drawing.Point(20, 52);
            this.listTrace.Name = "listTrace";
            this.listTrace.Size = new System.Drawing.Size(660, 696);
            //
            // labelTraceFooter
            //
            this.labelTraceFooter.AutoSize = false;
            this.labelTraceFooter.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelTraceFooter.Location = new System.Drawing.Point(20, 758);
            this.labelTraceFooter.Name = "labelTraceFooter";
            this.labelTraceFooter.Size = new System.Drawing.Size(660, 26);
            this.labelTraceFooter.Text = "• server   ◦ context created / disposed   → SQL sent to the database (ms)   ← result back in the handler";
            //
            // panelActions  (bottom bar, row 1: Module 2 paths)
            //
            this.panelActions.Controls.Add(this.buttonOverlongTitle);
            this.panelActions.Controls.Add(this.buttonDeleteCustomer);
            this.panelActions.Controls.Add(this.buttonDeleteAgent);
            this.panelActions.Controls.Add(this.buttonDeleteTicket);
            this.panelActions.Controls.Add(this.buttonReset);
            this.panelActions.Controls.Add(this.buttonClear);
            this.panelActions.Location = new System.Drawing.Point(30, 846);
            this.panelActions.Name = "panelActions";
            this.panelActions.Size = new System.Drawing.Size(1288, 44);
            //
            // buttonOverlongTitle  (failure path: the CHECK constraint)
            //
            this.buttonOverlongTitle.Location = new System.Drawing.Point(0, 4);
            this.buttonOverlongTitle.Name = "buttonOverlongTitle";
            this.buttonOverlongTitle.Size = new System.Drawing.Size(220, 36);
            this.buttonOverlongTitle.Text = "Save a 200-char title (fails)";
            this.buttonOverlongTitle.ToolTipText = "Inserts a ticket with a 200-character title. SQLite ignores HasMaxLength(180); the CHECK constraint refuses it → DbUpdateException → friendly banner.";
            this.buttonOverlongTitle.Click += new System.EventHandler(this.buttonOverlongTitle_Click);
            //
            // buttonDeleteCustomer  (failure path: Restrict)
            //
            this.buttonDeleteCustomer.Location = new System.Drawing.Point(228, 4);
            this.buttonDeleteCustomer.Name = "buttonDeleteCustomer";
            this.buttonDeleteCustomer.Size = new System.Drawing.Size(280, 36);
            this.buttonDeleteCustomer.Text = "Delete a customer with tickets (refused)";
            this.buttonDeleteCustomer.ToolTipText = "Removes a customer that still has tickets. ON DELETE RESTRICT refuses the DELETE → DbUpdateException → friendly banner.";
            this.buttonDeleteCustomer.Click += new System.EventHandler(this.buttonDeleteCustomer_Click);
            //
            // buttonDeleteAgent  (SetNull)
            //
            this.buttonDeleteAgent.Location = new System.Drawing.Point(516, 4);
            this.buttonDeleteAgent.Name = "buttonDeleteAgent";
            this.buttonDeleteAgent.Size = new System.Drawing.Size(210, 36);
            this.buttonDeleteAgent.Text = "Unassign an agent (SetNull)";
            this.buttonDeleteAgent.ToolTipText = "Deletes an agent that owns tickets. ON DELETE SET NULL clears AgentId on their tickets — the trace shows the unassigned count before and after.";
            this.buttonDeleteAgent.Click += new System.EventHandler(this.buttonDeleteAgent_Click);
            //
            // buttonDeleteTicket  (Cascade)
            //
            this.buttonDeleteTicket.Location = new System.Drawing.Point(734, 4);
            this.buttonDeleteTicket.Name = "buttonDeleteTicket";
            this.buttonDeleteTicket.Size = new System.Drawing.Size(270, 36);
            this.buttonDeleteTicket.Text = "Delete a ticket with comments (Cascade)";
            this.buttonDeleteTicket.ToolTipText = "Deletes a ticket that has comments. ON DELETE CASCADE removes the comments — the trace shows the comment count before and after.";
            this.buttonDeleteTicket.Click += new System.EventHandler(this.buttonDeleteTicket_Click);
            //
            // buttonReset  (lab prop: run the delete demos again)
            //
            this.buttonReset.Location = new System.Drawing.Point(1012, 4);
            this.buttonReset.Name = "buttonReset";
            this.buttonReset.Size = new System.Drawing.Size(150, 36);
            this.buttonReset.Text = "Reset & reseed";
            this.buttonReset.ToolTipText = "Empties every table (children first) and seeds again, so the delete demos can be repeated. Development only.";
            this.buttonReset.Click += new System.EventHandler(this.buttonReset_Click);
            //
            // buttonClear
            //
            this.buttonClear.Location = new System.Drawing.Point(1178, 4);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(110, 36);
            this.buttonClear.Text = "Clear trace";
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            //
            // panelActions2  (bottom bar, row 2: Module 1 paths)
            //
            this.panelActions2.Controls.Add(this.labelActions2);
            this.panelActions2.Controls.Add(this.buttonSlowCount);
            this.panelActions2.Controls.Add(this.buttonRapid);
            this.panelActions2.Controls.Add(this.buttonBreak);
            this.panelActions2.Controls.Add(this.buttonRestore);
            this.panelActions2.Controls.Add(this.buttonAntiPattern);
            this.panelActions2.Location = new System.Drawing.Point(30, 896);
            this.panelActions2.Name = "panelActions2";
            this.panelActions2.Size = new System.Drawing.Size(1288, 44);
            //
            // labelActions2
            //
            this.labelActions2.AutoSize = false;
            this.labelActions2.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelActions2.Location = new System.Drawing.Point(0, 4);
            this.labelActions2.Name = "labelActions2";
            this.labelActions2.Size = new System.Drawing.Size(150, 36);
            this.labelActions2.Text = "Module 1 · lifetimes:";
            this.labelActions2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // buttonSlowCount  (progress path: the loading guard)
            //
            this.buttonSlowCount.Location = new System.Drawing.Point(150, 4);
            this.buttonSlowCount.Name = "buttonSlowCount";
            this.buttonSlowCount.Size = new System.Drawing.Size(170, 36);
            this.buttonSlowCount.Text = "Slow count (2.5 s)";
            this.buttonSlowCount.ToolTipText = "CountTicketsSlowlyAsync: click Count tickets while it runs and watch the guard ignore the click.";
            this.buttonSlowCount.Click += new System.EventHandler(this.buttonSlowCount_Click);
            //
            // buttonRapid
            //
            this.buttonRapid.Location = new System.Drawing.Point(328, 4);
            this.buttonRapid.Name = "buttonRapid";
            this.buttonRapid.Size = new System.Drawing.Size(190, 36);
            this.buttonRapid.Text = "▶ Count ×3 rapid (guard)";
            this.buttonRapid.ToolTipText = "Three counts started at once: the first runs, the loading flag drops the other two.";
            this.buttonRapid.Click += new System.EventHandler(this.buttonRapid_Click);
            //
            // buttonBreak  (failure path)
            //
            this.buttonBreak.Location = new System.Drawing.Point(542, 4);
            this.buttonBreak.Name = "buttonBreak";
            this.buttonBreak.Size = new System.Drawing.Size(180, 36);
            this.buttonBreak.Text = "Break the database";
            this.buttonBreak.ToolTipText = "Turns the development outage switch on and counts: the catch shows a friendly message, finally restores the buttons.";
            this.buttonBreak.Click += new System.EventHandler(this.buttonBreak_Click);
            //
            // buttonRestore  (recovery)
            //
            this.buttonRestore.Location = new System.Drawing.Point(730, 4);
            this.buttonRestore.Name = "buttonRestore";
            this.buttonRestore.Size = new System.Drawing.Size(160, 36);
            this.buttonRestore.Text = "Restore and count";
            this.buttonRestore.Click += new System.EventHandler(this.buttonRestore_Click);
            //
            // buttonAntiPattern  (what a shared context does)
            //
            this.buttonAntiPattern.Location = new System.Drawing.Point(914, 4);
            this.buttonAntiPattern.Name = "buttonAntiPattern";
            this.buttonAntiPattern.Size = new System.Drawing.Size(248, 36);
            this.buttonAntiPattern.Text = "Two ops, one context (anti-pattern)";
            this.buttonAntiPattern.ToolTipText = "Two concurrent queries on one DbContext instance — what a static/shared context does with two sessions.";
            this.buttonAntiPattern.Click += new System.EventHandler(this.buttonAntiPattern_Click);
            //
            // TicketBrowserPage
            //
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.panelQuery);
            this.Controls.Add(this.panelTrace);
            this.Controls.Add(this.panelActions);
            this.Controls.Add(this.panelActions2);
            this.Name = "TicketBrowserPage";
            this.Size = new System.Drawing.Size(1348, 970);
            this.Text = "Support Desk Data Console — Module 2";
            this.Load += new System.EventHandler(this.TicketBrowserPage_Load);
            this.panelQuery.ResumeLayout(false);
            this.panelTrace.ResumeLayout(false);
            this.panelActions.ResumeLayout(false);
            this.panelActions2.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel panelQuery;
        private Wisej.Web.Label labelTitle;
        private Wisej.Web.Label labelState;
        private Wisej.Web.Label labelLead;
        private Wisej.Web.Button countButton;
        private Wisej.Web.Button btnSeed;
        private Wisej.Web.Label statusLabel;
        private Wisej.Web.Label labelBanner;
        private Wisej.Web.Label labelModelTitle;
        private Wisej.Web.Label labelModel;
        private Wisej.Web.Label labelLifetimesTitle;
        private Wisej.Web.Label labelLifetimes;
        private Wisej.Web.Label labelRule;
        private Wisej.Web.Panel panelTrace;
        private Wisej.Web.Label labelTraceTitle;
        private Wisej.Web.ListBox listTrace;
        private Wisej.Web.Label labelTraceFooter;
        private Wisej.Web.Panel panelActions;
        private Wisej.Web.Button buttonOverlongTitle;
        private Wisej.Web.Button buttonDeleteCustomer;
        private Wisej.Web.Button buttonDeleteAgent;
        private Wisej.Web.Button buttonDeleteTicket;
        private Wisej.Web.Button buttonReset;
        private Wisej.Web.Button buttonClear;
        private Wisej.Web.Panel panelActions2;
        private Wisej.Web.Label labelActions2;
        private Wisej.Web.Button buttonSlowCount;
        private Wisej.Web.Button buttonRapid;
        private Wisej.Web.Button buttonBreak;
        private Wisej.Web.Button buttonRestore;
        private Wisej.Web.Button buttonAntiPattern;
    }
}
