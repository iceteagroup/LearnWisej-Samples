using System;
using System.Collections.Generic;
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
    /// DiagnosticsPage — the Module 11 screen: the runbook, as a screen.
    ///
    /// Left column:  the DiagnosticSnapshot (version · environment · node · theme) behind a role check,
    ///               the live session &amp; health card, the performance budget table (PerfBudgetPanel) and
    ///               the structured log rendered as JSON lines.
    /// Right column: the live activity trace — every UI action, every service decision, every data access
    ///               and every diagnostics verdict, each carrying the correlation id of its action.
    /// Bottom bar:   success (run query), three failure paths (slow query · store failure · session leak),
    ///               their recoveries, the progress path (live refresh timer), health check and clear.
    ///
    /// Handlers stay thin: they build a CommandContext, call a service, and display what came back.
    /// Nothing on this page decides whether an operation is over budget, healthy, allowed or leaking —
    /// PerformanceBudget, HealthCheck, DiagnosticsAccessPolicy and SessionMemoryAudit decide, the page shows.
    /// </summary>
    public partial class DiagnosticsPage : Page
    {
        private const int MaxTraceLines = 400;
        private const int MaxLogLines = 60;
        private const int ApproxBytesPerTraceLine = 140;

        private static readonly Color Green = Color.FromArgb(31, 157, 87);
        private static readonly Color Amber = Color.FromArgb(232, 161, 60);
        private static readonly Color Red = Color.FromArgb(224, 86, 59);
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
        private readonly ReportCacheService _reportCache;
        private readonly DiagnosticsAccessPolicy _accessPolicy;
        private readonly AuditTrail _auditTrail;

        private CancellationTokenSource _cts;

        /// <summary>The page size the next query asks for. The slow-query failure path pushes it to 5,000.</summary>
        private int _pageSize = 50;

        /// <summary>True while the page is populating cboUser, so the role check does not run on its own binding.</summary>
        private bool _binding;

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
            _diagnostics = new DiagnosticsService(_config, _session, _log, Trace);

            _store = new InMemoryWorkOrderStore(Trace);
            _workOrders = new WorkOrderService(_store, _log, _budget, Trace);
            _reportCache = new ReportCacheService(_log, Trace);

            _accessPolicy = new DiagnosticsAccessPolicy();
            _auditTrail = new AuditTrail();

            RegisterRetainedState();
            RegisterHealthProbes();

            // The disposal review, in code: everything this page started, this page stops.
            this.Disposed += DiagnosticsPage_Disposed;
        }

        #region Load — startup timing, role check, first snapshot

        private void DiagnosticsPage_Load(object sender, EventArgs e)
        {
            var screenLoad = System.Diagnostics.Stopwatch.StartNew();
            CommandContext ctx = _session.NewCommand();

            Trace($"UI → DiagnosticsPage.Load · session …{_session.SessionId?.Substring(Math.Max(0, _session.SessionId.Length - 6))} · correlation {ctx.CorrelationId}");

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
            Trace($"Diagnostics: startup {startupMs:N0} ms · budget ≤ {startup.BudgetMs} ms → {startup.StatusText}");

            _binding = true;
            this.cboUser.DataSource = AppUser.Directory.ToList();
            this.cboUser.SelectedIndex = AppUser.Directory.ToList().FindIndex(u => u.UserName == _session.User.UserName);
            _binding = false;

            this.lblTenant.Text = $"tenant {_session.Tenant.Id}";
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
            Trace($"Diagnostics: screen load {screenLoad.ElapsedMilliseconds:N0} ms · budget ≤ {screen.BudgetMs} ms → {screen.StatusText}");

            SetStatus($"Diagnostics — live · {_config.NodeName} · {DiagnosticsService.Version} · tenant {_session.Tenant.Id}", Green);
        }

        #endregion

        #region Success path — one timed, logged, correlated query

        /// <summary>
        /// The shape the lab guide asks for: build the context, call the service, show the result,
        /// and own the failure path. The timing, the budget verdict and the log entry all happen
        /// inside the service — the handler only displays what came back.
        /// </summary>
        private async void btnRunQuery_Click(object sender, EventArgs e)
        {
            try
            {
                CommandContext ctx = NewAction($"Run query · pageSize {_pageSize:N0}");
                SearchResult result = await RunSearchAsync(_pageSize, ctx);
                ShowSearchResult(result, ctx);
            }
            catch (Exception ex)
            {
                ReportFailure("SearchWorkOrders", ex);
            }
        }

        /// <summary>Failure path 1 — the bypassed paged query: pageSize 5,000 breaks the 400 ms budget.</summary>
        private async void btnSlowQuery_Click(object sender, EventArgs e)
        {
            try
            {
                _pageSize = 5000;
                this.btnRunQuery.Text = $"Run query · {_pageSize:N0}";

                CommandContext ctx = NewAction("Slow query · pageSize 5,000 (paged query bypassed)");
                Trace($"UI → the caller asked for every row at once — the same handler, the same service, one changed field");

                SearchResult result = await RunSearchAsync(_pageSize, ctx);
                ShowSearchResult(result, ctx);
            }
            catch (Exception ex)
            {
                ReportFailure("SearchWorkOrders", ex);
            }
        }

        /// <summary>Recovery 1 — back to pageSize 50: the same operation, the budget row turns green.</summary>
        private async void btnFixPageSize_Click(object sender, EventArgs e)
        {
            try
            {
                _pageSize = 50;
                this.btnRunQuery.Text = $"Run query · {_pageSize:N0}";

                CommandContext ctx = NewAction("Fix page size · back to 50");
                Trace("UI → the structured log named the field (pageSize), so the fix is the field — not a rewrite");

                SearchResult result = await RunSearchAsync(_pageSize, ctx);
                ShowSearchResult(result, ctx);

                if (result.Budget != null && !result.Budget.IsOver)
                    ShowBanner($"✔ Recovered — SearchWorkOrders {result.ElapsedMs:N0} ms with pageSize 50, inside the {result.Budget.BudgetMs} ms budget. Correlation {ctx.CorrelationId} proves it is the same flow.", BannerKind.Ok);
            }
            catch (Exception ex)
            {
                ReportFailure("SearchWorkOrders", ex);
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

            if (row != null && row.IsOver)
            {
                SetStatus($"OperationTimer — SearchWorkOrders {result.ElapsedMs:N0} ms · correlation {ctx.CorrelationId} · OVER BUDGET", Red);
                ShowBanner(
                    $"✖ SearchWorkOrders took {result.ElapsedMs:N0} ms against a {row.BudgetMs} ms budget.\r\n" +
                    $"Read the log line for correlation {ctx.CorrelationId}: \"pageSize\": {result.Page.PageSize:N0} — the paged query was bypassed.",
                    BannerKind.Error);
                AlertBox.Show($"SearchWorkOrders is over budget ({result.ElapsedMs:N0} ms). Reference {ctx.CorrelationId}.",
                    MessageBoxIcon.Warning, alignment: ContentAlignment.TopRight, autoCloseDelay: 4000);
            }
            else
            {
                SetStatus($"OperationTimer — SearchWorkOrders {result.ElapsedMs:N0} ms · correlation {ctx.CorrelationId} · within budget", Green);
                HideBanner();
            }

            Trace($"UI → showing {result.Page.Rows.Count:N0} of {result.Page.Total:N0} rows (page {result.Page.Page}, pageSize {result.Page.PageSize:N0}) · correlation {ctx.CorrelationId}");
        }

        #endregion

        #region Failure path 2 — an operation that throws, logged with its correlation id

        private async void btnThrow_Click(object sender, EventArgs e)
        {
            CommandContext ctx = NewAction("Store failure · the next query loses its connection");

            try
            {
                _store.FailNextCall = true;
                Trace("UI → the store will drop the connection on the next call; the handler owns the message the user sees");

                SearchResult result = await RunSearchAsync(_pageSize, ctx);
                ShowSearchResult(result, ctx);
            }
            catch (Exception ex)
            {
                // Full detail on the server, with the correlation id. The user gets the safe message.
                _log.Error(WorkOrderService.SearchOperation, ctx.CorrelationId, ex, new
                {
                    tenant = ctx.TenantId,
                    user = ctx.UserName,
                    pageSize = _pageSize,
                });
                _auditTrail.Record(ctx.UserName, "SearchWorkOrders", "failed", ctx.CorrelationId);

                Trace($"Log: {ex.GetType().Name} written with full detail · correlation {ctx.CorrelationId} — the message text stays on the server");
                Trace($"UI → the user is told: \"{SafeErrorMessage.For(ctx.CorrelationId)}\"");

                ShowBanner($"✖ {SafeErrorMessage.For(ctx.CorrelationId)}", BannerKind.Error);
                SetStatus($"SearchWorkOrders failed · correlation {ctx.CorrelationId} · detail in the server log", Red);
                AlertBox.Show(SafeErrorMessage.For(ctx.CorrelationId), MessageBoxIcon.Error,
                    alignment: ContentAlignment.TopRight, autoCloseDelay: 4000);

                RefreshLiveNumbers(ctx);
            }
        }

        #endregion

        #region Failure path 3 — session memory growth, caught by the audit

        private void btnLeakSession_Click(object sender, EventArgs e)
        {
            try
            {
                CommandContext ctx = NewAction("Leak · cache a 50,000-row report in a session field");

                LeakResult leak = _reportCache.Retain(50000, ctx);
                AuditReport report = RunMemoryAudit(ctx);

                ShowBanner(
                    $"✖ Session memory: the cached report retains {leak.RowsRetained:N0} rows (+{SessionMemoryAudit.Mb(leak.GrowthBytes)} on the managed heap).\r\n" +
                    $"The audit flags it — {report.Summary}. Two hundred sessions would be the server.",
                    BannerKind.Error);
                SetStatus($"Session memory audit failed · {report.Failing} holder(s) over budget · correlation {ctx.CorrelationId}", Red);
                AlertBox.Show($"Session memory audit failed: {report.Failing} holder(s) over budget. Reference {ctx.CorrelationId}.",
                    MessageBoxIcon.Warning, alignment: ContentAlignment.TopRight, autoCloseDelay: 4000);
            }
            catch (Exception ex)
            {
                ReportFailure("RetainReport", ex);
            }
        }

        /// <summary>Recovery 3 — drop the reference, re-run the audit, watch the heap come back.</summary>
        private void btnReleaseLeak_Click(object sender, EventArgs e)
        {
            try
            {
                CommandContext ctx = NewAction("Release cache · drop the retained report");

                _reportCache.Release();
                AuditReport report = RunMemoryAudit(ctx);

                if (report.Passed)
                {
                    ShowBanner($"✔ Recovered — the report was released; {report.Summary}.", BannerKind.Ok);
                    SetStatus($"Session memory audit passed · correlation {ctx.CorrelationId}", Green);
                }
                else
                {
                    ShowBanner($"⚠ Still over budget — {report.Summary}.", BannerKind.Warning);
                    SetStatus($"Session memory audit still fails · {report.Failing} holder(s) over budget · correlation {ctx.CorrelationId}", Amber);
                }
            }
            catch (Exception ex)
            {
                ReportFailure("ReleaseReport", ex);
            }
        }

        private AuditReport RunMemoryAudit(CommandContext ctx)
        {
            AuditReport report = _memoryAudit.Run(ctx.CorrelationId);

            Trace($"Diagnostics: session memory audit · {report.Items.Count} holders · ≈{SessionMemoryAudit.Mb(report.TotalRetainedBytes)} retained · managed heap {SessionMemoryAudit.Mb(report.ManagedHeapBytes)} · correlation {ctx.CorrelationId}");
            foreach (RetainedItem item in report.Items)
            {
                string advice = string.IsNullOrEmpty(item.Advice) ? "" : " → " + item.Advice;
                Trace($"Diagnostics:   [{item.VerdictText}] {item.Name} ≈{SessionMemoryAudit.Mb(item.Bytes)} · {item.Lifetime}{advice}");
            }
            foreach (TimerState timer in report.Timers)
                Trace($"Diagnostics:   timer {timer.Name} · {(timer.Running ? "RUNNING" : "stopped")} · stopped in {timer.StoppedWhere}");

            RefreshLiveNumbers(ctx);
            return report;
        }

        #endregion

        #region Progress path — the live refresh timer

        private void btnLive_Click(object sender, EventArgs e)
        {
            CommandContext ctx = NewAction(this.timerLive.Enabled ? "Live refresh · stop" : "Live refresh · start");

            if (this.timerLive.Enabled)
            {
                this.timerLive.Stop();
                this.btnLive.Text = "▶ Live refresh";
                Trace($"UI → timerLive stopped · correlation {ctx.CorrelationId}");
                SetStatus($"Live refresh stopped · {_config.NodeName} · {DiagnosticsService.Version}", Green);
            }
            else
            {
                this.timerLive.Start();
                this.btnLive.Text = "■ Stop refresh";
                Trace($"UI → timerLive started (1 s) — each tick is measured as '{PerformanceBudget.BackgroundJobTick}' · correlation {ctx.CorrelationId}");
                SetStatus($"Live refresh running · {_config.NodeName} · {DiagnosticsService.Version}", Green);
            }
        }

        /// <summary>
        /// One tick: refresh the live numbers and the health chip, and measure how long that took against
        /// the background-job budget. The tick logs nothing per second — the log must not become the leak
        /// it exists to catch — but it does update the budget row.
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
                this.btnLive.Text = "▶ Live refresh";
                ReportFailure("LiveRefreshTick", ex);
            }
        }

        #endregion

        #region Health check

        private void btnHealth_Click(object sender, EventArgs e)
        {
            try
            {
                CommandContext ctx = NewAction("Health check");
                HealthReport report = _health.Run(ctx.CorrelationId, writeLog: true);

                Trace($"Diagnostics: health {report.Status.ToString().ToUpperInvariant()} · {report.Checks.Count} checks · correlation {ctx.CorrelationId}");
                foreach (HealthCheckResult check in report.Checks)
                    Trace($"Diagnostics:   [{check.Status.ToString().ToLowerInvariant()}] {check.Name} — {check.Detail}");

                ShowHealth(report);

                if (report.Status == HealthStatus.Healthy)
                {
                    HideBanner();
                    SetStatus($"Health {report.Status.ToString().ToLowerInvariant()} · {report.Checks.Count} checks · correlation {ctx.CorrelationId}", Green);
                }
                else
                {
                    string names = string.Join(", ", report.NotHealthy.Select(c => c.Name));
                    ShowBanner($"⚠ Health {report.Status.ToString().ToLowerInvariant()} — {names}. The detail is in the trace, with correlation {ctx.CorrelationId}.", BannerKind.Warning);
                    SetStatus($"Health {report.Status.ToString().ToLowerInvariant()} · {names} · correlation {ctx.CorrelationId}", Amber);
                }
            }
            catch (Exception ex)
            {
                ReportFailure("HealthCheck", ex);
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

        private void cboUser_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_binding)
                return;

            try
            {
                var user = this.cboUser.SelectedItem as AppUser;
                if (user == null)
                    return;

                _session.User = user;
                CommandContext ctx = NewAction($"Open diagnostics as {user.UserName}");
                ApplyAccessDecision(ctx);
            }
            catch (Exception ex)
            {
                ReportFailure("OpenDiagnostics", ex);
            }
        }

        /// <summary>
        /// Asks the policy, audits the answer, and only then decides what the page may render.
        /// A denied user sees the page frame and nothing else — the values are never sent to the browser.
        /// </summary>
        private void ApplyAccessDecision(CommandContext ctx)
        {
            AccessDecision decision = _accessPolicy.CanViewDiagnostics(_session.User);
            AuditEntry entry = _auditTrail.Record(_session.User.UserName, "OpenDiagnostics", decision.Allowed ? "allowed" : "denied", ctx.CorrelationId);

            Trace($"Security: DiagnosticsAccessPolicy → {(decision.Allowed ? "allowed" : "DENIED")} — {decision.Reason} · correlation {ctx.CorrelationId}");
            Trace($"Security: audited · {entry}");
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
                ShowBanner($"✖ {_session.User.DisplayName} may not open diagnostics: {decision.Reason}. The attempt is audited (correlation {ctx.CorrelationId}).", BannerKind.Error);
                SetStatus($"Diagnostics denied for {_session.User.UserName} · correlation {ctx.CorrelationId}", Red);
                AlertBox.Show("You do not have permission to view diagnostics.", MessageBoxIcon.Warning,
                    alignment: ContentAlignment.TopRight, autoCloseDelay: 4000);
                return;
            }

            HideBanner();
            ShowSnapshot(ctx);
        }

        #endregion

        #region The snapshot, the live numbers and the budget table

        /// <summary>Captures a fresh DiagnosticSnapshot and binds it to the four cards. Safe by type.</summary>
        private void ShowSnapshot(CommandContext ctx)
        {
            DiagnosticSnapshot snapshot = _diagnostics.CaptureSnapshot();

            SetSnapshotCards(snapshot.Version, snapshot.Environment, snapshot.NodeName, snapshot.ActiveTheme);

            this.lblRedacted.Text = $"secrets redacted ({_diagnostics.RedactionNotes.Count})";
            this.lblRedacted.ToolTipText = string.Join("\r\n", _diagnostics.RedactionNotes);
            this.lblFlags.Text = "flags " + string.Join("  ", snapshot.FeatureFlags.Select(f => $"{f.Key}={f.Value}"));
            this.lblFlags.ToolTipText = this.lblFlags.Text;

            foreach (string safeEvent in snapshot.SafeRecentEvents)
                Trace($"Diagnostics:   safe recent event — {safeEvent}");

            Trace($"UI → snapshot bound to the cards · correlation {ctx.CorrelationId}");
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
            this.lblMemory.Text = $"managed heap {SessionMemoryAudit.Mb(stats.ManagedHeapBytes)}  ·  working set {SessionMemoryAudit.Mb(stats.WorkingSetBytes)}  ·  report cache {_reportCache.RetainedRows:N0} rows";

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
            Trace($"Diagnostics: binding refresh {bindMs:N0} ms for {_budget.Rows.Count} rows · budget ≤ {row.BudgetMs} ms → {row.StatusText}");
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
                Name = "DiagnosticsPage.lstTrace.Items",
                Reason = "the on-screen activity trace",
                Bounded = true,
                Bytes = () => (long)this.lstTrace.Items.Count * ApproxBytesPerTraceLine,
                Lifetime = () => $"whole session — trimmed to the last {MaxTraceLines} lines",
            });

            // The deliberate mistake: a collection in a field, filled once, cleared never.
            _memoryAudit.Register(new RetainedHolder
            {
                Name = "ReportCacheService._cached",
                Reason = "\"cache the big report so the second open is instant\"",
                Bounded = false,
                Bytes = () => _reportCache.RetainedBytesEstimate,
                Lifetime = () => _reportCache.Lifetime,
            });

            _memoryAudit.RegisterTimer(() => new TimerState
            {
                Name = "timerLive (1 s)",
                Running = !this.IsDisposed && this.timerLive.Enabled,
                StoppedWhere = "btnLive_Click (toggle) and DiagnosticsPage_Disposed",
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

        #region Trace, log, banner, status — display only

        /// <summary>Every layer's decision, one line, newest last. UI → · Service: · Data: · Diagnostics: · Security: · Log: · Job:</summary>
        private void Trace(string message)
        {
            if (this.IsDisposed)
                return;

            this.lstTrace.Items.Add($"{DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture)}  {message}");

            while (this.lstTrace.Items.Count > MaxTraceLines)
                this.lstTrace.Items.RemoveAt(0);

            this.lstTrace.SelectedIndex = this.lstTrace.Items.Count - 1;
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
                ? entry.SafeSummary + "   ← error detail is server-log only"
                : entry.ToJsonLine();

        /// <summary>A new user action: one correlation id, shown in the header, carried by every layer.</summary>
        private CommandContext NewAction(string label)
        {
            CommandContext ctx = _session.NewCommand();
            this.lblCorrelation.Text = $"correlation {ctx.CorrelationId}";
            this.lblCorrelation.ToolTipText = $"{label} · tenant {ctx.TenantId} · user {ctx.UserName}";
            Trace($"UI → {label} · new correlation {ctx.CorrelationId}");
            return ctx;
        }

        private enum BannerKind { Ok, Warning, Error }

        private void ShowBanner(string text, BannerKind kind)
        {
            this.lblBanner.Text = text;
            this.lblBanner.Visible = true;

            switch (kind)
            {
                case BannerKind.Ok:
                    this.lblBanner.BackColor = GreenBack;
                    this.lblBanner.ForeColor = GreenInk;
                    break;
                case BannerKind.Warning:
                    this.lblBanner.BackColor = AmberBack;
                    this.lblBanner.ForeColor = AmberInk;
                    break;
                default:
                    this.lblBanner.BackColor = RedBack;
                    this.lblBanner.ForeColor = RedInk;
                    break;
            }
        }

        private void HideBanner()
        {
            this.lblBanner.Text = "";
            this.lblBanner.Visible = false;
        }

        private void SetStatus(string text, Color color)
        {
            this.lblStatus.Text = "● " + text;
            this.lblStatus.ForeColor = color;
        }

        private void SetButtonsEnabled(bool enabled)
        {
            this.btnRunQuery.Enabled = enabled;
            this.btnSlowQuery.Enabled = enabled;
            this.btnFixPageSize.Enabled = enabled;
            this.btnThrow.Enabled = enabled;
        }

        /// <summary>The one place an unexpected exception becomes a log entry plus a safe message.</summary>
        private void ReportFailure(string operation, Exception ex)
        {
            string correlationId = _session.NewCorrelationId();
            _log.Error(operation, correlationId, ex);

            Trace($"Log: unhandled {ex.GetType().Name} in {operation} · correlation {correlationId} — detail on the server only");
            ShowBanner($"✖ {SafeErrorMessage.For(correlationId)}", BannerKind.Error);
            SetStatus($"{operation} failed · correlation {correlationId}", Red);
            AlertBox.Show(SafeErrorMessage.For(correlationId), MessageBoxIcon.Error,
                alignment: ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        private void btnClearTrace_Click(object sender, EventArgs e)
        {
            this.lstTrace.Items.Clear();
            Trace("UI → trace cleared (the structured log is untouched — it is the record)");
        }

        private static string Format(TimeSpan span) =>
            span.TotalHours >= 1
                ? $"{(int)span.TotalHours}h {span.Minutes:D2}m"
                : span.TotalMinutes >= 1 ? $"{span.Minutes}m {span.Seconds:D2}s" : $"{span.Seconds}s";

        #endregion

        #region Disposal — everything this page started, this page stops

        /// <summary>
        /// The disposal review, executed. The timer is stopped (a running timer keeps the page alive),
        /// the log subscription is removed (a live handler holds a reference to a disposed control), the
        /// cancellation source is disposed, and the retained report is released.
        /// </summary>
        private void DiagnosticsPage_Disposed(object sender, EventArgs e)
        {
            this.timerLive.Stop();
            _log.EntryWritten -= Log_EntryWritten;
            _cts?.Dispose();
            _cts = null;
            _reportCache.Release();
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
