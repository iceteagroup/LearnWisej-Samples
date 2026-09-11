using System;

namespace IntegrationLab.Widgets
{
    /// <summary>
    /// Data for <c>DataLoaded</c>: the vendor grid finished a fetch. Everything here
    /// was copied from the event payload the client widget sent (data, never behavior).
    /// </summary>
    public class DataLoadedEventArgs : EventArgs
    {
        public DataLoadedEventArgs(int count, int total, int page, int pages, int elapsed)
        {
            this.Count = count;
            this.Total = total;
            this.Page = page;
            this.Pages = pages;
            this.Elapsed = elapsed;
        }

        /// <summary>Rows in the page that was rendered.</summary>
        public int Count { get; }

        /// <summary>Rows in the whole dataset, as reported by the endpoint.</summary>
        public int Total { get; }

        public int Page { get; }
        public int Pages { get; }

        /// <summary>Client-measured round trip in milliseconds.</summary>
        public int Elapsed { get; }
    }

    /// <summary>
    /// Data for <c>WidgetError</c>: the vendor grid could not load (HTTP failure,
    /// rejected WebMethod call, or a vendor exception caught by the adapter).
    /// </summary>
    public class GridErrorEventArgs : EventArgs
    {
        public GridErrorEventArgs(int status, string message, string phase)
        {
            this.Status = status;
            this.Message = message;
            this.Phase = phase;
        }

        /// <summary>HTTP status code, or 0 when the failure was not an HTTP response.</summary>
        public int Status { get; }
        public string Message { get; }
        public string Phase { get; }
    }

    public class RowClickedEventArgs : EventArgs
    {
        public RowClickedEventArgs(string id) { this.Id = id; }
        public string Id { get; }
    }

    public enum TraceDirection { ServerToClient, ClientToServer, Http }

    /// <summary>One request, response or call, for the page's Network list.</summary>
    public class TraceEventArgs : EventArgs
    {
        public TraceEventArgs(TraceDirection direction, string name, string payload)
        {
            this.Direction = direction;
            this.Name = name;
            this.Payload = payload;
        }

        public TraceDirection Direction { get; }
        public string Name { get; }
        public string Payload { get; }
    }
}
