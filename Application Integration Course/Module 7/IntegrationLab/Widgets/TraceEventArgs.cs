using System;

namespace IntegrationLab.Widgets
{
    public enum TraceDirection { ClientToServer, Rejected }

    /// <summary>
    /// One line of the "Server WidgetEvent log": a payload received from the client, or the
    /// reason the server rejected it.
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
