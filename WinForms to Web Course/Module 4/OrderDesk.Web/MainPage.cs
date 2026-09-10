using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using OrderDesk.Domain;
using OrderDesk.Legacy;
using OrderDesk.Services;
using OrderDesk.Shared;
using Wisej.Web;

namespace OrderDesk
{
    /// <summary>
    /// Module 4 — Sessions, Statics &amp; Multi-User Safety.
    /// The LoginForm became a Sign-in card; the Orders screen shows the signed-in user's orders; the
    /// State store card lets the same screen run against the legacy AppState statics (✕ one slot for
    /// the whole server) or the typed UserContext in Application.Session (✓ one per session) so the
    /// corruption and the fix can be seen side by side. Registry preferences (HKCU on the SERVER) are
    /// replaced by a per-user server profile under App_Data/users.
    /// </summary>
    public partial class MainPage : Page
    {
        private readonly OrderService _service = new OrderService();
        private readonly LegacyStaticStore _staticStore = new LegacyStaticStore();
        private readonly SessionContextStore _sessionStore = new SessionContextStore();
        private readonly UserSettingsStore _profiles = new UserSettingsStore();
        private IStateStore _active;
        private bool _syncingCombos;
        private int _replayStep = -1;
        private UserContext _sessionB;   // the simulated second session used by the single-tab demos

        /// <summary>A row of the Orders grid (a projection: the grid never binds the Customer aggregate with its TaxId).</summary>
        public sealed class OrderRow
        {
            public int Id { get; set; }
            public string Customer { get; set; }
            public decimal Total { get; set; }
            public string Status { get; set; }
            public string Match { get; set; }
        }

        public sealed class AuditRow
        {
            public string Static { get; set; }
            public string Class { get; set; }
            public string Decision { get; set; }
        }

        /// <summary>The desktop app's known clerks with the defaults the walkthrough uses (kelly = Acme/Northwind/Open, sam = Globex/Fabrikam/Invoiced).</summary>
        private sealed class KnownUser
        {
            public int Id; public string Name; public string Company; public string Role; public string Customer; public OrderStatus Filter;
        }

        private static readonly KnownUser[] KnownUsers =
        {
            new KnownUser { Id = 1, Name = "kelly", Company = "Acme",   Role = "Clerk",   Customer = "Northwind Traders", Filter = OrderStatus.Open },
            new KnownUser { Id = 2, Name = "sam",   Company = "Globex", Role = "Clerk",   Customer = "Fabrikam Inc",      Filter = OrderStatus.Invoiced },
            new KnownUser { Id = 3, Name = "dana",  Company = "Acme",   Role = "Manager", Customer = "Adventure Works",   Filter = OrderStatus.Invoiced },
            new KnownUser { Id = 4, Name = "priya", Company = "Acme",   Role = "Clerk",   Customer = "Contoso Ltd",       Filter = OrderStatus.Shipped },
        };

        public MainPage()
        {
            InitializeComponent();
            _active = _staticStore;
        }

        // ───────────────────────────── load ─────────────────────────────

        private void MainPage_Load(object sender, EventArgs e)
        {
            trace.Server("Program.Main", "Application.MainPage = new MainPage()  · session " + SessionLifecycle.ShortSessionId() + " · SessionCount " + Application.SessionCount);
            trace.Server("Program.Main", "Application.ApplicationExit += … UserContext.Clear()   Application.SessionTimeout += … trace");

            _syncingCombos = true;
            cmbCustomer.Items.Clear();
            foreach (var c in _service.Customers) cmbCustomer.Items.Add(c.Name);
            cmbUser.SelectedIndex = 0;
            _syncingCombos = false;

            gridAudit.DataSource = new List<AuditRow>
            {
                new AuditRow { Static = "AppState.CurrentUser",     Class = "per-user",   Decision = "→ UserContext.User" },
                new AuditRow { Static = "AppState.CurrentCustomer", Class = "per-user",   Decision = "→ UserContext.CurrentCustomer" },
                new AuditRow { Static = "AppState.ActiveFilter",    Class = "per-user",   Decision = "→ UserContext.ActiveFilter" },
                new AuditRow { Static = "AppState.CurrentOrder",    Class = "per-user",   Decision = "→ dialog parameter (M3)" },
                new AuditRow { Static = "UserPreferences (HKCU)",   Class = "per-user",   Decision = "→ App_Data/users/<name>.json" },
                new AuditRow { Static = "AppState.Countries",       Class = "immutable",  Decision = "keep static" },
                new AuditRow { Static = "OrderStore.Shared()",      Class = "shared+lock", Decision = "keep static (reference data)" },
            };
            trace.Server("Static-state audit", "7 statics found · 5 per-user → session · 2 immutable/shared → keep");

            RefreshGrid(traceIt: true);

            // A page refresh keeps the session: whatever UserContext holds survives, the AppState static too.
            if (UserContext.Exists && UserContext.Current.IsSignedIn)
            {
                var u = UserContext.Current.User;
                trace.Ok("UserContext.Current", UserContext.Current.Handle + " already holds " + u.UserName + " — the session survived the page reload");
                cmbUser.SelectedItem = u.UserName;
                ShowSignedIn(u);
            }
            else if (AppState.CurrentUser != null)
            {
                trace.Finding("AppState.CurrentUser", "already \"" + AppState.CurrentUser.UserName + "\" before this session signed in — set by ANOTHER session (the static is process-wide)");
            }

            RefreshReadback();
            SetStatus("idle");
        }

