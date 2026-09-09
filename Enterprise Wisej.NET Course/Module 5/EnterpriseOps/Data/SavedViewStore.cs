using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using EnterpriseOps.Services.WorkQueues;

namespace EnterpriseOps.Data
{
    /// <summary>
    /// Where saved views live. In production this is a table keyed by (tenant, owner); here it is an
    /// in-memory list owned by the <c>SessionContext</c>, so a view saved in this session survives a page
    /// rebuild ("refresh") but not a process restart. A saved view is a stored <b>query definition</b>
    /// (the serialized <see cref="WorkQueueQuery"/>), never a cached result — see docs/SavedViewDefinition.md.
    /// </summary>
    public sealed class SavedViewStore
    {
        private readonly List<SavedView> _views = new List<SavedView>();
        private int _nextId = 1;

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions { WriteIndented = false };

        public SavedViewStore(string tenantId, string owner)
        {
            // Two views every dispatcher starts with. "My critical queue" is the chip the walkthrough shows.
            Add("My critical queue", tenantId, owner,
                new WorkQueueQuery(tenantId, null, "Open", null, "Priority", true, 1, 50));
            Add("Overdue HVAC", tenantId, owner,
                new WorkQueueQuery(tenantId, "HVAC", "Open", null, "DueAt", false, 1, 50));
        }

        public IReadOnlyList<SavedView> All => _views;

        public SavedView Find(string name) =>
            _views.FirstOrDefault(v => string.Equals(v.Name, name, StringComparison.OrdinalIgnoreCase));

        public SavedView Add(string name, string tenantId, string owner, WorkQueueQuery definition)
        {
            var view = new SavedView
            {
                Id = _nextId++,
                Name = name,
                TenantId = tenantId,
                Owner = owner,
                Definition = definition with { Page = 1 },       // a view never remembers the page
                CreatedUtc = DateTime.UtcNow,
            };
            _views.Add(view);
            return view;
        }

        /// <summary>The definition as it would be stored in the database column.</summary>
        public static string Serialize(WorkQueueQuery definition) => JsonSerializer.Serialize(definition, JsonOptions);

        public static WorkQueueQuery Deserialize(string json) => JsonSerializer.Deserialize<WorkQueueQuery>(json, JsonOptions);

        /// <summary>A readable name derived from the definition, used when the user saves the current filters.</summary>
        public static string SuggestName(WorkQueueQuery q)
        {
            var parts = new List<string>();
            if (!string.IsNullOrWhiteSpace(q.SearchText)) parts.Add("\"" + q.SearchText.Trim() + "\"");
            parts.Add("Status " + (string.IsNullOrEmpty(q.Status) ? "any" : q.Status));
            if (!string.IsNullOrEmpty(q.AssignedTo)) parts.Add("→ " + q.AssignedTo);
            parts.Add(q.SortBy + (q.Descending ? " ↓" : " ↑"));
            return string.Join(" · ", parts);
        }
    }
}
