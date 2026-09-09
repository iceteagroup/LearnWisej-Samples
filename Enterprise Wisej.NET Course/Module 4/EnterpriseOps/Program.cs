using System.Collections.Specialized;
using Wisej.Web;

namespace EnterpriseOps
{
    internal static class Program
    {
        /// <summary>
        /// Wisej.NET session entry point (configured in Default.json "startup").
        /// Module 4: the Approvals screen — work-order search, the ApprovePanel and the live trace.
        /// Every session gets its own page, its own SessionContext and its own in-memory database.
        /// </summary>
        static void Main(NameValueCollection args)
        {
            Application.MainPage = new UI.ApprovalsPage();
        }
    }
}
