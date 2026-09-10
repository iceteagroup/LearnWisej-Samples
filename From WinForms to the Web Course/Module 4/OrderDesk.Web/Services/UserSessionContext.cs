using System;
using OrderDesk.Domain;

namespace OrderDesk.Services
{
    /// <summary>
    /// ✓ Everything LegacyOrderDesk kept in static AppState fields, as ONE typed object that lives
    /// in the browser session (see <see cref="SessionContext"/>). Plain data: no Wisej reference,
    /// so it is testable and could be serialized into a distributed session store later.
    ///
    /// The lesson's rule decided what belongs here: "would the value differ if two users opened
    /// the app at the same time?" — user, company, current customer, filter, last search: yes.
    /// </summary>
    public sealed class UserSessionContext
    {
        /// <summary>The sign-in name (kelly, sam). Null until the user signs in.</summary>
        public string UserName { get; set; }

        /// <summary>Display name for the title bar and the trace.</summary>
        public string DisplayName { get; set; }

        /// <summary>The company the user signed in for (LegacyOrderDesk: AppState.CurrentCompany).</summary>
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

        /// <summary>
        /// One-line summary for the console and the trace. An anonymous context that still carries a
        /// filter or a customer (a legacy static written before sign-in, or left behind after a sign-out)
        /// says so instead of hiding it behind "(not signed in)".
        /// </summary>
        public string Describe(CustomerService customers)
        {
            string customer = CurrentCustomerId.HasValue ? customers.Find(CurrentCustomerId.Value)?.Name ?? "?" : "all customers";
            string filter = CurrentFilter.HasValue ? CurrentFilter.Value.ToString() : "All";
            if (!IsSignedIn)
                return CurrentCustomerId.HasValue || CurrentFilter.HasValue ? $"(not signed in) · {customer} · filter {filter}" : "(not signed in)";
            return $"{UserName} · {Company} · {customer} · filter {filter}";
        }
    }
}
