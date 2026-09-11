using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SupportDesk.Data;

namespace SupportDesk.Services;

/// <summary>
/// Who is running the editor. Only <see cref="Supervisor"/> may overwrite another operator's change — see
/// <see cref="ConflictResolution.CanOverwrite"/>. A page-level <c>cboRole</c> selects one; there is no
/// authentication in this sample, so it stands in for a real role claim.
/// </summary>
public enum UserRole
{
    Agent,
    Supervisor
}

/// <summary>
/// One field where the operator's value and the database's current value disagree, for the
/// <c>ConflictDialog</c> grid.
/// </summary>
/// <remarks>
/// <see cref="OriginalValue"/> is EF Core's own <c>OriginalValues</c> for the property — for an ordinary
/// field this is what <c>TicketCommandService.SaveAsync</c>'s own tracked read saw, which (because the
/// conflicting write already landed before that read ran) is the same as <see cref="DatabaseValue"/> for
/// every property except <c>RowVersion</c>. <c>RowVersion</c> differs on purpose: <c>SaveAsync</c> forces
/// its <c>OriginalValue</c> to the token the editor's <see cref="TicketEditModel"/> carried in, which is
/// deliberately stale — that one restored value is what turns the save into a detected conflict at all.
/// A true "value the editor started from" for an ordinary field would need <see cref="TicketEditModel"/>
/// to keep its own pristine snapshot, which it does not (out of scope for this lab) — see
/// <c>docs/ConcurrencyResolution.md</c> for the worked example and why this is still an honest answer.
/// </remarks>
public sealed record ConflictField(string Field, string YourValue, string DatabaseValue, string OriginalValue);

/// <summary>
/// What <see cref="ConflictResolution.BuildConflictListAsync"/> found. <see cref="DeletedByAnotherUser"/> is
/// true when <c>GetDatabaseValuesAsync</c> returned <see langword="null"/> — the row is gone, there is
/// nothing to compare and nothing to overwrite. <see cref="DatabaseRowVersion"/> is the token the database
/// holds <b>right now</b>; Overwrite needs it as the fresh <c>OriginalValue</c> for the retry.
/// </summary>
public sealed record ConflictSet(bool DeletedByAnotherUser, byte[]? DatabaseRowVersion, IReadOnlyList<ConflictField> Fields)
{
    public static ConflictSet Deleted() => new(true, null, Array.Empty<ConflictField>());
}

/// <summary>
/// Turns a <see cref="DbUpdateConcurrencyException"/> into business language: which fields the operator's
/// edit and the database disagree about, and the policy for what the operator is allowed to do next.
/// </summary>
/// <remarks>
/// <para>
/// <b>Why this runs inside <c>TicketCommandService.SaveAsync</c>'s own catch, not the editor's.</b>
/// <c>DbUpdateConcurrencyException.Entries</c> are <see cref="EntityEntry"/> objects that belong to the
/// <c>DbContext</c> that threw. <c>GetDatabaseValuesAsync</c> issues a fresh query through that same
/// context to read what the database holds right now — it needs the context to still be open.
/// <c>TicketCommandService.SaveAsync</c>'s <c>await using var db = …</c> disposes that context the moment
/// the method's stack frame unwinds, which happens <b>before</b> the exception reaches the editor's catch
/// block. So <see cref="BuildConflictListAsync"/> is called from inside <c>SaveAsync</c>'s own
/// <c>catch (DbUpdateConcurrencyException ex)</c>, while <c>db</c> is still alive, and the finished
/// <see cref="ConflictSet"/> — plain data, no live entities — is attached to the very same exception
/// instance via <c>ex.Data["ConflictSet"]</c> before it is rethrown. The editor's own
/// <c>catch (DbUpdateConcurrencyException ex)</c> (which must sit above its <c>catch (DbUpdateException ex)</c>
/// — <c>DbUpdateConcurrencyException</c> derives from it, so the compiler requires that order) reads the
/// set back out of <c>ex.Data</c> instead of touching <c>ex.Entries</c> itself.
/// </para>
/// </remarks>
public sealed class ConflictResolution
{
    /// <summary>
    /// Overwrite is a policy decision, never an automatic retry (see the Module 7 lesson's "Common mistake:
    /// silently overwriting another user's change"). Only a Supervisor may reopen a ticket a colleague just
    /// changed; an ordinary Agent gets Reload and Cancel only. <c>cboRole</c> on the page selects the role
    /// passed into the editor's constructor for this lab.
    /// </summary>
    public static bool CanOverwrite(UserRole role) => role == UserRole.Supervisor;

    /// <summary>
    /// Walks every failed entry, fetches what the database holds now, and reports either "deleted" (a
    /// <see langword="null"/> result from <c>GetDatabaseValuesAsync</c>) or the list of properties where the
    /// operator's <c>CurrentValues</c> differ from the database's. <c>UpdatedAt</c> is skipped — it changes
    /// on every save and never explains the conflict, pure noise. <c>RowVersion</c> is kept, but relabelled
    /// and hex-formatted — it is the proof the conflict happened, not noise, and Overwrite needs the
    /// database's copy of it (<see cref="ConflictSet.DatabaseRowVersion"/>).
    /// </summary>
    public async Task<ConflictSet> BuildConflictListAsync(DbUpdateConcurrencyException ex, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(ex);

        foreach (var entry in ex.Entries)
        {
            var databaseValues = await entry.GetDatabaseValuesAsync(token);
            if (databaseValues is null)
                return ConflictSet.Deleted();

            var fields = new List<ConflictField>();
            byte[]? databaseRowVersion = null;

            foreach (var property in entry.Metadata.GetProperties())
            {
                if (property.Name == nameof(Ticket.Id))
                    continue;

                var current = entry.CurrentValues[property];
                var original = entry.OriginalValues[property];
                var database = databaseValues[property];

                if (property.Name == nameof(Ticket.RowVersion))
                {
                    databaseRowVersion = database as byte[];
                    fields.Add(new ConflictField("RowVersion (concurrency token)", FormatValue(current), FormatValue(database), FormatValue(original)));
                    continue;
                }

                if (property.Name == nameof(Ticket.UpdatedAt))
                    continue;

                if (!Equals(current, database))
                    fields.Add(new ConflictField(property.Name, FormatValue(current), FormatValue(database), FormatValue(original)));
            }

            return new ConflictSet(false, databaseRowVersion, fields);
        }

        // No entries at all would be unusual for a real concurrency exception, but return an empty,
        // non-deleted set rather than throw — the dialog then shows "no field differs", which is still
        // an honest (if surprising) answer.
        return new ConflictSet(false, null, Array.Empty<ConflictField>());
    }

    /// <summary>Renders one property value for the conflict grid — bytes as hex, dates without the tick precision, booleans as words.</summary>
    public static string FormatValue(object? value) => value switch
    {
        null => "—",
        byte[] bytes => bytes.Length == 0 ? "—" : "0x" + Convert.ToHexString(bytes),
        DateTime dt => dt.ToString("yyyy-MM-dd HH:mm:ss"),
        bool b => b ? "true" : "false",
        _ => value.ToString() ?? "—"
    };
}
