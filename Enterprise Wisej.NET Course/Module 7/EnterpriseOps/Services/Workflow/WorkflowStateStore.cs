using System.Collections.Generic;
using System.Linq;

namespace EnterpriseOps.Services.Workflow
{
    /// <summary>
    /// Server-side draft store for wizard state, keyed by work order. Per session here (it lives in the
    /// page's service graph, which Wisej.NET keeps on the server across a browser refresh); in production
    /// it is a table keyed by user + work order so a draft also survives a new login on another device.
    /// </summary>
    public class WorkflowStateStore
    {
        private readonly ActivityTrace _trace;
        private readonly Dictionary<int, EscalationWizardState> _drafts = new Dictionary<int, EscalationWizardState>();

        public WorkflowStateStore(ActivityTrace trace)
        {
            _trace = trace;
        }

        public int Count => _drafts.Count;

        public IReadOnlyList<EscalationWizardState> All => _drafts.Values.OrderByDescending(d => d.UpdatedUtc).ToList();

        public EscalationWizardState Find(int workOrderId)
        {
            return _drafts.TryGetValue(workOrderId, out var state) ? state : null;
        }

        /// <summary>Called by the wizard after every completed step — the reason a refresh loses nothing.</summary>
        public void Save(EscalationWizardState state)
        {
            _drafts[state.WorkOrderId] = state;
            _trace.Write($"Service: WorkflowStateStore.Save draft {state.DraftId} for WO-{state.WorkOrderId} — {state.Progress}");
        }

        public void Remove(int workOrderId, string why)
        {
            if (_drafts.Remove(workOrderId))
                _trace.Write($"Service: WorkflowStateStore.Remove draft for WO-{workOrderId} — {why}");
        }
    }
}
