using System;
using System.Globalization;
using System.Linq;
using OrderDesk.Domain;
using OrderDesk.Migration;
using OrderDesk.Reporting;
using OrderDesk.Views;
using Wisej.Web;

namespace OrderDesk
{
    /// <summary>
    /// Module 1 · Migration discovery &amp; the first slice.
    ///
    /// Left, top:    the assessment workbook for LegacyOrderDesk (every form/feature tagged by
    ///               dependency, risk and verdict) with the four-verdict filter.
    /// Left, bottom: the first vertical slice running in the browser — the Orders screen with
    ///               the reused OrderService, Print Invoice (server PDF) and Export (download).
    /// Right:        the migration log (live trace) and the session check that proves why the
    ///               static "current user" is a migration task: open a second tab and sign in
    ///               as somebody else.
    /// </summary>
    public partial class MainPage : Page
    {
        private readonly OrderService _orderService = new OrderService();
        private readonly CustomerService _customerService = new CustomerService();
        private Verdict? _verdictFilter;
        private string _verdictName = "all";

        public MainPage()
        {
            InitializeComponent();
        }

        private void MainPage_Load(object sender, EventArgs e)
        {
            trace.Add(TraceKind.Server, "startup", "Default.json → OrderDesk.Program.Main → Application.MainPage = new MainPage()");
            trace.Add(TraceKind.Server, "session", $"new browser session {Short(Application.SessionId)} · {Application.SessionCount} session(s) in this process");

            BindWorkbook();
            BindOrders();
            RefreshSessionCard(announce: false);
            Ui.SetStatus(labelStatus, "assessment loaded · first slice running", Ui.Ok);
        }

        #region Assessment workbook (deliverable 1 + 2)

        private void BindWorkbook()
        {
            var rows = AssessmentWorkbook.Items
                .Where(i => _verdictFilter == null || i.Verdict == _verdictFilter.Value)
                .ToList();

            gridWorkbook.Rows.Clear();
            foreach (var item in rows)
            {
                int index = gridWorkbook.Rows.Add(item.Feature, item.Dependency, item.RiskTag, item.VerdictText, item.Effort, item.SliceText);
                gridWorkbook.Rows[index].Tag = item;
                gridWorkbook.Rows[index].Cells[3].Style.ForeColor = VerdictColor(item.Verdict);
                gridWorkbook.Rows[index].Cells[3].Style.Font = Ui.SmallBold;
            }

            int slice = AssessmentWorkbook.Items.Count(i => i.InFirstSlice);
            labelWorkbookCount.Text = _verdictFilter == null
                ? $"{rows.Count} items · {slice} in the first slice"
                : $"{rows.Count} of {AssessmentWorkbook.Items.Count} · {_verdictName}";

            if (gridWorkbook.Rows.Count > 0)
            {
                gridWorkbook.Rows[0].Selected = true;
                ShowWorkbookDetail(gridWorkbook.Rows[0].Tag as AssessmentItem);
            }
            else
            {
                ShowWorkbookDetail(null);
            }
        }

        private static System.Drawing.Color VerdictColor(Verdict verdict) => verdict switch
        {
            Verdict.DirectPort => Ui.Ok,
            Verdict.Adapt => Ui.Accent,
            Verdict.Redesign => Ui.Warn,
            Verdict.Defer => Ui.Purple,
            _ => Ui.Muted
        };

        private void gridWorkbook_SelectionChanged(object sender, EventArgs e)
        {
            ShowWorkbookDetail(gridWorkbook.CurrentRow?.Tag as AssessmentItem);
        }

        private void ShowWorkbookDetail(AssessmentItem item)
        {
            if (item == null)
            {
                labelWorkbookDetail.Text = "";
                return;
            }
            labelWorkbookDetail.Text = $"{item.Area} · {item.Feature}\nWhy: {item.Why}\nReplacement: {item.Replacement}";
        }

        private void buttonFilterAll_Click(object sender, EventArgs e) => ApplyVerdictFilter(null, "all");
        private void buttonFilterDirect_Click(object sender, EventArgs e) => ApplyVerdictFilter(Verdict.DirectPort, "direct-port");
        private void buttonFilterAdapt_Click(object sender, EventArgs e) => ApplyVerdictFilter(Verdict.Adapt, "port-with-adaptation");
        private void buttonFilterRedesign_Click(object sender, EventArgs e) => ApplyVerdictFilter(Verdict.Redesign, "redesign");
        private void buttonFilterDefer_Click(object sender, EventArgs e) => ApplyVerdictFilter(Verdict.Defer, "defer");

