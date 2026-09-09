namespace WisejTrainingApp.Models
{
    /// <summary>
    /// Safe display data prepared by <see cref="Services.StatusService"/>.
    /// Everything in here may be shown on the page; only three of the values
    /// (Percent, Label, Status) are ever copied into widStatus.Options.
    /// </summary>
    public class StatusInfo
    {
        /// <summary>What the gauge draws (0–100). In this lab it is the system load.</summary>
        public int Percent { get; set; }

        /// <summary>Caption the gauge shows next to the number ("System load").</summary>
        public string Label { get; set; }

        /// <summary>"ok" | "warn" | "error" — decided by the C# rule, never by JavaScript.</summary>
        public string Status { get; set; }

        /// <summary>Open tickets — shown in the native data card only.</summary>
        public int Open { get; set; }

        /// <summary>Closed tickets — shown in the native data card only.</summary>
        public int Closed { get; set; }

        /// <summary>System load in percent — the input of the status rule.</summary>
        public int SystemLoad { get; set; }
    }
}
