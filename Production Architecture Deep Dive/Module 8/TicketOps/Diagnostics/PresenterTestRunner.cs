using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketOps.Data;
using TicketOps.Domain;
using TicketOps.Infrastructure;
using TicketOps.Services;

namespace TicketOps.Diagnostics
{
    /// <summary>The verdict of one presenter test.</summary>
    public sealed class PresenterTestResult
    {
        public string Name { get; init; }
        public bool Passed { get; init; }
        public string Detail { get; init; }
    }

    /// <summary>
    /// The presenter's unit tests: every test builds the presenter with hand-written fakes — no container,
    /// no Form, no browser — calls one method and checks the outcome. It is plain C# (a test project would
    /// host the same methods under [Test]). Nothing here touches the Form's injected services: that
    /// independence is the point.
    /// </summary>
    public sealed class PresenterTestRunner
    {
        private readonly ILog _log;
        private readonly List<(string Name, Func<Task> Body)> _tests;

        public PresenterTestRunner(ILog log)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _tests = new List<(string, Func<Task>)>
            {
                ("Close_reports_success_and_records_an_audit_entry", Close_reports_success_and_records_an_audit_entry),
                ("Close_is_denied_for_a_viewer", Close_is_denied_for_a_viewer),
                ("Close_is_denied_when_the_permission_fake_says_no", Close_is_denied_when_the_permission_fake_says_no),
                ("Close_without_hours_is_rejected_by_the_domain_rule", Close_without_hours_is_rejected_by_the_domain_rule),
                ("Close_without_a_reason_never_reaches_a_service", Close_without_a_reason_never_reaches_a_service),
                ("Close_of_an_unknown_ticket_reports_not_found", Close_of_an_unknown_ticket_reports_not_found),
                ("Assign_to_me_notifies_the_new_assignee", Assign_to_me_notifies_the_new_assignee),
                ("Data_outage_surfaces_as_an_exception_not_a_result", Data_outage_surfaces_as_an_exception_not_a_result)
            };
        }

        public int Count => _tests.Count;

        public IReadOnlyList<string> Names => _tests.Select(t => t.Name).ToList();

        /// <summary>Runs test <paramref name="index"/> and turns its outcome into a result (an assertion failure never escapes).</summary>
        public async Task<PresenterTestResult> RunAsync(int index)
        {
            var (name, body) = _tests[index];
            try
            {
                await body();
                _log.Info(LogLayer.Infrastructure, "PresenterTests", $"✔ {index + 1}/{Count} {name}");
                return new PresenterTestResult { Name = name, Passed = true, Detail = "passed" };
            }
            catch (Exception ex)
            {
                _log.Error(LogLayer.Infrastructure, "PresenterTests", ex, $"✖ {index + 1}/{Count} {name} — {ex.Message}");
                return new PresenterTestResult { Name = name, Passed = false, Detail = ex.Message };
            }
        }

        // ---- the fixture: fresh fakes for every test, wired by constructor ---------------------------

        private sealed class Fixture
        {
            public readonly FakeTicketService Tickets;
            public readonly FakeUserService Users;
            public readonly FakePermissionService Permissions;
            public readonly FakeNotificationService Notifications;
            public readonly FakeAuditLogService Audit;
            public readonly TicketWorkflowPresenter Presenter;

            public Fixture(ILog log)
            {
                Tickets = new FakeTicketService(log);
                Users = new FakeUserService();
                Permissions = new FakePermissionService(Users);
                Notifications = new FakeNotificationService(log);
                Audit = new FakeAuditLogService();
                Presenter = new TicketWorkflowPresenter(Tickets, Users, Permissions, Notifications, Audit, log);
            }
        }

        private static void Assert(bool condition, string message)
        {
            if (!condition)
                throw new InvalidOperationException("assertion failed: " + message);
        }

        // ---- the tests -------------------------------------------------------------------------------

        private async Task Close_reports_success_and_records_an_audit_entry()
        {
            var f = new Fixture(_log);                                          // Dana Reyes, Technician, owns #1041 (1.5 h)

            var result = await f.Presenter.CloseAsync(1041, "Fixed on site");

            Assert(result.Outcome == WorkflowOutcome.Ok, $"expected Ok, got {result}");
            Assert(result.Ticket.Status == TicketStatus.Closed, "ticket should be Closed");
            Assert(f.Audit.Count == 1 && f.Audit.Recent(1)[0].Action == "close", "one 'close' audit entry expected");
            Assert(f.Notifications.Sent.Count == 1 && f.Notifications.Sent[0].OperatorId == 1, "the assignee should be notified");
        }

