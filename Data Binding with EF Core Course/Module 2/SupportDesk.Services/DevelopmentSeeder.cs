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
/// sixty tickets (a few with comments), deterministic and realistic so every screenshot of the course looks
/// the same. One context from the factory, one <c>AnyAsync</c> check, one <c>AddRange</c>, one
/// <c>SaveChangesAsync</c>, disposed at the end — idempotent by construction: a second call adds nothing.
/// </summary>
/// <remarks>
/// It is a service, not a migration: fixed lookup rows could travel with a migration through
/// <c>HasData</c>, but sixty sample tickets do not belong in production schema history. The host calls
/// it at start in Development (never in Production) and the page's <c>btnSeed</c> calls it on demand.
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

    // (title, category index) — sixty realistic support requests.
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

    private static readonly string[] Statuses = { "Open", "In Progress", "Waiting", "Closed" };
    private static readonly string[] Priorities = { "Low", "Normal", "High" };

    public static List<Ticket> Tickets(List<Customer> customers, List<Agent> agents, List<Category> categories)
    {
        var today = DateTime.UtcNow.Date;
        var tickets = new List<Ticket>(Requests.Length);

        for (var i = 0; i < Requests.Length; i++)
        {
            var (title, category) = Requests[i];
            var status = Statuses[(i * 7 + i / 4) % Statuses.Length];          // mixed, deterministic
            var priority = Priorities[(i * 5 + i / 3) % Priorities.Length];
            var createdAt = today.AddDays(-(i % 45) - 1).AddHours(8 + i % 9).AddMinutes((i * 13) % 60);
            var unassigned = i % 10 is 3 or 6 or 9;                            // 30 % without an agent
            var hasDueDate = i % 3 != 1;                                        // two thirds carry a due date

            var ticket = new Ticket
            {
                Number = $"SD-{1001 + i}",
                Title = title,
                Description = i % 4 == 0 ? $"Reported by the customer's service desk. Reference {1001 + i}." : null,
                Status = status,
                Priority = priority,
                IsUrgent = priority == "High" && status != "Closed" && i % 2 == 0,
                DueDate = hasDueDate ? createdAt.Date.AddDays(priority == "High" ? 2 : priority == "Normal" ? 5 : 10) : null,
                CreatedAt = createdAt,
                UpdatedAt = status == "Open" ? createdAt : createdAt.AddHours(3 + i % 30),
                Customer = customers[i % customers.Count],
                Category = categories[category],
                Agent = unassigned ? null : agents[i % agents.Count],
            };

            if (i % 5 == 0)
            {
                var author = ticket.Agent?.DisplayName ?? "Service desk";
                ticket.Comments.Add(new TicketComment { Author = author, Body = "Acknowledged. Gathering logs and device details from the customer.", CreatedAt = createdAt.AddMinutes(20) });
                if (status is "Waiting" or "Closed")
                    ticket.Comments.Add(new TicketComment { Author = author, Body = status == "Closed" ? "Fix confirmed with the customer. Closing." : "Waiting for the customer to confirm a maintenance window.", CreatedAt = createdAt.AddHours(2) });
            }

            tickets.Add(ticket);
        }

        return tickets;
    }
}
