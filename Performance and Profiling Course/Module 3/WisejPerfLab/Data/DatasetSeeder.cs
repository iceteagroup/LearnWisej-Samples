using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace WisejPerfLab.Data
{
    /// <summary>
    /// Creates the lab dataset once, so every run of every scenario starts from the same rows —
    /// the first requirement of a baseline. The counts are written to the log and belong in
    /// <c>docs/Baseline.md</c>: without them, a measurement cannot be compared with anything.
    /// </summary>
    /// <remarks>
    /// The rows are inserted with one prepared command inside one transaction rather than through EF Core.
    /// Seeding is not what this course profiles, and tracking 50,000 entities to write them once would only
    /// add minutes to the first start.
    /// </remarks>
    public static class DatasetSeeder
    {
        private static readonly string[] Regions =
            { "North", "South", "East", "West", "Central", "Nordics", "Iberia", "Benelux" };

        private static readonly string[] AccountNames =
            { "Northwind Traders", "Contoso Ltd", "Fabrikam Inc", "Adventure Works", "Tailspin Toys",
              "Wingtip Toys", "Litware Inc", "Proseware Inc", "Fourth Coffee", "Graphic Design Institute",
              "Humongous Insurance", "Lucerne Publishing", "Margies Travel", "Trey Research",
              "School of Fine Art", "Alpine Ski House", "Blue Yonder Airlines", "City Power and Light",
              "Coho Vineyard", "Consolidated Messenger" };

        private static readonly string[] Statuses = { "Open", "Waiting", "Escalated", "Closed" };
        private static readonly string[] Priorities = { "Low", "Normal", "High", "Critical" };

        private static readonly string[] Subjects =
            { "Cannot sign in after the maintenance window", "Report export stops at 40 percent",
              "Grid is slow with the full customer list", "Invoice PDF is missing the footer",
              "Session drops on the shop floor tablets", "Import job finished with warnings",
              "New user cannot see the dashboard", "Timesheet totals disagree with the export",
              "Search returns closed tickets first", "Printer queue rejects the batch" };

        /// <summary>Seeds the database when it is empty. Returns the row counts that were used.</summary>
        public static (int Tickets, int Customers) EnsureSeeded(PerfLabContext db, int ticketCount, int customerNodeCount)
        {
            var existing = db.Tickets.Count();
            if (existing > 0)
                return (existing, db.Customers.Count());

            var sw = Stopwatch.StartNew();
            var connection = (SqliteConnection)db.Database.GetDbConnection();
            if (connection.State != System.Data.ConnectionState.Open)
                connection.Open();

            // Bulk writes on SQLite: one transaction, one prepared command, no journal.
            using (var pragma = connection.CreateCommand())
            {
                pragma.CommandText = "PRAGMA journal_mode=OFF; PRAGMA synchronous=OFF;";
                pragma.ExecuteNonQuery();
            }

            var random = new Random(20260914);
            using (var transaction = connection.BeginTransaction())
            {
                var customerIds = SeedCustomers(connection, transaction, customerNodeCount);
                SeedTickets(connection, transaction, random, customerIds, ticketCount);
                transaction.Commit();
            }

            sw.Stop();
            Console.Error.WriteLine(
                $"[WisejPerfLab] seeded {ticketCount:N0} tickets and {customerNodeCount:N0} customer nodes in {sw.ElapsedMilliseconds:N0} ms");

            return (db.Tickets.Count(), db.Customers.Count());
        }

        /// <summary>Region, then account, then site: three levels and a few thousand nodes.</summary>
        private static List<int> SeedCustomers(SqliteConnection connection, SqliteTransaction transaction, int nodeCount)
        {
            var leafIds = new List<int>();
            using var insert = connection.CreateCommand();
            insert.Transaction = transaction;
            insert.CommandText =
                "INSERT INTO Customers (Id, Name, Region, ParentId) VALUES ($id, $name, $region, $parent)";
            var pId = AddParameter(insert, "$id");
            var pName = AddParameter(insert, "$name");
            var pRegion = AddParameter(insert, "$region");
            var pParent = AddParameter(insert, "$parent");

            var id = 0;
            var regionIds = new List<int>();
            foreach (var region in Regions)
            {
                id++;
                pId.Value = id;
                pName.Value = region + " region";
                pRegion.Value = region;
                pParent.Value = DBNull.Value;
                insert.ExecuteNonQuery();
                regionIds.Add(id);
            }

            var accountIds = new List<(int Id, string Region)>();
            for (var r = 0; r < regionIds.Count; r++)
            {
                for (var a = 0; a < AccountNames.Length; a++)
                {
                    id++;
                    pId.Value = id;
                    pName.Value = AccountNames[a] + " - " + Regions[r];
                    pRegion.Value = Regions[r];
                    pParent.Value = regionIds[r];
                    insert.ExecuteNonQuery();
                    accountIds.Add((id, Regions[r]));
                }
            }

            var site = 0;
            while (id < nodeCount)
            {
                var account = accountIds[site % accountIds.Count];
                id++;
                pId.Value = id;
                pName.Value = "Site " + (site / accountIds.Count + 1) + " - " + account.Region + "-" + id.ToString("D4");
                pRegion.Value = account.Region;
                pParent.Value = account.Id;
                insert.ExecuteNonQuery();
                leafIds.Add(id);
                site++;
            }

            if (leafIds.Count == 0)
                leafIds.AddRange(accountIds.Select(a => a.Id));

            return leafIds;
        }

        private static void SeedTickets(SqliteConnection connection, SqliteTransaction transaction, Random random, List<int> customerIds, int ticketCount)
        {
            using var insert = connection.CreateCommand();
            insert.Transaction = transaction;
            insert.CommandText =
                "INSERT INTO Tickets (Id, Number, CustomerId, Status, Priority, Subject, Description, Notes, CreatedAt, UpdatedAt, ClosedAt, MinutesToFirstResponse) " +
                "VALUES ($id, $number, $customer, $status, $priority, $subject, $description, $notes, $created, $updated, $closed, $minutes)";

            var pId = AddParameter(insert, "$id");
            var pNumber = AddParameter(insert, "$number");
            var pCustomer = AddParameter(insert, "$customer");
            var pStatus = AddParameter(insert, "$status");
            var pPriority = AddParameter(insert, "$priority");
            var pSubject = AddParameter(insert, "$subject");
            var pDescription = AddParameter(insert, "$description");
            var pNotes = AddParameter(insert, "$notes");
            var pCreated = AddParameter(insert, "$created");
            var pUpdated = AddParameter(insert, "$updated");
            var pClosed = AddParameter(insert, "$closed");
            var pMinutes = AddParameter(insert, "$minutes");

            var now = new DateTime(2026, 9, 14, 9, 0, 0, DateTimeKind.Unspecified);
            var filler = new string('.', 40);

            for (var i = 1; i <= ticketCount; i++)
            {
                // Weighted so roughly a quarter of the table is Open and more than half is Closed:
                // a filter that matches every row would hide the cost of the rows it should exclude.
                var roll = random.Next(100);
                var status = roll < 24 ? Statuses[0] : roll < 38 ? Statuses[1] : roll < 44 ? Statuses[2] : Statuses[3];
                var created = now.AddMinutes(-random.Next(60, 260000));
                var updated = created.AddMinutes(random.Next(5, 4000));
                var subject = Subjects[random.Next(Subjects.Length)];

                pId.Value = i;
                pNumber.Value = "T-" + (10000 + i);
                pCustomer.Value = customerIds[random.Next(customerIds.Count)];
                pStatus.Value = status;
                pPriority.Value = Priorities[random.Next(Priorities.Length)];
                pSubject.Value = subject;
                pDescription.Value = BuildLongText("Reported by the service desk. ", subject, filler, 4);
                pNotes.Value = BuildLongText("Handover note. ", subject, filler, 3);
                pCreated.Value = created.ToString("yyyy-MM-dd HH:mm:ss");
                pUpdated.Value = updated.ToString("yyyy-MM-dd HH:mm:ss");
                pClosed.Value = status == "Closed"
                    ? (object)updated.AddMinutes(random.Next(10, 900)).ToString("yyyy-MM-dd HH:mm:ss")
                    : DBNull.Value;
                pMinutes.Value = random.Next(2, 480);
                insert.ExecuteNonQuery();
            }
        }

        /// <summary>The long text columns no list screen shows, and that a SELECT * still pays for.</summary>
        private static string BuildLongText(string prefix, string subject, string filler, int repeats)
        {
            var text = new StringBuilder(prefix).Append(subject).Append(". ");
            for (var i = 0; i < repeats; i++)
                text.Append(filler).Append(' ').Append(subject).Append(". ");
            return text.ToString();
        }

        private static SqliteParameter AddParameter(SqliteCommand command, string name)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            command.Parameters.Add(parameter);
            return parameter;
        }
    }
}
