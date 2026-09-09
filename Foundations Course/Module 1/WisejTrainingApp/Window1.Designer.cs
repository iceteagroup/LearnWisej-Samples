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
            this.btnGreet = new Wisej.Web.Button();
            this.lblResult = new Wisej.Web.Label();
            this.lblStatus = new Wisej.Web.Label();
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
            this.panelActions = new Wisej.Web.Panel();
            this.btnTryBlank = new Wisej.Web.Button();
            this.btnFillSample = new Wisej.Web.Button();
            this.btnClearLog = new Wisej.Web.Button();
            this.panelGreet.SuspendLayout();
            this.panelFiles.SuspendLayout();
            this.panelLog.SuspendLayout();
            this.panelActions.SuspendLayout();
            this.SuspendLayout();
            //
            // panelGreet  (the "Say hello" card — the lab's four controls live here)
            //
            this.panelGreet.BackColor = System.Drawing.Color.White;
            this.panelGreet.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelGreet.Controls.Add(this.labelGreetCard);
            this.panelGreet.Controls.Add(this.lblTitle);
            this.panelGreet.Controls.Add(this.lblPrompt);
            this.panelGreet.Controls.Add(this.txtName);
            this.panelGreet.Controls.Add(this.btnGreet);
            this.panelGreet.Controls.Add(this.lblResult);
            this.panelGreet.Controls.Add(this.lblStatus);
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
            // btnGreet  (double-click in the Designer created btnGreet_Click)
            //
            this.btnGreet.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.btnGreet.Location = new System.Drawing.Point(416, 126);
            this.btnGreet.Name = "btnGreet";
            this.btnGreet.Size = new System.Drawing.Size(120, 38);
            this.btnGreet.Text = "Say Hello";
            this.btnGreet.ToolTipText = "Runs btnGreet_Click on the server: read txtName, validate, update lblResult.";
            this.btnGreet.Click += new System.EventHandler(this.btnGreet_Click);
            //
            // lblResult  (the second label: output)
            //
            this.lblResult.AutoSize = false;
            this.lblResult.BackColor = System.Drawing.Color.FromArgb(247, 249, 252);
            this.lblResult.Font = new System.Drawing.Font("default", 13F, System.Drawing.FontStyle.Bold);
            this.lblResult.Location = new System.Drawing.Point(24, 182);
            this.lblResult.Name = "lblResult";
            this.lblResult.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.lblResult.Size = new System.Drawing.Size(512, 48);
            this.lblResult.Text = "…";
            this.lblResult.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = false;
            this.lblStatus.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.lblStatus.Location = new System.Drawing.Point(24, 244);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(512, 26);
            this.lblStatus.Text = "● ready — type a name and click Say Hello";
            //
            // lblHint
            //
            this.lblHint.AutoSize = false;
            this.lblHint.Font = new System.Drawing.Font("monospace", 9F);
            this.lblHint.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblHint.Location = new System.Drawing.Point(24, 276);
            this.lblHint.Name = "lblHint";
            this.lblHint.Size = new System.Drawing.Size(512, 44);
            this.lblHint.Text = "Controls: lblPrompt · txtName · btnGreet · lblResult\nHandler:  btnGreet_Click in Window1.cs (Trim + IsNullOrWhiteSpace guard)";
            this.lblHint.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // panelFiles  (lab step 7: inspect the project files)
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
            this.labelFilesCard.Text = "Inspect files  ·  what each file is for";
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
            this.panelLog.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelLog.BackColor = System.Drawing.Color.White;
            this.panelLog.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelLog.Controls.Add(this.labelLogCard);
            this.panelLog.Controls.Add(this.lstEventLog);
            this.panelLog.Controls.Add(this.labelLogFooter);
            this.panelLog.Location = new System.Drawing.Point(618, 30);
            this.panelLog.Name = "panelLog";
            this.panelLog.Size = new System.Drawing.Size(700, 580);
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
            this.lstEventLog.Size = new System.Drawing.Size(660, 476);
            //
            // labelLogFooter
            //
            this.labelLogFooter.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelLogFooter.AutoSize = false;
            this.labelLogFooter.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelLogFooter.Location = new System.Drawing.Point(20, 538);
            this.labelLogFooter.Name = "labelLogFooter";
            this.labelLogFooter.Size = new System.Drawing.Size(660, 26);
            this.labelLogFooter.Text = "browser → event → C# handler on the server → UI updated → browser refreshed";
            //
            // panelActions
            //
            this.panelActions.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelActions.Controls.Add(this.btnTryBlank);
            this.panelActions.Controls.Add(this.btnFillSample);
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
            this.Controls.Add(this.panelActions);
            this.Name = "Window1";
            this.Text = "WisejTrainingApp — First app (Module 1)";
            this.Load += new System.EventHandler(this.Window1_Load);
            this.panelGreet.ResumeLayout(false);
            this.panelFiles.ResumeLayout(false);
            this.panelLog.ResumeLayout(false);
            this.panelActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel panelGreet;
        private Wisej.Web.Label labelGreetCard;
        private Wisej.Web.Label lblTitle;
        private Wisej.Web.Label lblPrompt;
        private Wisej.Web.TextBox txtName;
        private Wisej.Web.Button btnGreet;
        private Wisej.Web.Label lblResult;
        private Wisej.Web.Label lblStatus;
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
        private Wisej.Web.Panel panelActions;
        private Wisej.Web.Button btnTryBlank;
        private Wisej.Web.Button btnFillSample;
        private Wisej.Web.Button btnClearLog;
    }
}
