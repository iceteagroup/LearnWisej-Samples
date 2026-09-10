using System;
using System.Globalization;
using System.Threading;
using OrderDesk.Dialogs;
using OrderDesk.Domain;
using OrderDesk.Shell;
using OrderDesk.Views;
using Wisej.Web;

namespace OrderDesk
{
    /// <summary>
    /// Module 3 · Forms, navigation, layouts and modal workflow.
    ///
    /// Left, top:    the ported application shell (Shell/AppShell): MenuBar + ToolBar + a content host
    ///               that swaps the three screens (Screens/OrdersScreen, CustomersScreen, ReportsScreen)
    ///               and a StatusBar — the product, running.
    /// Left, bottom: dialog lifetime. The DialogTracker counter, the correct disposal path, the leak,
    ///               the intentional-reuse pattern, a decision that stays modal, and a blocking handler
    ///               next to Application.StartTask with a Timer-driven progress bar as the probe.
    /// Right:        the migration log (live trace) and the banner that explains each path.
    /// </summary>
    public partial class MainPage : Page
    {
        private readonly OrderService _orderService = new OrderService();
        private readonly CustomerService _customerService = new CustomerService();

        // The intentional-reuse pattern: one dialog for the life of the page, Bind(order) before each show.
        private EditOrderDialog _reusableDialog;

        // The blocking-vs-StartTask probe.
        private DateTime _opStarted;
        private bool _opRunning;
        private int _ticksWhileRunning;

        public MainPage()
        {
            InitializeComponent();
            // Page-lifetime objects the designer must not own are released with the page (and the session).
            Disposed += MainPage_Disposed;
        }

        private void MainPage_Load(object sender, EventArgs e)
        {
            trace.Add(TraceKind.Server, "startup", "Default.json → OrderDesk.Program.Main → Application.MainPage = new MainPage()");
            trace.Add(TraceKind.Server, "session", $"new browser session {Short(Application.SessionId)} · {Application.SessionCount} session(s) in this process");
            trace.Add(TraceKind.Server, "shell", "MenuStrip → MenuBar · buttons → ToolBar · StatusStrip → StatusBar · Form → screen host (Dock = Fill)");

            shell.NavigateTo(AppShell.OrdersScreenName);
            RefreshDialogCounter();
            Ui.SetStatus(labelStatus, "shell running · Orders screen", Ui.Ok);
            Ui.ShowBanner(labelBanner,
                "Double-click an order in the shell (or press Edit (disposed)): the modal EditOrderDialog opens, Save runs the reused OrderService, " +
                "a Toast confirms, and the dialog is disposed — the live count below returns to where it was. Then press Edit (leak ×1) to see the desktop habit.",
                Ui.BannerKind.Ok);
        }

        #region The shell (Shell/AppShell + Screens/*) — trace forwarding

        /// <summary>The shell and its screens know nothing about the console; their Trace event lands here.</summary>
        private void shell_Trace(object sender, TraceEventArgs e)
        {
            trace.Add(e.Kind, e.Name, e.Payload);
            RefreshDialogCounter();
        }

        private void shell_Navigated(object sender, EventArgs e)
        {
            Ui.SetStatus(labelStatus, $"shell running · {shell.CurrentScreen} screen", Ui.Ok);
        }

        private Order SelectedOrder
        {
            get
            {
                var order = shell.Orders.SelectedOrder;
                if (order == null)
                {
                    Ui.SetStatus(labelStatus, "select an order on the Orders screen first", Ui.Warn);
                    trace.Add(TraceKind.Server, "no order selected", "navigate to Orders and select a row");
                }
                return order;
            }
        }

        #endregion

        #region Dialog lifetime · DialogTracker

        private void RefreshDialogCounter()
        {
            int mine = DialogTracker.LiveFor(Application.SessionId);
            int all = DialogTracker.LiveTotal;
            string reuse = _reusableDialog == null ? "none" : "1 (kept on purpose, disposed with the page)";
            labelDialogCount.Text =
                $"live EditOrderDialog instances   this session {mine}   ·   process-wide {all}\n" +
                $"reuse instance {reuse}";
            labelDialogCount.ForeColor = mine > (_reusableDialog == null ? 0 : 1) ? Ui.Error : Ui.Muted;
        }

