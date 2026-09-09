namespace EnterpriseOps.Diagnostics
{
    /// <summary>
    /// The one sink every layer writes to. The page renders it in the "Server · live activity trace" card; a
    /// production build points it at the logger. Each line starts with its layer: <c>UI →</c>, <c>Service:</c>,
    /// <c>Data:</c>, <c>Security:</c>, <c>Audit:</c>, <c>Job:</c> — that is how a reviewer proves the handler
    /// was thin and the service decided.
    /// </summary>
    public interface IActivityTrace
    {
        void Write(string message);
    }
}
