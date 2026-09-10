using System;
using OrderDesk.Domain;
using OrderDesk.Legacy;

namespace OrderDesk.Services
{
    /// <summary>
    /// The two places the migrated screen can keep "who is signed in, which customer, which filter".
    /// The page writes through whichever store is active so the legacy behaviour and the web-safe
    /// behaviour can be compared side by side in the same session.
    /// </summary>
    public interface IStateStore
    {
        /// <summary>"AppState (static)" or "UserContext (Application.Session)".</summary>
        string Name { get; }
        /// <summary>Where the values physically live — shown in the State store card.</summary>
        string Location { get; }
        User User { get; set; }
        Customer Customer { get; set; }
        OrderStatus? Filter { get; set; }
    }

    /// <summary>✕ The desktop way: process-wide statics. One slot for every browser session.</summary>
    public sealed class LegacyStaticStore : IStateStore
    {
        public string Name => "AppState (static)";
        public string Location => "static fields · one slot per PROCESS · shared by all sessions";

        public User User { get => AppState.CurrentUser; set => AppState.CurrentUser = value; }
        public Customer Customer { get => AppState.CurrentCustomer; set => AppState.CurrentCustomer = value; }

        public OrderStatus? Filter
        {
            get => Enum.TryParse<OrderStatus>(AppState.ActiveFilter, out var s) ? s : (OrderStatus?)null;
            set => AppState.ActiveFilter = value?.ToString();
        }
    }

    /// <summary>✓ The web way: a typed context in Application.Session. One object per browser session.</summary>
    public sealed class SessionContextStore : IStateStore
    {
        public string Name => "UserContext (Application.Session)";
        public string Location => UserContext.Current.Handle + " · one object per SESSION";

        public User User { get => UserContext.Current.User; set => UserContext.Current.User = value; }
        public Customer Customer { get => UserContext.Current.CurrentCustomer; set => UserContext.Current.CurrentCustomer = value; }
        public OrderStatus? Filter { get => UserContext.Current.ActiveFilter; set => UserContext.Current.ActiveFilter = value; }
    }
}
