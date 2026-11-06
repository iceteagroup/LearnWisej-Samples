using System.Globalization;
using AccessOps.Infrastructure;
using Microsoft.Extensions.Logging;

namespace AccessOps.Services
{
    /// <summary>
    /// The admin console's permission editor: grants and revocations are per-employee claims in the
    /// identity store (employee_claims). Each change and its audit row commit in one transaction.
    /// A revoke writes a denial, which wins over any role; the change takes effect at the
    /// employee's next sign-in or the next "force re-resolve".
    /// </summary>
    public sealed class PermissionEditorService
    {
        private readonly IAuthorize _authorize;
        private readonly AccessOpsDatabase _db;
        private readonly AuditService _audit;
        private readonly ILogger<PermissionEditorService> _log;

        public PermissionEditorService(IAuthorize authorize, AccessOpsDatabase db, AuditService audit, ILogger<PermissionEditorService> log)
        {
            _authorize = authorize;
            _db = db;
            _audit = audit;
            _log = log;
        }

        public long Grant(CommandContext ctx, int employeeId, Permission permission) => Change(ctx, employeeId, permission, grant: true);

        public long Revoke(CommandContext ctx, int employeeId, Permission permission) => Change(ctx, employeeId, permission, grant: false);

        private long Change(CommandContext ctx, int employeeId, Permission permission, bool grant)
        {
            var action = grant ? "Permission.Grant" : "Permission.Revoke";
            var target = employeeId.ToString(CultureInfo.InvariantCulture) + "/" + permission;
            try
            {
                _authorize.Require(Permission.ManageUsers, "employee " + employeeId);
            }
            catch (NotAuthorizedException)
            {
                _audit.Record(ctx, action, "Employee", target, "Denied");
                throw;
            }

            using var connection = _db.Open();
            using var tx = connection.BeginTransaction();
            using (var remove = connection.CreateCommand())
            {
                remove.Transaction = tx;
                remove.CommandText = "DELETE FROM employee_claims WHERE employee_id = $id AND permission = $permission";
                remove.Parameters.AddWithValue("$id", employeeId);
                remove.Parameters.AddWithValue("$permission", permission.ToString());
                remove.ExecuteNonQuery();
            }
            using (var add = connection.CreateCommand())
            {
                add.Transaction = tx;
                add.CommandText = "INSERT INTO employee_claims (employee_id, kind, permission) VALUES ($id, $kind, $permission)";
                add.Parameters.AddWithValue("$id", employeeId);
                add.Parameters.AddWithValue("$kind", grant ? "grant" : "deny");
                add.Parameters.AddWithValue("$permission", permission.ToString());
                add.ExecuteNonQuery();
            }
            var seq = _audit.Record(connection, tx, ctx, action, "Employee", target, "Allowed");
            tx.Commit();

            _log.LogInformation("{Action} {Permission} for employee {EmployeeId} · audit entry {Seq}", action, permission, employeeId, seq);
            return seq;
        }
    }
}
