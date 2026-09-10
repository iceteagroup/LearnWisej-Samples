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
            this.expandTimer = new Wisej.Web.Timer(this.components);
            this.pnlHeader = new Wisej.Web.Panel();
            this.lblTitle = new Wisej.Web.Label();
            this.lblSubtitle = new Wisej.Web.Label();
            this.lblCounters = new Wisej.Web.Label();
            this.pnlCommands = new Wisej.Web.Panel();
            this.btnReloadTree = new Wisej.Web.Button();
            this.btnExpandTopLevel = new Wisej.Web.Button();
            this.chkSimulateFailure = new Wisej.Web.CheckBox();
            this.btnRetry = new Wisej.Web.Button();
            this.lblCreatedCaption = new Wisej.Web.Label();
            this.txtCreatedItems = new Wisej.Web.TextBox();
            this.pnlExplorer = new Wisej.Web.Panel();
            this.pnlRight = new Wisej.Web.Panel();
            this.pnlList = new Wisej.Web.Panel();
            this.documentList = new Wisej.Web.ListView();
            this.lblEmptyList = new Wisej.Web.Label();
            this.lblListTitle = new Wisej.Web.Label();
            this.pnlDetail = new Wisej.Web.Panel();
            this.documentDetail = new OperationsConsole.ListsTrees.DocumentDetailControl();
            this.lblDetailTitle = new Wisej.Web.Label();
            this.pnlTree = new Wisej.Web.Panel();
            this.categoryTree = new Wisej.Web.TreeView();
            this.lblTreeTitle = new Wisej.Web.Label();
            this.pnlHeader.SuspendLayout();
            this.pnlCommands.SuspendLayout();
            this.pnlExplorer.SuspendLayout();
            this.pnlRight.SuspendLayout();
            this.pnlList.SuspendLayout();
            this.pnlDetail.SuspendLayout();
            this.pnlTree.SuspendLayout();
            this.SuspendLayout();
            //
            // imageList  (shared by categoryTree and documentList: folder, contract, invoice, drawing, warning)
            // The five SVG files are added in the code-behind (RegisterIcons) so the URL convention is commented there.
            //
            this.imageList.ImageSize = new System.Drawing.Size(16, 16);
            //
            // expandTimer  (progress path: "Expand all top level" expands one node per tick, each one lazy-loading)
            //
            this.expandTimer.Interval = 500;
            this.expandTimer.Tick += new System.EventHandler(this.expandTimer_Tick);
            //
            // pnlHeader  (Dock = Top — title and the "312 rows · 8 items created" counters from the walkthrough)
            //
            this.pnlHeader.BackColor = System.Drawing.Color.White;
            this.pnlHeader.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Controls.Add(this.lblSubtitle);
            this.pnlHeader.Controls.Add(this.lblCounters);
            this.pnlHeader.Dock = Wisej.Web.DockStyle.Top;
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(740, 66);
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = false;
            this.lblTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(14, 8);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(420, 26);
            this.lblTitle.Text = "Document explorer";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblSubtitle
            //
            this.lblSubtitle.AutoSize = false;
            this.lblSubtitle.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblSubtitle.Location = new System.Drawing.Point(14, 34);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(420, 22);
            this.lblSubtitle.Text = "TREEVIEW (LAZY) · LISTVIEW (VIRTUAL MODE) · DETAIL USERCONTROL";
            this.lblSubtitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblCounters  (VirtualListSize vs the number of ListViewItem objects RetrieveVirtualItem actually built)
            //
            this.lblCounters.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblCounters.AutoSize = false;
            this.lblCounters.Font = new System.Drawing.Font("monospace", 9F);
            this.lblCounters.ForeColor = System.Drawing.Color.FromArgb(58, 77, 99);
            this.lblCounters.Location = new System.Drawing.Point(440, 20);
            this.lblCounters.Name = "lblCounters";
            this.lblCounters.Size = new System.Drawing.Size(286, 26);
            this.lblCounters.Text = "— rows · 0 items created";
            this.lblCounters.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblCounters.ToolTipText = "Left: VirtualListSize (how many rows the category has). Right: how many ListViewItem objects RetrieveVirtualItem had to create.";
            //
            // pnlCommands  (Dock = Top — success, progress, failure and recovery are all one click away)
            //
            this.pnlCommands.BackColor = System.Drawing.Color.White;
            this.pnlCommands.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlCommands.Controls.Add(this.btnReloadTree);
            this.pnlCommands.Controls.Add(this.btnExpandTopLevel);
            this.pnlCommands.Controls.Add(this.chkSimulateFailure);
            this.pnlCommands.Controls.Add(this.btnRetry);
            this.pnlCommands.Controls.Add(this.lblCreatedCaption);
            this.pnlCommands.Controls.Add(this.txtCreatedItems);
            this.pnlCommands.Dock = Wisej.Web.DockStyle.Top;
            this.pnlCommands.Name = "pnlCommands";
            this.pnlCommands.Size = new System.Drawing.Size(740, 54);
            //
            // btnReloadTree  (success path)
            //
            this.btnReloadTree.AccessibleName = "Reload the category tree";
            this.btnReloadTree.Location = new System.Drawing.Point(12, 10);
            this.btnReloadTree.Name = "btnReloadTree";
            this.btnReloadTree.Size = new System.Drawing.Size(104, 34);
            this.btnReloadTree.TabIndex = 1;
            this.btnReloadTree.Text = "Reload tree";
            this.btnReloadTree.ToolTipText = "Calls DocumentService.GetTopCategories() and rebuilds the top level only — every node gets a placeholder child.";
            this.btnReloadTree.Click += new System.EventHandler(this.btnReloadTree_Click);
            //
            // btnExpandTopLevel  (progress path — a Wisej.Web.Timer expands one node per tick)
            //
            this.btnExpandTopLevel.AccessibleName = "Expand every top level category one by one";
            this.btnExpandTopLevel.Location = new System.Drawing.Point(124, 10);
            this.btnExpandTopLevel.Name = "btnExpandTopLevel";
            this.btnExpandTopLevel.Size = new System.Drawing.Size(150, 34);
            this.btnExpandTopLevel.TabIndex = 2;
            this.btnExpandTopLevel.Text = "Expand all top level";
            this.btnExpandTopLevel.ToolTipText = "Expands the top-level categories one per tick; every expand runs AfterExpand and lazy-loads that branch.";
            this.btnExpandTopLevel.Click += new System.EventHandler(this.btnExpandTopLevel_Click);
            //
            // chkSimulateFailure  (failure path — DocumentService.SimulateFailure)
            //
            this.chkSimulateFailure.AccessibleName = "Make the next document service call fail";
            this.chkSimulateFailure.Location = new System.Drawing.Point(282, 16);
            this.chkSimulateFailure.Name = "chkSimulateFailure";
            this.chkSimulateFailure.Size = new System.Drawing.Size(150, 24);
            this.chkSimulateFailure.TabIndex = 3;
            this.chkSimulateFailure.Text = "Simulate failure";
            this.chkSimulateFailure.ToolTipText = "DocumentService.SimulateFailure — the next expand, page load or document load throws. One-shot: it clears itself so Retry can recover.";
            this.chkSimulateFailure.CheckedChanged += new System.EventHandler(this.chkSimulateFailure_CheckedChanged);
            //
            // btnRetry  (recovery path — re-runs whatever failed last)
            //
            this.btnRetry.AccessibleName = "Retry the last document service call";
            this.btnRetry.Location = new System.Drawing.Point(440, 10);
            this.btnRetry.Name = "btnRetry";
            this.btnRetry.Size = new System.Drawing.Size(76, 34);
            this.btnRetry.TabIndex = 4;
            this.btnRetry.Text = "Retry";
            this.btnRetry.ToolTipText = "Runs the last action again (reload tree / expand node / load page / load document).";
            this.btnRetry.Click += new System.EventHandler(this.btnRetry_Click);
            //
            // lblCreatedCaption
            //
            this.lblCreatedCaption.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblCreatedCaption.AutoSize = false;
            this.lblCreatedCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.lblCreatedCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblCreatedCaption.Location = new System.Drawing.Point(530, 16);
            this.lblCreatedCaption.Name = "lblCreatedCaption";
            this.lblCreatedCaption.Size = new System.Drawing.Size(98, 24);
            this.lblCreatedCaption.Text = "CREATED ITEMS";
            this.lblCreatedCaption.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // txtCreatedItems  (read-only counter: how many ListViewItem objects exist for the current page)
            //
            this.txtCreatedItems.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.txtCreatedItems.Font = new System.Drawing.Font("monospace", 9F);
            this.txtCreatedItems.Location = new System.Drawing.Point(634, 13);
            this.txtCreatedItems.Name = "txtCreatedItems";
            this.txtCreatedItems.ReadOnly = true;
            this.txtCreatedItems.Size = new System.Drawing.Size(56, 28);
            this.txtCreatedItems.TabIndex = 5;
            this.txtCreatedItems.Text = "0";
            this.txtCreatedItems.TextAlign = Wisej.Web.HorizontalAlignment.Center;
            this.txtCreatedItems.ToolTipText = "Items RetrieveVirtualItem created for the current page. With 312 rows this stays a handful — that is virtual mode.";
            //
            // pnlExplorer  (Dock = Fill — the tree on the left, the list and the detail on the right)
            //
            this.pnlExplorer.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            // Dock is applied from the LAST Controls.Add to the FIRST: pnlTree (Left) claims its column, pnlRight fills the rest.
            this.pnlExplorer.Controls.Add(this.pnlRight);
            this.pnlExplorer.Controls.Add(this.pnlTree);
            this.pnlExplorer.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlExplorer.Name = "pnlExplorer";
            this.pnlExplorer.Size = new System.Drawing.Size(740, 584);
            //
            // pnlRight  (Dock = Fill — the list fills, the detail card sits at the bottom)
            //
            this.pnlRight.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.pnlRight.Controls.Add(this.pnlList);
            this.pnlRight.Controls.Add(this.pnlDetail);
            this.pnlRight.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlRight.Name = "pnlRight";
            this.pnlRight.Size = new System.Drawing.Size(504, 584);
            //
            // pnlList
            //
            this.pnlList.BackColor = System.Drawing.Color.White;
            this.pnlList.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlList.Controls.Add(this.documentList);
            this.pnlList.Controls.Add(this.lblEmptyList);
            this.pnlList.Controls.Add(this.lblListTitle);
            this.pnlList.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlList.Name = "pnlList";
            this.pnlList.Size = new System.Drawing.Size(504, 344);
            //
            // documentList  (ListView · Details · VIRTUAL MODE — VirtualListSize + RetrieveVirtualItem)
            //
            this.documentList.BorderStyle = Wisej.Web.BorderStyle.None;
            this.documentList.Columns.Add("Name", 250);
            this.documentList.Columns.Add("Type", 80);
            this.documentList.Columns.Add("Size", 74);
            this.documentList.Columns.Add("Modified", 94);
            this.documentList.Dock = Wisej.Web.DockStyle.Fill;
            this.documentList.GridLines = false;
            // Wisej's Details view renders through a DataGridView and always selects the whole row, so the
            // WinForms FullRowSelect property does not exist here: MultiSelect = false + SelectionMode.One is the equivalent.
            this.documentList.MultiSelect = false;
            this.documentList.Name = "documentList";
            // PrefetchItems stays at its default 0 on purpose, so the "items created" counter shows exactly what
            // the visible rows cost. Raising it (e.g. 20) pre-renders rows outside the viewport for smoother
            // scrolling — the third strategy after lazy loading and virtual mode. See docs/ExplorerNotes.md.
            this.documentList.PrefetchItems = 0;
            this.documentList.SelectionMode = Wisej.Web.SelectionMode.One;
            this.documentList.SmallImageList = this.imageList;
            this.documentList.TabIndex = 11;
            this.documentList.View = Wisej.Web.View.Details;
            this.documentList.VirtualMode = true;
            this.documentList.VirtualListSize = 0;
            this.documentList.CacheVirtualItems += new Wisej.Web.CacheVirtualItemsEventHandler(this.documentList_CacheVirtualItems);
            this.documentList.RetrieveVirtualItem += new Wisej.Web.RetrieveVirtualItemEventHandler(this.documentList_RetrieveVirtualItem);
            this.documentList.SelectedIndexChanged += new System.EventHandler(this.documentList_SelectedIndexChanged);
            //
            // lblEmptyList  (Dock = Top, hidden — the empty-category state; takes no space while invisible)
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
            // documentDetail  (the detail UserControl — it only ever receives a DocumentModel)
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
            // pnlTree  (Dock = Left)
            //
            this.pnlTree.BackColor = System.Drawing.Color.White;
            this.pnlTree.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlTree.Controls.Add(this.categoryTree);
            this.pnlTree.Controls.Add(this.lblTreeTitle);
            this.pnlTree.Dock = Wisej.Web.DockStyle.Left;
            this.pnlTree.Name = "pnlTree";
            this.pnlTree.Size = new System.Drawing.Size(236, 584);
            //
            // categoryTree  (TreeView · LAZY LOADING — top level only, a placeholder child per node, AfterExpand fetches)
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
            // Fill first, the Top bars last (docking is applied from the last Controls.Add to the first).
            this.Controls.Add(this.pnlExplorer);
            this.Controls.Add(this.pnlCommands);
            this.Controls.Add(this.pnlHeader);
            this.Name = "ListsTreesPage";
            this.Size = new System.Drawing.Size(740, 704);
            this.pnlHeader.ResumeLayout(false);
            this.pnlCommands.ResumeLayout(false);
            this.pnlExplorer.ResumeLayout(false);
            this.pnlRight.ResumeLayout(false);
            this.pnlList.ResumeLayout(false);
            this.pnlDetail.ResumeLayout(false);
            this.pnlTree.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        // shared by both controls — the tree and the list speak the same visual language
        private Wisej.Web.ImageList imageList;
        // progress path
        private Wisej.Web.Timer expandTimer;

        // header
        private Wisej.Web.Panel pnlHeader;
        private Wisej.Web.Label lblTitle;
        private Wisej.Web.Label lblSubtitle;
        private Wisej.Web.Label lblCounters;

        // command row
        private Wisej.Web.Panel pnlCommands;
        private Wisej.Web.Button btnReloadTree;
        private Wisej.Web.Button btnExpandTopLevel;
        private Wisej.Web.CheckBox chkSimulateFailure;
        private Wisej.Web.Button btnRetry;
        private Wisej.Web.Label lblCreatedCaption;
        private Wisej.Web.TextBox txtCreatedItems;

        // explorer
        private Wisej.Web.Panel pnlExplorer;
        private Wisej.Web.Panel pnlTree;
        private Wisej.Web.Label lblTreeTitle;
        private Wisej.Web.TreeView categoryTree;
        private Wisej.Web.Panel pnlRight;
        private Wisej.Web.Panel pnlList;
        private Wisej.Web.Label lblListTitle;
        private Wisej.Web.Label lblEmptyList;
        private Wisej.Web.ListView documentList;
        private Wisej.Web.Panel pnlDetail;
        private Wisej.Web.Label lblDetailTitle;
        private OperationsConsole.ListsTrees.DocumentDetailControl documentDetail;
    }
}
