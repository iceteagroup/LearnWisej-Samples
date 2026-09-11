using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using IntegrationLab.Contracts;
using IntegrationLab.Data;
using Wisej.Core;
using Wisej.Web;

namespace IntegrationLab.Widgets
{
    /// <summary>
    /// The reusable editable grid: a Wisej.NET Widget hosting the (stand-in) VendorGrid
    /// library, configured with Kendo-style DataSource semantics.
    ///
    ///   - Typed properties (PageSize, Editable, Sort, Columns) replace the vendor's option
    ///     bag; the rare extra option goes through <see cref="OnConfigureOptions"/>.
    ///   - The vendor's transport points at this control's postback URL with
    ///     &amp;action=load|create|update|destroy; the <see cref="Widget.WebRequest"/> handler
    ///     validates the action, reads the query/body, and calls <see cref="GridDataController"/>.
    ///   - Vendor events cellclick / rowupdate become the .NET events <see cref="CellClick"/>
    ///     and <see cref="RowUpdated"/> with validated payloads.
    ///
    /// Nothing in this file knows what "Kendo" looks like; that knowledge is in wwwroot/grid-init.js.
    /// </summary>
    [ToolboxItem(true)]
    [DefaultEvent("RowUpdated")]
    [DefaultProperty("Columns")]
    [Description("Editable grid backed by a vendor-free load/insert/update/delete contract.")]
    public class WorkOrderGrid : Widget
    {
        private int _pageSize = 20;
        private bool _editable = true;
        private string _sort = "";
        private readonly List<GridColumn> _columns = new List<GridColumn>();

        public WorkOrderGrid()
        {
            // Vendor resources are packages: loaded once per page, in list order (CSS first).
            this.Packages.Add(new Package { Name = "vendor-grid-css", Source = "wwwroot/vendor-grid.css" });
            this.Packages.Add(new Package { Name = "vendor-grid", Source = "wwwroot/vendor-grid.js" });

            // The client adapter (the only place that knows the vendor's DataSource shape).
            this.InitScript = GetResourceString("IntegrationLab.wwwroot.grid-init.js");

            // The events the wrapper may raise: the documented contract.
            this.WiredEvents = new[] { "cellClick", "rowUpdated", "error" };

            // Subscribing to WebRequest is what makes Wisej render the wrapper's "postbackUrl"
            // property, which grid-init.js reads with this.getPostbackUrl().
            this.WebRequest += this.HandleWebRequest;

            this.Size = new System.Drawing.Size(640, 280);

            _columns.Add(new GridColumn("id", "Work order", 90, editable: false));
            _columns.Add(new GridColumn("asset", "Asset", 110));
            _columns.Add(new GridColumn("status", "Status", 100, type: "select", values: WorkOrderStore.Statuses));
            _columns.Add(new GridColumn("priority", "Priority", 90, type: "select", values: WorkOrderStore.Priorities));
            _columns.Add(new GridColumn("assignee", "Assignee", 90));
            _columns.Add(new GridColumn("hours", "Hours", 70, type: "number"));
            _columns.Add(new GridColumn("site", "Site", 90, type: "select", values: WorkOrderStore.Sites));

            PushOptions();
        }

        #region Typed properties (the vendor option bag, promoted)

        /// <summary>The controller both entry points share. Set by the page.</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridDataController DataController { get; set; }

        [DefaultValue(20)]
        [Description("Rows per page. 1..100; the server rejects larger pages with a 400.")]
        public int PageSize
        {
            get => _pageSize;
            set
            {
                if (value < 1 || value > GridOperationRequest.MaxTake)
                    throw new ArgumentOutOfRangeException(nameof(PageSize), value, $"PageSize must be between 1 and {GridOperationRequest.MaxTake}.");
                if (_pageSize == value) return;
                _pageSize = value;
                dynamic options = this.Options;
                options.pageSize = value;          // first-level field: Wisej renders it and calls update(options, old)
            }
        }

        [DefaultValue(true)]
        [Description("Enables inline cell editing, Add row and Delete in the browser. The server validates regardless.")]
        public bool Editable
        {
            get => _editable;
            set
            {
                if (_editable == value) return;
                _editable = value;
                dynamic options = this.Options;
                options.editable = value;
            }
        }

