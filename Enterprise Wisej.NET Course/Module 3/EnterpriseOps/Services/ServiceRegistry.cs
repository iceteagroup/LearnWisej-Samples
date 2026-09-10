using System;
using EnterpriseOps.Data;
using EnterpriseOps.Diagnostics;
using EnterpriseOps.Security;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// The composition root for one session. Note what it is not: there is no <c>ServiceRegistry.Current</c>
    /// static anywhere in this file. A static registry would be the leak the module is about — every session
    /// on the server would share one graph of objects, and whichever session signed in last would own it.
    ///
    /// Instead the page creates one registry, the registry creates one graph, and the graph lives and dies
    /// with the session — except for the two things that are deliberately application-scoped and documented
    /// as such: the work order store (or two tabs could never collide) and the audit trail (or an audit would
    /// vanish when its author closed the browser).
    ///
    /// Which services can safely be shared? Ones that hold no per-user state and synchronize what they do
    /// hold. Everything that touches identity — the session context, the guard's trace, the error log — is
    /// per session, because sharing it would mean sharing a user.
    /// </summary>
    public sealed class ServiceRegistry
    {
        private ServiceRegistry(SessionContext session, ActivityTrace trace)
        {
            Session = session;
            Trace = trace;
            Log = new ErrorLog(trace);

            // Application-scoped, documented, synchronized — the only two things shared between sessions.
            Store = WorkOrderStore.Shared;
            Audit = AuditTrail.Shared;

            // Per-session instances. Each holds a reference to this session's trace, nothing else about the user.
            TenantGuard = new TenantGuard(trace);
            WorkOrders = new WorkOrderService(Store, TenantGuard, Audit, trace);
            Conflicts = new ConflictResolutionService(Audit, trace);
            OtherSession = new OtherSessionSimulator(Store, Audit, trace);
            StateAudit = new StateAuditService(Audit, trace);
        }

        public SessionContext Session { get; }
        public ActivityTrace Trace { get; }
        public ErrorLog Log { get; }
        public IWorkOrderStore Store { get; }
        public AuditTrail Audit { get; }
        public ITenantGuard TenantGuard { get; }
        public WorkOrderService WorkOrders { get; }
        public ConflictResolutionService Conflicts { get; }
        public OtherSessionSimulator OtherSession { get; }
        public StateAuditService StateAudit { get; }

        /// <summary>
        /// Signs the user in from the identity provider and builds the session's service graph. Called once,
        /// by the screen's constructor; the resulting <see cref="SessionContext"/> is what goes into
        /// <c>Application.Session</c>.
        /// </summary>
        public static ServiceRegistry CreateForSession(string userId, string sessionId, ActivityTrace trace)
        {
            if (trace == null) throw new ArgumentNullException(nameof(trace));

            VerifiedIdentity identity = new IdentityProvider().SignIn(userId);
            var session = SessionContext.Create(identity, sessionId);

            trace.Session($"signed in {session.UserId} ({session.DisplayName}) · roles {string.Join("/", session.Roles)} · " +
                          $"entitled to {string.Join(", ", session.EntitledTenants)} · default tenant {session.TenantId}");

            return new ServiceRegistry(session, trace);
        }
    }
}
