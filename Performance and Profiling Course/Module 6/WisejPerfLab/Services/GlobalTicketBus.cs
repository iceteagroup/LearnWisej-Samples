using System;

namespace WisejPerfLab.Services
{
    /// <summary>
    /// A process-wide notification channel: any screen can announce that a ticket changed, and any
    /// screen can listen. Convenient, and the most common retention root there is.
    /// </summary>
    /// <remarks>
    /// The event is static, so its invocation list is rooted for the lifetime of the process. Every
    /// handler that is never removed keeps its target object — a form, and through the form its controls,
    /// its binding source and every row that binding source holds — alive in the heap of a session that
    /// closed an hour ago. Module 4 finds exactly this path to root in a Memory Usage snapshot comparison.
    /// </remarks>
    public static class GlobalTicketBus
    {
        public static event EventHandler<int> TicketChanged;

        public static void RaiseTicketChanged(int ticketId)
            => TicketChanged?.Invoke(null, ticketId);

        /// <summary>How many handlers are currently subscribed. The leak, as a number on the screen.</summary>
        public static int SubscriberCount => TicketChanged?.GetInvocationList().Length ?? 0;
    }
}
