using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnterpriseOps.Data;
using EnterpriseOps.Domain;
using EnterpriseOps.Interop;
using EnterpriseOps.Security;
using EnterpriseOps.Services;
using Wisej.Core;
using Wisej.Web;

namespace EnterpriseOps.UI
{
    /// <summary>
    /// EnterpriseOps — Command Center (Module 9 lab screen).
    ///
    /// Left column   : the command palette host (the browser-side script) and the browser
    ///                 capability panel (what detection found, and the fallback the SERVER chose).
    /// Right column  : the live activity trace — Client → Interop → Security → Service → Data —
    ///                 and the append-only audit log.
    /// Bottom bar    : the success paths, four contract failure paths, the "trust the client"
    ///                 anti-pattern and its recovery.
    ///
    /// This is a top-level Page (Application.MainPage), which is why the two [WebMethod]s below are
    /// registered automatically and reachable from JavaScript as App.MainPage.RunClientCommand…
    ///
    /// The handlers are deliberately boring: they push a call to the widget or ask a service, then
    /// render. Every decision — catalogue, arity, permission, target, state — lives in
    /// <see cref="ClientCommandService"/>, which a reviewer can read without opening the designer.
    /// </summary>
    public partial class CommandCenterShell : Page
    {
        private readonly ActivityTrace _trace = new ActivityTrace();
        private readonly SessionContext _session;
        private readonly IWorkOrderRepository _repository;
        private readonly WorkOrderService _workOrders;
        private readonly PermissionService _permissions;
        private readonly AuditLog _audit;
        private readonly ClientCommandService _commands;
        private readonly BrowserCapabilityService _capabilities;

        private bool _loadingCombos;

        public CommandCenterShell()
        {
            InitializeComponent();

            // One graph of per-session services. Nothing user- or tenant-specific is static.
            _session = ServiceRegistry.CreateSessionContext();
            _repository = new InMemoryWorkOrderRepository();
            _permissions = new PermissionService(_trace);
            _audit = new AuditLog(_trace);
            _workOrders = new WorkOrderService(_repository, _trace);
            _commands = new ClientCommandService(_session, _permissions, _workOrders, _audit, _trace);
            _capabilities = new BrowserCapabilityService(_trace);

            _trace.EntryAdded += OnTraceEntry;

            // The widget gets the boundary owner — and nothing else about the server.
            this.commandPaletteHost.Attach(_commands);
            this.commandPaletteHost.Ready += commandPaletteHost_Ready;
            this.commandPaletteHost.Deferred += commandPaletteHost_Deferred;
            this.commandPaletteHost.PaletteState += commandPaletteHost_PaletteState;
            this.commandPaletteHost.CommandRun += commandPaletteHost_CommandRun;
            this.commandPaletteHost.CapabilitiesReported += commandPaletteHost_CapabilitiesReported;
            this.commandPaletteHost.PaletteError += commandPaletteHost_PaletteError;

            FillCombos();
        }

        #region Load

        private void CommandCenterShell_Load(object sender, EventArgs e)
        {
            _trace.Ui($"CommandCenterShell loaded · session {_session.SessionId} · correlation {_session.CorrelationId}");
            _trace.Interop($"contract v{InteropContract.Version} · {InteropContract.Catalog.Count} commands · fields commandName, entityId, correlationId");
            _trace.Interop("WebMethods: App.MainPage.RunClientCommand (contract) · App.MainPage.RunTrustedClientCommand (⚠ anti-pattern) · commandPaletteHost.GetCommandCatalog (RegisterWebMethods)");

            this.lblCorrelation.Text = $"session {_session.CorrelationId} · contract v{InteropContract.Version}";
            this.commandPaletteHost.TargetEntityId = this.txtEntityId.Text;
            this.capabilityPanel.Render(null, _capabilities.Summary(), 0);
            RenderAudit();

            // Lifecycle proof: this call is made while the client widget does not exist yet.
            // It is not lost and it is not sent — it waits for paletteReady, and the trace says so.
            this.commandPaletteHost.ShowResult("WAITING", "contract loaded, palette not attached yet");
            UpdatePaletteState();
        }

        private void FillCombos()
        {
            _loadingCombos = true;
            try
            {
                foreach (Tenant tenant in Tenant.All)
                    this.cboTenant.Items.Add($"{tenant.Name} ({tenant.Id})");
                this.cboTenant.SelectedIndex = 0;

                foreach (AppUser user in AppUser.Directory)
                    this.cboUser.Items.Add($"{user.UserName} — {user.Role}");
                this.cboUser.SelectedIndex = 0;
            }
            finally
            {
                _loadingCombos = false;
            }
            ShowIdentity();
        }

