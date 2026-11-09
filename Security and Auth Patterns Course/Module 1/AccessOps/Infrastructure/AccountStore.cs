using System;
using AccessOps.Identity;

namespace AccessOps.Infrastructure
{
    /// <summary>
    /// Reads accounts from the database. It verifies nothing: only a verifier
    /// (<see cref="IAccountVerifier"/>) may turn a sign-in attempt into an <see cref="Account"/>.
    /// </summary>
    public sealed class AccountStore
    {
        private readonly AccessOpsDatabase _db;

        public AccountStore(AccessOpsDatabase db)
        {
            _db = db;
        }

        /// <summary>The account with this sign-in name, or null.</summary>
        public Account FindByUserName(string userName)
        {
            using var db = _db.Open();
            using var cmd = db.CreateCommand();
            cmd.CommandText = "SELECT id, user_name, tenant, grant_id, roles FROM accounts WHERE user_name = $name";
            cmd.Parameters.AddWithValue("$name", userName ?? "");
            using var r = cmd.ExecuteReader();
            if (!r.Read())
                return null;

            return new Account
            {
                Id = r.GetString(0),
                UserName = r.GetString(1),
                Tenant = r.GetString(2),
                GrantId = r.IsDBNull(3) ? null : r.GetString(3),
                Roles = r.GetString(4).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            };
        }
    }
}
