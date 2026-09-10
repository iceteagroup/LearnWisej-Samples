using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SupportDesk.Data.Diagnostics;

namespace SupportDesk.Data;

public static class SupportDeskDataServiceCollectionExtensions
{
    /// <summary>
    /// Registers EF Core for the Support Desk in Microsoft DI: <c>AddDbContextFactory</c> with the SQLite
    /// provider, the lab interceptors, and development-only diagnostics. The web project calls this one
    /// method, so the provider packages stay inside SupportDesk.Data.
    /// </summary>
    public static IServiceCollection AddSupportDeskData(this IServiceCollection services, string connectionString, bool isDevelopment)
    {
        services.AddSingleton<DevelopmentOutageSwitch>();
        // Module 7 lab props/diagnostics: the transaction-rollback demo's failure switch, and what the
        // host decided at startup (environment, migrations, sensitive-data logging) for the diagnostics panel.
        services.AddSingleton<TransactionFailureSwitch>();
        services.AddSingleton<StartupDiagnostics>();

        services.AddDbContextFactory<SupportDeskContext>((provider, options) =>
        {
            options.UseSqlite(connectionString);

            // Lab instruments (see Diagnostics/): the SQL trace the UI shows, and the outage switch.
            options.AddInterceptors(
                new QueryTraceInterceptor(),
                new OutageInterceptor(provider.GetRequiredService<DevelopmentOutageSwitch>()));

            if (isDevelopment)
            {
                options.EnableDetailedErrors();
                // Parameter values in logs: local development only, never in production logs. See
                // docs/DeploymentNotes.md — this flag must never be true outside Development.
                options.EnableSensitiveDataLogging();
            }
        });

        return services;
    }
}
