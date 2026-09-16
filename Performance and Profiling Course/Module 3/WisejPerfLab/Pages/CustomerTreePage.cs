using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Built the way trees usually are the first time: load the whole table, ask for every count, create
    /// every node, and let the control worry about it. Module 5 measures what that costs on the server and
    /// in the update payload, and replaces it with children loaded on expand.
    /// </remarks>
    public partial class CustomerTreePage : UserControl, IScenarioPage
    {
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

        /// <summary>The shell measures the expand, loading the tree first if it is not there yet.</summary>
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
                CustomerTreeResult result;
                using (scope.Stage("load every node + per-node count query"))
                    result = _customers.LoadAll();

                scope.Rows = result.Nodes.Count;

                var byParent = result.Nodes
                    .GroupBy(n => n.ParentId ?? 0)
                    .ToDictionary(g => g.Key, g => g.ToList());

                using (scope.Stage("build 3,200 TreeNode objects"))
                {
                    treeView1.BeginUpdate();
                    try
                    {
                        treeView1.Nodes.Clear();

                        // Every node, all the way down, before the user has opened anything.
                        var roots = byParent.TryGetValue(0, out var list) ? list : new List<CustomerNodeRow>();
                        foreach (var root in roots)
                            treeView1.Nodes.Add(BuildNode(root, byParent));
                    }
                    finally
                    {
                        treeView1.EndUpdate();
                    }
                }

                _loaded = true;
                var elapsed = scope.ElapsedMs;
                lblTreeStatus.Text =
                    $"{result.Nodes.Count:N0} nodes built   {result.QueryCount:N0} SQL statements   " +
                    PerfBudget.Describe(Scenario, "LoadTree", elapsed);
                _shell.SetStatus(PerfBudget.StateFor(Scenario, "LoadTree", elapsed),
                    $"Customers/LoadTree {PerfBudget.Describe(Scenario, "LoadTree", elapsed)} — every node built up front");
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

        private static TreeNode BuildNode(CustomerNodeRow row, IReadOnlyDictionary<int, List<CustomerNodeRow>> byParent)
        {
            var node = new TreeNode(row.Label) { Tag = row.Id };

            if (byParent.TryGetValue(row.Id, out var children))
                foreach (var child in children)
                    node.Nodes.Add(BuildNode(child, byParent));

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
        /// The expand handler. Every node is already built, so all this does is read the branch again —
        /// by loading the whole customer table and counting every node in it a second time.
        /// </summary>
        private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node?.Tag is not int customerId)
                return;

            using var scope = _probe.Measure(Scenario, UserAction);

            try
            {
                var children = _customers.GetChildren(customerId);
                scope.Rows = children.Nodes.Count;

                var elapsed = scope.ElapsedMs;
                lblTreeStatus.Text =
                    $"expanded {e.Node.Text}: {children.Nodes.Count:N0} children   {children.QueryCount:N0} SQL statements   " +
                    PerfBudget.Describe(Scenario, UserAction, elapsed);
                _shell.SetStatus(PerfBudget.StateFor(Scenario, UserAction, elapsed),
                    $"Customers/ExpandNode {PerfBudget.Describe(Scenario, UserAction, elapsed)} — {children.Nodes.Count:N0} children");
            }
            catch (DatabaseUnavailableException ex)
            {
                // A branch whose children cannot be loaded says so on the node instead of collapsing silently.
                scope.Fail(ex);
                e.Cancel = true;
                e.Node.Text = e.Node.Text + "  (children unavailable)";
                lblTreeStatus.Text = "expand failed after " + scope.ElapsedMs + " ms";
                _shell.SetStatus(ShellState.Fault, "Customers/ExpandNode failed — the ticket database is unreachable");
                _shell.ShowBanner("This branch could not be expanded: the ticket database is unreachable.");
            }
        }

        #endregion
    }
}
