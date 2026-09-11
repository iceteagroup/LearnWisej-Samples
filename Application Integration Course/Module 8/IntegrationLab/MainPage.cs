using System;
using System.Globalization;
using IntegrationLab.Widgets;
using Wisej.Core;
using Wisej.Web;

namespace IntegrationLab
{
    /// <summary>
    /// Work Orders: the same read-only dataset in two vendor grids. The left grid pulls
    /// its rows from the widget's postback URL (WebRequest); the middle grid calls the
    /// page's [WebMethod] and awaits the result. The right card lists the requests.
    ///
    /// This is a top-level Page (Application.MainPage), which is why its [WebMethod]
    /// is discovered automatically and callable as App.MainPage.GetWorkOrders.
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();

            this.gridPostback.WidgetError += grid_WidgetError;
            this.gridPostback.RowClicked += grid_RowClicked;
            this.gridPostback.Trace += (s, e) => AddTrace(e.Direction, "[postback] " + e.Name, e.Payload);

            this.gridLookup.WidgetError += grid_WidgetError;
            this.gridLookup.RowClicked += grid_RowClicked;
            this.gridLookup.Trace += (s, e) => AddTrace(e.Direction, "[webmethod] " + e.Name, e.Payload);
        }

        /// <summary>
        /// Called from the browser as App.MainPage.GetWorkOrdersAsync(page, size, sort, desc).
        /// Arguments are marshaled to the typed parameters; the returned object is
        /// marshaled back to the caller.
        /// </summary>
        [WebMethod]
        public object GetWorkOrders(int page, int size, string sort, bool desc)
            => this.gridLookup.ExecuteGetWorkOrders(page, size, sort, desc);

        private void grid_WidgetError(object sender, GridErrorEventArgs e)
        {
            string grid = sender == this.gridPostback ? "Postback grid" : "WebMethod grid";
            string text = e.Status > 0 ? $"HTTP {e.Status}: {e.Message}" : e.Message;
            AlertBox.Show($"{grid}: {text}", MessageBoxIcon.Error,
                alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 5000);
        }

        private void grid_RowClicked(object sender, RowClickedEventArgs e)
        {
            string which = sender == this.gridPostback ? "postback" : "webmethod";
            AlertBox.Show($"{e.Id} selected in the {which} grid.", MessageBoxIcon.Information,
                alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 2500);
        }

        private void AddTrace(TraceDirection direction, string name, string payload)
        {
            string prefix = direction switch
            {
                TraceDirection.ServerToClient => "→ .NET→JS ",
                TraceDirection.ClientToServer => "← JS→.NET ",
                _ => "⇄ HTTP    ",
            };
            string time = DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
            this.listTrace.Items.Add($"{time}  {prefix} {name,-34} {payload}");
            this.listTrace.SelectedIndex = this.listTrace.Items.Count - 1;
        }
    }
}
