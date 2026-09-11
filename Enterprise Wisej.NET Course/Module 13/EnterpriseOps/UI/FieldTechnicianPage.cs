using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using EnterpriseOps.Data;
using EnterpriseOps.Domain;
using EnterpriseOps.Hybrid;
using EnterpriseOps.Security;
using EnterpriseOps.Services;
using Wisej.Web;

namespace EnterpriseOps.UI
{
    /// <summary>
    /// Field Technician mode.
    ///
    /// The device frame holds the cached work orders, the field actions, the completion queue, the sync
    /// conflict panel and the device status strip. Its width follows the detected device (phone, tablet or
    /// desktop) — that is the device-aware layout.
    ///
    /// The page owns no rules. It reads the selection, builds a typed command and hands it to a service:
    /// online to <see cref="WorkOrderService"/> directly, offline to <see cref="IOfflineCommandQueue"/>.
    /// It asks the device about itself only through <see cref="IDeviceServices"/>.
    /// </summary>
    public partial class FieldTechnicianPage : Page
    {
        private const string DispatcherUser = "ana.ops";
        private const int DispatcherCancelledWorkOrderId = 1037;

        private readonly IActivityTrace _trace;
        private readonly WorkOrderService _service;

        private readonly LocalStore _localStore;
        private readonly BrowserDeviceServices _shell;   // the implementation: detection + connectivity
        private readonly IDeviceServices _device;        // what the screen is allowed to call
        private readonly LocalOfflineCommandQueue _queue;
        private readonly SyncWorkflow _sync;

        private readonly SessionContext _session;
        private readonly BindingSource _cacheBinding = new BindingSource();

        private List<CachedWorkOrder> _cacheRows = new List<CachedWorkOrder>();
        private DeviceInfo _deviceInfo;
        private SyncConflict _conflict;
        private bool _syncing;
        private bool _dispatcherActed;

        public FieldTechnicianPage()
        {
            InitializeComponent();

            _session = new SessionContext
            {
                TenantId = "contoso",
                TenantName = "Contoso Utilities",
                User = "ben.tech",
                Role = "Technician",
            };

            _trace = new ActivityTrace();
            var permissions = new PermissionService();
            _service = new WorkOrderService(new FakeWorkOrderRepository(), permissions, new AuditLog(), _trace);

            _localStore = new LocalStore(_trace);
            _shell = new BrowserDeviceServices(_trace);
            _device = _shell;
            _queue = new LocalOfflineCommandQueue(_localStore, _trace);
            _sync = new SyncWorkflow(_queue, _localStore, _service, permissions, _session, _trace);

            this.dgvCache.DataSource = _cacheBinding;
            this.conflictPanel.KeepServerRequested += conflictPanel_KeepServerRequested;
            this.conflictPanel.ApplyMineRequested += conflictPanel_ApplyMineRequested;
        }

        private async void FieldTechnicianPage_Load(object sender, EventArgs e)
        {
            try
            {
                _deviceInfo = _shell.Describe();
                ApplyDeviceLayout();

                await ProvisionDeviceAsync();

                UpdateConnectionChip();
                ShowStatus($"Online — cache: {_localStore.Cache.Count} work orders (SQLite) · queue empty");
            }
            catch (Exception ex)
            {
                Fail("load", ex);
            }
            finally
            {
                // The awaits end after the request returned: push the final UI state to the browser.
                Application.Update(this);
            }
        }

        // ── Field actions ───────────────────────────────────────────────────────────────────────────