        #endregion

        #region (a) The server callback methods — the WebMethods JavaScript may call

        /// <summary>
        /// THE boundary. Three named strings in, one typed result out, and five gates in between
        /// (see <see cref="ClientCommandService.ExecuteFromClient"/>).
        ///
        /// The method itself does exactly two things: parse the payload against the published
        /// contract, and hand a validated request to the owner. It never touches a repository, and
        /// it never trusts a field — not even the correlation id, which is only echoed for logging.
        /// </summary>
        [WebMethod]
        public object RunClientCommand(string commandName, string entityId, string correlationId)
        {
            try
            {
                _trace.Client($"App.MainPage.RunClientCommand(\"{Safe(commandName, 40)}\", \"{Safe(entityId, 16)}\", \"{Safe(correlationId, 12)}\") — untrusted");

                if (!InteropContract.TryParse(commandName, entityId, correlationId,
                        out ClientCommandRequest request, out string code, out string error))
                {
                    _trace.Interop($"payload rejected by the contract before any service ran · {error}");
                    CommandContext context = _session.NewCommand();
                    _audit.Record(context, Safe(commandName, 40), Safe(entityId, 16), AuditOutcome.Rejected, $"{code}: {error}");
                    ClientCommandResult rejected = ClientCommandResult.Fail(Safe(commandName, 40), context.CorrelationId, code,
                        "That request was not understood.");     // the user-facing text never quotes the payload
                    Render(rejected);
                    return rejected;
                }

                ClientCommandResult result = _commands.ExecuteFromClient(request);
                Render(result);
                return result;
            }
            catch (Exception ex)
            {
                // The browser must never see an exception message: it gets a named code instead.
                _trace.Interop($"unhandled {ex.GetType().Name} in RunClientCommand — answering SERVER_ERROR");
                ClientCommandResult failure = ClientCommandResult.Fail(Safe(commandName, 40), _session.CorrelationId,
                    ResultCodes.ServerError, "The command could not be completed. Check the log for details.");
                Render(failure);
                return failure;
            }
        }

        /// <summary>
        /// ⚠ The anti-pattern the walkthrough warns about, kept as a SEPARATE endpoint so nobody can
        /// reach it by accident: the role and the new status arrive in the payload and are believed.
        /// Run it, watch the audit log say TAMPERED, then press "Revert tampered".
        /// </summary>
        [WebMethod]
        public object RunTrustedClientCommand(string commandName, string entityId, string claimedRole, string claimedStatus)
        {
            try
            {
                _trace.Client($"App.MainPage.RunTrustedClientCommand(\"{Safe(commandName, 40)}\", \"{Safe(entityId, 16)}\", role=\"{Safe(claimedRole, 16)}\", status=\"{Safe(claimedStatus, 16)}\") ⚠");
                ClientCommandResult result = _commands.ExecuteTrustingClient(
                    Safe(commandName, 40), Safe(entityId, 16), Safe(claimedRole, 16), Safe(claimedStatus, 16));
                Render(result, antiPattern: true);
                return result;
            }
            catch (Exception ex)
            {
                _trace.Interop($"unhandled {ex.GetType().Name} in RunTrustedClientCommand");
                return ClientCommandResult.Fail(Safe(commandName, 40), _session.CorrelationId, ResultCodes.ServerError, "Failed.");
            }
        }

        #endregion

        #region (b) Client → server events raised by the host widget

        private void commandPaletteHost_Ready(object sender, PaletteReadyEventArgs e)
        {
            _trace.Client($"paletteReady · hotkey {e.Hotkey} · contract v{e.ContractVersion} — the client widget now exists");
            _trace.Interop($"flushing {this.commandPaletteHost.DeferredCalls} deferred server → client call(s)");
            UpdatePaletteState();
            SetStatus("Palette attached — press Ctrl+K, or use the buttons below.", StatusKind.Ok);
        }

        private void commandPaletteHost_Deferred(object sender, PaletteErrorEventArgs e)
        {
            _trace.Interop($"server → client call \"{e.Message}\" DEFERRED — the host widget does not exist yet");
            UpdatePaletteState();
        }

        private void commandPaletteHost_PaletteState(object sender, PaletteErrorEventArgs e)
        {
            _trace.Client($"palette {e.Phase} (via {(string.IsNullOrEmpty(e.Message) ? "—" : e.Message)}) — a UI event, no business meaning");
        }

