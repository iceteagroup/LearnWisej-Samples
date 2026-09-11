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
    /// Data Widgets: the editable WorkOrderGrid (Kendo-style DataSource → postback URL →
    /// WebRequest handler) and the read-only WorkOrderPivot (DevExtreme-style CustomStore →
    /// [WebMethod] LoadPivot), with the remote operations listed on the right.
    ///
    /// Both widgets end in the same GridDataController: one contract, two entry points.
    /// It is a Page (a top-level container) so the WebMethod is reachable as App.MainPage.LoadPivotAsync.
    /// </summary>
    public partial class MainPage : Page
    {
        private readonly WorkOrderStore _store = new WorkOrderStore(150);
        private readonly GridDataController _controller;

        public MainPage()
        {
            _controller = new GridDataController(_store);

            InitializeComponent();

            // The grid's postback handler and the page's WebMethod share this controller.
            this.gridWorkOrders.DataController = _controller;

            this.gridWorkOrders.RowUpdated += gridWorkOrders_RowUpdated;
            this.gridWorkOrders.WidgetError += widget_WidgetError;
            this.gridWorkOrders.Trace += widget_Trace;

            this.pivotWorkOrders.WidgetError += widget_WidgetError;
            this.pivotWorkOrders.Trace += widget_Trace;

            // JSON options prototype: the pivot has no typed properties yet. The vendor option
            // object is handed to Options as-is and camelCased by Wisej when rendered.
            this.pivotWorkOrders.Options = new { rowField = "site", columnField = "status", measure = "hours" };
        }

        /// <summary>
        /// Called from pivot-init.js as <c>App.MainPage.LoadPivotAsync(rowField, columnField, measure)</c>.
        /// Same controller, same validation, same status codes as the postback handler; the
        /// status travels inside the returned object because a WebMethod has no HTTP status.
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

            AddTrace(TraceDirection.ClientToServer, "LoadPivot (WebMethod)",
                new PivotRequest(rowField, columnField, measure).ToTraceString() + " → " + result.Status + " " + result.Summary);

            return result.Body;
        }

        private void gridWorkOrders_RowUpdated(object sender, RowUpdatedEventArgs e)
        {
            AlertBox.Show($"{e.RowKey} changed: {JsonCodec.DictionaryToTrace(e.Changes)}", MessageBoxIcon.Information,
                alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 3000);
        }

        private void widget_WidgetError(object sender, DataWidgetErrorEventArgs e)
        {
            string who = sender == this.pivotWorkOrders ? "Pivot" : "Grid";
            AlertBox.Show($"{who} {e.Phase} failed{(e.Status > 0 ? " (" + e.Status + ")" : "")}: {e.Message}", MessageBoxIcon.Error,
                alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 5000);
        }

        private void widget_Trace(object sender, TraceEventArgs e)
            => AddTrace(e.Direction, e.Name, e.Payload);

        private void AddTrace(TraceDirection direction, string name, string payload)
        {
            string prefix = direction == TraceDirection.ServerToClient ? "→ .NET→JS " : "← JS→.NET ";
            string time = DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
            this.listTrace.Items.Add($"{time}  {prefix} {name,-26} {payload}");
            this.listTrace.SelectedIndex = this.listTrace.Items.Count - 1;
        }
    }
}
