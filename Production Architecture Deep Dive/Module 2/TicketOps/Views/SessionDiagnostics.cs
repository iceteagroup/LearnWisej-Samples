using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TicketOps.Controls;
using TicketOps.Data;
using TicketOps.Diagnostics;
using TicketOps.Domain;
using TicketOps.Infrastructure;
using TicketOps.Resources;
using TicketOps.Services;
using Wisej.Web;

namespace TicketOps.Views
{
    /// <summary>
    /// TicketOps Console · Session diagnostics — the Module 2 screen.
    ///
    /// Left card:   two panels that must never be confused — <b>Application</b> (identical in every session)
    ///              and <b>This session</b> (from the injected SessionContext) — plus the per-session
    ///              selectors (user / tenant / theme) and the tenant's ticket grid.
    /// Right card:  the activity trace: UI → SVC → SESSION / DATA and back.
    /// Bottom bar:  success (stamp a ticket for this session), progress (simulate 20 sessions), failure
    ///              (switch to a forbidden tenant), the legacy-static probe, the error path (directory outage)
    ///              with its recovery, and Clear trace.
    ///
    /// The Form takes <see cref="SessionContext"/> in its constructor exactly like the services do: it never
    /// reads a static, never calls a <c>SessionContext.Current</c>. Handlers stay thin: read the screen, ask a
    /// service, show the result. "async void" handlers own their try/catch.
    /// </summary>
    public partial class SessionDiagnostics : Form
    {
        private readonly SessionContext _ctx;
        private readonly ISessionService _session;
        private readonly ITicketService _tickets;
        private readonly IDiagnosticsService _diagnostics;
        private readonly InMemoryUserDirectory _directory;   // only for the lab's outage switch
        private readonly ILog _log;

        private IReadOnlyList<Ticket> _rows = new List<Ticket>();
        private int _simulatedRemaining;
        private bool _loadingSelectors;
        private bool _fillingGrid;

        // The Designer keeps the parameterless constructor; real wiring goes through the other one.
        public SessionDiagnostics() : this(new SessionContext(), null, null, null, null, new ActivityLog())
        {
        }

        public SessionDiagnostics(SessionContext ctx, ISessionService session, ITicketService tickets,
            IDiagnosticsService diagnostics, InMemoryUserDirectory directory, ILog log)
        {
            InitializeComponent();

            _ctx = ctx;
            _session = session;
            _tickets = tickets;
            _diagnostics = diagnostics;
            _directory = directory;
            _log = log;

            if (log is ActivityLog activityLog)
                this.tracePanel.Attach(activityLog);

            // Injected, session-scoped: the caption belongs to THIS operator's console.
            this.Text = $"TicketOps Console — Module 2 · {_ctx.Tenant} / {_ctx.CurrentUser}";
        }

        #region Screen lifecycle

        private async void SessionDiagnostics_Load(object sender, EventArgs e)
        {
            _log.Info(LogLayer.UI, "SessionDiagnostics.Load", $"screen shown for session {_ctx.ShortId} → load selectors, diagnostics, tickets");
            await LoadSelectorsAsync();
            RefreshDiagnostics();
            await RefreshGridAsync();
        }

        /// <summary>data → UI: fills the user / tenant / theme selectors from the directory and the settings.</summary>
        private async Task LoadSelectorsAsync()
        {
            try
            {
                _loadingSelectors = true;
                var users = await _session.GetUsersAsync();
                var tenants = await _session.GetTenantsAsync();

                this.comboUser.Items.Clear();
                foreach (var u in users) this.comboUser.Items.Add(u.Name);
                this.comboTenant.Items.Clear();
                foreach (var t in tenants) this.comboTenant.Items.Add(t);
                SyncSelectorsToContext();
            }
            catch (DirectoryOutageException ex)
            {
                ReportFailure("SessionDiagnostics.LoadSelectors", ex, Strings.DirectoryUnavailable);
            }
            catch (Exception ex)
            {
                ReportFailure("SessionDiagnostics.LoadSelectors", ex);
            }
            finally
            {
                _loadingSelectors = false;
            }
        }

        /// <summary>data → UI: the only place that fills the two diagnostics grids.</summary>
        private void RefreshDiagnostics()
        {
            try
            {
                var snapshot = _diagnostics.GetSnapshot();

                this.gridApplication.Rows.Clear();
                foreach (var row in snapshot.Application)
                    this.gridApplication.Rows.Add(row.Key, row.Value);
                this.gridApplication.ClearSelection();

                this.gridSession.Rows.Clear();
                foreach (var row in snapshot.Session)
                    this.gridSession.Rows.Add(row.Key, row.Value);
                this.gridSession.ClearSelection();

                this.labelSessionHeader.Text = $"This session · {_ctx.ShortId} · {_ctx.CurrentUser} @ {_ctx.Tenant}";
                this.Text = $"TicketOps Console — Module 2 · {_ctx.Tenant} / {_ctx.CurrentUser}";
                _log.Info(LogLayer.UI, "SessionDiagnostics.RefreshDiagnostics", $"{snapshot.Application.Count} application rows · {snapshot.Session.Count} session rows shown");
            }
            catch (Exception ex)
            {
                ReportFailure("SessionDiagnostics.RefreshDiagnostics", ex);
            }
        }

