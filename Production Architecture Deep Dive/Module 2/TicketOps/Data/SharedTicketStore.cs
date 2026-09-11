using System;
using System.Collections.Generic;
using System.Linq;
using TicketOps.Domain;

namespace TicketOps.Data
{
    /// <summary>
    /// The process-wide ticket store that stands in for the database. It is <b>application scope on
    /// purpose</b>: tickets are business data that every session of the same tenant sees, exactly like rows
    /// in SQL Server. Because every session shares it, every member is thread-safe (one lock, copies out,
    /// never a live reference) — the audit's rule for shared state: read-only or thread-safe. Nothing
    /// per-user lives here: no "current tenant", no "selected ticket".
    /// </summary>
    public sealed class SharedTicketStore
    {
        /// <summary>One store per process, created on first use (the "runs once per process" moment).</summary>
        public static SharedTicketStore Instance { get; } = new SharedTicketStore();

        private readonly object _gate = new object();
        private readonly Dictionary<int, Ticket> _tickets = new Dictionary<int, Ticket>();

        private SharedTicketStore()
        {
            foreach (var t in SeedData.Tickets())
                _tickets[t.Id] = t;
        }

        public int Count
        {
            get { lock (_gate) return _tickets.Count; }
        }

        public IReadOnlyList<Ticket> GetByTenant(string tenant)
        {
            lock (_gate)
            {
                return _tickets.Values
                    .Where(t => string.Equals(t.Tenant, tenant, StringComparison.OrdinalIgnoreCase))
                    .Where(t => t.Status != TicketStatus.Closed)
                    .OrderBy(t => t.Id)
                    .Select(Clone)
                    .ToList();
            }
        }

        private static Ticket Clone(Ticket t) => new Ticket
        {
            Id = t.Id,
            Title = t.Title,
            Tenant = t.Tenant,
            Author = t.Author,
            Priority = t.Priority,
            Status = t.Status,
            CreatedAt = t.CreatedAt
        };
    }

    /// <summary>The tickets the walkthrough video shows in the TicketOps Console, spread over three tenants.</summary>
    public static class SeedData
    {
        public static IEnumerable<Ticket> Tickets()
        {
            yield return new Ticket { Id = 1001, Title = "Login page returns 500 error", Tenant = "Contoso", Author = "Alice Rivera", Priority = TicketPriority.High, CreatedAt = DateTime.Now.AddDays(-2) };
            yield return new Ticket { Id = 1002, Title = "CSV export missing columns", Tenant = "Contoso", Author = "Bob Chen", Priority = TicketPriority.Medium, CreatedAt = DateTime.Now.AddDays(-1) };
            yield return new Ticket { Id = 1003, Title = "Dashboard slow on Safari", Tenant = "Contoso", Author = "Sara Patel", Priority = TicketPriority.Medium, CreatedAt = DateTime.Now.AddHours(-20) };
            yield return new Ticket { Id = 1004, Title = "Add dark theme toggle", Tenant = "Contoso", Author = "Jae Kim", Priority = TicketPriority.Low, CreatedAt = DateTime.Now.AddHours(-9) };
            yield return new Ticket { Id = 1005, Title = "Password reset email delayed", Tenant = "Contoso", Author = "Alice Rivera", Priority = TicketPriority.High, CreatedAt = DateTime.Now.AddHours(-3) };
            yield return new Ticket { Id = 2001, Title = "Warehouse scanner app offline", Tenant = "Fabrikam", Author = "Jae Kim", Priority = TicketPriority.High, CreatedAt = DateTime.Now.AddHours(-30) };
            yield return new Ticket { Id = 2002, Title = "Invoice PDF shows wrong VAT", Tenant = "Fabrikam", Author = "Alice Rivera", Priority = TicketPriority.Medium, CreatedAt = DateTime.Now.AddHours(-6) };
            yield return new Ticket { Id = 2003, Title = "Add SSO for contractors", Tenant = "Fabrikam", Author = "Sara Patel", Priority = TicketPriority.Low, CreatedAt = DateTime.Now.AddHours(-2) };
            yield return new Ticket { Id = 3001, Title = "Nightly import job stuck", Tenant = "Northwind", Author = "Sara Patel", Priority = TicketPriority.High, CreatedAt = DateTime.Now.AddHours(-12) };
            yield return new Ticket { Id = 3002, Title = "Rename 'Region' column", Tenant = "Northwind", Author = "Sara Patel", Priority = TicketPriority.Low, CreatedAt = DateTime.Now.AddHours(-1) };
        }
    }
}
