using System;
using OrderDesk.Domain;

namespace OrderDesk.Services
{
    /// <summary>
    /// Everything LegacyOrderDesk kept in static AppState fields, as ONE typed object that lives
    /// in the browser session (see <see cref="SessionContext"/>). Plain data: no Wisej reference,
    /// so it is testable and could be serialized into a distributed session store later.
    /// </summary>
    public sealed class UserSessionContext
    {
        /// <summary>The sign-in name (kelly, sam). Null until the user signs in.</summary>
        public string UserName { get; set; }

        /// <summary>Display name for the title bar.</summary>
        public string DisplayName { get; set; }

        /// <summary>The company the user signed in for (was AppState.CurrentCompany).</summary>
        public string Company { get; set; }

        /// <summary>The customer selected in the Orders screen (was the static AppState.CurrentCustomer).</summary>
        public int? CurrentCustomerId { get; set; }

        /// <summary>The status filter of the Orders screen (was the static AppState.CurrentFilter).</summary>
        public OrderStatus? CurrentFilter { get; set; }

        /// <summary>The last free-text search (was the static AppState.LastSearch).</summary>
        public string LastSearch { get; set; }

        /// <summary>The culture negotiated for this browser (Application.CurrentCulture at sign-in).</summary>
        public string Culture { get; set; }

        /// <summary>When this session signed in — null while anonymous.</summary>
        public DateTime? SignedInAt { get; set; }

        public bool IsSignedIn => !string.IsNullOrEmpty(UserName);
    }
}
