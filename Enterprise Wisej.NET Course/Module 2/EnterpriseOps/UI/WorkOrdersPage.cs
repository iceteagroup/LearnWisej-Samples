using System;
using System.Collections.Generic;
using System.Linq;
using EnterpriseOps.Domain;
using EnterpriseOps.Security;
using EnterpriseOps.Services;
using Wisej.Web;

namespace EnterpriseOps.UI
{
    /// <summary>
    /// TicketOps — Work Orders, the migrated screen the walkthrough opens in the Designer and then runs on
    /// Wisej.NET 4: three tab buttons (Open · In progress · Done), <c>+ New work order</c>, <c>dgvWorkOrders</c>
    /// and the dark footer.
    ///
    /// What makes it a migration screen: it paints itself from <see cref="ThemeService.Current"/> — the accent
    /// colour of the app bar and the active tab, the corner radius of the buttons and the per-priority colours of
    /// the grid all come from the theme map. Open it while the theme is unmapped and the visual diff is on the
    /// screen, not in a document: grey accent, square corners, no priority colours. Map the mixin on the dossier
    /// page, come back, and the screen matches the 3.5 baseline again.
    ///
    /// Behaviour is deliberately unchanged from the Intermediate console: the tab filter runs on the server, a
    /// blank title is rejected, a stale Version is refused, and only a Manager or an Admin may approve. Those are
    /// flows 2–5 of the regression harness, exercised here by hand.
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

        #region Event handlers — thin, one service call each

        private void WorkOrdersPage_Load(object sender, EventArgs e)
        {
            _trace.Attach(AppendTrace);
            _trace.Ui($"WorkOrdersPage_Load → session {_session} (same instance as on the dossier page), theme \"{_services.Theme.Current.Name}\"");

            lblTenant.Text = "tenant: " + _session.TenantId;
            lblUser.Text = $"Signed in: {_session.UserName} · {_session.Role}";

            ApplyTheme();
            LoadBucket(QueueBucket.Open);
        }

        private void btnOpen_Click(object sender, EventArgs e) => LoadBucket(QueueBucket.Open);

        private void btnInProgress_Click(object sender, EventArgs e) => LoadBucket(QueueBucket.InProgress);

        private void btnDone_Click(object sender, EventArgs e) => LoadBucket(QueueBucket.Done);

