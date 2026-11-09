// AccessOps PolicyHarness - Module 7 (cumulative: Modules 1-6 plus the capstone rules).
// The allow/deny rules the AccessOps services apply, run without Wisej.NET, IIS, a domain or an
// identity provider - the "Harness" label of the evidence matrix. Adapted from the course pack's
// code_snippets/PolicyHarness/Program.cs.
//   M1  read rule: current grant (exists, same account, not revoked, not expired, read right), tenant,
//       and only then the repository - every deny at zero repository calls.
//   M2  a host-authenticated Windows account is evidence: unmapped or without Reader -> deny.
//   M3  the policy decides before any impersonation scope opens; paths stay under one approved root.
//   M4  sign-out revokes every grant of the account (stale tab F05); continuation URLs stay local (F11).
//   M5  a validated issuer + subject becomes one grant through an approved mapping; a hand-off redeems once.
//   M6  passkey challenges are single-use, counters increase, the store commit is a compare-and-swap.
//   M7  both routes reach ONE RecordPolicy; the hand-off and the grants live in a shared store, so a
//       callback after node loss resumes once or fails safely, and a rollback cannot un-revoke a grant.

using System.Security.Cryptography;

var store = new SharedStore();          // the durable, shared authority (SQLite in the app)
var now = DateTimeOffset.UnixEpoch;
var reads = 0;                          // the read-counting repository
var fixtures = new Dictionary<(string Tenant, int Id), string>
{
    [("A", 17)] = "Tenant A fixture: Supplier contract",
    [("B", 17)] = "Tenant B fixture: Supplier contract — Tailspin logistics"
};

string Issue(string account, string tenant, bool canRead = true, int hours = 8)
{
    var id = "G-" + Convert.ToHexString(RandomNumberGenerator.GetBytes(16)).ToLowerInvariant();   // opaque, random
    store.Grants[id] = new Grant(account, tenant, canRead, false, now.AddHours(hours));
    return id;
}

// RecordPolicy.Read: the one check both routes call.
string Decide(string account, string grantId, string tenant)
{
    if (grantId == null || !store.Grants.TryGetValue(grantId, out var g)) return "missing";
    if (g.Account != account) return "account";
    if (g.Revoked) return "revoked";
    if (g.ExpiresAt <= now) return "expired";
    if (!g.CanRead) return "role";
    if (!StringComparer.Ordinal.Equals(g.Tenant, tenant)) return "tenant";
    return null;
}

string Read(string account, string grantId, string tenant, int recordId)
{
    if (Decide(account, grantId, tenant) != null)
        return null;                    // AuditService.Deny, repository never runs
    reads++;
    return fixtures.GetValueOrDefault((tenant, recordId));
}

int SignOut(string account)
{
    var ids = store.Grants.Where(x => x.Value.Account == account && !x.Value.Revoked).Select(x => x.Key).ToList();
    foreach (var id in ids) store.Grants[id] = store.Grants[id] with { Revoked = true };
    return ids.Count;
}

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("AccessOps PolicyHarness · Module 7 · " + System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription);

