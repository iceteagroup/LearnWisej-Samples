using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SupportDesk.Data;

namespace SupportDesk.Web
{
    /// <summary>
    /// Development-only startup helper. Module 1 has no migrations yet, so it calls EnsureCreated on the
    /// local SQLite file and drops in a dozen sample tickets so the first count is not zero; Module 2
    /// replaces this with the migrations workflow and the real seed. It runs once, at host start, with
    /// one context from the factory — it is not session code.
    /// </summary>
    internal static class SupportDeskDevelopmentDatabase
    {
        private static readonly string[] SampleTitles =
        {
            "Printer offline on floor 2", "VPN drops every hour", "Password reset for finance",
            "Laptop battery swells", "Outlook rules vanished", "Badge reader ignores new hires",
            "Shared drive read-only", "Monitor flickers after sleep", "Guest Wi-Fi captive page loops",
            "Invoice export times out", "Two-factor codes arrive late", "Conference room screen no signal"
        };

        public static async Task EnsureReadyAsync(IServiceProvider services)
        {
            var factory = services.GetRequiredService<IDbContextFactory<SupportDeskContext>>();
            await using var db = await factory.CreateDbContextAsync();
            await db.Database.EnsureCreatedAsync();

            if (await db.Tickets.AnyAsync())
                return;

            db.Tickets.AddRange(SampleTitles.Select((title, i) => new Ticket
            {
                Number = $"T-{1001 + i}",
                Title = title,
                Status = i % 4 == 0 ? "Closed" : "Open",
                CreatedAt = DateTime.UtcNow.AddDays(-i)
            }));
            await db.SaveChangesAsync();
        }
    }
}
