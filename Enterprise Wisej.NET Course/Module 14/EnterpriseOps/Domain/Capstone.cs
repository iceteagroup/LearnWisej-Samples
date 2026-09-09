using System;
using System.Collections.Generic;

namespace EnterpriseOps.Domain
{
    /// <summary>One entry of the AI prompt library (a "### " section of docs/PromptLibrary.md).</summary>
    public sealed class PromptEntry
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Purpose { get; set; }
        public string Body { get; set; }
        public bool IncludesHeader { get; set; }    // does the prompt start from the project prompt header?

        public override string ToString() => Title;
    }

    /// <summary>One document of the MCP-ready index (docs/index.json). Title, purpose, path — what a tool needs to retrieve it.</summary>
    public sealed class DocEntry
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Purpose { get; set; }
        public string Path { get; set; }
        public string Module { get; set; }
        public string LastVerified { get; set; }
        public List<string> Decisions { get; set; } = new List<string>();
        public bool Exists { get; set; }
        public long Bytes { get; set; }
    }

    /// <summary>One line of the capstone-package self-check ("Verify capstone package").</summary>
    public sealed class PackageCheck
    {
        public string Deliverable { get; set; }
        public string Path { get; set; }
        public bool Passed { get; set; }
        public string Detail { get; set; }
        public bool Required { get; set; }
    }

    /// <summary>The numbers the dashboard cards show. A projection built by DashboardService — never an entity.</summary>
    public sealed class DashboardKpis
    {
        public int Open { get; set; }
        public int Escalated { get; set; }
        public int DueToday { get; set; }
        public int Overdue { get; set; }
        public int CompletedLast7Days { get; set; }
        public int Total { get; set; }
        public string TenantId { get; set; }
        public DateTime ComputedUtc { get; set; }
    }

    public enum ProbeState { Pending, Running, Healthy, Degraded, Failed }

    /// <summary>A health probe of the diagnostics card (Module 11/12 pattern, compacted for the capstone).</summary>
    public sealed class HealthProbe
    {
        public string Name { get; set; }
        public ProbeState State { get; set; }
        public string Detail { get; set; }
        public int DurationMs { get; set; }
    }
}
