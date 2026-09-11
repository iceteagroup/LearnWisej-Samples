using System;
using System.ComponentModel;
using System.Text.Json;
using IntegrationLab.Data;
using Wisej.Core;
using Wisej.Web;

namespace IntegrationLab.Widgets
{
    /// <summary>
    /// Hosts the third-party VendorGrid and feeds it through a POSTBACK URL.
    ///
    /// The vendor library owns the fetching: it GETs a URL with its own paging
    /// arguments. Wisej.NET gives every Widget a postback URL bound to this
    /// instance in this session, so the request arrives here, in
    /// <see cref="grid_WebRequest"/>, with the component's state and the
    /// application services at hand. The handler behaves like a small web service:
    /// it validates the action, sets the content type and writes JSON.
    /// </summary>
    [ToolboxItem(true)]
    [DefaultEvent("DataLoaded")]
    [Description("Hosts VendorGrid and serves its rows through the widget postback URL (WebRequest).")]
    public class GridWidget : Widget
    {
        // The only actions this endpoint offers. The string from the browser is
        // compared against this list; it is never used to build a method name.
        private static readonly string[] KnownActions = { "load" };

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        private readonly WorkOrderService _service = WorkOrderService.Instance;
        private int _pageSize = PageRequest.DefaultPageSize;

        public GridWidget()
        {
            // Vendor library + stylesheet are packages: loaded once per page, cached by name.
            this.Packages.Add(new Package { Name = "vendor-grid-css", Source = "wwwroot/vendor-grid.css" });
            this.Packages.Add(new Package { Name = "vendor-grid", Source = "wwwroot/vendor-grid.js" });

            // The client adapter is embedded in this assembly and handed to the wrapper.
            this.InitScript = GetResourceString("IntegrationLab.wwwroot.grid-init.js");

            // The events this wrapper is allowed to raise (the documented contract).
            this.WiredEvents = new[] { "dataLoaded", "error", "rowClick" };

            // Subscribing WebRequest is what turns this widget into a postback
            // endpoint: the client wrapper exposes getPostbackUrl() and every GET
            // on that URL is routed to this instance and raises this event.
            this.WebRequest += grid_WebRequest;

            this.Size = new System.Drawing.Size(408, 392);
            PushState();
        }

        #region Properties (server state)

        /// <summary>
        /// The postback URL bound to this component instance and this session.
        /// Session-scoped and short-lived: never store or share it.
        /// </summary>
        [Browsable(false)]
        public string PostbackUrl => ((IWisejHandler)this).GetPostbackURL();

        /// <summary>Rows per page the vendor asks for (bounded on the server anyway).</summary>
        [DefaultValue(PageRequest.DefaultPageSize)]
        public int PageSize
        {
            get => _pageSize;
            set
            {
                if (value < 1 || value > PageRequest.MaxPageSize)
                    throw new ArgumentOutOfRangeException(nameof(PageSize), value, $"PageSize must be between 1 and {PageRequest.MaxPageSize}.");
                if (_pageSize == value) return;
                _pageSize = value;
                dynamic options = this.Options;
                options.pageSize = value;
            }
        }

        #endregion

        #region Events

        [Description("Raised when the vendor grid finished loading a page from the postback URL.")]
        public event EventHandler<DataLoadedEventArgs> DataLoaded;

        [Description("Raised when the vendor grid could not load (HTTP failure or vendor error).")]
        public event EventHandler<GridErrorEventArgs> WidgetError;

        [Description("Raised when the user clicks a row.")]
        public event EventHandler<RowClickedEventArgs> RowClicked;

        /// <summary>Raised for every request and response on the postback URL.</summary>
        [Browsable(false)]
        public event EventHandler<TraceEventArgs> Trace;

        #endregion

        #region The postback endpoint: WebRequest

        // Runs in the component's context: this instance, this session. Nothing about
        // tenant, filters or permissions has to come from the browser.
        private void grid_WebRequest(object sender, WebRequestEventArgs e)
        {
            try { ServeRequest(e); }
            finally
            {
                // The postback runs outside the normal Wisej request/response cycle: push the Network lines now.
                try { Application.Update(this); } catch (Exception) { }
            }
        }

