using System;
using System.Collections.Generic;
using System.Linq;
using TicketOps.Controls;
using TicketOps.Domain;
using TicketOps.Infrastructure;
using TicketOps.Resources;
using TicketOps.Services;
using Wisej.Services;
using Wisej.Web;

namespace TicketOps.Views
{
    /// <summary>
    /// TicketOps Console · Ticket Workflow.
    ///
    /// The Form is created by Program.Main with "new" and never sees a constructor argument: its five
    /// services arrive through [Inject] properties filled from Application.Services. It hands them to a
    /// plain TicketWorkflowPresenter and becomes a translator: read the controls, call the presenter, render
    /// the WorkflowResult. Nothing in this file decides whether a ticket may close — that is the presenter's job.
    /// </summary>
    public partial class TicketWorkflow : Form
    {
        // ---- property injection: the framework creates the Form, so it cannot take these by constructor ----

        [Inject(Required = true)]
        public ITicketService Tickets { get; set; }

        [Inject(Required = true)]
        public IUserService Users { get; set; }

        [Inject(Required = true)]
        public IPermissionService Permissions { get; set; }

        [Inject(Required = true)]
        public INotificationService Notifications { get; set; }

        [Inject(Required = true)]
        public IAuditLogService Audit { get; set; }

        [Inject]
        public ILog Log { get; set; }

        private ILog _log;
        private TicketWorkflowPresenter _presenter;
        private IReadOnlyList<Ticket> _rows = new List<Ticket>();
        private IReadOnlyList<Operator> _operators = new List<Operator>();
        private bool _syncingOperator;

        /// <summary>The Designer (and the container) use the parameterless constructor; there is no other one.</summary>
        public TicketWorkflow()
        {
            InitializeComponent();
        }

        private async void TicketWorkflow_Load(object sender, EventArgs e)
        {
            try
            {
                EnsureInjected();
                _presenter = new TicketWorkflowPresenter(Tickets, Users, Permissions, Notifications, Audit, _log);

                FillOperators();
                await RefreshGridAsync();
            }
            catch (Exception ex)
            {
                ReportFailure("TicketWorkflow.Load", ex);
            }
        }

        /// <summary>
        /// Wisej.NET injects top-level containers (Form, Page, Desktop) automatically. If that has not happened
        /// by Load for a Form created with "new", ask the container explicitly.
        /// </summary>
        private void EnsureInjected()
        {
            bool injected = Tickets != null && Users != null && Permissions != null
                            && Notifications != null && Audit != null && Log != null;
            if (!injected)
                Application.Services.Inject(this);

            _log = Log ?? new ActivityLog();

            if (Tickets == null || Users == null || Permissions == null || Notifications == null || Audit == null)
                throw new InvalidOperationException("A required service is not registered: check ServiceRegistration.");
        }

        /// <summary>data → UI: the only place that fills the ticket grid.</summary>
        private async System.Threading.Tasks.Task RefreshGridAsync()
        {
            try
            {
                this.statusBanner.SetStatus("loading", StatusKind.Busy);
                _rows = await _presenter.LoadOpenTicketsAsync();

                this.gridTickets.Rows.Clear();
                foreach (var t in _rows)
                    this.gridTickets.Rows.Add(t.Id, t.Title, t.Priority.ToString(), t.Status.ToString(), t.HoursLogged, AssigneeName(t));

                this.labelSelected.Text = "Select a ticket";
                this.statusBanner.SetStatus("ready", StatusKind.Success);
            }
            catch (Exception ex)
            {
                ReportFailure("TicketWorkflow.RefreshGrid", ex);
            }
        }

        private void FillOperators()
        {
            _operators = Users.AllOperators();
            _syncingOperator = true;
            this.comboOperator.Items.Clear();
            foreach (var o in _operators)
                this.comboOperator.Items.Add(o.ToString());
            this.comboOperator.SelectedIndex = IndexOfOperator(Users.Current.Id);
            _syncingOperator = false;
        }

        #region UI → data and data → UI

