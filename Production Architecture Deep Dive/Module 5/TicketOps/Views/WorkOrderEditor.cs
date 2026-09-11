using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TicketOps.Controls;
using TicketOps.Domain;
using TicketOps.Infrastructure;
using TicketOps.Resources;
using TicketOps.Services;
using TicketOps.Validation;
using Wisej.Web;

namespace TicketOps.Views
{
    /// <summary>
    /// TicketOps Console · Work Orders: the work order grid and the editor. Field errors are painted by an
    /// ErrorProvider next to the offending control; the summary panel lists every problem.
    ///
    /// The Save handler: build the command, pre-check it with the same validator the server uses, hand it to
    /// the service, show the result.
    /// </summary>
    public partial class WorkOrderEditor : Form
    {
        private readonly IWorkOrderService _workOrders;
        private readonly WorkOrderValidator _validator;
        private readonly SessionContext _session;
        private readonly ILog _log;

        private IReadOnlyList<WorkOrder> _rows = new List<WorkOrder>();
        private WorkOrderStatus _fromStatus = WorkOrderStatus.New;   // the status the editor was opened with
        private bool _loadingGrid;

        // The Designer keeps the parameterless constructor; real wiring goes through the other one.
        public WorkOrderEditor() : this(null, new WorkOrderValidator(), new SessionContext(), new ActivityLog())
        {
        }

        public WorkOrderEditor(IWorkOrderService workOrders, WorkOrderValidator validator, SessionContext session, ILog log)
        {
            InitializeComponent();

            _workOrders = workOrders;
            _validator = validator;
            _session = session;
            _log = log;

            this.comboStatus.Items.AddRange(Enum.GetNames(typeof(WorkOrderStatus)));
            this.comboPriority.Items.AddRange(Enum.GetNames(typeof(WorkOrderPriority)));
            this.comboRole.Items.AddRange(Enum.GetNames(typeof(UserRole)));
            this.comboRole.SelectedIndex = (int)_session.Role;
            this.comboStatus.SelectedIndex = 0;
            this.comboPriority.SelectedIndex = 1;
            this.dateDue.Value = DateTime.Today.AddDays(7);
        }

        private async void WorkOrderEditor_Load(object sender, EventArgs e)
        {
            await RefreshGridAsync();
            SelectRow(2002);                                          // the order the video edits
        }

        /// <summary>data → UI: the only place that fills the grid. Keeps the current selection; never touches the editor.</summary>
        private async Task RefreshGridAsync()
        {
            try
            {
                int? keep = SelectedWorkOrderId();
                this.statusBanner.SetStatus("loading", StatusKind.Busy);
                _rows = await _workOrders.GetWorkOrdersAsync();

                _loadingGrid = true;
                this.gridOrders.Rows.Clear();
                foreach (var o in _rows)
                    this.gridOrders.Rows.Add(o.Id, o.Title, o.AssigneeId, o.Status.ToString(),
                        o.DueDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                        o.EstimatedCost.ToString("N2", CultureInfo.InvariantCulture),
                        o.EstimatedHours.ToString("0.#", CultureInfo.InvariantCulture));
                if (!keep.HasValue || !SelectRowCore(keep.Value))
                {
                    // No row was selected before (New work order): make sure the grid does not pick one for us.
                    this.gridOrders.ClearSelection();
                    this.gridOrders.CurrentCell = null;
                }
                _loadingGrid = false;

                this.labelCount.Text = $"{_rows.Count} work orders";
                this.statusBanner.SetStatus("ready", StatusKind.Success);
            }
            catch (Exception ex)
            {
                _loadingGrid = false;
                ReportFailure("WorkOrderEditor.RefreshGrid", ex, Strings.ActionFailed);
            }
        }

        #region UI → command (read the editor) and result → UI (show it)

