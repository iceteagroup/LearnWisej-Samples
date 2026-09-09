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

        public static void Add(string message) => Shell?.AddLog(message);

        public static void Status(string text, StatusLevel level = StatusLevel.Ok) => Shell?.SetStatus(text, level);
    }
}
