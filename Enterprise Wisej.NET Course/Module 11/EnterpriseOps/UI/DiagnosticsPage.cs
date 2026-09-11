using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EnterpriseOps.Data;
using EnterpriseOps.Diagnostics;
using EnterpriseOps.Security;
using EnterpriseOps.Services;
using Wisej.Web;

namespace EnterpriseOps.UI
{
    /// <summary>
    /// DiagnosticsPage — the runbook, as a screen: the DiagnosticSnapshot (version · environment · node ·
    /// theme · flags) behind a role check, session &amp; health, the performance budget table and the
    /// structured log. The status bar carries the last operation's result and correlation id.
    ///
    /// Handlers stay thin: they build a CommandContext, call a service, and display what came back.
    /// PerformanceBudget, HealthCheck, DiagnosticsAccessPolicy and SessionMemoryAudit decide; the page shows.
    /// </summary>
    public partial class DiagnosticsPage : Page
    {
        private const int MaxLogLines = 60;
        private const int ApproxBytesPerLogLine = 300;

        private static readonly Color AmberBack = Color.FromArgb(255, 248, 236);
        private static readonly Color AmberInk = Color.FromArgb(122, 82, 16);
        private static readonly Color GreenBack = Color.FromArgb(240, 249, 243);
        private static readonly Color GreenInk = Color.FromArgb(15, 122, 58);
        private static readonly Color RedBack = Color.FromArgb(253, 236, 234);
        private static readonly Color RedInk = Color.FromArgb(178, 59, 39);

        // Per-session services. Created here, never static: two browsers are two independent sessions.
        private readonly SessionContext _session;
        private readonly DeploymentConfig _config;
        private readonly StructuredLog _log;
        private readonly PerformanceBudget _budget;
        private readonly SessionMemoryAudit _memoryAudit;
        private readonly HealthCheck _health;
        private readonly DiagnosticsService _diagnostics;
        private readonly InMemoryWorkOrderStore _store;
        private readonly WorkOrderService _workOrders;
        private readonly DiagnosticsAccessPolicy _accessPolicy;
        private readonly AuditTrail _auditTrail;

        private CancellationTokenSource _cts;

        /// <summary>The page size the next query asks for. The slow query pushes it to 5,000.</summary>
        private int _pageSize = 50;

        /// <summary>False when DiagnosticsAccessPolicy denied the current user: the snapshot values stay hidden.</summary>
        private bool _snapshotVisible;

        public DiagnosticsPage()
        {
            InitializeComponent();

            _session = ReadSession();
            _config = DeploymentConfig.Load();

            _log = new StructuredLog();
            _log.EntryWritten += Log_EntryWritten;

            _budget = new PerformanceBudget();
            _memoryAudit = new SessionMemoryAudit(_log);
            _health = new HealthCheck(_budget, _memoryAudit, _log);
            _diagnostics = new DiagnosticsService(_config, _session, _log, ServerTrace);

            _store = new InMemoryWorkOrderStore(ServerTrace);
            _workOrders = new WorkOrderService(_store, _log, _budget, ServerTrace);

            _accessPolicy = new DiagnosticsAccessPolicy();
            _auditTrail = new AuditTrail();

            RegisterRetainedState();
            RegisterHealthProbes();

            this.Disposed += DiagnosticsPage_Disposed;
        }

        #region Load — startup timing, role check, first snapshot

