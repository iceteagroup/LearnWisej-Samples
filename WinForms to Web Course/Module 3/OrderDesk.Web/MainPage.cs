using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using OrderDesk.Dialogs;
using OrderDesk.Domain;
using OrderDesk.Pages;
using OrderDesk.Services;
using OrderDesk.Shared;
using Wisej.Web;

namespace OrderDesk
{
    /// <summary>
    /// Module 3 — Forms, Navigation, Layout &amp; Modal Workflow. The Wisej.NET shell gains what the
    /// WinForms OrdersForm carried itself: a MenuBar (was MenuStrip), a StatusBar (was StatusStrip)
    /// and a left nav that switches between the three ported screens (Orders / Customers / Reports).
    /// The bottom button bar exercises the modal workflow: the using-block edit (success), five
    /// leaked dialogs (failure), the dispose fix (recovery), MessageBox → Toast, and a timed walk of
    /// the tab order (progress). Everything is logged to the TracePanel on the right.
    /// </summary>
    public partial class MainPage : Page
    {
        private readonly OrderService _service = new OrderService();
        private readonly OrdersPage _ordersPage;
        private readonly CustomersPage _customersPage;
        private readonly ReportsPage _reportsPage;
        private readonly List<ModulePage> _pages = new List<ModulePage>();
        // Lab bookkeeping only: the dialogs 'Leak dialogs ✕' abandoned, so 'Dispose fix ✓' can reach
        // them even if the session registry no longer lists them. The WinForms code kept no such reference.
        private readonly List<EditOrderDialog> _leaked = new List<EditOrderDialog>();

        // "Tab order / docking" walk state
        private List<TabStopInfo> _walk = new List<TabStopInfo>();
        private int _walkIndex;
        private Panel _highlight;
        private const string HighlightName = "tabHighlight";

        public MainPage()
        {
            InitializeComponent();

            // The three ported screens: created once, hosted in pageHost, shown by toggling Visible.
            _ordersPage = new OrdersPage(_service) { Dock = DockStyle.Fill, Trace = trace, Visible = false };
            _customersPage = new CustomersPage(_service) { Dock = DockStyle.Fill, Trace = trace, Visible = false };
            _reportsPage = new ReportsPage { Dock = DockStyle.Fill, Trace = trace, Visible = false };
            _pages.AddRange(new ModulePage[] { _ordersPage, _customersPage, _reportsPage });
            foreach (var p in _pages)
                pageHost.Controls.Add(p);

            _ordersPage.OrdersChanged += (s, e) => UpdateStatusBar();
        }

        private void MainPage_Load(object sender, EventArgs e)
        {
            trace.Server("Program.Main", "Application.MainPage = new MainPage()  · session " + Application.SessionId.Substring(0, 8));
            trace.Finding("MenuStrip → MenuBar · StatusStrip → StatusBar", "chrome moved from OrdersForm into the shell (MainPage.Designer.cs)");
            trace.Finding("OrdersForm → OrdersPage : Panel", "Anchor → Dock (header Top · detail Right 220 · grid Fill) · TabIndex values kept");
            trace.Finding("EditOrderDialog → Wisej.Web.Form", "same fields + DialogResult · shown with ShowDialog() inside a using block");
            trace.Server("new OrdersPage / CustomersPage / ReportsPage", "one instance each · navigation toggles Visible, never recreates");

            _ordersPage.ReloadGrid();
            ShowPage(_ordersPage, "startup");
            UpdateStatusBar();
        }

        // ── navigation ───────────────────────────────────────────────────────────────

        private void ShowPage(ModulePage page, string origin)
        {
            trace.In("navigate → " + page.Title, origin);
            foreach (var p in _pages)
                p.Visible = ReferenceEquals(p, page);

            if (page == _customersPage) _customersPage.EnsureLoaded();

            StyleNav(navOrders, page == _ordersPage);
            StyleNav(navCustomers, page == _customersPage);
            StyleNav(navReports, page == _reportsPage);
            statusPanelPage.Text = page.Title;

            trace.Server("ShowPage(" + page.Title + ")", page.GetType().Name + ".Visible = true · other pages hidden (same instances)");
        }

        private static void StyleNav(Button b, bool active)
        {
            b.BackColor = active ? Color.FromArgb(70, 26, 134, 255) : Color.FromArgb(22, 20, 46);
            b.ForeColor = active ? Color.White : Color.FromArgb(159, 176, 200);
            b.Font = new Font("default", 10F, active ? FontStyle.Bold : FontStyle.Regular);
        }

