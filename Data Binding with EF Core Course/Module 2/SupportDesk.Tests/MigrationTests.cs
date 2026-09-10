using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SupportDesk.Data;
using SupportDesk.Services;

namespace SupportDesk.Tests;

/// <summary>
/// The development start-up path without Wisej.NET: a fresh SQLite database, <c>MigrateAsync</c> applies
/// InitialCreate (creating __EFMigrationsHistory), a second call is a no-op, and the seeder fills the tables
/// the migration created. This is what SupportDeskDevelopmentDatabase.EnsureReadyAsync does at host start.
/// </summary>
public sealed class MigrationTests : IAsyncLifetime
{
    private readonly SqliteConnection _connection = new("Data Source=:memory:");
    private DbContextOptions<SupportDeskContext> _options = null!;

    public async Task InitializeAsync()
    {
        await _connection.OpenAsync();
        _options = new DbContextOptionsBuilder<SupportDeskContext>().UseSqlite(_connection).Options;
    }

    public async Task DisposeAsync() => await _connection.DisposeAsync();

    [Fact]
    public async Task MigrateAsync_applies_InitialCreate_once_and_the_seeder_fills_the_schema()
    {
        await using (var db = new SupportDeskContext(_options))
        {
            Assert.Equal(new[] { "20260910150534_InitialCreate" }, await db.Database.GetPendingMigrationsAsync());
            Assert.Empty(await db.Database.GetAppliedMigrationsAsync());

            await db.Database.MigrateAsync();

            Assert.Equal(new[] { "20260910150534_InitialCreate" }, await db.Database.GetAppliedMigrationsAsync());
            Assert.Empty(await db.Database.GetPendingMigrationsAsync());

            await db.Database.MigrateAsync();                                           // second start: nothing to do
            Assert.Single(await db.Database.GetAppliedMigrationsAsync());
        }

        var factory = new OptionsFactory(_options);
        var seed = await new DevelopmentSeeder(factory).SeedDevelopmentDataAsync();
        Assert.True(seed.Seeded);

        await using (var db = new SupportDeskContext(_options))
        {
            Assert.True(await db.Tickets.CountAsync() >= 50);

            // The migrated schema carries the same rules the tests exercise through EnsureCreated.
            var sql = await ReadCreateTableAsync(db, "Tickets");
            Assert.Contains("CONSTRAINT \"CK_Tickets_Title_Length\" CHECK (length(\"Title\") <= 180)", sql);
            Assert.Contains("ON DELETE SET NULL", sql);
            Assert.Contains("ON DELETE RESTRICT", sql);
            Assert.Contains("\"RowVersion\" BLOB NOT NULL", sql);
            Assert.Contains("ON DELETE CASCADE", await ReadCreateTableAsync(db, "TicketComments"));
        }
    }

    private static async Task<string> ReadCreateTableAsync(SupportDeskContext db, string table)
    {
        await using var command = db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "SELECT sql FROM sqlite_master WHERE type = 'table' AND name = $name";
        var p = command.CreateParameter();
        p.ParameterName = "$name";
        p.Value = table;
        command.Parameters.Add(p);
        await db.Database.OpenConnectionAsync();
        return (string)(await command.ExecuteScalarAsync())!;
    }

    private sealed class OptionsFactory : IDbContextFactory<SupportDeskContext>
    {
        private readonly DbContextOptions<SupportDeskContext> _options;
        public OptionsFactory(DbContextOptions<SupportDeskContext> options) => _options = options;
        public SupportDeskContext CreateDbContext() => new(_options);
    }
}
