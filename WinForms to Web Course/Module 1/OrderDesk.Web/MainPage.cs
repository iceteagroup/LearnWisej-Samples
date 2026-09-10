using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using OrderDesk.Domain;
using OrderDesk.Legacy;
using OrderDesk.Migration;
using OrderDesk.Shared;
using Wisej.Web;

namespace OrderDesk
{
    /// <summary>
    /// Module 1 — Migration discovery &amp; the first slice. "The assessment workbook, alive":
    /// the inventory of LegacyOrderDesk as a grid, the first-vertical-slice checklist that lights up as
    /// the buttons run, and the five buttons that exercise the success path (Run first slice), the
    /// finding (Second session), the failure (Export the desktop way), the recovery (Export via
    /// Download) and the progress path (Replay assessment on a Timer). Every step is written to the
    /// TracePanel on the right.
    /// </summary>
    public partial class MainPage : Page
    {
        private static readonly CultureInfo Money = CultureInfo.GetCultureInfo("en-US");

        private readonly OrderService _service = new OrderService();
        private List<Order> _sliceOrders;
        private int _replayIndex = -1;

        public MainPage()
        {
            InitializeComponent();
        }

        private static string SessionShort
        {
            get
            {
                var id = Application.SessionId ?? "";
                return id.Length >= 8 ? id.Substring(0, 8) : id;
            }
        }

        // ------------------------------------------------------------------ load

        private void MainPage_Load(object sender, EventArgs e)
        {
            trace.Server("Program.Main", "Application.MainPage = new MainPage()  · session " + SessionShort);
            trace.Server("Application.SessionCount", Application.SessionCount + " active session(s) in this process");
            trace.Finding("runtime shift", "one .exe for one user → one server, many sessions (each with its own session, UI tree and connection)");

            FillWorkbook(AssessmentWorkbook.Items);
            trace.Server("AssessmentWorkbook", AssessmentWorkbook.Items.Count + " items · " + AssessmentWorkbook.Summary());

            // The "second tab" half of the Second session demo: a brand-new session finds the static already set.
            if (AppState.CurrentUser != null)
            {
                bool mine = AppState.CurrentUserSetBy == SessionShort;
                trace.Fail("Legacy/AppState.CurrentUser", "already \"" + AppState.CurrentUser.UserName + "\" in this NEW session — set by session " + AppState.CurrentUserSetBy + (mine ? " (this tab)" : " (ANOTHER tab)"));
                trace.Finding("static-state", "nobody signed in here, yet the static says kelly: a static field is process-wide, not per-user (Module 4)");
                ShowBanner(Palette.WarnSoft, Palette.Warn, "This new session already sees AppState.CurrentUser = \"" + AppState.CurrentUser.UserName + "\" — written by session " + AppState.CurrentUserSetBy + ". Static state is shared by every browser tab.");
            }

            SetStatus("idle");
        }

        // ------------------------------------------------------------------ success: the first slice

        private void buttonRunSlice_Click(object sender, EventArgs e)
        {
            trace.In("click", "Run first slice ✓");
            SetStatus("working");
            HideBanner();

            trace.Server("Startup.cs", "Kestrel owns the process · app.UseWisej() · Default.json startup = OrderDesk.Program.Main");
            trace.Server("Program.Main", "runs once per SESSION (not per process) · Application.MainPage = new MainPage() · session " + SessionShort);
            Tick(labelSliceStartup, "Startup — Startup.cs + Program.Main");

            trace.Server("OrderService.GetAll()", "the same call OrdersForm.ReloadGrid() made — Domain/OrderService.cs reused as-is");
            var all = _service.GetAll();
            _sliceOrders = all.Take(5).ToList();
            trace.Server("OrderService.GetAll()", all.Count + " orders in the store · the slice shows the first 5 (newest first)");

            FillOrders(_sliceOrders);
            trace.Out("gridOrders.Rows", "5 rows → browser: " + string.Join(" · ", _sliceOrders.Select(o => o.Id + " " + o.CustomerName + " " + o.Total.ToString("C2", Money) + " " + o.Status)));
            Tick(labelSliceScreen, "One read-only screen — Orders");
            Tick(labelSliceGrid, "One grid — DataGridView, 5 rows");

            trace.Finding("grid-volume", all.Count + " rows would ALL travel to the browser the desktop way — verdict adapt (Module 5: VirtualMode + OrderQuery)");
            trace.Ok("first slice", "startup ✓ · read-only screen ✓ · grid ✓ — parity first, modernization second, deployment third");
            ShowBanner(Palette.GoodSoft, Palette.Good, "First slice running: Program.Main → OrderService.GetAll() → 5 orders in a read-only grid. Parity first.");
            SetStatus("idle");
        }

