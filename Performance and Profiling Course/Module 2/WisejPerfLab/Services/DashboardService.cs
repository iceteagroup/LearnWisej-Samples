using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WisejPerfLab.Data;
using WisejPerfLab.Data.Entities;

namespace WisejPerfLab.Services
{
    /// <summary>
    /// The data the dashboard refresh needs. In this module it only reads: the counting, the formatting
    /// and the control work all still live in <c>DashboardPage</c>, which is where Module 3 finds them.
    /// </summary>
    public sealed class DashboardService
    {
        private readonly IDbContextFactory<PerfLabContext> _dbFactory;

        public DashboardService(IDbContextFactory<PerfLabContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        /// <summary>
        /// Every ticket, as tracked entities with every column, ordered by last update.
        /// </summary>
        /// <remarks>
        /// The KPIs are counts over the whole ticket population, so the obvious implementation loads the
        /// whole ticket population and counts it in memory. It is correct, it is one statement, and it
        /// carries fifty thousand entities with two long text columns each into this session to produce
        /// three numbers.
        /// </remarks>
        public IReadOnlyList<Ticket> LoadAllTickets()
        {
            using var db = _dbFactory.CreateDbContext();

            return db.Tickets
                .OrderByDescending(t => t.UpdatedAt)
                .ToList();
        }
    }
}
