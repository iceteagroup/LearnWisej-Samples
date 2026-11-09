using AccessOps.Identity;

namespace AccessOps.Infrastructure
{
    /// <summary>
    /// Reads accounts and their tenant from the database. It decides nothing: only
    /// <see cref="LoginService"/> turns a sign-in attempt into a verified account.
    /// </summary>
    public sealed class AccountStore
    {
        private readonly AccessOpsDatabase _db;

        /// <summary>Creates the store over the AccessOps database.</summary>
        public AccountStore(AccessOpsDatabase db)
        {
            _db = db;
        }

        /// <summary>The account with this (normalized) sign-in name, or null.</summary>
        public Account FindByUserName(string userName) => Find("a.user_name = $key", userName);

        /// <summary>The account with this ID, or null.</summary>
        public Account FindById(string accountId) => Find("a.id = $key", accountId);

        private Account Find(string where, string key)
        {
            using var db = _db.Open();
            using var cmd = db.CreateCommand();
            // the WHERE text is one of the two constants above, never input; the key is a parameter
            cmd.CommandText = @"
SELECT a.id, a.user_name, a.display_name, a.tenant, t.approved, a.role, a.disabled, a.password_hash
FROM accounts a JOIN tenants t ON t.id = a.tenant WHERE " + where;
            cmd.Parameters.AddWithValue("$key", key ?? "");
            using var r = cmd.ExecuteReader();
            if (!r.Read())
                return null;

            return new Account
            {
                Id = r.GetString(0),
                UserName = r.GetString(1),
                DisplayName = r.GetString(2),
                Tenant = r.GetString(3),
                TenantApproved = r.GetInt64(4) == 1,
                Role = r.GetString(5),
                Disabled = r.GetInt64(6) == 1,
                PasswordHash = r.IsDBNull(7) ? null : r.GetString(7)
            };
        }
    }
}
