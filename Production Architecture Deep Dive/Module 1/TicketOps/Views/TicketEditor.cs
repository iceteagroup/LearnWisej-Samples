using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TicketOps.Controls;
using TicketOps.Diagnostics;
using TicketOps.Domain;
using TicketOps.Infrastructure;
using TicketOps.Resources;
using TicketOps.Services;
using Wisej.Web;

namespace TicketOps.Views
{
    /// <summary>
    /// TicketOps Console · Open Tickets: the ticket grid and the editor.
    /// Handlers read the form, ask <see cref="ITicketService"/>, and show the result.
    /// </summary>
    public partial class TicketEditor : Form
    {
        private readonly ITicketService _tickets;
        private readonly ILog _log;

        private IReadOnlyList<Ticket> _rows = new List<Ticket>();

        // The Designer keeps the parameterless constructor; real wiring goes through the other one.
        public TicketEditor() : this(null, new ActivityLog())
        {
        }

        public TicketEditor(ITicketService tickets, ILog log)
        {
            InitializeComponent();

            _tickets = tickets;
            _log = log;

            this.comboPriority.Items.AddRange(new object[] { "Low", "Medium", "High" });
            this.comboPriority.SelectedIndex = 1;
        }

        private async void TicketEditor_Load(object sender, EventArgs e)
        {
            await RefreshGridAsync();
        }

        private async Task RefreshGridAsync()
        {
            try
            {
                this.statusBanner.SetStatus("loading", StatusKind.Busy);
                _rows = await _tickets.GetOpenTicketsAsync();

                this.gridTickets.Rows.Clear();
                foreach (var t in _rows)
                    this.gridTickets.Rows.Add(t.Id, t.Title, t.Priority.ToString(), t.Status.ToString(), t.HoursLogged);

                this.labelCount.Text = $"{_rows.Count} open tickets";
                this.statusBanner.HideBanner();
                this.statusBanner.SetStatus("Ticket list refreshed.", StatusKind.Success);
            }
            catch (Exception ex)
            {
                ReportFailure("TicketEditor.RefreshGrid", ex);
            }
        }

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
            }
            else
            {
                this.statusBanner.ShowBanner(result.Message, StatusKind.Warning);
                this.statusBanner.SetStatus("not saved", StatusKind.Warning);
            }
        }

        /// <summary>Unexpected failure: the details go to the log, the user sees one safe sentence.</summary>
        private void ReportFailure(string source, Exception ex)
        {
            _log.Error(LogLayer.UI, source, ex);
            this.statusBanner.ShowBanner("✖ " + Strings.ActionFailed, StatusKind.Error);
            this.statusBanner.SetStatus("failed", StatusKind.Error);
            AlertBox.Show(Strings.ActionFailed, MessageBoxIcon.Error,
                alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

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
        }

        private async void buttonSave_Click(object sender, EventArgs e)
        {
            try
            {
                var draft = ReadDraftFromForm();
                var result = await _tickets.SaveAsync(draft);
                ShowResult(result);
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
            await RefreshGridAsync();
        }
    }
}
