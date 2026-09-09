using System;
using System.Globalization;
using Wisej.Web;
using WisejTrainingApp.Dialogs;
using WisejTrainingApp.Models;
using WisejTrainingApp.Services;

namespace WisejTrainingApp
{
    /// <summary>
    /// Tickets page — Module 5 lab window (Dialogs and the modal workflow / Validation and the Save-Cancel pattern).
    ///
    /// Main card:   the Module 4 ticket grid (dgvTickets bound through a BindingSource) with the command row
    ///              New Ticket · Edit Ticket · Delete Ticket · Refresh. New and Edit open the same TicketDialog,
    ///              Delete asks for confirmation with MessageBox.ShowAsync.
    /// Right card:  the five-step workflow (Button click → Open dialog → Validate → Save → Refresh UI) with the
    ///              running step marked ▶, and the event log — every server-side decision, with a timestamp.
    /// Bottom bar:  helpers for the failure paths (clear the selection so the Edit guard fires), a data reset
    ///              (recovery) and Clear log.
    ///
    /// The handlers that open dialogs are <c>async void</c>: Wisej.NET has no blocking ShowDialog(), the
    /// page awaits <c>ShowDialogAsync()</c> and continues when the dialog closes.
    /// </summary>
    public partial class TicketsWindow : Form
    {
        // One service per user session (instance field, never static): the in-memory list lives here.
        private TicketService ticketService = new TicketService();

        public TicketsWindow()
        {
            InitializeComponent();
        }

        private void TicketsWindow_Load(object sender, EventArgs e)
        {
            AddLog("Program.Main → new TicketsWindow().Show()");
            AddLog("TicketService seeded with 6 tickets (per session, instance field)");
            RefreshTicketGrid();
            ShowWorkflowStep(0);
            SetStatus($"ready — {ticketService.GetTickets().Count} tickets loaded", StatusKind.Ok);
        }

        #region The five-step workflow: New / Edit / Delete

        /// <summary>
        /// New: open a blank dialog, and only when it returns OK add the ticket through the service and refresh.
        /// Cancel (or closing the dialog) saves nothing and refreshes nothing.
        /// </summary>
        private async void btnNew_Click(object sender, EventArgs e)
        {
            ShowWorkflowStep(1);
            AddLog("btnNew_Click → step 1 Button click (New)");

            var dialog = new TicketDialog();
            dialog.ValidationChecked += OnDialogValidationChecked;

            ShowWorkflowStep(2);
            AddLog("step 2 Open dialog → new TicketDialog() → await ShowDialogAsync()");
            var result = await dialog.ShowDialogAsync();

            if (result == DialogResult.OK)
            {
                ShowWorkflowStep(4);
                ticketService.AddTicket(dialog.TicketResult);
                AddLog($"step 4 Save → DialogResult.OK → ticketService.AddTicket(\"{dialog.TicketResult.Title}\") → Id {dialog.TicketResult.Id}");

                ShowWorkflowStep(5);
                RefreshTicketGrid();
                SelectTicket(dialog.TicketResult.Id);
                ShowSuccess("Ticket created.");
            }
            else
            {
                ShowWorkflowStep(0);
                SetStatus("create cancelled — nothing saved", StatusKind.Warn);
                AddLog($"dialog returned DialogResult.{result} → Create cancelled — nothing saved, grid not refreshed");
            }

            Application.Update(this);
        }

