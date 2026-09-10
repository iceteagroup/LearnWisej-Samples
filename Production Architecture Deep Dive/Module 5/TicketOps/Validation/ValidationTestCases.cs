using System;
using System.Collections.Generic;
using System.Linq;
using TicketOps.Domain;
using TicketOps.Infrastructure;

namespace TicketOps.Validation
{
    /// <summary>One documented case: the input, what the rules must answer, and which layer answers.</summary>
    public sealed class ValidationTestCase
    {
        public string Id { get; init; }
        public string Name { get; init; }
        /// <summary>"validator" runs WorkOrderValidator only; "rules" runs WorkOrderRules against a stored record and a role.</summary>
        public string Layer { get; init; }
        public SaveWorkOrderCommand Command { get; init; }
        public WorkOrder Stored { get; init; }
        public UserRole Role { get; init; } = UserRole.Technician;
        /// <summary>Field names that must carry an error (empty = none).</summary>
        public string[] ExpectedFields { get; init; } = new string[0];
        /// <summary>How many summary errors are expected.</summary>
        public int ExpectedSummaryErrors { get; init; }
        public bool ExpectValid => ExpectedFields.Length == 0 && ExpectedSummaryErrors == 0;
    }

    public sealed class TestCaseOutcome
    {
        public ValidationTestCase Case { get; init; }
        public bool Passed { get; init; }
        public ValidationResult Actual { get; init; }
        public string Detail { get; init; }
    }

    /// <summary>
    /// The table in docs/TestCases.md, as code. Because the validator and the rules are pure, running a
    /// case is one method call with no form on screen — the "Run test cases" button paces these through a
    /// Timer and the trace shows PASS/FAIL per case. A real project moves this file into an xUnit project
    /// unchanged; nothing here references Wisej.NET.
    /// </summary>
    public static class ValidationTestCases
    {
        public static readonly DateTime Today = new DateTime(2026, 9, 10);

        private static SaveWorkOrderCommand Valid(Action<SaveWorkOrderCommandBuilder> tweak = null)
        {
            var b = new SaveWorkOrderCommandBuilder();
            tweak?.Invoke(b);
            return b.Build();
        }

        private static WorkOrder StoredAs(WorkOrderStatus status, int id = 2002)
            => new WorkOrder { Id = id, Title = "Repair loading dock pump", AssigneeId = "T. Nguyen", DueDate = Today.AddDays(10), EstimatedCost = 2150m, EstimatedHours = 6, Status = status };

        public static IReadOnlyList<ValidationTestCase> All()
        {
            return new List<ValidationTestCase>
            {
                new ValidationTestCase { Id = "TC-01", Name = "valid edit passes every rule", Layer = "validator", Command = Valid() },
                new ValidationTestCase { Id = "TC-02", Name = "empty title", Layer = "validator", Command = Valid(b => b.Title = "   "), ExpectedFields = new[] { "Title" } },
                new ValidationTestCase { Id = "TC-03", Name = "title longer than 120 characters", Layer = "validator", Command = Valid(b => b.Title = new string('x', 121)), ExpectedFields = new[] { "Title" } },
                new ValidationTestCase { Id = "TC-04", Name = "no assignee", Layer = "validator", Command = Valid(b => b.AssigneeId = ""), ExpectedFields = new[] { "AssigneeId" } },
                new ValidationTestCase { Id = "TC-05", Name = "cost above $10,000", Layer = "validator", Command = Valid(b => b.EstimatedCost = 25000m), ExpectedFields = new[] { "EstimatedCost" } },
                new ValidationTestCase { Id = "TC-06", Name = "hours out of range (1,200)", Layer = "validator", Command = Valid(b => b.EstimatedHours = 1200), ExpectedFields = new[] { "EstimatedHours" } },
                new ValidationTestCase { Id = "TC-07", Name = "due date in the past on an open order", Layer = "validator", Command = Valid(b => b.DueDate = Today.AddDays(-8)), ExpectedFields = new[] { "DueDate" } },
                new ValidationTestCase { Id = "TC-08", Name = "closed order with a future due date", Layer = "validator", Command = Valid(b => { b.FromStatus = WorkOrderStatus.Completed; b.ToStatus = WorkOrderStatus.Closed; b.DueDate = Today.AddDays(10); }), ExpectedFields = new[] { "DueDate" } },
                new ValidationTestCase { Id = "TC-09", Name = "illegal transition New → Completed", Layer = "validator", Command = Valid(b => { b.FromStatus = WorkOrderStatus.New; b.ToStatus = WorkOrderStatus.Completed; }), ExpectedSummaryErrors = 1 },
                new ValidationTestCase { Id = "TC-10", Name = "three problems collected at once", Layer = "validator", Command = Valid(b => { b.Title = ""; b.DueDate = Today.AddDays(-8); b.EstimatedCost = 25000m; }), ExpectedFields = new[] { "Title", "DueDate", "EstimatedCost" } },
                new ValidationTestCase { Id = "TC-11", Name = "closed order edited by a Technician", Layer = "rules", Stored = StoredAs(WorkOrderStatus.Closed, 2006), Command = Valid(b => { b.Id = 2006; b.FromStatus = WorkOrderStatus.Closed; b.ToStatus = WorkOrderStatus.Closed; b.DueDate = Today.AddDays(-12); }), Role = UserRole.Technician, ExpectedSummaryErrors = 1 },
                new ValidationTestCase { Id = "TC-12", Name = "Supervisor reopens a closed order", Layer = "rules", Stored = StoredAs(WorkOrderStatus.Closed, 2006), Command = Valid(b => { b.Id = 2006; b.FromStatus = WorkOrderStatus.Closed; b.ToStatus = WorkOrderStatus.Assigned; }), Role = UserRole.Supervisor },
                new ValidationTestCase { Id = "TC-13", Name = "Technician sets cost above the $2,500 threshold", Layer = "rules", Stored = StoredAs(WorkOrderStatus.Assigned), Command = Valid(b => b.EstimatedCost = 9500m), Role = UserRole.Technician, ExpectedSummaryErrors = 1 },
                new ValidationTestCase { Id = "TC-14", Name = "Supervisor sets cost above the threshold", Layer = "rules", Stored = StoredAs(WorkOrderStatus.Assigned), Command = Valid(b => b.EstimatedCost = 9500m), Role = UserRole.Supervisor },
                new ValidationTestCase { Id = "TC-15", Name = "stale editor: stored status moved on", Layer = "rules", Stored = StoredAs(WorkOrderStatus.Completed), Command = Valid(b => { b.FromStatus = WorkOrderStatus.Assigned; b.ToStatus = WorkOrderStatus.InProgress; }), Role = UserRole.Supervisor, ExpectedSummaryErrors = 1 },
            };
        }
    }

