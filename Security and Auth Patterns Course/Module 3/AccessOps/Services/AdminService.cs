using System.Collections.Generic;
using System.Linq;
using AccessOps.Infrastructure;

namespace AccessOps.Services
{
    public sealed class AccountListItem
    {
        public int Id { get; init; }
        public string UserName { get; init; }
        public string DisplayName { get; init; }
        public string Department { get; init; }
        public string Roles { get; init; }
        public string Claims { get; init; }
        public string Permissions { get; init; }
    }

    /// <summary>The admin console's queries. Every method requires ManageUsers.</summary>
    public sealed class AdminService
    {
        private readonly IAuthorize _authorize;
        private readonly AccessOpsDatabase _db;
        private readonly PermissionService _permissions;

        public AdminService(IAuthorize authorize, AccessOpsDatabase db, PermissionService permissions)
        {
            _authorize = authorize;
            _db = db;
            _permissions = permissions;
        }

        public IReadOnlyList<AccountListItem> ListAccounts()
        {
            _authorize.Require(Permission.ManageUsers);

            using var connection = _db.Open();
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
SELECT id, user_name, first_name || ' ' || last_name, department, roles
FROM employees WHERE user_name IS NOT NULL ORDER BY department, last_name";
            var rows = new List<(int Id, string User, string Name, string Dept, string Roles)>();
            using (var r = cmd.ExecuteReader())
            {
                while (r.Read())
                    rows.Add((r.GetInt32(0), r.GetString(1), r.GetString(2), r.GetString(3), r.GetString(4)));
            }

            return rows.Select(row =>
            {
                var claims = _permissions.Claims(row.Id);
                return new AccountListItem
                {
                    Id = row.Id,
                    UserName = row.User,
                    DisplayName = row.Name,
                    Department = row.Dept,
                    Roles = row.Roles.Replace(";", ", "),
                    Claims = string.Join(", ", claims.Select(c => c.Kind + " " + c.Permission)),
                    Permissions = _permissions.ResolveFor(row.Roles, claims).ToString()
                };
            }).ToList();
        }
    }
}
