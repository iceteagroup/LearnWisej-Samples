using System;
using TicketOps.Controls;
using TicketOps.Data;
using TicketOps.Infrastructure;
using TicketOps.Resources;
using TicketOps.Services;
using Wisej.Web;

namespace TicketOps.Views
{
    /// <summary>
    /// TicketOps Console · the Module 3 screen: a Page that fills the browser and hosts the responsive
    /// <see cref="TicketWorkspace"/> UserControl.
    ///
    /// Left card:   the workspace (all four regions on desktop, tabs on tablet, one task at a time on phone)
    ///              under a strip that shows the active profile and the browser size live.
    /// Right card:  the Diagnostics activity trace — CLIENT (browser / profile events) → UI → SVC → DATA.
    /// Bottom bar:  "Preview as" (desktop / tablet / phone on demand — the success path), a Timer-paced tour
    ///              of all three (progress path), a too-short search (validation failure), an unknown profile
    ///              (rule failure with fallback), the simulated data outage and its recovery, Clear trace.
    ///
    /// The frame is Dock-based too: the trace card moves from the right (desktop) to the bottom (tablet) and
    /// disappears on phone, so the real browser resize can be watched end to end. Handlers stay thin: read,
    /// call, show. Every "async void" handler owns its try/catch.
    /// </summary>
    public partial class MainPage : Page
    {
        private const int TabletPreviewWidth = 700;
        private const int PhonePreviewWidth = 400;

        private readonly ITicketService _tickets;
        private readonly InMemoryTicketRepository _repository;   // only for the lab's outage switch
        private readonly ClientProfileCatalog _profiles;
        private readonly ILog _log;

        private bool _loaded;
        private int _tourStep;
        private int _lastLoggedWidth;

        // The Designer keeps the parameterless constructor; real wiring goes through the other one.
        public MainPage() : this(null, null, null, new ActivityLog())
        {
        }

        public MainPage(ITicketService tickets, InMemoryTicketRepository repository, ClientProfileCatalog profiles, ILog log)
        {
            InitializeComponent();

            _tickets = tickets;
            _repository = repository;
            _profiles = profiles;
            _log = log ?? new ActivityLog();

            if (log is ActivityLog activityLog)
                this.tracePanel.Attach(activityLog);
            this.tracePanel.Title = "Activity trace · CLIENT → UI → Service → Data";
            this.tracePanel.Footer = "CLIENT = browser size / profile events · UI toggles panels · SVC decides · DATA persists · ⚠ handled · ✖ failure (details stay here, the user sees a safe message)";

            // The Designer-placed workspace receives its dependencies the way the trace panel does.
            if (tickets != null)
                this.workspace.Attach(tickets, _log);
            this.workspace.StatusChanged += this.workspace_StatusChanged;
            this.workspace.ProfileApplied += this.workspace_ProfileApplied;

            this.comboPreview.Items.AddRange(new object[]
            {
                "Live — follow the browser",
                "Preview as desktop",
                "Preview as tablet",
                "Preview as phone"
            });
            this.comboPreview.SelectedIndex = 0;

            // Session-level events: the frame follows the real profile; the strip follows the browser size.
            // Both are unsubscribed in Dispose (MainPage.Designer.cs).
            Application.ResponsiveProfileChanged += this.Application_ResponsiveProfileChanged;
            Application.BrowserSizeChanged += this.Application_BrowserSizeChanged;
        }

        #region Screen lifecycle

        private async void MainPage_Load(object sender, EventArgs e)
        {
            try
            {
                _loaded = true;
                string summary = _profiles != null ? _profiles.Summary : "catalog not loaded";
                _log.Info(LogLayer.UI, "MainPage.Load", $"page shown · profiles from ClientProfiles.json: {summary}");

                ApplyFrameProfile(Application.ActiveProfile.Name);
                this.workspace.FollowBrowser();                    // real profile → workspace layout
                UpdateProfileStrip("Load");

                if (_tickets != null)
                    await this.workspace.LoadAsync();
            }
            catch (Exception ex)
            {
                ReportFailure("MainPage.Load", ex);
            }
        }

        /// <summary>The browser crossed a boundary from ClientProfiles.json: re-arrange the frame (the workspace listens on its own).</summary>
        private void Application_ResponsiveProfileChanged(object sender, ResponsiveProfileChangedEventArgs e)
        {
            try
            {
                string previous = e.PreviousProfile != null ? e.PreviousProfile.Name : "?";
                string current = e.CurrentProfile != null ? e.CurrentProfile.Name : Application.ActiveProfile.Name;
                _log.Info(LogLayer.Client, "MainPage.ResponsiveProfileChanged", $"Application.ResponsiveProfileChanged {previous} → {current}");
                ApplyFrameProfile(current);
                UpdateProfileStrip("ResponsiveProfileChanged");
            }
            catch (Exception ex)
            {
                ReportFailure("MainPage.ResponsiveProfileChanged", ex);
            }
        }