        // ------------------------------------------------------------------ finding: the static user

        private void buttonSecondSession_Click(object sender, EventArgs e)
        {
            trace.In("click", "Second session");
            SetStatus("working");

            trace.Server("Application.SessionCount", Application.SessionCount + " active session(s) · this session " + SessionShort);

            var before = AppState.CurrentUser;
            if (before == null)
                trace.Server("Legacy/AppState.CurrentUser", "null — nobody has signed in yet in this PROCESS");
            else
                trace.Fail("Legacy/AppState.CurrentUser", "\"" + before.UserName + "\" is already there — set by session " + AppState.CurrentUserSetBy + (AppState.CurrentUserSetBy == SessionShort ? " (this tab)" : " (ANOTHER tab)"));

            var kelly = new User { Id = 1, UserName = "kelly", DisplayName = "Kelly", Company = "Acme" };
            AppState.CurrentUser = kelly;                        // ✕ the LegacyOrderDesk Program.Main line, copied as-is
            AppState.CurrentUserSetBy = SessionShort;
            trace.Server("AppState.CurrentUser = kelly", "the LegacyOrderDesk Program.Main line, copied as-is (static field)");
            trace.Server("read back", "AppState.CurrentUser → \"" + AppState.CurrentUser.UserName + "\" (" + AppState.CurrentUser.Company + ") · the same static instance for every session in this process");
            trace.Finding("static-state", "open a second browser tab: its Load trace shows kelly already signed in — per-user state cannot live in a static (Module 4: Application.Session)");

            Tick(labelSliceUser, "User context / login — kelly (static ✕ → session, Module 4)", Palette.Warn);
            ShowBanner(Palette.WarnSoft, Palette.Warn, "AppState.CurrentUser = \"kelly\" is a static field. Open a second tab of this app: its trace shows kelly already signed in. Static ≠ per-user.");
            SetStatus("alarm");
        }

        // ------------------------------------------------------------------ failure: the desktop export on the server

        private void buttonExportDesktop_Click(object sender, EventArgs e)
        {
            trace.In("click", "Export (desktop way) ✕");
            SetStatus("working");
            var orders = SliceOrders();

            trace.Server("ExcelExport.ExportToExcel", "new Excel.Application() — a COM server on the machine running the code: the SERVER");
            try
            {
                var path = ExcelExport.ExportToExcel(orders);
                trace.Ok("ExcelExport.ExportToExcel", "unexpected: Excel answered and wrote " + path + " (ORDERDESK_HAS_EXCEL=1 on an interactive desktop)");
            }
            catch (COMException ex)
            {
                trace.Fail("COMException 0x" + ex.HResult.ToString("X8"), Shorten(ex.Message, 140));
                trace.Finding("office", "Office Automation is unsupported on a server — verdict redesign: managed writer + Application.Download (Module 6)");
            }

            trace.Server("LocalExport.WriteCsv", @"the desktop fallback: Directory.CreateDirectory(C:\Orders) + File.WriteAllText(out.csv)");
            try
            {
                LocalExport.WriteCsv(orders);
                trace.Ok("LocalExport.WriteCsv", "unexpected: the file was written");
            }
            catch (InvalidOperationException ex)
            {
                trace.Fail("would write " + LocalExport.TargetPath + " on the SERVER", Shorten(ex.Message, 160));
                trace.Finding("file-system", @"C:\Orders is the server's disk; the user never sees it — verdict adapt: Application.Download (now), App_Data (Module 6)");
            }

            Tick(labelSliceBoundary, @"One deployment boundary — Excel Interop + C:\Orders hit the SERVER", Palette.Bad, "✖");
            ShowBanner(Palette.BadSoft, Palette.Bad, @"Desktop export failed on the server: Excel Interop needs an interactive desktop, and C:\Orders\out.csv would land on the server's disk, not the user's PC.");
            SetStatus("alarm");
        }

        // ------------------------------------------------------------------ recovery: the same CSV as a browser download

        private void buttonExportDownload_Click(object sender, EventArgs e)
        {
            trace.In("click", "Export (Download) ✓");
            SetStatus("working");
            var orders = SliceOrders();

            var csv = LocalExport.ToCsv(orders);                 // ✓ the pure-formatting half of LocalExport, reused as-is
            var bytes = Encoding.UTF8.GetBytes(csv);
            trace.Server("LocalExport.ToCsv", orders.Count + " rows · " + bytes.Length + " bytes — the pure formatting half of LocalExport, reused unchanged");

            Application.Download(new MemoryStream(bytes), "orders.csv");
            trace.Out("Application.Download", "orders.csv (" + bytes.Length + " bytes) → browser download · nothing written on the server");
            trace.Ok("browser boundary crossed", @"file/report path replaced: C:\Orders\out.csv → Application.Download(stream, ""orders.csv"")");

            Tick(labelSliceFile, "One file / report — orders.csv via Application.Download");
            Tick(labelSliceBoundary, "One deployment boundary — server → browser download");
            ShowBanner(Palette.GoodSoft, Palette.Good, "orders.csv streamed to the browser with Application.Download — the same CSV bytes LocalExport.ToCsv always produced, no server path.");
            Notify.Info("orders.csv sent to the browser.");
            SetStatus("idle");
        }

