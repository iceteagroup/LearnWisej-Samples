using System;
using System.Collections.Generic;
using EnterpriseOps.Domain;
using EnterpriseOps.Security;
using EnterpriseOps.Services;
using Wisej.Web;

namespace EnterpriseOps.UI
{
    /// <summary>
    /// TicketOps — Work Orders, the migrated screen running on Wisej.NET 4: three tab buttons
    /// (Open · In progress · Done), <c>+ New work order</c>, <c>dgvWorkOrders</c>, <c>lblStatus</c> and the dark footer.
    ///
    /// It paints itself from <see cref="ThemeService.Current"/> — the accent colour of the app bar and the active
    /// tab, the corner radius of the buttons and the per-priority colours of the grid all come from the theme map.
    /// While the theme is unmapped the screen shows the visual diff: grey accent, square corners, no priority
    /// colours. Once the mixin is mapped it matches the 3.5 baseline again.
    ///
    /// Behaviour is unchanged from the Intermediate console: the tab filter runs on the server and a new work
    /// order goes through the same validation.
    /// </summary>
    public partial class WorkOrdersPage : Page
    {
        private readonly ServiceRegistry _services;
        private readonly SessionContext _session;
        private readonly ActivityTrace _trace;

        private QueueBucket _bucket = QueueBucket.Open;
        private List<WorkQueueRow> _rows = new List<WorkQueueRow>();
        private CommandContext _current;

        /// <summary>Designer / default constructor: a standalone session so the screen opens in the Wisej.NET Designer.</summary>
        public WorkOrdersPage()
            : this(NewStandaloneRegistry())
        {
        }

        public WorkOrdersPage(ServiceRegistry services)
        {
            InitializeComponent();

            _services = services;
            _session = services.Session;
            _trace = services.Trace;
        }

        private CommandContext CurrentContext => _current ?? NewCommand();

        #region Event handlers

        private void WorkOrdersPage_Load(object sender, EventArgs e)
        {
            ApplyTheme();
            LoadBucket(QueueBucket.Open);
        }

        private void btnOpen_Click(object sender, EventArgs e) => LoadBucket(QueueBucket.Open);

        private void btnInProgress_Click(object sender, EventArgs e) => LoadBucket(QueueBucket.InProgress);

        private void btnDone_Click(object sender, EventArgs e) => LoadBucket(QueueBucket.Done);

        /// <summary>A new work order: the service validates, writes and reports; the screen reloads.</summary>
        private void btnNewWorkOrder_Click(object sender, EventArgs e)
        {
            NewCommand();
            try
            {
                var result = _services.WorkOrders.Save(
                    new SaveWorkOrderCommand { Title = "Replace air filter — bay 4", Customer = "Contoso Plant", Site = "Bay 4", Priority = Priority.Normal },
                    CurrentContext);
                ShowCommandResult(result);
                LoadBucket(QueueBucket.Open);
            }
            catch (Exception ex)
            {
                ReportFailure(ex);
            }
        }

        private void btnBackToDossier_Click(object sender, EventArgs e)
        {
            Application.MainPage = new MigrationDossierPage(_services);
        }

        #endregion

        #region Work — the screen asks, the service decides

        /// <summary>The tab filter: one query, decided on the server, exactly as on Wisej.NET 3.5.</summary>
        private void LoadBucket(QueueBucket bucket)
        {
            try
            {
                _bucket = bucket;

                var page = _services.WorkOrders.Query(new WorkQueueQuery { Bucket = bucket }, CurrentContext);
                _rows = page.Rows;
                dgvWorkOrders.DataSource = new BindingSource { DataSource = _rows };

                ApplyTheme();
                lblStatus.Text = $"{page.Total} work orders loaded";
                ShowFooter(page.Total);
            }
            catch (Exception ex)
            {
                ReportFailure(ex);
            }
        }

        #endregion

        #region The theme map on the screen

        /// <summary>
        /// Repaints the screen from <see cref="ThemeService.Current"/>: accent colour, corner radius and the
        /// per-priority grid colours. The token values come from the theme map the migration left behind, and
        /// the banner shows how far it is from the 3.5 baseline.
        /// </summary>
        private void ApplyTheme()
        {
            var theme = _services.Theme.Current;
            var accent = HexToColor(theme.AccentColor, System.Drawing.Color.FromArgb(107, 124, 143));
            string radius = $"border-radius: {theme.CornerRadius}px !important;";

            pnlHeader.BackColor = accent;

            PaintTab(btnOpen, _bucket == QueueBucket.Open, accent, radius);
            PaintTab(btnInProgress, _bucket == QueueBucket.InProgress, accent, radius);
            PaintTab(btnDone, _bucket == QueueBucket.Done, accent, radius);

            btnNewWorkOrder.BackColor = accent;
            btnNewWorkOrder.ForeColor = System.Drawing.Color.White;
            btnNewWorkOrder.CssStyle = radius;

            PaintPriorityColumn(theme);

            var diff = _services.Theme.Diff();
            if (diff.Count == 0)
                HideBanner();
            else
                ShowBanner($"Visual diff — {diff.Count} failure(s): {string.Join(" · ", diff)}", BannerKind.Error);
        }

