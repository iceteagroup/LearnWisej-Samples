using System;
using Microsoft.Extensions.DependencyInjection;

namespace AccessOps
{
    /// <summary>
    /// The one bridge from Wisej.NET code to the ASP.NET Core service container built in
    /// Startup.cs. It holds process-wide services only (the database, stateless services):
    /// nothing in here may ever describe a user, because every session shares it.
    /// </summary>
    internal static class AppHost
    {
        /// <summary>The application's root service provider.</summary>
        public static IServiceProvider Services { get; set; }

        /// <summary>A required service.</summary>
        public static T Get<T>() => Services.GetRequiredService<T>();

        /// <summary>An optional service, or null when this build does not register it.</summary>
        public static T Find<T>() where T : class => Services.GetService<T>();
    }
}
