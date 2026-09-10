using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using SupportDesk.Data;
using SupportDesk.Data.Diagnostics;

namespace SupportDesk.Services;

/// <summary>What one seed run did. <see cref="Seeded"/> is false when the database already had tickets.</summary>
public sealed record SeedResult(bool Seeded, int Customers, int Agents, int Categories, int Tickets, int Comments, int ExistingTickets, double ElapsedMs)
{
    public string Describe() => Seeded
        ? $"seeded {Customers} customers, {Agents} agents, {Categories} categories, {Tickets} tickets, {Comments} comments in {ElapsedMs:0} ms"
        : $"nothing to seed: {ExistingTickets} tickets already exist — Tickets.AnyAsync() was true, so the seeder returned before AddRange";
}

/// <summary>
/// Development-only seed for the Support Desk console: five customers, three agents, six categories and
/// <b>312 tickets</b> (some with comments), deterministic and realistic so every screenshot of the course
/// looks the same. One context from the factory, one <c>AnyAsync</c> check, one <c>AddRange</c>, one
/// <c>SaveChangesAsync</c>, disposed at the end — idempotent by construction: a second call adds nothing.
/// </summary>
/// <remarks>
/// <para>
/// It is a service, not a migration: fixed lookup rows could travel with a migration through
/// <c>HasData</c>, but three hundred sample tickets do not belong in production schema history. The host
/// calls it at start in Development (never in Production) and the page's <c>btnSeed</c> calls it on demand.
/// </para>
/// <para>
/// Module 3 raised the count from sixty to <see cref="SampleData.TicketCount"/> so the ticket browser has
/// seven pages to move through at a page size of fifty — "Showing 50 of 312" is the status line the
/// walkthrough shows, and paging that only ever has two pages proves nothing.
/// </para>
/// </remarks>
public sealed class DevelopmentSeeder
{
    private readonly IDbContextFactory<SupportDeskContext> _dbFactory;

    public DevelopmentSeeder(IDbContextFactory<SupportDeskContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    /// <summary>Inserts the sample data unless tickets already exist. Returns what happened.</summary>
    public async Task<SeedResult> SeedDevelopmentDataAsync(CancellationToken token = default)
    {
        var watch = Stopwatch.StartNew();
        await using var db = await _dbFactory.CreateDbContextAsync(token);

        if (await db.Tickets.AnyAsync(token))
        {
            var existing = await db.Tickets.CountAsync(token);
            QueryTrace.Note($"tickets already exist ({existing}) — returning without AddRange");
            return new SeedResult(false, 0, 0, 0, 0, 0, existing, watch.Elapsed.TotalMilliseconds);
        }

        var customers = SampleData.Customers();
        var agents = SampleData.Agents();
        var categories = SampleData.Categories();
        var tickets = SampleData.Tickets(customers, agents, categories);
        var comments = tickets.Sum(t => t.Comments.Count);

        QueryTrace.Note($"AddRange: {customers.Count} customers, {agents.Count} agents, {categories.Count} categories, {tickets.Count} tickets ({comments} comments) → one SaveChangesAsync");

        db.Customers.AddRange(customers);
        db.Agents.AddRange(agents);
        db.Categories.AddRange(categories);
        db.Tickets.AddRange(tickets);          // comments travel with their tickets through the navigation
        await db.SaveChangesAsync(token);

        return new SeedResult(true, customers.Count, agents.Count, categories.Count, tickets.Count, comments, 0, watch.Elapsed.TotalMilliseconds);
    }

    /// <summary>
    /// Lab prop: empties every table (in foreign-key order) so the delete demos can be run again, then seeds.
    /// Two operations, two contexts. Development only — nothing in production ever bulk-deletes tickets.
    /// </summary>
    public async Task<SeedResult> ResetDevelopmentDataAsync(CancellationToken token = default)
    {
        await using (var db = await _dbFactory.CreateDbContextAsync(token))
        {
            QueryTrace.Note("ExecuteDeleteAsync on TicketComments, Tickets, Customers, Agents, Categories (children first)");
            await db.TicketComments.ExecuteDeleteAsync(token);
            await db.Tickets.ExecuteDeleteAsync(token);
            await db.Customers.ExecuteDeleteAsync(token);
            await db.Agents.ExecuteDeleteAsync(token);
            await db.Categories.ExecuteDeleteAsync(token);
        }

        return await SeedDevelopmentDataAsync(token);
    }
}

/// <summary>The deterministic Support Desk sample data.</summary>
internal static class SampleData
{
    public static List<Customer> Customers() => new()
    {
        new Customer { Name = "Halden Logistics", Email = "it@halden-logistics.example" },
        new Customer { Name = "Brightwater Clinics", Email = "helpdesk@brightwater.example" },
        new Customer { Name = "Northgate Retail Group", Email = "servicedesk@northgate.example" },
        new Customer { Name = "Verity Insurance", Email = "support@verity-ins.example" },
        new Customer { Name = "Copperfield Engineering", Email = "ops@copperfield.example" },
    };

