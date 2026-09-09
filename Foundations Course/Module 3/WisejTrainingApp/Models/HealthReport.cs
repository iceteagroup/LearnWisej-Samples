using System.Collections.Generic;

namespace WisejTrainingApp.Models
{
    /// <summary>What "Run Health Check" on the Dashboard produces.</summary>
    public class HealthReport
    {
        public bool AllHealthy { get; set; }
        public string Summary { get; set; }
        public List<string> Details { get; } = new List<string>();
    }
}
