using System.Collections.Specialized;
using Wisej.Web;

namespace EnterpriseOps
{
    internal static class Program
    {
        /// <summary>
        /// Wisej.NET session entry point (configured in Default.json "startup").
        /// Module 7: the work-order screen that opens the Escalation Wizard modally.
        /// </summary>
        static void Main(NameValueCollection args)
        {
            Application.MainPage = new UI.WorkQueuePage();
        }
    }
}
