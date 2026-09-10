using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Wisej.Core;
using Wisej.Web;

namespace EnterpriseOps.Interop
{
    #region Event args (the small, named payloads the client is allowed to send)

    public sealed class PaletteReadyEventArgs : EventArgs
    {
        public PaletteReadyEventArgs(string hotkey, string contractVersion) { Hotkey = hotkey; ContractVersion = contractVersion; }
        public string Hotkey { get; }
        public string ContractVersion { get; }
    }

    public sealed class PaletteCommandEventArgs : EventArgs
    {
        public PaletteCommandEventArgs(string commandName, string entityId, string code, int elapsedMs)
        {
            CommandName = commandName; EntityId = entityId; Code = code; ElapsedMs = elapsedMs;
        }
        public string CommandName { get; }
        public string EntityId { get; }
        public string Code { get; }
        public int ElapsedMs { get; }
    }

    public sealed class CapabilityReportEventArgs : EventArgs
    {
        public CapabilityReportEventArgs(string report, bool forged) { Report = report; Forged = forged; }
        /// <summary>The raw <c>key=value;key=value</c> string. Untrusted: the service decides what to keep.</summary>
        public string Report { get; }
        /// <summary>True when the demo button asked the script to add keys the server does not know.</summary>
        public bool Forged { get; }
    }

    public sealed class PaletteErrorEventArgs : EventArgs
    {
        public PaletteErrorEventArgs(string phase, string message) { Phase = phase; Message = message; }
        public string Phase { get; }
        public string Message { get; }
    }

    #endregion

    /// <summary>
    /// The host control for the browser-side command palette (the walkthrough's
    /// <c>commandPaletteHost1</c> on <c>CommandCenterShell</c>).
    ///
    /// It is a <see cref="Wisej.Web.Widget"/> rather than a plain UserControl because that is the
    /// verified way to ship page-level JavaScript in Wisej.NET: a <b>Package</b> loads the palette
    /// script from the project folder (<c>Interop/palette.client.js</c> → <c>/Interop/palette.client.js</c>)
    /// and an <b>InitScript</b> (an embedded resource) wires the wrapper to it.
    ///
    /// LIFECYCLE — the review question "what happens before the target widget exists?":
    ///   • the script attaches its document keydown handler inside <c>init()</c>, i.e. after the
    ///     client widget has been created — never on page load;
    ///   • every server → client call here goes through <see cref="Send"/>, which checks
    ///     <see cref="Widget.IsLoaded"/> and QUEUES the call when the widget does not exist yet;
    ///   • the queue is flushed when the client raises <c>paletteReady</c>;
    ///   • the script removes the listener and its overlay element in <c>dispose()</c> — every
    ///     attach has a matching detach, so a closed screen leaves nothing behind on document.
    /// </summary>
    [ToolboxItem(true)]
    [DefaultEvent("PaletteReady")]
    [Description("Hosts the Ctrl+K command palette and the browser capability probe, and calls back through the interop contract.")]
    public class CommandPaletteHost : Widget
    {
        private readonly List<Action> _pending = new List<Action>();
        private IClientCommandService _commands;
        private string _hotkey = "Ctrl+K";
        private string _targetEntityId = "";

        public CommandPaletteHost()
        {
            // The palette "library": a self-contained script served from the project folder.
            this.Packages.Add(new Package { Name = "enterpriseops-palette", Source = "Interop/palette.client.js" });

            // The adapter that binds the library to this widget (embedded resource — see the csproj).
            this.InitScript = GetResourceString("EnterpriseOps.Interop.command-palette-host.js");

            // The only events the browser may raise. Anything else is not part of the contract.
            this.WiredEvents = new[] { "paletteReady", "paletteOpened", "paletteClosed", "commandRun", "capabilities", "error" };

            this.Size = new System.Drawing.Size(620, 230);
            PushOptions();
        }

        #region Wiring

        /// <summary>
        /// The page hands the widget the boundary owner after construction (the designer needs a
        /// parameterless constructor; the walkthrough sketch shows the same dependency as a ctor
        /// argument). Nothing else about the service is exposed to the client.
        /// </summary>
        public void Attach(IClientCommandService commands)
        {
            _commands = commands;
        }

