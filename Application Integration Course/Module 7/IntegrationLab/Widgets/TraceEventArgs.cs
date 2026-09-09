using System;

namespace IntegrationLab.Widgets
{
    public enum TraceDirection { ServerToClient, ClientToServer, Server, Rejected }

    /// <summary>
    /// One line of the "Server WidgetEvent log" shown by the lab UI: every message that
    /// crosses the wire in either direction, every .NET event raised, every rejection.
    /// </summary>
    public class TraceEventArgs : EventArgs
    {
        public TraceEventArgs(TraceDirection direction, string name, string payload)
        {
            this.Direction = direction;
            this.Name = name;
            this.Payload = payload;
        }

        public TraceDirection Direction { get; }
        public string Name { get; }
        public string Payload { get; }
    }
}
