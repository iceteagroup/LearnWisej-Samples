using Wisej.Web;

namespace OperationsConsole.Shell
{
    /// <summary>
    /// What sections, editors and services call to update the shell's status area.
    /// Sections never hold a reference to <c>MainPage</c>: this gateway resolves the current session's page.
    /// </summary>
    public static class ShellStatus
    {
        static IConsoleShell Shell => Application.MainPage as IConsoleShell;

        /// <summary>Status text, coloured by level.</summary>
        public static void Show(string text, StatusLevel level = StatusLevel.Ok) => Shell?.SetStatus(text, level);

        /// <summary>Diagnostic panel: the control that was used last (pass <c>control.Name</c>).</summary>
        public static void Control(string controlName) => Shell?.SetSelectedControl(controlName);

        /// <summary>Diagnostic panel: the stable ID of the selected record (null or "" shows "—").</summary>
        public static void Record(string recordId) => Shell?.SetSelectedRecord(recordId);
    }
}
