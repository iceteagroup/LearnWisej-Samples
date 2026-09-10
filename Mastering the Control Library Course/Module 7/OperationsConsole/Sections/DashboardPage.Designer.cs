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

        /// <summary>
        /// The dashboard tab (Module 6). Five cards in a <see cref="Wisej.Web.TableLayoutPanel"/> so the page keeps
        /// working at any size — it is hosted docked <c>Fill</c> inside the shell's content area (a TabPage from
        /// Module 3 on): the trend chart on the left across both rows, the document preview and the upload card in
        /// the middle column, the completion indicator and the reserved map slot on the right.
        /// <para>
        /// Dock order rule (verified in Module 1): docking is applied from the LAST <c>Controls.Add</c> to the FIRST,
        /// so inside every container the <c>Fill</c> child is added first and the edge bars last.
        /// </para>
        /// <para>
        /// The chart's axes, legend, title and colours are not set here: they are configured once in
        /// <c>DashboardPage.ConfigureChart()</c>, and its <c>Labels</c> / <c>DataSets</c> are filled together in
        /// <c>RefreshDashboard(model)</c>. Nothing else in the page touches chart data.
        /// </para>
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlHeader = new Wisej.Web.Panel();
            this.lblQuestion = new Wisej.Web.Label();
            this.lblTitle = new Wisej.Web.Label();
            this.pnlCommands = new Wisej.Web.FlowLayoutPanel();
            this.btnRefresh = new Wisej.Web.Button();
            this.lblLastRefreshed = new Wisej.Web.Label();
            this.chkSimulateFailure = new Wisej.Web.CheckBox();
            this.btnSimulateOversized = new Wisej.Web.Button();
            this.btnSimulateWrongType = new Wisej.Web.Button();
            this.tlpDashboard = new Wisej.Web.TableLayoutPanel();
            this.pnlChartCard = new Wisej.Web.Panel();
            this.chartTickets = new Wisej.Web.Ext.ChartJS.ChartJS();
            this.lblChartHint = new Wisej.Web.Label();
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
            this.pnlHeader.SuspendLayout();
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
            // pnlHeader  (Top) — what this screen is for, in one sentence
            //
            this.pnlHeader.Controls.Add(this.lblQuestion);
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Dock = Wisej.Web.DockStyle.Top;
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1100, 62);
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = false;
            this.lblTitle.Dock = Wisej.Web.DockStyle.Top;
            this.lblTitle.Font = new System.Drawing.Font("default", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(1100, 32);
            this.lblTitle.Text = "Dashboard";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblQuestion  (the question the dashboard answers — task 1 of the lab, on the page and in DashboardNotes.md)
            //
            this.lblQuestion.AutoSize = false;
            this.lblQuestion.Dock = Wisej.Web.DockStyle.Fill;
            this.lblQuestion.ForeColor = System.Drawing.Color.FromArgb(58, 77, 99);
            this.lblQuestion.Name = "lblQuestion";
            this.lblQuestion.Size = new System.Drawing.Size(1100, 30);
            this.lblQuestion.Text = "Question: are we on track for this month's ticket target?";
            this.lblQuestion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // pnlCommands  (Top) — every path of the lab has a control here
            //
            this.pnlCommands.Controls.Add(this.btnRefresh);
            this.pnlCommands.Controls.Add(this.lblLastRefreshed);
            this.pnlCommands.Controls.Add(this.chkSimulateFailure);
            this.pnlCommands.Controls.Add(this.btnSimulateOversized);
            this.pnlCommands.Controls.Add(this.btnSimulateWrongType);
            this.pnlCommands.Dock = Wisej.Web.DockStyle.Top;
            this.pnlCommands.FlowDirection = Wisej.Web.FlowDirection.LeftToRight;
            this.pnlCommands.Name = "pnlCommands";
            this.pnlCommands.Size = new System.Drawing.Size(1100, 44);
            this.pnlCommands.WrapContents = true;
            //
            // btnRefresh  (the single entry point: SetLoading → GetDashboard → RefreshDashboard)
            //
            this.btnRefresh.AccessibleName = "Refresh the dashboard";
            this.btnRefresh.Margin = new Wisej.Web.Padding(0, 4, 12, 4);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(110, 30);
            this.btnRefresh.TabIndex = 1;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            //
            // lblLastRefreshed  (the freshness stamp RefreshDashboard writes)
            //
            this.lblLastRefreshed.AutoSize = false;
            this.lblLastRefreshed.ForeColor = System.Drawing.Color.FromArgb(58, 77, 99);
            this.lblLastRefreshed.Margin = new Wisej.Web.Padding(0, 4, 18, 4);
            this.lblLastRefreshed.Name = "lblLastRefreshed";
            this.lblLastRefreshed.Size = new System.Drawing.Size(230, 30);
            this.lblLastRefreshed.Text = "Last refreshed —";
            this.lblLastRefreshed.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // chkSimulateFailure  (failure path: DashboardService.GetDashboard throws)
            //
            this.chkSimulateFailure.AccessibleName = "Make the next dashboard refresh fail";
            this.chkSimulateFailure.Margin = new Wisej.Web.Padding(0, 4, 16, 4);
            this.chkSimulateFailure.Name = "chkSimulateFailure";
            this.chkSimulateFailure.Size = new System.Drawing.Size(190, 30);
            this.chkSimulateFailure.TabIndex = 2;
            this.chkSimulateFailure.Text = "Simulate service failure";
            this.chkSimulateFailure.CheckedChanged += new System.EventHandler(this.chkSimulateFailure_CheckedChanged);
            //
            // btnSimulateOversized  (validation path: the SERVER-side size check, which the browser normally blocks first)
            //
            this.btnSimulateOversized.AccessibleName = "Send an oversized report to the server-side check";
            this.btnSimulateOversized.Margin = new Wisej.Web.Padding(0, 4, 12, 4);
            this.btnSimulateOversized.Name = "btnSimulateOversized";
            this.btnSimulateOversized.Size = new System.Drawing.Size(200, 30);
            this.btnSimulateOversized.TabIndex = 3;
            this.btnSimulateOversized.Text = "Simulate oversized upload";
            this.btnSimulateOversized.Click += new System.EventHandler(this.btnSimulateOversized_Click);
            //
            // btnSimulateWrongType  (validation path: the SERVER-side type check)
            //
            this.btnSimulateWrongType.AccessibleName = "Send a non-PDF report to the server-side check";
            this.btnSimulateWrongType.Margin = new Wisej.Web.Padding(0, 4, 12, 4);
            this.btnSimulateWrongType.Name = "btnSimulateWrongType";
            this.btnSimulateWrongType.Size = new System.Drawing.Size(210, 30);
            this.btnSimulateWrongType.TabIndex = 4;
            this.btnSimulateWrongType.Text = "Simulate wrong-type upload";
            this.btnSimulateWrongType.Click += new System.EventHandler(this.btnSimulateWrongType_Click);
            //
            // tlpDashboard  (Fill, added FIRST) — 3 columns x 2 rows, all percent so the tab can be resized
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
            this.tlpDashboard.Size = new System.Drawing.Size(1100, 570);
            //
            // pnlChartCard  (white card: chart Fill first, then the two header labels)
            //
            this.pnlChartCard.BackColor = System.Drawing.Color.White;
            this.pnlChartCard.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlChartCard.Controls.Add(this.chartTickets);
            this.pnlChartCard.Controls.Add(this.lblChartHint);
            this.pnlChartCard.Controls.Add(this.lblChartTitle);
            this.pnlChartCard.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlChartCard.Margin = new Wisej.Web.Padding(0, 0, 12, 12);
            this.pnlChartCard.Name = "pnlChartCard";
            this.pnlChartCard.Padding = new Wisej.Web.Padding(14, 10, 14, 12);
            //
            // chartTickets  (ChartJS · Bar — the trend; options in ConfigureChart(), data in RefreshDashboard())
            //
            this.chartTickets.ChartType = Wisej.Web.Ext.ChartJS.ChartType.Bar;
            this.chartTickets.Dock = Wisej.Web.DockStyle.Fill;
            this.chartTickets.Name = "chartTickets";
            this.chartTickets.TabIndex = 5;
            //
            // lblChartHint
            //
            this.lblChartHint.AutoSize = false;
            this.lblChartHint.Dock = Wisej.Web.DockStyle.Top;
            this.lblChartHint.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblChartHint.Name = "lblChartHint";
            this.lblChartHint.Size = new System.Drawing.Size(400, 22);
            this.lblChartHint.Text = "ChartJS · bar · Labels and DataSets filled together from DashboardModel";
            this.lblChartHint.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
            // pdfPreview  (PdfViewer — the document the model points at; PdfSource for the shipped sample,
            //              PdfStream for a document DocumentStore is holding in memory)
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
            // lblUploadResult  (Fill: what the server did with the bytes)
            //
            this.lblUploadResult.AutoSize = false;
            this.lblUploadResult.Dock = Wisej.Web.DockStyle.Fill;
            this.lblUploadResult.ForeColor = System.Drawing.Color.FromArgb(58, 77, 99);
            this.lblUploadResult.Name = "lblUploadResult";
            this.lblUploadResult.Text = "No report uploaded yet — the preview shows the sample shipped in wwwroot/.";
            this.lblUploadResult.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // uploadReport  (Upload — AllowedFileTypes / MaxFileSize are set in ConfigureUpload() from DocumentStore,
            //                so the browser filter and the server-side rule can never drift apart)
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
            this.lblUploadHint.Size = new System.Drawing.Size(300, 34);
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
            // htmlPreview  (HtmlPanel — the model in words; the markup is built by Dashboard/PreviewHtml.cs and
            //               every value in it is HTML-encoded there)
            //
            this.htmlPreview.Dock = Wisej.Web.DockStyle.Fill;
            this.htmlPreview.Name = "htmlPreview";
            this.htmlPreview.ScrollBars = Wisej.Web.ScrollBars.Vertical;
            this.htmlPreview.TabIndex = 9;
            //
            // progressCompletion  (ProgressBar — ONE status value; Minimum/Maximum make the value read as a percentage)
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
            // RESERVED SLOT for the GoogleMaps extension (Wisej-4-GoogleMaps NuGet package).
            // GoogleMaps is a server-side component like every other control here — it has map types, markers,
            // options, shapes, routes and server-side events (map click, marker drag) — but it needs a
            // **Google Maps API key** configured on the component before it renders a single tile, and the key
            // is billed per load. This lab therefore reserves the slot and documents the boundary instead of
            // shipping a key. With the package and a key it becomes:
            //
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
            this.lblMapNote.Text = "Map slot reserved · GoogleMaps needs an API key configured on the component";
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
            // Fill first, edge bars last: tlpDashboard (Fill) → pnlCommands (Top) → pnlHeader (Top, added last so
            // it ends up above the command row).
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.tlpDashboard);
            this.Controls.Add(this.pnlCommands);
            this.Controls.Add(this.pnlHeader);
            this.Name = "DashboardPage";
            this.Padding = new Wisej.Web.Padding(16, 14, 16, 16);
            this.Size = new System.Drawing.Size(1100, 704);
            this.pnlMapPlaceholder.ResumeLayout(false);
            this.pnlMapCard.ResumeLayout(false);
            this.pnlCompletionCard.ResumeLayout(false);
            this.pnlUploadCard.ResumeLayout(false);
            this.pnlPreviewCard.ResumeLayout(false);
            this.pnlChartCard.ResumeLayout(false);
            this.tlpDashboard.ResumeLayout(false);
            this.pnlCommands.ResumeLayout(false);
            this.pnlHeader.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlHeader;
        private Wisej.Web.Label lblTitle;
        private Wisej.Web.Label lblQuestion;
        private Wisej.Web.FlowLayoutPanel pnlCommands;
        private Wisej.Web.Button btnRefresh;
        private Wisej.Web.Label lblLastRefreshed;
        private Wisej.Web.CheckBox chkSimulateFailure;
        private Wisej.Web.Button btnSimulateOversized;
        private Wisej.Web.Button btnSimulateWrongType;
        private Wisej.Web.TableLayoutPanel tlpDashboard;
        private Wisej.Web.Panel pnlChartCard;
        private Wisej.Web.Ext.ChartJS.ChartJS chartTickets;
        private Wisej.Web.Label lblChartTitle;
        private Wisej.Web.Label lblChartHint;
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
