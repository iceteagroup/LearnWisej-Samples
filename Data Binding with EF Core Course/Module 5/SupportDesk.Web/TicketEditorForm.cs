using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SupportDesk.Services;
using Wisej.Services;
using Wisej.Web;

namespace SupportDesk.Web
{
    /// <summary>
    /// The Add/Edit Ticket modal with validation: DataAnnotations on TicketEditModel, TicketValidator for the
    /// domain rules, errorProvider and validationSummaryLabel for feedback, and DbUpdateException as the
    /// database's final word. The form, its editBindingSource and the current model live while the dialog is
    /// open; every DbContext is created and disposed inside one TicketCommandService call.
    /// </summary>
    /// <remarks>
    /// dtpDueDate is copied by hand (Checked/Value) instead of bound: the model's DueDate is a DateTime?
    /// and DateTimePicker.Value is not nullable.
    /// </remarks>
    public partial class TicketEditorForm : Form
    {
        [Inject]
        private TicketCommandService Commands { get; set; }

        [Inject]
        private TicketValidator Validator { get; set; }

        private readonly int? _ticketId;

        // Save and Delete share the guard, so a second click while either runs is dropped.
        private bool _saving;

        // Field-change handlers do nothing until the editor has finished loading, so a blank
        // "Add Ticket" does not open covered in error icons.
        private bool _wireLiveValidation;

        private string _ticketNumber;
        private bool _lookupsBound;

        /// <param name="ticketId">Null for "Add Ticket"; an existing ticket's key for "Edit Ticket".</param>
        public TicketEditorForm(int? ticketId)
        {
            _ticketId = ticketId;

            InitializeComponent();
        }

        private async void TicketEditorForm_Load(object sender, EventArgs e)
        {
            try
            {
                await LoadEditorAsync();
            }
            catch (TicketNotFoundException)
            {
                // Deleted by another session after the grid was loaded: close with OK so the grid refreshes.
                ShowWarning("This ticket was already deleted by someone else. The list will refresh.");
                this.DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                ShowError("The ticket could not be loaded right now. Please try again in a moment.", ex);
                this.DialogResult = DialogResult.Cancel;
                Close();
            }
            finally
            {
                // The load ends after the request has returned: push the alert and the close to the browser.
                Application.Update(this);
            }
        }

        #region Load

        private async Task LoadEditorAsync()
        {
            var lookups = await Commands.GetLookupsAsync();

            // The lookups get their DataSource before the model becomes editBindingSource.DataSource.
            FillLookups(lookups);

            // Status and Priority are strings, but SelectedValue still needs a ValueMember.
            this.cboStatus.DisplayMember = nameof(NamedValue.Name);
            this.cboStatus.ValueMember = nameof(NamedValue.Value);
            this.cboStatus.DataSource = lookups.Statuses.Select(s => new NamedValue(s, s)).ToList();
            this.cboPriority.DisplayMember = nameof(NamedValue.Name);
            this.cboPriority.ValueMember = nameof(NamedValue.Value);
            this.cboPriority.DataSource = lookups.Priorities.Select(p => new NamedValue(p, p)).ToList();

            TicketEditModel model;
            if (_ticketId is int existingId)
            {
                var data = await Commands.LoadEditModelAsync(existingId);
                model = data.Model;
                _ticketNumber = data.Number;
                this.Text = $"Edit Ticket — {_ticketNumber}";
                this.btnDelete.Visible = true;
            }
            else
            {
                model = TicketEditModel.NewTicket();
                _ticketNumber = null;
                this.Text = "Add Ticket";
                this.btnDelete.Visible = false;
            }

            this.editBindingSource.DataSource = model;
            BindLookupControls();

            this.dtpDueDate.Checked = model.DueDate.HasValue;
            this.dtpDueDate.Value = model.DueDate ?? DateTime.Today;

            // Initial validity decides Save.Enabled, but no icons are painted before the user touches a field.
            this.errorProvider.Clear();
            var initialMessages = Validator.Validate(model);
            this.btnSave.Enabled = initialMessages.Count == 0;
            if (initialMessages.Count > 0 && _ticketId is null)
            {
                this.validationSummaryLabel.Text = "Fill in Title, Customer and Category.";
                this.validationSummaryLabel.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
                this.validationSummaryLabel.BackColor = this.BackColor;
                this.validationSummaryLabel.Visible = true;
            }
            else
            {
                this.validationSummaryLabel.Visible = false;
            }

            _wireLiveValidation = true;
            Application.Update(this);
        }

