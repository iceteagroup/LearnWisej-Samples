using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SupportDesk.Data;
using SupportDesk.Services;

namespace SupportDesk.Web
{
    /// <summary>
    /// Development-only startup helper. Module 2 replaces Module 1's EnsureCreated with the migrations
    /// workflow: <c>MigrateAsync</c> brings the local SQLite file up to the last committed migration
    /// (creating the file and the __EFMigrationsHistory table on first start), then the development seeder
    /// fills it so the console is never empty. It runs once, at host start, with contexts from the factory —
    /// it is not session code, and it is not how production databases change: there the reviewed
    /// idempotent script (artifacts/sql/supportdesk_migrations.sql) or a migration bundle runs before the
    /// release switches traffic, never Migrate() from several starting instances at once.
    /// </summary>
    internal static class SupportDeskDevelopmentDatabase
    {
        public static async Task EnsureReadyAsync(IServiceProvider services)
        {
            var factory = services.GetRequiredService<IDbContextFactory<SupportDeskContext>>();

            await using (var db = await factory.CreateDbContextAsync())
            {
                var pending = (await db.Database.GetPendingMigrationsAsync()).ToList();
                await db.Database.MigrateAsync();
                var applied = (await db.Database.GetAppliedMigrationsAsync()).ToList();
                Console.Error.WriteLine($"[SupportDesk] MigrateAsync: {pending.Count} pending migration(s) applied, {applied.Count} applied in total ({string.Join(", ", applied)})");
            }

            var seed = await services.GetRequiredService<DevelopmentSeeder>().SeedDevelopmentDataAsync();
            Console.Error.WriteLine($"[SupportDesk] Development seed: {seed.Describe()}");
        }
    }
}
