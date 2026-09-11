using System;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// The server-side diagnostics log. Services, the local store, the queue and the sync workflow receive
    /// it in their constructor so "who decided what" can be followed in the server output without opening
    /// the designer. Layer prefixes: UI → · Service: · Data: · Security: · Device: · Job:
    /// </summary>
    public interface IActivityTrace
    {
        void Log(string layer, string message);
    }

    /// <summary>Writes each line to <see cref="System.Diagnostics.Trace"/> with a timestamp and the layer prefix.</summary>
    public sealed class ActivityTrace : IActivityTrace
    {
        public void Log(string layer, string message)
            => System.Diagnostics.Trace.WriteLine($"{DateTime.Now:HH:mm:ss.fff}  {layer} {message}");
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