        private void commandPaletteHost_CommandRun(object sender, PaletteCommandEventArgs e)
        {
            _trace.Client($"browser rendered {e.Code} for {e.CommandName} in {e.ElapsedMs} ms — its view of the same crossing");
        }

        private void commandPaletteHost_CapabilitiesReported(object sender, CapabilityReportEventArgs e)
        {
            if (e.Forged) _trace.Client("⚠ the report carries keys the server never published");

            IReadOnlyList<CapabilityReading> readings = _capabilities.Accept(e.Report);
            this.capabilityPanel.Render(readings, _capabilities.Summary(), _capabilities.IgnoredKeys);

            if (_capabilities.IgnoredKeys > 0)
            {
                ShowBanner($"{_capabilities.IgnoredKeys} unknown capability key(s) ignored — a report cannot invent a capability, and a capability cannot grant a permission.");
                SetStatus("Capability report accepted with unknown keys dropped.", StatusKind.Warn);
            }
            else
            {
                HideBanner();
                SetStatus($"Capability report accepted — {_capabilities.Summary()}", StatusKind.Ok);
            }
        }

        private void commandPaletteHost_PaletteError(object sender, PaletteErrorEventArgs e)
        {
            _trace.Client($"widget error · {e.Phase} · {Safe(e.Message, 120)}");
            ShowBanner($"The palette script reported a problem in {e.Phase}.");
            SetStatus("The palette script reported an error — the server state is unchanged.", StatusKind.Error);
        }

        #endregion

        #region (c) Buttons — success and progress paths

        /// <summary>Server → client: push the target into the widget's Options.</summary>
        private void btnSetTarget_Click(object sender, EventArgs e)
        {
            string entityId = (this.txtEntityId.Text ?? "").Trim().ToUpperInvariant();
            this.txtEntityId.Text = entityId;
            _trace.Ui($"set palette target {entityId} (Options.entityId → update(options, old))");
            this.commandPaletteHost.TargetEntityId = entityId;

            if (InteropContract.TryReadEntityKey(entityId, out int key))
            {
                WorkOrder order = _workOrders.Resolve(_session.TenantId, key);
                SetStatus(order == null
                        ? $"{entityId} does not resolve in {_session.TenantId} — running a command on it will answer INVALID_TARGET."
                        : $"{entityId} · {order.Status} · {order.Priority} · v{order.Version}",
                    order == null ? StatusKind.Warn : StatusKind.Ok);
            }
            else
            {
                SetStatus($"{entityId} is not a valid wire id — the contract expects WO-1040.", StatusKind.Warn);
            }
        }

        /// <summary>Server → client callback: open the palette without a keyboard.</summary>
        private void btnOpenPalette_Click(object sender, EventArgs e)
        {
            _trace.Ui("Call(\"paletteOpen\") — server callback into the client widget");
            this.commandPaletteHost.OpenPalette();
            UpdatePaletteState();
        }

        /// <summary>
        /// Success path (and, as ben.tech, the walkthrough's failure path): the SCRIPT sends
        /// workorder.approve for the current target, so the round trip really is browser → WebMethod.
        /// </summary>
        private void btnRunApprove_Click(object sender, EventArgs e)
        {
            SendFromPalette("workorder.approve", (this.txtEntityId.Text ?? "").Trim().ToUpperInvariant());
        }

        private void btnRunEscalate_Click(object sender, EventArgs e)
        {
            SendFromPalette("workorder.escalate", (this.txtEntityId.Text ?? "").Trim().ToUpperInvariant());
        }

        private void btnRunQueue_Click(object sender, EventArgs e)
        {
            SendFromPalette("workqueue.open", "");
        }

        #endregion

        #region (d) Buttons — the four contract failure paths

        private void btnUnknownCommand_Click(object sender, EventArgs e)
        {
            _trace.Ui("failure path · a command id that is not in the catalogue");
            SendFromPalette("workorder.delete", (this.txtEntityId.Text ?? "").Trim().ToUpperInvariant());
        }

        private void btnMalformed_Click(object sender, EventArgs e)
        {
            _trace.Ui("failure path · oversized entity id + a correlation id that is not 8 hex characters");
            this.commandPaletteHost.RunWithBadCorrelation("workorder.approve", "WO-1040-INJECT");
            UpdatePaletteState();
        }

        private void btnInvalidState_Click(object sender, EventArgs e)
        {
            _trace.Ui("failure path · approve a Completed work order (permitted, resolvable, still refused)");
            SendFromPalette("workorder.approve", "WO-1041");
        }

