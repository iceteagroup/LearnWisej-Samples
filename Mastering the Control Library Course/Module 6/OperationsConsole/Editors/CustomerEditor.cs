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
    /// The reusable customer editor of Module 2 — the point where a handful of controls becomes an
    /// application-level component.
    ///
    /// It owns its labels, its six value-matched editors, ONE <see cref="ErrorProvider"/> (field errors),
    /// a <see cref="ToolTip"/> and a <see cref="HelpTip"/> (guidance), and the three commands
    /// (<c>btnValidate</c>, <c>btnReset</c>, <c>btnSave</c>). Everything else in the application talks to
    /// it through a small public surface — <see cref="Customer"/>, <see cref="LoadCustomer"/>,
    /// <see cref="ValidateContent"/>, <see cref="SaveAsync"/>, <see cref="Reset"/>, <see cref="IsDirty"/>,
    /// <see cref="Saved"/> and <see cref="ValidationFailed"/> — and never learns how the email rule works.
    ///
    /// The six-step validation flow of the reading is implemented in exactly this order:
    ///   1. editor properties (MaxLength, Minimum/Maximum, MinDate/MaxDate, DropDownList) — see the designer;
    ///   2. typed values instead of parsing (<see cref="ReadFromEditors"/>);
    ///   3. field validation in each editor's Validating event (one named validator per rule);
    ///   4. form-level then service-level validation on Save;
    ///   5. on failure: stay on the screen, focus the first invalid control, explain;
    ///   6. on success: a non-blocking confirmation and a status update.
    /// </summary>
    public partial class CustomerEditor : UserControl
    {
        private static readonly Color OkColor = Color.FromArgb(31, 157, 87);
        private static readonly Color HintColor = Color.FromArgb(90, 107, 125);
        private static readonly Color BusyColor = Color.FromArgb(232, 161, 60);

        private readonly IList<OptionItem> _statusOptions = CustomerService.StatusOptions();
        private readonly IList<OptionItem> _customerTypeOptions = CustomerService.CustomerTypeOptions();

        private CustomerService _service = new CustomerService();

        /// <summary>The last saved (or loaded) snapshot: what Reset restores and what IsDirty compares against.</summary>
        private CustomerModel _baseline;

        /// <summary>True while a save is running: the second click of a double click is swallowed here.</summary>
        private bool _busy;

        /// <summary>True while Load() writes into the editors, so the Validating / SelectedIndexChanged handlers stay quiet.</summary>
        private bool _loading;

        public CustomerEditor()
        {
            InitializeComponent();

            // Step 1 — editor properties that make impossible values impossible.
            // These two depend on "today", so they cannot be constants in the designer file.
            dtpStartDate.MinDate = new DateTime(2000, 1, 1);
            dtpStartDate.MaxDate = DateTime.Today.AddYears(1);

            // ComboBox lists: DisplayMember/ValueMember are set in the designer, the data comes from the service.
            cboStatus.DataSource = _statusOptions;
            cboCustomerType.DataSource = _customerTypeOptions;

            LoadCustomer(_service.CreateBlank());
        }

        // ------------------------------------------------------------------------------------------------
        // Public surface — everything the host page is allowed to know
        // ------------------------------------------------------------------------------------------------

        /// <summary>
        /// The service the editor saves through. The hosting page assigns its own instance so the
        /// "Simulate service failure" checkbox and the editor share one back end.
        /// </summary>
        public CustomerService Service
        {
            get => _service;
            set => _service = value ?? new CustomerService();
        }

        /// <summary>The customer as the editors currently hold it (typed values, no parsing).</summary>
        public CustomerModel Customer => ReadFromEditors();

        /// <summary>True when the user changed one of the six fields since the last Load / Save.</summary>
        public bool IsDirty => !ReadFromEditors().HasSameValues(_baseline);

        /// <summary>Raised after the service committed the record — the host reads <see cref="Customer"/>.</summary>
        public event EventHandler Saved;

        /// <summary>Raised when <see cref="ValidateContent"/> found at least one field error.</summary>
        public event EventHandler ValidationFailed;

        /// <summary>The result of the last <see cref="ValidateContent"/> call, for the host's log lines.</summary>
        public ValidationResult LastValidation { get; private set; }

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
            RefreshState(model);
        }

        /// <summary>
        /// Form-level validation (step 4a): runs every named validator, marks each offending control with its
        /// own ErrorProvider message, focuses the first invalid control and returns what the host can read.
        /// Named <c>ValidateContent</c> so it cannot be confused with the validation members a
        /// <see cref="ContainerControl"/> already exposes.
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
            LastValidation = result;

            ConsoleLog.Add("CustomerEditor.ValidateContent() → " + result.Summary
                + (result.IsValid ? "" : " · first invalid: " + result.Errors[0].ControlName));

            foreach (var error in result.Errors)
                ConsoleLog.Add("   ✗ " + error.ControlName + " — " + error.Message);

            return result;
        }

        /// <summary>
        /// The explicit Save path of the reading: <b>validate, persist, refresh state, notify</b>.
        /// The commands are disabled and <c>btnSave.ShowLoader</c> is on for the whole call (try / finally),
        /// so a second click while it runs cannot submit twice. Returns true when the record was committed.
        /// </summary>
        public async Task<bool> SaveAsync()
        {
            ConsoleLog.Control(btnSave.Name);

            if (_busy)
            {
                // this is the guarantee behind "Save twice does nothing twice" — the disabled buttons are the
                // visible half of it, this flag is the half that survives a click that was already in flight
                ConsoleLog.Add("btnSave ignored — a save is already running (busy state)");
                return false;
            }

            // step 4a — form-level validation
            var validation = ValidateContent();
            if (!validation.IsValid)
            {
                ReportValidationFailure(validation);
                return false;
            }

            var customer = ReadFromEditors();   // carries the baseline id: an update stays an update

            // step 4b — service-level validation (rules that need the whole store, not one screen)
            var serviceErrors = _service.ValidateForSave(customer);
            if (serviceErrors.Count > 0)
            {
                ConsoleLog.Add("✗ service validation rejected the save — " + serviceErrors[0]);
                ConsoleLog.Status("The customer was not saved — " + serviceErrors[0], StatusLevel.Warning);
                AlertBox.Show(serviceErrors[0] + " Change the email address and save again.",
                    MessageBoxIcon.Warning, alignment: ContentAlignment.TopRight, autoCloseDelay: 5000);
                return false;
            }

            SetBusy(true);
            try
            {
                var stored = await _service.SaveAsync(customer);

                // step: refresh state — the id and the save stamp the service produced
                _baseline = stored.Clone();
                RefreshState(stored);
                ConsoleLog.Record(stored.Id);

                // step 6 — notify without interrupting: a Toast, not a MessageBox
                ConsoleLog.Status("Customer " + stored.Name + " saved as " + stored.Id + ".", StatusLevel.Ok);
                ShowToast("Customer " + stored.Name + " saved as " + stored.Id + ".", "icon-check");
                Saved?.Invoke(this, EventArgs.Empty);
                return true;
            }
            catch (Exception ex)
            {
                // one friendly sentence for the user, the exception details for the Event log only
                ConsoleLog.Add("✗ CustomerService.SaveAsync threw " + ex.GetType().Name);
                ConsoleLog.Add("   " + ex.Message);
                ConsoleLog.Status("The customer could not be saved — nothing was changed.", StatusLevel.Error);
                AlertBox.Show("The customer could not be saved. Nothing was changed — please try again in a moment.",
                    MessageBoxIcon.Error, alignment: ContentAlignment.TopRight, autoCloseDelay: 5000);
                return false;
            }
            finally
            {
                SetBusy(false);
            }
        }

        /// <summary>Puts the last saved (or loaded) values back and clears every error mark.</summary>
        public void Reset()
        {
            var restored = _baseline?.Clone() ?? _service.CreateBlank();
            LoadCustomer(restored);

            ConsoleLog.Add("CustomerEditor.Reset() → restored " + (restored.IsNew ? "the blank record" : restored.Id)
                + ", ErrorProvider cleared");
            ConsoleLog.Status("Reset to the last saved values.", StatusLevel.Ok);
        }

        // ------------------------------------------------------------------------------------------------
        // Step 3 — field validation: one named validator per rule, called from the editor's Validating event.
        // A validator sets or clears exactly ONE ErrorProvider message on exactly ONE control, and it never
        // cancels the event: the user is allowed to leave a field that is not finished yet.
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

        /// <summary>
        /// The credit-limit and start-date rules depend on the status, so changing the status re-runs them:
        /// switching Active → Prospect must clear a mark that is no longer true.
        /// </summary>
        private void cboStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loading) return;

            ConsoleLog.Add("cboStatus → stored key \"" + SelectedKey(cboStatus) + "\" (display text \""
                + CustomerService.TextOf(_statusOptions, SelectedKey(cboStatus)) + "\")");

            // only re-run the rules that read the status; do not mark fields the user has not touched
            if (errorProvider.GetError(numCreditLimit).Length > 0 || numCreditLimit.Value > 0)
                ValidateCreditLimit();

            if (errorProvider.GetError(dtpStartDate).Length > 0)
                ValidateStartDate();
        }

        /// <summary>Required field. The editor cannot express "not empty", so this is the first real validator.</summary>
        private bool ValidateRequiredName()
        {
            var name = (txtName.Text ?? "").Trim();

            if (name.Length == 0)
                return Fail(txtName, "Enter the customer name — it appears on every order and invoice.");

            if (name.Length < 2)
                return Fail(txtName, "The customer name needs at least two characters.");

            return Pass(txtName);
        }

        /// <summary>
        /// Email shape. <c>MaxLength</c> and <c>CharacterCasing.Lower</c> already bound and normalise the text;
        /// the shape still needs a rule, and the rule sets or clears one message on <c>txtEmail</c>.
        /// </summary>
        private bool ValidateEmail()
        {
            var email = (txtEmail.Text ?? "").Trim();

            if (email.Length == 0)
                return Fail(txtEmail, "Enter an email address such as name@company.com — order confirmations go there.");

            if (!LooksLikeEmail(email))
                return Fail(txtEmail, "Enter an email address such as name@company.com (one @ and a dot in the domain).");

            return Pass(txtEmail);
        }

        /// <summary>
        /// Business range. 0 … 250,000 is already enforced by <c>Minimum</c> / <c>Maximum</c>, so this validator
        /// only carries what the control cannot express: what the range means for THIS status.
        /// </summary>
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

        /// <summary>
        /// Date rule. <c>MinDate</c> / <c>MaxDate</c> already exclude an out-of-era date and anything beyond a
        /// year from now, so the reachable rule is the business one: an active customer cannot start in the future.
        /// </summary>
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
            errorProvider.SetError(control, "");   // "" is how an ErrorProvider message is cleared
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
        // Commands — short handlers that call the named methods above
        // ------------------------------------------------------------------------------------------------

        private async void btnSave_Click(object sender, EventArgs e)
        {
            await SaveAsync();
        }

        /// <summary>
        /// The one place a <see cref="MessageBox"/> is justified in this module: discarding the user's typing
        /// is a decision, not a status message, so it blocks until the user answers.
        /// </summary>
        private async void btnReset_Click(object sender, EventArgs e)
        {
            ConsoleLog.Control(btnReset.Name);

            if (IsDirty)
            {
                ConsoleLog.Add("btnReset → the form is dirty, asking for a confirmation (MessageBox)");
                var answer = await MessageBox.ShowAsync(
                    "Discard the changes and go back to the last saved values?", "Reset",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (answer != DialogResult.Yes)
                {
                    ConsoleLog.Add("btnReset → the user kept the changes");
                    ConsoleLog.Status("Reset cancelled — your changes are still here.", StatusLevel.Warning);
                    return;
                }
            }
            else
            {
                ConsoleLog.Add("btnReset → nothing was changed, no confirmation needed");
            }

            Reset();
        }

        private void btnValidate_Click(object sender, EventArgs e)
        {
            ConsoleLog.Control(btnValidate.Name);

            var result = ValidateContent();
            if (result.IsValid)
            {
                ConsoleLog.Status("Validation passed — " + result.Summary + ".", StatusLevel.Ok);
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

        /// <summary>Step 5: keep the user on the screen, say how many fields need work, mark them, focus the first.</summary>
        private void ReportValidationFailure(ValidationResult result)
        {
            ConsoleLog.Status(result.Summary + " — the marked fields explain what to fix.", StatusLevel.Warning);
            AlertBox.Show(result.Summary.Substring(0, 1).ToUpperInvariant() + result.Summary.Substring(1)
                + ". The red marks beside the fields say what to change.",
                MessageBoxIcon.Warning, alignment: ContentAlignment.TopRight, autoCloseDelay: 4000);
            ValidationFailed?.Invoke(this, EventArgs.Empty);
        }

        private static void ShowToast(string text, string icon)
        {
            new Toast(text, icon)
            {
                AutoCloseDelay = 3000,
                Alignment = ContentAlignment.TopRight
            }.Show();
        }

        /// <summary>
        /// Busy state: all three commands off and the loader on <c>btnSave</c> while the service call runs.
        /// Disabled buttons and busy feedback are part of reliability, not decoration.
        /// </summary>
        private void SetBusy(bool busy)
        {
            _busy = busy;

            btnSave.Enabled = !busy;
            btnReset.Enabled = !busy;
            btnValidate.Enabled = !busy;
            btnSave.ShowLoader = busy;

            lblBusy.Text = busy
                ? "Saving… Save, Reset and Validate are disabled."
                : "Idle — the three commands are enabled.";
            lblBusy.ForeColor = busy ? BusyColor : HintColor;

            if (busy)
            {
                ConsoleLog.Add("busy state on — btnSave.ShowLoader = true, btnSave / btnReset / btnValidate disabled");
                ConsoleLog.Status("Saving the customer…", StatusLevel.Warning);
            }
            else
            {
                ConsoleLog.Add("busy state off — the three commands are enabled again");
            }
        }

        /// <summary>The state refresh of the Save path: the record id and the save stamp the service produced.</summary>
        private void RefreshState(CustomerModel model)
        {
            lblRecordId.Text = model.IsNew
                ? "Record: — (new customer, not saved yet)"
                : "Record: " + model.Id + " · status " + model.StatusKey + " · type " + model.CustomerTypeKey;

            lblSavedStamp.Text = model.SavedAt.HasValue
                ? "Last saved: " + model.SavedAt.Value.ToString("HH:mm:ss") + " · " + _service.Count + " record(s) in memory"
                : "Last saved: —";

            lblRecordId.ForeColor = model.IsNew ? HintColor : OkColor;
        }

        // ------------------------------------------------------------------------------------------------
        // Step 2 — typed values: nothing here parses a date or a number out of text
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

        /// <summary>The STORED key of a bound ComboBox, never its display text.</summary>
        private static string SelectedKey(ComboBox combo)
        {
            if (combo.SelectedItem is OptionItem option)
                return option.Key;

            return combo.SelectedValue as string ?? "";
        }

        /// <summary>
        /// Selects an item by its STORED key. <c>SelectedValue</c> is the data-binding route
        /// (<c>ValueMember = "Key"</c>); the index lookup behind it keeps the editor deterministic
        /// whatever the binding does, and is also what makes "ACT" → "Active" a one-way mapping.
        /// </summary>
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
