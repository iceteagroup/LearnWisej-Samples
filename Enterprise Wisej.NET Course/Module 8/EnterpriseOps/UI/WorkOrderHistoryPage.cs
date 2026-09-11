using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnterpriseOps.Controls;
using EnterpriseOps.Data;
using EnterpriseOps.Domain;
using EnterpriseOps.Security;
using EnterpriseOps.Services;
using EnterpriseOps.Widgets;
using Wisej.Web;

namespace EnterpriseOps.UI
{
    /// <summary>
    /// EnterpriseOps — Work order history. The usage example screen for the two reusable components: the
    /// <see cref="StatusTimeline"/> UserControl and the <see cref="WorkOrderChartWidget"/> wrapper, plus the
    /// grid the chart filters.
    ///
    /// <para>
    /// The screen only uses the components' typed properties, methods and named events. It owns UI state
    /// (what the labels say, what the grid is bound to); every decision — may this user see this work order,
    /// is this segment key real — is made in <see cref="WorkOrderHistoryService"/> and <see cref="AccessPolicy"/>.
    /// </para>
    /// </summary>
    public partial class WorkOrderHistoryPage : Page
    {
        // Per-session services, created here. Instance fields, never statics.
        private readonly SessionContext _session;
        private readonly ActivityTrace _trace;
        private readonly WorkOrderHistoryService _service;

        private readonly int _workOrderId = 2002;

        public WorkOrderHistoryPage()
            : this(new SessionContext("fabrikam", "Fabrikam Field Services", "ana.ops", "Manager"))
        {
        }

        public WorkOrderHistoryPage(SessionContext session)
        {
            InitializeComponent();

            _session = session;
            _trace = new ActivityTrace();
            _service = new WorkOrderHistoryService(new FakeWorkOrderStore(), new AccessPolicy(_trace), _trace);
        }

        #region Event handlers

        private async void WorkOrderHistoryPage_Load(object sender, EventArgs e)
        {
            try
            {
                await LoadHistoryAsync();
                await LoadBreakdownAsync();
            }
            catch (Exception ex)
            {
                ReportUnexpected(ex);
            }
            finally
            {
                // The awaits end after the request returned: push the final UI state to the browser.
                Application.Update(this);
            }
        }

        private void statusTimeline_ItemSelected(object sender, TimelineItemEventArgs e)
        {
            lblStatusBar.Text = $"{e.Item.Status}: {e.Item.Message}";
        }

        /// <summary>
        /// A click in the browser arrives as a named server event carrying a key; the service — not the
        /// browser — decides what that key may see.
        /// </summary>
        private async void chartWorkOrders_SegmentClicked(object sender, ChartSegmentEventArgs e)
        {
            try
            {
                await ShowSegmentAsync(e.Key);
            }
            catch (Exception ex)
            {
                ReportUnexpected(ex);
            }
            finally
            {
                // The awaits end after the request returned: push the final UI state to the browser.
                Application.Update(this);
            }
        }

        /// <summary>
        /// The wrapper caught a client failure. The chart is already showing its embedded fallback and the
        /// timeline is untouched; the screen logs the reason and tells the user.
        /// </summary>
        private void chartWorkOrders_WidgetError(object sender, WidgetErrorEventArgs e)
        {
            _trace.Component($"WorkOrderChartWidget error in '{e.Phase}': {e.Message} (fallback rendered: {e.FallbackRendered})");

            if (e.Phase == "init")
            {
                ShowFailure("WidgetLoadError — vendor-opschart.js · fallback rendered · screen still works");
                lblStatusBar.Text = "vendor-opschart.js unavailable — wrapper fell back · diagnostics logged · timeline unaffected";
            }
            else
            {
                ShowFailure($"Chart error ({e.Phase}) — fallback rendered · screen still works");
                lblStatusBar.Text = "Chart unavailable — wrapper fell back · diagnostics logged · timeline unaffected";
            }
        }

        #endregion

        #region Loading — one service call each, results into the components