        private void FillLookups(EditorLookups lookups)
        {
            this.cboCustomer.DisplayMember = nameof(LookupItem.Name);
            this.cboCustomer.ValueMember = nameof(LookupItem.Id);
            this.cboCustomer.DataSource = lookups.Customers.ToList();

            var agentRows = new List<LookupItem> { new LookupItem(TicketCommandService.UnassignedAgentId, "— unassigned —") };
            agentRows.AddRange(lookups.Agents);
            this.cboAgent.DisplayMember = nameof(LookupItem.Name);
            this.cboAgent.ValueMember = nameof(LookupItem.Id);
            this.cboAgent.DataSource = agentRows;

            this.cboCategory.DisplayMember = nameof(LookupItem.Name);
            this.cboCategory.ValueMember = nameof(LookupItem.Id);
            this.cboCategory.DataSource = lookups.Categories.ToList();
        }

        /// <summary>
        /// The ComboBox SelectedValue bindings are added once the lookup lists and the model both exist;
        /// added earlier they neither show the model value nor write the selection back.
        /// </summary>
        private void BindLookupControls()
        {
            if (_lookupsBound)
                return;
            _lookupsBound = true;

            this.cboCustomer.DataBindings.Add("SelectedValue", this.editBindingSource, nameof(TicketEditModel.CustomerId), true, DataSourceUpdateMode.OnPropertyChanged);

            var agentBinding = this.cboAgent.DataBindings.Add("SelectedValue", this.editBindingSource, nameof(TicketEditModel.AgentId), true, DataSourceUpdateMode.OnPropertyChanged);
            agentBinding.Format += new ConvertEventHandler(this.AgentBinding_Format);
            agentBinding.Parse += new ConvertEventHandler(this.AgentBinding_Parse);

            this.cboCategory.DataBindings.Add("SelectedValue", this.editBindingSource, nameof(TicketEditModel.CategoryId), true, DataSourceUpdateMode.OnPropertyChanged);
            this.cboStatus.DataBindings.Add("SelectedValue", this.editBindingSource, nameof(TicketEditModel.Status), true, DataSourceUpdateMode.OnPropertyChanged);
            this.cboPriority.DataBindings.Add("SelectedValue", this.editBindingSource, nameof(TicketEditModel.Priority), true, DataSourceUpdateMode.OnPropertyChanged);
        }

        // The "— unassigned —" row maps to a null AgentId and back.
        private void AgentBinding_Format(object sender, ConvertEventArgs e)
        {
            e.Value = (e.Value as int?) ?? TicketCommandService.UnassignedAgentId;
        }

        private void AgentBinding_Parse(object sender, ConvertEventArgs e)
        {
            if (e.Value is int selected && selected == TicketCommandService.UnassignedAgentId)
                e.Value = null;
        }

        #endregion

        #region Validation

        private void Field_Changed(object sender, EventArgs e)
        {
            if (!_wireLiveValidation)
                return;

            RunValidation(paint: true);
        }

        private void DueDate_Changed(object sender, EventArgs e)
        {
            if (!_wireLiveValidation)
                return;

            if (this.editBindingSource.Current is TicketEditModel model)
                model.DueDate = this.dtpDueDate.Checked ? this.dtpDueDate.Value.Date : (DateTime?)null;

            RunValidation(paint: true);
        }

        /// <summary>Validates the current model and keeps Save enabled only while it is valid.</summary>
        private IReadOnlyList<ValidationMessage> RunValidation(bool paint)
        {
            if (this.editBindingSource.Current is not TicketEditModel model)
                return Array.Empty<ValidationMessage>();

            var messages = Validator.Validate(model);
            if (paint)
                ShowValidation(messages);

            this.btnSave.Enabled = messages.Count == 0 && !_saving;
            return messages;
        }

        /// <summary>One errorProvider icon per field message; every message in validationSummaryLabel.</summary>
        private void ShowValidation(IReadOnlyList<ValidationMessage> messages)
        {
            this.errorProvider.Clear();

            foreach (var group in messages.GroupBy(m => m.FieldName))
            {
                var text = string.Join(" ", group.Select(m => m.Message));
                switch (group.Key)
                {
                    case nameof(TicketEditModel.Title):
                        this.errorProvider.SetError(this.txtTitle, text);
                        break;
                    case nameof(TicketEditModel.Description):
                        this.errorProvider.SetError(this.txtDescription, text);
                        break;
                    case nameof(TicketEditModel.Status):
                        this.errorProvider.SetError(this.cboStatus, text);
                        break;
                    case nameof(TicketEditModel.Priority):
                        this.errorProvider.SetError(this.cboPriority, text);
                        break;
                    case nameof(TicketEditModel.CustomerId):
                        this.errorProvider.SetError(this.cboCustomer, text);
                        break;
                    case nameof(TicketEditModel.CategoryId):
                        this.errorProvider.SetError(this.cboCategory, text);
                        break;
                    case nameof(TicketEditModel.DueDate):
                        this.errorProvider.SetError(this.dtpDueDate, text);
                        break;
                }
            }

            if (messages.Count == 0)
            {
                this.validationSummaryLabel.Visible = false;
                return;
            }

            this.validationSummaryLabel.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            this.validationSummaryLabel.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
            this.validationSummaryLabel.Text = string.Join("   ·   ", messages.Select(m => m.Message));
            this.validationSummaryLabel.Visible = true;
        }

