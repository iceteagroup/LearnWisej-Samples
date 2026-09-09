using System;
using System.Collections.Generic;
using Wisej.Web;
using WisejTrainingApp.Models;

namespace WisejTrainingApp.Dialogs
{
    /// <summary>
    /// The one dialog the module builds — used for both Create and Edit.
    ///
    /// It does exactly what the lesson says a dialog should do: collect the values for one ticket,
    /// validate them, and return a clear result. It does not save anything itself: the parent page
    /// reads <see cref="TicketResult"/> when <see cref="Form.ShowDialogAsync"/> returns
    /// <see cref="DialogResult.OK"/> and calls the service. Cancel returns <see cref="DialogResult.Cancel"/>
    /// and <see cref="TicketResult"/> stays null.
    /// </summary>
    public partial class TicketDialog : Form
    {
        /// <summary>The longest title the ticket store accepts — the one length rule ValidateForm() checks.</summary>
        public const int MaxTitleLength = 80;

        /// <summary>The ticket being edited, or null when the dialog was opened for a new ticket.</summary>
        private readonly Ticket editingTicket;

        /// <summary>
        /// Filled by btnSave_Click after ValidateForm() passed. Null until then, and null after Cancel.
        /// </summary>
        public Ticket TicketResult { get; private set; }

        /// <summary>
        /// Raised every time ValidateForm() runs, with the outcome and the message shown to the user.
        /// Only the parent's event log listens to it — it exists so the "Validate" step of the workflow is
        /// visible from outside the dialog. It plays no part in the Save/Cancel logic.
        /// </summary>
        public event Action<bool, string> ValidationChecked;

        /// <summary>
        /// One constructor for both jobs (lab step 4): no ticket → "Create Ticket" with blank fields and the
        /// defaults Open / Medium; a ticket → "Edit Ticket #n" with every field pre-filled.
        /// </summary>
        public TicketDialog(Ticket ticket = null)
        {
            InitializeComponent();

            editingTicket = ticket;

            if (ticket == null)
            {
                this.Text = "Create Ticket";
                cboStatus.SelectedItem = "Open";
                cboPriority.SelectedItem = "Medium";
            }
            else
            {
                this.Text = $"Edit Ticket #{ticket.Id}";
                txtTitle.Text = ticket.Title;
                txtCustomer.Text = ticket.Customer;
                txtAssignedTo.Text = ticket.AssignedTo;
                txtDescription.Text = ticket.Description;
                cboStatus.SelectedItem = ticket.Status;
                cboPriority.SelectedItem = string.IsNullOrEmpty(ticket.Priority) ? "Medium" : ticket.Priority;
            }
        }

        #region Save / Cancel — the pattern from lesson s22

        /// <summary>
        /// The readable Save handler: validate, build, return OK, close. Nothing is saved here;
        /// the parent page (TicketsWindow) owns the ticketService.AddTicket / UpdateTicket call, the
        /// lblStatus + AlertBox feedback and RefreshTicketGrid(), so the grid refreshes only after a real save.
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateForm())
                return;

            TicketResult = BuildTicketFromFields();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>Cancel closes without saving — TicketResult stays null, the parent refreshes nothing.</summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        #endregion

        #region Validation — simple UI rules, close to the fields (business rules stay in the service)

        /// <summary>
        /// Checks the field-level rules and tells the user exactly what to fix. Returns false and keeps the
        /// dialog open when anything is wrong; the message lists every problem, not just the first one.
        /// </summary>
        private bool ValidateForm()
        {
            var problems = new List<string>();
            Control firstBadField = null;

            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                problems.Add("Title is required.");
                firstBadField = firstBadField ?? txtTitle;
            }
            else if (txtTitle.Text.Trim().Length > MaxTitleLength)
            {
                problems.Add($"Title is too long ({txtTitle.Text.Trim().Length} characters, maximum {MaxTitleLength}).");
                firstBadField = firstBadField ?? txtTitle;
            }

            if (cboStatus.SelectedItem == null)
            {
                problems.Add("Status is required — choose Open, In Progress or Closed.");
                firstBadField = firstBadField ?? cboStatus;
            }

            if (string.IsNullOrWhiteSpace(txtCustomer.Text))
            {
                problems.Add("Customer is required.");
                firstBadField = firstBadField ?? txtCustomer;
            }

            if (problems.Count == 0)
            {
                lblValidation.Text = "";
                lblValidation.Visible = false;
                ValidationChecked?.Invoke(true, "all rules passed");
                return true;
            }

            // The message tells the user exactly what to fix (lesson s22, "A consistent Save / Cancel pattern").
            lblValidation.Text = "Please fix the following before saving:\n• " + string.Join("\n• ", problems);
            lblValidation.Visible = true;
            firstBadField?.Focus();
            ValidationChecked?.Invoke(false, string.Join(" ", problems));
            return false;
        }

        #endregion

        #region Build the ticket from the fields

        /// <summary>
        /// Turns the fields into a Ticket. When editing, the Id and CreatedDate of the original ticket are
        /// kept so UpdateTicket replaces the right row; a new ticket gets Id 0 (the service assigns the
        /// next one) and CreatedDate = now.
        /// </summary>
        private Ticket BuildTicketFromFields()
        {
            return new Ticket
            {
                Id = editingTicket?.Id ?? 0,
                CreatedDate = editingTicket?.CreatedDate ?? DateTime.Now,
                Title = txtTitle.Text.Trim(),
                Customer = txtCustomer.Text.Trim(),
                Status = cboStatus.SelectedItem as string,
                Priority = (cboPriority.SelectedItem as string) ?? "Medium",
                AssignedTo = txtAssignedTo.Text.Trim(),
                Description = txtDescription.Text.Trim(),
            };
        }

        #endregion
    }
}
