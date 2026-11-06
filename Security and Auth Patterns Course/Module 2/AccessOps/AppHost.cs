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
        public static IServiceProvider Services { get; set; }

        public static T Get<T>() => Services.GetRequiredService<T>();
    }
}
