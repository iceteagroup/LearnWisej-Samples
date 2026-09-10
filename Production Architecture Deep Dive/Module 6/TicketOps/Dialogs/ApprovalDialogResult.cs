using TicketOps.Domain;

namespace TicketOps.Dialogs
{
    /// <summary>
    /// The typed contract the Approve/Reject dialog publishes when it closes. The caller reads this
    /// object — never the dialog's radio buttons or text box — and makes exactly one check:
    /// <see cref="Confirmed"/>. It is false on Cancel and on the ✕ button, so "cancelled" and "never
    /// answered" look the same to the caller: do nothing.
    ///
    /// Plain data, no Wisej.NET type: a unit test can build one and hand it to the service.
    /// A bare <c>DialogResult.OK</c> could not carry the action or the comments; this can.
    /// </summary>
    public sealed class ApprovalDialogResult
    {
        /// <summary>True only when the user pressed Confirm and the dialog's own validation passed.</summary>
        public bool Confirmed { get; private set; }

        public ApprovalAction Action { get; private set; }

        /// <summary>Trimmed reviewer comments. Required when <see cref="Action"/> is Reject.</summary>
        public string Comments { get; private set; } = string.Empty;

        private ApprovalDialogResult()
        {
        }

        /// <summary>The default: Cancel, ✕, or a dialog that never reached Confirm.</summary>
        public static ApprovalDialogResult NotConfirmed() => new ApprovalDialogResult { Confirmed = false };

        /// <summary>Built by the dialog's Confirm handler only after its validation passed.</summary>
        public static ApprovalDialogResult Confirm(ApprovalAction action, string comments)
            => new ApprovalDialogResult { Confirmed = true, Action = action, Comments = (comments ?? string.Empty).Trim() };

        public override string ToString()
            => Confirmed ? $"{{confirmed:true, action:{Action}, comments:\"{Comments}\"}}" : "{confirmed:false}";
    }
}
