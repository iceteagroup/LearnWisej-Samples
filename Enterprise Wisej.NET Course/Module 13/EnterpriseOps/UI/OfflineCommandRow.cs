using System;
using System.Drawing;
using EnterpriseOps.Hybrid;
using Wisej.Web;

namespace EnterpriseOps.UI
{
    /// <summary>
    /// One card in the completion queue: the work order code, what the technician did, and the command's
    /// <see cref="SyncState"/> as a coloured pill. It is a view over an <see cref="OfflineCommand"/> and
    /// holds no state of its own — the queue is the truth, the row only renders it.
    /// </summary>
    public partial class OfflineCommandRow : UserControl
    {
        public OfflineCommandRow()
        {
            InitializeComponent();
        }

        /// <summary>The command this row is showing (the page reads it back when a row is clicked).</summary>
        public OfflineCommand Command { get; private set; }

        public void Bind(OfflineCommand command)
        {
            this.Command = command;

            this.lblRowCode.Text = command.EntityId;
            this.lblRowSummary.Text = string.IsNullOrEmpty(command.SyncMessage)
                ? command.Summary
                : command.Summary + " · " + command.SyncMessage;
            this.lblRowSummary.ToolTipText = this.lblRowSummary.Text;

            this.lblRowState.Text = command.State.ToString();
            this.lblRowState.BackColor = BackFor(command.State);
            this.lblRowState.ForeColor = ForeFor(command.State);
        }

        internal static Color BackFor(SyncState state)
        {
            switch (state)
            {
                case SyncState.PendingSync: return Color.FromArgb(255, 248, 236);
                case SyncState.Synced: return Color.FromArgb(240, 249, 243);
                case SyncState.Conflict: return Color.FromArgb(253, 236, 236);
                default: return Color.FromArgb(244, 247, 250);
            }
        }

        internal static Color ForeFor(SyncState state)
        {
            switch (state)
            {
                case SyncState.PendingSync: return Color.FromArgb(185, 119, 14);
                case SyncState.Synced: return Color.FromArgb(31, 138, 76);
                case SyncState.Conflict: return Color.FromArgb(192, 57, 43);
                default: return Color.FromArgb(107, 120, 134);
            }
        }
    }
}
