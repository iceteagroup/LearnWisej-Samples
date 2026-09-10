using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using SupportDesk.Data;

namespace SupportDesk.Services;

public sealed record TableCounts(int Customers, int Agents, int Categories, int Tickets, int TicketComments);

public sealed record IndexInfo(string Table, string Name, IReadOnlyList<string> Columns, bool IsUnique);

public sealed record RelationshipInfo(string DependentTable, string ForeignKeyColumn, string PrincipalTable, DeleteBehavior DeleteBehavior);

public sealed record CheckConstraintInfo(string Table, string Name, string Sql);

/// <summary>Everything the page's "Model &amp; migration" card shows — read from the database and the model, never hard-coded.</summary>
public sealed record SchemaInfo(
    TableCounts Counts,
    IReadOnlyList<string> AppliedMigrations,
    IReadOnlyList<string> PendingMigrations,
    IReadOnlyList<IndexInfo> Indexes,
    IReadOnlyList<RelationshipInfo> Relationships,
    IReadOnlyList<CheckConstraintInfo> CheckConstraints);

/// <summary>
/// Describes the schema the application is running on: row counts per table (five COUNTs), the migrations
/// applied to this database and the ones still pending (the <c>__EFMigrationsHistory</c> table versus the
/// migrations compiled into SupportDesk.Data), and the indexes, foreign keys with their delete behaviour and
/// check constraints as EF Core's model metadata reports them. One context, disposed before returning.
/// </summary>
public sealed class SchemaInfoService
{
    private readonly IDbContextFactory<SupportDeskContext> _dbFactory;

    public SchemaInfoService(IDbContextFactory<SupportDeskContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<SchemaInfo> DescribeAsync(CancellationToken token = default)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(token);

        var counts = new TableCounts(
            await db.Customers.CountAsync(token),
            await db.Agents.CountAsync(token),
            await db.Categories.CountAsync(token),
            await db.Tickets.CountAsync(token),
            await db.TicketComments.CountAsync(token));

        var applied = (await db.Database.GetAppliedMigrationsAsync(token)).ToList();
        var pending = (await db.Database.GetPendingMigrationsAsync(token)).ToList();

        var indexes = new List<IndexInfo>();
        var relationships = new List<RelationshipInfo>();
        var checks = new List<CheckConstraintInfo>();

        // Check constraints are design-time configuration: the read-optimised runtime model drops them, so ask
        // the design-time model (the one migrations are scaffolded from) for the metadata.
        var model = db.GetService<IDesignTimeModel>().Model;

        foreach (var entityType in model.GetEntityTypes())
        {
            var table = entityType.GetTableName() ?? entityType.ShortName();
            var storeObject = StoreObjectIdentifier.Table(table, entityType.GetSchema());

            foreach (var index in entityType.GetIndexes())
            {
                indexes.Add(new IndexInfo(
                    table,
                    index.GetDatabaseName(storeObject) ?? index.GetDatabaseName() ?? "(unnamed)",
                    index.Properties.Select(p => p.GetColumnName(storeObject) ?? p.Name).ToList(),
                    index.IsUnique));
            }

            foreach (var fk in entityType.GetForeignKeys())
            {
                relationships.Add(new RelationshipInfo(
                    table,
                    string.Join(", ", fk.Properties.Select(p => p.GetColumnName(storeObject) ?? p.Name)),
                    fk.PrincipalEntityType.GetTableName() ?? fk.PrincipalEntityType.ShortName(),
                    fk.DeleteBehavior));
            }

            foreach (var check in entityType.GetCheckConstraints())
            {
                checks.Add(new CheckConstraintInfo(table, check.Name ?? "(unnamed)", check.Sql));
            }
        }

        return new SchemaInfo(counts, applied, pending, indexes, relationships, checks);
    }
}