    public static List<Agent> Agents() => new()
    {
        new Agent { DisplayName = "Priya Natarajan", Email = "priya.natarajan@supportdesk.example" },
        new Agent { DisplayName = "Tomasz Wierzbicki", Email = "tomasz.wierzbicki@supportdesk.example" },
        new Agent { DisplayName = "Elena Marchetti", Email = "elena.marchetti@supportdesk.example" },
    };

    public static List<Category> Categories() => new()
    {
        new Category { Name = "Hardware" },
        new Category { Name = "Network" },
        new Category { Name = "Accounts & Access" },
        new Category { Name = "Email & Collaboration" },
        new Category { Name = "Business Applications" },
        new Category { Name = "Printing" },
    };

    // (title, category index) — sixty realistic support requests, reused across five rounds with a site
    // qualifier so the browser has 312 rows without 312 invented sentences.
    private static readonly (string Title, int Category)[] Requests =
    {
        ("Printer offline on floor 2", 5), ("VPN drops every hour on the Copenhagen link", 1), ("Password reset for finance shared mailbox", 2),
        ("Laptop battery swells on the 14-inch fleet", 0), ("Outlook rules vanished after the update", 3), ("Badge reader ignores new hires", 2),
        ("Shared drive shows read-only for the sales team", 1), ("Monitor flickers after sleep on docking station", 0), ("Guest Wi-Fi captive page loops", 1),
        ("Invoice export times out after 30 seconds", 4), ("Two-factor codes arrive late", 2), ("Conference room screen shows no signal", 0),
        ("Teams calls drop when switching networks", 3), ("Label printer prints blank pages", 5), ("Payroll report shows last month's figures", 4),
        ("New starter needs CRM access", 2), ("Keyboard keys stick on the warehouse terminal", 0), ("Calendar invites arrive as plain text", 3),
        ("Switch port 12 keeps flapping", 1), ("ERP login rejects valid credentials", 4), ("Toner low warning never clears", 5),
        ("Leaver's account still active after 14 days", 2), ("Docking station drops the second monitor", 0), ("Shared mailbox not receiving external mail", 3),
        ("Site-to-site tunnel down to the Leeds branch", 1), ("Purchase order screen freezes on save", 4), ("Scanner to email fails with 550", 5),
        ("Headset not detected by softphone", 0), ("Distribution list missing three members", 3), ("Approval workflow stuck at manager step", 4),
        ("MFA device lost, needs re-enrolment", 2), ("Slow file copies from the archive server", 1), ("Projector remote not pairing", 0),
        ("Out-of-office reply not sent externally", 3), ("Stock report exports duplicate rows", 4), ("Colour printer prints with a magenta tint", 5),
        ("Service account password expires tomorrow", 2), ("Intermittent packet loss on VLAN 40", 1), ("Tablet will not charge on the ward cart", 0),
        ("Meeting room booking panel offline", 3), ("Timesheet app rejects overtime entries", 4), ("Print queue stuck with 40 jobs", 5),
        ("Contractor needs temporary VPN access", 2), ("DHCP scope exhausted in the training room", 1), ("Barcode scanner double-scans", 0),
        ("Shared calendar permissions reset", 3), ("Dashboard tiles show stale data", 4), ("Duplex printing not available on the finance printer", 5),
        ("Group membership change not applied", 2), ("Wireless roaming fails between floors", 1), ("Desk phone displays 'Registering'", 0),
        ("Large attachments bounce at 25 MB", 3), ("Customer portal returns a 502 after login", 4), ("Plotter feeds paper at an angle", 5),
        ("Admin rights requested for local install", 2), ("Firewall blocks the new payment gateway", 1), ("Webcam shows black image in meetings", 0),
        ("Retention policy deleted a shared folder", 3), ("Month-end batch job fails on step 7", 4), ("Receipt printer cuts paper mid-line", 5),
    };