        private void DiagnosticsPage_Load(object sender, EventArgs e)
        {
            var screenLoad = System.Diagnostics.Stopwatch.StartNew();
            CommandContext ctx = _session.NewCommand();

            // Startup: Program.Main → this Load, measured by the session's own stopwatch.
            long startupMs = _session.StopStartupTimer();
            BudgetRow startup = _budget.Record(PerformanceBudget.Startup, startupMs, ctx.CorrelationId);
            _log.Write(startup.IsOver ? LogLevel.Warning : LogLevel.Information, PerformanceBudget.Startup, ctx.CorrelationId, new
            {
                elapsedMs = startupMs,
                budgetMs = startup.BudgetMs,
                budget = startup.IsOver ? "over" : "ok",
                node = _config.NodeName,
                environment = _config.Environment,
            });

            this.btnRunQuery.Text = $"Run query · {_pageSize:N0}";

            ApplyAccessDecision(ctx);
            RefreshLiveNumbers(ctx);
            RebindBudgets(ctx);

            screenLoad.Stop();
            BudgetRow screen = _budget.Record(PerformanceBudget.ScreenLoad, screenLoad.ElapsedMilliseconds, ctx.CorrelationId);
            _log.Write(screen.IsOver ? LogLevel.Warning : LogLevel.Information, PerformanceBudget.ScreenLoad, ctx.CorrelationId, new
            {
                elapsedMs = screenLoad.ElapsedMilliseconds,
                budgetMs = screen.BudgetMs,
                budget = screen.IsOver ? "over" : "ok",
            });
            this.perfBudgetPanel.UpdateRow(screen);

            // The live numbers and the "Background job tick" row refresh once a second.
            this.timerLive.Start();

            if (_snapshotVisible)
                SetStatus($"Diagnostics — live · {_config.NodeName} · {DiagnosticsService.Version} · tenant {_session.Tenant.Id}");
        }

        #endregion

        #region Query — one timed, logged, correlated operation

        private async void btnRunQuery_Click(object sender, EventArgs e)
        {
            await RunQueryAsync();
        }

        /// <summary>The slow query: pageSize 5,000 breaks the 400 ms budget, and the log names the field.</summary>
        private async void btnSlowQuery_Click(object sender, EventArgs e)
        {
            _pageSize = 5000;
            await RunQueryAsync();
        }

        /// <summary>The fix: back to pageSize 50 — the same operation, the budget row turns green.</summary>
        private async void btnFixPageSize_Click(object sender, EventArgs e)
        {
            _pageSize = 50;
            await RunQueryAsync();
        }

        /// <summary>
        /// Build the context, call the service, show the result, and own the failure path. The timing, the
        /// budget verdict and the log entry all happen inside the service.
        /// </summary>
        private async Task RunQueryAsync()
        {
            this.btnRunQuery.Text = $"Run query · {_pageSize:N0}";
            CommandContext ctx = _session.NewCommand();

            try
            {
                SearchResult result = await RunSearchAsync(_pageSize, ctx);
                ShowSearchResult(result, ctx);
            }
            catch (Exception ex)
            {
                ReportFailure(WorkOrderService.SearchOperation, ctx, ex);
            }
        }

        private async Task<SearchResult> RunSearchAsync(int pageSize, CommandContext ctx)
        {
            _cts?.Dispose();
            _cts = new CancellationTokenSource();

            SetButtonsEnabled(false);
            try
            {
                var query = new WorkQueueQuery { TenantId = ctx.TenantId, Page = 1, PageSize = pageSize };
                return await _workOrders.SearchWorkOrdersAsync(query, ctx, _cts.Token);
            }
            finally
            {
                SetButtonsEnabled(true);
                Application.Update(this);
            }
        }

        private void ShowSearchResult(SearchResult result, CommandContext ctx)
        {
            BudgetRow row = result.Budget;
            this.perfBudgetPanel.UpdateRow(row);
            RefreshLiveNumbers(ctx);

            string verdict = row != null && row.IsOver ? "OVER BUDGET" : "within budget";
            SetStatus($"OperationTimer — SearchWorkOrders {result.ElapsedMs:N0} ms · correlation {ctx.CorrelationId} · {verdict}");
        }

        #endregion

        #region Live refresh

        /// <summary>
        /// One tick: refresh the live numbers and the health chip, and measure how long that took against
        /// the background-job budget. The tick writes no log entry — a once-per-second line would fill any sink.
        /// </summary>
        private void timerLive_Tick(object sender, EventArgs e)
        {
            if (this.IsDisposed)
                return;

            try
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                CommandContext ctx = _session.NewCommand();

                RefreshLiveNumbers(ctx);

                watch.Stop();
                BudgetRow row = _budget.Record(PerformanceBudget.BackgroundJobTick, watch.ElapsedMilliseconds, ctx.CorrelationId);
                this.perfBudgetPanel.UpdateRow(row);
            }
            catch (ObjectDisposedException)
            {
                // The session ended between the tick and the update; nothing to do.
            }
            catch (Exception ex)
            {
                this.timerLive.Stop();
                ReportFailure("LiveRefreshTick", _session.NewCommand(), ex);
            }
        }

