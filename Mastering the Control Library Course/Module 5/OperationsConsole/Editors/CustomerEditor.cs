using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using OperationsConsole.Models;
using OperationsConsole.Services;
using OperationsConsole.Shell;
using Wisej.Web;

namespace OperationsConsole.Editors
{
    /// <summary>
    /// The reusable customer editor: six value-matched editors, one <see cref="ErrorProvider"/> driven by named
    /// validators, a <see cref="ToolTip"/> for guidance, and the Save / Reset / Validate commands.
    /// The host page talks to it only through <see cref="Service"/>, <see cref="Customer"/>, <see cref="IsDirty"/>,
    /// <see cref="LoadCustomer"/>, <see cref="ValidateContent"/>, <see cref="SaveAsync"/> and <see cref="Reset"/>.
    /// </summary>
    public partial class CustomerEditor : UserControl
    {
        private readonly IList<OptionItem> _statusOptions = CustomerService.StatusOptions();
        private readonly IList<OptionItem> _customerTypeOptions = CustomerService.CustomerTypeOptions();

        private CustomerService _service = new CustomerService();

        /// <summary>The last saved (or loaded) snapshot: what Reset restores and what IsDirty compares against.</summary>
        private CustomerModel _baseline;

        /// <summary>True while a save is running, so a second Save cannot submit twice.</summary>
        private bool _busy;

        /// <summary>True while LoadCustomer writes into the editors, so the change handlers stay quiet.</summary>
        private bool _loading;

        public CustomerEditor()
        {
            InitializeComponent();

            // these two depend on "today", so they are set here rather than in the designer
            dtpStartDate.MinDate = new DateTime(2000, 1, 1);
            dtpStartDate.MaxDate = DateTime.Today.AddYears(1);

            cboStatus.DataSource = _statusOptions;
            cboCustomerType.DataSource = _customerTypeOptions;

            LoadCustomer(_service.CreateBlank());
        }

        // ------------------------------------------------------------------------------------------------
        // Public surface
        // ------------------------------------------------------------------------------------------------

        /// <summary>The service the editor saves through; the hosting page assigns its own instance.</summary>
        public CustomerService Service
        {
            get => _service;
            set => _service = value ?? new CustomerService();
        }

        /// <summary>The customer as the editors currently hold it.</summary>
        public CustomerModel Customer => ReadFromEditors();

        /// <summary>True when the user changed one of the six fields since the last load or save.</summary>
        public bool IsDirty => !ReadFromEditors().HasSameValues(_baseline);

        /// <summary>Fills the editors from a model and makes it the baseline for Reset / IsDirty.</summary>
        public void LoadCustomer(CustomerModel customer)
        {
            var model = (customer ?? _service.CreateBlank()).Clone();

            _loading = true;
            try
            {
                txtName.Text = model.Name;
                txtEmail.Text = model.Email;
                SelectStoredKey(cboStatus, _statusOptions, model.StatusKey);
                SelectStoredKey(cboCustomerType, _customerTypeOptions, model.CustomerTypeKey);
                dtpStartDate.Value = ClampToPickerRange(model.StartDate);
                numCreditLimit.Value = ClampToSpinRange(model.CreditLimit);
            }
            finally
            {
                _loading = false;
            }

            _baseline = model;
            errorProvider.Clear();
        }

        /// <summary>
        /// Runs every named validator, marks each invalid control with its own message, focuses the first
        /// invalid control and returns the result.
        /// </summary>
        public ValidationResult ValidateContent()
        {
            var result = new ValidationResult();

            if (!ValidateRequiredName())
                result.Add(txtName, errorProvider.GetError(txtName));

            if (!ValidateEmail())
                result.Add(txtEmail, errorProvider.GetError(txtEmail));

            if (!ValidateCreditLimit())
                result.Add(numCreditLimit, errorProvider.GetError(numCreditLimit));

            if (!ValidateStartDate())
                result.Add(dtpStartDate, errorProvider.GetError(dtpStartDate));

            result.FocusFirstInvalid();
            return result;
        }

