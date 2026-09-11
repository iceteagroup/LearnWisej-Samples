using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using TicketOps.Controls;
using TicketOps.Domain;
using TicketOps.Infrastructure;
using TicketOps.Resources;
using TicketOps.Services;
using Wisej.Web;

namespace TicketOps.Views
{
    /// <summary>
    /// TicketOps Console · Work Orders: a data-bound grid with search and status filter, a master-detail
    /// editor and Save / Discard with dirty tracking.
    ///
    /// The binding contract, in one picture:
    ///
    ///   BindingList&lt;WorkOrder&gt; _visibleOrders ──► workOrderSource (BindingSource) ──► dgvWorkOrders (complex binding)
    ///                                                        └──► textTitle.Text, textAssignedTo.Text,        (simple bindings
    ///                                                             dateDue.Value, numericCost.Value             to the CURRENT item)
    ///
    /// The grid and the detail fields share one BindingSource, so clicking a row IS selecting the record the
    /// editor works on — there is no "copy the row into the fields" code. The fields push their value into
    /// the WorkOrder as soon as it changes (DataSourceUpdateMode.OnPropertyChanged); the WorkOrder raises
    /// PropertyChanged; the BindingList turns that into ListChanged; the grid repaints the cell. When the
    /// service rolls a row back (RejectChanges), the same chain runs in the other direction.
    ///
    /// What stays manual, and why, is written down in docs/BindingDecisions.md: the two enum combos
    /// (enum ↔ index), the dirty indicator (display of WorkOrder.IsDirty) and the filter (the BindingList is
    /// rebuilt from the service's answer; BindingSource.Filter is not honoured by a BindingList).
    /// </summary>
    public partial class WorkOrdersPage : Form
    {
        private static readonly Color DirtyColor = Color.FromArgb(214, 122, 0);
        private static readonly Color OverdueBack = Color.FromArgb(253, 232, 232);
        private static readonly Color OverdueFore = Color.FromArgb(192, 57, 43);
        private static readonly Color UnassignedFore = Color.FromArgb(154, 167, 180);

        private readonly IWorkOrderService _workOrders;
        private readonly ILog _log;

        /// <summary>Every loaded object (the master list). Filtering never removes anything from here.</summary>
        private readonly List<WorkOrder> _allOrders = new List<WorkOrder>();

        /// <summary>The rows on screen: the BindingSource's DataSource. Same object instances as the master list.</summary>
        private readonly BindingList<WorkOrder> _visibleOrders = new BindingList<WorkOrder>();

        private bool _loaded;          // filters are ignored until the first load has run
        private bool _fillingDetail;   // the enum combos are being set from the current item, not by the user

        // The Designer keeps the parameterless constructor; real wiring goes through the other one.
        public WorkOrdersPage() : this(null, new ActivityLog())
        {
        }

        public WorkOrdersPage(IWorkOrderService workOrders, ILog log)
        {
            InitializeComponent();

            _workOrders = workOrders;
            _log = log;

            this.comboStatusFilter.Items.AddRange(new object[] { "All statuses", "Open", "Scheduled", "In Progress", "Closed" });
            this.comboStatusFilter.SelectedIndex = 0;
            this.comboStatus.Items.AddRange(new object[] { "Open", "Scheduled", "In Progress", "Closed" });
            this.comboPriority.Items.AddRange(new object[] { "Low", "Medium", "High" });

            InitializeBinding();
        }

        #region Binding configuration

