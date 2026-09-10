using System;
using System.Collections.Generic;
using Wisej.Services;

namespace TicketOps.Infrastructure
{
    /// <summary>Which registration profile the container currently holds.</summary>
    public enum ServiceProfile
    {
        /// <summary>In-memory fakes: build and demo the screens before any backend exists.</summary>
        Fake,

        /// <summary>Production-shaped implementations (in this lab: stand-ins that log the SQL / SMTP / directory calls they would make).</summary>
        Production
    }

    /// <summary>One row of the lifetime table: contract → implementation → lifetime → why that lifetime.</summary>
    public sealed class ServiceRegistrationEntry
    {
        public Type ServiceType { get; init; }
        public Type ImplementationType { get; init; }
        public ServiceLifetime Lifetime { get; init; }
        public string Why { get; init; }

        public string ServiceName => ServiceType.Name;
        public string ImplementationName => ImplementationType.Name;

        public override string ToString() => $"{ServiceName} → {ImplementationName} · {Lifetime}";
    }

    /// <summary>
    /// Describes the registration profile that is live in <c>Application.Services</c>. Registered itself as a
    /// Shared service, because the registration table is application-wide: a Form injects it to show which
    /// profile it runs under, and Program.Main reads it to decide whether anything needs (re)registering.
    /// It holds no per-user state.
    /// </summary>
    public sealed class ActiveProfile
    {
        public ServiceProfile Profile { get; }
        public DateTime RegisteredAt { get; }
        public IReadOnlyList<ServiceRegistrationEntry> Entries { get; }

        public ActiveProfile(ServiceProfile profile, IReadOnlyList<ServiceRegistrationEntry> entries)
        {
            Profile = profile;
            RegisteredAt = DateTime.Now;
            Entries = entries ?? throw new ArgumentNullException(nameof(entries));
        }

        public string DisplayName => Profile == ServiceProfile.Fake ? "fake (in-memory)" : "production (stand-ins)";

        public ServiceRegistrationEntry Find(Type serviceType)
        {
            foreach (var e in Entries)
                if (e.ServiceType == serviceType)
                    return e;
            return null;
        }
    }
}
