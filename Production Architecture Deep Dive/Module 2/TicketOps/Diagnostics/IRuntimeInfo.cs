namespace TicketOps.Diagnostics
{
    /// <summary>
    /// The handful of facts the diagnostics page needs from the Wisej.NET runtime (Application.*), behind an
    /// interface so <see cref="DiagnosticsService"/> stays free of Wisej types and testable. Infrastructure
    /// implements it (<c>WisejRuntimeInfo</c>); a test can return fixed values.
    /// </summary>
    public interface IRuntimeInfo
    {
        /// <summary>Application.SessionCount — sessions alive in this process right now.</summary>
        int? ActiveSessionCount { get; }

        /// <summary>Default.json "sessionTimeout" (seconds), via Application.Configuration.</summary>
        int? ConfiguredSessionTimeoutSeconds { get; }

        /// <summary>Default.json "theme", via Application.Configuration — the default for everyone.</summary>
        string ConfiguredThemeName { get; }

        /// <summary>Application.Theme.Name — the theme this session is actually rendering with.</summary>
        string ActiveThemeName { get; }

        /// <summary>Application.ActiveProfile.Name / Application.Browser.Device for this session.</summary>
        string ActiveClientProfile { get; }

        /// <summary>Application.ServerName:ServerPort.</summary>
        string Server { get; }
    }
}
