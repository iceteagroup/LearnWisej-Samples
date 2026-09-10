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
    /// Module 7's concurrency conflict dialog: modal, <see cref="FormStartPosition.CenterParent"/>, opened by
    /// <c>TicketEditorForm.SaveAsync</c> when it catches <see cref="Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException"/>.
    /// A <see cref="DataGridView"/> lists every differing field (Field / Your value / Database value /
    /// Original value), a label explains what happened in plain language, and three buttons decide what
    /// happens next: <b>Reload</b> (discard the local edit, take the database's current values — always
    /// offered), <b>Overwrite</b> (save the operator's values anyway, visible only when
    /// <see cref="ConflictResolution.CanOverwrite"/> allows the given role — never a silent retry, see the
    /// Module 7 lesson's "Common mistake: silently overwriting another user's change"), and <b>Cancel</b>
    /// (leave the editor exactly as it is, nothing saved). This form has no trace list of its own — like
    /// <see cref="TicketEditorForm"/> it logs into the parent page's, through the same
    /// <c>Action&lt;char, string, string&gt;</c> callback.
    /// </summary>
    public partial class ConflictDialog : Form
    {
        private readonly Action<char, string, string> _trace;

        /// <summary>What the operator picked. Read by the caller after <c>ShowDialogAsync</c> returns — this form always closes with <see cref="DialogResult.OK"/>; the decision lives here, not in <c>DialogResult</c>.</summary>
        public ConflictChoice Choice { get; private set; } = ConflictChoice.Cancel;

        public ConflictDialog(ConflictSet conflicts, UserRole role, Action<char, string, string> trace)
        {
            _trace = trace ?? ((symbol, label, text) => { });

            InitializeComponent();

            if (conflicts.DeletedByAnotherUser)
            {
                this.labelExplanation.Text = "Someone else deleted this ticket while you were editing it. There is nothing left to compare — Reload will close the editor.";
                this.conflictGridView.DataSource = Array.Empty<ConflictField>();
            }
            else
            {
                this.labelExplanation.Text = "Someone else changed this ticket while you were editing it. Review the fields below, then choose what to do.";
                this.conflictGridView.DataSource = conflicts.Fields;
            }

            // Overwrite is a policy decision, never automatic — see ConflictResolution.CanOverwrite. An
            // ordinary Agent gets Reload and Cancel only; a Supervisor also gets Overwrite.
            var canOverwrite = !conflicts.DeletedByAnotherUser && ConflictResolution.CanOverwrite(role);
            this.btnOverwrite.Visible = canOverwrite;
            this.labelPolicy.Text = canOverwrite
                ? $"Role: {role} — Overwrite is available."
                : $"Role: {role} — Overwrite is hidden. Only a Supervisor may discard another operator's change.";

            _trace('•', "conflict", $"dialog opened · role {role} · {(conflicts.DeletedByAnotherUser ? "row deleted by another user" : $"{conflicts.Fields.Count} field(s) differ")} · Overwrite {(canOverwrite ? "offered" : "hidden by policy")}");
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            Choice = ConflictChoice.Reload;
            _trace('•', "dialog", "Conflict → Reload");
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void btnOverwrite_Click(object sender, EventArgs e)
        {
            Choice = ConflictChoice.Overwrite;
            _trace('•', "dialog", "Conflict → Overwrite (Supervisor)");
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Choice = ConflictChoice.Cancel;
            _trace('•', "dialog", "Conflict → Cancel — nothing resolved, the editor stays open with your edits");
            this.DialogResult = DialogResult.OK;
            Close();
        }
    }
}
