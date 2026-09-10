using Microsoft.EntityFrameworkCore;
using SupportDesk.Data;
using SupportDesk.Data.Diagnostics;
using SupportDesk.Services;
using SupportDesk.Tests.Support;

namespace SupportDesk.Tests;

/// <summary>
/// The Module 4 deliverable, tested without the UI: <see cref="TicketCommandService"/> against SQLite in
/// memory with the real development seed. What is proved here is what the instructor acceptance criteria
/// ask for — create and update map the approved fields and leave <c>Number</c>/<c>CreatedAt</c> alone,
/// delete confirms by loading fresh and honours the business rule, an already-deleted ticket comes back as
/// a result rather than an exception, and the edit-model round trip (load → model → save → reload) is
/// exactly what the operator typed.
/// </summary>
public sealed class TicketCommandServiceTests : IDisposable
{
    private readonly SqliteTestFactory _factory = new();
    private readonly ConflictResolution _conflictResolution = new();
    private readonly TransactionFailureSwitch _transactionFailure = new();
    private readonly TicketCommandService _commands;

    public TicketCommandServiceTests()
    {
        _commands = new TicketCommandService(_factory, _conflictResolution, _transactionFailure);
    }

    private Task<SeedResult> SeedAsync() => new DevelopmentSeeder(_factory).SeedDevelopmentDataAsync();

    #region Save — create

    [Fact]
    public async Task SaveAsync_creates_a_new_ticket_with_a_generated_number_and_maps_the_approved_fields()
    {
        await SeedAsync();
        int customerId, agentId, categoryId;
        await using (var db = _factory.CreateDbContext())
        {
            customerId = (await db.Customers.OrderBy(c => c.Id).FirstAsync()).Id;
            agentId = (await db.Agents.OrderBy(a => a.Id).FirstAsync()).Id;
            categoryId = (await db.Categories.OrderBy(c => c.Id).FirstAsync()).Id;
        }

        var model = TicketEditModel.NewTicket();
        model.Title = "New printer will not accept the network driver";
        model.Description = "Escalated from the walk-up desk.";
        model.CustomerId = customerId;
        model.AgentId = agentId;
        model.CategoryId = categoryId;
        model.DueDate = DateTime.UtcNow.Date.AddDays(5);
        model.IsUrgent = true;

        var before = DateTime.UtcNow;
        var result = await _commands.SaveAsync(model);

        Assert.Equal("SD-1313", result.Number);          // the seed's highest is SD-1312

        await using var check = _factory.CreateDbContext();
        var ticket = await check.Tickets.SingleAsync(t => t.Id == result.Id);
        Assert.Equal(model.Title, ticket.Title);
        Assert.Equal(model.Description, ticket.Description);
        Assert.Equal(TicketStatuses.Open, ticket.Status);
        Assert.Equal(TicketPriorities.Normal, ticket.Priority);
        Assert.Equal(customerId, ticket.CustomerId);
        Assert.Equal(agentId, ticket.AgentId);
        Assert.Equal(categoryId, ticket.CategoryId);
        Assert.Equal(model.DueDate, ticket.DueDate);
        Assert.True(ticket.IsUrgent);
        Assert.True(ticket.UpdatedAt >= before);           // stamped by SupportDeskContext.SaveChanges, not by SaveAsync itself
        Assert.Equal(16, ticket.RowVersion.Length);
    }

    [Fact]
    public async Task SaveAsync_continues_the_next_number_after_the_highest_existing_number()
    {
        await using var db = _factory.CreateDbContext();
        var customer = TestData.Customer();
        var category = TestData.Category();
        db.AddRange(customer, category);
        db.Tickets.Add(TestData.Ticket(customer, category, "SD-1007", "An early manually-numbered ticket"));
        db.Tickets.Add(TestData.Ticket(customer, category, "SD-1500", "A gap in the numbering"));
        await db.SaveChangesAsync();

        var model = TicketEditModel.NewTicket();
        model.Title = "The next ticket";
        model.CustomerId = customer.Id;
        model.CategoryId = category.Id;

        var result = await _commands.SaveAsync(model);

        Assert.Equal("SD-1501", result.Number);            // continues after the HIGHEST number, not the count of rows
    }

    [Fact]
    public async Task SaveAsync_create_sends_exactly_two_statements_the_number_scan_and_the_INSERT()
    {
        await SeedAsync();
        int customerId, categoryId;
        await using (var db = _factory.CreateDbContext())
        {
            customerId = (await db.Customers.FirstAsync()).Id;
            categoryId = (await db.Categories.FirstAsync()).Id;
        }

        var model = TicketEditModel.NewTicket();
        model.Title = "Statement-count check";
        model.CustomerId = customerId;
        model.CategoryId = categoryId;

        var entries = new List<TraceEntry>();
        using var scope = QueryTrace.Begin(entries.Add);

        await _commands.SaveAsync(model);

        Assert.Equal(2, scope.Commands);
        Assert.Equal(1, scope.ContextsCreated);
        Assert.Equal(1, scope.ContextsDisposed);
    }

