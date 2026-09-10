using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Threading.Tasks;
using OperationsConsole.Models;
using OperationsConsole.Services;
using OperationsConsole.Shell;
using Wisej.Web;

namespace OperationsConsole.Sections
{
    /// <summary>
    /// <b>Lists and Trees</b> — the Operations Console document explorer (Module 4 · Lists, Trees, Repeaters,
    /// and Hierarchical Data).
    /// <para>
    /// Three controls, three jobs. <c>categoryTree</c> is a <see cref="TreeView"/> that loads <b>only the top
    /// level</b>: every node gets one placeholder child so the expand glyph appears, and
    /// <see cref="categoryTree_AfterExpand"/> replaces it with the real children the first time the node opens.
    /// <c>documentList</c> is a <see cref="ListView"/> in <b>virtual mode</b>: the page sets
    /// <c>VirtualListSize</c> from a server-side <see cref="DocumentPage"/> and builds a
    /// <see cref="ListViewItem"/> only when <see cref="documentList_RetrieveVirtualItem"/> is asked for one.
    /// <c>documentDetail</c> is a UserControl that receives a finished <see cref="DocumentModel"/> and knows
    /// nothing else.
    /// </para>
    /// <para>
    /// <b>Identity never lives in the text.</b> Three folders are called "Contracts" (categories 7, 10 and 13).
    /// Every <see cref="TreeNode"/> carries its category ID in <c>Tag</c> (and "cat-7" in <c>Name</c>), every
    /// <see cref="ListViewItem"/> carries its document ID in <c>Tag</c>, and no handler in this file ever
    /// reads <c>Node.Text</c> to decide anything.
    /// </para>
    /// <para>
    /// <b>Selection is an event, not a query.</b> The handlers resolve an ID, call
    /// <see cref="DocumentService"/> inside try/catch with a visible loading state, and hand the result to the
    /// detail control. No data access appears in a tree or list handler.
    /// </para>
    /// </summary>
    public partial class ListsTreesPage : UserControl, ISection
    {
        /// <summary>The marker that says "this branch has not been loaded yet".</summary>
        private const string PlaceholderName = "placeholder";

        /// <summary>The marker of the node that reports a failed expand.</summary>
        private const string ErrorNodeName = "load-error";

        private readonly DocumentService _documents = new DocumentService();

        /// <summary>
        /// The server-side page the virtual ListView reads from. <c>RetrieveVirtualItem</c> indexes into this and
        /// never calls the service — that is the whole point of a cache between the control and the data.
        /// </summary>
        private DocumentPage _page;

        /// <summary>How many <see cref="ListViewItem"/> objects <c>RetrieveVirtualItem</c> actually had to build.</summary>
        private int _createdItems;

        /// <summary>Set while the page rewrites <c>VirtualListSize</c>, so the resulting selection change is ignored.</summary>
        private bool _suspendSelection;

        /// <summary>Set while the page unticks the checkbox itself, so <c>CheckedChanged</c> does not log twice.</summary>
        private bool _syncingCheckBox;

        /// <summary>What the <b>Retry</b> button runs again — the recovery path of whatever failed last.</summary>
        private Func<Task> _lastAction;

        /// <summary>The nodes the progress path still has to expand, one per <c>expandTimer</c> tick.</summary>
        private readonly List<TreeNode> _expandQueue = new List<TreeNode>();

        public ListsTreesPage()
        {
            InitializeComponent();

            RegisterIcons();
            LoadTopCategories();
        }

        /// <inheritdoc/>
        public string Title => "Lists and Trees";

        /// <inheritdoc/>
        public void RefreshSection()
        {
            ConsoleLog.Add("ListsTreesPage.RefreshSection() → reloading the top level; open branches are dropped and will lazy-load again");
            LoadTopCategories();
        }

        // ------------------------------------------------------------------------------------------------
        // Icons — one ImageList shared by the tree and the list, so both speak the same visual language
        // ------------------------------------------------------------------------------------------------