// ---- M1 / M7: one policy, both routes -------------------------------------------------------------
Section("RecordPolicy - both routes (M1, M2, M7)");
// intranet: Windows SID -> account -> grant by role (AccessGrants.FindForAccount). Invented SIDs.
var windows = new Dictionary<string, (string Account, string Tenant, string Role)>
{
    ["S-1-5-21-1004336348-1177238915-682003330-1103"] = ("reader1", "A", "Reader"),    // LAB\reader1
    ["S-1-5-21-1004336348-1177238915-682003330-1104"] = ("visitor1", "A", "Visitor")   // LAB\visitor1
};
string IntranetSignIn(string sid, out string account)
{
    var mapped = windows.TryGetValue(sid, out var a);
    account = mapped ? a.Account : "win-" + sid[^4..];
    return mapped ? Issue(a.Account, a.Tenant, canRead: a.Role == "Reader") : null;
}
var readerGrant = IntranetSignIn("S-1-5-21-1004336348-1177238915-682003330-1103", out var reader);
var partnerGrant = Issue("dana", "A");                                  // partner: OIDC hand-off grant
Expect("allow (intranet)      reader1, A / 17", Read(reader, readerGrant, "A", 17) != null && reads == 1, "allowed", 1);
Expect("allow (partner)       dana, A / 17", Read("dana", partnerGrant, "A", 17) != null && reads == 2, "allowed", 1);
var before = reads;
Expect("tenant (F02, intranet) reader1, B / 17", Decide(reader, readerGrant, "B") == "tenant" && Read(reader, readerGrant, "B", 17) == null && reads == before, "tenant", reads - before);
Expect("tenant (F02, partner) dana, B / 17", Decide("dana", partnerGrant, "B") == "tenant" && Read("dana", partnerGrant, "B", 17) == null && reads == before, "tenant", reads - before);
var visitorGrant = IntranetSignIn("S-1-5-21-1004336348-1177238915-682003330-1104", out var visitor);
Expect("missing role (M2)     visitor1, host-authenticated", Decide(visitor, visitorGrant, "A") == "role" && Read(visitor, visitorGrant, "A", 17) == null && reads == before, "role", 0);
var unmappedGrant = IntranetSignIn("S-1-5-21-1004336348-1177238915-682003330-1188", out var unmapped);
Expect("missing grant (M2)    unmapped Windows SID", Decide(unmapped, unmappedGrant, "A") == "missing" && reads == before, "missing", 0);
Expect("foreign grant claim   kai presents dana's grant ID", Decide("kai", partnerGrant, "A") == "account" && Read("kai", partnerGrant, "A", 17) == null && reads == before, "account", 0);
now = now.AddHours(8);
Expect("expired grant         reader1 at the grant's expiry", Decide(reader, readerGrant, "A") == "expired" && Read(reader, readerGrant, "A", 17) == null && reads == before, "expired", 0);
now = DateTimeOffset.UnixEpoch;
store.Grants[readerGrant] = store.Grants[readerGrant] with { Revoked = true };
Expect("revoked grant         reader1 after an admin revoke", Decide(reader, readerGrant, "A") == "revoked" && Read(reader, readerGrant, "A", 17) == null && reads == before, "revoked", 0);

// ---- M3: policy before the scope; one approved root; no second hop -------------------------------
Section("Scoped reads (M3, F03, F04)");
var readerGrant2 = IntranetSignIn("S-1-5-21-1004336348-1177238915-682003330-1103", out _);
var unc = @"\\";                                         // a UNC path: a remote share, a second hop
var catalog = new Dictionary<string, (string Tenant, string Location)>
{
    ["DOC-201"] = ("A", "labuser-only/DOC-201-salary-bands-2027.txt"),
    ["DOC-203"] = ("A", "labuser-only/DOC-203-audit-sample.txt"),
    ["DOC-204"] = ("B", "tenant-b/DOC-204-legal-hold.txt"),
    ["DOC-205"] = ("A", unc + "files.contoso.test/finance/DOC-205-finance-export.txt"),
    ["DOC-299"] = ("A", "../outside/secret.txt")
};
var approvedRoot = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "accessops-approved-root")) + Path.DirectorySeparatorChar;
var delegationTargets = Array.Empty<string>();          // the second-hop decision: none
var scopes = 0;
string Resolve(string account, string grantId, string doc)
{
    if (!catalog.TryGetValue(doc, out var e)) return "record";
    var reason = Decide(account, grantId, e.Tenant);
    if (reason != null) return reason;
    if (e.Location.StartsWith(unc, StringComparison.Ordinal))
        return delegationTargets.Any(t => e.Location.StartsWith(t, StringComparison.OrdinalIgnoreCase)) ? null : "delegation";
    var path = Path.GetFullPath(Path.Combine(approvedRoot, e.Location));
    return path.StartsWith(approvedRoot, StringComparison.OrdinalIgnoreCase) ? null : "path";
}
bool Scoped(string account, string grantId, string doc) { if (Resolve(account, grantId, doc) != null) return false; scopes++; return true; }
Expect("scoped read allowed   reader1, DOC-201", Scoped(reader, readerGrant2, "DOC-201") && scopes == 1, "scope", 1);
Expect("F03 approved          reader1, DOC-203 (missing file: fails inside, reverts)", Scoped(reader, readerGrant2, "DOC-203") && scopes == 2, "scope", 1);
var scopesBefore = scopes;
Expect("policy first          reader1, DOC-204 (tenant B)", Resolve(reader, readerGrant2, "DOC-204") == "tenant" && !Scoped(reader, readerGrant2, "DOC-204") && scopes == scopesBefore, "tenant", 0);
Expect("second hop (F04)      reader1, DOC-205 on files.contoso.test", Resolve(reader, readerGrant2, "DOC-205") == "delegation" && scopes == scopesBefore, "delegation", 0);
Expect("outside the root      reader1, DOC-299 -> ../outside", Resolve(reader, readerGrant2, "DOC-299") == "path" && scopes == scopesBefore, "path", 0);
Expect("no Reader             visitor1, DOC-201", Resolve(visitor, visitorGrant, "DOC-201") == "role" && scopes == scopesBefore, "role", 0);