    /// <summary>Mutable helper so a test case can describe itself as "the valid command, except …".</summary>
    public sealed class SaveWorkOrderCommandBuilder
    {
        public int? Id = 2002;
        public string Title = "Repair loading dock pump";
        public string AssigneeId = "T. Nguyen";
        public WorkOrderPriority Priority = WorkOrderPriority.High;
        public DateTime DueDate = ValidationTestCases.Today.AddDays(10);
        public decimal EstimatedCost = 2150m;
        public double EstimatedHours = 6;
        public WorkOrderStatus FromStatus = WorkOrderStatus.Assigned;
        public WorkOrderStatus ToStatus = WorkOrderStatus.InProgress;

        public SaveWorkOrderCommand Build() => new SaveWorkOrderCommand
        {
            Id = Id, Title = Title, AssigneeId = AssigneeId, Priority = Priority, DueDate = DueDate,
            EstimatedCost = EstimatedCost, EstimatedHours = EstimatedHours, FromStatus = FromStatus, ToStatus = ToStatus
        };
    }

    /// <summary>
    /// Runs one documented case against the real validator / rules and compares with the expectation.
    /// No UI type: the same class would sit inside a unit-test project. It logs at the DOMAIN layer because
    /// that is whose answer it is checking.
    /// </summary>
    public sealed class ValidationTestRunner
    {
        private readonly WorkOrderValidator _validator;
        private readonly ILog _log;

        public ValidationTestRunner(ILog log)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _validator = new WorkOrderValidator(() => ValidationTestCases.Today);   // pinned "today" so the cases never rot
        }

        public TestCaseOutcome Run(ValidationTestCase tc)
        {
            if (tc == null) throw new ArgumentNullException(nameof(tc));

            ValidationResult actual = tc.Layer == "rules"
                ? WorkOrderRules.Check(tc.Stored, tc.Command, tc.Role)
                : _validator.Validate(tc.Command);

            var actualFields = actual.FieldErrors.Select(e => e.Field).OrderBy(f => f).ToArray();
            var expectedFields = tc.ExpectedFields.OrderBy(f => f).ToArray();
            int actualSummary = actual.SummaryErrors.Count();

            bool passed = actualFields.SequenceEqual(expectedFields) && actualSummary == tc.ExpectedSummaryErrors;

            string expected = tc.ExpectValid ? "valid" : Describe(expectedFields, tc.ExpectedSummaryErrors);
            string got = actual.IsValid ? "valid" : Describe(actualFields, actualSummary);
            string detail = $"{tc.Id} \"{tc.Name}\" · expected {expected} · got {got}"
                          + (actual.IsValid ? "" : $" · \"{actual.Errors[0].Message}\"");

            string source = tc.Layer == "rules" ? "WorkOrderRules.Check" : "WorkOrderValidator.Validate";
            if (passed)
                _log.Info(LogLayer.Domain, source, detail + " → PASS");
            else
                _log.Warn(LogLayer.Domain, source, detail + " → FAIL");

            return new TestCaseOutcome { Case = tc, Passed = passed, Actual = actual, Detail = detail };
        }

        private static string Describe(string[] fields, int summary)
        {
            var parts = new List<string>();
            if (fields.Length > 0) parts.Add(string.Join("+", fields));
            if (summary > 0) parts.Add($"{summary} summary");
            return string.Join(" + ", parts);
        }
    }
}
