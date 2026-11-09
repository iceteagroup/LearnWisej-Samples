using System;
using Microsoft.Extensions.DependencyInjection;

namespace AccessOps
{
    /// <summary>The two entry points of AccessOps.</summary>
    public enum AccessOpsRoute
    {
        /// <summary>Staff: Windows integrated authentication (IIS, or Kestrel Negotiate in the lab), scoped reads.</summary>
        Intranet,

        /// <summary>Partners: public HTTPS, own sign-in, OpenID Connect, passkeys.</summary>
        Partner
    }

    /// <summary>
    /// The one bridge from Wisej.NET code to the ASP.NET Core service container built in
    /// Startup.cs. It holds process-wide services only (the database, stateless services):
    /// nothing in here may ever describe a user, because every session shares it.
    /// </summary>
    internal static class AppHost
    {
        /// <summary>
        /// The entry point this process serves (<c>AccessOps:Route</c>): one build, started once per
        /// route, like the course's two IIS applications.
        /// </summary>
        public static AccessOpsRoute Route { get; set; }

        /// <summary>True when the process runs in the Development environment (lab-only tools).</summary>
        public static bool IsDevelopment { get; set; }

        /// <summary>The application's root service provider.</summary>
        public static IServiceProvider Services { get; set; }

        /// <summary>A required service.</summary>
        public static T Get<T>() => Services.GetRequiredService<T>();

        /// <summary>An optional service, or null when this build does not register it.</summary>
        public static T Find<T>() where T : class => Services.GetService<T>();
    }
}
