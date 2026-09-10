namespace OperationsConsole.Shell
{
    /// <summary>
    /// Optional companion to <see cref="ISection"/>, added in Module 3.
    /// <para>
    /// A section that actually holds records implements it so the shell's StatusBar can show a real record
    /// count and the ToolBar's <b>New</b> and <b>Save</b> commands have something to act on. A section that does
    /// not implement it is not broken — the shell simply answers "— records" and "nothing to save", which is what
    /// the Module 2 / 4–7 placeholders do until their own module fills them in.
    /// </para>
    /// <para>
    /// This is the same idea as <see cref="ISection"/>: the shell talks to a small, named contract instead of to
    /// a concrete page type, so no section ever appears by name in <c>MainPage</c>.
    /// </para>
    /// </summary>
    public interface ISectionRecords
    {
        /// <summary>How many records the section currently holds (shown in the StatusBar).</summary>
        int RecordCount { get; }

        /// <summary>Creates a record and returns its stable ID (the shell shows it as the selected record).</summary>
        string NewRecord();

        /// <summary>Saves the section and says what happened, in words a user can read.</summary>
        SectionSaveResult Save();
    }

    /// <summary>
    /// The answer of <see cref="ISectionRecords.Save"/>: accepted or not, one message for the user and the
    /// status level the shell should use. Sections never format the StatusBar themselves.
    /// </summary>
    public sealed class SectionSaveResult
    {
        private SectionSaveResult(bool accepted, string message, StatusLevel level)
        {
            Accepted = accepted;
            Message = message;
            Level = level;
        }

        /// <summary>True when the save went through.</summary>
        public bool Accepted { get; }

        /// <summary>What to tell the user — no exception text, no internals.</summary>
        public string Message { get; }

        /// <summary>Green, amber or red.</summary>
        public StatusLevel Level { get; }

        /// <summary>The success path.</summary>
        public static SectionSaveResult Ok(string message) => new SectionSaveResult(true, message, StatusLevel.Ok);

        /// <summary>The validation path: the section refused, and the user can fix it.</summary>
        public static SectionSaveResult Rejected(string message) => new SectionSaveResult(false, message, StatusLevel.Warning);

        /// <summary>The error path: the service failed and the user cannot fix it from here.</summary>
        public static SectionSaveResult Failed(string message) => new SectionSaveResult(false, message, StatusLevel.Error);
    }
}
