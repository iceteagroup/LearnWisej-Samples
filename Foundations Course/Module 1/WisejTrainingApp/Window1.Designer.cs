namespace WisejTrainingApp
{
    partial class Window1
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
            this.panelGreet = new Wisej.Web.Panel();
            this.labelGreetCard = new Wisej.Web.Label();
            this.lblTitle = new Wisej.Web.Label();
            this.lblPrompt = new Wisej.Web.Label();
            this.txtName = new Wisej.Web.TextBox();
            this.btnSayHello = new Wisej.Web.Button();
            this.lblStatus = new Wisej.Web.Label();
            this.lblRunState = new Wisej.Web.Label();
            this.lblHint = new Wisej.Web.Label();
            this.panelFiles = new Wisej.Web.Panel();
            this.labelFilesCard = new Wisej.Web.Label();
            this.lstFiles = new Wisej.Web.ListBox();
            this.btnInspectFiles = new Wisej.Web.Button();
            this.lblFilesPath = new Wisej.Web.Label();
            this.panelLog = new Wisej.Web.Panel();
            this.labelLogCard = new Wisej.Web.Label();
            this.lstEventLog = new Wisej.Web.ListBox();
            this.labelLogFooter = new Wisej.Web.Label();
            this.panelTicket = new Wisej.Web.Panel();
            this.labelTicketCard = new Wisej.Web.Label();
            this.lblTicketTitleCaption = new Wisej.Web.Label();
            this.txtTicketTitle = new Wisej.Web.TextBox();
            this.lblTicketCustomerCaption = new Wisej.Web.Label();
            this.txtTicketCustomer = new Wisej.Web.TextBox();
            this.btnSaveTicket = new Wisej.Web.Button();
            this.lstTickets = new Wisej.Web.ListBox();
            this.lblTicketHint = new Wisej.Web.Label();
            this.panelActions = new Wisej.Web.Panel();
            this.btnTryBlank = new Wisej.Web.Button();
            this.btnFillSample = new Wisej.Web.Button();
            this.btnTryEmptyTicket = new Wisej.Web.Button();
            this.btnClearLog = new Wisej.Web.Button();
            this.panelGreet.SuspendLayout();
            this.panelFiles.SuspendLayout();
            this.panelLog.SuspendLayout();
            this.panelTicket.SuspendLayout();
            this.panelActions.SuspendLayout();
            this.SuspendLayout();
            //
            // panelGreet  (the "Say hello" card — the readable handler from the lesson lives here)
            //
            this.panelGreet.BackColor = System.Drawing.Color.White;
            this.panelGreet.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelGreet.Controls.Add(this.labelGreetCard);
            this.panelGreet.Controls.Add(this.lblTitle);
            this.panelGreet.Controls.Add(this.lblPrompt);
            this.panelGreet.Controls.Add(this.txtName);
            this.panelGreet.Controls.Add(this.btnSayHello);
            this.panelGreet.Controls.Add(this.lblStatus);
            this.panelGreet.Controls.Add(this.lblRunState);
            this.panelGreet.Controls.Add(this.lblHint);
            this.panelGreet.Location = new System.Drawing.Point(30, 30);
            this.panelGreet.Name = "panelGreet";
            this.panelGreet.Size = new System.Drawing.Size(560, 330);
            //
            // labelGreetCard
            //
            this.labelGreetCard.AutoSize = false;
            this.labelGreetCard.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelGreetCard.Location = new System.Drawing.Point(24, 14);
            this.labelGreetCard.Name = "labelGreetCard";
            this.labelGreetCard.Size = new System.Drawing.Size(512, 28);
            this.labelGreetCard.Text = "Say hello  ·  design → name → handle event → run";
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = false;
            this.lblTitle.Font = new System.Drawing.Font("default", 20F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(24, 50);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(512, 40);
            this.lblTitle.Text = "Hello, Wisej.NET";
            //
            // lblPrompt
            //
            this.lblPrompt.AutoSize = false;
            this.lblPrompt.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblPrompt.Location = new System.Drawing.Point(24, 100);
            this.lblPrompt.Name = "lblPrompt";
            this.lblPrompt.Size = new System.Drawing.Size(380, 22);
            this.lblPrompt.Text = "Your name";
            //
            // txtName
            //
            this.txtName.Font = new System.Drawing.Font("default", 11F);
            this.txtName.Location = new System.Drawing.Point(24, 126);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(380, 38);
            this.txtName.Watermark = "Type a name and press Enter";
            this.txtName.KeyDown += new Wisej.Web.KeyEventHandler(this.txtName_KeyDown);
            //
            // btnSayHello  (double-click in the Designer created btnSayHello_Click)
            //
            this.btnSayHello.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.btnSayHello.Location = new System.Drawing.Point(416, 126);
            this.btnSayHello.Name = "btnSayHello";
            this.btnSayHello.Size = new System.Drawing.Size(120, 38);
            this.btnSayHello.Text = "Say Hello";
            this.btnSayHello.ToolTipText = "Runs btnSayHello_Click on the server: read txtName, validate, update lblStatus.";
            this.btnSayHello.Click += new System.EventHandler(this.btnSayHello_Click);
            //
            // lblStatus  (the second label: the lesson's output label, written by both examples)
            //
            this.lblStatus.AutoSize = false;
            this.lblStatus.BackColor = System.Drawing.Color.FromArgb(247, 249, 252);
            this.lblStatus.Font = new System.Drawing.Font("default", 13F, System.Drawing.FontStyle.Bold);
            this.lblStatus.Location = new System.Drawing.Point(24, 182);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.lblStatus.Size = new System.Drawing.Size(512, 48);
            this.lblStatus.Text = "…";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblRunState  (this sample's own traffic light — not part of the lesson's four controls)
            //
            this.lblRunState.AutoSize = false;
            this.lblRunState.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblRunState.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.lblRunState.Location = new System.Drawing.Point(24, 244);
            this.lblRunState.Name = "lblRunState";
            this.lblRunState.Size = new System.Drawing.Size(512, 26);
            this.lblRunState.Text = "● ready — type a name and click Say Hello";
            //
            // lblHint
            //
            this.lblHint.AutoSize = false;
            this.lblHint.Font = new System.Drawing.Font("monospace", 9F);
            this.lblHint.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblHint.Location = new System.Drawing.Point(24, 276);
            this.lblHint.Name = "lblHint";
            this.lblHint.Size = new System.Drawing.Size(512, 44);
            this.lblHint.Text = "Controls: lblTitle · txtName · btnSayHello · lblStatus\nHandler:  btnSayHello_Click in Window1.cs (Trim + IsNullOrWhiteSpace guard)";
            this.lblHint.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // panelFiles  (the lesson's "Inspect the solution structure")
            //
            this.panelFiles.BackColor = System.Drawing.Color.White;
            this.panelFiles.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelFiles.Controls.Add(this.labelFilesCard);
            this.panelFiles.Controls.Add(this.lstFiles);
            this.panelFiles.Controls.Add(this.btnInspectFiles);
            this.panelFiles.Controls.Add(this.lblFilesPath);
            this.panelFiles.Location = new System.Drawing.Point(30, 378);
            this.panelFiles.Name = "panelFiles";
            this.panelFiles.Size = new System.Drawing.Size(560, 232);
            //
            // labelFilesCard
            //
            this.labelFilesCard.AutoSize = false;
            this.labelFilesCard.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelFilesCard.Location = new System.Drawing.Point(24, 14);
            this.labelFilesCard.Name = "labelFilesCard";
            this.labelFilesCard.Size = new System.Drawing.Size(320, 28);
            this.labelFilesCard.Text = "Solution structure  ·  what each file is for";
            //
            // btnInspectFiles
            //
            this.btnInspectFiles.Location = new System.Drawing.Point(356, 12);
            this.btnInspectFiles.Name = "btnInspectFiles";
            this.btnInspectFiles.Size = new System.Drawing.Size(180, 32);
            this.btnInspectFiles.Text = "Inspect project files";
            this.btnInspectFiles.ToolTipText = "Checks that each file exists on disk (Application.StartupPath) and describes its job.";
            this.btnInspectFiles.Click += new System.EventHandler(this.btnInspectFiles_Click);
            //
            // lstFiles
            //
            this.lstFiles.Font = new System.Drawing.Font("monospace", 9F);
            this.lstFiles.Location = new System.Drawing.Point(24, 50);
            this.lstFiles.Name = "lstFiles";
            this.lstFiles.Size = new System.Drawing.Size(512, 146);
            //
            // lblFilesPath
            //
            this.lblFilesPath.AutoSize = false;
            this.lblFilesPath.AutoEllipsis = true;
            this.lblFilesPath.Font = new System.Drawing.Font("monospace", 8F);
            this.lblFilesPath.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblFilesPath.Location = new System.Drawing.Point(24, 200);
            this.lblFilesPath.Name = "lblFilesPath";
            this.lblFilesPath.Size = new System.Drawing.Size(512, 22);
            this.lblFilesPath.Text = "";
            //
            // panelLog  (Event log · what the code did)
            //
            this.panelLog.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelLog.BackColor = System.Drawing.Color.White;
            this.panelLog.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelLog.Controls.Add(this.labelLogCard);
            this.panelLog.Controls.Add(this.lstEventLog);
            this.panelLog.Controls.Add(this.labelLogFooter);
            this.panelLog.Location = new System.Drawing.Point(618, 30);
            this.panelLog.Name = "panelLog";
            this.panelLog.Size = new System.Drawing.Size(700, 330);
            //
            // labelLogCard
            //
            this.labelLogCard.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelLogCard.AutoSize = false;
            this.labelLogCard.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelLogCard.Location = new System.Drawing.Point(20, 14);
            this.labelLogCard.Name = "labelLogCard";
            this.labelLogCard.Size = new System.Drawing.Size(660, 30);
            this.labelLogCard.Text = "Event log  ·  what the server-side code did";
            //
            // lstEventLog
            //
            this.lstEventLog.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lstEventLog.Font = new System.Drawing.Font("monospace", 9F);
            this.lstEventLog.Location = new System.Drawing.Point(20, 52);
            this.lstEventLog.Name = "lstEventLog";
            this.lstEventLog.Size = new System.Drawing.Size(660, 226);
            //
            // labelLogFooter
            //
            this.labelLogFooter.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelLogFooter.AutoSize = false;
            this.labelLogFooter.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelLogFooter.Location = new System.Drawing.Point(20, 288);
            this.labelLogFooter.Name = "labelLogFooter";
            this.labelLogFooter.Size = new System.Drawing.Size(660, 26);
            this.labelLogFooter.Text = "browser → event → C# handler on the server → UI updated → browser refreshed";
            //
            // panelTicket  (the lesson's second example: the click stays readable, the service owns the rules)
            //
            this.panelTicket.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelTicket.BackColor = System.Drawing.Color.White;
            this.panelTicket.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelTicket.Controls.Add(this.labelTicketCard);
            this.panelTicket.Controls.Add(this.lblTicketTitleCaption);
            this.panelTicket.Controls.Add(this.txtTicketTitle);
            this.panelTicket.Controls.Add(this.lblTicketCustomerCaption);
            this.panelTicket.Controls.Add(this.txtTicketCustomer);
            this.panelTicket.Controls.Add(this.btnSaveTicket);
            this.panelTicket.Controls.Add(this.lstTickets);
            this.panelTicket.Controls.Add(this.lblTicketHint);
            this.panelTicket.Location = new System.Drawing.Point(618, 378);
            this.panelTicket.Name = "panelTicket";
            this.panelTicket.Size = new System.Drawing.Size(700, 232);
            //
            // labelTicketCard
            //
            this.labelTicketCard.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelTicketCard.AutoSize = false;
            this.labelTicketCard.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelTicketCard.Location = new System.Drawing.Point(20, 14);
            this.labelTicketCard.Name = "labelTicketCard";
            this.labelTicketCard.Size = new System.Drawing.Size(660, 28);
            this.labelTicketCard.Text = "Separate the business logic  ·  Models/ + Services/";
            //
            // lblTicketTitleCaption
            //
            this.lblTicketTitleCaption.AutoSize = false;
            this.lblTicketTitleCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblTicketTitleCaption.Location = new System.Drawing.Point(20, 48);
            this.lblTicketTitleCaption.Name = "lblTicketTitleCaption";
            this.lblTicketTitleCaption.Size = new System.Drawing.Size(300, 20);
            this.lblTicketTitleCaption.Text = "Ticket title";
            //
            // txtTicketTitle
            //
            this.txtTicketTitle.Location = new System.Drawing.Point(20, 70);
            this.txtTicketTitle.Name = "txtTicketTitle";
            this.txtTicketTitle.Size = new System.Drawing.Size(300, 34);
            this.txtTicketTitle.Watermark = "Short summary of the problem";
            //
            // lblTicketCustomerCaption
            //
            this.lblTicketCustomerCaption.AutoSize = false;
            this.lblTicketCustomerCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblTicketCustomerCaption.Location = new System.Drawing.Point(332, 48);
            this.lblTicketCustomerCaption.Name = "lblTicketCustomerCaption";
            this.lblTicketCustomerCaption.Size = new System.Drawing.Size(200, 20);
            this.lblTicketCustomerCaption.Text = "Customer";
            //
            // txtTicketCustomer
            //
            this.txtTicketCustomer.Location = new System.Drawing.Point(332, 70);
            this.txtTicketCustomer.Name = "txtTicketCustomer";
            this.txtTicketCustomer.Size = new System.Drawing.Size(200, 34);
            this.txtTicketCustomer.Watermark = "Who reported it";
            //
            // btnSaveTicket  (the lesson's second handler, by name)
            //
            this.btnSaveTicket.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.btnSaveTicket.Location = new System.Drawing.Point(544, 70);
            this.btnSaveTicket.Name = "btnSaveTicket";
            this.btnSaveTicket.Size = new System.Drawing.Size(136, 34);
            this.btnSaveTicket.Text = "Save Ticket";
            this.btnSaveTicket.ToolTipText = "btnSaveTicket_Click → ValidateInput() → ReadTicketFromScreen() → ticketService.Save(ticket).";
            this.btnSaveTicket.Click += new System.EventHandler(this.btnSaveTicket_Click);
            //
            // lstTickets
            //
            this.lstTickets.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lstTickets.Font = new System.Drawing.Font("monospace", 9F);
            this.lstTickets.Location = new System.Drawing.Point(20, 116);
            this.lstTickets.Name = "lstTickets";
            this.lstTickets.Size = new System.Drawing.Size(660, 68);
            //
            // lblTicketHint
            //
            this.lblTicketHint.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblTicketHint.AutoSize = false;
            this.lblTicketHint.Font = new System.Drawing.Font("monospace", 9F);
            this.lblTicketHint.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblTicketHint.Location = new System.Drawing.Point(20, 190);
            this.lblTicketHint.Name = "lblTicketHint";
            this.lblTicketHint.Size = new System.Drawing.Size(660, 32);
            this.lblTicketHint.Text = "btnSaveTicket_Click → ValidateInput() → ReadTicketFromScreen() → ticketService.Save(ticket) → lblStatus\nModels/Ticket.cs holds the shape · Services/TicketService.cs holds the rule · the click stays 4 lines";
            this.lblTicketHint.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // panelActions
            //
            this.panelActions.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelActions.Controls.Add(this.btnTryBlank);
            this.panelActions.Controls.Add(this.btnFillSample);
            this.panelActions.Controls.Add(this.btnTryEmptyTicket);
            this.panelActions.Controls.Add(this.btnClearLog);
            this.panelActions.Location = new System.Drawing.Point(30, 626);
            this.panelActions.Name = "panelActions";
            this.panelActions.Size = new System.Drawing.Size(1288, 44);
            //
            // btnTryBlank  (failure path: the validation guard)
            //
            this.btnTryBlank.Location = new System.Drawing.Point(0, 4);
            this.btnTryBlank.Name = "btnTryBlank";
            this.btnTryBlank.Size = new System.Drawing.Size(220, 36);
            this.btnTryBlank.Text = "Try a blank name (validation)";
            this.btnTryBlank.ToolTipText = "Clears txtName and runs the same Click handler — the IsNullOrWhiteSpace guard answers.";
            this.btnTryBlank.Click += new System.EventHandler(this.btnTryBlank_Click);
            //
            // btnFillSample  (recovery: a valid input through the same handler)
            //
            this.btnFillSample.Location = new System.Drawing.Point(228, 4);
            this.btnFillSample.Name = "btnFillSample";
            this.btnFillSample.Size = new System.Drawing.Size(220, 36);
            this.btnFillSample.Text = "Fill a sample name and greet";
            this.btnFillSample.Click += new System.EventHandler(this.btnFillSample_Click);
            //
            // btnTryEmptyTicket  (failure path for the second example: ValidateInput() blocks the save)
            //
            this.btnTryEmptyTicket.Location = new System.Drawing.Point(456, 4);
            this.btnTryEmptyTicket.Name = "btnTryEmptyTicket";
            this.btnTryEmptyTicket.Size = new System.Drawing.Size(240, 36);
            this.btnTryEmptyTicket.Text = "Save an empty ticket (validation)";
            this.btnTryEmptyTicket.ToolTipText = "Clears the ticket fields and runs btnSaveTicket_Click — ValidateInput() stops it before the service.";
            this.btnTryEmptyTicket.Click += new System.EventHandler(this.btnTryEmptyTicket_Click);
            //
            // btnClearLog
            //
            this.btnClearLog.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.btnClearLog.Location = new System.Drawing.Point(1178, 4);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(110, 36);
            this.btnClearLog.Text = "Clear log";
            this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);
            //
            // Window1
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.ClientSize = new System.Drawing.Size(1348, 680);
            this.Controls.Add(this.panelGreet);
            this.Controls.Add(this.panelFiles);
            this.Controls.Add(this.panelLog);
            this.Controls.Add(this.panelTicket);
            this.Controls.Add(this.panelActions);
            this.Name = "Window1";
            this.Text = "WisejTrainingApp — First app (Module 1)";
            this.Load += new System.EventHandler(this.Window1_Load);
            this.panelGreet.ResumeLayout(false);
            this.panelFiles.ResumeLayout(false);
            this.panelLog.ResumeLayout(false);
            this.panelTicket.ResumeLayout(false);
            this.panelActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel panelGreet;
        private Wisej.Web.Label labelGreetCard;
        private Wisej.Web.Label lblTitle;
        private Wisej.Web.Label lblPrompt;
        private Wisej.Web.TextBox txtName;
        private Wisej.Web.Button btnSayHello;
        private Wisej.Web.Label lblStatus;
        private Wisej.Web.Label lblRunState;
        private Wisej.Web.Label lblHint;
        private Wisej.Web.Panel panelFiles;
        private Wisej.Web.Label labelFilesCard;
        private Wisej.Web.ListBox lstFiles;
        private Wisej.Web.Button btnInspectFiles;
        private Wisej.Web.Label lblFilesPath;
        private Wisej.Web.Panel panelLog;
        private Wisej.Web.Label labelLogCard;
        private Wisej.Web.ListBox lstEventLog;
        private Wisej.Web.Label labelLogFooter;
        private Wisej.Web.Panel panelTicket;
        private Wisej.Web.Label labelTicketCard;
        private Wisej.Web.Label lblTicketTitleCaption;
        private Wisej.Web.TextBox txtTicketTitle;
        private Wisej.Web.Label lblTicketCustomerCaption;
        private Wisej.Web.TextBox txtTicketCustomer;
        private Wisej.Web.Button btnSaveTicket;
        private Wisej.Web.ListBox lstTickets;
        private Wisej.Web.Label lblTicketHint;
        private Wisej.Web.Panel panelActions;
        private Wisej.Web.Button btnTryBlank;
        private Wisej.Web.Button btnFillSample;
        private Wisej.Web.Button btnTryEmptyTicket;
        private Wisej.Web.Button btnClearLog;
    }
}