        // ───────────────────────────── sign in / out ─────────────────────────────

        private void btnSignIn_Click(object sender, EventArgs e)
        {
            var name = cmbUser.SelectedItem as string ?? "kelly";
            trace.In("Sign in.Click", "user = \"" + name + "\"");
            var known = KnownUsers.First(k => k.Name == name);
            var user = new User { Id = known.Id, UserName = known.Name, DisplayName = char.ToUpperInvariant(name[0]) + name.Substring(1), Role = known.Role, Company = known.Company };
            var customer = FindCustomer(known.Customer);

            // The desktop way (kept alive for the comparison): one static slot for the whole process.
            _staticStore.User = user;
            _staticStore.Customer = customer;
            _staticStore.Filter = known.Filter;
            trace.Fail("AppState.CurrentUser =", "\"" + name + "\" · CurrentCustomer = " + customer.Name + " · ActiveFilter = " + known.Filter + "   ✕ static: visible to every session");

            // The web way: this session's typed context.
            _sessionStore.User = user;
            _sessionStore.Customer = customer;
            _sessionStore.Filter = known.Filter;
            trace.Ok("UserContext.Current", UserContext.Current.Handle + " ← " + name + " · " + customer.Name + " · " + known.Filter + "   ✓ Application.Session of " + SessionLifecycle.ShortSessionId());

            ShowSignedIn(user);
            HideBanner();
            RefreshReadback();
            RefreshGrid(traceIt: true);
            Notify.Saved("Signed in as " + name + " (" + user.Company + ").");
        }

        private void btnSignOut_Click(object sender, EventArgs e)
        {
            trace.In("Sign out.Click", "");
            var who = UserContext.Exists ? UserContext.Current.User?.UserName : null;
            UserContext.Clear();
            trace.Ok("UserContext.Clear()", "Application.Session.UserContext = null · session " + SessionLifecycle.ShortSessionId() + " forgot " + (who ?? "(nobody)") + " — other sessions untouched");
            trace.Finding("AppState.CurrentUser", "left as \"" + (AppState.CurrentUser?.UserName ?? "null") + "\": setting the static to null would sign out EVERY session");
            trace.Server("SessionLifecycle", "the same Clear() runs on Application.ApplicationExit; SessionTimeout is traced (docs/session-cleanup.md)");

            lblSignedIn.Text = "Not signed in — pick a user and click Sign in.";
            lblSignedIn.ForeColor = Palette.Ink;
            HideBanner();
            RefreshReadback();
            RefreshGrid(traceIt: false);
            SetStatus("idle");
            Notify.Saved("Signed out — this session only.");
        }

        private void ShowSignedIn(User user)
        {
            lblSignedIn.Text = "Signed in as " + user.UserName + " · " + user.Company + " · " + user.Role;
            lblSignedIn.ForeColor = Palette.Good;
        }

        // ───────────────────────────── store toggle + combos ─────────────────────────────

