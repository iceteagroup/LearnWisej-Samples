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
            fieldMap["Phone"] = txtPhone;
            fieldMap["CustomerCode"] = txtCustomerCode;
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
            txtCustomerCode.InvalidMessage = "";
            validationSummaryLabel.Text = "";
            validationSummaryLabel.Visible = false;
        }

        private void RefreshSummary()
        {
            var messages = fieldMap.Values.Select(errorProvider.GetError).Where(x => !string.IsNullOrEmpty(x)).Concat(unmappedErrors);
            messages = messages.Concat(new[] { txtCustomerCode.InvalidMessage }.Where(x => !string.IsNullOrEmpty(x)));
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
            txtPhone.Text = valid ? "2125550123" : "12"; txtCustomerCode.Text = valid ? "CUS-0001" : "X";

            lblStatus.Text = "Example values loaded. Press Save to run all checks.";
        }

        private ContactEditModel ReadModel()
        {
            model.Name = txtName.Text.Trim(); model.Email = txtEmail.Text.Trim();
            model.Phone = txtPhone.Text.Trim(); model.CustomerCode = txtCustomerCode.Text.Trim();

            return model;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!Apply(ValidateName(), ValidateEmail(), ValidatePhone())) { Stop("field"); return; }
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

        private sealed record FieldValidation(Control Control, string FieldName, string Message)
        { public bool IsValid => string.IsNullOrEmpty(Message); }

        private FieldValidation ValidateName() => new(txtName, "Name", string.IsNullOrWhiteSpace(txtName.Text) ? "Name is required." : "");

        private FieldValidation ValidateEmail() => new(txtEmail, "Email", !txtEmail.Text.Trim().Contains('@') ? "Enter a valid email address." : "");

        private FieldValidation ValidatePhone() => new(txtPhone, "Phone", txtPhone.Text.Trim().Count(char.IsDigit) < 7 ? "Enter a phone number with at least 7 digits." : "");

        private bool Apply(params FieldValidation[] results)
        {
            foreach (var result in results) errorProvider.SetError(result.Control, result.Message);
            RefreshSummary();
            return results.All(x => x.IsValid);
        }

        private void ValidateField(FieldValidation result, CancelEventArgs e)
        {
            e.Cancel = !result.IsValid;
            Apply(result);
        }

        private void txtCustomerCode_Validating(object sender, CancelEventArgs e)
        {
            e.Cancel = txtCustomerCode.Text.Trim().Length < 3;
            txtCustomerCode.InvalidMessage = e.Cancel ? "Enter a customer code of at least 3 characters." : "";
            RefreshSummary();
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

        private void txtName_Validating(object sender, CancelEventArgs e)
        {
            ValidateField(ValidateName(), e);
        }

        private void txtEmail_Validating(object sender, CancelEventArgs e)
        {
            ValidateField(ValidateEmail(), e);
        }

        private void txtPhone_Validating(object sender, CancelEventArgs e)
        {
            ValidateField(ValidatePhone(), e);
        }
    }
}
