using System;
using System.Globalization;
using IntegrationLab.Widgets;
using Wisej.Core;
using Wisej.Web;

namespace IntegrationLab
{
    /// <summary>
    /// IntegrationLab — Work Orders (Module 8 lab page).
    ///
    /// Left card:   GridWidget   — the vendor grid pulls JSON from the widget's postback URL (WebRequest).
    /// Middle card: LookupWidget — the same grid calls a [WebMethod] and awaits the marshaled result.
    /// Right card:  every message that crosses the wire, in both directions.
    /// Bottom bar:  success path (reload / page / sort), failure paths (invalid action, oversized page),
    ///              the WebMethod target switch, the postback URL, clear.
    ///
    /// This is a top-level Page (Application.MainPage), which is why its [WebMethod]
    /// below is discovered automatically and callable as App.MainPage.GetWorkOrders.
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();

            this.gridPostback.DataLoaded += gridPostback_DataLoaded;
            this.gridPostback.WidgetError += gridPostback_WidgetError;
            this.gridPostback.RowClicked += grid_RowClicked;
            this.gridPostback.Trace += (s, e) => AddTrace(e.Direction, "[postback] " + e.Name, e.Payload);

            this.gridLookup.DataLoaded += gridLookup_DataLoaded;
            this.gridLookup.WidgetError += gridLookup_WidgetError;
            this.gridLookup.RowClicked += grid_RowClicked;
            this.gridLookup.Trace += (s, e) => AddTrace(e.Direction, "[webmethod] " + e.Name, e.Payload);
        }

        private void MainPage_Load(object sender, EventArgs e)
        {
            AddTrace(TraceDirection.ServerToClient, "[postback] render → init(options)", this.gridPostback.ToJson());
            AddTrace(TraceDirection.ServerToClient, "[webmethod] render → init(options)", this.gridLookup.ToJson());
            AddTrace(TraceDirection.Server, "endpoints",
                "postback: WebRequest on GridWidget · webmethod: App.MainPage.GetWorkOrders + LookupWidget.GetWorkOrders");
            SetStatus(this.labelPostbackStatus, "loading…", StatusKind.Warn);
            SetStatus(this.labelLookupStatus, "loading…", StatusKind.Warn);
            UpdateTargetButton();
        }

        #region (a) The WebMethod on the top-level Page

        // ==== UNVERIFIED (top-level WebMethod, client call shape) ================
        // Instance method on Application.MainPage: Wisej registers it automatically and
        // the client calls it as
        //     App.MainPage.GetWorkOrdersAsync(page, size, sort, desc)            → Promise
        //     App.MainPage.GetWorkOrders(page, size, sort, desc, function (r) {}) → callback
        // (see wwwroot/lookup-init.js, _callWebMethod). Arguments are marshaled to the
        // typed parameters; the returned object is marshaled back to the caller.
        [WebMethod]
        public object GetWorkOrders(int page, int size, string sort, bool desc)
            => this.gridLookup.ExecuteGetWorkOrders("App.MainPage.GetWorkOrders", page, size, sort, desc);
        // ==========================================================================

        #endregion

        #region Buttons

        /// <summary>Success + recovery: both grids back to a known-good request, page 1.</summary>
        private void buttonReload_Click(object sender, EventArgs e)
        {
            HideBanner(this.labelPostbackBanner);
            HideBanner(this.labelLookupBanner);
            SetStatus(this.labelPostbackStatus, "loading…", StatusKind.Warn);
            SetStatus(this.labelLookupStatus, "loading…", StatusKind.Warn);
            this.gridPostback.Reload();
            this.gridLookup.Reload();
        }

        /// <summary>Progress: page through both datasets (the server knows the current page from dataLoaded).</summary>
        private void buttonNextPage_Click(object sender, EventArgs e)
        {
            this.gridPostback.NextPage();
            this.gridLookup.NextPage();
        }

        /// <summary>Sort by a whitelisted column; clicking again toggles the direction (vendor behavior).</summary>
        private void buttonSortStatus_Click(object sender, EventArgs e)
        {
            this.gridPostback.SortBy("status");
            this.gridLookup.SortBy("status");
        }

        /// <summary>
        /// Failure path 1 (postback only): the vendor GETs the postback URL with action=delete.
        /// The handler compares it against its fixed list and answers 400 with a short JSON
        /// body; the vendor shows a red error row; the adapter raises "error".
        /// </summary>
        private void buttonInvalidAction_Click(object sender, EventArgs e)
        {
            SetStatus(this.labelPostbackStatus, "400 expected…", StatusKind.Warn);
            this.gridPostback.LoadWithAction("delete");
        }

        /// <summary>
        /// Failure path 2 (both): size=1000 is above the bound (50). The postback handler
        /// answers 400; the WebMethod throws ArgumentException, which the client sees as
        /// a Wisej exception popup and a null result (the adapter turns that into "error").
        /// </summary>
        private void buttonOversized_Click(object sender, EventArgs e)
        {
            SetStatus(this.labelPostbackStatus, "400 expected…", StatusKind.Warn);
            SetStatus(this.labelLookupStatus, "exception expected…", StatusKind.Warn);
            this.gridPostback.LoadWithSize(1000);
            this.gridLookup.LoadWithSize(1000);
        }

        /// <summary>Switches the WebMethod grid between App.MainPage.* and this.* (RegisterWebMethods).</summary>
        private void buttonSwitchTarget_Click(object sender, EventArgs e)
        {
            HideBanner(this.labelLookupBanner);
            SetStatus(this.labelLookupStatus, "loading…", StatusKind.Warn);
            this.gridLookup.DataSourceMode = this.gridLookup.DataSourceMode == LookupWidget.ModePage
                ? LookupWidget.ModeWidget
                : LookupWidget.ModePage;
            UpdateTargetButton();
        }

        /// <summary>
        /// Shows the server-side postback URL (GetPostbackURL) with the middle redacted:
        /// it is session-scoped and short-lived, so it is displayed, never stored or shared.
        /// </summary>
        private void buttonShowUrl_Click(object sender, EventArgs e)
        {
            try
            {
                string url = this.gridPostback.PostbackUrl ?? "";
                AddTrace(TraceDirection.Server, "GetPostbackURL()", Redact(url) + "  (+ &action=load appended by the client)");
                AddTrace(TraceDirection.Server, "postback stats",
                    $"requests={this.gridPostback.RequestCount} rejected={this.gridPostback.RejectedCount} · webmethod calls={this.gridLookup.CallCount} rejected={this.gridLookup.RejectedCount}");
            }
            catch (Exception ex)
            {
                AddTrace(TraceDirection.Server, "GetPostbackURL() failed", ex.GetType().Name + ": " + ex.Message);
            }
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            this.listTrace.Items.Clear();
        }

        #endregion

        #region Widget → .NET events

        private void gridPostback_DataLoaded(object sender, DataLoadedEventArgs e)
        {
            HideBanner(this.labelPostbackBanner);
            SetStatus(this.labelPostbackStatus, $"{e.Count} rows · page {e.Page}/{e.Pages} · {e.Elapsed} ms", StatusKind.Normal);
            this.labelPostbackInfo.Text = this.gridPostback.LastRequest;
        }

        private void gridPostback_WidgetError(object sender, GridErrorEventArgs e)
        {
            SetStatus(this.labelPostbackStatus, e.Status > 0 ? $"HTTP {e.Status}" : "fault", StatusKind.Error);
            ShowBanner(this.labelPostbackBanner,
                (e.Status > 0 ? $"✖ HTTP {e.Status}: {e.Message}" : $"✖ {e.Phase}: {e.Message}") + "  → click “Reload both”.");
            this.labelPostbackInfo.Text = this.gridPostback.LastRequest;
        }

        private void gridLookup_DataLoaded(object sender, DataLoadedEventArgs e)
        {
            HideBanner(this.labelLookupBanner);
            SetStatus(this.labelLookupStatus, $"{e.Count} rows · page {e.Page}/{e.Pages} · {e.Elapsed} ms", StatusKind.Normal);
            this.labelLookupInfo.Text = this.gridLookup.LastCall + "\n" + e.Via;
        }

        private void gridLookup_WidgetError(object sender, GridErrorEventArgs e)
        {
            SetStatus(this.labelLookupStatus, "rejected", StatusKind.Error);
            ShowBanner(this.labelLookupBanner, $"✖ {e.Message}  → click “Reload both”.");
            this.labelLookupInfo.Text = this.gridLookup.LastCall + "\n" + e.Via;
        }

        private void grid_RowClicked(object sender, RowClickedEventArgs e)
        {
            string which = sender == this.gridPostback ? "postback" : "webmethod";
            AlertBox.Show($"{e.Id} selected in the {which} grid.", MessageBoxIcon.Information,
                alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 2500);
        }

        #endregion

        #region UI helpers

        private enum StatusKind { Normal, Warn, Error }

        private void AddTrace(TraceDirection direction, string name, string payload)
        {
            string prefix = direction switch
            {
                TraceDirection.ServerToClient => "→ .NET→JS ",
                TraceDirection.ClientToServer => "← JS→.NET ",
                TraceDirection.Http => "⇄ HTTP    ",
                _ => "• server  ",
            };
            string time = DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
            this.listTrace.Items.Add($"{time}  {prefix} {name,-34} {payload}");
            this.listTrace.SelectedIndex = this.listTrace.Items.Count - 1;
        }

        private void SetStatus(Label label, string text, StatusKind kind)
        {
            label.Text = "● " + text;
            label.ForeColor = kind switch
            {
                StatusKind.Error => System.Drawing.Color.FromArgb(224, 86, 59),
                StatusKind.Warn => System.Drawing.Color.FromArgb(232, 161, 60),
                _ => System.Drawing.Color.FromArgb(31, 157, 87),
            };
        }

        private static void ShowBanner(Label banner, string text)
        {
            banner.Text = text;
            banner.Visible = true;
        }

        private static void HideBanner(Label banner)
        {
            banner.Visible = false;
        }

        private void UpdateTargetButton()
        {
            bool page = this.gridLookup.DataSourceMode == LookupWidget.ModePage;
            this.buttonSwitchTarget.Text = page
                ? "WebMethod target: App.MainPage ▸ switch to widget"
                : "WebMethod target: widget (RegisterWebMethods) ▸ switch to page";
            this.labelLookupSub.Text = page
                ? "LookupWidget · load() awaits App.MainPage.GetWorkOrders(page, size, sort, desc)"
                : "LookupWidget · load() awaits this.GetWorkOrders(…) registered by RegisterWebMethods";
        }

        /// <summary>Keeps the scheme/host and the tail, hides the session/component identifiers.</summary>
        private static string Redact(string url)
        {
            if (string.IsNullOrEmpty(url)) return "(empty)";
            if (url.Length <= 48) return url;
            return url.Substring(0, 22) + "…[redacted]…" + url.Substring(url.Length - 18);
        }

        #endregion
    }
}
