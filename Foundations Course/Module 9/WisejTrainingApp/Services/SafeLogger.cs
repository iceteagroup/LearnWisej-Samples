using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;

namespace WisejTrainingApp.Services
{
    /// <summary>
    /// The troubleshooting log from lesson s42 §2: short, readable, timestamped — and it never
    /// records passwords, license keys or private data. Every line goes through <see cref="Redact"/>
    /// before it is stored, so a careless caller cannot leak a secret into the log by accident.
    ///
    /// One instance per window (per user session). Lines are kept in memory for the on-screen list
    /// and mirrored to <see cref="Trace"/> — the "server-side log" of this lab.
    /// </summary>
    public class SafeLogger
    {
        // key=..., password=..., pwd=..., secret=..., token=..., licensekey=..., connectionstring=...
        // Value ends at ';', ',', whitespace, a quote or the end of the line.
        private static readonly Regex SecretPattern = new Regex(
            @"(?<name>\b(?:api[_ -]?key|license[_ -]?key|connection[_ -]?string|password|pwd|secret|token|key)\b\s*[=:]\s*)(?<value>[^;,\s""']+)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private readonly List<string> _entries = new List<string>();

        public IReadOnlyList<string> Entries => _entries;

        /// <summary>Redacts, stamps and stores the line; returns the stored line so the UI can show it.</summary>
        public string Log(string message)
        {
            string time = DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
            string line = $"{time}  {Redact(message ?? string.Empty)}";
            _entries.Add(line);
            Trace.WriteLine("[ReleaseReview] " + line);
            return line;
        }

        /// <summary>Masks anything that looks like <c>name=value</c> for a secret-ish name.</summary>
        public static string Redact(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            return SecretPattern.Replace(text, m => m.Groups["name"].Value + "•••[redacted]");
        }
    }
}
