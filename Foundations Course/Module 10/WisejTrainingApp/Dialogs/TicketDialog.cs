using System;
using Wisej.Web;
using WisejTrainingApp.Models;
using WisejTrainingApp.Services;

namespace WisejTrainingApp.Dialogs
{
    public partial class TicketDialog : Form
    {
        // The same rules for create and edit — and TicketService checks them again before saving.
        private readonly TicketValidator validator = new TicketValidator();
        private readonly Ticket editingTicket;

        /// <summary>The ticket built from the fields when the user saved.</summary>
        public Ticket Ticket { get; private set; }

        // Blank for New, pre-filled for Edit.
        public TicketDialog(Ticket ticket = null)
        {
            InitializeComponent();

            editingTicket = ticket;
            cboStatus.SelectedItem = ticket?.Status ?? "Open";
            cboPriority.SelectedItem = ticket?.Priority ?? "Medium";

            if (ticket != null)
            {
                this.Text = "Edit Ticket";
                txtTitle.Text = ticket.Title;
                txtCustomer.Text = ticket.Customer;
                txtAssignedTo.Text = ticket.AssignedTo;
                txtDescription.Text = ticket.Description;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateForm())
                return;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private bool ValidateForm()
        {
            Ticket candidate = BuildTicketFromFields();
            ValidationResult result = validator.Validate(candidate);

            if (!result.IsValid)
            {
                MessageBox.Show(result.Message);
                return false;
            }

            Ticket = candidate;
            return true;
        }

        private Ticket BuildTicketFromFields()
        {
            return new Ticket
            {
                Id = editingTicket?.Id ?? 0,
                CreatedDate = editingTicket?.CreatedDate ?? default,
                Title = txtTitle.Text.Trim(),
                Customer = txtCustomer.Text.Trim(),
                Status = cboStatus.SelectedItem as string,
                Priority = cboPriority.SelectedItem as string,
                AssignedTo = txtAssignedTo.Text.Trim(),
                Description = txtDescription.Text.Trim(),
            };
        }
    }
}
