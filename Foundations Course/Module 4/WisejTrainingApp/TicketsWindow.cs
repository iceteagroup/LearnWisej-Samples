using System;
using System.Collections.Generic;
using System.Globalization;
using Wisej.Web;
using WisejTrainingApp.Models;
using WisejTrainingApp.Services;

namespace WisejTrainingApp
{
    /// <summary>
    /// Ticket Management — Module 4 lab window (data binding, DataGridView and the service layer;
    /// SplitContainer, Dock and AutoSize).
    ///
    /// Header (Dock Top):   lblPageTitle on the left, lblStatus on the right ("Selected ticket #1", "Saved ticket #1", …).
    /// splitContainer1:     Dock Fill, vertical. Panel1 = dgvTickets (Dock Fill) over the "State" card;
    ///                      Panel2 = grpTicketDetails (Dock Top) over the event-log card (Dock Fill).
    /// Binding:             one BindingSource. The grid and every detail control bind to it; the selection
    ///                      follows CurrentChanged. Save goes through TicketService, then ResetBindings(false)
    ///                      refreshes the screen on purpose. Refresh reloads from the service on purpose.
    /// </summary>
    public partial class TicketsWindow : Form
    {
        // One service and one BindingSource per user session — instance fields, never static.
        private TicketService ticketService = new TicketService();
        private BindingSource ticketsBindingSource = new BindingSource();

        private int sampleCounter;

        public TicketsWindow()
        {
            // InitializeComponent() builds every control from TicketsWindow.Designer.cs — the Designer owns that file.
            InitializeComponent();
        }

        private void TicketsWindow_Load(object sender, EventArgs e)
        {
            AddLog("Program.Main → new TicketsWindow().Show()");
            AddLog("InitializeComponent() built splitContainer1 (Dock Fill, Vertical, SplitterDistance 760), dgvTickets, grpTicketDetails");

            // Load first: a Binding resolves its member ("Title") against the BindingSource's current list,
            // so the DataSource must be set before DataBindings.Add — otherwise Wisej.NET throws
            // "Cannot bind to the property or column Title on the DataSource" (verified at runtime).
            LoadTickets();          // load once …
            BindDetailControls();   // … then bind once; later changes are refreshed intentionally
        }

        #region The BindingSource pattern (lesson s16 §4–§5)

        /// <summary>
        /// The lesson's LoadTickets(): the list from the service goes into the BindingSource, the grid points at
        /// the BindingSource. Called on load and again from Refresh — the detail bindings are untouched by it.
        /// </summary>
        private void LoadTickets()
        {
            ticketsBindingSource.DataSource = ticketService.GetTickets();
            dgvTickets.DataSource = ticketsBindingSource;

            AddLog($"LoadTickets → ticketsBindingSource.DataSource = ticketService.GetTickets() ({ticketService.Count} tickets) → dgvTickets.DataSource = ticketsBindingSource");
            UpdateStateBox();
        }

        /// <summary>The selected record is whatever the BindingSource says is current — not a grid cell.</summary>
        private Ticket SelectedTicket
        {
            get { return ticketsBindingSource.Current as Ticket; }
        }

        /// <summary>
        /// Each detail control binds to the same BindingSource, so it follows the current ticket automatically.
        /// OnPropertyChanged pushes an edit into the Ticket object as soon as the control reports it — the object
        /// changes (business state), the service does not (persisted data) until Save.
        /// </summary>
        private void BindDetailControls()
        {
            txtTitle.DataBindings.Add("Text", ticketsBindingSource, "Title", true, DataSourceUpdateMode.OnPropertyChanged);
            txtCustomer.DataBindings.Add("Text", ticketsBindingSource, "Customer", true, DataSourceUpdateMode.OnPropertyChanged);
            cmbStatus.DataBindings.Add("Text", ticketsBindingSource, "Status", true, DataSourceUpdateMode.OnPropertyChanged);
            cmbPriority.DataBindings.Add("Text", ticketsBindingSource, "Priority", true, DataSourceUpdateMode.OnPropertyChanged);
            txtAssignedTo.DataBindings.Add("Text", ticketsBindingSource, "AssignedTo", true, DataSourceUpdateMode.OnPropertyChanged);
            dtpCreated.DataBindings.Add("Value", ticketsBindingSource, "CreatedDate", true, DataSourceUpdateMode.OnPropertyChanged);
            txtDescription.DataBindings.Add("Text", ticketsBindingSource, "Description", true, DataSourceUpdateMode.OnPropertyChanged);

            // Selection follows the grid through the BindingSource — one event, no grid-cell code.
            ticketsBindingSource.CurrentChanged += ticketsBindingSource_CurrentChanged;

            AddLog("BindDetailControls → 7 DataBindings on ticketsBindingSource (Text ×6, Value ×1), DataSourceUpdateMode.OnPropertyChanged");
        }