        /// <summary>data → UI: the only place that fills the ticket grid (this session's tenant only).</summary>
        private async Task RefreshGridAsync()
        {
            try
            {
                this.statusBanner.SetStatus("loading", StatusKind.Busy);
                _rows = await _tickets.GetOpenTicketsAsync();

                // Filling the grid raises SelectionChanged for the first row: that is not a user choice, so it must not reach SessionContext.
                _fillingGrid = true;
                try
                {
                    this.gridTickets.Rows.Clear();
                    foreach (var t in _rows)
                        this.gridTickets.Rows.Add(t.Id, t.Title, t.Priority.ToString(), t.Author);
                    this.gridTickets.ClearSelection();
                    this.gridTickets.CurrentCell = null;
                }
                finally
                {
                    _fillingGrid = false;
                }

                this.labelTickets.Text = $"Open tickets · {_ctx.Tenant} ({_rows.Count})";
                this.labelSelected.Text = Strings.SelectTicketHint;
                this.statusBanner.SetStatus("ready", StatusKind.Success);
                _log.Info(LogLayer.UI, "SessionDiagnostics.RefreshGrid", $"{_rows.Count} rows shown for {_ctx.Tenant}");
            }
            catch (Exception ex)
            {
                ReportFailure("SessionDiagnostics.RefreshGrid", ex);
            }
        }

        #endregion

        #region UI → data (read the form) and data → UI (show the result)

        private void SyncSelectorsToContext()
        {
            _loadingSelectors = true;
            try
            {
                this.comboUser.SelectedIndex = this.comboUser.Items.IndexOf(_ctx.CurrentUser);
                this.comboTenant.SelectedIndex = this.comboTenant.Items.IndexOf(_ctx.Tenant);
                this.comboTheme.SelectedIndex = this.comboTheme.Items.IndexOf(_ctx.Theme);
            }
            finally
            {
                _loadingSelectors = false;
            }
        }

        private string SelectedText(ComboBox combo)
            => combo.SelectedIndex >= 0 ? combo.Items[combo.SelectedIndex].ToString() : null;

        private int? SelectedTicketId()
        {
            var row = this.gridTickets.CurrentRow;
            if (row == null || row.Index < 0 || row.Index >= _rows.Count)
                return null;
            return _rows[row.Index].Id;
        }

        private void ShowResult<T>(OperationResult<T> result)
        {
            if (result.Succeeded)
            {
                this.statusBanner.HideBanner();
                this.statusBanner.SetStatus(result.Message, StatusKind.Success);
                _log.Info(LogLayer.UI, "SessionDiagnostics.ShowResult", $"OK · {result.Message}");
            }
            else
            {
                // Expected outcome: the service explained it in words the user may read.
                this.statusBanner.ShowBanner(result.Message, StatusKind.Warning);
                this.statusBanner.SetStatus("not applied", StatusKind.Warning);
                _log.Warn(LogLayer.UI, "SessionDiagnostics.ShowResult", $"FAIL · {result.Message}");
            }
        }

