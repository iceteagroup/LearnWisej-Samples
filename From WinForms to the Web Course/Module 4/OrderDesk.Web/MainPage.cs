using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using OrderDesk.Domain;
using OrderDesk.Migration;
using OrderDesk.Services;
using OrderDesk.Views;
using Wisej.Web;

namespace OrderDesk
{
    /// <summary>
    /// Module 4 · Sessions, statics and multi-user safety.
    ///
    /// Left, top:    the static-state audit of LegacyOrderDesk — every static/singleton found,
    ///               classified, with the verdict filter (lab steps 1–2).
    /// Left, bottom: the Orders screen running on two stores: the legacy statics (one slot for
    ///               the whole server) or the typed UserContext in Application.Session. Sign in
    ///               as kelly here and as sam in a second session, then Re-read (lab steps 3–5).
    /// Right:        the migration log, and the settings card: HKCU on the server (the failure),
    ///               the per-user profile store and browser storage (the replacements), plus the
    ///               logout / timeout cleanup routine (lab step 6).
    /// </summary>
    public partial class MainPage : Page
    {
        /// <summary>Which store the Orders screen reads and writes.</summary>
        private enum StateMode { LegacyStatics, SessionContext }

        private const int CountdownSeconds = 5;

        private readonly OrderService _orderService = new OrderService();
        private readonly CustomerService _customerService = new CustomerService();
        private readonly IList<Customer> _customers;
        private readonly OrderStatus[] _statuses = (OrderStatus[])Enum.GetValues(typeof(OrderStatus));

        // What THIS page last wrote to each store: the reference the corruption check compares
        // the store against. The page instance is per session, so this is per session too.
        private readonly Dictionary<StateMode, UserSessionContext> _lastWritten = new Dictionary<StateMode, UserSessionContext>();

        private StaticVerdict? _verdictFilter;
        private StateMode _mode = StateMode.SessionContext;
        private bool _binding;          // true while code fills the combos — their events must not write back
        private bool _loading;          // true while MainPage_Load sets the initial mode — traced as the server's doing, not a click
        private int _countdown;

        private string _registryLine = "(not tried)";
        private string _profileLine = "(not saved yet)";
        private string _browserLine = "(not saved yet)";

        public MainPage()
        {
            InitializeComponent();
            _customers = _customerService.GetCustomers();

            // Session lifecycle hook — trace only. A static event → released in Dispose (Designer.cs →
            // DetachApplicationEvents). The real cleanup on Application.ApplicationExit is deliberately NOT
            // subscribed by the page: Wisej.NET may dispose the page before that event fires, and a handler
            // detached in Dispose would then never run. It lives in Program.Main for the session's lifetime.
            Application.SessionTimeout += Application_SessionTimeout;
        }

        private void MainPage_Load(object sender, EventArgs e)
        {
            trace.Add(TraceKind.Server, "startup", "Default.json → OrderDesk.Program.Main → Application.MainPage = new MainPage()");
            trace.Add(TraceKind.Server, "session", $"browser session {Short(Application.SessionId)} · {Application.SessionCount} session(s) share this process");
            trace.Add(TraceKind.Server, "lifecycle hooks", "Application.SessionTimeout subscribed by the page, trace only (released in Dispose) · Application.ApplicationExit cleanup subscribed in Program.Main for the session's lifetime");

            BindAudit();
            FillCombos();
            _loading = true;                    // the initial mode is the server's choice, not a click
            radioContext.Checked = true;        // → radioContext_CheckedChanged → SetMode(SessionContext)
            _loading = false;
            RefreshSettingsValues();
            Ui.SetStatus(labelStatus, "audit loaded · not signed in", Ui.Ok);
        }

        #region Card A · Static-state audit (lab steps 1 + 2)

        private void BindAudit()
        {
            var rows = StaticStateAudit.Items
                .Where(i => _verdictFilter == null || i.Verdict == _verdictFilter.Value)
                .ToList();

            gridAudit.Rows.Clear();
            foreach (var item in rows)
            {
                int index = gridAudit.Rows.Add(item.Member, item.DeclaredIn, item.KindText, item.VerdictText);
                gridAudit.Rows[index].Tag = item;
                gridAudit.Rows[index].Cells[3].Style.ForeColor = VerdictColor(item.Verdict);
                gridAudit.Rows[index].Cells[3].Style.Font = Ui.SmallBold;
            }

            int keep = StaticStateAudit.Items.Count(i => i.Verdict == StaticVerdict.KeepStatic);
            labelAuditCount.Text = _verdictFilter == null
                ? $"{rows.Count} statics · {keep} may stay"
                : $"{rows.Count} of {StaticStateAudit.Items.Count} statics";

            if (gridAudit.Rows.Count > 0)
            {
                gridAudit.Rows[0].Selected = true;
                ShowAuditDetail(gridAudit.Rows[0].Tag as StaticStateItem);
            }
            else
            {
                ShowAuditDetail(null);
            }
        }

