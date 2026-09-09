using System;
using System.ComponentModel;
using IntegrationLab.Data;
using Wisej.Core;
using Wisej.Web;

namespace IntegrationLab.Widgets
{
    /// <summary>
    /// Hosts the same VendorGrid over the same dataset, but feeds it through a
    /// [WebMethod] instead of a URL: the adapter's data-source function calls
    /// <c>GetWorkOrders(page, size, sort, desc)</c> and awaits the marshaled result.
    ///
    /// The method is exposed in two places so the two registration styles can be
    /// compared at runtime (<see cref="DataSourceMode"/>):
    ///   "page"   → MainPage.GetWorkOrders (top-level Page: registered automatically, App.MainPage.*)
    ///   "widget" → this.GetWorkOrders     (child control: registered via RegisterWebMethods in OnWebRender)
    /// Both delegate to <see cref="ExecuteGetWorkOrders"/> so validation and tracing are identical.
    /// </summary>
    [ToolboxItem(true)]
    [DefaultEvent("DataLoaded")]
    [Description("Hosts VendorGrid and serves its rows through a [WebMethod] RPC call.")]
    public class LookupWidget : Widget
    {
        public const string ModePage = "page";
        public const string ModeWidget = "widget";

        private readonly WorkOrderService _service = WorkOrderService.Instance;
        private int _pageSize = PageRequest.DefaultPageSize;
        private string _mode = ModePage;

        public LookupWidget()
        {
            this.Packages.Add(new Package { Name = "vendor-grid-css", Source = "wwwroot/vendor-grid.css" });
            this.Packages.Add(new Package { Name = "vendor-grid", Source = "wwwroot/vendor-grid.js" });
            this.InitScript = GetResourceString("IntegrationLab.wwwroot.lookup-init.js");
            this.WiredEvents = new[] { "dataLoaded", "error", "rowClick" };
            this.Size = new System.Drawing.Size(408, 392);
            PushState();
        }

        #region Properties (server state)

        /// <summary>"page" calls App.MainPage.GetWorkOrders; "widget" calls this.GetWorkOrders.</summary>
        [DefaultValue(ModePage)]
        [Description("Which WebMethod the client calls: \"page\" (App.MainPage) or \"widget\" (this control).")]
        public string DataSourceMode
        {
            get => _mode;
            set
            {
                if (value != ModePage && value != ModeWidget)
                    throw new ArgumentOutOfRangeException(nameof(DataSourceMode), value, "DataSourceMode must be \"page\" or \"widget\".");
                if (_mode == value) return;
                _mode = value;
                dynamic options = this.Options;
                options.dataSourceMode = value;     // first-level field → update(options, old) → the adapter reloads
                RaiseTrace(TraceDirection.ServerToClient, "update(options)", $"{{\"dataSourceMode\":\"{value}\"}}");
            }
        }

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
        [Browsable(false)] public int CallCount { get; private set; }
        [Browsable(false)] public int RejectedCount { get; private set; }
        [Browsable(false)] public string LastCall { get; private set; } = "";

        /// <summary>The call shape the client reported using last ("…Async → Promise" or "+ trailing callback").</summary>
        [Browsable(false)] public string LastClientShape { get; private set; } = "";

        #endregion

        #region Events

        public event EventHandler<DataLoadedEventArgs> DataLoaded;
        public event EventHandler<GridErrorEventArgs> WidgetError;
        public event EventHandler<RowClickedEventArgs> RowClicked;

        [Browsable(false)]
        public event EventHandler<TraceEventArgs> Trace;

        #endregion

        #region The RPC endpoint: [WebMethod]

        // ==== UNVERIFIED (RegisterWebMethods on a child control) ==================
        // Wisej only searches top-level containers (Page, Form, Desktop) and static
        // Program methods for [WebMethod]. A child control registers its own by
        // overriding OnWebRender and calling RegisterWebMethods(config): the render
        // then carries config.webMethods = ["GetWorkOrders"] and the client creates
        // this.GetWorkOrders(args…, callback) and this.GetWorkOrdersAsync(args…) on the
        // wrapper (verified in the framework client: Wisej.Core.registerWebMethods).
        protected override void OnWebRender(dynamic config)
        {
            base.OnWebRender((object)config);
            RegisterWebMethods((object)config);
        }

