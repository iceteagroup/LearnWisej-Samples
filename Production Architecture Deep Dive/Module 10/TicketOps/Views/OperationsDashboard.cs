using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TicketOps.Controls;
using TicketOps.Data;
using TicketOps.Domain;
using TicketOps.Infrastructure;
using TicketOps.Resources;
using TicketOps.Services;
using Wisej.Web;

namespace TicketOps.Views
{
    /// <summary>
    /// TicketOps Console · Operations Dashboard — the Module 10 screen.
    ///
    /// Left card:   the themed, localized dashboard: a Theme and a Culture picker in the header, four KPI cards
    ///              (one StatusChip each), the work-order grid and a detail strip — display + input only.
    /// Right card:  the Diagnostics activity trace: every click travels UI → SVC → DATA/DOMAIN and back, and the
    ///              theme/culture switches show what the session (SESSION) and the framework (INFRA) did.
    /// Bottom bar:  success (refresh), progress (walk a work order through its statuses), two failure paths
    ///              (a domain rule; resource gaps), the error path (simulated outage) with recovery, Clear trace.
    ///
    /// What is NOT in this file: a colour, a hex value, a date pattern, a currency symbol or an English sentence
    /// meant for the operator. Colours belong to the theme (and the chip's theme state), text to the resources,
    /// formats to the session's CultureInfo — so a redesign or a new language never means editing a handler.
    /// Handlers that await are "async void", so each owns its try/catch; the operator sees Strings.ActionFailed,
    /// never an exception message.
    /// </summary>
    public partial class OperationsDashboard : Form
    {
        private static readonly string[] CultureChoices = { "en-US", "de-DE", "it-IT" };   // it-IT is offered but not shipped: the validation path

        private readonly IWorkOrderService _workOrders;
        private readonly ILocalizationService _localization;
        private readonly ThemeSwitcher _themes;
        private readonly InMemoryWorkOrderRepository _repository;   // only for the lab's outage switch
        private readonly ILog _log;

        private DashboardSnapshot _snapshot = new DashboardSnapshot(new List<WorkOrder>());
        private int? _selectedId;
        private int _walkId;
        private int _walkStep;
        private bool _syncingHeader;

        // The Designer keeps the parameterless constructor; real wiring goes through the other one.
        public OperationsDashboard() : this(null, null, null, null, new ActivityLog())
        {
        }

        public OperationsDashboard(IWorkOrderService workOrders, ILocalizationService localization, ThemeSwitcher themes,
                                   InMemoryWorkOrderRepository repository, ILog log)
        {
            InitializeComponent();

            _workOrders = workOrders;
            _localization = localization;
            _themes = themes;
            _repository = repository;
            _log = log;

            if (log is ActivityLog activityLog)
                this.tracePanel.Attach(activityLog);
        }

        #region Screen lifecycle

        private async void OperationsDashboard_Load(object sender, EventArgs e)
        {
            try
            {
                _log.Info(LogLayer.UI, "OperationsDashboard.Load", "screen shown → restore the session's theme, resolve the resources, load the dashboard");

                Application.ThemeChanged += Application_ThemeChanged;
                Application.CultureChanged += Application_CultureChanged;

                _themes.RestoreSaved();                 // before the first paint: no flash of the wrong theme
                ApplyLocalizedText();                   // every caption from Resources/Strings.resx (fills the theme picker)
                SyncHeaderControls();                   // pickers reflect the session without firing their events
                await RefreshAsync();
            }
            catch (Exception ex)
            {
                ReportFailure("OperationsDashboard.Load", ex);
            }
        }

        private void OperationsDashboard_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.ThemeChanged -= Application_ThemeChanged;
            Application.CultureChanged -= Application_CultureChanged;
        }

        private void Application_ThemeChanged(object sender, EventArgs e)
        {
            _log.Info(LogLayer.Session, "Application.ThemeChanged", $"→ \"{_themes.Current}\" — every open window re-renders against the new appearances");
        }

        private void Application_CultureChanged(object sender, EventArgs e)
        {
            _log.Info(LogLayer.Session, "Application.CultureChanged", $"→ {Application.CurrentCulture?.Name} — the client locale follows the session");
        }

        #endregion

        #region data → UI: resources, formatting, binding

        private string T(string key) => _localization.Text(key);

