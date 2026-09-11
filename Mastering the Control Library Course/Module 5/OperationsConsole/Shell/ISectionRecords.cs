namespace OperationsConsole.Shell
{
    /// <summary>
    /// Optional companion to <see cref="ISection"/>: a section that holds records implements it so the StatusBar can
    /// show a record count and the ToolBar's New and Save commands have something to act on. A section that does not
    /// implement it simply shows "— records" and "nothing to save".
    /// </summary>
    public interface ISectionRecords
    {
        /// <summary>How many records the section currently holds.</summary>
        int RecordCount { get; }

        /// <summary>Creates a record and returns its stable ID.</summary>
        string NewRecord();

        /// <summary>Saves the section and says what happened, in words a user can read.</summary>
        SectionSaveResult Save();
    }

    /// <summary>
    /// The answer of <see cref="ISectionRecords.Save"/>: accepted or not, one message for the user and the status
    /// level the shell should use.
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

        /// <summary>What to tell the user.</summary>
        public string Message { get; }

        /// <summary>Green, amber or red.</summary>
        public StatusLevel Level { get; }

        /// <summary>The save went through.</summary>
        public static SectionSaveResult Ok(string message) => new SectionSaveResult(true, message, StatusLevel.Ok);

        /// <summary>The section refused, and the user can fix it.</summary>
        public static SectionSaveResult Rejected(string message) => new SectionSaveResult(false, message, StatusLevel.Warning);

        /// <summary>The service failed and the user cannot fix it from here.</summary>
        public static SectionSaveResult Failed(string message) => new SectionSaveResult(false, message, StatusLevel.Error);
    }
}
