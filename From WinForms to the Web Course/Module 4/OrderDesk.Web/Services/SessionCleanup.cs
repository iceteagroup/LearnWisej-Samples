using System;
using System.Collections.Generic;
using System.IO;

namespace OrderDesk.Services
{
    /// <summary>
    /// The one cleanup routine every exit shares: logout (the user clicks Sign out) and
    /// ApplicationExit (the session is torn down — timeout expired, browser gone; subscribed in
    /// Program.Main). Long-running transactions, temporary files, report jobs and locks must be
    /// released here because an abandoned browser tab never reaches "workflow complete".
    ///
    /// What this sample really owns per session is a workspace folder under &lt;StorageRoot&gt;/tmp
    /// (created at sign-in for temp files and staged report output); the other steps are the
    /// requirement as it stands in a production system.
    ///
    /// Every step runs in its own try/catch: a locked temp file must not stop the steps after it —
    /// SessionContext.Reset() in particular — and nothing may throw out of a Click handler or the
    /// ApplicationExit handler. A failed step is reported as a step line instead.
    /// </summary>
    public static class SessionCleanup
    {
        public static string TempRoot => StorageRoot.Combine("tmp");

        /// <summary>&lt;StorageRoot&gt;/tmp/&lt;session&gt; — the temp files this session would leave behind.</summary>
        public static string WorkspaceFor(string sessionId) => Path.Combine(TempRoot, ShortId(sessionId));

        /// <summary>Called at sign-in: the session now owns server-side resources that must be released later.</summary>
        public static string CreateWorkspace(string sessionId, string userName)
        {
            string folder = WorkspaceFor(sessionId);
            Directory.CreateDirectory(folder);
            File.WriteAllText(Path.Combine(folder, "report-job.txt"), $"invoice batch for {userName} · started {DateTime.Now:HH:mm:ss}");
            return folder;
        }

        /// <summary>
        /// Releases everything this session holds and returns one line per step (logged by Program.Main).
        /// Idempotent: running it on logout and again on ApplicationExit is harmless. Never throws.
        /// </summary>
        public static IList<string> Run(string reason, string sessionId)
        {
            var steps = new List<string>();
            string folder = WorkspaceFor(sessionId);

            // 1. Temp files / staged report output — real in this sample.
            Step(steps, "temp files", $"could not delete {folder}", () =>
            {
                if (!Directory.Exists(folder)) return "nothing to release";
                int files = Directory.GetFiles(folder).Length;
                Directory.Delete(folder, recursive: true);
                return $"deleted {folder} ({files} file(s))";
            });

            // 2–4. Report jobs / long-running transactions / locks — the requirement, logged.
            Step(steps, "report jobs", "could not cancel the session's jobs", () => "cancel queued jobs keyed by this session (none running in the sample)");
            Step(steps, "transactions", "could not roll back", () => "roll back any open unit of work (the in-memory repository holds none)");
            Step(steps, "locks", "could not release", () => "release row/record locks held on behalf of this session (none in the sample)");

            // 5. The per-user context — the typed session context gives one place to clear it. Always reached.
            Step(steps, "UserContext", "SessionContext.Reset() failed", () =>
            {
                SessionContext.Reset();
                return $"SessionContext.Reset() → session {ShortId(sessionId)} is anonymous again ({reason})";
            });

            return steps;
        }

        /// <summary>One guarded step: "&lt;name&gt;: &lt;result&gt;" or "&lt;name&gt;: &lt;failure&gt; (&lt;ExceptionType&gt;: &lt;message&gt;)".</summary>
        private static void Step(ICollection<string> steps, string name, string failure, Func<string> action)
        {
            try
            {
                steps.Add($"{name}: {action()}");
            }
            catch (Exception ex)
            {
                steps.Add($"{name}: {failure} ({ex.GetType().Name}: {ex.Message})");
            }
        }

        public static string ShortId(string id) => string.IsNullOrEmpty(id) ? "?" : (id.Length > 8 ? id.Substring(0, 8) : id);
    }
}
