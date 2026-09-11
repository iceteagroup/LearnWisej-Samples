using System;
using Wisej.Web;
using WisejTrainingApp.Models;

namespace WisejTrainingApp.Dialogs
{
    public partial class TicketDialog : Form
    {
        private readonly Ticket editingTicket;

        /// <summary>The ticket built from the fields when the user saved.</summary>
        public Ticket Ticket { get; private set; }

        // Blank for New, pre-filled for Edit.
        public TicketDialog(Ticket ticket = null)
        {
            InitializeComponent();

            editingTicket = ticket;
            if (ticket != null)
            {
                this.Text = "Edit Ticket";
                txtTitle.Text = ticket.Title;
                txtCustomer.Text = ticket.Customer;
                cboStatus.SelectedItem = ticket.Status;
                cboPriority.SelectedItem = ticket.Priority;
                txtAssignedTo.Text = ticket.AssignedTo;
                txtDescription.Text = ticket.Description;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateForm())
                return;

            Ticket = BuildTicketFromFields();

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
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Please enter a title.");
                txtTitle.Focus();
                return false;
            }

            if (cboStatus.SelectedItem == null)
            {
                MessageBox.Show("Please choose a status.");
                cboStatus.Focus();
                return false;
            }

            return true;
        }

        private Ticket BuildTicketFromFields()
        {
            return new Ticket
            {
                Id = editingTicket?.Id ?? 0,
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
