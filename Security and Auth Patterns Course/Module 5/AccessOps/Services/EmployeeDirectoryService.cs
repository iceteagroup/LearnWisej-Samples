using System.Collections.Generic;
using AccessOps.Infrastructure;

namespace AccessOps.Services
{
    /// <summary>
    /// One row of the employee directory: the projection every viewer may see. Nothing else is
    /// in this type, so nothing else can be bound to a grid and serialized to the browser.
    /// Salary band, review note, address, phone and date of birth stay in the database.
    /// </summary>
    public sealed class EmployeeListItem
    {
        public int Id { get; init; }
        public string DisplayName { get; init; }
        public string Department { get; init; }
        public string Role { get; init; }
        public string ManagerName { get; init; }
    }

    /// <summary>
    /// The query decides what leaves the server, not a designer property. The directory screen
    /// gets a projection; there is no method here that returns a whole employee record.
    /// </summary>
    public sealed class EmployeeDirectoryService
    {
        private readonly AccessOpsDatabase _db;
        private readonly IAuthorize _authorize;

        public EmployeeDirectoryService(AccessOpsDatabase db, IAuthorize authorize)
        {
            _db = db;
            _authorize = authorize;
        }

        public IReadOnlyList<EmployeeListItem> GetDirectory()
        {
            // managers and administrators use the directory; the screen hides it from everyone else
            _authorize.RequireAny(Permission.ApproveRequest, Permission.ManageUsers);

            using var connection = _db.Open();
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
SELECT e.id,
       e.first_name || ' ' || e.last_name,
       e.department,
       e.role,
       CASE WHEN m.id IS NULL THEN '' ELSE substr(m.first_name, 1, 1) || '. ' || m.last_name END
FROM employees e
LEFT JOIN employees m ON m.id = e.manager_id
ORDER BY e.id;";

            var rows = new List<EmployeeListItem>();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                rows.Add(new EmployeeListItem
                {
                    Id = reader.GetInt32(0),
                    DisplayName = reader.GetString(1),
                    Department = reader.GetString(2),
                    Role = reader.GetString(3),
                    ManagerName = reader.GetString(4)
                });
            }
            return rows;
        }
    }
}
