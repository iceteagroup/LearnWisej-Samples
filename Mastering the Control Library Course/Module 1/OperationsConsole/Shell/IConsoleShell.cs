namespace OperationsConsole.Shell
{
    /// <summary>
    /// Colour of the status area: green, amber or red.
    /// </summary>
    public enum StatusLevel { Ok, Warning, Error }

    /// <summary>
    /// The shell contract every section, editor and service talks to (through <see cref="ConsoleLog"/>).
    /// Implemented by <c>MainPage</c>; per session, <c>Application.MainPage</c> is this session's page.
    /// Defined in Module 1 and never changed afterwards.
    /// </summary>
    public interface IConsoleShell
    {
        /// <summary>Appends "HH:mm:ss  message" to the Event log (<c>lstEventLog</c>) and selects the last item.</summary>
        void AddLog(string message);

        /// <summary>Sets the status text (<c>statusLabel</c> in Module 1–2, the StatusBar panel from Module 3) coloured by level.</summary>
        void SetStatus(string text, StatusLevel level);
    }
}
