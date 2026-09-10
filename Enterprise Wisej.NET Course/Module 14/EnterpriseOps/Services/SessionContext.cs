using System;
using EnterpriseOps.Data;
using EnterpriseOps.Diagnostics;
using EnterpriseOps.Domain;
using EnterpriseOps.Security;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// Everything that belongs to one browser session: the tenant, the signed-in user, the services built for
    /// them, the shared activity trace — and nothing at all in a static field.
    ///
    /// This is the answer to the first question an architecture review asks about the capstone: <b>where does
    /// state live?</b> One instance is created in <see cref="Program"/>, parked in <c>Application.Session</c>,
    /// and handed to every screen. Two browsers are two of these; they share the process, the catalog and the
    /// permission matrix (immutable reference data) and nothing else.
    ///
    /// It is also the object checklist rule Q2 exists to protect: the moment a tenant id, a selected work
    /// order or a "current user" moves into a static, this class stops being the truth.
    /// </summary>
    public sealed class SessionContext
    {
        private SessionContext(Tenant tenant, UserIdentity user, string docsStartupPath)
        {
            Tenant = tenant;
            User = user;
            SessionStartedUtc = DateTime.UtcNow;
            Services = new ServiceRegistry(this, docsStartupPath);
        }

        public static SessionContext CreateFor(Tenant tenant, UserIdentity user, string docsStartupPath = null)
            => new SessionContext(tenant, user, docsStartupPath);

        /// <summary>What the Designer and a parameterless page constructor get: contoso, signed in as ana.ops.</summary>
        public static SessionContext CreateDefault()
            => CreateFor(InMemoryWorkOrderStore.Tenants[0], KnownUsers.AnaOps);

        public Tenant Tenant { get; }

        public UserIdentity User { get; private set; }

        public DateTime SessionStartedUtc { get; }

        /// <summary>The per-session services. Created once, shared by both screens of the session.</summary>
        public ServiceRegistry Services { get; }

        /// <summary>The command running right now — the correlation id the header and the audit lines show.</summary>
        public CommandContext CurrentCommand { get; private set; }

        /// <summary>
        /// The lab's "review as ben.tech" switch. Real authentication arrives in Module 7; what matters here
        /// is that the identity lives in the session and every service re-reads it from the CommandContext.
        /// </summary>
        public void SignInAs(UserIdentity user)
        {
            User = user ?? throw new ArgumentNullException(nameof(user));
            Services.Trace.Security($"signed in as {User}");
        }

        /// <summary>Starts a new unit of work: same tenant and user, a fresh correlation id.</summary>
        public CommandContext BeginCommand()
        {
            CurrentCommand = new CommandContext(Tenant.Id, User, Guid.NewGuid().ToString("N").Substring(0, 8));
            return CurrentCommand;
        }
    }

    /// <summary>
    /// The composition root of one session: builds every service once and wires them together, so a screen
    /// receives what it needs instead of newing services in a constructor. Deliberately a plain class created
    /// per session — the moment this became a static singleton, two tenants would share a work-order store.
    /// </summary>
    public sealed class ServiceRegistry
    {
        internal ServiceRegistry(SessionContext session, string docsStartupPath)
        {
            Trace = new ActivityTrace();
            Docs = new DocsFolder(docsStartupPath);
            Store = new InMemoryWorkOrderStore();
            Permissions = new PermissionService();
            Audit = new AuditLog();

            Dashboard = new DashboardService(Store, Permissions, Trace);
            WorkOrders = new WorkOrderService(Store, Permissions, Audit, Trace);
            DocumentationIndex = new DocumentationIndexService(Docs, Trace);
            PromptLibrary = new PromptLibraryService(Docs, Trace);
            Review = new GeneratedCodeReviewService(Permissions, Audit, Trace);
            CapstonePackage = new CapstonePackageService(Docs, DocumentationIndex, Permissions, Audit, Trace);
            Health = new DiagnosticsService(Trace, Permissions, Store, Audit, DocumentationIndex, CapstonePackage, Review);

            Trace.Service($"session services ready · tenant {session.Tenant.Id} · docs {(Docs.Found ? Docs.Root : "NOT FOUND")}");
        }

        public ActivityTrace Trace { get; }
        public DocsFolder Docs { get; }
        public InMemoryWorkOrderStore Store { get; }
        public PermissionService Permissions { get; }
        public AuditLog Audit { get; }
        public DashboardService Dashboard { get; }
        public WorkOrderService WorkOrders { get; }
        public DocumentationIndexService DocumentationIndex { get; }
        public PromptLibraryService PromptLibrary { get; }
        public GeneratedCodeReviewService Review { get; }
        public CapstonePackageService CapstonePackage { get; }
        public DiagnosticsService Health { get; }
    }
}
