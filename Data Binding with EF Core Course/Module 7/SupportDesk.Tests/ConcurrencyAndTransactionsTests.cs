using Microsoft.EntityFrameworkCore;
using SupportDesk.Data;
using SupportDesk.Data.Diagnostics;
using SupportDesk.Services;
using SupportDesk.Tests.Support;
using Xunit.Abstractions;

namespace SupportDesk.Tests;

/// <summary>
/// The Module 7 deliverables, tested without the UI: the <c>RowVersion</c> round trip really produces
/// <see cref="DbUpdateConcurrencyException"/> for a stale save (through <see cref="TicketCommandService"/>,
/// not just raw EF Core), <see cref="ConflictResolution.BuildConflictListAsync"/> reports the right fields
/// and the "deleted by another user" case, Reload and Overwrite behave as the conflict dialog promises, and
/// <see cref="TicketCommandService.CloseTicketWithCommentAsync"/> is a real transaction — it rolls back on
/// the lab's simulated failure and commits otherwise.
/// </summary>
public sealed class ConcurrencyAndTransactionsTests : IDisposable
{
    private readonly SqliteTestFactory _factory = new();
    private readonly ConflictResolution _conflictResolution = new();
    private readonly TransactionFailureSwitch _transactionFailure = new();
    private readonly TicketCommandService _commands;
    private readonly ITestOutputHelper _output;

    public ConcurrencyAndTransactionsTests(ITestOutputHelper output)
    {
        _output = output;
        _commands = new TicketCommandService(_factory, _conflictResolution, _transactionFailure);
    }

    private Task<SeedResult> SeedAsync() => new DevelopmentSeeder(_factory).SeedDevelopmentDataAsync();

    #region RowVersion round trip → DbUpdateConcurrencyException

    [Fact]
    public async Task SaveAsync_with_a_stale_RowVersion_throws_DbUpdateConcurrencyException()
    {
        await SeedAsync();
        int id;
        await using (var db = _factory.CreateDbContext())
            id = (await db.Tickets.OrderBy(t => t.Id).FirstAsync()).Id;

        // The editor's load — model.RowVersion carries the token this read saw.
        var data = await _commands.LoadEditModelAsync(id);
        var model = data.Model;
        model.Title = "Escalated by the agent";

        // Another operator changes the same row through a separate context, exactly like
        // TicketCommandService.SimulateAnotherOperatorChangeAsync — the row now has a new RowVersion.
        await _commands.SimulateAnotherOperatorChangeAsync(id);

        // The stale model.RowVersion becomes OriginalValue; SaveChangesAsync's
        // WHERE "Id" = @p AND "RowVersion" = @original matches zero rows.
        var ex = await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => _commands.SaveAsync(model));