        private void ShowHealth(HealthReport report)
        {
            this.lblHealth.Text = report.Status.ToString().ToUpperInvariant();
            switch (report.Status)
            {
                case HealthStatus.Healthy:
                    this.lblHealth.BackColor = GreenBack;
                    this.lblHealth.ForeColor = GreenInk;
                    break;
                case HealthStatus.Degraded:
                    this.lblHealth.BackColor = AmberBack;
                    this.lblHealth.ForeColor = AmberInk;
                    break;
                default:
                    this.lblHealth.BackColor = RedBack;
                    this.lblHealth.ForeColor = RedInk;
                    break;
            }
            this.lblHealth.ToolTipText = string.Join("\r\n", report.Checks.Select(c => $"{c.Name}: {c.Status.ToString().ToLowerInvariant()} — {c.Detail}"));
        }

        #endregion

        #region Role check — the diagnostics page is an attack surface

        /// <summary>
        /// Asks the policy, audits the answer, and only then decides what the page may render.
        /// A denied user sees the page frame and nothing else — the values are never sent to the browser.
        /// </summary>
        private void ApplyAccessDecision(CommandContext ctx)
        {
            AccessDecision decision = _accessPolicy.CanViewDiagnostics(_session.User);
            _auditTrail.Record(_session.User.UserName, "OpenDiagnostics", decision.Allowed ? "allowed" : "denied", ctx.CorrelationId);

            _log.Write(decision.Allowed ? LogLevel.Information : LogLevel.Warning, "OpenDiagnostics", ctx.CorrelationId, new
            {
                user = _session.User.UserName,
                role = _session.User.Role.ToString(),
                outcome = decision.Allowed ? "allowed" : "denied",
                tenant = ctx.TenantId,
            });

            _snapshotVisible = decision.Allowed;

            this.lblRole.Text = decision.Allowed ? $"Role: {_session.User.Role} ✓" : $"Role: {_session.User.Role} ✕";
            this.lblRole.BackColor = decision.Allowed ? GreenBack : RedBack;
            this.lblRole.ForeColor = decision.Allowed ? GreenInk : RedInk;
            this.lblRole.ToolTipText = decision.Reason;

            this.pnlBudgets.Visible = decision.Allowed;
            this.pnlLog.Visible = decision.Allowed;
            this.pnlHealth.Visible = decision.Allowed;

            if (!decision.Allowed)
            {
                SetSnapshotCards("—", "—", "—", "—");
                this.lblRedacted.Text = "hidden — not an operator role";
                this.lblFlags.Text = "flags —";
                SetStatus($"Diagnostics denied for {_session.User.UserName} · correlation {ctx.CorrelationId}");
                AlertBox.Show("You do not have permission to view diagnostics.", MessageBoxIcon.Warning,
                    alignment: ContentAlignment.TopRight, autoCloseDelay: 4000);
                return;
            }

            ShowSnapshot();
        }

        #endregion

        #region The snapshot, the live numbers and the budget table

        /// <summary>Captures a fresh DiagnosticSnapshot and binds it to the four cards. Safe by type.</summary>
        private void ShowSnapshot()
        {
            DiagnosticSnapshot snapshot = _diagnostics.CaptureSnapshot();

            SetSnapshotCards(snapshot.Version, snapshot.Environment, snapshot.NodeName, snapshot.ActiveTheme);

            this.lblRedacted.Text = $"secrets redacted ({_diagnostics.RedactionNotes.Count})";
            this.lblRedacted.ToolTipText = string.Join("\r\n", _diagnostics.RedactionNotes);
            this.lblFlags.Text = "flags " + string.Join("  ", snapshot.FeatureFlags.Select(f => $"{f.Key}={f.Value}"));
            this.lblFlags.ToolTipText = this.lblFlags.Text;
        }

        private void SetSnapshotCards(string version, string environment, string node, string theme)
        {
            this.lblVersion.Text = version;
            this.lblVersion.ToolTipText = version;
            this.lblEnvironment.Text = environment;
            this.lblNode.Text = node;
            this.lblTheme.Text = theme;
        }

