using System.Collections.Generic;

namespace IntegrationLab.Contracts
{
    /// <summary>
    /// What the read-only pivot asks for: which field goes on the rows, which on the
    /// columns, and which measure fills the cells. All three are whitelisted by the store.
    /// </summary>
    public sealed class PivotRequest
    {
        public PivotRequest() { }
        public PivotRequest(string rowField, string columnField, string measure)
        {
            this.RowField = rowField;
            this.ColumnField = columnField;
            this.Measure = measure;
        }

        public string RowField { get; set; } = "site";
        public string ColumnField { get; set; } = "status";

        /// <summary>"hours" (sum of Hours) or "count" (number of work orders).</summary>
        public string Measure { get; set; } = "hours";

        public string ToTraceString() => "{rowField:\"" + this.RowField + "\",columnField:\"" + this.ColumnField + "\",measure:\"" + this.Measure + "\"}";
    }

    /// <summary>
    /// The pivot response. The data itself is the flat JSON array <see cref="Cells"/>
    /// (one {row, column, value} tuple per non-empty cell); the key lists tell the vendor the
    /// row/column order. This is the "JSON array" option from the lesson; an OLAP-style nested
    /// response would carry the same information as nested axes.
    /// </summary>
    public sealed class PivotResult
    {
        public string RowField { get; set; }
        public string ColumnField { get; set; }
        public string Measure { get; set; }
        public List<string> RowKeys { get; set; } = new List<string>();
        public List<string> ColumnKeys { get; set; } = new List<string>();
        public List<PivotCell> Cells { get; set; } = new List<PivotCell>();
    }

    public sealed class PivotCell
    {
        public PivotCell() { }
        public PivotCell(string row, string column, double value) { this.Row = row; this.Column = column; this.Value = value; }

        public string Row { get; set; }
        public string Column { get; set; }
        public double Value { get; set; }
    }
}
