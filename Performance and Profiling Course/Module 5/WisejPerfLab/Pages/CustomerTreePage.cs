using System;
using WisejPerfLab.Data;
using WisejPerfLab.Diagnostics;
using WisejPerfLab.Models;
using WisejPerfLab.Services;
using WisejPerfLab.Shell;
using Wisej.Web;

namespace WisejPerfLab.Pages
{
    /// <summary>
    /// The customer hierarchy: regions, accounts and sites, each labelled with its ticket count.
    /// </summary>
    /// <remarks>
    /// Module 5 rewrote this screen. The tree now loads the eight regions and stops. A branch that has
    /// children gets one placeholder node so the expander appears; opening the branch replaces the
    /// placeholder with the real children, fetched then and not before. Nodes the user never opens cost
    /// no query, no control and no payload.
    /// </remarks>
    public partial class CustomerTreePage : UserControl, IScenarioPage
    {
        /// <summary>Marks a branch whose children have not been loaded yet.</summary>
        private const string PlaceholderTag = "placeholder";

        private readonly IPerfLabShell _shell;
        private readonly ScenarioProbe _probe;
        private readonly CustomerTreeService _customers;

        private bool _loaded;

        public CustomerTreePage(IPerfLabShell shell)
        {
            InitializeComponent();

            _shell = shell;
            _probe = PerfLabServices.Get<ScenarioProbe>();
            _customers = PerfLabServices.Get<CustomerTreeService>();
        }

        public string Scenario => "Customers";

        public string UserAction => "ExpandNode";

        public void RunScenario()
        {
            if (!_loaded)
                LoadTree();

            ExpandFirstBranch();
        }

        #region Customers/LoadTree

        private void btnLoadTree_Click(object sender, EventArgs e) => LoadTree();

        private void LoadTree()
        {
            using var scope = _probe.Measure(Scenario, "LoadTree");

            btnLoadTree.Enabled = false;
            _shell.ClearBanner();

            try
            {
                CustomerTreeResult roots;
                using (scope.Stage("load the root level (2 grouped queries)"))
                    roots = _customers.GetRoots();

                scope.Rows = roots.Nodes.Count;

                using (scope.Stage("build the root nodes"))
                {
                    treeView1.BeginUpdate();
                    try
                    {
                        treeView1.Nodes.Clear();
                        foreach (var root in roots.Nodes)
                            treeView1.Nodes.Add(BuildNode(root));
                    }
                    finally
                    {
                        treeView1.EndUpdate();
                    }
                }

                _loaded = true;
                var elapsed = scope.ElapsedMs;
                lblTreeStatus.Text =
                    $"{roots.Nodes.Count:N0} root nodes   {roots.QueryCount:N0} SQL statements   " +
                    PerfBudget.Describe(Scenario, "LoadTree", elapsed);
                _shell.SetStatus(PerfBudget.StateFor(Scenario, "LoadTree", elapsed),
                    $"Customers/LoadTree {PerfBudget.Describe(Scenario, "LoadTree", elapsed)} — roots only, children on demand");
            }
            catch (DatabaseUnavailableException ex)
            {
                scope.Fail(ex);
                treeView1.Nodes.Clear();
                lblTreeStatus.Text = "tree load failed after " + scope.ElapsedMs + " ms";
                _shell.SetStatus(ShellState.Fault, "Customers/LoadTree failed — the ticket database is unreachable");
                _shell.ShowBanner("The customer tree could not be loaded: the ticket database is unreachable.");
            }
            finally
            {
                btnLoadTree.Enabled = true;
            }
        }

        /// <summary>
        /// One node, plus a placeholder child when the node has children. The placeholder is what makes
        /// the expander appear without loading anything, and the count in the label is what makes a
        /// collapsed branch informative — the two halves of a lazy tree that still reads like a tree.
        /// </summary>
        private static TreeNode BuildNode(CustomerNodeRow row)
        {
            var node = new TreeNode(row.Label) { Tag = row.Id };

            if (row.ChildCount > 0)
                node.Nodes.Add(new TreeNode($"loading {row.ChildCount:N0} …") { Tag = PlaceholderTag });

            return node;
        }

        #endregion

        #region Customers/ExpandNode

        private void btnExpandFirst_Click(object sender, EventArgs e) => ExpandFirstBranch();

        private void ExpandFirstBranch()
        {
            if (treeView1.Nodes.Count == 0)
            {
                _shell.SetStatus(ShellState.Idle, "load the tree first — there is nothing to expand");
                return;
            }

            var first = treeView1.Nodes[0];
            first.Collapse();
            first.Expand();
        }

        /// <summary>
        /// Loads the children of the branch being opened, once. A branch that has already been loaded
        /// expands with no query at all.
        /// </summary>
        private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node?.Tag is not int customerId)
                return;

            if (!HasPlaceholder(e.Node))
                return;      // already loaded: expanding again costs nothing

            using var scope = _probe.Measure(Scenario, UserAction);

            try
            {
                CustomerTreeResult children;
                using (scope.Stage("load one level"))
                    children = _customers.GetChildren(customerId);

                scope.Rows = children.Nodes.Count;

                using (scope.Stage("build the child nodes"))
                {
                    e.Node.Nodes.Clear();
                    foreach (var child in children.Nodes)
                        e.Node.Nodes.Add(BuildNode(child));
                }

                var elapsed = scope.ElapsedMs;
                lblTreeStatus.Text =
                    $"expanded {e.Node.Text}: {children.Nodes.Count:N0} children   {children.QueryCount:N0} SQL statements   " +
                    PerfBudget.Describe(Scenario, UserAction, elapsed);
                _shell.SetStatus(PerfBudget.StateFor(Scenario, UserAction, elapsed),
                    $"Customers/ExpandNode {PerfBudget.Describe(Scenario, UserAction, elapsed)} — {children.Nodes.Count:N0} children loaded on demand");
            }
            catch (DatabaseUnavailableException ex)
            {
                // A branch whose children cannot be loaded says so on the node and keeps its placeholder,
                // so the next attempt tries again instead of showing an empty branch as if it were empty.
                scope.Fail(ex);
                e.Cancel = true;
                e.Node.Text = e.Node.Text + "  (children unavailable)";
                lblTreeStatus.Text = "expand failed after " + scope.ElapsedMs + " ms";
                _shell.SetStatus(ShellState.Fault, "Customers/ExpandNode failed — the ticket database is unreachable");
                _shell.ShowBanner("This branch could not be expanded: the ticket database is unreachable.");
            }
        }

        private static bool HasPlaceholder(TreeNode node)
            => node.Nodes.Count == 1 && (node.Nodes[0].Tag as string) == PlaceholderTag;

        #endregion
    }
}
