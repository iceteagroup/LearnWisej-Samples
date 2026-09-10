using System;

namespace OrderDesk.Views
{
    /// <summary>
    /// Raised by the shell and its screens so the lab console can log what the migrated code does
    /// without the migrated code knowing about the TracePanel (it will not exist in the product).
    /// </summary>
    public sealed class TraceEventArgs : EventArgs
    {
        public TraceEventArgs(TraceKind kind, string name, string payload)
        {
            Kind = kind;
            Name = name;
            Payload = payload ?? "";
        }

        public TraceKind Kind { get; }
        public string Name { get; }
        public string Payload { get; }
    }
}
