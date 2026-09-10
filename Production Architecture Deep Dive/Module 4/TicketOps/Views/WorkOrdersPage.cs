using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
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
    /// TicketOps Console · Work Orders — the Module 4 screen: a data-bound grid with search and status
    /// filter, a master-detail editor and Save / Discard with dirty tracking.
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
    ///
    /// Handlers stay thin — read the screen, call IWorkOrderService, show the result — and every async
    /// handler owns its try/catch: the user never sees an exception, the trace does.
    /// </summary>
    public partial class WorkOrdersPage : Form
    {
        private const int ImportTotal = 60;
        private const int ImportBatch = 5;

        private static readonly Color DirtyColor = Color.FromArgb(214, 122, 0);
        private static readonly Color OverdueBack = Color.FromArgb(253, 232, 232);
        private static readonly Color OverdueFore = Color.FromArgb(192, 57, 43);
        private static readonly Color UnassignedFore = Color.FromArgb(154, 167, 180);

        private readonly IWorkOrderService _workOrders;
        private readonly InMemoryWorkOrderRepository _repository;   // only for the lab's outage switch
        private readonly ILog _log;

        /// <summary>Every loaded object (the master list). Filtering never removes anything from here.</summary>
        private readonly List<WorkOrder> _allOrders = new List<WorkOrder>();

        /// <summary>The rows on screen: the BindingSource's DataSource. Same object instances as the master list.</summary>
        private readonly BindingList<WorkOrder> _visibleOrders = new BindingList<WorkOrder>();

        private bool _loaded;          // filters are ignored until the first load has run
        private bool _fillingDetail;   // the enum combos are being set from the current item, not by the user
        private bool _settingSearch;   // the search box is being set from code (bottom bar), not typed
        private int _importRemaining;
        private int _importNext;

        // The Designer keeps the parameterless constructor; real wiring goes through the other one.
        public WorkOrdersPage() : this(null, null, new ActivityLog())
        {
        }

        public WorkOrdersPage(IWorkOrderService workOrders, InMemoryWorkOrderRepository repository, ILog log)
        {
            InitializeComponent();

            _workOrders = workOrders;
            _repository = repository;
            _log = log;

            if (log is ActivityLog activityLog)
                this.tracePanel.Attach(activityLog);
            this.tracePanel.Title = "Activity trace · UI → Service → Data · binding events";

            this.comboStatusFilter.Items.AddRange(new object[] { "All statuses", "Open", "Scheduled", "In Progress", "Closed" });
            this.comboStatusFilter.SelectedIndex = 0;
            this.comboStatus.Items.AddRange(new object[] { "Open", "Scheduled", "In Progress", "Closed" });
            this.comboPriority.Items.AddRange(new object[] { "Low", "Medium", "High" });

            InitializeBinding();
        }

        #region Binding configuration (the lab's "BindingSource configuration" deliverable)

        /// <summary>
        /// Wires the list, the BindingSource and the controls together — once. From here on nothing in
        /// this file copies a value between a control and a WorkOrder.
        /// </summary>
        private void InitializeBinding()
        {
            // 1. A change-announcing list of change-announcing objects. ListChanged is also what drives the
            //    dirty indicator and the "binding events" lines in the trace.
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

            _log.Info(LogLayer.UI, "WorkOrdersPage.InitializeBinding",
                "BindingList<WorkOrder> → workOrderSource → dgvWorkOrders (AutoGenerateColumns = false, 7 columns by DataPropertyName) + 4 detail bindings (Title, AssignedTo, DueDate, Cost · OnPropertyChanged)");
        }

        #endregion

        #region Screen lifecycle

        private async void WorkOrdersPage_Load(object sender, EventArgs e)
        {
            _log.Info(LogLayer.UI, "WorkOrdersPage.Load", "screen shown → IWorkOrderService.LoadAsync()");
            await LoadAsync("WorkOrdersPage.Load");
        }

        /// <summary>Data → UI: the only place that fills the master list. The grid is filled by the binding.</summary>
        private async Task LoadAsync(string source)
        {
            try
            {
                this.statusBanner.SetStatus("loading", StatusKind.Busy);
                var rows = await _workOrders.LoadAsync();

                _allOrders.Clear();
                _allOrders.AddRange(rows);
                _loaded = true;

                ApplyFilter(source);
            }
            catch (Exception ex)
            {
                ReportFailure(source + " → LoadAsync", ex);
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

        private void ApplyFilter(string source)
        {
            if (!_loaded)
                return;

            var query = ReadQueryFromForm();
            _log.Info(LogLayer.UI, source, $"→ IWorkOrderService.Filter({_allOrders.Count} rows, {query})");
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
            _log.Info(LogLayer.UI, "WorkOrdersPage.RebindVisible", $"BindingList rebuilt with {rows.Count} rows (events off during the rebuild → one ResetBindings) — current: {DescribeCurrent()}");
        }

        private void textSearch_TextChanged(object sender, EventArgs e)
        {
            if (_settingSearch)
                return;
            ApplyFilter("textSearch.TextChanged");
        }

        private void comboStatusFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilter("comboStatusFilter.SelectedIndexChanged");
        }

        #endregion

        #region Master-detail: the BindingSource's current item drives the editor

        /// <summary>
        /// The bound current item, or null. Verified at runtime: BindingSource.Current THROWS IndexOutOfRangeException
        /// ("Index -1 does not have a value") while the list is being reset or has no rows, instead of returning null,
        /// so the position is checked first — a ListChanged(Reset) handler reaches this getter in that very state.
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

        private string DescribeCurrent() => Current == null ? "none" : $"#{Current.Id}";

        /// <summary>
        /// Fires when the current item moves — a row click, an arrow key, a filter that changed the list.
        /// The bound fields re-read themselves; this handler only does the extras: header, enum combos, dirty state.
        /// </summary>
        private void workOrderSource_CurrentChanged(object sender, EventArgs e)
        {
            RefreshDetail();
            var current = Current;
            if (current != null)
                _log.Info(LogLayer.UI, "workOrderSource.CurrentChanged", $"current → #{current.Id} — Title/AssignedTo/DueDate/Cost re-read by their bindings; Status/Priority combos filled by hand (enum ↔ index)");
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

        #region Binding events → dirty indicator + trace

        /// <summary>
        /// BindingList&lt;T&gt; turns every item PropertyChanged into ListChanged(ItemChanged). The grid uses
        /// it to repaint the cell; this screen uses it to keep the dirty indicator honest and to show the
        /// reader, in the trace, that no code copied anything.
        /// </summary>
        private void visibleOrders_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.ItemChanged && e.PropertyDescriptor != null
                && e.NewIndex >= 0 && e.NewIndex < _visibleOrders.Count)
            {
                string property = e.PropertyDescriptor.Name;
                if (property != nameof(WorkOrder.IsDirty) && property != nameof(WorkOrder.IsOverdue))
                {
                    var o = _visibleOrders[e.NewIndex];
                    _log.Info(LogLayer.UI, "BindingList.ListChanged",
                        $"#{o.Id}.{property} PropertyChanged → ItemChanged(row {e.NewIndex}) → grid cell repaints · IsDirty = {o.IsDirty}");
                }
            }

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
                _log.Info(LogLayer.UI, "WorkOrdersPage.ShowResult", $"OK · {result.Message}");
            }
            else
            {
                // Expected outcome: the service explained it in words the user may read.
                this.statusBanner.ShowBanner(result.Message, StatusKind.Warning);
                this.statusBanner.SetStatus("not saved", StatusKind.Warning);
                _log.Warn(LogLayer.UI, "WorkOrdersPage.ShowResult", $"FAIL · {result.Message}");
            }
        }

        /// <summary>
        /// Unexpected failure: details go to the log (with the exception type and message), the user sees
        /// one safe sentence. Nothing internal leaks through the banner.
        /// </summary>
        private void ReportFailure(string source, Exception ex, string safeMessage = null)
        {
            safeMessage = safeMessage ?? Strings.ActionFailed;
            _log.Error(LogLayer.UI, source, ex, $"caught {ex.GetType().Name} — user sees the safe message");
            this.statusBanner.ShowBanner("✖ " + safeMessage, StatusKind.Error);
            this.statusBanner.SetStatus("failed", StatusKind.Error);
            AlertBox.Show(safeMessage, MessageBoxIcon.Error,
                alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        #endregion

        #region Thin handlers: Save / Discard / Reload

        private async void buttonSave_Click(object sender, EventArgs e)
        {
            try
            {
                var current = Current;
                if (current == null)
                    return;

                this.workOrderSource.EndEdit();                                  // BindingSource.EndEdit(): flush a pending bound value into the object
                _log.Info(LogLayer.UI, "WorkOrdersPage.buttonSave_Click", $"workOrderSource.EndEdit() → IWorkOrderService.SaveAsync(#{current.Id})");
                var result = await _workOrders.SaveAsync(current);               // validate → persist → AcceptChanges, all in the service
                ShowResult(result);                                              // IsDirty already notified: indicator and amber title clear themselves
            }
            catch (Exception ex)
            {
                // The store failed before AcceptChanges ran: the object is untouched and still dirty, so the edits stay on screen.
                ReportFailure("WorkOrdersPage.buttonSave_Click", ex, Strings.SaveFailedEditsKept);
                _log.Warn(LogLayer.UI, "WorkOrdersPage.buttonSave_Click", $"edits kept: {DescribeCurrent()} still dirty = {Current?.IsDirty}");
            }
        }

        private void buttonDiscard_Click(object sender, EventArgs e)
        {
            try
            {
                var current = Current;
                if (current == null)
                    return;

                _log.Info(LogLayer.UI, "WorkOrdersPage.buttonDiscard_Click", $"→ IWorkOrderService.Discard(#{current.Id}) — the grid row and the bound fields revert through PropertyChanged");
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
                int dirty = _allOrders.Count(o => o.IsDirty);
                if (dirty > 0)
                {
                    // Guard the unsaved edit: a reload would replace the objects and lose them silently.
                    _log.Warn(LogLayer.UI, "WorkOrdersPage.buttonReload_Click", $"{dirty} row(s) with unsaved changes — reload refused");
                    ShowResult(OperationResult<WorkOrder>.Fail(Strings.ReloadBlockedByUnsaved));
                    return;
                }

                _log.Info(LogLayer.UI, "WorkOrdersPage.buttonReload_Click", "→ IWorkOrderService.LoadAsync()");
                await LoadAsync("WorkOrdersPage.buttonReload_Click");
            }
            catch (Exception ex)
            {
                ReportFailure("WorkOrdersPage.buttonReload_Click", ex);
            }
        }

        #endregion

        #region Bottom bar: progress, validation, empty result, outage and recovery

        /// <summary>Progress path: a Timer imports five work orders per tick through the service; the grid grows through the BindingList.</summary>
        private void buttonImport_Click(object sender, EventArgs e)
        {
            if (this.timerImport.Enabled || !_loaded)
                return;

            _importRemaining = ImportTotal;
            _importNext = 1;
            this.progressImport.Value = 0;
            this.progressImport.Visible = true;
            this.statusBanner.HideBanner();
            this.statusBanner.SetStatus($"importing 0/{ImportTotal}", StatusKind.Busy);
            _log.Info(LogLayer.UI, "WorkOrdersPage.buttonImport_Click", $"{ImportTotal} work orders in batches of {ImportBatch} per tick — each batch is added to the BindingList, never to dgvWorkOrders.Rows");
            this.timerImport.Start();
        }

        private async void timerImport_Tick(object sender, EventArgs e)
        {
            try
            {
                int batch = Math.Min(ImportBatch, _importRemaining);
                var created = await _workOrders.ImportBatchAsync(_importNext, batch);
                _importNext += batch;
                _importRemaining -= batch;

                _allOrders.AddRange(created);

                // Respect the active filter: only matching rows join the bound list. ItemAdded does the rest.
                var query = ReadQueryFromForm();
                var visible = query.IsEmpty ? created : _workOrders.Filter(created, query);
                foreach (var o in visible)
                    _visibleOrders.Add(o);

                int done = ImportTotal - _importRemaining;
                this.progressImport.Value = done;
                this.labelCount.Text = $"{_visibleOrders.Count} of {_allOrders.Count} work orders";
                this.statusBanner.SetStatus($"importing {done}/{ImportTotal}", StatusKind.Busy);
                _log.Info(LogLayer.UI, "WorkOrdersPage.timerImport_Tick", $"{visible.Count} of {created.Count} added to the BindingList → ItemAdded × {visible.Count} → grid now {_visibleOrders.Count} rows");

                if (_importRemaining == 0)
                {
                    this.timerImport.Stop();
                    this.progressImport.Visible = false;
                    this.statusBanner.SetStatus($"{ImportTotal} work orders imported", StatusKind.Success);
                    _log.Info(LogLayer.UI, "WorkOrdersPage.timerImport_Tick", $"import complete — {_allOrders.Count} work orders, {_visibleOrders.Count} shown, no Rows.Add and no rebind");
                }
            }
            catch (Exception ex)
            {
                this.timerImport.Stop();
                this.progressImport.Visible = false;
                ReportFailure("WorkOrdersPage.timerImport_Tick", ex);
            }
        }

        /// <summary>Failure path (validation): the service rejects the save; the edit stays on screen and the row stays dirty.</summary>
        private async void buttonSaveEmpty_Click(object sender, EventArgs e)
        {
            try
            {
                var current = Current;
                if (current == null)
                {
                    ShowResult(OperationResult<WorkOrder>.Fail("Select a work order first."));
                    return;
                }

                _log.Info(LogLayer.UI, "WorkOrdersPage.buttonSaveEmpty_Click", $"#{current.Id}.Title = \"\" written on the object — the bound field and the grid cell empty themselves → IWorkOrderService.SaveAsync");
                current.Title = "";
                ShowResult(await _workOrders.SaveAsync(current));
            }
            catch (Exception ex)
            {
                ReportFailure("WorkOrdersPage.buttonSaveEmpty_Click", ex, Strings.SaveFailedEditsKept);
            }
        }

        /// <summary>Not a failure: a search with no matches. Click again to clear it.</summary>
        private void buttonNoMatch_Click(object sender, EventArgs e)
        {
            const string term = "turbine";
            bool clearing = this.textSearch.Text == term;

            _settingSearch = true;
            try
            {
                this.textSearch.Text = clearing ? "" : term;
            }
            finally
            {
                _settingSearch = false;
            }

            this.buttonNoMatch.Text = clearing ? "Search with no matches" : "Clear the search";
            _log.Info(LogLayer.UI, "WorkOrdersPage.buttonNoMatch_Click", clearing ? "search cleared → every row comes back from the master list, no reload" : $"search \"{term}\" — expect 0 matches, an empty grid and a disabled editor, not an error");
            ApplyFilter("WorkOrdersPage.buttonNoMatch_Click");
        }

        /// <summary>Error path + recovery: toggle the repository outage; the next Save or Reload shows the effect.</summary>
        private void buttonOutage_Click(object sender, EventArgs e)
        {
            if (_repository == null)
                return;

            _repository.SimulateOutage = !_repository.SimulateOutage;
            bool on = _repository.SimulateOutage;
            this.buttonOutage.Text = on ? "Recover the data store" : "Simulate data outage";
            this.statusBanner.HideBanner();
            this.statusBanner.SetStatus(on ? "data store offline (simulated)" : "data store online", on ? StatusKind.Warning : StatusKind.Success);
            _log.Info(LogLayer.UI, "WorkOrdersPage.buttonOutage_Click",
                on ? "outage ON — edit a row and Save: expect ✖ in DATA, the safe message in the UI, and the edits still on screen"
                   : "outage OFF (recovery) — Save the same row again: the edits are still there and go through");
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