        private void ticketsBindingSource_CurrentChanged(object sender, EventArgs e)
        {
            Ticket ticket = SelectedTicket;
            if (ticket == null)
            {
                SetStatus("No ticket selected", StatusKind.Warn);
                AddLog("ticketsBindingSource.CurrentChanged → Current is null");
            }
            else
            {
                // A small UI event stays small: a label and a log line. No service calls in here.
                lblStatus.Text = "Selected ticket #" + ticket.Id;
                SetStatus(lblStatus.Text, StatusKind.Ok);
                AddLog($"ticketsBindingSource.CurrentChanged → Position {ticketsBindingSource.Position} → Selected ticket #{ticket.Id} \"{ticket.Title}\"");
            }

            UpdateStateBox();
        }

        #endregion

        #region Save, refresh, failure, recovery

        /// <summary>
        /// The lab's Save handler: read the current ticket, call the service, refresh the bindings, update the
        /// status label. The service owns the rule (Title required, Status in the allowed set); the UI only
        /// decides how to show its answer.
        /// </summary>
        private void btnSaveTicket_Click(object sender, EventArgs e)
        {
            Ticket ticket = ticketsBindingSource.Current as Ticket;

            try
            {
                ticketService.SaveTicket(ticket);
                ticketsBindingSource.ResetBindings(false);
                lblStatus.Text = "Saved ticket #" + ticket.Id;
                SetStatus(lblStatus.Text, StatusKind.Ok);

                AddLog($"btnSaveTicket_Click → ticketService.SaveTicket(#{ticket.Id} \"{ticket.Title}\", {ticket.Status}) ✓ → ResetBindings(false) → lblStatus = \"{lblStatus.Text}\"");
                AlertBox.Show($"Saved ticket #{ticket.Id}.", MessageBoxIcon.Information, alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 3000);
            }
            catch (ArgumentException ex)
            {
                // Failure path: the service refused; nothing was persisted, the bound object keeps the bad value.
                SetStatus("Not saved — " + ex.Message, StatusKind.Error);
                AddLog($"btnSaveTicket_Click → ticketService.SaveTicket threw ArgumentException: \"{ex.Message}\" → nothing persisted");
                AlertBox.Show(ex.Message, MessageBoxIcon.Warning, alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
                txtTitle.Focus();
            }

            UpdateStateBox();
        }

        /// <summary>Refresh on purpose: reload from the service and reselect the same ticket. Unsaved edits are dropped.</summary>
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            int selectedId = SelectedTicket?.Id ?? 0;

            LoadTickets();
            SelectTicket(selectedId);

            SetStatus("Grid refreshed from TicketService", StatusKind.Ok);
            AddLog("btnRefresh_Click → Grid refreshed from TicketService (unsaved edits discarded, new tickets now visible)");
        }

        /// <summary>
        /// Failure path: blank the Title through the bound textbox, then run the very same Save handler.
        /// Recovery: type a title and click Save Ticket, or click Refresh to reload the saved copy.
        /// </summary>
        private void btnSimulateBadSave_Click(object sender, EventArgs e)
        {
            Ticket ticket = SelectedTicket;
            if (ticket == null)
            {
                SetStatus("Select a ticket first", StatusKind.Warn);
                return;
            }

            string previousTitle = ticket.Title;
            txtTitle.Text = "";

            bool pushedByBinding = string.IsNullOrEmpty(ticket.Title);
            if (pushedByBinding)
            {
                AddLog("btnSimulateBadSave_Click → txtTitle.Text = \"\" → the OnPropertyChanged binding pushed a blank Title into the Ticket object");
            }
            else
            {
                // A server-side Text change did not reach the object through the binding; do what a user's edit would have done.
                ticket.Title = "";
                AddLog("btnSimulateBadSave_Click → txtTitle.Text = \"\" (binding did not push a server-side change) → ticket.Title = \"\" set directly");
            }

            AddLog("btnSimulateBadSave_Click → calling btnSaveTicket_Click — same handler, same service rule");
            btnSaveTicket_Click(sender, e);

            AddLog($"recovery → type a title (it was \"{previousTitle}\") and click Save Ticket, or click Refresh to reload the saved copy");
        }