        /// <summary>Success path: the same code the double-click runs (Screens/OrdersScreen.EditOrder).</summary>
        private void buttonEditGood_Click(object sender, EventArgs e)
        {
            var order = SelectedOrder;
            if (order == null) return;

            trace.Add(TraceKind.FromClient, "Edit (disposed)", $"order {order.Id} → OrdersScreen.EditOrder — the same path as the double-click");
            shell.NavigateTo(AppShell.OrdersScreenName);
            shell.Orders.EditOrder(order);

            Ui.ShowBanner(labelBanner,
                "✓ using (var dialog = new EditOrderDialog(…)) { var result = await dialog.ShowDialogAsync(); … }  — ShowDialog does not block, so the " +
                "result is awaited; Dispose runs when the await completes. Watch the counter: +1 while the dialog is open, back to the previous value when it closes.",
                Ui.BannerKind.Ok);
            Ui.SetStatus(labelStatus, $"editing order {order.Id} — disposed on close", Ui.Ok);
        }

        /// <summary>Failure path: the desktop habit, line for line. Nothing disposes the dialog — ever.</summary>
        private void buttonEditLeak_Click(object sender, EventArgs e)
        {
            var order = SelectedOrder;
            if (order == null) return;
            string session = Application.SessionId;

            trace.Add(TraceKind.FromClient, "Edit (leak ×1)", $"order {order.Id} — the desktop habit: new + ShowDialog, no using, no Dispose");

            // ✕ desktop: var dlg = new EditOrderDialog(...); if (dlg.ShowDialog(this) == DialogResult.OK) {...}
            //    On the desktop the forgotten dialog died with the process at the end of the day.
            //    Here the only change is the callback (ShowDialog returns at once) — the missing Dispose is the same.
            var dlg = new EditOrderDialog(order, _customerService.GetCustomers());
            trace.Add(TraceKind.Server, "new EditOrderDialog", $"order {order.Id} · live dialogs: session {DialogTracker.LiveFor(session)} · process {DialogTracker.LiveTotal}   ✕ no using, no Dispose planned");
            RefreshDialogCounter();

            dlg.ShowDialog((form, result) =>
            {
                if (result == DialogResult.OK)
                {
                    var saved = _orderService.Save(dlg.Order);
                    trace.Add(TraceKind.Server, "OrderService.Save", $"order {saved.Id} · CalculateOrderTotal = {N2(saved.Total)}");
                    shell.Orders.Reload(saved.Id);
                    Ui.Toast($"Order {saved.Id} saved.");
                }
                // ✕ form.Dispose() is missing — Close() hid the window; the server-side object stays alive with the session.
                int mine = DialogTracker.LiveFor(session);
                trace.Add(TraceKind.FromClient, "EditOrderDialog closed", $"DialogResult = {result} — closed, NOT disposed · live dialogs: session {mine} · process {DialogTracker.LiveTotal}");
                trace.Add(TraceKind.Boundary, "dialog leak", $"{mine} live dialog(s) held by session {Short(session)} — on the desktop the process end cleaned up; on the server nothing will until the session ends");
                RefreshDialogCounter();
                Ui.ShowBanner(labelBanner,
                    $"✕ Leak: {mine} live EditOrderDialog(s) in this session (process-wide {DialogTracker.LiveTotal}). Close() hides a Wisej.NET form, it does not dispose it, and " +
                    "nobody holds a reference any more. On the desktop the process ended with the user's day; on the server this session lives for hours and the process for weeks. " +
                    "Press again: +1 every time. Recovery: Edit (disposed) — or Reuse one dialog.",
                    Ui.BannerKind.Error);
                Ui.SetStatus(labelStatus, $"{mine} leaked dialog(s) in this session", Ui.Error);
            });
        }

