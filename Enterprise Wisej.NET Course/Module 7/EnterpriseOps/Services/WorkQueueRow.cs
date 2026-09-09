using System;
using System.Collections.Generic;
using System.Linq;
using EnterpriseOps.Data;
using EnterpriseOps.Domain;

namespace EnterpriseOps.Services
{
    /// <summary>The grid projection — the UI binds this, never the WorkOrder entity.</summary>
    public class WorkQueueRow
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public string Title { get; set; }
        public string Customer { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public string Due { get; set; }
        public int Version { get; set; }
    }

    /// <summary>Reads the queue for the session's tenant. Tiny on purpose: Module 7 is about the workflow.</summary>
    public class WorkQueueService
    {
        private readonly InMemoryWorkOrderStore _store;
        private readonly ActivityTrace _trace;

        public WorkQueueService(InMemoryWorkOrderStore store, ActivityTrace trace)
        {
            _store = store;
            _trace = trace;
        }

        public List<WorkQueueRow> Load(SessionContext session)
        {
            var rows = _store.ForTenant(session.TenantId)
                .Select(w => new WorkQueueRow
                {
                    Id = w.Id,
                    Number = w.Number,
                    Title = w.Title,
                    Customer = w.Customer,
                    Status = w.Status.ToString(),
                    Priority = w.Priority.ToString(),
                    Due = w.DueUtc.HasValue ? w.DueUtc.Value.ToString("MMM d") : "—",
                    Version = w.Version,
                })
                .ToList();

            _trace.Write($"Service: WorkQueueService.Load tenant '{session.TenantId}' → {rows.Count} rows");
            return rows;
        }
    }
}
