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
            this.btnSeed = new Wisej.Web.Button();
            this.statusLabel = new Wisej.Web.Label();
            this.labelCounts = new Wisej.Web.Label();
            this.buttonOverlongTitle = new Wisej.Web.Button();
            this.buttonDeleteCustomer = new Wisej.Web.Button();
            this.panelQuery.SuspendLayout();
            this.SuspendLayout();
            //
            // panelQuery
            //
            this.panelQuery.BackColor = System.Drawing.Color.White;
            this.panelQuery.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelQuery.Controls.Add(this.labelTitle);
            this.panelQuery.Controls.Add(this.countButton);
            this.panelQuery.Controls.Add(this.btnSeed);
            this.panelQuery.Controls.Add(this.statusLabel);
            this.panelQuery.Controls.Add(this.labelCounts);
            this.panelQuery.Controls.Add(this.buttonOverlongTitle);
            this.panelQuery.Controls.Add(this.buttonDeleteCustomer);
            this.panelQuery.Location = new System.Drawing.Point(30, 30);
            this.panelQuery.Name = "panelQuery";
            this.panelQuery.Size = new System.Drawing.Size(560, 250);
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
            this.countButton.Size = new System.Drawing.Size(170, 44);
            this.countButton.Text = "Count tickets";
            this.countButton.Click += new System.EventHandler(this.countButton_Click);
            //
            // btnSeed
            //
            this.btnSeed.Font = new System.Drawing.Font("default", 11F, System.Drawing.FontStyle.Bold);
            this.btnSeed.Location = new System.Drawing.Point(204, 64);
            this.btnSeed.Name = "btnSeed";
            this.btnSeed.Size = new System.Drawing.Size(220, 44);
            this.btnSeed.Text = "Seed development data";
            this.btnSeed.Click += new System.EventHandler(this.btnSeed_Click);
            //
            // statusLabel
            //
            this.statusLabel.AutoSize = false;
            this.statusLabel.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.statusLabel.Location = new System.Drawing.Point(24, 118);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(512, 30);
            this.statusLabel.Text = "Ready";
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // labelCounts
            //
            this.labelCounts.AutoSize = false;
            this.labelCounts.ForeColor = System.Drawing.Color.FromArgb(60, 72, 88);
            this.labelCounts.Location = new System.Drawing.Point(24, 152);
            this.labelCounts.Name = "labelCounts";
            this.labelCounts.Size = new System.Drawing.Size(512, 24);
            this.labelCounts.Text = "";
            this.labelCounts.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // buttonOverlongTitle
            //
            this.buttonOverlongTitle.Location = new System.Drawing.Point(24, 192);
            this.buttonOverlongTitle.Name = "buttonOverlongTitle";
            this.buttonOverlongTitle.Size = new System.Drawing.Size(244, 36);
            this.buttonOverlongTitle.Text = "Save a 200-character title";
            this.buttonOverlongTitle.Click += new System.EventHandler(this.buttonOverlongTitle_Click);
            //
            // buttonDeleteCustomer
            //
            this.buttonDeleteCustomer.Location = new System.Drawing.Point(280, 192);
            this.buttonDeleteCustomer.Name = "buttonDeleteCustomer";
            this.buttonDeleteCustomer.Size = new System.Drawing.Size(256, 36);
            this.buttonDeleteCustomer.Text = "Delete a customer with tickets";
            this.buttonDeleteCustomer.Click += new System.EventHandler(this.buttonDeleteCustomer_Click);
            //
            // TicketBrowserPage
            //
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.panelQuery);
            this.Name = "TicketBrowserPage";
            this.Size = new System.Drawing.Size(620, 310);
            this.Text = "Support Desk Data Console";
            this.Load += new System.EventHandler(this.TicketBrowserPage_Load);
            this.panelQuery.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel panelQuery;
        private Wisej.Web.Label labelTitle;
        private Wisej.Web.Button countButton;
        private Wisej.Web.Button btnSeed;
        private Wisej.Web.Label statusLabel;
        private Wisej.Web.Label labelCounts;
        private Wisej.Web.Button buttonOverlongTitle;
        private Wisej.Web.Button buttonDeleteCustomer;
    }
}
