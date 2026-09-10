using System;
using System.Collections.Generic;
using System.Linq;
using EnterpriseOps.Diagnostics;
using EnterpriseOps.Domain;

namespace EnterpriseOps.Data
{
    public interface INoteStore
    {
        IReadOnlyList<WorkOrderNote> ForWorkOrder(string tenantId, int workOrderId);

        WorkOrderNote Find(int id);
    }

    /// <summary>
    /// The notes table. Notice what it does **not** do: it never sanitizes on the way in.
    ///
    /// Sanitizing at storage time destroys the original, hides what an attacker actually sent, and breaks the
    /// moment the same row is rendered somewhere that needs different escaping (a PDF, a CSV, an e-mail). Store
    /// the bytes that arrived; encode for the surface at the moment of rendering. That is the rule the safe-HTML
    /// review on this screen enforces.
    /// </summary>
    public sealed class InMemoryNoteStore : INoteStore
    {
        private readonly ActivityTrace _trace;
        private readonly List<WorkOrderNote> _notes;

        public InMemoryNoteStore(ActivityTrace trace)
        {
            _trace = trace;
            _notes = SeedData.Notes();
        }

        public IReadOnlyList<WorkOrderNote> ForWorkOrder(string tenantId, int workOrderId)
        {
            var rows = _notes
                .Where(n => StringComparer.Ordinal.Equals(n.TenantId, tenantId) && n.WorkOrderId == workOrderId)
                .OrderBy(n => n.CreatedUtc)
                .ToList();

            _trace?.Data($"notes for WO-{workOrderId:0000} → {rows.Count} rows, stored raw (never sanitized on the way in)");
            return rows;
        }

        public WorkOrderNote Find(int id) => _notes.FirstOrDefault(n => n.Id == id);
    }
}