        /// <summary>Fires on every browser resize; the strip follows, the trace logs only meaningful steps (≥ 80 px).</summary>
        private void Application_BrowserSizeChanged(object sender, EventArgs e)
        {
            try
            {
                var size = Application.Browser.Size;
                bool log = Math.Abs(size.Width - _lastLoggedWidth) >= 80;
                if (log)
                    _lastLoggedWidth = size.Width;
                UpdateProfileStrip(log ? "BrowserSizeChanged" : null);
            }
            catch (Exception ex)
            {
                _log.Error(LogLayer.UI, "MainPage.BrowserSizeChanged", ex);
            }
        }

        /// <summary>
        /// The frame's own arrangement per profile — Dock and Visible on panels that already exist:
        /// desktop = trace on the right · tablet = trace below the workspace · phone = trace hidden, taller button bar.
        /// </summary>
        private void ApplyFrameProfile(string profileName)
        {
            switch (profileName)
            {
                case TicketWorkspace.ProfilePhone:
                    this.pnlTraceHost.Visible = false;
                    this.pnlActionsHost.Height = 200;
                    break;

                case TicketWorkspace.ProfileTablet:
                    this.pnlTraceHost.Visible = true;
                    this.pnlTraceHost.Dock = DockStyle.Bottom;
                    this.pnlTraceHost.Height = 220;
                    this.pnlTraceHost.Padding = new Padding(0, 16, 0, 0);
                    this.pnlActionsHost.Height = 108;
                    break;

                default:
                    this.pnlTraceHost.Visible = true;
                    this.pnlTraceHost.Dock = DockStyle.Right;
                    this.pnlTraceHost.Width = 528;
                    this.pnlTraceHost.Padding = new Padding(20, 0, 0, 0);
                    this.pnlActionsHost.Height = 60;
                    break;
            }
            _log.Info(LogLayer.UI, "MainPage.ApplyFrameProfile", $"{profileName} → trace card {(this.pnlTraceHost.Visible ? this.pnlTraceHost.Dock.ToString().ToLowerInvariant() : "hidden")}, button bar {this.pnlActionsHost.Height} px");
        }

        /// <summary>"Active profile: Desktop · browser 1400×760 px · device Desktop · Phone ≤ 600 · Tablet 601–1024 · Desktop ≥ 1025".</summary>
        private void UpdateProfileStrip(string reason)
        {
            var size = Application.Browser.Size;
            string real = Application.ActiveProfile.Name;
            string device = Application.Browser.Device;
            string profiles = _profiles != null ? _profiles.Summary : "";
            string preview = this.workspace.IsPinned ? $" · preview pinned: {this.workspace.AppliedProfile}" : "";

            this.lblProfile.Text = $"Active profile: {real} · browser {size.Width}×{size.Height} px · device {device} · {profiles}{preview}";

            if (reason != null)
                _log.Info(LogLayer.Client, "MainPage.UpdateProfileStrip", $"{reason}: Application.ActiveProfile = {real} · Application.Browser.Size = {size.Width}×{size.Height} · Device = {device}");
        }

        #endregion

        #region Workspace → StatusBanner

        private void workspace_StatusChanged(object sender, WorkspaceStatusEventArgs e)
        {
            this.statusBanner.SetStatus(e.Status, e.Kind);
            if (e.Banner != null)
                this.statusBanner.ShowBanner(e.Banner, e.Kind);
            else
                this.statusBanner.HideBanner();
        }

        private void workspace_ProfileApplied(object sender, string profileName)
        {
            UpdateProfileStrip(null);
        }

        /// <summary>Unexpected failure in the frame itself: details to the log, safe sentence to the user.</summary>
        private void ReportFailure(string source, Exception ex)
        {
            _log.Error(LogLayer.UI, source, ex, $"caught {ex.GetType().Name} — user sees the safe message");
            this.statusBanner.ShowBanner("✖ " + Strings.ActionFailed, StatusKind.Error);
            this.statusBanner.SetStatus("failed", StatusKind.Error);
            AlertBox.Show(Strings.ActionFailed, MessageBoxIcon.Error,
                alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        #endregion

        #region Bottom bar: preview (success), tour (progress), failures, outage + recovery, clear

        /// <summary>Success path: apply a profile's layout on demand, sized like the device, without resizing the browser.</summary>
        private void comboPreview_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_loaded)
                return;

            try
            {
                switch (this.comboPreview.SelectedIndex)
                {
                    case 1: Preview(TicketWorkspace.ProfileDesktop, 0); break;
                    case 2: Preview(TicketWorkspace.ProfileTablet, TabletPreviewWidth); break;
                    case 3: Preview(TicketWorkspace.ProfilePhone, PhonePreviewWidth); break;
                    default: FollowBrowser(); break;
                }
            }
            catch (Exception ex)
            {
                ReportFailure("MainPage.comboPreview_SelectedIndexChanged", ex);
            }
        }

