// AccessOps PolicyHarness - Module 1.
// The same allow/deny rule DocumentService.Read applies, run without Wisej.NET, IIS or an
// identity provider: load the current grant, refuse a missing, revoked or expired grant, a missing
// read right or a tenant mismatch, and only then call the repository. Every deny is proven at zero
// repository calls. Adapted from the course pack's code_snippets/PolicyHarness/Program.cs.

var grants = new Dictionary<string, Grant>
{
    ["reader"] = new("A", true, false, DateTimeOffset.UnixEpoch.AddHours(1)),
    ["visitor"] = new("A", false, false, DateTimeOffset.UnixEpoch.AddHours(1))
};

var fixtures = new Dictionary<(string Tenant, int Id), string>
{
    [("A", 17)] = "Tenant A fixture: Supplier contract — Northwind renewal",
    [("B", 17)] = "Tenant B fixture: Supplier contract — Fabrikam logistics"
};

var now = DateTimeOffset.UnixEpoch;
var reads = 0;   // the read-counting repository

string Read(string grantId, string tenant, int recordId)
{
    if (!grants.TryGetValue(grantId, out var grant) ||
        grant.Revoked || grant.ExpiresAt <= now ||
        !grant.CanRead || !StringComparer.Ordinal.Equals(grant.Tenant, tenant))
    {
        return null;   // denied before the repository
    }

    reads++;           // only now does the repository run
    return fixtures.GetValueOrDefault((tenant, recordId));
}

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("AccessOps PolicyHarness · Module 1 · " + System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription);
Console.WriteLine();

var content = Read("reader", "A", 17);
Expect("authorized reader     reader grant, tenant A, record 17", content == "Tenant A fixture: Supplier contract — Northwind renewal" && reads == 1, "allowed", 1);

var before = reads;
Expect("tenant boundary (F02) reader grant, tenant B, record 17", Read("reader", "B", 17) == null && reads == before, "denied", reads - before);
Expect("missing role          visitor grant (no read right)", Read("visitor", "A", 17) == null && reads == before, "denied", reads - before);
Expect("unknown grant         no such grant ID", Read("unknown", "A", 17) == null && reads == before, "denied", reads - before);

now = now.AddHours(1);
Expect("expired grant         reader grant at its expiry", Read("reader", "A", 17) == null && reads == before, "denied", reads - before);
now = DateTimeOffset.UnixEpoch;

grants["reader"] = grants["reader"] with { Revoked = true };
Expect("revoked grant (F05)   reader grant revoked, A / 17", Read("reader", "A", 17) == null && reads == before, "denied", reads - before);

Console.WriteLine();
Console.WriteLine($"Repository reads in total: {reads} (one allowed read, every deny at zero calls)");

void Expect(string scenario, bool passed, string decision, int repositoryCalls)
{
    if (!passed)
    {
        throw new InvalidOperationException($"Failed: {scenario}");
    }

    Console.WriteLine($"PASS {scenario,-58} {decision,-8} repository calls +{repositoryCalls}");
}

sealed record Grant(string Tenant, bool CanRead, bool Revoked, DateTimeOffset ExpiresAt);
