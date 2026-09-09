using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SupportDesk.Data;
using SupportDesk.Data.Diagnostics;

namespace SupportDesk.Tests.Support;

/// <summary>
/// An <see cref="IDbContextFactory{TContext}"/> over one open in-memory SQLite connection, so the
/// services under test get real relational behaviour (constraints, SQL translation) without a file.
/// Every CreateDbContext still hands out a fresh context — the same lifetime rule as production.
/// </summary>
public sealed class SqliteTestFactory : IDbContextFactory<SupportDeskContext>, IDisposable
{
    private readonly SqliteConnection _connection = new("Data Source=:memory:");
    private readonly DbContextOptions<SupportDeskContext> _options;

    public SqliteTestFactory()
    {
        _connection.Open();
        _options = new DbContextOptionsBuilder<SupportDeskContext>()
            .UseSqlite(_connection)
            .AddInterceptors(new QueryTraceInterceptor())
            .Options;

        using var db = CreateDbContext();
        db.Database.EnsureCreated();
    }

    public SupportDeskContext CreateDbContext() => new(_options);

    public void Dispose() => _connection.Dispose();
}