        private async void btnComplete_Click(object sender, EventArgs e)
        {
            try
            {
                var selected = SelectedCached();
                if (selected == null)
                {
                    Notify("Pick a work order first.", MessageBoxIcon.Warning);
                    return;
                }

                string notes = (this.txtNotes.Text ?? "").Trim();
                if (notes.Length == 0)
                {
                    ShowStatus("Completion notes are required.");
                    Notify("Enter what you did before completing the work order.", MessageBoxIcon.Warning);
                    return;
                }

                var command = new CompleteWorkOrderCommand
                {
                    WorkOrderId = selected.Id,
                    ExpectedVersion = selected.Version,
                    Notes = notes,
                    CompletedAt = DateTimeOffset.Now,
                };

                if (await _device.IsOnlineAsync())
                {
                    CommandResult result = await _service.CompleteAsync(command, _session.NewCommand());
                    if (result.Succeeded)
                    {
                        selected.Status = WorkOrderStatus.Completed.ToString();
                        selected.Version = result.NewVersion;
                        selected.LocalState = "";
                        ShowStatus(result.Message);
                        Notify(result.Message, MessageBoxIcon.Information);
                    }
                    else
                    {
                        ShowStatus(result.Message);
                        Notify($"{result.Message} (ref {result.CorrelationId})", MessageBoxIcon.Error);
                    }
                }
                else
                {
                    // Offline: the record is not updated. A command is appended with an explicit SyncState.
                    var offline = _queue.CreateCompletion(command, selected.Code,
                        $"Completed {command.CompletedAt.ToLocalTime():HH:mm} — {notes}");
                    await _queue.EnqueueAsync(offline);
                    await _device.VibrateAsync();

                    selected.LocalState = "Completed · pending sync";
                    ShowOfflineStatus();
                    Notify($"No signal — {selected.Code} is queued locally and will sync when you reconnect. It is not lost and it is not applied.",
                        MessageBoxIcon.Warning);
                }

                RefreshCacheGrid();
                RenderQueue();
            }
            catch (Exception ex)
            {
                Fail("complete", ex);
            }
            finally
            {
                // The awaits end after the request returned: push the final UI state to the browser.
                Application.Update(this);
            }
        }

        /// <summary>
        /// The scanner works offline, but its output is untrusted input: online it is validated by the
        /// server immediately, offline it travels inside the queued command and is validated at sync time.
        /// </summary>
        private async void btnScan_Click(object sender, EventArgs e)
        {
            try
            {
                var selected = SelectedCached();
                if (selected == null)
                {
                    Notify("Pick a work order first.", MessageBoxIcon.Warning);
                    return;
                }

                string scanned = await _device.ScanDocumentAsync();

                if (await _device.IsOnlineAsync())
                {
                    CommandResult result = await _service.ValidateScannedAssetAsync(selected.Id, scanned, _session.NewCommand());
                    this.lblScanResult.Text = (result.Succeeded ? "✓ " : "✖ ") + result.Message;
                    this.lblScanResult.ForeColor = result.Succeeded ? OkColor : ErrorColor;
                }
                else
                {
                    this.lblScanResult.Text = $"{scanned} — recorded offline, validated at sync";
                    this.lblScanResult.ForeColor = WarnColor;
                }

                this.txtNotes.Text = $"{(this.txtNotes.Text ?? "").Trim()} (asset {scanned})".Trim();
            }
            catch (Exception ex)
            {
                Fail("scan", ex);
            }
            finally
            {
                // The awaits end after the request returned: push the final UI state to the browser.
                Application.Update(this);
            }
        }

        // ── Connectivity and the reconnect sync ─────────────────────────────────────────────────────

        private async void btnToggleConnection_Click(object sender, EventArgs e)
        {
            try
            {
                bool goingOnline = !_shell.IsOnline;
                _shell.SetOnline(goingOnline);
                UpdateConnectionChip();

                if (!goingOnline)
                {
                    await DispatcherCancelsWhileOfflineAsync();
                    ShowOfflineStatus();
                    return;
                }

                if (_queue.PendingCount == 0)
                {
                    _sync.RefreshPermissions();
                    ShowStatus("Connection restored — permissions refreshed · queue empty");
                    return;
                }

                StartSync();
            }
            catch (Exception ex)
            {
                Fail("toggle connection", ex);
            }
            finally
            {
                // The awaits end after the request returned: push the final UI state to the browser.
                Application.Update(this);
            }
        }

