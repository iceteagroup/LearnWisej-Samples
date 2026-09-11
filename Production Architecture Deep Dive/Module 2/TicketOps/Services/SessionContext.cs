using System;

namespace TicketOps.Services
{
    /// <summary>
    /// The per-session values the TicketOps screens and services keep reaching for — one instance
    /// <b>per session</b>, never static, never shared.
    ///
    /// It is a plain object (no Wisej.NET type): AppComposition creates exactly one per browser session
    /// and hands it to every Form and service through their constructors. A second tab is a second
    /// session and gets a second instance, so nothing one tab writes here is visible to the other —
    /// the same isolation two launches of a desktop app enjoy.
    ///
    /// What does NOT belong here: durable business data (tickets live in the repository) and anything
    /// that is the same for everyone (that is <c>AppSettings</c>, read once per process).
    /// </summary>
    public sealed class SessionContext
    {
        /// <summary>Wisej.NET's id for this session (Application.SessionId), copied in by AppComposition.</summary>
        public string SessionId { get; set; }

        /// <summary>The operator signed in to this instance — the user scope attached to the session, not the session itself.</summary>
        public string CurrentUser { get; set; }

        /// <summary>The tenant this console currently works in.</summary>
        public string Tenant { get; set; }

        /// <summary>The theme this session switched to (the app-wide default lives in configuration).</summary>
        public string Theme { get; set; }

        /// <summary>The responsive client profile Wisej.NET resolved for this browser (Application.ActiveProfile).</summary>
        public string ClientProfile { get; set; }

        /// <summary>Working state: the ticket selected in this console. Static in the junior app — the classic leak.</summary>
        public int? SelectedTicketId { get; set; }

        public DateTime StartedUtc { get; } = DateTime.UtcNow;
    }
}