        private static System.Drawing.Color VerdictColor(StaticVerdict verdict) => verdict switch
        {
            StaticVerdict.KeepStatic => Ui.Ok,
            StaticVerdict.MoveToSession => Ui.Accent,
            StaticVerdict.MoveToProfileStore => Ui.Purple,
            StaticVerdict.MoveToBrowserStorage => Ui.Warn,
            _ => Ui.Muted
        };

        private void gridAudit_SelectionChanged(object sender, EventArgs e)
        {
            ShowAuditDetail(gridAudit.CurrentRow?.Tag as StaticStateItem);
        }

        private void ShowAuditDetail(StaticStateItem item)
        {
            if (item == null)
            {
                labelAuditDetail.Text = "";
                return;
            }
            labelAuditDetail.Text = $"{item.Member} · {item.KindText} → {item.VerdictText}\nWhy: {item.Why}\nReplacement: {item.Replacement}";
        }

        private void buttonAuditAll_Click(object sender, EventArgs e) => ApplyVerdictFilter(null, "all");
        private void buttonAuditKeep_Click(object sender, EventArgs e) => ApplyVerdictFilter(StaticVerdict.KeepStatic, "keep static");
        private void buttonAuditSession_Click(object sender, EventArgs e) => ApplyVerdictFilter(StaticVerdict.MoveToSession, "move to session");
        private void buttonAuditProfile_Click(object sender, EventArgs e) => ApplyVerdictFilter(StaticVerdict.MoveToProfileStore, "move to profile store");
        private void buttonAuditBrowser_Click(object sender, EventArgs e) => ApplyVerdictFilter(StaticVerdict.MoveToBrowserStorage, "move to browser storage");

        private void ApplyVerdictFilter(StaticVerdict? verdict, string name)
        {
            _verdictFilter = verdict;
            BindAudit();
            trace.Add(TraceKind.FromClient, "audit.filter", $"verdict = {name} → {gridAudit.Rows.Count} rows");
        }

        #endregion

        #region Card B · Two-session isolation test (lab steps 3 + 4 + 5)

        // ── the two stores behind the same values ────────────────────────────────────────

        private string StoreName => _mode == StateMode.LegacyStatics ? "Legacy.AppState (static)" : "UserContext (session)";

        private UserSessionContext LastWritten
        {
            get => _lastWritten.TryGetValue(_mode, out var value) ? value : null;
            set => _lastWritten[_mode] = value;
        }

        /// <summary>The legacy statics as a snapshot — whatever the LAST session wrote is what every session reads.</summary>
        private static UserSessionContext LegacySnapshot() => new UserSessionContext
        {
            UserName = Legacy.AppState.CurrentUser,                    // ✕ one slot for the whole server
            Company = Legacy.AppState.CurrentCompany,
            CurrentCustomerId = Legacy.AppState.CurrentCustomer?.Id,
            CurrentFilter = Legacy.AppState.CurrentFilter,
            LastSearch = Legacy.AppState.LastSearch
        };

        /// <summary>What the Orders screen sees right now through the active store.</summary>
        private UserSessionContext ReadActiveStore() =>
            _mode == StateMode.LegacyStatics ? LegacySnapshot() : SessionContext.Current;   // ✓ per browser session

        private static UserSessionContext Copy(UserSessionContext source) => new UserSessionContext
        {
            UserName = source.UserName, DisplayName = source.DisplayName, Company = source.Company,
            CurrentCustomerId = source.CurrentCustomerId, CurrentFilter = source.CurrentFilter,
            LastSearch = source.LastSearch, Culture = source.Culture, SignedInAt = source.SignedInAt
        };

        private static bool SameState(UserSessionContext a, UserSessionContext b) =>
            a.UserName == b.UserName && a.Company == b.Company && a.CurrentCustomerId == b.CurrentCustomerId && a.CurrentFilter == b.CurrentFilter;

        private void WriteSignIn(string user, string company, int customerId, OrderStatus filter)
        {
            if (_mode == StateMode.LegacyStatics)
            {
                // ✕ the desktop way — exactly what LoginForm.okButton_Click and OrdersForm do
                Legacy.AppState.CurrentUser = user;
                Legacy.AppState.CurrentCompany = company;
                Legacy.AppState.CurrentCustomer = _customerService.Find(customerId);
                Legacy.AppState.CurrentFilter = filter;
                Legacy.AppState.LastSearch = null;
                return;
            }

            // ✓ the web way — the caller does not know (or care) that this lives in Application.Session
            var context = SessionContext.Current;
            context.UserName = user;
            context.DisplayName = char.ToUpperInvariant(user[0]) + user.Substring(1);
            context.Company = company;
            context.CurrentCustomerId = customerId;
            context.CurrentFilter = filter;
            context.LastSearch = null;
            context.Culture = Application.CurrentCulture?.Name;
            context.SignedInAt = DateTime.Now;
        }

