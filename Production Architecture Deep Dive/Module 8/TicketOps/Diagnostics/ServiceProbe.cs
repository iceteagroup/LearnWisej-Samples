using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TicketOps.Data;
using TicketOps.Infrastructure;
using TicketOps.Services;
using Wisej.Services;
using Wisej.Web;

namespace TicketOps.Diagnostics
{
    /// <summary>One row of the "Injected services" grid.</summary>
    public sealed class ServiceProbeRow
    {
        public string Service { get; init; }
        public string Implementation { get; init; }
        public string Lifetime { get; init; }
        public string Instance { get; init; }
        public string SecondResolve { get; init; }
    }

    /// <summary>
    /// Diagnostics only: resolves each registered contract twice and reports whether the container handed
    /// back the same instance. Session and Shared services come back identical; a Transient comes back new.
    /// This is the one place outside ServiceRegistration that talks to the container directly — diagnostics
    /// read the container, application code receives its dependencies.
    /// </summary>
    public static class ServiceProbe
    {
        public static IReadOnlyList<ServiceProbeRow> Probe(ActiveProfile profile)
        {
            return new List<ServiceProbeRow>
            {
                Row<ITicketService>(profile),
                Row<IUserService>(profile),
                Row<IPermissionService>(profile),
                Row<INotificationService>(profile),
                Row<IAuditLogService>(profile),
                Row<ILog>(profile),
                Row<DataStoreHealth>(profile)
            };
        }

        /// <summary>A short, stable id for an object: the last four hex digits of its identity hash.</summary>
        public static string IdOf(object instance)
            => instance == null ? "—" : "#" + (RuntimeHelpers.GetHashCode(instance) & 0xFFFF).ToString("x4");

        private static ServiceProbeRow Row<TService>(ActiveProfile profile)
        {
            var services = Application.Services;
            var entry = profile?.Find(typeof(TService));
            string lifetime = entry != null ? entry.Lifetime.ToString() : "?";

            if (!services.HasService<TService>())
            {
                return new ServiceProbeRow
                {
                    Service = typeof(TService).Name,
                    Implementation = "(not registered)",
                    Lifetime = lifetime,
                    Instance = "—",
                    SecondResolve = "—"
                };
            }

            object first = services.GetService<TService>();
            object second = services.GetService<TService>();
            bool same = ReferenceEquals(first, second);

            return new ServiceProbeRow
            {
                Service = typeof(TService).Name,
                Implementation = first?.GetType().Name ?? "(null)",
                Lifetime = lifetime,
                Instance = IdOf(first),
                SecondResolve = same
                    ? "same instance"
                    : $"new instance {IdOf(second)}" + (entry != null && entry.Lifetime == ServiceLifetime.Transient ? " (transient)" : " (!)")
            };
        }
    }
}
