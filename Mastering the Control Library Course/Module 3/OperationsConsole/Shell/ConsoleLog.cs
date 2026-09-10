using Wisej.Web;

namespace OperationsConsole.Shell
{
    /// <summary>
    /// What every section, editor and service calls to report to the shell.
    /// Sections never hold a reference to <c>MainPage</c>: they go through this static gateway,
    /// which resolves the current session's page (<see cref="Application.MainPage"/>).
    /// </summary>
    public static class ConsoleLog
    {
        static IConsoleShell Shell => Application.MainPage as IConsoleShell;

        /// <summary>Event log line: "HH:mm:ss  message".</summary>
        public static void Add(string message) => Shell?.AddLog(message);

        /// <summary>Status area text, coloured by level.</summary>
        public static void Status(string text, StatusLevel level = StatusLevel.Ok) => Shell?.SetStatus(text, level);

        /// <summary>Diagnostic panel: the control that was used last (pass <c>control.Name</c>).</summary>
        public static void Control(string controlName) => Shell?.SetSelectedControl(controlName);

        /// <summary>Diagnostic panel: the stable ID of the selected record (null or "" shows "—").</summary>
        public static void Record(string recordId) => Shell?.SetSelectedRecord(recordId);
    }
}