        /// <summary>
        /// Wires the list, the BindingSource and the controls together — once. From here on nothing in
        /// this file copies a value between a control and a WorkOrder.
        /// </summary>
        private void InitializeBinding()
        {
            // 1. A change-announcing list of change-announcing objects. ListChanged also drives the dirty indicator.
            _visibleOrders.ListChanged += visibleOrders_ListChanged;

            // 2. The coordination point every control binds through.
            this.workOrderSource.DataSource = _visibleOrders;

            // 3. The grid shows the whole list (complex binding). Columns are declared in the Designer by
            //    DataPropertyName, so nothing internal (IsDirty, IsOverdue) leaks into the grid.
            this.dgvWorkOrders.AutoGenerateColumns = false;
            this.dgvWorkOrders.DataSource = this.workOrderSource;

            // 4. The detail fields show — and edit — the CURRENT item (simple, two-way bindings).
            //    OnPropertyChanged: the value reaches the WorkOrder as soon as the control reports a change,
            //    which is what makes the grid row follow the editor live.
            this.textTitle.DataBindings.Add("Text", this.workOrderSource, nameof(WorkOrder.Title), true, DataSourceUpdateMode.OnPropertyChanged);
            this.textAssignedTo.DataBindings.Add("Text", this.workOrderSource, nameof(WorkOrder.AssignedTo), true, DataSourceUpdateMode.OnPropertyChanged);
            this.dateDue.DataBindings.Add("Value", this.workOrderSource, nameof(WorkOrder.DueDate), true, DataSourceUpdateMode.OnPropertyChanged);
            this.numericCost.DataBindings.Add("Value", this.workOrderSource, nameof(WorkOrder.Cost), true, DataSourceUpdateMode.OnPropertyChanged);
        }

        #endregion

        #region Load

        private async void WorkOrdersPage_Load(object sender, EventArgs e)
        {
            await LoadAsync();
        }

        /// <summary>Data → UI: the only place that fills the master list. The grid is filled by the binding.</summary>
        private async Task LoadAsync()
        {
            try
            {
                this.statusBanner.SetStatus("loading", StatusKind.Busy);
                var rows = await _workOrders.LoadAsync();

                _allOrders.Clear();
                _allOrders.AddRange(rows);
                _loaded = true;

                ApplyFilter();
            }
            catch (Exception ex)
            {
                ReportFailure("WorkOrdersPage.LoadAsync", ex);
            }
        }

        #endregion

        #region Search / filter (through the service; the master list is never thrown away)

        private WorkOrderQuery ReadQueryFromForm()
        {
            int i = this.comboStatusFilter.SelectedIndex;
            WorkOrderStatus? status = i <= 0 ? (WorkOrderStatus?)null : (WorkOrderStatus)(i - 1);
            return new WorkOrderQuery(this.textSearch.Text, status);
        }

        private void ApplyFilter()
        {
            if (!_loaded)
                return;

            var query = ReadQueryFromForm();
            var matches = _workOrders.Filter(_allOrders, query);

            RebindVisible(matches);
            this.labelCount.Text = $"{matches.Count} of {_allOrders.Count} work orders";

            if (matches.Count == 0 && _allOrders.Count > 0)
            {
                // An empty result is a normal outcome: say so, in grey, and leave the editor disabled.
                this.statusBanner.ShowBanner(Strings.NoMatches, StatusKind.Normal);
                this.statusBanner.SetStatus("no matches", StatusKind.Normal);
            }
            else
            {
                this.statusBanner.HideBanner();
                this.statusBanner.SetStatus(query.IsEmpty ? $"{matches.Count} work orders" : $"{matches.Count} match", StatusKind.Success);
            }
        }

        /// <summary>
        /// Rebuilds the bound list from the service's answer. The objects are the same instances as in the
        /// master list, so a row with unsaved changes keeps them when it is filtered out and back in.
        /// </summary>
        private void RebindVisible(IReadOnlyList<WorkOrder> rows)
        {
            _visibleOrders.RaiseListChangedEvents = false;
            _visibleOrders.Clear();
            foreach (var o in rows)
                _visibleOrders.Add(o);
            _visibleOrders.RaiseListChangedEvents = true;
            _visibleOrders.ResetBindings();          // one Reset for the grid instead of one event per Add

            RefreshDetail();
        }

        private void textSearch_TextChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        private void comboStatusFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        #endregion

        #region Master-detail: the BindingSource's current item drives the editor

