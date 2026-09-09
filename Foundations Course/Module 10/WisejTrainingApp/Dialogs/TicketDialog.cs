using System;
using Wisej.Web;
using WisejTrainingApp.Models;
using WisejTrainingApp.Services;

namespace WisejTrainingApp.Dialogs
{
    /// <summary>
    /// One dialog for New and Edit (lab step 6: "reuse one TicketDialog for new and edit"). Pass no ticket to
    /// create, pass a ticket to edit. Save runs ValidateForm() — which asks TicketValidator, not its own rules —
    /// and closes with DialogResult.OK only when the ticket is valid; the caller then reads TicketResult.
    ///
    /// The dialog never touches TicketService: the screen that opened it decides whether the result is added
    /// or updated. That keeps the dialog reusable and the data flow visible in one place (TicketsView).
    /// </summary>
    public partial class TicketDialog : Form
    {
        private readonly TicketValidator validator = new TicketValidator();
        private readonly Ticket original;

        /// <summary>The validated ticket when the dialog closed with OK; null otherwise.</summary>
        public Ticket TicketResult { get; private set; }

        public bool IsEdit => original != null;

        public TicketDialog(Ticket ticket = null)
        {
            InitializeComponent();

            original = ticket;
            Text = IsEdit ? $"Edit ticket #{ticket.Id}" : "New ticket";
            lblDialogHint.Text = IsEdit
                ? "Change the fields and Save. The same TicketValidator rules apply as for a new ticket."
                : "Fill the ticket and Save. Title and Customer are required; a Closed ticket needs an assignee.";

            LoadTicket(ticket);
        }

        /// <summary>Controls ← ticket. A new ticket starts as Open / Medium.</summary>
        private void LoadTicket(Ticket ticket)
        {
            txtTitle.Text = ticket?.Title ?? "";
            txtCustomer.Text = ticket?.Customer ?? "";
            cboStatus.SelectedItem = ticket?.Status ?? "Open";
            cboPriority.SelectedItem = ticket?.Priority ?? "Medium";
            txtAssignedTo.Text = ticket?.AssignedTo ?? "";
            txtDescription.Text = ticket?.Description ?? "";
            lblValidation.Text = "";
        }

        /// <summary>Ticket ← controls. Id and CreatedDate are kept from the original when editing.</summary>
        private Ticket ReadTicket()
        {
            return new Ticket
            {
                Id = original?.Id ?? 0,
                CreatedDate = original?.CreatedDate ?? default,
                Title = txtTitle.Text.Trim(),
                Customer = txtCustomer.Text.Trim(),
                Status = cboStatus.SelectedItem as string,
                Priority = cboPriority.SelectedItem as string,
                AssignedTo = txtAssignedTo.Text.Trim(),
                Description = txtDescription.Text.Trim(),
            };
        }

        /// <summary>
        /// The validation step of the flow (s46 §3: "the dialog opens, validation runs, …"). Returns true when
        /// TicketValidator is happy; otherwise shows every message and keeps the dialog open.
        /// </summary>
        private bool ValidateForm()
        {
            Ticket candidate = ReadTicket();
            ValidationResult result = validator.Validate(candidate);

            if (!result.IsValid)
            {
                lblValidation.Text = result.Message;
                lblValidation.ForeColor = System.Drawing.Color.FromArgb(224, 86, 59);
                FocusFirstInvalid(result);
                return false;
            }

            TicketResult = candidate;
            lblValidation.Text = "";
            return true;
        }

        private void FocusFirstInvalid(ValidationResult result)
        {
            string first = result.Errors[0];
            if (first.StartsWith("Title", StringComparison.Ordinal)) txtTitle.Focus();
            else if (first.StartsWith("Customer", StringComparison.Ordinal)) txtCustomer.Focus();
            else if (first.StartsWith("A Closed", StringComparison.Ordinal)) txtAssignedTo.Focus();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateForm())
                return;         // failure path: the dialog stays open with the messages under the fields

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            TicketResult = null;
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