        [DefaultValue("Ctrl+K")]
        [Description("The browser-side shortcut that opens the palette. Cosmetic: it grants nothing.")]
        public string Hotkey
        {
            get => _hotkey;
            set
            {
                value = string.IsNullOrEmpty(value) ? "Ctrl+K" : value;
                if (_hotkey == value) return;
                _hotkey = value;
                dynamic options = this.Options;
                options.hotkey = value;               // first-level change → update(options, old) on the client
            }
        }

        /// <summary>
        /// The work order the palette acts on, in WIRE form (WO-1040). The server pushes it, the
        /// palette displays it and echoes it back — and the server still re-resolves it inside the
        /// session tenant before anything runs, because the echo is a suggestion like every other field.
        /// </summary>
        [DefaultValue("")]
        [Description("The work order the palette targets, in wire form (WO-1040).")]
        public string TargetEntityId
        {
            get => _targetEntityId;
            set
            {
                value = value ?? "";
                if (_targetEntityId == value) return;
                _targetEntityId = value;
                dynamic options = this.Options;
                options.entityId = value;
            }
        }

        /// <summary>True once the client wrapper has run init() and reported back.</summary>
        [Browsable(false)]
        public bool PaletteReady { get; private set; }

        [Browsable(false)] public int DeferredCalls { get; private set; }

        #endregion

        #region Events surfaced to the page

        public event EventHandler<PaletteReadyEventArgs> Ready;
        public event EventHandler<PaletteCommandEventArgs> CommandRun;
        public event EventHandler<CapabilityReportEventArgs> CapabilitiesReported;
        public event EventHandler<PaletteErrorEventArgs> PaletteError;

        /// <summary>Raised for the opened/closed telemetry, so the page can trace the browser's view.</summary>
        public event EventHandler<PaletteErrorEventArgs> PaletteState;

        #endregion

        #region The widget-level [WebMethod] (RegisterWebMethods)

        /// <summary>
        /// Wisej discovers [WebMethod] on top-level containers automatically; a child control
        /// registers its own by calling RegisterWebMethods from OnWebRender. The render then carries
        /// <c>config.webMethods</c> and the client gets <c>this.GetCommandCatalog(args…, callback)</c>
        /// and <c>this.GetCommandCatalogAsync(args…)</c> on the wrapper.
        /// </summary>
        protected override void OnWebRender(dynamic config)
        {
            base.OnWebRender((object)config);
            RegisterWebMethods((object)config);
        }

        /// <summary>
        /// The catalogue endpoint the palette calls while the user types. It returns only what the
        /// palette needs to draw a row, and an <c>Allowed</c> flag that is a HINT — the same
        /// permission is checked again, from the session, when the command is actually run.
        ///
        /// Never returns null and never throws: a palette that cannot list is still a working screen.
        /// </summary>
        [WebMethod]
        public object GetCommandCatalog(string query)
        {
            if (_commands == null)
                return new object[0];

            try
            {
                IReadOnlyList<CommandDescriptor> visible = _commands.GetVisibleCatalog(query ?? "");
                var service = _commands as Services.ClientCommandService;
                return visible.Select(c => new
                {
                    Id = c.Id,
                    Title = c.Title,
                    Shortcut = c.Shortcut,
                    RequiresEntity = c.RequiresEntity,
                    Allowed = service == null || service.MayRun(c),
                }).ToArray();
            }
            catch (Exception)
            {
                return new object[0];
            }
        }

        #endregion

        #region Server → client callbacks (all lifecycle-guarded)

        /// <summary>Opens the palette from the server (the toolbar button, or a keyboard-less device).</summary>
        public void OpenPalette() => Send("paletteOpen", () => this.Call("paletteOpen"));

        public void ClosePalette() => Send("paletteClose", () => this.Call("paletteClose"));

        /// <summary>
        /// Asks the SCRIPT to run a command, so the round trip really goes browser → WebMethod.
        /// Used by the bottom bar, including the deliberately malformed payloads.
        /// </summary>
        public void RunFromClient(string commandName, string entityId)
            => Send($"paletteRun({commandName}, {(string.IsNullOrEmpty(entityId) ? "—" : entityId)})",
                    () => this.Call("paletteRun", commandName ?? "", entityId ?? ""));

