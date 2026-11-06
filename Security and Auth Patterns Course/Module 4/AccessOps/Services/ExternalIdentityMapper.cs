using System;
using System.Globalization;
using AccessOps.Infrastructure;

namespace AccessOps.Services
{
    public enum ExternalMapOutcome { Linked, Provisioned, Rejected }

    public sealed record ExternalMapResult(ExternalMapOutcome Outcome, int EmployeeId);

    /// <summary>
    /// The decision point after the provider has proven who someone is. Finds the employee by
    /// issuer + subject (never by email, which can be reissued), provisions an employee with no
    /// roles on first login - in the same transaction as the audit row - and refuses a disabled
    /// account. The provider answers "who"; PermissionService still answers "what they may do":
    /// nothing in the token is turned into a permission.
    /// </summary>
    public sealed class ExternalIdentityMapper
    {
        private readonly AccessOpsDatabase _db;

        public ExternalIdentityMapper(AccessOpsDatabase db) => _db = db;

        public ExternalMapResult Map(ExternalIdentity identity, string clientAddress)
        {
            using var connection = _db.Open();
            using var tx = connection.BeginTransaction();

            int? employeeId = null;
            using (var find = connection.CreateCommand())
            {
                find.Transaction = tx;
                find.CommandText = "SELECT employee_id FROM external_logins WHERE issuer = $issuer AND subject = $subject";
                find.Parameters.AddWithValue("$issuer", identity.Issuer);
                find.Parameters.AddWithValue("$subject", identity.Subject);
                var found = find.ExecuteScalar();
                if (found != null)
                    employeeId = Convert.ToInt32(found);
            }

            ExternalMapOutcome outcome;
            if (employeeId == null)
            {
                employeeId = Provision(connection, tx, identity);
                outcome = ExternalMapOutcome.Provisioned;
            }
            else
            {
                using var disabled = connection.CreateCommand();
                disabled.Transaction = tx;
                disabled.CommandText = "SELECT is_disabled FROM employees WHERE id = $id";
                disabled.Parameters.AddWithValue("$id", employeeId.Value);
                outcome = Convert.ToInt32(disabled.ExecuteScalar()) != 0 ? ExternalMapOutcome.Rejected : ExternalMapOutcome.Linked;
            }

            // the audit row shares the transaction: a provisioned employee without its audit row is not a state we can reach
            using (var audit = connection.CreateCommand())
            {
                audit.Transaction = tx;
                audit.CommandText = @"
INSERT INTO login_audit (at_utc, user_name_tried, client_address, outcome)
VALUES ($at, $user, $address, $outcome);";
                audit.Parameters.AddWithValue("$at", DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture));
                audit.Parameters.AddWithValue("$user", "oidc:" + identity.Subject);
                audit.Parameters.AddWithValue("$address", clientAddress ?? "unknown");
                audit.Parameters.AddWithValue("$outcome", outcome switch
                {
                    ExternalMapOutcome.Provisioned => LoginOutcome.ExternalProvisioned.ToString(),
                    ExternalMapOutcome.Rejected => LoginOutcome.ExternalRejected.ToString(),
                    _ => LoginOutcome.ExternalSuccess.ToString()
                });
                audit.ExecuteNonQuery();
            }

            tx.Commit();
            return new ExternalMapResult(outcome, employeeId.Value);
        }

        private static int Provision(Microsoft.Data.Sqlite.SqliteConnection connection, Microsoft.Data.Sqlite.SqliteTransaction tx, ExternalIdentity identity)
        {
            using var insert = connection.CreateCommand();
            insert.Transaction = tx;
            // no roles: a first login can see its own dashboard and nothing else until an administrator assigns a role
            insert.CommandText = @"
INSERT INTO employees (first_name, last_name, email, phone, home_address, date_of_birth, hire_date, department, role, roles,
                       is_disabled, manager_id, salary_band, review_note, cost_centre)
VALUES ($first, $last, $email, '', '', '', $hired, 'Unassigned', 'New joiner', '', 0, NULL, '', '', '');
SELECT last_insert_rowid();";
            insert.Parameters.AddWithValue("$first", identity.GivenName ?? "");
            insert.Parameters.AddWithValue("$last", identity.FamilyName ?? identity.Subject);
            insert.Parameters.AddWithValue("$email", identity.Email ?? "");
            insert.Parameters.AddWithValue("$hired", DateTime.UtcNow.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            var id = Convert.ToInt32(insert.ExecuteScalar());

            using var link = connection.CreateCommand();
            link.Transaction = tx;
            link.CommandText = "INSERT INTO external_logins (issuer, subject, employee_id) VALUES ($issuer, $subject, $id)";
            link.Parameters.AddWithValue("$issuer", identity.Issuer);
            link.Parameters.AddWithValue("$subject", identity.Subject);
            link.Parameters.AddWithValue("$id", id);
            link.ExecuteNonQuery();
            return id;
        }
    }
}
