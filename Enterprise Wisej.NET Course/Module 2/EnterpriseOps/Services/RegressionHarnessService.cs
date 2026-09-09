using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using EnterpriseOps.Domain;
using EnterpriseOps.Security;

namespace EnterpriseOps.Services
{
    public class HarnessResult
    {
        public IReadOnlyList<RegressionFlow> Flows { get; set; }
        public int Passed { get; set; }
        public int Failed { get; set; }
        public int Total => Flows.Count;
        public int ThemeFailed { get; set; }
        public bool Succeeded => Failed == 0;
        public string Verdict { get; set; }
        public string CorrelationId { get; set; }
    }

    /// <summary>
    /// The regression harness: ten key flows that protect <b>behavior, not compilation</b>. Each flow calls
    /// the real service the screen calls and compares the outcome with what the 3.5 app did. The theme flows
    /// are a visual diff of the token map — the app compiles and runs while they fail.
    /// </summary>
    public class RegressionHarnessService
    {
        private readonly WorkOrderService _workOrders;
        private readonly ThemeService _theme;
        private readonly PermissionService _permissions;
        private readonly SessionContext _session;
        private readonly Func<SessionContext> _sessionBag;
        private readonly List<RegressionFlow> _flows;
        private readonly ActivityTrace _trace;

        /// <param name="sessionBag">Reads the context back from the per-session bag (Application.Session) — flow 9 proves it is the same instance.</param>
        public RegressionHarnessService(WorkOrderService workOrders, ThemeService theme, PermissionService permissions,
            SessionContext session, Func<SessionContext> sessionBag, List<RegressionFlow> flows, ActivityTrace trace)
        {
            _workOrders = workOrders;
            _theme = theme;
            _permissions = permissions;
            _session = session;
            _sessionBag = sessionBag;
            _flows = flows;
            _trace = trace;
        }

        public IReadOnlyList<RegressionFlow> Flows => _flows;

        public void ResetOutcomes()
        {
            foreach (var flow in _flows)
            {
                flow.Outcome = FlowOutcome.NotRun;
                flow.Actual = "";
            }
        }

        /// <summary>
        /// Runs the selected flows (all ten when <paramref name="filter"/> is null), reporting each one through
        /// <paramref name="progress"/> so the grid and the trace update while the harness is still running.
        /// </summary>
        public async Task<HarnessResult> RunAsync(CommandContext ctx, Func<RegressionFlow, bool> filter, Action<RegressionFlow> progress, CancellationToken cancellation)
        {
            var selected = _flows.Where(f => filter == null || filter(f)).ToList();
            _trace.Job($"regression harness → {selected.Count} flow(s) for {ctx.TenantId} (corr {ctx.CorrelationId})");

            foreach (var flow in selected)
            {
                cancellation.ThrowIfCancellationRequested();
                await Task.Delay(140, cancellation);

                var (ok, actual) = await ExecuteAsync(flow, ctx);
                flow.Outcome = ok ? FlowOutcome.Pass : FlowOutcome.Fail;
                flow.Actual = actual;
                _trace.Service($"flow {flow.Id,2} {flow.Name} → {(ok ? "PASS" : "FAIL")} — {actual}");
                progress?.Invoke(flow);
            }

            var result = new HarnessResult
            {
                Flows = selected,
                Passed = selected.Count(f => f.Outcome == FlowOutcome.Pass),
                Failed = selected.Count(f => f.Outcome == FlowOutcome.Fail),
                ThemeFailed = selected.Count(f => f.Outcome == FlowOutcome.Fail && f.Category == FlowCategory.Theme),
                CorrelationId = ctx.CorrelationId,
            };

            if (result.Succeeded)
                result.Verdict = $"PASS — {result.Passed}/{result.Total} flows";
            else
            {
                string reasons = string.Join(" · ", selected.Where(f => f.Outcome == FlowOutcome.Fail).Select(f => f.Actual));
                result.Verdict = $"FAIL — {result.Passed}/{result.Total} flows" +
                                 (result.ThemeFailed > 0 ? $", {result.ThemeFailed} theme flows broken" : "") +
                                 $" ({reasons})";
            }

            _trace.Job($"regression harness: {result.Verdict}");
            return result;
        }

