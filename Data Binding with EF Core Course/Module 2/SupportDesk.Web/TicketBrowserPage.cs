using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

        [Inject]
        private SchemaInfoService SchemaInfo { get; set; }

        [Inject]
        private DevelopmentSeeder Seeder { get; set; }

        [Inject]
        private ModelDemoService ModelDemos { get; set; }

        // The loading guard: one database operation per page at a time.
        private bool _loading;

        public TicketBrowserPage()
        {
            InitializeComponent();
        }

        private async void TicketBrowserPage_Load(object sender, EventArgs e)
        {
            await RefreshCountsAsync();
            Application.Update(this);
        }

        private async void countButton_Click(object sender, EventArgs e)
        {
            await RunAsync(
                "Counting tickets...",
                async () => $"{await TicketQueries.CountTicketsAsync()} tickets in the Support Desk database",
                "Tickets could not be counted. Please try again in a moment.",
                "Tickets could not be counted. Please try again in a moment.");
        }

        private async void btnSeed_Click(object sender, EventArgs e)
        {
            await RunAsync(
                "Seeding development data...",
                async () => (await Seeder.SeedDevelopmentDataAsync()).Describe(),
                "The development data could not be saved because the database rejected the change.",
                "The development data could not be seeded. Please try again in a moment.");
        }

        private async void buttonOverlongTitle_Click(object sender, EventArgs e)
        {
            await RunAsync(
                "Saving a ticket with a 200-character title...",
                async () =>
                {
                    await ModelDemos.SaveOverlongTitleAsync();
                    return "Ticket saved";
                },
                "The ticket could not be saved because the database rejected the change.",
                "The ticket could not be saved. Please try again in a moment.");
        }

        private async void buttonDeleteCustomer_Click(object sender, EventArgs e)
        {
            await RunAsync(
                "Deleting a customer that still has tickets...",
                async () =>
                {
                    await ModelDemos.DeleteCustomerWithTicketsAsync();
                    return "Customer deleted";
                },
                "This customer still has tickets and cannot be deleted.",
                "The customer could not be deleted. Please try again in a moment.");
        }

        /// <summary>
        /// Guard → busy UI → one awaited service call → result; a friendly message in catch, the UI restored in finally.
        /// </summary>
        private async Task RunAsync(string busyStatus, Func<Task<string>> operation, string dbUpdateMessage, string genericMessage)
        {
            if (_loading)
                return;

            try
            {
                _loading = true;
                SetBusy(true);
                statusLabel.Text = busyStatus;

                statusLabel.Text = await operation();
            }
            catch (DbUpdateException ex)
            {
                ShowError(dbUpdateMessage, ex);
            }
            catch (Exception ex)
            {
                ShowError(genericMessage, ex);
            }
            finally
            {
                _loading = false;
                SetBusy(false);
                await RefreshCountsAsync();
                Application.Update(this);
            }
        }

        private async Task RefreshCountsAsync()
        {
            try
            {
                var info = await SchemaInfo.DescribeAsync();
                var c = info.Counts;
                labelCounts.Text =
                    $"Customers {c.Customers} · Agents {c.Agents} · Categories {c.Categories} · Tickets {c.Tickets} · " +
                    $"Pending migrations {info.PendingMigrations.Count}";
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("[SupportDesk] Counts not refreshed: " + ex);
                labelCounts.Text = "Counts unavailable";
            }
        }

        private void ShowError(string friendlyMessage, Exception ex)
        {
            Console.Error.WriteLine("[SupportDesk] " + ex);
            AlertBox.Show(friendlyMessage, MessageBoxIcon.Error, alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
            statusLabel.Text = "Nothing was changed";
        }

        private void SetBusy(bool busy)
        {
            countButton.Enabled = !busy;
            btnSeed.Enabled = !busy;
            buttonOverlongTitle.Enabled = !busy;
            buttonDeleteCustomer.Enabled = !busy;
        }
    }
}
