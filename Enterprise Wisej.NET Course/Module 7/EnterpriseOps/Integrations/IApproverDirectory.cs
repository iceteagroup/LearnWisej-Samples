using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EnterpriseOps.Domain;

namespace EnterpriseOps.Integrations
{
    /// <summary>
    /// The directory the Approver step queries (Active Directory, an HR API, …). External, so it can time out —
    /// the failure path matrix's "timeout" column for step 3.
    /// </summary>
    public interface IApproverDirectory
    {
        Task<IReadOnlyList<Approver>> LookupAsync(string tenantId, CancellationToken cancellationToken);

        Approver Find(string tenantId, string approverId);
    }
}
