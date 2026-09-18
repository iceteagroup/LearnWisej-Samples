using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Wisej.Web;
using ValidationClinic.Models;
using ValidationClinic.Services;

namespace ValidationClinic
{
    public partial class CustomerIntakeForm : Form
    {
        private readonly ContactRepository repository;
        private readonly Action<string, string> trace;
        private readonly Dictionary<string, Control> fieldMap = new();
        private readonly List<string> unmappedErrors = new();

        private ContactEditModel model = new();

        public event EventHandler Saved;

        public CustomerIntakeForm(ContactRepository repository, Action<string, string> trace)
        {
            this.repository = repository;
            this.trace = trace;
            InitializeComponent();
            SetupValidation();

        }

        private void SetupValidation()
        {
            // Allow editing another field to fix a cross-field error; Save always validates explicitly.
            AutoValidate = AutoValidate.EnableAllowFocusChange;
            btnCancel.CausesValidation = false;
            fieldMap["Name"] = txtName;
            fieldMap["Email"] = txtEmail;
            foreach (var control in fieldMap.Values)
            {
                errorProvider.SetIconAlignment(control, ErrorIconAlignment.MiddleRight);
                errorProvider.SetIconPadding(control, 4);
            }

        }

        private void ResetSummary()
        {
            errorProvider.Clear();
            unmappedErrors.Clear();

            validationSummaryLabel.Text = "";
            validationSummaryLabel.Visible = false;
        }

        private void RefreshSummary()
        {
            var messages = fieldMap.Values.Select(errorProvider.GetError).Where(x => !string.IsNullOrEmpty(x)).Concat(unmappedErrors);

            validationSummaryLabel.Text = string.Join(Environment.NewLine, messages.Distinct());
            validationSummaryLabel.Visible = validationSummaryLabel.Text.Length > 0;
        }

        private void SetError(Control control, string message)
        {
            errorProvider.SetError(control, message);
            RefreshSummary();
        }

        private bool Stop(string layer)
        {
            RefreshSummary();
            lblStatus.Text = $"Nothing saved. Correct the {layer} errors and try again.";
            trace(layer, "Save blocked; repository not called.");
            return false;
        }

        private void LoadValues(bool valid)
        {
            ResetSummary();
            model = new ContactEditModel { Name = valid ? "  Maria Chen  " : "", Email = valid ? "maria@example.com" : "maria.example.com" };
            txtName.Text = model.Name; txtEmail.Text = model.Email;

            lblStatus.Text = "Example values loaded. Press Save to run all checks.";
        }

        private ContactEditModel ReadModel()
        {
            model.Name = txtName.Text.Trim(); model.Email = txtEmail.Text.Trim();

            return model;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateChildren()) { Stop("field"); return; }
            trace("field", "ValidateChildren passed, including untouched fields.");
            ReadModel();

            try
            {
                repository.Save(model);
            }
            catch (DuplicateNameException)
            {
                SetError(txtName, "A contact with this name already exists. Choose another name.");
                lblStatus.Text = "Nothing saved. Correct the name errors and try again.";
                trace("duplicate", "Write rejected; nothing changed.");
                return;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                Console.Error.WriteLine(ex);
                lblStatus.Text = "We could not save right now. Your entries are still here. Please try again.";
                trace("system", "Write failed; no data changed. Technical details logged on server.");
                return;
            }
            trace("write", $"Saved. Repository writes: {repository.WriteCount}.");
            lblStatus.Text = $"Customer saved. Repository writes: {repository.WriteCount}.";
            Saved?.Invoke(this, EventArgs.Empty);
            AlertBox.Show("Saved.", MessageBoxIcon.Information);
        }

        private void txtName_Validating(object sender, CancelEventArgs e)
        {
            e.Cancel = string.IsNullOrWhiteSpace(txtName.Text);
            SetError(txtName, e.Cancel ? "Enter the customer's name." : "");
            trace("field", e.Cancel ? "Name rejected." : "Name accepted.");
        }

        private void txtEmail_Validating(object sender, CancelEventArgs e)
        {
            e.Cancel = string.IsNullOrWhiteSpace(txtEmail.Text) || !txtEmail.Text.Contains('@');
            SetError(txtEmail, e.Cancel ? "Enter an email address containing @." : "");
            trace("field", e.Cancel ? "Email rejected." : "Email accepted.");
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ResetSummary();
            trace("cancel", "Closed without writing.");
            Close();
        }

        private void btnValid_Click(object sender, EventArgs e)
        {
            LoadValues(true);
        }

        private void btnInvalid_Click(object sender, EventArgs e)
        {
            LoadValues(false);
        }

        private void btnFailure_Click(object sender, EventArgs e)
        {
            repository.FailNextSave = true;
            lblStatus.Text = "Next valid write will fail. Correctable input is checked first.";
        }

        private void txtName_Validated(object sender, EventArgs e)
        {
            txtName.Text = txtName.Text.Trim();
            trace("validated", "Name accepted and trimmed; no write.");
        }
    }
}
