using System;
using System.Collections.Generic;
using TicketOps.Data;
using TicketOps.Services;
using Wisej.Services;
using Wisej.Web;

namespace TicketOps.Infrastructure
{
    /// <summary>
    /// The one place that knows which implementation stands behind each contract — Module 1's
    /// hand-written AppComposition factory replaced by Wisej.NET's container, <c>Application.Services</c>
    /// (a <see cref="ServiceProvider"/>).
    ///
    /// Two things are easy to get wrong here, so they are spelled out:
    /// 1. The registration table is APPLICATION-wide (one per server process), while <c>Program.Main</c>
    ///    runs once per browser session. Registering twice throws, so <see cref="Apply"/> is idempotent:
    ///    it registers on the first session and only re-registers (with <c>AddOrReplaceService</c>) when a
    ///    different profile is requested. The only static member is the lock — a lock is not state.
    /// 2. Instances are NOT application-wide unless the lifetime says so: a Session registration gives every
    ///    browser session its own instance, created on first resolve inside that session. That is what keeps
    ///    the ticket list, the current operator and the log per user without a single static field.
    ///
    /// Constructor injection is explicit: each factory lambda calls the constructor with its collaborators
    /// resolved from the same container, so a service's constructor stays the complete list of what it needs
    /// and the container never has to guess. Registrations with no collaborators use the plain
    /// <c>AddService&lt;TService, TImpl&gt;(lifetime)</c> form shown in the video.
    /// </summary>
    public static class ServiceRegistration
    {
        private static readonly object Gate = new object();

        /// <summary>Reads the startup switch: <c>?profile=production</c> on the URL; anything else means fake.</summary>
        public static ServiceProfile ProfileFrom(string value)
        {
            return string.Equals(value?.Trim(), "production", StringComparison.OrdinalIgnoreCase)
                ? ServiceProfile.Production
                : ServiceProfile.Fake;
        }

        /// <summary>
        /// Makes sure <c>Application.Services</c> holds the requested profile. Returns the live profile and
        /// whether this call changed anything (false when an earlier session already registered it).
        /// </summary>
        public static ActiveProfile Apply(ServiceProfile requested, out bool changed)
        {
            lock (Gate)
            {
                var services = Application.Services;
                var current = services.HasService<ActiveProfile>() ? services.GetService<ActiveProfile>() : null;
                if (current != null && current.Profile == requested)
                {
                    changed = false;
                    return current;
                }

                var table = new List<ServiceRegistrationEntry>();
                RegisterInfrastructure(services, table);

                if (requested == ServiceProfile.Production)
                    RegisterProductionServices(services, table);
                else
                    RegisterFakeServices(services, table);

                var active = new ActiveProfile(requested, table);
                services.AddOrReplaceService<ActiveProfile>(active, ServiceLifetime.Shared);
                changed = true;
                return active;
            }
        }

        /// <summary>
        /// Registered once, never replaced by a profile switch: the session log the trace panel is attached
        /// to, and the session's outage switch. Both have parameterless constructors, so the container can
        /// create them by type.
        /// </summary>
        private static void RegisterInfrastructure(ServiceProvider services, List<ServiceRegistrationEntry> table)
        {
            if (!services.HasService<ILog>())
                services.AddService<ILog, ActivityLog>(ServiceLifetime.Session);
            table.Add(Entry<ILog, ActivityLog>(ServiceLifetime.Session,
                "the activity trace is per browser tab; a Shared log would interleave every user's clicks"));

            if (!services.HasService<DataStoreHealth>())
                services.AddService<DataStoreHealth, DataStoreHealth>(ServiceLifetime.Session);
            table.Add(Entry<DataStoreHealth, DataStoreHealth>(ServiceLifetime.Session,
                "the lab's outage switch breaks one tab's data store, not the server's"));
        }