        private void navOrders_Click(object sender, EventArgs e) => ShowPage(_ordersPage, "navOrders.Click");
        private void navCustomers_Click(object sender, EventArgs e) => ShowPage(_customersPage, "navCustomers.Click");
        private void navReports_Click(object sender, EventArgs e) => ShowPage(_reportsPage, "navReports.Click");

        private void viewOrdersMenuItem_Click(object sender, EventArgs e) => ShowPage(_ordersPage, "View › Orders");
        private void viewCustomersMenuItem_Click(object sender, EventArgs e) => ShowPage(_customersPage, "View › Customers");
        private void viewReportsMenuItem_Click(object sender, EventArgs e) => ShowPage(_reportsPage, "View › Reports");

        private void reportOpenOrdersMenuItem_Click(object sender, EventArgs e)
        {
            ShowPage(_reportsPage, "Reports › Open orders by customer");
            _reportsPage.Select("Open orders by customer");
        }

        private void reportInvoicesMenuItem_Click(object sender, EventArgs e)
        {
            ShowPage(_reportsPage, "Reports › Invoices this month");
            _reportsPage.Select("Invoices this month");
        }

        private void editSelectedMenuItem_Click(object sender, EventArgs e)
        {
            RunEditSelected("Edit › Edit selected order…");
        }

        private void newOrderMenuItem_Click(object sender, EventArgs e)
        {
            ShowPage(_ordersPage, "Edit › New order…");
            trace.In("Edit › New order…", "same path as the New Order button on the detail panel");
            // The detail panel's New Order button does the work; the menu is a second entry point.
            var button = _ordersPage.Controls.Find("newOrderButton", true).FirstOrDefault() as Button;
            button?.PerformClick();
            UpdateStatusBar();
        }

        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            trace.In("File › Exit", "");
            trace.Finding("Exit is a session action on the web", "Close() ended the desktop PROCESS; here it would end only this browser session (Application.Exit) — Module 4 turns it into Sign out");
            Notify.Info("On the web, Exit ends this session only — see Module 4 (Sign out).");
        }

        private void aboutMenuItem_Click(object sender, EventArgs e)
        {
            trace.In("Help › About", "");
            trace.Server("MessageBox.Show(About)", "modal on purpose: the user opened it and dismisses it — not a status message");
            MessageBox.Show("OrderDesk 4.0 (Module 3) — LegacyOrderDesk 3.2 migrated to Wisej.NET.\nNavigation, modal EditOrderDialog with disposal, Toast confirmations.", "About");
        }

        // ── button bar ───────────────────────────────────────────────────────────────

        /// <summary>Success path: the using-block edit → Save → Toast; trace shows DialogResult and IsDisposed.</summary>
        private void buttonEditSelected_Click(object sender, EventArgs e)
        {
            RunEditSelected("Edit selected ✓");
        }

        private void RunEditSelected(string origin)
        {
            ShowPage(_ordersPage, origin);
            HideBanner();
            SetStatus("working");
            var order = _ordersPage.SelectedOrder;
            trace.In(origin, order == null ? "no selection" : "Order " + order.Id + " · " + order.CustomerName);

            bool saved = _ordersPage.EditSelected(origin);      // blocks here while the dialog is open

            UpdateStatusBar();
            SetStatus("idle");
            if (saved)
                ShowBanner("✓ Saved through the using block — dlg.IsDisposed = true, confirmation is a Toast (no extra click). LiveInstances = " + EditOrderDialog.LiveInstances + ".", BannerKind.Good);
            else
                ShowBanner("Dialog cancelled — still disposed by the using block. LiveInstances = " + EditOrderDialog.LiveInstances + ".", BannerKind.Neutral);
        }

        /// <summary>Failure path: the WinForms habit — five dialogs opened and closed, never disposed.</summary>
        private void buttonLeak_Click(object sender, EventArgs e)
        {
            ShowPage(_ordersPage, "Leak dialogs ✕");
            HideBanner();
            SetStatus("working");
            trace.In("Leak dialogs ✕", "the WinForms way: var dlg = new EditOrderDialog(order); dlg.ShowDialog(); … no Dispose");
            trace.Server("before", "LiveInstances=" + EditOrderDialog.LiveInstances + " · Application.OpenForms.Count=" + Application.OpenForms.Count);

            var order = _ordersPage.SelectedOrder ?? _service.Find(1042);
            for (int i = 1; i <= 5; i++)
            {
                // ShowDialog(onclose) is modal in the browser only and returns at once, so the lab can
                // close the dialog itself in the same handler — "opened and closed", exactly like a user
                // pressing Cancel five times. Nothing disposes it.
                var dlg = new EditOrderDialog(order, _service);
                dlg.ShowDialog((form, result) => { });
                dlg.Close();
                _leaked.Add(dlg);
                trace.Server("dialog #" + i, "ShowDialog() → Close() · IsDisposed=" + dlg.IsDisposed + " · LiveInstances=" + EditOrderDialog.LiveInstances);
            }

            int alive = CountAliveDialogs();
            trace.Fail("Application.OpenForms.Count", Application.OpenForms.Count + " (closed dialogs are no longer 'open' — but they still exist)");
            trace.Fail("Application.FindComponents(c => c is EditOrderDialog)", alive + " closed EditOrderDialog instances still registered in this session");
            trace.Fail("EditOrderDialog.LiveInstances", EditOrderDialog.LiveInstances + " constructed, never disposed (process-wide counter)");
            trace.Finding("closed dialogs are not disposed", "ShowDialog keeps the instance for reuse — the CALLER must Dispose (using block)");

            UpdateStatusBar();
            SetStatus("alarm");
            ShowBanner("✖ closed dialogs are not disposed — LiveInstances = " + EditOrderDialog.LiveInstances + " after 5 open/close cycles (FindComponents: " + alive + " still registered in this session).", BannerKind.Bad);
        }

