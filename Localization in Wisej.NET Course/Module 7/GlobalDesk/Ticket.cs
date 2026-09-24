using System;

namespace GlobalDesk
{
    /// <summary>
    /// What a ticket is doing. A domain value, not a caption.
    ///
    /// The enum is the thing the application reasons about: a report groups by it, a rule fires on
    /// it, a database column stores it, a test asserts on it. None of that survives if the domain
    /// hands out "In progress" instead - which is what happens the first time somebody finds it
    /// convenient to put the display text in the model.
    /// </summary>
    public enum TicketStatus
    {
        New,
        InProgress,
        Waiting,
        Open,
        Closed,
    }

    /// <summary>
    /// A ticket. Note what it does not have: a <c>StatusText</c> property, a
    /// <c>FormattedDueDate</c>, a <c>DisplayTotal</c>. Those are presentation, they depend on who
    /// is looking, and they belong to the UI.
    /// </summary>
    public class Ticket
    {
        public Ticket(string reference, string customer, TicketStatus status, DateTime due, decimal amount)
        {
            Reference = reference;
            Customer = customer;
            Status = status;
            Due = due;
            Amount = amount;
        }

        /// <summary>An identifier. Never translated, never reordered - see Module 5.</summary>
        public string Reference { get; }

        /// <summary>A registered company name. Data, not words: the same characters everywhere.</summary>
        public string Customer { get; }

        public TicketStatus Status { get; }

        public DateTime Due { get; }

        public decimal Amount { get; }

        /// <summary>
        /// The one mapping from domain value to resource key, beside the enum it describes.
        ///
        /// A <c>switch</c> rather than <c>"TicketStatus." + status</c> on purpose: string
        /// concatenation compiles whatever you rename the enum to and fails at run time with a
        /// missing key, while this fails at the point of the change. The <c>default</c> throws
        /// because an unmapped status is a programming error, not a content gap.
        /// </summary>
        public static string ResourceKeyFor(TicketStatus status)
        {
            switch (status)
            {
                case TicketStatus.New: return "TicketStatus.New";
                case TicketStatus.InProgress: return "TicketStatus.InProgress";
                case TicketStatus.Waiting: return "TicketStatus.Waiting";
                case TicketStatus.Open: return "TicketStatus.Open";
                case TicketStatus.Closed: return "TicketStatus.Closed";
                default: throw new ArgumentOutOfRangeException(nameof(status), status, "no resource key for this status");
            }
        }
    }
}
