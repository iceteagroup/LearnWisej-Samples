using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using EnterpriseOps.Domain;
using EnterpriseOps.Security;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// Reference data: the customers this deployment serves. Application-scoped, immutable after start-up and
    /// therefore one of the few things a static may legitimately hold — the audit says so out loud rather than
    /// leaving a reader to guess. Which tenants a given <b>user</b> may pick is not in here: that is a claim on
    /// the identity, and it is the session context that answers it.
    /// </summary>
    public static class TenantDirectory
    {
        [SharedState(StateScope.Application,
            holds: "the three tenants this deployment serves — reference data, identical for every user",
            synchronization: "immutable after construction: a read-only collection of immutable Tenant objects")]
        private static readonly ReadOnlyCollection<Tenant> All = new ReadOnlyCollection<Tenant>(new List<Tenant>
        {
            new Tenant("contoso", "Contoso Retail & Logistics"),
            new Tenant("fabrikam", "Fabrikam Utilities"),
            new Tenant("northwind", "Northwind Facilities"),
        });

        public static IReadOnlyList<Tenant> Tenants => All;

        public static Tenant Find(string tenantId)
            => All.FirstOrDefault(t => StringComparer.Ordinal.Equals(t.Id, tenantId));

        public static string NameOf(string tenantId) => Find(tenantId)?.Name ?? tenantId;

        /// <summary>The tenants one session may switch to — the directory filtered by the session's verified claims.</summary>
        public static IReadOnlyList<Tenant> EntitledFor(SessionContext session)
        {
            if (session == null) return Array.Empty<Tenant>();
            return All.Where(t => session.IsEntitledTo(t.Id)).ToList();
        }
    }
}