        private void WriteFilter(OrderStatus? filter)
        {
            if (_mode == StateMode.LegacyStatics) Legacy.AppState.CurrentFilter = filter;        // ✕
            else SessionContext.Current.CurrentFilter = filter;                                   // ✓
        }

        private void WriteCustomer(int? customerId)
        {
            if (_mode == StateMode.LegacyStatics) Legacy.AppState.CurrentCustomer = customerId.HasValue ? _customerService.Find(customerId.Value) : null;   // ✕
            else SessionContext.Current.CurrentCustomerId = customerId;                                                                                     // ✓
        }

        // ── mode toggle ──────────────────────────────────────────────────────────────────

        private void radioLegacy_CheckedChanged(object sender, EventArgs e)
        {
            if (radioLegacy.Checked) SetMode(StateMode.LegacyStatics);
        }

        private void radioContext_CheckedChanged(object sender, EventArgs e)
        {
            if (radioContext.Checked) SetMode(StateMode.SessionContext);
        }

        private void SetMode(StateMode mode)
        {
            _mode = mode;
            // a radio click arrives from the browser; the initial mode in MainPage_Load is set by server code
            trace.Add(_loading ? TraceKind.Server : TraceKind.FromClient, "mode", _mode == StateMode.LegacyStatics
                ? "Legacy statics — the screen reads/writes Legacy.AppState.*  (✕ one slot for the server)"
                : "UserContext — the screen reads/writes SessionContext.Current  (✓ one context per browser session)");
            SyncCombosFromStore();
            BindOrders();
            RefreshStores(announce: false);
            Ui.HideBanner(labelBanner);
        }

        // ── combos: the filter and the current customer ──────────────────────────────────

        private void FillCombos()
        {
            _binding = true;
            comboFilter.Items.Clear();
            comboFilter.Items.Add("All");
            foreach (var status in _statuses) comboFilter.Items.Add(status.ToString());
            comboCustomer.Items.Clear();
            comboCustomer.Items.Add("All customers");
            foreach (var customer in _customers) comboCustomer.Items.Add(customer.Name);
            comboFilter.SelectedIndex = 0;
            comboCustomer.SelectedIndex = 0;
            comboDensity.Items.Clear();
            comboDensity.Items.Add("Comfortable");
            comboDensity.Items.Add("Compact");
            comboDensity.Items.Add("Dense");
            comboDensity.SelectedIndex = 0;
            _binding = false;
        }

        private OrderStatus? SelectedFilter => comboFilter.SelectedIndex <= 0 ? (OrderStatus?)null : _statuses[comboFilter.SelectedIndex - 1];
        private int? SelectedCustomerId => comboCustomer.SelectedIndex <= 0 ? (int?)null : _customers[comboCustomer.SelectedIndex - 1].Id;

        /// <summary>Shows the active store in the combos without writing anything back.</summary>
        private void SyncCombosFromStore()
        {
            var state = ReadActiveStore();
            _binding = true;
            comboFilter.SelectedIndex = state.CurrentFilter.HasValue ? Array.IndexOf(_statuses, state.CurrentFilter.Value) + 1 : 0;
            int customerIndex = state.CurrentCustomerId.HasValue ? _customers.ToList().FindIndex(c => c.Id == state.CurrentCustomerId.Value) : -1;
            comboCustomer.SelectedIndex = customerIndex + 1;
            _binding = false;
        }

