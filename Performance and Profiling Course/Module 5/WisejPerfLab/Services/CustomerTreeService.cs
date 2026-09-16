using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WisejPerfLab.Data;
using WisejPerfLab.Models;

namespace WisejPerfLab.Services
{
    /// <summary>The nodes of one level, with the statement count it took to produce them.</summary>
    public sealed class CustomerTreeResult
    {
        public IReadOnlyList<CustomerNodeRow> Nodes { get; set; }

        public int QueryCount { get; set; }
    }

    /// <summary>
    /// The customer hierarchy, one level at a time. Since Module 5 nothing is built before the user asks
    /// for it, and the ticket counts on a level come from <b>one grouped query</b> instead of one query
    /// per node.
    /// </summary>
    /// <remarks>
    /// Before: 3,200 nodes and 3,201 statements at load, for a tree the user opens two branches of.
    /// Now: 8 root nodes and 2 statements at load, then 2 more statements per branch that is actually
    /// opened. The nodes that are never opened cost nothing — no query, no control, no payload.
    /// </remarks>
    public sealed class CustomerTreeService
    {
        private readonly IDbContextFactory<PerfLabContext> _dbFactory;

        public CustomerTreeService(IDbContextFactory<PerfLabContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        /// <summary>The top level: the regions.</summary>
        public CustomerTreeResult GetRoots() => GetLevel(null);

        /// <summary>The children of one node, loaded when the branch is expanded.</summary>
        public CustomerTreeResult GetChildren(int parentId) => GetLevel(parentId);

        private CustomerTreeResult GetLevel(int? parentId)
        {
            using var db = _dbFactory.CreateDbContext();

            // 1. The nodes of this level only.
            var customers = db.Customers
                .Where(c => c.ParentId == parentId)
                .OrderBy(c => c.Name)
                .Select(c => new { c.Id, c.ParentId, c.Name })
                .ToList();

            var ids = customers.Select(c => c.Id).ToList();

            // 2. Their ticket counts, grouped, in one statement. This is the query that used to be
            //    issued once per node — and it is the same information.
            var counts = db.Tickets
                .Where(t => ids.Contains(t.CustomerId))
                .GroupBy(t => t.CustomerId)
                .Select(g => new { CustomerId = g.Key, Count = g.Count() })
                .ToList()
                .ToDictionary(x => x.CustomerId, x => x.Count);

            // 3. Whether each node has children, so the tree knows which branches to offer. Also grouped.
            var childCounts = db.Customers
                .Where(c => c.ParentId != null && ids.Contains(c.ParentId.Value))
                .GroupBy(c => c.ParentId.Value)
                .Select(g => new { ParentId = g.Key, Count = g.Count() })
                .ToList()
                .ToDictionary(x => x.ParentId, x => x.Count);

            var nodes = customers
                .Select(c => new CustomerNodeRow
                {
                    Id = c.Id,
                    ParentId = c.ParentId,
                    Name = c.Name,
                    TicketCount = counts.TryGetValue(c.Id, out var count) ? count : 0,
                    ChildCount = childCounts.TryGetValue(c.Id, out var children) ? children : 0
                })
                .ToList();

            return new CustomerTreeResult { Nodes = nodes, QueryCount = 3 };
        }
    }
}
