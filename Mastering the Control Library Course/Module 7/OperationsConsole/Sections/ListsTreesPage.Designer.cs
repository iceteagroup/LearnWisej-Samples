namespace OperationsConsole.Sections
{
    partial class ListsTreesPage
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
            this.imageList = new Wisej.Web.ImageList(this.components);
            this.pnlOptions = new Wisej.Web.Panel();
            this.chkSimulateFailure = new Wisej.Web.CheckBox();
            this.pnlExplorer = new Wisej.Web.Panel();
            this.pnlRight = new Wisej.Web.Panel();
            this.pnlList = new Wisej.Web.Panel();
            this.documentList = new Wisej.Web.ListView();
            this.lblCounters = new Wisej.Web.Label();
            this.lblEmptyList = new Wisej.Web.Label();
            this.lblListTitle = new Wisej.Web.Label();
            this.pnlDetail = new Wisej.Web.Panel();
            this.documentDetail = new OperationsConsole.ListsTrees.DocumentDetailControl();
            this.lblDetailTitle = new Wisej.Web.Label();
            this.pnlTree = new Wisej.Web.Panel();
            this.categoryTree = new Wisej.Web.TreeView();
            this.lblTreeTitle = new Wisej.Web.Label();
            this.pnlOptions.SuspendLayout();
            this.pnlExplorer.SuspendLayout();
            this.pnlRight.SuspendLayout();
            this.pnlList.SuspendLayout();
            this.pnlDetail.SuspendLayout();
            this.pnlTree.SuspendLayout();
            this.SuspendLayout();
            //
            // imageList
            //
            this.imageList.ImageSize = new System.Drawing.Size(16, 16);
            //
            // pnlOptions
            //
            this.pnlOptions.BackColor = System.Drawing.Color.White;
            this.pnlOptions.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlOptions.Controls.Add(this.chkSimulateFailure);
            this.pnlOptions.Dock = Wisej.Web.DockStyle.Top;
            this.pnlOptions.Name = "pnlOptions";
            this.pnlOptions.Size = new System.Drawing.Size(740, 44);
            //
            // chkSimulateFailure
            //
            this.chkSimulateFailure.AccessibleName = "Make the document service fail";
            this.chkSimulateFailure.Location = new System.Drawing.Point(12, 10);
            this.chkSimulateFailure.Name = "chkSimulateFailure";
            this.chkSimulateFailure.Size = new System.Drawing.Size(210, 24);
            this.chkSimulateFailure.TabIndex = 1;
            this.chkSimulateFailure.Text = "Simulate service failure";
            this.chkSimulateFailure.CheckedChanged += new System.EventHandler(this.chkSimulateFailure_CheckedChanged);
            //
            // pnlExplorer
            //
            this.pnlExplorer.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.pnlExplorer.Controls.Add(this.pnlRight);
            this.pnlExplorer.Controls.Add(this.pnlTree);
            this.pnlExplorer.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlExplorer.Name = "pnlExplorer";
            this.pnlExplorer.Size = new System.Drawing.Size(740, 660);
            //
            // pnlRight
            //
            this.pnlRight.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.pnlRight.Controls.Add(this.pnlList);
            this.pnlRight.Controls.Add(this.pnlDetail);
            this.pnlRight.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlRight.Name = "pnlRight";
            this.pnlRight.Size = new System.Drawing.Size(504, 660);
            //
            // pnlList
            //
            this.pnlList.BackColor = System.Drawing.Color.White;
            this.pnlList.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlList.Controls.Add(this.documentList);
            this.pnlList.Controls.Add(this.lblCounters);
            this.pnlList.Controls.Add(this.lblEmptyList);
            this.pnlList.Controls.Add(this.lblListTitle);
            this.pnlList.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlList.Name = "pnlList";
            this.pnlList.Size = new System.Drawing.Size(504, 420);
            //
            // documentList
            //
            this.documentList.BorderStyle = Wisej.Web.BorderStyle.None;
            this.documentList.Columns.Add("Name", 250);
            this.documentList.Columns.Add("Type", 80);
            this.documentList.Columns.Add("Size", 74);
            this.documentList.Columns.Add("Modified", 94);
            this.documentList.Dock = Wisej.Web.DockStyle.Fill;
            this.documentList.GridLines = false;
            this.documentList.MultiSelect = false;
            this.documentList.Name = "documentList";
            this.documentList.PrefetchItems = 0;
            this.documentList.SelectionMode = Wisej.Web.SelectionMode.One;
            this.documentList.SmallImageList = this.imageList;
            this.documentList.TabIndex = 11;
            this.documentList.View = Wisej.Web.View.Details;
            this.documentList.VirtualMode = true;
            this.documentList.VirtualListSize = 0;
            this.documentList.RetrieveVirtualItem += new Wisej.Web.RetrieveVirtualItemEventHandler(this.documentList_RetrieveVirtualItem);
            this.documentList.SelectedIndexChanged += new System.EventHandler(this.documentList_SelectedIndexChanged);
            //
            // lblCounters
            //
            this.lblCounters.AutoSize = false;
            this.lblCounters.Dock = Wisej.Web.DockStyle.Bottom;
            this.lblCounters.Font = new System.Drawing.Font("monospace", 9F);
            this.lblCounters.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblCounters.Name = "lblCounters";
            this.lblCounters.Padding = new Wisej.Web.Padding(14, 0, 8, 0);
            this.lblCounters.Size = new System.Drawing.Size(504, 28);
            this.lblCounters.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblEmptyList
            //
            this.lblEmptyList.AutoSize = false;
            this.lblEmptyList.BackColor = System.Drawing.Color.FromArgb(253, 246, 232);
            this.lblEmptyList.Dock = Wisej.Web.DockStyle.Top;
            this.lblEmptyList.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblEmptyList.ForeColor = System.Drawing.Color.FromArgb(232, 161, 60);
            this.lblEmptyList.Name = "lblEmptyList";
            this.lblEmptyList.Padding = new Wisej.Web.Padding(14, 0, 8, 0);
            this.lblEmptyList.Size = new System.Drawing.Size(504, 32);
            this.lblEmptyList.Text = "No documents in this category";
            this.lblEmptyList.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblEmptyList.Visible = false;
            //
            // lblListTitle
            //
            this.lblListTitle.AutoSize = false;
            this.lblListTitle.Dock = Wisej.Web.DockStyle.Top;
            this.lblListTitle.Font = new System.Drawing.Font("default", 11F, System.Drawing.FontStyle.Bold);
            this.lblListTitle.Name = "lblListTitle";
            this.lblListTitle.Padding = new Wisej.Web.Padding(14, 0, 8, 0);
            this.lblListTitle.Size = new System.Drawing.Size(504, 34);
            this.lblListTitle.Text = "Documents";
            this.lblListTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // pnlDetail
            //
            this.pnlDetail.BackColor = System.Drawing.Color.White;
            this.pnlDetail.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlDetail.Controls.Add(this.documentDetail);
            this.pnlDetail.Controls.Add(this.lblDetailTitle);
            this.pnlDetail.Dock = Wisej.Web.DockStyle.Bottom;
            this.pnlDetail.Name = "pnlDetail";
            this.pnlDetail.Size = new System.Drawing.Size(504, 240);
            //
            // documentDetail
            //
            this.documentDetail.Dock = Wisej.Web.DockStyle.Fill;
            this.documentDetail.Name = "documentDetail";
            this.documentDetail.Size = new System.Drawing.Size(504, 206);
            this.documentDetail.TabIndex = 12;
            //
            // lblDetailTitle
            //
            this.lblDetailTitle.AutoSize = false;
            this.lblDetailTitle.Dock = Wisej.Web.DockStyle.Top;
            this.lblDetailTitle.Font = new System.Drawing.Font("default", 11F, System.Drawing.FontStyle.Bold);
            this.lblDetailTitle.Name = "lblDetailTitle";
            this.lblDetailTitle.Padding = new Wisej.Web.Padding(14, 0, 8, 0);
            this.lblDetailTitle.Size = new System.Drawing.Size(504, 34);
            this.lblDetailTitle.Text = "Document detail";
            this.lblDetailTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // pnlTree
            //
            this.pnlTree.BackColor = System.Drawing.Color.White;
            this.pnlTree.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlTree.Controls.Add(this.categoryTree);
            this.pnlTree.Controls.Add(this.lblTreeTitle);
            this.pnlTree.Dock = Wisej.Web.DockStyle.Left;
            this.pnlTree.Name = "pnlTree";
            this.pnlTree.Size = new System.Drawing.Size(236, 660);
            //
            // categoryTree
            //
            this.categoryTree.BorderStyle = Wisej.Web.BorderStyle.None;
            this.categoryTree.Dock = Wisej.Web.DockStyle.Fill;
            this.categoryTree.ImageList = this.imageList;
            this.categoryTree.Name = "categoryTree";
            this.categoryTree.ShowRootLines = true;
            this.categoryTree.TabIndex = 10;
            this.categoryTree.AfterExpand += new Wisej.Web.TreeViewEventHandler(this.categoryTree_AfterExpand);
            this.categoryTree.AfterSelect += new Wisej.Web.TreeViewEventHandler(this.categoryTree_AfterSelect);
            //
            // lblTreeTitle
            //
            this.lblTreeTitle.AutoSize = false;
            this.lblTreeTitle.Dock = Wisej.Web.DockStyle.Top;
            this.lblTreeTitle.Font = new System.Drawing.Font("default", 11F, System.Drawing.FontStyle.Bold);
            this.lblTreeTitle.Name = "lblTreeTitle";
            this.lblTreeTitle.Padding = new Wisej.Web.Padding(14, 0, 8, 0);
            this.lblTreeTitle.Size = new System.Drawing.Size(236, 34);
            this.lblTreeTitle.Text = "Categories";
            this.lblTreeTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // ListsTreesPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.pnlExplorer);
            this.Controls.Add(this.pnlOptions);
            this.Name = "ListsTreesPage";
            this.Size = new System.Drawing.Size(740, 704);
            this.pnlOptions.ResumeLayout(false);
            this.pnlExplorer.ResumeLayout(false);
            this.pnlRight.ResumeLayout(false);
            this.pnlList.ResumeLayout(false);
            this.pnlDetail.ResumeLayout(false);
            this.pnlTree.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.ImageList imageList;
        private Wisej.Web.Panel pnlOptions;
        private Wisej.Web.CheckBox chkSimulateFailure;
        private Wisej.Web.Panel pnlExplorer;
        private Wisej.Web.Panel pnlTree;
        private Wisej.Web.Label lblTreeTitle;
        private Wisej.Web.TreeView categoryTree;
        private Wisej.Web.Panel pnlRight;
        private Wisej.Web.Panel pnlList;
        private Wisej.Web.Label lblListTitle;
        private Wisej.Web.Label lblEmptyList;
        private Wisej.Web.Label lblCounters;
        private Wisej.Web.ListView documentList;
        private Wisej.Web.Panel pnlDetail;
        private Wisej.Web.Label lblDetailTitle;
        private OperationsConsole.ListsTrees.DocumentDetailControl documentDetail;
    }
}
