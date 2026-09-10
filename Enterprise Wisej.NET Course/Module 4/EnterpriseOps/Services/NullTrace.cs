using System;
using System.Collections.Generic;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// An <see cref="IActivityTrace"/> with no screen behind it. This is what makes the answer to the
    /// module's first review question — "can a save be tested without creating a Form?" — a yes you can
    /// run: the command service, the repository and the DbContext all take this instead of the page.
    ///
    /// <code>
    /// var trace    = new NullTrace();
    /// using var db = new SessionDatabase(trace);
    /// var service  = new WorkOrderCommandService(db, trace, new FaultInjector(), () => TimeSpan.FromSeconds(5));
    /// var result   = await service.ApproveAsync(command, context, CancellationToken.None);
    /// </code>
    ///
    /// <see cref="Lines"/> keeps what was traced, so a test can assert on the order of the decisions
    /// ("did the transaction commit before the audit was written?") without a UI.
    /// </summary>
    public sealed class NullTrace : IActivityTrace
    {
        private readonly List<string> _lines = new List<string>();

        /// <summary>Every line traced so far, in order. Empty unless <see cref="Capture"/> is on.</summary>
        public IReadOnlyList<string> Lines => _lines;

        /// <summary>False (the default) throws the lines away; true keeps them for assertions.</summary>
        public bool Capture { get; set; }

        public void Trace(string layer, string message)
        {
            if (Capture)
                _lines.Add($"{DateTime.UtcNow:HH:mm:ss.fff}  {layer} {message}");
        }
    }
}
