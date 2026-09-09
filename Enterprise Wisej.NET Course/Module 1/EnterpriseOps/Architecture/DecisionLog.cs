using System;
using System.Collections.Generic;

namespace EnterpriseOps.Architecture
{
    /// <summary>
    /// The team's ADRs as data, so a screen (or a diagnostics page) can list them with their review dates.
    /// The prose version of each decision sits next to this file: Architecture/ADR-NNN-*.md.
    /// </summary>
    public static class DecisionLog
    {
        public static readonly ArchitectureDecision Adr001SolutionStructure = new ArchitectureDecision(
            Id: "ADR-001",
            Title: "Solution structure",
            Context: "EnterpriseOps will grow: more screens, more integrations, multiple teams. One project with mixed concerns will not survive that growth.",
            Decision: "Split by responsibility — UI · Controls · Domain · Services · Data · Integrations — with Security and Diagnostics as their own layers, plus Resources and Deployment assets. One project, folder-per-layer, namespaces follow folders; screens stay designable; durable decisions live in services and contracts.",
            Consequences: "New screens follow the reference example. Workflow logic is findable by folder name. The designer keeps working — orchestration never lives in event handlers.",
            Owner: "Tech lead",
            Date: new DateTime(2026, 9, 9),
            ReviewDate: new DateTime(2027, 3, 9));

        public static IReadOnlyList<ArchitectureDecision> All => new[] { Adr001SolutionStructure };
    }
}
