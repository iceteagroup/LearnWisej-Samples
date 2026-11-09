// AccessOps PolicyHarness - Module 4.
// The same allow/deny rules the AccessOps services apply, run without Wisej.NET, IIS or an
// identity provider. Module 1's read rule (grant, read right, tenant, then the repository) plus
// the Module 4 rules: a login grant is opaque, expiring and revocable; sign-out revokes every
// grant of the account, so a stale tab is denied at zero repository calls (F05); and a
// continuation URL is a local path or "/" (F11). Adapted from the course pack's
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
Console.WriteLine("AccessOps PolicyHarness · Module 4 · " + System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription);
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

Console.WriteLine();
Console.WriteLine($"Repository reads in total: {reads} (one allowed export, every deny at zero calls)");

void Expect(string scenario, bool passed, string decision, int repositoryCalls)
{
    if (!passed)
    {
        throw new InvalidOperationException($"Failed: {scenario}");
    }

    Console.WriteLine($"PASS {scenario,-58} {decision,-8} repository calls +{repositoryCalls}");
}

sealed record Grant(string Account, string Tenant, bool CanRead, bool Revoked, DateTimeOffset ExpiresAt);
