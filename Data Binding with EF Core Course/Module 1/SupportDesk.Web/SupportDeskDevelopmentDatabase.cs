using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SupportDesk.Data;

namespace SupportDesk.Web
{
    /// <summary>
    /// Development-only startup helper. Module 1 has no migrations yet, so it calls EnsureCreated on the
    /// local SQLite file; Module 2 replaces this with the migrations workflow. It runs once, at host start,
    /// with one context from the factory — it is not session code.
    /// </summary>
    internal static class SupportDeskDevelopmentDatabase
    {
        public static async Task EnsureReadyAsync(IServiceProvider services)
        {
            var factory = services.GetRequiredService<IDbContextFactory<SupportDeskContext>>();
            await using var db = await factory.CreateDbContextAsync();
            await db.Database.EnsureCreatedAsync();
        }
    }
}
