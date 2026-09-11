using System;
using SupportDesk.Services;
using Wisej.Web;

namespace SupportDesk.Web
{
    /// <summary>What the operator chose in <see cref="ConflictDialog"/>.</summary>
    public enum ConflictChoice
    {
        Cancel,
        Reload,
        Overwrite
    }

    /// <summary>
    /// The concurrency conflict dialog, opened by <c>TicketEditorForm.SaveAsync</c> when it catches
    /// <see cref="Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException"/>: the differing fields (your value,
    /// database value, original value), Reload, Overwrite (only when <see cref="ConflictResolution.CanOverwrite"/>
    /// allows the role) and Cancel.
    /// </summary>
    public partial class ConflictDialog : Form
    {
        /// <summary>What the operator picked. Read by the caller after <c>ShowDialogAsync</c> returns.</summary>
        public ConflictChoice Choice { get; private set; } = ConflictChoice.Cancel;

        public ConflictDialog(ConflictSet conflicts, UserRole role)
        {
            InitializeComponent();

            if (conflicts.DeletedByAnotherUser)
            {
                this.labelExplanation.Text = "Someone else deleted this ticket while you were editing it. There is nothing left to compare. Reload will close the editor.";
                this.conflictGridView.DataSource = Array.Empty<ConflictField>();
            }
            else
            {
                this.labelExplanation.Text = "Someone else changed this ticket while you were editing it. Compare the values and choose what to keep.";
                this.conflictGridView.DataSource = conflicts.Fields;
            }

            // Overwrite is a policy decision: only a Supervisor may discard another operator's change.
            this.btnOverwrite.Visible = !conflicts.DeletedByAnotherUser && ConflictResolution.CanOverwrite(role);
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            Choice = ConflictChoice.Reload;
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void btnOverwrite_Click(object sender, EventArgs e)
        {
            Choice = ConflictChoice.Overwrite;
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Choice = ConflictChoice.Cancel;
            this.DialogResult = DialogResult.OK;
            Close();
        }
    }
}
