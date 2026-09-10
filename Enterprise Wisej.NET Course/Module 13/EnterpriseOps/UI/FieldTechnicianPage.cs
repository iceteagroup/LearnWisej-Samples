using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using EnterpriseOps.Data;
using EnterpriseOps.Domain;
using EnterpriseOps.Hybrid;
using EnterpriseOps.Security;
using EnterpriseOps.Services;
using Wisej.Web;

namespace EnterpriseOps.UI
{
    /// <summary>
    /// Field Technician mode — Module 13's screen.
    ///
    /// Left card:   the device frame. Its width is the device-aware layout (Simulate phone / tablet /
    ///              desktop changes it and nothing else), and it holds the cached work orders, the field
    ///              actions, the completion queue, the sync conflict panel and the device status strip.
    /// Right card:  the live activity trace — every layer's decision, so the reviewer can see that the
    ///              handlers are thin and the services decided.
    /// Bottom bar:  the connection toggle (offline / online), the sync replay, the two failure setups
    ///              (dispatcher cancels · permission revoked), the anti-pattern and the recovery.
    ///
    /// The page owns no rules. It reads the selection, builds a typed command and hands it to a service:
    /// online to <see cref="WorkOrderService"/> directly, offline to <see cref="IOfflineCommandQueue"/>.
    /// It asks the device about itself only through <see cref="IDeviceServices"/>.
    ///
    /// Every handler that awaits is an "async void" event handler — the framework does not await it, so
    /// each one owns its try/catch and reports failures to the trace, the banner and a toast.
    /// </summary>
    public partial class FieldTechnicianPage : Page, IActivityTrace
    {
        private const string DispatcherUser = "ana.ops";
        private const string AdminUser = "cara.admin";

        // ── server side (the "central system"; the device never reaches past WorkOrderService) ─────────
        private readonly FakeWorkOrderRepository _repository;
        private readonly PermissionService _permissions;
        private readonly AuditLog _audit;
        private readonly WorkOrderService _service;

        // ── device side ───────────────────────────────────────────────────────────────────────────────
        private readonly LocalStore _localStore;
        private readonly BrowserDeviceServices _shell;   // the implementation: connectivity + simulate switch
        private readonly IDeviceServices _device;        // what the screen is allowed to call
        private readonly LocalOfflineCommandQueue _queue;
        private readonly SyncWorkflow _sync;

        private readonly SessionContext _session;
        private readonly BindingSource _cacheBinding = new BindingSource();

        private List<CachedWorkOrder> _cacheRows = new List<CachedWorkOrder>();
        private DeviceInfo _deviceInfo;
        private SyncConflict _conflict;
        private bool _initializing = true;
        private bool _syncing;

        /// <summary>Set by the anti-pattern button so the recovery knows which row to reconcile.</summary>
        private int _directWriteVictimId;

        public FieldTechnicianPage()
        {
            InitializeComponent();

            // Per-session identity — never a static. Services receive it, they never read the session bag.
            _session = new SessionContext
            {
                TenantId = "contoso",
                TenantName = "Contoso Utilities",
                User = "ben.tech",
                Role = "Technician",
                SessionId = Guid.NewGuid().ToString("N").Substring(0, 8),
            };

            _repository = new FakeWorkOrderRepository();
            _permissions = new PermissionService();
            _audit = new AuditLog();
            _service = new WorkOrderService(_repository, _permissions, _audit, this);

            _localStore = new LocalStore(this);
            _shell = new BrowserDeviceServices(this);
            _device = _shell;
            _queue = new LocalOfflineCommandQueue(_localStore, this);
            _sync = new SyncWorkflow(_queue, _localStore, _service, _permissions, _session, this);

            this.dgvCache.DataSource = _cacheBinding;
            this.conflictPanel.KeepServerRequested += conflictPanel_KeepServerRequested;
            this.conflictPanel.ApplyMineRequested += conflictPanel_ApplyMineRequested;
        }

