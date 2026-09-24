namespace IconDesk
{
    partial class SummaryPage
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
            this.appTitleBar = new IconDesk.AppTitleBar();
            this.pnlNav = new Wisej.Web.Panel();
            this.layoutShell = new Wisej.Web.TableLayoutPanel();
            this.pnlContent = new Wisej.Web.Panel();
            this.pnlSummary = new Wisej.Web.Panel();
            this.pnlSummaryHead = new Wisej.Web.Panel();
            this.lblSummaryHeading = new Wisej.Web.Label();
            this.btnThemeChip = new Wisej.Web.Button();
            this.layoutTiles = new Wisej.Web.TableLayoutPanel();
            this.layoutVerdicts = new Wisej.Web.TableLayoutPanel();
            this.lblSummaryFooter = new Wisej.Web.Label();
            this.pnlActions = new Wisej.Web.Panel();
            this.pnlActionsHead = new Wisej.Web.Panel();
            this.lblActionsHeading = new Wisej.Web.Label();
            this.lblQaChip = new Wisej.Web.Label();
            this.gridCustomers = new Wisej.Web.DataGridView();
            this.colOrder = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colCustomer = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colActions = new Wisej.Web.DataGridViewTextBoxColumn();
            this.layoutLegend = new Wisej.Web.TableLayoutPanel();
            this.lblClickReport = new Wisej.Web.Label();
            this.SuspendLayout();
            //
            // appTitleBar
            //
            this.appTitleBar.Title = "IconDesk";
            //
            // pnlNav - the application map, exactly the seven sections the course built
            //
            this.pnlNav.BackColor = System.Drawing.Color.FromArgb(244, 247, 250);
            this.pnlNav.CssStyle = "border-right:1px solid #e4eaf1;";
            this.pnlNav.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlNav.Name = "pnlNav";
            this.pnlNav.Padding = new Wisej.Web.Padding(10, 13, 10, 13);
            this.pnlNav.Size = new System.Drawing.Size(168, 600);
            //
            // ── the Icon Summary view ──────────────────────────────────────────
            //
            this.lblSummaryHeading.AutoSize = false;
            this.lblSummaryHeading.Dock = Wisej.Web.DockStyle.Fill;
            this.lblSummaryHeading.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblSummaryHeading.ForeColor = System.Drawing.Color.FromArgb(13, 27, 42);
            this.lblSummaryHeading.Name = "lblSummaryHeading";
            this.lblSummaryHeading.Text = "Every image on this page, and where it comes from";
            this.lblSummaryHeading.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.btnThemeChip.BackColor = System.Drawing.Color.FromArgb(234, 243, 255);
            this.btnThemeChip.CssStyle = "border:1px solid #c5ddff;border-radius:999px;";
            this.btnThemeChip.Dock = Wisej.Web.DockStyle.Right;
            this.btnThemeChip.Font = new System.Drawing.Font("default", 9.4F, System.Drawing.FontStyle.Bold);
            this.btnThemeChip.ForeColor = System.Drawing.Color.FromArgb(21, 101, 216);
            this.btnThemeChip.Margin = new Wisej.Web.Padding(0, 3, 0, 3);
            this.btnThemeChip.Name = "btnThemeChip";
            this.btnThemeChip.TabIndex = 0;
            this.btnThemeChip.Click += this.btnThemeChip_Click;
            this.btnThemeChip.Size = new System.Drawing.Size(200, 32);
            this.btnThemeChip.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.pnlSummaryHead.Dock = Wisej.Web.DockStyle.Top;
            this.pnlSummaryHead.Name = "pnlSummaryHead";
            this.pnlSummaryHead.Size = new System.Drawing.Size(800, 40);
            this.pnlSummaryHead.Controls.Add(this.lblSummaryHeading);
            this.pnlSummaryHead.Controls.Add(this.btnThemeChip);

            this.layoutTiles.ColumnCount = 5;
            this.layoutTiles.RowCount = 1;
            for (var column = 0; column < 5; column++)
                this.layoutTiles.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 20F));
            this.layoutTiles.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Absolute, 132F));
            this.layoutTiles.Dock = Wisej.Web.DockStyle.Top;
            this.layoutTiles.Name = "layoutTiles";
            this.layoutTiles.Padding = new Wisej.Web.Padding(0, 6, 0, 0);
            this.layoutTiles.Size = new System.Drawing.Size(800, 138);

            this.layoutVerdicts.ColumnCount = 5;
            this.layoutVerdicts.RowCount = 1;
            for (var column = 0; column < 5; column++)
                this.layoutVerdicts.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 20F));
            this.layoutVerdicts.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Absolute, 32F));
            this.layoutVerdicts.Dock = Wisej.Web.DockStyle.Top;
            this.layoutVerdicts.Name = "layoutVerdicts";
            this.layoutVerdicts.Padding = new Wisej.Web.Padding(0, 13, 0, 0);
            this.layoutVerdicts.Size = new System.Drawing.Size(800, 45);

            this.lblSummaryFooter.AllowHtml = true;
            this.lblSummaryFooter.AutoSize = false;
            this.lblSummaryFooter.BackColor = System.Drawing.Color.FromArgb(244, 247, 250);
            this.lblSummaryFooter.CssStyle = "border:1px solid #e4eaf1;border-radius:9px;";
            this.lblSummaryFooter.Dock = Wisej.Web.DockStyle.Bottom;
            this.lblSummaryFooter.Font = new System.Drawing.Font("default", 9.8F);
            this.lblSummaryFooter.ForeColor = System.Drawing.Color.FromArgb(70, 88, 106);
            this.lblSummaryFooter.Name = "lblSummaryFooter";
            this.lblSummaryFooter.Padding = new Wisej.Web.Padding(15, 0, 15, 0);
            this.lblSummaryFooter.Size = new System.Drawing.Size(800, 42);
            this.lblSummaryFooter.Text =
                "<span style='color:#1f9d6b'>&#9679;</span> &nbsp;Every tile carries a visible name and a tooltip — nothing on this page is an unlabelled icon.";
            this.lblSummaryFooter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.pnlSummary.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlSummary.Name = "pnlSummary";
            this.pnlSummary.Padding = new Wisej.Web.Padding(20, 18, 20, 18);
            this.pnlSummary.Controls.Add(this.lblSummaryFooter);
            this.pnlSummary.Controls.Add(this.layoutVerdicts);
            this.pnlSummary.Controls.Add(this.layoutTiles);
            this.pnlSummary.Controls.Add(this.pnlSummaryHead);
            //
            // ── the Customer Actions view ──────────────────────────────────────
            //
            this.lblActionsHeading.AutoSize = false;
            this.lblActionsHeading.Dock = Wisej.Web.DockStyle.Fill;
            this.lblActionsHeading.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold);
            this.lblActionsHeading.ForeColor = System.Drawing.Color.FromArgb(13, 27, 42);
            this.lblActionsHeading.Name = "lblActionsHeading";
            this.lblActionsHeading.Text = "gridCustomers.AllowHtml = true";
            this.lblActionsHeading.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.lblQaChip.AutoSize = false;
            this.lblQaChip.BackColor = System.Drawing.Color.FromArgb(234, 243, 255);
            this.lblQaChip.CssStyle = "border:1px solid #c5ddff;border-radius:999px;";
            this.lblQaChip.Dock = Wisej.Web.DockStyle.Right;
            this.lblQaChip.Font = new System.Drawing.Font("default", 9.4F, System.Drawing.FontStyle.Bold);
            this.lblQaChip.ForeColor = System.Drawing.Color.FromArgb(21, 101, 216);
            this.lblQaChip.Name = "lblQaChip";
            this.lblQaChip.Size = new System.Drawing.Size(200, 26);
            this.lblQaChip.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.pnlActionsHead.Dock = Wisej.Web.DockStyle.Top;
            this.pnlActionsHead.Name = "pnlActionsHead";
            this.pnlActionsHead.Size = new System.Drawing.Size(800, 40);
            this.pnlActionsHead.Controls.Add(this.lblActionsHeading);
            this.pnlActionsHead.Controls.Add(this.lblQaChip);
            //
            // gridCustomers - four orders, and one cell with two things a click can land on
            //
            this.colOrder.DataPropertyName = "Order";
            this.colOrder.HeaderText = "Order";
            this.colOrder.Name = "colOrder";
            this.colOrder.Width = 110;

            this.colCustomer.DataPropertyName = "Customer";
            this.colCustomer.HeaderText = "Customer";
            this.colCustomer.Name = "colCustomer";
            this.colCustomer.AutoSizeMode = Wisej.Web.DataGridViewAutoSizeColumnMode.Fill;

            this.colStatus.AllowHtml = true;
            this.colStatus.DataPropertyName = "Status";
            this.colStatus.HeaderText = "Status";
            this.colStatus.Name = "colStatus";
            this.colStatus.Width = 140;

            // AllowHtml goes on the COLUMN, not on the grid: that is what lets a cell hold the
            // two glyph spans the click handler asks about.
            this.colActions.AllowHtml = true;
            this.colActions.DataPropertyName = "Actions";
            this.colActions.HeaderText = "Actions";
            this.colActions.Name = "colActions";
            this.colActions.Width = 260;

            this.gridCustomers.AllowUserToAddRows = false;
            this.gridCustomers.AllowUserToDeleteRows = false;
            this.gridCustomers.AutoGenerateColumns = false;
            this.gridCustomers.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.gridCustomers.ColumnHeadersHeight = 34;
            this.gridCustomers.CssStyle = "border-radius:10px;";
            this.gridCustomers.Dock = Wisej.Web.DockStyle.Top;
            this.gridCustomers.Name = "gridCustomers";
            this.gridCustomers.ReadOnly = true;
            this.gridCustomers.RowHeadersVisible = false;
            this.gridCustomers.RowTemplate.Height = 54;
            this.gridCustomers.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.gridCustomers.Size = new System.Drawing.Size(800, 262);
            this.gridCustomers.Columns.Add(this.colOrder);
            this.gridCustomers.Columns.Add(this.colCustomer);
            this.gridCustomers.Columns.Add(this.colStatus);
            this.gridCustomers.Columns.Add(this.colActions);
            this.gridCustomers.CellClick += this.gridCustomers_CellClick;

            this.layoutLegend.ColumnCount = 3;
            this.layoutLegend.RowCount = 1;
            for (var column = 0; column < 3; column++)
                this.layoutLegend.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 33.34F));
            this.layoutLegend.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Absolute, 62F));
            this.layoutLegend.Dock = Wisej.Web.DockStyle.Top;
            this.layoutLegend.Name = "layoutLegend";
            this.layoutLegend.Padding = new Wisej.Web.Padding(0, 13, 0, 0);
            this.layoutLegend.Size = new System.Drawing.Size(800, 75);

            this.lblClickReport.AllowHtml = true;
            this.lblClickReport.AutoSize = false;
            this.lblClickReport.Dock = Wisej.Web.DockStyle.Bottom;
            this.lblClickReport.Font = new System.Drawing.Font("default", 10.1F, System.Drawing.FontStyle.Bold);
            this.lblClickReport.Name = "lblClickReport";
            this.lblClickReport.Padding = new Wisej.Web.Padding(16, 0, 16, 0);
            this.lblClickReport.Size = new System.Drawing.Size(800, 44);
            this.lblClickReport.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.pnlActions.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlActions.Name = "pnlActions";
            this.pnlActions.Padding = new Wisej.Web.Padding(20, 18, 20, 18);
            this.pnlActions.Visible = false;
            this.pnlActions.Controls.Add(this.lblClickReport);
            this.pnlActions.Controls.Add(this.layoutLegend);
            this.pnlActions.Controls.Add(this.gridCustomers);
            this.pnlActions.Controls.Add(this.pnlActionsHead);
            //
            // pnlContent
            //
            this.pnlContent.BackColor = System.Drawing.Color.White;
            this.pnlContent.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Controls.Add(this.pnlSummary);
            this.pnlContent.Controls.Add(this.pnlActions);
            //
            // SummaryPage
            //
            this.BackColor = System.Drawing.Color.White;
            this.Name = "SummaryPage";
            this.Size = new System.Drawing.Size(1000, 640);
            this.Text = "IconDesk — Icon Summary";
            // A two-column shell. Dock alone is not enough here: the nav and the content have to
            // share the row, and a table says so without either of them guessing a width.
            this.layoutShell.ColumnCount = 2;
            this.layoutShell.RowCount = 1;
            this.layoutShell.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Absolute, 168F));
            this.layoutShell.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 100F));
            this.layoutShell.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Percent, 100F));
            this.layoutShell.Dock = Wisej.Web.DockStyle.Fill;
            this.layoutShell.Name = "layoutShell";
            this.layoutShell.Controls.Add(this.pnlNav, 0, 0);
            this.layoutShell.Controls.Add(this.pnlContent, 1, 0);

            this.Controls.Add(this.layoutShell);
            this.Controls.Add(this.appTitleBar);
            this.ResumeLayout(false);
        }

        #endregion

        private IconDesk.AppTitleBar appTitleBar;
        private Wisej.Web.Panel pnlNav;
        private Wisej.Web.TableLayoutPanel layoutShell;
        private Wisej.Web.Panel pnlContent;
        private Wisej.Web.Panel pnlSummary;
        private Wisej.Web.Panel pnlSummaryHead;
        private Wisej.Web.Label lblSummaryHeading;
        private Wisej.Web.Button btnThemeChip;
        private Wisej.Web.TableLayoutPanel layoutTiles;
        private Wisej.Web.TableLayoutPanel layoutVerdicts;
        private Wisej.Web.Label lblSummaryFooter;
        private Wisej.Web.Panel pnlActions;
        private Wisej.Web.Panel pnlActionsHead;
        private Wisej.Web.Label lblActionsHeading;
        private Wisej.Web.Label lblQaChip;
        private Wisej.Web.DataGridView gridCustomers;
        private Wisej.Web.DataGridViewTextBoxColumn colOrder;
        private Wisej.Web.DataGridViewTextBoxColumn colCustomer;
        private Wisej.Web.DataGridViewTextBoxColumn colStatus;
        private Wisej.Web.DataGridViewTextBoxColumn colActions;
        private Wisej.Web.TableLayoutPanel layoutLegend;
        private Wisej.Web.Label lblClickReport;
    }
}
