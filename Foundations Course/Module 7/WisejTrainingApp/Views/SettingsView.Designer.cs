namespace WisejTrainingApp.Views
{
    partial class SettingsView
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
            this.lblPageTitle = new Wisej.Web.Label();
            this.lblPageDescription = new Wisej.Web.Label();
            this.cardUiSettings = new Wisej.Web.Panel();
            this.lblUiSettingsTitle = new Wisej.Web.Label();
            this.lblThemeValue = new Wisej.Web.Label();
            this.lblThemeNote = new Wisej.Web.Label();
            this.cardShell = new Wisej.Web.Panel();
            this.lblShellTitle = new Wisej.Web.Label();
            this.lblShellText = new Wisej.Web.Label();
            this.cardUiSettings.SuspendLayout();
            this.cardShell.SuspendLayout();
            this.SuspendLayout();
            //
            // lblPageTitle
            //
            this.lblPageTitle.AutoSize = false;
            this.lblPageTitle.Font = new System.Drawing.Font("default", 18F, System.Drawing.FontStyle.Bold);
            this.lblPageTitle.Location = new System.Drawing.Point(32, 24);
            this.lblPageTitle.Name = "lblPageTitle";
            this.lblPageTitle.Size = new System.Drawing.Size(600, 34);
            this.lblPageTitle.Text = "Settings";
            //
            // lblPageDescription
            //
            this.lblPageDescription.AutoSize = false;
            this.lblPageDescription.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblPageDescription.Location = new System.Drawing.Point(32, 60);
            this.lblPageDescription.Name = "lblPageDescription";
            this.lblPageDescription.Size = new System.Drawing.Size(1000, 22);
            this.lblPageDescription.Text = "UI settings live in the shell. The ticket service has no idea a theme exists.";
            //
            // cardUiSettings  (524 × 220 at 32,100)
            //
            this.cardUiSettings.BackColor = System.Drawing.Color.White;
            this.cardUiSettings.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.cardUiSettings.Controls.Add(this.lblUiSettingsTitle);
            this.cardUiSettings.Controls.Add(this.lblThemeValue);
            this.cardUiSettings.Controls.Add(this.lblThemeNote);
            this.cardUiSettings.Location = new System.Drawing.Point(32, 100);
            this.cardUiSettings.Name = "cardUiSettings";
            this.cardUiSettings.Size = new System.Drawing.Size(524, 220);
            //
            // lblUiSettingsTitle
            //
            this.lblUiSettingsTitle.AutoSize = false;
            this.lblUiSettingsTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblUiSettingsTitle.Location = new System.Drawing.Point(20, 14);
            this.lblUiSettingsTitle.Name = "lblUiSettingsTitle";
            this.lblUiSettingsTitle.Size = new System.Drawing.Size(484, 28);
            this.lblUiSettingsTitle.Text = "UI settings";
            //
            // lblThemeValue
            //
            this.lblThemeValue.AutoSize = false;
            this.lblThemeValue.BackColor = System.Drawing.Color.FromArgb(247, 249, 252);
            this.lblThemeValue.Font = new System.Drawing.Font("default", 13F, System.Drawing.FontStyle.Bold);
            this.lblThemeValue.Location = new System.Drawing.Point(20, 52);
            this.lblThemeValue.Name = "lblThemeValue";
            this.lblThemeValue.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.lblThemeValue.Size = new System.Drawing.Size(484, 48);
            this.lblThemeValue.Text = "Current theme: Bootstrap-4";
            this.lblThemeValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblThemeNote
            //
            this.lblThemeNote.AutoSize = false;
            this.lblThemeNote.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblThemeNote.Location = new System.Drawing.Point(20, 112);
            this.lblThemeNote.Name = "lblThemeNote";
            this.lblThemeNote.Size = new System.Drawing.Size(484, 92);
            this.lblThemeNote.Text = "Change the theme with the selector in the header (cboTheme → Application.LoadTheme → lblCurrentTheme). " +
                "It is a UI setting: it never reaches TicketService, the grid binding or ValidateForm. " +
                "Switch to BootstrapDark-4 and open a ticket — the same rules apply, only the look changed.";
            this.lblThemeNote.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // cardShell  (524 × 220 at 580,100)
            //
            this.cardShell.BackColor = System.Drawing.Color.White;
            this.cardShell.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.cardShell.Controls.Add(this.lblShellTitle);
            this.cardShell.Controls.Add(this.lblShellText);
            this.cardShell.Location = new System.Drawing.Point(580, 100);
            this.cardShell.Name = "cardShell";
            this.cardShell.Size = new System.Drawing.Size(524, 220);
            //
            // lblShellTitle
            //
            this.lblShellTitle.AutoSize = false;
            this.lblShellTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblShellTitle.Location = new System.Drawing.Point(20, 14);
            this.lblShellTitle.Name = "lblShellTitle";
            this.lblShellTitle.Size = new System.Drawing.Size(484, 28);
            this.lblShellTitle.Text = "What the visual layer owns";
            //
            // lblShellText
            //
            this.lblShellText.AutoSize = false;
            this.lblShellText.Font = new System.Drawing.Font("monospace", 9F);
            this.lblShellText.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblShellText.Location = new System.Drawing.Point(20, 52);
            this.lblShellText.Name = "lblShellText";
            this.lblShellText.Size = new System.Drawing.Size(484, 152);
            this.lblShellText.Text = "Theme      cboTheme · lblCurrentTheme · Application.LoadTheme\n" +
                "Header     lblAppTitle · lblModule            (Dock Top)\n" +
                "Nav        btnDashboard … btnSettings         (Dock Left)\n" +
                "           SetActiveButton(Button)          active state\n" +
                "Content    Views/*View · NavigateTo(page)    (Dock Fill)\n" +
                "Cards      Panel + BorderStyle.Solid · LayoutRules\n" +
                "Status     lblStatus                        (Dock Bottom)\n" +
                "Untouched  TicketService · Ticket · BindingSource · ValidateForm";
            this.lblShellText.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // SettingsView
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.lblPageTitle);
            this.Controls.Add(this.lblPageDescription);
            this.Controls.Add(this.cardUiSettings);
            this.Controls.Add(this.cardShell);
            this.Name = "SettingsView";
            this.Size = new System.Drawing.Size(1148, 620);
            this.cardUiSettings.ResumeLayout(false);
            this.cardShell.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Label lblPageTitle;
        private Wisej.Web.Label lblPageDescription;
        private Wisej.Web.Panel cardUiSettings;
        private Wisej.Web.Label lblUiSettingsTitle;
        private Wisej.Web.Label lblThemeValue;
        private Wisej.Web.Label lblThemeNote;
        private Wisej.Web.Panel cardShell;
        private Wisej.Web.Label lblShellTitle;
        private Wisej.Web.Label lblShellText;
    }
}
