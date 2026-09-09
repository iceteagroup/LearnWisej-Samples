using WisejTrainingApp.Models;

namespace WisejTrainingApp.Services
{
    /// <summary>
    /// The "Run Health Check" quick action behind the Dashboard's System Status card.
    /// Simulated: three services are pinged; every third run the email gateway answers slowly,
    /// so the card can show a warning state as well as the green one.
    /// </summary>
    public class HealthCheckService
    {
        private int runs;

        public int Runs => runs;

        public HealthReport Run(int openTickets)
        {
            runs++;
            bool emailSlow = runs % 3 == 0;

            var report = new HealthReport();
            report.Details.Add("Database ........ OK (12 ms)");
            report.Details.Add("Ticket API ...... OK (38 ms)");
            report.Details.Add(emailSlow ? "Email gateway ... SLOW (1,240 ms)" : "Email gateway ... OK (95 ms)");
            report.Details.Add($"Open tickets .... {openTickets}");

            report.AllHealthy = !emailSlow;
            report.Summary = emailSlow ? "Degraded: email gateway slow" : "All systems operational";
            return report;
        }
    }
}
