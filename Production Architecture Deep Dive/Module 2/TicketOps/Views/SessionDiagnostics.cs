using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TicketOps.Controls;
using TicketOps.Diagnostics;
using TicketOps.Domain;
using TicketOps.Infrastructure;
using TicketOps.Resources;
using TicketOps.Services;
using Wisej.Web;

namespace TicketOps.Views
{
    /// <summary>
    /// TicketOps Console · Session diagnostics. Two panels that must never be confused —
    /// <b>Application</b> (identical in every session) and <b>This session</b> (from the injected
    /// SessionContext) — plus the per-session selectors (operator / tenant / theme) and the tenant's tickets.
    ///
    /// The Form takes <see cref="SessionContext"/> in its constructor exactly like the services do: it never
    /// reads a static, never calls a <c>SessionContext.Current</c>.
    /// </summary>
    public partial class SessionDiagnostics : Form
    {
        private readonly SessionContext _ctx;
        private readonly ISessionService _session;
        private readonly ITicketService _tickets;
        private readonly IDiagnosticsService _diagnostics;
        private readonly ILog _log;

        private IReadOnlyList<Ticket> _rows = new List<Ticket>();
        private bool _loadingSelectors;
        private bool _fillingGrid;

        // The Designer keeps the parameterless constructor; real wiring goes through the other one.
        public SessionDiagnostics() : this(new SessionContext(), null, null, null, new ActivityLog())
        {
        }

        public SessionDiagnostics(SessionContext ctx, ISessionService session, ITicketService tickets,
            IDiagnosticsService diagnostics, ILog log)
        {
            InitializeComponent();

            _ctx = ctx;
            _session = session;
            _tickets = tickets;
            _diagnostics = diagnostics;
            _log = log;

            UpdateCaption();
        }

        private async void SessionDiagnostics_Load(object sender, EventArgs e)
        {
            await LoadSelectorsAsync();
            RefreshDiagnostics();
            await RefreshGridAsync();
        }

        /// <summary>The window caption belongs to THIS operator's console.</summary>
        private void UpdateCaption()
        {
            this.Text = $"TicketOps Console — {_ctx.Tenant} / {_ctx.CurrentUser}";
        }

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
            catch (Exception ex)
            {
                ReportFailure("SessionDiagnostics.LoadSelectors", ex);
            }
            finally
            {
                _loadingSelectors = false;
            }
        }

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

                UpdateCaption();
            }
            catch (Exception ex)
            {
                ReportFailure("SessionDiagnostics.RefreshDiagnostics", ex);
            }
        }

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
                this.labelSelected.Text = Strings.NoTicketSelected;
                this.statusBanner.SetStatus("ready", StatusKind.Success);
            }
            catch (Exception ex)
            {
                ReportFailure("SessionDiagnostics.RefreshGrid", ex);
            }
        }

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
            }
            else
            {
                this.statusBanner.ShowBanner(result.Message, StatusKind.Warning);
                this.statusBanner.SetStatus("not applied", StatusKind.Warning);
            }
        }

        /// <summary>Unexpected failure: the details go to the log, the user sees one safe sentence.</summary>
        private void ReportFailure(string source, Exception ex)
        {
            _log.Error(LogLayer.UI, source, ex);
            this.statusBanner.ShowBanner("✖ " + Strings.ActionFailed, StatusKind.Error);
            this.statusBanner.SetStatus("failed", StatusKind.Error);
            AlertBox.Show(Strings.ActionFailed, MessageBoxIcon.Error,
                alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        /// <summary>
        /// Application.Theme is per session, so assigning a theme object here restyles THIS browser only
        /// (Application.LoadTheme would swap it for every session). ThemeCatalog reads the built-in theme JSON.
        /// </summary>
        private void ApplySessionTheme()
        {
            var theme = ThemeCatalog.Load(_ctx.Theme);
            if (theme == null)
                throw new InvalidOperationException($"Theme '{_ctx.Theme}' is not embedded in Wisej.Framework.dll.");

            Application.Theme = theme;
        }

        /// <summary>Applies the three selectors to THIS session: sign in, switch tenant, select theme — each a service decision.</summary>
        private async void buttonApply_Click(object sender, EventArgs e)
        {
            try
            {
                string user = SelectedText(this.comboUser);
                string tenant = SelectedText(this.comboTenant);
                string theme = SelectedText(this.comboTheme);

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
                    result = OperationResult<SessionContext>.Ok(_ctx, "Nothing to change.");

                ShowResult(result);
                if (themeChanged)
                    ApplySessionTheme();

                SyncSelectorsToContext();
                RefreshDiagnostics();
                await RefreshGridAsync();
            }
            catch (Exception ex)
            {
                ReportFailure("SessionDiagnostics.buttonApply_Click", ex);
            }
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
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
                this.labelSelected.Text = Strings.NoTicketSelected;
                return;
            }

            _session.SelectTicket(id);
            this.labelSelected.Text = string.Format(Strings.TicketSelected, id);
            RefreshDiagnostics();
        }
    }
}