        /// <summary>Recovery: dispose what leaked, then run the same five through a using block.</summary>
        private void buttonDisposeFix_Click(object sender, EventArgs e)
        {
            ShowPage(_ordersPage, "Dispose fix ✓");
            HideBanner();
            SetStatus("working");
            trace.In("Dispose fix ✓", "using (var dlg = new EditOrderDialog(order)) { dlg.ShowDialog(); }");

            var leaked = FindAliveDialogs();
            int fromRegistry = leaked.Count;
            foreach (var d in _leaked.Where(d => !d.IsDisposed))
                if (!leaked.Contains(d)) leaked.Add(d);
            _leaked.Clear();
            if (leaked.Count > 0)
            {
                trace.Server("cleanup", "disposing " + leaked.Count + " leaked dialog(s) · " + fromRegistry + " found with Application.FindComponents, " + (leaked.Count - fromRegistry) + " from the lab's own list");
                foreach (var d in leaked)
                    d.Dispose();
                trace.Ok("leaked dialogs disposed", "LiveInstances=" + EditOrderDialog.LiveInstances);
            }
            else
            {
                trace.Server("cleanup", "nothing leaked — click 'Leak dialogs ✕' first to see the difference");
            }

            var order = _ordersPage.SelectedOrder ?? _service.Find(1042);
            for (int i = 1; i <= 5; i++)
            {
                EditOrderDialog disposedRef;
                using (var dlg = new EditOrderDialog(order, _service))
                {
                    disposedRef = dlg;
                    dlg.ShowDialog((form, result) => { });
                    dlg.Close();
                }
                trace.Ok("using #" + i, "ShowDialog() → Close() → Dispose() · IsDisposed=" + disposedRef.IsDisposed + " · LiveInstances=" + EditOrderDialog.LiveInstances);
            }

            int alive = CountAliveDialogs();
            trace.Ok("Application.FindComponents(c => c is EditOrderDialog)", alive + " alive");
            trace.Finding("using block frees the dialog every time", "an intentionally reused dialog must instead call ResetState() before each ShowDialog");

            UpdateStatusBar();
            SetStatus("idle");
            ShowBanner("✓ using block: 5 dialogs opened, closed and disposed — LiveInstances = " + EditOrderDialog.LiveInstances + ", " + alive + " alive in this session.", BannerKind.Good);
        }

        /// <summary>Before/after: the blocking "Saved." MessageBox, then the non-blocking Toast.</summary>
        private void buttonToast_Click(object sender, EventArgs e)
        {
            HideBanner();
            SetStatus("working");
            trace.In("Saved via MessageBox ✕ → Toast ✓", "");
            trace.Fail("MessageBox.Show(\"Saved.\")", "blocking — this handler is suspended until OK is clicked (one extra click, no decision to make)");

            var sw = Stopwatch.StartNew();
            var result = MessageBox.Show("Saved.");               // ✕ the legacy confirmation
            sw.Stop();
            trace.In("MessageBox.Show returned", "DialogResult." + result + " after " + sw.ElapsedMilliseconds + " ms — the user paid a click for an informational message");

            Notify.Saved("Order 1042 saved.");                     // ✓ Toast (Shared/Notify.cs)
            trace.Ok("Toast \"Order 1042 saved.\"", "non-blocking · handler continued immediately · AutoCloseDelay 3500 ms · BottomRight");
            trace.Finding("notification policy", "informational → Toast/AlertBox · decisions & validation failures → MessageBox stays modal");

            SetStatus("idle");
            ShowBanner("✓ \"Saved.\" moved from a blocking MessageBox (waited " + sw.ElapsedMilliseconds + " ms for OK) to a Toast that costs no click.", BannerKind.Good);
        }