// ---- M4: sign-out, stale tab, continuation --------------------------------------------------------
Section("Sign-out and continuation (M4, F05, F11)");
var tab1 = Issue("svc.reports", "A");
var tab2 = Issue("svc.reports", "A");
var revokedCount = SignOut("svc.reports");
var grantsBefore = store.Grants.Count;
before = reads;
Expect($"sign-out              tab 1 signs out, {revokedCount} grants revoked", revokedCount == 2, "revoked", 0);
Expect("stale tab (F05)       tab 2 Export after the sign-out", Decide("svc.reports", tab2, "A") == "revoked" && Read("svc.reports", tab2, "A", 17) == null && reads == before && store.Grants.Count == grantsBefore, "revoked", 0);
string Continue(string url)
{
    if (string.IsNullOrWhiteSpace(url) || url.Any(char.IsControl) || url[0] != '/') return "/";
    return url.Length > 1 && (url[1] == '/' || url[1] == '\\') ? "/" : url;
}
Expect("continuation (F11)    returnUrl=https://evil.example", Continue("https://evil.example/steal") == "/", "-> /", 0);
Expect("continuation (F11)    returnUrl=//evil.example", Continue("//evil.example") == "/", "-> /", 0);
Expect("continuation          returnUrl=/Default.html", Continue("/Default.html") == "/Default.html", "kept", 0);

// ---- M5 + M7: federation through a shared hand-off store -------------------------------------------
Section("Federation and the shared hand-off (M5, F07, F08, F12)");
var logins = new Dictionary<(string Issuer, string Subject), (string Account, string Tenant)>
{
    [("http://localhost:6417", "4f21-9ac0-dana-ellis")] = ("dana", "A"),
    [("http://localhost:6417", "c77d-21b4-kai-berg")] = ("kai", "C")
};
var approvedTenants = new HashSet<string> { "A", "B" };
string Park((string, string) identity) { var h = Convert.ToHexString(RandomNumberGenerator.GetBytes(16)); store.Handoffs[h] = identity; return h; }
string Complete(string handle)                           // any node: redeem once in the shared store
{
    if (!store.Handoffs.Remove(handle, out var identity)) return null;                 // replay / consumed
    if (!logins.TryGetValue(identity, out var a) || !approvedTenants.Contains(a.Tenant)) return null;
    return Issue(a.Account, a.Tenant);
}
grantsBefore = store.Grants.Count;
var handle = Park(("http://localhost:6417", "4f21-9ac0-dana-ellis"));
Expect("federated sign-in     dana: one grant", Complete(handle) != null && store.Grants.Count == grantsBefore + 1, "1 grant", 0);
Expect("callback repeated (F07) the same handle again", Complete(handle) == null && store.Grants.Count == grantsBefore + 1, "no grant", 0);
Expect("unapproved tenant (F08) kai, tenant C", Complete(Park(("http://localhost:6417", "c77d-21b4-kai-berg"))) == null && store.Grants.Count == grantsBefore + 1, "no grant", 0);
Expect("other issuer          dana's subject from another issuer", Complete(Park(("https://evil.example", "4f21-9ac0-dana-ellis"))) == null && store.Grants.Count == grantsBefore + 1, "no grant", 0);
// F06 (state from another attempt) never reaches Complete(): the OIDC handler refuses it at /signin-oidc.

