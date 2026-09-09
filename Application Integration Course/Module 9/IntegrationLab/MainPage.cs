using System;
using System.Collections.Specialized;
using System.Globalization;
using IntegrationLab.Contracts;
using IntegrationLab.Data;
using Wisej.Core;
using Wisej.Web;

namespace IntegrationLab
{
    /// <summary>
    /// IntegrationLab — Data Widgets (Module 9 lab page).
    ///
    /// Top-left card:    the editable WorkOrderGrid (Kendo-style DataSource → postback URL → WebRequest handler).
    /// Bottom-left card: the read-only WorkOrderPivot (DevExtreme-style CustomStore → [WebMethod] LoadPivot).
    /// Right card:       every operation and event that crosses the wire, in both directions.
    /// Bottom bar:       success paths (reload, page, sort, insert, delete, pivot), two failure
    ///                   paths (404 unknown key, 400 take too large) and Clear trace.
    ///
    /// Both widgets end in the same GridDataController: one contract, two entry points.
    /// It is a Page (a top-level container) so the WebMethod is reachable as App.MainPage.LoadPivotAsync.
    /// </summary>
    public partial class MainPage : Page
    {
        private readonly WorkOrderStore _store = new WorkOrderStore(150);
        private readonly GridDataController _controller;

        private bool _pivotByPriority;
        private int _sampleCounter;
        private int _lastSkip, _lastTake = 20, _lastTotal = 150;

        public MainPage()
        {
            _controller = new GridDataController(_store);

            InitializeComponent();

            // The grid's postback handler and the page's WebMethod share this controller.
            this.gridWorkOrders.DataController = _controller;

            // Widget events → .NET events (the contract in action).
            this.gridWorkOrders.CellClick += gridWorkOrders_CellClick;
            this.gridWorkOrders.RowUpdated += gridWorkOrders_RowUpdated;
            this.gridWorkOrders.WidgetError += widget_WidgetError;
            this.gridWorkOrders.Trace += widget_Trace;

            this.pivotWorkOrders.CellClick += pivotWorkOrders_CellClick;
            this.pivotWorkOrders.DataLoaded += pivotWorkOrders_DataLoaded;
            this.pivotWorkOrders.WidgetError += widget_WidgetError;
            this.pivotWorkOrders.Trace += widget_Trace;

            // -----------------------------------------------------------------------------
            // JSON OPTIONS PROTOTYPE (the "fast path" from the lesson). The pivot has no typed
            // properties: the vendor option object is handed to Options as-is and camelCased
            // by Wisej when rendered. Fine for a prototype; promote to typed properties before
            // the control is reused (see WorkOrderGrid for the promoted version).
            // Runtime not yet verified: the Options setter accepts an object and the anonymous
            // type is expected to be copied into the widget's DynamicObject.
            // -----------------------------------------------------------------------------
            this.pivotWorkOrders.Options = PivotOptions("site", "status", "hours");
        }

        private static object PivotOptions(string rowField, string columnField, string measure)
            => new { rowField, columnField, measure };

        private void MainPage_Load(object sender, EventArgs e)
        {
            var g = this.gridWorkOrders;
            AddTrace(TraceDirection.ServerToClient, "render grid → init(options)",
                $"{{pageSize:{g.PageSize},editable:{g.Editable.ToString().ToLowerInvariant()},sort:\"{g.Sort}\",columns:[{g.Columns.Count}]}}");
            AddTrace(TraceDirection.ServerToClient, "render pivot → init(options)", "{rowField:\"site\",columnField:\"status\",measure:\"hours\"}");
            AddTrace(TraceDirection.Server, "note", "grid data goes over HTTP to the postback URL; pivot data over the WebMethod");
            SetStatus("idle", StatusKind.Normal);
            UpdateStateLabels();
        }

        #region [WebMethod] entry point: the DevExtreme-style CustomStore lands here

