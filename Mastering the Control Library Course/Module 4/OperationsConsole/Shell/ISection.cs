namespace OperationsConsole.Shell
{
    /// <summary>
    /// Implemented by every <c>Sections/*Page</c> UserControl so the shell can name it and refresh it
    /// without knowing which module built it.
    /// </summary>
    public interface ISection
    {
        /// <summary>"Editors", "Layouts", "Lists and Trees", "DataGridView", "Dashboard", "Widgets".</summary>
        string Title { get; }

        /// <summary>The shell's Refresh command (button / ToolBar / ContextMenu) calls this on the visible section.</summary>
        void RefreshSection();
    }
}
