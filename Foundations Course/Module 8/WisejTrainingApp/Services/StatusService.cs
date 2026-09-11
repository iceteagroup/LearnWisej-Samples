using System;
using WisejTrainingApp.Models;

namespace WisejTrainingApp.Services
{
    public class StatusService
    {
        // Credentials such as a monitoring API key or a database connection string would be used
        // here, on the server. They are never copied into StatusInfo, so they never reach the widget.

        private readonly Random random = new Random();
        private int open = 12;
        private int closed = 47;
        private int systemLoad = 42;

        // Only the simple display values the widget needs.
        public StatusInfo GetStatus()
        {
            return new StatusInfo
            {
                Percent = systemLoad,
                Label = "System load",
                Status = StatusFor(systemLoad),
                Open = open,
                Closed = closed,
                SystemLoad = systemLoad,
            };
        }

        // Takes a new (simulated) reading.
        public void Refresh()
        {
            systemLoad = random.Next(20, 95);
            open = random.Next(5, 20);
            closed++;
        }

        public StatusInfo SetHealthy()
        {
            systemLoad = 35;
            return GetStatus();
        }

        public StatusInfo SetWarning()
        {
            systemLoad = 72;
            return GetStatus();
        }

        public StatusInfo SetCritical()
        {
            systemLoad = 93;
            return GetStatus();
        }

        // The business rule stays in C#: the widget only picks a colour for the status it receives.
        private static string StatusFor(int load)
        {
            if (load > 85) return "error";
            if (load >= 60) return "warn";
            return "ok";
        }
    }
}
