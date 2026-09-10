using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using EnterpriseOps.Domain;
using EnterpriseOps.Services.Jobs;

namespace EnterpriseOps.Data
{
    /// <summary>
    /// The work-order table the imports write into — an in-memory dictionary standing in for the database.
    /// It belongs to <see cref="Services.Jobs.JobInfrastructure"/> (the process), not to a session, so an
    /// import that outlives a browser tab still has somewhere to write.
    ///
    /// Two things make the retry policy safe:
    ///   * <see cref="Upsert"/> is keyed by tenant + <see cref="ImportRow.ExternalRef"/>, so the same row written
    ///     twice produces ONE work order — idempotency, which is what turns "retry" from a risk into a fix.
    ///   * a transient failure is raised BEFORE anything is written, so a timed-out row leaves no half state.
    /// </summary>
    public sealed class FakeWorkOrderRepository : IWorkOrderImportWriter
    {
        private readonly object _gate = new object();
        private readonly Dictionary<string, WorkOrder> _byKey = new Dictionary<string, WorkOrder>(StringComparer.Ordinal);
        private int _nextId = 1;
        private long _writes;

        public FakeWorkOrderRepository()
        {
            // 60 work orders already exist with the same external refs as the first 60 lines of contoso_q2.csv,
            // so the very first import reports "created 940, updated 60" instead of a suspiciously round number.
            for (int line = 1; line <= 60; line++)
            {
                var wo = new WorkOrder
                {
                    Id = _nextId++,
                    TenantId = "contoso",
                    ExternalRef = $"WO-EXT-{line:00000}",
                    Title = $"Preventive maintenance · unit {line:0000}",
                    Customer = "Contoso Retail",
                    Site = "Milan DC",
                    AssetCode = $"AC-{1000 + (line % 900):0000}",
                    Status = WorkOrderStatus.New,
                    Priority = Priority.Normal,
                    CreatedUtc = DateTime.UtcNow.AddDays(-3),
                    Version = 1,
                };
                _byKey[Key(wo.TenantId, wo.ExternalRef)] = wo;
            }
        }

        /// <summary>Total rows in the fake table — the page shows it so an import's effect is countable.</summary>
        public int Count
        {
            get { lock (_gate) return _byKey.Count; }
        }

        public int CountFor(string tenantId)
        {
            lock (_gate)
                return _byKey.Values.Count(w => w.TenantId == tenantId);
        }

        /// <summary>Successful row writes since the process started (retries included once they succeed).</summary>
        public long Writes => Interlocked.Read(ref _writes);

        /// <summary>
        /// Writes one row.
        /// Throws <see cref="TerminalRowException"/> when the row can never succeed (unknown asset code, no title):
        /// the job records it and moves on. Throws <see cref="TransientRowException"/> when the simulated write
        /// times out: the retry policy decides whether to try again.
        /// </summary>
        /// <returns>true when a work order was created, false when an existing one was updated.</returns>
        public bool Upsert(string tenantId, ImportRow row)
        {
            // Terminal validation first — a row that can never succeed must not consume retry attempts.
            if (string.IsNullOrWhiteSpace(row.Title))
                throw new TerminalRowException("missing title");
            if (row.AssetCode == null || !row.AssetCode.StartsWith("AC-", StringComparison.Ordinal))
                throw new TerminalRowException($"unknown asset code '{row.AssetCode}' — not in the asset register");

            // The flaky downstream system. It fails BEFORE the write, so nothing is half-written and the
            // retry is a plain repeat, not a compensation.
            if (row.TransientFailuresBeforeSuccess > 0)
            {
                row.TransientFailuresBeforeSuccess--;
                throw new TransientRowException($"timeout writing row {row.LineNumber} (write lock held, 2,000 ms)");
            }

            lock (_gate)
            {
                string key = Key(tenantId, row.ExternalRef);
                Interlocked.Increment(ref _writes);

                if (_byKey.TryGetValue(key, out var existing))
                {
                    // Idempotent: the same ExternalRef never creates a second work order.
                    existing.Title = row.Title;
                    existing.Customer = row.Customer;
                    existing.Site = row.Site;
                    existing.AssetCode = row.AssetCode;
                    existing.Priority = row.Priority;
                    existing.Version++;
                    return false;
                }

                _byKey[key] = new WorkOrder
                {
                    Id = _nextId++,
                    TenantId = tenantId,
                    ExternalRef = row.ExternalRef,
                    Title = row.Title,
                    Customer = row.Customer,
                    Site = row.Site,
                    AssetCode = row.AssetCode,
                    Status = WorkOrderStatus.New,
                    Priority = row.Priority,
                    CreatedUtc = DateTime.UtcNow,
                    Version = 1,
                };
                return true;
            }
        }

        private static string Key(string tenantId, string externalRef) => tenantId + "|" + externalRef;
    }
}
