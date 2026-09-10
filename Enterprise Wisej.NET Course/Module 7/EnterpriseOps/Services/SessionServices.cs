using EnterpriseOps.Data;
using EnterpriseOps.Integrations;
using EnterpriseOps.Security;
using EnterpriseOps.Services.Workflow;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// The per-session composition root. One instance is created in WorkQueuePage's constructor and handed
    /// to the EscalationWizard, so the page, the wizard and the workflow share the same stores, the same
    /// audit log, the same compensation queue and the same trace — and two browser sessions share nothing.
    ///
    /// Never a static: Module 3's static-state audit is about exactly this class.
    /// </summary>
    public class SessionServices
    {
        public SessionServices(string tenantId, UserIdentity user)
        {
            Trace = new ActivityTrace();
            Session = new SessionContext(tenantId, user);

            WorkOrders = new InMemoryWorkOrderStore(Trace);
            Escalations = new InMemoryEscalationStore(Trace);
            Staging = new AttachmentStaging(Trace);

            Directory = new FakeApproverDirectory(Trace);
            Notifications = new FakeNotificationGateway(Trace);

            Audit = new AuditLog(Trace);
            Permissions = new PermissionService(Trace);

            Compensation = new CompensationLog(Trace);
            Drafts = new WorkflowStateStore(Trace);
            WorkQueue = new WorkQueueService(WorkOrders, Trace);

            Workflow = new EscalationWorkflow(
                WorkOrders, Escalations, Staging, Directory, Notifications,
                Audit, Permissions, Compensation, Trace);
        }

        public ActivityTrace Trace { get; }
        public SessionContext Session { get; }

        public InMemoryWorkOrderStore WorkOrders { get; }
        public InMemoryEscalationStore Escalations { get; }
        public AttachmentStaging Staging { get; }

        /// <summary>Typed as the fake so the lab buttons can arm the failure paths; the workflow sees the interface.</summary>
        public FakeApproverDirectory Directory { get; }
        public FakeNotificationGateway Notifications { get; }

        public AuditLog Audit { get; }
        public PermissionService Permissions { get; }

        public CompensationLog Compensation { get; }
        public WorkflowStateStore Drafts { get; }
        public WorkQueueService WorkQueue { get; }

        /// <summary>The interface, not the class — the screens may only call what the contract exposes.</summary>
        public IEscalationWorkflow Workflow { get; }
    }
}
