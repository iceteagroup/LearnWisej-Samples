using System;
using System.Collections.Generic;
using TicketOps.Controls;
using TicketOps.Data;
using TicketOps.Domain;
using TicketOps.Infrastructure;
using TicketOps.Resources;
using TicketOps.Services;
using Wisej.Web;

namespace TicketOps.Views
{
    /// <summary>
    /// TicketOps Console · Open Tickets — the Module 1 screen after the refactor.
    ///
    /// Left card:   the ticket grid and the editor (display + input only).
    /// Right card:  the Diagnostics activity trace: every click travels UI → SVC → DATA and back.
    /// Bottom bar:  the progress path (load 200 tickets), two failure paths (validation, domain rule),
    ///              the error path (simulated data outage) with its recovery, and Clear trace.
    ///
    /// Every handler reads like a sentence: read the form, ask the service, show the result.
    /// The words "Open", "INSERT" and "connection" do not appear in this file.
    /// Handlers that await are "async void", so each owns its try/catch: an exception is logged with
    /// its details and the user sees the safe message from Resources.Strings.
    /// </summary>
    public partial class TicketEditor : Form
    {
        private readonly ITicketService _tickets;
        private readonly InMemoryTicketRepository _repository;   // only for the lab's outage switch
        private readonly ILog _log;

        private IReadOnlyList<Ticket> _rows = new List<Ticket>();
        private int _bulkRemaining;

        // The Designer keeps the parameterless constructor; real wiring goes through the other one.
        public TicketEditor() : this(null, null, new ActivityLog())
        {
        }

        public TicketEditor(ITicketService tickets, InMemoryTicketRepository repository, ILog log)
        {
            InitializeComponent();

            _tickets = tickets;
            _repository = repository;
            _log = log;

            if (log is ActivityLog activityLog)
                this.tracePanel.Attach(activityLog);

            this.comboPriority.Items.AddRange(new object[] { "Low", "Medium", "High" });
            this.comboPriority.SelectedIndex = 1;
        }

        #region Screen lifecycle

        private async void TicketEditor_Load(object sender, EventArgs e)
        {
            _log.Info(LogLayer.UI, "TicketEditor.Load", "screen shown → ITicketService.GetOpenTicketsAsync()");
            await RefreshGridAsync();
        }

        /// <summary>data → UI: the only place that fills the grid.</summary>
        private async System.Threading.Tasks.Task RefreshGridAsync()
        {
            try
            {
                this.statusBanner.SetStatus("loading", StatusKind.Busy);
                _rows = await _tickets.GetOpenTicketsAsync();

                this.gridTickets.Rows.Clear();
                foreach (var t in _rows)
                    this.gridTickets.Rows.Add(t.Id, t.Title, t.Priority.ToString(), t.Status.ToString(), t.HoursLogged);

                this.labelCount.Text = $"{_rows.Count} open tickets";
                this.statusBanner.SetStatus("ready", StatusKind.Success);
                _log.Info(LogLayer.UI, "TicketEditor.RefreshGrid", $"{_rows.Count} rows shown");
            }
            catch (Exception ex)
            {
                ReportFailure("TicketEditor.RefreshGrid", ex);
            }
        }

        #endregion

        #region UI → data (read the form) and data → UI (show the result)

        private TicketDraft ReadDraftFromForm()
        {
            return new TicketDraft
            {
                Id = SelectedTicketId(),
                Title = this.textTitle.Text,
                Priority = (TicketPriority)this.comboPriority.SelectedIndex,
                HoursLogged = (double)this.numericHours.Value
            };
        }

        private int? SelectedTicketId()
        {
            var row = this.gridTickets.CurrentRow;
            if (row == null || row.Index < 0 || row.Index >= _rows.Count)
                return null;
            return _rows[row.Index].Id;
        }