        /// <summary>Validate, persist, refresh state, notify. Returns true when the record was saved.</summary>
        public async Task<bool> SaveAsync()
        {
            ShellStatus.Control(btnSave.Name);

            if (_busy)
                return false;

            var validation = ValidateContent();
            if (!validation.IsValid)
            {
                ReportValidationFailure(validation);
                return false;
            }

            var customer = ReadFromEditors();

            var serviceErrors = _service.ValidateForSave(customer);
            if (serviceErrors.Count > 0)
            {
                ShellStatus.Show("The customer was not saved — " + serviceErrors[0], StatusLevel.Warning);
                AlertBox.Show(serviceErrors[0] + " Change the email address and save again.",
                    MessageBoxIcon.Warning, alignment: ContentAlignment.TopRight, autoCloseDelay: 5000);
                return false;
            }

            SetBusy(true);
            try
            {
                var stored = await _service.SaveAsync(customer);

                _baseline = stored.Clone();
                ShellStatus.Record(stored.Id);

                ShellStatus.Show("Customer " + stored.Name + " saved as " + stored.Id + ".", StatusLevel.Ok);
                ShowToast("Customer " + stored.Name + " saved as " + stored.Id + ".", "icon-check");
                return true;
            }
            catch (Exception)
            {
                ShellStatus.Show("The customer could not be saved — nothing was changed.", StatusLevel.Error);
                AlertBox.Show("The customer could not be saved. Nothing was changed — please try again in a moment.",
                    MessageBoxIcon.Error, alignment: ContentAlignment.TopRight, autoCloseDelay: 5000);
                return false;
            }
            finally
            {
                SetBusy(false);
                Application.Update(this);   // past an await: push the re-enabled buttons and the result to the browser
            }
        }

        /// <summary>Puts the last saved (or loaded) values back and clears every error mark.</summary>
        public void Reset()
        {
            LoadCustomer(_baseline?.Clone() ?? _service.CreateBlank());
            ShellStatus.Show("Reset to the last saved values.", StatusLevel.Ok);
        }

        // ------------------------------------------------------------------------------------------------
        // Field validation: one named validator per rule, called from the editor's Validating event.
        // A validator sets or clears one ErrorProvider message and never cancels the event.
        // ------------------------------------------------------------------------------------------------