        /// <summary>
        /// Called from pivot-init.js as <c>App.MainPage.LoadPivotAsync(rowField, columnField, measure)</c>
        /// (Promise) — or <c>App.MainPage.LoadPivot(rowField, columnField, measure, callback)</c>.
        /// Same controller, same validation, same status codes as the postback handler; the
        /// status travels inside the returned object because a WebMethod has no HTTP status.
        /// Implemented per the cookbook and the client core; runtime not yet verified in the browser.
        /// </summary>
        [WebMethod]
        public object LoadPivot(string rowField, string columnField, string measure)
        {
            var args = new NameValueCollection
            {
                ["rowField"] = rowField,
                ["columnField"] = columnField,
                ["measure"] = measure
            };
            var result = _controller.Handle("pivot", args, null);

            // ← JS→.NET LoadPivot {rowField:"site",columnField:"status",measure:"hours"} → 200 (5×4, 20 cells)
            AddTrace(TraceDirection.ClientToServer, "LoadPivot (WebMethod)",
                new PivotRequest(rowField, columnField, measure).ToTraceString() + " → " + result.Status + " " + result.Summary);

            if (!result.IsSuccess)
            {
                ShowAlarm($"✖ Pivot request rejected on the server ({result.Status}): {result.Summary}", AlarmKind.Error);
                SetStatus("pivot rejected", StatusKind.Error);
            }
            return result.Body;
        }

        #endregion

        #region Success paths

        private void buttonReloadGrid_Click(object sender, EventArgs e)
        {
            HideAlarm();
            SetStatus("loading", StatusKind.Warn);
            this.gridWorkOrders.Reload();
        }

        private void buttonNextPage_Click(object sender, EventArgs e)
        {
            HideAlarm();
            SetStatus("loading", StatusKind.Warn);
            this.gridWorkOrders.NextPage();
        }

        private void buttonSortStatus_Click(object sender, EventArgs e)
        {
            // A typed property change: Options.sort is a first-level field, so Wisej renders
            // {"sort":"status asc"} and the adapter's update() makes the vendor re-read.
            HideAlarm();
            string next = this.gridWorkOrders.Sort == "status asc" ? "status desc" : this.gridWorkOrders.Sort == "status desc" ? "" : "status asc";
            this.gridWorkOrders.Sort = next;
            AddTrace(TraceDirection.ServerToClient, "update(options)", $"{{sort:\"{next}\"}}");
            SetStatus("loading", StatusKind.Warn);
            UpdateStateLabels();
        }

        private void buttonInsert_Click(object sender, EventArgs e)
        {
            // Server-driven insert: the server hands the values to the vendor, which POSTs them
            // to &action=create like a user "Add row" would. The store assigns the key.
            HideAlarm();
            _sampleCounter++;
            var values = new
            {
                asset = "Sample asset " + _sampleCounter,
                status = "Open",
                priority = _sampleCounter % 2 == 0 ? "High" : "Medium",
                assignee = "Lab",
                hours = 2.5 + _sampleCounter,
                site = WorkOrderStore.Sites[_sampleCounter % WorkOrderStore.Sites.Length]
            };
            SetStatus("inserting", StatusKind.Warn);
            this.gridWorkOrders.InsertRow(values);
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            // Deletes the row selected in the browser; with no selection the vendor throws and
            // the adapter reports it as an error event (a small client-side failure path).
            HideAlarm();
            SetStatus("deleting", StatusKind.Warn);
            this.gridWorkOrders.DeleteSelected();
        }

        private void buttonReloadPivot_Click(object sender, EventArgs e)
        {
            HideAlarm();
            SetStatus("pivot loading", StatusKind.Warn);
            this.pivotWorkOrders.Reload();
        }