        /// <summary>
        /// The bound current item, or null. BindingSource.Current throws IndexOutOfRangeException while the list
        /// is being reset or has no rows, instead of returning null, so the position is checked first.
        /// </summary>
        private WorkOrder Current
        {
            get
            {
                if (this.workOrderSource.Count == 0 || this.workOrderSource.Position < 0)
                    return null;
                try { return this.workOrderSource.Current as WorkOrder; }
                catch (IndexOutOfRangeException) { return null; }
            }
        }

        /// <summary>
        /// Fires when the current item moves — a row click, an arrow key, a filter that changed the list.
        /// The bound fields re-read themselves; this handler only does the extras: header, enum combos, dirty state.
        /// </summary>
        private void workOrderSource_CurrentChanged(object sender, EventArgs e)
        {
            RefreshDetail();
        }

        private void RefreshDetail()
        {
            var current = Current;
            _fillingDetail = true;
            try
            {
                this.panelDetail.Enabled = current != null;
                if (current == null)
                {
                    this.labelDetailTitle.Text = Strings.NoSelection;
                    this.comboStatus.SelectedIndex = -1;
                    this.comboPriority.SelectedIndex = -1;
                }
                else
                {
                    this.labelDetailTitle.Text = $"Work Order {current.Id}";
                    this.comboStatus.SelectedIndex = (int)current.Status;
                    this.comboPriority.SelectedIndex = (int)current.Priority;
                }
            }
            finally
            {
                _fillingDetail = false;
            }

            UpdateDirtyState();
        }

        /// <summary>UI → object by hand (the one conversion the binding does not do for us: enum ↔ index).</summary>
        private void comboStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            var current = Current;
            if (_fillingDetail || current == null || this.comboStatus.SelectedIndex < 0)
                return;
            current.Status = (WorkOrderStatus)this.comboStatus.SelectedIndex;    // PropertyChanged → grid cell + IsDirty
        }

        private void comboPriority_SelectedIndexChanged(object sender, EventArgs e)
        {
            var current = Current;
            if (_fillingDetail || current == null || this.comboPriority.SelectedIndex < 0)
                return;
            current.Priority = (WorkOrderPriority)this.comboPriority.SelectedIndex;
        }

        #endregion

        #region Dirty indicator

        /// <summary>BindingList&lt;T&gt; turns every item PropertyChanged into ListChanged(ItemChanged); keep the dirty indicator honest.</summary>
        private void visibleOrders_ListChanged(object sender, ListChangedEventArgs e)
        {
            UpdateDirtyState();
        }

        /// <summary>Display only: WorkOrder.IsDirty decides, the label and the two buttons show it.</summary>
        private void UpdateDirtyState()
        {
            var current = Current;
            bool dirty = current != null && current.IsDirty;
            int dirtyElsewhere = _allOrders.Count(o => o.IsDirty && !ReferenceEquals(o, current));

            this.labelDirty.Text = dirty
                ? (dirtyElsewhere > 0 ? $"{Strings.UnsavedChanges} (+{dirtyElsewhere} more)" : Strings.UnsavedChanges)
                : (dirtyElsewhere > 0 ? $"● {dirtyElsewhere} unsaved elsewhere" : "");
            this.buttonSave.Enabled = dirty;
            this.buttonDiscard.Enabled = dirty;
        }

        #endregion

        #region Grid formatting (UI layer — the WorkOrder holds values, never display strings)

