using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SupportDesk.Services;
using Wisej.Services;
using Wisej.Web;

namespace SupportDesk.Web
{
    /// <summary>
    /// The Add/Edit Ticket modal. The form, its editBindingSource and the current TicketEditModel live while
    /// the dialog is open; every DbContext is created and disposed inside one TicketCommandService call.
    /// </summary>
    /// <remarks>
    /// dtpDueDate is copied by hand (Checked/Value) instead of bound: the model's DueDate is a DateTime?
    /// and DateTimePicker.Value is not nullable.
    /// </remarks>
    public partial class TicketEditorForm : Form
    {
        [Inject]
        private TicketCommandService Commands { get; set; }

        private readonly int? _ticketId;

        // Save and Delete share the guard, so a second click while either runs is dropped.
        private bool _saving;

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
            this.cboCustomer.DisplayMember = nameof(LookupItem.Name);
            this.cboCustomer.ValueMember = nameof(LookupItem.Id);
            this.cboCustomer.DataSource = lookups.Customers.ToList();

            this.cboCategory.DisplayMember = nameof(LookupItem.Name);
            this.cboCategory.ValueMember = nameof(LookupItem.Id);
            this.cboCategory.DataSource = lookups.Categories.ToList();

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

            Application.Update(this);
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
            this.cboCategory.DataBindings.Add("SelectedValue", this.editBindingSource, nameof(TicketEditModel.CategoryId), true, DataSourceUpdateMode.OnPropertyChanged);
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

                this.editBindingSource.EndEdit();
                var model = (TicketEditModel)this.editBindingSource.Current;
                model.DueDate = this.dtpDueDate.Checked ? this.dtpDueDate.Value.Date : (DateTime?)null;

                // Validation placeholder: Title is required.
                this.errorProvider.Clear();
                if (string.IsNullOrWhiteSpace(model.Title))
                {
                    this.errorProvider.SetError(this.txtTitle, "Title is required.");
                    return;
                }

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
                ShowError("The ticket could not be saved because the database rejected the change.", ex);
            }
            catch (Exception ex)
            {
                ShowError("The ticket could not be saved. Please try again in a moment.", ex);
            }
            finally
            {
                _saving = false;
                this.btnSave.Enabled = true;
                this.btnDelete.Enabled = true;
                Application.Update(this);
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
                ShowError("The ticket could not be deleted because the database rejected the change.", ex);
            }
            catch (Exception ex)
            {
                ShowError("The ticket could not be deleted. Please try again in a moment.", ex);
            }
            finally
            {
                _saving = false;
                this.btnSave.Enabled = true;
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

        private static void ShowError(string message, Exception ex)
        {
            Console.Error.WriteLine("[SupportDesk] " + ex);
            AlertBox.Show(message, MessageBoxIcon.Error, alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }
    }
}