        /// <summary>
        /// (b) The WebMethod on the widget itself, called from the InitScript as
        /// <c>this.GetWorkOrdersAsync(page, size, sort, desc)</c> when DataSourceMode is "widget".
        /// Typed arguments are marshaled from JavaScript; the returned object is
        /// marshaled back and the client awaits it. No default parameter values.
        /// </summary>
        [WebMethod]
        public object GetWorkOrders(int page, int size, string sort, bool desc)
            => ExecuteGetWorkOrders("this.GetWorkOrders (LookupWidget)", page, size, sort, desc);
        // ==========================================================================

        /// <summary>
        /// The one implementation both WebMethods share. Same rules as the postback
        /// handler (bounds, whitelist); the difference is how a rejection travels:
        /// here it is an ArgumentException, which Wisej reports to the client as an
        /// exception action (default: message popup) and the awaiting Promise
        /// resolves with null — there is no status code to inspect.
        /// </summary>
        public PageResult ExecuteGetWorkOrders(string target, int page, int size, string sort, bool desc)
        {
            CallCount++;
            RaiseTrace(TraceDirection.ClientToServer, $"WebMethod {target}",
                $"{{\"page\":{page},\"size\":{size},\"sort\":\"{sort}\",\"desc\":{(desc ? "true" : "false")}}}");

            if (!PageRequest.TryCreate(page, size, sort, desc, out PageRequest request, out string error))
            {
                RejectedCount++;
                LastCall = $"{target}({page}, {size}, \"{sort}\", {desc}) → ArgumentException: {error}";
                RaiseTrace(TraceDirection.Server, "ArgumentException", $"{error}  → client: Wisej exception popup, Promise resolves null");
                throw new ArgumentException(error);
            }

            PageResult result = _service.LoadPage(request);
            LastCall = $"{target}({page}, {size}, \"{request.Sort}\", {desc}) → PageResult · {result.Rows.Count} rows of {result.Total}";
            RaiseTrace(TraceDirection.ServerToClient, "return PageResult",
                $"{{\"rows\":{result.Rows.Count},\"total\":{result.Total},\"page\":{result.Page},\"size\":{result.Size},\"sort\":\"{result.Sort}\"}}");
            return result;
        }

        #endregion

        #region Commands (server → client, Control.Call)

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
                        string via = ToStr(data?.via), keys = ToStr(data?.keys);
                        CurrentPage = page > 0 ? page : 1;
                        TotalPages = pages;
                        RowCount = count;
                        TotalRows = total;
                        LastClientShape = via;
                        RaiseTrace(TraceDirection.ClientToServer, "dataLoaded",
                            $"{{\"count\":{count},\"total\":{total},\"page\":{page},\"pages\":{pages},\"elapsed\":{elapsed},\"via\":\"{via}\",\"keys\":\"{keys}\"}}");
                        DataLoaded?.Invoke(this, new DataLoadedEventArgs(count, total, page, pages, elapsed, via));
                        break;
                    }
                case "error":
                    {
                        int status = ToInt(data?.status);
                        string message = ToStr(data?.message), phase = ToStr(data?.phase), via = ToStr(data?.via);
                        RowCount = 0;
                        RaiseTrace(TraceDirection.ClientToServer, "error",
                            $"{{\"status\":{status},\"message\":\"{message}\",\"phase\":\"{phase}\",\"via\":\"{via}\"}}");
                        WidgetError?.Invoke(this, new GridErrorEventArgs(status, message, phase, via));
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

        public string ToJson()
            => $"{{\"pageSize\":{_pageSize},\"dataSourceMode\":\"{_mode}\",\"columns\":[id,asset,status,priority,assignee,dueDate,hours]}}";

        private void PushState()
        {
            dynamic options = this.Options;
            options.pageSize = _pageSize;
            options.dataSourceMode = _mode;
            options.columns = GridColumns.All;
        }

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
}
