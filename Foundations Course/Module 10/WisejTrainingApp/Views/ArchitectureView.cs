using System;
using System.Collections.Generic;

namespace WisejTrainingApp.Views
{
    /// <summary>
    /// Architecture: the data-flow list from lesson s46 §3, mapped to the real files of this project, and a
    /// one-job-per-layer explanation. Select a layer to read what it does and what it must not do.
    /// </summary>
    public partial class ArchitectureView : HelpdeskView
    {
        private static readonly List<(string Layer, string Job, string File)> Layers = new List<(string, string, string)>
        {
            ("Program.cs -> starts Window1",
             "The session entry point. Default.json names Program.Main; Main does new Window1().Show() and nothing else.",
             "Program.cs"),
            ("Window1.cs -> shell, navigation, pages",
             "Draws the frame (header, nav, content, status bar), swaps screens in NavigateTo(), keeps the activity log. No business logic — it does not know what a valid ticket is.",
             "Window1.cs · Window1.Designer.cs"),
            ("Ticket.cs -> ticket data model",
             "Plain properties: Id, Title, Customer, Status, Priority, AssignedTo, CreatedDate, Description. No behaviour, so grid, dialog, validator and repository all share it.",
             "Models/Ticket.cs · Models/Customer.cs"),
            ("TicketDialog -> create/edit form + validation",
             "One Form for New and Edit. Save calls ValidateForm(), which asks TicketValidator; OK only when valid. It never calls the service — the screen that opened it does.",
             "Dialogs/TicketDialog.cs · Services/TicketValidator.cs"),
            ("TicketService -> CRUD logic and a fake repository",
             "GetTickets / AddTicket / UpdateTicket / DeleteTicket / SaveTicket / GetSummary. Validates again before storing, then talks to TicketRepository — the only class that owns the list.",
             "Services/TicketService.cs · Services/TicketRepository.cs"),
            ("DataGridView -> displays tickets",
             "dgvTickets binds through a BindingSource; RefreshTicketGrid() re-reads from the service after every change. The grid never edits data in place.",
             "Views/TicketsView.cs"),
            ("Theme -> polished UI",
             "cboTheme in the header calls Application.LoadTheme(name); every screen restyles live. Spacing numbers are fixed in one comment in Window1.cs.",
             "Window1.cs (cboTheme_SelectedIndexChanged) · Default.json \"theme\""),
            ("Deployment -> release checklist",
             "The Deployment screen's required checklist (lesson s42 §4) and lblPackageStatus, plus docs/DeploymentChecklist.md and docs/ReadinessNote.md.",
             "Views/DeploymentView.cs · docs/DeploymentChecklist.md"),
        };

        public ArchitectureView(IHelpdeskShell shell)
            : base(shell)
        {
            InitializeComponent();

            lblDataFlow.Text = string.Join("\n", new[]
            {
                "Program.cs    -> starts Window1",
                "Window1.cs    -> shell, navigation, pages",
                "Ticket.cs     -> ticket data model",
                "TicketDialog  -> create/edit form + validation",
                "TicketService -> CRUD logic and a fake repository",
                "DataGridView  -> displays tickets",
                "Theme         -> polished UI",
                "Deployment    -> release checklist",
            });

            foreach (var layer in Layers)
                lstLayers.Items.Add(layer.Layer);
        }

        public override void ActivateScreen()
        {
            if (lstLayers.SelectedIndex < 0 && lstLayers.Items.Count > 0)
                lstLayers.SelectedIndex = 0;
        }

        private void lstLayers_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = lstLayers.SelectedIndex;
            if (index < 0 || index >= Layers.Count)
                return;

            var layer = Layers[index];
            lblLayerTitle.Text = layer.Layer;
            lblLayerDetail.Text = layer.Job;
            lblLayerFile.Text = "Where: " + layer.File;
            ShowStatus(lblStatus, $"layer {index + 1} of {Layers.Count} — {layer.File}", StatusKind.Ok);
        }
    }
}
