using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Security.Principal;
using AccessOps.Identity;
using Microsoft.Extensions.Logging;

namespace AccessOps.Documents
{
    /// <summary>One row of the identity trace as the screen shows it (names only: no SID, no path, no exception text).</summary>
    public sealed class IdentityTraceRow
    {
        public string Document { get; init; }
        public string Step { get; init; }
        public string AppSubject { get; init; }
        public string RequestIdentity { get; init; }
        public string EffectiveToken { get; init; }
        public string Outcome { get; init; }

        /// <summary>"ok", "bad", "warn" or "idle" (the chip colour).</summary>
        public string Tone { get; init; }
    }

    /// <summary>
    /// The before / inside / after trace of one scoped read (Module 3 lab): the app subject, the
    /// request identity and <c>WindowsIdentity.GetCurrent()</c> (<c>Name</c>, <c>User</c>,
    /// <c>ImpersonationLevel</c>) at each step. The caller passes identities it disposes; the trace
    /// copies values and keeps no identity or token. Screen rows carry names only; the log line
    /// (<see cref="IdentityTraceLog"/>) keeps the detail: SIDs, the logon-type group of the token,
    /// the path and the exception.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class IdentityTrace
    {
        private readonly List<IdentityTraceRow> _rows = new List<IdentityTraceRow>();
        private readonly IdentityTraceLog _log;
        private readonly string _document;
        private readonly Subject _subject;
        private string _requestName = "(none)";
        private string _requestSid = "-";
        private string _beforeSid;
        private TokenFacts _before;

        /// <summary>Starts the trace of one read of <paramref name="documentId"/> for <paramref name="subject"/>.</summary>
        public IdentityTrace(string documentId, Subject subject, IdentityTraceLog log)
        {
            _document = documentId;
            _subject = subject;
            _log = log;
        }

        /// <summary>The rows recorded so far.</summary>
        public IReadOnlyList<IdentityTraceRow> Rows => _rows;

        /// <summary>True when the read reached the impersonation scope.</summary>
        public bool ScopeOpened => _rows.Any(r => r.Step == "Inside");

        /// <summary>The exception the scoped read threw, if any.</summary>
        public Exception InsideFailure { get; private set; }

        /// <summary>Bytes read inside the scope.</summary>
        public int BytesRead { get; private set; }

        /// <summary>Records the request identity (copied: name and SID).</summary>
        public void SetRequestIdentity(WindowsIdentity request)
        {
            _requestName = request.Name;
            _requestSid = request.User?.Value ?? "-";
        }

        /// <summary>The application decision refused the document: no scope was opened.</summary>
        public void Policy(WindowsIdentity current, string reason)
            => Add("Policy", current, "Denied before any scope", "bad", "policy deny reason=" + reason);

        /// <summary>The "before" row, with the baseline outcome of the process token on the same file.</summary>
        public void Before(WindowsIdentity current, Exception baseline)
        {
            _before = Facts(current);
            _beforeSid = _before.Sid;
            var (outcome, tone) = baseline switch
            {
                null => ("Readable as pool", "ok"),
                UnauthorizedAccessException => ("Access denied (ACL)", "bad"),
                FileNotFoundException or DirectoryNotFoundException => ("File missing", "warn"),
                _ => (baseline.GetType().Name, "warn")
            };
            Add("Before", current, outcome, tone, baseline == null ? "baseline open as process: ok" : "baseline open as process: " + Detail(baseline));
        }

        /// <summary>The "inside" row: the token inside <c>RunImpersonated</c> and the read's outcome.</summary>
        public void Inside(WindowsIdentity current, int bytes, Exception failure)
        {
            InsideFailure = failure;
            BytesRead = bytes;
            var (outcome, tone) = failure switch
            {
                null => (bytes >= 1024 ? $"Read {Math.Round(bytes / 1024.0)} KB" : $"Read {bytes} bytes", "ok"),
                UnauthorizedAccessException => ("Access denied (ACL)", "bad"),
                FileNotFoundException => ("FileNotFoundException", "warn"),
                DirectoryNotFoundException => ("DirectoryNotFoundException", "warn"),
                _ => (failure.GetType().Name, "warn")
            };
            Add("Inside", current, outcome, tone, failure == null ? $"read {bytes} bytes" : "scoped read threw " + Detail(failure));
        }

        /// <summary>The "after" row: the token once the scope closed (returned or threw).</summary>
        public void After(WindowsIdentity current, bool failed)
        {
            var facts = Facts(current);
            var reverted = facts.Sid == _beforeSid && facts.Level == _before.Level && facts.LogonGroup == _before.LogonGroup;
            Add("After", current, failed ? "Reverted" : "Scope closed", failed ? "ok" : "idle",
                $"same token as before: {reverted}");
        }

        private void Add(string step, WindowsIdentity current, string outcome, string tone, string detail)
        {
            var facts = Facts(current);
            var row = new IdentityTraceRow
            {
                Document = _document,
                Step = step,
                AppSubject = _subject?.Redacted ?? "none",
                RequestIdentity = _requestName,
                EffectiveToken = facts.Level == TokenImpersonationLevel.None ? facts.Name : $"{facts.Name} · {facts.Level}",
                Outcome = outcome,
                Tone = tone
            };
            _rows.Add(row);
            _log.Write(row, _requestSid, facts, detail, _subject?.Opaque ?? "anonymous");
        }

        private static string Detail(Exception ex) => $"{ex.GetType().Name}: {ex.Message}";

        private static TokenFacts Facts(WindowsIdentity identity)
        {
            var groups = identity.Groups?.Select(g => g.Value).ToHashSet() ?? new HashSet<string>();
            var logon = groups.Contains("S-1-5-2") ? "NETWORK"
                : groups.Contains("S-1-5-4") ? "INTERACTIVE"
                : groups.Contains("S-1-5-6") ? "SERVICE"
                : groups.Contains("S-1-5-3") ? "BATCH"
                : "other";
            return new TokenFacts(identity.Name, identity.User?.Value ?? "-", identity.ImpersonationLevel, logon);
        }
    }

    /// <summary>The values copied from a token (never the token).</summary>
    public readonly record struct TokenFacts(string Name, string Sid, TokenImpersonationLevel Level, string LogonGroup);

    /// <summary>
    /// The log side of the identity trace (logger <c>AccessOps.IdentityTrace</c>): every row with
    /// the SIDs, the token's logon-type group (NETWORK for the Negotiate logon, INTERACTIVE for the
    /// process here; the pool's SERVICE/BATCH on IIS), the path or exception detail and the opaque
    /// subject. This is where the detail lives; the screen shows plain messages.
    /// </summary>
    public sealed class IdentityTraceLog
    {
        private readonly ILogger _logger;

        public IdentityTraceLog(ILoggerFactory loggers)
        {
            _logger = loggers.CreateLogger("AccessOps.IdentityTrace");
        }

        /// <summary>Writes one trace row with its detail.</summary>
        public void Write(IdentityTraceRow row, string requestSid, TokenFacts effective, string detail, string opaqueSubject)
        {
            _logger.LogInformation(
                "identity-trace doc={Document} step={Step} appSubject={AppSubject} subject={Opaque} request={Request} requestSid={RequestSid} effective={Effective} effectiveSid={EffectiveSid} level={Level} logon={Logon} outcome={Outcome} detail={Detail}",
                row.Document, row.Step, row.AppSubject, opaqueSubject, row.RequestIdentity, requestSid,
                effective.Name, effective.Sid, effective.Level, effective.LogonGroup, row.Outcome, detail);
        }
    }
}
