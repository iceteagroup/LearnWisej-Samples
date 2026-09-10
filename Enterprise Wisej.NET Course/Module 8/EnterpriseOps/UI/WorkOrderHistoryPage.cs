using System;
using System.Collections.Generic;
using System.IO;
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
    /// EnterpriseOps — Work order history. The lab's <b>usage example screen</b>: the screen that proves the
    /// two reusable components are finished.
    ///
    /// <para>
    /// Left card: a work-order picker, the <see cref="StatusTimeline"/> UserControl and the
    /// <see cref="WorkOrderChartWidget"/> wrapper side by side, the grid the chart filters, a failure banner
    /// and the dark footer line. Right card: the live activity trace, where every layer says what it decided
    /// — including the two components, which report every option that went down and every event that came up.
    /// Bottom bar: four failure paths, the recovery and the anti-pattern gate.
    /// </para>
    ///
    /// <para>
    /// <b>What this file is allowed to know:</b> the components' typed properties, their methods and their
    /// named events. It contains no package name, no script, no option name, no JavaScript, no vendor name
    /// and no wire format — the "Anti-pattern: leaky screen" button proves it by running
    /// <see cref="ComponentApiGate"/> over this very file and over the screen that does all of those things.
    /// </para>
    ///
    /// <para>
    /// <b>The boundary:</b> this file owns UI state (which work order is picked, what the labels say, what
    /// the grid is bound to). Every decision — may this user see this work order, what counts as a reporting
    /// group, is this segment key real — is made in <see cref="WorkOrderHistoryService"/> and
    /// <see cref="AccessPolicy"/>. Every handler is a few lines and calls a service or a component method.
    /// </para>
    /// </summary>
    public partial class WorkOrderHistoryPage : Page
    {
        // Per-session services, created here (the composition root of this screen). Instance fields, never
        // statics: two browser sessions must never share a store, a policy or a trace.
        private readonly SessionContext _session;
        private readonly ActivityTrace _trace;
        private readonly WorkOrderHistoryService _service;
        private readonly ComponentApiGate _gate;

        private int _workOrderId = 2002;

        /// <summary>Designer / default constructor: the walkthrough's session (fabrikam, ana.ops).</summary>
        public WorkOrderHistoryPage()
            : this(new SessionContext("fabrikam", "Fabrikam Field Services", "ana.ops", "Manager"))
        {
        }

        public WorkOrderHistoryPage(SessionContext session)
        {
            InitializeComponent();

            _session = session;
            _trace = new ActivityTrace();
            _trace.EntryAdded += trace_EntryAdded;

            _service = new WorkOrderHistoryService(new FakeWorkOrderStore(), new AccessPolicy(_trace), _trace);
            _gate = new ComponentApiGate(_trace);

            // Diagnostics: the wrapper reports what it sends and what it receives, so the trace shows the
            // contract working without the screen knowing a single option name.
            chartWorkOrders.Trace += chartWorkOrders_Trace;
        }

        #region Load

        private async void WorkOrderHistoryPage_Load(object sender, EventArgs e)
        {
            _trace.Ui($"WorkOrderHistoryPage_Load  session={_session.TenantId}/{_session.UserName} ({_session.Role})");
            lblTenant.Text = "tenant: " + _session.TenantId;
            lblUser.Text = $"Signed in: {_session.UserName} · {_session.Role}";

            _trace.Package($"{ComponentResourcePackage.PackageName} {ComponentResourcePackage.PackageVersion} registered by the components, not by this screen");

            FillPicker();
            await ReloadAsync();
        }

        /// <summary>The tenant's first work orders, plus one that belongs to another tenant (the denied path).</summary>
        private void FillPicker()
        {
            cboWorkOrder.Items.Clear();
            foreach (WorkOrder workOrder in _service.PickerWorkOrders(_session))
                cboWorkOrder.Items.Add(new WorkOrderPick(workOrder));

            int index = cboWorkOrder.Items.Cast<WorkOrderPick>().ToList().FindIndex(p => p.Id == _workOrderId);
            cboWorkOrder.SelectedIndex = index >= 0 ? index : 0;
        }

        #endregion

        #region Event handlers — thin, one service or component call each

        /// <summary>The success path: the service returns typed results, the components render them.</summary>
        private async void btnReload_Click(object sender, EventArgs e)
        {
            try
            {
                await ReloadAsync();
            }
            catch (Exception ex)
            {
                ReportUnexpected(ex);
            }
        }

        private async void cboWorkOrder_SelectedIndexChanged(object sender, EventArgs e)
        {
            var pick = cboWorkOrder.SelectedItem as WorkOrderPick;
            if (pick == null || pick.Id == _workOrderId)
                return;

            _workOrderId = pick.Id;
            _trace.Ui($"cboWorkOrder_SelectedIndexChanged → work order {_workOrderId}");

            try
            {
                await LoadHistoryAsync();
            }
            catch (Exception ex)
            {
                ReportUnexpected(ex);
            }
        }

        /// <summary>The user clicked an entry of the reusable timeline. A named event with a typed argument.</summary>
        private void statusTimeline_ItemSelected(object sender, TimelineItemEventArgs e)
        {
            _trace.Client($"StatusTimeline.ItemSelected → #{e.Index} {e.Item.Status} at {e.Item.At:yyyy-MM-dd HH:mm} ({e.Item.Severity})");
            _trace.Component(statusTimeline.DescribeContract());
            ShowOk($"● timeline entry {e.Index + 1} of {statusTimeline.Items.Count}: {e.Item.Status}");
            lblStatusBar.Text = $"ItemSelected — {e.Item.Status}: {e.Item.Message}";
        }

        /// <summary>
        /// The walkthrough's contract in one handler: a click in the browser arrives as a <b>named server
        /// event</b> carrying a key, and the server — not the browser — decides what that key may see.
        /// </summary>
        private async void chartWorkOrders_SegmentClicked(object sender, ChartSegmentEventArgs e)
        {
            _trace.Client($"WorkOrderChartWidget.SegmentClicked → key '{e.Key}' ({e.Value}, {e.Percent}%) — a key, not a work order");

            try
            {
                await ShowSegmentAsync(e.Key);
            }
            catch (Exception ex)
            {
                ReportUnexpected(ex);
            }
        }

        /// <summary>
        /// The wrapper caught a client failure and reported it. The chart is already showing its embedded
        /// fallback, the timeline is untouched, and the screen simply says so.
        /// </summary>
        private void chartWorkOrders_WidgetError(object sender, WidgetErrorEventArgs e)
        {
            _trace.Client($"WorkOrderChartWidget.WidgetError → phase '{e.Phase}': {e.Message}");
            _trace.Component($"fallback rendered: {e.FallbackRendered} · the timeline and the grid are unaffected");

            ShowWarning($"⚠ chart unavailable — {e.Phase}: {e.Message}");
            lblStatusBar.Text = "WidgetLoadError — vendor-opschart.js · fallback rendered · screen still works";
            AlertBox.Show("The chart could not be drawn. The numbers are shown as a list instead.",
                MessageBoxIcon.Warning,
                alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        /// <summary>What the components render with no services and no data — exactly what the Designer shows.</summary>
        private void btnSampleMode_Click(object sender, EventArgs e)
        {
            bool on = !statusTimeline.SampleMode;
            _trace.Ui($"btnSampleMode_Click → SampleMode = {on} on both components (no service call)");

            statusTimeline.SampleMode = on;
            chartWorkOrders.SampleMode = on;

            ShowOk(on ? "● design-time sample mode" : "● live data");
            lblStatusBar.Text = on
                ? "SampleMode = true — the components render sample data; nothing was queried"
                : "SampleMode = false — click Reload to fetch live data again";
        }

        /// <summary>The embedded resource package: what ships inside the assembly, in what order.</summary>
        private void btnResourcePackage_Click(object sender, EventArgs e)
        {
            _trace.Ui("btnResourcePackage_Click → ComponentResourcePackage.Describe() + Verify()");

            foreach (string line in ComponentResourcePackage.Describe())
                _trace.Package(line);

            IReadOnlyList<string> problems = ComponentResourcePackage.Verify(ProjectRoot());
            if (problems.Count == 0)
            {
                _trace.Package("✓ every declared resource is embedded and the served copies match");
                ShowOk("● resource package intact");
                lblStatusBar.Text = $"{ComponentResourcePackage.PackageName} {ComponentResourcePackage.PackageVersion} — {ComponentResourcePackage.All.Count} resources, all embedded";
                return;
            }

            foreach (string problem in problems)
                _trace.Package("✗ " + problem);

            ShowWarning($"⚠ resource package: {problems.Count} problem(s) — see the trace");
        }

        /// <summary>Failure path 1: an option the component does not support, refused on the server.</summary>
        private void btnBadOption_Click(object sender, EventArgs e)
        {
            _trace.Ui("btnBadOption_Click → chartWorkOrders.Palette = \"neon-pink\"");

            try
            {
                chartWorkOrders.Palette = "neon-pink";
            }
            catch (ArgumentOutOfRangeException ex)
            {
                // The component refused before anything was rendered: the chart on screen never changed.
                _trace.Component($"the wrapper threw: {ex.Message}");
                ShowFailure("✗ invalid palette refused on the server — nothing was sent to the browser");
                lblStatusBar.Text = $"Palette rejected · known palettes: {string.Join(", ", WorkOrderChartWidget.KnownPalettes)}";
                AlertBox.Show("That chart palette does not exist. The chart was left as it was.",
                    MessageBoxIcon.Error,
                    alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
            }
        }

        /// <summary>Failure path 2: a segment key the chart never rendered — the browser could have forged it.</summary>
        private async void btnForgedKey_Click(object sender, EventArgs e)
        {
            const string forged = "all-tenants";
            _trace.Ui($"btnForgedKey_Click → the service is called with the key '{forged}' as if the browser had sent it");

            try
            {
                await ShowSegmentAsync(forged);
            }
            catch (Exception ex)
            {
                ReportUnexpected(ex);
            }
        }

        /// <summary>Failure path 3: the vendor library throws; the adapter turns it into the contract's error event.</summary>
        private void btnVendorError_Click(object sender, EventArgs e)
        {
            _trace.Ui("btnVendorError_Click → chartWorkOrders.SimulateVendorFailure()");
            chartWorkOrders.SimulateVendorFailure();
            lblStatusBar.Text = "The next client update will be refused by the vendor — watch for WidgetError";
        }

        /// <summary>Failure path 4: the walkthrough's blocked script.</summary>
        private void btnBlockVendor_Click(object sender, EventArgs e)
        {
            _trace.Ui("btnBlockVendor_Click → chartWorkOrders.SimulateBlockedVendor = true (a proxy eats the package)");
            chartWorkOrders.SimulateBlockedVendor = true;
        }

        /// <summary>The recovery: the same component, the same API, the chart back.</summary>
        private async void btnRestoreVendor_Click(object sender, EventArgs e)
        {
            _trace.Ui("btnRestoreVendor_Click → SimulateBlockedVendor = false, then reload the breakdown");
            chartWorkOrders.SimulateBlockedVendor = false;

            try
            {
                await LoadBreakdownAsync();
                ShowOk("● chart restored");
                lblStatusBar.Text = "Vendor package available again — same wrapper, same API, same data";
            }
            catch (Exception ex)
            {
                ReportUnexpected(ex);
            }
        }

        /// <summary>The walkthrough's anti-pattern, measured against this screen.</summary>
        private void btnComponentGate_Click(object sender, EventArgs e)
        {
            _trace.Ui("btnComponentGate_Click → ComponentApiGate over the leaky screen and over this one");

            string root = ProjectRoot();
            ComponentApiReport leaky = _gate.Check(Path.Combine(root, "Controls", "Samples", "LeakyChartScreen.cs.txt"));
            ComponentApiReport here = _gate.Check(Path.Combine(root, "UI", "WorkOrderHistoryPage.cs"));

            if (here.Passed && !leaky.Passed)
            {
                ShowOk($"● {leaky.FileName}: {leaky.Findings.Count} leak(s) · {here.FileName}: none");
                lblStatusBar.Text = $"Component API gate — leaky screen touches {leaky.Findings.Count} kinds of internals and builds {leaky.InlineControls} controls by hand; this screen builds 0";
                return;
            }

            ShowWarning("⚠ the gate found component internals in this screen — see the trace");
        }

        private void btnClearTrace_Click(object sender, EventArgs e)
        {
            lstTrace.Items.Clear();
            _trace.Ui("btnClearTrace_Click → trace cleared");
        }

        #endregion

        #region The work — one service call each, results into the components

        private async Task ReloadAsync()
        {
            statusTimeline.SampleMode = false;
            chartWorkOrders.SampleMode = false;

            await LoadHistoryAsync();
            await LoadBreakdownAsync();
        }

        /// <summary>
        /// Reads the history and hands it to the component as <see cref="TimelineItem"/>s. The mapping is
        /// the boundary: the component gets when / what / why, and cannot leak the actor, the tenant or the
        /// version because it was never given them.
        /// </summary>
        private async Task LoadHistoryAsync()
        {
            CommandResult<WorkOrderHistoryView> result = await _service.GetHistoryAsync(_session, _workOrderId);
            lblCorrelation.Text = "corr " + result.CorrelationId;

            if (!result.Succeeded)
            {
                statusTimeline.Clear();
                statusTimeline.Caption = $"WORK ORDER {_workOrderId}";
                ShowFailure("✗ " + result.ErrorText);
                lblCardTitle.Text = $"Work order {_workOrderId} — not available";
                lblStatusBar.Text = $"AccessPolicy refused the read · correlation {result.CorrelationId}";
                return;
            }

            WorkOrderHistoryView history = result.Value;
            statusTimeline.Caption = $"STATUS TIMELINE · {history.Entries.Length} ENTRIES";
            statusTimeline.SetItems(history.Entries.Select(ToTimelineItem));

            lblCardTitle.Text = $"Work order {history.WorkOrderId} — {history.Title}";
            _trace.Component(statusTimeline.DescribeContract());

            ShowOk($"● history loaded · {history.Entries.Length} entries");
            lblStatusBar.Text = $"Work order {history.WorkOrderId} — history loaded · StatusTimeline + WorkOrderChartWidget · correlation {result.CorrelationId}";
        }

        /// <summary>Reads the tenant's status breakdown and hands it to the wrapper as chart segments.</summary>
        private async Task LoadBreakdownAsync()
        {
            CommandResult<IReadOnlyList<StatusCountView>> result = await _service.GetStatusBreakdownAsync(_session);
            lblCorrelation.Text = "corr " + result.CorrelationId;

            if (!result.Succeeded)
            {
                ShowFailure("✗ " + result.ErrorText);
                return;
            }

            chartWorkOrders.Caption = $"WORK-ORDER HISTORY · {_session.TenantName}";
            chartWorkOrders.SetSegments(result.Value.Select(c => new ChartSegment(c.Key, c.Label, c.Count)));
            _trace.Component("wire payload → " + chartWorkOrders.DescribeWirePayload());
        }

        /// <summary>
        /// The key came from the browser, so the service validates it again before it queries anything.
        /// An unknown key is a rejected command, not an exception and not an empty grid.
        /// </summary>
        private async Task ShowSegmentAsync(string groupKey)
        {
            CommandResult<PagedResult<WorkQueueRow>> result = await _service.GetWorkOrdersByGroupAsync(_session, groupKey);
            lblCorrelation.Text = "corr " + result.CorrelationId;

            if (!result.Succeeded)
            {
                dgvSegment.DataSource = null;
                ShowFailure("✗ " + result.ErrorText);
                lblStatusBar.Text = $"Segment key refused by the service · correlation {result.CorrelationId}";
                AlertBox.Show("That view is not available.", MessageBoxIcon.Error,
                    alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
                return;
            }

            dgvSegment.DataSource = result.Value.Rows.ToList();
            chartWorkOrders.Select(groupKey);

            ShowOk($"● {result.Value.Total} work orders in '{groupKey}'");
            lblStatusBar.Text = $"SegmentClicked(\"{groupKey}\") — named server event · grid filtered server-side · {result.Value.Total} rows";
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

        #region Screen plumbing

        private void trace_EntryAdded(object sender, string line)
        {
            lstTrace.Items.Add(line);
            lstTrace.SelectedIndex = lstTrace.Items.Count - 1;
        }

        private void chartWorkOrders_Trace(object sender, ComponentTraceEventArgs e)
            => _trace.Component($"{e.Arrow}  {e.Name}  {e.Payload}");

        private void ShowOk(string text)
        {
            lblStatus.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            lblStatus.Text = text;
            lblBanner.Visible = false;
        }

        private void ShowWarning(string text)
        {
            lblStatus.ForeColor = System.Drawing.Color.FromArgb(232, 161, 60);
            lblStatus.Text = "● warning";
            lblBanner.BackColor = System.Drawing.Color.FromArgb(255, 248, 236);
            lblBanner.ForeColor = System.Drawing.Color.FromArgb(150, 100, 20);
            lblBanner.Text = text;
            lblBanner.Visible = true;
        }

        private void ShowFailure(string text)
        {
            lblStatus.ForeColor = System.Drawing.Color.FromArgb(224, 86, 59);
            lblStatus.Text = "● refused";
            lblBanner.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
            lblBanner.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            lblBanner.Text = text;
            lblBanner.Visible = true;
        }

        private void ReportUnexpected(Exception ex)
        {
            _trace.Write("Service: unexpected " + ex.GetType().Name + " — " + ex.Message);
            ShowFailure("✗ the action could not be completed. Check the trace for details.");
            AlertBox.Show("The action could not be completed. Check the log for details.",
                MessageBoxIcon.Error,
                alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        /// <summary>
        /// Where the sources and the served component files live. Under <c>dotnet run</c> that is the
        /// current directory; from a build output it is found by walking up to the project file.
        /// </summary>
        private static string ProjectRoot()
        {
            var directory = new DirectoryInfo(Environment.CurrentDirectory);
            for (int i = 0; i < 6 && directory != null; i++)
            {
                if (File.Exists(Path.Combine(directory.FullName, "EnterpriseOps.csproj")))
                    return directory.FullName;
                directory = directory.Parent;
            }

            directory = new DirectoryInfo(AppContext.BaseDirectory);
            for (int i = 0; i < 6 && directory != null; i++)
            {
                if (File.Exists(Path.Combine(directory.FullName, "EnterpriseOps.csproj")))
                    return directory.FullName;
                directory = directory.Parent;
            }

            return Environment.CurrentDirectory;
        }

        /// <summary>One entry of the work-order picker. A view model: it carries the id and what to show.</summary>
        private sealed class WorkOrderPick
        {
            private readonly string _text;

            public WorkOrderPick(WorkOrder workOrder)
            {
                this.Id = workOrder.Id;
                _text = $"{workOrder.Id} · {workOrder.Title} ({workOrder.TenantId})";
            }

            public int Id { get; }

            public override string ToString() => _text;
        }

        #endregion
    }
}
