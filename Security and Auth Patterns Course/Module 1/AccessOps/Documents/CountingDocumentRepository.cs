using System;
using System.Collections.Generic;
using System.Threading;
using AccessOps.Infrastructure;

namespace AccessOps.Documents
{
    /// <summary>A confidential document as the repository returns it.</summary>
    public sealed class Document
    {
        public string Tenant { get; init; }
        public int Id { get; init; }
        public string Title { get; init; }
        public string Classification { get; init; }
        public string Body { get; init; }
    }

    /// <summary>A row of the Documents grid: no body, no internal keys.</summary>
    public sealed class DocumentListItem
    {
        public int Record { get; init; }
        public string Title { get; init; }
        public string Classification { get; init; }
        public string Tenant { get; init; }
    }

    /// <summary>
    /// Document storage. It does not decide anything: callers reach it only through
    /// <see cref="DocumentService"/>, which checks the grant, the tenant and the record first.
    /// </summary>
    public interface IDocumentRepository
    {
        /// <summary>The document, or null when the tenant has no such record.</summary>
        Document Read(string tenant, int recordId);

        /// <summary>The tenant's documents for the list (titles only).</summary>
        IReadOnlyList<DocumentListItem> List(string tenant);
    }

    /// <summary>
    /// The lab's read-counting repository: every call to <see cref="Read"/> adds one to
    /// <see cref="ReadCount"/> before it touches the database, so "denied at zero repository
    /// calls" is a number you can check instead of an empty screen you have to trust.
    /// Listing titles is not a record read and is not counted.
    /// <para>
    /// Training seam: <see cref="FailNextRead"/> makes the next read throw, so the lab can show
    /// how an unexpected repository failure is caught, logged and reported.
    /// </para>
    /// </summary>
    public sealed class CountingDocumentRepository : IDocumentRepository
    {
        private readonly AccessOpsDatabase _db;
        private int _readCount;
        private int _failNext;

        public CountingDocumentRepository(AccessOpsDatabase db)
        {
            _db = db;
        }

        /// <summary>How many times <see cref="Read"/> ran since the process started.</summary>
        public int ReadCount => Volatile.Read(ref _readCount);

        /// <summary>When true, the next <see cref="Read"/> throws (then the flag resets).</summary>
        public bool FailNextRead
        {
            get => Volatile.Read(ref _failNext) == 1;
            set => Volatile.Write(ref _failNext, value ? 1 : 0);
        }

        /// <inheritdoc />
        public Document Read(string tenant, int recordId)
        {
            Interlocked.Increment(ref _readCount);

            if (Interlocked.Exchange(ref _failNext, 0) == 1)
                throw new InvalidOperationException("Injected repository fault: the document store did not answer.");

            using var db = _db.Open();
            using var cmd = db.CreateCommand();
            cmd.CommandText = "SELECT tenant, id, title, classification, body FROM documents WHERE tenant = $tenant AND id = $id";
            cmd.Parameters.AddWithValue("$tenant", tenant);
            cmd.Parameters.AddWithValue("$id", recordId);
            using var r = cmd.ExecuteReader();
            if (!r.Read())
                return null;

            return new Document
            {
                Tenant = r.GetString(0),
                Id = r.GetInt32(1),
                Title = r.GetString(2),
                Classification = r.GetString(3),
                Body = r.GetString(4)
            };
        }

        /// <inheritdoc />
        public IReadOnlyList<DocumentListItem> List(string tenant)
        {
            var rows = new List<DocumentListItem>();
            using var db = _db.Open();
            using var cmd = db.CreateCommand();
            cmd.CommandText = "SELECT id, title, classification, tenant FROM documents WHERE tenant = $tenant ORDER BY id";
            cmd.Parameters.AddWithValue("$tenant", tenant);
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                rows.Add(new DocumentListItem
                {
                    Record = r.GetInt32(0),
                    Title = r.GetString(1),
                    Classification = r.GetString(2),
                    Tenant = r.GetString(3)
                });
            }
            return rows;
        }
    }
}
