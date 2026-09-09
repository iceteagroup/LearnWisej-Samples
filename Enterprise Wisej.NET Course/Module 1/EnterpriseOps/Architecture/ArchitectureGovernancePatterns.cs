// ArchitectureGovernancePatterns.cs
// Verbatim from the Module 1 lesson resource (and the walkthrough video's review-gate scene).
// - ArchitectureDecision: the shape of an ADR (see DecisionLog.cs for ADR-001 as data, ADR-001-SolutionStructure.md as prose)
// - IWorkflowScreen:      every course screen implements it (CommandCenterDashboard does)
// - ReviewGate:           the two rules that catch a fat handler before merge (ReviewGateService runs them on real files)
using System;
using System.Collections.Generic;

namespace EnterpriseOps.Architecture;

public sealed record ArchitectureDecision(
    string Id,
    string Title,
    string Context,
    string Decision,
    string Consequences,
    string Owner,
    DateTime Date,
    DateTime? ReviewDate);

public interface IWorkflowScreen
{
    string ScreenName { get; }
    void LoadScreen();
    bool CanClose();
}

public static class ReviewGate
{
    public static IReadOnlyList<string> CheckEventHandler(
        string handlerName, int approximateLines, bool callsService)
    {
        var issues = new List<string>();

        if (approximateLines > 20)
            issues.Add($"{handlerName} is too large; " +
                "move decisions into a service.");

        if (!callsService)
            issues.Add($"{handlerName} does not call an application " +
                "service; verify business logic location.");

        return issues;
    }
}
