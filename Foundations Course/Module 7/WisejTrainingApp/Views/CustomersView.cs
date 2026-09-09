using System.Linq;
using Wisej.Web;

namespace WisejTrainingApp.Views
{
    /// <summary>
    /// Customers page — one card, the same spacing rules as the Dashboard (32 px padding, 524 px card at 32,100).
    /// The list is derived from the tickets in TicketService; nothing is stored here.
    /// </summary>
    public partial class CustomersView : UserControl, IAppView
    {
        private readonly IAppShell shell;

        public CustomersView(IAppShell shell)
        {
            this.shell = shell;
            InitializeComponent();
        }

        public void RefreshView()
        {
            lstCustomers.Items.Clear();

            var groups = shell.Tickets.GetTickets()
                .GroupBy(t => t.Customer)
                .OrderBy(g => g.Key)
                .ToList();

            foreach (var group in groups)
            {
                int open = group.Count(t => t.Status != "Closed");
                lstCustomers.Items.Add($"{group.Key,-24} {group.Count(),2} ticket(s)   {open,2} open");
            }

            lblCustomersSummary.Text = $"{groups.Count} customers · {shell.Tickets.GetTickets().Count} tickets (read from TicketService.GetTickets)";
        }
    }
}