        private void rdoStore_CheckedChanged(object sender, EventArgs e)
        {
            var rdo = (RadioButton)sender;
            if (!rdo.Checked) return;
            _active = rdo == rdoSession ? (IStateStore)_sessionStore : _staticStore;
            trace.In("State store toggle", "active store = " + _active.Name);
            RefreshReadback();
            trace.Server(_active.Name, Describe(_active));
        }

        private void cmbCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_syncingCombos) return;
            var customer = FindCustomer(cmbCustomer.SelectedItem as string);
            if (customer == null) return;
            trace.In("Select customer", "\"" + customer.Name + "\"");
            _active.Customer = customer;
            if (_active == _staticStore)
                trace.Fail("AppState.CurrentCustomer =", customer.Name + "   ✕ every session now reads " + customer.Name);
            else
                trace.Ok("UserContext.Current.CurrentCustomer =", customer.Name + "   ✓ " + UserContext.Current.Handle + " only");
            RefreshReadback();
        }

        private void cmbFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_syncingCombos) return;
            var text = cmbFilter.SelectedItem as string;
            if (!Enum.TryParse<OrderStatus>(text, out var status)) return;
            trace.In("Select filter", "\"" + status + "\"");
            _active.Filter = status;
            if (_active == _staticStore)
                trace.Fail("AppState.ActiveFilter =", "\"" + status + "\"   ✕ every session's grid now filters on " + status);
            else
                trace.Ok("UserContext.Current.ActiveFilter =", status + "   ✓ " + UserContext.Current.Handle + " only");
            RefreshReadback();
            RefreshGrid(traceIt: false);
        }

        // ───────────────────────────── read back ─────────────────────────────

        private void btnReadBack_Click(object sender, EventArgs e)
        {
            trace.In("Read back.Click", "active store = " + _active.Name);
            trace.Server("AppState (static)", Describe(_staticStore));
            trace.Server("UserContext (session)", Describe(_sessionStore) + " · " + UserContext.Current.Handle);
            trace.Server("Application.SessionId", SessionLifecycle.ShortSessionId() + "… · SessionCount " + Application.SessionCount);
            RefreshReadback();

            var sessionUser = _sessionStore.User?.UserName;
            var staticUser = _staticStore.User?.UserName;
            var sessionCustomer = _sessionStore.Customer?.Name;
            var staticCustomer = _staticStore.Customer?.Name;
            if (sessionUser != null && staticUser != null && sessionUser != staticUser)
            {
                trace.Fail("AppState.CurrentUser", "\"" + staticUser + "\" but this session signed in as \"" + sessionUser + "\" — another session overwrote the static");
                ShowBanner("✖ AppState.CurrentUser is \"" + staticUser + "\" — this session signed in as \"" + sessionUser + "\": another session overwrote the shared static.", bad: true);
                SetStatus("alarm");
            }
            else if (sessionCustomer != null && staticCustomer != null && sessionCustomer != staticCustomer)
            {
                trace.Fail("AppState.CurrentCustomer", staticCustomer + " ≠ UserContext " + sessionCustomer + " — the static was overwritten");
                ShowBanner("✖ " + (staticUser ?? "another session") + "'s selection overwrote " + sessionUser + "'s: AppState says " + staticCustomer + ", UserContext says " + sessionCustomer + ".", bad: true);
                SetStatus("alarm");
            }
            else if (sessionUser != null)
            {
                trace.Ok("Read back", "both stores agree for " + sessionUser + " — open a second tab as sam to see them diverge (README: two-session test)");
                ShowBanner("✓ Both stores agree for " + sessionUser + ". Open a second tab, sign in as sam, come back and Read back in static mode.", bad: false);
                SetStatus("idle");
            }
            else
            {
                trace.Server("Read back", "nobody is signed in in this session");
                SetStatus("idle");
            }
        }

        // ───────────────────────────── failure: the static corruption ─────────────────────────────

        private void btnCorrupt_Click(object sender, EventArgs e)
        {
            trace.In("Corrupt from here ✕.Click", "simulate session B (sam) writing the AppState static while kelly is signed in here");
            var kelly = EnsureSignedIn("kelly");
            var northwind = FindCustomer("Northwind Traders");
            var fabrikam = FindCustomer("Fabrikam Inc");
            var sam = MakeUser("sam");

            // Baseline: kelly's selection in the static, exactly as the desktop app left it.
            _staticStore.User = kelly;
            _staticStore.Customer = northwind;
            _staticStore.Filter = OrderStatus.Open;
            rdoStatic.Checked = true;
            trace.Server("AppState ← kelly", "CurrentCustomer = Northwind Traders · ActiveFilter = Open   (session A, this tab)");
            RefreshReadback();
            SetStatus("working");
            HideBanner();

            Application.StartTask(() =>
            {
                try
                {
                    Thread.Sleep(500);
                    if (IsDisposed) return;
                    // Session B (sam) runs the SAME code path: OrdersForm's "select customer" wrote the static.
                    AppState.CurrentCustomer = fabrikam;
                    AppState.ActiveFilter = "Invoiced";
                    trace.Server("session B (sam) · StartTask", "AppState.CurrentCustomer = Fabrikam Inc · ActiveFilter = \"Invoiced\"   — the same static slot");
                    Application.Update(this);

                    Thread.Sleep(500);
                    if (IsDisposed) return;
                    var seen = AppState.CurrentCustomer?.Name;
                    trace.Fail("session A (kelly) reads", "AppState.CurrentCustomer = " + seen + " · ActiveFilter = " + AppState.ActiveFilter + "   ✕ kelly's customer silently changed");
                    trace.Finding("static CurrentCustomer", "one slot for the whole server — sam's selection overwrote kelly's; passes single-user testing, fails in production");
                    ShowBanner("✖ sam's selection overwrote kelly's: AppState.CurrentCustomer is now " + seen + " (was Northwind Traders). A static is one slot for the whole server.", bad: true);
                    RefreshReadback();
                    RefreshGrid(traceIt: false);
                    SetStatus("alarm");
                    Application.Update(this);
                }
                catch (Exception ex)
                {
                    if (IsDisposed) return;
                    trace.Fail("Corrupt from here", ex.GetType().Name + ": " + ex.Message);
                    SetStatus("alarm");
                    Application.Update(this);
                }
            });
        }

        // ───────────────────────────── recovery: the session context ─────────────────────────────

        private void btnUseContext_Click(object sender, EventArgs e)
        {
            trace.In("Use UserContext ✓.Click", "the same sequence through Application.Session");
            var kelly = EnsureSignedIn("kelly");
            var northwind = FindCustomer("Northwind Traders");
            var fabrikam = FindCustomer("Fabrikam Inc");
            var sam = MakeUser("sam");

            _sessionStore.User = kelly;
            _sessionStore.Customer = northwind;
            _sessionStore.Filter = OrderStatus.Open;
            rdoSession.Checked = true;
            var handleA = UserContext.Current.Handle;
            trace.Ok("UserContext.Current → session " + SessionLifecycle.ShortSessionId(), handleA + " ← kelly · Northwind Traders · Open   (session A, this tab)");
            RefreshReadback();
            SetStatus("working");
            HideBanner();

            Application.StartTask(() =>
            {
                try
                {
                    Thread.Sleep(500);
                    if (IsDisposed) return;
                    // Session B gets its own bag from Application.Session; in this single tab we stand it in with a second UserContext object.
                    _sessionB = new UserContext { User = sam, CurrentCustomer = fabrikam, ActiveFilter = OrderStatus.Invoiced };
                    trace.Server("session B (sam) · simulated", _sessionB.Handle + " ← sam · Fabrikam Inc · Invoiced   (a second UserContext — what Application.Session hands the other tab)");
                    Application.Update(this);

                    Thread.Sleep(500);
                    if (IsDisposed) return;
                    // Read back from THIS session's context — Application.Session is still kelly's inside StartTask.
                    var ctx = UserContext.Current;
                    trace.Ok("session A (kelly) reads", ctx.Handle + " · CurrentCustomer = " + ctx.CurrentCustomer?.Name + " · ActiveFilter = " + ctx.ActiveFilter + "   ✓ isolated — " + handleA + " ≠ " + _sessionB.Handle);
                    trace.Finding("UserContext.Current", "typed context behind Application.Session: callers never see the storage; one place to initialize defaults and Clear() on logout");
                    ShowBanner("✓ kelly keeps Northwind Traders / Open — each session reads its own UserContext (" + handleA + " vs " + _sessionB.Handle + "). The leak is gone.", bad: false);
                    RefreshReadback();
                    RefreshGrid(traceIt: false);
                    SetStatus("idle");
                    Application.Update(this);
                }
                catch (Exception ex)
                {
                    if (IsDisposed) return;
                    trace.Fail("Use UserContext", ex.GetType().Name + ": " + ex.Message);
                    SetStatus("alarm");
                    Application.Update(this);
                }
            });
        }

        // ───────────────────────────── progress: the two-session replay (Timer) ─────────────────────────────

        private void btnReplay_Click(object sender, EventArgs e)
        {
            if (timerReplay.Enabled)
            {
                timerReplay.Stop();
                _replayStep = -1;
                btnReplay.Text = "Two-session replay ▶";
                trace.In("Two-session replay", "stopped");
                SetStatus("idle");
                return;
            }
            trace.In("Two-session replay ▶.Click", "Wisej.Web.Timer · 700 ms per step · the video's retest, first through UserContext then through the static");
            var kelly = EnsureSignedIn("kelly");
            var northwind = FindCustomer("Northwind Traders");
            _sessionStore.User = kelly; _sessionStore.Customer = northwind; _sessionStore.Filter = OrderStatus.Open;
            _staticStore.User = kelly; _staticStore.Customer = northwind; _staticStore.Filter = OrderStatus.Open;
            _sessionB = new UserContext { User = MakeUser("sam"), CurrentCustomer = FindCustomer("Fabrikam Inc"), ActiveFilter = OrderStatus.Open };
            HideBanner();
            RefreshReadback();
            _replayStep = 0;
            btnReplay.Text = "Two-session replay ■";
            SetStatus("working");
            timerReplay.Start();
        }

        private void timerReplay_Tick(object sender, EventArgs e)
        {
            var a = UserContext.Current;
            switch (_replayStep)
            {
                case 0:
                    trace.Server("A", "UserContext.Current → session " + SessionLifecycle.ShortSessionId() + " · " + a.Handle + " · kelly · Northwind Traders · Open");
                    break;
                case 1:
                    trace.Server("B", "UserContext.Current → session (2nd tab, simulated) · " + _sessionB.Handle + " · sam · Fabrikam Inc · Open");
                    break;
                case 2:
                    _sessionB.ActiveFilter = OrderStatus.Invoiced;
                    trace.Server("B", "B sets filter = \"Invoiced\"");
                    break;
                case 3:
                    trace.Ok("A", "A filter unchanged = \"" + a.ActiveFilter + "\" ✓ isolated");
                    rdoSession.Checked = true;
                    RefreshReadback();
                    break;
                case 4:
                    AppState.ActiveFilter = "Invoiced";
                    trace.Server("B", "same test against the static: B sets AppState.ActiveFilter = \"Invoiced\"");
                    break;
                case 5:
                    trace.Fail("A", "A reads AppState.ActiveFilter = \"" + AppState.ActiveFilter + "\" ✖ leaked across sessions");
                    trace.Finding("two-session test", "UserContext isolated ✓ · AppState leaked ✕ — prove isolation with two real tabs before user acceptance testing");
                    rdoStatic.Checked = true;
                    RefreshReadback();
                    RefreshGrid(traceIt: false);
                    ShowBanner("✖ Replay: B's filter change leaked into A through AppState.ActiveFilter, and did NOT through UserContext. Test two sessions before you call it done.", bad: true);
                    timerReplay.Stop();
                    _replayStep = -1;
                    btnReplay.Text = "Two-session replay ▶";
                    SetStatus("alarm");
                    return;
            }
            _replayStep++;
        }

        // ───────────────────────────── failure: registry preferences ─────────────────────────────

        private void btnRegistry_Click(object sender, EventArgs e)
        {
            trace.In("Registry preference ✕.Click", "Legacy.UserPreferences — HKEY_CURRENT_USER\\" + UserPreferences.KeyPath);
            var who = _active.User?.UserName ?? "kelly";
            try
            {
                trace.Server("Environment", "MachineName = " + Environment.MachineName + " · UserName = " + Environment.UserName + " · OS = " + Environment.OSVersion.Platform);
                var last = UserPreferences.Get("LastUser", "(not set)");
                trace.Fail("UserPreferences.Get(\"LastUser\")", "→ \"" + last + "\"  read from HKCU of the SERVER (" + Environment.MachineName + ") under the service account (" + Environment.UserName + "), not from " + who + "'s PC");
                UserPreferences.Set("LastUser", who);
                trace.Fail("UserPreferences.Set(\"LastUser\")", "wrote \"" + who + "\" into the server's HKCU\\" + UserPreferences.KeyPath + " — every browser user now shares one \"last user\"");
                trace.Finding("registry on the server", "wrong machine, wrong user: HKCU belongs to the process account; per-user settings need a database row, a server profile or browser storage");
                ShowBanner("✖ Wrong machine (" + Environment.MachineName + "), wrong user (" + Environment.UserName + "): HKCU on the server is the service account's registry, not " + who + "'s.", bad: true);
            }
            catch (PlatformNotSupportedException ex)
            {
                trace.Fail("Microsoft.Win32.Registry", ex.GetType().Name + " — no registry on " + Environment.OSVersion.Platform + " (Linux/macOS host)");
                trace.Finding("registry on the server", "the API does not even exist on a non-Windows host; the setting must move to a server profile or a database");
                ShowBanner("✖ Registry is not available on this server OS (" + Environment.OSVersion.Platform + ") — PlatformNotSupportedException. User settings must move off HKCU.", bad: true);
            }
            catch (Exception ex)
            {
                trace.Fail("UserPreferences", ex.GetType().Name + ": " + ex.Message);
                ShowBanner("✖ Registry access failed on the server: " + ex.GetType().Name + " — " + ex.Message, bad: true);
            }
            SetStatus("alarm");
        }

        // ───────────────────────────── recovery: the server profile ─────────────────────────────

        private void btnServerProfile_Click(object sender, EventArgs e)
        {
            trace.In("Server profile ✓.Click", "Services.UserSettingsStore → App_Data/users/<name>.json via System.Text.Json");
            var user = _sessionStore.User ?? _active.User;
            if (user == null)
            {
                trace.Fail("Server profile", "nobody is signed in in this session — sign in first (the profile is keyed by the signed-in user, not by the machine)");
                ShowBanner("Sign in first: the server profile is keyed by the signed-in user.", bad: true);
                return;
            }
            try
            {
                var profile = new UserProfile
                {
                    UserName = user.UserName,
                    LastCustomer = _active.Customer?.Name,
                    LastFilter = _active.Filter?.ToString(),
                    SavedFromSession = SessionLifecycle.ShortSessionId(),
                };
                _profiles.Save(profile);
                trace.Server("UserSettingsStore.Save", _profiles.RelativePath(user.UserName) + " ← lastCustomer = " + (profile.LastCustomer ?? "null") + " · lastFilter = " + (profile.LastFilter ?? "null"));
                var back = _profiles.Load(user.UserName);
                trace.Ok("UserSettingsStore.Load", "\"" + user.UserName + "\" → lastCustomer = " + (back.LastCustomer ?? "null") + " · lastFilter = " + (back.LastFilter ?? "null") + " · savedUtc = " + back.SavedUtc.ToString("HH:mm:ss") + "Z · from session " + back.SavedFromSession);
                trace.Finding("registry → server profile", "keyed by the signed-in user, owned by the server, roams to any browser; a database row adds audit — browser storage only for device-only UI preferences (Application.Eval + localStorage)");
                ShowBanner("✓ " + user.UserName + "'s preferences live in " + _profiles.RelativePath(user.UserName) + " — the server owns them and the user, not the machine, is the key.", bad: false);
                SetStatus("idle");
            }
            catch (Exception ex)
            {
                trace.Fail("UserSettingsStore", ex.GetType().Name + ": " + ex.Message);
                ShowBanner("✖ Server profile failed: " + ex.Message, bad: true);
                SetStatus("alarm");
            }
        }

        // ───────────────────────────── lifecycle hook (called from SessionLifecycle) ─────────────────────────────

        /// <summary>Called by Program.Main's ApplicationExit / SessionTimeout handlers so the hook shows up in the trace.</summary>
        public void TraceLifecycle(string name, string payload)
        {
            if (IsDisposed) return;
            try
            {
                trace.Server(name, payload);
                Application.Update(this);
            }
            catch
            {
                // the session may already be gone — Console.WriteLine in SessionLifecycle keeps the record
            }
        }

        // ───────────────────────────── helpers ─────────────────────────────

        private User EnsureSignedIn(string fallback)
        {
            var current = _sessionStore.User;
            if (current != null) return current;
            var user = MakeUser(fallback);
            _sessionStore.User = user;
            _staticStore.User = user;
            cmbUser.SelectedItem = fallback;
            ShowSignedIn(user);
            trace.Server("auto sign-in", "\"" + fallback + "\" — nobody was signed in in this session");
            return user;
        }

        private static User MakeUser(string name)
        {
            var k = KnownUsers.First(x => x.Name == name);
            return new User { Id = k.Id, UserName = k.Name, DisplayName = char.ToUpperInvariant(name[0]) + name.Substring(1), Role = k.Role, Company = k.Company };
        }

        private Customer FindCustomer(string name)
            => name == null ? null : _service.Customers.FirstOrDefault(c => c.Name == name);

        private static string Describe(IStateStore s)
            => "user = " + (s.User?.UserName ?? "null") + " · company = " + (s.User?.Company ?? "null") + " · customer = " + (s.Customer?.Name ?? "null") + " · filter = " + (s.Filter?.ToString() ?? "null");

        /// <summary>Show what the ACTIVE store returns right now (labels + combos), without firing the combo events.</summary>
        private void RefreshReadback()
        {
            var s = _active;
            lblValUser.Text = s.User?.UserName ?? "—";
            lblValCompany.Text = s.User?.Company ?? "—";
            lblValCustomer.Text = s.Customer?.Name ?? "—";
            lblValFilter.Text = s.Filter?.ToString() ?? "—";
            lblValStore.Text = s.Name;
            lblStoreLocation.Text = s.Location;
            lblValStore.ForeColor = s == _staticStore ? Palette.Bad : Palette.Good;
            lblSession.Text = "Application.SessionId " + SessionLifecycle.ShortSessionId() + "…  ·  SessionCount " + Application.SessionCount;

            _syncingCombos = true;
            try
            {
                cmbCustomer.SelectedItem = s.Customer?.Name;
                if (s.Customer == null) cmbCustomer.SelectedIndex = -1;
                cmbFilter.SelectedItem = s.Filter?.ToString();
                if (s.Filter == null) cmbFilter.SelectedIndex = -1;
            }
            finally { _syncingCombos = false; }
        }

        /// <summary>The Orders screen: the five walkthrough orders (GetAll() returns them first) with a ✓ on the rows matching the active filter.</summary>
        private void RefreshGrid(bool traceIt)
        {
            var filter = _active.Filter;
            var all = _service.GetAll();
            var rows = all.Take(5).Select(o => new OrderRow
            {
                Id = o.Id,
                Customer = o.CustomerName,
                Total = o.Total,
                Status = o.Status.ToString(),
                Match = filter.HasValue && o.Status == filter.Value ? "✓" : "—",
            }).ToList();
            gridOrders.DataSource = rows;
            var matches = rows.Count(r => r.Match == "✓");
            var who = _active.User?.UserName;
            lblOrdersTitle.Text = "Orders — " + (who != null ? who + "'s screen · " : "") + "top 5 of " + all.Count
                + (filter.HasValue ? " · filter " + filter + " (" + matches + " match)" : " · no filter");
            if (traceIt)
                trace.Server("OrderService.GetAll()", all.Count + " rows · bound top 5: " + string.Join(" · ", rows.Select(r => r.Id + " " + r.Customer + " " + r.Total.ToString("C2") + " " + r.Status)));
        }

        private void SetStatus(string kind)
        {
            switch (kind)
            {
                case "working": lblStatus.Text = "● working"; lblStatus.ForeColor = System.Drawing.Color.FromArgb(255, 224, 130); break;
                case "alarm": lblStatus.Text = "● alarm"; lblStatus.ForeColor = System.Drawing.Color.FromArgb(255, 190, 180); break;
                default: lblStatus.Text = "● idle"; lblStatus.ForeColor = System.Drawing.Color.White; break;
            }
        }

        private void ShowBanner(string text, bool bad)
        {
            lblBanner.Text = text;
            lblBanner.BackColor = bad ? Palette.BadSoft : Palette.GoodSoft;
            lblBanner.ForeColor = bad ? Palette.Bad : Palette.Good;
            lblBanner.Visible = true;
        }

        private void HideBanner()
        {
            lblBanner.Visible = false;
        }
    }
}
