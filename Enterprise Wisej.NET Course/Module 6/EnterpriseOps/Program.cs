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
        /// Who is using the app is PER SESSION and lives in Application.Session. Nothing about a job is created
        /// here — the queue, the job store, the notification service and the work-order table belong to the
        /// process (<see cref="Services.Jobs.JobInfrastructure"/>), which is why a job survives this session ending.
        /// </summary>
        static void Main(NameValueCollection args)
        {
            Application.Session.Context = new SessionContext("contoso", "Contoso Field Services", "ana.ops", "Manager");

            Application.MainPage = new UI.ImportCenterPage();
        }
    }
}
