using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TicketOps.Controls;
using TicketOps.Domain;
using TicketOps.Infrastructure;
using TicketOps.Resources;
using TicketOps.Services;
using Wisej.Web;

namespace TicketOps.Views
{
    /// <summary>
    /// TicketOps Console · Operations Dashboard: a Theme and a Culture picker in the header, four KPI cards
    /// (one StatusChip each), the work-order grid and a detail strip.
    ///
    /// What is NOT in this file: a colour, a hex value, a date pattern, a currency symbol or an English sentence
    /// meant for the operator. Colours belong to the theme (and the chip's theme state), text to the resources,
    /// formats to the session's CultureInfo — so a redesign or a new language never means editing a handler.
    /// Handlers that await are "async void", so each owns its try/catch; the operator sees Strings.ActionFailed,
    /// never an exception message.
    /// </summary>
    public partial class OperationsDashboard : Form
    {
        private static readonly string[] CultureChoices = { "en-US", "de-DE", "it-IT" };   // it-IT is offered but not shipped: the service rejects it

        private readonly IWorkOrderService _workOrders;
        private readonly ILocalizationService _localization;
        private readonly ThemeSwitcher _themes;
        private readonly ILog _log;

        private DashboardSnapshot _snapshot = new DashboardSnapshot(new List<WorkOrder>());
        private int? _selectedId;
        private bool _syncingHeader;

        // The Designer keeps the parameterless constructor; real wiring goes through the other one.
        public OperationsDashboard() : this(null, null, null, new ActivityLog())
        {
        }

        public OperationsDashboard(IWorkOrderService workOrders, ILocalizationService localization, ThemeSwitcher themes, ILog log)
        {
            InitializeComponent();

            _workOrders = workOrders;
            _localization = localization;
            _themes = themes;
            _log = log;
        }

        private async void OperationsDashboard_Load(object sender, EventArgs e)
        {
            try
            {
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

        #region data → UI: resources, formatting, binding

        private string T(string key) => _localization.Text(key);

        /// <summary>Every caption on the screen, resolved for the session's culture. Called on load and after each culture switch.</summary>
        private void ApplyLocalizedText()
        {
            this.Text = T("App.Title");
            this.labelScreenTitle.Text = T("Dashboard.Title");
            this.labelThemeCaption.Text = T("Header.Theme");
            this.labelCultureCaption.Text = T("Header.Culture");

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
            }
            else
            {
                // Expected outcome: the service explained it — already in the operator's language.
                this.statusBanner.ShowBanner(result.Message, StatusKind.Warning);
                this.statusBanner.SetStatus(T("State.Rejected"), StatusKind.Warning);
            }
        }

        /// <summary>Unexpected failure: the details go to the log, the operator sees one localized, generic sentence.</summary>
        private void ReportFailure(string source, Exception ex)
        {
            _log.Error(LogLayer.UI, source, ex);
            this.statusBanner.ShowBanner("✖ " + Strings.ActionFailed, StatusKind.Error);
            this.statusBanner.SetStatus(T("State.Failed"), StatusKind.Error);
            AlertBox.Show(Strings.ActionFailed, MessageBoxIcon.Error,
                alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        #endregion

        #region Header: theme and culture

        private void comboTheme_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_syncingHeader)
                return;

            try
            {
                string themeName = this.comboTheme.SelectedIndex == 1 ? ThemeSwitcher.Dark : ThemeSwitcher.Light;
                _themes.Apply(themeName);                                   // the handler names a theme, nothing else

                this.labelFooter.Text = _localization.Format("Message.ThemeApplied", themeName);
                this.statusBanner.HideBanner();
                this.statusBanner.SetStatus(T("State.Ready"), StatusKind.Success);
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
                var result = _localization.SetCulture(cultureName);        // the decision (shipped or not) lives in the service
                if (!result.Succeeded)
                {
                    ShowResult(result);                                     // validation path: stays on the current culture
                    SyncHeaderControls();
                    return;
                }

                // Session-wide: Wisej.NET carries the culture to every later request of this session and to the client locale.
                Application.CurrentCulture = result.Value;

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

        #region Grid and buttons

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

                await RefreshAsync();                                       // its status may have advanced in the dialog
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

                var result = await _workOrders.AdvanceStatusAsync(_selectedId.Value);   // the rule lives in the domain
                ShowResult(result);
                if (result.Succeeded)
                    await RefreshAsync();
            }
            catch (Exception ex)
            {
                ReportFailure("OperationsDashboard.buttonNextStatus_Click", ex);
            }
        }

        #endregion
    }
}