        /// <summary>Every caption on the screen, resolved for the session's culture. Called on load and after each culture switch.</summary>
        private void ApplyLocalizedText()
        {
            int before = TraceCountSnapshot();

            this.Text = T("App.Title") + " — Module 10";
            this.labelScreenTitle.Text = T("Dashboard.Title");
            this.labelThemeCaption.Text = T("Header.Theme");
            this.labelCultureCaption.Text = T("Header.Culture");
            this.checkSessionOnly.Text = T("Header.SessionOnly");

            _syncingHeader = true;
            int themeIndex = Math.Max(0, this.comboTheme.SelectedIndex);
            this.comboTheme.Items.Clear();
            this.comboTheme.Items.AddRange(new object[] { T("Theme.Light"), T("Theme.Dark") });
            this.comboTheme.SelectedIndex = themeIndex;
            _syncingHeader = false;

            this.columnId.HeaderText = T("Column.Id");
            this.columnTitle.HeaderText = T("Column.WorkOrder");
            this.columnStatus.HeaderText = T("Column.Status");
            this.columnDue.HeaderText = T("Column.Due");
            this.columnCost.HeaderText = T("Column.Cost");

            this.buttonOpenDetail.Text = T("Button.OpenDetail");
            this.buttonNextStatus.Text = T("Button.NextStatus");
            this.buttonRefresh.Text = "↻ " + T("Button.Refresh");

            _log.Info(LogLayer.UI, "OperationsDashboard.ApplyLocalizedText",
                $"captions resolved for {_localization.Culture.Name} · request thread UI culture = {Thread.CurrentThread.CurrentUICulture.Name} · resource warnings so far: {TraceCountSnapshot() - before}");
        }

        /// <summary>The only place that fills the KPI cards, the grid and the detail strip — from raw values, formatted with the culture.</summary>
        private void Bind(DashboardSnapshot snapshot)
        {
            var culture = _localization.Culture;

            this.labelCountOpen.Text = snapshot.CountOf(WorkOrderStatus.Open).ToString(culture);
            this.labelCountInProgress.Text = snapshot.CountOf(WorkOrderStatus.InProgress).ToString(culture);
            this.labelCountBlocked.Text = snapshot.CountOf(WorkOrderStatus.Blocked).ToString(culture);
            this.labelCountDone.Text = snapshot.CountOf(WorkOrderStatus.Done).ToString(culture);

            // The same control four times: set the status, hand it the resource text. No colour is chosen here.
            this.chipOpen.Show(WorkOrderStatus.Open, _localization.StatusText(WorkOrderStatus.Open));
            this.chipInProgress.Show(WorkOrderStatus.InProgress, _localization.StatusText(WorkOrderStatus.InProgress));
            this.chipBlocked.Show(WorkOrderStatus.Blocked, _localization.StatusText(WorkOrderStatus.Blocked));
            this.chipDone.Show(WorkOrderStatus.Done, _localization.StatusText(WorkOrderStatus.Done));

            this.labelSummary.Text = _localization.Format("Dashboard.Summary", snapshot.Orders.Count, snapshot.OpenCount);

            this.gridWorkOrders.Rows.Clear();
            foreach (var o in snapshot.Orders)
                this.gridWorkOrders.Rows.Add(o.Id, o.Title, _localization.StatusText(o.Status), _localization.FormatDate(o.DueOn), _localization.FormatCurrency(o.LaborCost));

            // Keep the operator's selection across refreshes (default: the first order).
            int index = _selectedId.HasValue ? IndexOf(snapshot, _selectedId.Value) : 0;
            if (index < 0) index = 0;
            if (snapshot.Orders.Count > 0)
            {
                this.gridWorkOrders.Rows[index].Selected = true;
                UpdateDetailStrip(snapshot.Orders[index]);
            }

            var sample = snapshot.Orders.FirstOrDefault(o => o.Id == 2002) ?? snapshot.Orders.FirstOrDefault();
            _log.Info(LogLayer.UI, "OperationsDashboard.Bind",
                sample == null
                    ? "0 rows"
                    : $"{snapshot.Orders.Count} rows · 4 KPI chips · formatted with {culture.Name}: #{sample.Id} due {_localization.FormatDate(sample.DueOn)} · {_localization.FormatCurrency(sample.LaborCost)}");
        }

        private void UpdateDetailStrip(WorkOrder order)
        {
            _selectedId = order.Id;
            this.labelSelected.Text = _localization.Format("Detail.Title", order.Id);
            this.chipSelected.Show(order.Status, _localization.StatusText(order.Status));
            this.labelDueValue.Text = $"{T("Detail.Due")}: {_localization.FormatDate(order.DueOn)}";
            this.labelCostValue.Text = $"{T("Detail.Cost")}: {_localization.FormatCurrency(order.LaborCost)}";
            this.labelCreatedValue.Text = $"{T("Detail.Created")}: {_localization.FormatDateTime(order.CreatedOn)}";
            this.labelHoursValue.Text = $"{T("Detail.Hours")}: {_localization.FormatNumber(order.Hours)}";
        }

