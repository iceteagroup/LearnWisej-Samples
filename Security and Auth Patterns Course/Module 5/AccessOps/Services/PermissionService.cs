using System;
using System.Collections.Generic;
using System.Linq;
using AccessOps.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace AccessOps.Services
{
    /// <summary>A per-employee grant or denial, from the employee_claims table (the identity store).</summary>
    public sealed record PermissionClaim(string Kind, Permission Permission);

    /// <summary>
    /// The only place roles, claims and permissions meet. Roles expand through the map in
    /// configuration ("Permissions:Roles"), claims add grants and remove denials - denial wins -
    /// and the answer is an immutable set resolved once, at sign-in.
    /// </summary>
    public sealed class PermissionService
    {
        private readonly IReadOnlyDictionary<string, Permission[]> _map;
        private readonly AccessOpsDatabase _db;

        public PermissionService(IConfiguration configuration, AccessOpsDatabase db)
            : this(ReadMap(configuration), db) { }

        public PermissionService(IReadOnlyDictionary<string, Permission[]> roleMap, AccessOpsDatabase db)
        {
            _map = roleMap;
            _db = db;
        }

        public PermissionSet ResolveFor(EmployeeAccount employee)
            => ResolveFor(employee.Roles, Claims(employee.Id));

        public PermissionSet ResolveFor(string roles, IEnumerable<PermissionClaim> claims)
        {
            var set = new HashSet<Permission>(PermissionsForRoles(roles));
            var list = (claims ?? Enumerable.Empty<PermissionClaim>()).ToList();
            set.UnionWith(list.Where(c => c.Kind == "grant").Select(c => c.Permission));
            set.ExceptWith(list.Where(c => c.Kind == "deny").Select(c => c.Permission));   // denial wins
            return PermissionSet.Immutable(set);
        }

        public IEnumerable<Permission> PermissionsForRoles(string roles)
        {
            foreach (var role in (roles ?? "").Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                if (_map.TryGetValue(role, out var permissions))
                {
                    foreach (var p in permissions)
                        yield return p;
                }
            }
        }

        public IReadOnlyList<PermissionClaim> Claims(int employeeId)
        {
            using var connection = _db.Open();
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT kind, permission FROM employee_claims WHERE employee_id = $id";
            cmd.Parameters.AddWithValue("$id", employeeId);
            var claims = new List<PermissionClaim>();
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                if (Enum.TryParse<Permission>(r.GetString(1), out var p))
                    claims.Add(new PermissionClaim(r.GetString(0), p));
            }
            return claims;
        }

        private static IReadOnlyDictionary<string, Permission[]> ReadMap(IConfiguration configuration)
        {
            var map = new Dictionary<string, Permission[]>(StringComparer.Ordinal);
            foreach (var role in configuration.GetSection("Permissions:Roles").GetChildren())
            {
                map[role.Key] = role.GetChildren()
                    .Select(c => Enum.Parse<Permission>(c.Value))
                    .ToArray();
            }
            return map;
        }
    }
}
