using Wisej.Web;

namespace AdaptiveOps.Dialogs
{
    /// <summary>
    /// The modal host of the ticket editor on the Phone profiles (layout in TicketEditorForm.Designer.cs).
    ///
    /// It owns no fields of its own: the page moves the single <see cref="Shell.TicketEditor"/> instance in here
    /// with <see cref="HostEditor"/> when the active profile is a phone, and back into the docked details region
    /// when it is not. The form is created once per session and reused — ShowDialog does not dispose a closed
    /// dialog — so rotating a phone ten times opens the same form, never ten of them.
    /// </summary>
    public partial class TicketEditorForm : Form
    {
        public TicketEditorForm()
        {
            InitializeComponent();
        }

        /// <summary>Reparents the editor into this form (idempotent: a second call is a no-op).</summary>
        public void HostEditor(Control editor)
        {
            if (editor.Parent == this)
                return;

            editor.Parent?.Controls.Remove(editor);
            this.Controls.Add(editor);
            editor.Dock = DockStyle.Fill;
        }
    }
}