        private void comboFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_binding) return;
            var filter = SelectedFilter;
            WriteFilter(filter);
            if (LastWritten != null) LastWritten.CurrentFilter = filter;
            trace.Add(TraceKind.FromClient, "filter", $"{Short(Application.SessionId)} sets filter = {FilterText(filter)} → {StoreName}");
            BindOrders();
            RefreshStores(announce: false);
        }

        private void comboCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_binding) return;
            var customerId = SelectedCustomerId;
            WriteCustomer(customerId);
            if (LastWritten != null) LastWritten.CurrentCustomerId = customerId;
            trace.Add(TraceKind.FromClient, "customer", $"{Short(Application.SessionId)} sets current customer = {CustomerName(customerId)} → {StoreName}");
            BindOrders();
            RefreshStores(announce: false);
        }

        // ── the Orders grid, filtered by the active store ────────────────────────────────

        private void BindOrders()
        {
            var state = ReadActiveStore();
            var orders = _orderService.Search(new OrderFilter { Status = state.CurrentFilter, Text = state.LastSearch });   // ✓ business logic reused as-is

            gridOrders.Rows.Clear();
            foreach (var order in orders)
            {
                int index = gridOrders.Rows.Add(order.Id, order.CustomerName, order.Total, order.Status.ToString());
                gridOrders.Rows[index].Tag = order;
                gridOrders.Rows[index].Cells[3].Style.ForeColor = StatusColor(order.Status);
                if (state.CurrentCustomerId.HasValue && order.CustomerId == state.CurrentCustomerId.Value)
                {
                    // the current customer is highlighted, the way LegacyOrderDesk preselects it in dialogs
                    gridOrders.Rows[index].Cells[1].Style.ForeColor = Ui.Accent;
                    gridOrders.Rows[index].Cells[1].Style.Font = Ui.SmallBold;
                }
            }
            labelOrdersCount.Text = $"{orders.Count} of {_orderService.GetOrders().Count} orders · filter {FilterText(state.CurrentFilter)} · {CustomerName(state.CurrentCustomerId)}";
        }

        private static System.Drawing.Color StatusColor(OrderStatus status) => status switch
        {
            OrderStatus.Shipped => Ui.Ok,
            OrderStatus.Invoiced => Ui.Purple,
            OrderStatus.Hold => Ui.Warn,
            _ => Ui.Accent
        };

        // ── sign in / re-read / second session ───────────────────────────────────────────

        private void buttonSignInKelly_Click(object sender, EventArgs e) => SignIn("kelly", "Acme", 1, OrderStatus.Open);          // Northwind Traders
        private void buttonSignInSam_Click(object sender, EventArgs e) => SignIn("sam", "Globex", 3, OrderStatus.Invoiced);        // Fabrikam Inc

        private void SignIn(string user, string company, int customerId, OrderStatus filter)
        {
            trace.Add(TraceKind.FromClient, "sign in", $"{user} · {company} · {CustomerName(customerId)} · filter {filter}");
            WriteSignIn(user, company, customerId, filter);
            LastWritten = Copy(ReadActiveStore());

            string workspaceError = null;
            try
            {
                string workspace = SessionCleanup.CreateWorkspace(Application.SessionId, user);
                trace.Add(TraceKind.Server, "workspace", $"{Rel(workspace)} created — temp files this session must release on logout/timeout");
            }
            catch (Exception ex)
            {
                // directory/file writes under the storage root can fail (permissions, a read-only deploy, a full disk)
                trace.Add(TraceKind.Boundary, "SessionCleanup.CreateWorkspace", $"{ex.GetType().Name}: {ex.Message}");
                workspaceError = $"{ex.GetType().Name}: {ex.Message}";
            }

            if (_mode == StateMode.LegacyStatics)
                trace.Add(TraceKind.Server, "AppState.CurrentUser (static)", $"= {user}   ← one slot: EVERY session on the server now reads {user}");
            else
                trace.Add(TraceKind.Server, "UserContext.Current", $"→ session {Short(Application.SessionId)} · {SessionContext.Current.Describe(_customerService)} · culture {SessionContext.Current.Culture ?? "?"}   ← only this session");

            SyncCombosFromStore();
            BindOrders();
            RefreshStores(announce: true);
            Ui.SetStatus(labelStatus, $"signed in as {user} · {StoreName}", Ui.Ok);

            if (workspaceError != null)
            {
                // the store was written and the isolation check ran; only the server-side workspace failed — said last, so it stays visible
                Ui.ShowBanner(labelBanner,
                    $"✕ Workspace: {Rel(SessionCleanup.WorkspaceFor(Application.SessionId))} could not be created ({workspaceError}). The sign-in was recorded, but the temp folder the cleanup routine releases does not exist — check the storage root's permissions.",
                    Ui.BannerKind.Error);
                Ui.SetStatus(labelStatus, $"signed in as {user} · workspace not created", Ui.Error);
            }
        }

        private void buttonReread_Click(object sender, EventArgs e)
        {
            trace.Add(TraceKind.FromClient, "re-read state", $"{StoreName} — compare with what this page last wrote");
            SyncCombosFromStore();
            BindOrders();
            RefreshStores(announce: true);
        }

        private void buttonSecondSession_Click(object sender, EventArgs e)
        {
            trace.Add(TraceKind.ToClient, "Application.Navigate", "same URL, target _blank → a second browser session in this process");
            Application.Navigate(Application.Url, "_blank");
        }

        /// <summary>Shows both stores side by side and decides: corrupted (red) or isolated (green).</summary>
        private void RefreshStores(bool announce)
        {
            var staticState = LegacySnapshot();
            var context = SessionContext.Current;
            string sid = Short(Application.SessionId);
            var lastWritten = LastWritten;

            labelStores.Text =
                $"session {sid} · {Application.SessionCount} live · {(_mode == StateMode.LegacyStatics ? "Legacy statics" : "UserContext")}\n" +
                $"static AppState (one slot per server)\n  {staticState.Describe(_customerService)}\n" +
                $"UserContext.Current (session {sid})\n  {context.Describe(_customerService)}\n" +
                $"this page last wrote\n  {(lastWritten == null ? "(nothing yet)" : lastWritten.Describe(_customerService))}";

            if (lastWritten == null)
            {
                if (announce)
                    Ui.ShowBanner(labelBanner, "This session has not signed in through this store yet. Sign in here, open a second session, sign in as the other user there (or change its filter), then come back and Re-read.", Ui.BannerKind.Warn);
                return;
            }

            var active = ReadActiveStore();
            if (!SameState(active, lastWritten))
            {
                // Only the static store can get here: another session wrote the one slot this page is reading.
                Ui.ShowBanner(labelBanner,
                    $"✕ Static slot corrupted: this session ({sid}, {lastWritten.UserName}) last wrote \"{lastWritten.Describe(_customerService)}\" but AppState now holds \"{active.Describe(_customerService)}\" — another session overwrote the one slot the whole server shares, and the grid now shows the OTHER user's filter. Single-user testing never sees this: with one session the last writer is always you.",
                    Ui.BannerKind.Error);
                Ui.SetStatus(labelStatus, "static state overwritten by another session", Ui.Error);
                if (announce)
                    trace.Add(TraceKind.Server, "✕ corrupted", $"static filter = {FilterText(active.CurrentFilter)} ≠ this page wrote {FilterText(lastWritten.CurrentFilter)} — {lastWritten.UserName}'s selection silently replaced by {active.UserName ?? "another session"}");
                return;
            }

            if (_mode == StateMode.SessionContext)
            {
                bool staticDiffers = staticState.IsSignedIn && !SameState(staticState, context);
                Ui.ShowBanner(labelBanner,
                    $"✓ Isolated: UserContext.Current → session {sid} still says \"{context.Describe(_customerService)}\"" +
                    (staticDiffers
                        ? $" while the static slot meanwhile says \"{staticState.Describe(_customerService)}\" — another session's writes never reached this context."
                        : ". Open a second session, sign in as the other user and change its filter, then Re-read here: nothing changes."),
                    Ui.BannerKind.Ok);
                Ui.SetStatus(labelStatus, $"signed in as {context.UserName} · isolated per session", Ui.Ok);
                if (announce)
                    trace.Add(TraceKind.Server, "✓ isolated", $"filter unchanged = {FilterText(context.CurrentFilter)} (UserContext.Current → session {sid})");
                return;
            }

            Ui.ShowBanner(labelBanner,
                $"✓ Static slot and this page agree (\"{active.Describe(_customerService)}\") — for now. Open a second session, switch it to Legacy statics, sign in as the other user, then Re-read here.",
                Ui.BannerKind.Ok);
            Ui.SetStatus(labelStatus, $"signed in as {active.UserName} · static slot (single-user so far)", Ui.Ok);
            if (announce)
                trace.Add(TraceKind.Server, "static slot", $"= {active.Describe(_customerService)} — matches this page (no second session has written yet)");
        }

        #endregion

        #region Card C · Settings storage: registry (✕) → profile store (✓) → browser storage (✓)

        private string CurrentUserName => ReadActiveStore().UserName;
        private string SelectedDensity => comboDensity.SelectedItem as string ?? "Comfortable";

        private void RefreshSettingsValues()
        {
            labelSettingsValues.Text =
                $"registry (HKCU)   {_registryLine}\n" +
                $"profile store     {_profileLine}\n" +
                $"browser storage   {_browserLine}\n" +
                $"density to save   {SelectedDensity} · signed in as {CurrentUserName ?? "(nobody)"}";
        }

        private void buttonRegistry_Click(object sender, EventArgs e)
        {
            string density = SelectedDensity;
            trace.Add(TraceKind.FromClient, "settings.registry", $"GridDensity = {density} → HKCU\\Software\\LegacyOrderDesk");
            try
            {
                var settings = Legacy.RegistrySettings.Load();        // ✕ reads the server's registry
                settings.GridDensity = density;
                Legacy.RegistrySettings.Save(settings);               // ✕ writes the server's registry
                var back = Legacy.RegistrySettings.Load();

                string account = Environment.UserName;
                string machine = Environment.MachineName;
                trace.Add(TraceKind.Boundary, "RegistrySettings.Save", $"HKCU on {machine} = the hive of \"{account}\" (the account running Kestrel), not {CurrentUserName ?? "the visitor"}'s desktop");
                trace.Add(TraceKind.Server, "RegistrySettings.Load", $"GridDensity = {back.GridDensity} · ExportFolder = {back.ExportFolder} — the same answer for every session");

                _registryLine = $"HKCU of {account}@{machine}: GridDensity = {back.GridDensity} · ExportFolder = {back.ExportFolder}";
                RefreshSettingsValues();
                Ui.ShowBanner(labelBanner,
                    $"✕ Registry: the write succeeded — into HKCU of \"{account}\" on {machine}. That is whose registry this really is on a server: the service account's, one hive shared by every visitor. kelly saves {density}, sam reads {density}. ExportFolder {back.ExportFolder} is a disk the user cannot see. On Linux/containers the same call throws PlatformNotSupportedException.",
                    Ui.BannerKind.Error);
                Ui.SetStatus(labelStatus, "registry = the server account's hive, shared by all", Ui.Error);
            }
            catch (PlatformNotSupportedException ex)
            {
                // thrown by Registry.CurrentUser itself — in RegistrySettings.Load(), before Save() is ever reached
                trace.Add(TraceKind.Boundary, "Registry.CurrentUser", $"{ex.GetType().Name}: {RuntimeInformation.OSDescription} has no registry");
                _registryLine = $"PlatformNotSupportedException on {RuntimeInformation.OSDescription}";
                RefreshSettingsValues();
                Ui.ShowBanner(labelBanner,
                    $"✕ Registry: PlatformNotSupportedException — {RuntimeInformation.OSDescription} has no registry at all. Even on Windows HKCU would be the service account's hive, shared by every visitor. Use the profile store or browser storage instead →",
                    Ui.BannerKind.Error);
                Ui.SetStatus(labelStatus, "registry: not supported on this server", Ui.Error);
            }
            catch (Exception ex)
            {
                trace.Add(TraceKind.Boundary, "Registry.CurrentUser", $"{ex.GetType().Name}: {ex.Message}");
                _registryLine = $"{ex.GetType().Name}: {ex.Message}";
                RefreshSettingsValues();
                Ui.ShowBanner(labelBanner,
                    $"✕ Registry: {ex.GetType().Name} — the service account may not even be allowed to write HKCU ({ex.Message}). Either way the server's registry is the wrong place for a user's settings.",
                    Ui.BannerKind.Error);
                Ui.SetStatus(labelStatus, "registry write failed on the server", Ui.Error);
            }
        }

        private void buttonProfile_Click(object sender, EventArgs e)
        {
            string user = CurrentUserName;
            if (user == null)
            {
                trace.Add(TraceKind.Server, "UserProfileStore", "no signed-in user — a profile belongs to a user");
                Ui.ShowBanner(labelBanner, "Sign in first — a profile belongs to a user, which is exactly what HKCU could not express on the server.", Ui.BannerKind.Warn);
                Ui.SetStatus(labelStatus, "profile store needs a signed-in user", Ui.Warn);
                return;
            }

            string density = SelectedDensity;
            trace.Add(TraceKind.FromClient, "settings.profile", $"GridDensity = {density} for {user}");

            try
            {
                var profile = UserProfileStore.Load(user);                 // ✓ per user, on the server
                profile.GridDensity = density;
                string path = UserProfileStore.Save(user, profile);
                var back = UserProfileStore.Load(user);
                string exportRoot = UserProfileStore.ExportRootFor(user, back);

                trace.Add(TraceKind.Server, "UserProfileStore.Save", $"{Rel(path)} · {new FileInfo(path).Length} bytes (System.Text.Json)");
                trace.Add(TraceKind.Server, "UserProfileStore.Load", $"GridDensity = {back.GridDensity} · ExportFolder = {back.ExportFolder} → {Rel(exportRoot)}");

                _profileLine = $"{Rel(path)}: GridDensity = {back.GridDensity} · exports → {Rel(exportRoot)}";
                RefreshSettingsValues();
                Ui.ShowBanner(labelBanner,
                    $"✓ Profile store: {user}'s settings live in {Rel(path)} on the server — per user, they follow the user to any device and server code can read them. ExportFolder is now a folder under App_Data ({Rel(exportRoot)}), not C:\\Orders: exports are staged there and reach the browser through Application.Download. Settings that must be audited or queried go one step further, into a database table.",
                    Ui.BannerKind.Ok);
                Ui.SetStatus(labelStatus, $"profile saved for {user}", Ui.Ok);
            }
            catch (Exception ex)
            {
                // file I/O under the storage root, or a hand-edited <user>.json that no longer parses (JsonException)
                trace.Add(TraceKind.Boundary, "UserProfileStore", $"{ex.GetType().Name}: {ex.Message}");
                _profileLine = $"{ex.GetType().Name}: {ex.Message}";
                RefreshSettingsValues();
                Ui.ShowBanner(labelBanner,
                    $"✕ Profile store: {ex.GetType().Name} — {ex.Message}. The profile file {Rel(UserProfileStore.PathFor(user))} could not be read or written: a hand-edited file that no longer parses, or the storage root is not writable for the account running Kestrel.",
                    Ui.BannerKind.Error);
                Ui.SetStatus(labelStatus, $"profile store failed for {user}", Ui.Error);
            }
        }

        private async void buttonBrowser_Click(object sender, EventArgs e)
        {
            string density = SelectedDensity;
            trace.Add(TraceKind.FromClient, "settings.browser", $"GridDensity = {density} → localStorage");

            string back;
            try
            {
                trace.Add(TraceKind.ToClient, "Application.Eval", BrowserPreferences.SaveDensityScript(density));
                BrowserPreferences.SaveDensity(density);                    // ✓ device-bound UI preference

                back = await BrowserPreferences.ReadDensityAsync();         // round trip: the browser answers
                if (IsDisposed) return;
                trace.Add(TraceKind.FromClient, "Application.EvalAsync", $"{BrowserPreferences.ReadDensityScript} → {(back == null ? "null" : "\"" + back + "\"")}");
            }
            catch (Exception ex)
            {
                // Eval/EvalAsync throw when the browser blocks storage (a SecurityError from the page) or the session is gone
                if (IsDisposed) return;
                trace.Add(TraceKind.Boundary, "localStorage", $"{ex.GetType().Name}: {ex.Message}");
                _browserLine = $"{ex.GetType().Name}: {ex.Message}";
                RefreshSettingsValues();
                Ui.ShowBanner(labelBanner,
                    $"✕ Browser storage: {ex.GetType().Name} — {ex.Message}. Application.Eval/EvalAsync fail when the browser blocks localStorage; a device-bound preference has no server-side fallback of its own.",
                    Ui.BannerKind.Error);
                Ui.SetStatus(labelStatus, "browser storage failed", Ui.Error);
                return;
            }

            if (back == null)
            {
                // setItem ran a moment ago, so null here means the browser refused the write: storage blocked by privacy settings or policy
                _browserLine = $"localStorage['{BrowserPreferences.DensityKey}'] = null (localStorage unavailable or blocked in this browser)";
                RefreshSettingsValues();
                Ui.ShowBanner(labelBanner,
                    $"✕ Browser storage: localStorage['{BrowserPreferences.DensityKey}'] read back null right after setItem — localStorage unavailable or blocked in this browser. Nothing was stored; the profile store's roaming copy (when signed in) is the only value that survives.",
                    Ui.BannerKind.Error);
                Ui.SetStatus(labelStatus, "localStorage unavailable or blocked in this browser", Ui.Error);
                return;
            }

            _browserLine = $"localStorage['{BrowserPreferences.DensityKey}'] = \"{back}\" (this browser profile only)";
            RefreshSettingsValues();
            Ui.ShowBanner(labelBanner,
                $"✓ Browser storage: localStorage['{BrowserPreferences.DensityKey}'] = \"{back}\" — stored in THIS browser profile on THIS device. The second session in the same browser reads the same value, another device starts empty, and the user can clear it at any time. Right for a UI preference like density; never for business settings, which stay server-owned.",
                Ui.BannerKind.Ok);
            Ui.SetStatus(labelStatus, $"density {back} stored in the browser", Ui.Ok);

            // The roaming copy: browser storage first; the profile store follows only while a user is signed in,
            // so a new device could start from the user's usual value (that seeding is not built in this sample).
            string user = CurrentUserName;
            if (user == null)
            {
                trace.Add(TraceKind.Server, "UserProfileStore", "no roaming copy — nobody is signed in; the value lives in this browser only");
                return;
            }
            try
            {
                var profile = UserProfileStore.Load(user);
                profile.GridDensity = back;
                string path = UserProfileStore.Save(user, profile);
                trace.Add(TraceKind.Server, "UserProfileStore.Save", $"roaming copy: GridDensity = {back} → {Rel(path)} for {user}");
            }
            catch (Exception ex)
            {
                // the browser value is stored either way; only the roaming copy failed
                trace.Add(TraceKind.Boundary, "UserProfileStore", $"roaming copy not written — {ex.GetType().Name}: {ex.Message}");
            }
        }

        #endregion

        #region Card C · Logout and timeout cleanup (lab step 6)

        private void buttonSignOut_Click(object sender, EventArgs e)
        {
            trace.Add(TraceKind.FromClient, "sign out", $"{CurrentUserName ?? "(nobody)"} · {StoreName}");
            bool legacy = _mode == StateMode.LegacyStatics && Legacy.AppState.CurrentUser != null;
            RunCleanup("logout");
            Ui.ShowBanner(labelBanner,
                "✓ Signed out: SessionContext.Reset() cleared this session's context and the cleanup routine released the session's temp files; report jobs, open transactions and locks are released in the same routine — the one place both logout and timeout call." +
                (legacy ? " In Legacy statics mode this logout also blanked AppState — for EVERY session on the server." : ""),
                legacy ? Ui.BannerKind.Warn : Ui.BannerKind.Ok);
        }

        private void buttonTimeout_Click(object sender, EventArgs e)
        {
            if (timerTimeout.Enabled) return;
            _countdown = CountdownSeconds;
            progressTimeout.Value = 0;
            progressTimeout.Visible = true;
            trace.Add(TraceKind.FromClient, "simulate timeout", $"{CountdownSeconds} s countdown → the cleanup routine Application.SessionTimeout → ApplicationExit would run; the session itself stays alive");
            Ui.SetStatus(labelStatus, $"session times out in {_countdown} s…", Ui.Warn);
            timerTimeout.Start();
        }

        private void timerTimeout_Tick(object sender, EventArgs e)
        {
            _countdown--;
            progressTimeout.Value = (CountdownSeconds - _countdown) * (100 / CountdownSeconds);
            if (_countdown > 0)
            {
                Ui.SetStatus(labelStatus, $"session times out in {_countdown} s… (a real timeout shows the built-in prolong dialog first)", Ui.Warn);
                return;
            }

            timerTimeout.Stop();
            progressTimeout.Visible = false;
            trace.Add(TraceKind.Server, "SessionTimeout (simulated)", "cleanup must run BEFORE the session is destroyed — afterwards there is no session left to clean");
            RunCleanup("timeout (simulated)");
            Ui.ShowBanner(labelBanner,
                "⏱ Simulated timeout: the cleanup routine ran (temp files deleted, context reset; report jobs, transactions and locks logged) but the session was NOT ended. The real Application.SessionTimeout fires after the configured sessionTimeout, shows the built-in prolong dialog (this sample leaves Handled = false), and if nobody answers the session ends and Application.ApplicationExit runs this same routine.",
                Ui.BannerKind.Warn);
        }

        /// <summary>The one routine every exit shares — see Services/SessionCleanup.cs.</summary>
        private void RunCleanup(string reason)
        {
            foreach (var step in SessionCleanup.Run(reason, Application.SessionId))
                trace.Add(TraceKind.Server, "cleanup", step);

            if (_mode == StateMode.LegacyStatics && Legacy.AppState.CurrentUser != null)
            {
                // ✕ there is no "this session" in a static: clearing it signs out everybody
                Legacy.AppState.CurrentUser = null;
                Legacy.AppState.CurrentCompany = "Acme";
                Legacy.AppState.CurrentCustomer = null;
                Legacy.AppState.CurrentFilter = null;
                Legacy.AppState.LastSearch = null;
                trace.Add(TraceKind.Boundary, "AppState cleared", "✕ the statics are one slot — this sign-out signed out EVERY session on the server");
            }

            LastWritten = null;
            SyncCombosFromStore();
            BindOrders();
            RefreshStores(announce: false);
            RefreshSettingsValues();
            Ui.SetStatus(labelStatus, $"signed out ({reason}) · cleanup ran", Ui.Ok);
        }

        // ── the real lifecycle hooks ─────────────────────────────────────────────────────

        private void Application_SessionTimeout(object sender, HandledEventArgs e)
        {
            // Handled stays false: the built-in "prolong the session?" dialog must still appear.
            // Nothing is released yet — the user may click "continue"; ApplicationExit is the point of no return.
            if (IsDisposed) return;
            trace.Add(TraceKind.Server, "Application.SessionTimeout", $"session {Short(Application.SessionId)} is about to time out — Handled = false, the prolong dialog is shown; cleanup waits for ApplicationExit");
        }

        // Application.ApplicationExit — the session is going away (timeout expired, Application.Exit, browser
        // gone) — is handled in Program.Main, not here: the page may already be disposed when it fires, and a
        // handler detached in Dispose would never run. See Program.cs and Services/SessionCleanup.cs.

        private void DetachApplicationEvents()
        {
            Application.SessionTimeout -= Application_SessionTimeout;
        }

        #endregion

        private void buttonClear_Click(object sender, EventArgs e) => trace.Clear();

        // ── small helpers ────────────────────────────────────────────────────────────────

        private static string Short(string id) => SessionCleanup.ShortId(id);

        private static string FilterText(OrderStatus? filter) => filter.HasValue ? filter.Value.ToString() : "All";

        private string CustomerName(int? customerId) =>
            customerId.HasValue ? _customerService.Find(customerId.Value)?.Name ?? "?" : "all customers";

        /// <summary>A path relative to the web root, for the console and the trace.</summary>
        private static string Rel(string path)
        {
            string root = Application.StartupPath ?? "";
            return path.StartsWith(root, StringComparison.OrdinalIgnoreCase) ? path.Substring(root.Length).TrimStart('\\', '/') : path;
        }
    }
}
