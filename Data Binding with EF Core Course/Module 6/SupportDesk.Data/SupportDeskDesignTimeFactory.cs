using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SupportDesk.Data;

/// <summary>
/// Lets <c>dotnet ef</c> build the context without starting the Wisej.NET host: the same provider
/// the application uses, a local development file, and no password anywhere.
/// </summary>
/// <example>
/// dotnet ef migrations add InitialCreate --project SupportDesk.Data --startup-project SupportDesk.Web --framework net10.0
/// dotnet ef database update            --project SupportDesk.Data --startup-project SupportDesk.Web --framework net10.0
/// </example>
public sealed class SupportDeskDesignTimeFactory : IDesignTimeDbContextFactory<SupportDeskContext>
{
    public SupportDeskContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<SupportDeskContext>()
            .UseSqlite($"Data Source={SupportDeskPaths.DevelopmentDatabaseFile()}")
            .Options;

        return new SupportDeskContext(options);
    }
}
