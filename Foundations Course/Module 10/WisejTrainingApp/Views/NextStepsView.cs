using System;
using System.Collections.Generic;

namespace WisejTrainingApp.Views
{
    /// <summary>
    /// Next Steps: what a production-ready version of this helpdesk still needs (lesson s47 §3). Each item
    /// names what exists today, what would replace it, and which file changes — so the list is a plan, not a wish.
    /// </summary>
    public partial class NextStepsView : HelpdeskView
    {
        private static readonly List<(string Title, string Today, string Production, string Touches)> Steps = new List<(string, string, string, string)>
        {
            ("Real database",
             "TicketRepository keeps a List<Ticket> per session; data is gone when the session ends and two agents never see the same tickets.",
             "An EF Core DbContext (SQL Server / PostgreSQL) behind the same repository methods; migrations for the Ticket and Customer tables; async queries.",
             "Services/TicketRepository.cs is replaced; TicketService and every screen stay as they are."),
            ("Authentication",
             "lblUser says \"Support Agent\" and every command is available to everyone.",
             "A sign-in page or an identity provider (Entra ID, Auth0); Application.Session holds the user and roles; commands like Delete and the Deployment screen are gated server-side by role, not only hidden.",
             "Program.cs (login before Window1), Window1.cs (lblUser, role checks), TicketService (per-command authorization)."),
            ("Logging destination",
             "lstActivity and lstJobLog show what happened in this browser tab only.",
             "Structured logging (Serilog / ILogger) to a file, Seq or Application Insights, with timestamps, session id and user; no secrets or personal data in the lines.",
             "Window1.AddActivity and JobsView.LogError forward to ILogger; a logging section in Default.json / appsettings."),
            ("Tests",
             "TicketValidator and TicketService are testable but untested.",
             "xUnit project: validator rules (blank title, 81-character title, Closed without assignee), service behaviour (Add assigns Ids, Update rejects unknown Ids, GetSummary counts), CustomerService duplicates.",
             "A new WisejTrainingApp.Tests project referencing Services/ and Models/ — no UI needed."),
            ("CI / CD",
             "Built and run by hand with dotnet run.",
             "A pipeline that restores, builds, runs the tests, publishes with -c Release, and deploys to a staging slot; the Deployment checklist becomes pipeline steps.",
             "A workflow file (GitHub Actions / Azure Pipelines); Default.json \"debug\": false for release; license key from a secret."),
            ("Concurrency & paging",
             "The grid loads every ticket; two agents editing the same ticket overwrite each other silently.",
             "Server-side paging and filtering in the repository; a RowVersion on Ticket so UpdateTicket can detect a stale edit and tell the second agent.",
             "Models/Ticket.cs (RowVersion), TicketRepository (paged queries), TicketsView (page controls, stale-edit message)."),
            ("Audit trail",
             "Only the activity log knows who changed what, and only for one session.",
             "A TicketHistory table written by TicketService on every Add/Update/Delete, shown as a tab in TicketDialog.",
             "Services/TicketService.cs, Dialogs/TicketDialog.cs, a new Models/TicketHistory.cs."),
        };

        public NextStepsView(IHelpdeskShell shell)
            : base(shell)
        {
            InitializeComponent();

            foreach (var step in Steps)
                lstNextSteps.Items.Add(step.Title);
        }

        public override void ActivateScreen()
        {
            if (lstNextSteps.SelectedIndex < 0 && lstNextSteps.Items.Count > 0)
                lstNextSteps.SelectedIndex = 0;
        }

        private void lstNextSteps_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = lstNextSteps.SelectedIndex;
            if (index < 0 || index >= Steps.Count)
                return;

            var step = Steps[index];
            lblStepTitle.Text = step.Title;
            lblStepToday.Text = "Today:  " + step.Today;
            lblStepProduction.Text = "Production:  " + step.Production;
            lblStepTouches.Text = "Touches:  " + step.Touches;
            ShowStatus(lblStatus, $"next step {index + 1} of {Steps.Count}", StatusKind.Ok);
        }
    }
}
