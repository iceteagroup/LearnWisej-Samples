using System;
using System.Collections.Generic;
using System.Globalization;
using AccessOps.Infrastructure;

namespace AccessOps.Services
{
    /// <summary>The real reason behind a sign-in result. The screen never shows it; the audit table does.</summary>
    public enum LoginOutcome
    {
        Success,
        RememberedDevice,
        BadCredential,
        UnknownUser,
        LockedOut,
        Disabled,
        SignOutExplicit,
        SignOutIdle
    }

    public sealed class LoginAuditRow
    {
        public long Id { get; init; }
        public DateTime AtUtc { get; init; }
        public string UserNameTried { get; init; }
        public string ClientAddress { get; init; }
        public string Outcome { get; init; }
    }

    /// <summary>
    /// One row per sign-in attempt and per sign-out: the user name tried, the time, the client
    /// address and the true outcome. Never the password - not even a failed one.
    /// </summary>
    public sealed class LoginAuditService
    {
        private readonly AccessOpsDatabase _db;

        public LoginAuditService(AccessOpsDatabase db) => _db = db;

        public void Write(string userNameTried, string clientAddress, LoginOutcome outcome)
        {
            using var connection = _db.Open();
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
INSERT INTO login_audit (at_utc, user_name_tried, client_address, outcome)
VALUES ($at, $user, $address, $outcome);";
            cmd.Parameters.AddWithValue("$at", DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture));
            // a user name is free text from the browser: keep it short, keep it as data
            cmd.Parameters.AddWithValue("$user", Truncate(userNameTried ?? "", 64));
            cmd.Parameters.AddWithValue("$address", clientAddress ?? "unknown");
            cmd.Parameters.AddWithValue("$outcome", outcome.ToString());
            cmd.ExecuteNonQuery();
        }

        public IReadOnlyList<LoginAuditRow> Latest(int count)
        {
            using var connection = _db.Open();
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT id, at_utc, user_name_tried, client_address, outcome FROM login_audit ORDER BY id DESC LIMIT $n";
            cmd.Parameters.AddWithValue("$n", count);
            var rows = new List<LoginAuditRow>();
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                rows.Add(new LoginAuditRow
                {
                    Id = r.GetInt64(0),
                    AtUtc = DateTime.Parse(r.GetString(1), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),
                    UserNameTried = r.GetString(2),
                    ClientAddress = r.GetString(3),
                    Outcome = r.GetString(4)
                });
            }
            return rows;
        }

        private static string Truncate(string s, int max) => s.Length <= max ? s : s.Substring(0, max);
    }
}
