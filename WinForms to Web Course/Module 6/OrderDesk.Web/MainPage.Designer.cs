namespace OrderDesk
{
    partial class MainPage
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

        #region Wisej Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.appBar = new Wisej.Web.Panel();
            this.appTitle = new Wisej.Web.Label();
            this.trace = new OrderDesk.Shared.TracePanel();
            this.statusLabel = new Wisej.Web.Label();
            this.bannerLabel = new Wisej.Web.Label();
            this.ordersCard = new Wisej.Web.Panel();
            this.ordersTitle = new Wisej.Web.Label();
            this.ordersGrid = new Wisej.Web.DataGridView();
            this.colOrder = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colCustomer = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colOwner = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colTotal = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.pdfCard = new Wisej.Web.Panel();
            this.pdfTitle = new Wisej.Web.Label();
            this.pdfDownloadButton = new Wisej.Web.Button();
            this.pdfViewer = new Wisej.Web.PdfViewer();
            this.pdfPlaceholder = new Wisej.Web.Label();
            this.importCard = new Wisej.Web.Panel();
            this.importTitle = new Wisej.Web.Label();
            this.upload = new Wisej.Web.Upload();
            this.storageLabel = new Wisej.Web.Label();
            this.importSummary = new Wisej.Web.Label();
            this.importResults = new Wisej.Web.ListBox();
            this.queueCard = new Wisej.Web.Panel();
            this.queueTitle = new Wisej.Web.Label();
            this.jobsGrid = new Wisej.Web.DataGridView();
            this.colJob = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colFile = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colJobStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colSession = new Wisej.Web.DataGridViewTextBoxColumn();
            this.queueProgress = new Wisej.Web.ProgressBar();
            this.queueLabel = new Wisej.Web.Label();
            this.downloadSampleButton = new Wisej.Web.Button();
            this.importLegacyButton = new Wisej.Web.Button();
            this.exportInteropButton = new Wisej.Web.Button();
            this.exportXlsxButton = new Wisej.Web.Button();
            this.printInvoiceButton = new Wisej.Web.Button();
            this.invoicePdfButton = new Wisej.Web.Button();
            this.queueReportButton = new Wisej.Web.Button();
            this.storageRootButton = new Wisej.Web.Button();
            this.appBar.SuspendLayout();
            this.ordersCard.SuspendLayout();
            this.pdfCard.SuspendLayout();
            this.importCard.SuspendLayout();
            this.queueCard.SuspendLayout();
            this.SuspendLayout();
            //
            // appBar
            //
            this.appBar.BackColor = OrderDesk.Shared.Palette.Accent;
            this.appBar.Controls.Add(this.appTitle);
            this.appBar.Dock = Wisej.Web.DockStyle.Top;
            this.appBar.Name = "appBar";
            this.appBar.Size = new System.Drawing.Size(1400, 44);
            //
            // appTitle
            //
            this.appTitle.AutoSize = false;
            this.appTitle.Dock = Wisej.Web.DockStyle.Fill;
            this.appTitle.Font = new System.Drawing.Font("default", 13F, System.Drawing.FontStyle.Bold);
            this.appTitle.ForeColor = System.Drawing.Color.White;
            this.appTitle.Name = "appTitle";
            this.appTitle.Padding = new Wisej.Web.Padding(20, 0, 0, 0);
            this.appTitle.Text = "OrderDesk — Documents   ·   Module 6: Files, Reports & Browser Boundaries";
            this.appTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // trace
            //
            this.trace.Dock = Wisej.Web.DockStyle.Right;
            this.trace.Name = "trace";
            this.trace.Size = new System.Drawing.Size(560, 716);
            //
            // statusLabel
            //
            this.statusLabel.AutoSize = false;
            this.statusLabel.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.statusLabel.ForeColor = OrderDesk.Shared.Palette.MutedText;
            this.statusLabel.Location = new System.Drawing.Point(16, 54);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(200, 28);
            this.statusLabel.Text = "● idle";
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // bannerLabel
            //
            this.bannerLabel.AutoEllipsis = true;
            this.bannerLabel.AutoSize = false;
            this.bannerLabel.BackColor = OrderDesk.Shared.Palette.BadSoft;
            this.bannerLabel.Font = new System.Drawing.Font("default", 9.5F);
            this.bannerLabel.ForeColor = OrderDesk.Shared.Palette.Ink;
            this.bannerLabel.Location = new System.Drawing.Point(224, 54);
            this.bannerLabel.Name = "bannerLabel";
            this.bannerLabel.Padding = new Wisej.Web.Padding(10, 0, 10, 0);
            this.bannerLabel.Size = new System.Drawing.Size(600, 28);
            this.bannerLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bannerLabel.Visible = false;
            //
            // ordersCard
            //
            this.ordersCard.BackColor = OrderDesk.Shared.Palette.CardBackground;
            this.ordersCard.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.ordersCard.Controls.Add(this.ordersGrid);
            this.ordersCard.Controls.Add(this.ordersTitle);
            this.ordersCard.Location = new System.Drawing.Point(16, 88);
            this.ordersCard.Name = "ordersCard";
            this.ordersCard.Padding = new Wisej.Web.Padding(8, 0, 8, 8);
            this.ordersCard.Size = new System.Drawing.Size(500, 186);
            //
            // ordersTitle
            //
            this.ordersTitle.AutoSize = false;
            this.ordersTitle.Dock = Wisej.Web.DockStyle.Top;
            this.ordersTitle.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.ordersTitle.ForeColor = OrderDesk.Shared.Palette.Ink;
            this.ordersTitle.Name = "ordersTitle";
            this.ordersTitle.Padding = new Wisej.Web.Padding(6, 0, 0, 0);
            this.ordersTitle.Size = new System.Drawing.Size(482, 30);
            this.ordersTitle.Text = "Orders  ·  the five walkthrough orders (+ imported rows) — select one for the invoice";
            this.ordersTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // ordersGrid
            //
            this.ordersGrid.AllowUserToAddRows = false;
            this.ordersGrid.AllowUserToDeleteRows = false;
            this.ordersGrid.AutoGenerateColumns = false;
            this.ordersGrid.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] { this.colOrder, this.colCustomer, this.colOwner, this.colTotal, this.colStatus });
            this.ordersGrid.Dock = Wisej.Web.DockStyle.Fill;
            this.ordersGrid.MultiSelect = false;
            this.ordersGrid.Name = "ordersGrid";
            this.ordersGrid.ReadOnly = true;
            this.ordersGrid.RowHeadersVisible = false;
            this.ordersGrid.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.ordersGrid.SelectionChanged += this.ordersGrid_SelectionChanged;
            this.colOrder.DataPropertyName = "Id";
            this.colOrder.HeaderText = "Order";
            this.colOrder.Name = "colOrder";
            this.colOrder.Width = 64;
            this.colCustomer.AutoSizeMode = Wisej.Web.DataGridViewAutoSizeColumnMode.Fill;
            this.colCustomer.DataPropertyName = "CustomerName";
            this.colCustomer.HeaderText = "Customer";
            this.colCustomer.Name = "colCustomer";
            this.colOwner.DataPropertyName = "Owner";
            this.colOwner.HeaderText = "Owner";
            this.colOwner.Name = "colOwner";
            this.colOwner.Width = 70;
            this.colTotal.DataPropertyName = "Total";
            this.colTotal.DefaultCellStyle.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
            this.colTotal.DefaultCellStyle.Format = "C2";
            this.colTotal.HeaderText = "Total";
            this.colTotal.Name = "colTotal";
            this.colTotal.Width = 96;
            this.colStatus.DataPropertyName = "Status";
            this.colStatus.HeaderText = "Status";
            this.colStatus.Name = "colStatus";
            this.colStatus.Width = 80;
            //
            // pdfCard
            //
            this.pdfCard.BackColor = OrderDesk.Shared.Palette.CardBackground;
            this.pdfCard.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pdfCard.Controls.Add(this.pdfViewer);
            this.pdfCard.Controls.Add(this.pdfPlaceholder);
            this.pdfCard.Controls.Add(this.pdfTitle);
            this.pdfCard.Location = new System.Drawing.Point(16, 282);
            this.pdfCard.Name = "pdfCard";
            this.pdfCard.Padding = new Wisej.Web.Padding(8, 0, 8, 8);
            this.pdfCard.Size = new System.Drawing.Size(500, 350);
            //
            // pdfTitle
            //
            this.pdfTitle.AutoSize = false;
            this.pdfTitle.Controls.Add(this.pdfDownloadButton);
            this.pdfTitle.Dock = Wisej.Web.DockStyle.Top;
            this.pdfTitle.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.pdfTitle.ForeColor = OrderDesk.Shared.Palette.Ink;
            this.pdfTitle.Name = "pdfTitle";
            this.pdfTitle.Padding = new Wisej.Web.Padding(6, 0, 0, 0);
            this.pdfTitle.Size = new System.Drawing.Size(482, 30);
            this.pdfTitle.Text = "PdfViewer  ·  no document yet";
            this.pdfTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // pdfDownloadButton
            //
            this.pdfDownloadButton.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.pdfDownloadButton.Location = new System.Drawing.Point(352, 2);
            this.pdfDownloadButton.Name = "pdfDownloadButton";
            this.pdfDownloadButton.Size = new System.Drawing.Size(130, 26);
            this.pdfDownloadButton.Text = "⬇ Download PDF";
            this.pdfDownloadButton.Visible = false;
            this.pdfDownloadButton.Click += this.pdfDownloadButton_Click;
            //
            // pdfViewer
            //
            this.pdfViewer.Dock = Wisej.Web.DockStyle.Fill;
            this.pdfViewer.Name = "pdfViewer";
            this.pdfViewer.ViewerType = Wisej.Web.PdfViewerType.Auto;
            this.pdfViewer.Visible = false;
            //
            // pdfPlaceholder
            //
            this.pdfPlaceholder.AutoSize = false;
            this.pdfPlaceholder.BackColor = OrderDesk.Shared.Palette.PanelBackground;
            this.pdfPlaceholder.Dock = Wisej.Web.DockStyle.Fill;
            this.pdfPlaceholder.ForeColor = OrderDesk.Shared.Palette.MutedText;
            this.pdfPlaceholder.Name = "pdfPlaceholder";
            this.pdfPlaceholder.Text = "Click  Invoice PDF ✓  to render the selected order on the server and show it here.\nPrint invoice ✕ shows what the desktop PrintDocument does on a server.";
            this.pdfPlaceholder.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // importCard
            //
            this.importCard.BackColor = OrderDesk.Shared.Palette.CardBackground;
            this.importCard.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.importCard.Controls.Add(this.importTitle);
            this.importCard.Controls.Add(this.upload);
            this.importCard.Controls.Add(this.storageLabel);
            this.importCard.Controls.Add(this.importSummary);
            this.importCard.Controls.Add(this.importResults);
            this.importCard.Location = new System.Drawing.Point(524, 88);
            this.importCard.Name = "importCard";
            this.importCard.Size = new System.Drawing.Size(300, 262);
            //
            // importTitle
            //
            this.importTitle.AutoSize = false;
            this.importTitle.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.importTitle.ForeColor = OrderDesk.Shared.Palette.Ink;
            this.importTitle.Location = new System.Drawing.Point(0, 0);
            this.importTitle.Name = "importTitle";
            this.importTitle.Padding = new Wisej.Web.Padding(14, 0, 0, 0);
            this.importTitle.Size = new System.Drawing.Size(298, 30);
            this.importTitle.Text = "Import  ·  Upload → server → OrderService.Save";
            this.importTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // upload
            //
            this.upload.AllowedFileTypes = ".csv";
            this.upload.AllowMultipleFiles = false;
            this.upload.Location = new System.Drawing.Point(12, 36);
            this.upload.MaxFileSize = 1048576;
            this.upload.Name = "upload";
            this.upload.Size = new System.Drawing.Size(274, 30);
            this.upload.Text = "Upload CSV…";
            this.upload.Uploading += this.upload_Uploading;
            this.upload.Progress += this.upload_Progress;
            this.upload.Uploaded += this.upload_Uploaded;
            this.upload.Error += this.upload_Error;
            //
            // storageLabel
            //
            this.storageLabel.AutoEllipsis = true;
            this.storageLabel.AutoSize = false;
            this.storageLabel.Font = new System.Drawing.Font("monospace", 8F);
            this.storageLabel.ForeColor = OrderDesk.Shared.Palette.MutedText;
            this.storageLabel.Location = new System.Drawing.Point(12, 70);
            this.storageLabel.Name = "storageLabel";
            this.storageLabel.Size = new System.Drawing.Size(274, 18);
            this.storageLabel.Text = "staged under App_Data/imports (Web.config)";
            this.storageLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // importSummary
            //
            this.importSummary.AutoEllipsis = true;
            this.importSummary.AutoSize = false;
            this.importSummary.Font = new System.Drawing.Font("default", 9.5F, System.Drawing.FontStyle.Bold);
            this.importSummary.ForeColor = OrderDesk.Shared.Palette.MutedText;
            this.importSummary.Location = new System.Drawing.Point(12, 92);
            this.importSummary.Name = "importSummary";
            this.importSummary.Size = new System.Drawing.Size(274, 22);
            this.importSummary.Text = "Upload order-batch.csv to begin";
            this.importSummary.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // importResults
            //
            this.importResults.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.importResults.Font = new System.Drawing.Font("monospace", 8F);
            this.importResults.Location = new System.Drawing.Point(12, 118);
            this.importResults.Name = "importResults";
            this.importResults.Size = new System.Drawing.Size(274, 132);
            //
            // queueCard
            //
            this.queueCard.BackColor = OrderDesk.Shared.Palette.CardBackground;
            this.queueCard.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.queueCard.Controls.Add(this.queueTitle);
            this.queueCard.Controls.Add(this.jobsGrid);
            this.queueCard.Controls.Add(this.queueProgress);
            this.queueCard.Controls.Add(this.queueLabel);
            this.queueCard.Location = new System.Drawing.Point(524, 358);
            this.queueCard.Name = "queueCard";
            this.queueCard.Size = new System.Drawing.Size(300, 274);
            //
            // queueTitle
            //
            this.queueTitle.AutoSize = false;
            this.queueTitle.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.queueTitle.ForeColor = OrderDesk.Shared.Palette.Ink;
            this.queueTitle.Location = new System.Drawing.Point(0, 0);
            this.queueTitle.Name = "queueTitle";
            this.queueTitle.Padding = new Wisej.Web.Padding(14, 0, 0, 0);
            this.queueTitle.Size = new System.Drawing.Size(298, 30);
            this.queueTitle.Text = "Report queue  ·  one worker, all sessions";
            this.queueTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // jobsGrid
            //
            this.jobsGrid.AllowUserToAddRows = false;
            this.jobsGrid.AllowUserToDeleteRows = false;
            this.jobsGrid.AutoGenerateColumns = false;
            this.jobsGrid.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] { this.colJob, this.colFile, this.colJobStatus, this.colSession });
            this.jobsGrid.Location = new System.Drawing.Point(12, 36);
            this.jobsGrid.MultiSelect = false;
            this.jobsGrid.Name = "jobsGrid";
            this.jobsGrid.ReadOnly = true;
            this.jobsGrid.RowHeadersVisible = false;
            this.jobsGrid.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.jobsGrid.Size = new System.Drawing.Size(274, 172);
            this.colJob.DataPropertyName = "Job";
            this.colJob.HeaderText = "Job";
            this.colJob.Name = "colJob";
            this.colJob.Width = 42;
            this.colFile.AutoSizeMode = Wisej.Web.DataGridViewAutoSizeColumnMode.Fill;
            this.colFile.DataPropertyName = "File";
            this.colFile.HeaderText = "File";
            this.colFile.Name = "colFile";
            this.colJobStatus.DataPropertyName = "Status";
            this.colJobStatus.HeaderText = "Status";
            this.colJobStatus.Name = "colJobStatus";
            this.colJobStatus.Width = 78;
            this.colSession.DataPropertyName = "Session";
            this.colSession.HeaderText = "Session";
            this.colSession.Name = "colSession";
            this.colSession.Width = 66;
            //
            // queueProgress
            //
            this.queueProgress.Location = new System.Drawing.Point(12, 216);
            this.queueProgress.Maximum = 1;
            this.queueProgress.Minimum = 0;
            this.queueProgress.Name = "queueProgress";
            this.queueProgress.Size = new System.Drawing.Size(274, 14);
            this.queueProgress.Value = 0;
            //
            // queueLabel
            //
            this.queueLabel.AutoEllipsis = true;
            this.queueLabel.AutoSize = false;
            this.queueLabel.Font = new System.Drawing.Font("default", 8.5F);
            this.queueLabel.ForeColor = OrderDesk.Shared.Palette.MutedText;
            this.queueLabel.Location = new System.Drawing.Point(12, 234);
            this.queueLabel.Name = "queueLabel";
            this.queueLabel.Size = new System.Drawing.Size(274, 34);
            this.queueLabel.Text = "0 queued · 0 running · 0 done · worker idle";
            this.queueLabel.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // button bar — row 1 (y 642)
            //
            this.downloadSampleButton.Location = new System.Drawing.Point(16, 642);
            this.downloadSampleButton.Name = "downloadSampleButton";
            this.downloadSampleButton.Size = new System.Drawing.Size(196, 32);
            this.downloadSampleButton.Text = "Download sample CSV";
            this.downloadSampleButton.ToolTipText = "Application.Download(App_Data/sample-import.csv) as order-batch.csv — then upload it with the Upload button";
            this.downloadSampleButton.Click += this.downloadSampleButton_Click;
            this.importLegacyButton.Location = new System.Drawing.Point(220, 642);
            this.importLegacyButton.Name = "importLegacyButton";
            this.importLegacyButton.Size = new System.Drawing.Size(196, 32);
            this.importLegacyButton.Text = "Import (C:\\Orders) ✕";
            this.importLegacyButton.ToolTipText = "Legacy.LocalExport.ReadCsv — File.ReadAllText(@\"C:\\Orders\\in.csv\") on the SERVER";
            this.importLegacyButton.Click += this.importLegacyButton_Click;
            this.exportInteropButton.Location = new System.Drawing.Point(424, 642);
            this.exportInteropButton.Name = "exportInteropButton";
            this.exportInteropButton.Size = new System.Drawing.Size(196, 32);
            this.exportInteropButton.Text = "Export (Excel Interop) ✕";
            this.exportInteropButton.ToolTipText = "Legacy.ExcelExport — new Excel.Application() on the server";
            this.exportInteropButton.Click += this.exportInteropButton_Click;
            this.exportXlsxButton.Location = new System.Drawing.Point(628, 642);
            this.exportXlsxButton.Name = "exportXlsxButton";
            this.exportXlsxButton.Size = new System.Drawing.Size(196, 32);
            this.exportXlsxButton.Text = "Export (managed .xlsx) ✓";
            this.exportXlsxButton.ToolTipText = "Services.XlsxWriter (ZipArchive OpenXML) → Application.Download(\"orders.xlsx\")";
            this.exportXlsxButton.Click += this.exportXlsxButton_Click;
            //
            // button bar — row 2 (y 682)
            //
            this.printInvoiceButton.Location = new System.Drawing.Point(16, 682);
            this.printInvoiceButton.Name = "printInvoiceButton";
            this.printInvoiceButton.Size = new System.Drawing.Size(196, 32);
            this.printInvoiceButton.Text = "Print invoice ✕";
            this.printInvoiceButton.ToolTipText = "Legacy.InvoicePrinter.Print — PrintDocument to the server's default printer";
            this.printInvoiceButton.Click += this.printInvoiceButton_Click;
            this.invoicePdfButton.Location = new System.Drawing.Point(220, 682);
            this.invoicePdfButton.Name = "invoicePdfButton";
            this.invoicePdfButton.Size = new System.Drawing.Size(196, 32);
            this.invoicePdfButton.Text = "Invoice PDF ✓";
            this.invoicePdfButton.ToolTipText = "Services.PdfWriter.Invoice(selected order) → PdfViewer.PdfStream + App_Data/reports";
            this.invoicePdfButton.Click += this.invoicePdfButton_Click;
            this.queueReportButton.Location = new System.Drawing.Point(424, 682);
            this.queueReportButton.Name = "queueReportButton";
            this.queueReportButton.Size = new System.Drawing.Size(196, 32);
            this.queueReportButton.Text = "Queue report (all invoices)";
            this.queueReportButton.ToolTipText = "Services.ReportQueue: 5 jobs, one worker (Application.StartTask), progress via Application.Update";
            this.queueReportButton.Click += this.queueReportButton_Click;
            this.storageRootButton.Location = new System.Drawing.Point(628, 682);
            this.storageRootButton.Name = "storageRootButton";
            this.storageRootButton.Size = new System.Drawing.Size(196, 32);
            this.storageRootButton.Text = "Show storage root";
            this.storageRootButton.ToolTipText = "Web.config OrderDesk.StorageRoot → Path.Combine(app folder, App_Data) and the files under it";
            this.storageRootButton.Click += this.storageRootButton_Click;
            //
            // MainPage
            //
            this.BackColor = OrderDesk.Shared.Palette.PageBackground;
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.bannerLabel);
            this.Controls.Add(this.ordersCard);
            this.Controls.Add(this.pdfCard);
            this.Controls.Add(this.importCard);
            this.Controls.Add(this.queueCard);
            this.Controls.Add(this.downloadSampleButton);
            this.Controls.Add(this.importLegacyButton);
            this.Controls.Add(this.exportInteropButton);
            this.Controls.Add(this.exportXlsxButton);
            this.Controls.Add(this.printInvoiceButton);
            this.Controls.Add(this.invoicePdfButton);
            this.Controls.Add(this.queueReportButton);
            this.Controls.Add(this.storageRootButton);
            this.Controls.Add(this.trace);
            this.Controls.Add(this.appBar);
            this.Name = "MainPage";
            this.Size = new System.Drawing.Size(1400, 760);
            this.Text = "OrderDesk — Documents";
            this.Load += this.MainPage_Load;
            this.appBar.ResumeLayout(false);
            this.ordersCard.ResumeLayout(false);
            this.pdfCard.ResumeLayout(false);
            this.importCard.ResumeLayout(false);
            this.queueCard.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel appBar;
        private Wisej.Web.Label appTitle;
        private OrderDesk.Shared.TracePanel trace;
        private Wisej.Web.Label statusLabel;
        private Wisej.Web.Label bannerLabel;
        private Wisej.Web.Panel ordersCard;
        private Wisej.Web.Label ordersTitle;
        private Wisej.Web.DataGridView ordersGrid;
        private Wisej.Web.DataGridViewTextBoxColumn colOrder;
        private Wisej.Web.DataGridViewTextBoxColumn colCustomer;
        private Wisej.Web.DataGridViewTextBoxColumn colOwner;
        private Wisej.Web.DataGridViewTextBoxColumn colTotal;
        private Wisej.Web.DataGridViewTextBoxColumn colStatus;
        private Wisej.Web.Panel pdfCard;
        private Wisej.Web.Label pdfTitle;
        private Wisej.Web.Button pdfDownloadButton;
        private Wisej.Web.PdfViewer pdfViewer;
        private Wisej.Web.Label pdfPlaceholder;
        private Wisej.Web.Panel importCard;
        private Wisej.Web.Label importTitle;
        private Wisej.Web.Upload upload;
        private Wisej.Web.Label storageLabel;
        private Wisej.Web.Label importSummary;
        private Wisej.Web.ListBox importResults;
        private Wisej.Web.Panel queueCard;
        private Wisej.Web.Label queueTitle;
        private Wisej.Web.DataGridView jobsGrid;
        private Wisej.Web.DataGridViewTextBoxColumn colJob;
        private Wisej.Web.DataGridViewTextBoxColumn colFile;
        private Wisej.Web.DataGridViewTextBoxColumn colJobStatus;
        private Wisej.Web.DataGridViewTextBoxColumn colSession;
        private Wisej.Web.ProgressBar queueProgress;
        private Wisej.Web.Label queueLabel;
        private Wisej.Web.Button downloadSampleButton;
        private Wisej.Web.Button importLegacyButton;
        private Wisej.Web.Button exportInteropButton;
        private Wisej.Web.Button exportXlsxButton;
        private Wisej.Web.Button printInvoiceButton;
        private Wisej.Web.Button invoicePdfButton;
        private Wisej.Web.Button queueReportButton;
        private Wisej.Web.Button storageRootButton;
    }
}
