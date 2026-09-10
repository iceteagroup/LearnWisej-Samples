namespace TicketOps.Domain
{
    /// <summary>
    /// The canonical, signed permalink for a work order. Built on the server by <c>ITicketLinkService</c>;
    /// the browser receives the finished string and only writes it to the clipboard. The client never
    /// assembles a URL and never sees the signing key.
    /// </summary>
    public sealed class TicketLink
    {
        public int WorkOrderId { get; }
        public string Url { get; }

        public TicketLink(int workOrderId, string url)
        {
            WorkOrderId = workOrderId;
            Url = url;
        }

        public override string ToString() => Url;
    }
}