    // Where a repeated request came from, so round 2 of "Printer offline on floor 2" is still a sentence a
    // service desk would write.
    private static readonly string[] Sites =
    {
        "Copenhagen office", "Leeds branch", "the warehouse", "the training room", "reception",
        "the finance floor", "ward 3", "the night shift", "the Dublin site", "the loading bay",
    };

    // The five statuses of TicketStatuses.All, weighted the way a live queue looks: mostly open and in
    // progress, a tail of resolved and closed. Every one of the five occurs, so the status lookup always
    // has something to find.
    private static readonly string[] WeightedStatuses =
    {
        TicketStatuses.Open, TicketStatuses.Open, TicketStatuses.Open,
        TicketStatuses.InProgress, TicketStatuses.InProgress,
        TicketStatuses.Waiting,
        TicketStatuses.Resolved, TicketStatuses.Resolved,
        TicketStatuses.Closed, TicketStatuses.Closed,
    };

    private static readonly string[] Priorities = { "Low", "Normal", "High" };

    /// <summary>How many tickets the development database holds — seven pages of fifty, plus twelve.</summary>
    public const int TicketCount = 312;

    /// <summary>Fixed <see cref="Random"/> seed: the same 312 tickets on every machine, on every reseed.</summary>
    private const int RandomSeed = 20260910;

    /// <summary>
    /// 312 deterministic tickets. The randomness is seeded, so the data is identical everywhere, and
    /// <c>UpdatedAt</c> is strictly decreasing with the index: the browser's default sort
    /// (<c>ORDER BY "UpdatedAt" DESC</c>) has no ties, so page 2 always shows the next fifty rows and a
    /// test can assert exactly which ones.
    /// </summary>
    public static List<Ticket> Tickets(List<Customer> customers, List<Agent> agents, List<Category> categories)
    {
        var random = new Random(RandomSeed);
        var today = DateTime.UtcNow.Date;
        var newest = today.AddHours(16);                                        // the most recently touched ticket
        var tickets = new List<Ticket>(TicketCount);

        for (var i = 0; i < TicketCount; i++)
        {
            var (request, category) = Requests[i % Requests.Length];
            var round = i / Requests.Length;
            var title = round == 0 ? request : $"{request} ({Sites[random.Next(Sites.Length)]})";

            // i * 37 minutes apart with at most 29 minutes of jitter: unique, ordered, and irregular.
            var updatedAt = newest.AddMinutes(-(i * 37 + random.Next(0, 30)));
            var createdAt = updatedAt.AddDays(-random.Next(0, 40)).AddHours(-random.Next(1, 9));

            var status = WeightedStatuses[random.Next(WeightedStatuses.Length)];
            var priority = Priorities[random.Next(Priorities.Length)];
            var unassigned = random.Next(10) < 3;                               // ~30 % without an agent
            var hasDueDate = random.Next(3) != 0;                               // two thirds carry a due date
            var dueOffset = random.Next(-12, 30);                               // some already overdue, most in the next weeks

            var ticket = new Ticket
            {
                Number = $"SD-{1001 + i}",
                Title = title,
                Description = i % 4 == 0 ? $"Reported by the customer's service desk. Reference {1001 + i}." : null,
                Status = status,
                Priority = priority,
                IsUrgent = priority == "High" && status is not (TicketStatuses.Closed or TicketStatuses.Resolved) && random.Next(2) == 0,
                DueDate = hasDueDate ? today.AddDays(dueOffset) : null,
                CreatedAt = createdAt,
                UpdatedAt = updatedAt,
                Customer = customers[random.Next(customers.Count)],
                Category = categories[category],
                Agent = unassigned ? null : agents[random.Next(agents.Count)],
            };

            if (i % 5 == 0)
            {
                var author = ticket.Agent?.DisplayName ?? "Service desk";
                ticket.Comments.Add(new TicketComment { Author = author, Body = "Acknowledged. Gathering logs and device details from the customer.", CreatedAt = createdAt.AddMinutes(20) });
                if (status is TicketStatuses.Waiting or TicketStatuses.Closed or TicketStatuses.Resolved)
                {
                    ticket.Comments.Add(new TicketComment
                    {
                        Author = author,
                        Body = status == TicketStatuses.Waiting
                            ? "Waiting for the customer to confirm a maintenance window."
                            : "Fix confirmed with the customer.",
                        CreatedAt = createdAt.AddHours(2)
                    });
                }
            }

            tickets.Add(ticket);
        }

        return tickets;
    }
}
