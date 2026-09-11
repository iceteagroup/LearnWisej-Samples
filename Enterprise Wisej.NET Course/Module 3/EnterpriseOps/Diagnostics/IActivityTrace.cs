namespace EnterpriseOps.Diagnostics
{
    /// <summary>
    /// The diagnostic log sink the services write to. Each line starts with its layer: <c>Session:</c>,
    /// <c>Service:</c>, <c>Data:</c>, <c>Security:</c>, <c>Audit:</c>.
    /// </summary>
    public interface IActivityTrace
    {
        void Write(string message);
    }
}
