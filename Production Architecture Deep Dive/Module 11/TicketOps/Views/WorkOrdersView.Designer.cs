namespace TicketOps.Views
{
    partial class WorkOrdersView
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
            this.panelScreen = new Wisej.Web.Panel();
            this.labelScreenTitle = new Wisej.Web.Label();
            this.statusBanner = new TicketOps.Controls.StatusBanner();
            this.labelSignedIn = new Wisej.Web.Label();
            this.buttonSignOut = new Wisej.Web.Button();
            this.labelCount = new Wisej.Web.Label();
            this.gridTickets = new Wisej.Web.DataGridView();
            this.columnId = new Wisej.Web.DataGridViewTextBoxColumn();
            this.columnTitle = new Wisej.Web.DataGridViewTextBoxColumn();
            this.columnStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.columnNote = new Wisej.Web.DataGridViewTextBoxColumn();
            this.columnClosedBy = new Wisej.Web.DataGridViewTextBoxColumn();
            this.labelSelected = new Wisej.Web.Label();
            this.textNote = new Wisej.Web.TextBox();
            this.buttonRenderNote = new Wisej.Web.Button();
            this.buttonCloseTicket = new Wisej.Web.Button();
            this.buttonDelete = new Wisej.Web.Button();
            this.labelDeleteHint = new Wisej.Web.Label();
            this.labelNotePlainCaption = new Wisej.Web.Label();
            this.labelNotePlain = new Wisej.Web.Label();
            this.labelNoteAllowListCaption = new Wisej.Web.Label();
            this.labelNoteAllowList = new Wisej.Web.Label();
            this.labelAuditCaption = new Wisej.Web.Label();
            this.listAudit = new Wisej.Web.ListBox();
            this.progressBulk = new Wisej.Web.ProgressBar();
            this.tracePanel = new TicketOps.Diagnostics.ActivityTracePanel();
            this.panelActions = new Wisej.Web.Panel();
            this.buttonBulkNotes = new Wisej.Web.Button();
            this.buttonBypass = new Wisej.Web.Button();
            this.buttonForceEnable = new Wisej.Web.Button();
            this.buttonOutage = new Wisej.Web.Button();
            this.buttonClear = new Wisej.Web.Button();
            this.timerBulk = new Wisej.Web.Timer(this.components);
            this.panelScreen.SuspendLayout();
            this.panelActions.SuspendLayout();
            this.SuspendLayout();
            //
            // panelScreen  (Work Orders: grid, note editor, actions, audit trail — display and input only)
            //
            this.panelScreen.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelScreen.BackColor = System.Drawing.Color.White;
            this.panelScreen.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelScreen.Controls.Add(this.labelScreenTitle);
            this.panelScreen.Controls.Add(this.statusBanner);
            this.panelScreen.Controls.Add(this.labelSignedIn);
            this.panelScreen.Controls.Add(this.buttonSignOut);
            this.panelScreen.Controls.Add(this.labelCount);
            this.panelScreen.Controls.Add(this.gridTickets);
            this.panelScreen.Controls.Add(this.labelSelected);
            this.panelScreen.Controls.Add(this.textNote);
            this.panelScreen.Controls.Add(this.buttonRenderNote);
            this.panelScreen.Controls.Add(this.buttonCloseTicket);
            this.panelScreen.Controls.Add(this.buttonDelete);
            this.panelScreen.Controls.Add(this.labelDeleteHint);
            this.panelScreen.Controls.Add(this.labelNotePlainCaption);
            this.panelScreen.Controls.Add(this.labelNotePlain);
            this.panelScreen.Controls.Add(this.labelNoteAllowListCaption);
            this.panelScreen.Controls.Add(this.labelNoteAllowList);
            this.panelScreen.Controls.Add(this.labelAuditCaption);
            this.panelScreen.Controls.Add(this.listAudit);
            this.panelScreen.Controls.Add(this.progressBulk);
            this.panelScreen.Location = new System.Drawing.Point(30, 30);
            this.panelScreen.Name = "panelScreen";
            this.panelScreen.Size = new System.Drawing.Size(760, 560);
            //
            // labelScreenTitle
            //
            this.labelScreenTitle.AutoSize = false;
            this.labelScreenTitle.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.labelScreenTitle.Location = new System.Drawing.Point(24, 18);
            this.labelScreenTitle.Name = "labelScreenTitle";
            this.labelScreenTitle.Size = new System.Drawing.Size(300, 30);
            this.labelScreenTitle.Text = "Work Orders";
            //
            // statusBanner  (Controls/StatusBanner: "● state" + banner line)
            //
            this.statusBanner.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.statusBanner.Location = new System.Drawing.Point(24, 20);
            this.statusBanner.Name = "statusBanner";
            this.statusBanner.Size = new System.Drawing.Size(712, 58);
            //
            // labelSignedIn  (identity read from the server session — the browser never supplies it)
            //
            this.labelSignedIn.AutoSize = false;
            this.labelSignedIn.Font = new System.Drawing.Font("default", 9.5F, System.Drawing.FontStyle.Bold);
            this.labelSignedIn.ForeColor = System.Drawing.Color.FromArgb(185, 119, 14);
            this.labelSignedIn.Location = new System.Drawing.Point(24, 84);
            this.labelSignedIn.Name = "labelSignedIn";
            this.labelSignedIn.Size = new System.Drawing.Size(560, 24);
            this.labelSignedIn.Text = "Signed in: —";
            //
            // buttonSignOut
            //
            this.buttonSignOut.Location = new System.Drawing.Point(636, 80);
            this.buttonSignOut.Name = "buttonSignOut";
            this.buttonSignOut.Size = new System.Drawing.Size(100, 30);
            this.buttonSignOut.Text = "Sign out";
            this.buttonSignOut.ToolTipText = "Clears the session identity (audited) and returns to the login gate";
            this.buttonSignOut.Click += new System.EventHandler(this.buttonSignOut_Click);
            //
            // labelCount
            //
            this.labelCount.AutoSize = false;
            this.labelCount.Font = new System.Drawing.Font("default", 8.5F);
            this.labelCount.ForeColor = System.Drawing.Color.FromArgb(106, 118, 134);
            this.labelCount.Location = new System.Drawing.Point(24, 108);
            this.labelCount.Name = "labelCount";
            this.labelCount.Size = new System.Drawing.Size(300, 18);
            this.labelCount.Text = "…";
            //
            // gridTickets
            //
            this.gridTickets.AllowUserToAddRows = false;
            this.gridTickets.AllowUserToDeleteRows = false;
            this.gridTickets.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
                this.columnId,
                this.columnTitle,
                this.columnStatus,
                this.columnNote,
                this.columnClosedBy});
            this.gridTickets.Location = new System.Drawing.Point(24, 128);
            this.gridTickets.Name = "gridTickets";
            this.gridTickets.ReadOnly = true;
            this.gridTickets.RowHeadersVisible = false;
            this.gridTickets.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.gridTickets.Size = new System.Drawing.Size(712, 138);
            this.gridTickets.SelectionChanged += new System.EventHandler(this.gridTickets_SelectionChanged);
            //
            // columns
            //
            this.columnId.HeaderText = "#";
            this.columnId.Name = "columnId";
            this.columnId.Width = 64;
            this.columnTitle.HeaderText = "Work order";
            this.columnTitle.Name = "columnTitle";
            this.columnTitle.Width = 300;
            this.columnStatus.HeaderText = "Status";
            this.columnStatus.Name = "columnStatus";
            this.columnStatus.Width = 100;
            this.columnNote.HeaderText = "Note";
            this.columnNote.Name = "columnNote";
            this.columnNote.Width = 110;
            this.columnClosedBy.HeaderText = "Closed by";
            this.columnClosedBy.Name = "columnClosedBy";
            this.columnClosedBy.Width = 120;
            //
            // labelSelected
            //
            this.labelSelected.AutoSize = false;
            this.labelSelected.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelSelected.Location = new System.Drawing.Point(24, 272);
            this.labelSelected.Name = "labelSelected";
            this.labelSelected.Size = new System.Drawing.Size(712, 20);
            this.labelSelected.Text = "No work order selected";
            //
            // textNote  (user-provided text: untrusted by default)
            //
            this.textNote.Location = new System.Drawing.Point(24, 296);
            this.textNote.MaxLength = 500;
            this.textNote.Name = "textNote";
            this.textNote.Size = new System.Drawing.Size(526, 32);
            this.textNote.Watermark = "Ticket note — try: Pump failed <b>again</b> <img src=x onerror=alert(1)>";
            //
            // buttonRenderNote  (success path)
            //
            this.buttonRenderNote.Location = new System.Drawing.Point(558, 296);
            this.buttonRenderNote.Name = "buttonRenderNote";
            this.buttonRenderNote.Size = new System.Drawing.Size(178, 32);
            this.buttonRenderNote.Text = "Render ticket note";
            this.buttonRenderNote.ToolTipText = "Success path: ITicketService.AddNoteAsync stores the text (authorized + audited); the screen renders it with AllowHtml = false and through the allow-list";
            this.buttonRenderNote.Click += new System.EventHandler(this.buttonRenderNote_Click);
            //
            // buttonCloseTicket
            //
            this.buttonCloseTicket.Location = new System.Drawing.Point(24, 336);
            this.buttonCloseTicket.Name = "buttonCloseTicket";
            this.buttonCloseTicket.Size = new System.Drawing.Size(150, 32);
            this.buttonCloseTicket.Text = "Close ticket";
            this.buttonCloseTicket.ToolTipText = "Requires CloseTicket (Supervisor / Admin) — disabled for other roles as a courtesy; TicketService.CloseAsync checks again";
            this.buttonCloseTicket.Click += new System.EventHandler(this.buttonCloseTicket_Click);
            //
            // buttonDelete  (hidden by ApplyPermissionsToControls when the role lacks DeleteTicket — a UX hint)
            //
            this.buttonDelete.Location = new System.Drawing.Point(182, 336);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(210, 32);
            this.buttonDelete.Text = "Delete ticket (Supervisor only)";
            this.buttonDelete.ToolTipText = "Requires DeleteTicket — the handler checks nothing; TicketService.DeleteAsync denies, audits and throws for a Technician";
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            //
            // labelDeleteHint
            //
            this.labelDeleteHint.AutoSize = false;
            this.labelDeleteHint.Font = new System.Drawing.Font("default", 8.5F);
            this.labelDeleteHint.ForeColor = System.Drawing.Color.FromArgb(106, 118, 134);
            this.labelDeleteHint.Location = new System.Drawing.Point(400, 334);
            this.labelDeleteHint.Name = "labelDeleteHint";
            this.labelDeleteHint.Size = new System.Drawing.Size(336, 36);
            this.labelDeleteHint.Text = "";
            //
            // labelNotePlainCaption
            //
            this.labelNotePlainCaption.AutoSize = false;
            this.labelNotePlainCaption.Font = new System.Drawing.Font("default", 8.5F, System.Drawing.FontStyle.Bold);
            this.labelNotePlainCaption.ForeColor = System.Drawing.Color.FromArgb(15, 122, 58);
            this.labelNotePlainCaption.Location = new System.Drawing.Point(24, 378);
            this.labelNotePlainCaption.Name = "labelNotePlainCaption";
            this.labelNotePlainCaption.Size = new System.Drawing.Size(350, 18);
            this.labelNotePlainCaption.Text = "RENDERED NOTE · AllowHtml = false (default) → escaped";
            //
            // labelNotePlain  (AllowHtml = false: user text is shown as text — the safe default)
            //
            this.labelNotePlain.AllowHtml = false;
            this.labelNotePlain.AutoSize = false;
            this.labelNotePlain.BackColor = System.Drawing.Color.FromArgb(246, 249, 252);
            this.labelNotePlain.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.labelNotePlain.Font = new System.Drawing.Font("monospace", 9F);
            this.labelNotePlain.Location = new System.Drawing.Point(24, 398);
            this.labelNotePlain.Name = "labelNotePlain";
            this.labelNotePlain.Padding = new Wisej.Web.Padding(8, 4, 8, 4);
            this.labelNotePlain.Size = new System.Drawing.Size(350, 46);
            this.labelNotePlain.Text = "";
            //
            // labelNoteAllowListCaption
            //
            this.labelNoteAllowListCaption.AutoSize = false;
            this.labelNoteAllowListCaption.Font = new System.Drawing.Font("default", 8.5F, System.Drawing.FontStyle.Bold);
            this.labelNoteAllowListCaption.ForeColor = System.Drawing.Color.FromArgb(185, 119, 14);
            this.labelNoteAllowListCaption.Location = new System.Drawing.Point(386, 378);
            this.labelNoteAllowListCaption.Name = "labelNoteAllowListCaption";
            this.labelNoteAllowListCaption.Size = new System.Drawing.Size(350, 18);
            this.labelNoteAllowListCaption.Text = "ALLOW-LIST · AllowHtml = true, only <b> <i> <br> survive";
            //
            // labelNoteAllowList
            //
            // REVIEWED AllowHtml = true (docs/SafeHtmlPolicy.md): the ONLY HTML-capable surface that shows user text.
            // It never receives raw input — WorkOrdersView.RenderNote assigns HtmlPolicy.RenderWithAllowList(note),
            // which encodes everything and restores only literal <b>, <i>, <br> tags.
            //
            this.labelNoteAllowList.AllowHtml = true;
            this.labelNoteAllowList.AutoSize = false;
            this.labelNoteAllowList.BackColor = System.Drawing.Color.FromArgb(255, 250, 240);
            this.labelNoteAllowList.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.labelNoteAllowList.Font = new System.Drawing.Font("default", 9.5F);
            this.labelNoteAllowList.Location = new System.Drawing.Point(386, 398);
            this.labelNoteAllowList.Name = "labelNoteAllowList";
            this.labelNoteAllowList.Padding = new Wisej.Web.Padding(8, 4, 8, 4);
            this.labelNoteAllowList.Size = new System.Drawing.Size(350, 46);
            this.labelNoteAllowList.Text = "";
            //
            // labelAuditCaption
            //
            this.labelAuditCaption.AutoSize = false;
            this.labelAuditCaption.Font = new System.Drawing.Font("default", 8.5F, System.Drawing.FontStyle.Bold);
            this.labelAuditCaption.ForeColor = System.Drawing.Color.FromArgb(74, 90, 106);
            this.labelAuditCaption.Location = new System.Drawing.Point(24, 452);
            this.labelAuditCaption.Name = "labelAuditCaption";
            this.labelAuditCaption.Size = new System.Drawing.Size(712, 18);
            this.labelAuditCaption.Text = "AUDIT TRAIL · who / what / target / when (server, append-only)";
            //
            // listAudit  (the ListBox escapes item text; it only ever receives AuditLog.Format output, which holds no user text)
            //
            this.listAudit.Font = new System.Drawing.Font("monospace", 8.5F);
            this.listAudit.Location = new System.Drawing.Point(24, 472);
            this.listAudit.Name = "listAudit";
            this.listAudit.Size = new System.Drawing.Size(712, 72);
            //
            // progressBulk
            //
            this.progressBulk.Location = new System.Drawing.Point(560, 108);
            this.progressBulk.Maximum = 20;
            this.progressBulk.Name = "progressBulk";
            this.progressBulk.Size = new System.Drawing.Size(176, 14);
            this.progressBulk.Visible = false;
            //
            // tracePanel  (Diagnostics: the live activity trace)
            //
            this.tracePanel.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Right;
            this.tracePanel.Location = new System.Drawing.Point(810, 30);
            this.tracePanel.Name = "tracePanel";
            this.tracePanel.Size = new System.Drawing.Size(508, 560);
            this.tracePanel.Title = "Activity trace · UI → Service → Data · [SESSION] · [AUDIT]";
            //
            // panelActions  (bottom bar: progress / bypass proofs / outage + recovery / clear)
            //
            this.panelActions.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelActions.Controls.Add(this.buttonBulkNotes);
            this.panelActions.Controls.Add(this.buttonBypass);
            this.panelActions.Controls.Add(this.buttonForceEnable);
            this.panelActions.Controls.Add(this.buttonOutage);
            this.panelActions.Controls.Add(this.buttonClear);
            this.panelActions.Location = new System.Drawing.Point(30, 606);
            this.panelActions.Name = "panelActions";
            this.panelActions.Size = new System.Drawing.Size(1288, 44);
            //
            // buttonBulkNotes  (progress path)
            //
            this.buttonBulkNotes.Location = new System.Drawing.Point(0, 4);
            this.buttonBulkNotes.Name = "buttonBulkNotes";
            this.buttonBulkNotes.Size = new System.Drawing.Size(170, 36);
            this.buttonBulkNotes.Text = "▶ Add 20 notes";
            this.buttonBulkNotes.ToolTipText = "Progress path: a Timer adds 5 notes per tick through AddNoteAsync — every one authorized and audited; the audit trail grows by 20";
            this.buttonBulkNotes.Click += new System.EventHandler(this.buttonBulkNotes_Click);
            //
            // buttonBypass  (failure path 1: permission — the service is called directly)
            //
            this.buttonBypass.Location = new System.Drawing.Point(180, 4);
            this.buttonBypass.Name = "buttonBypass";
            this.buttonBypass.Size = new System.Drawing.Size(300, 36);
            this.buttonBypass.Text = "Call DeleteAsync directly (button hidden)";
            this.buttonBypass.ToolTipText = "Failure path (permission): calls ITicketService.DeleteAsync(#2002) without the Delete button — as a Technician the service denies, audits ⛔ and throws; as a Supervisor it deletes";
            this.buttonBypass.Click += new System.EventHandler(this.buttonBypass_Click);
            //
            // buttonForceEnable  (failure path 2: permission — the hidden button is forced back)
            //
            this.buttonForceEnable.Location = new System.Drawing.Point(490, 4);
            this.buttonForceEnable.Name = "buttonForceEnable";
            this.buttonForceEnable.Size = new System.Drawing.Size(250, 36);
            this.buttonForceEnable.Text = "Force-enable Delete (DevTools)";
            this.buttonForceEnable.ToolTipText = "Failure path (permission): makes the hidden Delete button visible again, as \"btnDelete.disabled = false\" in the browser console would — then click Delete and watch the service refuse";
            this.buttonForceEnable.Click += new System.EventHandler(this.buttonForceEnable_Click);
            //
            // buttonOutage  (error path + recovery)
            //
            this.buttonOutage.Location = new System.Drawing.Point(750, 4);
            this.buttonOutage.Name = "buttonOutage";
            this.buttonOutage.Size = new System.Drawing.Size(200, 36);
            this.buttonOutage.Text = "Simulate data outage";
            this.buttonOutage.ToolTipText = "Error path: the repository throws like a real driver (✖ in DATA with sql01:1433); the user sees only the safe message. Click again to recover";
            this.buttonOutage.Click += new System.EventHandler(this.buttonOutage_Click);
            //
            // buttonClear
            //
            this.buttonClear.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.buttonClear.Location = new System.Drawing.Point(1148, 4);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(140, 36);
            this.buttonClear.Text = "Clear trace";
            this.buttonClear.ToolTipText = "Empties the activity trace (the audit trail is append-only and is not affected)";
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            //
            // timerBulk
            //
            this.timerBulk.Interval = 250;
            this.timerBulk.Tick += new System.EventHandler(this.timerBulk_Tick);
            //
            // WorkOrdersView
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.ClientSize = new System.Drawing.Size(1348, 680);
            this.Controls.Add(this.panelScreen);
            this.Controls.Add(this.tracePanel);
            this.Controls.Add(this.panelActions);
            this.Name = "WorkOrdersView";
            this.Text = "TicketOps Console — Module 11 · Work Orders";
            this.Load += new System.EventHandler(this.WorkOrdersView_Load);
            this.FormClosed += new Wisej.Web.FormClosedEventHandler(this.WorkOrdersView_FormClosed);
            this.panelScreen.ResumeLayout(false);
            this.panelActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel panelScreen;
        private Wisej.Web.Label labelScreenTitle;
        private TicketOps.Controls.StatusBanner statusBanner;
        private Wisej.Web.Label labelSignedIn;
        private Wisej.Web.Button buttonSignOut;
        private Wisej.Web.Label labelCount;
        private Wisej.Web.DataGridView gridTickets;
        private Wisej.Web.DataGridViewTextBoxColumn columnId;
        private Wisej.Web.DataGridViewTextBoxColumn columnTitle;
        private Wisej.Web.DataGridViewTextBoxColumn columnStatus;
        private Wisej.Web.DataGridViewTextBoxColumn columnNote;
        private Wisej.Web.DataGridViewTextBoxColumn columnClosedBy;
        private Wisej.Web.Label labelSelected;
        private Wisej.Web.TextBox textNote;
        private Wisej.Web.Button buttonRenderNote;
        private Wisej.Web.Button buttonCloseTicket;
        private Wisej.Web.Button buttonDelete;
        private Wisej.Web.Label labelDeleteHint;
        private Wisej.Web.Label labelNotePlainCaption;
        private Wisej.Web.Label labelNotePlain;
        private Wisej.Web.Label labelNoteAllowListCaption;
        private Wisej.Web.Label labelNoteAllowList;
        private Wisej.Web.Label labelAuditCaption;
        private Wisej.Web.ListBox listAudit;
        private Wisej.Web.ProgressBar progressBulk;
        private TicketOps.Diagnostics.ActivityTracePanel tracePanel;
        private Wisej.Web.Panel panelActions;
        private Wisej.Web.Button buttonBulkNotes;
        private Wisej.Web.Button buttonBypass;
        private Wisej.Web.Button buttonForceEnable;
        private Wisej.Web.Button buttonOutage;
        private Wisej.Web.Button buttonClear;
        private Wisej.Web.Timer timerBulk;
    }
}