        Assert.NotNull(ex.Entries);
        Assert.Single(ex.Entries);
    }

    [Fact]
    public async Task SaveAsync_without_a_stale_RowVersion_does_not_throw_when_nothing_else_changed_the_row()
    {
        await SeedAsync();
        int id;
        await using (var db = _factory.CreateDbContext())
            id = (await db.Tickets.OrderBy(t => t.Id).FirstAsync()).Id;

        var data = await _commands.LoadEditModelAsync(id);
        data.Model.Title = "An ordinary edit, nobody else touched the row";

        var result = await _commands.SaveAsync(data.Model);

        Assert.Equal(id, result.Id);
    }

    #endregion

    #region ConflictResolution.BuildConflictListAsync

    [Fact]
    public async Task SaveAsync_conflict_attaches_a_ConflictSet_listing_the_differing_fields_with_your_database_and_original_values()
    {
        await SeedAsync();
        int id;
        await using (var db = _factory.CreateDbContext())
            id = (await db.Tickets.Where(t => t.Status != TicketStatuses.Closed).OrderBy(t => t.Id).FirstAsync()).Id;

        var data = await _commands.LoadEditModelAsync(id);
        var model = data.Model;
        model.Status = TicketStatuses.Resolved;   // "your value"

        // "database value": SimulateAnotherOperatorChangeAsync sets Status = Closed, Priority = High.
        await _commands.SimulateAnotherOperatorChangeAsync(id);

        var ex = await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => _commands.SaveAsync(model));

        var conflicts = Assert.IsType<ConflictSet>(ex.Data["ConflictSet"]);
        Assert.False(conflicts.DeletedByAnotherUser);
        Assert.NotNull(conflicts.DatabaseRowVersion);
        Assert.Equal(16, conflicts.DatabaseRowVersion!.Length);

        var statusField = Assert.Single(conflicts.Fields, f => f.Field == nameof(Ticket.Status));
        Assert.Equal("Resolved", statusField.YourValue);
        Assert.Equal(TicketStatuses.Closed, statusField.DatabaseValue);
        // SaveAsync always tracks a FRESH read (never the editor's stale copy), so for an ordinary property
        // EF's OriginalValues is "what this save attempt's own read saw" — which, because the conflicting
        // write already landed before this read happened, is the database's current value too. Only
        // RowVersion's OriginalValue differs on purpose (it is forced to the stale editor token below).
        Assert.Equal(TicketStatuses.Closed, statusField.OriginalValue);

        var priorityField = Assert.Single(conflicts.Fields, f => f.Field == nameof(Ticket.Priority));
        Assert.Equal(TicketPriorities.High, priorityField.DatabaseValue);

        // UpdatedAt is pure noise (it changes on every save) and is never reported.
        Assert.DoesNotContain(conflicts.Fields, f => f.Field == nameof(Ticket.UpdatedAt));

        // RowVersion IS reported — it is the proof the conflict happened, hex-formatted, not skipped.
        var rowVersionField = Assert.Single(conflicts.Fields, f => f.Field.StartsWith("RowVersion"));
        Assert.StartsWith("0x", rowVersionField.DatabaseValue);
    }

    [Fact]
    public async Task BuildConflictListAsync_reports_the_row_as_deleted_when_it_is_removed_between_the_tracked_read_and_SaveChangesAsync()
    {
        await SeedAsync();
        int id;
        await using (var db = _factory.CreateDbContext())
            id = (await db.Tickets.OrderBy(t => t.Id).FirstAsync()).Id;

        // A tracked read that stays open while "another user" deletes the row through a second context —
        // TicketCommandService.SaveAsync cannot reach this exact race (its own fresh read would already see
        // the row gone and throw TicketNotFoundException first), so this test drives EF Core directly to
        // prove ConflictResolution.BuildConflictListAsync's "deleted" branch on a genuine
        // DbUpdateConcurrencyException whose GetDatabaseValuesAsync legitimately returns null.
        await using var db1 = _factory.CreateDbContext();
        var tracked = await db1.Tickets.SingleAsync(t => t.Id == id);
        tracked.Status = TicketStatuses.Resolved;

        await using (var db2 = _factory.CreateDbContext())
        {
            var toDelete = await db2.Tickets.SingleAsync(t => t.Id == id);
            db2.Tickets.Remove(toDelete);
            await db2.SaveChangesAsync();
        }

        var ex = await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => db1.SaveChangesAsync());

        var conflicts = await _conflictResolution.BuildConflictListAsync(ex);

        Assert.True(conflicts.DeletedByAnotherUser);
        Assert.Null(conflicts.DatabaseRowVersion);
        Assert.Empty(conflicts.Fields);
    }

    #endregion

    #region Reload and Overwrite

    [Fact]
    public async Task LoadEditModelAsync_after_a_conflict_returns_the_current_database_values_the_way_the_dialogs_Reload_does()
    {
        await SeedAsync();
        int id;
        await using (var db = _factory.CreateDbContext())
            id = (await db.Tickets.OrderBy(t => t.Id).FirstAsync()).Id;

        var before = await _commands.LoadEditModelAsync(id);
        var staleRowVersion = before.Model.RowVersion;

        await _commands.SimulateAnotherOperatorChangeAsync(id);

        var reloaded = await _commands.LoadEditModelAsync(id);

        Assert.Equal(TicketStatuses.Closed, reloaded.Model.Status);
        Assert.Equal(TicketPriorities.High, reloaded.Model.Priority);
        Assert.NotEqual(staleRowVersion, reloaded.Model.RowVersion);
    }

    [Fact]
    public async Task SaveAsync_overwrite_with_the_databases_RowVersion_wins_and_produces_a_new_token()
    {
        await SeedAsync();
        int id;
        await using (var db = _factory.CreateDbContext())
            id = (await db.Tickets.Where(t => t.Status != TicketStatuses.Closed).OrderBy(t => t.Id).FirstAsync()).Id;

        var data = await _commands.LoadEditModelAsync(id);
        var model = data.Model;
        model.Title = "The agent's edit — should win on Overwrite";

        await _commands.SimulateAnotherOperatorChangeAsync(id);

        var ex = await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => _commands.SaveAsync(model));
        var conflicts = (ConflictSet)ex.Data["ConflictSet"]!;
        Assert.NotNull(conflicts.DatabaseRowVersion);

        // The ConflictDialog's Overwrite path: the database's current token becomes OriginalValue.
        var result = await _commands.SaveAsync(model, TimeSpan.Zero, forceDuplicateNumber: false, conflicts.DatabaseRowVersion);
        Assert.Equal(id, result.Id);

        await using var check = _factory.CreateDbContext();
        var reloaded = await check.Tickets.SingleAsync(t => t.Id == id);
        Assert.Equal("The agent's edit — should win on Overwrite", reloaded.Title);   // your value won
        Assert.NotEqual(conflicts.DatabaseRowVersion, reloaded.RowVersion);            // a fresh token was stamped by SaveChanges
    }

    #endregion

    #region Transactions — CloseTicketWithCommentAsync

    [Fact]
    public async Task CloseTicketWithCommentAsync_commits_both_writes_together()
    {
        await SeedAsync();
        int id;
        await using (var db = _factory.CreateDbContext())
            id = (await db.Tickets.Where(t => t.Status != TicketStatuses.Closed).OrderBy(t => t.Id).FirstAsync()).Id;

        var result = await _commands.CloseTicketWithCommentAsync(id, "Resolved and closed by the transaction demo.");

        await using var check = _factory.CreateDbContext();
        var ticket = await check.Tickets.Include(t => t.Comments).SingleAsync(t => t.Id == id);
        Assert.Equal(TicketStatuses.Closed, ticket.Status);
        Assert.Contains(ticket.Comments, c => c.Id == result.CommentId && c.Body.Contains("Resolved and closed"));
    }

    [Fact]
    public async Task CloseTicketWithCommentAsync_rolls_back_the_status_write_when_the_switch_fails_after_it()
    {
        await SeedAsync();
        int id;
        string originalStatus;
        await using (var db = _factory.CreateDbContext())
        {
            var ticket = await db.Tickets.Where(t => t.Status != TicketStatuses.Closed).OrderBy(t => t.Id).FirstAsync();
            id = ticket.Id;
            originalStatus = ticket.Status;
        }

        _transactionFailure.FailAfterFirstWrite = true;

        await Assert.ThrowsAsync<SimulatedTransactionFailureException>(() => _commands.CloseTicketWithCommentAsync(id, "Should never be written."));

        await using var check = _factory.CreateDbContext();
        var reloaded = await check.Tickets.Include(t => t.Comments).SingleAsync(t => t.Id == id);
        Assert.Equal(originalStatus, reloaded.Status);                                         // rolled back — status unchanged
        Assert.DoesNotContain(reloaded.Comments, c => c.Body.Contains("Should never be written")); // the second write never landed
        Assert.False(_transactionFailure.FailAfterFirstWrite);                                  // fires once, then resets itself
    }

    #endregion

    #region Evidence — the raw trace for one reproduced conflict (docs/ConcurrencyResolution.md quotes this)

    /// <summary>
    /// Not an assertion beyond "it threw and was caught" — this test exists to print, with
    /// <c>ITestOutputHelper</c>, exactly what <c>QueryTrace</c> reports for one reproduced conflict: the
    /// UPDATE with the stale token in its WHERE clause, the 0-rows-matched note, and one line per differing
    /// field. Run it with <c>dotnet test --filter Prints_the_trace_for_one_reproduced_conflict -v n</c> to
    /// see the output; the lines it prints are quoted verbatim in docs/ConcurrencyResolution.md.
    /// </summary>
    [Fact]
    public async Task Prints_the_trace_for_one_reproduced_conflict()
    {
        await SeedAsync();
        int id;
        await using (var db = _factory.CreateDbContext())
            id = (await db.Tickets.Where(t => t.Status != TicketStatuses.Closed).OrderBy(t => t.Id).FirstAsync()).Id;

        var data = await _commands.LoadEditModelAsync(id);
        var model = data.Model;
        model.Status = TicketStatuses.Resolved;

        await _commands.SimulateAnotherOperatorChangeAsync(id);

        var lines = new List<string>();
        using (QueryTrace.Begin(entry => lines.Add($"{entry.Kind}: {entry.Text}" + (entry.Milliseconds is double ms ? $" ({ms:0.0} ms)" : ""))))
        {
            var ex = await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => _commands.SaveAsync(model));
            var conflicts = (ConflictSet)ex.Data["ConflictSet"]!;
            foreach (var field in conflicts.Fields)
                lines.Add($"conflict {field.Field}: yours '{field.YourValue}' · database '{field.DatabaseValue}' · original '{field.OriginalValue}'");
        }

        foreach (var line in lines)
            _output.WriteLine(line);

        Assert.Contains(lines, l => l.Contains("UPDATE") && l.Contains("RowVersion"));
    }

    /// <summary>Same idea as <see cref="Prints_the_trace_for_one_reproduced_conflict"/>, for the transaction rollback: both SaveChangesAsync calls, the simulated failure, and the rollback note.</summary>
    [Fact]
    public async Task Prints_the_trace_for_the_transaction_rollback()
    {
        await SeedAsync();
        int id;
        await using (var db = _factory.CreateDbContext())
            id = (await db.Tickets.Where(t => t.Status != TicketStatuses.Closed).OrderBy(t => t.Id).FirstAsync()).Id;

        _transactionFailure.FailAfterFirstWrite = true;

        var lines = new List<string>();
        using (QueryTrace.Begin(entry => lines.Add($"{entry.Kind}: {entry.Text}")))
        {
            await Assert.ThrowsAsync<SimulatedTransactionFailureException>(() => _commands.CloseTicketWithCommentAsync(id, "Should never be written."));
        }

        foreach (var line in lines)
            _output.WriteLine(line);

        Assert.Contains(lines, l => l.Contains("rolled back"));
    }

    #endregion

    public void Dispose() => _factory.Dispose();
}
