namespace TicketOps.Diagnostics
{
    /// <summary>Builds the two-panel diagnostics view: global application values vs this session's values.</summary>
    public interface IDiagnosticsService
    {
        DiagnosticsSnapshot GetSnapshot();
    }
}