        /// <summary>
        /// The office keeps working while the device is away: the dispatcher cancels WO-1037 on the server.
        /// The device cannot know until it reconnects and the replay finds that the version moved on.
        /// </summary>
        private async Task DispatcherCancelsWhileOfflineAsync()
        {
            if (_dispatcherActed)
                return;

            _dispatcherActed = true;

            var cached = _localStore.Find(DispatcherCancelledWorkOrderId);
            if (cached == null
                || cached.Status == WorkOrderStatus.Completed.ToString()
                || cached.Status == WorkOrderStatus.Cancelled.ToString())
                return;

            var dispatcher = new CommandContext(_session.TenantId, DispatcherUser, Guid.NewGuid().ToString("N").Substring(0, 8));
            await _service.CancelAsync(DispatcherCancelledWorkOrderId, "site rescheduled to Thursday crew", dispatcher);
        }

        /// <summary>
        /// The replay runs on a background task; every step pushes its own UI update with
        /// <c>Application.Update(this, …)</c>. The workflow paces itself (700 ms per command).
        /// </summary>
        private void StartSync()
        {
            if (_syncing)
                return;

            _syncing = true;
            SetBusy(true);
            this.prgSync.Maximum = Math.Max(1, _queue.PendingCount);
            this.prgSync.Value = 0;
            this.lblConnection.Text = "ONLINE — SYNCING";
            ShowStatus("Connection restored — refreshing permissions…");

            Application.StartTask(() =>
            {
                try
                {
                    SyncReport report = _sync.Replay(p => Push(() => OnSyncProgress(p)));
                    Push(() => OnSyncFinished(report));
                }
                catch (ObjectDisposedException)
                {
                    // The session went away while the replay was running: nothing to update.
                }
                catch (Exception ex)
                {
                    Push(() => OnSyncFailed(ex));
                }
            });
        }

        /// <summary>Pushes a UI change from the background task, guarding against a disposed session.</summary>
        private void Push(Action change)
        {
            if (this.IsDisposed)
                return;

            try
            {
                Application.Update(this, change);
            }
            catch (ObjectDisposedException)
            {
            }
        }

        private void OnSyncProgress(SyncProgress p)
        {
            this.prgSync.Value = Math.Min(p.Index, this.prgSync.Maximum);
            ShowStatus(p.Message);
            RefreshCacheGrid();
            RenderQueue();

            if (p.Conflict != null)
                ShowConflict(p.Conflict);
        }

        private void OnSyncFinished(SyncReport report)
        {
            _syncing = false;
            SetBusy(false);
            UpdateConnectionChip();

            if (report.StoppedOnConflict)
            {
                ShowStatus($"Sync conflict on {report.Conflict.Code} — server changed while offline · waiting for the technician");
                Notify(report.Message, MessageBoxIcon.Warning);
                return;
            }

            this.prgSync.Value = this.prgSync.Maximum;
            ShowStatus(report.Message);
            Notify(report.Rejected > 0
                    ? $"{report.Rejected} queued command(s) were rejected by the server. {report.Synced} synced."
                    : $"{report.Synced} completion(s) synced. The queue is empty.",
                report.Rejected > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);
        }

        private void OnSyncFailed(Exception ex)
        {
            _syncing = false;
            SetBusy(false);
            UpdateConnectionChip();
            Fail("sync", ex);
        }

        // ── Conflict resolution ─────────────────────────────────────────────────────────────────────

        private void ShowConflict(SyncConflict conflict)
        {
            _conflict = conflict;
            this.pnlCache.Visible = false;
            this.conflictPanel.ShowConflict(conflict);
        }

        private void HideConflict()
        {
            _conflict = null;
            this.conflictPanel.Clear();
            this.pnlCache.Visible = true;
        }