        /// <summary>
        /// A change made behind the screen's back: the service gets a new ticket, but the grid is bound to the list
        /// loaded earlier and cannot know. That is the point of "bind once, refresh intentionally".
        /// </summary>
        private void btnAddSample_Click(object sender, EventArgs e)
        {
            string[] customers = { "Northwind", "Contoso", "Fabrikam", "Adventure Works", "Tailspin", "Wide World Importers" };
            string customer = customers[sampleCounter++ % customers.Length];

            var ticket = new Ticket
            {
                Title = $"Follow-up call with {customer}",
                Customer = customer,
                Status = "Open",
                Priority = "Medium",
                AssignedTo = "Unassigned",
                Description = "Added through ticketService.AddTicket() — the grid did not see it until Refresh.",
            };

            ticketService.AddTicket(ticket);

            SetStatus($"Ticket #{ticket.Id} added in the service — the grid still shows {dgvTickets.Rows.Count} rows; click Refresh", StatusKind.Warn);
            AddLog($"btnAddSample_Click → ticketService.AddTicket → #{ticket.Id} persisted ({ticketService.Count} in the service); grid still bound to the earlier list ({dgvTickets.Rows.Count} rows) → click Refresh");
            UpdateStateBox();
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            lstEventLog.Items.Clear();
        }

        #endregion

        #region UI state vs business state vs persisted data (lesson s16 §7)

        /// <summary>
        /// Three lines, one per state type, recomputed after every action:
        /// what the screen shows, what the bound object holds, what the service has saved.
        /// </summary>
        private void UpdateStateBox()
        {
            Ticket ticket = SelectedTicket;
            int rows = dgvTickets.Rows.Count;

            string ui = ticket == null
                ? $"no row selected of {rows} · lblStatus = \"{lblStatus.Text}\""
                : $"row {ticketsBindingSource.Position + 1} of {rows} selected · lblStatus = \"{lblStatus.Text}\"";

            string business = ticket == null
                ? "no current Ticket object"
                : $"Ticket #{ticket.Id} Status = \"{ticket.Status}\" · Title = \"{Shorten(ticket.Title, 28)}\" · unsaved edits: {DescribeChanges(ticket)}";

            string persisted = $"TicketService holds {ticketService.Count} tickets";
            if (ticket != null)
            {
                Ticket saved = ticketService.GetTicket(ticket.Id);
                persisted += saved == null
                    ? $" · #{ticket.Id} not saved yet"
                    : $" · #{saved.Id} Status = \"{saved.Status}\" · Title = \"{Shorten(saved.Title, 28)}\" (saved copy)";
            }

            lblState.Text = $"UI state       : {ui}\nBusiness state : {business}\nPersisted data : {persisted}";
        }

        /// <summary>Which fields of the bound object differ from the service's saved copy.</summary>
        private string DescribeChanges(Ticket edited)
        {
            Ticket saved = ticketService.GetTicket(edited.Id);
            if (saved == null)
                return "not persisted yet";

            var changed = new List<string>();
            if (edited.Title != saved.Title) changed.Add("Title");
            if (edited.Customer != saved.Customer) changed.Add("Customer");
            if (edited.Status != saved.Status) changed.Add("Status");
            if (edited.Priority != saved.Priority) changed.Add("Priority");
            if (edited.AssignedTo != saved.AssignedTo) changed.Add("AssignedTo");
            if (edited.CreatedDate.Date != saved.CreatedDate.Date) changed.Add("CreatedDate");
            if (edited.Description != saved.Description) changed.Add("Description");

            return changed.Count == 0 ? "none" : string.Join(", ", changed);
        }

        #endregion

        #region Helpers (small, reusable — the habit the course teaches)

        private void SelectTicket(int id)
        {
            if (id == 0)
                return;

            var tickets = ticketsBindingSource.DataSource as List<Ticket>;
            int index = tickets == null ? -1 : tickets.FindIndex(t => t.Id == id);
            if (index >= 0)
                ticketsBindingSource.Position = index;
        }

        private static string Shorten(string text, int max)
        {
            if (string.IsNullOrEmpty(text))
                return "";
            return text.Length <= max ? text : text.Substring(0, max - 1) + "…";
        }

        private enum StatusKind { Ok, Warn, Error }

        private void SetStatus(string text, StatusKind kind)
        {
            lblStatus.Text = text;
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
