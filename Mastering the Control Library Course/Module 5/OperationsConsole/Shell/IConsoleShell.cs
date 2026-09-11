namespace OperationsConsole.Shell
{
    /// <summary>
    /// Colour of the status area: green, amber or red.
    /// </summary>
    public enum StatusLevel { Ok, Warning, Error }

    /// <summary>
    /// The shell contract sections and services talk to (through <see cref="ShellStatus"/>).
    /// Implemented by <c>MainPage</c>; per session, <c>Application.MainPage</c> is this session's page.
    /// </summary>
    public interface IConsoleShell
    {
        /// <summary>Sets the status text, coloured by level.</summary>
        void SetStatus(string text, StatusLevel level);

        /// <summary>Diagnostic panel: the name of the control the user used last.</summary>
        void SetSelectedControl(string controlName);

        /// <summary>Diagnostic panel: the stable ID of the selected record ("—" when nothing is selected).</summary>
        void SetSelectedRecord(string recordId);
    }
}