        private void ServeRequest(WebRequestEventArgs e)
        {
            string query = e.Request.Url != null ? e.Request.Url.Query : "";
            RaiseTrace(TraceDirection.Http, "GET", Shorten(query, 96));

            // 1. action: compared against a fixed list, never used to build a method name.
            var action = e.Request.QueryString["action"];
            if (Array.IndexOf(KnownActions, action) < 0)
            {
                Reject(e, 400, "unknown action");
                return;
            }

            // 2. paging parsed as bounded integers, sort mapped to a whitelist of columns.
            if (!PageRequest.TryParse(e.Request.QueryString, out PageRequest request, out string error))
            {
                Reject(e, 400, error);
                return;
            }

            // 3. produce the data with the component's state and services…
            PageResult page = _service.LoadPage(request);

            // 4. …set the content type explicitly (application/json, never text) and write the body.
            e.Response.ContentType = "application/json";
            e.Response.Write(JsonSerializer.Serialize(page, JsonOptions));

            RaiseTrace(TraceDirection.Http, "200 application/json",
                $"{{\"rows\":{page.Rows.Count},\"total\":{page.Total},\"page\":{page.Page},\"size\":{page.Size},\"sort\":\"{page.Sort}\"}}");
        }

        /// <summary>
        /// Error responses carry a status code and a short, generic reason. No stack
        /// trace, no type names, nothing about the server beyond "you asked wrong".
        /// </summary>
        private void Reject(WebRequestEventArgs e, int status, string reason)
        {
            e.Response.StatusCode = status;
            e.Response.ContentType = "application/json";
            e.Response.Write("{\"error\":\"" + reason.Replace("\"", "'") + "\"}");

            RaiseTrace(TraceDirection.Http, $"{status} application/json", $"{{\"error\":\"{reason}\"}}");
        }

        #endregion

        #region Widget → .NET events

        protected override void OnWidgetEvent(WidgetEventArgs e)
        {
            dynamic data = e.Data;

            switch (e.Type)
            {
                case "dataLoaded":
                    DataLoaded?.Invoke(this, new DataLoadedEventArgs(
                        ToInt(data?.count), ToInt(data?.total), ToInt(data?.page), ToInt(data?.pages), ToInt(data?.elapsed)));
                    break;

                case "error":
                    WidgetError?.Invoke(this, new GridErrorEventArgs(ToInt(data?.status), ToStr(data?.message), ToStr(data?.phase)));
                    break;

                case "rowClick":
                    RowClicked?.Invoke(this, new RowClickedEventArgs(ToStr(data?.id)));
                    break;

                default:
                    base.OnWidgetEvent(e);
                    break;
            }
        }

        #endregion

        #region Helpers

        private void PushState()
        {
            dynamic options = this.Options;
            options.pageSize = _pageSize;
            options.columns = GridColumns.All;
        }

        private static string Shorten(string value, int max)
            => string.IsNullOrEmpty(value) || value.Length <= max ? value : value.Substring(0, max - 1) + "…";

        private void RaiseTrace(TraceDirection direction, string name, string payload)
        {
            try { Trace?.Invoke(this, new TraceEventArgs(direction, name, payload)); }
            catch (ObjectDisposedException) { }
        }

        private static int ToInt(object value)
        {
            try { return value == null ? 0 : Convert.ToInt32(value, System.Globalization.CultureInfo.InvariantCulture); }
            catch { return 0; }
        }

        private static string ToStr(object value) => value == null ? "" : Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture);

        #endregion
    }

    /// <summary>The column list both grids render (client field names = DTO camelCase names).</summary>
    internal static class GridColumns
    {
        public static object[] All => new object[]
        {
            new { field = "id", title = "ID" },
            new { field = "asset", title = "Asset" },
            new { field = "status", title = "Status" },
            new { field = "priority", title = "Priority" },
            new { field = "assignee", title = "Assignee" },
            new { field = "dueDate", title = "Due" },
            new { field = "hours", title = "Hours" }
        };
    }
}
