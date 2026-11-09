using System;
using System.Collections.Generic;
using AccessOps.Infrastructure;
using Microsoft.Extensions.Logging;

namespace AccessOps.Security
{
    /// <summary>The real reason of a sign-in outcome. It goes to the login audit, never to the screen.</summary>
    public enum LoginOutcome
    {
        /// <summary>Every check passed and a grant was created.</summary>
        Succeeded,
        /// <summary>The account exists and the password did not match.</summary>
        BadCredential,
        /// <summary>No account has this name (checked against the dummy credential).</summary>
        UnknownUser,
        /// <summary>The password matched a disabled account.</summary>
        Disabled,
        /// <summary>The password matched an account in a tenant AccessOps has not approved.</summary>
        TenantNotApproved,
        /// <summary>The name or the address is locked by <c>LoginThrottle</c>.</summary>
        LockedOut,
        /// <summary>A valid credential for an account that may not use this sign-in form.</summary>
        NotAllowedHere,
        /// <summary>The account signed out; its grants were revoked.</summary>
        SignedOut
    }

    /// <summary>
    /// The append-only <c>login_audit</c> table plus one log line per attempt: opaque name,
    /// opaque address, outcome and correlation ID. It has no field for a password, a typed name
    /// in clear, a session ID or a token.
    /// </summary>
    public sealed class LoginAudit
    {
        private readonly AccessOpsDatabase _db;
        private readonly ILogger<LoginAudit> _logger;

        /// <summary>Creates the audit over the AccessOps database.</summary>
        public LoginAudit(AccessOpsDatabase db, ILogger<LoginAudit> logger)
        {
            _db = db;
            _logger = logger;
        }

        /// <summary>Appends one row and writes one log line.</summary>
        public void Write(string opaqueSubject, string opaqueAddress, LoginOutcome outcome, string correlationId, string grant = null)
        {
            using (var db = _db.Open())
            using (var cmd = db.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO login_audit (at, subject, address, outcome, correlation_id) VALUES ($at, $subject, $address, $outcome, $correlation)";
                cmd.Parameters.AddWithValue("$at", AccessOpsDatabase.Stamp(DateTimeOffset.UtcNow));
                cmd.Parameters.AddWithValue("$subject", opaqueSubject);
                cmd.Parameters.AddWithValue("$address", opaqueAddress);
                cmd.Parameters.AddWithValue("$outcome", outcome.ToString());
                cmd.Parameters.AddWithValue("$correlation", correlationId);
                cmd.ExecuteNonQuery();
            }

            _logger.LogInformation(
                "login subject={Subject} address={Address} outcome={Outcome} grant={Grant} correlation={CorrelationId}",
                opaqueSubject, opaqueAddress, outcome, grant ?? "-", correlationId);
        }

        /// <summary>The newest rows first (for tests and the lab write-up).</summary>
        public IReadOnlyList<(string Subject, string Outcome)> Recent(int count)
        {
            var rows = new List<(string, string)>();
            using var db = _db.Open();
            using var cmd = db.CreateCommand();
            cmd.CommandText = "SELECT subject, outcome FROM login_audit ORDER BY seq DESC LIMIT $count";
            cmd.Parameters.AddWithValue("$count", count);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                rows.Add((r.GetString(0), r.GetString(1)));
            return rows;
        }
    }
}
