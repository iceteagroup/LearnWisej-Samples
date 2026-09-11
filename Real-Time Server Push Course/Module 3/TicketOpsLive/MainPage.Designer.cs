namespace TicketOpsLive
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
            this.panelImport = new Wisej.Web.Panel();
            this.labelTitle = new Wisej.Web.Label();
            this.elapsedLabel = new Wisej.Web.Label();
            this.importStatusLabel = new Wisej.Web.Label();
            this.recordsImportedLabel = new Wisej.Web.Label();
            this.importProgressBar = new Wisej.Web.ProgressBar();
            this.importLogListBox = new Wisej.Web.ListBox();
            this.startImportButton = new Wisej.Web.Button();
            this.cancelImportButton = new Wisej.Web.Button();
            this.failAt87Button = new Wisej.Web.Button();
            this.panelImport.SuspendLayout();
            this.SuspendLayout();
            //
            // panelImport
            //
            this.panelImport.BackColor = System.Drawing.Color.White;
            this.panelImport.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelImport.Controls.Add(this.labelTitle);
            this.panelImport.Controls.Add(this.elapsedLabel);
            this.panelImport.Controls.Add(this.importStatusLabel);
            this.panelImport.Controls.Add(this.recordsImportedLabel);
            this.panelImport.Controls.Add(this.importProgressBar);
            this.panelImport.Controls.Add(this.importLogListBox);
            this.panelImport.Controls.Add(this.startImportButton);
            this.panelImport.Controls.Add(this.cancelImportButton);
            this.panelImport.Controls.Add(this.failAt87Button);
            this.panelImport.Location = new System.Drawing.Point(20, 18);
            this.panelImport.Name = "panelImport";
            this.panelImport.Size = new System.Drawing.Size(720, 364);
            //
            // labelTitle
            //
            this.labelTitle.AutoSize = false;
            this.labelTitle.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.labelTitle.Location = new System.Drawing.Point(24, 16);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(360, 30);
            this.labelTitle.Text = "Background Import";
            //
            // elapsedLabel
            //
            this.elapsedLabel.AutoSize = false;
            this.elapsedLabel.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.elapsedLabel.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.elapsedLabel.Location = new System.Drawing.Point(456, 20);
            this.elapsedLabel.Name = "elapsedLabel";
            this.elapsedLabel.Size = new System.Drawing.Size(240, 22);
            this.elapsedLabel.Text = "Elapsed 0.00 s";
            this.elapsedLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // importStatusLabel
            //
            this.importStatusLabel.AutoSize = false;
            this.importStatusLabel.Font = new System.Drawing.Font("default", 11F);
            this.importStatusLabel.Location = new System.Drawing.Point(24, 56);
            this.importStatusLabel.Name = "importStatusLabel";
            this.importStatusLabel.Size = new System.Drawing.Size(520, 26);
            this.importStatusLabel.Text = "Idle — ready to import";
            //
            // recordsImportedLabel
            //
            this.recordsImportedLabel.AutoSize = false;
            this.recordsImportedLabel.Font = new System.Drawing.Font("default", 16F, System.Drawing.FontStyle.Bold);
            this.recordsImportedLabel.Location = new System.Drawing.Point(556, 52);
            this.recordsImportedLabel.Name = "recordsImportedLabel";
            this.recordsImportedLabel.Size = new System.Drawing.Size(140, 30);
            this.recordsImportedLabel.Text = "0/200";
            this.recordsImportedLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // importProgressBar
            //
            this.importProgressBar.Location = new System.Drawing.Point(24, 90);
            this.importProgressBar.Name = "importProgressBar";
            this.importProgressBar.Size = new System.Drawing.Size(672, 24);
            this.importProgressBar.Value = 0;
            //
            // importLogListBox
            //
            this.importLogListBox.Font = new System.Drawing.Font("monospace", 9F);
            this.importLogListBox.Location = new System.Drawing.Point(24, 130);
            this.importLogListBox.Name = "importLogListBox";
            this.importLogListBox.Size = new System.Drawing.Size(460, 210);
            //
            // startImportButton
            //
            this.startImportButton.Location = new System.Drawing.Point(504, 130);
            this.startImportButton.Name = "startImportButton";
            this.startImportButton.Size = new System.Drawing.Size(192, 36);
            this.startImportButton.Text = "Start Import";
            this.startImportButton.Click += new System.EventHandler(this.startImportButton_Click);
            //
            // cancelImportButton
            //
            this.cancelImportButton.Enabled = false;
            this.cancelImportButton.Location = new System.Drawing.Point(504, 176);
            this.cancelImportButton.Name = "cancelImportButton";
            this.cancelImportButton.Size = new System.Drawing.Size(192, 36);
            this.cancelImportButton.Text = "Cancel Import";
            this.cancelImportButton.Click += new System.EventHandler(this.cancelImportButton_Click);
            //
            // failAt87Button
            //
            this.failAt87Button.ForeColor = System.Drawing.Color.FromArgb(192, 57, 43);
            this.failAt87Button.Location = new System.Drawing.Point(504, 222);
            this.failAt87Button.Name = "failAt87Button";
            this.failAt87Button.Size = new System.Drawing.Size(192, 36);
            this.failAt87Button.Text = "Fail at 87";
            this.failAt87Button.Click += new System.EventHandler(this.failAt87Button_Click);
            //
            // MainPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.panelImport);
            this.Name = "MainPage";
            this.Size = new System.Drawing.Size(760, 400);
            this.Text = "TicketOps Live — Import Monitor";
            this.Load += new System.EventHandler(this.MainPage_Load);
            this.panelImport.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel panelImport;
        private Wisej.Web.Label labelTitle;
        private Wisej.Web.Label elapsedLabel;
        private Wisej.Web.Label importStatusLabel;
        private Wisej.Web.Label recordsImportedLabel;
        private Wisej.Web.ProgressBar importProgressBar;
        private Wisej.Web.ListBox importLogListBox;
        private Wisej.Web.Button startImportButton;
        private Wisej.Web.Button cancelImportButton;
        private Wisej.Web.Button failAt87Button;
    }
}
