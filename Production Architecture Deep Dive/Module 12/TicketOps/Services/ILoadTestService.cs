using System.Threading.Tasks;

namespace TicketOps.Services
{
    /// <summary>
    /// The simulated load test of the capstone demo: opens fake work units through the same ITicketService a
    /// user would hit, so the count that climbs on the diagnostics page is real work on this node.
    /// The screen paces it with a Timer; the service knows nothing about timers or controls.
    /// </summary>
    public interface ILoadTestService
    {
        int OpenUnits { get; }
        int Target { get; }
        bool IsComplete { get; }

        /// <summary>Resets the counter and announces the run.</summary>
        void Start(int target);

        /// <summary>Opens one work unit; returns how many are open.</summary>
        Task<int> OpenWorkUnitAsync();
    }
}
