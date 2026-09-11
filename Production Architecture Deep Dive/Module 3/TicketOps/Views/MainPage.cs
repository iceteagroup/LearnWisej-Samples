using System;
using TicketOps.Controls;
using TicketOps.Infrastructure;
using TicketOps.Resources;
using TicketOps.Services;
using Wisej.Web;

namespace TicketOps.Views
{
    /// <summary>
    /// TicketOps Console · a Page that fills the browser and hosts the responsive
    /// <see cref="TicketWorkspace"/> UserControl, with the StatusBanner above it.
    /// </summary>
    public partial class MainPage : Page
    {
        private readonly ITicketService _tickets;
        private readonly ILog _log;

        // The Designer keeps the parameterless constructor; real wiring goes through the other one.
        public MainPage() : this(null, new ActivityLog())
        {
        }

        public MainPage(ITicketService tickets, ILog log)
        {
            InitializeComponent();

            _tickets = tickets;
            _log = log ?? new ActivityLog();

            // The Designer-placed workspace receives its dependencies through Attach.
            if (tickets != null)
                this.workspace.Attach(tickets, _log);
            this.workspace.StatusChanged += this.workspace_StatusChanged;
        }

        private async void MainPage_Load(object sender, EventArgs e)
        {
            try
            {
                this.workspace.ApplyActiveProfile();
                if (_tickets != null)
                    await this.workspace.LoadAsync();
            }
            catch (Exception ex)
            {
                ReportFailure("MainPage.Load", ex);
            }
        }

        private void workspace_StatusChanged(object sender, WorkspaceStatusEventArgs e)
        {
            this.statusBanner.SetStatus(e.Status, e.Kind);
            if (e.Banner != null)
                this.statusBanner.ShowBanner(e.Banner, e.Kind);
            else
                this.statusBanner.HideBanner();
        }

        /// <summary>Unexpected failure: the details go to the log, the user sees one safe sentence.</summary>
        private void ReportFailure(string source, Exception ex)
        {
            _log.Error(LogLayer.UI, source, ex);
            this.statusBanner.ShowBanner("✖ " + Strings.ActionFailed, StatusKind.Error);
            this.statusBanner.SetStatus("failed", StatusKind.Error);
            AlertBox.Show(Strings.ActionFailed, MessageBoxIcon.Error,
                alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }
    }
}