        private static void PaintTab(Button tab, bool active, System.Drawing.Color accent, string radius)
        {
            tab.BackColor = active ? accent : System.Drawing.Color.White;
            tab.ForeColor = active ? System.Drawing.Color.White : System.Drawing.Color.FromArgb(70, 88, 106);
            tab.CssStyle = radius;
        }

        /// <summary>High red, Normal amber, Low green — when the theme map has them. Otherwise plain grey.</summary>
        private void PaintPriorityColumn(ThemeMap theme)
        {
            var fallback = System.Drawing.Color.FromArgb(70, 88, 106);
            for (int i = 0; i < dgvWorkOrders.Rows.Count && i < _rows.Count; i++)
            {
                var cell = dgvWorkOrders.Rows[i].Cells[colWoPriority];
                if (cell == null)
                    continue;
                var color = theme.PriorityColors.TryGetValue(_rows[i].Priority, out string hex)
                    ? HexToColor(hex, fallback)
                    : fallback;
                var style = cell.Style ?? new DataGridViewCellStyle();
                style.ForeColor = color;
                cell.Style = style;
            }
        }

        /// <summary>"#1565D8" → Color. The domain keeps colours as hex so it never depends on System.Drawing.</summary>
        private static System.Drawing.Color HexToColor(string hex, System.Drawing.Color fallback)
        {
            if (string.IsNullOrWhiteSpace(hex) || hex.Length != 7 || hex[0] != '#')
                return fallback;
            try
            {
                return System.Drawing.Color.FromArgb(
                    Convert.ToInt32(hex.Substring(1, 2), 16),
                    Convert.ToInt32(hex.Substring(3, 2), 16),
                    Convert.ToInt32(hex.Substring(5, 2), 16));
            }
            catch (FormatException)
            {
                return fallback;
            }
        }

        #endregion

        #region Showing results — UI state only

        private void ShowCommandResult(CommandResult result)
        {
            if (result.Succeeded)
            {
                ShowBanner(result.Message, BannerKind.Success);
                AlertBox.Show(result.Message, MessageBoxIcon.Information, alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
                return;
            }

            ShowBanner(result.ErrorText, BannerKind.Error);
            AlertBox.Show(result.ErrorText, MessageBoxIcon.Warning, alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        /// <summary>The dark footer — and, when the theme is still unmapped, what is wrong with it.</summary>
        private void ShowFooter(int total)
        {
            int diff = _services.Theme.Diff().Count;
            if (diff > 0)
                lblStatusBar.Text = $"{total} {BucketText(_bucket).ToLowerInvariant()} work orders — Wisej.NET 4, theme NOT mapped: {diff} visual difference(s)";
            else if (_bucket == QueueBucket.Open)
                lblStatusBar.Text = $"{total} open work orders — Wisej.NET 4, mapped theme mixin";
            else
                lblStatusBar.Text = $"{total} {BucketText(_bucket).ToLowerInvariant()} — filter ran on the server, same as 3.5";
        }

        private void ReportFailure(Exception ex)
        {
            _trace.Service($"unhandled {ex.GetType().Name} (ref {CurrentContext.CorrelationId}) — {ex.Message}");
            ShowBanner($"The action could not be completed. Check the log for details.  (ref {CurrentContext.CorrelationId})", BannerKind.Error);
            AlertBox.Show("The action could not be completed. Check the log for details.",
                MessageBoxIcon.Error, alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        #endregion

        #region Helpers

        private enum BannerKind { Success, Error }

        private static ServiceRegistry NewStandaloneRegistry()
        {
            var session = new SessionContext("contoso", "ana.ops", Role.Manager);
            return new ServiceRegistry(session, () => session);
        }

        private static string BucketText(QueueBucket bucket)
        {
            return bucket == QueueBucket.InProgress ? "In progress" : bucket.ToString();
        }

        private CommandContext NewCommand()
        {
            _current = CommandContext.From(_session);
            return _current;
        }

        private void ShowBanner(string text, BannerKind kind)
        {
            lblBanner.Text = text;
            if (kind == BannerKind.Success)
            {
                lblBanner.BackColor = System.Drawing.Color.FromArgb(233, 247, 238);
                lblBanner.ForeColor = System.Drawing.Color.FromArgb(15, 122, 58);
            }
            else
            {
                lblBanner.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
                lblBanner.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            }
            lblBanner.Visible = true;
        }

        private void HideBanner()
        {
            lblBanner.Visible = false;
        }

        #endregion
    }
}