        /// <summary>
        /// Unexpected failure: details go to the log (with the exception type and message),
        /// the user sees one generic sentence. Nothing internal leaks through the banner.
        /// </summary>
        private void ReportFailure(string source, Exception ex, string safeMessage = null)
        {
            string message = safeMessage ?? Strings.ActionFailed;
            _log.Error(LogLayer.UI, source, ex, $"caught {ex.GetType().Name} — user sees the safe message");
            this.statusBanner.ShowBanner("✖ " + message, StatusKind.Error);
            this.statusBanner.SetStatus("failed", StatusKind.Error);
            AlertBox.Show(message, MessageBoxIcon.Error,
                alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        /// <summary>
        /// data → UI for the theme: Application.Theme is per session, so assigning a theme object here
        /// restyles THIS browser only (Application.LoadTheme would swap it for every session — the
        /// framework-level version of the static trap). ThemeCatalog reads the built-in theme JSON.
        /// </summary>
        private void ApplySessionTheme()
        {
            var theme = ThemeCatalog.Load(_ctx.Theme);
            if (theme == null)
                throw new InvalidOperationException($"Theme '{_ctx.Theme}' is not embedded in Wisej.Framework.dll.");

            Application.Theme = theme;
            _log.Info(LogLayer.Client, "Application.Theme", $"= {theme.Name} — this session only (other tabs keep theirs; LoadTheme would change all of them)");
        }

        #endregion

        #region Thin handlers (the card)

        /// <summary>Applies the three selectors to THIS session: sign in, switch tenant, select theme — each a service decision.</summary>
        private async void buttonApply_Click(object sender, EventArgs e)
        {
            try
            {
                string user = SelectedText(this.comboUser);
                string tenant = SelectedText(this.comboTenant);
                string theme = SelectedText(this.comboTheme);
                _log.Info(LogLayer.UI, "SessionDiagnostics.buttonApply_Click", $"user=\"{user}\" tenant={tenant} theme={theme} → ISessionService");

                // What the operator actually changed, measured against the context BEFORE any call:
                // signing in may move the tenant to the new operator's default, and that is not a request.
                bool userRequested = user != null && user != _ctx.CurrentUser;
                bool tenantRequested = tenant != null && tenant != _ctx.Tenant;
                bool themeRequested = theme != null && theme != _ctx.Theme;

                OperationResult<SessionContext> result = null;
                bool themeChanged = false;

                if (userRequested)
                    result = await _session.SignInAsync(user);

                if ((result == null || result.Succeeded) && tenantRequested && tenant != _ctx.Tenant)
                    result = await _session.SwitchTenantAsync(tenant);

                if ((result == null || result.Succeeded) && themeRequested)
                {
                    result = _session.SelectTheme(theme);
                    themeChanged = result.Succeeded;
                }

                if (result == null)
                    result = OperationResult<SessionContext>.Ok(_ctx, "Nothing to change — the selectors already match this session.");

                ShowResult(result);
                if (themeChanged)
                    ApplySessionTheme();

                SyncSelectorsToContext();
                RefreshDiagnostics();
                await RefreshGridAsync();
            }
            catch (DirectoryOutageException ex)
            {
                ReportFailure("SessionDiagnostics.buttonApply_Click", ex, Strings.DirectoryUnavailable);
            }
            catch (Exception ex)
            {
                ReportFailure("SessionDiagnostics.buttonApply_Click", ex);
            }
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            _log.Info(LogLayer.UI, "SessionDiagnostics.buttonRefresh_Click", "→ IDiagnosticsService.GetSnapshot()");
            RefreshDiagnostics();
            this.statusBanner.HideBanner();
            this.statusBanner.SetStatus("diagnostics refreshed", StatusKind.Success);
        }

        private void gridTickets_SelectionChanged(object sender, EventArgs e)
        {
            if (_fillingGrid || _loadingSelectors)
                return;

            int? id = SelectedTicketId();
            if (id == null)
            {
                this.labelSelected.Text = Strings.SelectTicketHint;
                return;
            }

            _log.Info(LogLayer.UI, "SessionDiagnostics.gridTickets_SelectionChanged", $"row #{id} → ISessionService.SelectTicket");
            _session.SelectTicket(id);
            this.labelSelected.Text = string.Format(Strings.TicketSelected, id);
            RefreshDiagnostics();
        }

        #endregion

        #region Bottom bar: success, progress, failure, probe, outage + recovery, clear

        /// <summary>Success path: the service stamps tenant and author from SessionContext; the shared store keeps the row.</summary>
        private async void buttonStamp_Click(object sender, EventArgs e)
        {
            try
            {
                string title = $"Stamped from session {_ctx.ShortId} at {DateTime.Now:HH:mm:ss}";
                _log.Info(LogLayer.UI, "SessionDiagnostics.buttonStamp_Click", $"→ ITicketService.CreateForCurrentUserAsync(\"{title}\")");
                var result = await _tickets.CreateForCurrentUserAsync(title);
                ShowResult(result);
                if (result.Succeeded)
                {
                    await RefreshGridAsync();
                    RefreshDiagnostics();
                }
            }
            catch (Exception ex)
            {
                ReportFailure("SessionDiagnostics.buttonStamp_Click", ex);
            }
        }

        /// <summary>Progress path: a Timer builds two throwaway session contexts per tick — none of them touches this session's.</summary>
        private void buttonSimulate_Click(object sender, EventArgs e)
        {
            if (this.timerSimulate.Enabled)
                return;

            _simulatedRemaining = 20;
            this.progressSimulate.Value = 0;
            this.progressSimulate.Visible = true;
            this.statusBanner.HideBanner();
            this.statusBanner.SetStatus("simulating 20 sessions", StatusKind.Busy);
            _log.Info(LogLayer.UI, "SessionDiagnostics.buttonSimulate_Click", "20 SessionContexts, 2 per tick — watch the shared counter grow while this session's panel stays put");
            this.timerSimulate.Start();
        }

        private void timerSimulate_Tick(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < 2 && _simulatedRemaining > 0; i++, _simulatedRemaining--)
                    _session.SimulateAnotherSession(20 - _simulatedRemaining + 1);

                this.progressSimulate.Value = 20 - _simulatedRemaining;
                this.statusBanner.SetStatus($"simulating {20 - _simulatedRemaining}/20", StatusKind.Busy);

                if (_simulatedRemaining == 0)
                {
                    this.timerSimulate.Stop();
                    this.progressSimulate.Visible = false;
                    _log.Info(LogLayer.UI, "SessionDiagnostics.timerSimulate_Tick", $"done — this session is still {_ctx}");
                    RefreshDiagnostics();
                    this.statusBanner.SetStatus("20 sessions simulated · this one unchanged", StatusKind.Success);
                }
            }
            catch (Exception ex)
            {
                this.timerSimulate.Stop();
                this.progressSimulate.Visible = false;
                ReportFailure("SessionDiagnostics.timerSimulate_Tick", ex);
            }
        }

