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
            this.panelDocuments = new Wisej.Web.Panel();
            this.labelImportTitle = new Wisej.Web.Label();
            this.upload = new Wisej.Web.Upload();
            this.labelImportResult = new Wisej.Web.Label();
            this.labelInvoiceTitle = new Wisej.Web.Label();
            this.comboOrders = new Wisej.Web.ComboBox();
            this.buttonPrintPdf = new Wisej.Web.Button();
            this.labelExportTitle = new Wisej.Web.Label();
            this.buttonExportXlsx = new Wisej.Web.Button();
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
            this.timerQueue = new Wisej.Web.Timer(this.components);
            this.panelDocuments.SuspendLayout();
            this.panelQueue.SuspendLayout();
            this.SuspendLayout();
            //
            // panelDocuments
            //
            this.panelDocuments.BackColor = System.Drawing.Color.White;
            this.panelDocuments.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelDocuments.Controls.Add(this.labelImportTitle);
            this.panelDocuments.Controls.Add(this.upload);
            this.panelDocuments.Controls.Add(this.labelImportResult);
            this.panelDocuments.Controls.Add(this.labelInvoiceTitle);
            this.panelDocuments.Controls.Add(this.comboOrders);
            this.panelDocuments.Controls.Add(this.buttonPrintPdf);
            this.panelDocuments.Controls.Add(this.labelExportTitle);
            this.panelDocuments.Controls.Add(this.buttonExportXlsx);
            this.panelDocuments.Location = new System.Drawing.Point(20, 20);
            this.panelDocuments.Name = "panelDocuments";
            this.panelDocuments.Size = new System.Drawing.Size(620, 256);
            //
            // labelImportTitle
            //
            this.labelImportTitle.AutoSize = false;
            this.labelImportTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelImportTitle.Location = new System.Drawing.Point(20, 12);
            this.labelImportTitle.Name = "labelImportTitle";
            this.labelImportTitle.Size = new System.Drawing.Size(300, 28);
            this.labelImportTitle.Text = "Import orders";
            //
            // upload
            //
            this.upload.AllowedFileTypes = ".csv";
            this.upload.AllowMultipleFiles = false;
            this.upload.Location = new System.Drawing.Point(20, 46);
            this.upload.MaxFileSize = 1048576;
            this.upload.Name = "upload";
            this.upload.Size = new System.Drawing.Size(200, 30);
            this.upload.Text = "Upload order-batch.csv…";
            this.upload.Uploaded += new Wisej.Web.UploadedEventHandler(this.upload_Uploaded);
            this.upload.Error += new Wisej.Web.UploadErrorEventHandler(this.upload_Error);
            //
            // labelImportResult
            //
            this.labelImportResult.AutoSize = false;
            this.labelImportResult.Font = new System.Drawing.Font("default", 9F);
            this.labelImportResult.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelImportResult.Location = new System.Drawing.Point(230, 46);
            this.labelImportResult.Name = "labelImportResult";
            this.labelImportResult.Size = new System.Drawing.Size(370, 30);
            this.labelImportResult.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // labelInvoiceTitle
            //
            this.labelInvoiceTitle.AutoSize = false;
            this.labelInvoiceTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelInvoiceTitle.Location = new System.Drawing.Point(20, 92);
            this.labelInvoiceTitle.Name = "labelInvoiceTitle";
            this.labelInvoiceTitle.Size = new System.Drawing.Size(300, 28);
            this.labelInvoiceTitle.Text = "Print Invoice";
            //
            // comboOrders
            //
            this.comboOrders.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.comboOrders.Location = new System.Drawing.Point(20, 126);
            this.comboOrders.Name = "comboOrders";
            this.comboOrders.Size = new System.Drawing.Size(300, 28);
            //
            // buttonPrintPdf
            //
            this.buttonPrintPdf.Location = new System.Drawing.Point(326, 126);
            this.buttonPrintPdf.Name = "buttonPrintPdf";
            this.buttonPrintPdf.Size = new System.Drawing.Size(170, 28);
            this.buttonPrintPdf.Text = "Print Invoice (PDF)";
            this.buttonPrintPdf.Click += new System.EventHandler(this.buttonPrintPdf_Click);
            //
            // labelExportTitle
            //
            this.labelExportTitle.AutoSize = false;
            this.labelExportTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelExportTitle.Location = new System.Drawing.Point(20, 172);
            this.labelExportTitle.Name = "labelExportTitle";
            this.labelExportTitle.Size = new System.Drawing.Size(300, 28);
            this.labelExportTitle.Text = "Export";
            //
            // buttonExportXlsx
            //
            this.buttonExportXlsx.Location = new System.Drawing.Point(20, 206);
            this.buttonExportXlsx.Name = "buttonExportXlsx";
            this.buttonExportXlsx.Size = new System.Drawing.Size(150, 28);
            this.buttonExportXlsx.Text = "Export .xlsx ⬇";
            this.buttonExportXlsx.Click += new System.EventHandler(this.buttonExportXlsx_Click);
            //
            // panelQueue
            //
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
            this.panelQueue.Location = new System.Drawing.Point(20, 290);
            this.panelQueue.Name = "panelQueue";
            this.panelQueue.Size = new System.Drawing.Size(620, 302);
            //
            // labelQueueTitle
            //
            this.labelQueueTitle.AutoSize = false;
            this.labelQueueTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelQueueTitle.Location = new System.Drawing.Point(20, 12);
            this.labelQueueTitle.Name = "labelQueueTitle";
            this.labelQueueTitle.Size = new System.Drawing.Size(280, 28);
            this.labelQueueTitle.Text = "Report queue";
            //
            // labelQueueCounts
            //
            this.labelQueueCounts.AutoSize = false;
            this.labelQueueCounts.Font = new System.Drawing.Font("default", 9F);
            this.labelQueueCounts.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelQueueCounts.Location = new System.Drawing.Point(300, 14);
            this.labelQueueCounts.Name = "labelQueueCounts";
            this.labelQueueCounts.Size = new System.Drawing.Size(300, 24);
            this.labelQueueCounts.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // gridJobs
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
            this.gridJobs.Size = new System.Drawing.Size(580, 150);
            this.gridJobs.SelectionChanged += new System.EventHandler(this.gridJobs_SelectionChanged);
            this.colJobId.HeaderText = "#"; this.colJobId.Name = "colJobId"; this.colJobId.Width = 36; this.colJobId.ReadOnly = true;
            this.colJobName.HeaderText = "Name"; this.colJobName.Name = "colJobName"; this.colJobName.Width = 176; this.colJobName.ReadOnly = true;
            this.colJobOwner.HeaderText = "Owner"; this.colJobOwner.Name = "colJobOwner"; this.colJobOwner.Width = 64; this.colJobOwner.ReadOnly = true;
            this.colJobStatus.HeaderText = "Status"; this.colJobStatus.Name = "colJobStatus"; this.colJobStatus.Width = 76; this.colJobStatus.ReadOnly = true;
            this.colJobProgress.HeaderText = "Progress"; this.colJobProgress.Name = "colJobProgress"; this.colJobProgress.Width = 84; this.colJobProgress.ReadOnly = true;
            this.colJobProgress.DefaultCellStyle.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
            this.colJobResult.HeaderText = "Result"; this.colJobResult.Name = "colJobResult"; this.colJobResult.Width = 136; this.colJobResult.ReadOnly = true;
            //
            // progressJob
            //
            this.progressJob.Location = new System.Drawing.Point(20, 202);
            this.progressJob.Maximum = 100;
            this.progressJob.Minimum = 0;
            this.progressJob.Name = "progressJob";
            this.progressJob.Size = new System.Drawing.Size(580, 12);
            this.progressJob.Value = 0;
            //
            // buttonQueueBatch
            //
            this.buttonQueueBatch.Location = new System.Drawing.Point(20, 222);
            this.buttonQueueBatch.Name = "buttonQueueBatch";
            this.buttonQueueBatch.Size = new System.Drawing.Size(170, 28);
            this.buttonQueueBatch.Text = "Queue invoice batch ×1,204";
            this.buttonQueueBatch.Click += new System.EventHandler(this.buttonQueueBatch_Click);
            //
            // buttonQueueStatement
            //
            this.buttonQueueStatement.Location = new System.Drawing.Point(196, 222);
            this.buttonQueueStatement.Name = "buttonQueueStatement";
            this.buttonQueueStatement.Size = new System.Drawing.Size(160, 28);
            this.buttonQueueStatement.Text = "Queue monthly statement";
            this.buttonQueueStatement.Click += new System.EventHandler(this.buttonQueueStatement_Click);
            //
            // buttonQueueSummary
            //
            this.buttonQueueSummary.Location = new System.Drawing.Point(362, 222);
            this.buttonQueueSummary.Name = "buttonQueueSummary";
            this.buttonQueueSummary.Size = new System.Drawing.Size(130, 28);
            this.buttonQueueSummary.Text = "Queue Q2 summary";
            this.buttonQueueSummary.Click += new System.EventHandler(this.buttonQueueSummary_Click);
            //
            // buttonCancelJob
            //
            this.buttonCancelJob.Location = new System.Drawing.Point(20, 258);
            this.buttonCancelJob.Name = "buttonCancelJob";
            this.buttonCancelJob.Size = new System.Drawing.Size(115, 28);
            this.buttonCancelJob.Text = "Cancel selected";
            this.buttonCancelJob.Click += new System.EventHandler(this.buttonCancelJob_Click);
            //
            // buttonViewResult
            //
            this.buttonViewResult.Location = new System.Drawing.Point(141, 258);
            this.buttonViewResult.Name = "buttonViewResult";
            this.buttonViewResult.Size = new System.Drawing.Size(110, 28);
            this.buttonViewResult.Text = "View result ▸";
            this.buttonViewResult.Click += new System.EventHandler(this.buttonViewResult_Click);
            //
            // buttonDownloadResult
            //
            this.buttonDownloadResult.Location = new System.Drawing.Point(257, 258);
            this.buttonDownloadResult.Name = "buttonDownloadResult";
            this.buttonDownloadResult.Size = new System.Drawing.Size(130, 28);
            this.buttonDownloadResult.Text = "Download result ⬇";
            this.buttonDownloadResult.Click += new System.EventHandler(this.buttonDownloadResult_Click);
            //
            // buttonSecondSession
            //
            this.buttonSecondSession.Location = new System.Drawing.Point(393, 258);
            this.buttonSecondSession.Name = "buttonSecondSession";
            this.buttonSecondSession.Size = new System.Drawing.Size(160, 28);
            this.buttonSecondSession.Text = "Open second session ↗";
            this.buttonSecondSession.Click += new System.EventHandler(this.buttonSecondSession_Click);
            //
            // timerQueue  (polls the shared queue once a second)
            //
            this.timerQueue.Interval = 1000;
            this.timerQueue.Tick += new System.EventHandler(this.timerQueue_Tick);
            //
            // MainPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.panelDocuments);
            this.Controls.Add(this.panelQueue);
            this.Name = "MainPage";
            this.Size = new System.Drawing.Size(660, 612);
            this.Text = "OrderDesk — Documents";
            this.Load += new System.EventHandler(this.MainPage_Load);
            this.panelDocuments.ResumeLayout(false);
            this.panelQueue.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel panelDocuments;
        private Wisej.Web.Label labelImportTitle;
        private Wisej.Web.Upload upload;
        private Wisej.Web.Label labelImportResult;
        private Wisej.Web.Label labelInvoiceTitle;
        private Wisej.Web.ComboBox comboOrders;
        private Wisej.Web.Button buttonPrintPdf;
        private Wisej.Web.Label labelExportTitle;
        private Wisej.Web.Button buttonExportXlsx;
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
        private Wisej.Web.Timer timerQueue;
    }
}