        #endregion

        #region Save

        private async void btnSave_Click(object sender, EventArgs e)
        {
            await SaveAsync();
        }

        private async Task SaveAsync()
        {
            if (_saving)
                return;

            try
            {
                _saving = true;
                this.btnSave.Enabled = false;
                this.btnDelete.Enabled = false;

                this.errorProvider.Clear();
                this.editBindingSource.EndEdit();
                var model = (TicketEditModel)this.editBindingSource.Current;
                model.DueDate = this.dtpDueDate.Checked ? this.dtpDueDate.Value.Date : (DateTime?)null;

                // Invalid: show the messages and return before any DbContext is created.
                var messages = RunValidation(paint: true);
                if (messages.Count > 0)
                    return;

                await Commands.SaveAsync(model);

                this.DialogResult = DialogResult.OK;
                Close();
            }
            catch (TicketNotFoundException)
            {
                // The row the editor opened on is gone; OK tells the browser to refresh.
                ShowWarning("This ticket was already deleted by someone else. Nothing was saved.");
                this.DialogResult = DialogResult.OK;
                Close();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                ShowError("Someone else changed this ticket in the meantime. Nothing was saved. Reload and try again.", ex);
            }
            catch (DbUpdateException ex)
            {
                // A duplicate ticket number or a stale foreign key: the database's final word.
                ShowError(FriendlyDatabaseErrors.TicketSaveRejected, ex);
                await ReloadLookupsAsync();
            }
            catch (Exception ex)
            {
                ShowError("The ticket could not be saved. Please try again in a moment.", ex);
            }
            finally
            {
                _saving = false;
                RunValidation(paint: false);
                this.btnDelete.Enabled = true;
                Application.Update(this);
            }
        }

        /// <summary>Re-fetches the lookups: a customer, agent or category may have gone while the dialog was open.</summary>
        private async Task ReloadLookupsAsync()
        {
            try
            {
                var lookups = await Commands.GetLookupsAsync();
                this.cboCustomer.DataSource = lookups.Customers.ToList();

                var agentRows = new List<LookupItem> { new LookupItem(TicketCommandService.UnassignedAgentId, "— unassigned —") };
                agentRows.AddRange(lookups.Agents);
                this.cboAgent.DataSource = agentRows;

                this.cboCategory.DataSource = lookups.Categories.ToList();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("[SupportDesk] Lookups not reloaded: " + ex);
            }
        }

        #endregion

        #region Delete

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            await DeleteAsync();
        }

        private async Task DeleteAsync()
        {
            if (_saving)
                return;

            if (_ticketId is not int id)
                return;

            var confirm = await MessageBox.ShowAsync(
                $"Delete ticket {_ticketNumber}? This cannot be undone.",
                "Confirm delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            try
            {
                _saving = true;
                this.btnSave.Enabled = false;
                this.btnDelete.Enabled = false;

                var result = await Commands.DeleteAsync(id);
                switch (result.Outcome)
                {
                    case DeleteOutcome.Deleted:
                        this.DialogResult = DialogResult.OK;
                        Close();
                        break;

                    case DeleteOutcome.NotFound:
                        // Already gone: close with OK anyway so the stale row leaves the grid.
                        ShowWarning(result.Reason);
                        this.DialogResult = DialogResult.OK;
                        Close();
                        break;

                    case DeleteOutcome.Refused:
                        ShowWarning(result.Reason);
                        break;
                }
            }
            catch (DbUpdateException ex)
            {
                ShowError(FriendlyDatabaseErrors.TicketDeleteRejected, ex);
            }
            catch (Exception ex)
            {
                ShowError("The ticket could not be deleted. Please try again in a moment.", ex);
            }
            finally
            {
                _saving = false;
                RunValidation(paint: false);
                this.btnDelete.Enabled = true;
                Application.Update(this);
            }
        }

        #endregion

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        private static void ShowWarning(string message)
        {
            AlertBox.Show(message, MessageBoxIcon.Warning, alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        /// <summary>The full exception goes to the server log; the user reads one plain sentence.</summary>
        private static void ShowError(string message, Exception ex)
        {
            Console.Error.WriteLine("[SupportDesk] " + ex);
            AlertBox.Show(message, MessageBoxIcon.Error, alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }
    }
}
