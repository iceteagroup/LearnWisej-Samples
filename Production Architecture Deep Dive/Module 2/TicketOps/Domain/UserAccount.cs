using System;
using System.Collections.Generic;
using System.Linq;

namespace TicketOps.Domain
{
    /// <summary>
    /// Who a person is and which tenants they may work in — the <b>user</b> scope of the lesson
    /// (identity, across sessions). It is deliberately separate from <c>SessionContext</c>, which is one
    /// running instance of the app for that person: one user, many possible sessions.
    /// </summary>
    public sealed class UserAccount
    {
        public string Name { get; }
        public IReadOnlyList<string> Tenants { get; }

        public UserAccount(string name, params string[] tenants)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Tenants = (tenants ?? new string[0]).ToList();
        }

        public string DefaultTenant => Tenants.Count > 0 ? Tenants[0] : null;

        /// <summary>The rule "an operator may only switch to a tenant they belong to" lives here, with the record.</summary>
        public bool IsMemberOf(string tenant)
            => tenant != null && Tenants.Any(t => string.Equals(t, tenant, StringComparison.OrdinalIgnoreCase));

        public override string ToString() => $"{Name} [{string.Join(", ", Tenants)}]";
    }
}