        /// <summary>Progress path: a Timer walks the OrdersPage controls in tab order.</summary>
        private void buttonTabOrder_Click(object sender, EventArgs e)
        {
            ShowPage(_ordersPage, "Tab order / docking");
            HideBanner();
            if (timerTabOrder.Enabled)
            {
                timerTabOrder.Stop();
                RemoveHighlight();
                trace.Server("Tab order / docking", "restarted");
            }

            trace.In("Tab order / docking", "walk the ported OrdersPage in TabIndex order");
            _walk = TabOrderWalker.Collect(_ordersPage, HighlightName);
            _walkIndex = 0;
            trace.Server("TabOrderWalker.Collect(OrdersPage)", _walk.Count + " controls · docking: headerLabel Top 36 · detailPanel Right 220 · ordersGrid Fill");
            SetStatus("working");
            timerTabOrder.Start();
        }

        private void timerTabOrder_Tick(object sender, EventArgs e)
        {
            if (_walkIndex >= _walk.Count)
            {
                timerTabOrder.Stop();
                RemoveHighlight();
                trace.Finding("tab order preserved", "TabIndex 0 ordersGrid → 1 detailPanel (0 Customer · 1 PO · 2 Lines · 3 New Order · 4 Print · 5 Export)");
                trace.Finding("anchors → docking", "OrdersForm anchored a 496×292 grid at a fixed 720×341 ClientSize; the page docks and fills whatever the browser gives");
                trace.Ok("Tab order / docking", "walk complete · " + _walk.Count + " controls");
                SetStatus("idle");
                ShowBanner("✓ Tab order walked: " + _walk.Count + " controls, TabIndex values preserved; Anchor → Dock (grid Fill · detail Right 220 · header Top).", BannerKind.Good);
                return;
            }

            var info = _walk[_walkIndex++];
            Highlight(info);
            trace.Server(TabOrderWalker.Describe(info), TabOrderWalker.Layout(info));
            if (info.Control.TabStop && info.Control.Enabled && info.Control.CanFocus)
            {
                try { info.Control.Focus(); } catch { /* focus is best-effort in the lab */ }
            }
        }

        private void Highlight(TabStopInfo info)
        {
            if (_highlight == null || _highlight.IsDisposed)
            {
                _highlight = new Panel
                {
                    Name = HighlightName,
                    BackColor = Color.FromArgb(90, 232, 161, 60),
                    BorderStyle = BorderStyle.Solid,
                    TabStop = false,
                };
                _ordersPage.Controls.Add(_highlight);
            }
            _highlight.Bounds = info.Bounds;
            _highlight.Visible = true;
            _highlight.BringToFront();
        }

        private void RemoveHighlight()
        {
            if (_highlight != null && !_highlight.IsDisposed)
            {
                _ordersPage.Controls.Remove(_highlight);
                _highlight.Dispose();
            }
            _highlight = null;
        }

        // ── helpers ──────────────────────────────────────────────────────────────────

        private static List<EditOrderDialog> FindAliveDialogs()
            => Application.FindComponents(c => c is EditOrderDialog d && !d.IsDisposed)
                          .OfType<EditOrderDialog>()
                          .ToList();

        private static int CountAliveDialogs() => FindAliveDialogs().Count;

        private void UpdateStatusBar()
        {
            statusPanelMain.Text = "Ready · " + _ordersPage.RowCount + " orders · multi-user web · session " + Application.SessionId.Substring(0, 8);
            statusPanelDialogs.Text = "Dialogs alive: " + EditOrderDialog.LiveInstances;
        }

        private void SetStatus(string state)
        {
            labelStatus.Text = "● " + state;
            switch (state)
            {
                case "working": labelStatus.ForeColor = Color.FromArgb(255, 214, 130); break;
                case "alarm": labelStatus.ForeColor = Color.FromArgb(255, 170, 150); break;
                default: labelStatus.ForeColor = Color.White; break;
            }
        }

        private enum BannerKind { Good, Bad, Neutral }

        private void ShowBanner(string text, BannerKind kind)
        {
            labelBanner.Text = text;
            switch (kind)
            {
                case BannerKind.Good:
                    labelBanner.BackColor = Palette.GoodSoft; labelBanner.ForeColor = Palette.Good; break;
                case BannerKind.Bad:
                    labelBanner.BackColor = Palette.BadSoft; labelBanner.ForeColor = Palette.Bad; break;
                default:
                    labelBanner.BackColor = Palette.AccentSoft; labelBanner.ForeColor = Palette.Accent; break;
            }
            labelBanner.Visible = true;
        }

        private void HideBanner() => labelBanner.Visible = false;
    }
}
