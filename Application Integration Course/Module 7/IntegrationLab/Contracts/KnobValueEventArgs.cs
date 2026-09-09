using System;

namespace IntegrationLab.Contracts
{
    /// <summary>
    /// DTO for the <c>valueChanged</c> payload: <c>{ value: number, source: "user" | "server" }</c>.
    /// The server-owned knob state changed; <see cref="Source"/> says who caused it.
    /// Handled by the page-level <c>WidgetEvent</c> handler (<c>knob_WidgetEvent</c>).
    /// </summary>
    public sealed class KnobValueEventArgs : EventArgs
    {
        public KnobValueEventArgs(double value, string source)
        {
            this.Value = value;
            this.Source = source;
        }

        /// <summary>The new value (validated: finite, within Minimum..Maximum, on the Step grid).</summary>
        public double Value { get; }

        /// <summary>"user" when the dial was dragged, "server" when the change echoed a server Options change.</summary>
        public string Source { get; }

        public bool IsUserChange => this.Source == "user";

        public override string ToString() => $"{{ Value={Value:0.##}, Source={Source} }}";
    }
}
