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
    /// TicketOps Console · Work Orders: global search (Ctrl+K from anywhere), the work-order grid, "Copy link",
    /// the last link the server built (also the manual fallback), and the server-side audit log.
    ///
    /// Two interop points, one rule: the browser requests or reports; the server validates, decides and records.
    ///   1. Ctrl+K   — JavaScript (embedded Platform/ticketops.interop.js, attached by the JavaScript extender)
    ///                 focuses the search box; the server is TOLD through the [WebMethod] GlobalSearchBox.ReportShortcut.
    ///   2. Copy link — the server builds and signs the URL (ITicketLinkService), BrowserApi asks the browser to
    ///                 copy it (Application.EvalAsync) and only after the browser CONFIRMS does the server audit it.
    /// </summary>
    public partial class WorkOrdersView : Form
    {
        private readonly IWorkOrderService _workOrders;
        private readonly ITicketLinkService _links;
        private readonly IAuditLogService _audit;
        private readonly BrowserApi _browser;
        private readonly SessionContext _session;
        private readonly ILog _log;

        private IReadOnlyList<WorkOrder> _rows = new List<WorkOrder>();
        private string _lastQuery = "";

        // The Designer keeps the parameterless constructor; real wiring goes through the other one.
        public WorkOrdersView() : this(null, null, null, null, null, new ActivityLog())
        {
        }

        public WorkOrdersView(IWorkOrderService workOrders, ITicketLinkService links, IAuditLogService audit,
            BrowserApi browser, SessionContext session, ILog log)
        {
            InitializeComponent();

            _workOrders = workOrders;
            _links = links;
            _audit = audit;
            _browser = browser;
            _session = session;
            _log = log;
        }

        private async void WorkOrdersView_Load(object sender, EventArgs e)
        {
            await RefreshGridAsync("");
            RefreshAudit();
            this.statusBanner.SetStatus("Ready.", StatusKind.Success);
        }

        /// <summary>data → UI: the only place that fills the grid.</summary>
        private async Task RefreshGridAsync(string query)
        {
            try
            {
                _rows = await _workOrders.SearchAsync(query);

                this.gridWorkOrders.Rows.Clear();
                foreach (var w in _rows)
                    this.gridWorkOrders.Rows.Add(w.Id, w.Title, w.Priority.ToString(), w.AssignedTo, w.Confidential ? "confidential" : "");

                string user = _session != null ? _session.User.ToString() : "designer";
                string shown = query.Length > 40 ? query.Substring(0, 40) + "…" : query;
                this.labelCount.Text = query.Length == 0
                    ? $"{_rows.Count} work orders · signed in as {user}"
                    : $"{_rows.Count} match{(_rows.Count == 1 ? "" : "es")} for \"{shown}\" · {user}";   // Label escapes text (AllowHtml = false)
            }
            catch (Exception ex)
            {
                ReportFailure("WorkOrdersView.RefreshGrid", ex);
            }
        }

        /// <summary>data → UI: mirrors the server-side audit log.</summary>
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
            }
            else
            {
                this.statusBanner.ShowBanner(result.Message, StatusKind.Warning);
                this.statusBanner.SetStatus("refused", StatusKind.Warning);
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

        #region Interop point 1 · Ctrl+K — the browser focuses, the server is told

        /// <summary>
        /// Raised by GlobalSearchBox.ReportShortcut, the [WebMethod] the client script calls after it moved the
        /// focus. Nothing to authorize, nothing to persist: a focus change has no business meaning.
        /// </summary>
        private void searchBox_ShortcutPressed(object sender, ShortcutEventArgs e)
        {
            this.statusBanner.HideBanner();
            this.statusBanner.SetStatus("Search focused.", StatusKind.Success);
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
            this.statusBanner.SetStatus($"Work order {w.Id} selected.", StatusKind.Normal);
        }

        #endregion

        #region Interop point 2 · Copy link — the server builds, the browser copies, the server confirms

        /// <summary>
        /// The server-confirmed clipboard round trip:
        ///   ITicketLinkService.BuildLinkAsync(id)   re-check the id, apply the rule, sign the URL
        ///   BrowserApi.CopyToClipboardAsync(url)    awaited: the browser answers ok / refused
        ///   ITicketLinkService.ConfirmCopiedAsync   the audit entry — only after confirmation
        /// </summary>
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

                var built = await _links.BuildLinkAsync(id.Value, _session.User);
                if (!built.Succeeded)
                {
                    ShowResult(built);                                                 // unknown id / domain rule: nothing crossed to the browser
                    return;
                }

                this.textLink.Text = built.Value.Url;                                  // visible fallback if the browser says no
                var outcome = await _browser.CopyToClipboardAsync(built.Value.Url);
                if (!outcome.Copied)
                {
                    // "It can say no" is a normal path: no audit entry, one plain sentence, the link stays selectable.
                    this.statusBanner.ShowBanner(Strings.CopyFailed, StatusKind.Warning);
                    this.statusBanner.SetStatus("not copied", StatusKind.Warning);
                    this.textLink.Focus();
                    this.textLink.SelectAll();
                    return;
                }

                await _links.ConfirmCopiedAsync(id.Value, _session.User);             // server-confirmed: the record is written now
                RefreshAudit();
                ShowResult(OperationResult<TicketLink>.Ok(built.Value, "Link copied — built and audited on the server."));
                AlertBox.Show(Strings.LinkCopied, MessageBoxIcon.Information,
                    alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 3000);
            }
            catch (Exception ex)
            {
                ReportFailure("WorkOrdersView.buttonCopyLink_Click", ex);
            }
            finally
            {
                // EvalAsync resolves when the browser answers, not in the click request: push the final UI state.
                Application.Update(this);
            }
        }

        #endregion
    }
}