        private void txtName_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_loading) return;
            ValidateRequiredName();
        }

        private void txtEmail_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_loading) return;
            ValidateEmail();
        }

        private void numCreditLimit_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_loading) return;
            ValidateCreditLimit();
        }

        private void dtpStartDate_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_loading) return;
            ValidateStartDate();
        }

        /// <summary>The credit-limit and start-date rules depend on the status, so a status change re-runs them.</summary>
        private void cboStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loading) return;

            if (errorProvider.GetError(numCreditLimit).Length > 0 || numCreditLimit.Value > 0)
                ValidateCreditLimit();

            if (errorProvider.GetError(dtpStartDate).Length > 0)
                ValidateStartDate();
        }

        private bool ValidateRequiredName()
        {
            var name = (txtName.Text ?? "").Trim();

            if (name.Length == 0)
                return Fail(txtName, "Enter the customer name — it appears on every order and invoice.");

            if (name.Length < 2)
                return Fail(txtName, "The customer name needs at least two characters.");

            return Pass(txtName);
        }

        private bool ValidateEmail()
        {
            var email = (txtEmail.Text ?? "").Trim();

            if (email.Length == 0)
                return Fail(txtEmail, "Enter an email address such as name@company.com — order confirmations go there.");

            if (!LooksLikeEmail(email))
                return Fail(txtEmail, "Enter an email address such as name@company.com (one @ and a dot in the domain).");

            return Pass(txtEmail);
        }

        /// <summary>0 … 250,000 is enforced by Minimum / Maximum; this carries what the range means for the status.</summary>
        private bool ValidateCreditLimit()
        {
            var limit = numCreditLimit.Value;
            var status = SelectedKey(cboStatus);

            if (status == "ACT" && limit < 1000)
                return Fail(numCreditLimit, "An active customer needs a credit limit of at least 1,000 — raise the limit or set the status to Prospect.");

            if (status == "PRO" && limit > 5000)
                return Fail(numCreditLimit, "A prospect may not exceed 5,000 — lower the limit or set the status to Active.");

            return Pass(numCreditLimit);
        }

        /// <summary>MinDate / MaxDate exclude out-of-range dates; this is the business rule on top.</summary>
        private bool ValidateStartDate()
        {
            var start = dtpStartDate.Value.Date;
            var status = SelectedKey(cboStatus);

            if (status == "ACT" && start > DateTime.Today)
                return Fail(dtpStartDate, "An active customer cannot start in the future — pick today or earlier, or set the status to Prospect.");

            if (status == "CLO" && start > DateTime.Today)
                return Fail(dtpStartDate, "A closed customer cannot start in the future — pick a date that has already passed.");

            return Pass(dtpStartDate);
        }

        private bool Fail(Control control, string message)
        {
            errorProvider.SetError(control, message);
            return false;
        }

        private bool Pass(Control control)
        {
            errorProvider.SetError(control, "");
            return true;
        }

        private static bool LooksLikeEmail(string value)
        {
            var at = value.IndexOf('@');
            if (at <= 0 || at != value.LastIndexOf('@') || at == value.Length - 1)
                return false;

            if (value.IndexOf(' ') >= 0)
                return false;

            var domain = value.Substring(at + 1);
            var dot = domain.IndexOf('.');
            return dot > 0 && dot < domain.Length - 1;
        }

        // ------------------------------------------------------------------------------------------------
        // Commands
        // ------------------------------------------------------------------------------------------------

        private async void btnSave_Click(object sender, EventArgs e)
        {
            await SaveAsync();
        }

        /// <summary>Discarding the user's typing is a decision, so this is the one place a MessageBox is used.</summary>
        private async void btnReset_Click(object sender, EventArgs e)
        {
            ShellStatus.Control(btnReset.Name);

            if (IsDirty)
            {
                var answer = await MessageBox.ShowAsync(
                    "Discard the changes and go back to the last saved values?", "Reset",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (answer != DialogResult.Yes)
                {
                    ShellStatus.Show("Reset cancelled — your changes are still here.", StatusLevel.Warning);
                    Application.Update(this);   // past an await: push the status to the browser
                    return;
                }
            }

            Reset();
            Application.Update(this);   // may be past an await: push the restored values to the browser
        }

        private void btnValidate_Click(object sender, EventArgs e)
        {
            ShellStatus.Control(btnValidate.Name);

            var result = ValidateContent();
            if (result.IsValid)
            {
                ShellStatus.Show("Validation passed — " + result.Summary + ".", StatusLevel.Ok);
                ShowToast("Validation passed: " + result.Summary + ".", "icon-check");
            }
            else
            {
                ReportValidationFailure(result);
            }
        }

        // ------------------------------------------------------------------------------------------------
        // Feedback and state
        // ------------------------------------------------------------------------------------------------

        private void ReportValidationFailure(ValidationResult result)
        {
            ShellStatus.Show(result.Summary + " — the marked fields explain what to fix.", StatusLevel.Warning);
            AlertBox.Show(result.Summary.Substring(0, 1).ToUpperInvariant() + result.Summary.Substring(1)
                + ". The red marks beside the fields say what to change.",
                MessageBoxIcon.Warning, alignment: ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        private static void ShowToast(string text, string icon)
        {
            new Toast(text, icon)
            {
                AutoCloseDelay = 3000,
                Alignment = ContentAlignment.TopRight
            }.Show();
        }

        /// <summary>All three commands off and the loader on Save while the service call runs.</summary>
        private void SetBusy(bool busy)
        {
            _busy = busy;

            btnSave.Enabled = !busy;
            btnReset.Enabled = !busy;
            btnValidate.Enabled = !busy;
            btnSave.ShowLoader = busy;
            lblBusy.Text = busy ? "Saving…" : "";

            if (busy)
                ShellStatus.Show("Saving the customer…", StatusLevel.Warning);
        }

        // ------------------------------------------------------------------------------------------------
        // Typed values
        // ------------------------------------------------------------------------------------------------

        private CustomerModel ReadFromEditors()
        {
            return new CustomerModel
            {
                Id = _baseline?.Id,
                Name = (txtName.Text ?? "").Trim(),
                Email = (txtEmail.Text ?? "").Trim(),
                StatusKey = SelectedKey(cboStatus),
                CustomerTypeKey = SelectedKey(cboCustomerType),
                StartDate = dtpStartDate.Value.Date,
                CreditLimit = numCreditLimit.Value,
                SavedAt = _baseline?.SavedAt
            };
        }

        /// <summary>The stored key of a bound ComboBox, never its display text.</summary>
        private static string SelectedKey(ComboBox combo)
        {
            if (combo.SelectedItem is OptionItem option)
                return option.Key;

            return combo.SelectedValue as string ?? "";
        }

        /// <summary>Selects an item by its stored key.</summary>
        private static void SelectStoredKey(ComboBox combo, IList<OptionItem> options, string key)
        {
            combo.SelectedValue = key;
            if (SelectedKey(combo) == key)
                return;

            for (var i = 0; i < options.Count; i++)
            {
                if (string.Equals(options[i].Key, key, StringComparison.OrdinalIgnoreCase))
                {
                    combo.SelectedIndex = i;
                    return;
                }
            }

            combo.SelectedIndex = 0;
        }

        private DateTime ClampToPickerRange(DateTime value)
        {
            if (value < dtpStartDate.MinDate) return dtpStartDate.MinDate;
            if (value > dtpStartDate.MaxDate) return dtpStartDate.MaxDate;
            return value;
        }

        private decimal ClampToSpinRange(decimal value)
        {
            if (value < numCreditLimit.Minimum) return numCreditLimit.Minimum;
            if (value > numCreditLimit.Maximum) return numCreditLimit.Maximum;
            return value;
        }
    }
}
