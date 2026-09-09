using System;
using EnterpriseOps.Data;
using EnterpriseOps.Security;
using EnterpriseOps.Services.WorkQueues;
using Wisej.Web;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// Per-session state: the signed-in user, the tenant, the work-queue grid state, the saved views and the
    /// activity trace. Created once in <c>Program.Main</c> and stored in <c>Application.Session</c> — never in
    /// a static field, because a static is shared by every user on the server.
    ///
    /// State ownership (the lab's review question): the <b>page</b> owns nothing that must survive a refresh.
    /// The grid state lives here, so "Simulate refresh" can throw the page away and rebuild it from this object.
    /// </summary>
    public sealed class SessionContext
    {
        public string SessionId { get; }
        public string TenantId { get; private set; }
        public string UserName { get; private set; }
        public UserRole Role { get; private set; }

        public GridState WorkQueueGrid { get; private set; }
        public SavedViewStore SavedViews { get; private set; }
        public ActivityTrace Trace { get; } = new ActivityTrace();

        private SessionContext(string sessionId, string tenantId, string userName)
        {
            SessionId = sessionId;
            TenantId = tenantId;
            UserName = userName;
            Role = PermissionService.RoleOf(userName);
            WorkQueueGrid = GridState.Default(tenantId);
            SavedViews = new SavedViewStore(tenantId, userName);
        }

        /// <summary>The dispatcher the walkthrough signs in as: ana.ops (Manager) in the contoso tenant.</summary>
        public static SessionContext CreateDefault() =>
            new SessionContext(Application.SessionId, "contoso", "ana.ops");

        /// <summary>
        /// Reads the context back from the Wisej.NET session bag (a dynamic per-user dictionary). A page never
        /// receives it through a constructor parameter, so a rebuilt page finds the same object.
        /// </summary>
        public static SessionContext Current
        {
            get
            {
                object stored = Application.Session.EnterpriseOpsContext;
                var context = stored as SessionContext;
                if (context == null)
                {
                    context = CreateDefault();
                    Application.Session.EnterpriseOpsContext = context;
                }
                return context;
            }
        }

        public CommandContext NewCommandContext() =>
            new CommandContext(TenantId, UserName, Role, CommandContext.NewCorrelationId());

        /// <summary>Switching tenant resets the grid state and the saved views: nothing leaks across tenants.</summary>
        public void SwitchTenant(string tenantId)
        {
            if (string.IsNullOrEmpty(tenantId) || tenantId == TenantId)
                return;
            TenantId = tenantId;
            WorkQueueGrid = GridState.Default(tenantId);
            SavedViews = new SavedViewStore(tenantId, UserName);
            Trace.Write($"Session: tenant → {tenantId}; grid state reset, saved views reloaded for {UserName}");
        }

        /// <summary>Switching user changes the role — the projection's permission flags are recomputed on the next page.</summary>
        public void SwitchUser(string userName)
        {
            if (string.IsNullOrEmpty(userName) || userName == UserName)
                return;
            UserName = userName;
            Role = PermissionService.RoleOf(userName);
            Trace.Write($"Security: user → {userName} ({Role}); CanReassign / CanApprove flags recomputed by the next query");
        }
    }
}
