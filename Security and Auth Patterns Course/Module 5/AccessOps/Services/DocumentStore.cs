using System;
using System.IO;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;

namespace AccessOps.Services
{
    /// <summary>
    /// The document store: two folders outside the web root - quarantine and documents - and file
    /// names the server generates (32 random hex characters). The name a user gave a file is never
    /// part of a path; it is display text in the database.
    /// </summary>
    public sealed class DocumentStore
    {
        public DocumentStore(string documentsFolder, string quarantineFolder)
        {
            this.DocumentsFolder = Path.GetFullPath(documentsFolder);
            this.QuarantineFolder = Path.GetFullPath(quarantineFolder);
            Directory.CreateDirectory(this.DocumentsFolder);
            Directory.CreateDirectory(this.QuarantineFolder);
        }

        public static DocumentStore FromConfiguration(IConfiguration configuration, string contentRoot)
        {
            string Resolve(string key, string fallback)
            {
                var value = configuration[key] ?? fallback;
                return Path.IsPathRooted(value) ? value : Path.Combine(contentRoot, value);
            }

            return new DocumentStore(
                Resolve("Storage:Documents", "../AccessOpsData/documents"),
                Resolve("Storage:Quarantine", "../AccessOpsData/quarantine"));
        }

        public string DocumentsFolder { get; }
        public string QuarantineFolder { get; }

        public static string NewStoredName() => Convert.ToHexString(RandomNumberGenerator.GetBytes(16)).ToLowerInvariant();

        /// <summary>Writes bytes into quarantine under a new server-generated name.</summary>
        public string WriteToQuarantine(byte[] content)
        {
            var name = NewStoredName();
            File.WriteAllBytes(Path.Combine(this.QuarantineFolder, name), content);
            return name;
        }

        public string QuarantinePath(string storedName) => Path.Combine(this.QuarantineFolder, Checked(storedName));

        /// <summary>Moves a scanned-clean file from quarantine into the store.</summary>
        public void Promote(string storedName)
            => File.Move(QuarantinePath(storedName), Path.Combine(this.DocumentsFolder, Checked(storedName)));

        /// <summary>Deletes a refused file from quarantine - a rejection leaves nothing on disk.</summary>
        public void Discard(string storedName)
        {
            var path = QuarantinePath(storedName);
            if (File.Exists(path))
                File.Delete(path);
        }

        public void WriteDocument(string storedName, byte[] content)
            => File.WriteAllBytes(Path.Combine(this.DocumentsFolder, Checked(storedName)), content);

        public byte[] ReadDocument(string storedName)
            => File.ReadAllBytes(Path.Combine(this.DocumentsFolder, Checked(storedName)));

        public bool IsUnderWebRoot(string webRoot)
        {
            var root = Path.GetFullPath(webRoot).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
            return this.DocumentsFolder.StartsWith(root, StringComparison.OrdinalIgnoreCase)
                || this.QuarantineFolder.StartsWith(root, StringComparison.OrdinalIgnoreCase);
        }

        // a stored name is 32 lowercase hex characters, nothing else - so it can never walk out of the folder
        private static string Checked(string storedName)
        {
            if (storedName == null || storedName.Length != 32 || !Uri.IsHexDigit(storedName[0]))
                throw new ArgumentException("Not a stored document name.", nameof(storedName));
            foreach (var c in storedName)
            {
                if (!((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f')))
                    throw new ArgumentException("Not a stored document name.", nameof(storedName));
            }
            return storedName;
        }
    }
}
