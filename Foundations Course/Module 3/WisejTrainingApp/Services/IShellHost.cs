using System.Collections.Generic;

namespace WisejTrainingApp.Services
{
    /// <summary>
    /// What a view is allowed to ask of the shell. The views never touch lblStatus, lblBreadcrumb or
    /// pnlContent themselves — they report through Log() and the shell decides how to show it.
    /// MainPage implements this; the views only know the interface.
    /// </summary>
    public interface IShellHost
    {
        /// <summary>The role the user picked in the header (Support Agent / Manager).</summary>
        string CurrentRole { get; }

        /// <summary>Writes a timestamped line to the status bar and to the recent-activity list.</summary>
        void Log(string message, LogKind kind = LogKind.Info);

        /// <summary>Everything logged in this session, oldest first — DashboardView shows it.</summary>
        IReadOnlyList<string> Activity { get; }
    }
}
