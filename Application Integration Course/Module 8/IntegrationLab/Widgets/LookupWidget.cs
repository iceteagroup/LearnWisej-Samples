using System;
using System.ComponentModel;
using IntegrationLab.Data;
using Wisej.Web;

namespace IntegrationLab.Widgets
{
    /// <summary>
    /// Hosts the same VendorGrid over the same dataset, but feeds it through a
    /// [WebMethod] instead of a URL: the adapter's data-source function calls
    /// <c>App.MainPage.GetWorkOrders(page, size, sort, desc)</c> and awaits the
    /// marshaled result. The page's WebMethod delegates to
    /// <see cref="ExecuteGetWorkOrders"/>.
    /// </summary>
    [ToolboxItem(true)]
    [DefaultEvent("DataLoaded")]
    [Description("Hosts VendorGrid and serves its rows through a [WebMethod] RPC call.")]
    public class LookupWidget : Widget
    {
        private readonly WorkOrderService _service = WorkOrderService.Instance;
        private int _pageSize = PageRequest.DefaultPageSize;

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

        public event EventHandler<DataLoadedEventArgs> DataLoaded;
        public event EventHandler<GridErrorEventArgs> WidgetError;
        public event EventHandler<RowClickedEventArgs> RowClicked;

        /// <summary>Raised for every WebMethod call and its result.</summary>
        [Browsable(false)]
        public event EventHandler<TraceEventArgs> Trace;

        #endregion

        #region The RPC endpoint

        /// <summary>
        /// Loads one page for the WebMethod. Same rules as the postback handler (bounds,
        /// whitelist); the difference is how a rejection travels: here it is an
        /// ArgumentException, which Wisej reports to the client as an exception action
        /// (default: message popup) and the awaiting Promise resolves with null.
        /// </summary>
        public PageResult ExecuteGetWorkOrders(int page, int size, string sort, bool desc)
        {
            RaiseTrace(TraceDirection.ClientToServer, "App.MainPage.GetWorkOrders",
                $"{{\"page\":{page},\"size\":{size},\"sort\":\"{sort}\",\"desc\":{(desc ? "true" : "false")}}}");

            if (!PageRequest.TryCreate(page, size, sort, desc, out PageRequest request, out string error))
            {
                RaiseTrace(TraceDirection.ServerToClient, "ArgumentException", error);
                throw new ArgumentException(error);
            }

            PageResult result = _service.LoadPage(request);
            RaiseTrace(TraceDirection.ServerToClient, "return PageResult",
                $"{{\"rows\":{result.Rows.Count},\"total\":{result.Total},\"page\":{result.Page},\"size\":{result.Size},\"sort\":\"{result.Sort}\"}}");
            return result;
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
