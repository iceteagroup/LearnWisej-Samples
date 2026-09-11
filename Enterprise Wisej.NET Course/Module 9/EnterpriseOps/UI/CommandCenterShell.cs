using System;
using System.Collections.Generic;
using System.Linq;
using EnterpriseOps.Data;
using EnterpriseOps.Interop;
using EnterpriseOps.Security;
using EnterpriseOps.Services;
using Wisej.Core;
using Wisej.Web;

namespace EnterpriseOps.UI
{
    /// <summary>
    /// EnterpriseOps — Command Center.
    ///
    /// The command palette host (the browser-side script, Ctrl+K) and the browser capability panel
    /// (what detection found, and the fallback the SERVER chose), with a failure banner and a
    /// status bar.
    ///
    /// This is a top-level Page (Application.MainPage), which is why the [WebMethod] below is
    /// registered automatically and reachable from JavaScript as App.MainPage.RunClientCommand.
    ///
    /// The handlers only ask a service and render. Every decision — catalogue, arity, permission,
    /// target, state — lives in <see cref="ClientCommandService"/>.
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

            // The widget gets the boundary owner — and nothing else about the server.
            this.commandPaletteHost.Attach(_commands);
            this.commandPaletteHost.CapabilitiesReported += commandPaletteHost_CapabilitiesReported;
            this.commandPaletteHost.PaletteError += commandPaletteHost_PaletteError;
        }

        private void CommandCenterShell_Load(object sender, EventArgs e)
        {
            // The palette acts on the first work order of the session's queue.
            IReadOnlyList<WorkQueueRow> queue = _workOrders.Queue(_session.TenantId, 1);
            this.commandPaletteHost.TargetEntityId = queue.Count > 0 ? queue[0].EntityId : "";

            this.capabilityPanel.Render(null);
        }

        #region The server callback method JavaScript may call

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
                _trace.Client($"App.MainPage.RunClientCommand(\"{Safe(commandName, 40)}\", \"{Safe(entityId, 16)}\", \"{Safe(correlationId, 12)}\")");

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

        #endregion

        #region Client → server events raised by the host widget

        private void commandPaletteHost_CapabilitiesReported(object sender, CapabilityReportEventArgs e)
        {
            IReadOnlyList<CapabilityReading> readings = _capabilities.Accept(e.Report);
            this.capabilityPanel.Render(readings);
        }

        private void commandPaletteHost_PaletteError(object sender, PaletteErrorEventArgs e)
        {
            _trace.Client($"widget error · {e.Phase} · {Safe(e.Message, 120)}");
            ShowBanner($"The palette script reported a problem in {e.Phase}.");
            SetStatus("The palette script reported an error — the server state is unchanged.");
        }

        #endregion

        #region Rendering helpers

        private void Render(ClientCommandResult result)
        {
            if (result.Succeeded)
            {
                HideBanner();
                SetStatus($"{result.Code} · {result.Message} · correlation {result.CorrelationId}");
                AlertBox.Show(result.Message, MessageBoxIcon.Information,
                    alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
                return;
            }

            ShowBanner($"{result.Message}  ({result.Code} · checked server-side in ClientCommandService · the command never ran)");
            SetStatus($"CommandResult.Fail — {result.Code} · audited · correlation {result.CorrelationId}");
        }

        private void SetStatus(string text)
        {
            this.lblStatus.Text = text;
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

        /// <summary>
        /// Everything that arrived from the browser is bounded and stripped before it is logged or
        /// audited: untrusted text should never decide how long a line is.
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
