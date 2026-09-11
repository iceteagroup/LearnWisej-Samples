using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using TicketOps.Controls;
using TicketOps.Dialogs;
using TicketOps.Domain;
using TicketOps.Infrastructure;
using TicketOps.Resources;
using TicketOps.Services;
using Wisej.Web;

namespace TicketOps.Views
{
    /// <summary>
    /// TicketOps Console · Work Orders: the work-order queue and the selected order (display only), plus
    /// "Approve / Reject…", which opens the modal <see cref="ApprovalDialog"/> and awaits it.
    ///
    /// <see cref="buttonReview_Click"/>: open the dialog, await it, and if — and only if — the user confirmed,
    /// hand the typed result to IApprovalService. It never reads a control of the dialog and never mutates
    /// the work order itself.
    /// </summary>
    public partial class WorkOrderQueue : Form
    {
        private static readonly CultureInfo Money = CultureInfo.GetCultureInfo("en-US");

        private readonly IApprovalService _approvals;
        private readonly ILog _log;

        private IReadOnlyList<WorkOrder> _rows = new List<WorkOrder>();

        // The Designer keeps the parameterless constructor; real wiring goes through the other one.
        public WorkOrderQueue() : this(null, new ActivityLog())
        {
        }

        public WorkOrderQueue(IApprovalService approvals, ILog log)
        {
            InitializeComponent();

            _approvals = approvals;
            _log = log;
        }

        private async void WorkOrderQueue_Load(object sender, EventArgs e)
        {
            await RefreshGridAsync(2002);
        }

        /// <summary>data → UI: the only place that fills the grid. Re-selects <paramref name="selectId"/> when given.</summary>
        private async Task RefreshGridAsync(int? selectId)
        {
            try
            {
                this.statusBanner.SetStatus("loading", StatusKind.Busy);
                _rows = await _approvals.GetQueueAsync();

                this.gridWorkOrders.Rows.Clear();
                int pending = 0;
                int selectIndex = -1;
                for (int i = 0; i < _rows.Count; i++)
                {
                    var w = _rows[i];
                    if (w.IsPending) pending++;
                    if (selectId.HasValue && w.Id == selectId.Value) selectIndex = i;
                    this.gridWorkOrders.Rows.Add(w.Number, w.Title, w.Requester, w.Amount.ToString("C2", Money), w.Status.ToString());
                }

                this.labelCount.Text = $"{pending} pending of {_rows.Count} work orders";
                if (selectIndex >= 0)
                    this.gridWorkOrders.CurrentCell = this.gridWorkOrders.Rows[selectIndex].Cells[0];
                ShowSelected();

                this.statusBanner.SetStatus("ready", StatusKind.Success);
            }
            catch (Exception ex)
            {
                ReportFailure("WorkOrderQueue.RefreshGrid", ex);
            }
        }

        #region Selection (display only) and result → UI

        private WorkOrder SelectedWorkOrder()
        {
            var row = this.gridWorkOrders.CurrentRow;
            if (row == null || row.Index < 0 || row.Index >= _rows.Count)
                return null;
            return _rows[row.Index];
        }

        private void gridWorkOrders_SelectionChanged(object sender, EventArgs e)
        {
            ShowSelected();
            var wo = SelectedWorkOrder();
            if (wo != null)
                SetWorkflowStatus($"Work order {wo.Id} selected.");
        }

        private void ShowSelected()
        {
            var wo = SelectedWorkOrder();
            if (wo == null)
            {
                this.labelSelected.Text = "No work order selected";
                this.labelSelectedDetail.Text = "";
                this.labelDecision.Text = "";
                this.buttonReview.Enabled = false;
                return;
            }

            this.labelSelected.Text = $"{wo.Number} · {wo.Title}";
            this.labelSelectedDetail.Text = $"Requester {wo.Requester} · Cost {wo.Amount.ToString("C2", Money)} · Requested {wo.RequestedAt:MMM d} · Approver you ({AppComposition.CurrentUser})";
            this.labelDecision.Text = wo.IsPending
                ? "Status: Pending approval"
                : $"Status: {wo.Status} by {wo.DecidedBy} on {wo.DecidedAtUtc:yyyy-MM-dd HH:mm} UTC · \"{wo.DecisionComments}\"";
            this.labelDecision.ForeColor = wo.IsPending
                ? System.Drawing.Color.FromArgb(185, 119, 14)
                : StatusBanner.ColorFor(wo.Status == WorkOrderStatus.Approved ? StatusKind.Success : StatusKind.Error);
            this.buttonReview.Enabled = wo.IsPending;
        }

        private void SetWorkflowStatus(string text)
        {
            this.labelWorkflow.Text = text;
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

        #endregion

        #region The modal workflow: open the dialog, await it, apply only on a confirmed result

        /// <summary>
        /// Open the dialog, wait, and if the user confirmed hand the confirmed intent to the service.
        /// No control of the dialog is read here — only DialogResult and Result.
        /// </summary>
        private async void buttonReview_Click(object sender, EventArgs e)
        {
            try
            {
                var workOrder = SelectedWorkOrder();
                if (workOrder == null)
                {
                    ShowResult(OperationResult<WorkOrder>.Fail(Strings.SelectWorkOrder));
                    return;
                }

                SetWorkflowStatus(string.Format(Strings.DialogOpen, workOrder.Id));
                this.statusBanner.HideBanner();
                this.statusBanner.SetStatus("deciding", StatusKind.Busy);

                using (var dialog = new ApprovalDialog(workOrder, _log))
                {
                    DialogResult outcome = await dialog.ShowDialogAsync();      // modal; the handler waits here
                    ApprovalDialogResult result = dialog.Result;                // typed result — never dialog.textComments.Text

                    // The one gate: did the user actually commit?
                    if (outcome != DialogResult.OK || !result.Confirmed)
                    {
                        SetWorkflowStatus(string.Format(Strings.DialogCancelled, workOrder.Id));
                        this.statusBanner.SetStatus("unchanged", StatusKind.Normal);
                        return;
                    }

                    // Dialog gathered intent; the SERVICE executes the transaction.
                    var applied = await _approvals.ApplyAsync(workOrder.Id, result);
                    ShowResult(applied);
                    SetWorkflowStatus(applied.Succeeded
                        ? $"{applied.Message} Status, audit and notification committed together."
                        : $"{applied.Message} Work order {workOrder.Id} unchanged.");
                }

                await RefreshGridAsync(workOrder.Id);
            }
            catch (Exception ex)
            {
                // The repository threw during the commit: nothing was applied; the user reads the safe message.
                ReportFailure("WorkOrderQueue.buttonReview_Click", ex);
                SetWorkflowStatus("Commit failed — nothing was applied.");
                await RefreshGridAsync(SelectedWorkOrder()?.Id);
            }
        }

        private async void buttonRefresh_Click(object sender, EventArgs e)
        {
            await RefreshGridAsync(SelectedWorkOrder()?.Id);
        }

        #endregion
    }
}
