using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EnterpriseOps.Domain;
using EnterpriseOps.Services;

namespace EnterpriseOps.Integrations
{
    /// <summary>In-memory approver directory (an external system in production, so the caller bounds it with a timeout).</summary>
    public class FakeApproverDirectory : IApproverDirectory
    {
        private readonly ActivityTrace _trace;

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

        public async Task<IReadOnlyList<Approver>> LookupAsync(string tenantId, CancellationToken cancellationToken)
        {
            _trace.Write($"Integrations: directory lookup for tenant '{tenantId}'…");

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