        /// <summary>
        /// Reads the history and hands it to the component as <see cref="TimelineItem"/>s. The component
        /// gets when / what / why, and never the actor, the tenant or the version.
        /// </summary>
        private async Task LoadHistoryAsync()
        {
            CommandResult<WorkOrderHistoryView> result = await _service.GetHistoryAsync(_session, _workOrderId);

            if (!result.Succeeded)
            {
                statusTimeline.Clear();
                lblCardTitle.Text = $"Work order {_workOrderId} — not available";
                ShowFailure($"{result.ErrorText} · correlation {result.CorrelationId}");
                lblStatusBar.Text = $"Work order {_workOrderId} — history not available";
                return;
            }

            WorkOrderHistoryView history = result.Value;
            statusTimeline.SetItems(history.Entries.Select(ToTimelineItem));

            lblCardTitle.Text = $"Work order {history.WorkOrderId} — history";
            lblStatusBar.Text = $"Work order {history.WorkOrderId} — history loaded · StatusTimeline + WorkOrderChartWidget";
        }

        /// <summary>Reads the tenant's status breakdown and hands it to the wrapper as chart segments.</summary>
        private async Task LoadBreakdownAsync()
        {
            CommandResult<IReadOnlyList<StatusCountView>> result = await _service.GetStatusBreakdownAsync(_session);

            if (!result.Succeeded)
            {
                ShowFailure($"{result.ErrorText} · correlation {result.CorrelationId}");
                return;
            }

            chartWorkOrders.SetSegments(result.Value.Select(c => new ChartSegment(c.Key, c.Label, c.Count)));
        }

        /// <summary>
        /// The key came from the browser, so the service validates it again before it queries anything.
        /// An unknown key is a rejected command, not an exception and not an empty grid.
        /// </summary>
        private async Task ShowSegmentAsync(string groupKey)
        {
            CommandResult<PagedResult<WorkQueueRow>> result = await _service.GetWorkOrdersByGroupAsync(_session, groupKey);

            if (!result.Succeeded)
            {
                dgvSegment.DataSource = null;
                ShowFailure($"That view is not available ({result.ErrorText}) · correlation {result.CorrelationId}");
                return;
            }

            dgvSegment.DataSource = result.Value.Rows.ToList();
            chartWorkOrders.Select(groupKey);

            ShowInfo($"SegmentClicked → \"{groupKey}\" · {result.Value.Total} work orders shown below");
            lblStatusBar.Text = $"SegmentClicked(\"{groupKey}\") — named server event · grid filtered server-side";
        }

        #endregion

        #region Mapping (the only place the domain meets the components)

        private static TimelineItem ToTimelineItem(HistoryEntryView entry)
            => new TimelineItem(entry.AtUtc, entry.StatusLabel, entry.Note, SeverityOf(entry.Status));

        private static TimelineSeverity SeverityOf(WorkOrderStatus status)
        {
            switch (status)
            {
                case WorkOrderStatus.Assigned:
                case WorkOrderStatus.InProgress:
                    return TimelineSeverity.Info;
                case WorkOrderStatus.OnHold:
                    return TimelineSeverity.Warning;
                case WorkOrderStatus.Escalated:
                    return TimelineSeverity.Critical;
                case WorkOrderStatus.Completed:
                    return TimelineSeverity.Success;
                default:
                    return TimelineSeverity.Neutral;
            }
        }

        #endregion

        #region Messages

        private void ShowInfo(string text)
        {
            lblBanner.BackColor = System.Drawing.Color.FromArgb(234, 243, 255);
            lblBanner.ForeColor = System.Drawing.Color.FromArgb(11, 74, 143);
            lblBanner.Text = text;
            lblBanner.Visible = true;
        }

        private void ShowFailure(string text)
        {
            lblBanner.BackColor = System.Drawing.Color.FromArgb(253, 236, 236);
            lblBanner.ForeColor = System.Drawing.Color.FromArgb(124, 42, 42);
            lblBanner.Text = text;
            lblBanner.Visible = true;
        }

        private void ReportUnexpected(Exception ex)
        {
            _trace.Write($"UI: unexpected {ex.GetType().Name} — {ex.Message} [{_session.CorrelationId}]");
            ShowFailure($"The action could not be completed · correlation {_session.CorrelationId}");
            AlertBox.Show("The action could not be completed. Please try again.",
                MessageBoxIcon.Error,
                alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        #endregion
    }
}
