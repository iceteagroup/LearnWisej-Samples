using System;
using System.Threading.Tasks;
using AccessOps.Identity;

namespace AccessOps.Documents
{
    /// <summary>
    /// The Export path: work that runs on a raw <c>Task.Run</c>, outside the Wisej.NET session.
    /// That thread has no session, so it cannot read <c>Application.User</c>; the click captures
    /// the subject as data (<see cref="Subject.From"/>) and the job hands it to
    /// <see cref="DocumentService.Read"/>, which rechecks the grant when the job runs - a grant
    /// revoked between the page load and the job is seen.
    /// </summary>
    public static class ExportJob
    {
        /// <summary>
        /// Queues the export. <paramref name="completed"/> runs on the pool thread; the caller
        /// marshals back into its session (<c>Application.Update(component, action)</c>).
        /// </summary>
        public static Task Queue(DocumentService documents, Subject subject, string tenant, int recordId, Action<DocumentResult> completed)
        {
            return Task.Run(() =>
            {
#if DEBUG
                IdentityTrace.Write("Background · Task.Run", subject);
#endif
                var result = documents.Read(subject, tenant, recordId, "export");   // rechecked now
                completed(result);
            });
        }
    }
}
