using EnterpriseOps.Data;
using EnterpriseOps.Diagnostics;
using EnterpriseOps.Security;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// The composition root of one browser session: it builds the object graph once and hands the screens
    /// finished services.
    ///
    /// Everything here is **per session** — trace, session context, repositories, permission service — because
    /// every session in the process belongs to a different person. The exceptions are
    /// <see cref="AuditLog.Shared"/>, which is application-scoped on purpose and documented as such (an audit
    /// that vanished with the session would not be an audit), and the pending-export queue inside
    /// <see cref="ExportService"/>, which the requester and a second approver share across sessions.
    ///
    /// Wiring order is the security order: identity → claims mapping → session → role store → permission
    /// service → the services that demand permissions.
    /// </summary>
    public sealed class ServiceRegistry
    {
        public ServiceRegistry(ActivityTrace trace)
        {
            Audit = AuditLog.Shared;
            SeedData.EnsureAuditHistory(AuditLog.Shared);

            Session = new SessionContext(trace);

            var provider = new SimulatedSsoIdentityProvider(trace);
            var mapper = new ClaimsMapper(trace);
            Roles = new RolePermissionStore(trace);

            Permissions = new PermissionService(new TenantGuard(trace), Roles, Audit, trace);

            SignIn = new SignInService(provider, mapper, Session, Roles, Audit, trace);

            // One repository instance for the session: the export must count the same rows the queue shows.
            var repository = new InMemoryWorkOrderRepository(trace);
            WorkOrders = new WorkOrderService(repository, Permissions, Audit, trace);
            Exports = new ExportService(repository, Permissions, Audit, trace);
            AuditQuery = new AuditQueryService(Audit, Permissions, trace);
            Notes = new NoteRenderService(trace);
            NoteStore = new InMemoryNoteStore(trace);
        }

        public SessionContext Session { get; }
        public IAuditLog Audit { get; }
        public RolePermissionStore Roles { get; }
        public IPermissionService Permissions { get; }
        public SignInService SignIn { get; }
        public WorkOrderService WorkOrders { get; }
        public ExportService Exports { get; }
        public AuditQueryService AuditQuery { get; }
        public NoteRenderService Notes { get; }
        public INoteStore NoteStore { get; }
    }
}