        /// <summary>Session count, memory, uptime and health — the numbers the live tick refreshes.</summary>
        private void RefreshLiveNumbers(CommandContext ctx)
        {
            if (!_snapshotVisible)
                return;

            SessionStats stats = _diagnostics.CaptureStats();

            this.lblSessions.Text = $"sessions {stats.SessionCount}  ·  this one …{stats.SessionIdSuffix} ({Format(stats.SessionAge)})";
            this.lblUptime.Text = $"uptime {Format(stats.ProcessUptime)}  ·  {_log.Entries.Count}/{StructuredLog.Capacity} log entries  ·  {_log.ErrorCount} error(s)";
            this.lblMemory.Text = $"managed heap {SessionMemoryAudit.Mb(stats.ManagedHeapBytes)}  ·  working set {SessionMemoryAudit.Mb(stats.WorkingSetBytes)}";

            ShowHealth(_health.Run(ctx.CorrelationId, writeLog: false));
        }

        /// <summary>
        /// Rebinding the whole table is itself a measurement: how long the grid takes to show the data
        /// after it arrives. That is the "Binding refresh" budget row.
        /// </summary>
        private void RebindBudgets(CommandContext ctx)
        {
            long bindMs = this.perfBudgetPanel.Bind(_budget.Rows);

            BudgetRow row = _budget.Record(PerformanceBudget.BindingRefresh, bindMs, ctx.CorrelationId);
            this.perfBudgetPanel.UpdateRow(row);

            _log.Write(row.IsOver ? LogLevel.Warning : LogLevel.Information, PerformanceBudget.BindingRefresh, ctx.CorrelationId, new
            {
                elapsedMs = bindMs,
                rows = _budget.Rows.Count,
                budgetMs = row.BudgetMs,
                budget = row.IsOver ? "over" : "ok",
            });
        }

        #endregion

        #region Session-memory registrations and health probes

        /// <summary>
        /// Answers the review question "which objects are retained for the whole session?" by making every
        /// holder declare itself: what it is, why it is kept, whether it is bounded and how big it is now.
        /// </summary>
        private void RegisterRetainedState()
        {
            _memoryAudit.Register(new RetainedHolder
            {
                Name = "InMemoryWorkOrderStore._rows",
                Reason = "the fake work-order table this session queries",
                Bounded = true,
                Bytes = () => _store.EstimatedBytes,
                Lifetime = () => $"whole session — seeded once, fixed at {_store.Count} rows",
            });

            _memoryAudit.Register(new RetainedHolder
            {
                Name = "StructuredLog._entries",
                Reason = "the session's structured log, read by the diagnostics page",
                Bounded = true,
                Bytes = () => _log.EstimatedBytes,
                Lifetime = () => $"whole session — ring buffer capped at {StructuredLog.Capacity} entries",
            });

            _memoryAudit.Register(new RetainedHolder
            {
                Name = "DiagnosticsPage.lstStructuredLog.Items",
                Reason = "the structured log lines shown on this page",
                Bounded = true,
                Bytes = () => (long)this.lstStructuredLog.Items.Count * ApproxBytesPerLogLine,
                Lifetime = () => $"whole session — trimmed to the last {MaxLogLines} lines",
            });

            _memoryAudit.RegisterTimer(() => new TimerState
            {
                Name = "timerLive (1 s)",
                Running = !this.IsDisposed && this.timerLive.Enabled,
                StoppedWhere = "DiagnosticsPage_Disposed",
            });
        }

        private void RegisterHealthProbes()
        {
            _health.AddProbe("work-order store", () =>
                _store.Count > 0
                    ? (HealthStatus.Healthy, $"{_store.Count} rows · ≈{_store.SimulatedLatencyMs(50):N0} ms for a 50-row page")
                    : (HealthStatus.Unhealthy, "no rows seeded"));

            _health.AddProbe("live refresh job", () =>
                (HealthStatus.Healthy, this.IsDisposed || !this.timerLive.Enabled
                    ? "stopped"
                    : $"running every {this.timerLive.Interval} ms"));

            _health.AddProbe("deployment config", () =>
                string.IsNullOrEmpty(_config.NodeName)
                    ? (HealthStatus.Degraded, "no node label configured")
                    : (HealthStatus.Healthy, $"{_config.Environment} · {_config.NodeName} · {_config.FeatureFlags.Count} flags"));
        }