        // ─────────────────────────────────────────────────────────────────────────────────────────────
        //  Load
        // ─────────────────────────────────────────────────────────────────────────────────────────────

        private async void FieldTechnicianPage_Load(object sender, EventArgs e)
        {
            try
            {
                this.lblSession.Text = $"{_session.TenantId} · {_session.User} ({_session.Role}) · session {_session.SessionId}";

                // 1. What kind of device is this? Detected once, then the Simulate switch may override it.
                _deviceInfo = _shell.Describe();
                Trace(TraceLayer.Device, $"device detected → {_deviceInfo}");
                this.cboDevice.SelectedIndex = (int)_deviceInfo.Profile;
                ApplyDeviceLayout();
                _initializing = false;

                // 2. Provision: fresh permissions, then only the rows this technician is allowed to cache.
                await ProvisionDeviceAsync();

                UpdateConnectionChip();
                SetStatus("ready · connected", StatusKind.Ok);
                this.lblSyncStatus.Text = "Online — the device is a thin client until you go offline";
                Trace(TraceLayer.UI, "field mode ready — complete a work order online, then go offline and complete two more");
            }
            catch (Exception ex)
            {
                Fail("load", ex);
            }
        }

        // ─────────────────────────────────────────────────────────────────────────────────────────────
        //  Field actions — the technician's two buttons
        // ─────────────────────────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// The success path AND the "offline is not a failure" path: the same click, the same command, two
        /// destinations. Online it goes to the server service; offline it becomes a durable OfflineCommand.
        /// The technician is told which happened; nothing is ever silently dropped.
        /// </summary>
        private async void btnComplete_Click(object sender, EventArgs e)
        {
            try
            {
                var selected = SelectedCached();
                if (selected == null)
                {
                    SetBanner("Pick a work order in the cache first.", StatusKind.Warning);
                    return;
                }

                string notes = (this.txtNotes.Text ?? "").Trim();
                if (notes.Length == 0)
                {
                    // Validation path: rejected before anything is queued or sent.
                    Trace(TraceLayer.UI, $"complete {selected.Code} rejected locally — completion notes are required in the field");
                    SetBanner("Completion notes are required — the office cannot audit \"done\".", StatusKind.Warning);
                    AlertBox.Show("Enter what you did before completing the work order.", MessageBoxIcon.Warning,
                        alignment: ContentAlignment.TopRight, autoCloseDelay: 4000);
                    return;
                }

                var command = new CompleteWorkOrderCommand
                {
                    WorkOrderId = selected.Id,
                    ExpectedVersion = selected.Version,
                    Notes = notes,
                    CompletedAt = DateTimeOffset.Now,
                };

                Trace(TraceLayer.UI, $"complete {selected.Code} clicked (device holds v{selected.Version}) — asking IDeviceServices whether we are online");

                if (await _device.IsOnlineAsync())
                {
                    var ctx = _session.NewCommand();
                    Trace(TraceLayer.UI, $"online → WorkOrderService.CompleteAsync directly (corr {ctx.CorrelationId})");

                    CommandResult result = await _service.CompleteAsync(command, ctx);
                    if (result.Succeeded)
                    {
                        selected.Status = WorkOrderStatus.Completed.ToString();
                        selected.Version = result.NewVersion;
                        selected.LocalState = "";
                        SetStatus($"{selected.Code} completed on the server", StatusKind.Ok);
                        SetBanner($"{result.Message} Nothing was queued — the device was connected.", StatusKind.Ok);
                        AlertBox.Show(result.Message, MessageBoxIcon.Information,
                            alignment: ContentAlignment.TopRight, autoCloseDelay: 4000);
                    }
                    else
                    {
                        SetStatus($"{selected.Code} refused by the server", StatusKind.Error);
                        SetBanner(result.Message, StatusKind.Error);
                        AlertBox.Show(result.Message, MessageBoxIcon.Error,
                            alignment: ContentAlignment.TopRight, autoCloseDelay: 5000);
                    }
                }
                else
                {
                    // Offline: the record is NOT updated. A command is appended with an explicit SyncState.
                    var offline = _queue.CreateCompletion(command, selected.Code,
                        $"Completed {command.CompletedAt.ToLocalTime():HH:mm} — {notes}");
                    await _queue.EnqueueAsync(offline);
                    await _device.VibrateAsync();

                    selected.LocalState = "Completed · pending sync";
                    Trace(TraceLayer.UI, $"offline → the work order row is untouched; {selected.Code} is a PendingSync command on the device");

                    SetStatus($"{selected.Code} saved on the device · {_queue.PendingCount} pending", StatusKind.Warning);
                    SetBanner($"No signal — {selected.Code} is queued locally and will sync when you reconnect. It is not lost and it is not applied.", StatusKind.Warning);
                    AlertBox.Show($"{selected.Code} saved on this device — pending sync.", MessageBoxIcon.Warning,
                        alignment: ContentAlignment.TopRight, autoCloseDelay: 4000);
                }

                RefreshCacheGrid();
                RenderQueue();
            }
            catch (Exception ex)
            {
                Fail("complete", ex);
            }
        }

