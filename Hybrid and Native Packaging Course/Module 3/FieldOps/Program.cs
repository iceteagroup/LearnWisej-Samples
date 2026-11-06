using System.Collections.Specialized;
using FieldOps.Services;
using Wisej.Web;

namespace FieldOps
{
    internal static class Program
    {
        /// <summary>
        /// Wisej.NET session entry point (Default.json "startup"). One session per browser tab,
        /// per PWA window and per Hybrid client window - the code below cannot tell them apart,
        /// and does not need to.
        /// </summary>
        static void Main(NameValueCollection args)
        {
            // Sign-in stand-in: the technician comes from ?tech= (default: Dana Whitfield).
            // A production FieldOps signs in through its identity provider; what matters for
            // this course is that the session knows WHO is signed in, so the list is filtered.
            var service = WorkOrderService.Current;
            var technician = service.FindTechnician(args?["tech"] ?? "dana") ?? service.FindTechnician("dana");

            // ?order=WO-nnnn: the second desktop window (Device.Windows.TryOpenAsync) or the
            // browser tab Application.Navigate opened for one work order.
            var order = args?["order"];
            if (!string.IsNullOrEmpty(order))
            {
                Session.Technician = technician;
                new WorkOrderWindow(order).Show();
                return;
            }

            SignIn.Complete(technician);
            Session.ListPage = new WorkOrdersPage();
            Session.ListPage.Show();
        }
    }
}