        /// <summary>Collect + map. No control reference leaks past this method.</summary>
        private SaveWorkOrderCommand BuildSaveCommandFromEditor()
        {
            return new SaveWorkOrderCommand
            {
                Id = SelectedWorkOrderId(),
                Title = this.textTitle.Text,
                AssigneeId = this.textAssignee.Text,
                Priority = (WorkOrderPriority)Math.Max(0, this.comboPriority.SelectedIndex),
                DueDate = this.dateDue.Value,
                EstimatedCost = this.numericCost.Value,
                EstimatedHours = (double)this.numericHours.Value,
                FromStatus = _fromStatus,
                ToStatus = (WorkOrderStatus)Math.Max(0, this.comboStatus.SelectedIndex)
            };
        }

        private int? SelectedWorkOrderId()
        {
            var row = this.gridOrders.CurrentRow;
            if (row == null || row.Index < 0 || row.Index >= _rows.Count)
                return null;
            return _rows[row.Index].Id;
        }

        private void FillEditor(WorkOrder o)
        {
            ClearErrors();
            this.textTitle.Text = o.Title;
            this.textAssignee.Text = o.AssigneeId;
            this.comboPriority.SelectedIndex = (int)o.Priority;
            this.dateDue.Value = o.DueDate;
            this.numericCost.Value = o.EstimatedCost;
            this.numericHours.Value = (decimal)o.EstimatedHours;
            this.comboStatus.SelectedIndex = (int)o.Status;
            _fromStatus = o.Status;
            this.labelSelected.Text = $"Editing work order {o.Id} · {o.Status}";
        }

        /// <summary>
        /// Translate a ValidationResult into on-screen feedback: a glyph + tooltip beside each failing
        /// control (ErrorProvider) and the summary panel listing every problem, so cross-field and
        /// workflow errors are never hidden behind a single field. Display only — nothing is decided here.
        /// </summary>
        private void ShowValidation(ValidationResult result)
        {
            ClearErrors();

            foreach (var error in result.FieldErrors)
            {
                var control = ControlForField(error.Field);
                if (control != null)
                    this.errorProvider.SetError(control, error.Message);
            }

            var lines = result.Errors
                .Select(e => e.IsFieldError ? $"• {CaptionForField(e.Field)} — {e.Message}" : $"• {e.Message}")
                .ToList();
            int shown = Math.Min(lines.Count, 3);
            string more = lines.Count > shown ? $"\n• … and {lines.Count - shown} more" : "";

            this.labelSummaryTitle.Text = result.Count == 1 ? "1 problem needs attention" : $"{result.Count} problems need attention";
            this.labelSummaryList.Text = string.Join("\n", lines.Take(shown)) + more;
            this.panelSummary.Visible = true;

            this.statusBanner.HideBanner();
            this.statusBanner.SetStatus(result.Count == 1 ? "1 problem found — nothing was saved" : $"{result.Count} problems found — nothing was saved", StatusKind.Warning);
        }

        private void ClearErrors()
        {
            this.errorProvider.Clear();
            this.panelSummary.Visible = false;
        }

        private Control ControlForField(string field)
        {
            switch (field)
            {
                case "Title": return this.textTitle;
                case "AssigneeId": return this.textAssignee;
                case "DueDate": return this.dateDue;
                case "EstimatedCost": return this.numericCost;
                case "EstimatedHours": return this.numericHours;
                default: return null;
            }
        }

        private static string CaptionForField(string field)
        {
            switch (field)
            {
                case "AssigneeId": return "Assignee";
                case "DueDate": return "Due date";
                case "EstimatedCost": return "Cost";
                case "EstimatedHours": return "Hours";
                default: return field;
            }
        }

        private void ShowSaveResult(SaveResult result)
        {
            if (result.Succeeded)
            {
                ClearErrors();
                this.statusBanner.HideBanner();
                this.statusBanner.SetStatus(result.Message, StatusKind.Success);
            }
            else
            {
                // Expected outcome: the pipeline explained it in words the user may read.
                ShowValidation(result.Errors);
            }
        }

