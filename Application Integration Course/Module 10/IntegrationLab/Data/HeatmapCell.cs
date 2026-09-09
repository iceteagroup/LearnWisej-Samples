namespace IntegrationLab.Data
{
    /// <summary>
    /// One reading of the line-load heatmap: a day row, an hour column and a load value (0..100).
    /// This is the only shape that crosses the wire (postback JSON, Call("setCells"), events):
    /// data, never a domain object.
    /// </summary>
    public readonly record struct HeatmapCell(int Day, int Hour, double Value);
}