        /// <summary>Theme and culture pickers show the session's current choice without raising their change events.</summary>
        private void SyncHeaderControls()
        {
            _syncingHeader = true;
            try
            {
                this.comboTheme.SelectedIndex = _themes.IsDark ? 1 : 0;
                int cultureIndex = Array.IndexOf(CultureChoices, _localization.Culture.Name);
                this.comboCulture.SelectedIndex = cultureIndex < 0 ? 0 : cultureIndex;
            }
            finally
            {
                _syncingHeader = false;
            }
        }

        private static int IndexOf(DashboardSnapshot snapshot, int id)
        {
            for (int i = 0; i < snapshot.Orders.Count; i++)
                if (snapshot.Orders[i].Id == id) return i;
            return -1;
        }

        private int TraceCountSnapshot() => (_log as ActivityLog)?.Entries.Count ?? 0;

        private async Task RefreshAsync()
        {
            try
            {
                this.statusBanner.SetStatus(T("State.Loading"), StatusKind.Busy);
                _snapshot = await _workOrders.GetDashboardAsync();
                Bind(_snapshot);
                this.statusBanner.HideBanner();
                this.statusBanner.SetStatus(T("State.Ready"), StatusKind.Success);
            }
            catch (Exception ex)
            {
                ReportFailure("OperationsDashboard.Refresh", ex);
            }
        }

        private void ShowResult<TValue>(OperationResult<TValue> result)
        {
            if (result.Succeeded)
            {
                this.statusBanner.HideBanner();
                this.statusBanner.SetStatus(T("State.Ready"), StatusKind.Success);
                this.labelFooter.Text = result.Message;
                _log.Info(LogLayer.UI, "OperationsDashboard.ShowResult", $"OK · {result.Message}");
            }
            else
            {
                // Expected outcome: the service explained it — already in the operator's language.
                this.statusBanner.ShowBanner(result.Message, StatusKind.Warning);
                this.statusBanner.SetStatus(T("State.Rejected"), StatusKind.Warning);
                _log.Warn(LogLayer.UI, "OperationsDashboard.ShowResult", $"FAIL · {result.Message}");
            }
        }

        /// <summary>
        /// Unexpected failure: details go to the log (exception type and message), the operator sees one localized,
        /// generic sentence. Nothing internal leaks through the banner — in any language.
        /// </summary>
        private void ReportFailure(string source, Exception ex)
        {
            _log.Error(LogLayer.UI, source, ex,
                $"caught {ex.GetType().Name} — user sees Strings.ActionFailed resolved for thread UI culture {Thread.CurrentThread.CurrentUICulture.Name}");
            this.statusBanner.ShowBanner("✖ " + Strings.ActionFailed, StatusKind.Error);
            this.statusBanner.SetStatus(T("State.Failed"), StatusKind.Error);
            AlertBox.Show(Strings.ActionFailed, MessageBoxIcon.Error,
                alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        #endregion

        #region Header: theme and culture (success paths + the culture validation path)

        private void comboTheme_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_syncingHeader)
                return;

            try
            {
                string themeName = this.comboTheme.SelectedIndex == 1 ? ThemeSwitcher.Dark : ThemeSwitcher.Light;
                bool sessionOnly = this.checkSessionOnly.Checked;
                _log.Info(LogLayer.UI, "OperationsDashboard.comboTheme_SelectedIndexChanged",
                    $"→ ThemeSwitcher.Apply(\"{themeName}\", sessionOnly: {sessionOnly}) — the handler names a theme, nothing else");

                _themes.Apply(themeName, sessionOnly);

                this.labelFooter.Text = _localization.Format(sessionOnly ? "Message.ThemeAppliedSession" : "Message.ThemeApplied", themeName);
                this.statusBanner.HideBanner();
                this.statusBanner.SetStatus(T("State.Ready"), StatusKind.Success);
                _log.Info(LogLayer.UI, "OperationsDashboard.comboTheme_SelectedIndexChanged",
                    "5 StatusChips, the grid, the cards and the trace re-skinned by the theme — no chip code ran, no control was rebuilt");
            }
            catch (Exception ex)
            {
                ReportFailure("OperationsDashboard.comboTheme_SelectedIndexChanged", ex);
            }
        }

