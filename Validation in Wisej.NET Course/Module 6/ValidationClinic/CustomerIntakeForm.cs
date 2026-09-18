using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Wisej.Web;
using ValidationClinic.Models;
using ValidationClinic.Services;
using ValidationClinic.Validation;

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
            contactBindingSource.DataSource = model;
            foreach (var entry in fieldMap)
            {
                string property = entry.Value is DateTimePicker ? "Value" : "Text";
                // OnPropertyChanged plus explicit WriteValue protects Enter-to-save as well as mouse clicks.
                entry.Value.DataBindings.Add(property, contactBindingSource, entry.Key, true, DataSourceUpdateMode.OnPropertyChanged);
            }
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
            fieldMap["Age"] = txtAge;
            fieldMap["CreditLimit"] = txtCreditLimit;
            fieldMap["BirthDate"] = dtpBirthDate;
            fieldMap["StartDate"] = dtpStart;
            fieldMap["EndDate"] = dtpEnd;
            foreach (var control in fieldMap.Values)
            {
                errorProvider.SetIconAlignment(control, ErrorIconAlignment.MiddleRight);
                errorProvider.SetIconPadding(control, 4);
            }
            ConfigureValidationRules();
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
            contactBindingSource.DataSource = model;
            txtPhone.Text = valid ? "2125550123" : "12"; txtCustomerCode.Text = valid ? "CUS-0001" : "X";
            txtAge.Text = valid ? "30" : "abc"; txtCreditLimit.Text = valid ? "100" : "money";
            dtpBirthDate.Value = valid ? DateTime.Today.AddYears(-30) : DateTime.Today.AddYears(-17);
            dtpStart.Value = DateTime.Today; dtpEnd.Value = valid ? DateTime.Today.AddDays(7) : DateTime.Today.AddDays(-1);
            lblStatus.Text = "Example values loaded. Press Save to run all checks.";
        }

        private ContactEditModel ReadModel()
        {
            model = (ContactEditModel)contactBindingSource.Current;
            return model;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            contactBindingSource.EndEdit();
            foreach (var control in fieldMap.Values)
                foreach (Binding binding in control.DataBindings) binding.WriteValue();
            trace("binding", "EndEdit + WriteValue completed before model checks.");
            if (!ValidateChildren()) { Stop("field"); return; }
            trace("field", "ValidateChildren passed, including untouched fields.");
            ReadModel();
            if (!ApplyAnnotationErrors()) { Stop("model"); return; }
            if (!ApplyModelValidation()) { Stop("business"); return; }
            if (!ApplyDateErrors()) { Stop("date"); return; }

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

        private void ConfigureValidationRules()
        {
            validation.ErrorProvider = errorProvider;
            validation.SetValidationRules(txtName, ClinicRules.RequiredName());

            validation.SetValidationRules(txtEmail, ClinicRules.RequiredEmail());
            validation.SetValidationRules(txtAge, ClinicRules.RequiredAge());
            validation.SetValidationRules(txtPhone, ClinicRules.RequiredPhone());
            validation.SetValidationRules(txtCreditLimit, ClinicRules.CreditLimit());
            validation.SetValidationRules(txtCustomerCode, ClinicRules.CustomerCode());
            validation.SetValidationRules(dtpBirthDate, new ValidationRule[] { new RequiredValidationRule { InvalidMessage = "Enter a birth date." }, new MinimumAgeValidationRule() });
            validation.Validating += (_, e) => { trace("rule", $"Checking {e.Control.Name}."); RefreshSummary(); };
            validation.Validated += (_, e) => { SetError(e.Control, ""); RefreshSummary(); };
            // Extender.Validating fires BEFORE its rules. These handlers are registered after
            // SetValidationRules, so they see the final icon message on success AND failure.
            foreach (var control in fieldMap.Values)
                control.Validating += (_, e) => RefreshSummary();
        }

        private string ValidateStartEndDates() => ContactValidator.ValidateDates(dtpStart.Value, dtpEnd.Value);

        private bool ApplyDateErrors()
        {
            string message = ValidateStartEndDates();
            SetError(dtpStart, message); SetError(dtpEnd, message);
            return message.Length == 0;
        }

        private bool ApplyModelValidation()
        {
            var errors = new ContactValidator().Validate(model);
            ApplyMessages(errors);
            trace("business", errors.Count == 0 ? "Business rules passed." : "Business rules rejected the model.");
            return errors.Count == 0;
        }

        private void ApplyMessages(IEnumerable<ValidationMessage> errors)
        {
            // This stage runs only after prior stages passed; clearing cannot hide a failed field stage.
            ResetSummary();
            foreach (var group in errors.GroupBy(x => x.Field))
            {
                var message = string.Join(Environment.NewLine, group.Select(x => x.Message).Distinct());
                if (fieldMap.TryGetValue(group.Key, out var control)) SetError(control, message);
                else unmappedErrors.Add(message);
            }
            RefreshSummary();
        }

        private bool ApplyAnnotationErrors()
        {
            var errors = ModelValidation.ValidateModel(model);
            ApplyMessages(errors.SelectMany(result => result.MemberNames.DefaultIfEmpty("")
                .Select(member => new ValidationMessage(member, result.ErrorMessage))));
            trace("model", errors.Count == 0 ? "All DataAnnotations passed." : "DataAnnotations rejected the model.");
            return errors.Count == 0;
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

        private void Dates_ValueChanged(object sender, EventArgs e)
        {
            ApplyDateErrors();
        }
    }
}
