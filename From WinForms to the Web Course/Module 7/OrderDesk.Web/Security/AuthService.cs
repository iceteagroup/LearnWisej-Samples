using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Wisej.Web;

namespace OrderDesk.Security
{
    /// <summary>The result of an authorization check — allowed, or denied with the reason the audit log gets.</summary>
    public sealed class AuthorizationResult
    {
        public bool Allowed { get; set; }
        public string Reason { get; set; }
        public string Permission { get; set; }
    }

    /// <summary>
    /// Server-side authentication and authorization. On the desktop the login form set a static user and
    /// every button trusted it; on the web the browser is the untrusted side, so every server action asks
    /// Authorize(permission) again — the click handler is the only thing the client can reach.
    ///
    /// Demo store: two accounts with salted SHA-256 password hashes hard-coded below. A real deployment
    /// keeps them in a user store (or delegates to ASP.NET Core / OpenID Connect and maps claims to roles);
    /// what stays the same is that the check runs on the server, per request, from Application.Session.
    /// </summary>
    public static class AuthService
    {
        public const string PermissionRead = "orders.read";
        public const string PermissionExport = "orders.export";
        public const string PermissionDownload = "files.download";
        public const string PermissionDelete = "orders.delete";

        private sealed class Account
        {
            public string UserName;
            public string Company;
            public string Role;
            public string Salt;
            public string Hash;   // hex SHA-256 of "salt:password"
        }

        // kelly / northwind-2026 · sam / fabrikam-2026 (the demo passwords the console buttons pass).
        private static readonly IReadOnlyDictionary<string, Account> Accounts = new Dictionary<string, Account>(StringComparer.OrdinalIgnoreCase)
        {
            ["kelly"] = new Account { UserName = "kelly", Company = "Acme", Role = "Manager", Salt = "a3f1c9d2e8b74c06", Hash = "17f9d6d8204c334829452c502f3b1a62a17d3ffb8233f181fedf1adb51499a56" },
            ["sam"] = new Account { UserName = "sam", Company = "Globex", Role = "Clerk", Salt = "5d0e7b2a91c4f836", Hash = "09a7d06183030dfba4b7c9ae50bfd7092f55c5ee7e7237ac061f256fce401381" },
        };

        // Role → permissions. Clerks read; Managers also export, download and delete.
        private static readonly IReadOnlyDictionary<string, string[]> RolePermissions = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            ["Clerk"] = new[] { PermissionRead },
            ["Manager"] = new[] { PermissionRead, PermissionExport, PermissionDownload, PermissionDelete },
        };

        /// <summary>The user of this browser session, or null. Read from Application.Session on every call.</summary>
        public static UserSessionContext Current
        {
            get { dynamic session = Application.Session; return session.UserContext as UserSessionContext; }
            private set { dynamic session = Application.Session; session.UserContext = value; }
        }

        /// <summary>Verifies the password against the salted hash and stores the context in this session only.</summary>
        public static UserSessionContext SignIn(string userName, string password)
        {
            if (!Accounts.TryGetValue(userName ?? "", out var account) || !VerifyPassword(account, password))
            {
                AuditLog.Record("sign-in", $"denied for '{userName}'", allowed: false, userOverride: userName ?? "?");
                return null;
            }

            var context = new UserSessionContext
            {
                UserName = account.UserName,
                Company = account.Company,
                Role = account.Role,
                SignedInUtc = DateTime.UtcNow,
                SessionId = Application.SessionId
            };
            Current = context;
            AuditLog.Record("sign-in", $"{context.Role} of {context.Company}", allowed: true);
            return context;
        }

        public static void SignOut()
        {
            var user = Current;
            if (user == null) return;
            AuditLog.Record("sign-out", user.Role, allowed: true);
            Current = null;
        }

        /// <summary>Checked on every server action. Denials are audited here so no caller can forget to.</summary>
        public static AuthorizationResult Authorize(string permission)
        {
            var user = Current;
            var result = new AuthorizationResult { Permission = permission };
            if (user == null)
            {
                result.Reason = "no user signed in for this session";
            }
            else if (!RolePermissions.TryGetValue(user.Role, out var permissions) || Array.IndexOf(permissions, permission) < 0)
            {
                result.Reason = $"role {user.Role} lacks '{permission}'";
            }
            else
            {
                result.Allowed = true;
                result.Reason = $"role {user.Role} has '{permission}'";
            }

            if (!result.Allowed)
                AuditLog.Record("authorize " + permission, "DENIED — " + result.Reason, allowed: false);
            return result;
        }

        /// <summary>Authorize or throw — for code paths that must not continue (export, download, delete).</summary>
        public static void Demand(string permission)
        {
            var result = Authorize(permission);
            if (!result.Allowed)
                throw new UnauthorizedAccessException($"'{permission}' denied: {result.Reason}.");
        }

        private static bool VerifyPassword(Account account, string password)
        {
            if (password == null) return false;
            var hash = Sha256Hex(account.Salt + ":" + password);
            // Constant-time compare so a wrong password does not leak how many leading bytes matched.
            return CryptographicOperations.FixedTimeEquals(Encoding.ASCII.GetBytes(hash), Encoding.ASCII.GetBytes(account.Hash));
        }

        private static string Sha256Hex(string text)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(text));
            var sb = new StringBuilder(bytes.Length * 2);
            foreach (var b in bytes) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }
}
