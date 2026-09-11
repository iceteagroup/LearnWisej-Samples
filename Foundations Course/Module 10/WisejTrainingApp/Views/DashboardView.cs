using Wisej.Web;
using WisejTrainingApp.Services;

namespace WisejTrainingApp.Views
{
    public partial class DashboardView : UserControl
    {
        private readonly TicketService ticketService;

        public DashboardView(TicketService ticketService)
        {
            InitializeComponent();
            this.ticketService = ticketService;
        }

        public void RefreshCards()
        {
            TicketSummary summary = ticketService.GetSummary();
            lblTotal.Text = summary.Total.ToString();
            lblOpen.Text = summary.Open.ToString();
            lblInProgress.Text = summary.InProgress.ToString();
            lblClosed.Text = summary.Closed.ToString();
        }
    }
}