        // ------------------------------------------------------------------ progress: replay the assessment on a Timer

        private void buttonReplay_Click(object sender, EventArgs e)
        {
            trace.In("click", "Replay assessment");
            timerReplay.Stop();
            gridWorkbook.Rows.Clear();
            _replayIndex = 0;
            HideBanner();
            SetStatus("working");
            trace.Server("Wisej.Web.Timer", "Replay · " + timerReplay.Interval + " ms per item · " + AssessmentWorkbook.Items.Count + " items to tag");
            timerReplay.Start();
        }

        private void timerReplay_Tick(object sender, EventArgs e)
        {
            if (_replayIndex < 0 || _replayIndex >= AssessmentWorkbook.Items.Count)
            {
                timerReplay.Stop();
                _replayIndex = -1;
                trace.Ok("assessment complete", AssessmentWorkbook.Items.Count + " items · " + AssessmentWorkbook.Summary());
                ShowBanner(Palette.AccentSoft, Palette.Accent, "Assessment workbook complete: " + AssessmentWorkbook.Summary() + ". Business logic reused as-is; every desktop boundary is an explicit task.");
                SetStatus("idle");
                return;
            }

            var item = AssessmentWorkbook.Items[_replayIndex++];
            AddWorkbookRow(item);
            trace.Finding(item.RiskTag, item.Feature + " → " + item.Verdict + " (" + item.Effort + ") · " + item.Finding);
        }

        // ------------------------------------------------------------------ helpers

        private List<Order> SliceOrders()
        {
            if (_sliceOrders == null)
            {
                trace.Server("OrderService.GetAll()", "slice not run yet — loading the 5 orders the slice shows");
                _sliceOrders = _service.GetAll().Take(5).ToList();
                FillOrders(_sliceOrders);
            }
            return _sliceOrders;
        }

        private void FillWorkbook(IEnumerable<AssessmentItem> items)
        {
            gridWorkbook.Rows.Clear();
            foreach (var item in items)
                AddWorkbookRow(item);
        }

        private void AddWorkbookRow(AssessmentItem item)
        {
            int index = gridWorkbook.Rows.Add(item.Feature, item.Dependency, item.RiskTag, item.Verdict, item.Effort);
            var row = gridWorkbook.Rows[index];
            row.Cells[2].Style.ForeColor = AssessmentWorkbook.TagColor(item.RiskTag);
            row.Cells[2].Style.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            row.Cells[3].Style.ForeColor = AssessmentWorkbook.VerdictColor(item.Verdict);
            row.Cells[3].Style.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            row.Cells[0].ToolTipText = item.Source;
            row.Cells[1].ToolTipText = item.Finding;
        }

        private void FillOrders(IEnumerable<Order> orders)
        {
            gridOrders.Rows.Clear();
            foreach (var o in orders)
            {
                int index = gridOrders.Rows.Add(o.Id, o.CustomerName, o.Total, o.Status.ToString());
                var row = gridOrders.Rows[index];
                row.Cells[0].Style.ForeColor = Palette.Accent;
                row.Cells[0].Style.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
                row.Cells[3].Style.ForeColor = Palette.StatusColor(o.Status);
                row.Cells[3].Style.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            }
            labelOrdersHint.Visible = false;
            gridOrders.Visible = true;
        }

        private static void Tick(Label label, string text, System.Drawing.Color? color = null, string mark = "✓")
        {
            label.Text = mark + "  " + text;
            label.ForeColor = color ?? Palette.Good;
            label.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
        }

        private void SetStatus(string state)
        {
            labelStatus.Text = "● " + state;
            labelStatus.ForeColor = state == "alarm" ? Palette.Bad : state == "working" ? Palette.Warn : Palette.Good;
        }

        private void ShowBanner(System.Drawing.Color back, System.Drawing.Color fore, string text)
        {
            labelBanner.BackColor = back;
            labelBanner.ForeColor = fore;
            labelBanner.Text = text;
            labelBanner.Visible = true;
        }

        private void HideBanner() => labelBanner.Visible = false;

        private static string Shorten(string s, int max) => s == null ? "" : s.Length <= max ? s : s.Substring(0, max - 1) + "…";
    }
}
