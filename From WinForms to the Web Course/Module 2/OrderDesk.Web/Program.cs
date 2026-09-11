using System.Collections.Specialized;
using Wisej.Web;

namespace OrderDesk
{
    /// <summary>
    /// Wisej.NET session entry point (Default.json → "startup"). Runs once per browser session.
    /// This replaces WinForms' Application.EnableVisualStyles() + Application.Run(new OrdersForm()).
    /// </summary>
    internal static class Program
    {
        static void Main(NameValueCollection args)
        {
            new OrdersForm().Show();
        }
    }
}