        private int? SelectedTicketId()
        {
            var row = this.gridTickets.CurrentRow;
            if (row == null || row.Index < 0 || row.Index >= _rows.Count)
                return null;
            return _rows[row.Index].Id;
        }

        private string AssigneeName(Ticket t)
        {
            if (!t.IsAssigned) return "—";
            var op = _operators.FirstOrDefault(o => o.Id == t.AssigneeId);
            return op != null ? op.Name : $"operator {t.AssigneeId}";
        }

        private int IndexOfOperator(int operatorId)
        {
            for (int i = 0; i < _operators.Count; i++)
                if (_operators[i].Id == operatorId)
                    return i;
            return -1;
        }

        private void ShowResult(WorkflowResult result)
        {
            if (result.Succeeded)
            {
                this.statusBanner.HideBanner();
                this.statusBanner.SetStatus(result.Message, StatusKind.Success);
                return;
            }

            // An expected "no": the presenter explained it in words the user may read.
            var kind = result.Outcome == WorkflowOutcome.Denied ? StatusKind.Error : StatusKind.Warning;
            this.statusBanner.ShowBanner(result.Message, kind);
            this.statusBanner.SetStatus(result.Outcome.ToString().ToLowerInvariant(), StatusKind.Warning);
        }

        /// <summary>Unexpected failure: the details go to the log, the user sees one safe sentence.</summary>
        private void ReportFailure(string source, Exception ex)
        {
            (_log ?? Log)?.Error(LogLayer.UI, source, ex);
            this.statusBanner.ShowBanner("✖ " + Strings.ActionFailed, StatusKind.Error);
            this.statusBanner.SetStatus("failed", StatusKind.Error);
            AlertBox.Show(Strings.ActionFailed, MessageBoxIcon.Error,
                alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        #endregion

        #region Thin handlers → presenter

        private void gridTickets_SelectionChanged(object sender, EventArgs e)
        {
            int? id = SelectedTicketId();
            if (id == null)
            {
                this.labelSelected.Text = "Select a ticket";
                return;
            }

            var t = _rows[this.gridTickets.CurrentRow.Index];
            this.labelSelected.Text = $"#{t.Id} · {t.Title} · {t.Status} · {AssigneeName(t)}";
        }

        private void comboOperator_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_syncingOperator || this.comboOperator.SelectedIndex < 0)
                return;

            try
            {
                var op = _operators[this.comboOperator.SelectedIndex];
                Users.SignInAs(op.Id);
                this.statusBanner.HideBanner();
                this.statusBanner.SetStatus($"signed in as {op.Name}", StatusKind.Normal);
            }
            catch (Exception ex)
            {
                ReportFailure("TicketWorkflow.comboOperator_SelectedIndexChanged", ex);
            }
        }

        /// <summary>Build the request, ask the presenter, show the message.</summary>
        private async void buttonClose_Click(object sender, EventArgs e)
        {
            try
            {
                int? id = SelectedTicketId();
                if (id == null)
                {
                    ShowResult(WorkflowResult.Invalid(Strings.SelectTicket));
                    return;
                }

                var result = await _presenter.CloseAsync(id.Value, this.textReason.Text);   // the decision lives in the presenter
                ShowResult(result);
                if (result.Succeeded)
                {
                    this.textReason.Text = "";
                    await RefreshGridAsync();
                }
            }
            catch (Exception ex)
            {
                ReportFailure("TicketWorkflow.buttonClose_Click", ex);
            }
        }

        private async void buttonAssign_Click(object sender, EventArgs e)
        {
            try
            {
                int? id = SelectedTicketId();
                if (id == null)
                {
                    ShowResult(WorkflowResult.Invalid(Strings.SelectTicket));
                    return;
                }

                var result = await _presenter.AssignToMeAsync(id.Value);
                ShowResult(result);
                if (result.Succeeded)
                    await RefreshGridAsync();
            }
            catch (Exception ex)
            {
                ReportFailure("TicketWorkflow.buttonAssign_Click", ex);
            }
        }

        private async void buttonRefresh_Click(object sender, EventArgs e)
        {
            await RefreshGridAsync();
        }

        #endregion
    }
}