        /// <summary>
        /// Edit: the guard first (a row must be selected), then the same dialog pre-filled with the selected
        /// ticket. On OK the service replaces the ticket and the grid refreshes.
        /// </summary>
        private async void btnEdit_Click(object sender, EventArgs e)
        {
            ShowWorkflowStep(1);
            AddLog("btnEdit_Click → step 1 Button click (Edit)");

            Ticket selected = GetSelectedTicket();
            if (selected == null)
            {
                // Failure path (lab step 8): no selection → message and stop. No dialog is opened.
                ShowWorkflowStep(0);
                SetStatus("Select a ticket to edit.", StatusKind.Warn);
                AlertBox.Show("Select a ticket to edit.", MessageBoxIcon.Warning, alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
                AddLog("guard: no row selected → \"Select a ticket to edit.\" → return (no dialog opened)");
                return;
            }

            var dialog = new TicketDialog(selected);
            dialog.ValidationChecked += OnDialogValidationChecked;

            ShowWorkflowStep(2);
            AddLog($"step 2 Open dialog → new TicketDialog(ticket #{selected.Id}) → fields pre-filled → await ShowDialogAsync()");
            var result = await dialog.ShowDialogAsync();

            if (result == DialogResult.OK)
            {
                ShowWorkflowStep(4);
                ticketService.UpdateTicket(dialog.TicketResult);
                AddLog($"step 4 Save → DialogResult.OK → ticketService.UpdateTicket(#{dialog.TicketResult.Id}) → Status \"{dialog.TicketResult.Status}\", Priority \"{dialog.TicketResult.Priority}\"");

                ShowWorkflowStep(5);
                RefreshTicketGrid();
                SelectTicket(dialog.TicketResult.Id);
                ShowSuccess("Ticket updated.");
            }
            else
            {
                ShowWorkflowStep(0);
                SetStatus("edit cancelled — nothing saved", StatusKind.Warn);
                AddLog($"dialog returned DialogResult.{result} → Edit cancelled — ticket #{selected.Id} unchanged, grid not refreshed");
            }

            Application.Update(this);
        }

        /// <summary>
        /// Delete is a risky action, so it asks first (lesson s21, "Message boxes and confirmations").
        /// </summary>
        private async void btnDelete_Click(object sender, EventArgs e)
        {
            AddLog("btnDelete_Click");

            Ticket selected = GetSelectedTicket();
            if (selected == null)
            {
                SetStatus("Select a ticket to delete.", StatusKind.Warn);
                AlertBox.Show("Select a ticket to delete.", MessageBoxIcon.Warning, alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
                AddLog("guard: no row selected → \"Select a ticket to delete.\" → return");
                return;
            }

            AddLog($"await MessageBox.ShowAsync(\"Delete ticket #{selected.Id}?\", YesNo, Question)");
            DialogResult answer = await MessageBox.ShowAsync(
                $"Delete ticket #{selected.Id}?",
                "Confirm delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (answer == DialogResult.Yes)
            {
                ticketService.DeleteTicket(selected.Id);
                AddLog($"DialogResult.Yes → ticketService.DeleteTicket({selected.Id})");
                RefreshTicketGrid();
                ShowSuccess($"Ticket #{selected.Id} deleted.");
            }
            else
            {
                SetStatus("delete cancelled", StatusKind.Warn);
                AddLog($"DialogResult.{answer} → Delete cancelled — ticket #{selected.Id} kept, grid not refreshed");
            }

            Application.Update(this);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            AddLog("btnRefresh_Click → RefreshTicketGrid()");
            RefreshTicketGrid();
            SetStatus($"grid refreshed — {ticketService.GetTickets().Count} tickets", StatusKind.Ok);
        }

        private void dgvTickets_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Double-clicking a row is just another way into Edit; there is one Edit handler.
            if (e.RowIndex < 0)
                return;

            AddLog($"dgvTickets.CellDoubleClick(row {e.RowIndex}) → btnEdit_Click");
            btnEdit_Click(sender, EventArgs.Empty);
        }

        private void dgvTickets_SelectionChanged(object sender, EventArgs e)
        {
            Ticket selected = GetSelectedTicket();
            lblSelection.Text = selected == null
                ? "selected: none"
                : $"selected: #{selected.Id} {selected.Title}";
        }

        /// <summary>Step 3 of the workflow happens inside the dialog; it reports the outcome so the log shows it.</summary>
        private void OnDialogValidationChecked(bool passed, string message)
        {
            ShowWorkflowStep(3);
            if (passed)
            {
                AddLog("step 3 Validate → ValidateForm() = true → BuildTicketFromFields() → DialogResult.OK → Close()");
            }
            else
            {
                SetStatus("validation failed — dialog stays open", StatusKind.Error);
                AddLog($"step 3 Validate → ValidateForm() = false → \"{message}\" → return (dialog stays open, nothing saved)");
            }
        }

        #endregion

        #region Grid refresh — only after a successful save (or a delete / explicit refresh)

        /// <summary>
        /// The one place the grid is filled: re-read the service and tell the BindingSource its list changed.
        /// Called after AddTicket / UpdateTicket / DeleteTicket succeeded and by Refresh — never after Cancel.
        /// </summary>
        private void RefreshTicketGrid()
        {
            ticketsBindingSource.DataSource = ticketService.GetTickets();
            ticketsBindingSource.ResetBindings(false);
            AddLog($"step 5 Refresh UI → RefreshTicketGrid() → GetTickets() = {ticketsBindingSource.Count} rows → ResetBindings(false)");
        }

        private Ticket GetSelectedTicket()
        {
            if (dgvTickets.SelectedRows.Count == 0)
                return null;

            return dgvTickets.SelectedRows[0].DataBoundItem as Ticket;
        }

        /// <summary>After a save, keep the saved ticket selected so the user sees the row that changed.</summary>
        private void SelectTicket(int id)
        {
            foreach (DataGridViewRow row in dgvTickets.Rows)
            {
                if (row.DataBoundItem is Ticket ticket && ticket.Id == id)
                {
                    row.Selected = true;
                    return;
                }
            }
        }

        #endregion

        #region Bottom bar: failure-path setup, recovery, clear

        private void btnClearSelection_Click(object sender, EventArgs e)
        {
            dgvTickets.ClearSelection();
            lblSelection.Text = "selected: none";
            SetStatus("selection cleared — click Edit Ticket to see the guard", StatusKind.Warn);
            AddLog("btnClearSelection_Click → dgvTickets.ClearSelection() → Edit / Delete will stop at their guard");
        }

        private void btnResetData_Click(object sender, EventArgs e)
        {
            ticketService = new TicketService();
            AddLog("btnResetData_Click → new TicketService() → 6 seeded tickets");
            RefreshTicketGrid();
            ShowWorkflowStep(0);
            SetStatus("sample data reset — 6 tickets", StatusKind.Ok);
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            lstEventLog.Items.Clear();
        }

        #endregion

        #region Helpers (small, reusable — the habit the course teaches)

        private enum StatusKind { Ok, Warn, Error }

        private static readonly string[] WorkflowSteps =
        {
            "1 Button click   New / Edit clicked",
            "2 Open dialog    await ShowDialogAsync()",
            "3 Validate       ValidateForm() in the dialog",
            "4 Save           service.AddTicket / UpdateTicket",
            "5 Refresh UI     RefreshTicketGrid() + feedback",
        };

        /// <summary>Redraws the workflow label with ▶ on the running step (0 = idle, nothing marked).</summary>
        private void ShowWorkflowStep(int step)
        {
            var lines = new string[WorkflowSteps.Length];
            for (int i = 0; i < WorkflowSteps.Length; i++)
            {
                int number = i + 1;
                string marker = number == step ? "▶" : (number < step ? "✓" : " ");
                lines[i] = $"{marker} {WorkflowSteps[i]}";
            }

            lblWorkflow.Text = string.Join("\n", lines);
        }

        /// <summary>Success feedback in one place: toast (top-right), green status, log line.</summary>
        private void ShowSuccess(string message)
        {
            AlertBox.Show(message, MessageBoxIcon.Information, alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
            SetStatus(message, StatusKind.Ok);
            AddLog($"ShowSuccess(\"{message}\") → AlertBox + lblStatus");
        }

        private void SetStatus(string text, StatusKind kind)
        {
            lblStatus.Text = "● " + text;
            lblStatus.ForeColor = kind switch
            {
                StatusKind.Error => System.Drawing.Color.FromArgb(224, 86, 59),
                StatusKind.Warn => System.Drawing.Color.FromArgb(232, 161, 60),
                _ => System.Drawing.Color.FromArgb(31, 157, 87),
            };
        }

        /// <summary>One place for logging, so every handler stays short.</summary>
        private void AddLog(string message)
        {
            string time = DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
            lstEventLog.Items.Add($"{time}  {message}");
            lstEventLog.SelectedIndex = lstEventLog.Items.Count - 1;
        }

        #endregion
    }
}
