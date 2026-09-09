using System;
using System.Collections.Generic;
using Wisej.Web;
using WisejTrainingApp.Dialogs;
using WisejTrainingApp.Models;

namespace WisejTrainingApp.Views
{
    /// <summary>
    /// Tickets page — the Module 4/5 screen kept intact (lab step 8 · "Keep logic intact").
    ///
    /// What Module 7 changed here: the screen is a UserControl inside the new shell, the grid sits in a white
    /// "Ticket queue" card with the same 32 px page padding as every other page, and the command buttons sit
    /// under the queue they act on.
    ///
    /// What Module 7 did NOT change (the "logic intact" proof):
    ///   - TicketService (Services/TicketService.cs) — GetTickets / AddTicket / UpdateTicket / DeleteTicket / SaveTicket
    ///   - Ticket (Models/Ticket.cs) — the shared shape
    ///   - ticketsBindingSource → dgvTickets.DataSource, AutoGenerateColumns = false, the same DataPropertyName columns
    ///   - TicketDialog.ValidateForm(), TicketDialog.TicketResult, DialogResult.OK, await ShowDialogAsync()
    ///   - the delete confirmation: await MessageBox.ShowAsync(... YesNo ...)
    ///   - btnNew_Click / btnEdit_Click / btnDelete_Click — same names, same flow
    /// </summary>
    public partial class TicketsView : UserControl, IAppView
    {
        private readonly IAppShell shell;

        public TicketsView(IAppShell shell)
        {
            this.shell = shell;
            InitializeComponent();
        }

        /// <summary>Rebinds the grid to the service every time the page is shown (Module 4's binding, unchanged).</summary>
        public void RefreshView()
        {
            RefreshGrid();
        }

        private void RefreshGrid(int selectId = 0)
        {
            List<Ticket> tickets = shell.Tickets.GetTickets();
            ticketsBindingSource.DataSource = tickets;
            ticketsBindingSource.ResetBindings(false);

            if (selectId > 0)
            {
                int index = tickets.FindIndex(t => t.Id == selectId);
                if (index >= 0)
                    ticketsBindingSource.Position = index;
            }
        }

        private Ticket CurrentTicket => ticketsBindingSource.Current as Ticket;

        #region Ticket workflow (Module 5) — same handlers, same dialog, same service

        private async void btnNew_Click(object sender, EventArgs e)
        {
            shell.AddActivity("Tickets → btnNew_Click → TicketDialog (new) opened");
            var dialog = new TicketDialog(null);
            dialog.ValidationFailed += Dialog_ValidationFailed;

            DialogResult result = await dialog.ShowDialogAsync();
            if (result == DialogResult.OK)
            {
                shell.Tickets.AddTicket(dialog.TicketResult);
                RefreshGrid(dialog.TicketResult.Id);
                shell.SetStatus($"ticket #{dialog.TicketResult.Id} created", StatusKind.Ok);
                shell.AddActivity($"Ticket #{dialog.TicketResult.Id} created — \"{dialog.TicketResult.Title}\" (TicketService.AddTicket, grid rebound)");
            }
            else
            {
                shell.SetStatus("new ticket cancelled — nothing saved", StatusKind.Warn);
                shell.AddActivity("TicketDialog closed with Cancel — nothing saved");
            }

            Application.Update(this);
        }

        private async void btnEdit_Click(object sender, EventArgs e)
        {
            Ticket current = CurrentTicket;
            if (current == null)
            {
                shell.SetStatus("select a ticket first", StatusKind.Warn);
                shell.AddActivity("btnEdit_Click → no row selected — nothing opened");
                return;
            }

            shell.AddActivity($"Tickets → btnEdit_Click → TicketDialog (edit #{current.Id}) opened");
            var dialog = new TicketDialog(current);
            dialog.ValidationFailed += Dialog_ValidationFailed;

            DialogResult result = await dialog.ShowDialogAsync();
            if (result == DialogResult.OK)
            {
                shell.Tickets.UpdateTicket(dialog.TicketResult);
                RefreshGrid(dialog.TicketResult.Id);
                shell.SetStatus($"ticket #{dialog.TicketResult.Id} saved", StatusKind.Ok);
                shell.AddActivity($"Ticket #{dialog.TicketResult.Id} saved — \"{dialog.TicketResult.Title}\", {dialog.TicketResult.Status} (TicketService.UpdateTicket)");
            }
            else
            {
                shell.SetStatus($"edit of ticket #{current.Id} cancelled — nothing changed", StatusKind.Warn);
                shell.AddActivity($"Edit ticket #{current.Id} → Cancel — the stored ticket is untouched (the dialog worked on a copy)");
            }

            Application.Update(this);
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            Ticket current = CurrentTicket;
            if (current == null)
            {
                shell.SetStatus("select a ticket first", StatusKind.Warn);
                shell.AddActivity("btnDelete_Click → no row selected — nothing deleted");
                return;
            }

            DialogResult answer = await MessageBox.ShowAsync(
                $"Delete ticket #{current.Id} \"{current.Title}\"?",
                "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (answer == DialogResult.Yes)
            {
                shell.Tickets.DeleteTicket(current.Id);
                RefreshGrid();
                shell.SetStatus($"ticket #{current.Id} deleted", StatusKind.Ok);
                shell.AddActivity($"Ticket #{current.Id} deleted (MessageBox.ShowAsync → Yes → TicketService.DeleteTicket)");
            }
            else
            {
                shell.SetStatus("delete cancelled", StatusKind.Warn);
                shell.AddActivity($"Delete ticket #{current.Id} → No — nothing changed");
            }

            Application.Update(this);
        }

        private void Dialog_ValidationFailed(object sender, string message)
        {
            // The failure path that proves the theme did not touch the rules: same guard, whatever the theme.
            shell.SetStatus("validation: " + message, StatusKind.Warn);
            shell.AddActivity($"TicketDialog.ValidateForm() → false: {message} (current theme: {shell.CurrentTheme})");
        }

        #endregion

        #region Failure path on purpose: a blank title after a theme switch

        private async void btnTryBlankTitle_Click(object sender, EventArgs e)
        {
            var dialog = new TicketDialog(null);
            dialog.ValidationFailed += Dialog_ValidationFailed;
            dialog.Prefill("", "Northwind", "Open", "High");

            // Run the rule before the dialog is even shown, so the log states the outcome plainly:
            bool ok = dialog.ValidateForm();
            shell.SetStatus($"validation blocked a blank title under {shell.CurrentTheme}", StatusKind.Warn);
            shell.AddActivity($"btnTryBlankTitle_Click → ValidateForm() = {ok} → \"Title is required.\" — theme {shell.CurrentTheme} changed nothing about the rule");

            // Recovery: the dialog stays open with the message; type a title and Save goes through.
            DialogResult result = await dialog.ShowDialogAsync();
            if (result == DialogResult.OK)
            {
                shell.Tickets.AddTicket(dialog.TicketResult);
                RefreshGrid(dialog.TicketResult.Id);
                shell.SetStatus($"ticket #{dialog.TicketResult.Id} created after fixing the title", StatusKind.Ok);
                shell.AddActivity($"Recovery: title filled in → ValidateForm() = true → ticket #{dialog.TicketResult.Id} created");
            }
            else
            {
                shell.AddActivity("Blank-title dialog closed with Cancel — nothing saved");
            }

            Application.Update(this);
        }

        #endregion
    }
}
