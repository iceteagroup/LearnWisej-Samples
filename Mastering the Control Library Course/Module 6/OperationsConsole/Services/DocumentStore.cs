using System;
using System.Collections.Generic;
using System.IO;

namespace OperationsConsole.Services
{
    /// <summary>
    /// One document the browser sent to the server. The bytes live in memory for the life of the session —
    /// this is a lab, not a document management system — but everything the dashboard shows about the file
    /// comes from the copy the <b>server</b> received, never from what the client said about it.
    /// </summary>
    public sealed class StoredDocument
    {
        public StoredDocument(string id, string fileName, string contentType, byte[] content, DateTime receivedAt)
        {
            Id = id;
            FileName = fileName;
            ContentType = contentType;
            Content = content;
            ReceivedAt = receivedAt;
        }

        /// <summary>Stable id ("DOC-000001") — what goes to the diagnostic panel through <c>ShellStatus.Record</c>.</summary>
        public string Id { get; }

        public string FileName { get; }

        public string ContentType { get; }

        /// <summary>The bytes the server actually received (the only size that counts).</summary>
        public byte[] Content { get; }

        public long SizeBytes => Content == null ? 0L : Content.LongLength;

        public DateTime ReceivedAt { get; }
    }

    /// <summary>Result of a store attempt: accepted with a document, or rejected with a sentence a user can read.</summary>
    public sealed class StoreResult
    {
        private StoreResult(bool accepted, StoredDocument document, string rejectionReason)
        {
            Accepted = accepted;
            Document = document;
            RejectionReason = rejectionReason;
        }

        public bool Accepted { get; }

        public StoredDocument Document { get; }

        /// <summary>Plain-language reason, safe to show to the user (no exception text, no paths).</summary>
        public string RejectionReason { get; }

        public static StoreResult Ok(StoredDocument document) => new StoreResult(true, document, null);

        public static StoreResult Rejected(string reason) => new StoreResult(false, null, reason);
    }

    /// <summary>
    /// The in-memory document service behind the dashboard's Upload workflow. The user selects a file, the browser
    /// uploads the bytes, and the server processes what arrived: it never browses the user's disk and never trusts
    /// the name or size the client reported — <see cref="Validate"/> runs here even though the <c>Upload</c> control
    /// already filters in the browser.
    /// </summary>
    public sealed class DocumentStore
    {
        private readonly List<StoredDocument> _documents = new List<StoredDocument>();
        private int _nextId;

        /// <summary>Extensions the server accepts. The <c>Upload</c> control mirrors this in <c>AllowedFileTypes</c>.</summary>
        public string[] AllowedExtensions { get; set; } = new[] { ".pdf" };

        /// <summary>Maximum accepted size in bytes. The <c>Upload</c> control mirrors this in <c>MaxFileSize</c>.</summary>
        public long MaxBytes { get; set; } = 2 * 1024 * 1024;

        /// <summary>When true, <see cref="Store"/> throws — the "the store is down" path of the lab.</summary>
        public bool SimulateFailure { get; set; }

        /// <summary>Everything received in this session, newest last.</summary>
        public IReadOnlyList<StoredDocument> Documents => _documents;

        /// <summary>The document the dashboard previews, or null when nothing has been uploaded yet.</summary>
        public StoredDocument Latest => _documents.Count == 0 ? null : _documents[_documents.Count - 1];

        /// <summary>Human-readable limit for the hint under the Upload control ("2 MB").</summary>
        public string MaxSizeText => FormatSize(MaxBytes);

        /// <summary>
        /// The server-side check. Returns null when the file is acceptable, otherwise the sentence to show the user.
        /// Runs before a single byte is stored.
        /// </summary>
        public string Validate(string fileName, long sizeBytes)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return "The file has no name — please choose the report again.";

            var extension = Path.GetExtension(fileName);
            if (!IsAllowedExtension(extension))
                return "Only " + string.Join(" / ", AllowedExtensions) + " reports are accepted — " +
                       (string.IsNullOrEmpty(extension) ? "that file has no extension." : "that file is a " + extension + " file.");

            if (sizeBytes <= 0)
                return "That file is empty.";

            if (MaxBytes > 0 && sizeBytes > MaxBytes)
                return "That file is " + FormatSize(sizeBytes) + " — the limit is " + MaxSizeText + ".";

            return null;
        }

        /// <summary>
        /// Validates and stores the uploaded bytes. Throws only when <see cref="SimulateFailure"/> is set;
        /// an ordinary rejection is a <see cref="StoreResult"/>, not an exception.
        /// </summary>
        public StoreResult Store(string fileName, string contentType, Stream content)
        {
            if (SimulateFailure)
                throw new InvalidOperationException("The document store did not answer.");

            if (content == null)
                return StoreResult.Rejected("Nothing arrived on the server — please try the upload again.");

            var bytes = ReadAllBytes(content);

            // Validate against the bytes that ACTUALLY arrived, not against what the browser announced.
            var rejection = Validate(fileName, bytes.LongLength);
            if (rejection != null)
                return StoreResult.Rejected(rejection);

            _nextId++;
            var document = new StoredDocument(
                "DOC-" + _nextId.ToString("000000"),
                Path.GetFileName(fileName),
                string.IsNullOrEmpty(contentType) ? "application/pdf" : contentType,
                bytes,
                DateTime.Now);

            _documents.Add(document);
            return StoreResult.Ok(document);
        }

        /// <summary>Opens a read-only stream over a stored document — what <c>PdfViewer.PdfStream</c> is fed with.</summary>
        public Stream OpenRead(string id)
        {
            foreach (var document in _documents)
                if (document.Id == id)
                    return new MemoryStream(document.Content, false);

            return null;
        }

        /// <summary>"2 MB", "512 KB", "914 bytes".</summary>
        public static string FormatSize(long bytes)
        {
            if (bytes >= 1024 * 1024)
                return (bytes / (1024.0 * 1024.0)).ToString("0.#") + " MB";
            if (bytes >= 1024)
                return (bytes / 1024.0).ToString("0.#") + " KB";
            return bytes + " bytes";
        }

        private bool IsAllowedExtension(string extension)
        {
            if (AllowedExtensions == null || AllowedExtensions.Length == 0)
                return true;

            foreach (var allowed in AllowedExtensions)
                if (string.Equals(allowed, extension, StringComparison.OrdinalIgnoreCase))
                    return true;

            return false;
        }

        private static byte[] ReadAllBytes(Stream stream)
        {
            if (stream is MemoryStream memory)
                return memory.ToArray();

            using (var buffer = new MemoryStream())
            {
                if (stream.CanSeek)
                    stream.Position = 0;

                stream.CopyTo(buffer);
                return buffer.ToArray();
            }
        }
    }
}
