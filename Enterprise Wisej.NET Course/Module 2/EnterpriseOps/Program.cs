using System.Collections.Specialized;
using EnterpriseOps.Security;
using EnterpriseOps.Services;
using Wisej.Web;

namespace EnterpriseOps
{
    internal static class Program
    {
        /// <summary>
        /// Wisej.NET session entry point (configured in Default.json "startup").
        /// One SessionContext and one ServiceRegistry per session, both kept in Application.Session
        /// (the per-user bag) — never in a static. The main page is the migration dossier.
        /// </summary>
        static void Main(NameValueCollection args)
        {
            var session = new SessionContext("contoso", "ana.ops", Role.Manager);
            Application.Session.Context = session;

            var services = new ServiceRegistry(session, () => (SessionContext)Application.Session.Context);
            Application.Session.Services = services;

            Application.MainPage = new UI.MigrationDossierPage(services);
        }
    }
}
