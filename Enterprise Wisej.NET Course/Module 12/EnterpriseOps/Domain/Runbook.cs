using System.Collections.Generic;

namespace EnterpriseOps.Domain
{
    /// <summary>State of one runbook step as the strip shows it: · pending, ✓ done, ✕ failed, ↩ rollback.</summary>
    public enum RunbookStepState { Pending, Running, Done, Failed, RolledBack }

    /// <summary>
    /// The eight steps of docs/ReleaseRunbook.md, in order. The last one is rollback: a release
    /// procedure that cannot be reversed is not finished.
    /// </summary>
    public class RunbookStep
    {
        public int Number;                   // 1..8
        public string Key;                   // short label on the strip
        public string Title;                 // the runbook sentence
        public RunbookStepState State = RunbookStepState.Pending;

        public static List<RunbookStep> CreateDefault() => new List<RunbookStep>
        {
            new RunbookStep { Number = 1, Key = "build",                Title = "Confirm build number and release notes." },
            new RunbookStep { Number = 2, Key = "config + secrets",     Title = "Verify configuration and secrets." },
            new RunbookStep { Number = 3, Key = "staging",              Title = "Deploy to staging." },
            new RunbookStep { Number = 4, Key = "smoke tests",          Title = "Run smoke tests." },
            new RunbookStep { Number = 5, Key = "health + diagnostics", Title = "Check HealthCheck.json and diagnostics." },
            new RunbookStep { Number = 6, Key = "production",           Title = "Deploy production." },
            new RunbookStep { Number = 7, Key = "monitor",              Title = "Monitor logs and health." },
            new RunbookStep { Number = 8, Key = "rollback",             Title = "Execute rollback if smoke test or health check fails." },
        };
    }

    /// <summary>A versioned, immutable artifact: the thing the pipeline produced and the runbook deploys.</summary>
    public class ReleasePackage
    {
        public string Version;
        public string Artifact;              // e.g. enterpriseops-2.4.2.zip / image tag
        public string ReleaseNotes;
        public bool IntroducesDbMigration;   // 2.4.2 does — that is what breaks app-node-B's database check

        public override string ToString() => Version;
    }
}
