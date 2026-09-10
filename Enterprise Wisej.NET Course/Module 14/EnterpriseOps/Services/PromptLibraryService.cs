using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnterpriseOps.Data;
using EnterpriseOps.Diagnostics;
using EnterpriseOps.Domain;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// The AI prompt library: the project rules every AI session starts with, plus the task prompts that
    /// build on them. Read from <c>docs/PromptLibrary.md</c>, so the document the team edits is the document
    /// the app shows — a prompt library that lives in a screenshot is a prompt library nobody updates.
    ///
    /// The rule that matters is number 5: <b>do not use an API unless it exists in the official Wisej.NET
    /// documentation</b>. Checklist rule Q1 is that rule, enforced.
    /// </summary>
    public sealed class PromptLibraryService
    {
        private readonly DocsFolder _docs;
        private readonly ActivityTrace _trace;
        private List<PromptEntry> _cache;

        public PromptLibraryService(DocsFolder docs, ActivityTrace trace)
        {
            _docs = docs;
            _trace = trace;
        }

        public const string LibraryPath = "docs/PromptLibrary.md";

        /// <summary>The placeholder every task prompt in the library uses to pull the header in.</summary>
        public const string HeaderPlaceholder = "<<prompt header>>";

        /// <summary>
        /// The project rules that open every AI session — the same six the walkthrough shows in
        /// <c>AIAssistedReviewPatterns.md</c>. Constant reference data, not session state.
        /// </summary>
        public static readonly string[] HeaderRules =
        {
            "Keep UI event handlers thin.",
            "Put workflow, validation, authorization, persistence, and integration logic in services.",
            "Treat JavaScript as enhancement only.",
            "Preserve session and tenant safety.",
            "Do not use APIs unless they exist in the official Wisej.NET documentation.",
            "Include failure paths and review notes.",
        };

        /// <summary>The header as it is pasted into a session.</summary>
        public static string HeaderText
        {
            get
            {
                var text = new StringBuilder("You are working on a Wisej.NET enterprise application. Follow these rules:");
                for (int i = 0; i < HeaderRules.Length; i++)
                    text.Append(Environment.NewLine).Append(i + 1).Append(". ").Append(HeaderRules[i]);
                return text.ToString();
            }
        }

        /// <summary>Parses the library. Each "### " section is one prompt; the fenced block is its body.</summary>
        public List<PromptEntry> Load()
        {
            if (_cache != null)
                return _cache;

            var entries = new List<PromptEntry>();
            string markdown = _docs.ReadAllText(LibraryPath);
            if (markdown == null)
            {
                _trace.Docs($"{LibraryPath} not found — the prompt library is part of the deliverable, not an optional extra");
                _cache = entries;
                return entries;
            }

            PromptEntry current = null;
            var body = new StringBuilder();
            bool inFence = false;

            foreach (string raw in markdown.Replace("\r\n", "\n").Split('\n'))
            {
                string line = raw.TrimEnd();

                if (line.StartsWith("### ", StringComparison.Ordinal))
                {
                    Close(entries, current, body);
                    current = NewEntry(line.Substring(4).Trim());
                    body.Clear();
                    inFence = false;
                    continue;
                }

                if (current == null)
                    continue;

                if (line.StartsWith("```", StringComparison.Ordinal))
                {
                    inFence = !inFence;
                    continue;
                }

                if (inFence)
                    body.Append(line).Append(Environment.NewLine);
                else if (line.StartsWith("- **Purpose:**", StringComparison.Ordinal))
                    current.Purpose = line.Substring("- **Purpose:**".Length).Trim();
            }

            Close(entries, current, body);
            _cache = entries;
            _trace.Docs($"{LibraryPath} → {entries.Count} prompts, {entries.Count(e => e.IncludesHeader)} of them pull in the project rules header");
            return entries;
        }

        /// <summary>Expands a prompt for pasting: the header first, then the task.</summary>
        public string Expand(PromptEntry entry)
        {
            if (entry == null)
                return HeaderText;
            return (entry.Body ?? "").Replace(HeaderPlaceholder, HeaderText);
        }

        private static PromptEntry NewEntry(string heading)
        {
            int separator = heading.IndexOf('·');
            return new PromptEntry
            {
                Id = separator > 0 ? heading.Substring(0, separator).Trim() : heading,
                Title = separator > 0 ? heading.Substring(separator + 1).Trim() : heading,
            };
        }

        private static void Close(List<PromptEntry> entries, PromptEntry entry, StringBuilder body)
        {
            if (entry == null)
                return;
            entry.Body = body.ToString().TrimEnd();
            entry.IncludesHeader = entry.Body.IndexOf(HeaderPlaceholder, StringComparison.Ordinal) >= 0;
            entries.Add(entry);
        }
    }
}
