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
            this.countButton = new Wisej.Web.Button();
            this.statusLabel = new Wisej.Web.Label();
            this.panelQuery.SuspendLayout();
            this.SuspendLayout();
            //
            // panelQuery
            //
            this.panelQuery.BackColor = System.Drawing.Color.White;
            this.panelQuery.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelQuery.Controls.Add(this.labelTitle);
            this.panelQuery.Controls.Add(this.countButton);
            this.panelQuery.Controls.Add(this.statusLabel);
            this.panelQuery.Location = new System.Drawing.Point(30, 30);
            this.panelQuery.Name = "panelQuery";
            this.panelQuery.Size = new System.Drawing.Size(560, 132);
            //
            // labelTitle
            //
            this.labelTitle.AutoSize = false;
            this.labelTitle.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.labelTitle.Location = new System.Drawing.Point(24, 18);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(512, 30);
            this.labelTitle.Text = "Support Desk Data Console";
            //
            // countButton
            //
            this.countButton.Font = new System.Drawing.Font("default", 11F, System.Drawing.FontStyle.Bold);
            this.countButton.Location = new System.Drawing.Point(24, 64);
            this.countButton.Name = "countButton";
            this.countButton.Size = new System.Drawing.Size(180, 44);
            this.countButton.Text = "Count tickets";
            this.countButton.Click += new System.EventHandler(this.countButton_Click);
            //
            // statusLabel
            //
            this.statusLabel.AutoSize = false;
            this.statusLabel.Font = new System.Drawing.Font("default", 11F, System.Drawing.FontStyle.Bold);
            this.statusLabel.Location = new System.Drawing.Point(220, 64);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(316, 44);
            this.statusLabel.Text = "Ready";
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // TicketBrowserPage
            //
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.panelQuery);
            this.Name = "TicketBrowserPage";
            this.Size = new System.Drawing.Size(620, 200);
            this.Text = "Support Desk Data Console";
            this.panelQuery.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel panelQuery;
        private Wisej.Web.Label labelTitle;
        private Wisej.Web.Button countButton;
        private Wisej.Web.Label statusLabel;
    }
}
