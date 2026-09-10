namespace TicketOps.Controls
{
    partial class SearchBar
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
            this.tableSearch = new Wisej.Web.TableLayoutPanel();
            this.txtSearch = new Wisej.Web.TextBox();
            this.btnSearch = new Wisej.Web.Button();
            this.tableSearch.SuspendLayout();
            this.SuspendLayout();
            //
            // tableSearch  (one engine inside the control: a 2-column table — 100% box + fixed button)
            //
            this.tableSearch.ColumnCount = 2;
            this.tableSearch.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Percent, 100F));
            this.tableSearch.ColumnStyles.Add(new Wisej.Web.ColumnStyle(Wisej.Web.SizeType.Absolute, 84F));
            this.tableSearch.Controls.Add(this.txtSearch, 0, 0);
            this.tableSearch.Controls.Add(this.btnSearch, 1, 0);
            this.tableSearch.Dock = Wisej.Web.DockStyle.Fill;
            this.tableSearch.Name = "tableSearch";
            this.tableSearch.RowCount = 1;
            this.tableSearch.RowStyles.Add(new Wisej.Web.RowStyle(Wisej.Web.SizeType.Percent, 100F));
            //
            // txtSearch
            //
            this.txtSearch.Dock = Wisej.Web.DockStyle.Fill;
            this.txtSearch.Margin = new Wisej.Web.Padding(0, 0, 6, 0);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Watermark = "Search…";
            this.txtSearch.KeyDown += new Wisej.Web.KeyEventHandler(this.txtSearch_KeyDown);
            //
            // btnSearch
            //
            this.btnSearch.Dock = Wisej.Web.DockStyle.Fill;
            this.btnSearch.Margin = new Wisej.Web.Padding(0);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Text = "Search";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            //
            // SearchBar
            //
            this.Controls.Add(this.tableSearch);
            this.Name = "SearchBar";
            this.Size = new System.Drawing.Size(320, 34);
            this.tableSearch.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.TableLayoutPanel tableSearch;
        private Wisej.Web.TextBox txtSearch;
        private Wisej.Web.Button btnSearch;
    }
}
