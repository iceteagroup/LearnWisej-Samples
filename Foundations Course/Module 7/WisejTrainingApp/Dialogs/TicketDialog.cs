using System;
using Wisej.Web;
using WisejTrainingApp.Models;

namespace WisejTrainingApp.Dialogs
{
    /// <summary>
    /// The Module 5 ticket dialog, carried into Module 7 unchanged in behaviour:
    /// ValidateForm() decides whether Save is allowed, TicketResult carries the ticket back,
    /// Save sets DialogResult.OK and closes, Cancel sets DialogResult.Cancel.
    /// The caller awaits ShowDialogAsync() and reads TicketResult only when the result is OK.
    ///
    /// The theme selector in the header does not know this class exists, and this class does not know
    /// which theme is loaded — that is the separation the lesson asks for. A blank title is blocked
    /// exactly the same way under Bootstrap-4, BootstrapDark-4 or Material-3.
    /// </summary>
    public partial class TicketDialog : Form
    {
        /// <summary>Raised when Save is clicked and ValidateForm() says no — the caller logs it.</summary>
        public event EventHandler<string> ValidationFailed;

        /// <summary>The ticket the caller should save (only meaningful when DialogResult is OK).</summary>
        public Ticket TicketResult { get; private set; }

        private readonly bool isNew;

        /// <summary>Pass null for a new ticket, or an existing ticket to edit (the dialog works on a copy).</summary>
        public TicketDialog(Ticket existing)
        {
            InitializeComponent();

            isNew = existing == null;
            Ticket source = existing?.Clone() ?? new Ticket { Status = "Open", Priority = "Medium", Customer = "", Title = "", AssignedTo = "", Description = "" };

            Text = isNew ? "New ticket" : $"Edit ticket #{source.Id}";
            lblDialogHeading.Text = isNew ? "New ticket" : $"Edit ticket #{source.Id}";
            TicketResult = source;

            txtTitle.Text = source.Title;
            txtCustomer.Text = source.Customer;
            cboStatus.SelectedItem = source.Status;
            cboPriority.SelectedItem = source.Priority;
            txtAssignedTo.Text = source.AssignedTo;
            txtDescription.Text = source.Description;
        }

        /// <summary>Pre-fills the fields (used by the "Try a blank title" button) — the validation is still ValidateForm().</summary>
        public void Prefill(string title, string customer, string status, string priority)
        {
            txtTitle.Text = title;
            txtCustomer.Text = customer;
            cboStatus.SelectedItem = status;
            cboPriority.SelectedItem = priority;
        }

        /// <summary>
        /// The validation rule set from Module 5. Returns true when the ticket may be saved; otherwise shows
        /// the reason in lblValidation and puts the focus on the offending field.
        /// </summary>
        public bool ValidateForm()
        {
            lblValidation.Text = "";

            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                lblValidation.Text = "Title is required.";
                txtTitle.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtCustomer.Text))
            {
                lblValidation.Text = "Customer is required.";
                txtCustomer.Focus();
                return false;
            }

            if (cboStatus.SelectedItem == null)
            {
                lblValidation.Text = "Choose a status.";
                cboStatus.Focus();
                return false;
            }

            if (cboPriority.SelectedItem == null)
            {
                lblValidation.Text = "Choose a priority.";
                cboPriority.Focus();
                return false;
            }

            return true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateForm())
            {
                ValidationFailed?.Invoke(this, lblValidation.Text);
                return;
            }

            TicketResult.Title = txtTitle.Text.Trim();
            TicketResult.Customer = txtCustomer.Text.Trim();
            TicketResult.Status = cboStatus.SelectedItem.ToString();
            TicketResult.Priority = cboPriority.SelectedItem.ToString();
            TicketResult.AssignedTo = txtAssignedTo.Text.Trim();
            TicketResult.Description = txtDescription.Text.Trim();

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
