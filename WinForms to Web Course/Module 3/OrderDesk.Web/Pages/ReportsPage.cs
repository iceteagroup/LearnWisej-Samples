using System;
using OrderDesk.Shared;
using Wisej.Web;

namespace OrderDesk.Pages
{
    /// <summary>
    /// The third navigation target: a placeholder list of the reports the desktop app printed or
    /// exported. Selecting one only logs it — report generation is Module 6 (files, reports and
    /// browser boundaries). The page exists so navigation (nav, View menu, Reports menu) is complete.
    /// </summary>
    public partial class ReportsPage : ModulePage
    {
        public ReportsPage()
        {
            InitializeComponent();
        }

        public override string Title => "Reports";

        /// <summary>Selects a report by name (used by the Reports menu).</summary>
        public void Select(string name)
        {
            for (int i = 0; i < reportsList.Items.Count; i++)
            {
                if (string.Equals(reportsList.Items[i] as string, name, StringComparison.OrdinalIgnoreCase))
                {
                    reportsList.SelectedIndex = i;
                    return;
                }
            }
        }

        private void reportsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var name = reportsList.SelectedItem as string;
            if (name == null) return;
            Log(TraceKind.ClientToServer, "reportsList.SelectedIndexChanged", "\"" + name + "\"");
            Log(TraceKind.Server, "ReportsPage", "placeholder — generation is Module 6 (PDF / .xlsx / queue)");
        }
    }
}