        private void Preview(string profileName, int width)
        {
            string real = Application.ActiveProfile.Name;
            _log.Info(LogLayer.UI, "MainPage.Preview", $"preview {profileName} (browser really {real}) → workspace.PreviewProfile(\"{profileName}\") in a {(width == 0 ? "full-width" : width + " px")} host");

            if (width == 0)
            {
                this.workspace.Dock = DockStyle.Fill;
            }
            else
            {
                this.workspace.Dock = DockStyle.Left;      // a device-like column, the rest of the host stays empty
                this.workspace.Width = width;
            }

            this.workspace.PreviewProfile(profileName);
            this.statusBanner.ShowBanner(string.Format(Strings.PreviewPinned, profileName.ToLowerInvariant(), real), StatusKind.Busy);
            this.statusBanner.SetStatus($"preview: {profileName.ToLowerInvariant()}", StatusKind.Busy);
        }

        private void FollowBrowser()
        {
            _log.Info(LogLayer.UI, "MainPage.FollowBrowser", $"live → workspace.FollowBrowser() applies Application.ActiveProfile = {Application.ActiveProfile.Name}");
            this.workspace.Dock = DockStyle.Fill;
            this.workspace.FollowBrowser();
            this.statusBanner.HideBanner();
            this.statusBanner.SetStatus("live", StatusKind.Success);
        }

        /// <summary>Progress path: a Timer walks desktop → tablet → phone → live, one step per tick.</summary>
        private void buttonTour_Click(object sender, EventArgs e)
        {
            if (this.timerTour.Enabled)
                return;

            _tourStep = 0;
            this.progressTour.Value = 0;
            this.progressTour.Visible = true;
            this.statusBanner.HideBanner();
            this.statusBanner.SetStatus("tour 0/4", StatusKind.Busy);
            _log.Info(LogLayer.UI, "MainPage.buttonTour_Click", "tour: desktop → tablet → phone → live, one step per 1.6 s — the Timer paces it, the comboPreview handler applies each layout");
            this.timerTour.Start();
        }

        private void timerTour_Tick(object sender, EventArgs e)
        {
            try
            {
                // combo index for each step: 1 desktop, 2 tablet, 3 phone, 0 live
                int[] steps = { 1, 2, 3, 0 };
                this.comboPreview.SelectedIndex = steps[_tourStep];
                _tourStep++;
                this.progressTour.Value = _tourStep;
                this.statusBanner.SetStatus($"tour {_tourStep}/4", StatusKind.Busy);

                if (_tourStep >= steps.Length)
                {
                    this.timerTour.Stop();
                    this.progressTour.Visible = false;
                    this.statusBanner.SetStatus("live", StatusKind.Success);
                    _log.Info(LogLayer.UI, "MainPage.timerTour_Tick", "tour complete — back to the browser's own profile");
                }
            }
            catch (Exception ex)
            {
                this.timerTour.Stop();
                this.progressTour.Visible = false;
                ReportFailure("MainPage.timerTour_Tick", ex);
            }
        }

        /// <summary>Failure path 1 (validation): a one-character query — the service rejects it, nothing is loaded.</summary>
        private async void buttonSearchShort_Click(object sender, EventArgs e)
        {
            _log.Info(LogLayer.UI, "MainPage.buttonSearchShort_Click", "→ workspace.SearchTicketsAsync(\"x\") — expect ⚠ in SVC, orange banner, no DATA line");
            await this.workspace.SearchTicketsAsync("x");
        }

        /// <summary>Failure path 2 (rule): a profile name ClientProfiles.json does not define — the fallback is visible and explained.</summary>
        private void buttonUnknownProfile_Click(object sender, EventArgs e)
        {
            try
            {
                const string unknown = "Kiosk";
                bool defined = _profiles != null && _profiles.Contains(unknown);
                _log.Info(LogLayer.Infrastructure, "ClientProfileCatalog.Contains", $"\"{unknown}\" defined in ClientProfiles.json? {(defined ? "yes" : "no")} (defined: {(_profiles != null ? _profiles.Summary : "-")})");
                _log.Info(LogLayer.UI, "MainPage.buttonUnknownProfile_Click", $"→ workspace.ApplyResponsiveProfile(\"{unknown}\") — expect ⚠ fallback to the desktop layout");
                this.workspace.ApplyResponsiveProfile(unknown);
            }
            catch (Exception ex)
            {
                ReportFailure("MainPage.buttonUnknownProfile_Click", ex);
            }
        }

        /// <summary>Error path + recovery: toggle the repository outage, then refresh the workspace through the service.</summary>
        private async void buttonOutage_Click(object sender, EventArgs e)
        {
            if (_repository == null)
                return;

            _repository.SimulateOutage = !_repository.SimulateOutage;
            this.buttonOutage.Text = _repository.SimulateOutage ? "Recover the data store" : "Simulate data outage";
            _log.Info(LogLayer.UI, "MainPage.buttonOutage_Click",
                _repository.SimulateOutage ? "outage ON → refresh (expect ✖ in DATA, safe message in UI)" : "outage OFF → refresh (recovery)");
            await this.workspace.RefreshAsync(_repository.SimulateOutage ? "outage ON" : "recovery");
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            this.tracePanel.ClearTrace();
            this.statusBanner.HideBanner();
            this.statusBanner.SetStatus("ready", StatusKind.Normal);
        }

        #endregion
    }
}