        private async Task Close_is_denied_for_a_viewer()
        {
            var f = new Fixture(_log);
            f.Users.SignInAs(4);                                                // Sam Okafor, Viewer

            var result = await f.Presenter.CloseAsync(1041, "Trying anyway");

            Assert(result.Outcome == WorkflowOutcome.Denied, $"expected Denied, got {result}");
            Assert(f.Audit.Count == 0, "nothing should be audited");
            Assert((await f.Tickets.FindAsync(1041)).Status != TicketStatus.Closed, "the ticket must still be open");
        }

        private async Task Close_is_denied_when_the_permission_fake_says_no()
        {
            var f = new Fixture(_log);
            f.Permissions.DenyEverything = true;                                // steer the fake — no roles to arrange

            var result = await f.Presenter.CloseAsync(1041, "Fixed on site");

            Assert(result.Outcome == WorkflowOutcome.Denied, $"expected Denied, got {result}");
            Assert(f.Notifications.Sent.Count == 0, "nobody should be notified");
        }

        private async Task Close_without_hours_is_rejected_by_the_domain_rule()
        {
            var f = new Fixture(_log);                                          // #1042: Dana's, 0 h logged

            var result = await f.Presenter.CloseAsync(1042, "Closing early");

            Assert(result.Outcome == WorkflowOutcome.Invalid, $"expected Invalid, got {result}");
            Assert(result.Message == "Log hours before closing.", $"unexpected message: {result.Message}");
            Assert(f.Audit.Count == 0, "a rejected close must not be audited");
        }

        private async Task Close_without_a_reason_never_reaches_a_service()
        {
            var f = new Fixture(_log);

            var result = await f.Presenter.CloseAsync(1041, "   ");

            Assert(result.Outcome == WorkflowOutcome.Invalid, $"expected Invalid, got {result}");
            Assert((await f.Tickets.FindAsync(1041)).Status != TicketStatus.Closed, "the ticket must still be open");
            Assert(f.Audit.Count == 0 && f.Notifications.Sent.Count == 0, "no side effects expected");
        }

        private async Task Close_of_an_unknown_ticket_reports_not_found()
        {
            var f = new Fixture(_log);

            var result = await f.Presenter.CloseAsync(9999, "Ghost ticket");

            Assert(result.Outcome == WorkflowOutcome.NotFound, $"expected NotFound, got {result}");
        }

        private async Task Assign_to_me_notifies_the_new_assignee()
        {
            var f = new Fixture(_log);                                          // #1044 is unassigned

            var result = await f.Presenter.AssignToMeAsync(1044);

            Assert(result.Outcome == WorkflowOutcome.Ok, $"expected Ok, got {result}");
            Assert(result.Ticket.AssigneeId == 1, "ticket should now belong to operator 1");
            Assert(f.Notifications.Sent.Count == 1 && f.Notifications.Sent[0].OperatorId == 1, "the new assignee should be notified");
            Assert(f.Audit.Recent(1)[0].Action == "assign", "an 'assign' audit entry expected");
        }

        private async Task Data_outage_surfaces_as_an_exception_not_a_result()
        {
            var f = new Fixture(_log);
            var presenter = new TicketWorkflowPresenter(new UnavailableTicketService(), f.Users, f.Permissions, f.Notifications, f.Audit, _log);

            try
            {
                await presenter.CloseAsync(1041, "Fixed on site");
                Assert(false, "expected a StoreUnavailableException");
            }
            catch (StoreUnavailableException)
            {
                // expected: an unexpected failure is an exception for the handler to catch, not a result for the user to read
            }

            Assert(f.Audit.Count == 0, "nothing should be audited during an outage");
        }

        // ---- test doubles ------------------------------------------------------------------------------

        private sealed class StoreUnavailableException : Exception
        {
            public StoreUnavailableException() : base("The ticket store did not answer.") { }
        }

        /// <summary>A ticket service whose store is down: every call throws.</summary>
        private sealed class UnavailableTicketService : ITicketService
        {
            public Task<IReadOnlyList<Ticket>> GetOpenTicketsAsync() => throw new StoreUnavailableException();
            public Task<Ticket> FindAsync(int ticketId) => throw new StoreUnavailableException();
            public Task<OperationResult<Ticket>> CloseAsync(int ticketId, string reason) => throw new StoreUnavailableException();
            public Task<OperationResult<Ticket>> AssignAsync(int ticketId, int operatorId) => throw new StoreUnavailableException();
        }
    }
}
