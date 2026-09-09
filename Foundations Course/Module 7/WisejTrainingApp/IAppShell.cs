using WisejTrainingApp.Services;

namespace WisejTrainingApp
{
    /// <summary>
    /// Colour of the status label: green ok, amber warning, red error.
    /// </summary>
    public enum StatusKind { Ok, Warn, Error }

    /// <summary>
    /// What a page (a Views/*View UserControl) is allowed to ask the shell for.
    /// The pages never touch the header, the navigation or the theme directly — they navigate,
    /// log an activity line, set the status label and use the one shared TicketService.
    /// </summary>
    public interface IAppShell
    {
        /// <summary>The one per-session ticket service every page shares (business logic — not changed in Module 7).</summary>
        TicketService Tickets { get; }

        /// <summary>The name of the theme currently loaded (a UI setting, read-only for the pages).</summary>
        string CurrentTheme { get; }

        /// <summary>Swap the content area to the named page: Dashboard · Tickets · Customers · Reports · Settings.</summary>
        void NavigateTo(string page);

        /// <summary>Append a timestamped line to the "Recent activity" list on the Dashboard.</summary>
        void AddActivity(string message);

        /// <summary>Set the text and colour of the status bar at the bottom of the window.</summary>
        void SetStatus(string text, StatusKind kind);
    }

    /// <summary>
    /// A page that recomputes what it shows (metric numbers, the current theme, …) every time the shell navigates to it.
    /// </summary>
    public interface IAppView
    {
        void RefreshView();
    }
}
