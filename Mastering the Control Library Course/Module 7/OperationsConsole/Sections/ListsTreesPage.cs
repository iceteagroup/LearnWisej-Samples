using System;
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
    /// <b>Lists and Trees</b> — the document explorer. <c>categoryTree</c> loads only the top level and fetches each
    /// branch in <see cref="categoryTree_AfterExpand"/>; <c>documentList</c> is a virtual-mode <see cref="ListView"/>
    /// fed from a server-side <see cref="DocumentPage"/>; <c>documentDetail</c> receives a finished
    /// <see cref="DocumentModel"/>. Every node and item carries its stable ID in <c>Tag</c>; no handler keys on text.
    /// </summary>
    public partial class ListsTreesPage : UserControl, ISection
    {
        /// <summary>The marker that says "this branch has not been loaded yet".</summary>
        private const string PlaceholderName = "placeholder";

        /// <summary>The marker of the node that reports a failed expand.</summary>
        private const string ErrorNodeName = "load-error";

        private readonly DocumentService _documents = new DocumentService();

        /// <summary>The server-side page the virtual ListView reads from.</summary>
        private DocumentPage _page;

        /// <summary>How many <see cref="ListViewItem"/> objects <c>RetrieveVirtualItem</c> has built for the page.</summary>
        private int _createdItems;

        /// <summary>Set while the page rewrites <c>VirtualListSize</c>, so the resulting selection change is ignored.</summary>
        private bool _suspendSelection;

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
            LoadTopCategories();
        }

        // ------------------------------------------------------------------------------------------------
        // Icons — one ImageList shared by the tree and the list
        // ------------------------------------------------------------------------------------------------

        /// <summary>
        /// <c>ImageList.Images.Add(key, nameOrUrl)</c>: static files are served from the project folder, so
        /// <c>"wwwroot/icons/folder.svg"</c> is fetched as <c>/wwwroot/icons/folder.svg</c>.
        /// </summary>
        private void RegisterIcons()
        {
            this.imageList.Images.Add("folder", "wwwroot/icons/folder.svg");
            this.imageList.Images.Add("contract", "wwwroot/icons/contract.svg");
            this.imageList.Images.Add("invoice", "wwwroot/icons/invoice.svg");
            this.imageList.Images.Add("drawing", "wwwroot/icons/drawing.svg");
            this.imageList.Images.Add("warning", "wwwroot/icons/warning.svg");
        }

        // ------------------------------------------------------------------------------------------------
        // The tree — lazy loading
        // ------------------------------------------------------------------------------------------------

        /// <summary>Builds the top level only; every node gets one placeholder child.</summary>
        private void LoadTopCategories()
        {
            try
            {
                var categories = _documents.GetTopCategories();

                this.categoryTree.BeginUpdate();
                this.categoryTree.Nodes.Clear();
                foreach (var category in categories)
                    this.categoryTree.Nodes.Add(CreateCategoryNode(category, withPlaceholder: true));
                this.categoryTree.EndUpdate();

                ClearList("Select a category on the left, then a document.");
                ShellStatus.Show("Select a category to list its documents.", StatusLevel.Ok);
            }
            catch (Exception)
            {
                this.categoryTree.Nodes.Clear();
                ReportFailure("The category tree could not be loaded.");
            }
        }

        /// <summary>One node per category: the stable ID goes into <c>Tag</c> and <c>Name</c> ("cat-7").</summary>
        private TreeNode CreateCategoryNode(CategoryNode category, bool withPlaceholder)
        {
            var node = new TreeNode(category.Title)
            {
                Name = category.NodeName,
                Tag = category.Id,
                ImageKey = "folder"
            };

            if (withPlaceholder)
                node.Nodes.Add(CreatePlaceholder());

            return node;
        }

        private static TreeNode CreatePlaceholder()
        {
            return new TreeNode("Loading…") { Name = PlaceholderName };
        }

        /// <summary>True while the branch still holds a placeholder, i.e. it has never been loaded.</summary>
        private static bool NeedsLoading(TreeNode node)
        {
            foreach (TreeNode child in node.Nodes)
            {
                if (child.Name == PlaceholderName)
                    return true;
            }
            return false;
        }

        private void categoryTree_AfterExpand(object sender, TreeViewEventArgs e)
        {
            LoadChildren(e.Node);
        }

        /// <summary>Replaces the placeholder with the real children the first time the node opens.</summary>
        private void LoadChildren(TreeNode node)
        {
            if (node == null || !(node.Tag is int) || !NeedsLoading(node))
                return;

            var categoryId = (int)node.Tag;

            try
            {
                var children = _documents.GetChildren(categoryId);

                node.Nodes.Clear();
                foreach (var child in children)
                    node.Nodes.Add(CreateCategoryNode(child, child.HasChildren));

                if (children.Count == 0)
                    ShellStatus.Show("\"" + node.Text + "\" has no sub-categories.", StatusLevel.Warning);
                else
                    ShellStatus.Show("Expanded \"" + node.Text + "\".", StatusLevel.Ok);
            }
            catch (Exception)
            {
                // an honest node plus the placeholder back, so expanding again retries
                node.Nodes.Clear();
                node.Nodes.Add(new TreeNode("Could not load — expand again to retry") { Name = ErrorNodeName, ImageKey = "warning" });
                node.Nodes.Add(CreatePlaceholder());
                node.Collapse();

                ReportFailure("\"" + node.Text + "\" could not be expanded.");
            }
        }

        /// <summary>Selecting a category resolves the ID from <c>Tag</c> and loads that category's page.</summary>
        private async void categoryTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var node = e.Node;
            ShellStatus.Control(this.categoryTree.Name);

            if (node == null || !(node.Tag is int))
                return;

            await LoadCategoryPage((int)node.Tag, node.Text);
        }

        // ------------------------------------------------------------------------------------------------
        // The list — virtual mode
        // ------------------------------------------------------------------------------------------------

        /// <summary>
        /// Asks the service for the category's page, then sets the list length. The list is cleared before the call,
        /// so a failure never leaves half-built items behind.
        /// </summary>
        private async Task LoadCategoryPage(int categoryId, string categoryTitle)
        {
            ClearList("Loading documents…");
            this.documentList.ShowLoader = true;
            ShellStatus.Show("Loading the documents of \"" + categoryTitle + "\" …", StatusLevel.Ok);

            try
            {
                var page = await _documents.GetDocumentPageAsync(categoryId);
                ApplyPage(page, categoryTitle);
            }
            catch (Exception)
            {
                ClearList("The documents of this category could not be loaded.");
                ReportFailure("The documents of \"" + categoryTitle + "\" could not be loaded.");
            }
            finally
            {
                this.documentList.ShowLoader = false;
                Application.Update(this);
            }
        }

        /// <summary>Keeps the page and sets <c>VirtualListSize</c>; no item is created here.</summary>
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
                ShellStatus.Show("\"" + categoryTitle + "\" holds no documents.", StatusLevel.Warning);
            }
            else
            {
                this.documentDetail.ShowMessage("Pick a document in the list.");
                ShellStatus.Show(page.Total + " documents in " + page.CategoryPath + ".", StatusLevel.Ok);
            }
        }

        /// <summary>Empties the list the way virtual mode allows: <c>VirtualListSize = 0</c> and drop the page.</summary>
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
            ShellStatus.Record(null);
        }

        /// <summary>Builds one item from the page in memory, with the stable ID in <c>Tag</c> and its <c>ImageKey</c>.</summary>
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
                ImageKey = document.ImageKey
            };

            _createdItems++;
            UpdateCounters();
        }

        /// <summary>In virtual mode the ID comes from the cached page at <c>SelectedIndices[0]</c>.</summary>
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

            ShellStatus.Control(this.documentList.Name);
            await LoadDocument(page.Items[index].Id);
        }

        // ------------------------------------------------------------------------------------------------
        // The detail control
        // ------------------------------------------------------------------------------------------------

        private async Task LoadDocument(int documentId)
        {
            this.documentDetail.ShowMessage("Loading document…");
            this.documentDetail.ShowLoader = true;

            try
            {
                var model = await _documents.GetDocumentAsync(documentId);

                this.documentDetail.Show(model);
                ShellStatus.Record(model.DocumentId);
                ShellStatus.Show(
                    model.DocumentId + " · " + model.CategoryPath + " · " + model.StatusText,
                    model.IsWarning ? StatusLevel.Warning : StatusLevel.Ok);
            }
            catch (Exception)
            {
                this.documentDetail.ShowMessage("This document could not be opened. Pick it again to retry.");
                ShellStatus.Record(null);
                ReportFailure("The document could not be opened.");
            }
            finally
            {
                this.documentDetail.ShowLoader = false;
                Application.Update(this);
            }
        }

        private void chkSimulateFailure_CheckedChanged(object sender, EventArgs e)
        {
            _documents.SimulateFailure = this.chkSimulateFailure.Checked;
        }

        // ------------------------------------------------------------------------------------------------
        // Feedback
        // ------------------------------------------------------------------------------------------------

        private void UpdateCounters()
        {
            this.lblCounters.Text = _page == null
                ? ""
                : _createdItems.ToString(CultureInfo.InvariantCulture) + " of " + _page.Total.ToString(CultureInfo.InvariantCulture) + " items created";
        }

        /// <summary>Plain words for the user; no exception text.</summary>
        private void ReportFailure(string friendly)
        {
            ShellStatus.Show(friendly, StatusLevel.Error);
            AlertBox.Show(friendly + " Please try again.",
                MessageBoxIcon.Error, alignment: ContentAlignment.TopRight, autoCloseDelay: 5000);
        }
    }
}
