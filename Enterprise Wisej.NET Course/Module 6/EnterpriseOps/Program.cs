using System.Collections.Specialized;
using EnterpriseOps.Services;
using Wisej.Web;

namespace EnterpriseOps
{
    internal static class Program
    {
        /// <summary>
        /// Wisej.NET session entry point (configured in Default.json "startup").
        ///
        /// Everything created here is PER SESSION and lives in Application.Session: who is using the app, and
        /// the activity trace they see. Nothing about a job is created here — the queue, the job store, the
        /// notification service and the work-order table belong to the process
        /// (<see cref="Services.Jobs.JobInfrastructure"/>), which is why a job survives this session ending.
        /// </summary>
        static void Main(NameValueCollection args)
        {
            var session = new SessionContext("contoso", "Contoso Field Services", "ana.ops", "Manager");
            var trace = new ActivityTrace();

            Application.Session.Context = session;
            Application.Session.Trace = trace;

            trace.Add($"UI → session {session.SessionCorrelationId} started for {session.User} ({session.Role}) of tenant {session.TenantId}.");

            Application.MainPage = new UI.ImportCenterPage();
        }
    }
}