        /// <summary>Fake profile: build and test the screen before any database exists.</summary>
        public static void RegisterFakeServices(ServiceProvider services, List<ServiceRegistrationEntry> table)
        {
            Register<ITicketService, FakeTicketService>(services, table, ServiceLifetime.Session,
                t => new FakeTicketService(services.GetService<ILog>(), services.GetService<DataStoreHealth>()),
                "the open-ticket list is per user; a Shared list would show user A's changes to user B");

            Register<IUserService, FakeUserService>(services, table, ServiceLifetime.Session,
                t => new FakeUserService(services.GetService<ILog>()),
                "\"who is signed in\" is the definition of per-session state");

            Register<IPermissionService, FakePermissionService>(services, table, ServiceLifetime.Session,
                t => new FakePermissionService(services.GetService<IUserService>(), services.GetService<ILog>()),
                "it captures the Session IUserService, and a service can never outlive a collaborator it holds");

            Register<INotificationService, FakeNotificationService>(services, table, ServiceLifetime.Transient,
                t => new FakeNotificationService(services.GetService<ILog>()),
                "stateless, cheap, one-shot: a fresh instance per resolve cannot leak anything between callers");

            RegisterShared<IAuditLogService, FakeAuditLogService>(services, table, new FakeAuditLogService(),
                "one append-only trail for the whole server; thread-safe; the operator id travels with each call");
        }

        /// <summary>Production profile: same contracts, production-shaped implementations — not one screen changes.</summary>
        public static void RegisterProductionServices(ServiceProvider services, List<ServiceRegistrationEntry> table)
        {
            Register<ITicketService, SqlTicketService>(services, table, ServiceLifetime.Session,
                t => new SqlTicketService(services.GetService<ILog>(), services.GetService<DataStoreHealth>()),
                "per-user connection scope and result set; the session disposes it when the user leaves");

            Register<IUserService, DirectoryUserService>(services, table, ServiceLifetime.Session,
                t => new DirectoryUserService(services.GetService<ILog>()),
                "the authenticated principal is per session");

            Register<IPermissionService, RolePermissionService>(services, table, ServiceLifetime.Session,
                t => new RolePermissionService(services.GetService<IUserService>(), services.GetService<ILog>()),
                "evaluates the role of the Session IUserService it holds");

            Register<INotificationService, EmailNotificationService>(services, table, ServiceLifetime.Transient,
                t => new EmailNotificationService(services.GetService<ILog>()),
                "one SMTP send per call, no state worth keeping");

            RegisterShared<IAuditLogService, SqlAuditLogService>(services, table, new SqlAuditLogService(),
                "one audit sink for the server; thread-safe; holds nothing per user");
        }

        // ---- helpers: register AND record the lifetime-table row in one step -------------------------

        private static void Register<TService, TImpl>(
            ServiceProvider services,
            List<ServiceRegistrationEntry> table,
            ServiceLifetime lifetime,
            Func<Type, object> factory,
            string why)
            where TImpl : class, TService
        {
            if (services.HasService<TService>())
                services.AddOrReplaceService<TService>(factory, lifetime);
            else
                services.AddService<TService>(factory, lifetime);

            table.Add(Entry<TService, TImpl>(lifetime, why));
        }

        private static void RegisterShared<TService, TImpl>(
            ServiceProvider services,
            List<ServiceRegistrationEntry> table,
            TImpl instance,
            string why)
            where TImpl : class, TService
        {
            if (services.HasService<TService>())
                services.AddOrReplaceService<TService>(instance, ServiceLifetime.Shared);
            else
                services.AddService<TService>(instance, ServiceLifetime.Shared);

            table.Add(Entry<TService, TImpl>(ServiceLifetime.Shared, why));
        }

        private static ServiceRegistrationEntry Entry<TService, TImpl>(ServiceLifetime lifetime, string why)
        {
            return new ServiceRegistrationEntry
            {
                ServiceType = typeof(TService),
                ImplementationType = typeof(TImpl),
                Lifetime = lifetime,
                Why = why
            };
        }
    }
}
