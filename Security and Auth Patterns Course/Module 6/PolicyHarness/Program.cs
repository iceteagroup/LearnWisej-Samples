// AccessOps PolicyHarness - Module 6.
// The same allow/deny rules the AccessOps services apply, run without Wisej.NET, IIS or an
// identity provider. Module 1's read rule (grant, read right, tenant, then the repository) plus
// the Module 4 rules: a login grant is opaque, expiring and revocable; sign-out revokes every
// grant of the account, so a stale tab is denied at zero repository calls (F05); and a
// continuation URL is a local path or "/" (F11). Module 5 adds the federation rule: a validated
// issuer + subject becomes exactly one grant only through an approved external_logins row, a
// hand-off redeems once, and every refused callback ends with no grant. Module 6 adds the passkey
// rules AccessOps owns around WebAuthn.Validate: a challenge is accepted once, a non-zero counter
// must increase, and the commit is a compare-and-swap that only an unchanged row passes.
// Adapted from the course pack's
// code_snippets/PolicyHarness/Program.cs.

using System.Security.Cryptography;

var grants = new Dictionary<string, Grant>();
var fixtures = new Dictionary<(string Tenant, int Id), string>
{
    [("A", 17)] = "Tenant A fixture: Supplier contract — Fabrikam",
    [("B", 17)] = "Tenant B fixture: Supplier contract — Tailspin logistics"
};

var now = DateTimeOffset.UnixEpoch;
var reads = 0;   // the read-counting repository

string Issue(string account, string tenant, bool canRead = true)
{
    var id = "G-" + Convert.ToHexString(RandomNumberGenerator.GetBytes(16)).ToLowerInvariant();   // opaque, random
    grants[id] = new Grant(account, tenant, canRead, false, now.AddHours(8));
    return id;
}

int SignOut(string account)
{
    var ids = grants.Where(g => g.Value.Account == account && !g.Value.Revoked).Select(g => g.Key).ToList();
    foreach (var id in ids)
        grants[id] = grants[id] with { Revoked = true };
    return ids.Count;
}

string Export(string account, string grantId, string tenant, int recordId)
{
    if (!grants.TryGetValue(grantId, out var grant) ||
        grant.Account != account || grant.Revoked || grant.ExpiresAt <= now ||
        !grant.CanRead || !StringComparer.Ordinal.Equals(grant.Tenant, tenant))
    {
        return null;   // denied before the repository
    }

    reads++;           // only now does the repository run
    return fixtures.GetValueOrDefault((tenant, recordId));
}

string Continue(string returnUrl)
{
    if (string.IsNullOrWhiteSpace(returnUrl) || returnUrl.Any(char.IsControl) || returnUrl[0] != '/')
        return "/";
    return returnUrl.Length > 1 && (returnUrl[1] == '/' || returnUrl[1] == '\\') ? "/" : returnUrl;
}

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("AccessOps PolicyHarness · Module 6 · " + System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription);
Console.WriteLine();

var tab1 = Issue("ana", "A");
var tab2 = Issue("ana", "A");
Expect("partner export        tab 1 grant, tenant A, record 17", Export("ana", tab1, "A", 17) != null && reads == 1, "allowed", 1);

var before = reads;
Expect("tenant boundary (F02) tab 1 grant, tenant B, record 17", Export("ana", tab1, "B", 17) == null && reads == before, "denied", reads - before);
Expect("foreign grant claim   kai presents ana's grant ID", Export("kai", tab1, "A", 17) == null && reads == before, "denied", reads - before);
Expect("unknown grant         a grant ID nobody issued", Export("ana", "G-00000000000000000000000000000000", "A", 17) == null && reads == before, "denied", reads - before);

now = now.AddHours(8);
Expect("expired grant         tab 1 grant at its expiry", Export("ana", tab1, "A", 17) == null && reads == before, "denied", reads - before);
now = DateTimeOffset.UnixEpoch;

var revoked = SignOut("ana");
Expect($"sign-out              tab 1 signs out, {revoked} grants revoked", revoked == 2, "revoked", 0);
Expect("stale tab (F05)       tab 2 Export after the sign-out", Export("ana", tab2, "A", 17) == null && reads == before, "denied", reads - before);
Expect("no new grant          grants after the stale export", grants.Count == 2, "2 total", 0);

Expect("continuation (F11)    returnUrl=//evil.example", Continue("//evil.example") == "/", "→ /", 0);
Expect("continuation (F11)    returnUrl=/\\evil.example", Continue("/\\evil.example") == "/", "→ /", 0);
Expect("continuation          returnUrl=/Default.html", Continue("/Default.html") == "/Default.html", "kept", 0);

