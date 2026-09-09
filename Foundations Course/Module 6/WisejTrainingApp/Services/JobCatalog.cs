using WisejTrainingApp.Models;

namespace WisejTrainingApp.Services
{
    /// <summary>
    /// The "service or helper owns reusable logic" part of the Module 6 pattern: the window knows how to
    /// run a job and show progress; this class knows what the jobs are. Add a step here and the progress
    /// bar, the log and the "step x/y" label all follow — nothing in JobsWindow changes.
    ///
    /// The steps are named after the real helpdesk work the course keeps coming back to (tickets in a
    /// CSV, an export to Excel) so the log reads like a production log, not "step 1, step 2".
    /// </summary>
    public class JobCatalog
    {
        public JobDefinition ImportJob()
        {
            return new JobDefinition
            {
                Name = "Import",
                InputFile = "TicketImport.csv",
                Steps = new[]
                {
                    "Opening TicketImport.csv",
                    "Reading the header row",
                    "Validating rows",
                    "Checking for duplicate ticket ids",
                    "Writing tickets through TicketService",
                    "Refreshing the ticket index",
                },
            };
        }

        public JobDefinition ExportJob()
        {
            return new JobDefinition
            {
                Name = "Export",
                InputFile = "TicketExport.xlsx",
                Steps = new[]
                {
                    "Loading tickets from TicketService",
                    "Applying the current filter",
                    "Formatting rows",
                    "Writing TicketExport.xlsx",
                    "Copying the file to the reports share",
                },
            };
        }
    }
}