    #endregion

    #region Save — update

    [Fact]
    public async Task SaveAsync_updates_an_existing_ticket_and_leaves_Number_and_CreatedAt_unchanged()
    {
        await SeedAsync();
        int id;
        string originalNumber;
        DateTime originalCreatedAt;
        int newCustomerId;
        await using (var db = _factory.CreateDbContext())
        {
            var ticket = await db.Tickets.OrderBy(t => t.Id).FirstAsync();
            id = ticket.Id;
            originalNumber = ticket.Number;
            originalCreatedAt = ticket.CreatedAt;
            newCustomerId = (await db.Customers.OrderByDescending(c => c.Id).FirstAsync()).Id;
        }

        var data = await _commands.LoadEditModelAsync(id);
        var model = data.Model;
        model.Title = "Escalated: " + model.Title;
        model.Status = TicketStatuses.InProgress;
        model.CustomerId = newCustomerId;

        var result = await _commands.SaveAsync(model);

        Assert.Equal(id, result.Id);
        Assert.Equal(originalNumber, result.Number);        // SaveAsync never touches Number on an update

        await using var check = _factory.CreateDbContext();
        var reloaded = await check.Tickets.SingleAsync(t => t.Id == id);
        Assert.Equal(originalNumber, reloaded.Number);
        Assert.Equal(originalCreatedAt, reloaded.CreatedAt);
        Assert.Equal(model.Title, reloaded.Title);
        Assert.Equal(TicketStatuses.InProgress, reloaded.Status);
        Assert.Equal(newCustomerId, reloaded.CustomerId);
    }

    [Fact]
    public async Task SaveAsync_update_sends_exactly_two_statements_the_tracked_read_and_the_UPDATE()
    {
        await SeedAsync();
        int id;
        await using (var db = _factory.CreateDbContext())
            id = (await db.Tickets.OrderBy(t => t.Id).FirstAsync()).Id;

        var data = await _commands.LoadEditModelAsync(id);
        data.Model.Title = "One more edit";

        var entries = new List<TraceEntry>();
        using var scope = QueryTrace.Begin(entries.Add);

        await _commands.SaveAsync(data.Model);

        Assert.Equal(2, scope.Commands);
        var sql = entries.Where(e => e.Kind == TraceKind.Command).Select(e => e.Text).ToList();
        Assert.Contains("UPDATE", sql[1]);
    }

    [Fact]
    public async Task SaveAsync_throws_TicketNotFoundException_when_the_ticket_was_deleted_since_it_was_loaded()
    {
        await SeedAsync();
        int id;
        await using (var db = _factory.CreateDbContext())
            id = (await db.Tickets.OrderBy(t => t.Id).FirstAsync()).Id;

        var data = await _commands.LoadEditModelAsync(id);       // the editor's no-tracking read
        var deleted = await _commands.DeleteAsync(id);            // "another operator" removes it
        Assert.Equal(DeleteOutcome.Deleted, deleted.Outcome);

        var ex = await Assert.ThrowsAsync<TicketNotFoundException>(() => _commands.SaveAsync(data.Model));
        Assert.Equal(id, ex.TicketId);
    }

    #endregion

    #region Delete

    [Fact]
    public async Task DeleteAsync_removes_an_existing_non_closed_ticket()
    {
        await SeedAsync();
        int id;
        string number;
        await using (var db = _factory.CreateDbContext())
        {
            var ticket = await db.Tickets.Where(t => t.Status != TicketStatuses.Closed).OrderBy(t => t.Id).FirstAsync();
            id = ticket.Id;
            number = ticket.Number;
        }

        var result = await _commands.DeleteAsync(id);

        Assert.Equal(DeleteOutcome.Deleted, result.Outcome);
        Assert.Equal(number, result.Number);
        await using var check = _factory.CreateDbContext();
        Assert.Null(await check.Tickets.FindAsync(id));
    }

    [Fact]
    public async Task DeleteAsync_returns_NotFound_and_sends_only_one_statement_when_the_ticket_is_already_gone()
    {
        await SeedAsync();
        int id;
        await using (var db = _factory.CreateDbContext())
            id = (await db.Tickets.OrderBy(t => t.Id).FirstAsync()).Id;

        await _commands.DeleteAsync(id);                          // gone after this

        var entries = new List<TraceEntry>();
        using var scope = QueryTrace.Begin(entries.Add);

        var result = await _commands.DeleteAsync(id);              // "someone else" tries again

        Assert.Equal(DeleteOutcome.NotFound, result.Outcome);
        Assert.Null(result.Number);
        Assert.Contains("already deleted", result.Reason);
        Assert.Equal(1, scope.Commands);                            // the SELECT only — no DELETE is sent
    }