        /// <summary>
        /// Unexpected failure: the real exception goes to the log; the user sees ONE safe sentence.
        /// The editor keeps the user's edits — nothing is reset.
        /// </summary>
        private void ReportFailure(string source, Exception ex, string safeMessage)
        {
            _log.Error(LogLayer.UI, source, ex);
            this.statusBanner.ShowBanner("✖ " + safeMessage, StatusKind.Error);
            this.statusBanner.SetStatus(safeMessage == Strings.SaveFailed ? Strings.SaveFailedStatus : "failed", StatusKind.Error);
            AlertBox.Show(safeMessage, MessageBoxIcon.Error,
                alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        #endregion

        #region Handlers

        private void gridOrders_SelectionChanged(object sender, EventArgs e)
        {
            if (_loadingGrid)
                return;

            int? id = SelectedWorkOrderId();
            if (id == null)
            {
                this.labelSelected.Text = "New work order";
                return;
            }

            FillEditor(_rows[this.gridOrders.CurrentRow.Index]);
        }

        private void comboRole_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboRole.SelectedIndex < 0)
                return;

            _session.Role = (UserRole)this.comboRole.SelectedIndex;
        }

        private void buttonNew_Click(object sender, EventArgs e)
        {
            this.gridOrders.ClearSelection();
            this.gridOrders.CurrentCell = null;
            ClearErrors();
            this.textTitle.Text = "";
            this.textAssignee.Text = "";
            this.comboPriority.SelectedIndex = 1;
            this.dateDue.Value = DateTime.Today.AddDays(7);
            this.numericCost.Value = 0;
            this.numericHours.Value = 0;
            this.comboStatus.SelectedIndex = (int)WorkOrderStatus.New;
            _fromStatus = WorkOrderStatus.New;
            this.labelSelected.Text = "New work order";
            this.textTitle.Focus();
        }

        /// <summary>
        /// Collect → map → UX pre-check with the shared validator → the service (which re-validates, applies
        /// rules and role, persists atomically) → show the result.
        /// </summary>
        private async void buttonSave_Click(object sender, EventArgs e)
        {
            try
            {
                var command = BuildSaveCommandFromEditor();                                  // UI → command; no control leaks past this line

                var preCheck = _validator.Validate(command);                                // UX: same rules, instant feedback, decides nothing
                if (!preCheck.IsValid)
                {
                    ShowValidation(preCheck);                                                // ErrorProvider glyphs + summary panel
                    return;
                }

                this.statusBanner.SetStatus("saving…", StatusKind.Busy);
                var result = await _workOrders.SaveAsync(command, _session);                // validate → rules → persist → confirm
                ShowSaveResult(result);                                                      // result → UI
                if (result.Succeeded)
                {
                    await RefreshGridAsync();
                    SelectRow(result.WorkOrder.Id);
                }
            }
            catch (Exception ex)
            {
                ReportFailure("WorkOrderEditor.buttonSave_Click", ex, Strings.SaveFailed); // real exception → log; safe message → user; edits stay
            }
        }

        /// <summary>Reload from the store and discard the editor's unsaved values (re-fills from the selected row).</summary>
        private async void buttonRefresh_Click(object sender, EventArgs e)
        {
            await RefreshGridAsync();
            int? id = SelectedWorkOrderId();
            if (id.HasValue)
                SelectRow(id.Value);
            else
                ClearErrors();
        }

        #endregion

        #region Grid selection helpers (display only)

        private void SelectRow(int id)
        {
            _loadingGrid = true;
            bool found = SelectRowCore(id);
            _loadingGrid = false;
            if (found)
                FillEditor(_rows[this.gridOrders.CurrentRow.Index]);
        }

        private bool SelectRowCore(int id)
        {
            for (int i = 0; i < _rows.Count && i < this.gridOrders.Rows.Count; i++)
            {
                if (_rows[i].Id == id)
                {
                    this.gridOrders.CurrentCell = this.gridOrders.Rows[i].Cells[0];
                    return true;
                }
            }
            return false;
        }

        #endregion
    }
}
