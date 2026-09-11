using System;
using SupportDesk.Services;
using Wisej.Services;
using Wisej.Web;

namespace SupportDesk.Web
{
    public partial class TicketBrowserPage : Page
    {
        // Resolved through Microsoft DI: Startup.cs registered app.Services with Wisej.NET.
        [Inject]
        private TicketQueryService TicketQueries { get; set; }

        // The loading guard: one database operation per page at a time.
        private bool _loading;

        public TicketBrowserPage()
        {
            InitializeComponent();
        }

        private async void countButton_Click(object sender, EventArgs e)
        {
            if (_loading)
                return;

            try
            {
                _loading = true;
                countButton.Enabled = false;
                statusLabel.Text = "Counting tickets...";

                var count = await TicketQueries.CountTicketsAsync();

                statusLabel.Text = $"{count} tickets in the Support Desk database";
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("[SupportDesk] Ticket count failed: " + ex);
                AlertBox.Show(
                    "Tickets could not be counted. The Support Desk database could not be reached. Please try again or contact support.",
                    MessageBoxIcon.Error, alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
                statusLabel.Text = "Count failed";
            }
            finally
            {
                countButton.Enabled = true;
                _loading = false;
                Application.Update(this);
            }
        }
    }
}
