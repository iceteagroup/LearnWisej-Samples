namespace OperationsConsole.Sections
{
    partial class WidgetsPage
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
            this.components = new System.ComponentModel.Container();
            this.pnlContent = new Wisej.Web.Panel();
            this.pnlRatingCard = new Wisej.Web.Panel();
            this.lblCardTitle = new Wisej.Web.Label();
            this.lblCustomer = new Wisej.Web.Label();
            this.ratingWidget = new Wisej.Web.Widget();
            this.lblSavedState = new Wisej.Web.Label();
            this.pnlTraceCard = new Wisej.Web.Panel();
            this.lblTraceTitle = new Wisej.Web.Label();
            this.lstMessageTrace = new Wisej.Web.ListBox();
            this.pnlOptions = new Wisej.Web.Panel();
            this.chkSimulateServiceFailure = new Wisej.Web.CheckBox();
            this.styleSheet = new Wisej.Web.StyleSheet(this.components);
            this.pnlContent.SuspendLayout();
            this.pnlRatingCard.SuspendLayout();
            this.pnlTraceCard.SuspendLayout();
            this.pnlOptions.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlContent
            //
            this.pnlContent.AutoScroll = true;
            this.pnlContent.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.pnlContent.Controls.Add(this.pnlRatingCard);
            this.pnlContent.Controls.Add(this.pnlTraceCard);
            this.pnlContent.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Size = new System.Drawing.Size(740, 660);
            this.pnlContent.TabIndex = 1;
            //
            // pnlRatingCard
            //
            this.pnlRatingCard.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlRatingCard.BackColor = System.Drawing.Color.White;
            this.pnlRatingCard.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlRatingCard.Controls.Add(this.lblCardTitle);
            this.pnlRatingCard.Controls.Add(this.lblCustomer);
            this.pnlRatingCard.Controls.Add(this.ratingWidget);
            this.pnlRatingCard.Controls.Add(this.lblSavedState);
            this.pnlRatingCard.Location = new System.Drawing.Point(16, 16);
            this.pnlRatingCard.Name = "pnlRatingCard";
            this.pnlRatingCard.Size = new System.Drawing.Size(692, 222);
            this.pnlRatingCard.TabIndex = 10;
            //
            // lblCardTitle
            //
            this.lblCardTitle.AutoSize = false;
            this.lblCardTitle.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblCardTitle.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblCardTitle.Location = new System.Drawing.Point(20, 14);
            this.lblCardTitle.Name = "lblCardTitle";
            this.lblCardTitle.Size = new System.Drawing.Size(400, 20);
            this.lblCardTitle.Text = "CUSTOMER RATING";
            this.lblCardTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblCustomer
            //
            this.lblCustomer.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblCustomer.AutoSize = false;
            this.lblCustomer.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblCustomer.Location = new System.Drawing.Point(20, 36);
            this.lblCustomer.Name = "lblCustomer";
            this.lblCustomer.Size = new System.Drawing.Size(652, 28);
            this.lblCustomer.Text = "Northwind Traders · customer CUST-1042";
            this.lblCustomer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // ratingWidget
            //
            this.ratingWidget.AccessibleName = "Customer satisfaction rating";
            this.ratingWidget.AccessibleDescription = "Five stars. Click a star, or use the arrow keys, to rate this customer from 1 to 5.";
            this.ratingWidget.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.ratingWidget.Location = new System.Drawing.Point(20, 72);
            this.ratingWidget.Name = "ratingWidget";
            // Packages load once per page, in list order: the stylesheet first, then the library.
            this.ratingWidget.Packages.Add(new Wisej.Web.Widget.Package() { Name = "rating-css", Source = "wwwroot/rating.css" });
            this.ratingWidget.Packages.Add(new Wisej.Web.Widget.Package() { Name = "rating-js", Source = "wwwroot/rating.js" });
            this.ratingWidget.Size = new System.Drawing.Size(652, 104);
            this.ratingWidget.TabIndex = 11;
            this.ratingWidget.WiredEvents = new string[] { "ratingChanged" };
            this.ratingWidget.WidgetEvent += new Wisej.Web.WidgetEventHandler(this.ratingWidget_WidgetEvent);
            //
            // lblSavedState
            //
            this.lblSavedState.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblSavedState.AutoSize = false;
            this.lblSavedState.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblSavedState.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblSavedState.Location = new System.Drawing.Point(20, 184);
            this.lblSavedState.Name = "lblSavedState";
            this.lblSavedState.Size = new System.Drawing.Size(652, 24);
            this.lblSavedState.Text = "Northwind Traders has no saved rating yet — click a star.";
            this.lblSavedState.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // pnlTraceCard
            //
            this.pnlTraceCard.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlTraceCard.BackColor = System.Drawing.Color.White;
            this.pnlTraceCard.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlTraceCard.Controls.Add(this.lblTraceTitle);
            this.pnlTraceCard.Controls.Add(this.lstMessageTrace);
            this.pnlTraceCard.Location = new System.Drawing.Point(16, 254);
            this.pnlTraceCard.Name = "pnlTraceCard";
            this.pnlTraceCard.Size = new System.Drawing.Size(692, 220);
            this.pnlTraceCard.TabIndex = 20;
            //
            // lblTraceTitle
            //
            this.lblTraceTitle.AutoSize = false;
            this.lblTraceTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblTraceTitle.Location = new System.Drawing.Point(20, 12);
            this.lblTraceTitle.Name = "lblTraceTitle";
            this.lblTraceTitle.Size = new System.Drawing.Size(400, 26);
            this.lblTraceTitle.Text = "Message trace";
            this.lblTraceTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lstMessageTrace
            //
            this.lstMessageTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lstMessageTrace.Font = new System.Drawing.Font("monospace", 9F);
            this.lstMessageTrace.Location = new System.Drawing.Point(20, 44);
            this.lstMessageTrace.Name = "lstMessageTrace";
            this.lstMessageTrace.Size = new System.Drawing.Size(652, 160);
            this.lstMessageTrace.TabIndex = 21;
            //
            // pnlOptions
            //
            this.pnlOptions.BackColor = System.Drawing.Color.White;
            this.pnlOptions.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlOptions.Controls.Add(this.chkSimulateServiceFailure);
            this.pnlOptions.Dock = Wisej.Web.DockStyle.Top;
            this.pnlOptions.Name = "pnlOptions";
            this.pnlOptions.Size = new System.Drawing.Size(740, 44);
            this.pnlOptions.TabIndex = 0;
            //
            // chkSimulateServiceFailure
            //
            this.chkSimulateServiceFailure.AccessibleName = "Make the ratings service fail on save";
            this.chkSimulateServiceFailure.Location = new System.Drawing.Point(16, 10);
            this.chkSimulateServiceFailure.Name = "chkSimulateServiceFailure";
            this.chkSimulateServiceFailure.Size = new System.Drawing.Size(240, 24);
            this.chkSimulateServiceFailure.TabIndex = 1;
            this.chkSimulateServiceFailure.Text = "Simulate service failure";
            this.chkSimulateServiceFailure.CheckedChanged += new System.EventHandler(this.chkSimulateServiceFailure_CheckedChanged);
            //
            // WidgetsPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlOptions);
            this.Name = "WidgetsPage";
            this.Size = new System.Drawing.Size(740, 704);
            this.pnlContent.ResumeLayout(false);
            this.pnlRatingCard.ResumeLayout(false);
            this.pnlTraceCard.ResumeLayout(false);
            this.pnlOptions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlContent;
        private Wisej.Web.Panel pnlRatingCard;
        private Wisej.Web.Label lblCardTitle;
        private Wisej.Web.Label lblCustomer;
        private Wisej.Web.Widget ratingWidget;
        private Wisej.Web.Label lblSavedState;
        private Wisej.Web.Panel pnlTraceCard;
        private Wisej.Web.Label lblTraceTitle;
        private Wisej.Web.ListBox lstMessageTrace;
        private Wisej.Web.Panel pnlOptions;
        private Wisej.Web.CheckBox chkSimulateServiceFailure;
        private Wisej.Web.StyleSheet styleSheet;
    }
}
