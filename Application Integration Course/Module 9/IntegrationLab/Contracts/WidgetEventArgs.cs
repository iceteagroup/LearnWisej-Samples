using System;
using System.Collections.Generic;

namespace IntegrationLab.Contracts
{
    /// <summary>
    /// Data for <c>WorkOrderGrid.CellClick</c>. Copied from the validated
    /// <c>cellClick { rowKey, field, value }</c> widget payload (data, never behavior).
    /// </summary>
    public sealed class CellClickEventArgs : EventArgs
    {
        public CellClickEventArgs(string rowKey, string field, object value)
        {
            this.RowKey = rowKey;
            this.Field = field;
            this.Value = value;
        }

        public string RowKey { get; }
        public string Field { get; }
        public object Value { get; }
    }

    /// <summary>
    /// Data for <c>WorkOrderGrid.RowUpdated</c>: the user committed an inline edit in the
    /// browser. Payload <c>rowUpdated { rowKey, changes }</c>. The persisted values are the
    /// ones the "update" handler accepted, which may differ from these.
    /// </summary>
    public sealed class RowUpdatedEventArgs : EventArgs
    {
        public RowUpdatedEventArgs(string rowKey, IReadOnlyDictionary<string, object> changes)
        {
            this.RowKey = rowKey;
            this.Changes = changes;
        }

        public string RowKey { get; }
        public IReadOnlyDictionary<string, object> Changes { get; }
    }

    /// <summary>Data for <c>WorkOrderPivot.CellClick</c>: payload <c>cellClick { rowKey, columnKey, value }</c>.</summary>
    public sealed class PivotCellClickEventArgs : EventArgs
    {
        public PivotCellClickEventArgs(string rowKey, string columnKey, double value)
        {
            this.RowKey = rowKey;
            this.ColumnKey = columnKey;
            this.Value = value;
        }

        public string RowKey { get; }
        public string ColumnKey { get; }
        public double Value { get; }
    }

    /// <summary>
    /// The client adapter caught a vendor or transport failure and reported it as one
    /// <c>error { phase, status, message }</c> event instead of crashing the page.
    /// </summary>
    public sealed class DataWidgetErrorEventArgs : EventArgs
    {
        public DataWidgetErrorEventArgs(string phase, int status, string message)
        {
            this.Phase = phase;
            this.Status = status;
            this.Message = message;
        }

        /// <summary>init, update, read, create, destroy, load, deleteSelected...</summary>
        public string Phase { get; }

        /// <summary>HTTP status when the failure came from a server handler, else 0.</summary>
        public int Status { get; }

        public string Message { get; }
    }

    public enum TraceDirection { ServerToClient, ClientToServer }

    /// <summary>One line of the page's Remote operations list.</summary>
    public sealed class TraceEventArgs : EventArgs
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
