using System;
using System.Threading.Tasks;
using TicketOps.Domain;
using TicketOps.Infrastructure;

namespace TicketOps.Services
{
    /// <summary>
    /// Opens "work units" — generated tickets saved through ITicketService — and counts them. Because every unit
    /// travels UI → SVC → DATA like a real save, the health probe that follows a run reports the rows it added,
    /// which is the evidence the demo script asks for: load went through, the node stayed Healthy.
    /// </summary>
    public sealed class LoadTestService : ILoadTestService
    {
        private readonly ITicketService _tickets;
        private readonly ILog _log;

        public int OpenUnits { get; private set; }
        public int Target { get; private set; }
        public bool IsComplete => Target > 0 && OpenUnits >= Target;

        public LoadTestService(ITicketService tickets, ILog log)
        {
            _tickets = tickets ?? throw new ArgumentNullException(nameof(tickets));
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public void Start(int target)
        {
            if (target <= 0) throw new ArgumentOutOfRangeException(nameof(target));
            Target = target;
            OpenUnits = 0;
            _log.Info(LogLayer.Service, "LoadTestService.Start", $"simulated load: {target} work units through ITicketService.SaveAsync — the same path a user takes");
        }

        public async Task<int> OpenWorkUnitAsync()
        {
            int n = OpenUnits + 1;
            var draft = new TicketDraft
            {
                Title = $"Load test unit {n:000}",
                Priority = (TicketPriority)(n % 3),
                HoursLogged = 0
            };

            var result = await _tickets.SaveAsync(draft);
            if (!result.Succeeded)
                throw new InvalidOperationException("Generated work unit was rejected: " + result.Message);

            OpenUnits = n;
            if (n % 10 == 0 || n == Target)
                _log.Info(LogLayer.Service, "LoadTestService.OpenWorkUnitAsync", $"{n}/{Target} work units open");
            return OpenUnits;
        }
    }
}
