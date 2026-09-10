using System.Collections.Specialized;
using Wisej.Web;

namespace EnterpriseOps
{
    internal static class Program
    {
        /// <summary>
        /// Wisej.NET session entry point (configured in Default.json "startup"). It runs once per browser
        /// session — which is exactly the scope this module is about. The page it creates builds one
        /// <c>SessionContext</c> and stores it in <c>Application.Session</c>. Open a second tab and this
        /// method runs again for a second session, with its own context, its own services and its own editor,
        /// sharing only the work order store and the audit trail — both documented and synchronized.
        /// </summary>
        static void Main(NameValueCollection args)
        {
            Application.MainPage = new UI.WorkOrderEditorPage();
        }
    }
}
