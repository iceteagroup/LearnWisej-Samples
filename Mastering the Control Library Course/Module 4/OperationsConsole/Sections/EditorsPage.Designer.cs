namespace OperationsConsole.Sections
{
    partial class EditorsPage
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
            this.pnlEditorHost = new Wisej.Web.Panel();
            this.customerEditor = new OperationsConsole.Editors.CustomerEditor();
            this.pnlCommands = new Wisej.Web.Panel();
            this.lblCommandsTitle = new Wisej.Web.Label();
            this.btnLoadSample = new Wisej.Web.Button();
            this.btnLoadInvalidSample = new Wisej.Web.Button();
            this.btnSaveTwice = new Wisej.Web.Button();
            this.chkSimulateFailure = new Wisej.Web.CheckBox();
            this.lblCommandsHint = new Wisej.Web.Label();
            this.pnlHeader = new Wisej.Web.Panel();
            this.lblTitle = new Wisej.Web.Label();
            this.lblModule = new Wisej.Web.Label();
            this.pnlEditorHost.SuspendLayout();
            this.pnlCommands.SuspendLayout();
            this.pnlHeader.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlEditorHost  (Dock = Fill — added FIRST so the Top / Bottom bars claim their space first)
            //
            this.pnlEditorHost.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.pnlEditorHost.Controls.Add(this.customerEditor);
            this.pnlEditorHost.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlEditorHost.Name = "pnlEditorHost";
            this.pnlEditorHost.Size = new System.Drawing.Size(740, 516);
            //
            // customerEditor  (the reusable UserControl this module builds — the page uses only its public surface)
            //
            this.customerEditor.Dock = Wisej.Web.DockStyle.Fill;
            this.customerEditor.Name = "customerEditor";
            this.customerEditor.Size = new System.Drawing.Size(740, 516);
            this.customerEditor.TabIndex = 1;
            //
            // pnlCommands  (Dock = Bottom — the command row that makes every path of the lab one click)
            //
            this.pnlCommands.BackColor = System.Drawing.Color.White;
            this.pnlCommands.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlCommands.Controls.Add(this.lblCommandsTitle);
            this.pnlCommands.Controls.Add(this.btnLoadSample);
            this.pnlCommands.Controls.Add(this.btnLoadInvalidSample);
            this.pnlCommands.Controls.Add(this.btnSaveTwice);
            this.pnlCommands.Controls.Add(this.chkSimulateFailure);
            this.pnlCommands.Controls.Add(this.lblCommandsHint);
            this.pnlCommands.Dock = Wisej.Web.DockStyle.Bottom;
            this.pnlCommands.Name = "pnlCommands";
            this.pnlCommands.Size = new System.Drawing.Size(740, 112);
            //
            // lblCommandsTitle
            //
            this.lblCommandsTitle.AutoSize = false;
            this.lblCommandsTitle.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblCommandsTitle.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblCommandsTitle.Location = new System.Drawing.Point(20, 8);
            this.lblCommandsTitle.Name = "lblCommandsTitle";
            this.lblCommandsTitle.Size = new System.Drawing.Size(560, 22);
            this.lblCommandsTitle.Text = "COMMAND ROW — EVERY PATH OF THE LAB IS ONE CLICK";
            this.lblCommandsTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // btnLoadSample  (success path: a record that passes every rule)
            //
            this.btnLoadSample.AccessibleName = "Load a valid sample customer";
            this.btnLoadSample.Location = new System.Drawing.Point(20, 34);
            this.btnLoadSample.Name = "btnLoadSample";
            this.btnLoadSample.Size = new System.Drawing.Size(140, 32);
            this.btnLoadSample.TabIndex = 10;
            this.btnLoadSample.Text = "Load sample";
            this.btnLoadSample.ToolTipText = "Fills the editor with a customer that passes every rule, ready for Validate and Save.";
            this.btnLoadSample.Click += new System.EventHandler(this.btnLoadSample_Click);
            //
            // btnLoadInvalidSample  (failure paths: four broken rules at once)
            //
            this.btnLoadInvalidSample.AccessibleName = "Load a sample that breaks four rules";
            this.btnLoadInvalidSample.Location = new System.Drawing.Point(170, 34);
            this.btnLoadInvalidSample.Name = "btnLoadInvalidSample";
            this.btnLoadInvalidSample.Size = new System.Drawing.Size(170, 32);
            this.btnLoadInvalidSample.TabIndex = 11;
            this.btnLoadInvalidSample.Text = "Load invalid sample";
            this.btnLoadInvalidSample.ToolTipText = "No name, a malformed email, a credit limit below the active minimum and a future start date — four ErrorProvider marks at once.";
            this.btnLoadInvalidSample.Click += new System.EventHandler(this.btnLoadInvalidSample_Click);
            //
            // btnSaveTwice  (proves the busy state swallows the second submit)
            //
            this.btnSaveTwice.AccessibleName = "Click Save twice within 200 milliseconds";
            this.btnSaveTwice.Location = new System.Drawing.Point(350, 34);
            this.btnSaveTwice.Name = "btnSaveTwice";
            this.btnSaveTwice.Size = new System.Drawing.Size(130, 32);
            this.btnSaveTwice.TabIndex = 12;
            this.btnSaveTwice.Text = "Save twice";
            this.btnSaveTwice.ToolTipText = "Calls SaveAsync() twice within 200 ms: the second call is swallowed by the busy state, so nothing is saved twice.";
            this.btnSaveTwice.Click += new System.EventHandler(this.btnSaveTwice_Click);
            //
            // chkSimulateFailure  (service failure path and its recovery — a state control, not a command)
            //
            this.chkSimulateFailure.AccessibleName = "Make the customer service fail on the next save";
            this.chkSimulateFailure.Location = new System.Drawing.Point(496, 38);
            this.chkSimulateFailure.Name = "chkSimulateFailure";
            this.chkSimulateFailure.Size = new System.Drawing.Size(220, 24);
            this.chkSimulateFailure.TabIndex = 13;
            this.chkSimulateFailure.Text = "Simulate service failure";
            this.chkSimulateFailure.ToolTipText = "CustomerService.SimulateFailure — the next save throws after the round trip and writes nothing. Untick it and save again to recover.";
            this.chkSimulateFailure.CheckedChanged += new System.EventHandler(this.chkSimulateFailure_CheckedChanged);
            //
            // lblCommandsHint
            //
            this.lblCommandsHint.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblCommandsHint.AutoSize = false;
            this.lblCommandsHint.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblCommandsHint.Location = new System.Drawing.Point(20, 72);
            this.lblCommandsHint.Name = "lblCommandsHint";
            this.lblCommandsHint.Size = new System.Drawing.Size(700, 30);
            this.lblCommandsHint.Text = "Save, Reset and Validate live on the CustomerEditor itself. These four only feed it data and switch the service — the page never touches a child control of the editor.";
            this.lblCommandsHint.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // pnlHeader  (Dock = Top — added LAST so it spans the full width)
            //
            this.pnlHeader.BackColor = System.Drawing.Color.White;
            this.pnlHeader.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Controls.Add(this.lblModule);
            this.pnlHeader.Dock = Wisej.Web.DockStyle.Top;
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(740, 76);
            //
            // lblTitle  (the title Label every section page carries)
            //
            this.lblTitle.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblTitle.AutoSize = false;
            this.lblTitle.Font = new System.Drawing.Font("default", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(20, 8);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(700, 32);
            this.lblTitle.Text = "Editors";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblModule
            //
            this.lblModule.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblModule.AutoSize = false;
            this.lblModule.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblModule.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblModule.Location = new System.Drawing.Point(20, 42);
            this.lblModule.Name = "lblModule";
            this.lblModule.Size = new System.Drawing.Size(700, 22);
            this.lblModule.Text = "MODULE 2 · EDITORS, BUTTONS, VALIDATION, AND FEEDBACK — CUSTOMEREDITOR USERCONTROL";
            this.lblModule.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // EditorsPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.pnlEditorHost);   // Fill first …
            this.Controls.Add(this.pnlCommands);     // … then the Bottom bar …
            this.Controls.Add(this.pnlHeader);       // … and the Top bar last (dock order, see the cookbook)
            this.Name = "EditorsPage";
            this.Size = new System.Drawing.Size(740, 704);
            this.pnlEditorHost.ResumeLayout(false);
            this.pnlCommands.ResumeLayout(false);
            this.pnlHeader.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlEditorHost;
        private OperationsConsole.Editors.CustomerEditor customerEditor;
        private Wisej.Web.Panel pnlCommands;
        private Wisej.Web.Label lblCommandsTitle;
        private Wisej.Web.Button btnLoadSample;
        private Wisej.Web.Button btnLoadInvalidSample;
        private Wisej.Web.Button btnSaveTwice;
        private Wisej.Web.CheckBox chkSimulateFailure;
        private Wisej.Web.Label lblCommandsHint;
        private Wisej.Web.Panel pnlHeader;
        private Wisej.Web.Label lblTitle;
        private Wisej.Web.Label lblModule;
    }
}
