using System;
using System.ComponentModel;
using IntegrationLab.Contracts;
using Wisej.Web;

namespace IntegrationLab.Widgets
{
    /// <summary>
    /// The read-only pivot prototype: a Wisej.NET Widget hosting the (stand-in) VendorPivot
    /// library with DevExtreme-style CustomStore semantics.
    ///
    /// This is deliberately the "JSON options" fast path from the lesson: there are NO typed
    /// properties. The page assigns the vendor option object straight to <see cref="Widget.Options"/>
    /// (<c>pivot.Options = new { rowField = "site", columnField = "status", measure = "hours" }</c>)
    /// and the InitScript (wwwroot/pivot-init.js) implements the CustomStore's load() by calling
    /// the page's [WebMethod] <c>LoadPivot</c>. Once the prototype proves the data contract,
    /// promote rowField/columnField/measure to typed properties exactly like <see cref="WorkOrderGrid"/>.
    /// </summary>
    [ToolboxItem(true)]
    [Description("Read-only pivot prototype configured through a JSON option object.")]
    public class WorkOrderPivot : Widget
    {
        public WorkOrderPivot()
        {
            this.Packages.Add(new Package { Name = "vendor-pivot-css", Source = "wwwroot/vendor-pivot.css" });
            this.Packages.Add(new Package { Name = "vendor-pivot", Source = "wwwroot/vendor-pivot.js" });

            this.InitScript = GetResourceString("IntegrationLab.wwwroot.pivot-init.js");
            this.WiredEvents = new[] { "cellClick", "dataLoaded", "error" };
            this.Size = new System.Drawing.Size(640, 160);
        }

        /// <summary>A cross-tab cell was clicked: payload cellClick { rowKey, columnKey, value }.</summary>
        public event EventHandler<PivotCellClickEventArgs> CellClick;

        /// <summary>The vendor rendered a new result: payload dataLoaded { rows, columns, cells }.</summary>
        public event EventHandler<EventArgs> DataLoaded;

        /// <summary>The client adapter caught a vendor/WebMethod failure: error { phase, status, message }.</summary>
        public event EventHandler<DataWidgetErrorEventArgs> WidgetError;

        /// <summary>Raised for every event the pivot sends to the server.</summary>
        [Browsable(false)]
        public event EventHandler<TraceEventArgs> Trace;

        protected override void OnWidgetEvent(WidgetEventArgs e)
        {
            dynamic data = e.Data;
            switch (e.Type)
            {
                case "cellClick":
                    {
                        string rowKey = (string)(data?.rowKey ?? "");
                        string columnKey = (string)(data?.columnKey ?? "");
                        double value = ToDouble((object)data?.value);
                        RaiseTrace(TraceDirection.ClientToServer, "cellClick", $"{{rowKey:\"{rowKey}\",columnKey:\"{columnKey}\",value:{value.ToString(System.Globalization.CultureInfo.InvariantCulture)}}}");
                        if (rowKey.Length == 0 || columnKey.Length == 0 || double.IsNaN(value))
                            return;   // payload outside the contract: dropped
                        CellClick?.Invoke(this, new PivotCellClickEventArgs(rowKey, columnKey, value));
                        break;
                    }
                case "dataLoaded":
                    DataLoaded?.Invoke(this, EventArgs.Empty);
                    break;

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
                    base.OnWidgetEvent(e);
                    break;
            }
        }

        private static double ToDouble(object value)
        {
            try { return value == null ? double.NaN : Convert.ToDouble(value, System.Globalization.CultureInfo.InvariantCulture); }
            catch { return double.NaN; }
        }

        private void RaiseTrace(TraceDirection direction, string name, string payload)
            => Trace?.Invoke(this, new TraceEventArgs(direction, name, payload));
    }
}