        private void btnForeignTarget_Click(object sender, EventArgs e)
        {
            string foreign = FindForeignEntityId();
            _trace.Ui($"failure path · {foreign} is well-formed but belongs to another tenant");
            SendFromPalette("workorder.approve", foreign);
        }

        #endregion

        #region (e) The anti-pattern and its recovery

        private async void btnTrustClient_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult confirm = await MessageBox.ShowAsync(
                    "Run the endpoint that believes the payload?\n\nThe browser will claim role Admin and status Completed. " +
                    "The work order changes state without a single rule running — that is the point of the demo.",
                    "Trust the client (anti-pattern)", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirm != DialogResult.Yes) return;

                string entityId = (this.txtEntityId.Text ?? "").Trim().ToUpperInvariant();
                _trace.Ui("anti-pattern · asking the script to call RunTrustedClientCommand with a claimed role and status");
                this.commandPaletteHost.RunTrustedFromClient("workorder.approve", entityId, "Admin", "Completed");
                UpdatePaletteState();
            }
            catch (Exception ex)
            {
                HandleUnexpected(ex, "The anti-pattern demo could not be started.");
            }
        }

        /// <summary>Recovery: the audit log kept a before-snapshot, so the tampered change can be undone.</summary>
        private async void btnRevert_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult confirm = await MessageBox.ShowAsync(
                    "Restore the last tampered work order from the audit log's before-snapshot?",
                    "Revert", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm != DialogResult.Yes) return;

                _trace.Ui("recovery · revert the last tampered change");
                CommandResult result = _commands.RevertLastTamperedChange();
                RenderAudit();

                if (result.Succeeded)
                {
                    HideBanner();
                    SetStatus(result.Message, StatusKind.Ok);
                    AlertBox.Show(result.Message, MessageBoxIcon.Information,
                        alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
                }
                else
                {
                    SetStatus(result.Message, StatusKind.Warn);
                }
            }
            catch (Exception ex)
            {
                HandleUnexpected(ex, "The revert could not be completed.");
            }
        }

        #endregion

        #region (f) Buttons — capabilities and housekeeping

        private void btnForgeCapabilities_Click(object sender, EventArgs e)
        {
            _trace.Ui("failure path · ask the script to send a report claiming canApprove and a role");
            this.commandPaletteHost.SendForgedCapabilities();
            UpdatePaletteState();
        }

        private void btnRecollect_Click(object sender, EventArgs e)
        {
            _trace.Ui("Call(\"paletteCollect\") — re-run feature detection in the browser");
            _capabilities.Reset();
            this.commandPaletteHost.CollectCapabilities();
            UpdatePaletteState();
        }

        private void btnClearTrace_Click(object sender, EventArgs e)
        {
            _trace.Clear();
            HideBanner();
            SetStatus("Trace cleared. The audit log is append-only and is not cleared.", StatusKind.Ok);
        }

        private void cboUser_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loadingCombos) return;
            AppUser user = AppUser.Directory[Math.Max(0, this.cboUser.SelectedIndex)];
            _session.SignInAs(user.UserName);
            _trace.Security($"session identity is now {user.UserName} ({user.Role}) — the payload has no say in this");
            ShowIdentity();
            SetStatus($"Signed in as {user.UserName} ({user.Role}). Try \"Run approve\" as ben.tech.", StatusKind.Ok);
        }

        private void cboTenant_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loadingCombos) return;
            Tenant tenant = Tenant.All[Math.Max(0, this.cboTenant.SelectedIndex)];
            _session.SwitchTenant(tenant.Id);
            _trace.Security($"session tenant is now {tenant.Id} — every entity id is resolved inside it");
            ShowIdentity();
            SetStatus($"Tenant {tenant.Id}. Ids from other tenants now answer INVALID_TARGET.", StatusKind.Ok);
        }

        #endregion

        #region Rendering helpers (thin, no decisions)

        private enum StatusKind { Ok, Warn, Error }

        /// <summary>One place the palette is asked to run something, so every path is traceable.</summary>
        private void SendFromPalette(string commandName, string entityId)
        {
            _trace.Ui($"palette → {commandName} {(string.IsNullOrEmpty(entityId) ? "(no entity)" : entityId)}");
            this.commandPaletteHost.RunFromClient(commandName, entityId);
            UpdatePaletteState();
        }

        private void Render(ClientCommandResult result, bool antiPattern = false)
        {
            RenderAudit();

            if (result.Succeeded && antiPattern)
            {
                ShowBanner("⚠ The browser just changed a work order's state. No catalogue, no session role, no state rule ran — press \"Revert tampered\".");
                SetStatus($"{result.Code} · {result.Message}", StatusKind.Warn);
                return;
            }

            if (result.Succeeded)
            {
                HideBanner();
                SetStatus($"{result.Code} · {result.Message} · correlation {result.CorrelationId}", StatusKind.Ok);
                AlertBox.Show(result.Message, MessageBoxIcon.Information,
                    alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
                return;
            }

            ShowBanner($"{result.Message}  ({result.Code} · checked server-side in ClientCommandService · the command never ran)");
            SetStatus($"{result.Code} · correlation {result.CorrelationId} · audited",
                result.Code == ResultCodes.ServerError ? StatusKind.Error : StatusKind.Warn);
        }

        private void RenderAudit()
        {
            this.lstAudit.BeginUpdate();
            try
            {
                this.lstAudit.Items.Clear();
                foreach (AuditEntry entry in _audit.Entries)
                {
                    string mark = entry.Reverted ? "↩" : " ";
                    this.lstAudit.Items.Add(
                        $"{entry.Utc:HH:mm:ss}Z {mark}{entry.Outcome.ToString().ToUpperInvariant(),-9} {entry.UserName,-10} {entry.TenantId,-9} {entry.CorrelationId} {entry.CommandName} {(string.IsNullOrEmpty(entry.EntityId) ? "—" : entry.EntityId)}");
                }
            }
            finally
            {
                this.lstAudit.EndUpdate();
            }
            if (this.lstAudit.Items.Count > 0)
                this.lstAudit.SelectedIndex = this.lstAudit.Items.Count - 1;
        }

        private void OnTraceEntry(string line)
        {
            if (line == null)
            {
                this.lstTrace.Items.Clear();
                return;
            }
            this.lstTrace.Items.Add(line);
            this.lstTrace.SelectedIndex = this.lstTrace.Items.Count - 1;
        }

        private void ShowIdentity()
        {
            this.lblRole.Text = $"role {_session.Role} (from the session)";
            this.lblScreenName.Text = $"EnterpriseOps — Command Center";
            this.lblCorrelation.Text = $"{_session.UserName} · {_session.TenantId} · session {_session.CorrelationId}";
        }

        private void UpdatePaletteState()
        {
            this.lblPaletteState.Text = this.commandPaletteHost.PaletteReady
                ? $"host: ready · {this.commandPaletteHost.DeferredCalls} deferred"
                : $"host: not created yet · {this.commandPaletteHost.DeferredCalls} deferred";
        }

        private void SetStatus(string text, StatusKind kind)
        {
            this.lblStatus.Text = text;
            this.lblStatus.ForeColor =
                kind == StatusKind.Ok ? System.Drawing.Color.FromArgb(31, 157, 87) :
                kind == StatusKind.Warn ? System.Drawing.Color.FromArgb(232, 161, 60) :
                System.Drawing.Color.FromArgb(224, 86, 59);
        }

        private void ShowBanner(string text)
        {
            this.lblBanner.Text = text;
            this.lblBanner.Visible = true;
        }

        private void HideBanner()
        {
            this.lblBanner.Visible = false;
        }

        private void HandleUnexpected(Exception ex, string userMessage)
        {
            _trace.Ui($"unhandled {ex.GetType().Name} in the UI — {Safe(ex.Message, 100)}");
            SetStatus(userMessage, StatusKind.Error);
            AlertBox.Show(userMessage, MessageBoxIcon.Error,
                alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        /// <summary>A well-formed id belonging to a different tenant, for the INVALID_TARGET path.</summary>
        private string FindForeignEntityId()
        {
            Tenant other = Tenant.All.FirstOrDefault(t => t.Id != _session.TenantId) ?? Tenant.All[1];
            IReadOnlyList<WorkQueueRow> rows = _workOrders.Queue(other.Id, 1);
            return rows.Count > 0 ? rows[0].EntityId : "WO-9999";
        }

        /// <summary>
        /// Everything that arrived from the browser is bounded and stripped before it is printed:
        /// the trace is a UI control, and untrusted text should never decide how long a line is.
        /// </summary>
        private static string Safe(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return "";
            string clean = new string(value.Where(c => !char.IsControl(c)).ToArray());
            return clean.Length <= maxLength ? clean : clean.Substring(0, maxLength) + "…";
        }

        #endregion
    }
}
