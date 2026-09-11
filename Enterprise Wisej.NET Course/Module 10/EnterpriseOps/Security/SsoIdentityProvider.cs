using System;
using System.Collections.Generic;
using System.Linq;
using EnterpriseOps.Diagnostics;

namespace EnterpriseOps.Security
{
    /// <summary>One claim as it arrives from the identity provider: a type and a string value. Nothing else.</summary>
    public sealed class SsoClaim
    {
        public SsoClaim(string type, string value)
        {
            Type = type;
            Value = value;
        }

        public string Type { get; }
        public string Value { get; }

        public override string ToString() => $"{Type}: {Value}";
    }

    /// <summary>The claim types this application knows how to read. Provider-specific names live here and nowhere else.</summary>
    public static class ClaimTypes
    {
        public const string Subject = "sub";
        public const string Name = "name";
        public const string Email = "email";
        /// <summary>Tenant id in the provider's directory ("tid" in an OIDC token).</summary>
        public const string TenantId = "tid";
        /// <summary>Group membership; repeated once per group. This is what becomes a <see cref="Role"/>.</summary>
        public const string Groups = "groups";
        /// <summary>Authentication method reference ("pwd+mfa"), kept for the audit entry.</summary>
        public const string AuthMethod = "amr";
        /// <summary>When the provider authenticated the user (ISO-8601).</summary>
        public const string AuthTime = "auth_time";
    }

    /// <summary>An account in the simulated corporate directory, with the claims the provider would send for it.</summary>
    public sealed class SsoAccount
    {
        public SsoAccount(string subject, string displayName, string description, IReadOnlyList<SsoClaim> claims)
        {
            Subject = subject;
            DisplayName = displayName;
            Description = description;
            Claims = claims;
        }

        public string Subject { get; }
        public string DisplayName { get; }

        /// <summary>What the sign-in screen shows next to the account, so the learner knows which path they are taking.</summary>
        public string Description { get; }

        public IReadOnlyList<SsoClaim> Claims { get; }

        public override string ToString() => $"{Subject} — {DisplayName}";
    }

    public interface ISsoIdentityProvider
    {
        /// <summary>The accounts the sign-in screen offers. A real provider would show its own login page here.</summary>
        IReadOnlyList<SsoAccount> Directory { get; }

        /// <summary>Returns the claims the provider asserts for a subject, or null when the subject is unknown.</summary>
        IReadOnlyList<SsoClaim> Authenticate(string subject);
    }

    /// <summary>
    /// A **simulation** of the OIDC/SSO boundary — the point where an external identity provider hands the
    /// application a verified identity. There is no identity provider here, no network call, no token signature
    /// and, deliberately, no password: this class stands in for everything that happens before the boundary so
    /// the lab can concentrate on everything that happens after it, which is the application's responsibility.
    ///
    /// What is real in the lab is the shape of what crosses the boundary: a set of claims, nothing more. The
    /// application never sees a password, and never asks for one.
    ///
    /// In production this class is replaced by the OIDC middleware; <see cref="ClaimsMapper"/>,
    /// <see cref="SessionContext"/>, <see cref="PermissionService"/> and the audit log stay exactly as they are.
    /// </summary>
    public sealed class SimulatedSsoIdentityProvider : ISsoIdentityProvider
    {
        private readonly ActivityTrace _trace;
        private readonly List<SsoAccount> _directory;

        public SimulatedSsoIdentityProvider(ActivityTrace trace)
        {
            _trace = trace;
            _directory = BuildDirectory();
        }

        public IReadOnlyList<SsoAccount> Directory => _directory;

        public IReadOnlyList<SsoClaim> Authenticate(string subject)
        {
            SsoAccount account = _directory.FirstOrDefault(a => StringComparer.Ordinal.Equals(a.Subject, subject));
            if (account == null)
            {
                _trace?.Identity($"SSO (simulated) — unknown subject '{subject}', no claims issued");
                return null;
            }

            _trace?.Identity($"SSO (simulated) — provider asserts {account.Claims.Count} claims for '{subject}' (no password was collected)");
            return account.Claims;
        }

        /// <summary>
        /// The corporate directory of the simulation. Note the deliberate variety:
        /// two tenants, one auditor with no write permissions, one service account, and one account whose group
        /// the application does not know — the "signed in with nothing" failure path.
        /// </summary>
        private static List<SsoAccount> BuildDirectory()
        {
            string authTime = DateTime.UtcNow.AddMinutes(-3).ToString("O");

            SsoAccount Account(string sub, string name, string email, string tenant, string description, params string[] groups)
            {
                var claims = new List<SsoClaim>
                {
                    new SsoClaim(ClaimTypes.Subject, sub),
                    new SsoClaim(ClaimTypes.Name, name),
                    new SsoClaim(ClaimTypes.Email, email),
                    new SsoClaim(ClaimTypes.TenantId, tenant),
                    new SsoClaim(ClaimTypes.AuthMethod, "pwd+mfa"),
                    new SsoClaim(ClaimTypes.AuthTime, authTime),
                };
                claims.AddRange(groups.Select(g => new SsoClaim(ClaimTypes.Groups, g)));
                return new SsoAccount(sub, name, description, claims);
            }

            return new List<SsoAccount>
            {
                Account("m.weber", "Marta Weber", "m.weber@fabrikam.example", "fabrikam",
                    "Dispatcher — approves work orders, may request an export",
                    "EnterpriseOps-Managers"),

                Account("l.romero", "Luis Romero", "l.romero@fabrikam.example", "fabrikam",
                    "Field technician — views and edits work orders",
                    "EnterpriseOps-Technicians"),

                Account("j.kim", "Jae Kim", "j.kim@fabrikam.example", "fabrikam",
                    "Compliance auditor — reads everything, changes nothing",
                    "EnterpriseOps-Auditors"),

                Account("d.singh", "Dev Singh", "d.singh@fabrikam.example", "fabrikam",
                    "Tenant administrator — the second pair of eyes on a large export",
                    "EnterpriseOps-Admins"),

                Account("svc.import", "Nightly Import", "svc.import@fabrikam.example", "fabrikam",
                    "Service account — edits work orders, never approves or exports",
                    "EnterpriseOps-Integration"),

                Account("ana.ops", "Ana Ops", "ana.ops@contoso.example", "contoso",
                    "Manager on the contoso tenant",
                    "EnterpriseOps-Managers"),

                Account("t.novak", "Tomas Novak", "t.novak@fabrikam.example", "fabrikam",
                    "Group the application does not know — signs in with zero permissions",
                    "CorpVPN-Users"),
            };
        }
    }
}
