using System;
using System.Globalization;
using TicketOps.Domain;
using TicketOps.Infrastructure;
using TicketOps.Resources;
using Wisej.Web;

namespace TicketOps.Dialogs
{
    /// <summary>
    /// The Approve/Reject checkpoint of the TicketOps Console — a modal Form that owns one decision.
    ///
    /// It does exactly three things: shows the work order it was given (read-only — it never mutates it),
    /// collects the decision and the comments, and publishes them as one typed <see cref="Result"/> when
    /// the user presses Confirm. The controls stay private; the caller reads <see cref="Result"/>, checks
    /// <see cref="ApprovalDialogResult.Confirmed"/> and hands the result to the service. Cancel and the ✕
    /// button leave <see cref="Result"/> unconfirmed, so nothing downstream can mistake them for a decision.
    ///
    /// Validation lives here, in the Confirm handler: a rejection without comments keeps the dialog open
    /// (DialogResult stays None) and tells the operator why. A confirmed Result is, by contract, already valid.
    /// </summary>
    public partial class ApprovalDialog : Form
    {
        private readonly WorkOrder _workOrder;
        private readonly ILog _log;
        private bool _closedByButton;

        /// <summary>The one member the caller reads. Unconfirmed until Confirm passes validation.</summary>
        public ApprovalDialogResult Result { get; private set; } = ApprovalDialogResult.NotConfirmed();

        // The Designer keeps the parameterless constructor; real wiring goes through the other one.
        public ApprovalDialog() : this(null, new ActivityLog())
        {
        }

        public ApprovalDialog(WorkOrder workOrder, ILog log)
        {
            InitializeComponent();

            _workOrder = workOrder;
            _log = log;

            if (workOrder != null)
            {
                // The dialog fills its own controls from the work order; it reads, never writes.
                this.Text = $"Approve Work Order {workOrder.Id}";
                this.labelWorkOrder.Text = $"{workOrder.Number} · {workOrder.Title} · {workOrder.Amount.ToString("C2", CultureInfo.GetCultureInfo("en-US"))} · requested by {workOrder.Requester}";
            }

            this.radioApprove.Checked = true;
            UpdateRequiredHint();
        }

        #region Decision → hint

        private void radioAction_CheckedChanged(object sender, EventArgs e)
        {
            UpdateRequiredHint();
            if (this.labelError.Visible && !this.radioReject.Checked)
                this.labelError.Visible = false;
        }

        private void UpdateRequiredHint()
        {
            this.labelRequired.Visible = this.radioReject.Checked;
        }

        #endregion

        #region Confirm / Cancel / ✕ — the explicit exits

        /// <summary>Validate inside the dialog; build the typed result only when the user confirms.</summary>
        private void buttonConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                var action = this.radioReject.Checked ? ApprovalAction.Reject : ApprovalAction.Approve;
                string comments = this.textComments.Text.Trim();

                if (action == ApprovalAction.Reject && comments.Length == 0)
                {
                    // Block the close and tell the operator why. Nothing has been produced yet.
                    this.DialogResult = DialogResult.None;
                    this.labelError.Text = Strings.CommentsRequiredToReject;
                    this.labelError.Visible = true;
                    this.textComments.Focus();
                    _log.Warn(LogLayer.UI, "ApprovalDialog.buttonConfirm_Click", "validation failed: comments are required when rejecting — dialog stays open, nothing sent to the service");
                    return;
                }

                Result = ApprovalDialogResult.Confirm(action, comments);
                _closedByButton = true;
                _log.Info(LogLayer.UI, "ApprovalDialog.buttonConfirm_Click", $"Confirm → Result {Result} · DialogResult.OK");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                _log.Error(LogLayer.UI, "ApprovalDialog.buttonConfirm_Click", ex);
                AlertBox.Show(Strings.ActionFailed, MessageBoxIcon.Error,
                    alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
            }
        }

        /// <summary>Cancel: Result stays unconfirmed, the caller does nothing.</summary>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Result = ApprovalDialogResult.NotConfirmed();
            _closedByButton = true;
            _log.Info(LogLayer.UI, "ApprovalDialog.buttonCancel_Click", "Cancel → Result {confirmed:false} · DialogResult.Cancel");
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        /// <summary>The ✕ button never touched a handler above: Result is still the unconfirmed default.</summary>
        private void ApprovalDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!_closedByButton)
                _log.Info(LogLayer.UI, "ApprovalDialog.FormClosed", $"closed with ✕ ({e.CloseReason}) → Result {Result} — same as Cancel for the caller");
        }

        #endregion
    }
}
