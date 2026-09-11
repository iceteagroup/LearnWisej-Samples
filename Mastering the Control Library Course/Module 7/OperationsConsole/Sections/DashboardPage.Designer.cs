namespace OperationsConsole.Sections
{
    partial class DashboardPage
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
            this.pnlCommands = new Wisej.Web.Panel();
            this.btnRefresh = new Wisej.Web.Button();
            this.lblLastRefreshed = new Wisej.Web.Label();
            this.chkSimulateFailure = new Wisej.Web.CheckBox();
            this.lblQuestion = new Wisej.Web.Label();
            this.tlpDashboard = new Wisej.Web.TableLayoutPanel();
            this.pnlChartCard = new Wisej.Web.Panel();
            this.chartTickets = new Wisej.Web.Ext.ChartJS.ChartJS();
            this.lblChartTitle = new Wisej.Web.Label();
            this.pnlPreviewCard = new Wisej.Web.Panel();
            this.pdfPreview = new Wisej.Web.PdfViewer();
            this.lblPreviewCaption = new Wisej.Web.Label();
            this.lblPreviewTitle = new Wisej.Web.Label();
            this.pnlUploadCard = new Wisej.Web.Panel();
            this.lblUploadResult = new Wisej.Web.Label();
            this.uploadReport = new Wisej.Web.Upload();
            this.lblUploadHint = new Wisej.Web.Label();
            this.lblUploadTitle = new Wisej.Web.Label();
            this.pnlCompletionCard = new Wisej.Web.Panel();
            this.htmlPreview = new Wisej.Web.HtmlPanel();
            this.progressCompletion = new Wisej.Web.ProgressBar();
            this.lblCompletionTitle = new Wisej.Web.Label();
            this.pnlMapCard = new Wisej.Web.Panel();
            this.pnlMapPlaceholder = new Wisej.Web.Panel();
            this.lblMapNote = new Wisej.Web.Label();
            this.lblMapTitle = new Wisej.Web.Label();
            this.pnlCommands.SuspendLayout();
            this.tlpDashboard.SuspendLayout();
            this.pnlChartCard.SuspendLayout();
            this.pnlPreviewCard.SuspendLayout();
            this.pnlUploadCard.SuspendLayout();
            this.pnlCompletionCard.SuspendLayout();
            this.pnlMapCard.SuspendLayout();
            this.pnlMapPlaceholder.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlCommands
            //
            this.pnlCommands.Controls.Add(this.btnRefresh);
            this.pnlCommands.Controls.Add(this.lblLastRefreshed);
            this.pnlCommands.Controls.Add(this.chkSimulateFailure);
            this.pnlCommands.Controls.Add(this.lblQuestion);
            this.pnlCommands.Dock = Wisej.Web.DockStyle.Top;
            this.pnlCommands.Name = "pnlCommands";
            this.pnlCommands.Size = new System.Drawing.Size(1068, 48);
            //
            // btnRefresh
            //
            this.btnRefresh.AccessibleName = "Refresh the dashboard";
            this.btnRefresh.Location = new System.Drawing.Point(0, 6);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(110, 30);
            this.btnRefresh.TabIndex = 1;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            //
            // lblLastRefreshed
            //
            this.lblLastRefreshed.AutoSize = false;
            this.lblLastRefreshed.ForeColor = System.Drawing.Color.FromArgb(58, 77, 99);
            this.lblLastRefreshed.Location = new System.Drawing.Point(122, 6);
            this.lblLastRefreshed.Name = "lblLastRefreshed";
            this.lblLastRefreshed.Size = new System.Drawing.Size(230, 30);
            this.lblLastRefreshed.Text = "Last refreshed —";
            this.lblLastRefreshed.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // chkSimulateFailure
            //
            this.chkSimulateFailure.AccessibleName = "Make the dashboard service fail";
            this.chkSimulateFailure.Location = new System.Drawing.Point(360, 9);
            this.chkSimulateFailure.Name = "chkSimulateFailure";
            this.chkSimulateFailure.Size = new System.Drawing.Size(190, 24);
            this.chkSimulateFailure.TabIndex = 2;
            this.chkSimulateFailure.Text = "Simulate service failure";
            this.chkSimulateFailure.CheckedChanged += new System.EventHandler(this.chkSimulateFailure_CheckedChanged);
            //
            // lblQuestion
            //
            this.lblQuestion.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblQuestion.AutoSize = false;
            this.lblQuestion.ForeColor = System.Drawing.Color.FromArgb(58, 77, 99);
            this.lblQuestion.Location = new System.Drawing.Point(560, 6);
            this.lblQuestion.Name = "lblQuestion";
            this.lblQuestion.Size = new System.Drawing.Size(508, 30);
            this.lblQuestion.Text = "Question: are we on track for this month's ticket target?";
            this.lblQuestion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // tlpDashboard
            //
            this.tlpDashboard.ColumnCount = 3;
            this.tlpDashboard.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 44F));
            this.tlpDashboard.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 30F));
            this.tlpDashboard.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 26F));
            this.tlpDashboard.RowCount = 2;
            this.tlpDashboard.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Percent, 55F));
            this.tlpDashboard.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Percent, 45F));
            this.tlpDashboard.Controls.Add(this.pnlChartCard);
            this.tlpDashboard.Controls.Add(this.pnlPreviewCard);
            this.tlpDashboard.Controls.Add(this.pnlUploadCard);
            this.tlpDashboard.Controls.Add(this.pnlCompletionCard);
            this.tlpDashboard.Controls.Add(this.pnlMapCard);
            this.tlpDashboard.SetColumn(this.pnlChartCard, 0);
            this.tlpDashboard.SetRow(this.pnlChartCard, 0);
            this.tlpDashboard.SetRowSpan(this.pnlChartCard, 2);
            this.tlpDashboard.SetColumn(this.pnlPreviewCard, 1);
            this.tlpDashboard.SetRow(this.pnlPreviewCard, 0);
            this.tlpDashboard.SetColumn(this.pnlUploadCard, 1);
            this.tlpDashboard.SetRow(this.pnlUploadCard, 1);
            this.tlpDashboard.SetColumn(this.pnlCompletionCard, 2);
            this.tlpDashboard.SetRow(this.pnlCompletionCard, 0);
            this.tlpDashboard.SetColumn(this.pnlMapCard, 2);
            this.tlpDashboard.SetRow(this.pnlMapCard, 1);
            this.tlpDashboard.Dock = Wisej.Web.DockStyle.Fill;
            this.tlpDashboard.Name = "tlpDashboard";
            this.tlpDashboard.Size = new System.Drawing.Size(1068, 626);
            //
            // pnlChartCard
            //
            this.pnlChartCard.BackColor = System.Drawing.Color.White;
            this.pnlChartCard.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlChartCard.Controls.Add(this.chartTickets);
            this.pnlChartCard.Controls.Add(this.lblChartTitle);
            this.pnlChartCard.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlChartCard.Margin = new Wisej.Web.Padding(0, 0, 12, 12);
            this.pnlChartCard.Name = "pnlChartCard";
            this.pnlChartCard.Padding = new Wisej.Web.Padding(14, 10, 14, 12);
            //
            // chartTickets
            //
            this.chartTickets.ChartType = Wisej.Web.Ext.ChartJS.ChartType.Bar;
            this.chartTickets.Dock = Wisej.Web.DockStyle.Fill;
            this.chartTickets.Name = "chartTickets";
            this.chartTickets.TabIndex = 5;
            //
            // lblChartTitle
            //
            this.lblChartTitle.AutoSize = false;
            this.lblChartTitle.Dock = Wisej.Web.DockStyle.Top;
            this.lblChartTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblChartTitle.Name = "lblChartTitle";
            this.lblChartTitle.Size = new System.Drawing.Size(400, 26);
            this.lblChartTitle.Text = "Tickets opened vs closed";
            this.lblChartTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // pnlPreviewCard
            //
            this.pnlPreviewCard.BackColor = System.Drawing.Color.White;
            this.pnlPreviewCard.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlPreviewCard.Controls.Add(this.pdfPreview);
            this.pnlPreviewCard.Controls.Add(this.lblPreviewCaption);
            this.pnlPreviewCard.Controls.Add(this.lblPreviewTitle);
            this.pnlPreviewCard.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlPreviewCard.Margin = new Wisej.Web.Padding(0, 0, 12, 12);
            this.pnlPreviewCard.Name = "pnlPreviewCard";
            this.pnlPreviewCard.Padding = new Wisej.Web.Padding(14, 10, 14, 10);
            //
            // pdfPreview
            //
            this.pdfPreview.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pdfPreview.Dock = Wisej.Web.DockStyle.Fill;
            this.pdfPreview.Name = "pdfPreview";
            this.pdfPreview.PdfSource = "wwwroot/sample-report.pdf";
            this.pdfPreview.TabIndex = 6;
            this.pdfPreview.ViewerType = Wisej.Web.PdfViewerType.Auto;
            //
            // lblPreviewCaption
            //
            this.lblPreviewCaption.AutoSize = false;
            this.lblPreviewCaption.Dock = Wisej.Web.DockStyle.Bottom;
            this.lblPreviewCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblPreviewCaption.Name = "lblPreviewCaption";
            this.lblPreviewCaption.Size = new System.Drawing.Size(300, 22);
            this.lblPreviewCaption.Text = "—";
            this.lblPreviewCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblPreviewTitle
            //
            this.lblPreviewTitle.AutoSize = false;
            this.lblPreviewTitle.Dock = Wisej.Web.DockStyle.Top;
            this.lblPreviewTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblPreviewTitle.Name = "lblPreviewTitle";
            this.lblPreviewTitle.Size = new System.Drawing.Size(300, 26);
            this.lblPreviewTitle.Text = "Document preview";
            this.lblPreviewTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // pnlUploadCard
            //
            this.pnlUploadCard.BackColor = System.Drawing.Color.White;
            this.pnlUploadCard.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlUploadCard.Controls.Add(this.lblUploadResult);
            this.pnlUploadCard.Controls.Add(this.uploadReport);
            this.pnlUploadCard.Controls.Add(this.lblUploadHint);
            this.pnlUploadCard.Controls.Add(this.lblUploadTitle);
            this.pnlUploadCard.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlUploadCard.Margin = new Wisej.Web.Padding(0, 0, 12, 0);
            this.pnlUploadCard.Name = "pnlUploadCard";
            this.pnlUploadCard.Padding = new Wisej.Web.Padding(14, 10, 14, 10);
            //
            // lblUploadResult
            //
            this.lblUploadResult.AutoSize = false;
            this.lblUploadResult.Dock = Wisej.Web.DockStyle.Fill;
            this.lblUploadResult.ForeColor = System.Drawing.Color.FromArgb(58, 77, 99);
            this.lblUploadResult.Name = "lblUploadResult";
            this.lblUploadResult.Text = "No report uploaded yet.";
            this.lblUploadResult.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // uploadReport
            //
            this.uploadReport.AccessibleName = "Upload a PDF report";
            this.uploadReport.Dock = Wisej.Web.DockStyle.Top;
            this.uploadReport.Name = "uploadReport";
            this.uploadReport.Size = new System.Drawing.Size(300, 36);
            this.uploadReport.TabIndex = 7;
            this.uploadReport.Text = "Choose report…";
            this.uploadReport.Uploaded += new Wisej.Web.UploadedEventHandler(this.uploadReport_Uploaded);
            this.uploadReport.Error += new Wisej.Web.UploadErrorEventHandler(this.uploadReport_Error);
            //
            // lblUploadHint
            //
            this.lblUploadHint.AutoSize = false;
            this.lblUploadHint.Dock = Wisej.Web.DockStyle.Top;
            this.lblUploadHint.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblUploadHint.Name = "lblUploadHint";
            this.lblUploadHint.Size = new System.Drawing.Size(300, 26);
            this.lblUploadHint.Text = "—";
            this.lblUploadHint.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblUploadTitle
            //
            this.lblUploadTitle.AutoSize = false;
            this.lblUploadTitle.Dock = Wisej.Web.DockStyle.Top;
            this.lblUploadTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblUploadTitle.Name = "lblUploadTitle";
            this.lblUploadTitle.Size = new System.Drawing.Size(300, 26);
            this.lblUploadTitle.Text = "Upload report";
            this.lblUploadTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // pnlCompletionCard
            //
            this.pnlCompletionCard.BackColor = System.Drawing.Color.White;
            this.pnlCompletionCard.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlCompletionCard.Controls.Add(this.htmlPreview);
            this.pnlCompletionCard.Controls.Add(this.progressCompletion);
            this.pnlCompletionCard.Controls.Add(this.lblCompletionTitle);
            this.pnlCompletionCard.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlCompletionCard.Margin = new Wisej.Web.Padding(0, 0, 0, 12);
            this.pnlCompletionCard.Name = "pnlCompletionCard";
            this.pnlCompletionCard.Padding = new Wisej.Web.Padding(14, 10, 14, 10);
            //
            // htmlPreview
            //
            this.htmlPreview.Dock = Wisej.Web.DockStyle.Fill;
            this.htmlPreview.Name = "htmlPreview";
            this.htmlPreview.ScrollBars = Wisej.Web.ScrollBars.Vertical;
            this.htmlPreview.TabIndex = 9;
            //
            // progressCompletion
            //
            this.progressCompletion.AccessibleName = "Completion of this month's ticket target";
            this.progressCompletion.BarColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.progressCompletion.Dock = Wisej.Web.DockStyle.Top;
            this.progressCompletion.Maximum = 100;
            this.progressCompletion.Minimum = 0;
            this.progressCompletion.Name = "progressCompletion";
            this.progressCompletion.Size = new System.Drawing.Size(260, 26);
            this.progressCompletion.TabIndex = 8;
            this.progressCompletion.Value = 0;
            //
            // lblCompletionTitle
            //
            this.lblCompletionTitle.AutoSize = false;
            this.lblCompletionTitle.Dock = Wisej.Web.DockStyle.Top;
            this.lblCompletionTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblCompletionTitle.Name = "lblCompletionTitle";
            this.lblCompletionTitle.Size = new System.Drawing.Size(260, 26);
            this.lblCompletionTitle.Text = "Monthly completion";
            this.lblCompletionTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // pnlMapCard
            //
            this.pnlMapCard.BackColor = System.Drawing.Color.White;
            this.pnlMapCard.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlMapCard.Controls.Add(this.pnlMapPlaceholder);
            this.pnlMapCard.Controls.Add(this.lblMapTitle);
            this.pnlMapCard.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlMapCard.Margin = new Wisej.Web.Padding(0, 0, 0, 0);
            this.pnlMapCard.Name = "pnlMapCard";
            this.pnlMapCard.Padding = new Wisej.Web.Padding(14, 10, 14, 10);
            //
            // pnlMapPlaceholder
            //
            // Reserved for the GoogleMaps extension (Wisej-4-GoogleMaps NuGet package). It needs a Google Maps
            // API key configured on the component before it renders, so the slot stays empty here:
            //     var map = new Wisej.Web.Ext.GoogleMaps.GoogleMaps { Dock = DockStyle.Fill, ApiKey = "<your key>" };
            //     pnlMapPlaceholder.Controls.Add(map);
            //
            this.pnlMapPlaceholder.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.pnlMapPlaceholder.BorderStyle = Wisej.Web.BorderStyle.Dashed;
            this.pnlMapPlaceholder.Controls.Add(this.lblMapNote);
            this.pnlMapPlaceholder.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlMapPlaceholder.Name = "pnlMapPlaceholder";
            //
            // lblMapNote
            //
            this.lblMapNote.AutoSize = false;
            this.lblMapNote.Dock = Wisej.Web.DockStyle.Fill;
            this.lblMapNote.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblMapNote.Name = "lblMapNote";
            this.lblMapNote.Text = "Map slot reserved · needs a Google Maps API key";
            this.lblMapNote.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // lblMapTitle
            //
            this.lblMapTitle.AutoSize = false;
            this.lblMapTitle.Dock = Wisej.Web.DockStyle.Top;
            this.lblMapTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblMapTitle.Name = "lblMapTitle";
            this.lblMapTitle.Size = new System.Drawing.Size(260, 26);
            this.lblMapTitle.Text = "Service area";
            this.lblMapTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // DashboardPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.tlpDashboard);
            this.Controls.Add(this.pnlCommands);
            this.Name = "DashboardPage";
            this.Padding = new Wisej.Web.Padding(16, 10, 16, 16);
            this.Size = new System.Drawing.Size(1100, 700);
            this.pnlMapPlaceholder.ResumeLayout(false);
            this.pnlMapCard.ResumeLayout(false);
            this.pnlCompletionCard.ResumeLayout(false);
            this.pnlUploadCard.ResumeLayout(false);
            this.pnlPreviewCard.ResumeLayout(false);
            this.pnlChartCard.ResumeLayout(false);
            this.tlpDashboard.ResumeLayout(false);
            this.pnlCommands.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlCommands;
        private Wisej.Web.Button btnRefresh;
        private Wisej.Web.Label lblLastRefreshed;
        private Wisej.Web.CheckBox chkSimulateFailure;
        private Wisej.Web.Label lblQuestion;
        private Wisej.Web.TableLayoutPanel tlpDashboard;
        private Wisej.Web.Panel pnlChartCard;
        private Wisej.Web.Ext.ChartJS.ChartJS chartTickets;
        private Wisej.Web.Label lblChartTitle;
        private Wisej.Web.Panel pnlPreviewCard;
        private Wisej.Web.PdfViewer pdfPreview;
        private Wisej.Web.Label lblPreviewTitle;
        private Wisej.Web.Label lblPreviewCaption;
        private Wisej.Web.Panel pnlUploadCard;
        private Wisej.Web.Upload uploadReport;
        private Wisej.Web.Label lblUploadTitle;
        private Wisej.Web.Label lblUploadHint;
        private Wisej.Web.Label lblUploadResult;
        private Wisej.Web.Panel pnlCompletionCard;
        private Wisej.Web.ProgressBar progressCompletion;
        private Wisej.Web.HtmlPanel htmlPreview;
        private Wisej.Web.Label lblCompletionTitle;
        private Wisej.Web.Panel pnlMapCard;
        private Wisej.Web.Panel pnlMapPlaceholder;
        private Wisej.Web.Label lblMapNote;
        private Wisej.Web.Label lblMapTitle;
    }
}
