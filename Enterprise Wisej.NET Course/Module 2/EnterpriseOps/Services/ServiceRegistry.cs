using System;
using EnterpriseOps.Data;
using EnterpriseOps.Security;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// The composition root of one session: every service the two screens share, created once in
    /// <c>Program.Main</c> and stored in <c>Application.Session.Services</c>. Instance fields — never statics:
    /// two users must never share a store, a theme state or a trace (Module 3's static-state audit says why).
    /// Both pages receive the same registry, so navigating MigrationDossierPage ⇄ WorkOrdersPage keeps the
    /// dossier, the steps, the harness outcomes and the trace buffer.
    /// </summary>
    public sealed class ServiceRegistry
    {
        /// <param name="session">The per-session identity (tenant, user, role).</param>
        /// <param name="sessionBag">Reads the identity back from Application.Session — flow 9 proves it is the same instance.</param>
        public ServiceRegistry(SessionContext session, Func<SessionContext> sessionBag)
        {
            Session = session;
            Trace = new ActivityTrace();
            Permissions = new PermissionService(Trace);

            WorkOrderStore = new FakeWorkOrderStore();
            WorkOrders = new WorkOrderService(WorkOrderStore, Trace);

            Theme = new ThemeService(new ThemeStore(), Trace);

            Inventory = new MigrationInventoryStore();
            Assessment = new MigrationAssessmentService(Inventory, Trace);
            Harness = new RegressionHarnessService(WorkOrders, Theme, Permissions, session, sessionBag, Inventory.Flows(), Trace);
            Workflow = new MigrationWorkflow(Inventory, Assessment, Harness, Theme, Permissions, WorkOrders, Trace);
        }

        public SessionContext Session { get; }
        public ActivityTrace Trace { get; }
        public PermissionService Permissions { get; }
        public FakeWorkOrderStore WorkOrderStore { get; }
        public WorkOrderService WorkOrders { get; }
        public ThemeService Theme { get; }
        public MigrationInventoryStore Inventory { get; }
        public MigrationAssessmentService Assessment { get; }
        public RegressionHarnessService Harness { get; }
        public MigrationWorkflow Workflow { get; }
    }
}