        private void ShowResult<T>(OperationResult<T> result)
        {
            if (result.Succeeded)
            {
                this.statusBanner.HideBanner();
                this.statusBanner.SetStatus(result.Message, StatusKind.Success);
                _log.Info(LogLayer.UI, "TicketEditor.ShowResult", $"OK · {result.Message}");
            }
            else
            {
                // Expected outcome: the service explained it in words the user may read.
                this.statusBanner.ShowBanner(result.Message, StatusKind.Warning);
                this.statusBanner.SetStatus("not saved", StatusKind.Warning);
                _log.Warn(LogLayer.UI, "TicketEditor.ShowResult", $"FAIL · {result.Message}");
            }
        }

        /// <summary>
        /// Unexpected failure: details go to the log (with the exception type and message),
        /// the user sees one generic sentence. Nothing internal leaks through the banner.
        /// </summary>
        private void ReportFailure(string source, Exception ex)
        {
            _log.Error(LogLayer.UI, source, ex, $"caught {ex.GetType().Name} — user sees the safe message");
            this.statusBanner.ShowBanner("✖ " + Strings.ActionFailed, StatusKind.Error);
            this.statusBanner.SetStatus("failed", StatusKind.Error);
            AlertBox.Show(Strings.ActionFailed, MessageBoxIcon.Error,
                alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        #endregion

        #region Thin handlers

        private void gridTickets_SelectionChanged(object sender, EventArgs e)
        {
            int? id = SelectedTicketId();
            if (id == null)
            {
                this.labelSelected.Text = "New ticket";
                return;
            }

            var t = _rows[this.gridTickets.CurrentRow.Index];
            this.textTitle.Text = t.Title;
            this.comboPriority.SelectedIndex = (int)t.Priority;
            this.numericHours.Value = (decimal)t.HoursLogged;
            this.labelSelected.Text = $"Editing #{t.Id} · {t.Status}";
        }

        private void buttonNew_Click(object sender, EventArgs e)
        {
            this.gridTickets.ClearSelection();
            this.gridTickets.CurrentCell = null;
            this.textTitle.Text = "";
            this.comboPriority.SelectedIndex = 1;
            this.numericHours.Value = 0;
            this.labelSelected.Text = "New ticket";
            this.textTitle.Focus();
            _log.Info(LogLayer.UI, "TicketEditor.buttonNew_Click", "form cleared (display only, no service call)");
        }

        private async void buttonSave_Click(object sender, EventArgs e)
        {
            try
            {
                var draft = ReadDraftFromForm();                                     // UI → data
                _log.Info(LogLayer.UI, "TicketEditor.buttonSave_Click", $"→ ITicketService.SaveAsync {draft}");
                var result = await _tickets.SaveAsync(draft);                       // the decision lives in the service
                ShowResult(result);                                                  // data → UI
                if (result.Succeeded)
                    await RefreshGridAsync();
            }
            catch (Exception ex)
            {
                ReportFailure("TicketEditor.buttonSave_Click", ex);
            }
        }

        private async void buttonClose_Click(object sender, EventArgs e)
        {
            try
            {
                int? id = SelectedTicketId();
                if (id == null)
                {
                    ShowResult(OperationResult<Ticket>.Fail("Select a ticket to close."));
                    return;
                }

                _log.Info(LogLayer.UI, "TicketEditor.buttonClose_Click", $"→ ITicketService.CloseAsync(#{id})");
                var result = await _tickets.CloseAsync(id.Value);
                ShowResult(result);
                if (result.Succeeded)
                    await RefreshGridAsync();
            }
            catch (Exception ex)
            {
                ReportFailure("TicketEditor.buttonClose_Click", ex);
            }
        }

        private async void buttonRefresh_Click(object sender, EventArgs e)
        {
            _log.Info(LogLayer.UI, "TicketEditor.buttonRefresh_Click", "→ ITicketService.GetOpenTicketsAsync()");
            await RefreshGridAsync();
        }

        #endregion

        #region Bottom bar: progress, failures, outage and recovery

        /// <summary>Progress path: a Timer saves ten generated tickets per tick through the same service.</summary>
        private void buttonBulk_Click(object sender, EventArgs e)
        {
            if (this.timerBulk.Enabled)
                return;

            _bulkRemaining = 200;
            this.progressBulk.Value = 0;
            this.progressBulk.Visible = true;
            this.statusBanner.HideBanner();
            this.statusBanner.SetStatus("loading 200 tickets", StatusKind.Busy);
            _log.Info(LogLayer.UI, "TicketEditor.buttonBulk_Click", "200 saves in batches of 10 per tick — the handler stays thin, the Timer paces it");
            this.timerBulk.Start();
        }

        private async void timerBulk_Tick(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < 10 && _bulkRemaining > 0; i++, _bulkRemaining--)
                {
                    int n = 200 - _bulkRemaining + 1;
                    var draft = new TicketDraft
                    {
                        Title = $"Bulk import ticket {n:000}",
                        Priority = (TicketPriority)(n % 3),
                        HoursLogged = n % 4 == 0 ? 0.5 : 0
                    };
                    var result = await _tickets.SaveAsync(draft);
                    if (!result.Succeeded)
                        throw new InvalidOperationException("Generated draft was rejected: " + result.Message);
                }

                this.progressBulk.Value = 200 - _bulkRemaining;
                this.statusBanner.SetStatus($"loading {200 - _bulkRemaining}/200", StatusKind.Busy);

                if (_bulkRemaining == 0)
                {
                    this.timerBulk.Stop();
                    this.progressBulk.Visible = false;
                    _log.Info(LogLayer.UI, "TicketEditor.timerBulk_Tick", "bulk load complete → refresh");
                    await RefreshGridAsync();
                }
            }
            catch (Exception ex)
            {
                this.timerBulk.Stop();
                this.progressBulk.Visible = false;
                ReportFailure("TicketEditor.timerBulk_Tick", ex);
            }
        }