        private void buttonPivotPriority_Click(object sender, EventArgs e)
        {
            // Replacing the whole JSON option object is a first-level change: Wisej renders it
            // and the adapter's update() hands the new axes to the vendor, which loads again.
            HideAlarm();
            _pivotByPriority = !_pivotByPriority;
            string columnField = _pivotByPriority ? "priority" : "status";
            string measure = _pivotByPriority ? "count" : "hours";
            this.pivotWorkOrders.Options = PivotOptions("site", columnField, measure);
            this.buttonPivotPriority.Text = _pivotByPriority ? "Pivot Site×Status" : "Pivot Site×Priority";
            AddTrace(TraceDirection.ServerToClient, "update(options)", $"{{rowField:\"site\",columnField:\"{columnField}\",measure:\"{measure}\"}}");
            SetStatus("pivot loading", StatusKind.Warn);
            UpdateStateLabels();
        }

        #endregion

        #region Failure paths

        private void buttonUnknownKey_Click(object sender, EventArgs e)
        {
            // The vendor POSTs {rowKey:"WO-9999",changes:{status:"Closed"}}; the store has no such
            // key → 404 → the vendor raises "error" → one error event → banner + trace.
            HideAlarm();
            SetStatus("expecting 404…", StatusKind.Warn);
            this.gridWorkOrders.UpdateRow("WO-9999", new { status = "Closed" });
        }

        private void buttonTake1000_Click(object sender, EventArgs e)
        {
            // take=1000 violates the contract (MaxTake=100) → 400 before any data is touched.
            HideAlarm();
            SetStatus("expecting 400…", StatusKind.Warn);
            this.gridWorkOrders.LoadWith(0, 1000);
        }

        #endregion

        #region Widget → .NET events

        private void gridWorkOrders_CellClick(object sender, CellClickEventArgs e)
        {
            AddTrace(TraceDirection.Server, "CellClick fired in C#", $"{e.RowKey}.{e.Field} = {JsonCodec.Serialize(e.Value)}");
            SetStatus($"selected {e.RowKey}", StatusKind.Normal);
        }

        private void gridWorkOrders_RowUpdated(object sender, RowUpdatedEventArgs e)
        {
            AddTrace(TraceDirection.Server, "RowUpdated fired in C#", $"{e.RowKey} {JsonCodec.DictionaryToTrace(e.Changes)}");
            SetStatus($"{e.RowKey} edited", StatusKind.Normal);
            AlertBox.Show($"{e.RowKey} changed: {JsonCodec.DictionaryToTrace(e.Changes)}", MessageBoxIcon.Information,
                alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 3000);
        }

        private void pivotWorkOrders_CellClick(object sender, PivotCellClickEventArgs e)
        {
            AddTrace(TraceDirection.Server, "Pivot CellClick fired in C#", $"{e.RowKey} × {e.ColumnKey} = {e.Value.ToString(CultureInfo.InvariantCulture)}");
            SetStatus($"pivot {e.RowKey} × {e.ColumnKey}", StatusKind.Normal);
        }

        private void pivotWorkOrders_DataLoaded(object sender, EventArgs e)
        {
            if (this.labelStatus.Text.Contains("pivot")) SetStatus("pivot loaded", StatusKind.Normal);
        }

        private void widget_WidgetError(object sender, DataWidgetErrorEventArgs e)
        {
            string who = sender == this.pivotWorkOrders ? "pivot" : "grid";
            AddTrace(TraceDirection.Server, "WidgetError fired in C#", $"{who} phase={e.Phase} status={e.Status}");
            ShowAlarm($"✖ {who} {e.Phase} failed{(e.Status > 0 ? " (" + e.Status + ")" : "")}: {e.Message}", AlarmKind.Error);
            SetStatus($"{who} error {(e.Status > 0 ? e.Status.ToString() : "")}".TrimEnd(), StatusKind.Error);
        }

        private void widget_Trace(object sender, TraceEventArgs e)
        {
            AddTrace(e.Direction, e.Name, e.Payload);

            // keep the grid state label in step with what the postback handler served.
            if (sender == this.gridWorkOrders && e.Direction == TraceDirection.ClientToServer && e.Name == "load" && e.Payload.Contains("→ 200"))
            {
                TryParseLoadTrace(e.Payload);
                SetStatus("grid loaded", StatusKind.Normal);
                UpdateStateLabels();
            }
        }

