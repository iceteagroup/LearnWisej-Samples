using System;
using System.Collections.Generic;
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
    /// TicketOps Console · Work Orders — the Module 9 screen (the video's "TicketOps — Work Orders" / MainPage).
    ///
    /// Left card:   global search (Ctrl+K from anywhere), the work-order grid, "Copy link", the last link the
    ///              server built (also the manual fallback), and the server-side audit log.
    /// Right card:  the activity trace. Interop lines are tagged [CLIENT]; everything the browser did is
    ///              visible next to what the server decided.
    /// Bottom bar:  progress path (verify 6 links server-side, no clipboard), two failure paths (unknown id,
    ///              domain rule), the client-side error path (clipboard denied → fallback, click again to
    ///              recover), the data outage with its recovery, and Clear trace.
    ///
    /// Three interop points, one rule: the browser requests or reports; the server validates, decides and records.
    ///   1. Ctrl+K   — JavaScript (embedded Platform/ticketops.interop.js, attached by the JavaScript extender)
    ///                 focuses the search box; the server is TOLD through the [WebMethod] GlobalSearchBox.ReportShortcut.
    ///   2. Copy link — the server builds and signs the URL (ITicketLinkService), BrowserApi asks the browser to
    ///                 copy it (Application.EvalAsync) and only after the browser CONFIRMS does the server audit it.
    ///   3. Search text / shortcut names / the copy answer — every value that crosses back is validated on the server.
    /// Handlers stay thin; "async void" handlers own their try/catch and the user never sees an exception.
    /// </summary>
    public partial class WorkOrdersView : Form
    {
        private readonly IWorkOrderService _workOrders;
        private readonly ITicketLinkService _links;
        private readonly IAuditLogService _audit;
        private readonly BrowserApi _browser;
        private readonly SessionContext _session;
        private readonly InMemoryWorkOrderRepository _repository;   // only for the lab's outage switch
        private readonly ILog _log;

        private IReadOnlyList<WorkOrder> _rows = new List<WorkOrder>();
        private string _lastQuery = "";
        private IReadOnlyList<WorkOrder> _batchRows;
        private int _batchIndex;
        private int _batchRefused;

        // The Designer keeps the parameterless constructor; real wiring goes through the other one.
        public WorkOrdersView() : this(null, null, null, null, null, null, new ActivityLog())
        {
        }

        public WorkOrdersView(IWorkOrderService workOrders, ITicketLinkService links, IAuditLogService audit,
            BrowserApi browser, SessionContext session, InMemoryWorkOrderRepository repository, ILog log)
        {
            InitializeComponent();

            _workOrders = workOrders;
            _links = links;
            _audit = audit;
            _browser = browser;
            _session = session;
            _repository = repository;
            _log = log;

            if (log is ActivityLog activityLog)
                this.tracePanel.Attach(activityLog);

            this.tracePanel.Title = "Activity trace · UI → Service → Data · [CLIENT] = the browser";
            this.searchBox.Log = log;                 // lets the [WebMethod] callback leave its [CLIENT] line
        }

        #region Screen lifecycle

        private async void WorkOrdersView_Load(object sender, EventArgs e)
        {
            _log.Info(LogLayer.UI, "WorkOrdersView.Load",
                "screen shown — the JavaScript extender runs ticketOps.attachSearchShortcuts(this) when the search widget is created (after the widget exists, not at page load)");
            await RefreshGridAsync("");
            RefreshAudit();
        }

        /// <summary>data → UI: the only place that fills the grid.</summary>
        private async Task RefreshGridAsync(string query)
        {
            try
            {
                this.statusBanner.SetStatus("loading", StatusKind.Busy);
                _rows = await _workOrders.SearchAsync(query);

                this.gridWorkOrders.Rows.Clear();
                foreach (var w in _rows)
                    this.gridWorkOrders.Rows.Add(w.Id, w.Title, w.Priority.ToString(), w.AssignedTo, w.Confidential ? "confidential" : "");

                string user = _session != null ? _session.User.ToString() : "designer";
                string shown = query.Length > 40 ? query.Substring(0, 40) + "…" : query;
                this.labelCount.Text = query.Length == 0
                    ? $"{_rows.Count} work orders · signed in as {user}"
                    : $"{_rows.Count} match{(_rows.Count == 1 ? "" : "es")} for \"{shown}\" · {user}";   // Label escapes text (AllowHtml = false)

                this.statusBanner.SetStatus("ready", StatusKind.Success);
                _log.Info(LogLayer.UI, "WorkOrdersView.RefreshGrid", $"{_rows.Count} rows shown");
            }
            catch (Exception ex)
            {
                ReportFailure("WorkOrdersView.RefreshGrid", ex);
            }
        }

        /// <summary>data → UI: mirrors the server-side audit log in the left card.</summary>
        private void RefreshAudit()
        {
            if (_audit == null)
                return;

            this.listAudit.Items.Clear();
            foreach (var entry in _audit.Entries)
                this.listAudit.Items.Add(entry.ToString());
            if (this.listAudit.Items.Count > 0)
                this.listAudit.SelectedIndex = this.listAudit.Items.Count - 1;
        }

        #endregion

        #region UI → data (read the screen) and data → UI (show the result)

        private int? SelectedWorkOrderId()
        {
            var row = this.gridWorkOrders.CurrentRow;
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
                _log.Info(LogLayer.UI, "WorkOrdersView.ShowResult", $"OK · {result.Message}");
            }
            else
            {
                // Expected outcome: the service explained it in words the user may read.
                this.statusBanner.ShowBanner(result.Message, StatusKind.Warning);
                this.statusBanner.SetStatus("refused", StatusKind.Warning);
                _log.Warn(LogLayer.UI, "WorkOrdersView.ShowResult", $"FAIL · {result.Message}");
            }
        }

        /// <summary>
        /// Unexpected failure: details go to the log (with the exception type and message),
        /// the user sees one generic sentence. Nothing internal leaks through the banner.
        /// </summary>
        private void ReportFailure(string source, Exception ex)
        {
            _log.Error(LogLayer.UI, source, ex, $"caught {ex.GetType().Name} — user sees the safe message");
            this.statusBanner.ShowBanner("✖ " + Strings.ActionFailed, StatusKind.Error);
            this.statusBanner.SetStatus("failed", StatusKind.Error);
            AlertBox.Show(Strings.ActionFailed, MessageBoxIcon.Error,
                alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        #endregion

        #region Interop point 1 · Ctrl+K — the browser focuses, the server is told

        /// <summary>
        /// Raised by GlobalSearchBox.ReportShortcut, the [WebMethod] the client script calls after it moved the
        /// focus. The [CLIENT] line is already in the trace; this is the [UI] half. Nothing to authorize,
        /// nothing to persist: a focus change has no business meaning, so the server only reflects it.
        /// </summary>
        private void searchBox_ShortcutPressed(object sender, ShortcutEventArgs e)
        {
            _log.Info(LogLayer.UI, "WorkOrdersView.searchBox_ShortcutPressed",
                $"{e.Shortcut} acknowledged — focus already moved in the browser; no service call, nothing persisted");
            this.statusBanner.HideBanner();
            this.statusBanner.SetStatus("search focused (Ctrl+K)", StatusKind.Success);
        }

        /// <summary>The search text is client input: the handler hands it to the service, which bounds it.</summary>
        private async void searchBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string query = this.searchBox.Text ?? "";
                if (query == _lastQuery)
                    return;
                _lastQuery = query;

                _log.Info(LogLayer.UI, "WorkOrdersView.searchBox_TextChanged", $"→ IWorkOrderService.SearchAsync({query.Length} chars)");
                await RefreshGridAsync(query);
            }
            catch (Exception ex)
            {
                ReportFailure("WorkOrdersView.searchBox_TextChanged", ex);
            }
        }

        private void gridWorkOrders_SelectionChanged(object sender, EventArgs e)
        {
            int? id = SelectedWorkOrderId();
            if (id == null)
            {
                this.labelSelected.Text = "Select a work order";
                return;
            }

            var w = _rows[this.gridWorkOrders.CurrentRow.Index];
            this.labelSelected.Text = $"Work order #{w.Id} · {w.Title} · {w.AssignedTo}{(w.Confidential ? " · confidential" : "")}";
        }

        #endregion

        #region Interop point 2 · Copy link — the server builds, the browser copies, the server confirms

        private async void buttonCopyLink_Click(object sender, EventArgs e)
        {
            try
            {
                int? id = SelectedWorkOrderId();
                if (id == null)
                {
                    ShowResult(OperationResult<TicketLink>.Fail(Strings.SelectWorkOrder));
                    return;
                }

                await CopyLinkServerConfirmedAsync(id.Value, "WorkOrdersView.buttonCopyLink_Click");
            }
            catch (Exception ex)
            {
                ReportFailure("WorkOrdersView.buttonCopyLink_Click", ex);
            }
        }

        /// <summary>
        /// The server-confirmed clipboard round trip, in order:
        ///   [UI]     → ITicketLinkService.BuildLinkAsync(id)            re-check the id, apply the rule, sign the URL
        ///   [CLIENT] → navigator.clipboard.writeText via BrowserApi     awaited: the browser answers ok / refused
        ///   [SVC]    → ITicketLinkService.ConfirmCopiedAsync            the audit entry — only after confirmation
        ///   [UI]     "Copied" / the fallback sentence
        /// The same method serves the button and the bottom-bar paths, so every path goes through the same checks.
        /// </summary>
        private async Task CopyLinkServerConfirmedAsync(int workOrderId, string source)
        {
            _log.Info(LogLayer.UI, source, $"→ ITicketLinkService.BuildLinkAsync(#{workOrderId}) — the server builds the link; the browser only copies it");
            var built = await _links.BuildLinkAsync(workOrderId, _session.User);
            if (!built.Succeeded)
            {
                ShowResult(built);                                                 // unknown id / domain rule: nothing crossed to the browser
                return;
            }

            this.textLink.Text = built.Value.Url;                                  // visible fallback if the browser says no
            var outcome = await _browser.CopyToClipboardAsync(built.Value.Url);    // [CLIENT] lines: asked, then answered
            if (!outcome.Copied)
            {
                // "It can say no" is a normal path: no audit entry, one plain sentence, the link stays selectable.
                _log.Warn(LogLayer.UI, "WorkOrdersView.CopyLink", $"not confirmed by the browser ({outcome.Reason}) → no audit entry; user sees Strings.CopyFailed");
                this.statusBanner.ShowBanner(Strings.CopyFailed, StatusKind.Warning);
                this.statusBanner.SetStatus("not copied", StatusKind.Warning);
                this.textLink.Focus();
                this.textLink.SelectAll();
                return;
            }

            await _links.ConfirmCopiedAsync(workOrderId, _session.User);          // server-confirmed: the record is written now
            RefreshAudit();
            ShowResult(OperationResult<TicketLink>.Ok(built.Value, $"Link for #{workOrderId} copied."));
            AlertBox.Show(Strings.LinkCopied, MessageBoxIcon.Information,
                alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 3000);
        }

        #endregion

        #region Bottom bar: progress, failures, client-side denial, outage and recovery

        /// <summary>
        /// Progress path: a Timer builds and verifies one link per tick through the same service.
        /// Deliberately no clipboard call: browsers allow clipboard writes only right after a user gesture,
        /// and a Timer tick is not one — so a batch never touches the browser API.
        /// </summary>
        private void buttonBatch_Click(object sender, EventArgs e)
        {
            if (this.timerBatch.Enabled || _rows.Count == 0)
                return;

            _batchRows = _rows;
            _batchIndex = 0;
            _batchRefused = 0;
            this.progressBatch.Maximum = _batchRows.Count;
            this.progressBatch.Value = 0;
            this.progressBatch.Visible = true;
            this.statusBanner.HideBanner();
            this.statusBanner.SetStatus($"verifying 0/{_batchRows.Count}", StatusKind.Busy);
            _log.Info(LogLayer.UI, "WorkOrdersView.buttonBatch_Click",
                $"▶ build & verify {_batchRows.Count} links server-side, one per tick — no clipboard call: the browser allows clipboard writes only right after a user gesture, and a Timer tick is not one");
            this.timerBatch.Start();
        }

        private async void timerBatch_Tick(object sender, EventArgs e)
        {
            try
            {
                var order = _batchRows[_batchIndex++];
                var built = await _links.BuildLinkAsync(order.Id, _session.User);
                if (built.Succeeded)
                    this.textLink.Text = built.Value.Url;
                else
                    _batchRefused++;

                this.progressBatch.Value = _batchIndex;
                this.statusBanner.SetStatus($"verifying {_batchIndex}/{_batchRows.Count}", StatusKind.Busy);

                if (_batchIndex >= _batchRows.Count)
                {
                    this.timerBatch.Stop();
                    this.progressBatch.Visible = false;
                    string summary = $"{_batchRows.Count} links verified — {_batchRows.Count - _batchRefused} shareable, {_batchRefused} refused by the domain rule";
                    _log.Info(LogLayer.UI, "WorkOrdersView.timerBatch_Tick", summary + " → nothing copied, nothing audited");
                    this.statusBanner.SetStatus(summary, StatusKind.Success);
                }
            }
            catch (Exception ex)
            {
                this.timerBatch.Stop();
                this.progressBatch.Visible = false;
                ReportFailure("WorkOrdersView.timerBatch_Tick", ex);
            }
        }

        /// <summary>Failure path 1: a tampered client id. The handler is the same; the service re-checks and refuses.</summary>
        private async void buttonCopyUnknown_Click(object sender, EventArgs e)
        {
            try
            {
                _log.Info(LogLayer.UI, "WorkOrdersView.buttonCopyUnknown_Click", "a forged client id (#9999) reaches the same handler — the service re-loads it and says no");
                await CopyLinkServerConfirmedAsync(9999, "WorkOrdersView.buttonCopyUnknown_Click");
            }
            catch (Exception ex)
            {
                ReportFailure("WorkOrdersView.buttonCopyUnknown_Click", ex);
            }
        }

        /// <summary>Failure path 2: a domain rule. #2006 is confidential and L. Romero is not a manager, so WorkOrder.CanShareLink says no.</summary>
        private async void buttonCopyConfidential_Click(object sender, EventArgs e)
        {
            try
            {
                const int confidentialWorkOrder = 2006;
                _log.Info(LogLayer.UI, "WorkOrdersView.buttonCopyConfidential_Click", $"→ copy #{confidentialWorkOrder} — the rule lives in the domain, not in the script");
                await CopyLinkServerConfirmedAsync(confidentialWorkOrder, "WorkOrdersView.buttonCopyConfidential_Click");
            }
            catch (Exception ex)
            {
                ReportFailure("WorkOrdersView.buttonCopyConfidential_Click", ex);
            }
        }

        /// <summary>
        /// Client-side error path + recovery: toggle the simulated clipboard denial and copy right away.
        /// ON → the script rejects with NotAllowedError, no audit entry, the fallback sentence and the selectable link.
        /// OFF → the same copy succeeds again.
        /// </summary>
        private async void buttonClipboardDenied_Click(object sender, EventArgs e)
        {
            if (_browser == null)
                return;

            try
            {
                _browser.SimulateClipboardDenied = !_browser.SimulateClipboardDenied;
                this.buttonClipboardDenied.Text = _browser.SimulateClipboardDenied ? "Clipboard denied: ON — restore" : "Simulate clipboard denied";

                int id = SelectedWorkOrderId() ?? 2002;
                _log.Info(LogLayer.UI, "WorkOrdersView.buttonClipboardDenied_Click", _browser.SimulateClipboardDenied
                    ? $"clipboard denied ON → copy #{id} (expect ⚠ in CLIENT, no audit entry, the manual fallback)"
                    : $"clipboard denied OFF → copy #{id} again (recovery)");
                await CopyLinkServerConfirmedAsync(id, "WorkOrdersView.buttonClipboardDenied_Click");
            }
            catch (Exception ex)
            {
                ReportFailure("WorkOrdersView.buttonClipboardDenied_Click", ex);
            }
        }

        /// <summary>Error path + recovery: toggle the repository outage, then reload through the service.</summary>
        private async void buttonOutage_Click(object sender, EventArgs e)
        {
            if (_repository == null)
                return;

            _repository.SimulateOutage = !_repository.SimulateOutage;
            this.buttonOutage.Text = _repository.SimulateOutage ? "Recover the data store" : "Simulate data outage";
            _log.Info(LogLayer.UI, "WorkOrdersView.buttonOutage_Click",
                _repository.SimulateOutage ? "outage ON → refresh (expect ✖ in DATA, safe message in UI; Copy link fails the same way)" : "outage OFF → refresh (recovery)");
            await RefreshGridAsync(_lastQuery);
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
