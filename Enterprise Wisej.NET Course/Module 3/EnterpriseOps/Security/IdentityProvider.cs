using System;
using System.Collections.Generic;

namespace EnterpriseOps.Security
{
    /// <summary>
    /// Stand-in for the real identity provider (Entra ID, Keycloak, ASP.NET Identity…). It answers one
    /// question — "who is this and what are they entitled to?" — from verified claims. It is an instance
    /// created per session on purpose: it holds no state that could leak between users.
    /// </summary>
    public sealed class IdentityProvider
    {
        private readonly Dictionary<string, VerifiedIdentity> _directory;

        public IdentityProvider()
        {
            // The course's three users. ana.ops is entitled to two of the three tenants — never to northwind.
            _directory = new Dictionary<string, VerifiedIdentity>(StringComparer.OrdinalIgnoreCase)
            {
                ["ana.ops"] = new VerifiedIdentity("ana.ops", "Ana Ortiz",
                    new[] { "Manager" }, new[] { "workorders.read", "workorders.write", "workorders.approve" },
                    new[] { "contoso", "fabrikam" }, "contoso", "en-US"),
                ["ben.tech"] = new VerifiedIdentity("ben.tech", "Ben Tanaka",
                    new[] { "Technician" }, new[] { "workorders.read", "workorders.write" },
                    new[] { "contoso" }, "contoso", "en-US"),
                ["cara.admin"] = new VerifiedIdentity("cara.admin", "Cara Adler",
                    new[] { "Admin" }, new[] { "workorders.read", "workorders.write", "workorders.approve", "tenants.manage" },
                    new[] { "contoso", "fabrikam", "northwind" }, "northwind", "de-DE"),
            };
        }

        /// <summary>Returns the verified claims for a user, or throws — an unknown user never gets a session.</summary>
        public VerifiedIdentity SignIn(string userId)
        {
            if (userId != null && _directory.TryGetValue(userId, out var identity))
                return identity;

            throw new UnauthorizedAccessException($"Unknown user '{userId}'.");
        }
    }
}
