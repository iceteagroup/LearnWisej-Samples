namespace WisejTrainingApp.Views
{
    /// <summary>Colour of the status line: green ok, amber warning, red error.</summary>
    public enum StatusKind
    {
        Ok,
        Warn,
        Error,
    }

    /// <summary>
    /// What a screen may ask the shell (Window1) to do. Screens never reach into Window1's controls: they
    /// log through AddActivity, report through SetStatus, and tell the shell when ticket data changed so the
    /// Dashboard cards refresh. Keeping this small is what makes each view testable on its own.
    /// </summary>
    public interface IHelpdeskShell
    {
        /// <summary>One line in the Dashboard's recent-activity card (with a timestamp) — every action goes through here.</summary>
        void AddActivity(string message);

        /// <summary>The shell's bottom status bar.</summary>
        void SetStatus(string text, StatusKind kind);

        /// <summary>Opens a screen by name: "Dashboard", "Tickets", "Customers", "Jobs", "Architecture", "Code Review", "Deployment", "Next Steps".</summary>
        void NavigateTo(string screen);

        /// <summary>Called after create / edit / delete so the Dashboard summary is recomputed.</summary>
        void TicketsChanged();
    }
}
