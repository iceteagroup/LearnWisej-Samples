namespace FieldOps
{
    /// <summary>
    /// The shell this server ships. It MUST equal the SHELL constant at the top of sw.js and is
    /// changed in the same commit as any shell file (Default.html, offline.html, the manifest,
    /// an icon). The list page compares it with the cache the browser holds at session start.
    /// </summary>
    internal static class ShellVersion
    {
        public const string Current = "fieldops-shell-v1";
    }
}