        private void ApplyVerdictFilter(Verdict? verdict, string name)
        {
            _verdictFilter = verdict;
            _verdictName = name;
            BindWorkbook();
            trace.Add(TraceKind.FromClient, "workbook.filter", $"verdict = {name} → {gridWorkbook.Rows.Count} rows");
        }

        #endregion

        #region First slice: the Orders screen in the browser (deliverable 3)

        private void BindOrders(int? selectId = null)
        {
            var orders = _orderService.GetOrders();     // ✓ business logic reused as-is
            gridOrders.Rows.Clear();
            foreach (var order in orders)
            {
                int index = gridOrders.Rows.Add(order.Id, order.CustomerName, order.Total, order.Status.ToString());
                gridOrders.Rows[index].Tag = order;
                gridOrders.Rows[index].Cells[3].Style.ForeColor = StatusColor(order.Status);
            }
            trace.Add(TraceKind.Server, "OrderService.GetOrders", $"{orders.Count} orders from the reused repository");

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
            // The same OrderService call LegacyOrderDesk makes — no UI, no desktop assumption.
            var customer = _customerService.Find(8);   // Litware Inc
            var order = new Order
            {
                Customer = customer, CustomerId = customer.Id, Owner = SessionUser ?? "",
                PoNumber = "LW-" + DateTime.Now.ToString("HHmmss"), Status = OrderStatus.Open, CreatedOn = DateTime.Today
            };
            order.Lines.Add(new OrderLine { Sku = "WJ-DEV-SEAT", Description = "Developer seat", Quantity = 5, UnitPrice = 190m });
            order.Lines.Add(new OrderLine { Sku = "WJ-SUP-GOLD", Description = "Gold support, 1 year", Quantity = 1, UnitPrice = 330m });

            var saved = _orderService.Save(order);
            trace.Add(TraceKind.Server, "OrderService.Save", $"order {saved.Id} · CalculateOrderTotal = {N2(saved.Total)} (business logic reused, unchanged)");
            BindOrders(saved.Id);
            Ui.Toast($"Order {saved.Id} saved — {N2(saved.Total)}.");
            Ui.SetStatus(labelStatus, $"order {saved.Id} saved through the reused OrderService", Ui.Ok);
            Ui.HideBanner(labelBanner);
        }

        private void buttonPrintInvoice_Click(object sender, EventArgs e)
        {
            var order = CurrentOrder;
            if (order == null) return;

            // Desktop: InvoicePrinter.Print → PrintDocument → the printer on the user's desk.
            // Web:     same InvoiceDocument lines → a PDF built on the server → PdfViewer / download.
            var lines = InvoiceDocument.Build(order, _orderService);
            var pdf = InvoicePdfWriter.Write(lines, $"Invoice {order.Id}");
            trace.Add(TraceKind.Boundary, "Print Invoice", $"PrintDocument → local printer  ⇒  server PDF ({pdf.Length:N0} bytes) → PdfViewer");
            trace.Add(TraceKind.ToClient, "InvoicePreviewForm", $"Invoice-{order.Id}.pdf  (PdfViewer.PdfStream, modal)");

            var preview = new InvoicePreviewForm(pdf, $"Invoice-{order.Id}.pdf");
            preview.ShowDialog((form, result) =>
            {
                // Module 3 rule: a transient dialog is disposed by the caller when it closes.
                form.Dispose();
                trace.Add(TraceKind.Server, "InvoicePreviewForm", "closed and disposed");
            });
            Ui.SetStatus(labelStatus, $"invoice {order.Id} rendered on the server", Ui.Ok);
        }

        private void buttonExport_Click(object sender, EventArgs e)
        {
            // Desktop: Excel Interop + C:\Orders\out.xlsx.  Web: bytes in memory → Application.Download.
            var orders = _orderService.GetOrders();
            var stream = CsvExport.OrdersStream(orders);
            trace.Add(TraceKind.Boundary, "Export to Excel", $"Excel.Application + C:\\Orders\\out.xlsx  ⇒  {stream.Length:N0} bytes → Application.Download(\"orders.csv\")");
            Application.Download(stream, "orders.csv");
            trace.Add(TraceKind.ToClient, "Application.Download", "orders.csv (the browser saves it — the server never touched a local path)");
            Ui.SetStatus(labelStatus, "export streamed to the browser", Ui.Ok);
        }