        /// <summary>Same, but the script sends a correlation id that breaks the contract's shape.</summary>
        public void RunWithBadCorrelation(string commandName, string entityId)
            => Send("paletteRunBadCorrelation", () => this.Call("paletteRunRaw", commandName ?? "", entityId ?? "", "not-a-corr-id"));

        /// <summary>
        /// ⚠ Asks the script to call the trusting server method with a claimed role and a claimed
        /// new status in the payload. Only the anti-pattern demo uses it.
        /// </summary>
        public void RunTrustedFromClient(string commandName, string entityId, string claimedRole, string claimedStatus)
            => Send("paletteRunTrusted ⚠",
                    () => this.Call("paletteRunTrusted", commandName ?? "", entityId ?? "", claimedRole ?? "", claimedStatus ?? ""));

        /// <summary>Re-runs feature detection in the browser and re-sends the report.</summary>
        public void CollectCapabilities() => Send("paletteCollect", () => this.Call("paletteCollect"));

        /// <summary>Sends a report with keys the server never published, to show what happens to them.</summary>
        public void SendForgedCapabilities() => Send("paletteForge", () => this.Call("paletteForge"));

        /// <summary>Shows the last server answer in the palette's resting card.</summary>
        public void ShowResult(string code, string message)
            => Send("paletteShowResult", () => this.Call("paletteShowResult", code ?? "", message ?? ""));

        /// <summary>
        /// The lifecycle gate. Before the client widget exists there is nothing to call, so the
        /// call is remembered instead of thrown away — and the page can see how many were deferred.
        /// </summary>
        private void Send(string label, Action call)
        {
            if (this.IsDisposed) return;

            if (!PaletteReady || !this.IsLoaded)
            {
                DeferredCalls++;
                _pending.Add(call);
                Deferred?.Invoke(this, new PaletteErrorEventArgs("deferred", label));
                return;
            }

            try { call(); }
            catch (ObjectDisposedException) { }
        }

        /// <summary>Raised when a server → client call arrived before the client widget existed.</summary>
        public event EventHandler<PaletteErrorEventArgs> Deferred;

        private void FlushPending()
        {
            if (_pending.Count == 0) return;
            List<Action> queued = _pending.ToList();
            _pending.Clear();
            foreach (Action call in queued)
            {
                try { call(); }
                catch (ObjectDisposedException) { }
            }
        }

        #endregion

        #region Client → server events

        protected override void OnWidgetEvent(WidgetEventArgs e)
        {
            dynamic data = e.Data;

            switch (e.Type)
            {
                case "paletteReady":
                    PaletteReady = true;
                    Ready?.Invoke(this, new PaletteReadyEventArgs(Str(data?.hotkey), Str(data?.contractVersion)));
                    FlushPending();
                    break;

                case "paletteOpened":
                    PaletteState?.Invoke(this, new PaletteErrorEventArgs("opened", Str(data?.via)));
                    break;

                case "paletteClosed":
                    PaletteState?.Invoke(this, new PaletteErrorEventArgs("closed", Str(data?.via)));
                    break;

                case "commandRun":
                    CommandRun?.Invoke(this, new PaletteCommandEventArgs(
                        Str(data?.command), Str(data?.entityId), Str(data?.code), Int(data?.elapsed)));
                    break;

                case "capabilities":
                    CapabilitiesReported?.Invoke(this, new CapabilityReportEventArgs(Str(data?.report), Bool(data?.forged)));
                    break;

                case "error":
                    PaletteError?.Invoke(this, new PaletteErrorEventArgs(Str(data?.phase), Str(data?.message)));
                    break;

                default:
                    base.OnWidgetEvent(e);
                    break;
            }
        }

        #endregion

        private void PushOptions()
        {
            dynamic options = this.Options;
            options.hotkey = _hotkey;
            options.placeholder = "Type a command…";
            options.contractVersion = InteropContract.Version;
            options.entityId = _targetEntityId;
        }

        private static string Str(object value)
            => value == null ? "" : Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture);

        private static int Int(object value)
        {
            try { return value == null ? 0 : Convert.ToInt32(value, System.Globalization.CultureInfo.InvariantCulture); }
            catch (Exception) { return 0; }
        }

        private static bool Bool(object value)
        {
            try { return value != null && Convert.ToBoolean(value, System.Globalization.CultureInfo.InvariantCulture); }
            catch (Exception) { return false; }
        }
    }
}
