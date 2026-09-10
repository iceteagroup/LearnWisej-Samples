using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace TicketOps.Domain
{
    /// <summary>
    /// The rules one CSV row must satisfy to become a <see cref="Ticket"/>. Pure functions with no I/O and no
    /// UI: the import service calls them once per row, a unit test calls them with a string.
    ///
    /// A row that breaks a rule is an expected outcome (the reason comes back as text), never an exception:
    /// that is what lets the import log the row and keep going. The tables below are constants, not state —
    /// the only kind of static a per-session application may share.
    /// </summary>
    public static class TicketImportRules
    {
        public const int MaxTitleLength = 80;
        public const string DueDateFormat = "yyyy-MM-dd";

        /// <summary>The header the import expects, in any order.</summary>
        public static readonly string[] RequiredColumns =
            { "TicketId", "Title", "Priority", "Assignee", "DueDate", "HoursLogged" };

        /// <summary>The user directory the sample knows. Row 287 of the sample file names "kbo", who is not in it.</summary>
        public static readonly string[] KnownAssignees = { "ana", "jle", "mrt", "pko", "sva" };

        /// <summary>Splits the header line into column name → index.</summary>
        public static IReadOnlyDictionary<string, int> ParseHeader(string headerLine)
        {
            var columns = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            if (string.IsNullOrWhiteSpace(headerLine))
                return columns;

            string[] names = headerLine.Split(',');
            for (int i = 0; i < names.Length; i++)
            {
                string name = names[i].Trim();
                if (name.Length > 0 && !columns.ContainsKey(name))
                    columns[name] = i;
            }
            return columns;
        }

        /// <summary>The required columns the header does not have (empty when the file is usable).</summary>
        public static IReadOnlyList<string> MissingColumns(IReadOnlyDictionary<string, int> columns)
            => RequiredColumns.Where(c => !columns.ContainsKey(c)).ToList();

        /// <summary>
        /// Parses one data line. Returns false with a one-sentence reason when the row breaks a rule
        /// (missing title, unknown priority, unknown assignee, impossible date, negative hours).
        /// </summary>
        public static bool TryParseRow(string line, IReadOnlyDictionary<string, int> columns, out Ticket ticket, out string reason)
        {
            ticket = null;
            string[] cells = (line ?? "").Split(',');

            string Cell(string column)
            {
                int index = columns[column];
                return index < cells.Length ? cells[index].Trim() : "";
            }

            if (cells.Length < columns.Count)
            {
                reason = $"expected {columns.Count} columns, found {cells.Length}";
                return false;
            }

            if (!int.TryParse(Cell("TicketId"), NumberStyles.Integer, CultureInfo.InvariantCulture, out int id) || id <= 0)
            {
                reason = $"invalid ticket id \"{Cell("TicketId")}\"";
                return false;
            }

            string title = Cell("Title");
            if (title.Length == 0)
            {
                reason = "title is required";
                return false;
            }
            if (title.Length > MaxTitleLength)
            {
                reason = $"title longer than {MaxTitleLength} characters";
                return false;
            }

            if (!Enum.TryParse(Cell("Priority"), true, out TicketPriority priority))
            {
                reason = $"unknown priority \"{Cell("Priority")}\"";
                return false;
            }

            string assignee = Cell("Assignee").ToLowerInvariant();
            if (Array.IndexOf(KnownAssignees, assignee) < 0)
            {
                reason = $"unknown assignee \"{Cell("Assignee")}\"";
                return false;
            }

            if (!DateTime.TryParseExact(Cell("DueDate"), DueDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dueDate))
            {
                reason = $"invalid due date \"{Cell("DueDate")}\"";
                return false;
            }

            if (!double.TryParse(Cell("HoursLogged"), NumberStyles.Float, CultureInfo.InvariantCulture, out double hours) || hours < 0 || hours > 999)
            {
                reason = $"invalid hours logged \"{Cell("HoursLogged")}\"";
                return false;
            }

            ticket = new Ticket
            {
                Id = id,
                Title = title,
                Priority = priority,
                Assignee = assignee,
                DueDate = dueDate,
                HoursLogged = hours,
                Status = hours > 0 ? TicketStatus.InProgress : TicketStatus.Open
            };
            reason = null;
            return true;
        }
    }
}
