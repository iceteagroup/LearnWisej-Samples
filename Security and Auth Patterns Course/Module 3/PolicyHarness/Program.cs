// AccessOps PolicyHarness - Module 3 (Module 1 rule + the Module 2 and Module 3 cases).
// The same allow/deny rule DocumentService.Read applies, run without Wisej.NET, IIS or an
// identity provider: load the current grant, refuse a missing, revoked or expired grant, a missing
// read right or a tenant mismatch, and only then call the repository. Every deny is proven at zero
// repository calls. Adapted from the course pack's code_snippets/PolicyHarness/Program.cs.
// Module 2 adds the intranet rule: a host-authenticated Windows identity is evidence, not a
// permission. Its SID is looked up (AccessGrants.FindForAccount) and an account without a Reader
// grant is denied with zero repository reads, exactly like an anonymous caller.

var grants = new Dictionary<string, Grant>
{
    ["reader"] = new("A", true, false, DateTimeOffset.UnixEpoch.AddHours(1)),
    ["visitor"] = new("A", false, false, DateTimeOffset.UnixEpoch.AddHours(1))
};

// Windows account SID -> AccessOps grant ID (null: the account exists but has no grant).
// Invented SIDs; the domain lab uses CONTOSO\reader1 and CONTOSO\visitor1.
var windowsAccounts = new Dictionary<string, string>
{
    ["S-1-5-21-1004336348-1177238915-682003330-1103"] = "reader",   // CONTOSO\reader1
    ["S-1-5-21-1004336348-1177238915-682003330-1104"] = null        // CONTOSO\visitor1
};

string FindForAccount(string sid) => windowsAccounts.GetValueOrDefault(sid);

var fixtures = new Dictionary<(string Tenant, int Id), string>
{
    [("A", 17)] = "Tenant A fixture: Supplier contract — Northwind renewal",
    [("B", 17)] = "Tenant B fixture: Supplier contract — Fabrikam logistics"
};

var now = DateTimeOffset.UnixEpoch;
var reads = 0;   // the read-counting repository

string Read(string grantId, string tenant, int recordId)
{
    if (grantId == null || !grants.TryGetValue(grantId, out var grant) ||
        grant.Revoked || grant.ExpiresAt <= now ||
        !grant.CanRead || !StringComparer.Ordinal.Equals(grant.Tenant, tenant))
    {
        return null;   // denied before the repository
    }

    reads++;           // only now does the repository run
    return fixtures.GetValueOrDefault((tenant, recordId));
}

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("AccessOps PolicyHarness · Module 3 · " + System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription);
Console.WriteLine();

var content = Read("reader", "A", 17);
Expect("authorized reader     reader grant, tenant A, record 17", content == "Tenant A fixture: Supplier contract — Northwind renewal" && reads == 1, "allowed", 1);

var before = reads;
Expect("tenant boundary (F02) reader grant, tenant B, record 17", Read("reader", "B", 17) == null && reads == before, "denied", reads - before);
Expect("missing role          visitor grant (no read right)", Read("visitor", "A", 17) == null && reads == before, "denied", reads - before);
Expect("unknown grant         no such grant ID", Read("unknown", "A", 17) == null && reads == before, "denied", reads - before);

// Module 2: authenticated by the host (Negotiate/NTLM or Kerberos), but no Reader grant.
var visitorGrant = FindForAccount("S-1-5-21-1004336348-1177238915-682003330-1104");
Expect("authenticated, no Reader (M2) CONTOSO\\visitor1, A / 17", Read(visitorGrant, "A", 17) == null && reads == before, "denied", reads - before);
var unmappedGrant = FindForAccount("S-1-5-21-1004336348-1177238915-682003330-1188");
Expect("authenticated, unmapped (M2)  Windows SID with no account", Read(unmappedGrant, "A", 17) == null && reads == before, "denied", reads - before);

now = now.AddHours(1);
Expect("expired grant         reader grant at its expiry", Read("reader", "A", 17) == null && reads == before, "denied", reads - before);
now = DateTimeOffset.UnixEpoch;