        /// <summary>
        /// A device capability used through the abstraction. The scanner works offline — but its output is
        /// untrusted input: online it is validated by the server immediately, offline it travels inside the
        /// queued command and is validated at sync time.
        /// </summary>
        private async void btnScan_Click(object sender, EventArgs e)
        {
            try
            {
                var selected = SelectedCached();
                if (selected == null)
                {
                    SetBanner("Pick a work order in the cache first.", StatusKind.Warning);
                    return;
                }

                Trace(TraceLayer.UI, $"scan requested for {selected.Code} — the screen calls IDeviceServices, not a camera API");
                string scanned = await _device.ScanDocumentAsync();

                if (await _device.IsOnlineAsync())
                {
                    var ctx = _session.NewCommand();
                    CommandResult result = await _service.ValidateScannedAssetAsync(selected.Id, scanned, ctx);
                    this.lblScanResult.Text = (result.Succeeded ? "✓ " : "✖ ") + result.Message;
                    this.lblScanResult.ForeColor = result.Succeeded ? OkColor : ErrorColor;
                    if (!result.Succeeded)
                        SetBanner(result.Message + " Scanned values are validated on the server like any other input.", StatusKind.Warning);
                }
                else
                {
                    this.lblScanResult.Text = $"{scanned} — recorded offline, the server validates it at sync";
                    this.lblScanResult.ForeColor = WarnColor;
                    Trace(TraceLayer.UI, "offline scan: the value is carried in the command payload, never trusted by the device");
                }

                this.txtNotes.Text = $"{(this.txtNotes.Text ?? "").Trim()} (asset {scanned})".Trim();
            }
            catch (Exception ex)
            {
                Fail("scan", ex);
            }
        }

        // ─────────────────────────────────────────────────────────────────────────────────────────────
        //  Connectivity and the reconnect sync
        // ─────────────────────────────────────────────────────────────────────────────────────────────

        private async void btnToggleConnection_Click(object sender, EventArgs e)
        {
            try
            {
                bool goingOnline = !_shell.IsOnline;
                _shell.SetOnline(goingOnline);
                UpdateConnectionChip();

                if (!goingOnline)
                {
                    SetStatus("offline — working from the local cache", StatusKind.Warning);
                    SetBanner($"Offline. The cache ({_localStore.Cache.Count} work orders) and the queue keep working; the server is unreachable.", StatusKind.Warning);
                    this.lblSyncStatus.Text = $"Offline — {_queue.PendingCount} completion(s) queued locally · cache: {_localStore.Cache.Count} work orders (SQLite)";
                    return;
                }

                SetStatus("connection restored", StatusKind.Ok);
                if (_queue.PendingCount == 0)
                {
                    var fresh = _sync.RefreshPermissions();
                    SetBanner($"Back online. Permissions refreshed ({fresh.Role}); nothing was queued.", StatusKind.Ok);
                    this.lblSyncStatus.Text = "Online — permissions refreshed · queue empty";
                    return;
                }

                SetBanner($"Back online — replaying {_queue.PendingCount} queued completion(s) through the server service.", StatusKind.Ok);
                StartSync();
            }
            catch (Exception ex)
            {
                Fail("toggle connection", ex);
            }
        }

