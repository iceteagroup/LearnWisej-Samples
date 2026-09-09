using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EnterpriseOps.Domain;
using EnterpriseOps.Services;

namespace EnterpriseOps.Integrations
{
    /// <summary>
    /// In-memory approver directory. HangNextLookup() makes the next lookup outlast the wizard's timeout,
    /// so the wizard shows the "directory did not answer" path and keeps the draft.
    /// </summary>
    public class FakeApproverDirectory : IApproverDirectory
    {
        private readonly ActivityTrace _trace;
        private bool _hangNext;

        private static readonly Approver[] Directory =
        {
            new Approver { Id = "m.weber",   DisplayName = "Mara Weber",    Role = "Supervisor" },
            new Approver { Id = "ana.ops",   DisplayName = "Ana Ops",       Role = "Manager" },
            new Approver { Id = "j.okafor",  DisplayName = "Jide Okafor",   Role = "Supervisor" },
            new Approver { Id = "ben.tech",  DisplayName = "Ben Tech",      Role = "Technician" },   // listed, but not allowed to approve
        };

        public FakeApproverDirectory(ActivityTrace trace)
        {
            _trace = trace;
        }

        /// <summary>Lab switch: the next lookup never answers within the wizard's timeout.</summary>
        public void HangNextLookup()
        {
            _hangNext = true;
            _trace.Write("Integrations: approver directory armed — the next lookup will hang past the timeout");
        }

        public async Task<IReadOnlyList<Approver>> LookupAsync(string tenantId, CancellationToken cancellationToken)
        {
            _trace.Write($"Integrations: directory lookup for tenant '{tenantId}'…");

            if (_hangNext)
            {
                _hangNext = false;
                await Task.Delay(30_000, cancellationToken);      // throws TaskCanceledException at the timeout
            }

            await Task.Delay(300, cancellationToken);
            var result = Directory.ToList();
            _trace.Write($"Integrations: directory answered — {result.Count} entries");
            return result;
        }

        public Approver Find(string tenantId, string approverId)
        {
            return Directory.FirstOrDefault(a => a.Id == approverId);
        }
    }
}
