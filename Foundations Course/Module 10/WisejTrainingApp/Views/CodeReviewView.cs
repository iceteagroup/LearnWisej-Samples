using System;
using System.Collections.Generic;
using Wisej.Web;

namespace WisejTrainingApp.Views
{
    /// <summary>
    /// Code Review: the peer-review focus from lesson s47 §2 as a CheckedListBox. Select an item to see where
    /// a reviewer looks in this project; tick it when you have looked. The habits card (s47 §1) lists what
    /// each habit looks like here.
    /// </summary>
    public partial class CodeReviewView : HelpdeskView
    {
        private static readonly List<(string Focus, string Question, string WhereToLook)> ReviewItems = new List<(string, string, string)>
        {
            ("Naming",
             "Do classes, controls and methods clearly describe their job?",
             "Window1.Designer.cs and Views/*.Designer.cs field lists — btnCreateTicket, dgvTickets, lblStatus, txtTitle, cboPriority; not one button1. Methods: NavigateTo, RefreshTicketGrid, ValidateForm, SetJobRunning."),
            ("Separation of concerns",
             "Is ticket logic in a service/validator instead of scattered across UI events?",
             "Services/TicketValidator.cs (all rules), Services/TicketService.cs (CRUD + summary), Services/TicketRepository.cs (the list). Then check Views/TicketsView.cs: every handler is open dialog → call service → refresh → status."),
            ("Navigation",
             "Can the reviewer follow how each page opens?",
             "Window1.cs: BuildScreens() registers eight views once; NavButton_Click reads the button's Tag; NavigateTo(string) is the only place a screen is shown. The Dashboard's activity log records every NavigateTo call."),
            ("Usability",
             "Are buttons placed where users expect them, with clear labels and feedback?",
             "Commands sit top-right of each card in the same order (primary, secondary, danger, neutral). Every action ends in lblStatus (green/amber/red) and an activity line. The spacing numbers are in the Window1.cs header comment."),
            ("Deployment",
             "Are configuration, logging, secrets, themes and release notes checked before review?",
             "Views/DeploymentView.cs (required checklist + lblPackageStatus), docs/DeploymentChecklist.md, docs/ReadinessNote.md. Web.config has an empty license key on purpose; Default.json holds the theme."),
        };

        public CodeReviewView(IHelpdeskShell shell)
            : base(shell)
        {
            InitializeComponent();

            foreach (var item in ReviewItems)
                chkReviewItems.Items.Add($"{item.Focus} — {item.Question}");

            lstHabits.Items.Add("Clear control names     → btnCreateTicket, dgvTickets, lblStatus, txtTitle, cboPriority");
            lstHabits.Items.Add("Small event handlers    → TicketsView handlers: dialog → service → refresh → status");
            lstHabits.Items.Add("Reusable validation     → TicketValidator, shared by New and Edit (and by the service)");
            lstHabits.Items.Add("Intentional refresh     → RefreshTicketGrid() + Shell.TicketsChanged() after save/delete");
            lstHabits.Items.Add("Safe errors             → JobsView: short message on screen, detail in the job log");

            UpdateProgress();
        }

        public override void ActivateScreen()
        {
            if (chkReviewItems.SelectedIndex < 0 && chkReviewItems.Items.Count > 0)
                chkReviewItems.SelectedIndex = 0;
        }

        private void chkReviewItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = chkReviewItems.SelectedIndex;
            if (index < 0 || index >= ReviewItems.Count)
                return;

            var item = ReviewItems[index];
            lblFocusTitle.Text = item.Focus + " — where a reviewer looks in this project";
            lblWhereToLook.Text = item.WhereToLook;
        }

        private void chkReviewItems_AfterItemCheck(object sender, ItemCheckEventArgs e)
        {
            UpdateProgress();
            var item = ReviewItems[e.Index];
            Shell.AddActivity($"Code review: \"{item.Focus}\" {(e.NewValue == CheckState.Checked ? "reviewed" : "unchecked")}");
        }

        private void btnResetReview_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < chkReviewItems.Items.Count; i++)
                chkReviewItems.SetItemChecked(i, false);

            UpdateProgress();
            Shell.AddActivity("Code review checklist reset");
        }

        private void UpdateProgress()
        {
            int done = chkReviewItems.CheckedItems.Count;
            int total = chkReviewItems.Items.Count;

            if (done == total)
                ShowStatus(lblStatus, $"all {total} review points covered — ready to submit", StatusKind.Ok);
            else
                ShowStatus(lblStatus, $"{done} of {total} review points covered", StatusKind.Warn);
        }
    }
}
