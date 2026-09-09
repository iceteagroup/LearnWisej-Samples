using System;
using System.Linq;
using Wisej.Web;
using WisejTrainingApp.Dialogs;
using WisejTrainingApp.Models;

namespace WisejTrainingApp.Views
{
    /// <summary>
    /// Dashboard page — lab step 6 ("Group into cards") and step 9 ("Add recent activity").
    ///
    /// Row 1: four metric cards with large numbers (Open, In Progress, Closed, Customers) computed from TicketService.
    /// Row 2: the "Commands" card (Create / Edit / Close ticket, grouped together) and the "Recent activity" card
    ///        (lstActivity — the course's event log; every navigation, theme change and ticket action lands here
    ///        through the shell's AddActivity).
    /// Spacing follows LayoutRules: 32 px page padding, cards 24 px apart, 250 px metric cards, 524 px wide cards.
    /// </summary>
    public partial class DashboardView : UserControl, IAppView
    {
        private readonly IAppShell shell;

        public DashboardView(IAppShell shell)
        {
            this.shell = shell;
            InitializeComponent();
        }

        /// <summary>Recomputes the metric numbers from the service — nothing is stored in the view.</summary>
        public void RefreshView()
        {
            var tickets = shell.Tickets.GetTickets();
            lblOpenValue.Text = tickets.Count(t => t.Status == "Open").ToString();
            lblInProgressValue.Text = tickets.Count(t => t.Status == "In Progress").ToString();
            lblClosedValue.Text = tickets.Count(t => t.Status == "Closed").ToString();
            lblCustomersValue.Text = tickets.Select(t => t.Customer).Distinct().Count().ToString();
        }

        /// <summary>Called by the shell's AddActivity — the list keeps growing across pages because this view is created once.</summary>
        public void AppendActivity(string line)
        {
            lstActivity.Items.Add(line);
            lstActivity.SelectedIndex = lstActivity.Items.Count - 1;
        }

        #region Commands card — the same TicketService and TicketDialog the Tickets page uses

        private async void btnCreateTicket_Click(object sender, EventArgs e)
        {
            shell.AddActivity("Dashboard → btnCreateTicket_Click → TicketDialog (new) opened");
            var dialog = new TicketDialog(null);
            dialog.ValidationFailed += (s, message) =>
            {
                shell.SetStatus("validation: " + message, StatusKind.Warn);
                shell.AddActivity($"TicketDialog.ValidateForm() → false: {message} (theme: {shell.CurrentTheme})");
            };

            DialogResult result = await dialog.ShowDialogAsync();
            if (result == DialogResult.OK)
            {
                shell.Tickets.AddTicket(dialog.TicketResult);
                RefreshView();
                shell.SetStatus($"ticket #{dialog.TicketResult.Id} created", StatusKind.Ok);
                shell.AddActivity($"Ticket #{dialog.TicketResult.Id} created — \"{dialog.TicketResult.Title}\" (TicketService.AddTicket, metric cards refreshed)");
            }
            else
            {
                shell.AddActivity("TicketDialog closed with Cancel — nothing saved");
            }

            Application.Update(this);
        }

        private void btnEditTicket_Click(object sender, EventArgs e)
        {
            // Editing needs a selected row — that lives in the ticket queue, so the command takes the user there.
            shell.AddActivity("Dashboard → btnEditTicket_Click → NavigateTo(\"Tickets\") to pick a ticket in the queue");
            shell.NavigateTo("Tickets");
            shell.SetStatus("select a ticket in the queue, then click Edit", StatusKind.Warn);
        }

        private async void btnCloseTicket_Click(object sender, EventArgs e)
        {
            Ticket next = shell.Tickets.GetTickets().FirstOrDefault(t => t.Status != "Closed");
            if (next == null)
            {
                shell.SetStatus("nothing to close — every ticket is already Closed", StatusKind.Warn);
                shell.AddActivity("btnCloseTicket_Click → no open ticket left");
                return;
            }

            DialogResult answer = await MessageBox.ShowAsync(
                $"Close ticket #{next.Id} \"{next.Title}\" ({next.Customer})?",
                "Close ticket", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (answer == DialogResult.Yes)
            {
                Ticket updated = next.Clone();
                updated.Status = "Closed";
                shell.Tickets.UpdateTicket(updated);
                RefreshView();
                shell.SetStatus($"ticket #{next.Id} closed", StatusKind.Ok);
                shell.AddActivity($"Ticket #{next.Id} closed (TicketService.UpdateTicket, Status = Closed) — metric cards refreshed");
            }
            else
            {
                shell.AddActivity($"Close ticket #{next.Id} → No — nothing changed");
            }

            Application.Update(this);
        }

        private void btnClearActivity_Click(object sender, EventArgs e)
        {
            lstActivity.Items.Clear();
        }

        #endregion
    }
}
