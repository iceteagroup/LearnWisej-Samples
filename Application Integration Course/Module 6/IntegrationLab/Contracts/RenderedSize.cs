namespace IntegrationLab.Contracts
{
    /// <summary>
    /// Result of <c>await gauge.CallAsync("getRenderedSize")</c>: the pixel box the browser
    /// actually laid out for the gauge host element.
    /// <para>
    /// Wire shape (JavaScript names, produced by <c>getRenderedSize()</c> in gauge-init.js):
    /// <c>{"width":480,"height":244}</c>. Two integers and nothing else; the server maps them
    /// into this type by their JavaScript names (<c>result.width</c>, never <c>result.Width</c>).
    /// </para>
    /// <para>
    /// Versioning: adding a property is safe (the mapper ignores what it does not read);
    /// renaming or removing one is a breaking change of the client/server contract.
    /// </para>
    /// </summary>
    public sealed class RenderedSize
    {
        /// <summary>Rendered width in CSS pixels (rounded).</summary>
        public int Width { get; set; }

        /// <summary>Rendered height in CSS pixels (rounded).</summary>
        public int Height { get; set; }

        public override string ToString() => $"{Width}×{Height}";
    }
}