        /// <summary>
        /// <c>ImageList.Images.Add(key, nameOrUrl)</c> takes a theme icon <b>name</b> or a <b>URL</b> as its second
        /// argument. Static files are served from the project folder, so <c>"wwwroot/icons/folder.svg"</c> is
        /// fetched by the browser as <c>/wwwroot/icons/folder.svg</c>.
        /// <para>
        /// The alternative is to pass built-in theme icon names (<c>"icon-folder"</c>, <c>"icon-warning"</c>, …):
        /// they follow the active theme for free, but you get whatever the theme ships. Own SVG files were chosen
        /// here because the five kinds — folder, contract, invoice, drawing, warning — are business meanings, not
        /// UI affordances, and the learner can open the files and see exactly what an image key is.
        /// </para>
        /// </summary>
        private void RegisterIcons()
        {
            this.imageList.Images.Add("folder", "wwwroot/icons/folder.svg");
            this.imageList.Images.Add("contract", "wwwroot/icons/contract.svg");
            this.imageList.Images.Add("invoice", "wwwroot/icons/invoice.svg");
            this.imageList.Images.Add("drawing", "wwwroot/icons/drawing.svg");
            this.imageList.Images.Add("warning", "wwwroot/icons/warning.svg");

            ConsoleLog.Add("imageList ← 5 keys (folder, contract, invoice, drawing, warning) shared by categoryTree and documentList");
        }

        // ------------------------------------------------------------------------------------------------
        // The tree — lazy loading
        // ------------------------------------------------------------------------------------------------

        /// <summary>
        /// Builds the <b>top level only</b>. This is the whole anti-pattern the reading warns about, inverted:
        /// nothing below the first level is fetched until the user asks for it.
        /// </summary>
        private void LoadTopCategories()
        {
            _lastAction = () => { LoadTopCategories(); return Task.CompletedTask; };

            this.expandTimer.Stop();
            this._expandQueue.Clear();
            this.btnExpandTopLevel.Enabled = true;

            ConsoleLog.Add("categoryTree ← DocumentService.GetTopCategories() — top level only, children load on expand");

            try
            {
                var categories = _documents.GetTopCategories();

                this.categoryTree.BeginUpdate();
                this.categoryTree.Nodes.Clear();
                foreach (var category in categories)
                    this.categoryTree.Nodes.Add(CreateCategoryNode(category, withPlaceholder: true));
                this.categoryTree.EndUpdate();

                ClearList("Select a category on the left, then a document.");
                ConsoleLog.Status(
                    categories.Count + " top-level categories · " + _documents.TotalDocumentCount + " documents in the store, none loaded yet.",
                    StatusLevel.Ok);
            }
            catch (Exception ex)
            {
                this.categoryTree.Nodes.Clear();
                ReportFailure("The category tree could not be loaded.", ex);
            }
        }

        /// <summary>
        /// One node per category. The stable ID goes into <c>Tag</c> and into <c>Name</c> ("cat-7");
        /// the text is for people only. A placeholder child is what makes a node expandable before its
        /// children exist.
        /// </summary>
        private TreeNode CreateCategoryNode(CategoryNode category, bool withPlaceholder)
        {
            var node = new TreeNode(category.Title)
            {
                Name = category.NodeName,
                Tag = category.Id,
                ImageKey = "folder",
                ToolTipText = category.Path + "  ·  category id " + category.Id
            };

            if (withPlaceholder)
                node.Nodes.Add(CreatePlaceholder());

            return node;
        }

        private static TreeNode CreatePlaceholder()
        {
            return new TreeNode("Loading…") { Name = PlaceholderName };
        }