        /// <summary>Failure path 1: validation. The service rejects it; nothing is persisted; the user reads why.</summary>
        private async void buttonSaveEmpty_Click(object sender, EventArgs e)
        {
            try
            {
                var draft = new TicketDraft { Id = null, Title = "   ", Priority = TicketPriority.Medium, HoursLogged = 0 };
                _log.Info(LogLayer.UI, "TicketEditor.buttonSaveEmpty_Click", $"→ ITicketService.SaveAsync {draft}");
                ShowResult(await _tickets.SaveAsync(draft));
            }
            catch (Exception ex)
            {
                ReportFailure("TicketEditor.buttonSaveEmpty_Click", ex);
            }
        }

        /// <summary>Failure path 2: a domain rule. #1042 has no hours logged, so Ticket.CanClose says no.</summary>
        private async void buttonCloseNoHours_Click(object sender, EventArgs e)
        {
            try
            {
                const int ticketWithoutHours = 1042;
                _log.Info(LogLayer.UI, "TicketEditor.buttonCloseNoHours_Click", $"→ ITicketService.CloseAsync(#{ticketWithoutHours})");
                ShowResult(await _tickets.CloseAsync(ticketWithoutHours));
            }
            catch (Exception ex)
            {
                ReportFailure("TicketEditor.buttonCloseNoHours_Click", ex);
            }
        }

        /// <summary>Error path + recovery: toggle the repository outage, then refresh through the service.</summary>
        private async void buttonOutage_Click(object sender, EventArgs e)
        {
            if (_repository == null)
                return;

            _repository.SimulateOutage = !_repository.SimulateOutage;
            this.buttonOutage.Text = _repository.SimulateOutage ? "Recover the data store" : "Simulate data outage";
            _log.Info(LogLayer.UI, "TicketEditor.buttonOutage_Click",
                _repository.SimulateOutage ? "outage ON → refresh (expect ✖ in DATA, safe message in UI)" : "outage OFF → refresh (recovery)");
            await RefreshGridAsync();
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            this.tracePanel.ClearTrace();
            this.statusBanner.HideBanner();
            this.statusBanner.SetStatus("ready", StatusKind.Normal);
        }

        #endregion
    }
}