        /// <summary>Success path: a valid work order. The service validates, writes and reports; the screen reloads.</summary>
        private void btnNewWorkOrder_Click(object sender, EventArgs e)
        {
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

        /// <summary>Failure path (flow 4): a blank title. Rejected with a message; the store is untouched.</summary>
        private void btnNewBlank_Click(object sender, EventArgs e)
        {
            try
            {
                int before = _services.WorkOrders.StoreCount;
                var result = _services.WorkOrders.Save(new SaveWorkOrderCommand { Title = "   " }, CurrentContext);
                _trace.Ui($"btnNewBlank_Click → store {before} rows before, {_services.WorkOrders.StoreCount} after — unchanged");
                ShowCommandResult(result);
            }
            catch (Exception ex)
            {
                ReportFailure(ex);
            }
        }

        /// <summary>Success path: approve the selected work order with its current Version.</summary>
        private void btnApprove_Click(object sender, EventArgs e) => Approve(stale: false);

        /// <summary>Failure path (flow 5): approve with Version - 1. Optimistic concurrency refuses it.</summary>
        private void btnApproveStale_Click(object sender, EventArgs e) => Approve(stale: true);

        private void btnBackToDossier_Click(object sender, EventArgs e)
        {
            _trace.Ui("btnBackToDossier_Click → Application.MainPage = MigrationDossierPage (same ServiceRegistry, same SessionContext)");
            _trace.Detach();
            Application.MainPage = new MigrationDossierPage(_services);
        }

        private void btnClearTrace_Click(object sender, EventArgs e)
        {
            _trace.Clear();
            lstTrace.Items.Clear();
        }

        #endregion

        #region Work — the screen asks, the service decides

        /// <summary>The tab filter: one query, decided on the server, exactly as on Wisej.NET 3.5.</summary>
        private void LoadBucket(QueueBucket bucket)
        {
            try
            {
                _bucket = bucket;
                _trace.Ui($"tab \"{BucketText(bucket)}\" → WorkOrderService.Query(bucket={bucket})");

                var page = _services.WorkOrders.Query(new WorkQueueQuery { Bucket = bucket }, CurrentContext);
                _rows = page.Rows;
                dgvWorkOrders.DataSource = new BindingSource { DataSource = _rows };

                ApplyTheme();
                HideBanner();
                SetStatus($"{page.Total} {BucketText(bucket).ToLowerInvariant()} work order(s) for {_session.TenantId}", StatusKind.Ok);
                ShowFooter(page.Total);
            }
            catch (Exception ex)
            {
                ReportFailure(ex);
            }
        }

        /// <summary>Approve the selected row, optionally with a deliberately stale Version.</summary>
        private void Approve(bool stale)
        {
            try
            {
                var permission = _services.Permissions.Check(CurrentContext, Permission.ApproveWorkOrder);
                if (!permission.Allowed)
                {
                    ShowBanner(permission.Reason, BannerKind.Warning);
                    SetStatus("not permitted", StatusKind.Warn);
                    return;
                }

                int id = SelectedWorkOrderId();
                if (id <= 0)
                {
                    ShowBanner("Select a work order in the grid first.", BannerKind.Warning);
                    SetStatus("nothing selected", StatusKind.Warn);
                    return;
                }

                int version = _services.WorkOrders.CurrentVersion(id);
                var result = _services.WorkOrders.Approve(
                    new ApproveWorkOrderCommand { WorkOrderId = id, Version = stale ? version - 1 : version },
                    CurrentContext);

                ShowCommandResult(result);
                if (result.Succeeded)
                    LoadBucket(_bucket);
            }
            catch (Exception ex)
            {
                ReportFailure(ex);
            }
        }

        private int SelectedWorkOrderId()
        {
            var row = dgvWorkOrders.CurrentRow;
            object value = row?.Cells[colWoId]?.Value;
            return value == null ? 0 : Convert.ToInt32(value);
        }

        #endregion

        #region The theme map on the screen — this is the visual diff, live

        /// <summary>
        /// Repaints the screen from <see cref="ThemeService.Current"/>: accent colour, corner radius and the
        /// per-priority grid colours. Nothing here decides anything — the token values come from the theme map
        /// the migration left behind, and the banner shows how far it is from the 3.5 baseline.
        /// </summary>
        private void ApplyTheme()
        {
            var theme = _services.Theme.Current;
            var accent = HexToColor(theme.AccentColor, System.Drawing.Color.FromArgb(107, 124, 143));
            string radius = $"border-radius: {theme.CornerRadius}px !important;";

            pnlHeader.BackColor = accent;
            lblTitle.Text = theme.IsMapped ? "TicketOps — Work Orders" : "TicketOps — Work Orders  (theme not mapped)";

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

        /// <summary>High red, Normal amber, Low green — when the theme map still has them. Otherwise plain grey, as on the broken build.</summary>
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
            lblCorrelation.Text = "corr " + result.CorrelationId;

            if (result.Succeeded)
            {
                ShowBanner(result.Message, BannerKind.Success);
                SetStatus(result.Message, StatusKind.Ok);
                AlertBox.Show(result.Message, MessageBoxIcon.Information, alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
                return;
            }

            ShowBanner(result.ErrorText, BannerKind.Error);
            SetStatus("rejected — see the banner", StatusKind.Error);
            AlertBox.Show(result.ErrorText, MessageBoxIcon.Warning, alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        /// <summary>The video's footer line — and, when the theme is still unmapped, what is wrong with it.</summary>
        private void ShowFooter(int total)
        {
            var theme = _services.Theme.Current;
            int diff = _services.Theme.Diff().Count;
            lblStatusBar.Text = diff == 0
                ? $"{total} {BucketText(_bucket).ToLowerInvariant()} work orders — Wisej.NET 4, {theme.Name}"
                : $"{total} {BucketText(_bucket).ToLowerInvariant()} work orders — Wisej.NET 4, theme NOT mapped: {diff} visual difference(s)";
        }

        private void ReportFailure(Exception ex)
        {
            _trace.Service($"unhandled {ex.GetType().Name} (ref {CurrentContext.CorrelationId}) — {ex.Message}");
            ShowBanner($"The action could not be completed. Check the log for details.  (ref {CurrentContext.CorrelationId})", BannerKind.Error);
            SetStatus("failed — see the log", StatusKind.Error);
            AlertBox.Show("The action could not be completed. Check the log for details.",
                MessageBoxIcon.Error, alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        #endregion

        #region Helpers

        private enum StatusKind { Ok, Warn, Error }
        private enum BannerKind { Success, Warning, Error }

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
            lblCorrelation.Text = "corr " + _current.CorrelationId;
            return _current;
        }

        private void SetStatus(string text, StatusKind kind)
        {
            lblStatus.Text = "● " + text;
            lblStatus.ForeColor = kind switch
            {
                StatusKind.Error => System.Drawing.Color.FromArgb(224, 86, 59),
                StatusKind.Warn => System.Drawing.Color.FromArgb(232, 161, 60),
                _ => System.Drawing.Color.FromArgb(31, 157, 87),
            };
        }

        private void ShowBanner(string text, BannerKind kind)
        {
            lblBanner.Text = text;
            switch (kind)
            {
                case BannerKind.Success:
                    lblBanner.BackColor = System.Drawing.Color.FromArgb(233, 247, 238);
                    lblBanner.ForeColor = System.Drawing.Color.FromArgb(15, 122, 58);
                    break;
                case BannerKind.Warning:
                    lblBanner.BackColor = System.Drawing.Color.FromArgb(255, 244, 229);
                    lblBanner.ForeColor = System.Drawing.Color.FromArgb(146, 64, 14);
                    break;
                default:
                    lblBanner.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
                    lblBanner.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
                    break;
            }
            lblBanner.Visible = true;
        }

        private void HideBanner()
        {
            lblBanner.Visible = false;
        }

        private void AppendTrace(string line)
        {
            lstTrace.Items.Add(line);
            lstTrace.SelectedIndex = lstTrace.Items.Count - 1;
        }

        #endregion
    }
}
