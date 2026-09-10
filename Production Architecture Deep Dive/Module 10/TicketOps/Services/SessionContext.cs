namespace TicketOps.Services
{
    /// <summary>
    /// The per-session context of the console: what THIS operator chose. One instance per session, created by
    /// AppComposition and handed to whoever needs it — never a static, because a static would make one
    /// operator's dark theme everybody's dark theme.
    ///
    /// The theme and the culture are remembered here (and mirrored into Application.Session by the
    /// Infrastructure code) so the dashboard reopens the way the operator left it.
    /// </summary>
    public sealed class SessionContext
    {
        /// <summary>Wisej.NET's id for this session, copied in by AppComposition.</summary>
        public string SessionId { get; set; }

        /// <summary>The theme this session switched to; null until the operator chooses one (the default lives in Default.json).</summary>
        public string Theme { get; set; }

        /// <summary>The culture this session works in (en-US by default).</summary>
        public string Culture { get; set; } = "en-US";
    }
}
