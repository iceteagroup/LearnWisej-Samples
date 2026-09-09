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

            // ==== UNVERIFIED (postback wiring) ==================================
            // Subscribing WebRequest is what turns this widget into a postback
            // endpoint: the client wrapper exposes getPostbackUrl() and every GET
            // on that URL is routed to this instance and raises this event.
            this.WebRequest += grid_WebRequest;
            // ====================================================================

            this.Size = new System.Drawing.Size(408, 392);
            PushState();
        }

        #region Properties (server state)

        /// <summary>
        /// The postback URL bound to this component instance and this session.
        /// Session-scoped and short-lived: display it, never store or share it.
        /// </summary>
        [Browsable(false)]
        public string PostbackUrl
        {
            // ==== UNVERIFIED (server-side postback URL) ==========================
            // GetPostbackURL is an extension method in Wisej.Core.IWisejHandlerExtension
            // over IWisejHandler, which Widget implements explicitly.
            get => ((IWisejHandler)this).GetPostbackURL();
            // ====================================================================
        }

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

        [Browsable(false)] public int CurrentPage { get; private set; } = 1;
        [Browsable(false)] public int TotalPages { get; private set; }
        [Browsable(false)] public int RowCount { get; private set; }
        [Browsable(false)] public int TotalRows { get; private set; }

        /// <summary>Number of postback requests this instance has handled.</summary>
        [Browsable(false)] public int RequestCount { get; private set; }

        /// <summary>Number of those that were rejected with a 4xx.</summary>
        [Browsable(false)] public int RejectedCount { get; private set; }

        /// <summary>Last request line and outcome, for the card info label.</summary>
        [Browsable(false)] public string LastRequest { get; private set; } = "";

        #endregion

        #region Events

        [Description("Raised when the vendor grid finished loading a page from the postback URL.")]
        public event EventHandler<DataLoadedEventArgs> DataLoaded;

        [Description("Raised when the vendor grid could not load (HTTP failure or vendor error).")]
        public event EventHandler<GridErrorEventArgs> WidgetError;

        [Description("Raised when the user clicks a row.")]
        public event EventHandler<RowClickedEventArgs> RowClicked;

        /// <summary>Every message that crosses the wire, for the lab trace.</summary>
        [Browsable(false)]
        public event EventHandler<TraceEventArgs> Trace;

        #endregion

        #region The postback endpoint: WebRequest

        // ==== UNVERIFIED (WebRequest handler) =====================================
        // The lesson's handler, with the validation the lesson asks for:
        //
        //   private void grid_WebRequest(object sender, WebRequestEventArgs e)
        //   {
        //       var action = e.Request.QueryString["action"];
        //       if (action != "load") { e.Response.StatusCode = 400; return; }
        //       var rows = _service.LoadPage(CurrentTenant, ParsePage(e.Request));
        //       e.Response.ContentType = "application/json";
        //       e.Response.Write(JsonSerializer.Serialize(rows));
        //   }
        //
        // Runs in the component's context: this instance, this session. Nothing about
        // tenant, filters or permissions has to come from the browser.
        private void grid_WebRequest(object sender, WebRequestEventArgs e)
        {
            RequestCount++;
            string query = e.Request.Url != null ? e.Request.Url.Query : "";
            RaiseTrace(TraceDirection.Http, "WebRequest GET", Shorten(query, 96));

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

            LastRequest = $"GET ?action={action}{RequestSummary(request)} → 200 application/json · {page.Rows.Count} rows of {page.Total}";
            RaiseTrace(TraceDirection.Http, "200 application/json",
                $"{{\"rows\":{page.Rows.Count},\"total\":{page.Total},\"page\":{page.Page},\"size\":{page.Size},\"sort\":\"{page.Sort}\"}}");
        }

        /// <summary>
        /// Error responses carry a status code and a short, generic reason. No stack
        /// trace, no type names, nothing about the server beyond "you asked wrong".
        /// </summary>
        private void Reject(WebRequestEventArgs e, int status, string reason)
        {
            RejectedCount++;
            e.Response.StatusCode = status;
            e.Response.ContentType = "application/json";
            e.Response.Write("{\"error\":\"" + reason.Replace("\"", "'") + "\"}");

            LastRequest = $"GET {Shorten(e.Request.Url?.Query ?? "", 40)} → {status} application/json · rejected: {reason}";
            RaiseTrace(TraceDirection.Http, $"{status} application/json", $"{{\"error\":\"{reason}\"}}  (rejected on the server)");
        }
        // ==========================================================================

        #endregion

        #region Commands (server → client, Control.Call)

        /// <summary>Recovery: back to action=load, default page size, page 1.</summary>
        public void Reload()
        {
            RaiseTrace(TraceDirection.ServerToClient, "Call(\"reload\")", "{}");
            this.Call("reload");
        }

        public void NextPage()
        {
            int next = TotalPages == 0 || CurrentPage >= TotalPages ? 1 : CurrentPage + 1;
            RaiseTrace(TraceDirection.ServerToClient, "Call(\"setPage\")", next.ToString());
            this.Call("setPage", next);
        }

        public void SortBy(string field)
        {
            RaiseTrace(TraceDirection.ServerToClient, "Call(\"sort\")", $"\"{field}\"");
            this.Call("sort", field);
        }

        /// <summary>Failure path 1: the vendor asks for an action the endpoint does not offer.</summary>
        public void LoadWithAction(string action)
        {
            RaiseTrace(TraceDirection.ServerToClient, "Call(\"loadWithAction\")", $"\"{action}\"");
            this.Call("loadWithAction", action);
        }

        /// <summary>Failure path 2: the vendor asks for more rows than the endpoint serves.</summary>
        public void LoadWithSize(int size)
        {
            RaiseTrace(TraceDirection.ServerToClient, "Call(\"loadWithSize\")", size.ToString());
            this.Call("loadWithSize", size);
        }

        #endregion

        #region Widget → .NET events

        protected override void OnWidgetEvent(WidgetEventArgs e)
        {
            dynamic data = e.Data;

            switch (e.Type)
            {
                case "dataLoaded":
                    {
                        int count = ToInt(data?.count), total = ToInt(data?.total), page = ToInt(data?.page),
                            pages = ToInt(data?.pages), elapsed = ToInt(data?.elapsed);
                        string via = ToStr(data?.via);
                        CurrentPage = page > 0 ? page : 1;
                        TotalPages = pages;
                        RowCount = count;
                        TotalRows = total;
                        RaiseTrace(TraceDirection.ClientToServer, "dataLoaded",
                            $"{{\"count\":{count},\"total\":{total},\"page\":{page},\"pages\":{pages},\"elapsed\":{elapsed},\"via\":\"{via}\"}}");
                        DataLoaded?.Invoke(this, new DataLoadedEventArgs(count, total, page, pages, elapsed, via));
                        break;
                    }
                case "error":
                    {
                        int status = ToInt(data?.status);
                        string message = ToStr(data?.message), phase = ToStr(data?.phase);
                        RowCount = 0;
                        RaiseTrace(TraceDirection.ClientToServer, "error",
                            $"{{\"status\":{status},\"message\":\"{message}\",\"phase\":\"{phase}\"}}");
                        WidgetError?.Invoke(this, new GridErrorEventArgs(status, message, phase, "postback"));
                        break;
                    }
                case "rowClick":
                    {
                        string id = ToStr(data?.id);
                        RaiseTrace(TraceDirection.ClientToServer, "rowClick", $"{{\"id\":\"{id}\"}}");
                        RowClicked?.Invoke(this, new RowClickedEventArgs(id));
                        break;
                    }
                default:
                    base.OnWidgetEvent(e);
                    break;
            }
        }

        #endregion

        #region Helpers

        /// <summary>Compact JSON of the state this component owns (what init(options) receives).</summary>
        public string ToJson()
            => $"{{\"pageSize\":{_pageSize},\"columns\":[id,asset,status,priority,assignee,dueDate,hours]}}";

        private void PushState()
        {
            dynamic options = this.Options;
            options.pageSize = _pageSize;
            options.columns = GridColumns.All;
        }

        private static string RequestSummary(PageRequest r)
            => $"&page={r.Page}&size={r.Size}&sort={r.Sort}&desc={(r.Desc ? "true" : "false")}";

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
