namespace IntegrationLab.Contracts
{
    /// <summary>
    /// The "selected client-side state" the gauge returns to the server from
    /// <c>await gauge.CallAsync("getSelectedState")</c>.
    /// <para>
    /// A DTO built for the crossing: four numbers and two booleans, named for what the server
    /// needs, serialized in a few dozen bytes and readable in one trace line. It carries no
    /// vendor object, no DOM element, no domain entity.
    /// </para>
    /// <para>
    /// Wire shape (camelCase, as produced by <c>getSelectedState()</c> in gauge-init.js and as
    /// Wisej.NET would serialize this class if it went the other way):
    /// <c>{"value":72,"isAboveThreshold":false,"width":480,"height":244,"isAnimating":false}</c>
    /// </para>
    /// <list type="table">
    ///   <item><term>C# (PascalCase)</term><description>JavaScript (camelCase)</description></item>
    ///   <item><term>Value</term><description>value</description></item>
    ///   <item><term>IsAboveThreshold</term><description>isAboveThreshold</description></item>
    ///   <item><term>Width</term><description>width</description></item>
    ///   <item><term>Height</term><description>height</description></item>
    ///   <item><term>IsAnimating</term><description>isAnimating</description></item>
    /// </list>
    /// </summary>
    public sealed class GaugeStateDto
    {
        /// <summary>The value the vendor gauge is showing right now (may be mid-sweep while animating).</summary>
        public double Value { get; set; }

        /// <summary>True when the shown value is at or above the threshold the server configured.</summary>
        public bool IsAboveThreshold { get; set; }

        /// <summary>Rendered width in CSS pixels.</summary>
        public int Width { get; set; }

        /// <summary>Rendered height in CSS pixels.</summary>
        public int Height { get; set; }

        /// <summary>
        /// True while the needle sweep or the reset animation is running. This is purely
        /// client-side state: the server never owns it, it only asks for it.
        /// </summary>
        public bool IsAnimating { get; set; }
    }
}