        /// <summary>Alternative: reuse one instance on purpose — visible, reset with Bind, disposed with the page.</summary>
        private void buttonReuse_Click(object sender, EventArgs e)
        {
            var order = SelectedOrder;
            if (order == null) return;
            string session = Application.SessionId;

            trace.Add(TraceKind.FromClient, "Reuse one dialog", $"order {order.Id}");
            if (_reusableDialog != null && _reusableDialog.Visible)
            {
                // Re-showing a form that is already visible throws — bring the open instance to front instead.
                _reusableDialog.BringToFront();
                trace.Add(TraceKind.Server, "EditOrderDialog (reused)", "already open — brought to front (ShowDialog on a visible form throws)");
                return;
            }
            if (_reusableDialog == null)
            {
                _reusableDialog = new EditOrderDialog(order, _customerService.GetCustomers());
                trace.Add(TraceKind.Server, "new EditOrderDialog (reused)", $"created once for this page · live dialogs: session {DialogTracker.LiveFor(session)} · process {DialogTracker.LiveTotal}");
            }
            else
            {
                _reusableDialog.Bind(order);
                trace.Add(TraceKind.Server, "EditOrderDialog.Bind", $"order {order.Id} — fields and DialogResult reset before re-showing (stale state is the reuse trap)");
            }
            RefreshDialogCounter();

            _reusableDialog.ShowDialog((form, result) =>
            {
                trace.Add(TraceKind.FromClient, "EditOrderDialog closed", $"DialogResult = {result} — kept alive on purpose, not disposed");
                if (result == DialogResult.OK)
                {
                    var saved = _orderService.Save(_reusableDialog.Order);
                    trace.Add(TraceKind.Server, "OrderService.Save", $"order {saved.Id} · CalculateOrderTotal = {N2(saved.Total)}");
                    shell.Orders.Reload(saved.Id);
                    Ui.Toast($"Order {saved.Id} saved.");
                }
                RefreshDialogCounter();
                Ui.ShowBanner(labelBanner,
                    "✓ Reuse on purpose: one EditOrderDialog lives in a field for the page's lifetime; Bind(order) resets the fields and the DialogResult before every show, " +
                    "and MainPage.Dispose releases it. The counter stays +1 by design — the difference from the leak is that somebody owns the reference.",
                    Ui.BannerKind.Ok);
                Ui.SetStatus(labelStatus, "one reusable dialog, reset with Bind", Ui.Ok);
            });
        }