        #endregion

        #region UI helpers

        private enum StatusKind { Normal, Warn, Error }
        private enum AlarmKind { Alarm, Error }

        private void AddTrace(TraceDirection direction, string name, string payload)
        {
            string prefix = direction switch
            {
                TraceDirection.ServerToClient => "→ .NET→JS ",
                TraceDirection.ClientToServer => "← JS→.NET ",
                _ => "• server  ",
            };
            string time = DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
            this.listTrace.Items.Add($"{time}  {prefix} {name,-26} {payload}");
            this.listTrace.SelectedIndex = this.listTrace.Items.Count - 1;
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            this.listTrace.Items.Clear();
        }

        private void SetStatus(string text, StatusKind kind)
        {
            this.labelStatus.Text = "● " + text;
            this.labelStatus.ForeColor = kind switch
            {
                StatusKind.Error => System.Drawing.Color.FromArgb(224, 86, 59),
                StatusKind.Warn => System.Drawing.Color.FromArgb(232, 161, 60),
                _ => System.Drawing.Color.FromArgb(31, 157, 87),
            };
        }

        private void ShowAlarm(string text, AlarmKind kind)
        {
            this.labelAlarm.Text = text;
            this.labelAlarm.BackColor = kind == AlarmKind.Alarm
                ? System.Drawing.Color.FromArgb(253, 236, 234)
                : System.Drawing.Color.FromArgb(255, 244, 229);
            this.labelAlarm.ForeColor = kind == AlarmKind.Alarm
                ? System.Drawing.Color.FromArgb(178, 59, 39)
                : System.Drawing.Color.FromArgb(146, 64, 14);
            this.labelAlarm.Visible = true;
        }

        private void HideAlarm()
        {
            this.labelAlarm.Visible = false;
        }

        private void TryParseLoadTrace(string payload)
        {
            // payload looks like: {skip:20,take:20,sort:"status asc"} → 200 (20/150)
            try
            {
                int s = payload.IndexOf("skip:", StringComparison.Ordinal);
                int t = payload.IndexOf("take:", StringComparison.Ordinal);
                if (s >= 0) _lastSkip = int.Parse(payload.Substring(s + 5, payload.IndexOfAny(new[] { ',', '}' }, s) - s - 5), CultureInfo.InvariantCulture);
                if (t >= 0) _lastTake = int.Parse(payload.Substring(t + 5, payload.IndexOfAny(new[] { ',', '}' }, t) - t - 5), CultureInfo.InvariantCulture);
                int slash = payload.LastIndexOf('/');
                if (slash >= 0) _lastTotal = int.Parse(payload.Substring(slash + 1).TrimEnd(')'), CultureInfo.InvariantCulture);
            }
            catch (FormatException) { }
        }

        private void UpdateStateLabels()
        {
            var g = this.gridWorkOrders;
            int page = _lastTake > 0 ? _lastSkip / _lastTake + 1 : 1;
            int pages = _lastTake > 0 ? Math.Max(1, (int)Math.Ceiling(_lastTotal / (double)_lastTake)) : 1;
            this.labelGridState.Text =
                $"SERVER STATE  PageSize={g.PageSize}  Editable={g.Editable.ToString().ToLowerInvariant()}  Sort=\"{g.Sort}\"  Columns={g.Columns.Count}   " +
                $"last load: page {page}/{pages} of {_lastTotal} rows   store={_store.Count} rows";

            string columnField = _pivotByPriority ? "priority" : "status";
            string measure = _pivotByPriority ? "count" : "hours";
            this.labelPivotState.Text =
                $"Options = {{\"rowField\":\"site\",\"columnField\":\"{columnField}\",\"measure\":\"{measure}\"}}   (JSON options prototype — no typed properties yet)";
        }

        #endregion
    }
}
