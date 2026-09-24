namespace VisualOperationsStudio
{
    partial class VisualOperationsPage
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
            this.pnlAppBar = new Wisej.Web.Panel();
            this.lblAppTitle = new Wisej.Web.Label();
            this.lblAppSubtitle = new Wisej.Web.Label();
            this.glyphs = new VisualOperationsStudio.Controls.WindowGlyphs();
            this.lblFooter = new Wisej.Web.Label();
            this.gridOperations = new Wisej.Web.DataGridView();
            this.colMachine = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colSite = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colHealth = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colTrend = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colStatusHtml = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colLastReading = new Wisej.Web.DataGridViewTextBoxColumn();
            this.SuspendLayout();
            //
            // lblAppTitle
            //
            this.lblAppTitle.AutoSize = false;
            this.lblAppTitle.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.lblAppTitle.ForeColor = System.Drawing.Color.White;
            this.lblAppTitle.Location = new System.Drawing.Point(16, 0);
            this.lblAppTitle.Name = "lblAppTitle";
            this.lblAppTitle.Size = new System.Drawing.Size(172, 38);
            this.lblAppTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblAppTitle.Text = "VisualOperationsStudio";
            //
            // lblAppSubtitle
            //
            this.lblAppSubtitle.AutoSize = false;
            this.lblAppSubtitle.CssStyle = "opacity:.85";
            this.lblAppSubtitle.Font = new System.Drawing.Font("default", 12.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblAppSubtitle.ForeColor = System.Drawing.Color.White;
            this.lblAppSubtitle.Location = new System.Drawing.Point(191, 0);
            this.lblAppSubtitle.Name = "lblAppSubtitle";
            this.lblAppSubtitle.Size = new System.Drawing.Size(180, 38);
            this.lblAppSubtitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblAppSubtitle.Text = "· Operations";
            //
            // glyphs
            //
            this.glyphs.BackColor = System.Drawing.Color.FromArgb(21, 101, 216);
            this.glyphs.Dock = Wisej.Web.DockStyle.Right;
            this.glyphs.Inset = 16;
            this.glyphs.Name = "glyphs";
            this.glyphs.Size = new System.Drawing.Size(85, 38);
            //
            // pnlAppBar
            //
            this.pnlAppBar.BackColor = System.Drawing.Color.FromArgb(21, 101, 216);
            this.pnlAppBar.Dock = Wisej.Web.DockStyle.Top;
            this.pnlAppBar.Name = "pnlAppBar";
            this.pnlAppBar.Size = new System.Drawing.Size(1400, 38);
            this.pnlAppBar.Controls.Add(this.lblAppTitle);
            this.pnlAppBar.Controls.Add(this.lblAppSubtitle);
            this.pnlAppBar.Controls.Add(this.glyphs);
            //
            // lblFooter
            //
            this.lblFooter.AutoSize = false;
            this.lblFooter.BackColor = System.Drawing.Color.FromArgb(244, 247, 250);
            this.lblFooter.CssStyle = "border-top:1px solid #e0e7ef";
            this.lblFooter.Dock = Wisej.Web.DockStyle.Bottom;
            this.lblFooter.Font = new System.Drawing.Font("Consolas", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lblFooter.ForeColor = System.Drawing.Color.FromArgb(125, 140, 156);
            this.lblFooter.Name = "lblFooter";
            this.lblFooter.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.lblFooter.Size = new System.Drawing.Size(1400, 26);
            this.lblFooter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblFooter.Text = "1,000 machines bound · painted cells: Health, Trend";
            //
            // colMachine
            //
            this.colMachine.DataPropertyName = "Machine";
            this.colMachine.DefaultCellStyle.Font = new System.Drawing.Font("Consolas", 12.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.colMachine.DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(21, 101, 216);
            this.colMachine.HeaderText = "MACHINE";
            this.colMachine.Name = "colMachine";
            this.colMachine.Width = 118;
            //
            // colSite
            //
            this.colSite.DataPropertyName = "Site";
            this.colSite.DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(70, 88, 106);
            this.colSite.HeaderText = "SITE";
            this.colSite.Name = "colSite";
            this.colSite.Width = 74;
            //
            // colHealth
            //
            this.colHealth.HeaderText = "HEALTH";
            this.colHealth.Name = "colHealth";
            this.colHealth.Width = 168;
            //
            // colTrend
            //
            this.colTrend.HeaderText = "TREND";
            this.colTrend.Name = "colTrend";
            this.colTrend.Width = 118;
            //
            // colStatusHtml
            //
            this.colStatusHtml.AllowHtml = true;
            this.colStatusHtml.DataPropertyName = "SeverityHtml";
            this.colStatusHtml.HeaderText = "STATUS · HTML";
            this.colStatusHtml.Name = "colStatusHtml";
            this.colStatusHtml.Width = 132;
            //
            // colLastReading
            //
            this.colLastReading.AutoSizeMode = Wisej.Web.DataGridViewAutoSizeColumnMode.Fill;
            this.colLastReading.DataPropertyName = "LastReadingText";
            this.colLastReading.DefaultCellStyle.Font = new System.Drawing.Font("Consolas", 11.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.colLastReading.DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(136, 149, 164);
            this.colLastReading.HeaderText = "LAST READING";
            this.colLastReading.Name = "colLastReading";
            //
            // gridOperations
            //
            this.gridOperations.AllowUserToAddRows = false;
            this.gridOperations.AllowUserToDeleteRows = false;
            this.gridOperations.AllowUserToOrderColumns = false;
            this.gridOperations.AutoGenerateColumns = false;
            this.gridOperations.BorderStyle = Wisej.Web.BorderStyle.None;
            this.gridOperations.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(244, 247, 250);
            this.gridOperations.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("default", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.gridOperations.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.gridOperations.ColumnHeadersHeight = 30;
            this.gridOperations.DefaultCellStyle.BackColor = System.Drawing.Color.White;
            this.gridOperations.DefaultCellStyle.Font = new System.Drawing.Font("default", 12.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.gridOperations.Dock = Wisej.Web.DockStyle.Fill;
            this.gridOperations.Name = "gridOperations";
            this.gridOperations.ReadOnly = true;
            this.gridOperations.RowHeadersVisible = false;
            this.gridOperations.RowTemplate.Height = 34;
            this.gridOperations.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.gridOperations.Columns.Add(this.colMachine);
            this.gridOperations.Columns.Add(this.colSite);
            this.gridOperations.Columns.Add(this.colHealth);
            this.gridOperations.Columns.Add(this.colTrend);
            this.gridOperations.Columns.Add(this.colStatusHtml);
            this.gridOperations.Columns.Add(this.colLastReading);
            //
            // VisualOperationsPage
            //
            this.BackColor = System.Drawing.Color.White;
            this.Name = "VisualOperationsPage";
            this.Load += this.VisualOperationsPage_Load;
            this.Size = new System.Drawing.Size(1400, 900);
            this.Text = "VisualOperationsStudio · Operations";
            this.Controls.Add(this.gridOperations);
            this.Controls.Add(this.lblFooter);
            this.Controls.Add(this.pnlAppBar);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlAppBar;
        private Wisej.Web.Label lblAppTitle;
        private Wisej.Web.Label lblAppSubtitle;
        private VisualOperationsStudio.Controls.WindowGlyphs glyphs;
        private Wisej.Web.Label lblFooter;
        private Wisej.Web.DataGridView gridOperations;
        private Wisej.Web.DataGridViewTextBoxColumn colMachine;
        private Wisej.Web.DataGridViewTextBoxColumn colSite;
        private Wisej.Web.DataGridViewTextBoxColumn colHealth;
        private Wisej.Web.DataGridViewTextBoxColumn colTrend;
        private Wisej.Web.DataGridViewTextBoxColumn colStatusHtml;
        private Wisej.Web.DataGridViewTextBoxColumn colLastReading;
    }
}