        /// <summary>True while the branch still holds a placeholder, i.e. it has never been loaded successfully.</summary>
        private static bool NeedsLoading(TreeNode node)
        {
            foreach (TreeNode child in node.Nodes)
            {
                if (child.Name == PlaceholderName)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// The lazy-loading hook. The placeholder is the marker: find it, drop it, ask the service for the real
        /// children. Expanding an already-loaded branch costs nothing.
        /// </summary>
        private void categoryTree_AfterExpand(object sender, TreeViewEventArgs e)
        {
            LoadChildren(e.Node);
        }

        private void LoadChildren(TreeNode node)
        {
            if (node == null || !(node.Tag is int) || !NeedsLoading(node))
                return;

            var categoryId = (int)node.Tag;
            _lastAction = () => { node.Expand(); LoadChildren(node); return Task.CompletedTask; };

            ConsoleLog.Add("categoryTree.AfterExpand \"" + node.Text + "\" → placeholder found, DocumentService.GetChildren(" + categoryId + ")");

            try
            {
                var children = _documents.GetChildren(categoryId);

                node.Nodes.Clear();                       // the placeholder goes away with everything else
                foreach (var child in children)
                    node.Nodes.Add(CreateCategoryNode(child, child.HasChildren));

                if (children.Count == 0)
                {
                    ConsoleLog.Add("   \"" + node.Text + "\" has no sub-categories — the node is now a leaf");
                    ConsoleLog.Status("\"" + node.Text + "\" has no sub-categories.", StatusLevel.Warning);
                }
                else
                {
                    ConsoleLog.Status(
                        "Expanded \"" + node.Text + "\" — " + children.Count + " children loaded on demand (nothing else was fetched).",
                        StatusLevel.Ok);
                }
            }
            catch (Exception ex)
            {
                // Leave the branch in a state the user can act on: an honest node plus the placeholder back,
                // so collapsing and expanding again runs AfterExpand and retries.
                node.Nodes.Clear();
                node.Nodes.Add(new TreeNode("Could not load — expand again to retry") { Name = ErrorNodeName, ImageKey = "warning" });
                node.Nodes.Add(CreatePlaceholder());
                node.Collapse();

                ReportFailure("\"" + node.Text + "\" could not be expanded.", ex);
            }
        }

        /// <summary>
        /// Selecting a category resolves the ID from <c>Tag</c> and loads that category's document page.
        /// The placeholder and error nodes carry no <c>Tag</c>, so they are simply not selectable targets.
        /// </summary>
        private async void categoryTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var node = e.Node;
            ConsoleLog.Control(this.categoryTree.Name);

            if (node == null || !(node.Tag is int))
            {
                ConsoleLog.Add("categoryTree.AfterSelect → the node carries no category id (placeholder or error node), nothing to load");
                return;
            }

            await LoadCategoryPage((int)node.Tag, node.Text);
        }

        // ------------------------------------------------------------------------------------------------
        // The list — virtual mode
        // ------------------------------------------------------------------------------------------------

        /// <summary>
        /// Asks the service for the category's page, then tells the ListView how long the list is.
        /// The list is cleared <b>before</b> the call, so a failure can never leave half-built items behind.
        /// </summary>
        private async Task LoadCategoryPage(int categoryId, string categoryTitle)
        {
            _lastAction = () => LoadCategoryPage(categoryId, categoryTitle);

            ConsoleLog.Add("categoryTree.AfterSelect \"" + categoryTitle + "\" → category id " + categoryId + " (from Node.Tag — three folders share that text)");

            ClearList("Loading documents…");
            this.documentList.ShowLoader = true;
            ConsoleLog.Status("Loading the documents of \"" + categoryTitle + "\" …", StatusLevel.Ok);

            try
            {
                var page = await _documents.GetDocumentPageAsync(categoryId);
                ApplyPage(page, categoryTitle);
            }
            catch (Exception ex)
            {
                ClearList("The documents of this category could not be loaded.");
                ReportFailure("The documents of \"" + categoryTitle + "\" could not be loaded.", ex);
            }
            finally
            {
                this.documentList.ShowLoader = false;
                Application.Update(this);   // we are past an await: push the pending changes over the socket
            }
        }

        /// <summary>
        /// Publishes the server-side page to the control: keep the page, reset the counter, set
        /// <c>VirtualListSize</c>. Not one <see cref="ListViewItem"/> is created here — the browser asks for
        /// the rows it can actually show and <see cref="documentList_RetrieveVirtualItem"/> answers.
        /// </summary>
        private void ApplyPage(DocumentPage page, string categoryTitle)
        {
            _page = page;
            _createdItems = 0;

            _suspendSelection = true;
            this.documentList.VirtualListSize = page.Total;
            _suspendSelection = false;

            this.lblEmptyList.Visible = page.IsEmpty;
            this.lblListTitle.Text = "Documents · " + page.CategoryPath;
            UpdateCounters();

            if (page.IsEmpty)
            {
                this.documentDetail.ShowMessage("No documents in this category.");
                ConsoleLog.Add("documentList.VirtualListSize = 0 — empty category, the list shows the empty-state band");
                ConsoleLog.Status("\"" + categoryTitle + "\" holds no documents.", StatusLevel.Warning);
            }
            else
            {
                this.documentDetail.ShowMessage("Pick a document in the list.");
                ConsoleLog.Add("documentList.VirtualListSize = " + page.Total + " — items are created only when the browser asks for them");
                ConsoleLog.Status(page.Total + " documents in " + page.CategoryPath + " (virtual mode).", StatusLevel.Ok);
            }
        }

        /// <summary>
        /// Empties the list the only way virtual mode allows: <c>VirtualListSize = 0</c> and drop the page.
        /// The <c>Items</c> collection is not the store here, so it is never touched.
        /// </summary>
        private void ClearList(string detailMessage)
        {
            _page = null;
            _createdItems = 0;

            _suspendSelection = true;
            this.documentList.VirtualListSize = 0;
            _suspendSelection = false;

            this.lblEmptyList.Visible = false;
            this.lblListTitle.Text = "Documents";
            UpdateCounters();

            this.documentDetail.ShowMessage(detailMessage);
            ConsoleLog.Record(null);
        }

        /// <summary>
        /// The cheap handler. It reads one row out of the page already in memory, sets the stable ID in
        /// <c>Tag</c> and the <c>ImageKey</c> at the same moment, and counts itself — so the header can show
        /// "312 rows · 8 items created".
        /// </summary>
        private void documentList_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            var page = _page;
            if (page == null || e.ItemIndex < 0 || e.ItemIndex >= page.Items.Count)
            {
                e.Item = new ListViewItem(new[] { "…", "", "", "" });
                return;
            }

            var document = page.Items[e.ItemIndex];
            e.Item = new ListViewItem(new[] { document.Name, document.TypeLabel, document.SizeText, document.ModifiedText })
            {
                Name = document.DocumentId,
                Tag = document.Id,
                ImageKey = document.ImageKey,
                ToolTipText = document.DocumentId + "  ·  " + page.CategoryPath
            };

            _createdItems++;
            UpdateCounters();
        }