        /// <summary>Kendo-style sort expression: "status asc" or "site asc,hours desc". Empty = natural order.</summary>
        [DefaultValue("")]
        [Description("Sort expression sent with every load: \"status asc\" or \"site asc,hours desc\".")]
        public string Sort
        {
            get => _sort;
            set
            {
                value = (value ?? "").Trim();
                foreach (var term in value.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    string sortField = term.Trim().Split(' ')[0];
                    if (!WorkOrderStore.QueryableFields.Contains(sortField.ToLowerInvariant()))
                        throw new ArgumentException($"\"{sortField}\" is not a sortable field.", nameof(Sort));
                }
                if (_sort == value) return;
                _sort = value;
                dynamic options = this.Options;
                options.sort = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("Typed column definitions. Call ApplyColumns() after editing the list at runtime.")]
        public List<GridColumn> Columns => _columns;

        /// <summary>Re-sends the column list (a nested object, so it is replaced as a whole).</summary>
        public void ApplyColumns()
        {
            dynamic options = this.Options;
            options.columns = _columns.Select(c => c.ToOption()).ToArray();
        }

        /// <summary>
        /// Escape hatch for the rare vendor option that has no typed property yet. Derived
        /// classes add fields to <paramref name="options"/> here; the base does nothing.
        /// </summary>
        protected virtual void OnConfigureOptions(dynamic options)
        {
        }

        private void PushOptions()
        {
            dynamic options = this.Options;
            options.pageSize = _pageSize;
            options.editable = _editable;
            options.sort = _sort;
            options.columns = _columns.Select(c => c.ToOption()).ToArray();
            OnConfigureOptions(options);
        }

        #endregion

        #region Events (.NET events raised from widget events)

        /// <summary>A cell was clicked: payload cellClick { rowKey, field, value }.</summary>
        public event EventHandler<CellClickEventArgs> CellClick;

        /// <summary>The user committed an inline edit: payload rowUpdated { rowKey, changes }.</summary>
        public event EventHandler<RowUpdatedEventArgs> RowUpdated;

        /// <summary>The client adapter caught a vendor/transport failure: error { phase, status, message }.</summary>
        public event EventHandler<DataWidgetErrorEventArgs> WidgetError;

        /// <summary>Raised for every operation and event the grid sends to the server.</summary>
        [Browsable(false)]
        public event EventHandler<TraceEventArgs> Trace;

        #endregion

        #region Postback handler: the Kendo-style transport lands here

        /// <summary>
        /// One HTTP request from the vendor's transport. The URL is this control's postback
        /// URL plus &amp;action=…; loads carry skip/take/sort/filter, modify operations carry
        /// their JSON document in &amp;payload=… (WebRequest is raised for GET only).
        /// </summary>
        private void HandleWebRequest(object sender, WebRequestEventArgs e)
        {
            string action = e.Request.QueryString["action"] ?? "";
            string body = null;
            OperationResult result;

            if (this.DataController == null)
            {
                result = OperationResult.Fail(500, "WorkOrderGrid.DataController is not set.");
            }
            else if (!GridDataController.Actions.Contains(action.ToLowerInvariant()) || action == "pivot")
            {
                // validate against the fixed list before touching anything else ("pivot" belongs to the WebMethod entry point).
                result = OperationResult.Fail(400, $"Unknown action \"{action}\". Allowed: load, create, update, destroy.");
            }
            else
            {
                // VERIFIED (Wisej-4 4.1.0): the postback endpoint raises WebRequest for GET only; a POST to
                // postback.wx is consumed by the framework pipeline and answers [{"type":0}]. The vendor adapter
                // therefore sends the operation document as a GET query parameter (payload=<url-encoded JSON>).
                // The POST branch is kept for hosts/versions where the body does arrive.
                if (string.Equals(e.Request.RequestType, "POST", StringComparison.OrdinalIgnoreCase))
                {
                    using (var reader = new StreamReader(e.Request.InputStream, Encoding.UTF8))
                        body = reader.ReadToEnd();
                }
                if (string.IsNullOrWhiteSpace(body))
                    body = e.Request.QueryString["payload"];

                result = this.DataController.Handle(action, e.Request.QueryString, body);
            }

            string json = JsonCodec.Serialize(result.Body);
            e.Response.StatusCode = result.Status;
            e.Response.ContentType = "application/json";
            e.Response.Write(json);

            // ← JS→.NET load {skip:0,take:20,sort:"status asc"} → 200 (20/150)
            RaiseTrace(TraceDirection.ClientToServer, action, DescribeRequest(action, e, body) + " → " + result.Status + " " + result.Summary);

            // This ran on the postback thread, outside the normal Wisej request/response cycle:
            // push the pending UI changes (the operation line) to the browser now. If the push is not
            // possible the line simply shows up with the next regular round trip, so never let it
            // break the data response that was already written.
            try { Application.Update(this); } catch (Exception) { }
        }

        private static string DescribeRequest(string action, WebRequestEventArgs e, string body)
        {
            if (action == "load")
                return GridDataController.ParseLoadArgumentsSafe(e.Request.QueryString);
            if (string.IsNullOrWhiteSpace(body)) return "{}";
            // compact the body: drop whitespace, cap the length.
            string compact = body.Replace("\r", "").Replace("\n", "").Replace("  ", "");
            return compact.Length > 90 ? compact.Substring(0, 87) + "..." : compact;
        }

        #endregion

        #region Widget events → .NET events (payloads validated on the server)

        protected override void OnWidgetEvent(WidgetEventArgs e)
        {
            dynamic data = e.Data;
            switch (e.Type)
            {
                case "cellClick":
                    {
                        string rowKey = (string)(data?.rowKey ?? "");
                        string field = (string)(data?.field ?? "");
                        object value = JsonCodec.NormalizeValue((object)data?.value);
                        RaiseTrace(TraceDirection.ClientToServer, "cellClick", $"{{rowKey:\"{rowKey}\",field:\"{field}\",value:{JsonCodec.Serialize(value)}}}");
                        if (!IsValidKey(rowKey) || !WorkOrderStore.QueryableFields.Contains(field.ToLowerInvariant()))
                            return;   // payload outside the contract: dropped
                        CellClick?.Invoke(this, new CellClickEventArgs(rowKey, field, value));
                        break;
                    }
                case "rowUpdated":
                    {
                        string rowKey = (string)(data?.rowKey ?? "");
                        var changes = JsonCodec.ToDictionary((object)data?.changes);
                        RaiseTrace(TraceDirection.ClientToServer, "rowUpdated", $"{{rowKey:\"{rowKey}\",changes:{JsonCodec.DictionaryToTrace(changes)}}}");
                        if (!IsValidKey(rowKey) || changes.Count == 0 || changes.Keys.Any(k => !WorkOrderStore.EditableFields.Contains(k.ToLowerInvariant())))
                            return;   // payload outside the contract: dropped
                        RowUpdated?.Invoke(this, new RowUpdatedEventArgs(rowKey, changes));
                        break;
                    }
                case "error":
                    {
                        string phase = (string)(data?.phase ?? "unknown");
                        int status = data?.status == null ? 0 : Convert.ToInt32((object)data.status);
                        string message = (string)(data?.message ?? "");
                        RaiseTrace(TraceDirection.ClientToServer, "error", $"{{phase:\"{phase}\",status:{status},message:\"{message}\"}}");
                        WidgetError?.Invoke(this, new DataWidgetErrorEventArgs(phase, status, message));
                        break;
                    }
                default:
                    base.OnWidgetEvent(e);   // never swallow unknown events
                    break;
            }
        }

        private static bool IsValidKey(string key)
            => !string.IsNullOrEmpty(key) && key.StartsWith("WO-", StringComparison.OrdinalIgnoreCase) && key.Length <= 12;

        #endregion

        private void RaiseTrace(TraceDirection direction, string name, string payload)
            => Trace?.Invoke(this, new TraceEventArgs(direction, name, payload));
    }
}