        /// <summary>Keep server — the server's version stands and the field notes are preserved.</summary>
        private async void conflictPanel_KeepServerRequested(object sender, EventArgs e)
        {
            try
            {
                if (_conflict == null) return;
                this.conflictPanel.SetBusy(true);

                var conflict = _conflict;
                CommandResult result = await _sync.KeepServerAsync(conflict);

                HideConflict();
                RefreshCacheGrid();
                RenderQueue();

                ShowStatus(result.Message);
                Notify($"Nothing lost: {conflict.Code} stays {conflict.Server.Status}, your notes are attached to it, and the audit log holds both versions.",
                    MessageBoxIcon.Information);

                if (_queue.PendingCount > 0 && _shell.IsOnline)
                    StartSync();
            }
            catch (Exception ex)
            {
                this.conflictPanel.SetBusy(false);
                Fail("keep server", ex);
            }
            finally
            {
                // The awaits end after the request returned: push the final UI state to the browser.
                Application.Update(this);
            }
        }

        /// <summary>
        /// Apply my completion — writing over the dispatcher. A Technician does not hold
        /// <c>workorder.override</c>, so the server refuses and audits the attempt; the conflict stays open.
        /// </summary>
        private async void conflictPanel_ApplyMineRequested(object sender, EventArgs e)
        {
            try
            {
                if (_conflict == null) return;
                this.conflictPanel.SetBusy(true);

                var conflict = _conflict;
                CommandResult result = await _sync.ApplyMineAsync(conflict);

                RefreshCacheGrid();
                RenderQueue();

                if (result.Succeeded)
                {
                    HideConflict();
                    ShowStatus(result.Message);
                    Notify($"Your completion was applied over the server change. {result.Message}", MessageBoxIcon.Information);
                }
                else
                {
                    this.conflictPanel.SetBusy(false);
                    ShowStatus("Override denied — " + conflict.Code + " is still in conflict");
                    Notify(result.Message, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                this.conflictPanel.SetBusy(false);
                Fail("apply my completion", ex);
            }
            finally
            {
                // The awaits end after the request returned: push the final UI state to the browser.
                Application.Update(this);
            }
        }

        // ── Device-aware layout ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// The frame width, the touch-target height and which context columns survive, all read from
        /// <see cref="DeviceInfo"/>. Nothing reads a user agent.
        /// </summary>
        private void ApplyDeviceLayout()
        {
            var info = _deviceInfo ?? new DeviceInfo { Profile = DeviceProfile.Desktop };

            this.pnlDevice.Width = info.FieldWidth;

            this.btnComplete.Height = info.TouchTargetHeight;
            this.btnScan.Height = info.TouchTargetHeight;
            this.pnlFieldActions.Height = 60 + info.TouchTargetHeight + 6;

            this.colCacheSite.Visible = info.ShowsContextColumns;
            this.colCacheVersion.Visible = info.ShowsContextColumns;
            this.lblScanResult.Visible = info.ShowsContextColumns;
        }

        // ── Provisioning and rendering ──────────────────────────────────────────────────────────────

        /// <summary>Permissions first, then only the rows the technician is allowed to hold.</summary>
        private async Task ProvisionDeviceAsync()
        {
            if (!_shell.IsOnline)
                return;

            _sync.RefreshPermissions();

            List<WorkOrder> rows = await _service.GetAssignedAsync(_session.NewCommand());
            _localStore.ReplaceCache(rows.Select(CachedWorkOrder.From));

            RenderCache();
            RenderQueue();
        }

        private void RenderCache()
        {
            _cacheRows = _localStore.Cache.ToList();
            _cacheBinding.DataSource = null;
            _cacheBinding.DataSource = _cacheRows;
            UpdateCacheTitle();
        }

        private void RefreshCacheGrid()
        {
            _cacheBinding.ResetBindings(false);
            UpdateCacheTitle();
        }

        private void UpdateCacheTitle()
        {
            string cachedAt = _localStore.CachedAtUtc.HasValue
                ? _localStore.CachedAtUtc.Value.ToLocalTime().ToString("HH:mm")
                : "never";
            this.lblCacheTitle.Text = $"WORK ORDERS — LOCAL CACHE (SQLITE) · {_cacheRows.Count} ROWS · DOWNLOADED {cachedAt}";
        }

        /// <summary>Re-draws the queue cards from the queue itself — the screen never keeps its own copy.</summary>
        private void RenderQueue()
        {
            var commands = _queue.All();
            int width = Math.Max(240, this.pnlDevice.Width - 52);

            this.flpQueue.SuspendLayout();
            try
            {
                foreach (Control existing in this.flpQueue.Controls.OfType<Control>().ToArray())
                {
                    this.flpQueue.Controls.Remove(existing);
                    existing.Dispose();
                }

                foreach (var command in commands)
                {
                    var row = new OfflineCommandRow
                    {
                        Width = width,
                        Margin = new Padding(0, 0, 0, 6),
                    };
                    row.Bind(command);
                    this.flpQueue.Controls.Add(row);
                }
            }
            finally
            {
                this.flpQueue.ResumeLayout(true);
            }

            this.lblQueueTitle.Text = commands.Count == 0
                ? "COMPLETION QUEUE — EMPTY"
                : $"COMPLETION QUEUE — {_queue.PendingCount} PENDING";
        }

        private CachedWorkOrder SelectedCached()
        {
            var row = this.dgvCache.CurrentRow;
            if (row == null || row.Index < 0 || row.Index >= _cacheRows.Count)
                return null;

            return _cacheRows[row.Index];
        }

        // ── Status and messages ─────────────────────────────────────────────────────────────────────

        private static readonly Color OkColor = Color.FromArgb(31, 157, 87);
        private static readonly Color WarnColor = Color.FromArgb(232, 161, 60);
        private static readonly Color ErrorColor = Color.FromArgb(224, 86, 59);

        private void ShowStatus(string text)
        {
            this.lblSyncStatus.Text = text;
        }

        private void ShowOfflineStatus()
        {
            ShowStatus($"Offline — {_queue.PendingCount} completion(s) queued locally · cache: {_localStore.Cache.Count} work orders (SQLite)");
        }

        private static void Notify(string text, MessageBoxIcon icon)
        {
            AlertBox.Show(text, icon, alignment: ContentAlignment.TopRight, autoCloseDelay: 5000);
        }

        private void UpdateConnectionChip()
        {
            bool online = _shell.IsOnline;
            this.lblConnection.Text = online ? "ONLINE" : "OFFLINE";
            this.lblConnection.BackColor = online ? Color.FromArgb(240, 249, 243) : Color.FromArgb(255, 248, 236);
            this.lblConnection.ForeColor = online ? Color.FromArgb(21, 95, 51) : Color.FromArgb(185, 119, 14);
            this.btnToggleConnection.Text = online ? "Go offline" : "Go online";
        }

        private void SetBusy(bool busy)
        {
            this.btnComplete.Enabled = !busy;
            this.btnScan.Enabled = !busy;
            this.btnToggleConnection.Enabled = !busy;
        }

        /// <summary>Every handler's catch: the server log gets the detail, the user gets a sentence and a reference.</summary>
        private void Fail(string action, Exception ex)
        {
            string reference = Guid.NewGuid().ToString("N").Substring(0, 8);
            _trace.Log(TraceLayer.UI, $"{action} failed (ref {reference}): {ex}");
            ShowStatus($"{action} failed — nothing was applied (ref {reference})");
            Notify($"The action could not be completed. Nothing was applied, and queued work is kept. (ref {reference})", MessageBoxIcon.Error);
        }
    }
}
