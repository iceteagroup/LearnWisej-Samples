namespace EnterpriseOps.Services
{
    /// <summary>
    /// The live activity trace every layer writes to (the page implements it with lstTrace).
    /// Lines name their layer: "UI →", "Service:", "Data:", "Security:", "Audit:", "Job:".
    /// </summary>
    public interface IActivityTrace
    {
        void Trace(string layer, string message);
    }

    public static class TraceLayer
    {
        public const string UI = "UI →";
        public const string Service = "Service:";
        public const string Data = "Data:";
        public const string Security = "Security:";
        public const string Audit = "Audit:";
        public const string Job = "Job:";
    }
}
