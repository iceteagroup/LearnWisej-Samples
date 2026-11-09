using System;
using System.Runtime.Versioning;
using System.Security.Principal;
using AccessOps.Identity;

namespace AccessOps.Infrastructure
{
    /// <summary>
    /// Maps Windows accounts (by SID, the immutable key; never by name) to AccessOps accounts.
    /// In a domain the directory team would provision this table from the HR or access-request
    /// system; for the lab one row is written at start-up so the sample works on any machine.
    /// </summary>
    public sealed class WindowsAccountMap
    {
        private readonly AccessOpsDatabase _db;
        private readonly AccountStore _accounts;

        public WindowsAccountMap(AccessOpsDatabase db, AccountStore accounts)
        {
            _db = db;
            _accounts = accounts;
        }

        /// <summary>The AccessOps account mapped to <paramref name="sid"/>, or null.</summary>
        public Account FindAccount(string sid)
        {
            if (string.IsNullOrEmpty(sid))
                return null;

            using var db = _db.Open();
            using var cmd = db.CreateCommand();
            cmd.CommandText = "SELECT account_id FROM windows_accounts WHERE sid = $sid";
            cmd.Parameters.AddWithValue("$sid", sid);
            var accountId = cmd.ExecuteScalar() as string;
            return accountId == null ? null : _accounts.FindById(accountId);
        }

        /// <summary>
        /// LAB FIXTURE: maps the Windows account that runs this process (the same account the
        /// browser signs in with on localhost) to the AccessOps account <paramref name="userName"/>
        /// (reader1 or visitor1; never a partner account), replacing any earlier mapping of that SID.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void MapCurrentWindowsUser(string userName)
        {
            var account = _accounts.FindByUserName(userName);
            if (account == null || account.Kind != "windows")
                throw new InvalidOperationException($"AccessOps:CurrentWindowsUser must name an intranet account (reader1 or visitor1), not '{userName}'.");

            using var current = WindowsIdentity.GetCurrent();
            using var db = _db.Open();
            using var cmd = db.CreateCommand();
            cmd.CommandText = @"
INSERT INTO windows_accounts (sid, account_id) VALUES ($sid, $account)
ON CONFLICT(sid) DO UPDATE SET account_id = excluded.account_id";
            cmd.Parameters.AddWithValue("$sid", current.User.Value);
            cmd.Parameters.AddWithValue("$account", account.Id);
            cmd.ExecuteNonQuery();
        }
    }
}