        /// <summary>A decision that stays modal: Yes/No must be answered before anything happens.</summary>
        private async void buttonDelete_Click(object sender, EventArgs e)
        {
            var order = SelectedOrder;
            if (order == null) return;

            trace.Add(TraceKind.FromClient, "Delete order…", $"order {order.Id} — a decision: this one stays modal");
            trace.Add(TraceKind.ToClient, "MessageBox.ShowAsync", $"\"Delete order {order.Id}?\" YesNo — awaited; the handler yields until the user answers");

            var result = await MessageBox.ShowAsync($"Delete order {order.Id}?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            trace.Add(TraceKind.FromClient, "MessageBox closed", $"DialogResult = {result}");

            if (result == DialogResult.Yes)
            {
                _orderService.Delete(order.Id);
                trace.Add(TraceKind.Server, "OrderService.Delete", $"order {order.Id} removed from the shared repository (every session sees it)");
                shell.Orders.Reload();
                Ui.Toast($"Order {order.Id} deleted.");
                trace.Add(TraceKind.ToClient, "Ui.Toast", $"\"Order {order.Id} deleted.\" — the confirmation is not a decision, so it does not block");
                Ui.ShowBanner(labelBanner,
                    $"✓ Delete order {order.Id}: the question blocked (MessageBox.ShowAsync, awaited) because a wrong answer destroys data; the confirmation afterwards is a Toast. " +
                    "That is the whole notification review: modal for decisions and validation, non-blocking for information.",
                    Ui.BannerKind.Ok);
                Ui.SetStatus(labelStatus, $"order {order.Id} deleted after a modal Yes", Ui.Ok);
            }
            else
            {
                Ui.ShowBanner(labelBanner,
                    $"✓ Delete order {order.Id} cancelled — the modal question did its job. Note the trace: ShowAsync returned control to the server while the box was open; " +
                    "the answer arrived as a normal event and the handler resumed after the await.",
                    Ui.BannerKind.Ok);
                Ui.SetStatus(labelStatus, "delete cancelled (No)", Ui.Ok);
            }
        }

        #endregion

        #region Blocking vs Application.StartTask (the Timer is the responsiveness probe)

        private void BeginOperation()
        {
            _opStarted = DateTime.Now;
            _opRunning = true;
            _ticksWhileRunning = 0;
            progressOp.Value = 0;
            labelProgress.Text = "0.0 s · 0 ticks";
            buttonBlocking.Enabled = false;
            buttonStartTask.Enabled = false;
            timerProgress.Start();
        }

        /// <summary>Failure path: the desktop habit — do the work in the handler and let the UI wait.</summary>
        private void buttonBlocking_Click(object sender, EventArgs e)
        {
            trace.Add(TraceKind.FromClient, "Blocking op (3 s)", "Thread.Sleep(3000) inside the Click handler  ✕");
            BeginOperation();
            trace.Add(TraceKind.Server, "blocking.begin", $"{Stamp()} — the request thread sleeps; nothing (not even Timer.Start) reaches the browser until it returns");

            // ✕ desktop: only this user's window froze; the message pump waited.
            // ✕ server:  this session's request is held; every other event for this session (timer ticks, clicks) queues behind it.
            Thread.Sleep(3000);

            _opRunning = false;
            trace.Add(TraceKind.Server, "blocking.end", $"{Stamp()} — both lines arrived together: for 3.0 s this session answered nothing");
            Ui.ShowBanner(labelBanner,
                "✕ Blocking: Thread.Sleep(3000) on the request thread. The browser received the begin trace, the timer start and the end trace in ONE response, " +
                "3 s late — the progress bar never moved, no tick was answered (see Timer.Tick below). On the desktop only this user waited; on the server the " +
                "session was frozen and long work like this would time out the request. Recovery: StartTask (3 s).",
                Ui.BannerKind.Warn);
            Ui.SetStatus(labelStatus, "session frozen for 3 s by a blocking handler", Ui.Warn);
        }

        /// <summary>Progress path: the work leaves the request; the result is pushed with Application.Update.</summary>
        private void buttonStartTask_Click(object sender, EventArgs e)
        {
            trace.Add(TraceKind.FromClient, "StartTask (3 s)", "Application.StartTask(() => { Thread.Sleep(3000); Application.Update(page, …); })  ✓");
            BeginOperation();
            trace.Add(TraceKind.Server, "StartTask.begin", $"{Stamp()} — the handler returns now; the work runs on a worker thread that keeps the session context");
            Ui.SetStatus(labelStatus, "background work running — the UI keeps answering", Ui.Ok);

            Application.StartTask(() =>
            {
                Thread.Sleep(3000);   // the same 3 s of work, off the request thread

                // ✓ Application.Update(page, callback) runs the callback in this session's context and pushes the changes once.
                Application.Update(this, () =>
                {
                    if (IsDisposed) return;
                    _opRunning = false;
                    trace.Add(TraceKind.Server, "StartTask.end", $"{Stamp()} — Application.Update(this, …) pushed this line from the worker thread");
                    Ui.Toast("Background work finished (3 s) — the page never froze.");
                    Ui.ShowBanner(labelBanner,
                        "✓ StartTask: the handler returned at once, the Timer kept ticking (the progress bar moved and the tick count grew), and the worker thread pushed " +
                        "its result with Application.Update(this, …). This is the web shape of every long operation the desktop app ran on the UI thread.",
                        Ui.BannerKind.Ok);
                    Ui.SetStatus(labelStatus, "background work finished · UI stayed responsive", Ui.Ok);
                });
            });
        }

        private void timerProgress_Tick(object sender, EventArgs e)
        {
            double elapsed = (DateTime.Now - _opStarted).TotalSeconds;
            if (_opRunning)
            {
                _ticksWhileRunning++;
                progressOp.Value = Math.Min(99, (int)(elapsed / 3.0 * 100));
                labelProgress.Text = $"{elapsed:0.0} s · {_ticksWhileRunning} ticks";
                return;
            }

            timerProgress.Stop();
            progressOp.Value = 100;
            labelProgress.Text = $"done · {_ticksWhileRunning} ticks";
            buttonBlocking.Enabled = true;
            buttonStartTask.Enabled = true;
            trace.Add(TraceKind.Server, "Timer.Tick", $"{_ticksWhileRunning} tick(s) were answered while the operation ran (0 = the session was frozen)");
        }

        #endregion

        private void buttonClear_Click(object sender, EventArgs e) => trace.Clear();

        private void MainPage_Disposed(object sender, EventArgs e)
        {
            // The reusable dialog is owned by the page: this is the one place it is disposed.
            _reusableDialog?.Dispose();
            _reusableDialog = null;
        }

        private static string Stamp() => DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);

        private static string N2(decimal value) => value.ToString("N2", CultureInfo.InvariantCulture);

        private static string Short(string id) => string.IsNullOrEmpty(id) ? "?" : (id.Length > 8 ? id.Substring(0, 8) : id);
    }
}