        private void buttonAttach_Click(object sender, EventArgs e)
        {
            // The one boundary the first slice deliberately does NOT cross yet: it is logged, not faked.
            trace.Add(TraceKind.Boundary, "Attach file", "OpenFileDialog + C:\\Orders\\Attachments cannot exist on the server → Upload control + storage root (Module 6)");
            Ui.ShowBanner(labelBanner, "✕ Attach file: OpenFileDialog and C:\\Orders\\Attachments assume the user's disk. Verdict: redesign → Upload control + server storage root (Module 6). Logged in migration-log.md.", Ui.BannerKind.Warn);
            Ui.SetStatus(labelStatus, "boundary hit — captured in the backlog", Ui.Warn);
        }

        #endregion

        #region Session check: one server, many sessions (why the static user is a migration task)

        private static string SessionUser
        {
            get { dynamic session = Application.Session; return session.User as string; }
            set { dynamic session = Application.Session; session.User = value; }
        }

        private void buttonSignInKelly_Click(object sender, EventArgs e) => SignIn("kelly");
        private void buttonSignInSam_Click(object sender, EventArgs e) => SignIn("sam");

        private void SignIn(string user)
        {
            Legacy.AppState.CurrentUser = user;      // ✕ the desktop way — one static slot for the whole server
            SessionUser = user;                      // ✓ the web way — one value per browser session
            trace.Add(TraceKind.FromClient, "sign in", $"user = {user}");
            trace.Add(TraceKind.Server, "AppState.CurrentUser (static)", $"= {user}   ← every session now sees this");
            trace.Add(TraceKind.Server, "Application.Session.User", $"= {user}   ← only session {Short(Application.SessionId)} sees this");
            RefreshSessionCard(announce: true);
        }

        private void buttonReread_Click(object sender, EventArgs e)
        {
            trace.Add(TraceKind.FromClient, "re-read state", "compare the static slot with this session's value");
            RefreshSessionCard(announce: true);
        }

        private void buttonSecondSession_Click(object sender, EventArgs e)
        {
            trace.Add(TraceKind.ToClient, "Application.Navigate", "same URL, target _blank → a second browser session in this process");
            Application.Navigate(Application.Url, "_blank");
        }

        private void RefreshSessionCard(bool announce)
        {
            string staticUser = Legacy.AppState.CurrentUser ?? "(not set)";
            string sessionUser = SessionUser ?? "(not set)";
            labelSessionValues.Text =
                $"session id                  {Short(Application.SessionId)}   ({Application.SessionCount} live in this process)\n" +
                $"static AppState.CurrentUser {staticUser}\n" +
                $"Application.Session.User    {sessionUser}";

            bool leaked = Legacy.AppState.CurrentUser != null && SessionUser != null && Legacy.AppState.CurrentUser != SessionUser;
            if (leaked)
            {
                Ui.ShowBanner(labelBanner, $"✕ Static state leaked across sessions: AppState.CurrentUser = {staticUser}, but this session signed in as {sessionUser}. A second tab overwrote the shared slot — per-user state must move to Application.Session (Module 4).", Ui.BannerKind.Error);
                Ui.SetStatus(labelStatus, "static current user corrupted by another session", Ui.Error);
                if (announce) trace.Add(TraceKind.Server, "✕ leak detected", $"static = {staticUser} ≠ session = {sessionUser}");
            }
            else if (announce)
            {
                if (SessionUser == null)
                    Ui.ShowBanner(labelBanner, "This session has not signed in yet. Sign in here, then open a second session and sign in as the other user, then come back and re-read.", Ui.BannerKind.Warn);
                else
                    Ui.ShowBanner(labelBanner, $"✓ static and session agree ({sessionUser}) — for now. Open a second session, sign in as the other user, then re-read here.", Ui.BannerKind.Ok);
                Ui.SetStatus(labelStatus, $"signed in as {sessionUser ?? "nobody"}", Ui.Ok);
            }
        }

        #endregion

        private void buttonClear_Click(object sender, EventArgs e) => trace.Clear();

        private static string N2(decimal value) => value.ToString("N2", CultureInfo.InvariantCulture);

        private static string Short(string id) => string.IsNullOrEmpty(id) ? "?" : (id.Length > 8 ? id.Substring(0, 8) : id);
    }
}
