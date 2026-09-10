using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using EnterpriseOps.Data;
using EnterpriseOps.Diagnostics;
using EnterpriseOps.Domain;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// Reads the MCP-ready documentation index — <c>docs/index.json</c> — and checks every entry against the
    /// files on disk.
    ///
    /// Why a JSON index next to the Markdown one: a person browses <c>docs/DocumentationIndex.md</c>, a tool
    /// retrieves <c>docs/index.json</c>. The JSON is shaped like an MCP <c>resources/list</c> answer (uri,
    /// name, description, mimeType) plus the fields a reviewer needs (purpose, module, lastVerified,
    /// decisions), so a documentation MCP server can serve it without another format in the middle.
    ///
    /// Nothing here calls a model or a network: the "MCP-ready" claim is about the <em>shape</em> of the
    /// index, and the lab proves the shape by reading it back and verifying every path resolves.
    /// </summary>
    public sealed class DocumentationIndexService
    {
        private readonly DocsFolder _docs;
        private readonly ActivityTrace _trace;
        private List<DocEntry> _cache;

        public DocumentationIndexService(DocsFolder docs, ActivityTrace trace)
        {
            _docs = docs;
            _trace = trace;
        }

        /// <summary>The index file the tool endpoint would serve.</summary>
        public const string IndexPath = "docs/index.json";

        /// <summary>Set by the "Simulate a missing document" button: an extra entry whose file is not there.</summary>
        public bool SimulateMissingDocument { get; set; }

        /// <summary>The entry the simulation injects — a document the readiness statement lists as an open item.</summary>
        public static DocEntry MissingDocumentProbe => new DocEntry
        {
            Id = "security-review-signoff",
            Title = "Security review sign-off",
            Purpose = "The signed record of the Module 10 security review (open item — owner ana.ops).",
            Path = "docs/SecurityReviewSignOff.md",
            Module = "10",
            LastVerified = "—",
            Exists = false,
        };

        /// <summary>Reads (and caches) the index. A parse failure is reported, never thrown at the user.</summary>
        public List<DocEntry> Load()
        {
            if (_cache != null)
                return WithSimulation(_cache);

            var entries = new List<DocEntry>();
            string json = _docs.ReadAllText(IndexPath);
            if (json == null)
            {
                _trace.Docs($"{IndexPath} not found — the documentation endpoint would answer 404");
                _cache = entries;
                return WithSimulation(entries);
            }

            try
            {
                using (JsonDocument document = JsonDocument.Parse(json))
                {
                    if (document.RootElement.TryGetProperty("resources", out JsonElement resources))
                    {
                        foreach (JsonElement resource in resources.EnumerateArray())
                            entries.Add(ReadEntry(resource));
                    }
                }
            }
            catch (JsonException ex)
            {
                _trace.Docs($"{IndexPath} is not valid JSON — {ex.Message}");
            }

            _cache = entries;
            return WithSimulation(entries);
        }

        /// <summary>Re-reads the index from disk (the recovery path after a document is added or fixed).</summary>
        public List<DocEntry> Reload()
        {
            _cache = null;
            List<DocEntry> entries = Load();
            _trace.Docs($"{IndexPath} reloaded → {entries.Count} resources");
            return entries;
        }

        /// <summary>
        /// The answer a documentation MCP server would return for <c>resources/list</c>, rendered as text so
        /// the reviewer can read the contract in the trace instead of taking it on trust.
        /// </summary>
        public string DescribeResourceList()
        {
            List<DocEntry> entries = Load();
            var text = new StringBuilder();
            text.Append("{\"resources\":[");
            for (int i = 0; i < entries.Count; i++)
            {
                if (i > 0) text.Append(',');
                text.Append("{\"uri\":\"file://").Append(entries[i].Path)
                    .Append("\",\"name\":\"").Append(entries[i].Title)
                    .Append("\",\"mimeType\":\"").Append(entries[i].Path.EndsWith(".svg", StringComparison.OrdinalIgnoreCase) ? "image/svg+xml" : "text/markdown")
                    .Append("\"}");
            }
            text.Append("]}");
            return text.ToString();
        }

        private DocEntry ReadEntry(JsonElement resource)
        {
            var entry = new DocEntry
            {
                Id = Text(resource, "id"),
                Title = Text(resource, "title"),
                Purpose = Text(resource, "purpose"),
                Path = Text(resource, "path"),
                Module = Text(resource, "module"),
                LastVerified = Text(resource, "lastVerified"),
            };

            if (resource.TryGetProperty("decisions", out JsonElement decisions) && decisions.ValueKind == JsonValueKind.Array)
            {
                foreach (JsonElement decision in decisions.EnumerateArray())
                    entry.Decisions.Add(decision.GetString());
            }

            entry.Exists = !string.IsNullOrEmpty(entry.Path) && _docs.Exists(entry.Path);
            entry.Bytes = entry.Exists ? _docs.SizeOf(entry.Path) : 0;
            return entry;
        }

        private static string Text(JsonElement element, string property)
            => element.TryGetProperty(property, out JsonElement value) && value.ValueKind == JsonValueKind.String ? value.GetString() : "";

        private List<DocEntry> WithSimulation(List<DocEntry> entries)
        {
            if (!SimulateMissingDocument)
                return entries;

            var withProbe = new List<DocEntry>(entries) { MissingDocumentProbe };
            return withProbe;
        }
    }
}
