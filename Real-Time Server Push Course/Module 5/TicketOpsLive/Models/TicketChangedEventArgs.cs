using System;

namespace TicketOpsLive.Models
{
    /// <summary>
    /// The model event of the lab. Ticket is a snapshot of the changed record (the page copies the fields into its
    /// own bound instance, or inserts the snapshot when the id is new). ChangeType is "Added" or "Updated".
    /// </summary>
    public class TicketChangedEventArgs : EventArgs
    {
        public Ticket Ticket { get; set; }
        public string ChangeType { get; set; }
        public string Message { get; set; }
    }

    /// <summary>Raised by the simulator when one of its runs ends (completed, stopped, page disposed or faulted).</summary>
    public class FeedStoppedEventArgs : EventArgs
    {
        public string Operation { get; set; }
        public string Reason { get; set; }
        public bool Faulted { get; set; }
        public int EventsRaised { get; set; }
    }
}