// ---- Module 5: issuer + subject -> one grant -------------------------------------------------------
var logins = new Dictionary<(string Issuer, string Subject), (string Account, string Tenant)>
{
    [("http://localhost:6415", "4f21-9ac0-dana-ellis")] = ("dana", "A"),
    [("http://localhost:6415", "c77d-21b4-kai-berg")] = ("kai", "C")
};
var approvedTenants = new HashSet<string> { "A", "B" };
var handoffs = new Dictionary<string, (string Issuer, string Subject)>();

string Park(string issuer, string subject)
{
    var handle = Convert.ToHexString(RandomNumberGenerator.GetBytes(16));
    handoffs[handle] = (issuer, subject);
    return handle;
}

string Complete(string handle)   // the page load: redeem once, map, one grant
{
    if (!handoffs.Remove(handle, out var identity))
        return null;                                         // replayed hand-off (F07)
    if (!logins.TryGetValue(identity, out var account) || !approvedTenants.Contains(account.Tenant))
        return null;                                         // unknown login or unapproved tenant (F08)
    return Issue(account.Account, account.Tenant);
}

var grantsBefore = grants.Count;
var handle = Park("http://localhost:6415", "4f21-9ac0-dana-ellis");
Expect("federated sign-in     dana: iss + sub mapped, tenant A", Complete(handle) != null && grants.Count == grantsBefore + 1, "1 grant", 0);
Expect("replayed hand-off (F07) the same handle again", Complete(handle) == null && grants.Count == grantsBefore + 1, "no grant", 0);
Expect("unapproved tenant (F08) kai: iss + sub mapped, tenant C", Complete(Park("http://localhost:6415", "c77d-21b4-kai-berg")) == null && grants.Count == grantsBefore + 1, "no grant", 0);
Expect("other issuer          dana's subject from another issuer", Complete(Park("https://evil.example", "4f21-9ac0-dana-ellis")) == null && grants.Count == grantsBefore + 1, "no grant", 0);
// F06 (a state from another browser attempt) and a provider denial never reach Complete():
// the OpenID Connect handler refuses them at the callback - see AccessOps.Tests/ExternalSignInTests.cs.

// ---- Module 6: one-time challenges, counter policy, compare-and-swap ---------------------------
var challenges = new HashSet<string>();
string NewChallenge() { var c = Convert.ToHexString(RandomNumberGenerator.GetBytes(32)); challenges.Add(c); return c; }
bool Consume(string c) => challenges.Remove(c);
bool CounterAccepts(long stored, long received) => (stored == 0 && received == 0) || received > stored;
var rows = new Dictionary<string, (long Count, long Version)> { ["row-1"] = (0, 1) };
string Commit(string row, long readVersion, long counter)
{
    if (!rows.TryGetValue(row, out var current)) return "NotFound";
    if (current.Version != readVersion) return "Conflict";
    rows[row] = (counter, current.Version + 1);
    return "Updated";
}

var challenge = NewChallenge();
Expect("passkey challenge     first use of a fresh challenge", Consume(challenge), "accepted", 0);
Expect("replayed challenge    the same challenge again", !Consume(challenge), "refused", 0);
Expect("counter policy        0 then 0 (authenticator without counter)", CounterAccepts(0, 0), "accepted", 0);
Expect("counter policy        7 then 7 (non-zero, not increasing)", !CounterAccepts(7, 7), "anomaly", 0);
var read1 = rows["row-1"].Version; var read2 = rows["row-1"].Version;
Expect("store race            read twice, commit the first", Commit("row-1", read1, 1) == "Updated", "Updated", 0);
Expect("store race            commit the stale second read", Commit("row-1", read2, 1) == "Conflict", "Conflict", 0);
var read3 = rows["row-1"].Version; rows.Remove("row-1");
Expect("store race            delete, then commit", Commit("row-1", read3, 2) == "NotFound" && !rows.ContainsKey("row-1"), "NotFound", 0);

Console.WriteLine();
Console.WriteLine($"Repository reads in total: {reads} (one allowed export, every deny at zero calls)");
Console.WriteLine($"Grants in total: {grants.Count} (two Module 4 sign-ins + one federated sign-in)");

void Expect(string scenario, bool passed, string decision, int repositoryCalls)
{
    if (!passed)
    {
        throw new InvalidOperationException($"Failed: {scenario}");
    }

    Console.WriteLine($"PASS {scenario,-58} {decision,-8} repository calls +{repositoryCalls}");
}

sealed record Grant(string Account, string Tenant, bool CanRead, bool Revoked, DateTimeOffset ExpiresAt);
