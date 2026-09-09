using System.Linq;
using Wisej.Web;

namespace WisejTrainingApp.Views
{
    /// <summary>
    /// Reports page — two cards side by side (524 wide, 24 px apart) with the same spacing rules as the Dashboard.
    /// The numbers come from TicketService every time the page is shown.
    /// </summary>
    public partial class ReportsView : UserControl, IAppView
    {
        private readonly IAppShell shell;

        public ReportsView(IAppShell shell)
        {
            this.shell = shell;
            InitializeComponent();
        }

        public void RefreshView()
        {
            var tickets = shell.Tickets.GetTickets();

            lstByStatus.Items.Clear();
            foreach (string status in new[] { "Open", "In Progress", "Closed" })
                lstByStatus.Items.Add($"{status,-14} {tickets.Count(t => t.Status == status),3}   {Bar(tickets.Count(t => t.Status == status))}");

            lstByPriority.Items.Clear();
            foreach (string priority in new[] { "High", "Medium", "Low" })
                lstByPriority.Items.Add($"{priority,-14} {tickets.Count(t => t.Priority == priority),3}   {Bar(tickets.Count(t => t.Priority == priority))}");

            lblReportsSummary.Text = $"{tickets.Count} tickets in total · computed from TicketService.GetTickets() on every visit";
        }

        private static string Bar(int count)
        {
            return new string('█', count);
        }
    }
}
