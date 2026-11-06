using AccessOps.UI;

namespace AccessOps.Pages
{
    /// <summary>The page an area view lives in: where it reports what the server did.</summary>
    internal interface IAreaHost
    {
        void Report(string status, StatusTone tone, string note = null);

        string WireNote { get; set; }
    }
}