        private void btnSync_Click(object sender, EventArgs e)
        {
            try
            {
                if (!_shell.IsOnline)
                {
                    SetBanner("Still offline — the queue replays only when the shell reports a connection.", StatusKind.Warning);
                    Trace(TraceLayer.UI, "sync refused: no connection (the queue is durable, so waiting costs nothing)");
                    return;
                }

                if (_queue.PendingCount == 0)
                {
                    SetBanner("Nothing to sync — the queue is empty.", StatusKind.Ok);
                    return;
                }

                StartSync();
            }
            catch (Exception ex)
            {
                Fail("sync", ex);
            }
        }

        /// <summary>
        /// The progress path. The replay runs on a background task; every step pushes its own UI update over
        /// the WebSocket with <c>Application.Update(this, …)</c>. The workflow paces itself (700 ms per
        /// command), well above the 250 ms floor for out-of-band pushes.
        /// </summary>
        private void StartSync()
        {
            if (_syncing)
                return;

            _syncing = true;
            SetBusy(true);
            this.prgSync.Maximum = Math.Max(1, _queue.PendingCount);
            this.prgSync.Value = 0;
            this.lblSyncStatus.Text = "Connection restored — refreshing permissions…";
            Trace(TraceLayer.UI, $"sync started on a background task — {_queue.PendingCount} command(s) to replay");

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
            this.lblSyncStatus.Text = p.Message;
            RefreshCacheGrid();
            RenderQueue();

            if (p.Conflict != null)
                ShowConflict(p.Conflict);
        }

        private void OnSyncFinished(SyncReport report)
        {
            _syncing = false;
            SetBusy(false);

            if (report.StoppedOnConflict)
            {
                this.lblSyncStatus.Text = $"Sync conflict on {report.Conflict.Code} — server changed while offline · waiting for the technician";
                SetStatus($"conflict on {report.Conflict.Code}", StatusKind.Error);
                SetBanner(report.Message + " Both versions are on screen — choose one; nothing is lost either way.", StatusKind.Error);
                AlertBox.Show(report.Message, MessageBoxIcon.Warning,
                    alignment: ContentAlignment.TopRight, autoCloseDelay: 5000);
                return;
            }

            this.prgSync.Value = this.prgSync.Maximum;
            this.lblSyncStatus.Text = report.Message;
            var kind = report.Rejected > 0 ? StatusKind.Warning : StatusKind.Ok;
            SetStatus(report.Rejected > 0 ? "sync finished with rejections" : "sync complete", kind);
            SetBanner(report.Rejected > 0
                ? $"{report.Rejected} queued command(s) were rejected by the server (re-checked against current permissions) and audited. {report.Synced} synced."
                : $"{report.Synced} completion(s) synced and audited. The queue is empty.", kind);
            AlertBox.Show(report.Message, report.Rejected > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information,
                alignment: ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        private void OnSyncFailed(Exception ex)
        {
            _syncing = false;
            SetBusy(false);
            this.lblSyncStatus.Text = "Sync failed — the queue is untouched and will retry";
            Trace(TraceLayer.Job, $"sync failed: {ex.GetType().Name} — {ex.Message} (nothing was marked Synced, so the queue retries safely)");
            SetStatus("sync failed", StatusKind.Error);
            SetBanner("The sync could not finish. The queue is durable: nothing was lost, press Sync now to retry.", StatusKind.Error);
        }

        // ─────────────────────────────────────────────────────────────────────────────────────────────
        //  Conflict resolution
        // ─────────────────────────────────────────────────────────────────────────────────────────────

        private void ShowConflict(SyncConflict conflict)
        {
            _conflict = conflict;
            this.pnlCache.Visible = false;         // the conflict takes the screen: resolve it, then carry on
            this.conflictPanel.ShowConflict(conflict);
            Trace(TraceLayer.UI, $"conflict screen for {conflict.Code}: local (v{conflict.LocalBaseVersion}) vs server (v{conflict.Server.Version}) — the user decides");
        }

        private void HideConflict()
        {
            _conflict = null;
            this.conflictPanel.Clear();
            this.pnlCache.Visible = true;
        }

        /// <summary>Resolution A — the server's version stands and the field notes are preserved.</summary>
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

                SetStatus($"{conflict.Code} resolved — server kept", StatusKind.Ok);
                SetBanner($"Nothing lost: {conflict.Code} stays {conflict.Server.Status}, your notes are attached to it, and the audit log holds both versions.", StatusKind.Ok);
                this.lblSyncStatus.Text = result.Message;
                AlertBox.Show(result.Message, MessageBoxIcon.Information,
                    alignment: ContentAlignment.TopRight, autoCloseDelay: 4000);

                if (_queue.PendingCount > 0 && _shell.IsOnline)
                {
                    Trace(TraceLayer.UI, $"resuming the replay — {_queue.PendingCount} command(s) still pending behind the conflict");
                    StartSync();
                }
            }
            catch (Exception ex)
            {
                this.conflictPanel.SetBusy(false);
                Fail("keep server", ex);
            }
        }