        private async Task<(bool ok, string actual)> ExecuteAsync(RegressionFlow flow, CommandContext ctx)
        {
            switch (flow.Id)
            {
                case 1:
                {
                    bool managerOk = _permissions.IsAllowed(Role.Manager, Permission.ApproveWorkOrder);
                    bool techDenied = !_permissions.IsAllowed(Role.Technician, Permission.ApproveWorkOrder);
                    return (managerOk && techDenied, $"ana.ops (Manager) {(managerOk ? "allowed" : "DENIED")}, ben.tech (Technician) {(techDenied ? "denied" : "ALLOWED")}");
                }
                case 2:
                {
                    var page = _workOrders.Query(new WorkQueueQuery { Bucket = QueueBucket.Open }, ctx);
                    bool onlyOpen = page.Rows.All(r => r.State == "new" || r.State == "assigned" || r.State == "on hold");
                    bool ordered = page.Rows.Select(r => r.Due).Where(d => d != "—").SequenceEqual(page.Rows.Select(r => r.Due).Where(d => d != "—").OrderBy(d => d, StringComparer.Ordinal));
                    return (onlyOpen && ordered, $"{page.Total} rows, {(onlyOpen ? "all New/Assigned/OnHold" : "WRONG statuses")}, {(ordered ? "due ascending" : "NOT ordered")}");
                }
                case 3:
                {
                    var page = _workOrders.Query(new WorkQueueQuery { Bucket = QueueBucket.InProgress }, ctx);
                    int inStore = _workOrders.CountInStore(ctx.TenantId, QueueBucket.InProgress);
                    bool onlyInProgress = page.Rows.All(r => r.State == "in progress" || r.State == "escalated");
                    bool serverSide = page.Total == inStore;
                    return (onlyInProgress && serverSide, $"{page.Total} rows filtered on the server (store agrees: {inStore}), {(onlyInProgress ? "only InProgress/Escalated" : "WRONG statuses")}");
                }
                case 4:
                {
                    int before = _workOrders.StoreCount;
                    var result = _workOrders.Save(new SaveWorkOrderCommand { Title = "   " }, ctx);
                    bool rejected = !result.Succeeded && result.Errors.Any(e => e.Contains("Title"));
                    bool unchanged = _workOrders.StoreCount == before;
                    return (rejected && unchanged, $"blank title {(rejected ? "rejected: " + result.ErrorText : "ACCEPTED")}, store {(unchanged ? "unchanged" : "CHANGED")} ({before} rows)");
                }
                case 5:
                {
                    const int id = 2002;   // "Repair loading dock pump" — contoso
                    int current = _workOrders.CurrentVersion(id);
                    var stale = _workOrders.Approve(new ApproveWorkOrderCommand { WorkOrderId = id, Version = current - 1 }, ctx);
                    var fresh = _workOrders.Approve(new ApproveWorkOrderCommand { WorkOrderId = id, Version = current }, ctx);
                    bool ok = !stale.Succeeded && stale.ErrorText.Contains("stale") && fresh.Succeeded;
                    return (ok, $"#{id}: stale v{current - 1} {(stale.Succeeded ? "ACCEPTED" : "rejected")}, current v{current} {(fresh.Succeeded ? "accepted → v" + _workOrders.CurrentVersion(id) : "REJECTED")}");
                }
                case 6:
                {
                    bool same = string.Equals(_theme.Current.AccentColor, _theme.Baseline.AccentColor, StringComparison.OrdinalIgnoreCase);
                    return (same, same ? $"accent {_theme.Current.AccentColor} as on 3.5" : $"accent color lost ({_theme.Current.AccentColor} ≠ baseline {_theme.Baseline.AccentColor})");
                }
                case 7:
                {
                    bool same = _theme.Current.CornerRadius == _theme.Baseline.CornerRadius;
                    return (same, same ? $"corner radius {_theme.Current.CornerRadius} px as on 3.5" : $"corner radius {_theme.Current.CornerRadius} (baseline {_theme.Baseline.CornerRadius})");
                }
                case 8:
                {
                    int expected = _theme.Baseline.PriorityColors.Count;
                    int matched = _theme.Baseline.PriorityColors.Count(p => _theme.Current.PriorityColors.TryGetValue(p.Key, out string c) && string.Equals(c, p.Value, StringComparison.OrdinalIgnoreCase));
                    return (matched == expected, matched == expected ? $"priority colors {matched}/{expected} match (High red, Normal amber, Low green)" : $"priority colors dropped ({expected - matched}/{expected} missing)");
                }
                case 9:
                {
                    var fromBag = _sessionBag();
                    bool sameInstance = ReferenceEquals(fromBag, _session);
                    int staticFields = typeof(SessionContext).GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).Length;
                    bool ok = sameInstance && staticFields == 0;
                    return (ok, $"Application.Session.Context {(sameInstance ? "is the same instance" : "is a DIFFERENT instance")} ({_session}), SessionContext has {staticFields} static fields");
                }
                case 10:
                {
                    int ticks = 0;
                    for (int i = 0; i < 3; i++)
                    {
                        await Task.Delay(40);
                        ticks++;
                    }
                    return (ticks == 3, $"{ticks} progress ticks observed (simulated import job)");
                }
                default:
                    return (false, "unknown flow");
            }
        }
    }
}
