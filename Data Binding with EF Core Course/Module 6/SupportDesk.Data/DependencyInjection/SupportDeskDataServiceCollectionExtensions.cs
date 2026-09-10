using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
                // Parameter values in logs: local development only, never in production logs.
                options.EnableSensitiveDataLogging();

                // Module 6: the "real" EF Core log, next to (not instead of) the lab's own QueryTrace card.
                // QueryTrace is a teaching instrument — an AsyncLocal scope the UI reads per click; LogTo is
                // what a production app would actually turn on: every Database.Command event (the generated
                // SQL and its elapsed time — EF Core's own log text already carries the ms, nothing here
                // computes it by hand) at LogLevel.Information, written to the server console with an [EF]
                // prefix so it is easy to grep out of the rest of the host's output. Development only, same
                // as EnableSensitiveDataLogging two lines up.
                options.LogTo(
                    message => Console.Error.WriteLine($"[EF] {message}"),
                    new[] { DbLoggerCategory.Database.Command.Name },
                    LogLevel.Information);
            }
        });

        return services;
    }
}