        /// <summary>
        /// Resolution B — writing over the dispatcher. A Technician does not hold <c>workorder.override</c>,
        /// so the server refuses and audits the attempt; the conflict stays open with the real reason on it.
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
                    SetStatus($"{conflict.Code} overridden", StatusKind.Warning);
                    SetBanner($"Your completion was applied over the server change and audited as an override. {result.Message}", StatusKind.Warning);
                }
                else
                {
                    this.conflictPanel.SetBusy(false);
                    SetStatus("override denied", StatusKind.Error);
                    SetBanner(result.Message, StatusKind.Error);
                    AlertBox.Show(result.Message, MessageBoxIcon.Error,
                        alignment: ContentAlignment.TopRight, autoCloseDelay: 6000);
                }
            }
            catch (Exception ex)
            {
                this.conflictPanel.SetBusy(false);
                Fail("apply my completion", ex);
            }
        }

        // ─────────────────────────────────────────────────────────────────────────────────────────────
        //  Failure setups, the anti-pattern and the recovery
        // ─────────────────────────────────────────────────────────────────────────────────────────────

        /// <summary>The other half of a conflict: the office changes the row while the device is away.</summary>
        private async void btnDispatcherCancel_Click(object sender, EventArgs e)
        {
            try
            {
                var selected = SelectedCached();
                if (selected == null)
                {
                    SetBanner("Pick the work order the dispatcher should cancel.", StatusKind.Warning);
                    return;
                }

                var dispatcher = new CommandContext(_session.TenantId, DispatcherUser, Guid.NewGuid().ToString("N").Substring(0, 8));
                Trace(TraceLayer.UI, $"dispatcher {DispatcherUser} cancels {selected.Code} in the office (corr {dispatcher.CorrelationId})");

                CommandResult result = await _service.CancelAsync(selected.Id, "site rescheduled to Thursday crew", dispatcher);
                if (!result.Succeeded)
                {
                    SetBanner(result.Message, StatusKind.Warning);
                    return;
                }

                Trace(TraceLayer.Device, $"the device still holds {selected.Code} at v{selected.Version} — it has no way to know, and that is normal");
                SetStatus($"{selected.Code} cancelled on the server", StatusKind.Warning);
                SetBanner($"{result.Message} The device's cached copy is now stale — complete it offline and the sync will find the conflict.", StatusKind.Warning);
            }
            catch (Exception ex)
            {
                Fail("dispatcher cancel", ex);
            }
        }

        /// <summary>Permissions go stale offline: the server's answer changes while the device holds yesterday's.</summary>
        private void btnRevokePermission_Click(object sender, EventArgs e)
        {
            try
            {
                _permissions.Revoke(_session.User, Permissions.CompleteWorkOrder);
                Trace(TraceLayer.Security, $"admin revoked {Permissions.CompleteWorkOrder} from {_session.User} on the SERVER");
                Trace(TraceLayer.Device, $"the device's cached snapshot is unchanged and now wrong: {_localStore.Permissions}");

                SetStatus("permission revoked on the server", StatusKind.Warning);
                SetBanner("ben.tech may no longer complete work orders. The device still shows the old permission set — the sync boundary re-checks every queued command, so they will be rejected and audited, not applied.", StatusKind.Warning);
            }
            catch (Exception ex)
            {
                Fail("revoke permission", ex);
            }
        }

        /// <summary>
        /// The anti-pattern the walkthrough warns about: the screen writes straight to the store because
        /// "it is only one field". No permission check, no version check, no audit entry, and the
        /// dispatcher's decision is gone with nothing to explain where it went.
        /// </summary>
        private void btnAntiPattern_Click(object sender, EventArgs e)
        {
            try
            {
                var selected = SelectedCached();
                if (selected == null)
                {
                    SetBanner("Pick a work order to demonstrate the anti-pattern on.", StatusKind.Warning);
                    return;
                }

                int auditBefore = _audit.All().Count;

                // ✖ DO NOT COPY. The screen reaching into Data/ is exactly what the sync boundary exists to prevent.
                var live = _repository.Find(_session.TenantId, selected.Id);
                string overwritten = $"{live.Status} v{live.Version}";
                int newVersion = _repository.Update(_session.TenantId, live.Id, live.Version, r =>
                {
                    r.Status = WorkOrderStatus.Completed;
                    r.Notes = (r.Notes ?? "") + " | direct write from the device";
                    r.LastChangedBy = _session.User + " (direct write)";
                });

                _directWriteVictimId = live.Id;
                selected.Status = WorkOrderStatus.Completed.ToString();
                selected.Version = newVersion;
                selected.LocalState = "written directly ✖";

                Trace(TraceLayer.UI, $"✖ anti-pattern: the screen called FakeWorkOrderRepository.Update({live.Code}) itself");
                Trace(TraceLayer.Security, $"✖ no permission check ran — {_session.User} may or may not still hold {Permissions.CompleteWorkOrder}; nobody asked");
                Trace(TraceLayer.Service, $"✖ no concurrency check against the device's version — {overwritten} was overwritten with Completed v{newVersion}");
                Trace(TraceLayer.Data, $"✖ audit log still has {_audit.All().Count} entries (was {auditBefore}) — support has no record that this happened");

                SetStatus("record changed with no rules and no audit", StatusKind.Error);
                SetBanner($"{live.Code}: the dispatcher's \"{overwritten}\" is gone, no permission was checked and nothing was audited. This is why the device only ever talks to WorkOrderService.", StatusKind.Error);
                AlertBox.Show("Direct write applied — and it is invisible to support.", MessageBoxIcon.Error,
                    alignment: ContentAlignment.TopRight, autoCloseDelay: 5000);

                RefreshCacheGrid();
            }
            catch (Exception ex)
            {
                Fail("anti-pattern", ex);
            }
        }

        /// <summary>
        /// The recovery, in the order support would do it: put the server row back through the service so
        /// the incident is audited by an account that is allowed to do it, then throw the device's local
        /// state away and re-provision it. Local data is never repaired in place — it is re-downloaded.
        /// </summary>
        private async void btnRecover_Click(object sender, EventArgs e)
        {
            try
            {
                if (!_shell.IsOnline)
                {
                    SetBanner("Re-provisioning needs a connection — go online first.", StatusKind.Warning);
                    return;
                }

                if (_directWriteVictimId > 0)
                {
                    var admin = new CommandContext(_session.TenantId, AdminUser, Guid.NewGuid().ToString("N").Substring(0, 8));
                    Trace(TraceLayer.UI, $"recovery step 1 — {AdminUser} replays the correction through WorkOrderService (corr {admin.CorrelationId})");
                    CommandResult fix = await _service.CancelAsync(_directWriteVictimId, "reconciled: restored after an unaudited direct write from a device", admin);
                    Trace(TraceLayer.Data, $"audit log now has {_audit.All().Count} entries — the incident is finally on the record ({fix.Message})");
                    _directWriteVictimId = 0;
                }

                Trace(TraceLayer.UI, "recovery step 2 — the device's local state is discarded, not repaired");
                _localStore.Wipe();
                HideConflict();
                await ProvisionDeviceAsync();
                RenderQueue();

                SetStatus("device re-provisioned", StatusKind.Ok);
                SetBanner("Recovered: the server row was corrected through the service and audited, and the device re-downloaded its cache and a fresh permission snapshot.", StatusKind.Ok);
                this.lblSyncStatus.Text = "Device re-provisioned — cache and permissions are the server's, not yesterday's";
            }
            catch (Exception ex)
            {
                Fail("recover", ex);
            }
        }

        // ─────────────────────────────────────────────────────────────────────────────────────────────
        //  Device-aware layout
        // ─────────────────────────────────────────────────────────────────────────────────────────────

        private void cboDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_initializing)
                return;

            try
            {
                var profile = (DeviceProfile)Math.Max(0, this.cboDevice.SelectedIndex);
                _shell.Simulate(profile);
                _deviceInfo = _shell.Describe();
                ApplyDeviceLayout();
                RenderQueue();

                SetBanner($"Layout re-laid out for {_deviceInfo.Profile}: {_deviceInfo.FieldWidth} px wide, {_deviceInfo.TouchTargetHeight} px touch targets, context columns {( _deviceInfo.ShowsContextColumns ? "on" : "off")}. No screen logic changed.", StatusKind.Ok);
            }
            catch (Exception ex)
            {
                Fail("simulate device", ex);
            }
        }

        /// <summary>
        /// The whole "device-aware" part of the screen, in one place: the frame width, the touch-target
        /// height and which context columns survive. Everything reads <see cref="DeviceInfo"/>; nothing
        /// reads a user agent.
        /// </summary>
        private void ApplyDeviceLayout()
        {
            var info = _deviceInfo ?? new DeviceInfo { Profile = DeviceProfile.Desktop };

            this.pnlDevice.Width = info.FieldWidth;
            this.lblFieldTitle.Text = "Field mode · " + info.Profile;

            this.btnComplete.Height = info.TouchTargetHeight;
            this.btnScan.Height = info.TouchTargetHeight;
            this.pnlFieldActions.Height = 60 + info.TouchTargetHeight + 6;

            this.colCacheSite.Visible = info.ShowsContextColumns;
            this.colCacheVersion.Visible = info.ShowsContextColumns;
            this.lblScanResult.Visible = info.ShowsContextColumns;

            Trace(TraceLayer.UI, $"layout → {info.Profile}: frame {info.FieldWidth}px · touch targets {info.TouchTargetHeight}px · context columns {(info.ShowsContextColumns ? "shown" : "hidden")}");
        }

        // ─────────────────────────────────────────────────────────────────────────────────────────────
        //  Provisioning and rendering
        // ─────────────────────────────────────────────────────────────────────────────────────────────

        /// <summary>Permissions first, then only the rows the technician is allowed to hold. That order matters.</summary>
        private async System.Threading.Tasks.Task ProvisionDeviceAsync()
        {
            if (!_shell.IsOnline)
            {
                Trace(TraceLayer.Device, "cannot provision while offline — the device keeps what it has");
                return;
            }

            var fresh = _sync.RefreshPermissions();
            Trace(TraceLayer.Security, $"permission snapshot issued to the device: {fresh}");

            var ctx = _session.NewCommand();
            List<WorkOrder> rows = await _service.GetAssignedAsync(ctx);
            _localStore.ReplaceCache(rows.Select(CachedWorkOrder.From));
            Trace(TraceLayer.Device, $"cache scope: {_session.User}'s own open assignments for {_session.TenantId} only — not the tenant's table, not other technicians' work");

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

            int pending = _queue.PendingCount;
            this.lblQueueTitle.Text = commands.Count == 0
                ? "COMPLETION QUEUE — EMPTY"
                : $"COMPLETION QUEUE — {pending} PENDING OF {commands.Count} · {_localStore.ApproximateSizeBytes()} BYTES ON THE DEVICE";
        }

        private CachedWorkOrder SelectedCached()
        {
            var row = this.dgvCache.CurrentRow;
            if (row == null || row.Index < 0 || row.Index >= _cacheRows.Count)
                return null;

            return _cacheRows[row.Index];
        }

        // ─────────────────────────────────────────────────────────────────────────────────────────────
        //  Trace, status, banner
        // ─────────────────────────────────────────────────────────────────────────────────────────────

        private static readonly Color OkColor = Color.FromArgb(31, 157, 87);
        private static readonly Color WarnColor = Color.FromArgb(232, 161, 60);
        private static readonly Color ErrorColor = Color.FromArgb(224, 86, 59);

        private enum StatusKind { Ok, Warning, Error }

        /// <summary>IActivityTrace — the one sink every layer writes to (see the right-hand card).</summary>
        void IActivityTrace.Log(string layer, string message) => Trace(layer, message);

        private void Trace(string layer, string message)
        {
            if (this.IsDisposed)
                return;

            try
            {
                this.lstTrace.Items.Add($"{DateTime.Now:HH:mm:ss.fff}  {layer} {message}");
                while (this.lstTrace.Items.Count > 400)
                    this.lstTrace.Items.RemoveAt(0);

                this.lstTrace.SelectedIndex = this.lstTrace.Items.Count - 1;
            }
            catch (ObjectDisposedException)
            {
            }
        }

        private void btnClearTrace_Click(object sender, EventArgs e)
        {
            this.lstTrace.Items.Clear();
            SetBanner("", StatusKind.Ok);
        }

        private void SetStatus(string text, StatusKind kind)
        {
            this.lblStatus.Text = "● " + text;
            this.lblStatus.ForeColor = ColorFor(kind);
        }

        private void SetBanner(string text, StatusKind kind)
        {
            this.lblBanner.Text = text;
            this.lblBanner.ForeColor = string.IsNullOrEmpty(text) ? Color.FromArgb(70, 88, 106) : ColorFor(kind);
            this.lblBanner.ToolTipText = text;
        }

        private static Color ColorFor(StatusKind kind)
            => kind == StatusKind.Ok ? OkColor : kind == StatusKind.Warning ? WarnColor : ErrorColor;

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
            this.btnSync.Enabled = !busy;
            this.btnToggleConnection.Enabled = !busy;
            this.btnDispatcherCancel.Enabled = !busy;
            this.btnRevokePermission.Enabled = !busy;
            this.btnAntiPattern.Enabled = !busy;
            this.btnRecover.Enabled = !busy;
        }

        /// <summary>Every handler's catch: the trace gets the detail, the user gets a sentence.</summary>
        private void Fail(string action, Exception ex)
        {
            Trace(TraceLayer.UI, $"{action} failed: {ex.GetType().Name} — {ex.Message}");
            SetStatus($"{action} failed", StatusKind.Error);
            SetBanner("The action could not be completed. The activity trace has the details; nothing was applied.", StatusKind.Error);
            AlertBox.Show("The action could not be completed. Check the activity trace for details.", MessageBoxIcon.Error,
                alignment: ContentAlignment.TopRight, autoCloseDelay: 5000);
        }
    }
}