        private void comboCulture_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_syncingHeader)
                return;

            try
            {
                string cultureName = CultureChoices[Math.Max(0, this.comboCulture.SelectedIndex)];
                _log.Info(LogLayer.UI, "OperationsDashboard.comboCulture_SelectedIndexChanged", $"→ ILocalizationService.SetCulture(\"{cultureName}\")");

                var result = _localization.SetCulture(cultureName);        // the decision (shipped or not) lives in the service
                if (!result.Succeeded)
                {
                    ShowResult(result);                                     // validation path: stays on the current culture
                    SyncHeaderControls();
                    return;
                }

                // Session-wide: Wisej.NET carries the culture to every later request of this session and to the client locale.
                Application.CurrentCulture = result.Value;
                _log.Info(LogLayer.Session, "Application.CurrentCulture", $"= {result.Value.Name} · request thread UI culture now {Thread.CurrentThread.CurrentUICulture.Name}");

                ApplyLocalizedText();                                       // captions follow the culture …
                Bind(_snapshot);                                            // … and so do dates, numbers and currency — same values, no reload
                ShowResult(result);
            }
            catch (Exception ex)
            {
                ReportFailure("OperationsDashboard.comboCulture_SelectedIndexChanged", ex);
            }
        }

        #endregion

        #region Thin handlers on the screen

        private void gridWorkOrders_SelectionChanged(object sender, EventArgs e)
        {
            var row = this.gridWorkOrders.CurrentRow;
            if (row == null || row.Index < 0 || row.Index >= _snapshot.Orders.Count)
                return;

            UpdateDetailStrip(_snapshot.Orders[row.Index]);
        }

        private async void buttonOpenDetail_Click(object sender, EventArgs e)
        {
            try
            {
                if (!_selectedId.HasValue)
                {
                    ShowResult(OperationResult<WorkOrder>.Fail(T("Rule.SelectWorkOrder")));
                    return;
                }

                _log.Info(LogLayer.UI, "OperationsDashboard.buttonOpenDetail_Click", $"→ IWorkOrderService.FindAsync(#{_selectedId}) → WorkOrderDetail dialog (second screen, same theme, same resources, same culture)");
                var order = await _workOrders.FindAsync(_selectedId.Value);
                if (order == null)
                {
                    ShowResult(OperationResult<WorkOrder>.Fail(T("Rule.WorkOrderMissing")));
                    return;
                }

                using (var dialog = new WorkOrderDetail(order, _workOrders, _localization, _log))
                {
                    await dialog.ShowDialogAsync();
                }

                _log.Info(LogLayer.UI, "OperationsDashboard.buttonOpenDetail_Click", "dialog closed → refresh (its status may have advanced)");
                await RefreshAsync();
            }
            catch (Exception ex)
            {
                ReportFailure("OperationsDashboard.buttonOpenDetail_Click", ex);
            }
        }

        private async void buttonNextStatus_Click(object sender, EventArgs e)
        {
            try
            {
                if (!_selectedId.HasValue)
                {
                    ShowResult(OperationResult<WorkOrder>.Fail(T("Rule.SelectWorkOrder")));
                    return;
                }

                _log.Info(LogLayer.UI, "OperationsDashboard.buttonNextStatus_Click", $"→ IWorkOrderService.AdvanceStatusAsync(#{_selectedId})");
                var result = await _workOrders.AdvanceStatusAsync(_selectedId.Value);   // the rule lives in the domain
                ShowResult(result);                                                     // data → UI
                if (result.Succeeded)
                    await RefreshAsync();
            }
            catch (Exception ex)
            {
                ReportFailure("OperationsDashboard.buttonNextStatus_Click", ex);
            }
        }

        #endregion

        #region Bottom bar: success, progress, failures, outage + recovery, clear

        /// <summary>Success path: reload through the service and re-format for the current culture.</summary>
        private async void buttonRefresh_Click(object sender, EventArgs e)
        {
            _log.Info(LogLayer.UI, "OperationsDashboard.buttonRefresh_Click", "→ IWorkOrderService.GetDashboardAsync()");
            await RefreshAsync();
            if (!_repository.SimulateOutage)
                this.labelFooter.Text = T("Message.DashboardRefreshed");
        }

        /// <summary>Progress path: a Timer walks one work order Open → In progress → Blocked → Done through the same service.</summary>
        private void buttonWalk_Click(object sender, EventArgs e)
        {
            if (this.timerWalk.Enabled)
                return;

            var candidate = (_selectedId.HasValue ? _snapshot.Orders.FirstOrDefault(o => o.Id == _selectedId.Value && o.Status != WorkOrderStatus.Done) : null)
                            ?? _snapshot.Orders.FirstOrDefault(o => o.Status == WorkOrderStatus.Open)
                            ?? _snapshot.Orders.FirstOrDefault(o => o.Status != WorkOrderStatus.Done);
            if (candidate == null)
            {
                ShowResult(OperationResult<WorkOrder>.Fail(T("Rule.DoneCannotAdvance")));
                return;
            }

            _walkId = candidate.Id;
            _walkStep = 0;
            _selectedId = candidate.Id;
            this.progressWalk.Value = 0;
            this.progressWalk.Visible = true;
            this.statusBanner.HideBanner();
            this.statusBanner.SetStatus(T("State.Walking"), StatusKind.Busy);
            _log.Info(LogLayer.UI, "OperationsDashboard.buttonWalk_Click", $"#{_walkId} from {candidate.Status}: one AdvanceStatusAsync per tick — the chip re-colours through its theme state, the caption through the resources");
            this.timerWalk.Start();
        }

        private async void timerWalk_Tick(object sender, EventArgs e)
        {
            try
            {
                var result = await _workOrders.AdvanceStatusAsync(_walkId);
                ShowResult(result);
                await RefreshAsync();

                _walkStep++;
                this.progressWalk.Value = Math.Min(_walkStep, this.progressWalk.Maximum);
                this.statusBanner.SetStatus($"{T("State.Walking")} {_walkStep}/{this.progressWalk.Maximum}", StatusKind.Busy);

                if (!result.Succeeded || result.Value.Status == WorkOrderStatus.Done)
                {
                    this.timerWalk.Stop();
                    this.progressWalk.Visible = false;
                    this.statusBanner.SetStatus(T("State.Ready"), StatusKind.Success);
                    _log.Info(LogLayer.UI, "OperationsDashboard.timerWalk_Tick", $"walk complete: #{_walkId} is {result.Value?.Status}");
                }
            }
            catch (Exception ex)
            {
                this.timerWalk.Stop();
                this.progressWalk.Visible = false;
                ReportFailure("OperationsDashboard.timerWalk_Tick", ex);
            }
        }

        /// <summary>Failure path 1: a domain rule. A finished order cannot advance; the handler never knew the rule.</summary>
        private async void buttonAdvanceDone_Click(object sender, EventArgs e)
        {
            try
            {
                var done = _snapshot.Orders.FirstOrDefault(o => o.Status == WorkOrderStatus.Done);
                int id = done?.Id ?? 2006;
                _log.Info(LogLayer.UI, "OperationsDashboard.buttonAdvanceDone_Click", $"→ IWorkOrderService.AdvanceStatusAsync(#{id}) (a finished order)");
                ShowResult(await _workOrders.AdvanceStatusAsync(id));
            }
            catch (Exception ex)
            {
                ReportFailure("OperationsDashboard.buttonAdvanceDone_Click", ex);
            }
        }

        /// <summary>
        /// Failure path 2: resource gaps. "Status.Archived" exists in no .resx → the key itself is shown and traced;
        /// "Button.Export" exists only in the neutral file → in German the English text shows and is traced as untranslated.
        /// The screen stays readable either way; nothing throws.
        /// </summary>
        private void buttonResourceGap_Click(object sender, EventArgs e)
        {
            try
            {
                _log.Info(LogLayer.UI, "OperationsDashboard.buttonResourceGap_Click", $"→ ILocalizationService.Text(\"Status.Archived\") and Text(\"Button.Export\") for {_localization.Culture.Name}");
                string missing = _localization.Text("Status.Archived");
                string untranslated = _localization.Text("Button.Export");

                this.statusBanner.ShowBanner($"{T("Message.ResourceGap")} → \"{missing}\" · \"{untranslated}\"", StatusKind.Warning);
                this.statusBanner.SetStatus(T("State.ResourceGap"), StatusKind.Warning);
            }
            catch (Exception ex)
            {
                ReportFailure("OperationsDashboard.buttonResourceGap_Click", ex);
            }
        }

        /// <summary>Error path + recovery: toggle the repository outage, then refresh through the service.</summary>
        private async void buttonOutage_Click(object sender, EventArgs e)
        {
            if (_repository == null)
                return;

            _repository.SimulateOutage = !_repository.SimulateOutage;
            this.buttonOutage.Text = _repository.SimulateOutage ? "Recover the data store" : "Simulate data outage";
            _log.Info(LogLayer.UI, "OperationsDashboard.buttonOutage_Click",
                _repository.SimulateOutage ? "outage ON → refresh (expect ✖ in DATA, localized safe message in UI)" : "outage OFF → refresh (recovery)");
            await RefreshAsync();
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            this.tracePanel.ClearTrace();
            this.statusBanner.HideBanner();
            this.statusBanner.SetStatus(T("State.Ready"), StatusKind.Normal);
        }

        #endregion
    }
}
