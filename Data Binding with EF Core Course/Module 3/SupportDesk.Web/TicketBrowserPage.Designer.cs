namespace SupportDesk.Web
{
    partial class TicketBrowserPage
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
            this.ticketBindingSource = new Wisej.Web.BindingSource(this.components);
            this.panelBrowser = new Wisej.Web.Panel();
            this.labelTitle = new Wisej.Web.Label();
            this.searchTextBox = new Wisej.Web.TextBox();
            this.statusComboBox = new Wisej.Web.ComboBox();
            this.customerComboBox = new Wisej.Web.ComboBox();
            this.searchButton = new Wisej.Web.Button();
            this.ticketsDataGridView = new Wisej.Web.DataGridView();
            this.colNumber = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colTitle = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colCustomerName = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colAgentName = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colDueDate = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colUpdatedAt = new Wisej.Web.DataGridViewTextBoxColumn();
            this.statusLabel = new Wisej.Web.Label();
            this.prevPageButton = new Wisej.Web.Button();
            this.nextPageButton = new Wisej.Web.Button();
            this.panelBrowser.SuspendLayout();
            this.SuspendLayout();
            //
            // panelBrowser
            //
            this.panelBrowser.BackColor = System.Drawing.Color.White;
            this.panelBrowser.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelBrowser.Controls.Add(this.labelTitle);
            this.panelBrowser.Controls.Add(this.searchTextBox);
            this.panelBrowser.Controls.Add(this.statusComboBox);
            this.panelBrowser.Controls.Add(this.customerComboBox);
            this.panelBrowser.Controls.Add(this.searchButton);
            this.panelBrowser.Controls.Add(this.ticketsDataGridView);
            this.panelBrowser.Controls.Add(this.statusLabel);
            this.panelBrowser.Controls.Add(this.prevPageButton);
            this.panelBrowser.Controls.Add(this.nextPageButton);
            this.panelBrowser.Location = new System.Drawing.Point(30, 30);
            this.panelBrowser.Name = "panelBrowser";
            this.panelBrowser.Size = new System.Drawing.Size(940, 530);
            //
            // labelTitle
            //
            this.labelTitle.AutoSize = false;
            this.labelTitle.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.labelTitle.Location = new System.Drawing.Point(24, 16);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(892, 30);
            this.labelTitle.Text = "Support Desk Data Console  ·  Tickets";
            //
            // searchTextBox
            //
            this.searchTextBox.Location = new System.Drawing.Point(24, 60);
            this.searchTextBox.Name = "searchTextBox";
            this.searchTextBox.Size = new System.Drawing.Size(452, 30);
            this.searchTextBox.Watermark = "Search number, title or customer…";
            //
            // statusComboBox
            //
            this.statusComboBox.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.statusComboBox.Location = new System.Drawing.Point(486, 60);
            this.statusComboBox.Name = "statusComboBox";
            this.statusComboBox.Size = new System.Drawing.Size(150, 30);
            //
            // customerComboBox
            //
            this.customerComboBox.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.customerComboBox.Location = new System.Drawing.Point(646, 60);
            this.customerComboBox.Name = "customerComboBox";
            this.customerComboBox.Size = new System.Drawing.Size(150, 30);
            //
            // searchButton
            //
            this.searchButton.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.searchButton.Location = new System.Drawing.Point(806, 60);
            this.searchButton.Name = "searchButton";
            this.searchButton.Size = new System.Drawing.Size(110, 30);
            this.searchButton.Text = "Search";
            this.searchButton.Click += new System.EventHandler(this.searchButton_Click);
            //
            // ticketsDataGridView
            //
            this.ticketsDataGridView.AllowUserToAddRows = false;
            this.ticketsDataGridView.AllowUserToDeleteRows = false;
            this.ticketsDataGridView.AutoGenerateColumns = false;
            this.ticketsDataGridView.AutoSizeColumnsMode = Wisej.Web.DataGridViewAutoSizeColumnsMode.Fill;
            this.ticketsDataGridView.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
            this.colNumber,
            this.colTitle,
            this.colCustomerName,
            this.colAgentName,
            this.colStatus,
            this.colDueDate,
            this.colUpdatedAt});
            this.ticketsDataGridView.Location = new System.Drawing.Point(24, 104);
            this.ticketsDataGridView.MultiSelect = false;
            this.ticketsDataGridView.Name = "ticketsDataGridView";
            this.ticketsDataGridView.ReadOnly = true;
            this.ticketsDataGridView.RowHeadersVisible = false;
            this.ticketsDataGridView.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.ticketsDataGridView.Size = new System.Drawing.Size(892, 360);
            this.ticketsDataGridView.DataSource = this.ticketBindingSource;
            //
            // colNumber
            //
            this.colNumber.DataPropertyName = "Number";
            this.colNumber.FillWeight = 70F;
            this.colNumber.HeaderText = "Number";
            this.colNumber.Name = "colNumber";
            this.colNumber.Width = 70;
            //
            // colTitle
            //
            this.colTitle.DataPropertyName = "Title";
            this.colTitle.FillWeight = 220F;
            this.colTitle.HeaderText = "Title";
            this.colTitle.Name = "colTitle";
            this.colTitle.Width = 220;
            //
            // colCustomerName
            //
            this.colCustomerName.DataPropertyName = "CustomerName";
            this.colCustomerName.FillWeight = 130F;
            this.colCustomerName.HeaderText = "Customer";
            this.colCustomerName.Name = "colCustomerName";
            this.colCustomerName.Width = 130;
            //
            // colAgentName
            //
            this.colAgentName.DataPropertyName = "AgentName";
            this.colAgentName.DefaultCellStyle.NullValue = "—";
            this.colAgentName.FillWeight = 120F;
            this.colAgentName.HeaderText = "Agent";
            this.colAgentName.Name = "colAgentName";
            this.colAgentName.Width = 120;
            //
            // colStatus
            //
            this.colStatus.DataPropertyName = "Status";
            this.colStatus.FillWeight = 80F;
            this.colStatus.HeaderText = "Status";
            this.colStatus.Name = "colStatus";
            this.colStatus.Width = 80;
            //
            // colDueDate
            //
            this.colDueDate.DataPropertyName = "DueDate";
            this.colDueDate.DefaultCellStyle.Format = "yyyy-MM-dd";
            this.colDueDate.DefaultCellStyle.NullValue = "";
            this.colDueDate.FillWeight = 80F;
            this.colDueDate.HeaderText = "Due";
            this.colDueDate.Name = "colDueDate";
            this.colDueDate.Width = 80;
            //
            // colUpdatedAt
            //
            this.colUpdatedAt.DataPropertyName = "UpdatedAt";
            this.colUpdatedAt.DefaultCellStyle.Format = "yyyy-MM-dd HH:mm";
            this.colUpdatedAt.FillWeight = 110F;
            this.colUpdatedAt.HeaderText = "Updated";
            this.colUpdatedAt.Name = "colUpdatedAt";
            this.colUpdatedAt.Width = 110;
            //
            // statusLabel
            //
            this.statusLabel.AutoSize = false;
            this.statusLabel.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.statusLabel.Location = new System.Drawing.Point(24, 476);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(600, 28);
            this.statusLabel.Text = "Ready";
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // prevPageButton
            //
            this.prevPageButton.Location = new System.Drawing.Point(652, 474);
            this.prevPageButton.Name = "prevPageButton";
            this.prevPageButton.Size = new System.Drawing.Size(128, 32);
            this.prevPageButton.Text = "◀ Previous page";
            this.prevPageButton.Click += new System.EventHandler(this.prevPageButton_Click);
            //
            // nextPageButton
            //
            this.nextPageButton.Location = new System.Drawing.Point(788, 474);
            this.nextPageButton.Name = "nextPageButton";
            this.nextPageButton.Size = new System.Drawing.Size(128, 32);
            this.nextPageButton.Text = "Next page ▶";
            this.nextPageButton.Click += new System.EventHandler(this.nextPageButton_Click);
            //
            // TicketBrowserPage
            //
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.panelBrowser);
            this.Name = "TicketBrowserPage";
            this.Size = new System.Drawing.Size(1000, 590);
            this.Text = "Support Desk Data Console";
            this.Load += new System.EventHandler(this.TicketBrowserPage_Load);
            this.panelBrowser.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.BindingSource ticketBindingSource;
        private Wisej.Web.Panel panelBrowser;
        private Wisej.Web.Label labelTitle;
        private Wisej.Web.TextBox searchTextBox;
        private Wisej.Web.ComboBox statusComboBox;
        private Wisej.Web.ComboBox customerComboBox;
        private Wisej.Web.Button searchButton;
        private Wisej.Web.DataGridView ticketsDataGridView;
        private Wisej.Web.DataGridViewTextBoxColumn colNumber;
        private Wisej.Web.DataGridViewTextBoxColumn colTitle;
        private Wisej.Web.DataGridViewTextBoxColumn colCustomerName;
        private Wisej.Web.DataGridViewTextBoxColumn colAgentName;
        private Wisej.Web.DataGridViewTextBoxColumn colStatus;
        private Wisej.Web.DataGridViewTextBoxColumn colDueDate;
        private Wisej.Web.DataGridViewTextBoxColumn colUpdatedAt;
        private Wisej.Web.Label statusLabel;
        private Wisej.Web.Button prevPageButton;
        private Wisej.Web.Button nextPageButton;
    }
}
