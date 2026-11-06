using System;
using System.Globalization;
using System.Security.Cryptography;
using AccessOps.Infrastructure;
using Microsoft.AspNetCore.WebUtilities;

namespace AccessOps.Services
{
    /// <summary>
    /// "Remember this device for 14 days". The password is never kept: the device gets a long
    /// random token in a cookie, the server keeps only its SHA-256 hash, with an expiry, and can
    /// revoke it. A stolen database row cannot be replayed as a cookie.
    /// </summary>
    public sealed class RememberedDeviceService
    {
        public const string CookieName = "AccessOps.Device";
        public static readonly TimeSpan Lifetime = TimeSpan.FromDays(14);

        private readonly AccessOpsDatabase _db;

        public RememberedDeviceService(AccessOpsDatabase db) => _db = db;

        /// <summary>Issues a token for the employee and returns the raw value for the cookie.</summary>
        public string Issue(int employeeId, out DateTime expiresUtc)
        {
            var raw = RandomNumberGenerator.GetBytes(32);
            var now = DateTime.UtcNow;
            expiresUtc = now + Lifetime;

            using var connection = _db.Open();
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
INSERT INTO remember_tokens (token_hash, employee_id, created_utc, expires_utc)
VALUES ($hash, $employee, $created, $expires);";
            cmd.Parameters.AddWithValue("$hash", SHA256.HashData(raw));
            cmd.Parameters.AddWithValue("$employee", employeeId);
            cmd.Parameters.AddWithValue("$created", now.ToString("O", CultureInfo.InvariantCulture));
            cmd.Parameters.AddWithValue("$expires", expiresUtc.ToString("O", CultureInfo.InvariantCulture));
            cmd.ExecuteNonQuery();

            return WebEncoders.Base64UrlEncode(raw);
        }

        /// <summary>The employee a presented token belongs to, or null when it is unknown or expired.</summary>
        public int? Redeem(string presented)
        {
            byte[] raw;
            try
            {
                raw = WebEncoders.Base64UrlDecode(presented ?? "");
            }
            catch (FormatException)
            {
                return null;
            }
            if (raw.Length != 32)
                return null;

            using var connection = _db.Open();
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT employee_id, expires_utc FROM remember_tokens WHERE token_hash = $hash";
            cmd.Parameters.AddWithValue("$hash", SHA256.HashData(raw));
            using var r = cmd.ExecuteReader();
            if (!r.Read())
                return null;

            var expires = DateTime.Parse(r.GetString(1), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            return expires > DateTime.UtcNow ? r.GetInt32(0) : null;
        }

        /// <summary>Revokes every remembered device of the employee (on any sign-out).</summary>
        public int RevokeAll(int employeeId)
        {
            using var connection = _db.Open();
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "DELETE FROM remember_tokens WHERE employee_id = $employee";
            cmd.Parameters.AddWithValue("$employee", employeeId);
            return cmd.ExecuteNonQuery();
        }
    }
}
