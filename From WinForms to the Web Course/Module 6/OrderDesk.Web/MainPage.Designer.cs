namespace OrderDesk
{
    partial class MainPage
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Wisej.NET Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panelBoundaries = new Wisej.Web.Panel();
            this.labelBoundariesTitle = new Wisej.Web.Label();
            this.labelStatus = new Wisej.Web.Label();
            this.buttonClassAll = new Wisej.Web.Button();
            this.buttonClassServer = new Wisej.Web.Button();
            this.buttonClassUpload = new Wisej.Web.Button();
            this.buttonClassDownload = new Wisej.Web.Button();
            this.buttonClassClient = new Wisej.Web.Button();
            this.buttonClassRedesign = new Wisej.Web.Button();
            this.gridBoundaries = new Wisej.Web.DataGridView();
            this.colFeature = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colLegacyApi = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colClass = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colReplacement = new Wisej.Web.DataGridViewTextBoxColumn();
            this.labelBoundaryDetail = new Wisej.Web.Label();
            this.panelUpload = new Wisej.Web.Panel();
            this.labelUploadTitle = new Wisej.Web.Label();
            this.upload = new Wisej.Web.Upload();
            this.buttonSampleCsv = new Wisej.Web.Button();
            this.buttonLegacyImport = new Wisej.Web.Button();
            this.labelUploadDetail = new Wisej.Web.Label();
            this.panelExport = new Wisej.Web.Panel();
            this.labelExportTitle = new Wisej.Web.Label();
            this.comboOrders = new Wisej.Web.ComboBox();
            this.buttonPrintPdf = new Wisej.Web.Button();
            this.buttonLegacyPrint = new Wisej.Web.Button();
            this.buttonExportXlsx = new Wisej.Web.Button();
            this.buttonExportCsv = new Wisej.Web.Button();
            this.buttonLegacyExcel = new Wisej.Web.Button();
            this.labelBanner = new Wisej.Web.Label();
            this.trace = new OrderDesk.Views.TracePanel();
            this.panelQueue = new Wisej.Web.Panel();
            this.labelQueueTitle = new Wisej.Web.Label();
            this.labelQueueCounts = new Wisej.Web.Label();
            this.gridJobs = new Wisej.Web.DataGridView();
            this.colJobId = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colJobName = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colJobOwner = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colJobStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colJobProgress = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colJobResult = new Wisej.Web.DataGridViewTextBoxColumn();
            this.progressJob = new Wisej.Web.ProgressBar();
            this.buttonQueueBatch = new Wisej.Web.Button();
            this.buttonQueueStatement = new Wisej.Web.Button();
            this.buttonQueueSummary = new Wisej.Web.Button();
            this.buttonCancelJob = new Wisej.Web.Button();
            this.buttonViewResult = new Wisej.Web.Button();
            this.buttonDownloadResult = new Wisej.Web.Button();
            this.buttonSecondSession = new Wisej.Web.Button();
            this.buttonClear = new Wisej.Web.Button();
            this.timerQueue = new Wisej.Web.Timer(this.components);
            this.panelBoundaries.SuspendLayout();
            this.panelUpload.SuspendLayout();
            this.panelExport.SuspendLayout();
            this.panelQueue.SuspendLayout();
            this.SuspendLayout();
            //
            // panelBoundaries  (card A: deliverable 1, the classified file boundaries)
            //
            this.panelBoundaries.BackColor = System.Drawing.Color.White;
            this.panelBoundaries.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelBoundaries.Controls.Add(this.labelBoundariesTitle);
            this.panelBoundaries.Controls.Add(this.labelStatus);
            this.panelBoundaries.Controls.Add(this.buttonClassAll);
            this.panelBoundaries.Controls.Add(this.buttonClassServer);
            this.panelBoundaries.Controls.Add(this.buttonClassUpload);
            this.panelBoundaries.Controls.Add(this.buttonClassDownload);
            this.panelBoundaries.Controls.Add(this.buttonClassClient);
            this.panelBoundaries.Controls.Add(this.buttonClassRedesign);
            this.panelBoundaries.Controls.Add(this.gridBoundaries);
            this.panelBoundaries.Controls.Add(this.labelBoundaryDetail);
            this.panelBoundaries.Location = new System.Drawing.Point(30, 30);
            this.panelBoundaries.Name = "panelBoundaries";
            this.panelBoundaries.Size = new System.Drawing.Size(640, 240);
            //
            // labelBoundariesTitle
            //
            this.labelBoundariesTitle.AutoSize = false;
            this.labelBoundariesTitle.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.labelBoundariesTitle.Location = new System.Drawing.Point(20, 14);
            this.labelBoundariesTitle.Name = "labelBoundariesTitle";
            this.labelBoundariesTitle.Size = new System.Drawing.Size(380, 30);
            this.labelBoundariesTitle.Text = "File boundary classification · LegacyOrderDesk";
            //
            // labelStatus
            //
            this.labelStatus.AutoSize = false;
            this.labelStatus.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelStatus.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.labelStatus.Location = new System.Drawing.Point(400, 18);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(222, 24);
            this.labelStatus.Text = "● loading";
            this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // class filter buttons: the five answers of the classification exercise
            //
            this.buttonClassAll.Location = new System.Drawing.Point(20, 50);
            this.buttonClassAll.Name = "buttonClassAll";
            this.buttonClassAll.Size = new System.Drawing.Size(50, 28);
            this.buttonClassAll.Text = "All";
            this.buttonClassAll.Click += new System.EventHandler(this.buttonClassAll_Click);
            this.buttonClassServer.Location = new System.Drawing.Point(76, 50);
            this.buttonClassServer.Name = "buttonClassServer";
            this.buttonClassServer.Size = new System.Drawing.Size(110, 28);
            this.buttonClassServer.Text = "Server storage";
            this.buttonClassServer.Click += new System.EventHandler(this.buttonClassServer_Click);
            this.buttonClassUpload.Location = new System.Drawing.Point(192, 50);
            this.buttonClassUpload.Name = "buttonClassUpload";
            this.buttonClassUpload.Size = new System.Drawing.Size(70, 28);
            this.buttonClassUpload.Text = "Upload";
            this.buttonClassUpload.Click += new System.EventHandler(this.buttonClassUpload_Click);
            this.buttonClassDownload.Location = new System.Drawing.Point(268, 50);
            this.buttonClassDownload.Name = "buttonClassDownload";
            this.buttonClassDownload.Size = new System.Drawing.Size(84, 28);
            this.buttonClassDownload.Text = "Download";
            this.buttonClassDownload.Click += new System.EventHandler(this.buttonClassDownload_Click);
            this.buttonClassClient.Location = new System.Drawing.Point(358, 50);
            this.buttonClassClient.Name = "buttonClassClient";
            this.buttonClassClient.Size = new System.Drawing.Size(124, 28);
            this.buttonClassClient.Text = "ClientFileSystem";
            this.buttonClassClient.Click += new System.EventHandler(this.buttonClassClient_Click);
            this.buttonClassRedesign.Location = new System.Drawing.Point(488, 50);
            this.buttonClassRedesign.Name = "buttonClassRedesign";
            this.buttonClassRedesign.Size = new System.Drawing.Size(84, 28);
            this.buttonClassRedesign.Text = "Redesign";
            this.buttonClassRedesign.Click += new System.EventHandler(this.buttonClassRedesign_Click);
            //
            // gridBoundaries
            //
            this.gridBoundaries.AllowUserToAddRows = false;
            this.gridBoundaries.AllowUserToDeleteRows = false;
            this.gridBoundaries.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] { this.colFeature, this.colLegacyApi, this.colClass, this.colReplacement });
            this.gridBoundaries.Location = new System.Drawing.Point(20, 86);
            this.gridBoundaries.MultiSelect = false;
            this.gridBoundaries.Name = "gridBoundaries";
            this.gridBoundaries.ReadOnly = true;
            this.gridBoundaries.RowHeadersVisible = false;
            this.gridBoundaries.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.gridBoundaries.Size = new System.Drawing.Size(602, 104);
            this.gridBoundaries.SelectionChanged += new System.EventHandler(this.gridBoundaries_SelectionChanged);
            this.colFeature.HeaderText = "Feature"; this.colFeature.Name = "colFeature"; this.colFeature.Width = 120; this.colFeature.ReadOnly = true;
            this.colLegacyApi.HeaderText = "Legacy API · path"; this.colLegacyApi.Name = "colLegacyApi"; this.colLegacyApi.Width = 210; this.colLegacyApi.ReadOnly = true;
            this.colClass.HeaderText = "Class"; this.colClass.Name = "colClass"; this.colClass.Width = 100; this.colClass.ReadOnly = true;
            this.colReplacement.HeaderText = "Replacement"; this.colReplacement.Name = "colReplacement"; this.colReplacement.Width = 150; this.colReplacement.ReadOnly = true;
            //
            // labelBoundaryDetail  (legacy call → replacement → evidence of the selected row)
            //
            this.labelBoundaryDetail.AutoSize = false;
            this.labelBoundaryDetail.Font = new System.Drawing.Font("monospace", 9F);
            this.labelBoundaryDetail.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelBoundaryDetail.Location = new System.Drawing.Point(20, 194);
            this.labelBoundaryDetail.Name = "labelBoundaryDetail";
            this.labelBoundaryDetail.Size = new System.Drawing.Size(602, 40);
            this.labelBoundaryDetail.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // panelUpload  (card B: the import workflow — upload + server storage)
            //
            this.panelUpload.BackColor = System.Drawing.Color.White;
            this.panelUpload.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelUpload.Controls.Add(this.labelUploadTitle);
            this.panelUpload.Controls.Add(this.upload);
            this.panelUpload.Controls.Add(this.buttonSampleCsv);
            this.panelUpload.Controls.Add(this.buttonLegacyImport);
            this.panelUpload.Controls.Add(this.labelUploadDetail);
            this.panelUpload.Location = new System.Drawing.Point(30, 284);
            this.panelUpload.Name = "panelUpload";
            this.panelUpload.Size = new System.Drawing.Size(640, 150);
            //
            // labelUploadTitle
            //
            this.labelUploadTitle.AutoSize = false;
            this.labelUploadTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelUploadTitle.Location = new System.Drawing.Point(20, 12);
            this.labelUploadTitle.Name = "labelUploadTitle";
            this.labelUploadTitle.Size = new System.Drawing.Size(602, 28);
            this.labelUploadTitle.Text = "Import · upload + server storage  (was File.ReadAllLines(@\"C:\\Orders\\in.csv\"))";
            //
            // upload  (✓ the client file reaches the server through the browser, not a path)
            //
            this.upload.AllowedFileTypes = ".csv";
            this.upload.AllowMultipleFiles = false;
            this.upload.Location = new System.Drawing.Point(20, 46);
            this.upload.MaxFileSize = 1048576;
            this.upload.Name = "upload";
            this.upload.Size = new System.Drawing.Size(190, 28);
            this.upload.Text = "Upload order-batch.csv…";
            this.upload.ToolTipText = "Wisej.Web.Upload → StorageRoot.Uploads → OrdersCsvImporter → OrderService.Save (.csv, max 1 MB).";
            this.upload.Uploaded += new Wisej.Web.UploadedEventHandler(this.upload_Uploaded);
            this.upload.Error += new Wisej.Web.UploadErrorEventHandler(this.upload_Error);
            this.buttonSampleCsv.Location = new System.Drawing.Point(216, 46);
            this.buttonSampleCsv.Name = "buttonSampleCsv";
            this.buttonSampleCsv.Size = new System.Drawing.Size(150, 28);
            this.buttonSampleCsv.Text = "Download sample CSV ⬇";
            this.buttonSampleCsv.ToolTipText = "wwwroot/samples/order-batch.csv (8 rows) → Application.Download; upload it back.";
            this.buttonSampleCsv.Click += new System.EventHandler(this.buttonSampleCsv_Click);
            this.buttonLegacyImport.Location = new System.Drawing.Point(372, 46);
            this.buttonLegacyImport.Name = "buttonLegacyImport";
            this.buttonLegacyImport.Size = new System.Drawing.Size(250, 28);
            this.buttonLegacyImport.Text = "Legacy import C:\\Orders\\in.csv";
            this.buttonLegacyImport.ToolTipText = "The failure path: File.ReadAllLines on the SERVER's disk.";
            this.buttonLegacyImport.Click += new System.EventHandler(this.buttonLegacyImport_Click);
            //
            // labelUploadDetail  (storage root · uploads folder · last import)
            //
            this.labelUploadDetail.AutoSize = false;
            this.labelUploadDetail.Font = new System.Drawing.Font("monospace", 9F);
            this.labelUploadDetail.Location = new System.Drawing.Point(20, 82);
            this.labelUploadDetail.Name = "labelUploadDetail";
            this.labelUploadDetail.Size = new System.Drawing.Size(602, 58);
            this.labelUploadDetail.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // panelExport  (card C: export and print)
            //
            this.panelExport.BackColor = System.Drawing.Color.White;
            this.panelExport.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelExport.Controls.Add(this.labelExportTitle);
            this.panelExport.Controls.Add(this.comboOrders);
            this.panelExport.Controls.Add(this.buttonPrintPdf);
            this.panelExport.Controls.Add(this.buttonLegacyPrint);
            this.panelExport.Controls.Add(this.buttonExportXlsx);
            this.panelExport.Controls.Add(this.buttonExportCsv);
            this.panelExport.Controls.Add(this.buttonLegacyExcel);
            this.panelExport.Location = new System.Drawing.Point(30, 448);
            this.panelExport.Name = "panelExport";
            this.panelExport.Size = new System.Drawing.Size(640, 124);
            //
            // labelExportTitle
            //
            this.labelExportTitle.AutoSize = false;
            this.labelExportTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelExportTitle.Location = new System.Drawing.Point(20, 12);
            this.labelExportTitle.Name = "labelExportTitle";
            this.labelExportTitle.Size = new System.Drawing.Size(602, 28);
            this.labelExportTitle.Text = "Export & print · managed writers → Download / PdfViewer  (was Excel Interop + PrintDocument)";
            //
            // comboOrders + print row
            //
            this.comboOrders.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.comboOrders.Location = new System.Drawing.Point(20, 46);
            this.comboOrders.Name = "comboOrders";
            this.comboOrders.Size = new System.Drawing.Size(230, 28);
            this.comboOrders.ToolTipText = "The order to print (newest first).";
            this.buttonPrintPdf.Location = new System.Drawing.Point(256, 46);
            this.buttonPrintPdf.Name = "buttonPrintPdf";
            this.buttonPrintPdf.Size = new System.Drawing.Size(150, 28);
            this.buttonPrintPdf.Text = "Print Invoice → PDF";
            this.buttonPrintPdf.ToolTipText = "InvoiceDocument → InvoicePdfWriter on the server → PdfViewer (success path).";
            this.buttonPrintPdf.Click += new System.EventHandler(this.buttonPrintPdf_Click);
            this.buttonLegacyPrint.Location = new System.Drawing.Point(412, 46);
            this.buttonLegacyPrint.Name = "buttonLegacyPrint";
            this.buttonLegacyPrint.Size = new System.Drawing.Size(110, 28);
            this.buttonLegacyPrint.Text = "Legacy print";
            this.buttonLegacyPrint.ToolTipText = "PrintDocument → the SERVER's printer (failure path, explained).";
            this.buttonLegacyPrint.Click += new System.EventHandler(this.buttonLegacyPrint_Click);
            //
            // export row
            //
            this.buttonExportXlsx.Location = new System.Drawing.Point(20, 82);
            this.buttonExportXlsx.Name = "buttonExportXlsx";
            this.buttonExportXlsx.Size = new System.Drawing.Size(120, 28);
            this.buttonExportXlsx.Text = "Export .xlsx ⬇";
            this.buttonExportXlsx.ToolTipText = "XlsxWriter (no Office) → exports/ → Application.Download(path, \"Orders.xlsx\") (recovery).";
            this.buttonExportXlsx.Click += new System.EventHandler(this.buttonExportXlsx_Click);
            this.buttonExportCsv.Location = new System.Drawing.Point(146, 82);
            this.buttonExportCsv.Name = "buttonExportCsv";
            this.buttonExportCsv.Size = new System.Drawing.Size(104, 28);
            this.buttonExportCsv.Text = "Export .csv ⬇";
            this.buttonExportCsv.ToolTipText = "The Module 1 CSV export — its layout is accepted by the importer (round-trip test).";
            this.buttonExportCsv.Click += new System.EventHandler(this.buttonExportCsv_Click);
            this.buttonLegacyExcel.Location = new System.Drawing.Point(256, 82);
            this.buttonLegacyExcel.Name = "buttonLegacyExcel";
            this.buttonLegacyExcel.Size = new System.Drawing.Size(150, 28);
            this.buttonLegacyExcel.Text = "Legacy Excel Interop";
            this.buttonLegacyExcel.ToolTipText = "Probes the Excel.Application ProgID only — never creates the COM object (failure path).";
            this.buttonLegacyExcel.Click += new System.EventHandler(this.buttonLegacyExcel_Click);
            //
            // labelBanner  (one paragraph that explains the last failure or recovery)
            //
            this.labelBanner.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.labelBanner.AutoSize = false;
            this.labelBanner.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
            this.labelBanner.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelBanner.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            this.labelBanner.Location = new System.Drawing.Point(30, 584);
            this.labelBanner.Name = "labelBanner";
            this.labelBanner.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.labelBanner.Size = new System.Drawing.Size(640, 70);
            this.labelBanner.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelBanner.Visible = false;
            //
            // trace  (the migration log)
            //
            this.trace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.trace.Location = new System.Drawing.Point(690, 30);
            this.trace.Name = "trace";
            this.trace.Size = new System.Drawing.Size(628, 300);
            //
            // panelQueue  (card D: the process-wide report queue)
            //
            this.panelQueue.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelQueue.BackColor = System.Drawing.Color.White;
            this.panelQueue.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelQueue.Controls.Add(this.labelQueueTitle);
            this.panelQueue.Controls.Add(this.labelQueueCounts);
            this.panelQueue.Controls.Add(this.gridJobs);
            this.panelQueue.Controls.Add(this.progressJob);
            this.panelQueue.Controls.Add(this.buttonQueueBatch);
            this.panelQueue.Controls.Add(this.buttonQueueStatement);
            this.panelQueue.Controls.Add(this.buttonQueueSummary);
            this.panelQueue.Controls.Add(this.buttonCancelJob);
            this.panelQueue.Controls.Add(this.buttonViewResult);
            this.panelQueue.Controls.Add(this.buttonDownloadResult);
            this.panelQueue.Controls.Add(this.buttonSecondSession);
            this.panelQueue.Controls.Add(this.buttonClear);
            this.panelQueue.Location = new System.Drawing.Point(690, 344);
            this.panelQueue.Name = "panelQueue";
            this.panelQueue.Size = new System.Drawing.Size(628, 310);
            //
            // labelQueueTitle / labelQueueCounts
            //
            this.labelQueueTitle.AutoSize = false;
            this.labelQueueTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelQueueTitle.Location = new System.Drawing.Point(20, 12);
            this.labelQueueTitle.Name = "labelQueueTitle";
            this.labelQueueTitle.Size = new System.Drawing.Size(320, 28);
            this.labelQueueTitle.Text = "Report queue · one process, every session";
            this.labelQueueCounts.AutoSize = false;
            this.labelQueueCounts.Font = new System.Drawing.Font("default", 9F);
            this.labelQueueCounts.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelQueueCounts.Location = new System.Drawing.Point(340, 14);
            this.labelQueueCounts.Name = "labelQueueCounts";
            this.labelQueueCounts.Size = new System.Drawing.Size(268, 24);
            this.labelQueueCounts.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // gridJobs  (refreshed by timerQueue — the same rows in every session)
            //
            this.gridJobs.AllowUserToAddRows = false;
            this.gridJobs.AllowUserToDeleteRows = false;
            this.gridJobs.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] { this.colJobId, this.colJobName, this.colJobOwner, this.colJobStatus, this.colJobProgress, this.colJobResult });
            this.gridJobs.Location = new System.Drawing.Point(20, 46);
            this.gridJobs.MultiSelect = false;
            this.gridJobs.Name = "gridJobs";
            this.gridJobs.ReadOnly = true;
            this.gridJobs.RowHeadersVisible = false;
            this.gridJobs.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.gridJobs.Size = new System.Drawing.Size(588, 150);
            this.gridJobs.SelectionChanged += new System.EventHandler(this.gridJobs_SelectionChanged);
            this.colJobId.HeaderText = "#"; this.colJobId.Name = "colJobId"; this.colJobId.Width = 36; this.colJobId.ReadOnly = true;
            this.colJobName.HeaderText = "Name"; this.colJobName.Name = "colJobName"; this.colJobName.Width = 180; this.colJobName.ReadOnly = true;
            this.colJobOwner.HeaderText = "Owner"; this.colJobOwner.Name = "colJobOwner"; this.colJobOwner.Width = 64; this.colJobOwner.ReadOnly = true;
            this.colJobStatus.HeaderText = "Status"; this.colJobStatus.Name = "colJobStatus"; this.colJobStatus.Width = 76; this.colJobStatus.ReadOnly = true;
            this.colJobProgress.HeaderText = "Progress"; this.colJobProgress.Name = "colJobProgress"; this.colJobProgress.Width = 84; this.colJobProgress.ReadOnly = true;
            this.colJobProgress.DefaultCellStyle.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
            this.colJobResult.HeaderText = "Result"; this.colJobResult.Name = "colJobResult"; this.colJobResult.Width = 136; this.colJobResult.ReadOnly = true;
            //
            // progressJob  (the selected job's progress)
            //
            this.progressJob.Location = new System.Drawing.Point(20, 202);
            this.progressJob.Maximum = 100;
            this.progressJob.Minimum = 0;
            this.progressJob.Name = "progressJob";
            this.progressJob.Size = new System.Drawing.Size(588, 12);
            this.progressJob.Value = 0;
            //
            // queue buttons, row 1: queue + cancel
            //
            this.buttonQueueBatch.Location = new System.Drawing.Point(20, 222);
            this.buttonQueueBatch.Name = "buttonQueueBatch";
            this.buttonQueueBatch.Size = new System.Drawing.Size(170, 28);
            this.buttonQueueBatch.Text = "Queue invoice batch ×1,204";
            this.buttonQueueBatch.ToolTipText = "1,204 invoice pages, ~30 ms each ≈ 36 s: the progress path.";
            this.buttonQueueBatch.Click += new System.EventHandler(this.buttonQueueBatch_Click);
            this.buttonQueueStatement.Location = new System.Drawing.Point(196, 222);
            this.buttonQueueStatement.Name = "buttonQueueStatement";
            this.buttonQueueStatement.Size = new System.Drawing.Size(160, 28);
            this.buttonQueueStatement.Text = "Queue monthly statement";
            this.buttonQueueStatement.ToolTipText = "≈ 15 s; queued behind whatever is running.";
            this.buttonQueueStatement.Click += new System.EventHandler(this.buttonQueueStatement_Click);
            this.buttonQueueSummary.Location = new System.Drawing.Point(362, 222);
            this.buttonQueueSummary.Name = "buttonQueueSummary";
            this.buttonQueueSummary.Size = new System.Drawing.Size(125, 28);
            this.buttonQueueSummary.Text = "Queue Q2 summary";
            this.buttonQueueSummary.ToolTipText = "≈ 3 s — the first job to be Done.";
            this.buttonQueueSummary.Click += new System.EventHandler(this.buttonQueueSummary_Click);
            this.buttonCancelJob.Location = new System.Drawing.Point(493, 222);
            this.buttonCancelJob.Name = "buttonCancelJob";
            this.buttonCancelJob.Size = new System.Drawing.Size(115, 28);
            this.buttonCancelJob.Text = "Cancel selected";
            this.buttonCancelJob.Click += new System.EventHandler(this.buttonCancelJob_Click);
            //
            // queue buttons, row 2: results + second session + clear
            //
            this.buttonViewResult.Location = new System.Drawing.Point(20, 258);
            this.buttonViewResult.Name = "buttonViewResult";
            this.buttonViewResult.Size = new System.Drawing.Size(110, 28);
            this.buttonViewResult.Text = "View result ▸";
            this.buttonViewResult.ToolTipText = "The Done job's PDF from reports/ in a PdfViewer.";
            this.buttonViewResult.Click += new System.EventHandler(this.buttonViewResult_Click);
            this.buttonDownloadResult.Location = new System.Drawing.Point(136, 258);
            this.buttonDownloadResult.Name = "buttonDownloadResult";
            this.buttonDownloadResult.Size = new System.Drawing.Size(130, 28);
            this.buttonDownloadResult.Text = "Download result ⬇";
            this.buttonDownloadResult.ToolTipText = "Application.Download(job.ResultPath, job.ResultFileName).";
            this.buttonDownloadResult.Click += new System.EventHandler(this.buttonDownloadResult_Click);
            this.buttonSecondSession.Location = new System.Drawing.Point(272, 258);
            this.buttonSecondSession.Name = "buttonSecondSession";
            this.buttonSecondSession.Size = new System.Drawing.Size(150, 28);
            this.buttonSecondSession.Text = "Open second session ↗";
            this.buttonSecondSession.ToolTipText = "A second browser session: same queue, different owner.";
            this.buttonSecondSession.Click += new System.EventHandler(this.buttonSecondSession_Click);
            this.buttonClear.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.buttonClear.Location = new System.Drawing.Point(550, 258);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(58, 28);
            this.buttonClear.Text = "Clear";
            this.buttonClear.ToolTipText = "Clear the trace.";
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            //
            // timerQueue  (polls the shared queue once a second)
            //
            this.timerQueue.Interval = 1000;
            this.timerQueue.Tick += new System.EventHandler(this.timerQueue_Tick);
            //
            // MainPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.panelBoundaries);
            this.Controls.Add(this.panelUpload);
            this.Controls.Add(this.panelExport);
            this.Controls.Add(this.labelBanner);
            this.Controls.Add(this.trace);
            this.Controls.Add(this.panelQueue);
            this.Name = "MainPage";
            this.Size = new System.Drawing.Size(1348, 684);
            this.Text = "OrderDesk — Files, Reports & Browser Boundaries";
            this.Load += new System.EventHandler(this.MainPage_Load);
            this.panelBoundaries.ResumeLayout(false);
            this.panelUpload.ResumeLayout(false);
            this.panelExport.ResumeLayout(false);
            this.panelQueue.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel panelBoundaries;
        private Wisej.Web.Label labelBoundariesTitle;
        private Wisej.Web.Label labelStatus;
        private Wisej.Web.Button buttonClassAll;
        private Wisej.Web.Button buttonClassServer;
        private Wisej.Web.Button buttonClassUpload;
        private Wisej.Web.Button buttonClassDownload;
        private Wisej.Web.Button buttonClassClient;
        private Wisej.Web.Button buttonClassRedesign;
        private Wisej.Web.DataGridView gridBoundaries;
        private Wisej.Web.DataGridViewTextBoxColumn colFeature;
        private Wisej.Web.DataGridViewTextBoxColumn colLegacyApi;
        private Wisej.Web.DataGridViewTextBoxColumn colClass;
        private Wisej.Web.DataGridViewTextBoxColumn colReplacement;
        private Wisej.Web.Label labelBoundaryDetail;
        private Wisej.Web.Panel panelUpload;
        private Wisej.Web.Label labelUploadTitle;
        private Wisej.Web.Upload upload;
        private Wisej.Web.Button buttonSampleCsv;
        private Wisej.Web.Button buttonLegacyImport;
        private Wisej.Web.Label labelUploadDetail;
        private Wisej.Web.Panel panelExport;
        private Wisej.Web.Label labelExportTitle;
        private Wisej.Web.ComboBox comboOrders;
        private Wisej.Web.Button buttonPrintPdf;
        private Wisej.Web.Button buttonLegacyPrint;
        private Wisej.Web.Button buttonExportXlsx;
        private Wisej.Web.Button buttonExportCsv;
        private Wisej.Web.Button buttonLegacyExcel;
        private Wisej.Web.Label labelBanner;
        private OrderDesk.Views.TracePanel trace;
        private Wisej.Web.Panel panelQueue;
        private Wisej.Web.Label labelQueueTitle;
        private Wisej.Web.Label labelQueueCounts;
        private Wisej.Web.DataGridView gridJobs;
        private Wisej.Web.DataGridViewTextBoxColumn colJobId;
        private Wisej.Web.DataGridViewTextBoxColumn colJobName;
        private Wisej.Web.DataGridViewTextBoxColumn colJobOwner;
        private Wisej.Web.DataGridViewTextBoxColumn colJobStatus;
        private Wisej.Web.DataGridViewTextBoxColumn colJobProgress;
        private Wisej.Web.DataGridViewTextBoxColumn colJobResult;
        private Wisej.Web.ProgressBar progressJob;
        private Wisej.Web.Button buttonQueueBatch;
        private Wisej.Web.Button buttonQueueStatement;
        private Wisej.Web.Button buttonQueueSummary;
        private Wisej.Web.Button buttonCancelJob;
        private Wisej.Web.Button buttonViewResult;
        private Wisej.Web.Button buttonDownloadResult;
        private Wisej.Web.Button buttonSecondSession;
        private Wisej.Web.Button buttonClear;
        private Wisej.Web.Timer timerQueue;
    }
}
