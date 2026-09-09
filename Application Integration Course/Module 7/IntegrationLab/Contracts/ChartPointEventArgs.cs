using System;

namespace IntegrationLab.Contracts
{
    /// <summary>
    /// DTO for the <c>pointClicked</c> payload: <c>{ index: number, label: string, value: number }</c>.
    /// A user drill-down intent. The index is a LOOKUP KEY: the server resolves label and value
    /// from its own data and never treats the client copy as authoritative.
    /// Becomes <see cref="Widgets.ChartWidget.PointClicked"/>.
    /// </summary>
    public sealed class ChartPointEventArgs : EventArgs
    {
        public ChartPointEventArgs(int index, string label, double value)
        {
            this.Index = index;
            this.Label = label;
            this.Value = value;
        }

        /// <summary>Zero-based index of the clicked point (validated: 0 ≤ index &lt; Labels.Length).</summary>
        public int Index { get; }

        /// <summary>Category label of the point, taken from the server data at <see cref="Index"/>.</summary>
        public string Label { get; }

        /// <summary>Series value of the point, taken from the server data at <see cref="Index"/>.</summary>
        public double Value { get; }

        public override string ToString() => $"{{ Index={Index}, Label={Label}, Value={Value:0.##} }}";
    }
}
