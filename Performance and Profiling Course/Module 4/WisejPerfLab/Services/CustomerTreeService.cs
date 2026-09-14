using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WisejPerfLab.Data;
using WisejPerfLab.Models;

namespace WisejPerfLab.Services
{
    /// <summary>The customer nodes of one tree load, with the statement count it took to produce them.</summary>
    public sealed class CustomerTreeResult
    {
        public IReadOnlyList<CustomerNodeRow> Nodes { get; set; }

        public int QueryCount { get; set; }
    }

    /// <summary>
    /// The customer hierarchy. Written the way a tree usually is the first time: load the whole table,
    /// ask the database how many tickets each node has, and build every node before the user has opened
    /// anything.
    /// </summary>
    /// <remarks>
    /// Three thousand nodes is not much data. It is three thousand count queries, three thousand
    /// server-side control objects held for the life of the session, and three thousand entries in the
    /// update the browser has to apply — for a tree the user opens two branches of. Module 5 replaces it
    /// with one grouped count query, root nodes only, and children loaded when a branch is expanded.
    /// </remarks>
    public sealed class CustomerTreeService
    {
        private readonly IDbContextFactory<PerfLabContext> _dbFactory;

        public CustomerTreeService(IDbContextFactory<PerfLabContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        /// <summary>Every node, each with its own count query.</summary>
        public CustomerTreeResult LoadAll()
        {
            using var db = _dbFactory.CreateDbContext();

            var customers = db.Customers.OrderBy(c => c.Name).ToList();
            var queries = 1;

            var nodes = new List<CustomerNodeRow>(customers.Count);
            foreach (var customer in customers)
            {
                // The label needs a number, so the node asks for it. Once per node.
                var count = db.Tickets.Count(t => t.CustomerId == customer.Id);
                queries++;

                nodes.Add(new CustomerNodeRow
                {
                    Id = customer.Id,
                    ParentId = customer.ParentId,
                    Name = customer.Name,
                    TicketCount = count
                });
            }

            return new CustomerTreeResult { Nodes = nodes, QueryCount = queries };
        }

        /// <summary>
        /// The children of one node — by loading the whole table again, counting every node again, and
        /// then filtering the result in memory.
        /// </summary>
        public CustomerTreeResult GetChildren(int parentId)
        {
            var all = LoadAll();

            return new CustomerTreeResult
            {
                Nodes = all.Nodes.Where(n => n.ParentId == parentId).ToList(),
                QueryCount = all.QueryCount
            };
        }
    }
}
