using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SupportDesk.Data;

public static class SupportDeskDataServiceCollectionExtensions
{
    /// <summary>
    /// Registers EF Core for the Support Desk in Microsoft DI: <c>AddDbContextFactory</c> with the SQLite
    /// provider and development-only diagnostics. The web project calls this one method, so the provider
    /// packages stay inside SupportDesk.Data.
    /// </summary>
    public static IServiceCollection AddSupportDeskData(this IServiceCollection services, string connectionString, bool isDevelopment)
    {
        services.AddDbContextFactory<SupportDeskContext>(options =>
        {
            options.UseSqlite(connectionString);

            if (isDevelopment)
            {
                options.EnableDetailedErrors();
                // Parameter values in logs: local development only, never in production logs.
                options.EnableSensitiveDataLogging();

                // EF Core logging in development: every Database.Command event (the SQL and its elapsed
                // time) on the server console with an [EF] prefix.
                options.LogTo(
                    message => Console.Error.WriteLine($"[EF] {message}"),
                    new[] { DbLoggerCategory.Database.Command.Name },
                    LogLevel.Information);
            }
        });

        return services;
    }
}
