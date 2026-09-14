namespace WisejPerfLab.Shell
{
    /// <summary>The state the status line reports after a scenario.</summary>
    public enum ShellState
    {
        Idle,
        Busy,
        Ok,
        Warn,
        Fault
    }

    /// <summary>
    /// What a tab page may ask the shell for: the status line, the failure banner, and the budget check
    /// that decides whether a measured scenario passed.
    /// </summary>
    public interface IPerfLabShell
    {
        void SetStatus(ShellState state, string text);

        /// <summary>Shows a friendly failure message. A scenario that fails never leaves the screen blank.</summary>
        void ShowBanner(string message);

        void ClearBanner();
    }
}
