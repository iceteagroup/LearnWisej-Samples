using System;
using OrderDesk.Domain;
using OrderDesk.Views;
using Wisej.Web;

namespace OrderDesk
{
    /// <summary>
    /// Template screen: replace with the module's page. Shows the shell works and the reused
    /// business logic answers.
    /// </summary>
    public partial class MainPage : Page
    {
        private readonly OrderService _orderService = new OrderService();

        public MainPage()
        {
            InitializeComponent();
        }

        private void MainPage_Load(object sender, EventArgs e)
        {
            trace.Add(TraceKind.Server, "startup", "Application.MainPage = new MainPage()  (Default.json → OrderDesk.Program.Main)");
            foreach (var order in _orderService.GetOrders())
                trace.Add(TraceKind.Server, $"order {order.Id}", $"{order.CustomerName}  {order.Total:N2}  {order.Status}");
            Ui.SetStatus(labelStatus, "shell running · Wisej-4 4.1.0", Ui.Ok);
        }
    }
}