        /// <summary>
        /// The control announcing which window of rows it is about to show. Here the page is already in memory,
        /// so the handler only reports the range; against a database this is where the next block is fetched.
        /// </summary>
        private void documentList_CacheVirtualItems(object sender, CacheVirtualItemsEventArgs e)
        {
            ConsoleLog.Add("documentList.CacheVirtualItems → rows " + e.StartIndex + ".." + e.EndIndex + " (served from the DocumentPage already in memory)");
        }

        /// <summary>
        /// In virtual mode <c>Items</c> is not the store, so the ID comes from the cached page at
        /// <c>SelectedIndices[0]</c>. Then it is the same three steps as the tree: resolve, ask the service,
        /// hand the model over.
        /// </summary>
        private async void documentList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_suspendSelection)
                return;

            var page = _page;
            if (page == null || this.documentList.SelectedIndices.Count == 0)
                return;

            var index = this.documentList.SelectedIndices[0];
            if (index < 0 || index >= page.Items.Count)
                return;

            var documentId = page.Items[index].Id;
            ConsoleLog.Control(this.documentList.Name);

            await LoadDocument(documentId);
        }

        // ------------------------------------------------------------------------------------------------
        // The detail control — the page is the only thing that talks to both sides
        // ------------------------------------------------------------------------------------------------

        private async Task LoadDocument(int documentId)
        {
            _lastAction = () => LoadDocument(documentId);

            ConsoleLog.Add("documentList.SelectedIndexChanged → document id " + documentId + " (from the cached page + SelectedIndices[0], not from Items)");

            this.documentDetail.ShowMessage("Loading document…");
            this.documentDetail.ShowLoader = true;

            try
            {
                var model = await _documents.GetDocumentAsync(documentId);

                this.documentDetail.Show(model);
                ConsoleLog.Record(model.DocumentId);
                ConsoleLog.Status(
                    model.DocumentId + " · " + model.CategoryPath + " · " + model.StatusText,
                    model.IsWarning ? StatusLevel.Warning : StatusLevel.Ok);
            }
            catch (Exception ex)
            {
                this.documentDetail.ShowMessage("This document could not be opened. Pick it again, or use Retry.");
                ConsoleLog.Record(null);
                ReportFailure("The document could not be opened.", ex);
            }
            finally
            {
                this.documentDetail.ShowLoader = false;
                Application.Update(this);
            }
        }

        // ------------------------------------------------------------------------------------------------
        // Command row — success, progress, failure, recovery
        // ------------------------------------------------------------------------------------------------

        private void btnReloadTree_Click(object sender, EventArgs e)
        {
            ConsoleLog.Control(this.btnReloadTree.Name);
            LoadTopCategories();
        }

        /// <summary>
        /// The progress path: a <see cref="Timer"/> expands one top-level node per tick, and every expand runs
        /// <see cref="categoryTree_AfterExpand"/>, so the branches arrive one after the other instead of at once.
        /// </summary>
        private void btnExpandTopLevel_Click(object sender, EventArgs e)
        {
            ConsoleLog.Control(this.btnExpandTopLevel.Name);

            _expandQueue.Clear();
            foreach (TreeNode node in this.categoryTree.Nodes)
                _expandQueue.Add(node);

            if (_expandQueue.Count == 0)
            {
                ConsoleLog.Status("Nothing to expand — reload the tree first.", StatusLevel.Warning);
                return;
            }

            ConsoleLog.Add("btnExpandTopLevel → " + _expandQueue.Count + " nodes, one every " + this.expandTimer.Interval + " ms; each expand lazy-loads its branch");
            ConsoleLog.Status("Expanding the top level, one category at a time …", StatusLevel.Ok);

            this.btnExpandTopLevel.Enabled = false;
            this.expandTimer.Start();
        }

        private void expandTimer_Tick(object sender, EventArgs e)
        {
            if (_expandQueue.Count == 0)
            {
                this.expandTimer.Stop();
                this.btnExpandTopLevel.Enabled = true;
                ConsoleLog.Add("expandTimer → queue empty, stopped");
                ConsoleLog.Status("Every top-level category is expanded; each branch was fetched when it opened.", StatusLevel.Ok);
                return;
            }

            var node = _expandQueue[0];
            _expandQueue.RemoveAt(0);

            node.Expand();        // fires AfterExpand → LoadChildren
            LoadChildren(node);   // safety net: a no-op once the placeholder is gone
        }

        private void chkSimulateFailure_CheckedChanged(object sender, EventArgs e)
        {
            _documents.SimulateFailure = this.chkSimulateFailure.Checked;

            if (_syncingCheckBox)
                return;

            ConsoleLog.Control(this.chkSimulateFailure.Name);
            ConsoleLog.Add(this.chkSimulateFailure.Checked
                ? "DocumentService.SimulateFailure = true — the next expand, page load or document load throws"
                : "DocumentService.SimulateFailure = false — the service answers normally again");
        }

        /// <summary>The recovery path: run the last thing that was attempted, whatever it was.</summary>
        private async void btnRetry_Click(object sender, EventArgs e)
        {
            ConsoleLog.Control(this.btnRetry.Name);

            if (_lastAction == null)
            {
                ConsoleLog.Status("Nothing to retry yet — select a category first.", StatusLevel.Warning);
                return;
            }

            ConsoleLog.Add("btnRetry → running the last action again");
            await _lastAction();
            Application.Update(this);
        }

        // ------------------------------------------------------------------------------------------------
        // Feedback
        // ------------------------------------------------------------------------------------------------

        private void UpdateCounters()
        {
            var rows = _page == null ? "—" : _page.Total.ToString(CultureInfo.InvariantCulture);

            this.lblCounters.Text = rows + " rows · " + _createdItems + " items created";
            this.txtCreatedItems.Text = _createdItems.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// One place for every failure: plain words for the user, the exception type and message for the
        /// Event log only, and the one-shot simulation switch reset so <b>Retry</b> is a real recovery.
        /// </summary>
        private void ReportFailure(string friendly, Exception ex)
        {
            ConsoleLog.Add("✗ " + ex.GetType().Name + " — " + ex.Message);
            ConsoleLog.Status(friendly + " Nothing was left half-built.", StatusLevel.Error);

            AlertBox.Show(
                friendly + " Please try again — Retry repeats the last action.",
                MessageBoxIcon.Error, alignment: ContentAlignment.TopRight, autoCloseDelay: 5000);

            if (!_documents.SimulateFailure)
                return;

            _syncingCheckBox = true;
            _documents.SimulateFailure = false;
            this.chkSimulateFailure.Checked = false;
            _syncingCheckBox = false;

            ConsoleLog.Add("DocumentService.SimulateFailure = false (one-shot) — Retry will now succeed");
        }
    }
}