    [Fact]
    public async Task DeleteAsync_refuses_a_Closed_ticket_and_sends_no_DELETE()
    {
        await SeedAsync();
        int id;
        await using (var db = _factory.CreateDbContext())
        {
            var ticket = await db.Tickets.Where(t => t.Status != TicketStatuses.Closed).OrderBy(t => t.Id).FirstAsync();
            ticket.Status = TicketStatuses.Closed;
            await db.SaveChangesAsync();
            id = ticket.Id;
        }

        var entries = new List<TraceEntry>();
        using var scope = QueryTrace.Begin(entries.Add);

        var result = await _commands.DeleteAsync(id);

        Assert.Equal(DeleteOutcome.Refused, result.Outcome);
        Assert.Contains("Closed", result.Reason);
        Assert.Equal(1, scope.Commands);                            // the SELECT only — refused before Remove

        await using var check = _factory.CreateDbContext();
        Assert.NotNull(await check.Tickets.FindAsync(id));          // still there
    }

    #endregion

    #region Load — the edit model, the lookups, and the round trip

    [Fact]
    public async Task LoadEditModelAsync_maps_every_editable_field_and_leaves_Number_out_of_the_model()
    {
        await SeedAsync();
        Ticket original;
        await using (var db = _factory.CreateDbContext())
            original = await db.Tickets.AsNoTracking().Where(t => t.AgentId != null && t.DueDate != null).OrderBy(t => t.Id).FirstAsync();

        var data = await _commands.LoadEditModelAsync(original.Id);

        Assert.Equal(original.Number, data.Number);                 // carried alongside the model, not inside it
        Assert.Equal(original.Id, data.Model.Id);
        Assert.Equal(original.Title, data.Model.Title);
        Assert.Equal(original.Description, data.Model.Description);
        Assert.Equal(original.Status, data.Model.Status);
        Assert.Equal(original.Priority, data.Model.Priority);
        Assert.Equal(original.CustomerId, data.Model.CustomerId);
        Assert.Equal(original.AgentId, data.Model.AgentId);
        Assert.Equal(original.CategoryId, data.Model.CategoryId);
        Assert.Equal(original.DueDate, data.Model.DueDate);
        Assert.Equal(original.IsUrgent, data.Model.IsUrgent);
    }

    [Fact]
    public async Task Load_then_edit_then_save_then_reload_round_trips_exactly_what_was_typed()
    {
        await SeedAsync();
        int id;
        await using (var db = _factory.CreateDbContext())
            id = (await db.Tickets.OrderBy(t => t.Id).FirstAsync()).Id;

        var loaded = await _commands.LoadEditModelAsync(id);
        var edited = loaded.Model;
        edited.Title = "Round-trip title";
        edited.Description = "Round-trip description";
        edited.Status = TicketStatuses.Waiting;
        edited.Priority = TicketPriorities.High;
        edited.AgentId = null;                                      // unassign
        edited.DueDate = DateTime.UtcNow.Date.AddDays(3);
        edited.IsUrgent = true;

        await _commands.SaveAsync(edited);
        var reloaded = await _commands.LoadEditModelAsync(id);

        Assert.Equal(edited.Title, reloaded.Model.Title);
        Assert.Equal(edited.Description, reloaded.Model.Description);
        Assert.Equal(edited.Status, reloaded.Model.Status);
        Assert.Equal(edited.Priority, reloaded.Model.Priority);
        Assert.Equal(edited.CustomerId, reloaded.Model.CustomerId);
        Assert.Null(reloaded.Model.AgentId);
        Assert.Equal(edited.CategoryId, reloaded.Model.CategoryId);
        Assert.Equal(edited.DueDate, reloaded.Model.DueDate);
        Assert.True(reloaded.Model.IsUrgent);
    }

    [Fact]
    public async Task GetLookupsAsync_returns_all_five_lists_and_the_agent_sentinel_id_is_free_for_the_editor_to_use()
    {
        await SeedAsync();

        var lookups = await _commands.GetLookupsAsync();

        Assert.Equal(5, lookups.Customers.Count);
        Assert.Equal(3, lookups.Agents.Count);
        Assert.Equal(6, lookups.Categories.Count);
        Assert.Equal(TicketStatuses.All, lookups.Statuses);
        Assert.Equal(TicketPriorities.All, lookups.Priorities);
        Assert.DoesNotContain(lookups.Agents, a => a.Id == TicketCommandService.UnassignedAgentId);
    }

    #endregion

    public void Dispose() => _factory.Dispose();
}
