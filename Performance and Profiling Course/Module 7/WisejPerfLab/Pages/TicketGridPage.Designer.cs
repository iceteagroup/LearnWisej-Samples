namespace WisejPerfLab.Pages
{
    partial class TicketGridPage
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        #region Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lblStatusCaption = new Wisej.Web.Label();
            this.cboStatus = new Wisej.Web.ComboBox();
            this.lblRowsCaption = new Wisej.Web.Label();
            this.cboPageSize = new Wisej.Web.ComboBox();
            this.btnSearch = new Wisej.Web.Button();
            this.btnRedraw = new Wisej.Web.Button();
            this.btnExport = new Wisej.Web.Button();
            this.btnCancelExport = new Wisej.Web.Button();
            this.progressExport = new Wisej.Web.ProgressBar();
            this.dataGridView1 = new Wisej.Web.DataGridView();
            this.bindingSource1 = new Wisej.Web.BindingSource(this.components);
            this.lblGridStatus = new Wisej.Web.Label();
            this.lblScenario = new Wisej.Web.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.SuspendLayout();
            //
            // lblStatusCaption
            //
            this.lblStatusCaption.AutoSize = true;
            this.lblStatusCaption.Location = new System.Drawing.Point(14, 18);
            this.lblStatusCaption.Name = "lblStatusCaption";
            this.lblStatusCaption.Size = new System.Drawing.Size(50, 16);
            this.lblStatusCaption.TabIndex = 0;
            this.lblStatusCaption.Text = "Status";
            //
            // cboStatus
            //
            this.cboStatus.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboStatus.Location = new System.Drawing.Point(66, 12);
            this.cboStatus.Name = "cboStatus";
            this.cboStatus.Size = new System.Drawing.Size(140, 30);
            this.cboStatus.TabIndex = 1;
            //
            // lblRowsCaption
            //
            this.lblRowsCaption.AutoSize = true;
            this.lblRowsCaption.Location = new System.Drawing.Point(218, 18);
            this.lblRowsCaption.Name = "lblRowsCaption";
            this.lblRowsCaption.Size = new System.Drawing.Size(40, 16);
            this.lblRowsCaption.TabIndex = 2;
            this.lblRowsCaption.Text = "Rows";
            //
            // cboPageSize
            //
            this.cboPageSize.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboPageSize.Location = new System.Drawing.Point(262, 12);
            this.cboPageSize.Name = "cboPageSize";
            this.cboPageSize.Size = new System.Drawing.Size(100, 30);
            this.cboPageSize.TabIndex = 3;
            //
            // btnSearch
            //
            this.btnSearch.Location = new System.Drawing.Point(374, 10);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(120, 34);
            this.btnSearch.TabIndex = 4;
            this.btnSearch.Text = "Search tickets";
            this.btnSearch.Click += this.btnSearch_Click;
            //
            // btnRedraw
            //
            this.btnRedraw.Location = new System.Drawing.Point(502, 10);
            this.btnRedraw.Name = "btnRedraw";
            this.btnRedraw.Size = new System.Drawing.Size(110, 34);
            this.btnRedraw.TabIndex = 5;
            this.btnRedraw.Text = "Redraw";
            this.btnRedraw.Click += this.btnRedraw_Click;
            //
            // btnExport
            //
            this.btnExport.Location = new System.Drawing.Point(620, 10);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(110, 34);
            this.btnExport.TabIndex = 6;
            this.btnExport.Text = "Export CSV";
            this.btnExport.Click += this.btnExport_Click;
            //
            // btnCancelExport
            //
            this.btnCancelExport.Enabled = false;
            this.btnCancelExport.Location = new System.Drawing.Point(738, 10);
            this.btnCancelExport.Name = "btnCancelExport";
            this.btnCancelExport.Size = new System.Drawing.Size(80, 34);
            this.btnCancelExport.TabIndex = 7;
            this.btnCancelExport.Text = "Cancel";
            this.btnCancelExport.Click += this.btnCancelExport_Click;
            //
            // progressExport
            //
            this.progressExport.Location = new System.Drawing.Point(14, 386);
            this.progressExport.Name = "progressExport";
            this.progressExport.Size = new System.Drawing.Size(846, 14);
            this.progressExport.TabIndex = 8;
            //
            // dataGridView1
            //
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.AutoSizeColumnsMode = Wisej.Web.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(14, 54);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(846, 312);
            this.dataGridView1.TabIndex = 8;
            this.dataGridView1.CellDoubleClick += this.dataGridView1_CellDoubleClick;
            this.dataGridView1.CellValueNeeded += this.dataGridView1_CellValueNeeded;
            this.dataGridView1.DataRead += this.dataGridView1_DataRead;
            //
            // lblGridStatus
            //
            this.lblGridStatus.Font = new System.Drawing.Font("monospace", 9F);
            this.lblGridStatus.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblGridStatus.Location = new System.Drawing.Point(14, 374);
            this.lblGridStatus.Name = "lblGridStatus";
            this.lblGridStatus.Size = new System.Drawing.Size(846, 18);
            this.lblGridStatus.TabIndex = 9;
            this.lblGridStatus.Text = "no search yet";
            //
            // lblScenario
            //
            this.lblScenario.Font = new System.Drawing.Font("monospace", 8F);
            this.lblScenario.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblScenario.Location = new System.Drawing.Point(14, 404);
            this.lblScenario.Name = "lblScenario";
            this.lblScenario.Size = new System.Drawing.Size(846, 32);
            this.lblScenario.TabIndex = 10;
            this.lblScenario.Text = "scenario Tickets/Search — budget 300 ms — tool: Database + .NET Object Allocation + the browser network panel\r\nModule 6: one statement per page, and an export that runs in the background";
            //
            // TicketGridPage
            //
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.lblScenario);
            this.Controls.Add(this.lblGridStatus);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.progressExport);
            this.Controls.Add(this.btnCancelExport);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.btnRedraw);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.cboPageSize);
            this.Controls.Add(this.lblRowsCaption);
            this.Controls.Add(this.cboStatus);
            this.Controls.Add(this.lblStatusCaption);
            this.Name = "TicketGridPage";
            this.Size = new System.Drawing.Size(876, 440);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private Wisej.Web.Label lblStatusCaption;
        private Wisej.Web.ComboBox cboStatus;
        private Wisej.Web.Label lblRowsCaption;
        private Wisej.Web.ComboBox cboPageSize;
        private Wisej.Web.Button btnSearch;
        private Wisej.Web.Button btnRedraw;
        private Wisej.Web.Button btnExport;
        private Wisej.Web.Button btnCancelExport;
        private Wisej.Web.ProgressBar progressExport;
        private Wisej.Web.DataGridView dataGridView1;
        private Wisej.Web.BindingSource bindingSource1;
        private Wisej.Web.Label lblGridStatus;
        private Wisej.Web.Label lblScenario;
    }
}
