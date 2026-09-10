namespace TicketOps.Domain
{
    public enum WorkOrderPriority
    {
        Low,
        Medium,
        High
    }

    /// <summary>
    /// The work order record and its own rule. Plain C# with no UI and no Wisej.NET dependency.
    ///
    /// The rule that matters for Module 9 is <see cref="CanShareLink"/>: whether a link to this work order
    /// may be handed out. It is a business decision, so it lives here — never in the JavaScript that
    /// writes the link to the clipboard. The browser asks; this method answers.
    /// </summary>
    public sealed class WorkOrder
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public WorkOrderPriority Priority { get; set; }
        public string AssignedTo { get; set; }

        /// <summary>Confidential work orders (HR investigations, security incidents) must not be shared by link.</summary>
        public bool Confidential { get; set; }

        /// <summary>
        /// Decides whether <paramref name="user"/> may share a permalink to this work order.
        /// Unit-testable without a browser; reused by the single copy action and by the batch verification.
        /// </summary>
        public bool CanShareLink(SessionUser user, out string reason)
        {
            if (user == null)
            {
                reason = "Sign in before sharing links.";
                return false;
            }

            if (Confidential && !user.IsManager)
            {
                reason = "Confidential work orders cannot be shared by link.";
                return false;
            }

            reason = null;
            return true;
        }

        public override string ToString() => $"#{Id} {Title}";
    }
}
