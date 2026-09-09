using System;

namespace EnterpriseOps.Services.Workflow
{
    /// <summary>
    /// A synchronous IProgress: the callback runs on the workflow's continuation, right where the step
    /// happened (System.Progress&lt;T&gt; would post to a synchronization context the Wisej session does not want).
    /// The wizard's callback updates its status strip and calls Application.Update(this).
    /// </summary>
    public sealed class WorkflowProgressObserver : IProgress<WorkflowProgress>
    {
        private readonly Action<WorkflowProgress> _onProgress;

        public WorkflowProgressObserver(Action<WorkflowProgress> onProgress)
        {
            _onProgress = onProgress;
        }

        public void Report(WorkflowProgress value) => _onProgress?.Invoke(value);
    }
}