        /// <summary>Failure path: a tenant the current operator does not belong to — UserAccount.IsMemberOf says no.</summary>
        private async void buttonForbiddenTenant_Click(object sender, EventArgs e)
        {
            try
            {
                string forbidden = FirstTenantNotInList();
                _log.Info(LogLayer.UI, "SessionDiagnostics.buttonForbiddenTenant_Click", $"→ ISessionService.SwitchTenantAsync({forbidden})");
                var result = await _session.SwitchTenantAsync(forbidden);
                ShowResult(result);
                if (result.Succeeded)
                {
                    SyncSelectorsToContext();
                    RefreshDiagnostics();
                    await RefreshGridAsync();
                }
            }
            catch (DirectoryOutageException ex)
            {
                ReportFailure("SessionDiagnostics.buttonForbiddenTenant_Click", ex, Strings.DirectoryUnavailable);
            }
            catch (Exception ex)
            {
                ReportFailure("SessionDiagnostics.buttonForbiddenTenant_Click", ex);
            }
        }

        /// <summary>Picks a tenant the current user is (probably) not a member of; the service decides for real.</summary>
        private string FirstTenantNotInList()
        {
            // Alice: Contoso+Fabrikam → Northwind; Bob: Contoso → Fabrikam; Sara: Northwind+Fabrikam → Contoso; Jae: Fabrikam → Contoso.
            switch (_ctx.CurrentUser)
            {
                case "Bob Chen": return "Fabrikam";
                case "Sara Patel": return "Contoso";
                case "Jae Kim": return "Contoso";
                default: return "Northwind";
            }
        }

        /// <summary>The audit probe: writes this session's user into the legacy static — and every other tab will read it.</summary>
        private void buttonLegacyStatic_Click(object sender, EventArgs e)
        {
            try
            {
                StaticLeakProbe.Write(_ctx.CurrentUser, _ctx.ShortId);
                _log.Warn(LogLayer.Infrastructure, "StaticLeakProbe.Write",
                    $"static LastWriter = \"{_ctx.CurrentUser}\" — ONE copy per server process: refresh the OTHER tab and it reads this value (the bug the audit removes)");
                RefreshDiagnostics();
                this.statusBanner.ShowBanner("⚠ Legacy static written — refresh diagnostics in the other tab to see it leak.", StatusKind.Warning);
                this.statusBanner.SetStatus("static probe written", StatusKind.Warning);
            }
            catch (Exception ex)
            {
                ReportFailure("SessionDiagnostics.buttonLegacyStatic_Click", ex);
            }
        }

        /// <summary>Error path + recovery: toggle the directory outage, then sign in again through the service.</summary>
        private async void buttonOutage_Click(object sender, EventArgs e)
        {
            if (_directory == null)
                return;

            _directory.SimulateOutage = !_directory.SimulateOutage;
            this.buttonOutage.Text = _directory.SimulateOutage ? "Recover the directory" : "Simulate directory outage";
            _log.Info(LogLayer.UI, "SessionDiagnostics.buttonOutage_Click",
                _directory.SimulateOutage
                    ? "outage ON → sign-in attempt (expect ✖ in DATA, safe message in UI, SessionContext unchanged)"
                    : "outage OFF → sign-in attempt (recovery)");

            try
            {
                var result = await _session.SignInAsync(_ctx.CurrentUser);
                ShowResult(result);
                RefreshDiagnostics();
            }
            catch (DirectoryOutageException ex)
            {
                ReportFailure("SessionDiagnostics.buttonOutage_Click", ex, Strings.DirectoryUnavailable);
                RefreshDiagnostics();   // proves the context survived the failed call untouched
            }
            catch (Exception ex)
            {
                ReportFailure("SessionDiagnostics.buttonOutage_Click", ex);
            }
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            this.tracePanel.ClearTrace();
            this.statusBanner.HideBanner();
            this.statusBanner.SetStatus("ready", StatusKind.Normal);
        }

        #endregion
    }
}