grants["reader"] = grants["reader"] with { Revoked = true };
Expect("revoked grant (F05)   reader grant revoked, A / 17", Read("reader", "A", 17) == null && reads == before, "denied", reads - before);

// Module 3: the application decision must allow the read before any impersonation scope opens.
// ResolveApprovedPath = policy (grant, read right, tenant) + catalog + canonical path under the root;
// "scopes" counts what WindowsIdentity.RunImpersonated would have been asked to open.
now = DateTimeOffset.UnixEpoch;
grants["reader"] = grants["reader"] with { Revoked = false };
var catalog = new Dictionary<string, (string Tenant, string RelativePath)>
{
    ["DOC-201"] = ("A", "labuser-only/DOC-201-salary-bands-2027.txt"),
    ["DOC-203"] = ("A", "labuser-only/DOC-203-audit-sample.txt"),
    ["DOC-204"] = ("B", "tenant-b/DOC-204-legal-hold.txt"),
    ["DOC-299"] = ("A", "../outside/secret.txt")   // a bad catalog row: must not escape the root
};
var approvedRoot = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "accessops-approved-root")) + Path.DirectorySeparatorChar;
var scopes = 0;

string ResolveApprovedPath(string grantId, string documentId)
{
    if (!catalog.TryGetValue(documentId, out var entry))
        return null;                                                       // unknown record
    if (grantId == null || !grants.TryGetValue(grantId, out var grant) ||
        grant.Revoked || grant.ExpiresAt <= now || !grant.CanRead ||
        !StringComparer.Ordinal.Equals(grant.Tenant, entry.Tenant))
        return null;                                                       // policy deny
    var path = Path.GetFullPath(Path.Combine(approvedRoot, entry.RelativePath));
    return path.StartsWith(approvedRoot, StringComparison.OrdinalIgnoreCase) ? path : null;
}

bool ScopedRead(string grantId, string documentId)
{
    var path = ResolveApprovedPath(grantId, documentId);
    if (path == null)
        return false;                                                      // no scope opened
    scopes++;                                                              // RunImpersonated would run here
    return true;
}

Console.WriteLine();
Expect("scoped read allowed (M3)   reader, DOC-201 (tenant A)", ScopedRead("reader", "DOC-201") && scopes == 1, "allowed", scopes);
Expect("F03 is approved (M3)       reader, DOC-203 (file missing)", ScopedRead("reader", "DOC-203") && scopes == 2, "allowed", 1);
var scopesBefore = scopes;
Expect("policy first (M3)          reader, DOC-204 (tenant B)", !ScopedRead("reader", "DOC-204") && scopes == scopesBefore, "denied", scopes - scopesBefore);
Expect("no Reader (M3)             visitor1 grant, DOC-201", !ScopedRead(visitorGrant, "DOC-201") && scopes == scopesBefore, "denied", scopes - scopesBefore);
Expect("unknown ID (M3)            reader, DOC-999", !ScopedRead("reader", "DOC-999") && scopes == scopesBefore, "denied", scopes - scopesBefore);
Expect("outside the root (M3)      reader, DOC-299 -> ../outside", !ScopedRead("reader", "DOC-299") && scopes == scopesBefore, "denied", scopes - scopesBefore);
Console.WriteLine($"Impersonation scopes opened: {scopes} (only after an allow; every deny at zero scopes)");

Console.WriteLine();
Console.WriteLine($"Repository reads in total: {reads} (one allowed read, every deny at zero calls)");

void Expect(string scenario, bool passed, string decision, int calls)
{
    if (!passed)
    {
        throw new InvalidOperationException($"Failed: {scenario}");
    }

    Console.WriteLine($"PASS {scenario,-58} {decision,-8} {(scenario.Contains("(M3)") ? "scopes" : "repository calls")} +{calls}");
}

sealed record Grant(string Tenant, bool CanRead, bool Revoked, DateTimeOffset ExpiresAt);
