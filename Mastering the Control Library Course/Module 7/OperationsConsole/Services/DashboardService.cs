using System;
using System.Collections.Generic;
using System.Threading;
using OperationsConsole.Models;

namespace OperationsConsole.Services
{
    /// <summary>
    /// The dashboard's data service. It owns a small in-memory ticket table and never lets it out:
    /// <see cref="GetDashboard"/> returns a <see cref="DashboardModel"/> with six monthly aggregates, one completion
    /// percentage and one document reference. <see cref="SimulateFailure"/> makes the call throw and
    /// <see cref="LatencyMs"/> keeps it slow enough for the loading state to be seen.
    /// </summary>
    public sealed class DashboardService
    {
        // The shape of the last six months of the Operations Console ticket queue (the numbers the walkthrough
        // video shows). The generated ticket table below reproduces these aggregates exactly.
        private static readonly int[] OpenedPattern = { 42, 51, 38, 60, 55, 47 };
        private static readonly int[] ClosedPattern = { 39, 47, 41, 52, 58, 50 };

        /// <summary>How many tickets the team is expected to close in a month (the denominator of the gauge).</summary>
        public const int MonthlyTarget = 64;

        /// <summary>The service level the console is measured against — the gauge is green from here up.</summary>
        public const int TargetPercent = 85;

        private readonly DocumentStore _documents;
        private readonly List<Ticket> _tickets = new List<Ticket>();
        private readonly Queue<Ticket> _open = new Queue<Ticket>();
        private readonly DateTime[] _months;
        private int _refreshCount;
        private int _nextTicketId;

        public DashboardService(DocumentStore documents)
        {
            _documents = documents;
            _months = LastSixMonths();
            SeedTicketTable();
        }

        /// <summary>Milliseconds <see cref="GetDashboard"/> spends "querying", so the loading state is visible.</summary>
        public int LatencyMs { get; set; } = 650;

        /// <summary>When true, <see cref="GetDashboard"/> throws.</summary>
        public bool SimulateFailure { get; set; }

        /// <summary>The one call the dashboard makes. Aggregates only — the ticket table never leaves this object.</summary>
        public DashboardModel GetDashboard()
        {
            if (SimulateFailure)
                throw new InvalidOperationException("The dashboard service did not answer.");

            if (LatencyMs > 0)
                Thread.Sleep(LatencyMs);   // the query the loading state is covering for

            // Every refresh moves the queue on a little, so the chart, the gauge and the stamp visibly change.
            _refreshCount++;
            AddActivity(2 + (_refreshCount % 3), 1 + (_refreshCount % 4));

            return BuildModel();
        }

        /// <summary>
        /// The first paint, taken when the page is shown: same aggregation, no latency, no drift and no failure —
        /// the dashboard must never open empty just because the service is slow.
        /// </summary>
        public DashboardModel GetInitialDashboard() => BuildModel();

        // ----------------------------------------------------------------------------------------------------
        // Aggregation: a few hundred rows in, twelve numbers out.
        // ----------------------------------------------------------------------------------------------------

        private DashboardModel BuildModel()
        {
            var labels = new List<string>(_months.Length);
            var opened = new List<int>(_months.Length);
            var closed = new List<int>(_months.Length);

            foreach (var month in _months)
            {
                labels.Add(month.ToString("MMM"));
                opened.Add(CountOpenedIn(month));
                closed.Add(CountClosedIn(month));
            }

            var closedThisMonth = closed[closed.Count - 1];
            var completion = (int)Math.Round(100.0 * closedThisMonth / MonthlyTarget, MidpointRounding.AwayFromZero);
            if (completion < 0) completion = 0;
            if (completion > 100) completion = 100;

            return new DashboardModel(
                labels,
                opened,
                closed,
                completion,
                TargetPercent,
                DescribePreviewDocument(),
                DateTime.Now);
        }

        /// <summary>
        /// The document reference the preview is fed from: the last upload if the session has one, otherwise the
        /// sample report that ships in <c>wwwroot/</c>. A reference — id, title, url — never the bytes.
        /// </summary>
        private PreviewDocument DescribePreviewDocument()
        {
            var latest = _documents?.Latest;
            if (latest != null)
                return new PreviewDocument(latest.Id, latest.FileName, null, true, latest.SizeBytes, latest.ReceivedAt);

            return new PreviewDocument(
                "DOC-SAMPLE",
                "SLA report — sample",
                "wwwroot/sample-report.pdf",
                false,
                0,
                DateTime.Today);
        }

        private int CountOpenedIn(DateTime month)
        {
            var count = 0;
            foreach (var ticket in _tickets)
                if (IsSameMonth(ticket.OpenedOn, month))
                    count++;

            return count;
        }

        private int CountClosedIn(DateTime month)
        {
            var count = 0;
            foreach (var ticket in _tickets)
                if (ticket.ClosedOn.HasValue && IsSameMonth(ticket.ClosedOn.Value, month))
                    count++;

            return count;
        }

        private static bool IsSameMonth(DateTime value, DateTime month) =>
            value.Year == month.Year && value.Month == month.Month;

        // ----------------------------------------------------------------------------------------------------
        // The in-memory ticket table (generated, deterministic, never returned to the caller).
        // ----------------------------------------------------------------------------------------------------

        private void SeedTicketTable()
        {
            for (var i = 0; i < _months.Length; i++)
            {
                var month = _months[i];
                OpenTickets(OpenedPattern[i], month);
                CloseTickets(ClosedPattern[i], month);
            }
        }

        /// <summary>The drift a refresh picks up: a few new tickets opened today and a few of the oldest closed.</summary>
        private void AddActivity(int newlyOpened, int newlyClosed)
        {
            var thisMonth = _months[_months.Length - 1];
            OpenTickets(newlyOpened, thisMonth);
            CloseTickets(newlyClosed, thisMonth);
        }

        private void OpenTickets(int count, DateTime month)
        {
            for (var i = 0; i < count; i++)
            {
                _nextTicketId++;
                var ticket = new Ticket(_nextTicketId, DayIn(month, i));
                _tickets.Add(ticket);
                _open.Enqueue(ticket);
            }
        }

        private void CloseTickets(int count, DateTime month)
        {
            for (var i = 0; i < count && _open.Count > 0; i++)
                _open.Dequeue().ClosedOn = DayIn(month, i + 1);
        }

        /// <summary>A stable day inside the month, so the generated table is the same on every run.</summary>
        private static DateTime DayIn(DateTime month, int index)
        {
            var days = DateTime.DaysInMonth(month.Year, month.Month);
            var day = 1 + (index % days);
            var candidate = new DateTime(month.Year, month.Month, Math.Min(day, days));

            // Never date a ticket in the future: the current month is only as long as today.
            var today = DateTime.Today;
            if (candidate.Year == today.Year && candidate.Month == today.Month && candidate.Day > today.Day)
                candidate = today;

            return candidate;
        }

        private static DateTime[] LastSixMonths()
        {
            var first = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-5);
            var months = new DateTime[6];
            for (var i = 0; i < months.Length; i++)
                months[i] = first.AddMonths(i);

            return months;
        }

        /// <summary>
        /// One row of the table the dashboard never returns. Private on purpose: nothing outside this service
        /// can take a dependency on the ticket shape, which is what keeps the payload an aggregate.
        /// </summary>
        private sealed class Ticket
        {
            public Ticket(int id, DateTime openedOn)
            {
                Id = id;
                OpenedOn = openedOn;
            }

            public int Id { get; }

            public DateTime OpenedOn { get; }

            public DateTime? ClosedOn { get; set; }
        }
    }
}
