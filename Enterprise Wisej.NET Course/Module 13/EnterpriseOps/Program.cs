using System.Collections.Specialized;
using Wisej.Web;

namespace EnterpriseOps
{
    internal static class Program
    {
        /// <summary>
        /// Wisej.NET session entry point (configured in Default.json "startup").
        /// Module 13 opens straight into Field Technician mode.
        /// </summary>
        static void Main(NameValueCollection args)
        {
            Application.MainPage = new UI.FieldTechnicianPage();
        }
    }
}