        /// <summary>
        /// Cost and DueDate are formatted by the columns' DefaultCellStyle.Format ("C2", "MMM d") — no code.
        /// The rest is here: enum text and colour, "Unassigned", the overdue tint, the unsaved amber title.
        /// Fires per visible cell on every repaint: map values only, never call a service here.
        /// </summary>
        private void dgvWorkOrders_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= this.workOrderSource.Count || e.ColumnIndex < 0 || e.CellStyle == null)
                return;

            var order = this.workOrderSource[e.RowIndex] as WorkOrder;
            if (order == null)
                return;

            switch (this.dgvWorkOrders.Columns[e.ColumnIndex].DataPropertyName)
            {
                case nameof(WorkOrder.Title):
                    if (order.IsDirty)
                        e.CellStyle.ForeColor = DirtyColor;                 // unsaved edits show amber in the grid too
                    break;

                case nameof(WorkOrder.Status):
                    e.Value = StatusText(order.Status);
                    e.CellStyle.ForeColor = StatusColor(order.Status);
                    e.FormattingApplied = true;
                    break;

                case nameof(WorkOrder.Priority):
                    e.Value = order.Priority.ToString();
                    e.CellStyle.ForeColor = PriorityColor(order.Priority);
                    e.FormattingApplied = true;
                    break;

                case nameof(WorkOrder.AssignedTo):
                    if (string.IsNullOrWhiteSpace(e.Value as string))
                    {
                        e.Value = "Unassigned";
                        e.CellStyle.ForeColor = UnassignedFore;
                        e.FormattingApplied = true;
                    }
                    break;

                case nameof(WorkOrder.DueDate):
                    if (order.IsOverdue)                                     // needs Status too — read the row object, not just e.Value
                    {
                        e.CellStyle.BackColor = OverdueBack;
                        e.CellStyle.ForeColor = OverdueFore;
                    }
                    break;
            }
        }

        private static string StatusText(WorkOrderStatus status) => status switch
        {
            WorkOrderStatus.InProgress => "In Progress",
            _ => status.ToString()
        };

        private static Color StatusColor(WorkOrderStatus status) => status switch
        {
            WorkOrderStatus.Open => Color.FromArgb(11, 106, 230),
            WorkOrderStatus.Scheduled => Color.FromArgb(106, 125, 146),
            WorkOrderStatus.InProgress => Color.FromArgb(185, 119, 14),
            _ => Color.FromArgb(31, 138, 76)
        };

        private static Color PriorityColor(WorkOrderPriority priority) => priority switch
        {
            WorkOrderPriority.High => Color.FromArgb(192, 57, 43),
            WorkOrderPriority.Medium => Color.FromArgb(185, 119, 14),
            _ => Color.FromArgb(31, 138, 76)
        };

        #endregion

        #region Data → UI (show the result) and failures

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
        private void ReportFailure(string source, Exception ex, string safeMessage = null)
        {
            safeMessage = safeMessage ?? Strings.ActionFailed;
            _log.Error(LogLayer.UI, source, ex);
            this.statusBanner.ShowBanner("✖ " + safeMessage, StatusKind.Error);
            this.statusBanner.SetStatus("failed", StatusKind.Error);
            AlertBox.Show(safeMessage, MessageBoxIcon.Error,
                alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        #endregion

        #region Handlers: Save / Discard / Reload

        private async void buttonSave_Click(object sender, EventArgs e)
        {
            try
            {
                var current = Current;
                if (current == null)
                    return;

                this.workOrderSource.EndEdit();                                  // flush a pending bound value into the object
                var result = await _workOrders.SaveAsync(current);               // validate → persist → AcceptChanges, all in the service
                ShowResult(result);                                              // IsDirty already notified: indicator and amber title clear themselves
            }
            catch (Exception ex)
            {
                // The store failed before AcceptChanges ran: the object is untouched and still dirty, so the edits stay on screen.
                ReportFailure("WorkOrdersPage.buttonSave_Click", ex, Strings.SaveFailedEditsKept);
            }
        }

        private void buttonDiscard_Click(object sender, EventArgs e)
        {
            try
            {
                var current = Current;
                if (current == null)
                    return;

                ShowResult(_workOrders.Discard(current));
            }
            catch (Exception ex)
            {
                ReportFailure("WorkOrdersPage.buttonDiscard_Click", ex);
            }
        }

        private async void buttonReload_Click(object sender, EventArgs e)
        {
            try
            {
                if (_allOrders.Any(o => o.IsDirty))
                {
                    // Guard the unsaved edit: a reload would replace the objects and lose them silently.
                    ShowResult(OperationResult<WorkOrder>.Fail(Strings.ReloadBlockedByUnsaved));
                    return;
                }

                await LoadAsync();
            }
            catch (Exception ex)
            {
                ReportFailure("WorkOrdersPage.buttonReload_Click", ex);
            }
        }

        #endregion
    }
}
