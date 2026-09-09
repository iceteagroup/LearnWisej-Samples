using System;
using System.Collections.Generic;

namespace EnterpriseOps.Domain
{
    /// <summary>How serious a checklist finding is. One <see cref="Reject"/> finding rejects the change.</summary>
    public enum ReviewSeverity { Warning, Reject }

    public enum ReviewVerdict { Pending, Accepted, AcceptedWithWarnings, Rejected }

    /// <summary>One question of the generated-code review checklist (docs/GeneratedCodeReviewChecklist.md).</summary>
    public sealed class ReviewRule
    {
        public string Id { get; set; }              // "Q1" … "Q6", "R7" … "R10"
        public string Question { get; set; }        // the checklist wording
        public ReviewSeverity Severity { get; set; }
        public string DocReference { get; set; }    // where the rule is written down (docs/…#anchor)
    }

    /// <summary>A rule that fired, with the evidence the reviewer can point at.</summary>
    public sealed class ReviewFinding
    {
        public string RuleId { get; set; }
        public ReviewSeverity Severity { get; set; }
        public int Line { get; set; }
        public string Evidence { get; set; }        // the offending source line, trimmed
        public string Message { get; set; }         // why it fails the checklist
        public string Fix { get; set; }             // what the accepted version does instead
        public string DocReference { get; set; }

        public override string ToString() => $"{RuleId} L{Line}: {Message}";
    }

    /// <summary>The result of running the checklist over one generated change.</summary>
    public sealed class ReviewReport
    {
        public string PullRequest { get; set; }
        public string Author { get; set; }          // "ai-assistant" for generated code
        public string ReviewedBy { get; set; }
        public DateTime ReviewedUtc { get; set; }
        public int RulesRun { get; set; }
        public int LinesReviewed { get; set; }
        public ReviewVerdict Verdict { get; set; }
        public List<ReviewFinding> Findings { get; } = new List<ReviewFinding>();
        public string CorrelationId { get; set; }
    }

    /// <summary>
    /// The record the lab asks for: which generated code was accepted, by whom and why.
    /// Appended to the AI usage notes (in memory here; docs/AIUsageNotes.md is the written copy).
    /// </summary>
    public sealed class ReviewDecision
    {
        public DateTime DecidedUtc { get; set; }
        public string PullRequest { get; set; }
        public ReviewVerdict Verdict { get; set; }
        public string Reviewer { get; set; }
        public string Author { get; set; }
        public string Reason { get; set; }
        public string CorrelationId { get; set; }
    }
}
