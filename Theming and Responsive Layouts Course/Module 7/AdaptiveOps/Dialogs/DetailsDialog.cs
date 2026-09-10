using System;
using AdaptiveOps.Models;
using AdaptiveOps.Shell;
using Wisej.Web;

namespace AdaptiveOps.Dialogs
{
    /// <summary>
    /// The phone presentation of the ticket editor: the same <see cref="DetailsEditor"/> control that is
    /// docked on the right at desktop width, hosted in a modal Form. Opening the editor as a dialog is
    /// behaviour a property value cannot express, so it is the one thing the profile handler does beyond
    /// assigning final values (see MainPage.ApplyProfile and docs/ArchitectureNote.md).
    /// </summary>
    public partial class DetailsDialog : Form
    {
        private readonly Func<Ticket, TicketValidationException> _save;

        /// <param name="save">Saves on the server; returns null on success or the validation error to show.</param>
        /// <param name="maximized">Phone profiles open the dialog maximized so it fills the viewport.</param>
        public DetailsDialog(Func<Ticket, TicketValidationException> save, bool maximized)
        {
            _save = save ?? throw new ArgumentNullException(nameof(save));
            InitializeComponent();

            this.editor.Compact = true;
            this.editor.SaveRequested += this.editor_SaveRequested;
            if (maximized)
                this.WindowState = FormWindowState.Maximized;
        }

        /// <summary>The hosted editor, so the page can fill it and read its validation label.</summary>
        public DetailsEditor Editor => this.editor;

        private void editor_SaveRequested(object sender, Ticket ticket)
        {
            var error = _save(ticket);
            if (error == null)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
                return;
            }

            // Failure stays inside the dialog: invalid editor state + announced message, nothing written.
            this.editor.ShowValidationError(error);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
