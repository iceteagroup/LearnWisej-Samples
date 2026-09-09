using System;
using TicketOps.Controls;
using TicketOps.Infrastructure;
using TicketOps.Resources;
using Wisej.Web;

namespace TicketOps.Views
{
    /// <summary>
    /// Template main screen — Module N replaces the left card with the module's screen.
    ///
    /// Left card:   the screen under study (display + input only).
    /// Right card:  the Diagnostics activity trace (UI → Service → Data), attached to the session log.
    /// Bottom bar:  buttons that exercise the success path, a progress path, at least one failure
    ///              path and the recovery, plus "Clear trace".
    ///
    /// Handlers stay thin: read the screen, call a service, show the result. Anything that awaits is an
    /// "async void" event handler, so it owns its try/catch and never lets an exception escape.
    /// </summary>
    public partial class MainView : Form
    {
        private readonly ILog _log;

        // The Designer keeps the parameterless constructor; the real wiring uses the other one.
        public MainView() : this(new ActivityLog())
        {
        }

        public MainView(ActivityLog log)
        {
            InitializeComponent();

            _log = log;
            this.tracePanel.Attach(log);
        }

        private void MainView_Load(object sender, EventArgs e)
        {
            _log.Info(LogLayer.UI, "MainView.Load", "screen shown — handlers read the form, call a service, show the result");
            this.statusBanner.SetStatus("idle", StatusKind.Normal);
        }

        private void buttonPing_Click(object sender, EventArgs e)
        {
            try
            {
                _log.Info(LogLayer.UI, "MainView.buttonPing_Click", "click → (a service would be called here)");
                this.statusBanner.SetStatus("pinged", StatusKind.Success);
                this.statusBanner.HideBanner();
            }
            catch (Exception ex)
            {
                _log.Error(LogLayer.UI, "MainView.buttonPing_Click", ex);
                this.statusBanner.ShowBanner(Strings.ActionFailed, StatusKind.Error);
            }
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            this.tracePanel.ClearTrace();
            this.statusBanner.HideBanner();
            this.statusBanner.SetStatus("idle", StatusKind.Normal);
        }
    }
}
