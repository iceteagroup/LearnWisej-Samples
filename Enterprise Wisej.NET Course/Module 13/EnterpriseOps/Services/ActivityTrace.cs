namespace EnterpriseOps.Services
{
    /// <summary>
    /// The one sink every layer writes its decisions to. The page implements it (→ lstTrace); services and
    /// the sync workflow receive it in their constructor so a reviewer can follow "who decided what" without
    /// opening the designer. Layer prefixes: UI → · Service: · Data: · Security: · Device: · Job:
    /// </summary>
    public interface IActivityTrace
    {
        void Log(string layer, string message);
    }

    public static class TraceLayer
    {
        public const string UI = "UI →";
        public const string Service = "Service:";
        public const string Data = "Data:";
        public const string Security = "Security:";
        public const string Device = "Device:";
        public const string Job = "Job:";
    }
}
