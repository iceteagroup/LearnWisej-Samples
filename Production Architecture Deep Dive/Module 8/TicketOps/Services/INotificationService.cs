using TicketOps.Domain;

namespace TicketOps.Services
{
    /// <summary>
    /// Tells an operator something happened. Lifetime: Transient — a stateless, lightweight helper; a new
    /// instance is created every time it is resolved, so nothing it holds can leak between callers.
    /// </summary>
    public interface INotificationService
    {
        Notification Notify(int operatorId, string message);
    }
}
