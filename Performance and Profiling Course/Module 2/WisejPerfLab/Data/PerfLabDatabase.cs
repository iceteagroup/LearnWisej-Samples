using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WisejPerfLab.Data
{
    /// <summary>
    /// Creates and seeds the lab database before the first session starts, so no measurement ever pays
    /// for the first-run setup. The row counts it returns are the dataset line of the baseline.
    /// </summary>
    public static class PerfLabDatabase
    {
        /// <summary>The dataset the scenarios ran against. Printed at start and shown in the app header.</summary>
        public static (int Tickets, int Customers) Dataset { get; private set; }

        public static async Task EnsureReadyAsync(IServiceProvider services)
        {
            var configuration = services.GetRequiredService<IConfiguration>();
            var ticketCount = configuration.GetValue("PerfLab:TicketCount", 50000);
            var customerNodeCount = configuration.GetValue("PerfLab:CustomerNodeCount", 3200);

            var factory = services.GetRequiredService<IDbContextFactory<PerfLabContext>>();
            await using var db = await factory.CreateDbContextAsync();

            await db.Database.EnsureCreatedAsync();
            Dataset = DatasetSeeder.EnsureSeeded(db, ticketCount, customerNodeCount);

            Console.Error.WriteLine(
                $"[WisejPerfLab] dataset ready: {Dataset.Tickets:N0} tickets, {Dataset.Customers:N0} customer nodes");
        }
    }
}