        #endregion

        #region Log, status — display only

        /// <summary>Server-side trace for the services (Service: · Data: · Diagnostics:), never shown on the page.</summary>
        private static void ServerTrace(string message)
        {
            System.Diagnostics.Trace.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture)}  {message}");
        }

        /// <summary>
        /// The structured log, on screen, as JSON lines. Error entries are shown redacted: their full
        /// detail (exception type, message) exists only in the server-side entry.
        /// </summary>
        private void Log_EntryWritten(LogEntry entry)
        {
            if (this.IsDisposed)
                return;

            this.lstStructuredLog.Items.Add(ForDisplay(entry));

            while (this.lstStructuredLog.Items.Count > MaxLogLines)
                this.lstStructuredLog.Items.RemoveAt(0);

            this.lstStructuredLog.SelectedIndex = this.lstStructuredLog.Items.Count - 1;
        }

        private static string ForDisplay(LogEntry entry) =>
            entry.Level == LogLevel.Error
                ? entry.SafeSummary + "   (error detail is in the server log)"
                : entry.ToJsonLine();

        private void SetStatus(string text)
        {
            this.lblStatus.Text = text;
        }

        private void SetButtonsEnabled(bool enabled)
        {
            this.btnRunQuery.Enabled = enabled;
            this.btnSlowQuery.Enabled = enabled;
            this.btnFixPageSize.Enabled = enabled;
        }

        /// <summary>
        /// An exception becomes a full-detail log entry and an audit record on the server; the user gets the
        /// safe message with the correlation id, and nothing else.
        /// </summary>
        private void ReportFailure(string operation, CommandContext ctx, Exception ex)
        {
            _log.Error(operation, ctx.CorrelationId, ex, new
            {
                tenant = ctx.TenantId,
                user = ctx.UserName,
                pageSize = _pageSize,
            });
            _auditTrail.Record(ctx.UserName, operation, "failed", ctx.CorrelationId);

            SetStatus($"{operation} failed · correlation {ctx.CorrelationId} · detail in the server log");
            AlertBox.Show(SafeErrorMessage.For(ctx.CorrelationId), MessageBoxIcon.Error,
                alignment: ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        private static string Format(TimeSpan span) =>
            span.TotalHours >= 1
                ? $"{(int)span.TotalHours}h {span.Minutes:D2}m"
                : span.TotalMinutes >= 1 ? $"{span.Minutes}m {span.Seconds:D2}s" : $"{span.Seconds}s";

        #endregion

        #region Disposal — everything this page started, this page stops

        /// <summary>
        /// The timer is stopped (a running timer keeps the page alive), the log subscription is removed
        /// (a live handler holds a reference to a disposed control) and the cancellation source is disposed.
        /// </summary>
        private void DiagnosticsPage_Disposed(object sender, EventArgs e)
        {
            this.timerLive.Stop();
            _log.EntryWritten -= Log_EntryWritten;
            _cts?.Dispose();
            _cts = null;
        }

        #endregion

        #region Session plumbing

        /// <summary>
        /// The SessionContext Program.Main put in Application.Session. Falling back to a fresh one keeps
        /// the page usable if it is opened directly (for example from the Wisej Designer preview).
        /// </summary>
        private static SessionContext ReadSession()
        {
            try
            {
                var existing = Application.Session.Context as SessionContext;
                if (existing != null)
                    return existing;
            }
            catch
            {
                // No Wisej session (design time): fall through.
            }

            SessionContext session = SessionContext.Start(SafeSessionId());
            try
            {
                Application.Session.Context = session;
            }
            catch
            {
                // Design time again: the page still runs off the local instance.
            }
            return session;
        }

        private static string SafeSessionId()
        {
            try
            {
                return Application.SessionId;
            }
            catch
            {
                return Guid.NewGuid().ToString("N");
            }
        }

        #endregion
    }
}
