namespace EnterpriseOps.Services
{
    /// <summary>
    /// The diagnostic log every layer writes to — <see cref="ActivityTrace"/> in the app, <see cref="NullTrace"/>
    /// in a test. Lines name their layer: "Service:", "Data:", "Security:", "Audit:".
    /// </summary>
    public interface IActivityTrace
    {
        void Trace(string layer, string message);
    }

    public static class TraceLayer
    {
        public const string Service = "Service:";
        public const string Data = "Data:";
        public const string Security = "Security:";
        public const string Audit = "Audit:";
    }
}
