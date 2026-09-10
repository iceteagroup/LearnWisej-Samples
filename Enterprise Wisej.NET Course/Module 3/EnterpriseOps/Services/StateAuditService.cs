using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EnterpriseOps.Diagnostics;
using EnterpriseOps.Security;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// Runs the two halves of the module's audit against the running assembly:
    ///
    ///  • <see cref="RunAudit"/> — every static field in the solution, classified: documented application or
    ///    tenant state, immutable reference data, or a finding;
    ///  • <see cref="DemonstrateLeak"/> — the classic leak, performed safely: a legacy helper "remembers" the
    ///    current user in a static, a second sign-in overwrites it, and this session's own view of who it is
    ///    is compared with what the static now claims. Nothing in the sample authorizes on the static; that is
    ///    the point — in the code this is modelled on, something did.
    /// </summary>
    public sealed class StateAuditService
    {
        private readonly AuditTrail _audit;
        private readonly ActivityTrace _trace;

        public StateAuditService(AuditTrail audit, ActivityTrace trace)
        {
            _audit = audit;
            _trace = trace;
        }

        public StaticStateReport RunAudit(CommandContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            Assembly assembly = typeof(StateAuditService).Assembly;
            IReadOnlyList<StaticStateFinding> all = new StaticStateAudit().Run(assembly);

            var report = new StaticStateReport
            {
                Assembly = assembly.GetName().Name,
                Entries = all,
                CorrelationId = context.CorrelationId,
            };

            _trace.Security($"StaticStateAudit over {report.Assembly}: {all.Count} statics · {report.Findings.Count} findings · " +
                            $"{report.Documented.Count} documented · {report.Immutable.Count} immutable");

            foreach (StaticStateFinding entry in all.OrderBy(e => e.Verdict == StaticStateVerdict.Finding ? 0 : 1))
                _trace.Security("  " + entry);

            _audit.Record(context, "state.audit", $"{all.Count} statics inspected, {report.Findings.Count} findings");
            return report;
        }

        /// <summary>
        /// The anti-pattern, demonstrated and then answered. Two sign-ins write the same static; the second
        /// wins for everybody. The session context beside it is untouched, because it belongs to this session.
        /// </summary>
        public StaticLeakResult DemonstrateLeak(CommandContext context, SessionContext thisSession)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (thisSession == null) throw new ArgumentNullException(nameof(thisSession));

            // 1. This session signs in through the legacy helper.
            LegacyCurrentUser.Remember(thisSession.UserId, thisSession.TenantId, thisSession.SessionId);
            string afterMine = LegacyCurrentUser.Describe();
            _trace.Security($"LegacyCurrentUser.Remember(this session) → {afterMine}");

            // 2. Another user signs in — a different tenant, a different session, the same static field.
            var other = SessionContext.Create(new IdentityProvider().SignIn("cara.admin"), Guid.NewGuid().ToString("N"));
            LegacyCurrentUser.Remember(other.UserId, other.TenantId, other.SessionId);
            string afterTheirs = LegacyCurrentUser.Describe();
            _trace.Security($"another session signs in … LegacyCurrentUser is now → {afterTheirs}");

            bool leaked = !StringComparer.Ordinal.Equals(LegacyCurrentUser.UserId, thisSession.UserId);

            _trace.Security(leaked
                ? $"LEAK: the static says {LegacyCurrentUser.UserId}@{LegacyCurrentUser.TenantId}, this session is {thisSession.UserId}@{thisSession.TenantId} — " +
                  "code that authorized on the static would now read another tenant's data from this session"
                : "no divergence this time — which is exactly why the bug survives testing on one machine with one user");

            _trace.Security($"SessionContext (untouched, per session) → {thisSession} · tenant {thisSession.TenantId}");
            _audit.Record(context, "state.leak-demo", leaked ? "static overwritten by another session" : "no divergence observed");

            return new StaticLeakResult
            {
                Leaked = leaked,
                AfterThisSession = afterMine,
                AfterOtherSession = afterTheirs,
                StaticUserId = LegacyCurrentUser.UserId,
                StaticTenantId = LegacyCurrentUser.TenantId,
                SessionUserId = thisSession.UserId,
                SessionTenantId = thisSession.TenantId,
                CorrelationId = context.CorrelationId,
            };
        }
    }

    /// <summary>The static-state audit as a report the screen can render and the docs can quote.</summary>
    public sealed class StaticStateReport
    {
        public string Assembly { get; init; }
        public IReadOnlyList<StaticStateFinding> Entries { get; init; }
        public string CorrelationId { get; init; }

        public IReadOnlyList<StaticStateFinding> Findings =>
            Entries.Where(e => e.Verdict == StaticStateVerdict.Finding).ToList();

        public IReadOnlyList<StaticStateFinding> Documented =>
            Entries.Where(e => e.Verdict == StaticStateVerdict.Documented).ToList();

        public IReadOnlyList<StaticStateFinding> Immutable =>
            Entries.Where(e => e.Verdict == StaticStateVerdict.ImmutableReference).ToList();

        public string Headline => Findings.Count == 0
            ? $"Static-state audit — {Entries.Count} statics, 0 findings"
            : $"Static-state audit — {Findings.Count} of {Entries.Count} statics are findings: " +
              string.Join(", ", Findings.Select(f => f.Type + "." + f.Member));
    }

    /// <summary>The leak demonstration, as data.</summary>
    public sealed class StaticLeakResult
    {
        public bool Leaked { get; init; }
        public string AfterThisSession { get; init; }
        public string AfterOtherSession { get; init; }
        public string StaticUserId { get; init; }
        public string StaticTenantId { get; init; }
        public string SessionUserId { get; init; }
        public string SessionTenantId { get; init; }
        public string CorrelationId { get; init; }

        public string Headline => Leaked
            ? $"Static-state leak — LegacyCurrentUser now says {StaticUserId}@{StaticTenantId}, but this session is {SessionUserId}@{SessionTenantId}"
            : "Static-state leak — no divergence observed in this run";
    }
}
