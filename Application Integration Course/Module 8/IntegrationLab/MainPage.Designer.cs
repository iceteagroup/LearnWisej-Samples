namespace IntegrationLab
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
            this.panelPostback = new Wisej.Web.Panel();
            this.labelPostbackTitle = new Wisej.Web.Label();
            this.gridPostback = new IntegrationLab.Widgets.GridWidget();
            this.panelLookup = new Wisej.Web.Panel();
            this.labelLookupTitle = new Wisej.Web.Label();
            this.gridLookup = new IntegrationLab.Widgets.LookupWidget();
            this.panelTrace = new Wisej.Web.Panel();
            this.labelTraceTitle = new Wisej.Web.Label();
            this.listTrace = new Wisej.Web.ListBox();
            this.panelPostback.SuspendLayout();
            this.panelLookup.SuspendLayout();
            this.panelTrace.SuspendLayout();
            this.SuspendLayout();
            //
            // panelPostback
            //
            this.panelPostback.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.panelPostback.BackColor = System.Drawing.Color.White;
            this.panelPostback.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelPostback.Controls.Add(this.labelPostbackTitle);
            this.panelPostback.Controls.Add(this.gridPostback);
            this.panelPostback.Location = new System.Drawing.Point(30, 30);
            this.panelPostback.Name = "panelPostback";
            this.panelPostback.Size = new System.Drawing.Size(440, 620);
            //
            // labelPostbackTitle
            //
            this.labelPostbackTitle.AutoSize = false;
            this.labelPostbackTitle.Font = new System.Drawing.Font("default", 13F, System.Drawing.FontStyle.Bold);
            this.labelPostbackTitle.Location = new System.Drawing.Point(16, 12);
            this.labelPostbackTitle.Name = "labelPostbackTitle";
            this.labelPostbackTitle.Size = new System.Drawing.Size(408, 28);
            this.labelPostbackTitle.Text = "Postback URL data source";
            //
            // gridPostback
            //
            this.gridPostback.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.gridPostback.Location = new System.Drawing.Point(16, 48);
            this.gridPostback.Name = "gridPostback";
            this.gridPostback.PageSize = 10;
            this.gridPostback.Size = new System.Drawing.Size(408, 556);
            //
            // panelLookup
            //
            this.panelLookup.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.panelLookup.BackColor = System.Drawing.Color.White;
            this.panelLookup.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelLookup.Controls.Add(this.labelLookupTitle);
            this.panelLookup.Controls.Add(this.gridLookup);
            this.panelLookup.Location = new System.Drawing.Point(486, 30);
            this.panelLookup.Name = "panelLookup";
            this.panelLookup.Size = new System.Drawing.Size(440, 620);
            //
            // labelLookupTitle
            //
            this.labelLookupTitle.AutoSize = false;
            this.labelLookupTitle.Font = new System.Drawing.Font("default", 13F, System.Drawing.FontStyle.Bold);
            this.labelLookupTitle.Location = new System.Drawing.Point(16, 12);
            this.labelLookupTitle.Name = "labelLookupTitle";
            this.labelLookupTitle.Size = new System.Drawing.Size(408, 28);
            this.labelLookupTitle.Text = "WebMethod RPC callback";
            //
            // gridLookup
            //
            this.gridLookup.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.gridLookup.Location = new System.Drawing.Point(16, 48);
            this.gridLookup.Name = "gridLookup";
            this.gridLookup.PageSize = 10;
            this.gridLookup.Size = new System.Drawing.Size(408, 556);
            //
            // panelTrace
            //
            this.panelTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelTrace.BackColor = System.Drawing.Color.White;
            this.panelTrace.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelTrace.Controls.Add(this.labelTraceTitle);
            this.panelTrace.Controls.Add(this.listTrace);
            this.panelTrace.Location = new System.Drawing.Point(942, 30);
            this.panelTrace.Name = "panelTrace";
            this.panelTrace.Size = new System.Drawing.Size(376, 620);
            //
            // labelTraceTitle
            //
            this.labelTraceTitle.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelTraceTitle.AutoSize = false;
            this.labelTraceTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelTraceTitle.Location = new System.Drawing.Point(16, 14);
            this.labelTraceTitle.Name = "labelTraceTitle";
            this.labelTraceTitle.Size = new System.Drawing.Size(344, 30);
            this.labelTraceTitle.Text = "Network";
            //
            // listTrace
            //
            this.listTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.listTrace.Font = new System.Drawing.Font("monospace", 9F);
            this.listTrace.Location = new System.Drawing.Point(16, 52);
            this.listTrace.Name = "listTrace";
            this.listTrace.Size = new System.Drawing.Size(344, 552);
            //
            // MainPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.panelPostback);
            this.Controls.Add(this.panelLookup);
            this.Controls.Add(this.panelTrace);
            this.Name = "MainPage";
            this.Size = new System.Drawing.Size(1348, 680);
            this.Text = "IntegrationLab — Work Orders";
            this.panelPostback.ResumeLayout(false);
            this.panelLookup.ResumeLayout(false);
            this.panelTrace.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel panelPostback;
        private Wisej.Web.Label labelPostbackTitle;
        private IntegrationLab.Widgets.GridWidget gridPostback;
        private Wisej.Web.Panel panelLookup;
        private Wisej.Web.Label labelLookupTitle;
        private IntegrationLab.Widgets.LookupWidget gridLookup;
        private Wisej.Web.Panel panelTrace;
        private Wisej.Web.Label labelTraceTitle;
        private Wisej.Web.ListBox listTrace;
    }
}
