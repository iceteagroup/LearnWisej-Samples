using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EnterpriseOps.Diagnostics;
using EnterpriseOps.Domain;

namespace EnterpriseOps.Integrations
{
    /// <summary>A status change reported by the field (technician app, monitoring, partner system).</summary>
    public sealed class FeedChange
    {
        public FeedChange(int workOrderId, WorkOrderStatus newStatus, string note)
        {
            WorkOrderId = workOrderId;
            NewStatus = newStatus;
            Note = note;
        }

        public int WorkOrderId { get; }
        public WorkOrderStatus NewStatus { get; }
        public string Note { get; }
    }

    /// <summary>
    /// A fake of the external operations feed. Each pull returns the next scripted batch of field changes;
    /// the first pull is the walkthrough's "INC-1042 mitigated".
    ///
    /// Integrations live behind their own folder/namespace so the workflow never knows whether the feed is
    /// HTTP, a queue or a fake.
    /// </summary>
    public sealed class OperationsFeed
    {
        private const int SimulatedLatencyMs = 350;

        private readonly ActivityTrace _trace;
        private readonly Queue<FeedChange[]> _script = new Queue<FeedChange[]>(new[]
        {
            new[] { new FeedChange(1042, WorkOrderStatus.Completed, "mitigated by ben.tech — gateway failed over to DC-West") },
            new[] { new FeedChange(1046, WorkOrderStatus.Escalated, "chiller still alarming, escalated to ana.ops") },
            new[] { new FeedChange(1043, WorkOrderStatus.InProgress, "ben.tech started re-indexing"),
                    new FeedChange(1047, WorkOrderStatus.Assigned, "assigned to ben.tech") },
        });

        public OperationsFeed(ActivityTrace trace)
        {
            _trace = trace;
        }

        public async Task<IReadOnlyList<FeedChange>> PullChangesAsync(string tenantId)
        {
            await Task.Delay(SimulatedLatencyMs);

            FeedChange[] batch = _script.Count > 0 ? _script.Dequeue() : Array.Empty<FeedChange>();
            _trace.Integration($"OperationsFeed returned {batch.Length} change(s) in {SimulatedLatencyMs} ms");
            return batch;
        }
    }
}
