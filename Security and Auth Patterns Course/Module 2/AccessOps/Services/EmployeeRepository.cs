using System;
using AccessOps.Infrastructure;

namespace AccessOps.Services
{
    /// <summary>The fields sign-in needs about an employee. No pay or personal data.</summary>
    public sealed class EmployeeAccount
    {
        public int Id { get; init; }
        public string UserName { get; init; }
        public string DisplayName { get; init; }
        public string Department { get; init; }
        public string Roles { get; init; }
        public bool IsDisabled { get; init; }
        public bool IsManager { get; init; }
    }

    /// <summary>Parameterised access to the employee rows the login flow reads.</summary>
    public sealed class EmployeeRepository
    {
        private readonly AccessOpsDatabase _db;

        public EmployeeRepository(AccessOpsDatabase db) => _db = db;

        /// <summary>The stored credential for a user name, or null when there is none.</summary>
        public StoredCredential FindCredential(string userName)
        {
            using var connection = _db.Open();
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
SELECT id, pw_algorithm, pw_iterations, pw_salt, pw_hash
FROM employees
WHERE user_name = $user AND pw_hash IS NOT NULL;";
            cmd.Parameters.AddWithValue("$user", userName);
            using var r = cmd.ExecuteReader();
            if (!r.Read())
                return null;

            return new StoredCredential
            {
                EmployeeId = r.GetInt32(0),
                Algorithm = r.GetString(1),
                Iterations = r.GetInt32(2),
                Salt = (byte[])r.GetValue(3),
                Hash = (byte[])r.GetValue(4)
            };
        }

        public EmployeeAccount Find(int employeeId)
        {
            using var connection = _db.Open();
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
SELECT e.id, e.user_name, e.first_name || ' ' || e.last_name, e.department, e.roles, e.is_disabled,
       EXISTS (SELECT 1 FROM employees r WHERE r.manager_id = e.id)
FROM employees e
WHERE e.id = $id;";
            cmd.Parameters.AddWithValue("$id", employeeId);
            using var r = cmd.ExecuteReader();
            if (!r.Read())
                return null;

            return new EmployeeAccount
            {
                Id = r.GetInt32(0),
                UserName = r.IsDBNull(1) ? null : r.GetString(1),
                DisplayName = r.GetString(2),
                Department = r.GetString(3),
                Roles = r.GetString(4),
                IsDisabled = r.GetInt32(5) != 0,
                IsManager = r.GetInt32(6) != 0
            };
        }
    }
}
