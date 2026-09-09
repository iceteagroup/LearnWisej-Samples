namespace WisejTrainingApp.Models
{
    /// <summary>
    /// What a background job looks like before it runs: a name the log and the status use,
    /// the file it works on (shown in lblJobInfo) and the ordered steps RunJobAsync walks through.
    /// This is plain data — no UI, no threading — so it can live in Models/.
    /// </summary>
    public class JobDefinition
    {
        public string Name { get; set; }          // "Import" | "Export" — appears in every log line
        public string InputFile { get; set; }     // "TicketImport.csv" | "TicketExport.xlsx"
        public string[] Steps { get; set; }       // the step names, in order; step numbers are 1-based
    }
}