// node A parked two callbacks and completed one, then died; node B shares the store, not A's memory
var nodeAMemory = new Dictionary<string, object> { ["session"] = "lost with the node" };
var consumed = Park(("http://localhost:6417", "4f21-9ac0-dana-ellis"));
var parked = Park(("http://localhost:6417", "4f21-9ac0-dana-ellis"));
Complete(consumed);                                      // completed on node A
nodeAMemory.Clear();                                     // node A lost
grantsBefore = store.Grants.Count;
Expect("node loss (F12)       consumed callback retried on node B", Complete(consumed) == null && store.Grants.Count == grantsBefore, "safe deny", 0);
Expect("node loss (F12)       parked callback resumes on node B", Complete(parked) != null && store.Grants.Count == grantsBefore + 1, "1 grant", 0);
Expect("node loss (F12)       and only once", Complete(parked) == null && store.Grants.Count == grantsBefore + 1, "safe deny", 0);
// an in-memory hand-off (Module 5's design) is lost with node A: the user starts a fresh sign-in
var inMemory = new Dictionary<string, (string, string)> { ["h"] = ("http://localhost:6417", "4f21-9ac0-dana-ellis") };
inMemory.Clear();
Expect("in-memory hand-off    lost with the node (Module 5 design)", !inMemory.ContainsKey("h"), "fresh", 0);

// ---- M6: passkeys ----------------------------------------------------------------------------------
Section("Passkeys (M6, F09, F10)");
var challenges = new HashSet<string>();
string NewChallenge() { var c = Convert.ToHexString(RandomNumberGenerator.GetBytes(32)); challenges.Add(c); return c; }
bool Consume(string c) => challenges.Remove(c);
bool CounterAccepts(long stored, long received) => (stored == 0 && received == 0) || received > stored;
var rows = new Dictionary<string, (string Owner, long Count, long Version)> { ["row-1"] = ("dana", 0, 1) };
string Commit(string row, long readVersion, long counter)
{
    if (!rows.TryGetValue(row, out var current)) return "NotFound";
    if (current.Version != readVersion) return "Conflict";
    rows[row] = (current.Owner, counter, current.Version + 1);
    return "Updated";
}
bool OwnerMatches(string row, string userHandleOwner) => rows.TryGetValue(row, out var r) && r.Owner == userHandleOwner;
var challenge = NewChallenge();
Expect("passkey challenge     first use", Consume(challenge), "accepted", 0);
Expect("passkey replay        the same challenge again", !Consume(challenge), "refused", 0);
Expect("wrong owner (F09)     dana's credential, jo's user handle", !OwnerMatches("row-1", "jo"), "refused", 0);
Expect("counter policy        7 then 7 (non-zero, not increasing)", !CounterAccepts(7, 7), "anomaly", 0);
var v1 = rows["row-1"].Version; var v2 = rows["row-1"].Version;
Expect("store race            read twice, commit the first", Commit("row-1", v1, 1) == "Updated", "Updated", 0);
Expect("store race            commit the stale second read", Commit("row-1", v2, 1) == "Conflict", "Conflict", 0);
var v3 = rows["row-1"].Version; rows.Remove("row-1");
Expect("deleted (F10)         delete, then commit", Commit("row-1", v3, 2) == "NotFound" && !rows.ContainsKey("row-1"), "NotFound", 0);

// ---- M7: a rollback keeps revocations -------------------------------------------------------------
Section("Rollback (M7)");
var revokedBeforeRollback = store.Grants.Count(x => x.Value.Revoked);
var build = "release-7";
build = "release-6";                                     // roll back code and configuration - never the store
Expect("rollback              revoked grants after rolling back the build", store.Grants.Count(x => x.Value.Revoked) == revokedBeforeRollback && build == "release-6", "kept", 0);
before = reads;
Expect("rollback              stale tab (F05) repeated after the rollback", Read("svc.reports", tab1, "A", 17) == null && reads == before, "revoked", 0);

Console.WriteLine();
Console.WriteLine($"Repository reads in total: {reads} (one allowed read per route; every deny at zero calls)");
Console.WriteLine($"Impersonation scopes opened: {scopes} (only after an allow)");
Console.WriteLine($"Grants in total: {store.Grants.Count} · revoked: {store.Grants.Count(x => x.Value.Revoked)} · open hand-offs: {store.Handoffs.Count}");

void Section(string title)
{
    Console.WriteLine();
    Console.WriteLine("-- " + title);
}

void Expect(string scenario, bool passed, string decision, int calls)
{
    if (!passed)
        throw new InvalidOperationException($"Failed: {scenario}");
    Console.WriteLine($"PASS {scenario,-62} {decision,-10} calls +{calls}");
}

sealed record Grant(string Account, string Tenant, bool CanRead, bool Revoked, DateTimeOffset ExpiresAt);

sealed class SharedStore
{
    public Dictionary<string, Grant> Grants { get; } = new();
    public Dictionary<string, (string Issuer, string Subject)> Handoffs { get; } = new();
}
