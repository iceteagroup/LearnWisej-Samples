using System;
using System.Globalization;
using System.Linq;
using OrderDesk.Domain;
using OrderDesk.Reporting;
using OrderDesk.Views;
using Wisej.Web;

namespace OrderDesk
{
    /// <summary>
    /// The first vertical slice of OrderDesk in the browser: the Orders screen running on the
    /// reused OrderService, with Print Invoice and Export replaced by web-safe versions, and a
    /// session card for the two-user test.
    /// </summary>
    public partial class MainPage : Page
    {
        private readonly OrderService _orderService = new OrderService();
        private readonly CustomerService _customerService = new CustomerService();

        public MainPage()
        {
            InitializeComponent();
        }

        private void MainPage_Load(object sender, EventArgs e)
        {
            BindOrders();
            RefreshSession();
        }

        #region Orders

        private void BindOrders(int? selectId = null)
        {
            var orders = _orderService.GetOrders();
            gridOrders.Rows.Clear();
            foreach (var order in orders)
            {
                int index = gridOrders.Rows.Add(order.Id, order.CustomerName, order.Total, order.Status.ToString());
                gridOrders.Rows[index].Tag = order;
                gridOrders.Rows[index].Cells[3].Style.ForeColor = StatusColor(order.Status);
            }

            int select = 0;
            if (selectId.HasValue)
                for (int i = 0; i < gridOrders.Rows.Count; i++)
                    if ((gridOrders.Rows[i].Tag as Order)?.Id == selectId.Value) select = i;
            if (gridOrders.Rows.Count > 0)
            {
                gridOrders.Rows[select].Selected = true;
                ShowOrderDetail(gridOrders.Rows[select].Tag as Order);
            }
        }

        private static System.Drawing.Color StatusColor(OrderStatus status) => status switch
        {
            OrderStatus.Shipped => Ui.Ok,
            OrderStatus.Invoiced => Ui.Purple,
            OrderStatus.Hold => Ui.Warn,
            _ => Ui.Accent
        };

        private Order CurrentOrder => gridOrders.CurrentRow?.Tag as Order;

        private void gridOrders_SelectionChanged(object sender, EventArgs e) => ShowOrderDetail(CurrentOrder);

        private void ShowOrderDetail(Order order)
        {
            if (order == null)
            {
                labelOrderTitle.Text = "Order";
                labelOrderDetail.Text = "";
                return;
            }
            labelOrderTitle.Text = $"Order {order.Id}";
            labelOrderDetail.Text = $"Customer   {order.CustomerName}\nPO #       {order.PoNumber}\nOwner      {(string.IsNullOrEmpty(order.Owner) ? "—" : order.Owner)}\nLines      {order.Lines.Count} items · {order.Lines.Sum(l => l.Quantity)} units\nTotal      {N2(order.Total)}";
        }

        private void buttonNewOrder_Click(object sender, EventArgs e)
        {
            var customer = _customerService.Find(8);   // Litware Inc
            var order = new Order
            {
                Customer = customer, CustomerId = customer.Id, Owner = SessionUser ?? "",
                PoNumber = "LW-" + DateTime.Now.ToString("HHmmss"), Status = OrderStatus.Open, CreatedOn = DateTime.Today
            };
            order.Lines.Add(new OrderLine { Sku = "WJ-DEV-SEAT", Description = "Developer seat", Quantity = 5, UnitPrice = 190m });
            order.Lines.Add(new OrderLine { Sku = "WJ-SUP-GOLD", Description = "Gold support, 1 year", Quantity = 1, UnitPrice = 330m });

            var saved = _orderService.Save(order);
            BindOrders(saved.Id);
            Ui.Toast($"Order {saved.Id} saved — {N2(saved.Total)}.");
        }

        private void buttonPrintInvoice_Click(object sender, EventArgs e)
        {
            var order = CurrentOrder;
            if (order == null) return;

            // The server has no printer on the user's desk: render the invoice as a PDF instead.
            var lines = InvoiceDocument.Build(order, _orderService);
            var pdf = InvoicePdfWriter.Write(lines, $"Invoice {order.Id}");

            var preview = new InvoicePreviewForm(pdf, $"Invoice-{order.Id}.pdf");
            preview.ShowDialog((form, result) => form.Dispose());
        }

        private void buttonExport_Click(object sender, EventArgs e)
        {
            // No Excel Interop and no local path on the server: stream the file to the browser.
            var stream = CsvExport.OrdersStream(_orderService.GetOrders());
            Application.Download(stream, "orders.csv");
        }

        #endregion

        #region Session

        private static string SessionUser
        {
            get { dynamic session = Application.Session; return session.User as string; }
            set { dynamic session = Application.Session; session.User = value; }
        }

        private void buttonSignInKelly_Click(object sender, EventArgs e) => SignIn("kelly");
        private void buttonSignInSam_Click(object sender, EventArgs e) => SignIn("sam");

        private void SignIn(string user)
        {
            Legacy.AppState.CurrentUser = user;      // the desktop static: one slot for the whole server
            SessionUser = user;                      // per browser session
            RefreshSession();
        }

        private void buttonReread_Click(object sender, EventArgs e) => RefreshSession();

        private void buttonSecondSession_Click(object sender, EventArgs e)
        {
            Application.Navigate(Application.Url, "_blank");
        }

        private void RefreshSession()
        {
            string staticUser = Legacy.AppState.CurrentUser ?? "(not set)";
            string sessionUser = SessionUser ?? "(not set)";
            labelSessionValues.Text =
                $"Session                        {Short(Application.SessionId)}   ({Application.SessionCount} open)\n" +
                $"Application.Session.User       {sessionUser}\n" +
                $"AppState.CurrentUser (static)  {staticUser}";

            bool leaked = Legacy.AppState.CurrentUser != null && SessionUser != null && Legacy.AppState.CurrentUser != SessionUser;
            labelSessionValues.ForeColor = leaked ? Ui.Error : System.Drawing.Color.Black;
        }

        #endregion

        private static string N2(decimal value) => value.ToString("N2", CultureInfo.InvariantCulture);

        private static string Short(string id) => string.IsNullOrEmpty(id) ? "?" : (id.Length > 8 ? id.Substring(0, 8) : id);
    }
}
