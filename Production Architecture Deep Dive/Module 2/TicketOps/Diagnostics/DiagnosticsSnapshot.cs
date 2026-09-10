using System.Collections.Generic;

namespace TicketOps.Diagnostics
{
    /// <summary>One line of a diagnostics panel: what the setting is called and its current value.</summary>
    public sealed class SettingRow
    {
        public string Key { get; }
        public string Value { get; }

        public SettingRow(string key, string value)
        {
            Key = key;
            Value = value ?? "(null)";
        }
    }

    /// <summary>
    /// What the diagnostics page shows: two lists that must never be confused. <see cref="Application"/>
    /// rows are identical in every session; <see cref="Session"/> rows come from the injected SessionContext.
    /// If a "per-session" value ever looks suspiciously global, something is read from the wrong place.
    /// </summary>
    public sealed class DiagnosticsSnapshot
    {
        public IReadOnlyList<SettingRow> Application { get; }
        public IReadOnlyList<SettingRow> Session { get; }

        public DiagnosticsSnapshot(IReadOnlyList<SettingRow> application, IReadOnlyList<SettingRow> session)
        {
            Application = application;
            Session = session;
        }
    }
}
