using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace AccessOps.Services
{
    public enum UploadCheck { Size, Extension, Signature, Scan }

    /// <summary>One gate's verdict, in the order the gates ran - the "what the Uploaded handler did" list.</summary>
    public sealed record UploadGateStep(UploadCheck Check, bool Passed, string Detail);

    public sealed class UploadDecision
    {
        public bool Accepted { get; init; }
        public string DisplayName { get; init; }
        public string ContentType { get; init; }
        public IReadOnlyList<UploadGateStep> Steps { get; init; }

        /// <summary>The sentence for the user: the rule they broke and the limits to meet. No internals.</summary>
        public string Message { get; init; }

        public UploadCheck? FailedCheck => this.Steps.FirstOrDefault(s => !s.Passed)?.Check;
    }

    /// <summary>
    /// Everything about an upload came from the browser: the name, the extension, the declared type,
    /// the length. So the gate checks, in order: size, extension against an allow-list, the content
    /// signature (the first bytes), and a scan - and only a file that passed all four is stored.
    /// </summary>
    public sealed class UploadGate
    {
        public const string Rule = "Upload a PDF, Word or Excel document up to 20 MB.";

        private static readonly Dictionary<string, (byte[] Signature, string ContentType, string Kind)> Allowed = new(StringComparer.OrdinalIgnoreCase)
        {
            [".pdf"] = (new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2D }, "application/pdf", "PDF"),                                       // %PDF-
            [".docx"] = (new byte[] { 0x50, 0x4B, 0x03, 0x04 }, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "Word"),  // PK..
            [".xlsx"] = (new byte[] { 0x50, 0x4B, 0x03, 0x04 }, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Excel"),
        };

        // the EICAR anti-virus test string: the stand-in scanner treats it as malware
        private static readonly byte[] Eicar = Encoding.ASCII.GetBytes("EICAR-STANDARD-ANTIVIRUS-TEST-FILE");

        public UploadGate(long maxBytes) => this.MaxBytes = maxBytes;

        public long MaxBytes { get; }

        /// <summary>The user's file name reduced to display text: no path, no control characters, bounded.</summary>
        public static string DisplayName(string clientName)
        {
            var name = Path.GetFileName((clientName ?? "").Replace('\\', '/').Split('/').Last());
            name = new string(name.Where(c => !char.IsControl(c)).ToArray()).Trim();
            if (name.Length > 120)
                name = name.Substring(0, 120);
            return name.Length == 0 ? "unnamed" : name;
        }

        public UploadDecision Check(string clientName, byte[] content)
        {
            var name = DisplayName(clientName);
            var steps = new List<UploadGateStep>();
            UploadDecision Refuse(UploadCheck check, string detail, string message)
            {
                steps.Add(new UploadGateStep(check, false, detail));
                return new UploadDecision { Accepted = false, DisplayName = name, Steps = steps, Message = message };
            }

            // 1. size - the host limit is the same ceiling, so a hand-built post cannot go around this
            if (content.LongLength == 0 || content.LongLength > this.MaxBytes)
                return Refuse(UploadCheck.Size, $"{name} · {content.LongLength / 1024} KB · limit {this.MaxBytes / 1048576} MB",
                    $"{name} was refused: the file is empty or larger than 20 MB. {Rule}");
            steps.Add(new UploadGateStep(UploadCheck.Size, true, $"{name} · {Math.Max(1, content.LongLength / 1024)} KB · under MaxFileSize ({this.MaxBytes / 1048576} MB)"));

            // 2. extension - an allow-list of what the feature needs
            var ext = Path.GetExtension(name);
            if (!Allowed.TryGetValue(ext, out var format))
                return Refuse(UploadCheck.Extension, $"{(ext.Length == 0 ? "(none)" : ext)} is not on the allow-list",
                    $"{name} was refused: that file type is not accepted. {Rule}");
            steps.Add(new UploadGateStep(UploadCheck.Extension, true, $"{ext.ToLowerInvariant()} is on the allow-list"));

            // 3. signature - the first bytes, because the extension and the declared type are the client's word
            var head = content.Take(format.Signature.Length).ToArray();
            if (!head.SequenceEqual(format.Signature) || (format.Kind != "PDF" && !IsOfficePackage(content, format.Kind)))
                return Refuse(UploadCheck.Signature, $"first bytes {Hex(content.Take(4))} — expected {Hex(format.Signature.Take(4))}",
                    $"{name} was refused: the file contents are not a {format.Kind} document. {Rule}");
            steps.Add(new UploadGateStep(UploadCheck.Signature, true, $"{Hex(head.Take(4))} · signature matches"));

            // 4. scan (a stand-in for the organisation's malware scanner: the EICAR test string)
            if (IndexOf(content, Eicar) >= 0)
                return Refuse(UploadCheck.Scan, "scanner flagged the content",
                    $"{name} was refused: the file did not pass the malware scan.");
            steps.Add(new UploadGateStep(UploadCheck.Scan, true, "scanned clean"));

            return new UploadDecision { Accepted = true, DisplayName = name, ContentType = format.ContentType, Steps = steps, Message = null };
        }

        private static bool IsOfficePackage(byte[] content, string kind)
        {
            try
            {
                using var zip = new ZipArchive(new MemoryStream(content), ZipArchiveMode.Read);
                var marker = kind == "Word" ? "word/" : "xl/";
                return zip.Entries.Any(e => e.FullName.StartsWith(marker, StringComparison.Ordinal))
                    && zip.Entries.Any(e => e.FullName == "[Content_Types].xml");
            }
            catch (InvalidDataException)
            {
                return false;
            }
        }

        private static string Hex(IEnumerable<byte> bytes) => string.Join(" ", bytes.Select(b => b.ToString("X2")));

        private static int IndexOf(byte[] haystack, byte[] needle)
        {
            for (int i = 0; i <= haystack.Length - needle.Length; i++)
            {
                int j = 0;
                while (j < needle.Length && haystack[i + j] == needle[j]) j++;
                if (j == needle.Length) return i;
            }
            return -1;
        }
    }
}
